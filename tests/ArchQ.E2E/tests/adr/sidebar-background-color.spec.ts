import { test, expect } from '@playwright/test';

test.describe('BUG-030: Sidebar should use correct surface color', () => {
  test.setTimeout(120000);

  test('sidebar background should be #1A1D27, not #12141f', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `sidebar-color-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Sidebar Tester', email, password: 'S3cur3P@ss!', organizationName: `SideCorp${ts}` },
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

    const sidebar = page.locator('.app-sidebar');
    await expect(sidebar).toBeVisible();

    const bgColor = await sidebar.evaluate(el => getComputedStyle(el).backgroundColor);

    // Design spec: sidebar background = #1A1D27 = rgb(26, 29, 39)
    // Bug: currently #12141f = rgb(18, 20, 31) — too dark
    expect(
      bgColor,
      `Sidebar background is ${bgColor}, should be rgb(26, 29, 39) (#1A1D27) per design`
    ).toBe('rgb(26, 29, 39)');

    await context.close();
  });
});
