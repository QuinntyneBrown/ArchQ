import { test, expect } from '@playwright/test';

test.describe('Route Guard', () => {

  test('should redirect unauthenticated users from /adrs to /login', async ({ page }) => {
    // Navigate directly to /adrs without logging in
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);

    // Should be redirected to login
    expect(page.url()).toContain('/login');
  });
});
