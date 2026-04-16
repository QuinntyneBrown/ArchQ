import { test, expect } from '@playwright/test';

test.describe('BUG-087: Unknown routes should redirect to /adrs', () => {
  test.setTimeout(120000);

  test('navigating to unknown route should redirect authenticated user to /adrs', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    // Log in first
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(1000);

    // Navigate to a non-existent route
    await page.goto('http://localhost:4200/this-page-does-not-exist', { waitUntil: 'networkidle' });
    await page.waitForTimeout(1000);

    // Should redirect to /adrs, not stay on the unknown route
    expect(page.url()).toContain('/adrs');

    await context.close();
  });
});
