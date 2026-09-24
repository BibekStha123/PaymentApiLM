import { apiGet, apiPatch, apiPost, type CursorPagedResponse } from './client'

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

// Mirrors Domain/Orders/OrderStatus.cs
export const OrderStatus = {
  0: 'Pending',
  1: 'Confirmed',
  2: 'Shipped',
  3: 'Delivered',
  4: 'Cancelled',
} as const

// Mirrors Application/Orders/OrderResponse.cs
export interface OrderItemResponse {
  productId: string
  unitPrice: number
  quantity: number
  totalPrice: number
}

export interface OrderResponse {
  id: string
  userId: string
  userName: string
  shippingAddress: string
  currencyCode: string
  status: number
  orderDate: string
  totalAmount: number
  items: OrderItemResponse[]
}

export function createOrder(request: OrderRequest): Promise<string> {
  const idempotencyKey = crypto.randomUUID()
  return apiPost<string>('/orders', request, { 'Idempotency-Key': idempotencyKey })
}

export function listMyOrders(limit = 50): Promise<CursorPagedResponse<OrderResponse>> {
  return apiGet<CursorPagedResponse<OrderResponse>>(`/orders/my-orders?limit=${limit}`)
}

export function listAllOrders(limit = 50): Promise<CursorPagedResponse<OrderResponse>> {
  return apiGet<CursorPagedResponse<OrderResponse>>(`/orders?limit=${limit}`)
}

export function cancelOrder(id: string): Promise<void> {
  return apiPatch<void>(`/orders/${id}/cancel`)
}
