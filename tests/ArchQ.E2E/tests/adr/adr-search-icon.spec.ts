import { test, expect } from '@playwright/test';

test.describe('ADR Search Input Icon', () => {
  test.setTimeout(60000);

  test('should display a search icon inside the search input field', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `searchicon-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Icon User', email, password: 'S3cur3P@ss!', organizationName: `IconCorp${ts}` },
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
    await page.waitForTimeout(1000);

    // Should have a search icon (SVG) near the search input
    const searchIcon = page.locator('.search-wrapper svg, [data-testid="search-icon"]');
    await expect(searchIcon).toBeVisible();

    await context.close();
  });
});
