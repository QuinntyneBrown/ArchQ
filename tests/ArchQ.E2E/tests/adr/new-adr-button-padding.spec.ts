import { test, expect } from '@playwright/test';

test.describe('BUG-046: New ADR button padding should be 10px 20px', () => {
  test.setTimeout(120000);

  test('new ADR button should have 10px 20px padding per design', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `btn-pad-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'BtnPad', email, password: 'S3cur3P@ss!', organizationName: `BP${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    const btn = page.locator('[data-testid="new-adr-button"]');
    await expect(btn).toBeVisible();

    const padding = await btn.evaluate(el => getComputedStyle(el).padding);
    // Design: padding [10, 20] = 10px top/bottom, 20px left/right
    expect(padding, `Button padding is ${padding}, should be 10px 20px`).toBe('10px 20px');

    await context.close();
  });
});
