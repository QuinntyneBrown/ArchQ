import { test, expect } from '@playwright/test';

test.describe('Create First ADR', () => {

  test('should create the first ADR in a new tenant without errors', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `firstadr-${ts}@example.com`;
    const orgName = `FirstAdrCorp${ts}`;

    // Register, verify, login
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'FirstAdr User', email, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 30000,
    });
    const tokenResp = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });
    await page.waitForTimeout(2000);

    // Navigate to create ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });

    // Fill in the ADR
    await page.fill('[data-testid="adr-title-input"]', 'Test ADR: Use Event-Driven Architecture');
    const editor = page.locator('textarea, [data-testid="markdown-editor"]').first();
    await editor.fill('## Context\nOur system needs event-driven architecture.\n\n## Decision\nWe will use an event bus.');

    // Save
    const saveBtn = page.locator('[data-testid="save-draft-button"]').first();
    const [saveResp] = await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      saveBtn.click(),
    ]);

    // Should return 201 (created), not 500
    expect(saveResp.status()).toBe(201);

    await context.close();
  });
});
