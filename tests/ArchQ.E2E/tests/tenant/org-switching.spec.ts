import { test, expect } from '@playwright/test';

test.describe('Organization Switching', () => {

  test('should display org switcher with current org name', async ({ page, request }) => {
    // Register, verify, login a user
    const email = `orgswitch-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Org Switch User', email, password: 'S3cur3P@ss!', organizationName: 'Switch Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Org switcher should be visible with org name
    await expect(page.locator('[data-testid="org-switcher"]')).toBeVisible();
    await expect(page.locator('[data-testid="active-org"]')).toContainText('Switch Org');
  });

  test('should not show dropdown for single-org user', async ({ page, request }) => {
    const email = `singleorg-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Single Org User', email, password: 'S3cur3P@ss!', organizationName: 'Solo Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Click org switcher — dropdown should NOT appear for single-org user
    const orgSwitcher = page.locator('[data-testid="org-switcher"]');
    await orgSwitcher.click();
    await expect(page.locator('[data-testid="org-dropdown"]')).not.toBeVisible();
  });

  test('API should list memberships', async ({ request }) => {
    const email = `listorgs-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'List Orgs User', email, password: 'S3cur3P@ss!', organizationName: 'List Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login to get cookies
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    expect(loginResp.status()).toBe(200);

    // List memberships
    const orgsResp = await request.get('http://localhost:5299/api/orgs');
    expect(orgsResp.status()).toBe(200);
    const body = await orgsResp.json();
    expect(body.memberships).toBeTruthy();
    expect(body.memberships.length).toBeGreaterThan(0);
    expect(body.memberships[0].displayName).toBeTruthy();
    expect(body.memberships[0].isActive).toBe(true);
  });

  test('API should return 403 for non-member switch', async ({ request }) => {
    const email = `noswitch-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'No Switch User', email, password: 'S3cur3P@ss!', organizationName: 'No Switch Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });

    // Try to switch to a non-existent org
    const switchResp = await request.post('http://localhost:5299/api/orgs/switch', {
      data: { tenantSlug: 'non-existent-org' },
    });
    expect(switchResp.status()).toBe(403);
    const body = await switchResp.json();
    expect(body.error).toBe('NOT_A_MEMBER');
  });
});
