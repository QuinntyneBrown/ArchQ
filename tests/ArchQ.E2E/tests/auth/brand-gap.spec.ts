import { test, expect } from '@playwright/test';

test.describe('BUG-039: Brand logo gap matches design', () => {

  test('Login brand row gap should be 12px, not 8px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const brand = page.locator('.brand');
    await brand.waitFor({ state: 'visible', timeout: 10000 });

    const gap = await brand.evaluate(el => getComputedStyle(el).gap);

    // Design specifies gap: 12 between logo icon and text
    // Current implementation uses 0.5rem = 8px
    expect(gap).toBe('12px');
  });
});
