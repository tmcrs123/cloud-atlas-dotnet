using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Services;
using MediatorLibrary;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/sign-up")]
        public async Task<IResult> CreateUser(CreateUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Created();
            } else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpGet]
        public async Task<IResult> GetUser([FromQuery] GetUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpPut]
        public async Task<IResult> UpdateUser(UpdateUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }

        }

        [HttpDelete]
        public async Task<IResult> DeleteUser(DeleteUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpPost]
        [Route("/sign-in")]
        public async Task<IResult> SignInUser(SignInUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                HttpContext.Response.Cookies.Append("AuthToken", response.Value.JwtToken, new CookieOptions() { HttpOnly = true});
                return Results.NoContent();
            } else
            {
                return Results.Problem(response.Error!.ProblemDetails);
            }

        }
    }
}
