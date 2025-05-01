using Cloud_Atlas_Dotnet.Application.Auth;
using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Application.Exceptions;
using Cloud_Atlas_Dotnet.Application.Filters;
using Cloud_Atlas_Dotnet.Application.Handlers;
using Cloud_Atlas_Dotnet.Application.Logging;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Domain.Services;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation.Interfaces;
using MediatorLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            builder.Services.AddTransient<IRequestHandler<GetUserCommand, Result<GetUserCommandResponse>>, GetUserHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteUserCommand, Result>, DeleteUserHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateUserCommand, Result>, UpdateUserHandler>();
            builder.Services.AddTransient<IRequestHandler<SignInUserCommand, Result<SignInUserResponse>>, SignInUserHandler>();

            builder.Services.AddTransient<IRequestHandler<VerifyAccountCommand, Result>, VerifyAccountHandler>();

            builder.Services.AddTransient<IRequestHandler<CreateAtlasCommand, Result<CreateAtlasCommandResponse>>, CreateAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<GetAtlasForUserCommand, Result<GetAtlasForUserCommandResponse>>, GetAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateAtlasCommand, Result<UpdateAtlasCommandResponse>>, UpdateAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteAtlasCommand, Result>, DeleteAtlasHandler>();

            builder.Services.AddTransient<IRequestHandler<CreateImageCommand, Result<CreateImageCommandResponse>>, CreateImageHandler>();
            builder.Services.AddTransient<IRequestHandler<UpdateImageCommand, Result>, UpdateImageHandler>();
            builder.Services.AddTransient<IRequestHandler<GetImagesForAtlasCommand, Result<GetImagesForAtlasCommandResponse>>, GetImagesForAtlasHandler>();
            builder.Services.AddTransient<IRequestHandler<DeleteImageCommand, Result>, DeleteImageHandler>();
        }

        public static void ConfigureInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRepository, Repository>();
        }

        public static void ConfigureValidations(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IValidationService, ValidationService>();

            // Bind & Validate AppSettings
            builder.Services.AddOptions<AppSettings>().BindConfiguration("AppSettings").ValidateOnStart();
            builder.Services.AddSingleton<IValidateOptions<AppSettings>>(s => new AppSettingsValidator());
            
            builder.Services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

            builder.Services.AddScoped<RequestBodyValidationFilter>();
            builder.Services.AddScoped<RequestBodyRedactionFilter>();
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            builder.Logging.AddFilter("Microsoft", LogLevel.None);
            builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.None);
            builder.Logging.AddFilter("System", LogLevel.None);

            builder.Logging.AddConsole(options =>
            {
                options.FormatterName = "CustomConsoleFormatter";
            }).AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>(options =>
            {
                options.IncludeScopes = true;
            });
        }

        public static void ConfigureGlobalErrorHandling(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        }

        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPasswordService, PasswordService>();
            builder.Services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();

            builder.Services.AddSingleton<IAuthService, AuthService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["AuthToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JwtSecretKey"])),
                    ClockSkew = TimeSpan.Zero                    
                };
            });
        }
    }
}
