import { test, expect } from '@playwright/test';

test.describe('BUG-037: Auth logo icon should be 28px', () => {

  test('login page logo icon should be 28px per design', async ({ page }) => {
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });

    const icon = page.locator('.logo-icon').first();
    await expect(icon).toBeVisible();

    const width = await icon.evaluate(el => getComputedStyle(el).width);

    // Design: icon 28px. Currently 32px (2rem).
    expect(width, `Logo icon width is ${width}, should be 28px`).toBe('28px');
  });
});
