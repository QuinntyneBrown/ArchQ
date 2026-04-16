import { test, expect } from '@playwright/test';

test.describe('BUG-044: Mobile card author should use #5C5F6E color', () => {
  test.setTimeout(120000);

  test('card author color should be #5C5F6E, not #9CA3AF', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 375, height: 812 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `card-auth-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'CardAuth', email, password: 'S3cur3P@ss!', organizationName: `CAuth${ts}` },
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

    // Create an ADR to get a card
    await page.click('[data-testid="new-adr-button-mobile"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Card Author Color Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    const author = page.locator('.card-author').first();
    await expect(author).toBeVisible();

    const color = await author.evaluate(el => getComputedStyle(el).color);
    // Design: #5C5F6E = rgb(92, 95, 110)
    expect(color, `Card author color is ${color}, should be rgb(92, 95, 110)`).toBe('rgb(92, 95, 110)');

    await context.close();
  });
});
