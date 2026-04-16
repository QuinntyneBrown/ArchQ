import { test, expect } from '@playwright/test';

test.describe('ADR Filter Dropdown Label', () => {

  test('should display "All Statuses" as the default filter option', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `filter-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Filter User', email, password: 'S3cur3P@ss!', organizationName: `FilterCorp${ts}` },
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

    // The first option in the status filter should say "All Statuses"
    const firstOption = page.locator('[data-testid="status-filter"] option').first();
    await expect(firstOption).toHaveText('All Statuses');

    await context.close();
  });
});
