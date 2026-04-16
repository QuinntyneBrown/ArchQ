import { test, expect } from '@playwright/test';

test.describe('BUG-036: Editor badge color should reflect actual status', () => {
  test.setTimeout(120000);

  test('editor badge should use status-specific CSS class, not hardcoded color', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `badge-cls-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Badge Cls', email, password: 'S3cur3P@ss!', organizationName: `BCls${ts}` },
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

    // Create a draft ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Badge Class Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.waitForTimeout(1000);

    // The badge should have a dynamic status class (e.g., "status-draft")
    // not just the base "status-badge" with hardcoded colors
    const badge = page.locator('[data-testid="status-badge"]');
    await expect(badge).toBeVisible();

    const hasStatusClass = await badge.evaluate(el =>
      Array.from(el.classList).some(c => c.startsWith('status-') && c !== 'status-badge')
    );

    expect(
      hasStatusClass,
      'Editor badge should have a dynamic status-* class (e.g., status-draft, status-approved) for color'
    ).toBe(true);

    await context.close();
  });
});
