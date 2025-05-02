using Cloud_Atlas_Dotnet.Application.Commands;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class AtlasController : BaseController
    {
        private readonly IMediator _mediator;

        public AtlasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IResult> CreateAtlas(CreateAtlasCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Created();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpGet]
        public async Task<IResult> GetAtlasForUser(GetAtlasForUserCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value.AtlasList);
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpPut]
        public async Task<IResult> UpdateAtlas(UpdateAtlasCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpDelete]
        public async Task<IResult> DeleteAtlas(DeleteAtlasCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpPost]
        [Route("geocode")]
        public async Task<IResult> AddCoordinatesToAtlas(GeocodeAtlasCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok();
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }
    }
}
