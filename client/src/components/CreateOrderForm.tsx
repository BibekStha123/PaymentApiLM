import { useEffect, useState } from 'react'
import { ApiError } from '../api/client'
import { listProducts, type ProductResponse } from '../api/products'
import { listCurrencies, type CurrencyResponse } from '../api/currencies'
import { listMyCards, type PaymentDetailResponse } from '../api/paymentDetails'
import { createOrder, type OrderItemRequest } from '../api/orders'

interface CreateOrderFormProps {
  onOrderCreated?: (orderId: string) => void
}

interface DraftItem {
  productId: string
  quantity: number
}

function emptyItem(defaultProductId: string): DraftItem {
  return { productId: defaultProductId, quantity: 1 }
}

export function CreateOrderForm({ onOrderCreated }: CreateOrderFormProps) {
  const [products, setProducts] = useState<ProductResponse[] | null>(null)
  const [currencies, setCurrencies] = useState<CurrencyResponse[] | null>(null)
  const [cards, setCards] = useState<PaymentDetailResponse[] | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)

  const [shippingAddress, setShippingAddress] = useState('')
  const [currencyId, setCurrencyId] = useState('')
  const [paymentDetailId, setPaymentDetailId] = useState('')
  const [items, setItems] = useState<DraftItem[]>([])

  const [submitError, setSubmitError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [successOrderId, setSuccessOrderId] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        const [productsPage, currenciesPage, myCards] = await Promise.all([
          listProducts(),
          listCurrencies(),
          listMyCards(),
        ])

        if (cancelled) return

        setProducts(productsPage.items)
        setCurrencies(currenciesPage.items)
        setCards(myCards)
        setCurrencyId(currenciesPage.items[0]?.id ?? '')
        setPaymentDetailId(myCards[0]?.id ?? '')
        setItems(productsPage.items[0] ? [emptyItem(productsPage.items[0].id)] : [])
      } catch (err) {
        if (cancelled) return
        setLoadError(err instanceof ApiError ? err.message : 'Failed to load order options.')
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [])

  function updateItem(index: number, patch: Partial<DraftItem>) {
    setItems((current) => current.map((item, i) => (i === index ? { ...item, ...patch } : item)))
  }

  function addItem() {
    if (!products || products.length === 0) return
    setItems((current) => [...current, emptyItem(products[0].id)])
  }

  function removeItem(index: number) {
    setItems((current) => current.filter((_, i) => i !== index))
  }

  function productPrice(productId: string): number {
    return products?.find((p) => p.id === productId)?.price ?? 0
  }

  const estimatedTotal = items.reduce((sum, item) => sum + productPrice(item.productId) * item.quantity, 0)
  const selectedCurrency = currencies?.find((c) => c.id === currencyId)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setSubmitError(null)

    if (!shippingAddress.trim()) {
      setSubmitError('Shipping address is required.')
      return
    }
    if (!currencyId) {
      setSubmitError('Select a currency.')
      return
    }
    if (!paymentDetailId) {
      setSubmitError('Select a payment card.')
      return
    }
    if (items.length === 0 || items.some((item) => !item.productId || item.quantity < 1)) {
      setSubmitError('Add at least one item with a valid quantity.')
      return
    }

    const requestItems: OrderItemRequest[] = items.map((item) => ({
      productId: item.productId,
      quantity: item.quantity,
    }))

    setIsSubmitting(true)
    try {
      const orderId = await createOrder({
        shippingAddress,
        currencyId,
        paymentDetailId,
        items: requestItems,
      })
      setSuccessOrderId(orderId)
      setItems(products?.[0] ? [emptyItem(products[0].id)] : [])
      onOrderCreated?.(orderId)
    } catch (err) {
      setSubmitError(err instanceof ApiError ? err.message : 'Something went wrong. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  if (loadError) {
    return (
      <p role="alert" style={{ color: 'var(--error)' }}>
        {loadError}
      </p>
    )
  }

  if (!products || !currencies || !cards) {
    return <p>Loading order options…</p>
  }

  if (cards.length === 0) {
    return <p>You need a saved payment card before you can place an order.</p>
  }

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 480 }}>
      <h2>Create order</h2>

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="shippingAddress" style={{ display: 'block', marginBottom: 4 }}>
          Shipping address
        </label>
        <input
          id="shippingAddress"
          type="text"
          value={shippingAddress}
          onChange={(e) => setShippingAddress(e.target.value)}
          required
          style={{ width: '100%', padding: 8, boxSizing: 'border-box' }}
        />
      </div>

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="currency" style={{ display: 'block', marginBottom: 4 }}>
          Currency
        </label>
        <select
          id="currency"
          value={currencyId}
          onChange={(e) => setCurrencyId(e.target.value)}
          style={{ width: '100%', padding: 8 }}
        >
          {currencies.map((c) => (
            <option key={c.id} value={c.id}>
              {c.currencyCode} — {c.name}
            </option>
          ))}
        </select>
      </div>

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="card" style={{ display: 'block', marginBottom: 4 }}>
          Payment card
        </label>
        <select
          id="card"
          value={paymentDetailId}
          onChange={(e) => setPaymentDetailId(e.target.value)}
          style={{ width: '100%', padding: 8 }}
        >
          {cards.map((card) => (
            <option key={card.id} value={card.id}>
              {card.cardOwnerName} — {card.cardNumber}
            </option>
          ))}
        </select>
      </div>

      <div style={{ marginBottom: 12 }}>
        <p style={{ marginBottom: 4 }}>Items</p>
        {items.map((item, index) => (
          <div key={index} style={{ display: 'flex', gap: 8, marginBottom: 8 }}>
            <select
              value={item.productId}
              onChange={(e) => updateItem(index, { productId: e.target.value })}
              style={{ flex: 1, padding: 8 }}
            >
              {products.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name} ({p.price.toFixed(2)}) — {p.stock} in stock
                </option>
              ))}
            </select>
            <input
              type="number"
              min={1}
              value={item.quantity}
              onChange={(e) => updateItem(index, { quantity: Number(e.target.value) })}
              style={{ width: 72, padding: 8, boxSizing: 'border-box' }}
            />
            <button
              type="button"
              onClick={() => removeItem(index)}
              disabled={items.length === 1}
              style={{ padding: '0 12px' }}
            >
              Remove
            </button>
          </div>
        ))}
        <button type="button" onClick={addItem} style={{ padding: '4px 12px' }}>
          Add item
        </button>
      </div>

      <p style={{ marginBottom: 12 }}>
        Estimated total: {estimatedTotal.toFixed(2)} {selectedCurrency?.currencyCode ?? ''}
      </p>

      {submitError && (
        <p role="alert" style={{ color: 'var(--error)', marginBottom: 12 }}>
          {submitError}
        </p>
      )}

      {successOrderId && (
        <p style={{ color: 'var(--success, green)', marginBottom: 12 }}>Order created: {successOrderId}</p>
      )}

      <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px' }}>
        {isSubmitting ? 'Placing order…' : 'Place order'}
      </button>
    </form>
  )
}
