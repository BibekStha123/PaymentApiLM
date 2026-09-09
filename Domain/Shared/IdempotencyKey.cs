using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Shared
{
    public class IdempotencyKey : Entity
    {
        public Guid UserId { get; private set; }
        public string Key { get; private set; } = string.Empty;
        public Guid? OrderId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private IdempotencyKey() { }

        public static IdempotencyKey Create(Guid userId, string key)
        {
            return new IdempotencyKey
            {
                UserId = userId,
                Key = key,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AttachOrder(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
