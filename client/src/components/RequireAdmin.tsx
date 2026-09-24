import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import type { Role } from '../api/auth'

interface RequireAdminProps {
  role: Role
  children: ReactNode
}

export function RequireAdmin({ role, children }: RequireAdminProps) {
  if (role !== 'Admin') {
    return <Navigate to="/shop" replace />
  }

  return <>{children}</>
}
