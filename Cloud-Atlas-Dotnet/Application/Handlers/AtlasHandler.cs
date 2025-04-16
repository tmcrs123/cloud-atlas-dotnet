using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class CreateAtlasHandler : IRequestHandler<CreateAtlasCommand, Result<CreateAtlasCommandResponse>>
    {
        public async Task<Result<CreateAtlasCommandResponse>> Handle(CreateAtlasCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateAtlasHandler : IRequestHandler<UpdateAtlasCommand, Result<UpdateAtlasCommandResponse>>
    {
        public async Task<Result<UpdateAtlasCommandResponse>> Handle(UpdateAtlasCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class GetAtlasHandler : IRequestHandler<GetAtlasCommand, Result<GetAtlasCommandResponse>>
    {
        public async Task<Result<GetAtlasCommandResponse>> Handle(GetAtlasCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteAtlasHandler : IRequestHandler<DeleteAtlasCommand, Result<DeleteAtlasCommandResponse>>
    {
        public async Task<Result<DeleteAtlasCommandResponse>> Handle(DeleteAtlasCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
