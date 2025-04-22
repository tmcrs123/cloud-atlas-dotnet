using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<CreateImageCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreateImageHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<CreateImageCommandResponse>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var images = await repository.AddImageToAtlas(request.AtlasId, request.Legend, request.ImageUri);

            return new Result<CreateImageCommandResponse>(new CreateImageCommandResponse() { Url = new Uri($"http://localhost:5099/api/image?atlasId={request.AtlasId}")}, true, null);
        }
    }

    public class GetImagesForAtlasHandler : IRequestHandler<GetImagesForAtlasCommand, Result<GetImagesForAtlasCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetImagesForAtlasHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<GetImagesForAtlasCommandResponse>> Handle(GetImagesForAtlasCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var images = await repository.GetImagesForAtlas(request.AtlasId);

            return new Result<GetImagesForAtlasCommandResponse>(new GetImagesForAtlasCommandResponse() { Images = images }, true, null);
        }
    }

    public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<UpdateImageCommandResponse>>
    {
        public Task<Result<UpdateImageCommandResponse>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteImageHandler : IRequestHandler<DeleteImageCommand, Result<DeleteImageCommandResponse>>
    {
        public Task<Result<DeleteImageCommandResponse>> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
