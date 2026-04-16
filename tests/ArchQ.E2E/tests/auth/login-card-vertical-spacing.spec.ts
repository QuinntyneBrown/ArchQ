import { test, expect } from '@playwright/test';

test.describe('BUG-077: Login card vertical spacing should be uniform 32px', () => {

  test('gaps between login card sections should all be 32px', async ({ page }) => {
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });

    const card = page.locator('.login-card');
    await expect(card).toBeVisible();

    const gaps = await page.evaluate(() => {
      const brand = document.querySelector('.brand');
      const subtitle = document.querySelector('h2.subtitle');
      const form = document.querySelector('form');
      const divider = document.querySelector('.divider');
      const signUp = document.querySelector('.sign-up-prompt');
      if (!brand || !subtitle || !form || !divider || !signUp) return null;
      return {
        brandToSub: Math.round(subtitle.getBoundingClientRect().top - brand.getBoundingClientRect().bottom),
        subToForm: Math.round(form.getBoundingClientRect().top - subtitle.getBoundingClientRect().bottom),
        formToDivider: Math.round(divider.getBoundingClientRect().top - form.getBoundingClientRect().bottom),
        dividerToSignUp: Math.round(signUp.getBoundingClientRect().top - divider.getBoundingClientRect().bottom),
      };
    });

    expect(gaps).not.toBeNull();
    // Design spec: Login Card gap = 32px between all top-level children
    expect(gaps!.brandToSub, 'Brand → Subtitle gap should be 32px').toBe(32);
    expect(gaps!.subToForm, 'Subtitle → Form gap should be 32px').toBe(32);
    expect(gaps!.formToDivider, 'Form → Divider gap should be 32px').toBe(32);
    expect(gaps!.dividerToSignUp, 'Divider → Sign-up gap should be 32px').toBe(32);
  });
});
