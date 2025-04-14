using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class TestController : BaseController
    {
        public string DbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";

        [HttpPost]
        [Route("user/create")]
        public async Task<IResult> CreateUser(CreateUserRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection) 
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "insert into users (name, username,email,password) values (@name, @username,@email,@password)";

                cmd.Parameters.AddWithValue("name", request.Name);
                cmd.Parameters.AddWithValue("username", request.Username);
                cmd.Parameters.AddWithValue("email", request.Email);
                cmd.Parameters.AddWithValue("password", request.Password);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return Results.Ok(rowsAffected);
            }
        }

        [HttpGet]
        [Route("/user/read")]
        public async Task<IResult> GetUser([FromQuery] Guid id)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "select id, name, username, email from users where id = @id";

                cmd.Parameters.AddWithValue("id", id);

                var reader = await cmd.ExecuteReaderAsync();

                if(!reader.HasRows) return Results.NotFound();

                User user = new User();

                while (await reader.ReadAsync())
                {
                    user.Id = reader.GetGuid(reader.GetOrdinal("id"));
                    user.Name = reader.GetString(reader.GetOrdinal("name"));
                    user.Username = reader.GetString(reader.GetOrdinal("username"));
                    user.Email = reader.GetString(reader.GetOrdinal("email"));
                }

                return Results.Ok(user);
            }
        }

        [HttpPut]
        [Route("/user/update")]
        public async Task<IResult> UpdateUser(UpdateUserRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE users set password=@password WHERE ID=@id";

                cmd.Parameters.AddWithValue("password", request.Password);
                cmd.Parameters.AddWithValue("id", request.Id);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return Results.NoContent();
            }
        }

        [HttpDelete]
        [Route("/user/delete")]
        public async Task<IResult> DeleteUser(DeleteUserRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE FROM users WHERE ID=@id";

                cmd.Parameters.AddWithValue("id", request.Id);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return Results.Ok(rowsAffected);
            }
        }

        [HttpPost]
        [Route("/accounts/verify")]
        public async Task<IResult> VerifyAccount(VerifyAccountRequest request)
        {
            if(!request.IsVerified)
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

        [HttpPost]
        [Route("/atlas/create")]
        public async Task<IResult> CreateAtlas(CreateAtlasRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);
            
            connection.Open();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                using var insertAtlasCommand = connection.CreateCommand();

                insertAtlasCommand.Transaction = transaction;
                insertAtlasCommand.CommandText = "insert into atlas (title) values (@title) RETURNING atlas_id";
                insertAtlasCommand.Parameters.AddWithValue("title", request.Title);

                var atlasId =  await insertAtlasCommand.ExecuteScalarAsync();

                if (atlasId is null) throw new Exception("Failed to add atlas");

                using var insertOwnerCommand = connection.CreateCommand();

                insertOwnerCommand.Transaction = transaction;
                insertOwnerCommand.CommandText = "insert into owners (owner_id, atlas_id) values (@owner_id, @atlas_id)";
                insertOwnerCommand.Parameters.AddWithValue("owner_id", request.UserId);
                insertOwnerCommand.Parameters.AddWithValue("atlas_id", (Guid) atlasId);

                await insertOwnerCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return Results.Ok(atlasId);
            }
        }

        [HttpGet]
        [Route("/atlas/read")]
        public async Task<IResult> GetAtlasForUser(Guid userId)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = @"
                select a.atlas_id, title from atlas a
                inner join owners o on o.atlas_id = a.atlas_id
                where o.owner_id = @owner_id
                ";

                cmd.Parameters.AddWithValue("owner_id", userId);

                var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows) return Results.NotFound();

                List<Atlas> atlasList = new();

                while (await reader.ReadAsync())
                {
                    atlasList.Add(new Atlas
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("atlas_id")),
                        OwnerId = userId,
                        Title = reader.GetString(reader.GetOrdinal("title"))
                    });
                }

                return Results.Ok(atlasList);
            }
        }

        [HttpPut]
        [Route("/atlas/update")]
        public async Task<IResult> UpdateAtlas(UpdateAtlasRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE atlas set title=@title WHERE atlas_id=@atlas_id";

                cmd.Parameters.AddWithValue("title", request.Title);
                cmd.Parameters.AddWithValue("atlas_id", request.AtlasId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return Results.NoContent();
            }
        }

        [HttpDelete]
        [Route("/atlas/delete")]
        public async Task<IResult> DeleteAtlas(DeleteAtlasRequest request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE FROM atlas WHERE atlas_id=@atlas_id";

                cmd.Parameters.AddWithValue("atlas_id", request.AtlasId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                
                return Results.Ok(rowsAffected);
            }
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
