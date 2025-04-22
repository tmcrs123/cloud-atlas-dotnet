using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;

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

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateUserHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var updated = await repository.UpdateUser(request.Password, request.Id);

            if (!updated)
            {
                return Result.Failure(new ApplicationError(ErrorType.Failure, null, "Failed to update user"));
            }

            return new Result(true, null);
        }                        
    }

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DeleteUserHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var deleted = await repository.DeleteUser(request.Id);

            if (!deleted)
            {
                return Result.Failure(new ApplicationError(ErrorType.Failure, null, "Failed to delete user"));
            }

            return new Result(true, null);
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
