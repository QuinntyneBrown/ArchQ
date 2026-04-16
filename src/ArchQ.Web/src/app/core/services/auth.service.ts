import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  organizationName: string;
  inviteToken?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserInfo {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
}

export interface TenantInfo {
  id: string;
  slug: string;
  displayName: string;
}

export interface MembershipInfo {
  tenantId: string;
  role: string;
}

export interface LoginResponse {
  user: UserInfo;
  tenant: TenantInfo;
  memberships: MembershipInfo[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5000/api/auth';

  private readonly _currentUser = signal<UserInfo | null>(null);
  private readonly _currentTenant = signal<TenantInfo | null>(null);
  private readonly _memberships = signal<MembershipInfo[]>([]);

  readonly currentUser = this._currentUser.asReadonly();
  readonly currentTenant = this._currentTenant.asReadonly();
  readonly memberships = this._memberships.asReadonly();
  readonly isLoggedIn = computed(() => this._currentUser() !== null);

  constructor(private readonly http: HttpClient) {}

  register(data: RegisterRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/register`, data);
  }

  verifyEmail(token: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/verify-email`, { token });
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password }, { withCredentials: true }).pipe(
      tap(response => {
        this._currentUser.set(response.user);
        this._currentTenant.set(response.tenant);
        this._memberships.set(response.memberships);
      })
    );
  }

  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true }).pipe(
      tap(() => {
        this._currentUser.set(null);
        this._currentTenant.set(null);
        this._memberships.set([]);
      })
    );
  }

  refreshToken(): Observable<any> {
    return this.http.post(`${this.apiUrl}/refresh`, {}, { withCredentials: true });
  }

  getCurrentUser(): UserInfo | null {
    return this._currentUser();
  }

  updateTenant(tenant: TenantInfo): void {
    this._currentTenant.set(tenant);
  }

  updateMemberships(memberships: MembershipInfo[]): void {
    this._memberships.set(memberships);
  }
}
