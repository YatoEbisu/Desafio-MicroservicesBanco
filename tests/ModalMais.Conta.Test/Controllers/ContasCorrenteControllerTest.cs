using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.AspNetCore.Mvc;
using ModalMais.Conta.Api.AutoMapper;
using ModalMais.Conta.Api.Controllers;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Service.Dtos;
using Moq;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Test.Controllers
{
    public class ContaCorrenteControllerTest
    {
        private readonly AutoMocker _mocker;

        public ContaCorrenteControllerTest()
        {
            _mocker = new();
            var configuration = new MapperConfiguration(config => config.AddProfile<AutoMapperConfig>());
            var mapper = configuration.CreateMapper();
            _mocker.Use(mapper);
        }

        [Fact(DisplayName = "GetContaPix deve retornar NotFound quando a chave não for encontrada")]
        public async Task GetContaPix_DeveRetornarNotFound_QuandoAChaveNaoForEncontrada()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("Conta", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<ContasCorrenteController>();
            var mock = _mocker.GetMock<IContaCorrenteService>();
            var request = new ContaPixRequest { Chave = new Faker().Person.Cpf(false), Tipo = TipoChave.CPF };
            mock.Setup(r => r.FindByPix(It.IsAny<Pix>())).ReturnsAsync((ContaCorrente)null);

            // Act
            var result = (ObjectResult)await controller.GetContaPix(request);

            //Assert
            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "GetContaPix deve retornar BadRequest quando a chave não for válida")]
        public async Task GetContaPix_DeveRetornarBadRequest_QuandoAChaveInformadaNaoForValida()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("aecio", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<ContasCorrenteController>();
            var conta = new ContaCorrente();
            var request = new ContaPixRequest { Chave = new Faker().Person.Cpf(false), Tipo = TipoChave.Email };
            var mockService = _mocker.GetMock<IContaCorrenteService>();
            mockService.Setup(r => r.FindByPix(It.IsAny<Pix>())).ReturnsAsync(conta);


            // Act
            var result = (ObjectResult)await controller.GetContaPix(request);

            //Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact(DisplayName = "GetContaPix deve retornar Ok quando a chave for válida")]
        public async Task GetContaPix_DeveRetornarOk_QuandoAChaveForValida()
        {
            // Arrange
            var controller = _mocker.CreateInstance<ContasCorrenteController>();
            var conta = new ContaCorrente();
            var faker = new Faker("pt_BR");
            conta.Cliente = new(
                faker.Person.Cpf(false),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.Phone,
                faker.Person.Email);
            var mockService = _mocker.GetMock<IContaCorrenteService>();
            var request = new ContaPixRequest { Chave = new Faker().Person.Cpf(false), Tipo = TipoChave.CPF };
            mockService.Setup(r => r.FindByPix(It.IsAny<Pix>())).ReturnsAsync(conta);

            // Act
            var result = (ObjectResult)await controller.GetContaPix(request);

            //Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Fact(DisplayName = "ClienteUpdate deve retornar NoContent quando cliente for atualizado")]
        public async Task ClienteUpdate_DeveRetornarNoContent_QuandoClienteAtualizado()
        {
            // Arrange
            var controller = _mocker.CreateInstance<ContasCorrenteController>();
            var faker = new Faker("pt_BR");
            var request = new AtualizaClienteRequest
            {
                CPF = faker.Person.Cpf(),
                Nome = faker.Person.FirstName,
                Sobrenome = faker.Person.LastName,
                Celular = faker.Person.Phone,
                Email = faker.Person.Email
            };

            // Act
            //var result = (ObjectResult)await controller.GetContaPix(request);
            var result = (NoContentResult)await controller.ClienteUpdate(request);

            //Assert
            Assert.Equal(204, result.StatusCode);
        }

        [Fact(DisplayName = "ClienteUpdate deve retornar BadRequest quando cliente não for atualizado")]
        public async Task ClienteUpdate_DeveRetornarBadRequest_QuandoClienteNaoAtualizado()
        {
            // Arrange
            var notifier = new Notifier();
            notifier.AddNotification(new("aecio", "value"));
            _mocker.Use<AbstractNotifier>(notifier);
            var controller = _mocker.CreateInstance<ContasCorrenteController>();
            var faker = new Faker("pt_BR");
            var request = new AtualizaClienteRequest
            {
                CPF = faker.Person.Cpf(),
                Nome = faker.Person.FirstName,
                Sobrenome = faker.Person.LastName,
                Celular = faker.Person.Phone,
                Email = faker.Person.Email
            };

            // Act
            var result = (ObjectResult)await controller.ClienteUpdate(request);

            //Assert
            Assert.Equal(400, result.StatusCode);
        }
    }
}