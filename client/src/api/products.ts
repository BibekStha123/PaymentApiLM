import { apiGet, type CursorPagedResponse } from './client'

// Mirrors Application/Products/ProductResponse.cs
export interface ProductResponse {
  id: string
  name: string
  description: string
  price: number
  stock: number
  categoryName: string
}

export function listProducts(limit = 50): Promise<CursorPagedResponse<ProductResponse>> {
  return apiGet<CursorPagedResponse<ProductResponse>>(`/products?limit=${limit}`)
}
