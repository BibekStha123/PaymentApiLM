using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Orders.Commands;
using System.Security.Claims;

namespace PaymentDetailApi.API.Controllers.Orders
{
    [Authorize]
    [Route("api/v1/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] OrderRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var command = new CreateOrderCommand(
                userId,
                request.ShippingAddress,
                request.CurrencyId,
                request.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)).ToList()
            );

            return Ok(await _mediator.Send(command));
        }
    }
}
