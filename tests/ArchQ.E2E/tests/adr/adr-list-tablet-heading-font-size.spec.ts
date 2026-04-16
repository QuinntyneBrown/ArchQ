import { test, expect } from '@playwright/test';

test.describe('BUG-086: Tablet ADR list heading should be 20px', () => {
  test.setTimeout(120000);

  test('heading font-size should be 20px at 768px tablet viewport', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 768, height: 1024 } });
    const page = await context.newPage();

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(1000);

    const heading = page.locator('.heading');
    await expect(heading).toBeVisible();

    const fontSize = await heading.evaluate(el => parseInt(getComputedStyle(el).fontSize));

    // Design spec: tablet page title is 20px, not desktop's 24px
    expect(fontSize, 'Tablet heading font-size should be 20px').toBe(20);

    await context.close();
  });
});
