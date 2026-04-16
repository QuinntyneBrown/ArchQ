import { test, expect } from '@playwright/test';

test.describe('ADR List & Browse', () => {

  async function setupUserWithAdrs(request: any, count: number) {
    const email = `adrlist-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'List User', email, password: 'S3cur3P@ss!', organizationName: 'List Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const slug = loginBody.tenant.slug;

    for (let i = 0; i < count; i++) {
      await request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
        data: { title: `ADR Title ${i + 1}`, body: `## Context\n\nContent ${i + 1}`, tags: ['test'] },
      });
    }
    return { email, slug, loginBody };
  }

  test('API should list ADRs with default sort (updatedAt desc)', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request, 3);

    const resp = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.items).toBeTruthy();
    expect(body.items.length).toBe(3);
    expect(body.totalCount).toBe(3);
    // Default sort: newest first
    expect(body.items[0].adrNumber).toBe('ADR-003');
  });

  test('API should filter by status', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request, 2);

    const resp = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs?status=draft`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.items.length).toBe(2);
    body.items.forEach((item: any) => expect(item.status).toBe('draft'));
  });

  test('API should paginate with cursor', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request, 5);

    // Get first page of 2
    const resp1 = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs?pageSize=2`);
    const body1 = await resp1.json();
    expect(body1.items.length).toBe(2);
    expect(body1.nextCursor).toBeTruthy();
    expect(body1.totalCount).toBe(5);

    // Get next page
    const resp2 = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs?pageSize=2&cursor=${body1.nextCursor}`);
    const body2 = await resp2.json();
    expect(body2.items.length).toBe(2);
    // Items should be different from page 1
    expect(body2.items[0].id).not.toBe(body1.items[0].id);
  });

  test('API should include author name in response', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request, 1);

    const resp = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs`);
    const body = await resp.json();
    expect(body.items[0].authorName).toBe('List User');
  });

  test('API should cap pageSize at 100', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request, 1);

    const resp = await request.get(`http://localhost:5299/api/tenants/${slug}/adrs?pageSize=200`);
    expect(resp.status()).toBe(200);
    // Server should silently cap at 100, not error
  });

  test('should display ADR list on dashboard', async ({ page, request }) => {
    const { email } = await setupUserWithAdrs(request, 3);

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Should show page title
    await expect(page.locator('[data-testid="page-title"]')).toContainText('Architecture Decision Records');

    // Should show New ADR button
    await expect(page.locator('[data-testid="new-adr-button"]')).toBeVisible();

    // Should show ADR rows or cards
    const adrCount = await page.locator('[data-testid="adr-row"], [data-testid="adr-card"]').count();
    expect(adrCount).toBe(3);
  });

  test('should show status filter on desktop', async ({ page, request }) => {
    const { email } = await setupUserWithAdrs(request, 1);

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    await expect(page.locator('[data-testid="status-filter"]')).toBeVisible();
  });
});

test.describe('ADR List — Mobile', () => {
  test.use({ viewport: { width: 375, height: 812 } });

  test('should show card layout on mobile', async ({ page, request }) => {
    const email = `adrlistmobile-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Mobile User', email, password: 'S3cur3P@ss!', organizationName: 'Mobile Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const slug = (await loginResp.json()).tenant.slug;
    await request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'Mobile ADR', body: '## Context\n\nMobile', tags: [] },
    });

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Cards should be visible on mobile
    await expect(page.locator('[data-testid="adr-card"]').first()).toBeVisible();
    // Table should not be visible
    await expect(page.locator('[data-testid="adr-table"]')).not.toBeVisible();
  });
});
