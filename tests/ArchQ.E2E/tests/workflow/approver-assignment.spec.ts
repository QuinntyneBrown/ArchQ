import { test, expect } from '@playwright/test';

test.describe('Approver Assignment', () => {

  async function setupTwoUsers(request: any) {
    const ts = Date.now();
    const adminEmail = `approver-admin-${ts}@example.com`;
    const reviewerEmail = `approver-reviewer-${ts}@example.com`;

    // Register admin user (creates org)
    await request.post('http://localhost:5000/api/auth/register', {
      data: { fullName: 'Admin Author', email: adminEmail, password: 'S3cur3P@ss!', organizationName: `Approver Org ${ts}` },
    });
    const adminTokenResp = await request.get(`http://localhost:5000/api/auth/test/verification-token?email=${encodeURIComponent(adminEmail)}`);
    const { token: adminToken } = await adminTokenResp.json();
    await request.post('http://localhost:5000/api/auth/verify-email', { data: { token: adminToken } });
    const adminLogin = await request.post('http://localhost:5000/api/auth/login', {
      data: { email: adminEmail, password: 'S3cur3P@ss!' },
    });
    const adminBody = await adminLogin.json();
    const slug = adminBody.tenant.slug;
    const tenantId = adminBody.tenant.id;
    const adminUserId = adminBody.user.id;

    // Create ADR as admin
    const adrResp = await request.post(`http://localhost:5000/api/tenants/${slug}/adrs`, {
      data: { title: 'Approver Test ADR', body: '## Context\n\nApprover test', tags: [] },
    });
    const adr = await adrResp.json();

    return { adminEmail, reviewerEmail, slug, tenantId, adminUserId, adr };
  }

  test('API should reject self-assignment as approver', async ({ request }) => {
    const { slug, adminUserId, adr } = await setupTwoUsers(request);

    const resp = await request.post(`http://localhost:5000/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'InReview', approverIds: [adminUserId] },
    });

    // Self-assignment should be rejected (author trying to approve own ADR)
    // Note: In our current setup, the admin IS the author. The validator
    // should reject this with SELF_ASSIGNMENT
    expect(resp.status()).toBe(422);
    const body = await resp.json();
    expect(body.error).toBe('VALIDATION_FAILED');
  });

  test('API should list users with Reviewer role', async ({ request }) => {
    const { slug } = await setupTwoUsers(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/users?role=reviewer`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.items).toBeTruthy();
    // May be empty if no reviewers assigned yet — that's OK
    expect(Array.isArray(body.items)).toBe(true);
  });

  test('API should require at least one approver for submit', async ({ request }) => {
    const { slug, adr } = await setupTwoUsers(request);

    const resp = await request.post(`http://localhost:5000/api/tenants/${slug}/adrs/${adr.id}/transitions`, {
      data: { targetStatus: 'InReview', approverIds: [] },
    });
    expect(resp.status()).toBe(422);
  });
});
