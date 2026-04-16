import { test, expect } from '@playwright/test';

test.describe('BUG-025: ADR table columns not truncated at tablet width', () => {
  test.setTimeout(120000);

  test('table should fit within its container at 768px without horizontal overflow', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 768, height: 1024 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `tablet-trunc-${ts}@example.com`;

    // Register, verify, and log in
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Alexandra Thompson', email, password: 'S3cur3P@ss!', organizationName: `TabletTestCorp${ts}` },
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

    // Create an ADR with a realistic long title (as shown in design mockups)
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill(
      '[data-testid="adr-title-input"]',
      'Use Event-Driven Architecture for Order Processing'
    );
    await page.locator('textarea').first().fill('## Context\nOur order processing pipeline needs to handle high throughput.');

    // Save the draft and wait for the URL to change (indicates save succeeded)
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.waitForTimeout(1000);

    // Navigate back to the ADR list
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // Table should be visible (not mobile card layout)
    const table = page.locator('[data-testid="adr-table"]');
    await expect(table).toBeVisible({ timeout: 10000 });

    // The table-wrapper should NOT overflow horizontally at 768px.
    // When the table is wider than the wrapper, scrollWidth > clientWidth,
    // causing columns (especially "Modified") to be clipped or require scrolling.
    const tableWrapper = page.locator('.table-wrapper');
    const overflow = await tableWrapper.evaluate(el => ({
      scrollWidth: el.scrollWidth,
      clientWidth: el.clientWidth,
    }));
    expect(
      overflow.scrollWidth,
      `Table overflows its container at 768px: scrollWidth (${overflow.scrollWidth}) > clientWidth (${overflow.clientWidth}). The "Modified" column and date values get clipped.`
    ).toBeLessThanOrEqual(overflow.clientWidth);

    await context.close();
  });
});
