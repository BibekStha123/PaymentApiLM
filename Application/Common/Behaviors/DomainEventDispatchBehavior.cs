using MediatR;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Infrastructure.DomainEvents;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Common.Behaviors
{
    public class DomainEventDispatchBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        private readonly PaymentDetailsContext _context;
        private readonly DomainEventDispatcher _dispatcher;

        public DomainEventDispatchBehavior(PaymentDetailsContext context, DomainEventDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var aggregates = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
            aggregates.ForEach(a => a.ClearEvents());

            await _dispatcher.Dispatch(events);
            await _context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}
