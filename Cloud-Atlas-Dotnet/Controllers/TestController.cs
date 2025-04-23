using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections;
using System.Data;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class TestController : BaseController
    {
        [HttpGet]
        public IResult Test()
        {
            var aBigList = Enumerable.Range(0, 1000).Select(x =>
            {
                return new Image()
                { Id = Guid.NewGuid(), Legend = "whjatever", Url = new Uri("http://banana.com") };
            });
            return Results.Ok(aBigList);
        }
      
  

    }
}
