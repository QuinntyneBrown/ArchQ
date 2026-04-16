import { test, expect } from '@playwright/test';

test.describe('BUG-026: Draft badge uses correct amber color', () => {
  test.setTimeout(120000);

  test('Draft status badge should be amber (#F59E0B), not green', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `badge-color-${ts}@example.com`;

    // Register, verify, and log in
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Badge Tester', email, password: 'S3cur3P@ss!', organizationName: `BadgeCorp${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await r.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    // Create a Draft ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Badge Color Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });

    // Go back to ADR list
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // Find the Draft badge and check its color
    const badge = page.locator('.status-badge').first();
    await expect(badge).toBeVisible();

    const color = await badge.evaluate(el => getComputedStyle(el).color);

    // Design spec: Draft = #F59E0B = rgb(245, 158, 11) (amber)
    // Bug: currently rgb(34, 197, 94) (green, same as Approved)
    expect(
      color,
      `Draft badge color should be amber rgb(245, 158, 11) but got ${color}`
    ).toBe('rgb(245, 158, 11)');

    await context.close();
  });
});
