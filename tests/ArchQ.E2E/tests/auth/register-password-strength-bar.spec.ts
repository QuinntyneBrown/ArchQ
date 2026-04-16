import { test, expect } from '@playwright/test';

test.describe('BUG-027: Password strength bar hidden when empty', () => {

  test('password strength bar should not be visible when password field is empty', async ({ page }) => {
    await page.goto('http://localhost:4200/register', { waitUntil: 'networkidle' });

    // The password field should be empty by default
    const passwordInput = page.locator('[data-testid="password-input"]');
    await expect(passwordInput).toHaveValue('');

    // The password strength bar container should either be hidden or have
    // transparent/no visible background when the password is empty.
    // Per design spec (ui-design.pen, Register Card node Liwyv), there
    // should be no visible element between Password and Organization Name.
    const strengthBar = page.locator('[data-testid="password-strength"]');
    const isVisible = await strengthBar.evaluate(el => {
      const style = getComputedStyle(el);
      const bgColor = style.backgroundColor;
      const height = parseFloat(style.height);
      // Bar is "visually present" if it has both a non-zero height
      // and a non-transparent background color
      const isTransparent = bgColor === 'transparent' || bgColor === 'rgba(0, 0, 0, 0)';
      return height > 0 && !isTransparent;
    });

    expect(
      isVisible,
      'Password strength bar should not be visually rendered when password is empty'
    ).toBe(false);
  });
});
