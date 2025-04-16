using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<CreateImageCommandResponse>>
    {
        public Task<Result<CreateImageCommandResponse>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class GetImagesForAtlasHandler : IRequestHandler<GetImagesForAtlasCommand, Result<GetImagesForAtlasCommandResponse>>
    {
        public Task<Result<GetImagesForAtlasCommandResponse>> Handle(GetImagesForAtlasCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
