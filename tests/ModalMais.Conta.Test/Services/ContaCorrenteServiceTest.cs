using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Service.Services;
using Moq;
using Moq.AutoMock;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Conta.Test.Services
{
    public class ContaCorrenteServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly AbstractNotifier _notifier;

        public ContaCorrenteServiceTest()
        {
            _notifier = new Notifier();
            _mocker = new();
            _mocker.Use(_notifier);
        }

        [Fact(DisplayName = "AddPix deve retornar uma notificação caso conta não exista")]
        public async Task AddPix_DeveRetornarUmaNotificacao_CasoContaNaoExista()
        {
            //Arrange
            var numeroConta = "";
            var data = new Pix(TipoChave.CPF, "");

            var mock = _mocker.GetMock<IContaCorrenteRepository>();
            mock.Setup(r => r.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((ContaCorrente)null);

            var service = _mocker.CreateInstance<ContaCorrenteService>();

            //Act
            await service.AddPix(numeroConta, data);

            //Assert
            Assert.True(_notifier.Notifications.Where(n => n.Key == "NumeroConta").Any());
        }

        [Fact(DisplayName = "AddPix deve retornar uma notificação caso a chave CPF não for igual a do dono da conta")]
        public async Task AddPix_DeveRetornarUmaNotificacao_CasoAChaveCPFNaoForIgualAoDonoDaConta()
        {
            //Arrange
            var numeroConta = "";
            var conta = new ContaCorrente();
            var faker = new Faker("pt_BR");
            conta.Cliente = new(
                faker.Person.Cpf(false),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.Phone,
                faker.Person.Email);
            var data = new Pix(TipoChave.CPF, new Faker("pt_BR").Person.Cpf(false));

            var mock = _mocker.GetMock<IContaCorrenteRepository>();
            mock.Setup(r => r.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(conta);

            var service = _mocker.CreateInstance<ContaCorrenteService>();

            //Act
            await service.AddPix(numeroConta, data);

            //Assert
            Assert.True(_notifier.Notifications.Where(n => n.Key == "Chave").Any());
        }

        [Fact(DisplayName = "AddPix deve retornar uma notificação caso a chave for inválida")]
        public async Task AddPix_DeveRetornarUmaNotificacao_CasoAChaveInformadaForInvalida()
        {
            //Arrange
            var numeroConta = "";
            var conta = new ContaCorrente();
            var faker = new Faker("pt_BR");
            conta.Cliente = new(
                faker.Person.Cpf(false),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.Phone,
                faker.Person.Email);
            var data = new Pix(TipoChave.CPF, "");

            var mock = _mocker.GetMock<IContaCorrenteRepository>();
            mock.Setup(r => r.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(conta);

            var service = _mocker.CreateInstance<ContaCorrenteService>();

            //Act
            await service.AddPix(numeroConta, data);

            //Assert
            Assert.True(_notifier.HasNotifications);
        }

        [Fact(DisplayName = "Atualiza cliente não deve possuir notificações se dados estiverem corretos")]
        public async void UpdateCliente_DeveAtualizarDadosCliente_CasoOClienteInformadoSejaValido()
        {
            //Arrange
            var faker = new Faker("pt_BR");
            var service = _mocker.CreateInstance<ContaCorrenteService>();
            var conta = new ContaCorrente();
            var cliente = new Cliente(
                faker.Person.Cpf(false),
                faker.Person.FirstName,
                faker.Person.LastName,
                "18997656785",
                faker.Person.Email);
            conta.Cliente = cliente;
            var mock = _mocker.GetMock<IContaCorrenteRepository>();
            mock.Setup(r => r.GetByCPF(It.IsAny<string>())).ReturnsAsync(conta);

            //Act
            await service.Update(cliente);

            //Assert
            Assert.False(_notifier.HasNotifications);
        }

        [Fact(DisplayName = "Atualiza cliente deve possuir notificações se dados estiverem incorretos")]
        public async void UpdateCliente_DevePossuirNotificacoes_CasoOClienteInformadoSejaInValido()
        {
            //Arrange
            var faker = new Faker("pt_BR");
            var service = _mocker.CreateInstance<ContaCorrenteService>();
            var cliente = new Cliente(
                "12312312312",
                "",
                null,
                "",
                "email");

            //Act
            await service.Update(cliente);

            //Assert
            Assert.True(_notifier.HasNotifications);
            Assert.True(_notifier.Notifications.Where(n => n.Key == "Nome").Any());
            Assert.True(_notifier.Notifications.Where(n => n.Key == "Sobrenome").Any());
            Assert.True(_notifier.Notifications.Where(n => n.Key == "Email").Any());
            Assert.True(_notifier.Notifications.Where(n => n.Key == "Celular").Any());
            Assert.True(_notifier.Notifications.Where(n => n.Key == "CPF").Any());
        }
    }
}