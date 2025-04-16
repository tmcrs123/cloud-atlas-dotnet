using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class AtlasController : BaseController
    {
        public string DbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";

        [HttpPost]
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

        [HttpGet]
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

        [HttpDelete]
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
    }
}
