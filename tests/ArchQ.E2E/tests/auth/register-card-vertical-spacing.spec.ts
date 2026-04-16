import { test, expect } from '@playwright/test';

test.describe('BUG-076: Register card vertical spacing should be uniform 24px', () => {

  test('gaps between register card sections should all be 24px', async ({ page }) => {
    await page.goto('http://localhost:4200/register', { waitUntil: 'networkidle' });

    const card = page.locator('.register-card');
    await expect(card).toBeVisible();

    const gaps = await page.evaluate(() => {
      const brand = document.querySelector('.brand');
      const subtitle = document.querySelector('h2.subtitle');
      const formGroups = document.querySelectorAll('.form-group');
      const btn = document.querySelector('.btn-primary');
      const signIn = document.querySelector('.sign-in-prompt');
      if (!brand || !subtitle || !formGroups.length || !btn || !signIn) return null;
      return {
        brandToSub: Math.round(subtitle.getBoundingClientRect().top - brand.getBoundingClientRect().bottom),
        lastFormToBtn: Math.round(btn.getBoundingClientRect().top - formGroups[formGroups.length - 1].getBoundingClientRect().bottom),
        btnToSignIn: Math.round(signIn.getBoundingClientRect().top - btn.getBoundingClientRect().bottom),
      };
    });

    expect(gaps).not.toBeNull();
    // Design spec: Register Card gap = 24px between all top-level children
    expect(gaps!.brandToSub, 'Brand → Subtitle gap should be 24px').toBe(24);
    expect(gaps!.lastFormToBtn, 'Last form group → Button gap should be 24px').toBe(24);
    expect(gaps!.btnToSignIn, 'Button → Sign-in link gap should be 24px').toBe(24);
  });
});
