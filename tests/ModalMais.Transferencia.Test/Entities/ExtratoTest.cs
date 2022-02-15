using System;
using ModalMais.Transferencia.Api.DTOs;
using ModalMais.Transferencia.Api.Entities.Validations;
using Xunit;

namespace ModalMais.Transferencia.Test.Entities
{
    public class ExtratoTest
    {
        [Fact(DisplayName = "RequestValidation Valido")]
        public void EntidadeDeveSerValida()
        {
            // Arrange
            var obj = new ExtratoRequest("123456", "123456", new DateTime(2000, 1, 2), new DateTime(2000, 2, 1));
            var validation = new ExtratoRequestValidation();

            // Act
            var result = validation.Validate(obj);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "RequestValidation Invalido")]
        public void EntidadeDeveSerInvalida()
        {
            // Arrange
            var obj = new ExtratoRequest("", "");
            var validation = new ExtratoRequestValidation();

            // Act
            var result = validation.Validate(obj);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "ExtratoRequestValidation Invalido se for apenas umas data")]
        [InlineData(1)]
        [InlineData(2)]
        public void EntidadeDeveSerInvalidaSeApenasUmaDataForInformada(int x)
        {
            // Arrange
            var obj = new ExtratoRequest("123465", "123456");
            if (x == 1)
                obj.DataInicial = new DateTime(2000, 1, 1);
            else
                obj.DataFinal = new DateTime(2000, 1, 1);

            var validation = new ExtratoRequestValidation();

            // Act
            var result = validation.Validate(obj);

            // Assert
            Assert.Single(result.Errors);
        }

        [Fact(DisplayName = "RequestValidation Invalido se o periodo entre as datas for maior que 30")]
        public void EntidadeDeveSerInvalidaSeOPeriodoEntreAsDatasForMaiorQue30()
        {
            // Arrange
            var obj = new ExtratoRequest("123465", "123456", new DateTime(2000, 1, 1), new DateTime(2000, 2, 1));
            var validation = new ExtratoRequestValidation();

            // Act
            var result = validation.Validate(obj);

            // Assert
            Assert.Single(result.Errors);
        }
    }
}