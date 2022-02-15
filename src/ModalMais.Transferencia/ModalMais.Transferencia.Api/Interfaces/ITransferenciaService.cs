using System.Threading.Tasks;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.Interfaces
{
    public interface ITransferenciaService
    {
        Task Transfer(TransferenciaPix obj);
        Task ObterExtrato(Extrato obj);
    }
}