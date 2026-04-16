import { test, expect } from '@playwright/test';

test.describe('ADR List Subtitle', () => {

  test('should display organization name in subtitle below heading', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `subtitle-${ts}@example.com`;
    const orgName = `SubtitleCorp${ts}`;

    // Register, verify, login
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Sub User', email, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 30000,
    });
    const tokenResp = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });
    await page.waitForTimeout(1000);

    // Assert subtitle contains org name
    const subtitle = page.locator('[data-testid="page-subtitle"]');
    await expect(subtitle).toBeVisible();
    await expect(subtitle).toContainText('Manage and track');
    await expect(subtitle).toContainText(orgName);

    await context.close();
  });
});
