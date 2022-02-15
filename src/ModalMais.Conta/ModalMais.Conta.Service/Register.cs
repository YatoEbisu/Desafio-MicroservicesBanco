using Microsoft.Extensions.DependencyInjection;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Service.Services;
using Notie;
using Notie.Contracts;

namespace ModalMais.Conta.Service
{
    public static class Register
    {
        public static void AddService(this IServiceCollection services)
        {
            services.AddScoped<IContaCorrenteService, ContaCorrenteService>();
            services.AddScoped<AbstractNotifier, Notifier>();
            services.AddScoped<IKafkaProducerService, KafkaProducerService>();
        }
    }
}