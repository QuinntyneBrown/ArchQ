import { test, expect } from '@playwright/test';

test.describe('BUG-038: Forgot password link font-size should be 14px', () => {

  test('forgot password link should have 14px font-size per design', async ({ page }) => {
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });

    const link = page.locator('.forgot-password');
    await expect(link).toBeVisible();

    const fontSize = await link.evaluate(el => getComputedStyle(el).fontSize);
    expect(fontSize, `Forgot password font-size is ${fontSize}, should be 14px`).toBe('14px');
  });
});
