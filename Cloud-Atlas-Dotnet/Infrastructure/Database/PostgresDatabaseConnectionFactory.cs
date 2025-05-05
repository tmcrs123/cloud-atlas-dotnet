using Cloud_Atlas_Dotnet.Application.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public class PgSqlConnection : IAppDbConnection
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly NpgsqlConnection connection;

        public PgSqlConnection(IOptions<AppSettings> settings)
        {
            _settings = settings;
            connection = new NpgsqlConnection(_settings.Value.DbConnectionString);
        }

        string IDbConnection.ConnectionString { 
            get => _settings.Value.DbConnectionString; 
            set => throw new InvalidOperationException();
        }

        int IDbConnection.ConnectionTimeout => 30000;

        string IDbConnection.Database => connection.Database;

        ConnectionState IDbConnection.State => connection.State;

        public async Task<DbTransaction> BeginTransactionAsync()
        {
            return await connection.BeginTransactionAsync();
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this.connection.BeginTransaction();
        }

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return this.connection.BeginTransaction();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        void IDbConnection.Close()
        {
            connection.Close();
        }

        DbCommand IAppDbConnection.CreateCommand()
        {
            return connection.CreateCommand();
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            return connection.CreateCommand();
        }

        void IDisposable.Dispose()
        {
            connection.Dispose();
        }

        void IDbConnection.Open()
        {
            connection.Open();
        }
    }

    public class PostgresDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IOptions<AppSettings> _settings;
        private PgSqlConnection _connection;

        public PostgresDatabaseConnectionFactory(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        public IAppDbConnection CreateConnection()
        {
            _connection = new PgSqlConnection(_settings);
            return _connection;
        }
    }
}
