using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.Products;
using PaymentDetailApi.Application.Products.Commands;
using PaymentDetailApi.Application.Products.Queries;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentDetailApi.API.Controllers.Products
{
    [Authorize]
    [Route("api/v1/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<CursorPagedResponse<ProductResponse>>> Get([FromQuery] int? cursor, [FromQuery] int limit = 10)
        {
            var result = await _mediator.Send(new GetAllProductQuery(cursor, limit));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] ProductRequest request)
        {
            var command = new CreateProductCommand(
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Stock,
                    request.CategoryId,
                    request.IsActive
                );

            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // PUT api/<ProductsController>/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<int>> Patch(int id, [FromBody] AddStockRequest request)
        {
            var command = new AddStockCommand(id, request.stock);

            var result = await _mediator.Send(command);

            return result;
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
