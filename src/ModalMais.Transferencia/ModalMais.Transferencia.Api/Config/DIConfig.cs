using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModalMais.Transferencia.Api.Data;
using ModalMais.Transferencia.Api.Interfaces;
using ModalMais.Transferencia.Api.Repository;
using ModalMais.Transferencia.Api.Services;
using Notie;
using Notie.Contracts;
using Npgsql;
using Refit;

namespace ModalMais.Transferencia.Api.Config
{
    public static class DiConfig
    {
        public static void AddDiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
            services.AddScoped<ITransferenciaService, TransferenciaService>();

            var apiContaUrl = configuration.GetSection("Refit:ApiContaUrl").Value;
            services.AddRefitClient<IContaApi>().ConfigureHttpClient(client => client.BaseAddress = new(apiContaUrl));

            services.AddScoped<IContaCorrenteService, ContaCorrenteService>();
            services.AddScoped<ITransferenciaService, TransferenciaService>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<AbstractNotifier, Notifier>();

            services.AddScoped(_ =>
                new DapperContext(new NpgsqlConnection(configuration.GetConnectionString("TransferenciasDB"))));
        }
    }
}