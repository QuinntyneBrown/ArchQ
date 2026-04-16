import { test, expect } from '@playwright/test';

test.describe('BUG-049: Editor toolbar should use elevated background', () => {
  test.setTimeout(120000);

  test('toolbar background should be #242736 (elevated), not #1A1D27', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `tb-bg-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'TbBg', email, password: 'S3cur3P@ss!', organizationName: `TBg${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });

    const toolbar = page.locator('.toolbar').first();
    await expect(toolbar).toBeVisible();

    const bg = await toolbar.evaluate(el => getComputedStyle(el).backgroundColor);
    // Design: #242736 = rgb(36, 39, 54)
    expect(bg, `Toolbar bg is ${bg}, should be rgb(36, 39, 54)`).toBe('rgb(36, 39, 54)');

    await context.close();
  });
});
