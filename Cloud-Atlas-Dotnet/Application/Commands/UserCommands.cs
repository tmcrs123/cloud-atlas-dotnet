using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Libraries;
using MediatorLibrary;
using System.Xml.Linq;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateUserCommand : IRequest<Result<CreateUserCommandResponse>>
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Whatever { get; set; }
    }

    public class CreateUserCommandValidator : SecondBaseValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Whatever).NotNull().NumberLessThanTarget(10).Even();
            RuleFor(x => x.Name).NotNull().Not("bananas");
        }
    }

    public class CreateUserCommandResponse
    {
        public Guid UserId { get; set; }
    }

    public class DeleteUserCommand : IRequest<Result<DeleteUserCommandResponse>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandResponse { }

    public class UpdateUserCommand : IRequest<Result<UpdateUserCommandResponse>>
    {
        public string Password { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateUserCommandResponse { };
}
