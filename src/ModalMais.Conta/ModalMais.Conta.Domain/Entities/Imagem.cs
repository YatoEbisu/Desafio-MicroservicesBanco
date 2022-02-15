using System;

namespace ModalMais.Conta.Domain.Entities
{
    public class Imagem
    {
        public Imagem(string enderecoImagem)
        {
            EnderecoImagem = enderecoImagem;
            Validado = false;
            Ativo = false;
            DataRegistro = DateTime.Now;
        }

        public string EnderecoImagem { get; }
        public DateTime DataRegistro { get; }
        public bool Validado { get; private set; }
        public DateTime? DataValidacao { get; private set; }
        public bool Ativo { get; private set; }

        public void DefineDataValidacao()
        {
            DataValidacao = DateTime.Now;
        }

        public void DefineValidado()
        {
            Validado = true;
        }

        public void DefineAtivo()
        {
            Ativo = true;
        }

        public void DefineInativo()
        {
            Ativo = false;
        }
    }
}