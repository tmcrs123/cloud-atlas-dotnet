using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using MediatorLibrary;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public class Repository : IRepository
    {
        public string DbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";

        public async Task<IResult> CreateUser(CreateUserCommand request)
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

                if (!reader.HasRows) return Results.NotFound();

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

        public async Task<IResult> UpdateUser(UpdateUserCommand request)
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

        public async Task<IResult> DeleteUser(DeleteUserCommand request)
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

        public async Task<IResult> CreateAtlas(CreateAtlasCommand request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            connection.Open();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                using var insertAtlasCommand = connection.CreateCommand();

                insertAtlasCommand.Transaction = transaction;
                insertAtlasCommand.CommandText = "insert into atlas (title) values (@title) RETURNING atlas_id";
                insertAtlasCommand.Parameters.AddWithValue("title", request.Title);

                var atlasId = await insertAtlasCommand.ExecuteScalarAsync();

                if (atlasId is null) throw new Exception("Failed to add atlas");

                using var insertOwnerCommand = connection.CreateCommand();

                insertOwnerCommand.Transaction = transaction;
                insertOwnerCommand.CommandText = "insert into owners (owner_id, atlas_id) values (@owner_id, @atlas_id)";
                insertOwnerCommand.Parameters.AddWithValue("owner_id", request.UserId);
                insertOwnerCommand.Parameters.AddWithValue("atlas_id", (Guid)atlasId);

                await insertOwnerCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return Results.Ok(atlasId);
            }
        }

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
        public async Task<IResult> UpdateAtlas(UpdateAtlasCommand request)
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

        public async Task<IResult> DeleteAtlas(DeleteAtlasCommand request)
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

        public async Task<IResult> AddImageToAtlas(CreateImageCommand request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "INSERT_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("image_url", request.ImageUri.ToString());
                cmd.Parameters.AddWithValue("legend", request.Legend);
                cmd.Parameters.AddWithValue("id_of_atlas", request.AtlasId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return Results.Ok(rowsAffected);
            }
        }

        public async Task<IResult> GetImagesForAtlas([FromQuery] GetImagesForAtlasCommand request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);
            List<Image> images = new();

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "SELECT jsonb_array_elements(image_details) FROM IMAGES WHERE ATLAS_ID=@atlas_id";

                cmd.Parameters.AddWithValue("atlas_id", request.AtlasId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader);
                    var img = JsonSerializer.Deserialize<Image>(reader.GetString(0));
                    images.Add(img);
                }

                return Results.Ok(images);
            }
        }

        public async Task<IResult> UpdateImageDetails(UpdateImageCommand request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("legend", request.Legend);
                cmd.Parameters.AddWithValue("id_of_atlas", request.AtlasId);
                cmd.Parameters.AddWithValue("image_id", request.ImageId.ToString());

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return Results.Ok(rowsAffected);
            }
        }

        public async Task<IResult> DeleteImage(DeleteImageCommand request)
        {
            var connection = new NpgsqlConnection(DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("id_of_atlas", request.AtlasId);
                cmd.Parameters.AddWithValue("image_id", request.ImageId.ToString());

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return Results.Ok(rowsAffected);
            }
        }
    }
}
