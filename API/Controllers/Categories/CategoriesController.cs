using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Categories;
using PaymentDetailApi.Application.Categories.Commands;
using PaymentDetailApi.Application.Categories.Queries;
using PaymentDetailApi.Application.Common;

namespace PaymentDetailApi.API.Controllers.Categories
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<CursorPagedResponse<CategoryResponse>>> Get([FromQuery] Guid? cursor, [FromQuery] int limit = 50)
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery(cursor, limit));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CategoryRequest request)
        {
            var command = new CreateCategoryCommand(request.Name, request.Type);

            var categoryId = await _mediator.Send(command);

            return Ok(categoryId);
        }
    }
}
