using Cloud_Atlas_Dotnet.Domain.Attributes;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class CreateUserCommand : IRequest<Result<CreateUserCommandResponse>>
    {
        [SensitiveData]
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [SensitiveData]
        public string Password { get; set; }
    }

    public class CreateUserCommandValidator : FluentValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Password).NotNull().DifferentFrom("aa");
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Email).MinLength(10).MustBeEmailFormat();
        }
    }

    public class CreateUserCommandResponse
    {
        public Guid UserId { get; set; }
    }

    public class GetUserCommand : IRequest<Result<GetUserCommandResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserCommandResponse {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public Guid Id { get; set; }
    }

    public class DeleteUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class UpdateUserCommand : IRequest<Result>
    {
        public string Password { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateUserCommandResponse { };
}
