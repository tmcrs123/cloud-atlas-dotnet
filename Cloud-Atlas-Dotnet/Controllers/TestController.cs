using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class TestController : BaseController
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IResult Test()
        {
            HttpContext context = HttpContext;
            var aBigList = Enumerable.Range(0, 1000).Select(x =>
            {
                return new Image()
                { Id = Guid.NewGuid(), Legend = "whatever", Url = new Uri("http://banana.com") };
            });

            return Results.Ok(aBigList);
        }
    }
}
