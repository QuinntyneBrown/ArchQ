import { test, expect } from '@playwright/test';

test.describe('Root Route Redirect', () => {
  test.setTimeout(60000);

  test('should redirect logged-in users from / to /adrs', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `root-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Root User', email, password: 'S3cur3P@ss!', organizationName: `RootCorp${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await r.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    // Navigate to root
    await page.goto('http://localhost:4200/', { waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);

    // Should be on /adrs, not /login
    expect(page.url()).toContain('/adrs');

    await context.close();
  });
});
