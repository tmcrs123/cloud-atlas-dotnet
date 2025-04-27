using Cloud_Atlas_Dotnet.Domain.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Application.Filters
{
    public class RequestBodyRedactionFilter : IActionFilter
    {
        private readonly ILogger<RequestBodyRedactionFilter> _logger;

        public RequestBodyRedactionFilter(ILogger<RequestBodyRedactionFilter> logger)
        {
            _logger = logger;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionArguments.TryGetValue("request", out var requestBody);

            if (requestBody is null) return;
            
            var jsonNode = JsonSerializer.SerializeToNode(requestBody, new JsonSerializerOptions() { WriteIndented = true});

            foreach (var property in requestBody.GetType().GetProperties())
            {
                if(property.GetCustomAttribute<SensitiveDataAttribute>() is not null)
                {
                    jsonNode[property.Name] = new string('*', 5);
                }
            }

            _logger.LogInformation(jsonNode.ToJsonString());
        }
    }
}
