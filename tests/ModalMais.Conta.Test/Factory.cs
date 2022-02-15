using System;
using Microsoft.Extensions.Configuration;
using ModalMais.Conta.Infra.Data.Contexts;
using MongoDB.Driver;

namespace ModalMais.Conta.Test
{
    public static class Factory
    {
        public static MongoClient DbClient { get; private set; }
        private static string DatabaseName { get; set; }

        public static MongoDbContext CreateContext()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration.GetSection("MongoConnection:ConnectionString").Value;
            var isSSL = Convert.ToBoolean(configuration.GetSection("MongoConnection:IsSSL").Value);
            DatabaseName = $"test_db_{Guid.NewGuid()}";

            var context = new MongoDbContext(connectionString, DatabaseName, isSSL);
            DbClient = new(connectionString);
            return context;
        }

        public static void DropDatabase()
        {
            DbClient.DropDatabase(DatabaseName);
        }
    }
}