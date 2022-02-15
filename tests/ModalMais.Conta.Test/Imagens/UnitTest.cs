using System.Collections.Generic;
using System.Linq;
using ModalMais.Conta.Domain.Entities;
using Xunit;

namespace ModalMais.Conta.Test.Imagens
{
    [Collection(nameof(MyFixtureCollection))]
    public class ImagemTest
    {
        private readonly MyFixture _Fixture;

        public ImagemTest(MyFixture fixture)
        {
            _Fixture = fixture;
            _Fixture.Notifier.Clear();
        }

        /*
            CPF VALIDO 						 - OK
            CPF CADASTRADO NO SISTEMA		 - OK
            DADOS BANCARIOS OK				 - OK
            IMAGEM SIZE MENOR QUE 4MB   	 - OK
            IMAGEM TYPE PNG					 - OK
            CONFIRMAR QUE FOI SALVO NO BANCO - OK
            SOMENTE UMA IMAGEM VÁLIDA		 - OK
         */
        [Fact]
        public async void ContaCorrenteService_AdicionaImagens_DeveRetornarVerdadeiro()
        {
            // Arrange
            const int NUMERO_DE_IMAGENS = 1;
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequest(NUMERO_DE_IMAGENS, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            // Act
            await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);

            // Assert
            Assert.Empty(notifications);
            Assert.Single(consultaImagens.Imagens);
        }

        // IMAGEM SIZE MAIOR QUE 4MB   	- FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemMaior4Mb_DeveRetornarFalso()
        {
            // Arrange
            const int NUMERO_DE_IMAGENS = 3;
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequestInvalidoPorTamanho(NUMERO_DE_IMAGENS, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            // Act
            var imagens = _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia).Result;
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);

            // Assert
            Assert.Equal(NUMERO_DE_IMAGENS, notifications.Count);
            Assert.Empty(consultaImagens.Imagens);
        }

        // 	IMAGEM TYPE OTHER - FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemFormatoInvalido_DeveRetornarFalso()
        {
            // Arrange
            const int NUMERO_DE_IMAGENS = 4;
            var imagens = new List<Imagem>();
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequestInvalidoPorExtensao(NUMERO_DE_IMAGENS, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            // Act
            imagens = await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);


            // Assert
            Assert.Equal(NUMERO_DE_IMAGENS, notifications.Count);
            Assert.Empty(consultaImagens.Imagens);
        }

        // CPF INVALIDO                  - FAIL
        // CPF NAO CADASTRADO NO SISTEMA - FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemCpfInvalido_DeveRetornarFalso()
        {
            // Arrange
            var imagens = new List<Imagem>();
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequest(1, contaCorrente);

            contaCorrente.Cliente.CPF = "15";

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);


            // Act
            imagens = await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);


            // Assert
            Assert.Equal(1, notifications.Count);
            Assert.Equal("Img_CPF", notifications.FirstOrDefault().Key);
            Assert.Empty(consultaImagens.Imagens);
        }

        // 	CPF CADASTRADO NO SISTEMA MAS COM OUTRA CONTA 	- FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemBancoInvalido_DeveRetornarFalso()
        {
            // Arrange
            var imagens = new List<Imagem>();
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequest(1, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            imagemRequest.Banco = "123";

            // Act
            imagens = await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);


            // Assert
            Assert.Equal(1, notifications.Count);
            Assert.Equal("Img_Dados", notifications.FirstOrDefault().Key);
            Assert.Empty(consultaImagens.Imagens);
        }

        // 	CPF CADASTRADO NO SISTEMA MAS COM OUTRA CONTA 	- FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemAgenciaInvalida_DeveRetornarFalso()
        {
            // Arrange
            var imagens = new List<Imagem>();
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequest(1, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            imagemRequest.Agencia = "123";

            // Act
            imagens = await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);


            // Assert
            Assert.Equal(1, notifications.Count);
            Assert.Equal("Img_Dados", notifications.FirstOrDefault().Key);
            Assert.Empty(consultaImagens.Imagens);
        }

        // 	CPF CADASTRADO NO SISTEMA MAS COM OUTRA CONTA 	- FAIL
        [Fact]
        public async void ContaCorrenteService_AdicionaImagemContaInvalida_DeveRetornarFalso()
        {
            // Arrange
            var imagens = new List<Imagem>();
            var contaCorrente = MyFixture.GerarContaCorrenteValida();
            var imagemRequest = _Fixture.GerarImagemRequest(1, contaCorrente);

            await _Fixture.ContaCorrenteRepository.Add(contaCorrente);

            imagemRequest.Conta = "123";

            // Act
            imagens = await _Fixture.ContaCorrenteService.SalvarImagem(imagemRequest.Imagens, imagemRequest.CPF,
                imagemRequest.Banco, imagemRequest.Conta, imagemRequest.Agencia);
            var notifications = _Fixture.Notifier.Notifications;

            var consultaImagens = await _Fixture.ContaCorrenteRepository.GetByCPF(contaCorrente.Cliente.CPF);


            // Assert
            Assert.Equal(1, notifications.Count);
            Assert.Equal("Img_Dados", notifications.FirstOrDefault().Key);
            Assert.Empty(consultaImagens.Imagens);
        }
    }
}