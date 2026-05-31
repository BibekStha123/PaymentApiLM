using MediatR;

namespace PaymentDetailApi.Application.Common
{
    public interface ICommand<TResponse> : IRequest<TResponse> { }
}
