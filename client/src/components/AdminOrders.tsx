import { useEffect, useState } from 'react'
import { ApiError } from '../api/client'
import { listAllOrders, OrderStatus, type OrderResponse } from '../api/orders'

export function AdminOrders() {
  const [orders, setOrders] = useState<OrderResponse[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    listAllOrders()
      .then((page) => setOrders(page.items))
      .catch((err) => setError(err instanceof ApiError ? err.message : 'Failed to load orders.'))
  }, [])

  if (error) {
    return (
      <p role="alert" style={{ color: 'var(--error)' }}>
        {error}
      </p>
    )
  }

  if (!orders) {
    return <p>Loading orders…</p>
  }

  return (
    <div>
      <h2>All orders</h2>
      {orders.length === 0 ? (
        <p>No orders yet.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Placed</th>
              <th>Customer</th>
              <th>Shipping address</th>
              <th>Status</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id}>
                <td>{new Date(order.orderDate).toLocaleString()}</td>
                <td>{order.userName}</td>
                <td>{order.shippingAddress}</td>
                <td>{OrderStatus[order.status as keyof typeof OrderStatus] ?? 'Unknown'}</td>
                <td>
                  {order.totalAmount.toFixed(2)} {order.currencyCode}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
