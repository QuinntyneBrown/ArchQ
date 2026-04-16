import { test, expect } from '@playwright/test';

test.describe('ADR Editor Save Button Label', () => {
  test.setTimeout(60000);

  test('should show "Save Draft" when editing a draft ADR', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `savelabel-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'SaveLabel User', email, password: 'S3cur3P@ss!', organizationName: `SLCorp${ts}` },
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

    // Create ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Save Label Test');
    await page.locator('textarea').first().fill('## Context\nTest.');
    await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      page.locator('[data-testid="save-draft-button"]').first().click(),
    ]);

    // Navigate to edit page
    await page.waitForURL(/\/edit/, { timeout: 5000 });
    await page.waitForTimeout(2000);

    // The save button should say "Save Draft" (not "Save Changes")
    const saveBtn = page.locator('[data-testid="save-draft-button"]');
    await expect(saveBtn).toContainText('Save Draft');

    await context.close();
  });
});
