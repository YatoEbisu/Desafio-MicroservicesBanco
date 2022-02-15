using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModalMais.Conta.Domain.Entities
{
    [Table("ContasCorrente")]
    public class ContaCorrente : BaseEntity
    {
        public ContaCorrente()
        {
            NumeroBanco = "746";
            NumeroConta = new Random().Next(100001, 999999).ToString();
            Agencia = "0001";
            DataRegistro = DateTime.Now;
            Imagens = new();
            ChavesPix = new();
        }

        public string NumeroBanco { get; set; }
        public string NumeroConta { get; set; }
        public string Agencia { get; set; }
        public DateTime DataRegistro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public Cliente Cliente { get; set; }
        public List<Imagem> Imagens { get; set; }
        public List<Pix> ChavesPix { get; set; }
    }
}