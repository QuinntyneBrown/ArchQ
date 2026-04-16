import { test, expect } from '@playwright/test';

test.describe('BUG-026: Auth subtitle font size matches design system', () => {

  test('Login subtitle should use $font-base 16px, not 14px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const subtitle = page.locator('.subtitle');
    await subtitle.waitFor({ state: 'visible', timeout: 10000 });

    const fontSize = await subtitle.evaluate(el => getComputedStyle(el).fontSize);

    // Design system $font-base is 16px
    // Current implementation incorrectly uses 0.875rem = 14px
    expect(fontSize).toBe('16px');
  });

  test('Register subtitle should use $font-base 16px, not 14px', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const subtitle = page.locator('.subtitle');
    await subtitle.waitFor({ state: 'visible', timeout: 10000 });

    const fontSize = await subtitle.evaluate(el => getComputedStyle(el).fontSize);

    // Design system $font-base is 16px
    expect(fontSize).toBe('16px');
  });
});
