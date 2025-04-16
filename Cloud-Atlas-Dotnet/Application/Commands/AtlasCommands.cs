using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateAtlasCommand : IRequest<Result<CreateAtlasCommandResponse>>
    {
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateAtlasCommandResponse { }

    public class GetAtlasCommand : IRequest<Result<GetAtlasCommandResponse>>
    {
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetAtlasCommandResponse { };

    public class UpdateAtlasCommand : IRequest<Result<UpdateAtlasCommandResponse>>
    {
        public string Title { get; set; }
        public Guid AtlasId { get; set; }
    }

    public class UpdateAtlasCommandResponse { }

    public class DeleteAtlasCommand : IRequest<Result<DeleteAtlasCommandResponse>>
    {
        public Guid AtlasId { get; set; }
    }

    public class DeleteAtlasCommandResponse { }
}
