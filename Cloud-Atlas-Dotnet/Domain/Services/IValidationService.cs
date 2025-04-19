using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation.Interfaces;

namespace Cloud_Atlas_Dotnet.Domain.Services
{
    public interface IValidationService
    {
        public ValidationOutcome Validate<T>(T instance);
    }

    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ValidationOutcome Validate<T>(T instance)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if(validator is null)
                {
                return new ValidationOutcome(){};
            }

            return validator.Validate(instance);
        }
    }
}
