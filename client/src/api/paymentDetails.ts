import { apiGet, type CursorPagedResponse } from './client'

// Mirrors Application/PaymentDetails/PaymentDetailResponse.cs
export interface PaymentDetailResponse {
  id: string
  cardOwnerName: string
  cardNumber: string
  expirationDate: string
  securityCode: string
  active: boolean
}

export function listMyCards(): Promise<PaymentDetailResponse[]> {
  return apiGet<PaymentDetailResponse[]>('/payment-details/my-cards')
}

export function listAllPaymentDetails(limit = 50): Promise<CursorPagedResponse<PaymentDetailResponse>> {
  return apiGet<CursorPagedResponse<PaymentDetailResponse>>(`/payment-details?limit=${limit}`)
}
