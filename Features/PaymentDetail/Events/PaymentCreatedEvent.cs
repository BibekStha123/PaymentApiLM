using MediatR;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Features.PaymentDetail.Events
{
    public record PaymentCreatedEvent(PaymentDetails PaymentDetails) : INotification;
}
