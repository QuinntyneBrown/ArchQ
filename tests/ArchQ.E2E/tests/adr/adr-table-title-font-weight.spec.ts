import { test, expect } from '@playwright/test';

test.describe('BUG-083: ADR table title font-weight should be 500', () => {
  test.setTimeout(120000);

  test('title-cell should have font-weight 500 in component stylesheet', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    // Log in to load the ADR list component and its styles
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(1000);

    // Check that the component stylesheet includes font-weight for .title-cell
    const hasFontWeight500 = await page.evaluate(() => {
      for (const sheet of document.styleSheets) {
        try {
          for (const rule of sheet.cssRules) {
            const text = rule.cssText || '';
            if (text.includes('title-cell') && text.includes('font-weight')) {
              // Check if the rule contains font-weight: 500
              if (text.includes('font-weight: 500') || text.includes('font-weight:500')) {
                return true;
              }
            }
          }
        } catch (e) {
          // Cross-origin stylesheets may throw
        }
      }
      return false;
    });

    expect(hasFontWeight500, 'title-cell should have font-weight: 500 in stylesheet').toBe(true);

    await context.close();
  });
});
