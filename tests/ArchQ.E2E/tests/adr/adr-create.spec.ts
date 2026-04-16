import { test, expect } from '@playwright/test';
import { TEST_ADRS } from '../../fixtures/test-data';

test.describe('ADR Creation', () => {

  test('API should create a new ADR in Draft status', async ({ request }) => {
    // Register, verify, login
    const email = `adrcreate-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'ADR Creator', email, password: 'S3cur3P@ss!', organizationName: 'ADR Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    // Create ADR
    const createResp = await request.post(
      `http://localhost:5299/api/tenants/${loginBody.tenant.slug}/adrs`,
      { data: { title: TEST_ADRS.draft.title, body: TEST_ADRS.draft.content, tags: ['architecture'] } }
    );

    expect(createResp.status()).toBe(201);
    const adr = await createResp.json();
    expect(adr.adrNumber).toBe('ADR-001');
    expect(adr.title).toBe(TEST_ADRS.draft.title);
    expect(adr.status).toBe('draft');
    expect(adr.version).toBe(1);
    expect(adr.tags).toContain('architecture');
  });

  test('API should assign sequential ADR numbers', async ({ request }) => {
    const email = `adrseq-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Seq Creator', email, password: 'S3cur3P@ss!', organizationName: 'Seq Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const slug = loginBody.tenant.slug;

    // Create first ADR
    const resp1 = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'First ADR', body: '## Context\n\nFirst', tags: [] },
    });
    const adr1 = await resp1.json();
    expect(adr1.adrNumber).toBe('ADR-001');

    // Create second ADR
    const resp2 = await request.post(`http://localhost:5299/api/tenants/${slug}/adrs`, {
      data: { title: 'Second ADR', body: '## Context\n\nSecond', tags: [] },
    });
    const adr2 = await resp2.json();
    expect(adr2.adrNumber).toBe('ADR-002');
  });

  test('API should return 400 for missing title', async ({ request }) => {
    const email = `adrmissing-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Missing Title', email, password: 'S3cur3P@ss!', organizationName: 'Missing Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const createResp = await request.post(
      `http://localhost:5299/api/tenants/${loginBody.tenant.slug}/adrs`,
      { data: { title: '', body: '## Context\n\nSomething', tags: [] } }
    );
    expect(createResp.status()).toBe(400);
  });

  test('API should get tenant template', async ({ request }) => {
    const email = `adrtempl-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Template User', email, password: 'S3cur3P@ss!', organizationName: 'Template Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5299/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();

    const templateResp = await request.get(
      `http://localhost:5299/api/tenants/${loginBody.tenant.slug}/config/template`
    );
    expect(templateResp.status()).toBe(200);
    const template = await templateResp.json();
    expect(template.body).toContain('## Context');
    expect(template.body).toContain('## Decision');
    expect(template.requiredSections).toBeTruthy();
  });

  test('should create ADR via UI editor', async ({ page, request }) => {
    const email = `adreditor-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Editor User', email, password: 'S3cur3P@ss!', organizationName: 'Editor Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    // Login via UI
    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    // Navigate to create
    await page.goto('/adrs/new');

    // Fill in ADR
    await expect(page.locator('[data-testid="adr-title-input"]')).toBeVisible();
    await page.locator('[data-testid="adr-title-input"]').fill('Test ADR Title');
    await page.locator('[data-testid="markdown-edit-area"]').fill('## Context\n\nTest context\n\n## Decision\n\nTest decision');
    await page.locator('[data-testid="save-draft-button"]').click();

    // Should show success
    const toast = await page.locator('[data-testid="toast-message"]');
    await expect(toast).toBeVisible();

    // Status badge should show Draft
    await expect(page.locator('[data-testid="status-badge"]')).toContainText('Draft');
  });

  test('should show live preview of markdown content', async ({ page, request }) => {
    const email = `adrpreview-${Date.now()}@example.com`;
    await request.post('http://localhost:5299/api/auth/register', {
      data: { fullName: 'Preview User', email, password: 'S3cur3P@ss!', organizationName: 'Preview Org' },
    });
    const tokenResp = await request.get(`http://localhost:5299/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5299/api/auth/verify-email', { data: { token } });

    await page.goto('/login');
    await page.locator('[data-testid="email-input"]').fill(email);
    await page.locator('[data-testid="password-input"]').fill('S3cur3P@ss!');
    await page.locator('[data-testid="sign-in-button"]').click();
    await expect(page).toHaveURL(/\/adrs/);

    await page.goto('/adrs/new');
    await page.locator('[data-testid="markdown-edit-area"]').fill('## Decision\n\nWe will use **PostgreSQL**.');

    // Preview pane should render markdown
    const previewPane = page.locator('[data-testid="preview-pane"]');
    await expect(previewPane.locator('h2')).toContainText('Decision');
    await expect(previewPane.locator('strong')).toContainText('PostgreSQL');
  });
});
