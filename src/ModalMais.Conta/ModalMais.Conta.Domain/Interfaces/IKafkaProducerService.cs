using System.Threading.Tasks;

namespace ModalMais.Conta.Domain.Interfaces
{
    public interface IKafkaProducerService
    {
        Task EnviarMensagem(string cpf);
    }
}