using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Users.Events;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Users
{
    public class UserCreatedEventHanlder : IDomainEventHandler<UserCreatedDomainEvent>
    {
        public Task Handle(UserCreatedDomainEvent domainEvent)
        {
            Console.Write(domainEvent.User + " Created");

            return Task.CompletedTask;
        }
    }
}
