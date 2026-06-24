using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Shared
{
    public class Log : Entity
    {
        public int Id { get; private set; }
        public string Action { get; private set; } = string.Empty;
        public string EntityName { get; private set; } = string.Empty;
        public int EntityId { get; private set; }
        public string Details { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }

        private Log() { }

        public static Log Create(string action, string entityName, int entityId, string details)
        {
            return new Log
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
