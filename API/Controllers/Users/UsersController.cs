using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentDetailApi.Application.Users.Commands;

namespace PaymentDetailApi.API.Controllers.Users
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUserRequest request)
        {
            var command = new RegisterUserCommand(
                request.UserName,
                request.Email,
                request.Password,
                request.ConfirmPassword,
                request.DisplayName);

            var userId = await _mediator.Send(command);

            return Ok(userId);
        }
    }
}
