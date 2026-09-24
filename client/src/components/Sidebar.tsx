import { NavLink } from 'react-router-dom'
import type { Role } from '../api/auth'
import type { UserResponse } from '../api/users'

interface SidebarProps {
  user: UserResponse
  role: Role
  onLogout: () => void
}

export function Sidebar({ user, role, onLogout }: SidebarProps) {
  return (
    <aside className="sidebar">
      <div className="sidebar-section">
        <p style={{ margin: 0, fontWeight: 600 }}>{user.displayName}</p>
        <p style={{ margin: '2px 0 0', fontSize: 13, opacity: 0.7 }}>{role}</p>
      </div>

      <div className="sidebar-section">
        <p className="sidebar-heading">Shop</p>
        <nav>
          <NavLink to="/shop">New order</NavLink>
          <NavLink to="/my-orders">My orders</NavLink>
        </nav>
      </div>

      {role === 'Admin' && (
        <div className="sidebar-section">
          <p className="sidebar-heading">Admin</p>
          <nav>
            <NavLink to="/admin/products">Products</NavLink>
            <NavLink to="/admin/categories">Categories</NavLink>
            <NavLink to="/admin/orders">All orders</NavLink>
            <NavLink to="/admin/payment-details">Payment details</NavLink>
          </nav>
        </div>
      )}

      <div className="sidebar-section">
        <button type="button" onClick={onLogout} style={{ padding: '8px 16px' }}>
          Log out
        </button>
      </div>
    </aside>
  )
}
