import { apiGet, type CursorPagedResponse } from './client'

// Mirrors Application/Currency/CurrencyResponse.cs
export interface CurrencyResponse {
  id: string
  currencyCode: string
  name: string
}

export function listCurrencies(limit = 50): Promise<CursorPagedResponse<CurrencyResponse>> {
  return apiGet<CursorPagedResponse<CurrencyResponse>>(`/currencies?limit=${limit}`)
}
