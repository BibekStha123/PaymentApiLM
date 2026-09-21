import { apiPost } from './client'

export interface OrderItemRequest {
  productId: string
  quantity: number
}

// Mirrors API/Controllers/Orders/OrderRequest.cs
export interface OrderRequest {
  shippingAddress: string
  currencyId: string
  paymentDetailId: string
  items: OrderItemRequest[]
}

export function createOrder(request: OrderRequest): Promise<string> {
  const idempotencyKey = crypto.randomUUID()
  return apiPost<string>('/orders', request, { 'Idempotency-Key': idempotencyKey })
}
