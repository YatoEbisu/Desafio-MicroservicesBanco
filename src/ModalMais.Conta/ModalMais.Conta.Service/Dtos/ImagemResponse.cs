using System;

namespace ModalMais.Conta.Service.Dtos
{
    public class ImagemResponse
    {
        public string EnderecoImagem { get; private set; }
        public DateTime DataRegistro { get; private set; }
        public bool Validado { get; private set; }
        public DateTime? DataValidacao { get; private set; }
        public bool Ativo { get; private set; }
    }
}