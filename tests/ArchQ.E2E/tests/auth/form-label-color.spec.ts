import { test, expect } from '@playwright/test';

test.describe('BUG-029: Form label color matches design system', () => {

  test('Login form labels should use $text-secondary #9CA3AF, not #D1D5DB', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const label = page.locator('label').first();
    await label.waitFor({ state: 'visible', timeout: 10000 });

    const color = await label.evaluate(el => getComputedStyle(el).color);

    // Design system $text-secondary is #9CA3AF = rgb(156, 163, 175)
    // Current implementation uses #D1D5DB = rgb(209, 213, 219)
    expect(color).toBe('rgb(156, 163, 175)');
  });

  test('Register form labels should use $text-secondary #9CA3AF', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const label = page.locator('label').first();
    await label.waitFor({ state: 'visible', timeout: 10000 });

    const color = await label.evaluate(el => getComputedStyle(el).color);

    expect(color).toBe('rgb(156, 163, 175)');
  });
});
