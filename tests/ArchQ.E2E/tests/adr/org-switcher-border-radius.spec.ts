import { test, expect } from '@playwright/test';

test.describe('BUG-035: Org switcher border-radius should be 4px', () => {
  test.setTimeout(120000);

  test('org switcher button should have 4px border-radius per design', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `org-radius-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Radius Test', email, password: 'S3cur3P@ss!', organizationName: `RadiusCorp${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await r.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    const orgBtn = page.locator('.org-pill').first();
    await expect(orgBtn).toBeVisible();

    const borderRadius = await orgBtn.evaluate(el => getComputedStyle(el).borderRadius);

    // Design: cornerRadius 4 = 4px. Currently 8px (0.5rem).
    expect(
      borderRadius,
      `Org switcher border-radius is ${borderRadius}, should be 4px per design`
    ).toBe('4px');

    await context.close();
  });
});
