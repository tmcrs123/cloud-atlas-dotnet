namespace Cloud_Atlas_Dotnet.Application.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers.Append("CorrelationId", Guid.NewGuid().ToString());
            await _next(context);
        }
    }
}
