import { test, expect } from '@playwright/test';

test.describe('BUG-041: Login form group spacing matches design', () => {

  test('Login form group margin-bottom should be 20px, not 16px', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const formGroup = page.locator('.form-group').first();
    await formGroup.waitFor({ state: 'visible', timeout: 10000 });

    const marginBottom = await formGroup.evaluate(el => getComputedStyle(el).marginBottom);

    // Design specifies gap: 20 between login form fields
    // Current implementation uses margin-bottom: 1rem = 16px
    expect(marginBottom).toBe('20px');
  });
});
