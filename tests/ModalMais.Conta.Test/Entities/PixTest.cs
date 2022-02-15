using System.Linq;
using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Service.Dtos;
using ModalMais.Conta.Service.Validations;
using ModalMais.Conta.Test.Imagens;
using Xunit;

namespace ModalMais.Conta.Test.Entities
{
    [Collection(nameof(MyFixtureCollection))]
    public class PixTest
    {
        private readonly MyFixture _fixture;

        public PixTest(MyFixture fixture)
        {
            _fixture = fixture;
            _fixture.Notifier.Clear();
        }

        #region Testes de Entidade

        [Theory]
        [InlineData(TipoChave.Email)]
        [InlineData(TipoChave.CPF)]
        [InlineData(TipoChave.Celular)]
        public void EntidadeDeveSerInvalidaCasoOsDadosSejamInvalidos(TipoChave tipo)
        {
            // Arrange
            var entity = new Pix(tipo);
            var validation = new PixValidation();

            //Act
            var result = validation.Validate(entity);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("email@email.com", TipoChave.Email)]
        [InlineData("43552749004", TipoChave.CPF)]
        [InlineData("66933983585", TipoChave.Celular)]
        [InlineData("SouUmaChaveAletoria", TipoChave.ChaveAleatoria)]
        public void EntidadeDeveSerValidaCasoOsDadosSejamValidos(string chave, TipoChave tipo)
        {
            // Arrange
            var entity = new Pix(tipo, chave);
            var validation = new PixValidation();

            //Act
            var result = validation.Validate(entity);

            // Assert
            Assert.True(result.IsValid);
        }

        #endregion

        #region Testes de Service e Repository

        [Theory]
        [InlineData("email@email.com", TipoChave.Email)]
        [InlineData("43552749004", TipoChave.CPF)]
        [InlineData("66933983585", TipoChave.Celular)]
        [InlineData("SouUmaChaveAletoria", TipoChave.ChaveAleatoria)]
        public async void ContaCorrenteDeveSerConsultadaCasoOsDadosSejamValidos(string chave, TipoChave tipo)
        {
            //Arrange

            // Criar uma conta válida
            var contaCorrente = MyFixture.GerarContaCorrenteValida();

            if (tipo == TipoChave.CPF) contaCorrente.Cliente.CPF = "43552749004";

            await _fixture.ContaCorrenteRepository.Add(contaCorrente);

            // Adicionar a chave pix a ela
            PixRequest pixRequest = new()
            {
                NumeroConta = contaCorrente.NumeroConta,
                Tipo = tipo,
                Chave = chave
            };

            var pixEscrita = _fixture.Mapper.Map<Pix>(pixRequest);

            await _fixture.ContaCorrenteService.AddPix(pixRequest.NumeroConta, pixEscrita);

            // Dados para consulta
            var obj = await _fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);
            var pixinteiro = obj.ChavesPix.Where(k => k.Tipo == tipo).FirstOrDefault();

            ContaPixRequest contaPixRequest = new()
            {
                Tipo = tipo,
                Chave = tipo == TipoChave.ChaveAleatoria ? pixinteiro.Chave : chave
            };
            //
            //Act
            var pixLeitura = _fixture.Mapper.Map<ContaPixRequest, Pix>(contaPixRequest);
            var contaConsultada = await _fixture.ContaCorrenteService.FindByPix(pixLeitura);

            var notifications = _fixture.Notifier.Notifications;

            //Assert
            Assert.Empty(notifications);
            Assert.NotNull(contaConsultada);
            Assert.Equal(contaCorrente.NumeroBanco, contaConsultada.NumeroBanco);
            Assert.Equal(contaCorrente.NumeroConta, contaConsultada.NumeroConta);
            Assert.Equal(contaCorrente.Agencia, contaConsultada.Agencia);
        }


        [Theory]
        [InlineData("email2@email.com", TipoChave.Email)]
        [InlineData("98387807079", TipoChave.CPF)]
        [InlineData("39988225140", TipoChave.Celular)]
        [InlineData("SouUmaChaveAletoria", TipoChave.ChaveAleatoria)]
        public async void ContaCorrenteNaoDeveSerConsultadaCasoChaveNaoCadastrada(string chave, TipoChave tipo)
        {
            //Arrange
            ContaPixRequest contaPixRequestErrado = new()
            {
                Tipo = tipo,
                Chave = chave
            };

            //Act
            var pixLeitura = _fixture.Mapper.Map<ContaPixRequest, Pix>(contaPixRequestErrado);

            var contaConsultada = await _fixture.ContaCorrenteService.FindByPix(pixLeitura);
            var notifications = _fixture.Notifier.Notifications;

            //Assert
            Assert.Single(notifications);
            Assert.Null(contaConsultada);
        }

        #endregion
    }
}