import { test, expect } from '@playwright/test';

test.describe('BUG-084: Mobile editor title font-size should be 16px', () => {
  test.setTimeout(120000);

  test('top-bar-title should be 16px at mobile viewport', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 375, height: 812 } });
    const page = await context.newPage();

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    await page.goto('http://localhost:4200/adrs/new', { waitUntil: 'networkidle' });
    await page.waitForTimeout(1000);

    const titleEl = page.locator('.top-bar-title');
    await expect(titleEl).toBeVisible();

    const fontSize = await titleEl.evaluate(el => parseInt(getComputedStyle(el).fontSize));

    // Design spec: mobile editor title uses $font-base (16px)
    expect(fontSize, 'Mobile editor title font-size should be 16px').toBe(16);

    await context.close();
  });
});
