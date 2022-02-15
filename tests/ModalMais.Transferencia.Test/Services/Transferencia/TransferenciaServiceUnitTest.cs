using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Interfaces;
using ModalMais.Transferencia.Api.Services;
using Moq;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Transferencia.Test.Services.Transferencia
{
    public class TransferenciaServiceUnitTest
    {
        private readonly AutoMocker _mocker;
        private readonly AbstractNotifier _notifier;

        public TransferenciaServiceUnitTest()
        {
            _notifier = new Notifier();
            _mocker = new();
            _mocker.Use(_notifier);
        }


        [Fact(DisplayName = "Transfer deve retornar notificações quando os dados da transferência forem inválidos")]
        public async Task Transfer_DeveRetornarNotifications_QuandoOsDadosDaTransferenciaForemInvalidos()
        {
            //Arrange
            var transferencia = new TransferenciaPix(TipoChave.Email, "", 0, "");
            var service = _mocker.CreateInstance<TransferenciaService>();

            // Act
            await service.Transfer(transferencia);

            // Assert
            Assert.Equal(4, _notifier.Notifications.Count);
        }

        [Fact(DisplayName = "Transfer deve retornar notificações quando a conta não for encontrada")]
        public async Task Transfer_DeveRetornarNotifications_QuandoAContaNaoForEncontrada()
        {
            //Arrange
            var data = new TransferenciaPix(TipoChave.Email, new Faker().Person.Email, 1000, "Essa é uma descrição");
            var service = _mocker.CreateInstance<TransferenciaService>();

            // Act
            await service.Transfer(data);

            var notificationExists = _notifier.Notifications.Any(x => x.Key == "Conta");
            Assert.True(notificationExists);
        }

        [Fact(DisplayName = "Transfer deve retornar notificações quando o limite diário for antigido")]
        public async Task Transfer_DeveRetornarNotifications_QuandoOLimiteDiarioForAtingido()
        {
            //Arrange
            var data = new TransferenciaPix(TipoChave.Email, new Faker().Person.Email, 1000, "Essa é uma descrição");
            var service = _mocker.CreateInstance<TransferenciaService>();

            var mockTransferenciaService = _mocker.GetMock<ITransferenciaRepository>();
            mockTransferenciaService.Setup(t => t.ObterTotalDiarioPorConta(It.IsAny<string>())).ReturnsAsync(100000);

            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();
            mockContaCorrenteService.Setup(t => t.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(new ContaPixResponse());

            // Act
            await service.Transfer(data);

            var notificationExists = _notifier.Notifications.Any(x => x.Key == "Valor");
            Assert.True(notificationExists);
        }

        [Fact(DisplayName = "Transfer não deve retornar notificações quando for concluído com sucesso")]
        public async Task Transfer_NaoDeveRetornarNotifications_QuandoConcluidoComSucesso()
        {
            //Arrange
            var data = new TransferenciaPix(TipoChave.Email, new Faker().Person.Email, 1000, "Essa é uma descrição");
            var service = _mocker.CreateInstance<TransferenciaService>();

            var mockTransferenciaService = _mocker.GetMock<ITransferenciaRepository>();

            mockTransferenciaService.Setup(t => t.ObterTotalDiarioPorConta(It.IsAny<string>())).ReturnsAsync(0);

            var mockContaCorrenteService = _mocker.GetMock<IContaCorrenteService>();

            mockContaCorrenteService.Setup(t => t.ObterContaPix(It.IsAny<ContaPixRequest>()))
                .ReturnsAsync(new ContaPixResponse());

            // Act
            await service.Transfer(data);

            // Assert
            Assert.False(_notifier.HasNotifications);
        }

        [Theory]
        [InlineData(true, true, true, 3)] // 3 transferencias, no redis
        [InlineData(true, false, true, 3)] // 3 transferencias, no SQL
        [InlineData(true, false, false, 0)] // 0 transferencias em nenhum banco de dados
        [InlineData(false, true, true, 3)] // 3 transferencias, no redis
        [InlineData(false, false, true, 3)] // 3 transferencias, no SQL
        [InlineData(false, false, false, 0)] // 0 transferencias, não existem transferencias
        public async Task RequestStatement_DeveRetornarExtrato_QuandoSolicitadoComSucesso(
            bool dataExist, bool cacheExist, bool extratoExist,
            int nTransf
        )
        {
            //Arrange
            var service = _mocker.CreateInstance<TransferenciaService>();
            var listaTransferencias = new List<TransferenciaPix>();

            for (var i = 0; i < nTransf; i++)
                listaTransferencias.Add(new(TipoChave.Email, new Faker().Person.Email, 1000,
                    "Essa é uma descrição"));

            var mockRedis = _mocker.GetMock<IRedisRepository>();

            mockRedis.Setup(
                t => t.SetExtrato(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<TransferenciaPix>>()
                ));

            if (cacheExist)
                mockRedis.Setup(
                        t => t.GetExtrato(It.IsAny<string>()))
                    .ReturnsAsync(listaTransferencias);
            else
                mockRedis.Setup(
                        t => t.GetExtrato(It.IsAny<string>()))
                    .ReturnsAsync(new List<TransferenciaPix>());

            var mockRepository = _mocker.GetMock<ITransferenciaRepository>();
            if (extratoExist)
                mockRepository.Setup(
                    t => t.ObterTransferencias(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).ReturnsAsync(listaTransferencias);
            else
                mockRepository.Setup(
                    t => t.ObterTransferencias(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).ReturnsAsync(new List<TransferenciaPix>());

            var numeroAgencia = "0001";
            var numeroConta = "0000";
            DateTime? dataInicial = null;
            DateTime? dataFinal = null;

            if (dataExist)
            {
                dataInicial = DateTime.Now.Date.AddDays(-15);
                dataFinal = DateTime.Now.Date;
            }

            var extrato = new Extrato(numeroConta, numeroAgencia, dataInicial, dataFinal);

            // Act
            await service.ObterExtrato(extrato);

            // Assert
            Assert.False(_notifier.HasNotifications);
            Assert.Equal(nTransf, extrato.TransferenciasPix.Count);
        }
    }
}