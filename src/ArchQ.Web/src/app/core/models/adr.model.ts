export interface AdrResponse {
  id: string;
  adrNumber: number;
  title: string;
  content: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAdrRequest {
  title: string;
  content: string;
}

export interface TemplateResponse {
  content: string;
}
