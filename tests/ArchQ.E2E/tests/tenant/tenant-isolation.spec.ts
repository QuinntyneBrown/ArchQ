import { test, expect } from '../../fixtures/tenant.fixture';
import { TEST_USERS } from '../../fixtures/test-data';

test.describe('Tenant Isolation', () => {

  test('should not expose tenants created by other users to unauthorized viewers', async ({ page, apiHelper }) => {
    // Create a tenant as admin
    const adminToken = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'Private Org',
      slug: `private-org-${Date.now()}`,
    }, adminToken);
    const tenant = await response.json();

    // Attempt to access tenant detail as a viewer from a different org
    // Should return 404, not 403 (per security design)
    const viewerToken = await apiHelper.login(TEST_USERS.viewer.email, TEST_USERS.viewer.password);
    const detailResponse = await apiHelper.getTenant(tenant.id, viewerToken);

    expect(detailResponse.status()).toBe(404);
  });

  test('API should return 201 for valid tenant creation', async ({ apiHelper }) => {
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'API Test Org',
      slug: `api-test-${Date.now()}`,
    }, token);

    expect(response.status()).toBe(201);

    const body = await response.json();
    expect(body.id).toBeTruthy();
    expect(body.displayName).toBe('API Test Org');
    expect(body.status).toBe('active');
    expect(body.createdAt).toBeTruthy();
  });

  test('API should return 409 for duplicate slug', async ({ apiHelper }) => {
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const slug = `dup-test-${Date.now()}`;

    await apiHelper.createTenant({ displayName: 'First', slug }, token);
    const response = await apiHelper.createTenant({ displayName: 'Second', slug }, token);

    expect(response.status()).toBe(409);
    const body = await response.json();
    expect(body.error).toBe('SLUG_CONFLICT');
  });

  test('API should return 400 for invalid slug format', async ({ apiHelper }) => {
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'Bad Slug',
      slug: 'INVALID_SLUG!',
    }, token);

    expect(response.status()).toBe(400);
    const body = await response.json();
    expect(body.error).toBe('VALIDATION_ERROR');
  });
});
