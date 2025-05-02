using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;
using Microsoft.Extensions.Caching.Hybrid;

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
        private readonly HybridCache _cache;

        public GetImagesForAtlasHandler(IServiceScopeFactory serviceScopeFactory, HybridCache cache)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
        }

        public async Task<Result<GetImagesForAtlasCommandResponse>> Handle(GetImagesForAtlasCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var images = await _cache.GetOrCreateAsync("images_" + request.AtlasId, async token => {
                return await repository.GetImagesForAtlas(request.AtlasId);
            });

            return new Result<GetImagesForAtlasCommandResponse>(new GetImagesForAtlasCommandResponse() { Images = images }, true, null);
        }
    }

    public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateImageHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var images = await repository.UpdateImageDetails(request.AtlasId, request.ImageId, request.Legend);

            return new Result(true, null);
        }
    }

    public class DeleteImageHandler : IRequestHandler<DeleteImageCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HybridCache _cache;

        public DeleteImageHandler(IServiceScopeFactory serviceScopeFactory, HybridCache cache)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
        }

        public async Task<Result> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var images = await repository.DeleteImage(request.AtlasId, request.ImageId);

            await _cache.RemoveAsync("images_" + request.AtlasId);

            return new Result(true, null);
        }
    }
}
