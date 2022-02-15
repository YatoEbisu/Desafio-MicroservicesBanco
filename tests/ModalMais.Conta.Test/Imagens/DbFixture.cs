using System;
using Microsoft.Extensions.Configuration;
using ModalMais.Conta.Infra.Data.Contexts;
using MongoDB.Driver;
using Xunit;

namespace ModalMais.Conta.Test.Imagens
{
    public class DbFixtureCollection : ICollectionFixture<DbFixture>
    {
    }

    public class DbFixture : IDisposable

    {
        public DbFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            DatabaseName = $"test_db_{Guid.NewGuid()}";
            ConnectionString = configuration.GetSection("MongoConnection:ConnectionString").Value;
            var isSSL = Convert.ToBoolean(configuration.GetSection("MongoConnection:IsSSL").Value);

            DbContext = new(ConnectionString, DatabaseName, isSSL);
            DbClient = new(ConnectionString);
        }

        public MongoDbContext DbContext { get; }
        private MongoClient DbClient { get; }
        internal string DatabaseName { get; }
        internal string ConnectionString { get; }

        public void Dispose()
        {
            DbClient.DropDatabase(DatabaseName);
        }
    }
}