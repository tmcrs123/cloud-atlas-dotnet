using Cloud_Atlas_Dotnet.Application.Commands;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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

             return Results.Created();
        }

        [HttpGet]
        public async Task<IResult> GetUser([FromQuery] Guid id)
        {
            throw new NotImplementedException();
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
