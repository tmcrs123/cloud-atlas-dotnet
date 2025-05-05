using Cloud_Atlas_Dotnet.Infrastructure.Database;
using Moq;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Atlas_Dotnet_Tests
{
    public class FakeCommand : DbCommand
    {
        private string _commandText;
        public override string CommandText { get => _commandText; set => _commandText = value; }
        public override int CommandTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override CommandType CommandType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool DesignTimeVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override UpdateRowSource UpdatedRowSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override DbConnection? DbConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override DbParameterCollection DbParameterCollection => new NpgsqlCommand().Parameters;

        protected override DbTransaction? DbTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object? ExecuteScalar()
        {
            return new Guid();
        }

        public Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object?>(new Guid());
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }
    }
 
    public class RepositoryTests
    {
        [Fact]
        public async Task CreateUser_Ok()
        {
            //arrange
            var mockDbConnection = new Mock<IAppDbConnection>();
            mockDbConnection.Setup(m => m.Open());
            mockDbConnection.Setup(m => m.CreateCommand()).Returns(() => new FakeCommand());

            var dbConnectionFactoryMock = new Mock<IDatabaseConnectionFactory>();
            dbConnectionFactoryMock.Setup(x => x.CreateConnection())
                .Returns(() => mockDbConnection.Object);

            Repository repo = new Repository(dbConnectionFactoryMock.Object);

            //act
            Guid id = await repo.CreateUser("whatever", "whatever", "whatever", "whatever");

            //assert
            Assert.False(String.IsNullOrEmpty(id.ToString()));
        }
    }
}
