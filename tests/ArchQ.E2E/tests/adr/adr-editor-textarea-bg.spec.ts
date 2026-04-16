import { test, expect } from '@playwright/test';

test.describe('BUG-050: Editor textarea bg should use surface color', () => {
  test.setTimeout(120000);

  test('textarea background should be #1A1D27, not #1e2030', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `ta-bg-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'TaBg', email, password: 'S3cur3P@ss!', organizationName: `TAB${ts}` },
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

    const textarea = page.locator('textarea').first();
    await expect(textarea).toBeVisible();

    const bg = await textarea.evaluate(el => getComputedStyle(el).backgroundColor);
    // Design: #1A1D27 = rgb(26, 29, 39)
    expect(bg, `Textarea bg is ${bg}, should be rgb(26, 29, 39)`).toBe('rgb(26, 29, 39)');

    await context.close();
  });
});
