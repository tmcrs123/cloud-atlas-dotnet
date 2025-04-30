using Cloud_Atlas_Dotnet.Domain.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Application.Filters
{
    public class RequestBodyRedactionFilter : IActionFilter
    {
        private readonly ILogger<RequestBodyRedactionFilter> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RequestBodyRedactionFilter(ILogger<RequestBodyRedactionFilter> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            if (_webHostEnvironment.IsProduction())
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
            };
        }
    }
}
