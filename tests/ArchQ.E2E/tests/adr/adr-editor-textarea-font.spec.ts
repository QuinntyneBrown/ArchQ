import { test, expect } from '@playwright/test';

test.describe('BUG-041: Editor textarea should use Geist Mono', () => {
  test.setTimeout(120000);

  test('markdown editor textarea should have Geist Mono as primary font', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `ed-font-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'EdFont', email, password: 'S3cur3P@ss!', organizationName: `EdFont${ts}` },
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

    const fontFamily = await textarea.evaluate(el => getComputedStyle(el).fontFamily);
    // Geist Mono should be the FIRST font in the stack
    expect(
      fontFamily.toLowerCase().startsWith('"geist mono"') || fontFamily.toLowerCase().startsWith("'geist mono'"),
      `Editor font starts with "${fontFamily.split(',')[0].trim()}" — should start with Geist Mono`
    ).toBe(true);

    await context.close();
  });
});
