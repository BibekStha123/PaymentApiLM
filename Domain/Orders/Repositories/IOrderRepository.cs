using PaymentDetailApi.Domain.Orders.Entities;

namespace PaymentDetailApi.Domain.Orders.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
    }
}
