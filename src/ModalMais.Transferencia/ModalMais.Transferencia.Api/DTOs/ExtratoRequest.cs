using System;

namespace ModalMais.Transferencia.Api.DTOs
{
    public class ExtratoRequest
    {
        public ExtratoRequest(
            string numeroConta, string numeroAgencia,
            DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            NumeroConta = numeroConta;
            NumeroAgencia = numeroAgencia;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
        }

        public string NumeroConta { get; set; }
        public string NumeroAgencia { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}