import { test, expect } from '@playwright/test';

test.describe('ADR List Tablet Responsive Layout', () => {

  test('sidebar should be hidden on tablet viewport (810px)', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 810, height: 1080 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `tablet-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Tablet User', email, password: 'S3cur3P@ss!', organizationName: `TabletCorp${ts}` },
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

    // Sidebar should be hidden on tablet
    const sidebar = page.locator('.app-sidebar');
    await expect(sidebar).toBeHidden();

    // Hamburger menu should be visible
    const hamburger = page.locator('[data-testid="hamburger-menu"]');
    await expect(hamburger).toBeVisible();

    await context.close();
  });
});
