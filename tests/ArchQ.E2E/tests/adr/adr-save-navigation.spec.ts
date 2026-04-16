import { test, expect } from '@playwright/test';

test.describe('ADR Save Navigation', () => {

  test('should navigate to edit route after creating a new ADR', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `savnav-${ts}@example.com`;

    // Register, verify, login
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'SaveNav User', email, password: 'S3cur3P@ss!', organizationName: `SaveNavCorp${ts}` },
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

    // Create new ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Navigation Test ADR');
    const editor = page.locator('textarea, [data-testid="markdown-editor"]').first();
    await editor.fill('## Context\nNav test.');

    // Save
    await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      page.locator('[data-testid="save-draft-button"]').first().click(),
    ]);

    // Should navigate away from /adrs/new to /adrs/{id}/edit
    await page.waitForTimeout(2000);
    const url = page.url();
    expect(url).not.toContain('/adrs/new');
    expect(url).toMatch(/\/adrs\/.+\/edit/);

    await context.close();
  });
});
