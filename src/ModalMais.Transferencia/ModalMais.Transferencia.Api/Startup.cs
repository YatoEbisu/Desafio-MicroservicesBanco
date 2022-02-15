using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModalMais.Transferencia.Api.Config;
using ModalMais.Transferencia.Api.Data;

namespace ModalMais.Transferencia.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDiConfig(Configuration);
            services.AddDbContext<TransferenciaPixContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("TransferenciasDB")));

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("RedisDB");
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Version = "v1",
                    Title = "API ModalMais.Transferência",
                    Description = "Implementação de microsserviços do Banco ModalMais - Transferência",
                    Contact = new()
                    {
                        Name = "Marcos Felipe, Breno Fortunato, Pedro Calado, Iury Ferreira, Higor Buiatti",
                        Email = string.Empty,
                        Url = new("https://github.com/Vaivoa/Turma-01-Desafio-3-Equipe-03")
                    },
                    License = new()
                    {
                        Name = "Apache Licence 2.0",
                        Url = new("https://github.com/Vaivoa/Turma-01-Desafio-3-Equipe-03/blob/main/LICENSE")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModalMais.Transferencia.Api v1");
                c.RoutePrefix = string.Empty;
                c.EnableFilter();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}