using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Interfaces;
using ModalMais.Transferencia.Api.Repository;
using ModalMais.Transferencia.Api.Services;
using Moq;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using StackExchange.Redis;
using Xunit;

namespace ModalMais.Transferencia.Test.Services.Transferencia
{
    public class TransferenciaServiceIntegrationTest
    {
        private readonly IDistributedCache _distributedCache;
        private readonly AutoMocker _mocker;
        private readonly Notifier _notifier;
        public readonly IRedisRepository _redisRepository;
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly ConnectionMultiplexer redis;
        private ITransferenciaService _transferenciaService;

        public TransferenciaServiceIntegrationTest()
        {
            _mocker = new();
            _notifier = new();

            _distributedCache = new Factory().factory.Services.GetService<IDistributedCache>();
            _redisRepository = new RedisRepository(_distributedCache);
            var (context, dapper) = Factory.CreateContext();
            _transferenciaRepository = new TransferenciaRepository(context, dapper);
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
        }

        [Fact(DisplayName = "Transfer deve adicionar transferencia no banco quando os dados forem válidos")]
        public async Task Transfer_DeveAdicionarDadosNoBanco_QuandoDadosForemValidos()
        {
            // Arrange
            var pixResponse = new ContaPixResponse
            {
                NumeroBanco = "123",
                NumeroConta = "1234",
                Agencia = "0001",
                Nome = "Jose",
                Sobrenome = "Maria"
            };
            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 1000, "Pagamento boleto");
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(() => pixResponse);
            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();

            // Act
            await _transferenciaService.Transfer(transferenciaPix);
            var dadosRepo = await _transferenciaRepository.Find(t => t.Chave == "teste@teste.com");

            // Assert
            Assert.True(dadosRepo.Any());
        }

        [Fact(DisplayName = "Transfer deve retornar notificação quando os dados da transferência forem inválidos")]
        public async Task Transfer_DeveAdicionarNotificacao_QuandoDadosForemInvalidos()
        {
            // Arrange
            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste.com", 1000, "Pagamento boleto");
            var pixResponse = new ContaPixResponse
            {
                NumeroBanco = "123",
                NumeroConta = "1234",
                Agencia = "0001",
                Nome = "Jose",
                Sobrenome = "Maria"
            };
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(() => pixResponse);
            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();
            // Act
            await _transferenciaService.Transfer(transferenciaPix);

            var dadosRepo = await _transferenciaRepository.Find(t => t.Chave == "teste@teste.com");

            // Assert
            Assert.True(!dadosRepo.Any());
            Assert.True(_notifier.Notifications.Any());
        }

        [Fact(DisplayName = "Transfer deve retornar notificação quando conta não encontrada")]
        public async Task Transfer_DeveAdicionarNotificacao_QuandoContaNaoEncontrada()
        {
            // Arrange
            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 1000, "Pagamento boleto");
            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>())).ReturnsAsync(() => null);
            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();
            // Act
            await _transferenciaService.Transfer(transferenciaPix);
            var dadosRepo = await _transferenciaRepository.Find(t => t.Chave == "teste@teste.com");

            // Assert
            Assert.True(!dadosRepo.Any());
            Assert.Contains(new("Conta", "Conta não encontrada para a chave informada."),
                _notifier.Notifications);
        }

        [Fact(DisplayName = "Transfer deve retornar notificação quando valor transação maior que 5000")]
        public async Task Transfer_DeveAdicionarNotificacao_QuandoValorTransacaoMaiorQueCincoMil()
        {
            // Arrange
            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 6000, "Pagamento boleto");
            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>())).ReturnsAsync(() => null);
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();
            // Act
            await _transferenciaService.Transfer(transferenciaPix);
            var dadosRepo = await _transferenciaRepository.Find(t => t.Chave == "teste@teste.com");

            // Assert
            Assert.True(!dadosRepo.Any());
            Assert.Contains(new("Valor", "O campo Valor deve estar entre 1 e 5000."), _notifier.Notifications);
        }

        [Fact(DisplayName = "Transfer deve retornar notificação quando valor diário maior que 100k")]
        public async Task Transfer_DeveAdicionarNotificacao_QuandoValorDiarioMaiorQueCemMil()
        {
            // Arrange
            var pixResponse = new ContaPixResponse
            {
                NumeroBanco = "123",
                NumeroConta = "1234",
                Agencia = "0001",
                Nome = "Jose",
                Sobrenome = "Maria"
            };
            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(() => pixResponse);
            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();

            // Act
            for (var i = 0; i < 19; i++)
                await _transferenciaService.Transfer(new(TipoChave.Email, "teste@teste.com", 5000, "Pagamento boleto"));

            var transferenciaPix1 = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 3000, "Pagamento boleto");

            await _transferenciaService.Transfer(transferenciaPix1);

            var transferenciaPix2 = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 4000, "Pagamento boleto");
            transferenciaPix2.Finalizar();
            await _transferenciaService.Transfer(transferenciaPix2);

            var dadosRepo = await _transferenciaRepository.Find(t => t.Chave == "teste@teste.com");
            var total = (await _transferenciaRepository
                    .Find(y => y.NumeroConta == transferenciaPix2.NumeroConta &&
                               DateTime.Now.Date.Equals(transferenciaPix2.DataTransferencia.Date)))
                .Sum(y => y.Valor);

            // Assert
            Assert.True(dadosRepo.Count() == 20);
            Assert.Equal(98000, total);
            var notificationExists = _notifier.Notifications.Any(x => x.Key == "Valor");
            Assert.True(notificationExists);
        }


        [Fact(DisplayName = "ObterExtrato quando existe transfencias em cache, deve consultar essa transferencia")]
        public async Task ObterExtrato_DeveConsultarCache_QuandoExisteEmCache()
        {
            // Arrange
            var pixResponse = new ContaPixResponse
            {
                NumeroBanco = "123",
                NumeroConta = "100000",
                Agencia = "0001",
                Nome = "Jose",
                Sobrenome = "Maria"
            };

            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 1000, "Pagamento boleto");

            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            _mocker.Use(_redisRepository);

            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();

            var dataInicial = DateTime.Now.AddDays(-3);
            var dataFinal = DateTime.Now.AddDays(1);

            // Dado Existe no Redis
            var key = TransferenciaService.FormatKey(pixResponse.NumeroConta, dataInicial, dataFinal);
            var listaTransferencias = new List<TransferenciaPix> { transferenciaPix };

            await _redisRepository.SetExtrato(key, listaTransferencias);

            var extrato = new Extrato(pixResponse.NumeroConta, pixResponse.Agencia, dataInicial, dataFinal);

            // Act
            await _transferenciaService.ObterExtrato(extrato);

            // Assert
            await _redisRepository.RemoveExtrato(key); // Cleanup

            Assert.Single(extrato.TransferenciasPix);
        }

        [Fact(DisplayName = "ObterExtrato quando não existe transfencias em cache, deve consultar o banco de dados")]
        public async Task ObterExtrato_DeveConsultarBancoQUandoNaoTemCache_QuandoNaoExisteCache()
        {
            // Arrange
            var pixResponse = new ContaPixResponse
            {
                NumeroBanco = "123",
                NumeroConta = "100000",
                Agencia = "0001",
                Nome = "Jose",
                Sobrenome = "Maria",
                Chave = "email@teste.com.br"
            };

            var transferenciaPix = new TransferenciaPix(TipoChave.Email, "teste@teste.com", 1000, "Pagamento boleto");

            _mocker.Use(_transferenciaRepository);
            _mocker.Use<AbstractNotifier>(_notifier);
            _mocker.Use(_redisRepository);

            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(r => r.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(() => pixResponse);

            _transferenciaService = _mocker.CreateInstance<TransferenciaService>();

            var dataInicial = DateTime.Now.AddDays(-3);
            var dataFinal = DateTime.Now;

            await _transferenciaService.Transfer(transferenciaPix);

            var extrato = new Extrato(pixResponse.NumeroConta, pixResponse.Agencia, dataInicial, dataFinal);

            // Act
            await _transferenciaService.ObterExtrato(extrato);

            // Assert
            Assert.Single(extrato.TransferenciasPix);
        }
    }
}