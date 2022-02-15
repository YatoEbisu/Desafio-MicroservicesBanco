using ModalMais.Conta.Domain.Entities;
using ModalMais.Conta.Service.Validations;
using Xunit;

namespace ModalMais.Conta.Test.Entities
{
    public class ClienteTest
    {
        [Fact]
        public void EntidadeDeveSerInvalidaCasoOsDadosSejamInvalidos()
        {
            // Arrange
            var entity = new Cliente("", "", "", "", "");
            var validation = new ClienteValidation();

            //Act
            var result = validation.Validate(entity);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void EntidadeDeveSerValidaCasoOsDadosSejamValidos()
        {
            // Arrange
            var entity = new Cliente("99179125093", "Nome", "Sobrenome", "32995432122", "teste@teste.com");
            var validation = new ClienteValidation();

            //Act
            var result = validation.Validate(entity);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}