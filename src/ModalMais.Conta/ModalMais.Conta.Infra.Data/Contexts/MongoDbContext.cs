using System;
using System.Security.Authentication;
using MongoDB.Driver;

namespace ModalMais.Conta.Infra.Data.Contexts
{
    public class MongoDbContext
    {
        public MongoDbContext(string connectionString, string databaseName, bool isSSL = false)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            IsSSL = isSSL;

            try
            {
                var settings = MongoClientSettings.FromUrl(new(ConnectionString));
                if (IsSSL)
                    settings.SslSettings = new()
                        { EnabledSslProtocols = SslProtocols.Tls12 };

                var mongoClient = new MongoClient(settings);

                _database = mongoClient.GetDatabase(DatabaseName);
            }
            catch (Exception ex)
            {
                throw new("Não foi possível se conectar com o servidor.", ex);
            }
        }

        protected string ConnectionString { get; set; }
        protected string DatabaseName { get; set; }
        protected bool IsSSL { get; set; }
        internal IMongoDatabase _database { get; set; }
    }
}