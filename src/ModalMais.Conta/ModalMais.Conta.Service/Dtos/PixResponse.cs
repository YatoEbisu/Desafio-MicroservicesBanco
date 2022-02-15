using ModalMais.Conta.Domain.Entities;

namespace ModalMais.Conta.Service.Dtos
{
    public class PixResponse
    {
        public string Chave { get; set; }
        public TipoChave Tipo { get; set; }
    }
}