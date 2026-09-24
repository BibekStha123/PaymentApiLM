import { apiGet, apiPost, type CursorPagedResponse } from './client'

// Mirrors Application/Categories/CategoryResponse.cs
export interface CategoryResponse {
  id: string
  name: string
  type: string
}

// Mirrors API/Controllers/Categories/CategoryRequest.cs
export interface CategoryRequest {
  name: string
  type: string
}

export function listCategories(limit = 50): Promise<CursorPagedResponse<CategoryResponse>> {
  return apiGet<CursorPagedResponse<CategoryResponse>>(`/categories?limit=${limit}`)
}

export function createCategory(request: CategoryRequest): Promise<string> {
  return apiPost<string>('/categories', request)
}
