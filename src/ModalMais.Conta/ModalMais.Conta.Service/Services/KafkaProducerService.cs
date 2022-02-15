using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using ModalMais.Conta.Domain.Interfaces;

namespace ModalMais.Conta.Service.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _producer;

        public KafkaProducerService(ProducerConfig producer)
        {
            _producer = producer;
        }

        public async Task EnviarMensagem(string cpf)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producer).Build())
            {
                await producer.ProduceAsync("CADASTRO_CONTA_CORRENTE_ATUALIZADO",
                    new() { Value = $"Dados do cliente {cpf} atualizados" });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}