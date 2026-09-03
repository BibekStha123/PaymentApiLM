import { apiPost } from './client'

// Mirrors Application/Users/UserResponse.cs
export interface UserResponse {
  email: string
  displayName: string
  token: string
}

export function login(email: string, password: string): Promise<UserResponse> {
  return apiPost<UserResponse>('/login', { email, password })
}
