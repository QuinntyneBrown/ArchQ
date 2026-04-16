import { test, expect } from '@playwright/test';

test.describe('BUG-051: Mobile card internal spacing should be uniform 10px', () => {
  test.setTimeout(120000);

  test('card-top-row margin-bottom should be 10px', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 375, height: 812 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `card-sp-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'CardSp', email, password: 'S3cur3P@ss!', organizationName: `CS${ts}` },
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

    await page.click('[data-testid="new-adr-button-mobile"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Spacing Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    const topRow = page.locator('.card-top-row').first();
    await expect(topRow).toBeVisible();

    const marginBottom = await topRow.evaluate(el => getComputedStyle(el).marginBottom);
    expect(marginBottom, `card-top-row margin is ${marginBottom}, should be 10px`).toBe('10px');

    await context.close();
  });
});
