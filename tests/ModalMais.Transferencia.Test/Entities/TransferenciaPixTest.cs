using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Entities.Validations;
using Xunit;

namespace ModalMais.Transferencia.Test.Entities
{
    public class TransferenciaPixTest
    {
        [Fact]
        public void EntidadeDeveSerValidaCasoOsDadosSejamInvalidos()
        {
            // Arrange
            var entity = new TransferenciaPix(TipoChave.CPF, "94448025071", 4000, "Pagamento boleto");
            var validation = new TransferenciaPixValidation();

            // Act
            var result = validation.Validate(entity);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void EntidadeDeveSerInvalidaCasoOsDadosSejamInvalidos()
        {
            // Arrange
            var entity = new TransferenciaPix(TipoChave.CPF, "94448025072", 6000, "");
            var validation = new TransferenciaPixValidation();

            // Act
            var result = validation.Validate(entity);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
        }
    }
}