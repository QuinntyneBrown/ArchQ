import { test, expect } from '@playwright/test';

test.describe('BUG-040: Org switcher font-weight should be 500', () => {
  test.setTimeout(120000);

  test('org name text should have font-weight 500', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `org-weight-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'OrgW Test', email, password: 'S3cur3P@ss!', organizationName: `OrgW${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });

    const orgName = page.locator('.org-name').first();
    await expect(orgName).toBeVisible();

    const fontWeight = await orgName.evaluate(el => getComputedStyle(el).fontWeight);
    expect(fontWeight, `Org name font-weight is ${fontWeight}, should be 500`).toBe('500');

    await context.close();
  });
});
