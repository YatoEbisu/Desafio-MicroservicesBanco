using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModalMais.Transferencia.Api.DTOs;

namespace ModalMais.Transferencia.Api.Controllers.Interfaces
{
    public interface ITransferenciasController
    {
        /// <summary>
        ///     Realiza uma transferência pix
        /// </summary>
        /// <remarks>
        ///     Restrições de request:
        ///     O valor deve ser menor que 5.000.
        ///     O limite diário por chave é de 100.000,00.
        ///     A chave deve ser válida
        ///     Um tipo deve ser informado
        /// </remarks>
        /// <param name="model">Dados da transferência.</param>
        /// <returns>A transferência realizada</returns>
        /// <response code="201">Transferência concluída com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Nenhuma conta encontrada para a chave especificada.</response>
        /// <response code="500">Erro no servidor.</response>
        public Task<IActionResult> Adicionar(TransferenciaPixRequest model);
    }
}