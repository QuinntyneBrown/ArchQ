import { test, expect } from '@playwright/test';

test.describe('BUG-079: Sidebar logo-to-org gap should be 24px', () => {
  test.setTimeout(120000);

  test('gap between sidebar logo and org switcher should be 24px', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(1000);

    const sidebar = page.locator('.app-sidebar');
    await expect(sidebar).toBeVisible();

    const gap = await page.evaluate(() => {
      const header = document.querySelector('.sidebar-header');
      const org = document.querySelector('.sidebar-org');
      if (!header || !org) return null;
      return Math.round(org.getBoundingClientRect().top - header.getBoundingClientRect().bottom);
    });

    expect(gap).not.toBeNull();
    // Design spec: sideLogoRow padding-bottom 16px + sidebar gap 8px = 24px
    expect(gap, 'Logo-to-org gap should be 24px').toBe(24);

    await context.close();
  });
});
