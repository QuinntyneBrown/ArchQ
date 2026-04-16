import { test, expect } from '@playwright/test';

test.describe('BUG-043: ADR list card border-radius should be 8px', () => {
  test.setTimeout(120000);

  test('adr-list-card border-radius should be 8px (content card radius)', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `card-rad-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'CardRad', email, password: 'S3cur3P@ss!', organizationName: `CRad${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    const card = page.locator('.adr-list-card').first();
    await expect(card).toBeVisible();

    const radius = await card.evaluate(el => getComputedStyle(el).borderRadius);
    expect(radius, `Card border-radius is ${radius}, should be 8px per design`).toBe('8px');

    await context.close();
  });
});
