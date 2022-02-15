using System;

namespace ModalMais.Transferencia.Api.DTOs
{
    public class TransferenciaPixResponse
    {
        public string Chave { get; set; }
        public double Valor { get; set; }
        public string Descricao { get; set; }
        public string NumeroBanco { get; set; }
        public string NumeroConta { get; set; }
        public string Agencia { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataTransferencia { get; set; }
    }
}