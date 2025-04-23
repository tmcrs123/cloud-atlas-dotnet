using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class ImageController : BaseController
    {
        private readonly IMediator _mediator;

        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IResult> AddImageToAtlas(CreateImageCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Created(response.Value.Url, null);
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpGet]
        public async Task<IResult> GetImagesForAtlas([FromQuery] GetImagesForAtlasCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value.Images);
            }
            else
            {
                return Results.Problem(response.Error!.ProblemDetails); //this is safe, we check for this in Result class ctor
            }
        }

        [HttpPut]
        public async Task<IResult> UpdateImageDetails(UpdateImageCommand request)
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

        [HttpDelete]
        public async Task<IResult> DeleteImage(DeleteImageCommand request)
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
