import { Outlet } from 'react-router-dom'
import type { Role } from '../api/auth'
import type { UserResponse } from '../api/users'
import { Sidebar } from './Sidebar'

interface AppLayoutProps {
  user: UserResponse
  role: Role
  onLogout: () => void
}

export function AppLayout({ user, role, onLogout }: AppLayoutProps) {
  return (
    <div className="app-shell">
      <Sidebar user={user} role={role} onLogout={onLogout} />
      <main className="app-content">
        <Outlet />
      </main>
    </div>
  )
}
