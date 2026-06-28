using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Categories.Commands
{
    public record CreateCategoryCommand(string Name, string Type) : ICommand<Guid>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly PaymentDetailsContext _context;

        public CreateCategoryCommandHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = Category.Create(request.Name, request.Type);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
