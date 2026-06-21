using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Products.Commands
{
    public record AddStockCommand(int Id, int Stock) : ICommand<int>;

    public class AddStockCommandHanlder : IRequestHandler<AddStockCommand, int>
    {
        private readonly PaymentDetailsContext _context;
        public AddStockCommandHanlder(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id, cancellationToken);
            if (product is null)
                throw new KeyNotFoundException($"Product with id {request.Id} not found.");

            product.AddStock(request.Stock);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Stock;
        }
    }
}
