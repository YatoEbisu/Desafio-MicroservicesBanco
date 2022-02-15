using System;

namespace ModalMais.Conta.Service.Dtos
{
    public class ContaCorrenteResponse
    {
        public string NumeroBanco { get; set; }
        public string NumeroConta { get; set; }
        public string Agencia { get; set; }
        public DateTime DataRegistro { get; set; }
        public ClienteResponse Cliente { get; set; }
    }
}