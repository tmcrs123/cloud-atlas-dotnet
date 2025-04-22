using Cloud_Atlas_Dotnet.Domain.Entities;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateAtlasCommand : IRequest<Result<CreateAtlasCommandResponse>>
    {
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateAtlasCommandResponse {
        public Uri Url { get; set; }
    }

    public class GetAtlasForUserCommand : IRequest<Result<GetAtlasForUserCommandResponse>>
    {
        public Guid UserId { get; set; }
    }

    public class GetAtlasForUserCommandResponse {
        public List<Atlas> AtlasList { get; set; }

    };

    public class UpdateAtlasCommand : IRequest<Result<UpdateAtlasCommandResponse>>
    {
        public string Title { get; set; }
        public Guid AtlasId { get; set; }
    }

    public class UpdateAtlasCommandResponse {
        public Uri Url { get; set; }
    }

    public class DeleteAtlasCommand : IRequest<Result>
    {
        public Guid AtlasId { get; set; }
    }
}
