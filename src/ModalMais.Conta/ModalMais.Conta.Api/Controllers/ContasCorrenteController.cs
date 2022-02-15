using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModalMais.Conta.Api.Controllers.Base;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Service.Dtos;
using ModalMais.Conta.Service.Validations;
using Notie.Contracts;
using Notie.Models;

namespace ModalMais.Conta.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/contascorrente")]
    [ApiController]
    public class ContasCorrenteController : BaseController
    {
        private readonly IContaCorrenteService _contaCorrenteService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public ContasCorrenteController(IContaCorrenteService contaCorrenteService,
            IKafkaProducerService kafkaProducerService, IMapper mapper,
            AbstractNotifier notifier) : base(mapper, notifier)
        {
            _kafkaProducerService = kafkaProducerService;
            _contaCorrenteService = contaCorrenteService;
        }

        /// <summary>
        ///     Abertura de nova Conta Corrente
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="obj">Dados do cliente</param>
        /// <returns>Request</returns>
        /// <response code="200">Não utilizado</response>
        /// <response code="201">Conta Corrente aberta com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<ContaCorrenteResponse>), 201)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        public async Task<IActionResult> Insert([FromBody] ContaCorrenteRequest obj)
        {
            var contaCorrente = _mapper.Map<ContaCorrente>(obj);
            await _contaCorrenteService.Add(contaCorrente);

            if (_notifier.HasNotifications)
                return BadRequest(_notifier.Notifications);

            return Created(nameof(Insert), _mapper.Map<ContaCorrenteResponse>(contaCorrente));
        }

        /// <summary>
        ///     Atualiza dados do cliente
        /// </summary>
        /// <remarks>
        ///     Restrições de request:
        ///     O CPF informado deve possuir uma conta cadastrada
        /// </remarks>
        /// <param name="obj">Dados do cliente</param>
        /// <returns>Request</returns>
        /// <response code="200">Não utilizado</response>
        /// <response code="204">Conta Corrente atualizada</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPut("clientes")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        public async Task<IActionResult> ClienteUpdate([FromBody] AtualizaClienteRequest obj)
        {
            var cliente = _mapper.Map<Cliente>(obj);
            await _contaCorrenteService.Update(cliente);

            if (_notifier.HasNotifications)
                return BadRequest(_notifier.Notifications);
            await _kafkaProducerService.EnviarMensagem(cliente.CPF);
            return NoContent();
        }

        /// <summary>
        ///     Envio de documento para validação e ativação da Conta Corrente
        /// </summary>
        /// <remarks>
        ///     Restrições de request:
        ///     A imagem deve ser menor que 4 MB.
        ///     A imagem deve ser no formato PNG.
        /// </remarks>
        /// <param name="imagemRequest">Dados da Conta Corrente e lista de imagens do Documento.</param>
        /// <returns>Lista de imagens válidas e a imagem ativa.</returns>
        /// <response code="200">Não utilizado.</response>
        /// <response code="201">Documento validado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="403">CPF não vinculado aos dados bancários.</response>
        [HttpPost("imagens")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ImagemResponse), 201)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 403)]
        public async Task<IActionResult> InsertImage([FromForm] ImagemRequest imagemRequest)
        {
            // validar CPF valido
            var validation = new ImagemRequestValidation();
            var result = await validation.ValidateAsync(imagemRequest);
            if (!result.IsValid)
            {
                _notifier.AddNotificationsByFluent(result);
                return BadRequest(_notifier.Notifications);
            }

            // Chama Service
            var imagens = await _contaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);

            // retornar 403 se cpf nao estiver vinculado aos dados bancarios
            if (_notifier.Notifications.Where(n => n.Key == "Img_Dados").Any())
                return StatusCode(403, _notifier.Notifications);

            // retornar 400 em caso de erro de validacao
            if (_notifier.HasNotifications) return BadRequest(_notifier.Notifications);

            // retornar 201 com imagens em caso de sucesso

            return Created(nameof(InsertImage), _mapper.Map<List<ImagemResponse>>(imagens));
        }

        /// <summary>
        ///     Registro de chave Pix
        /// </summary>
        /// <remarks>
        ///     Tipos de Chave:
        ///     E-mail = 1
        ///     CPF = 2
        ///     Celular = 3
        ///     Chave Aleatória = 4
        /// </remarks>
        /// <param name="model">Request com chave, tipo e número da conta.</param>
        /// <returns>Chave gerada.</returns>
        /// <response code="200">Não utilizado.</response>
        /// <response code="201">Chave Gerada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost("pix")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(PixResponse), 201)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        public async Task<IActionResult> InsertPix([FromBody] PixRequest model)
        {
            var validation = new PixRequestValidation();
            var result = validation.Validate(model);
            if (!result.IsValid)
            {
                _notifier.AddNotificationsByFluent(result);
                return BadRequest(_notifier.Notifications);
            }

            var pix = _mapper.Map<Pix>(model);
            await _contaCorrenteService.AddPix(model.NumeroConta, pix);

            if (_notifier.HasNotifications) return BadRequest(_notifier.Notifications);

            return Created(nameof(InsertPix), _mapper.Map<PixResponse>(pix));
        }


        /// <summary>
        ///     Obter dados pela chave Pix
        /// </summary>
        /// <param name="model">Tipo e chave pix informada</param>
        /// <returns>Conta corrente do dono da chave</returns>
        /// <response code="200">Dados do correntista</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Conta não encontrada</response>
        [ProducesResponseType(typeof(ContaPixResponse), 200)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 400)]
        [ProducesResponseType(typeof(IReadOnlyCollection<Notification>), 404)]
        [HttpPost("obter-conta-pela-chave")]
        public async Task<IActionResult> GetContaPix([FromBody] ContaPixRequest model)
        {
            var obj = _mapper.Map<ContaPixRequest, Pix>(model);
            var conta = await _contaCorrenteService.FindByPix(obj);
            if (_notifier.Notifications.Any(n => n.Key == "Conta")) return NotFound(_notifier.Notifications);
            if (_notifier.HasNotifications) return BadRequest(_notifier.Notifications);

            var result = _mapper.Map<ContaCorrente, ContaPixResponse>(conta);
            result.Chave = obj.Chave;
            return Ok(result);
        }
    }
}