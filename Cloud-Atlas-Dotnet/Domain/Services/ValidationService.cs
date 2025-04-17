using Cloud_Atlas_Dotnet.Libraries;

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
            var validator = _serviceProvider.GetService<ISecondValidator<T>>();

            if(validator is null)
                {
                return new ValidationOutcome(){};
            }

            return validator.Validate(instance);
        }
    }
}
