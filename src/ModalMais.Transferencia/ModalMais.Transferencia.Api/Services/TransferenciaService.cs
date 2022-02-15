using System;
using System.Linq;
using System.Threading.Tasks;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Entities.Validations;
using ModalMais.Transferencia.Api.Interfaces;
using Notie.Contracts;

namespace ModalMais.Transferencia.Api.Services
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly IContaCorrenteService _contaCorrenteService;
        private readonly AbstractNotifier _notifier;
        private readonly IRedisRepository _redisRepository;
        private readonly ITransferenciaRepository _transferenciaRepository;

        public TransferenciaService(IContaCorrenteService contaCorrenteService,
            ITransferenciaRepository transferenciaRepository, AbstractNotifier notifier,
            IRedisRepository redisRepository)
        {
            _contaCorrenteService = contaCorrenteService;
            _transferenciaRepository = transferenciaRepository;
            _redisRepository = redisRepository;
            _notifier = notifier;
        }

        public async Task Transfer(TransferenciaPix obj)
        {
            var validation = new TransferenciaPixValidation();
            var result = await validation.ValidateAsync(obj);

            if (!result.IsValid)
            {
                _notifier.AddNotificationsByFluent(result);
                return;
            }

            var pix = new ContaPixRequest { Chave = obj.Chave, Tipo = (int)obj.TipoChave };
            var conta = await _contaCorrenteService.ObterContaPix(pix);

            if (conta == null)
            {
                _notifier.AddNotification(new("Conta", "Conta não encontrada para a chave informada."));
                return;
            }

            obj.NumeroBanco = conta.NumeroBanco;
            obj.NumeroConta = conta.NumeroConta;
            obj.Agencia = conta.Agencia;
            obj.Nome = conta.Nome;
            obj.Sobrenome = conta.Sobrenome;

            var total = await _transferenciaRepository.ObterTotalDiarioPorConta(obj.NumeroConta);

            if (total + obj.Valor <= TransferenciaPix.LIMITE_DIARIO)
            {
                obj.Finalizar();
                await _transferenciaRepository.Add(obj);
                return;
            }

            var limiteDisponivel = TransferenciaPix.LIMITE_DIARIO - total;
            var mensagem = "O valor ultrapassa o limite de 100.000,00 diário";
            mensagem += limiteDisponivel > 0 ? $"Limite diário disponivel {limiteDisponivel}." : ".";
            _notifier.AddNotification(new("Valor", mensagem));
        }

        public async Task ObterExtrato(Extrato obj)
        {
            var validation = new ExtratoValidation();
            var result = await validation.ValidateAsync(obj);
            if (!result.IsValid)
            {
                _notifier.AddNotificationsByFluent(result);
                return;
            }

            var dateExist = obj.DataInicial != null && obj.DataFinal != null;
            var isToday = obj.DataFinal != null && obj.DataFinal.Value.Date == DateTime.Now.Date;

            if (!dateExist) obj.AdicionarPeriodo(DateTime.Now.AddDays(-3), DateTime.Now);
            
            var key = FormatKey(obj.NumeroConta, obj.DataInicial.Value, obj.DataFinal.Value);

            if (!isToday) obj.AdicionarTransferencias( await _redisRepository.GetExtrato(key));
          
            if (obj.TransferenciasPix.Count != 0) return;

            var transferencias = await _transferenciaRepository.ObterTransferencias(obj.NumeroConta, obj.NumeroAgencia, obj.DataInicial, obj.DataFinal);
            
            obj.AdicionarTransferencias(transferencias);

            if (obj.TransferenciasPix.Any())
                await _redisRepository.SetExtrato(
                    FormatKey(obj.NumeroConta, obj.DataInicial.Value, obj.DataFinal.Value), transferencias);
        }

        public static string FormatKey(string conta, DateTime dataInicial, DateTime dataFinal)
        {
            return $"{conta}:{dataInicial:yyyy/MM/dd}:{dataFinal:yyyy/MM/dd}";
        }
    }
}