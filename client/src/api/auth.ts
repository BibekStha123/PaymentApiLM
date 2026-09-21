import type { UserResponse } from './users'

const STORAGE_KEY = 'auth.user'

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
