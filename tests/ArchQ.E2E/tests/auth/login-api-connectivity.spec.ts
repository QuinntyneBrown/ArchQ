import { test, expect } from '@playwright/test';
import { LoginPage } from '../../pages/login.page';

test.describe('Login API Connectivity', () => {

  test('should send login request to the correct API port and not get connection refused', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();

    // Intercept the login API request to verify it targets the correct port
    const requestPromise = page.waitForRequest(
      request => request.url().includes('/api/auth/login'),
      { timeout: 10000 }
    );

    const responsePromise = page.waitForResponse(
      response => response.url().includes('/api/auth/login'),
      { timeout: 10000 }
    );

    await loginPage.login('test@example.com', 'SomePassword123!');

    const request = await requestPromise;
    const requestUrl = new URL(request.url());

    // The API request should target port 5299 (local dev API), not port 5000 (Docker)
    expect(requestUrl.port).toBe('5299');

    // The request should actually reach the server (not connection refused)
    const response = await responsePromise;
    expect(response.status()).toBeGreaterThanOrEqual(200);
    expect(response.status()).toBeLessThan(600);
  });
});
