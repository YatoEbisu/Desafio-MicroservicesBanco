using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.Interfaces
{
    public interface ITransferenciaRepository
    {
        Task Add(TransferenciaPix obj);
        Task<List<TransferenciaPix>> Find(Expression<Func<TransferenciaPix, bool>> predicate);

        Task<List<TransferenciaPix>> ObterTransferencias(string numeroConta, string agencia,
            DateTime? dataInicial = null, DateTime? dataFinal = null);

        Task<decimal> ObterTotalDiarioPorConta(string conta);
    }
}