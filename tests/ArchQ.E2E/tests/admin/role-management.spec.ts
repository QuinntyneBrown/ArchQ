import { test, expect } from '@playwright/test';

test.describe('Role Management', () => {

  test('API should add a role to a user', async ({ request }) => {
    // Register admin user, verify, login
    const email = `roleadmin-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Role Admin', email, password: 'S3cur3P@ss!', organizationName: 'Role Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const tenantId = loginBody.tenant.id;
    const userId = loginBody.user.id;

    // Add reviewer role
    const addResp = await request.post(
      `http://localhost:5299/api/tenants/${tenantId}/users/${userId}/roles`,
      { data: { role: 'reviewer' } }
    );
    expect(addResp.status()).toBe(200);
    const body = await addResp.json();
    expect(body.roles).toContain('reviewer');
  });

  test('API should remove a role from a user', async ({ request }) => {
    const email = `roleremove-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Role Remove', email, password: 'S3cur3P@ss!', organizationName: 'Remove Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const tenantId = loginBody.tenant.id;
    const userId = loginBody.user.id;

    // First add author role
    await request.post(
      `http://localhost:5299/api/tenants/${tenantId}/users/${userId}/roles`,
      { data: { role: 'author' } }
    );

    // Remove author role
    const removeResp = await request.delete(
      `http://localhost:5299/api/tenants/${tenantId}/users/${userId}/roles/author`
    );
    expect(removeResp.status()).toBe(200);
    const body = await removeResp.json();
    expect(body.roles).not.toContain('author');
  });

  test('API should return 400 for invalid role name', async ({ request }) => {
    const email = `roleinvalid-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Invalid Role', email, password: 'S3cur3P@ss!', organizationName: 'Invalid Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const addResp = await request.post(
      `http://localhost:5299/api/tenants/${loginBody.tenant.id}/users/${loginBody.user.id}/roles`,
      { data: { role: 'superadmin' } }
    );
    expect(addResp.status()).toBe(400);
    const body = await addResp.json();
    expect(body.error).toBe('INVALID_ROLE');
  });

  test('API should return 409 when removing last admin', async ({ request }) => {
    const email = `lastadmin-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Last Admin', email, password: 'S3cur3P@ss!', organizationName: 'Last Admin Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    // Try to remove admin role from the only admin
    const removeResp = await request.delete(
      `http://localhost:5299/api/tenants/${loginBody.tenant.id}/users/${loginBody.user.id}/roles/admin`
    );
    expect(removeResp.status()).toBe(409);
    const body = await removeResp.json();
    expect(body.error).toBe('LAST_ADMIN');
  });

  test('API should get user roles with permissions', async ({ request }) => {
    const email = `getroles-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Get Roles', email, password: 'S3cur3P@ss!', organizationName: 'Get Roles Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const rolesResp = await request.get(
      `http://localhost:5299/api/tenants/${loginBody.tenant.id}/users/${loginBody.user.id}/roles`
    );
    expect(rolesResp.status()).toBe(200);
    const body = await rolesResp.json();
    expect(body.roles).toContain('admin');
    expect(body.permissions).toBeTruthy();
    expect(body.permissions.length).toBeGreaterThan(0);
  });

  test('API should set roles via PUT (full replace)', async ({ request }) => {
    const email = `setroles-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Set Roles', email, password: 'S3cur3P@ss!', organizationName: 'Set Roles Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const setResp = await request.put(
      `http://localhost:5299/api/tenants/${loginBody.tenant.id}/users/${loginBody.user.id}/roles`,
      { data: { roles: ['admin', 'author', 'reviewer'] } }
    );
    expect(setResp.status()).toBe(200);
    const body = await setResp.json();
    expect(body.roles).toEqual(expect.arrayContaining(['admin', 'author', 'reviewer']));
  });

  test('API should list all role definitions', async ({ request }) => {
    const email = `listroles-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'List Roles', email, password: 'S3cur3P@ss!', organizationName: 'List Roles Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const rolesResp = await request.get(
      `http://localhost:5299/api/tenants/${loginBody.tenant.id}/roles`
    );
    expect(rolesResp.status()).toBe(200);
    const body = await rolesResp.json();
    expect(body.length).toBe(4);
    expect(body.map((r: any) => r.role)).toEqual(
      expect.arrayContaining(['admin', 'author', 'reviewer', 'viewer'])
    );
  });
});
