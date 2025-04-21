using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Services;
using MediatorLibrary;
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
            throw new NotImplementedException();

        }

        [HttpDelete]
        public async Task<IResult> DeleteUser(DeleteUserCommand request)
        {
            throw new NotImplementedException();

        }
    }
}
