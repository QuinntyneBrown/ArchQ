import { test, expect } from '@playwright/test';

test.describe('BUG-032: Button font-weight matches design system', () => {

  test('Login Sign In button should use font-weight 600, not 500', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const btn = page.locator('[data-testid="sign-in-button"]');
    await btn.waitFor({ state: 'visible', timeout: 10000 });

    const fontWeight = await btn.evaluate(el => getComputedStyle(el).fontWeight);

    // Design system Button/Primary uses fontWeight 600 (semibold)
    // Current implementation uses 500 (medium)
    expect(fontWeight).toBe('600');
  });

  test('Register Create Account button should use font-weight 600', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const btn = page.locator('[data-testid="create-account-button"]');
    await btn.waitFor({ state: 'visible', timeout: 10000 });

    const fontWeight = await btn.evaluate(el => getComputedStyle(el).fontWeight);

    expect(fontWeight).toBe('600');
  });
});
