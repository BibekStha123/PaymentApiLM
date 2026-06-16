using PaymentDetailApi.Domain.Common;
using DomainUser = PaymentDetailApi.Domain.User.Entities.User;

namespace PaymentDetailApi.Domain.Users.Events
{
    public class UserCreatedDomainEvent : DomainEvent
    {
        public DomainUser User { get; }

        public UserCreatedDomainEvent(DomainUser user)
        {
            User = user;
        }
    }
}
