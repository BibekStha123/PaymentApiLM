using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.Currency;
using PaymentDetailApi.Application.Currency.Queries;

namespace PaymentDetailApi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<CursorPagedResponse<CurrencyResponse>>> Get([FromQuery] int? cursor, [FromQuery] int limit = 10)
        {
            var result = await _mediator.Send(new GetAllCurrencyQuery(cursor, limit));
            return Ok(result);
        }
    }
}
