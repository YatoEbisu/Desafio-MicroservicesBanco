using ModalMais.Conta.Domain.Entities;

namespace ModalMais.Conta.Service.Dtos
{
    public class ContaPixRequest
    {
        public string Chave { get; set; }
        public TipoChave Tipo { get; set; }
    }
}