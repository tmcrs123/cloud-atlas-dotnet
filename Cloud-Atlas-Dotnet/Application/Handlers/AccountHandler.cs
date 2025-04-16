using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Handlers
{
    public class VerifyAccountHandler : IRequestHandler<VerifyAccountCommand, Result<VerifyCommandResponse>>
    {
        public async Task<Result<VerifyCommandResponse>> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
