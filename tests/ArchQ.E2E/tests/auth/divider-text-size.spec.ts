import { test, expect } from '@playwright/test';

test.describe('BUG-037: Divider text font-size matches design', () => {

  test('Login divider "OR" text should be 11px, not 12px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const dividerText = page.locator('.divider-text');
    await dividerText.waitFor({ state: 'visible', timeout: 10000 });

    const fontSize = await dividerText.evaluate(el => getComputedStyle(el).fontSize);

    // Design specifies 11px for the "OR" divider text
    // Current implementation uses 0.75rem = 12px
    expect(fontSize).toBe('11px');
  });
});
