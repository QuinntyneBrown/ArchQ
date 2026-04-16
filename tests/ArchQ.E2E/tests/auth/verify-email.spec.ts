import { test, expect } from '@playwright/test';
import { VerifyEmailPage } from '../../pages/verify-email.page';

test.describe('Email Verification', () => {

  test('should show error for invalid verification token', async ({ page }) => {
    const verifyPage = new VerifyEmailPage(page);

    await page.goto('/verify-email?token=invalid-token-123');

    await expect(verifyPage.errorMessage).toBeVisible();
    await expect(verifyPage.errorMessage).toContainText('invalid or has expired');
  });

  test('should show error for expired verification token', async ({ page }) => {
    const verifyPage = new VerifyEmailPage(page);

    await page.goto('/verify-email?token=expired-token-456');

    await expect(verifyPage.errorMessage).toBeVisible();
    await expect(verifyPage.errorMessage).toContainText('invalid or has expired');
  });

  test('should show success and sign-in link for valid token', async ({ page, request }) => {
    // Register a user first to get a real token
    const email = `verify-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: {
        fullName: 'Verify User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Verify Org',
      },
    });

    // In a real test, we'd extract the token from the email.
    // For E2E, we use a test helper API to get the verification token.
    const tokenResp = await request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await tokenResp.json();

    const verifyPage = new VerifyEmailPage(page);
    await page.goto(`/verify-email?token=${token}`);

    await expect(verifyPage.statusMessage).toBeVisible();
    await expect(verifyPage.statusMessage).toContainText('verified');
    await expect(verifyPage.signInLink).toBeVisible();
  });
});
