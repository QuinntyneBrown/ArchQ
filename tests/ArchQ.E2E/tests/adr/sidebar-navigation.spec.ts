import { test, expect } from '@playwright/test';

test.describe('Sidebar Navigation', () => {

  test('should show ADR Records navigation link in the sidebar on desktop', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `sidenav-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Nav User', email, password: 'S3cur3P@ss!', organizationName: `NavCorp${ts}` },
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

    // Sidebar should contain a navigation link for "ADR Records"
    const navLink = page.locator('.app-sidebar a[routerLink="/adrs"], .app-sidebar [data-testid="nav-adr-records"]');
    await expect(navLink).toBeVisible();
    await expect(navLink).toContainText('ADR Records');

    await context.close();
  });
});
