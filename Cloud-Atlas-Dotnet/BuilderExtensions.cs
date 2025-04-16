using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Application.Handlers;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using MediatorLibrary;

namespace Cloud_Atlas_Dotnet
{
    public static class BuilderExtensions
    {
        public static void ConfigureMediator(this WebApplicationBuilder builder)
        {
            //general mediator serivce
            builder.Services.AddSingleton<IMediator, Mediator>();

            //register handlers
            builder.Services.AddTransient<IRequestHandler<CreateUserCommand, Result<CreateUserCommandResponse>>, CreateUserHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteUserCommand, Result<DeleteUserCommandResponse>>, DeleteUserHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateUserCommand, Result<UpdateUserCommandResponse>>, UpdateUserHandler>();

            builder.Services.AddTransient<IRequestHandler<VerifyAccountCommand, Result<VerifyCommandResponse>>, VerifyAccountHandler>();

            builder.Services.AddTransient<IRequestHandler<CreateAtlasCommand, Result<CreateAtlasCommandResponse>>, CreateAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<GetAtlasCommand, Result<GetAtlasCommandResponse>>, GetAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateAtlasCommand, Result<UpdateAtlasCommandResponse>>, UpdateAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteAtlasCommand, Result<DeleteAtlasCommandResponse>>, DeleteAtlasHandler>();

            builder.Services.AddTransient<IRequestHandler<CreateImageCommand, Result<CreateImageCommandResponse>>, CreateImageHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateImageCommand, Result<UpdateImageCommandResponse>>, UpdateImageHandler>();
            builder.Services.AddTransient<IRequestHandler<GetImagesForAtlasCommand, Result<GetImagesForAtlasCommandResponse>>, GetImagesForAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteImageCommand, Result<DeleteImageCommandResponse>>, DeleteImageHandler>();
        }

        public static void ConfigureDependencyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRepository, Repository>();
        }
    }
}
