using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.AspNetCore.Mvc;
using ModalMais.Transferencia.Api.AutoMapper;
using ModalMais.Transferencia.Api.Controllers;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Transferencia.Test.Controllers
{
    public class TransferenciasControllerTest
    {
        private readonly AutoMocker _mocker;

        public TransferenciasControllerTest()
        {
            _mocker = new();
            var configuration = new MapperConfiguration(config => config.AddProfile<AutoMapperConfig>());
            var mapper = configuration.CreateMapper();
            _mocker.Use(mapper);
        }

        [Fact(DisplayName = "Adicionar deve retornar NotFound quando a chave não for encontrada")]
        public async Task Adicionar_DeveRetornarNotFound_QuandoAChaveNaoForEncontrada()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("Conta", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new TransferenciaPixRequest
                { Chave = new Faker().Person.Cpf(false), TipoChave = TipoChave.CPF };

            // Act
            var result = (ObjectResult)await controller.Adicionar(request);

            //Assert
            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "Adicionar deve retornar BadRequest quando a chave não for válida")]
        public async Task Adicionar_DeveRetornarBadRequest_QuandoAChaveInformadaNaoForValida()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("aecio", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new TransferenciaPixRequest
                { Chave = new Faker().Person.Cpf(false), TipoChave = TipoChave.Email };

            // Act
            var result = (ObjectResult)await controller.Adicionar(request);

            //Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact(DisplayName = "Adicionar deve retornar Ok quando a chave for válida")]
        public async Task Adicionar_DeveRetornarOk_QuandoAChaveForValida()
        {
            // Arrange
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new TransferenciaPixRequest
                { Chave = new Faker().Person.Cpf(false), TipoChave = TipoChave.CPF };

            // Act
            var result = (ObjectResult)await controller.Adicionar(request);

            //Assert
            Assert.Equal(201, result.StatusCode);
        }

        [Fact(DisplayName = "ObterExtrato deve retornar NotFound quando a chave não for encontrada")]
        public async Task ObterExtrato_DeveRetornarNotFound_QuandoAChaveNaoForEncontrada()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("Conta", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new ExtratoRequest("", "");

            // Act
            var result = (ObjectResult)await controller.ObterExtrato(request);

            //Assert
            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "ObterExtrato deve retornar BadRequest quando a chave não for válida")]
        public async Task ObterExtrato_DeveRetornarBadRequest_QuandoAChaveInformadaNaoForValida()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("aecio", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new ExtratoRequest("", "");

            // Act
            var result = (ObjectResult)await controller.ObterExtrato(request);

            //Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact(DisplayName = "ObterExtrato deve retornar Ok quando a chave for válida")]
        public async Task ObterExtrato_DeveRetornarOk_QuandoAChaveForValida()
        {
            // Arrange
            var controller = _mocker.CreateInstance<TransferenciasController>();
            var request = new ExtratoRequest("002123", "321321");

            // Act
            var result = (ObjectResult)await controller.ObterExtrato(request);

            //Assert
            Assert.Equal(200, result.StatusCode);
        }
    }
}