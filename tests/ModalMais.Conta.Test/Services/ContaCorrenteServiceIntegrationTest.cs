using System;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Domain.Interfaces;
using ModalMais.Conta.Infra.Data.Repositories;
using ModalMais.Conta.Service.Services;
using ModalMais.Conta.Test.Imagens;
using Notie;
using Notie.Contracts;
using Xunit;

namespace ModalMais.Conta.Test.Services
{
    public class ContaCorrenteServiceIntegrationTest : IDisposable
    {
        private readonly AbstractNotifier _notifier;
        private readonly IContaCorrenteRepository _repository;

        public ContaCorrenteServiceIntegrationTest()
        {
            _notifier = new Notifier();
            _repository = new ContaCorrenteRepository(Factory.CreateContext());
        }

        public void Dispose()
        {
            Factory.DropDatabase();
        }

        [Fact(DisplayName = "Update deve gerar uma notificação caso o cliente com o CPF informado não for encontrado")]
        public async Task Update_DeveGerarNotificacao_CasoOCPFInformadoNaoForEncontrado()
        {
            var faker = new Faker("pt_BR");
            var cliente = new Cliente(
                faker.Person.Cpf(false),
                faker.Person.FirstName,
                faker.Person.LastName,
                "88999999999",
                faker.Person.Email);

            var service = new ContaCorrenteService(_repository, _notifier);
            await service.Update(cliente);

            Assert.Contains(_notifier.Notifications, n => n.Key == "Conta");
        }

        [Fact(DisplayName = "Update deve atualizar os dados caso o cliente com o CPF informado for encontrado")]
        public async Task Update_DeveAtualizarOsDados_CasoOCPFSejaEncontrado()
        {
            var contaGerada = MyFixture.GerarContaCorrenteValida();
            await _repository.Add(contaGerada);

            contaGerada.Cliente.Nome = "João";

            var service = new ContaCorrenteService(_repository, _notifier);
            await service.Update(contaGerada.Cliente);


            var conta = await _repository.GetByCPF(contaGerada.Cliente.CPF);

            Assert.Equal("João", conta.Cliente.Nome);
        }
    }
}