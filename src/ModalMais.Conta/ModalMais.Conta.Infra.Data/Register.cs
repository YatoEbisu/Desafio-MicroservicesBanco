using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Infra.Data.Contexts;
using ModalMais.Conta.Infra.Data.Repositories;

namespace ModalMais.Conta.Infra.Data
{
    public static class Register
    {
        public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
        {
            //Mongo
            var connectionString = configuration.GetSection("MongoConnection:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoConnection:Database").Value;
            var isSSL = Convert.ToBoolean(configuration.GetSection("MongoConnection:IsSSL").Value);
            services.AddSingleton<MongoDbContext>(_ => new(connectionString, databaseName, isSSL));
            services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();

            //Kafka
            var producerConfig = new ProducerConfig();
            configuration.Bind("Producer", producerConfig);
            services.AddSingleton(producerConfig);
        }
    }
}