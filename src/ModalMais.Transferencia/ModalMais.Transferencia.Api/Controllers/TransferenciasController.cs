using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModalMais.Transferencia.Api.Controllers.Interfaces;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Interfaces;
using Notie.Contracts;
using Notie.Models;

namespace ModalMais.Transferencia.Api.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/transferencias")]
    [ApiController]
    public class TransferenciasController : ControllerBase, ITransferenciasController
    {
        private readonly IMapper _mapper;
        private readonly AbstractNotifier _notifier;
        private readonly ITransferenciaService _service;

        public TransferenciasController(AbstractNotifier notifier, IMapper mapper, ITransferenciaService service)
        {
            _notifier = notifier;
            _mapper = mapper;
            _service = service;
        }


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
        [HttpPost]
        [ProducesResponseType(typeof(TransferenciaPixResponse), 201)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 404)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 500)]
        public async Task<IActionResult> Adicionar(TransferenciaPixRequest model)
        {
            var obj = _mapper.Map<TransferenciaPixRequest, TransferenciaPix>(model);
            await _service.Transfer(obj);

            if (_notifier.Notifications.Any(n => n.Key == "Conta")) return NotFound(_notifier.Notifications);

            if (_notifier.HasNotifications)
                return BadRequest(_notifier.Notifications);

            return Created(nameof(Adicionar), _mapper.Map<TransferenciaPixResponse>(obj));
        }

        /// <summary>
        ///     Obtém o extrato de uma conta informada
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model">Dados da transferência.</param>
        /// <returns>A transferência realizada</returns>
        /// <response code="200">Transferência concluída com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Nenhuma conta encontrada.</response>
        /// <response code="500">Erro no servidor.</response>
        [HttpPost("obter-extrato")]
        [ProducesResponseType(typeof(ExtratoResponse), 200)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 404)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 500)]
        public async Task<IActionResult> ObterExtrato(ExtratoRequest model)
        {
            var obj = _mapper.Map<ExtratoRequest, Extrato>(model);
            await _service.ObterExtrato(obj);

            if (_notifier.Notifications.Any(n => n.Key == "Conta")) return NotFound(_notifier.Notifications);

            if (_notifier.HasNotifications)
                return BadRequest(_notifier.Notifications);

            return Ok(_mapper.Map<ExtratoResponse>(obj));
        }
    }
}