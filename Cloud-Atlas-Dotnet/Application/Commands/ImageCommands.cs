using Cloud_Atlas_Dotnet.Domain.Entities;
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

    public class CreateImageCommandResponse {
        public Uri Url { get; set; }
    }


    public class GetImagesForAtlasCommand : IRequest<Result<GetImagesForAtlasCommandResponse>>
    {
        public Guid AtlasId { get; set; }
    }

    public class GetImagesForAtlasCommandResponse {
        public List<Image> Images { get; set; }
    }

    public class UpdateImageCommand : IRequest<Result>
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
        public string Legend { get; set; }
    }

    public class DeleteImageCommand : IRequest<Result>
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
    }
}
