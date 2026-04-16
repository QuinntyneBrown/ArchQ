import { test, expect } from '@playwright/test';

test.describe('Sidebar Logo Icon', () => {
  test.setTimeout(60000);

  test('should display a temple icon before ArchQ text in the sidebar', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `logo-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Logo User', email, password: 'S3cur3P@ss!', organizationName: `LogoCorp${ts}` },
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

    // Sidebar should have an SVG logo icon
    const logoIcon = page.locator('.sidebar-header svg');
    await expect(logoIcon).toBeVisible();

    await context.close();
  });
});
