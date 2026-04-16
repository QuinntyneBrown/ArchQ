import { test, expect } from '@playwright/test';

test.describe('ADR Author Attribution', () => {

  test('should set the logged-in user as author when creating an ADR', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `author-${ts}@example.com`;
    const fullName = 'Author Test User';
    const orgName = `AuthorCorp${ts}`;

    // Register, verify
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName, email, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 30000,
    });
    const tokenResp = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 10000 });
    await page.waitForTimeout(2000);

    // Create ADR
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });
    await page.fill('[data-testid="adr-title-input"]', 'Test Author ADR');
    const editor = page.locator('textarea, [data-testid="markdown-editor"]').first();
    await editor.fill('## Context\nTest');

    const saveBtn = page.locator('[data-testid="save-draft-button"]').first();
    const [saveResp] = await Promise.all([
      page.waitForResponse(r => r.url().includes('/adrs') && r.request().method() === 'POST', { timeout: 10000 }),
      saveBtn.click(),
    ]);

    const body = await saveResp.json();

    // The authorId should NOT be empty
    expect(body.authorId).toBeTruthy();
    expect(body.authorId).not.toBe('');

    await context.close();
  });
});
