import { test, expect } from '@playwright/test';

test.describe('Session Restore on Page Refresh', () => {

  test('should maintain session after page refresh', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `session-${ts}@example.com`;
    const orgName = `SessionCorp${ts}`;

    // Register, verify
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Session User', email, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 30000,
    });
    const tokenResp = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login via UI
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });
    await page.waitForTimeout(1000);

    // Verify session is active
    const subtitleBefore = page.locator('[data-testid="page-subtitle"]');
    await expect(subtitleBefore).toContainText(orgName);

    // Simulate page refresh
    await page.reload({ waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);

    // After refresh, session should still be active
    // Should still be on /adrs (not redirected to /login)
    expect(page.url()).toContain('/adrs');

    // Subtitle should still show org name
    const subtitleAfter = page.locator('[data-testid="page-subtitle"]');
    await expect(subtitleAfter).toContainText(orgName);

    await context.close();
  });
});
