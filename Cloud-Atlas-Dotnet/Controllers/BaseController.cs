using Cloud_Atlas_Dotnet.Application.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    [ApiController]
    [ServiceFilter<RequestBodyValidationFilter>]
    [ServiceFilter<RequestBodyRedactionFilter>]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseController : Controller
    {
    }
}
