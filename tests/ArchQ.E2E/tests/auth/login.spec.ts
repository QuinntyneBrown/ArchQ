import { test, expect } from '@playwright/test';
import { LoginPage } from '../../pages/login.page';
import { TEST_USERS } from '../../fixtures/test-data';

test.describe('User Login', () => {

  test('should login with valid credentials and redirect to dashboard', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    await loginPage.login(TEST_USERS.author.email, TEST_USERS.author.password);

    // Should redirect to ADR list (dashboard)
    await expect(page).toHaveURL(/\/adrs/);
  });

  test('should show error for invalid credentials', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    await loginPage.login('wrong@email.com', 'wrongpassword');

    await expect(loginPage.errorMessage).toBeVisible();
    await expect(loginPage.errorMessage).toContainText('email or password');
  });

  test('should show error for unverified account', async ({ page, request }) => {
    // Register but don't verify email
    const email = `unverified-${Date.now()}@example.com`;
    await request.post('http://localhost:5000/api/auth/register', {
      data: {
        fullName: 'Unverified User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Unverified Org',
      },
    });

    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(email, 'S3cur3P@ss!');

    await expect(loginPage.errorMessage).toBeVisible();
    await expect(loginPage.errorMessage).toContainText('verify your email');
  });

  test('should lock account after 5 failed attempts', async ({ page, request }) => {
    // Register and verify a user first
    const email = `lockout-${Date.now()}@example.com`;
    await request.post('http://localhost:5000/api/auth/register', {
      data: {
        fullName: 'Lockout User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Lockout Org',
      },
    });
    // Verify email via test endpoint
    const tokenResp = await request.get(
      `http://localhost:5000/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5000/api/auth/verify-email', {
      data: { token },
    });

    // Attempt 5 failed logins via API
    for (let i = 0; i < 5; i++) {
      await request.post('http://localhost:5000/api/auth/login', {
        data: { email, password: 'WrongPassword!' },
      });
    }

    // 6th attempt via UI should show lockout
    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(email, 'S3cur3P@ss!');

    await expect(loginPage.errorMessage).toBeVisible();
    await expect(loginPage.errorMessage).toContainText('temporarily locked');
  });

  test('should display login response with user info', async ({ page, request }) => {
    // Register and verify a user
    const email = `logininfo-${Date.now()}@example.com`;
    await request.post('http://localhost:5000/api/auth/register', {
      data: {
        fullName: 'Login Info User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Login Info Org',
      },
    });
    const tokenResp = await request.get(
      `http://localhost:5000/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5000/api/auth/verify-email', {
      data: { token },
    });

    // Login via API and check response shape
    const loginResp = await request.post('http://localhost:5000/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });

    expect(loginResp.status()).toBe(200);
    const body = await loginResp.json();
    expect(body.user).toBeTruthy();
    expect(body.user.email).toBe(email);
    expect(body.user.fullName).toBe('Login Info User');
    expect(body.tenant).toBeTruthy();
    expect(body.tenant.slug).toBeTruthy();
  });

  test('should navigate to registration page', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    await loginPage.signUpLink.click();

    await expect(page).toHaveURL(/\/register/);
  });

  test('should show ArchQ branding on login page', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    await expect(page.locator('[data-testid="app-logo"]')).toBeVisible();
    await expect(page.locator('text=Sign in to your account')).toBeVisible();
  });
});
