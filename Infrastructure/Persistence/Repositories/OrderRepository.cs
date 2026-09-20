using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Domain.Orders.Repositories;

namespace PaymentDetailApi.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PaymentDetailsContext _context;

        public OrderRepository(PaymentDetailsContext context)
        {
            _context = context;
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
            await _context.Orders.AddAsync(order, cancellationToken);
    }
}
