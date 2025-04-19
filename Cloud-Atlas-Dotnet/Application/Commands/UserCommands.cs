using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateUserCommand : IRequest<Result<CreateUserCommandResponse>>
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserCommandValidator : FluentValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Password).NotNull().DifferentFrom("aa");
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
