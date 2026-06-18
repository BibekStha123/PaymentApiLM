using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Products.Commands;

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
        // GET: api/<ProductsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
