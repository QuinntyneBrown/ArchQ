import { test, expect } from '@playwright/test';

test.describe('BUG-033: ADR numbers should use monospace font', () => {
  test.setTimeout(120000);

  test('ADR number link should use a monospace font family', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `mono-font-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Mono Tester', email, password: 'S3cur3P@ss!', organizationName: `MonoCorp${ts}` },
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

    // Create an ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Monospace Font Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await page.locator('[data-testid="save-draft-button"]').first().click();
    await page.waitForURL(/\/adrs\/.*\/edit/, { timeout: 15000 });
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // Check the ADR number font family
    const adrLink = page.locator('.adr-number-link').first();
    await expect(adrLink).toBeVisible();

    const fontFamily = await adrLink.evaluate(el => getComputedStyle(el).fontFamily);

    // The font-family should contain a monospace font (Geist Mono, Fira Code, monospace, etc.)
    // not just Inter/sans-serif
    const isMonospace = /mono|consolas|courier/i.test(fontFamily);
    expect(
      isMonospace,
      `ADR number font-family is "${fontFamily}" — should contain a monospace font per design`
    ).toBe(true);

    await context.close();
  });
});
