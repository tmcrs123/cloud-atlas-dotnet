using Cloud_Atlas_Dotnet.Application.Commands;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IResult> VerifyAccount(VerifyAccountCommand request)
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
    }
}
