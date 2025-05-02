using System.Data;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public interface IDatabaseConnectionFactory
    {
        T CreateConnection<T>();
    }
}
