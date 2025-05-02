using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using Microsoft.Extensions.Options;

namespace Cloud_Atlas_Dotnet.Application.Configuration
{
    public class AppSettings
    {
        public string DbConnectionString { get; set; }
        public string JwtSecretKey { get; set; }
        public string GeocodingApiKey { get; set; }
    }

    public class AppSettingsValidator : FluentValidator<AppSettings>, IValidateOptions<AppSettings>
    {
        public AppSettingsValidator()
        {
            RuleFor(x => x.DbConnectionString).NotNull().NotEmpty();
            RuleFor(x => x.JwtSecretKey).NotNull().NotEmpty();
            RuleFor(x => x.GeocodingApiKey).NotNull().NotEmpty();
        }

        ValidateOptionsResult IValidateOptions<AppSettings>.Validate(string? name, AppSettings options)
        {
            var validationOutcome = Validate(options);

            if(validationOutcome is null)
            {
                throw new InvalidOperationException("Validation of app settings returned a null validation outcome");
            }

            if (!validationOutcome.Valid) return ValidateOptionsResult.Fail(validationOutcome.PrintAllFailures());

            return ValidateOptionsResult.Success;
        }
    }
}
