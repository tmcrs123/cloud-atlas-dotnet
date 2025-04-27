using Cloud_Atlas_Dotnet.Application.Middleware;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Cloud_Atlas_Dotnet.Application.Logging
{
    public class CustomConsoleFormatter : ConsoleFormatter
    {
        public CustomConsoleFormatter() : base("CustomConsoleFormatter")
        {
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            var logLevel = logEntry.LogLevel;
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            var exception = logEntry.Exception;
            var correlationId = string.Empty;

            textWriter.WriteLine($"[{DateTime.UtcNow:f}] [{logLevel}]");

            scopeProvider?.ForEachScope((scope, writer) =>
            {
                if(scope is CustomRequestScope)
                {
                    correlationId = ((CustomRequestScope)scope).CorrelationId;
                }

            }, textWriter);

            if (exception != null)
            {
                textWriter.WriteLine($"Exception: {exception.Message}");
                textWriter.WriteLine($"Stack Trace: {exception.StackTrace}");
            } else
            {
                textWriter.WriteLine($"{correlationId}: {message}");
            }

           textWriter.WriteLine();
        }
    }
}
