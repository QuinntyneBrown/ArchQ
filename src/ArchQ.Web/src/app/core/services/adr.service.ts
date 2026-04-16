import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdrResponse, AdrDetailResponse, CreateAdrRequest, UpdateAdrRequest, UpdateAdrResponse, TemplateResponse } from '../models/adr.model';

@Injectable({ providedIn: 'root' })
export class AdrService {
  private readonly API_URL = 'http://localhost:5000/api/tenants';

  constructor(private readonly http: HttpClient) {}

  createAdr(tenantSlug: string, data: CreateAdrRequest): Observable<AdrResponse> {
    return this.http.post<AdrResponse>(`${this.API_URL}/${tenantSlug}/adrs`, data, { withCredentials: true });
  }

  getAdr(tenantSlug: string, adrId: string): Observable<AdrDetailResponse> {
    return this.http.get<AdrDetailResponse>(`${this.API_URL}/${tenantSlug}/adrs/${adrId}`, { withCredentials: true });
  }

  updateAdr(tenantSlug: string, adrId: string, data: UpdateAdrRequest): Observable<UpdateAdrResponse> {
    return this.http.put<UpdateAdrResponse>(`${this.API_URL}/${tenantSlug}/adrs/${adrId}`, data, { withCredentials: true });
  }

  getTemplate(tenantSlug: string): Observable<TemplateResponse> {
    return this.http.get<TemplateResponse>(`${this.API_URL}/${tenantSlug}/config/template`, { withCredentials: true });
  }
}
