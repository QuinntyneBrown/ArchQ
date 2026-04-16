import { test, expect } from '@playwright/test';

test.describe('BUG-036: Editor badge color should reflect actual status', () => {
  test.setTimeout(120000);

  test('non-draft ADR editor badge should not use amber color', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const authorEmail = `badge-dyn-${ts}@example.com`;
    const reviewerEmail = `badge-rev-${ts}@example.com`;
    const orgName = `BDyn${ts}`;

    // Register author + reviewer in same org
    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Author', email: authorEmail, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 60000,
    });
    let r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(authorEmail)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    await context.request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Reviewer', email: reviewerEmail, password: 'S3cur3P@ss!', organizationName: orgName },
      timeout: 60000,
    });
    r = await context.request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(reviewerEmail)}`);
    await context.request.post('http://localhost:5299/api/auth/verify-email', { data: { token: (await r.json()).token } });

    // Login as author
    let login = await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    const { tenant } = await login.json();
    const slug = tenant.slug;

    // Get reviewer ID
    login = await context.request.post('http://localhost:5299/api/auth/login', { data: { email: reviewerEmail, password: 'S3cur3P@ss!' } });
    const reviewerId = (await login.json()).user.id;

    // Login as author, assign reviewer role
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/users/${reviewerId}/roles`, { data: { role: 'reviewer' } });

    // Create ADR via API
    const createResp = await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'Badge Dynamic Test', body: '## Context\nTest.' },
    });
    const adr = await createResp.json();
    const adrId = adr.id;

    // Submit for review + approve
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adrId}/transitions`, {
      data: { targetStatus: 'in-review', approverIds: [reviewerId] },
    });
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: reviewerEmail, password: 'S3cur3P@ss!' } });
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adrId}/decisions`, {
      data: { decision: 'approved', comment: 'OK' },
    });

    // Login as author and navigate to editor
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    await page.goto(`http://localhost:4200/adrs/${adrId}/edit`, { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // The badge should show "approved" with emerald color, not hardcoded amber
    const badge = page.locator('[data-testid="status-badge"]');
    await expect(badge).toBeVisible();

    const color = await badge.evaluate(el => getComputedStyle(el).color);
    // If the badge is hardcoded amber, color will be rgb(245, 158, 11)
    // It should NOT be amber for an approved ADR
    expect(
      color,
      `Editor badge color is amber ${color} for an approved ADR. Should vary by status.`
    ).not.toBe('rgb(245, 158, 11)');

    await context.close();
  });
});
