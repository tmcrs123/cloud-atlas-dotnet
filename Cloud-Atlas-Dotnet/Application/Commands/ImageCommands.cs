using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateImageCommand : IRequest<Result<CreateImageCommandResponse>>
    {
        public Guid AtlasId { get; set; }
        public Uri ImageUri { get; set; }
        public string Legend { get; set; }
    }

    public class CreateImageCommandResponse { }


    public class GetImagesForAtlasCommand : IRequest<Result<GetImagesForAtlasCommandResponse>>
    {
        public Guid AtlasId { get; set; }
    }

    public class GetImagesForAtlasCommandResponse { }

    public class UpdateImageCommand : IRequest<Result<UpdateImageCommandResponse>>
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
        public string Legend { get; set; }
    }

    public class UpdateImageCommandResponse { }

    public class DeleteImageCommand : IRequest<Result<DeleteImageCommandResponse>>
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
    }

    public class DeleteImageCommandResponse { }
}
