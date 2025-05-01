namespace Cloud_Atlas_Dotnet.Application.Logging
{
    public class CustomRequestScope
    {
        public string CorrelationId { get; set; }
        public string? Endpoint { get; set; }
    }
}
