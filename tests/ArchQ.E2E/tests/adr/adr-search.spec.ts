import { test, expect } from '@playwright/test';

test.describe('ADR Search', () => {

  async function setupUserWithAdrs(request: any) {
    const email = `adrsearch-${Date.now()}@example.com`;
    await request.post('http://localhost:5000/api/auth/register', {
      data: { fullName: 'Search User', email, password: 'S3cur3P@ss!', organizationName: 'Search Org' },
    });
    const tokenResp = await request.get(`http://localhost:5000/api/auth/test/verification-token?email=${encodeURIComponent(email)}`);
    const { token } = await tokenResp.json();
    await request.post('http://localhost:5000/api/auth/verify-email', { data: { token } });
    const loginResp = await request.post('http://localhost:5000/api/auth/login', {
      data: { email, password: 'S3cur3P@ss!' },
    });
    const loginBody = await loginResp.json();
    const slug = loginBody.tenant.slug;

    await request.post(`http://localhost:5000/api/tenants/${slug}/adrs`, {
      data: { title: 'Use Event-Driven Architecture', body: '## Context\n\nWe need event-driven processing for orders', tags: ['architecture', 'events'] },
    });
    await request.post(`http://localhost:5000/api/tenants/${slug}/adrs`, {
      data: { title: 'Adopt PostgreSQL Database', body: '## Context\n\nUnified database strategy', tags: ['database'] },
    });
    await request.post(`http://localhost:5000/api/tenants/${slug}/adrs`, {
      data: { title: 'CQRS Pattern Implementation', body: '## Context\n\nSeparate read and write models with event sourcing', tags: ['architecture', 'cqrs'] },
    });
    return { email, slug };
  }

  test('API should search by title keyword', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=PostgreSQL`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.results.length).toBeGreaterThan(0);
    expect(body.results[0].title).toContain('PostgreSQL');
    expect(body.totalHits).toBeGreaterThan(0);
  });

  test('API should search body content', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=event-driven`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.results.length).toBeGreaterThan(0);
  });

  test('API should return 400 for query less than 2 characters', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=a`);
    expect(resp.status()).toBe(400);
  });

  test('API should return empty results for no match', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=xyznonexistent123`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    expect(body.results.length).toBe(0);
    expect(body.totalHits).toBe(0);
  });

  test('API should filter search by status', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=architecture&status=draft`);
    expect(resp.status()).toBe(200);
    const body = await resp.json();
    body.results.forEach((r: any) => expect(r.status).toBe('draft'));
  });

  test('API should include snippet in results', async ({ request }) => {
    const { slug } = await setupUserWithAdrs(request);

    const resp = await request.get(`http://localhost:5000/api/tenants/${slug}/search?q=event-driven`);
    const body = await resp.json();
    if (body.results.length > 0) {
      expect(body.results[0].snippet).toBeTruthy();
    }
  });
});
