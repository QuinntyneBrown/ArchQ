import { test, expect } from '@playwright/test';

test.describe('BUG-031: Mobile filter pills should be single row', () => {
  test.setTimeout(120000);

  test('filter pills should not wrap to multiple rows at 375px', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 375, height: 812 } });
    const page = await context.newPage();

    const ts = Date.now();
    const email = `pill-row-${ts}@example.com`;

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Pill Tester', email, password: 'S3cur3P@ss!', organizationName: `PillCorp${ts}` },
      timeout: 60000,
    });
    const r = await context.request.get(
      `http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`
    );
    const { token } = await r.json();
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', email);
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(2000);

    // Filter pills should be visible on mobile
    const pills = page.locator('.filter-pills');
    await expect(pills).toBeVisible();

    // All pills should be on the same vertical position (single row).
    // If any pill has a different top offset, the pills are wrapping.
    const pillTops = await pills.evaluate(el =>
      Array.from(el.children).map(c => Math.round(c.getBoundingClientRect().top))
    );

    const uniqueTops = [...new Set(pillTops)];
    expect(
      uniqueTops.length,
      `Filter pills span ${uniqueTops.length} rows (tops: ${uniqueTops.join(', ')}). Should be 1 row.`
    ).toBe(1);

    await context.close();
  });
});
