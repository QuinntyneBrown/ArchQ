import { test, expect } from '@playwright/test';

test.describe('ADR Status Filter', () => {
  test.setTimeout(120000);

  test('should filter ADRs by Draft status correctly', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `statusfilter-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'StatusFilter User', email, password: 'S3cur3P@ss!', organizationName: `SFCorp${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
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
    await page.fill('[data-testid="adr-title-input"]', 'Status Filter Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      page.locator('[data-testid="save-draft-button"]').first().click(),
    ]);
    await page.goto('http://localhost:4200/adrs', { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // Verify ADR is visible with "All Statuses"
    const allCount = await page.locator('[data-testid="adr-row"]').count();
    expect(allCount).toBeGreaterThan(0);

    // Select "Draft" filter
    await page.selectOption('[data-testid="status-filter"]', 'Draft');
    await page.waitForTimeout(3000);

    // Should still show the ADR (it's in draft status)
    const draftCount = await page.locator('[data-testid="adr-row"]').count();
    expect(draftCount).toBeGreaterThan(0);

    await context.close();
  });
});
