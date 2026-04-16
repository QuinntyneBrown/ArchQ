import { test, expect } from '@playwright/test';

test.describe('BUG-052: Editor badge should have border', () => {
  test.setTimeout(120000);

  test('editor status badge should have a visible border', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `badge-brd-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'BadgeBrd', email, password: 'S3cur3P@ss!', organizationName: `BB${ts}` },
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

    const badge = page.locator('[data-testid="status-badge"]');
    await expect(badge).toBeVisible();

    const borderWidth = await badge.evaluate(el => getComputedStyle(el).borderWidth);
    expect(
      borderWidth,
      `Editor badge borderWidth is "${borderWidth}" — should be "1px" (has a visible border)`
    ).toBe('1px');

    await context.close();
  });
});
