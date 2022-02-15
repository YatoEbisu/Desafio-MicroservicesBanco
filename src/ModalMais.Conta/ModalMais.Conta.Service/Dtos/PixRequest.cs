using ModalMais.Conta.Domain.Entities;

namespace ModalMais.Conta.Service.Dtos
{
    public class PixRequest
    {
        public string Chave { get; set; }
        public TipoChave Tipo { get; set; }
        public string NumeroConta { get; set; }
    }
}