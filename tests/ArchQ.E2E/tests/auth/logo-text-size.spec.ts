import { test, expect } from '@playwright/test';

test.describe('BUG-038: Logo text font-size matches design', () => {

  test('Login logo "ArchQ" text should be 28px, not 24px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const logoText = page.locator('.logo-text');
    await logoText.waitFor({ state: 'visible', timeout: 10000 });

    const fontSize = await logoText.evaluate(el => getComputedStyle(el).fontSize);

    // Design specifies 28px for logo text
    // Current implementation uses 1.5rem = 24px
    expect(fontSize).toBe('28px');
  });

  test('Register logo "ArchQ" text should be 28px', async ({ page }) => {
    await page.goto('/register');
    await page.waitForLoadState('networkidle');

    const logoText = page.locator('.logo-text');
    await logoText.waitFor({ state: 'visible', timeout: 10000 });

    const fontSize = await logoText.evaluate(el => getComputedStyle(el).fontSize);

    expect(fontSize).toBe('28px');
  });
});
