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

export interface AdrDetailResponse {
  id: string;
  adrNumber: string;
  title: string;
  body: string;
  status: string;
  authorId: string;
  tags: string[];
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateAdrRequest {
  title: string;
  body: string;
  tags?: string[];
}

export interface UpdateAdrResponse {
  id: string;
  adrNumber: string;
  title: string;
  status: string;
  version: number;
  updatedAt: string;
}

export interface TemplateResponse {
  body: string;
  requiredSections: string[];
  isCustom: boolean;
  updatedAt: string | null;
}
