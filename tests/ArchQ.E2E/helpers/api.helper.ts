import { type APIRequestContext } from '@playwright/test';

const API_BASE = process.env.API_URL || 'http://localhost:5000';

export class ApiHelper {
  constructor(private readonly request: APIRequestContext) {}

  async createTenant(data: { displayName: string; slug: string }, token: string) {
    return this.request.post(`${API_BASE}/api/tenants`, {
      data,
      headers: { Authorization: `Bearer ${token}` },
    });
  }

  async getTenant(id: string, token: string) {
    return this.request.get(`${API_BASE}/api/tenants/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  }

  async updateTenant(id: string, data: { displayName: string }, token: string) {
    return this.request.patch(`${API_BASE}/api/tenants/${id}`, {
      data,
      headers: { Authorization: `Bearer ${token}` },
    });
  }

  async login(email: string, password: string) {
    const resp = await this.request.post(`${API_BASE}/api/auth/login`, {
      data: { email, password },
    });
    const body = await resp.json();
    return body.token as string;
  }
}
