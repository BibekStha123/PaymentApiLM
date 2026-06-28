using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.Currency;
using PaymentDetailApi.Application.Currency.Queries;

namespace PaymentDetailApi.API.Controllers.Currencies
{
    [Authorize]
    [Route("api/v1/currencies")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<CursorPagedResponse<CurrencyResponse>>> Get([FromQuery] Guid? cursor, [FromQuery] int limit = 10)
        {
            var result = await _mediator.Send(new GetAllCurrencyQuery(cursor, limit));
            return Ok(result);
        }
    }
}
