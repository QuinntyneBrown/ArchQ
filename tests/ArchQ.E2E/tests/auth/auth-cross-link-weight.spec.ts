import { test, expect } from '@playwright/test';

test.describe('BUG-034: Auth cross-link font-weight matches design', () => {

  test('Login "Sign up" link should be semibold (600)', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const link = page.locator('[data-testid="sign-up-link"]');
    await link.waitFor({ state: 'visible', timeout: 10000 });

    const fontWeight = await link.evaluate(el => getComputedStyle(el).fontWeight);

    // Design specifies fontWeight 600 for the "Sign up" link
    expect(fontWeight).toBe('600');
  });

  test('Register "Sign in" link should be semibold (600)', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const link = page.locator('[data-testid="sign-in-link"]');
    await link.waitFor({ state: 'visible', timeout: 10000 });

    const fontWeight = await link.evaluate(el => getComputedStyle(el).fontWeight);

    expect(fontWeight).toBe('600');
  });
});
