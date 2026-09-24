import { useEffect, useState } from 'react'
import { ApiError } from '../api/client'
import { listAllPaymentDetails, type PaymentDetailResponse } from '../api/paymentDetails'

export function AdminPaymentDetails() {
  const [cards, setCards] = useState<PaymentDetailResponse[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    listAllPaymentDetails()
      .then((page) => setCards(page.items))
      .catch((err) => setError(err instanceof ApiError ? err.message : 'Failed to load payment details.'))
  }, [])

  if (error) {
    return (
      <p role="alert" style={{ color: 'var(--error)' }}>
        {error}
      </p>
    )
  }

  if (!cards) {
    return <p>Loading payment details…</p>
  }

  return (
    <div>
      <h2>Payment details</h2>
      {cards.length === 0 ? (
        <p>No saved payment details.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Owner</th>
              <th>Card number</th>
              <th>Expiration</th>
              <th>Active</th>
            </tr>
          </thead>
          <tbody>
            {cards.map((card) => (
              <tr key={card.id}>
                <td>{card.cardOwnerName}</td>
                <td>{card.cardNumber}</td>
                <td>{card.expirationDate}</td>
                <td>{card.active ? 'Yes' : 'No'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
