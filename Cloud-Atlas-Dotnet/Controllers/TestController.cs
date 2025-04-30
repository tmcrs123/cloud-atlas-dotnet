using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class TestController : BaseController
    {
        private readonly ILogger<TestController> _logger;
        public IOptions<AppSettings> _appSettings { get; set; }

        public TestController(ILogger<TestController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
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
