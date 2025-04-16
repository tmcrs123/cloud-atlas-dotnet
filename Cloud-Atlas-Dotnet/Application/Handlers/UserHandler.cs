using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<CreateUserCommandResponse>>
    {
        private readonly IRepository _repository;

        public CreateUserHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CreateUserCommandResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserCommandResponse>>
    {
        public async Task<Result<UpdateUserCommandResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result<DeleteUserCommandResponse>>
    {
        public async Task<Result<DeleteUserCommandResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
