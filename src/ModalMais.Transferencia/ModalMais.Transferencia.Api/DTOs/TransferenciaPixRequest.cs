using System.ComponentModel.DataAnnotations;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.DTOs
{
    public class TransferenciaPixRequest
    {
        /// <example>email@email.com</example>
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Chave { get; set; }

        /// <example>1</example>
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public TipoChave TipoChave { get; set; }

        /// <example>Pagamento de boleto</example>
        public string Descricao { get; set; }

        /// <example>4403.20</example>
        public decimal Valor { get; set; }
    }
}