using Cloud_Atlas_Dotnet.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class AccountController : BaseController
    {
        public string DbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";

        [HttpPost]
        public async Task<IResult> VerifyAccount(VerifyAccountCommand request)
        {
            if (!request.IsVerified)
            {
                return Results.Ok();
            }

            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "update accounts set verified = @verified where user_id=@userid";

                cmd.Parameters.AddWithValue("userid", request.UserId);
                cmd.Parameters.AddWithValue("verified", request.IsVerified);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return Results.Ok(rowsAffected);
            }
        }
    }
}
