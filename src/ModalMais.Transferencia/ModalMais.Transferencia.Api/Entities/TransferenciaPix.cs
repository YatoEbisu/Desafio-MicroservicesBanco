using System;
using System.ComponentModel.DataAnnotations.Schema;
using ModalMais.Transferencia.Api.Entities.Base;

namespace ModalMais.Transferencia.Api.Entities
{
    [Table("Transferencias")]
    public class TransferenciaPix : BaseEntity
    {
        public const int LIMITE_DIARIO = 100000;
        public const int LIMITE_UNITARIO = 5000;

        public TransferenciaPix(TipoChave tipoChave, string chave, decimal valor, string descricao)
        {
            TipoChave = tipoChave;
            Chave = chave;
            Valor = valor;
            Descricao = descricao;
        }

        public TransferenciaPix()
        {
        }

        public TipoChave TipoChave { get; set; }
        public string Chave { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public string NumeroBanco { get; set; }
        public string NumeroConta { get; set; }
        public string Agencia { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataTransferencia { get; set; }

        public void Finalizar()
        {
            DataTransferencia = DateTime.Now;
        }
    }

    public enum TipoChave
    {
        Email = 1,
        CPF = 2,
        Celular = 3,
        ChaveAleatoria = 4
    }
}