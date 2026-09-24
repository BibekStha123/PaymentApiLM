import { useEffect, useState, type FormEvent } from 'react'
import { ApiError } from '../api/client'
import { createCategory, listCategories, type CategoryResponse } from '../api/categories'

export function AdminCategories() {
  const [categories, setCategories] = useState<CategoryResponse[] | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)

  const [name, setName] = useState('')
  const [type, setType] = useState('')
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function load() {
    try {
      const page = await listCategories()
      setCategories(page.items)
    } catch (err) {
      setLoadError(err instanceof ApiError ? err.message : 'Failed to load categories.')
    }
  }

  useEffect(() => {
    load()
  }, [])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSubmitError(null)

    if (!name.trim() || !type.trim()) {
      setSubmitError('Name and type are required.')
      return
    }

    setIsSubmitting(true)
    try {
      await createCategory({ name, type })
      setName('')
      setType('')
      await load()
    } catch (err) {
      setSubmitError(err instanceof ApiError ? err.message : 'Failed to create category.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div>
      <h2>Categories</h2>

      <form onSubmit={handleSubmit} style={{ maxWidth: 360, marginBottom: 24 }}>
        <div style={{ marginBottom: 12 }}>
          <label htmlFor="categoryName" style={{ display: 'block', marginBottom: 4 }}>
            Name
          </label>
          <input
            id="categoryName"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
          />
        </div>

        <div style={{ marginBottom: 12 }}>
          <label htmlFor="categoryType" style={{ display: 'block', marginBottom: 4 }}>
            Type
          </label>
          <input
            id="categoryType"
            type="text"
            value={type}
            onChange={(e) => setType(e.target.value)}
            style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
          />
        </div>

        {submitError && (
          <p role="alert" style={{ color: 'var(--error)', marginBottom: 12 }}>
            {submitError}
          </p>
        )}

        <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px' }}>
          {isSubmitting ? 'Creating…' : 'Create category'}
        </button>
      </form>

      {loadError && (
        <p role="alert" style={{ color: 'var(--error)' }}>
          {loadError}
        </p>
      )}

      {!loadError && !categories && <p>Loading categories…</p>}

      {categories && categories.length === 0 && <p>No categories yet.</p>}

      {categories && categories.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Type</th>
            </tr>
          </thead>
          <tbody>
            {categories.map((category) => (
              <tr key={category.id}>
                <td>{category.name}</td>
                <td>{category.type}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
