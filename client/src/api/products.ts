import { apiGet, apiPatch, apiPost, type CursorPagedResponse } from './client'

// Mirrors Application/Products/ProductResponse.cs
export interface ProductResponse {
  id: string
  name: string
  description: string
  price: number
  stock: number
  categoryName: string
}

// Mirrors API/Controllers/Products/ProductRequest.cs
export interface ProductRequest {
  name: string
  description: string
  price: number
  stock: number
  categoryId: string
  isActive: boolean
}

export function listProducts(limit = 50): Promise<CursorPagedResponse<ProductResponse>> {
  return apiGet<CursorPagedResponse<ProductResponse>>(`/products?limit=${limit}`)
}

export function createProduct(request: ProductRequest): Promise<string> {
  return apiPost<string>('/products', request)
}

export function addStock(id: string, stock: number): Promise<number> {
  return apiPatch<number>(`/products/${id}`, { stock })
}
