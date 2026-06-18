namespace PaymentDetailApi.Domain.Common
{
    public abstract class Entity
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;
        protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearEvents() => _domainEvents.Clear();
    }
}
