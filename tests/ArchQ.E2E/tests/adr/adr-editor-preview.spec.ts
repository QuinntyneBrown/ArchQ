import { test, expect } from '@playwright/test';

test.describe('ADR Editor Preview', () => {

  test('should render markdown content in the preview panel on desktop', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `preview-${ts}@example.com`;

    // Register, verify, login
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Preview User', email, password: 'S3cur3P@ss!', organizationName: `PreviewCorp${ts}` },
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

    // Navigate to new ADR editor
    await page.click('[data-testid="new-adr-button"]');
    await page.waitForURL(/\/adrs\/new/, { timeout: 5000 });

    // Type markdown content
    const editor = page.locator('textarea, [data-testid="markdown-editor"]').first();
    await editor.fill('## Context\nOur system needs event-driven architecture.');
    await page.waitForTimeout(500);

    // The preview pane should render the markdown as HTML
    const previewPane = page.locator('[data-testid="preview-pane"]');
    await expect(previewPane).toBeVisible();

    // Should contain rendered h2 heading
    const previewH2 = previewPane.locator('h2');
    await expect(previewH2).toBeVisible({ timeout: 3000 });
    await expect(previewH2).toContainText('Context');

    await context.close();
  });
});
