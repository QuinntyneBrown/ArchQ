import { test, expect } from '@playwright/test';

test.describe('BUG-047: Table td and th borders should match', () => {
  test.setTimeout(120000);

  test('td border-bottom should use same color as th (#2E3142)', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `td-brd-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'TdBorder', email, password: 'S3cur3P@ss!', organizationName: `TB${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    // Create ADR to get table rows
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Border Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    const td = page.locator('.adr-table td').first();
    await expect(td).toBeVisible();

    const borderColor = await td.evaluate(el => {
      const border = getComputedStyle(el).borderBottomColor;
      return border;
    });

    // Should be #1F2231 = rgb(31, 34, 49) — design uses subtle row dividers
    // distinct from the stronger #2E3142 card/container border
    expect(
      borderColor,
      `td border color is ${borderColor}, should be rgb(31, 34, 49) per design`
    ).toBe('rgb(31, 34, 49)');

    await context.close();
  });
});
