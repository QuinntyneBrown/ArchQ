export interface AdrResponse {
  id: string;
  adrNumber: string;
  title: string;
  status: string;
  authorId: string;
  tags: string[];
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAdrRequest {
  title: string;
  body: string;
  tags?: string[];
}

export interface TemplateResponse {
  body: string;
  requiredSections: string[];
  isCustom: boolean;
  updatedAt: string | null;
}
