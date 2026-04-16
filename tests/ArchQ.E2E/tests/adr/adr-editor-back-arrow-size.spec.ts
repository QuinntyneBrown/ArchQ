import { test, expect } from '@playwright/test';

test.describe('BUG-085: Editor back arrow should be 16px on desktop', () => {
  test.setTimeout(120000);

  test('back arrow SVG should be 16px on desktop viewport', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1280, height: 800 } });
    const page = await context.newPage();

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    await page.goto('http://localhost:4200/adrs/new', { waitUntil: 'networkidle' });
    await page.waitForTimeout(1000);

    const backBtn = page.locator('[data-testid="back-button"]');
    await expect(backBtn).toBeVisible();

    const svgSize = await backBtn.evaluate(el => {
      const svg = el.querySelector('svg');
      if (!svg) return null;
      const rect = svg.getBoundingClientRect();
      return { width: Math.round(rect.width), height: Math.round(rect.height) };
    });

    expect(svgSize).not.toBeNull();
    // Design spec: edBackIcon is 16x16 on desktop
    expect(svgSize!.width, 'Back arrow width should be 16px on desktop').toBe(16);
    expect(svgSize!.height, 'Back arrow height should be 16px on desktop').toBe(16);

    await context.close();
  });
});
