using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cloud_Atlas_Dotnet.Domain.Patterns
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4
    }
    
    public class ApplicationError
    {
        public ErrorType ErrorType;
        public ProblemDetails ProblemDetails;

        public ApplicationError(ErrorType errorType, IDictionary<string,object?>? problemDetailsExtensions)
        {
            ErrorType = errorType;

            ProblemDetails = new ProblemDetails();

            if(problemDetailsExtensions is not null) ProblemDetails.Extensions = problemDetailsExtensions;

            switch (errorType)
            {

                case ErrorType.Validation:
                    ProblemDetails.Title = "Validation Error";
                    ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    ProblemDetails.Status = (int?)HttpStatusCode.BadRequest;

                    break;
                case ErrorType.NotFound:
                    ProblemDetails.Title =  "Not Found Error";
                    ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    ProblemDetails.Status = (int?)HttpStatusCode.NotFound;
                    break;

                case ErrorType.Conflict:
                    ProblemDetails.Title = "Conflict";
                    ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                    ProblemDetails.Status = (int?)HttpStatusCode.Conflict;
                    break;
                case ErrorType.Unauthorized:
                    ProblemDetails.Title = "Unauthorized";
                    ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    ProblemDetails.Status = (int?)HttpStatusCode.Unauthorized;
                    break;
                default:
                    ProblemDetails.Title = "Server Error";
                    ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                    ProblemDetails.Status = (int?)HttpStatusCode.InternalServerError;
                    break;
            }

        }


        // needs to be serializable
        public ProblemDetails test()
        {
            return new ProblemDetails()
            {
                Detail = "what is this aboout",
                Extensions = null, //this is the list of errors
                Instance = "dafuq",
                Status = 400,
                Title = "some title",
                Type = "this is the url"
            };
        }
    }
}
