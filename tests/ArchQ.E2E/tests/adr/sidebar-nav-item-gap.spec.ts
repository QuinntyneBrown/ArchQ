import { test, expect } from '@playwright/test';

test.describe('BUG-039: Sidebar nav item gap should be 10px', () => {
  test.setTimeout(120000);

  test('nav item gap between icon and text should be 10px', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `nav-gap-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Nav Gap', email, password: 'S3cur3P@ss!', organizationName: `NavGap${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    const navItem = page.locator('.nav-item').first();
    await expect(navItem).toBeVisible();

    const gap = await navItem.evaluate(el => getComputedStyle(el).gap);
    expect(gap, `Nav item gap is ${gap}, should be 10px`).toBe('10px');

    await context.close();
  });
});
