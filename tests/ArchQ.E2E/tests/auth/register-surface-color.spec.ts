import { test, expect } from '@playwright/test';

test.describe('BUG-025: Register card surface and border colors match design system', () => {

  test('Register card background should use $bg-surface #1A1D27', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const card = page.locator('.register-card');
    await card.waitFor({ state: 'visible', timeout: 10000 });

    const bg = await card.evaluate(el => getComputedStyle(el).backgroundColor);

    // Design system $bg-surface is #1A1D27 = rgb(26, 29, 39)
    // Current implementation incorrectly uses #1A1D2E = rgb(26, 29, 46)
    expect(bg).toBe('rgb(26, 29, 39)');
  });

  test('Register card border should use $border-default #2E3142', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const card = page.locator('.register-card');
    await card.waitFor({ state: 'visible', timeout: 10000 });

    const border = await card.evaluate(el => getComputedStyle(el).borderColor);

    // Design system $border-default is #2E3142 = rgb(46, 49, 66)
    // Current implementation incorrectly uses #2A2D3E = rgb(42, 45, 62)
    expect(border).toBe('rgb(46, 49, 66)');
  });

  test('Login card background should also use $bg-surface #1A1D27', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const card = page.locator('.login-card');
    await card.waitFor({ state: 'visible', timeout: 10000 });

    const bg = await card.evaluate(el => getComputedStyle(el).backgroundColor);

    // Design system $bg-surface is #1A1D27 = rgb(26, 29, 39)
    expect(bg).toBe('rgb(26, 29, 39)');
  });
});
