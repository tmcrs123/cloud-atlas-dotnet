using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public class Repository : IRepository
    {
        private readonly IOptions<AppSettings> _settings;
        public Repository(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        public async Task<Guid> CreateUser(string name, string username, string email, string password)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "insert into users (name, username,email,password) values (@name, @username,@email,@password) returning Id";

                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("password", password);

                var result = await cmd.ExecuteScalarAsync();

                if (result is Guid newUserId)
                {
                    return newUserId;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to convert value {result} to a valid Guid when creating new user");
                }
            }
        }

        public async Task<bool> UsernameExists(string username)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "select COUNT(id) from users where username=@username";

                cmd.Parameters.AddWithValue("username", username);

                var reader = await cmd.ExecuteReaderAsync();

                int count = -1;

                if (!reader.HasRows) return false;

                while (await reader.ReadAsync())
                {
                    count = reader.GetInt32(0);
                }

                return count == 1;
            }
        }

        public async Task<User> GetUser(Guid id)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "select id, name, username, email from users where id = @id";

                cmd.Parameters.AddWithValue("id", id);

                var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows) return null;

                User user = new User();

                while (await reader.ReadAsync())
                {
                    user.Id = reader.GetGuid(reader.GetOrdinal("id"));
                    user.Name = reader.GetString(reader.GetOrdinal("name"));
                    user.Username = reader.GetString(reader.GetOrdinal("username"));
                    user.Email = reader.GetString(reader.GetOrdinal("email"));
                }

                return user;
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "select id, name, username, email, password from users where username = @username";

                cmd.Parameters.AddWithValue("username", username);

                var reader = await cmd.ExecuteReaderAsync();

                User user = new User();

                while (await reader.ReadAsync())
                {
                    user.Id = reader.GetGuid(reader.GetOrdinal("id"));
                    user.Name = reader.GetString(reader.GetOrdinal("name"));
                    user.Username = reader.GetString(reader.GetOrdinal("username"));
                    user.Email = reader.GetString(reader.GetOrdinal("email"));
                    user.Password = reader.GetString(reader.GetOrdinal("password"));
                }

                return user;
            }
        }

        public async Task<bool> UpdateUser(string password, Guid id)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE users set password=@password WHERE Id=@id";

                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("id", id);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected == 1;
            }
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE FROM users WHERE Id=@id";

                cmd.Parameters.AddWithValue("id", id);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected == 1;
            }
        }

        public async Task<bool> VerifyAccount(Guid userId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE accounts set verified=true WHERE user_id=@userId";

                cmd.Parameters.AddWithValue("userId", userId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected == 1;
            }
        }

        public async Task<Atlas> CreateAtlas(string title, Guid userId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            connection.Open();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                using var insertAtlasCommand = connection.CreateCommand();

                insertAtlasCommand.Transaction = transaction;
                insertAtlasCommand.CommandText = "insert into atlas (title) values (@title) RETURNING atlas_id";
                insertAtlasCommand.Parameters.AddWithValue("title", title);

                var atlasId = await insertAtlasCommand.ExecuteScalarAsync();

                if (atlasId is null) throw new Exception("Failed to add atlas");

                using var insertOwnerCommand = connection.CreateCommand();

                insertOwnerCommand.Transaction = transaction;
                insertOwnerCommand.CommandText = "insert into owners (owner_id, atlas_id) values (@owner_id, @atlas_id)";
                insertOwnerCommand.Parameters.AddWithValue("owner_id", userId);
                insertOwnerCommand.Parameters.AddWithValue("atlas_id", (Guid)atlasId);

                await insertOwnerCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return new Atlas() { Id = (Guid)atlasId, OwnerId = userId, Title = title };
            }
        }

        public async Task<List<Atlas>> GetAtlasForUser(Guid userId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                List<Atlas> atlasList = new();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = @"
                select a.atlas_id, title from atlas a
                inner join owners o on o.atlas_id = a.atlas_id
                where o.owner_id = @owner_id
                ";

                cmd.Parameters.AddWithValue("owner_id", userId);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    atlasList.Add(new Atlas
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("atlas_id")),
                        OwnerId = userId,
                        Title = reader.GetString(reader.GetOrdinal("title"))
                    });
                }

                return atlasList;
            }
        }
        public async Task<bool> UpdateAtlas(Guid atlasId, string title)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE atlas set title=@title WHERE atlas_id=@atlas_id";

                cmd.Parameters.AddWithValue("atlas_id", atlasId);
                cmd.Parameters.AddWithValue("title", title);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected == 1;
            }
        }

        public async Task<bool> DeleteAtlas(Guid atlasId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE FROM atlas WHERE atlas_id=@atlas_id";

                cmd.Parameters.AddWithValue("atlas_id", atlasId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected == 1;
            }
        }

        public async Task<bool> AddImageToAtlas(Guid atlasId, string legend, Uri imageUri)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "INSERT_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("id_of_atlas", atlasId);
                cmd.Parameters.AddWithValue("legend", legend);
                cmd.Parameters.AddWithValue("image_url", imageUri.ToString());

                var affectedRowsParam = new NpgsqlParameter("affected_rows", DbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(affectedRowsParam);

                await cmd.ExecuteNonQueryAsync(); //change sproc to return number of rows affected

                var affectedRows = affectedRowsParam.Value;

                if (affectedRows is null) return false;

                return (int)affectedRows == 1;
            }
        }

        public async Task<List<Image>> GetImagesForAtlas(Guid atlasId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);
            List<Image> images = new();

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "SELECT jsonb_array_elements(image_details) FROM IMAGES WHERE ATLAS_ID=@atlas_id";

                cmd.Parameters.AddWithValue("atlas_id", atlasId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader);
                    var img = JsonSerializer.Deserialize<Image>(reader.GetString(0));
                    if(img is not null) images.Add(img);
                }

                return images;
            }
        }

        public async Task<bool> UpdateImageDetails(Guid atlasId, Guid imageId, string legend)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "UPDATE_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("id_of_atlas", atlasId);
                cmd.Parameters.AddWithValue("image_id", imageId.ToString());
                cmd.Parameters.AddWithValue("legend", legend);

                var affectedRowsParam = new NpgsqlParameter("affected_rows", DbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(affectedRowsParam);

                await cmd.ExecuteNonQueryAsync();

                if (affectedRowsParam.Value is null) return false;

                int affectedRows = (int)affectedRowsParam.Value;

                return affectedRows == 1;
            }
        }

        public async Task<bool> DeleteImage(Guid atlasId, Guid imageId)
        {
            var connection = new NpgsqlConnection(_settings.Value.DbConnectionString);

            using (connection)
            {
                connection.Open();

                using var cmd = connection.CreateCommand();

                cmd.CommandText = "DELETE_IMAGE";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("id_of_atlas", atlasId);
                cmd.Parameters.AddWithValue("image_id", imageId.ToString());

                var affectedRowsParam = new NpgsqlParameter("affected_rows", DbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(affectedRowsParam);

                await cmd.ExecuteNonQueryAsync();

                if (affectedRowsParam.Value is null) return false;

                int affectedRows = (int)affectedRowsParam.Value;

                return affectedRows == 1;
            }
        }
    }
}