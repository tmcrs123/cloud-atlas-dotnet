﻿using Cloud_Atlas_Dotnet.Domain.Patterns;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet.Application.Commands
{
    public class VerifyAccountCommand : IRequest<Result<VerifyCommandResponse>>
    {
        public bool IsVerified { get; set; }
        public Guid UserId { get; set; }
    }

    public class VerifyCommandResponse
    {

    }
}
