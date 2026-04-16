import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdrResponse, AdrDetailResponse, CreateAdrRequest, UpdateAdrRequest, UpdateAdrResponse, TemplateResponse, AdrListResponse, AdrListParams, SearchResponse } from '../models/adr.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdrService {
  private readonly API_URL = `${environment.apiUrl}/tenants`;

  constructor(private readonly http: HttpClient) {}

  listAdrs(tenantSlug: string, params?: AdrListParams): Observable<AdrListResponse> {
    const queryParams: Record<string, string> = {};
    if (params?.status && params.status !== 'All') {
      queryParams['status'] = params.status;
    }
    if (params?.search) {
      queryParams['search'] = params.search;
    }
    if (params?.cursor) {
      queryParams['cursor'] = params.cursor;
    }
    if (params?.limit) {
      queryParams['limit'] = params.limit.toString();
    }
    return this.http.get<AdrListResponse>(`${this.API_URL}/${tenantSlug}/adrs`, {
      params: queryParams,
      withCredentials: true
    });
  }

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

  searchAdrs(tenantSlug: string, query: string, status?: string, pageSize?: number, offset?: number): Observable<SearchResponse> {
    const queryParams: Record<string, string> = { q: query };
    if (status) {
      queryParams['status'] = status;
    }
    if (pageSize !== undefined) {
      queryParams['pageSize'] = pageSize.toString();
    }
    if (offset !== undefined) {
      queryParams['offset'] = offset.toString();
    }
    return this.http.get<SearchResponse>(`${this.API_URL}/${tenantSlug}/search`, {
      params: queryParams,
      withCredentials: true
    });
  }
}
