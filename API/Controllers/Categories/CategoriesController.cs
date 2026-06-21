using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Categories.Commands;
using PaymentDetailApi.Application.Common;

namespace PaymentDetailApi.API.Controllers.Categories
{
    [Authorize]
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateCategory([FromBody] CategoryRequest request)
        {
            var command = new CreateCategoryCommand(request.Name, request.Type);

            var categoryId = await _mediator.Send(command);

            return Ok(categoryId);
        }
    }
}
