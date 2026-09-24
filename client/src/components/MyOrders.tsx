import { useEffect, useState } from 'react'
import { ApiError } from '../api/client'
import { cancelOrder, listMyOrders, OrderStatus, type OrderResponse } from '../api/orders'

export function MyOrders() {
  const [orders, setOrders] = useState<OrderResponse[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [cancellingId, setCancellingId] = useState<string | null>(null)

  async function load() {
    try {
      const page = await listMyOrders()
      setOrders(page.items)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load orders.')
    }
  }

  useEffect(() => {
    load()
  }, [])

  async function handleCancel(id: string) {
    setError(null)
    setCancellingId(id)
    try {
      await cancelOrder(id)
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to cancel order.')
    } finally {
      setCancellingId(null)
    }
  }

  if (error) {
    return (
      <p role="alert" style={{ color: 'var(--error)' }}>
        {error}
      </p>
    )
  }

  if (!orders) {
    return <p>Loading your orders…</p>
  }

  if (orders.length === 0) {
    return <p>You haven't placed any orders yet.</p>
  }

  return (
    <div>
      <h2>My orders</h2>
      <table>
        <thead>
          <tr>
            <th>Placed</th>
            <th>Shipping address</th>
            <th>Status</th>
            <th>Total</th>
            <th />
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => {
            const status = OrderStatus[order.status as keyof typeof OrderStatus] ?? 'Unknown'
            const canCancel = status === 'Pending' || status === 'Confirmed'
            return (
              <tr key={order.id}>
                <td>{new Date(order.orderDate).toLocaleString()}</td>
                <td>{order.shippingAddress}</td>
                <td>{status}</td>
                <td>
                  {order.totalAmount.toFixed(2)} {order.currencyCode}
                </td>
                <td>
                  {canCancel && (
                    <button
                      type="button"
                      onClick={() => handleCancel(order.id)}
                      disabled={cancellingId === order.id}
                      style={{ padding: '4px 10px' }}
                    >
                      {cancellingId === order.id ? 'Cancelling…' : 'Cancel'}
                    </button>
                  )}
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
