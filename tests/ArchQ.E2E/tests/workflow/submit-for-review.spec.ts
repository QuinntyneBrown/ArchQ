import { test, expect } from '@playwright/test';

test.describe('ADR Workflow — Submit for Review', () => {

  async function setupUserAndAdr(request: any) {
    const email = `workflow-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Workflow User', email, password: 'S3cur3P@ss!', organizationName: 'Workflow Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const slug = loginBody.tenant.slug;
    const userId = loginBody.user.id;

    const adrResp = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'Workflow Test ADR', body: '## Context\n\nWorkflow test', tags: [] },
    });
    const adr = await adrResp.json();
    return { email, slug, userId, adr, loginBody };
  }

  test('API should transition Draft -> InReview with approvers', async ({ request }) => {
    const { slug, userId, adr } = await setupUserAndAdr(request);

    const resp = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'InReview', approverIds: [userId] },
    });
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.status).toBe('InReview');
  });

  test('API should reject invalid transition Draft -> Approved', async ({ request }) => {
    const { slug, adr } = await setupUserAndAdr(request);

    const resp = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'Approved' },
    });
    expect(resp.status()).toBe(400);
    const body = await resp.json();
    expect(body.error).toBe('INVALID_TRANSITION');
  });

  test('API should require approvers for Draft -> InReview', async ({ request }) => {
    const { slug, adr } = await setupUserAndAdr(request);

    const resp = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'InReview', approverIds: [] },
    });
    expect(resp.status()).toBe(422);
    const body = await resp.json();
    expect(body.error).toBe('VALIDATION_FAILED');
  });

  test('API should allow Rejected -> Draft', async ({ request }) => {
    const { slug, userId, adr } = await setupUserAndAdr(request);

    // Submit for review
    await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'InReview', approverIds: [userId] },
    });

    // Reject
    await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'Rejected', reason: 'Needs more analysis' },
    });

    // Return to draft
    const resp = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'Draft' },
    });
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.status).toBe('Draft');
  });
});
