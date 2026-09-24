import { getToken } from './auth'

const API_BASE = '/api/v1'

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

// Mirrors Application/Common/CursorPagedResponse.cs
export interface CursorPagedResponse<T> {
  items: T[]
  nextCursor: string | null
}

function authHeaders(): Record<string, string> {
  const token = getToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}

async function handleResponse<TResponse>(res: Response): Promise<TResponse> {
  if (!res.ok) {
    const text = await res.text()
    throw new ApiError(res.status, text || res.statusText)
  }

  if (res.status === 204) {
    return undefined as TResponse
  }

  return res.json() as Promise<TResponse>
}

export async function apiGet<TResponse>(path: string): Promise<TResponse> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: { ...authHeaders() },
  })

  return handleResponse<TResponse>(res)
}

export async function apiPost<TResponse>(
  path: string,
  body: unknown,
  extraHeaders?: Record<string, string>,
): Promise<TResponse> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders(), ...extraHeaders },
    body: JSON.stringify(body),
  })

  return handleResponse<TResponse>(res)
}

export async function apiPatch<TResponse>(path: string, body?: unknown): Promise<TResponse> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })

  return handleResponse<TResponse>(res)
}
