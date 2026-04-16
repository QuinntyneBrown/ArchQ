import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdrResponse, CreateAdrRequest, TemplateResponse } from '../models/adr.model';

@Injectable({ providedIn: 'root' })
export class AdrService {
  private readonly API_URL = 'http://localhost:5000/api/tenants';

  constructor(private readonly http: HttpClient) {}

  createAdr(tenantSlug: string, data: CreateAdrRequest): Observable<AdrResponse> {
    return this.http.post<AdrResponse>(`${this.API_URL}/${tenantSlug}/adrs`, data, { withCredentials: true });
  }

  getTemplate(tenantSlug: string): Observable<TemplateResponse> {
    return this.http.get<TemplateResponse>(`${this.API_URL}/${tenantSlug}/config/template`, { withCredentials: true });
  }
}
