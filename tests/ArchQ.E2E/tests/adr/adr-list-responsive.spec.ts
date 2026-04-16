import { test, expect, devices } from '@playwright/test';

test.describe('ADR List Responsive Layout', () => {

  test('sidebar should be hidden on mobile viewport', async ({ browser }) => {
    // Setup: register, verify, login
    const context = await browser.newContext({ ...devices['iPhone 14'] });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `responsive-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Mobile User', email, password: 'S3cur3P@ss!', organizationName: `MobileCorp${ts}` },
      timeout: 30000,
    });
    const tokenResp = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });

    // Assert: sidebar should NOT be visible on mobile
    const sidebar = page.locator('.app-sidebar');
    await expect(sidebar).toBeHidden();

    await context.close();
  });
});
