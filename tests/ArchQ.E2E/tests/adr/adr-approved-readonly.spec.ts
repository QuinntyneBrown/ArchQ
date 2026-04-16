import { test, expect } from '@playwright/test';

test.describe('BUG-054: Approved ADR should be read-only', () => {
  test.setTimeout(120000);

  test('save button should be disabled for an approved ADR', async ({ browser }) => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const page = await context.newPage();

    const ts = Date.now();
    const authorEmail = `readonly-${ts}@example.com`;
    const reviewerEmail = `ro-rev-${ts}@example.com`;
    const orgName = `RO${ts}`;

    // Register author + reviewer
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

    // Login as author, get slugs/IDs
    let login = await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    const slug = (await login.json()).tenant.slug;

    login = await context.request.post('http://localhost:5299/api/auth/login', { data: { email: reviewerEmail, password: 'S3cur3P@ss!' } });
    const reviewerId = (await login.json()).user.id;

    // Assign reviewer role
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/users/${reviewerId}/roles`, { data: { role: 'reviewer' } });

    // Create ADR via API
    const createResp = await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'Readonly Test', body: '## Context\nTest.' },
    });
    const adr = await createResp.json();

    // Approve via workflow
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'in-review', approverIds: [reviewerId] },
    });
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: reviewerEmail, password: 'S3cur3P@ss!' } });
    await context.request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/decisions`, {
      data: { decision: 'approved', comment: 'OK' },
    });

    // Login as author and navigate to editor
    await context.request.post('http://localhost:5299/api/auth/login', { data: { email: authorEmail, password: 'S3cur3P@ss!' } });
    await page.goto(`http://localhost:4200/adrs/${adr.id}/edit`, { waitUntil: 'networkidle' });
    await page.waitForTimeout(3000);

    // Save button should be DISABLED for an approved ADR
    const saveBtn = page.locator('[data-testid="save-draft-button"]').first();
    await expect(saveBtn).toBeVisible();

    const isDisabled = await saveBtn.isDisabled();
    expect(isDisabled, 'Save button should be disabled for approved ADR').toBe(true);

    await context.close();
  });
});
