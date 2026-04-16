import { test, expect } from '@playwright/test';

test.describe('BUG-029: Buttons should use Inter font', () => {

  test('Sign In button should use Inter font, not browser default Arial', async ({ page }) => {
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });

    const signInButton = page.locator('[data-testid="sign-in-button"]');
    await expect(signInButton).toBeVisible();

    const fontFamily = await signInButton.evaluate(el => getComputedStyle(el).fontFamily);

    // The font-family should start with Inter (or contain 'Inter'),
    // not be plain 'Arial' which is the browser default for buttons.
    expect(
      fontFamily.toLowerCase(),
      `Button font-family is "${fontFamily}" — expected Inter, not browser default Arial`
    ).toContain('inter');
  });
});
