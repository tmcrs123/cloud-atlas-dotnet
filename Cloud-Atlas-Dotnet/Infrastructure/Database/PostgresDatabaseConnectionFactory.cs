using Cloud_Atlas_Dotnet.Application.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public class PostgresDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IOptions<AppSettings> _settings;

        public PostgresDatabaseConnectionFactory(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        public T CreateConnection<T>()
        {
            if(typeof(T) == typeof(NpgsqlConnection))
            {
                return (T)(object) new NpgsqlConnection(_settings.Value.DbConnectionString);
            }
            throw new InvalidOperationException("Unsupported database engine");
        }
    }
}
