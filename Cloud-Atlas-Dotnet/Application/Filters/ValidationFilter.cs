using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Domain.Services;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloud_Atlas_Dotnet.Application.Filters
{
    public class ValidationFilter : IActionFilter
    {
        private readonly IValidationService ValidationService;
        public ValidationFilter(IValidationService validationService, IServiceProvider serviceProvider)
        {
            ValidationService = validationService;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var boundRequestModel = context.ActionArguments["request"];

            if (boundRequestModel is null) return;

            Type t = boundRequestModel.GetType();

            var serviceMethod = typeof(IValidationService).GetMethod("Validate");

            if(serviceMethod is null)
            {
                throw new InvalidOperationException("Validation method not found");
            }

            var typedMethod = serviceMethod.MakeGenericMethod(t);

            //I know for a fact there's a Validate method on the ValidationService so it's safe to !
            ValidationOutcome validationOutcome = (ValidationOutcome) typedMethod.Invoke(ValidationService, new object[] { boundRequestModel })!;

            if (!validationOutcome.Valid)
            {
                ApplicationError error = new ApplicationError(ErrorType.Validation, validationOutcome.ValidationFailures.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value));

                context.Result = new BadRequestObjectResult(error.ProblemDetails);
            }
        }
    }
}
