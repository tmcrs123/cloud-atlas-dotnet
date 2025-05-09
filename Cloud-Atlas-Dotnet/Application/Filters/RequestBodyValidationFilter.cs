﻿using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Domain.Services;
using Cloud_Atlas_Dotnet.Libraries.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloud_Atlas_Dotnet.Application.Filters
{
    public class RequestBodyValidationFilter : IActionFilter
    {
        private readonly IValidationService ValidationService;
        public RequestBodyValidationFilter(IValidationService validationService, IServiceProvider serviceProvider)
        {
            ValidationService = validationService;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
             var success = context.ActionArguments.TryGetValue("request", out var boundRequestModel);

            if (boundRequestModel is null || !success) return;

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
