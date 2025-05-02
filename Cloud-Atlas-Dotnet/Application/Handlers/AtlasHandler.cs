using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Application.Services;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class CreateAtlasHandler : IRequestHandler<CreateAtlasCommand, Result<CreateAtlasCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreateAtlasHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<CreateAtlasCommandResponse>> Handle(CreateAtlasCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var newAtlas = await repository.CreateAtlas(request.Title, request.UserId);

            if (newAtlas is null)
            {
                return Result<CreateAtlasCommandResponse>.Failure(new ApplicationError(ErrorType.Failure, null, "Failed to create new atlas"));
            }

            return new Result<CreateAtlasCommandResponse>(new CreateAtlasCommandResponse() { Url = new Uri($"http://localhost:5099/api/atlas/{newAtlas.Id}") }, true, null);
        }
    }

    public class UpdateAtlasHandler : IRequestHandler<UpdateAtlasCommand, Result<UpdateAtlasCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateAtlasHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<UpdateAtlasCommandResponse>> Handle(UpdateAtlasCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var success = await repository.UpdateAtlas(request.AtlasId, request.Title);

            if (!success)
            {
                return Result<UpdateAtlasCommandResponse>.Failure(new ApplicationError(ErrorType.Failure, null, "Failed to update atlas"));
            }

            return new Result<UpdateAtlasCommandResponse>(new UpdateAtlasCommandResponse() { Url = new Uri($"http://localhost:5099/api/atlas/{request.AtlasId}") }, true, null);
        }
    }

    public class GetAtlasHandler : IRequestHandler<GetAtlasForUserCommand, Result<GetAtlasForUserCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetAtlasHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<GetAtlasForUserCommandResponse>> Handle(GetAtlasForUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var atlasList = await repository.GetAtlasForUser(request.UserId);

            return new Result<GetAtlasForUserCommandResponse>(new GetAtlasForUserCommandResponse() { AtlasList = atlasList }, true, null);
        }
    }

    public class DeleteAtlasHandler : IRequestHandler<DeleteAtlasCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DeleteAtlasHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result> Handle(DeleteAtlasCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var atlasList = await repository.DeleteAtlas(request.AtlasId);

            return new Result(true, null);
        }
    }

    public class GeocodeAtlasHandler : IRequestHandler<GeocodeAtlasCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IGeocodingService _geocodingService;

        public GeocodeAtlasHandler(IServiceScopeFactory serviceScopeFactory, IGeocodingService geocodingService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _geocodingService = geocodingService;
        }

        public async Task<Result> Handle(GeocodeAtlasCommand request, CancellationToken cancellationToken)
        {
            Coordinates coordinates = await _geocodingService.GeocodeAtlas(request.AtlasId, request.PlaceIdentifier);

            if (coordinates is null)
            {
                return new Result(false, new ApplicationError(ErrorType.Failure,
                    new Dictionary<string, object?>()
                    {
                        ["Location"] = request.PlaceIdentifier
                    },
                    "Failed to retrieve coordinates for location provided"));
            }

            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var updated = await repository.AddCoordinatesToAtlas(request.AtlasId, coordinates);

            if(updated) return new Result(true, null);
            else return new Result(false, new ApplicationError(ErrorType.NotFound, null, "Atlas was not found"));
        }
    }
}
