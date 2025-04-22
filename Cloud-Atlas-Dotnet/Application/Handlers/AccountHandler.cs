using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class VerifyAccountHandler : IRequestHandler<VerifyAccountCommand, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public VerifyAccountHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var verified = await repository.VerifyAccount(request.UserId);

            if (!verified)
            {
                return Result.Failure(new ApplicationError(ErrorType.Failure, null, "Failed to verify account"));
            }

            return new Result(true, null);
        }
    }
}
