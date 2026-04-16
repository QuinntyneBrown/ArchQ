import { test, expect } from '@playwright/test';
import { RegisterPage } from '../../pages/register.page';

test.describe('Register Page — Mobile Responsive', () => {
  test.use({ viewport: { width: 375, height: 812 } });

  test('register card should be full-width on mobile', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    // Form card should be visible and take full width
    await expect(registerPage.fullNameInput).toBeVisible();
    await expect(registerPage.createAccountButton).toBeVisible();

    // Button should be full width
    const buttonBox = await registerPage.createAccountButton.boundingBox();
    expect(buttonBox!.width).toBeGreaterThan(280);
  });

  test('all form fields should be visible on mobile', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await expect(registerPage.fullNameInput).toBeVisible();
    await expect(registerPage.emailInput).toBeVisible();
    await expect(registerPage.passwordInput).toBeVisible();
    await expect(registerPage.organizationNameInput).toBeVisible();
    await expect(registerPage.createAccountButton).toBeVisible();
    await expect(registerPage.signInLink).toBeVisible();
  });
});
