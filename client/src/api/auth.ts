import type { UserResponse } from './users'
import { getRoleFromToken } from './jwt'

const STORAGE_KEY = 'auth.user'

export type Role = 'Admin' | 'User'

export function getStoredUser(): UserResponse | null {
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw ? (JSON.parse(raw) as UserResponse) : null
}

export function storeUser(user: UserResponse): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(user))
}

export function clearStoredUser(): void {
  localStorage.removeItem(STORAGE_KEY)
}

export function getToken(): string | null {
  return getStoredUser()?.token ?? null
}

export function getRole(): Role {
  const token = getToken()
  const role = token ? getRoleFromToken(token) : null
  return role === 'Admin' ? 'Admin' : 'User'
}
