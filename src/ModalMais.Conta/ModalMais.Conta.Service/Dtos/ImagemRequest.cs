using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ModalMais.Conta.Service.Dtos
{
    public class ImagemRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Banco { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Agencia { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Conta { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public List<IFormFile> Imagens { get; set; }
    }
}