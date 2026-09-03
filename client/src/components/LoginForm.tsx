import { useState, type FormEvent } from 'react'
import { ApiError } from '../api/client'
import { login, type UserResponse } from '../api/users'

interface LoginFormProps {
  onSuccess: (user: UserResponse) => void
}

export function LoginForm({ onSuccess }: LoginFormProps) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      const user = await login(email, password)
      onSuccess(user)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Something went wrong. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h1>Log in</h1>

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="email" style={{ display: 'block', marginBottom: 4 }}>
          Email
        </label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
        />
      </div>

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="password" style={{ display: 'block', marginBottom: 4 }}>
          Password
        </label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
        />
      </div>

      {error && (
        <p role="alert" style={{ color: 'var(--error)', marginBottom: 12 }}>
          {error}
        </p>
      )}

      <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px' }}>
        {isSubmitting ? 'Logging in…' : 'Log in'}
      </button>
    </form>
  )
}
