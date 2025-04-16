using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Controllers
{
    public class ImageController : BaseController
    {
        public string DbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cloud-atlas-dotnet";

        [HttpPost]
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

        [HttpGet]
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

        [HttpPut]
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

        [HttpDelete]
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
