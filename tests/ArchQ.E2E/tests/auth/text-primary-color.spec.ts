import { test, expect } from '@playwright/test';

test.describe('BUG-040: Body text uses $text-primary #F0F1F5', () => {

  test('Login logo text color should be #F0F1F5, not pure white #FFFFFF', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const logoText = page.locator('.logo-text');
    await logoText.waitFor({ state: 'visible', timeout: 10000 });

    const color = await logoText.evaluate(el => getComputedStyle(el).color);

    // Design system $text-primary is #F0F1F5 = rgb(240, 241, 245)
    // Current implementation uses #FFFFFF = rgb(255, 255, 255)
    expect(color).toBe('rgb(240, 241, 245)');
  });
});
