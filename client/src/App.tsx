import { useState } from 'react'
import { LoginForm } from './components/LoginForm'
import type { UserResponse } from './api/users'

const STORAGE_KEY = 'auth.user'

function loadStoredUser(): UserResponse | null {
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw ? (JSON.parse(raw) as UserResponse) : null
}

function App() {
  const [user, setUser] = useState<UserResponse | null>(loadStoredUser)

  function handleLoginSuccess(loggedInUser: UserResponse) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(loggedInUser))
    setUser(loggedInUser)
  }

  function handleLogout() {
    localStorage.removeItem(STORAGE_KEY)
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
      </div>
    )
  }

  return <LoginForm onSuccess={handleLoginSuccess} />
}

export default App
