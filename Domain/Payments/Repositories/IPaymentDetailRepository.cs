using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Domain.Payment.Repositories
{
    public interface IPaymentDetailRepository
    {
        Task<PaymentDetail?> FindActiveForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
