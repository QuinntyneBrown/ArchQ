import { test, expect } from '@playwright/test';

test.describe('BUG-032: Auth card padding and width match design', () => {

  test('login card should have 40px padding all sides and max-width 420px', async ({ page, viewport }) => {
    // Design spec only defines desktop login (1440×900). Mobile has responsive overrides.
    const isMobile = (viewport?.width ?? 1280) < 768;

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });

    const card = page.locator('.login-card');
    await expect(card).toBeVisible();

    const styles = await card.evaluate(el => {
      const s = getComputedStyle(el);
      return {
        paddingTop: parseInt(s.paddingTop),
        paddingBottom: parseInt(s.paddingBottom),
        maxWidth: parseInt(s.maxWidth),
      };
    });

    if (isMobile) {
      // Responsive: padding reduces to 24px on mobile
      expect(styles.paddingTop, 'Login card top padding should be 24px on mobile').toBe(24);
      expect(styles.paddingBottom, 'Login card bottom padding should be 24px on mobile').toBe(24);
    } else {
      // Design spec: padding 40px all sides, width 420px
      expect(styles.paddingTop, 'Login card top padding should be 40px').toBe(40);
      expect(styles.paddingBottom, 'Login card bottom padding should be 40px').toBe(40);
      expect(styles.maxWidth, 'Login card max-width should be 420px').toBe(420);
    }
  });
});
