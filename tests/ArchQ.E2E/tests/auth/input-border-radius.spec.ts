import { test, expect } from '@playwright/test';

test.describe('BUG-031: Input and button border-radius matches $radius-sm', () => {

  test('Login input border-radius should be 4px ($radius-sm), not 6px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const input = page.locator('[data-testid="email-input"]');
    await input.waitFor({ state: 'visible', timeout: 10000 });

    const radius = await input.evaluate(el => getComputedStyle(el).borderRadius);

    // Design system $radius-sm is 4px
    // Current implementation uses 0.375rem = 6px
    expect(radius).toBe('4px');
  });

  test('Login button border-radius should be 4px ($radius-sm), not 6px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const btn = page.locator('[data-testid="sign-in-button"]');
    await btn.waitFor({ state: 'visible', timeout: 10000 });

    const radius = await btn.evaluate(el => getComputedStyle(el).borderRadius);

    expect(radius).toBe('4px');
  });
});
