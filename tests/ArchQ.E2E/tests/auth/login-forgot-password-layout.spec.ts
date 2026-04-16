import { test, expect } from '@playwright/test';
import { LoginPage } from '../../pages/login.page';

test.describe('Login Page - Forgot Password Layout', () => {

  test('should display forgot password link below the sign in button, not inline with password label', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    const signInButton = loginPage.signInButton;
    const forgotPasswordLink = loginPage.forgotPasswordLink;

    // Both elements should be visible
    await expect(signInButton).toBeVisible();
    await expect(forgotPasswordLink).toBeVisible();

    // Get bounding boxes
    const signInBox = await signInButton.boundingBox();
    const forgotBox = await forgotPasswordLink.boundingBox();

    expect(signInBox).not.toBeNull();
    expect(forgotBox).not.toBeNull();

    // Per design spec: "Forgot password?" should be BELOW the Sign In button
    // forgotBox.y should be greater than signInBox.y + signInBox.height
    const signInBottom = signInBox!.y + signInBox!.height;
    expect(forgotBox!.y).toBeGreaterThan(signInBottom);
  });
});
