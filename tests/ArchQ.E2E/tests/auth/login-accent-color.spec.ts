import { test, expect } from '@playwright/test';

test.describe('BUG-024: Login accent color matches design system', () => {

  test('Sign In button should use accent-primary #0062FF, not info blue #3B82F6', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const signInButton = page.locator('[data-testid="sign-in-button"]');
    await signInButton.waitFor({ state: 'visible', timeout: 10000 });

    const bg = await signInButton.evaluate(
      el => getComputedStyle(el).backgroundColor
    );

    // Design system $accent-primary is #0062FF = rgb(0, 98, 255)
    // Current implementation incorrectly uses #2563EB = rgb(37, 99, 235)
    expect(bg).toBe('rgb(0, 98, 255)');
  });

  test('Forgot password link should use accent-primary #0062FF', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const forgotLink = page.locator('[data-testid="forgot-password"]');
    await forgotLink.waitFor({ state: 'visible', timeout: 10000 });

    const color = await forgotLink.evaluate(
      el => getComputedStyle(el).color
    );

    // Design system $accent-primary is #0062FF = rgb(0, 98, 255)
    expect(color).toBe('rgb(0, 98, 255)');
  });

  test('Sign up link should use accent-primary #0062FF', async ({ page }) => {
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const signUpLink = page.locator('[data-testid="sign-up-link"]');
    await signUpLink.waitFor({ state: 'visible', timeout: 10000 });

    const color = await signUpLink.evaluate(
      el => getComputedStyle(el).color
    );

    // Design system $accent-primary is #0062FF = rgb(0, 98, 255)
    expect(color).toBe('rgb(0, 98, 255)');
  });
});
