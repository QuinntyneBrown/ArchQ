export interface Tenant {
  id: string;
  displayName: string;
  slug: string;
  status: string;
  plan: string;
  createdAt: string;
  updatedAt: string;
  settings: {
    maxUsers: number;
    maxAdrs: number;
  };
}

export interface CreateTenantRequest {
  displayName: string;
  slug: string;
}

export interface UpdateTenantRequest {
  displayName: string;
}

export interface ApiError {
  error: string;
  message: string;
}
