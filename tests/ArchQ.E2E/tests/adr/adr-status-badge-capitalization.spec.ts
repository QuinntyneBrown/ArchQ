import { test, expect } from '@playwright/test';

test.describe('Status Badge Capitalization', () => {

  test('should display status badges with capitalized text', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `badge-${ts}@example.com`;

    // Register, verify, login
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Badge User', email, password: 'S3cur3P@ss!', organizationName: `BadgeCorp${ts}` },
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

    // Create an ADR (status will be "draft")
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Badge Test ADR');
    const editor = page.locator('textarea, [data-testid="markdown-editor"]').first();
    await editor.fill('## Context\nTest.');
    await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      page.locator('[data-testid="save-draft-button"]').first().click(),
    ]);

    // Go to list
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // The badge should display capitalized text (via CSS text-transform)
    const badge = page.locator('.status-badge').first();
    const displayedText = await badge.evaluate(el => {
      const style = getComputedStyle(el);
      const raw = el.textContent?.trim() || '';
      // If text-transform is capitalize, the visual output is capitalized
      if (style.textTransform === 'capitalize') return raw.replace(/\b\w/g, c => c.toUpperCase());
      return raw;
    });

    // First character should be uppercase
    expect(displayedText[0]).toBe(displayedText[0].toUpperCase());

    await context.close();
  });
});
