import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tenant, CreateTenantRequest, UpdateTenantRequest } from '../models/tenant.model';

@Injectable({ providedIn: 'root' })
export class TenantService {
  private readonly API_URL = 'http://localhost:5000/api/tenants';

  constructor(private readonly http: HttpClient) {}

  create(req: CreateTenantRequest): Observable<Tenant> {
    return this.http.post<Tenant>(this.API_URL, req);
  }

  getById(id: string): Observable<Tenant> {
    return this.http.get<Tenant>(`${this.API_URL}/${id}`);
  }

  update(id: string, req: UpdateTenantRequest): Observable<Tenant> {
    return this.http.put<Tenant>(`${this.API_URL}/${id}`, req);
  }
}
