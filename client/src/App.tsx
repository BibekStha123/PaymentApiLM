import { useState } from 'react'
import { LoginForm } from './components/LoginForm'
import { CreateOrderForm } from './components/CreateOrderForm'
import type { UserResponse } from './api/users'
import { clearStoredUser, getStoredUser, storeUser } from './api/auth'

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

  if (user) {
    return (
      <div>
        <h1>Welcome, {user.displayName}</h1>
        <p>{user.email}</p>
        <button type="button" onClick={handleLogout} style={{ padding: '8px 16px' }}>
          Log out
        </button>

        <hr style={{ margin: '24px 0' }} />

        <CreateOrderForm />
      </div>
    )
  }

  return <LoginForm onSuccess={handleLoginSuccess} />
}

export default App
