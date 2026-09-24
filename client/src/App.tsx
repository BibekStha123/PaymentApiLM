import { useState } from 'react'
import { Navigate, Route, Routes } from 'react-router-dom'
import { AdminCategories } from './components/AdminCategories'
import { AdminOrders } from './components/AdminOrders'
import { AdminPaymentDetails } from './components/AdminPaymentDetails'
import { AdminProducts } from './components/AdminProducts'
import { AppLayout } from './components/AppLayout'
import { CreateOrderForm } from './components/CreateOrderForm'
import { LoginForm } from './components/LoginForm'
import { MyOrders } from './components/MyOrders'
import { RequireAdmin } from './components/RequireAdmin'
import { clearStoredUser, getRole, getStoredUser, storeUser } from './api/auth'
import type { UserResponse } from './api/users'

function App() {
  const [user, setUser] = useState<UserResponse | null>(getStoredUser)

  function handleLoginSuccess(loggedInUser: UserResponse) {
    storeUser(loggedInUser)
    setUser(loggedInUser)
  }

  function handleLogout() {
    clearStoredUser()
    setUser(null)
  }

  if (!user) {
    return (
      <div className="centered">
        <LoginForm onSuccess={handleLoginSuccess} />
      </div>
    )
  }

  const role = getRole()

  return (
    <Routes>
      <Route element={<AppLayout user={user} role={role} onLogout={handleLogout} />}>
        <Route path="/shop" element={<CreateOrderForm />} />
        <Route path="/my-orders" element={<MyOrders />} />
        <Route
          path="/admin/products"
          element={
            <RequireAdmin role={role}>
              <AdminProducts />
            </RequireAdmin>
          }
        />
        <Route
          path="/admin/categories"
          element={
            <RequireAdmin role={role}>
              <AdminCategories />
            </RequireAdmin>
          }
        />
        <Route
          path="/admin/orders"
          element={
            <RequireAdmin role={role}>
              <AdminOrders />
            </RequireAdmin>
          }
        />
        <Route
          path="/admin/payment-details"
          element={
            <RequireAdmin role={role}>
              <AdminPaymentDetails />
            </RequireAdmin>
          }
        />
        <Route path="*" element={<Navigate to="/shop" replace />} />
      </Route>
    </Routes>
  )
}

export default App
