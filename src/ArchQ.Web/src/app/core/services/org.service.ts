import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TenantInfo, MembershipInfo } from './auth.service';
import { environment } from '../../../environments/environment';

export interface MembershipsResponse {
  memberships: {
    tenantId: string;
    tenantSlug: string;
    tenantDisplayName: string;
    role: string;
  }[];
}

export interface SwitchOrgResponse {
  tenant: TenantInfo;
  user: { id: string; roles: string[] };
}

@Injectable({ providedIn: 'root' })
export class OrgService {
  private readonly apiUrl = `${environment.apiUrl}/orgs`;

  constructor(private readonly http: HttpClient) {}

  listMemberships(): Observable<MembershipsResponse> {
    return this.http.get<MembershipsResponse>(this.apiUrl, { withCredentials: true });
  }

  switchOrg(tenantSlug: string): Observable<SwitchOrgResponse> {
    return this.http.post<SwitchOrgResponse>(`${this.apiUrl}/switch`, { tenantSlug }, { withCredentials: true });
  }
}
