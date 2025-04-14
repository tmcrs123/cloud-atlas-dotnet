using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class TestController : BaseController
    {
        public string DbConnection = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";
        
        [HttpPost]
        [Route("/user/create")]
        public async Task<IResult> CreateUser()
        {
            
            return Results.Ok("bananas");
        }

        [HttpGet]
        [Route("/user/read")]
        public async Task<IResult> GetUser()
        {

            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("/user/update")]
        public async Task<IResult> UpdateUser()
        {

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("/user/delete")]
        public async Task<IResult> DeleteUser()
        {

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("/accounts/verify")]
        public async Task<IResult> VerifyAccount()
        {

            return Results.Ok("bananas");
        }

        [HttpPost]
        [Route("/atlas/create")]
        public async Task<IResult> CreateAtlas()
        {

            return Results.Ok("bananas");
        }

        [HttpGet]
        [Route("/atlas/read")]
        public async Task<IResult> GetAtlas()
        {

            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("/atlas/update")]
        public async Task<IResult> UpdateAtlas()
        {

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("/atlas/delete")]
        public async Task<IResult> DeleteAtlas()
        {

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("/images/create")]
        public async Task<IResult> AddImageToAtlas()
        {

            return Results.Ok("bananas");
        }

        [HttpGet]
        [Route("/images/read")]
        public async Task<IResult> GetImagesForAtlas()
        {

            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("/images/update")]
        public async Task<IResult> UpdateImageDetails()
        {

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("/images/delete")]
        public async Task<IResult> DeleteImage()
        {

            throw new NotImplementedException();
        }
    }
}
