import { test, expect } from '@playwright/test';

test.describe('ADR Author Avatar Color', () => {
  test.setTimeout(60000);

  test('should display author avatar with a non-gray color generated from name', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `avatar-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Quinn Brown', email, password: 'S3cur3P@ss!', organizationName: `AvCorp${ts}` },
      timeout: 30000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await r.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });
    await page.waitForTimeout(2000);

    // Create an ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Avatar Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      page.locator('[data-testid="save-draft-button"]').first().click(),
    ]);

    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // The avatar should NOT be the default gray #374151 (rgb(55, 65, 81))
    const avatar = page.locator('.author-avatar').first();
    const bg = await avatar.evaluate(el => getComputedStyle(el).backgroundColor);

    // Should not be the flat gray
    expect(bg).not.toBe('rgb(55, 65, 81)');

    await context.close();
  });
});
