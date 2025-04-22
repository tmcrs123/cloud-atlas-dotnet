using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class VerifyAccountCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
    }
}
