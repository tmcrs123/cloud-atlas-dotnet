using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cloud_Atlas_Dotnet.Application.Middleware
{

    public class CustomRequestScope() {
        public string CorrelationId { get; set; }
        public string Endpoint { get; set; }
    }

    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggerMiddleware> _logger;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var hasCorrelationId = context.Request.Headers.TryGetValue("CorrelationId", out var correlationId);
            var endpoint = context.Request.Path;
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (hasCorrelationId)
            {
                using (_logger.BeginScope(new CustomRequestScope() { CorrelationId = correlationId, Endpoint = endpoint}))
                {
                    _logger.LogInformation("Started handling request. Endpoint = {Endpoint}", endpoint);
                    
                    await _next(context);
                    
                    stopwatch.Stop();

                    _logger.LogInformation("Finished handling request. Duration = {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                }
            }
            else
            {
                _logger.LogWarning("CorrelationId not present!!! {endpoint}", endpoint);
                await _next(context);
            }
        }     
    }
}
