using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.Orders;
using PaymentDetailApi.Application.Orders.Commands;
using PaymentDetailApi.Application.Orders.Queries;
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

            string? idempotencyKey = Request.Headers["Idempotency-Key"];

            var command = new CreateOrderCommand(
                userId,
                request.ShippingAddress,
                request.CurrencyId,
                request.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)).ToList(),
                idempotencyKey
            );

            return Ok(await _mediator.Send(command));
        }
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CursorPagedResponse<OrderResponse>>> Get([FromQuery] Guid? cursor, [FromQuery] int limit = 10)
        {
            var result = await _mediator.Send(new GetAllOrderQuery(cursor, limit));
            return Ok(result);
        }
        [HttpGet("my-orders")]
        public async Task<ActionResult<CursorPagedResponse<OrderResponse>>> GetMine([FromQuery] Guid? cursor, [FromQuery] int limit = 10)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetOrdersByUserIdQuery(userId, cursor, limit));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetById(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetOrderByIdQuery(id, userId, User.IsInRole("Admin")));
            return Ok(result);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _mediator.Send(new CancelOrderCommand(id, userId, User.IsInRole("Admin")));
            return NoContent();
        }
    }
}
