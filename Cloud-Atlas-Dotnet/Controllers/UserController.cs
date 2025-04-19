using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Services;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IValidationService _validationService;

        public UserController(IMediator mediator, IValidationService validationService)
        {
            _mediator = mediator;
            _validationService = validationService;
        }

        [HttpPost]
        public async Task<IResult> CreateUser(CreateUserCommand request)
        {
            var validationResult = _validationService.Validate(request);
            //if (!validationResult.IsValid)
            //{
            //    JsonSerializer.Serialize(validationResult.ValidationFailures);
            //    return Results.Problem("validation problems");

            //}
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
