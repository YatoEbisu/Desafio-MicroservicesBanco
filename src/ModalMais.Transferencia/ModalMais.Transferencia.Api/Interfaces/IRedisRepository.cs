using System.Collections.Generic;
using System.Threading.Tasks;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.Interfaces
{
    public interface IRedisRepository
    {
        Task SetExtrato(string key, IEnumerable<TransferenciaPix> value);
        Task<List<TransferenciaPix>> GetExtrato(string key);
        Task RemoveExtrato(string key);
    }
}