import { test, expect } from '@playwright/test';

test.describe('BUG-078: Editor split pane should have border-radius, border, and clip', () => {
  test.setTimeout(120000);

  test('split pane should have 8px border-radius, border, and overflow hidden', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    // Use an existing verified test user (from N1QL: new-test-org scope)
    const email = 'test-1776344632458@example.com';

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    // Navigate to editor
    await page.goto('http://localhost:4200/adrs/new', { waitUntil: 'networkidle' });
    await page.waitForTimeout(1000);

    const splitPane = page.locator('.split-pane');
    await expect(splitPane).toBeVisible();

    const styles = await splitPane.evaluate(el => {
      const s = getComputedStyle(el);
      return {
        borderRadius: parseInt(s.borderRadius),
        border: s.borderStyle,
        borderColor: s.borderColor,
        overflow: s.overflow,
      };
    });

    // Design spec: Split Pane Editor has cornerRadius $radius-md (8px), stroke border-default, clip
    expect(styles.borderRadius, 'Split pane border-radius should be 8px').toBe(8);
    expect(styles.border, 'Split pane should have a solid border').toBe('solid');
    expect(styles.overflow, 'Split pane should clip content (overflow hidden)').toBe('hidden');

    await context.close();
  });
});
