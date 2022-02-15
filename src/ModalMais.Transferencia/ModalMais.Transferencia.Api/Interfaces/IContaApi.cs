using System.Threading.Tasks;
using ModalMais.Transferencia.Api.DTOs;
using Refit;

namespace ModalMais.Transferencia.Api.Interfaces
{
    public interface IContaApi
    {
        [Post("/api/contascorrente/obter-conta-pela-chave")]
        Task<ContaPixResponse> ObterContaPix(ContaPixRequest model);
    }
}