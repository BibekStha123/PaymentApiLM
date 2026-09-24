import { useEffect, useState, type FormEvent } from 'react'
import { ApiError } from '../api/client'
import { listCategories, type CategoryResponse } from '../api/categories'
import { addStock, createProduct, listProducts, type ProductResponse } from '../api/products'

export function AdminProducts() {
  const [products, setProducts] = useState<ProductResponse[] | null>(null)
  const [categories, setCategories] = useState<CategoryResponse[] | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)

  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [price, setPrice] = useState('')
  const [stock, setStock] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const [stockDrafts, setStockDrafts] = useState<Record<string, string>>({})
  const [updatingStockId, setUpdatingStockId] = useState<string | null>(null)
  const [stockError, setStockError] = useState<string | null>(null)

  async function loadProducts() {
    try {
      const page = await listProducts()
      setProducts(page.items)
    } catch (err) {
      setLoadError(err instanceof ApiError ? err.message : 'Failed to load products.')
    }
  }

  useEffect(() => {
    loadProducts()
    listCategories()
      .then((page) => {
        setCategories(page.items)
        setCategoryId(page.items[0]?.id ?? '')
      })
      .catch((err) => setLoadError(err instanceof ApiError ? err.message : 'Failed to load categories.'))
  }, [])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSubmitError(null)

    const priceValue = Number(price)
    const stockValue = Number(stock)

    if (!name.trim()) {
      setSubmitError('Name is required.')
      return
    }
    if (!categoryId) {
      setSubmitError('Select a category.')
      return
    }
    if (!Number.isFinite(priceValue) || priceValue < 0) {
      setSubmitError('Enter a valid price.')
      return
    }
    if (!Number.isInteger(stockValue) || stockValue < 0) {
      setSubmitError('Enter a valid stock quantity.')
      return
    }

    setIsSubmitting(true)
    try {
      await createProduct({
        name,
        description,
        price: priceValue,
        stock: stockValue,
        categoryId,
        isActive: true,
      })
      setName('')
      setDescription('')
      setPrice('')
      setStock('')
      await loadProducts()
    } catch (err) {
      setSubmitError(err instanceof ApiError ? err.message : 'Failed to create product.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleAddStock(productId: string) {
    setStockError(null)
    const draft = Number(stockDrafts[productId])
    if (!Number.isInteger(draft) || draft <= 0) {
      setStockError('Enter a positive whole number to add.')
      return
    }

    setUpdatingStockId(productId)
    try {
      await addStock(productId, draft)
      setStockDrafts((current) => ({ ...current, [productId]: '' }))
      await loadProducts()
    } catch (err) {
      setStockError(err instanceof ApiError ? err.message : 'Failed to update stock.')
    } finally {
      setUpdatingStockId(null)
    }
  }

  return (
    <div>
      <h2>Products</h2>

      <form onSubmit={handleSubmit} style={{ maxWidth: 420, marginBottom: 24 }}>
        <div style={{ marginBottom: 12 }}>
          <label htmlFor="productName" style={{ display: 'block', marginBottom: 4 }}>
            Name
          </label>
          <input
            id="productName"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
          />
        </div>

        <div style={{ marginBottom: 12 }}>
          <label htmlFor="productDescription" style={{ display: 'block', marginBottom: 4 }}>
            Description
          </label>
          <input
            id="productDescription"
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
          />
        </div>

        <div style={{ display: 'flex', gap: 12, marginBottom: 12 }}>
          <div style={{ flex: 1 }}>
            <label htmlFor="productPrice" style={{ display: 'block', marginBottom: 4 }}>
              Price
            </label>
            <input
              id="productPrice"
              type="number"
              min={0}
              step="0.01"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
            />
          </div>
          <div style={{ flex: 1 }}>
            <label htmlFor="productStock" style={{ display: 'block', marginBottom: 4 }}>
              Initial stock
            </label>
            <input
              id="productStock"
              type="number"
              min={0}
              value={stock}
              onChange={(e) => setStock(e.target.value)}
              style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
            />
          </div>
        </div>

        <div style={{ marginBottom: 12 }}>
          <label htmlFor="productCategory" style={{ display: 'block', marginBottom: 4 }}>
            Category
          </label>
          <select
            id="productCategory"
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            style={{ width: '100%', padding: 8 }}
          >
            {(categories ?? []).map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
          {categories && categories.length === 0 && (
            <p style={{ fontSize: 13, opacity: 0.7, marginTop: 4 }}>
              No categories yet — create one first.
            </p>
          )}
        </div>

        {submitError && (
          <p role="alert" style={{ color: 'var(--error)', marginBottom: 12 }}>
            {submitError}
          </p>
        )}

        <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px' }}>
          {isSubmitting ? 'Creating…' : 'Create product'}
        </button>
      </form>

      {loadError && (
        <p role="alert" style={{ color: 'var(--error)' }}>
          {loadError}
        </p>
      )}

      {!loadError && !products && <p>Loading products…</p>}

      {stockError && (
        <p role="alert" style={{ color: 'var(--error)', marginBottom: 12 }}>
          {stockError}
        </p>
      )}

      {products && products.length === 0 && <p>No products yet.</p>}

      {products && products.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th>Price</th>
              <th>Stock</th>
              <th>Add stock</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td>{product.name}</td>
                <td>{product.categoryName}</td>
                <td>{product.price.toFixed(2)}</td>
                <td>{product.stock}</td>
                <td>
                  <div style={{ display: 'flex', gap: 6 }}>
                    <input
                      type="number"
                      min={1}
                      value={stockDrafts[product.id] ?? ''}
                      onChange={(e) =>
                        setStockDrafts((current) => ({ ...current, [product.id]: e.target.value }))
                      }
                      style={{ width: 64, padding: 4 }}
                    />
                    <button
                      type="button"
                      onClick={() => handleAddStock(product.id)}
                      disabled={updatingStockId === product.id}
                      style={{ padding: '4px 10px' }}
                    >
                      {updatingStockId === product.id ? 'Adding…' : 'Add'}
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
