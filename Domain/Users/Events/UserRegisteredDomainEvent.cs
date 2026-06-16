using PaymentDetailApi.Domain.Common;
using DomainUser = PaymentDetailApi.Domain.User.Entities.User;

namespace PaymentDetailApi.Domain.Users.Events
{
    public class UserRegisteredDomainEvent : DomainEvent
    {
        public DomainUser User { get; }

        public UserRegisteredDomainEvent(DomainUser user)
        {
            User = user;
        }
    }
}
