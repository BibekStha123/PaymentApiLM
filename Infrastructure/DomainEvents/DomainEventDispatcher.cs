using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Infrastructure.DomainEvents
{
    public class DomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(IEnumerable<DomainEvent> events)
        {
            foreach (var domainEvent in events)
            {
                var eventType = domainEvent.GetType();

                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    await (Task)handlerType
                        .GetMethod("Handle")
                        .Invoke(handler, new[] { domainEvent });
                }
            }
        }
    }
}
