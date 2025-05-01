using Cloud_Atlas_Dotnet.Application.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Application.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            //if an exception is thrown logger scope is lost so I need to fetch the correlation id from the headers and begin a new scope
            var correlationIdExists = httpContext.Request.Headers.TryGetValue("CorrelationId", out var correlationId);

            using (_logger.BeginScope(new CustomRequestScope() { CorrelationId = correlationId, Endpoint = httpContext.Request.Path }))
            {
                _logger.LogError(exception, exception.Message);

                var unknownErrorResponse = new ProblemDetails()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unknown error has occurred",
                    Extensions = new Dictionary<string, object?>()
                    {
                        ["CorrelationId"] = correlationId
                    }
                };
            
                await httpContext.Response.WriteAsJsonAsync(unknownErrorResponse, cancellationToken);
            }
            
            return true;
        }
    }
}
