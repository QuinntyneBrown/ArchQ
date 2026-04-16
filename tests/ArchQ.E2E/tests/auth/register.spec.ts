import { test, expect } from '@playwright/test';
import { RegisterPage } from '../../pages/register.page';

test.describe('User Registration', () => {

  test('should register with valid credentials and show success message', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    const uniqueEmail = `test-${Date.now()}@example.com`;
    await registerPage.register(
      'Jane Smith',
      uniqueEmail,
      'S3cur3P@ss!',
      'New Test Org'
    );

    await expect(registerPage.successMessage).toBeVisible();
    await expect(registerPage.successMessage).toContainText('check your email');
  });

  test('should show validation error for missing full name', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.emailInput.fill('test@example.com');
    await registerPage.passwordInput.fill('S3cur3P@ss!');
    await registerPage.organizationNameInput.fill('Test Org');
    await registerPage.createAccountButton.click();

    await expect(registerPage.fullNameError).toBeVisible();
  });

  test('should show validation error for invalid email format', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.fullNameInput.fill('Jane Smith');
    await registerPage.emailInput.fill('not-an-email');
    await registerPage.passwordInput.fill('S3cur3P@ss!');
    await registerPage.organizationNameInput.fill('Test Org');
    await registerPage.createAccountButton.click();

    await expect(registerPage.emailError).toBeVisible();
  });

  test('should show validation error for weak password — too short', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.fullNameInput.fill('Jane Smith');
    await registerPage.emailInput.fill('test@example.com');
    await registerPage.passwordInput.fill('Ab1!');
    await registerPage.organizationNameInput.fill('Test Org');
    await registerPage.createAccountButton.click();

    await expect(registerPage.passwordError).toBeVisible();
    await expect(registerPage.passwordError).toContainText('8 characters');
  });

  test('should show validation error for password without uppercase', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.fullNameInput.fill('Jane Smith');
    await registerPage.emailInput.fill('test@example.com');
    await registerPage.passwordInput.fill('s3cur3p@ss!');
    await registerPage.organizationNameInput.fill('Test Org');
    await registerPage.createAccountButton.click();

    await expect(registerPage.passwordError).toBeVisible();
    await expect(registerPage.passwordError).toContainText('uppercase');
  });

  test('should show validation error for password without special character', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.fullNameInput.fill('Jane Smith');
    await registerPage.emailInput.fill('test@example.com');
    await registerPage.passwordInput.fill('S3cur3Pass1');
    await registerPage.organizationNameInput.fill('Test Org');
    await registerPage.createAccountButton.click();

    await expect(registerPage.passwordError).toBeVisible();
    await expect(registerPage.passwordError).toContainText('special character');
  });

  test('should show validation error for missing organization name', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.fullNameInput.fill('Jane Smith');
    await registerPage.emailInput.fill('test@example.com');
    await registerPage.passwordInput.fill('S3cur3P@ss!');
    await registerPage.createAccountButton.click();

    await expect(registerPage.organizationError).toBeVisible();
  });

  test('should return generic success for duplicate email (no enumeration)', async ({ page, request }) => {
    const registerPage = new RegisterPage(page);

    // Register the first user via API
    const firstEmail = `dup-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: {
        fullName: 'First User',
        email: firstEmail,
        password: 'S3cur3P@ss!',
        organizationName: 'Dup Test Org',
      },
    });

    // Try to register again with same email via UI
    await registerPage.navigate();
    await registerPage.register(
      'Second User',
      firstEmail,
      'An0th3rP@ss!',
      'Another Org'
    );

    // Should still show success message (no enumeration)
    await expect(registerPage.successMessage).toBeVisible();
    await expect(registerPage.successMessage).toContainText('check your email');
  });

  test('should display password strength indicator', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.passwordInput.fill('S3cur3P@ss!');

    await expect(registerPage.passwordStrength).toBeVisible();
  });

  test('should navigate to sign in page', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    await registerPage.signInLink.click();

    await expect(page).toHaveURL(/\/login/);
  });

  test('should show ArchQ branding on register page', async ({ page }) => {
    const registerPage = new RegisterPage(page);
    await registerPage.navigate();

    // Should see the ArchQ logo/title and subtitle
    await expect(page.locator('[data-testid="app-logo"]')).toBeVisible();
    await expect(page.locator('text=Create your account')).toBeVisible();
  });
});
