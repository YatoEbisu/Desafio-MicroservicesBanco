using System.Threading.Tasks;
using ModalMais.Transferencia.Api.DTOs;

namespace ModalMais.Transferencia.Api.Interfaces
{
    public interface IContaCorrenteService
    {
        Task<ContaPixResponse> ObterContaPix(ContaPixRequest model);
    }
}