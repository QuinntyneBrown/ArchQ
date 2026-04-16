import { test, expect } from '@playwright/test';

test.describe('BUG-028: Input field colors match design system', () => {

  test('Login email input background should use $bg-elevated #242736', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const input = page.locator('[data-testid="email-input"]');
    await input.waitFor({ state: 'visible', timeout: 10000 });

    const bg = await input.evaluate(el => getComputedStyle(el).backgroundColor);

    // Design system $bg-elevated is #242736 = rgb(36, 39, 54)
    // Current implementation incorrectly uses #252836 = rgb(37, 40, 54)
    expect(bg).toBe('rgb(36, 39, 54)');
  });

  test('Login email input border should use $border-default #2E3142', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const input = page.locator('[data-testid="email-input"]');
    await input.waitFor({ state: 'visible', timeout: 10000 });

    const border = await input.evaluate(el => getComputedStyle(el).borderColor);

    // Design system $border-default is #2E3142 = rgb(46, 49, 66)
    // Current implementation incorrectly uses #3A3F54 = rgb(58, 63, 84)
    expect(border).toBe('rgb(46, 49, 66)');
  });
});
