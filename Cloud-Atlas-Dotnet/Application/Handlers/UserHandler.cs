using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<CreateUserCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreateUserHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<CreateUserCommandResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var usernameExists = await repository.UsernameExists(request.Username);

            if (usernameExists)
            {
                return Result<CreateUserCommandResponse>.Failure(new ApplicationError(ErrorType.Conflict, null, "Username already exists"));
            }

            var userId = await repository.CreateUser(request.Name, request.Username, request.Email, request.Password);

            return new Result<CreateUserCommandResponse>(new CreateUserCommandResponse() { UserId = userId }, true, null);
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

    public class GetUserHandler : IRequestHandler<GetUserCommand, Result<GetUserCommandResponse>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetUserHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<GetUserCommandResponse>> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var user = await repository.GetUser(request.Id);

            return new Result<GetUserCommandResponse>(new GetUserCommandResponse() {Email = user.Email, Id = user.Id, Name = user.Name, Username = user.Username }, true, null);
        }
    }
}
