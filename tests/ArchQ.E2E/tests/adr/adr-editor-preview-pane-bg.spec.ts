import { test, expect } from '@playwright/test';

test.describe('BUG-048: Preview pane should use dark primary background', () => {
  test.setTimeout(120000);

  test('preview pane background should be #0F1117 (dark), not #242736', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `prev-bg-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'PrevBg', email, password: 'S3cur3P@ss!', organizationName: `PB${ts}` },
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

    const previewPane = page.locator('.preview-pane').first();
    await expect(previewPane).toBeVisible();

    const bg = await previewPane.evaluate(el => getComputedStyle(el).backgroundColor);
    // Design: #0F1117 = rgb(15, 17, 23) — dark primary background
    expect(bg, `Preview pane bg is ${bg}, should be rgb(15, 17, 23)`).toBe('rgb(15, 17, 23)');

    await context.close();
  });
});
