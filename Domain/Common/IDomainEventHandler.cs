namespace PaymentDetailApi.Domain.Common
{
    public interface IDomainEventHandler<T> where T : DomainEvent
    {
        Task Handle(T domainEvent);
    }
}
