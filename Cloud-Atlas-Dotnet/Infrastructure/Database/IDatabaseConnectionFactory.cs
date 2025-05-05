using Cloud_Atlas_Dotnet.Application.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public interface IDatabaseConnectionFactory {
        IAppDbConnection CreateConnection();
    }

    public interface IAppDbConnection : IDbConnection
    {
        public new DbCommand CreateCommand();
        public Task<DbTransaction> BeginTransactionAsync();
    }

    public abstract class AppDbTransaction : DbTransaction {
        public abstract Task CommitAsync();
    }

    public abstract class AppDbCommand : DbCommand
    {
        public abstract Task<object> ExecuteScalarAsync();
        public abstract Task<int> ExecuteNonQueryAsync();
        public abstract Task<IAppDataReader> ExecuteReaderAsync();
    }

    public interface IAppDataReader : IDataReader
    {
        bool HasRows { get; }

        public Task<bool> ReadAsync();

    }
}
