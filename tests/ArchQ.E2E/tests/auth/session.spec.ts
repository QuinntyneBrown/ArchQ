import { test, expect } from '@playwright/test';

test.describe('Session Management', () => {

  test('logout should clear session and redirect to login', async ({ page, request }) => {
    // Register, verify, and login a user
    const email = `session-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: {
        fullName: 'Session User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Session Org',
      },
    });
    const tokenResp = await request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', {
      data: { token },
    });

    // Login via UI
    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Open sidebar on mobile/tablet if hamburger menu is visible
    const hamburger = page.locator('[data-testid="hamburger-menu"]');
    if (await hamburger.isVisible()) {
      await hamburger.click();
    }

    // Click logout
    await page.locator('[data-testid="logout-button"]').click();

    // Should redirect to login
    await expect(page).toHaveURL(/\/login/);
  });

  test('API should return 401 for invalid refresh token', async ({ request }) => {
    const resp = await request.post('http://localhost:5299/api/auth/refresh', {
      headers: {
        Cookie: 'archq_refresh=invalid-token',
      },
    });
    expect(resp.status()).toBe(401);
  });

  test('API logout should return 200 and clear cookies', async ({ request }) => {
    // Register, verify, login
    const email = `logout-api-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: {
        fullName: 'Logout User',
        email,
        password: 'S3cur3P@ss!',
        organizationName: 'Logout Org',
      },
    });
    const tokenResp = await request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', {
      data: { token },
    });

    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    expect(loginResp.status()).toBe(200);

    // Logout
    const logoutResp = await request.post('http://localhost:5299/api/auth/logout');
    expect(logoutResp.status()).toBe(200);
    const body = await logoutResp.json();
    expect(body.message).toContain('Signed out');
  });
});
