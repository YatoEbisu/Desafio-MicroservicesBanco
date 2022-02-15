using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModalMais.Transferencia.Api;
using ModalMais.Transferencia.Api.Data;

namespace ModalMais.Transferencia.Test
{
    public class Factory
    {
        internal readonly WebApplicationFactory<Startup> factory;

        internal Factory()
        {
            var (context, _) = CreateContext();

            factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<TransferenciaPixContext>();
                    services.AddScoped(_ => context);
                });
            });
        }

        internal static (TransferenciaPixContext, DapperContext) CreateContext()
        {
            var connection = new SqliteConnection("Datasource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TransferenciaPixContext>()
                .UseSqlite(connection)
                .Options;

            var context = new TransferenciaPixContext(options);
            context.Database.EnsureCreated();
            return (context, new(connection));
        }

        internal static DapperContext CreateDapperContext()
        {
            var connection = new SqliteConnection("Datasource=./db.sqlite3");
            connection.Open();

            var options = new DbContextOptionsBuilder<TransferenciaPixContext>()
                .UseSqlite(connection)
                .Options;

            var context = new TransferenciaPixContext(options);
            context.Database.EnsureCreated();
            return new(connection);
        }
    }
}