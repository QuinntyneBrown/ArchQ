import { test, expect } from '@playwright/test';

test.describe('BUG-028: Editor header should not wrap on mobile', () => {
  test.setTimeout(120000);

  test('top-bar-title should be truncated to single line at 375px', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 375, height: 812 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `mobile-editor-${ts}@example.com`;

    // Register, verify, and log in
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Mobile Editor', email, password: 'S3cur3P@ss!', organizationName: `MobEdit${ts}` },
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

    // Create an ADR with a long title (typical real-world title)
    await page.click('[data-testid="new-adr-button-mobile"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill(
      '[data-testid="adr-title-input"]',
      'Use Event-Driven Architecture for Order Processing Pipeline'
    );
    await page.locator('textarea').first().fill('## Context\nTest content.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.waitForTimeout(1000);

    // The top-bar-title should not wrap to multiple lines.
    // We check that its rendered height is no more than ~24px (one line of 1rem/16px text
    // plus padding). If it wraps, the height will be 48px+ (2+ lines).
    const titleEl = page.locator('.top-bar-title');
    await expect(titleEl).toBeVisible();
    const titleHeight = await titleEl.evaluate(el => el.getBoundingClientRect().height);

    expect(
      titleHeight,
      `Title element is ${titleHeight}px tall, indicating text wraps to multiple lines. Should be ≤24px (single line).`
    ).toBeLessThanOrEqual(24);

    await context.close();
  });
});
