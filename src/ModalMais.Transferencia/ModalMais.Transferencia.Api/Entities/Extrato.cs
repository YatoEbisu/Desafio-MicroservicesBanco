using System;
using System.Collections.Generic;

namespace ModalMais.Transferencia.Api.Entities
{
    public class Extrato
    {
        public Extrato(string numeroConta, string numeroAgencia, DateTime? dataInicial = null,
            DateTime? dataFinal = null)
        {
            NumeroConta = numeroConta;
            NumeroAgencia = numeroAgencia;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            TransferenciasPix = new();
        }

        public string NumeroConta { get; }
        public string NumeroAgencia { get; }
        public DateTime? DataInicial { get; private set; }
        public DateTime? DataFinal { get; private set; }
        public List<TransferenciaPix> TransferenciasPix { get; private set; }

        public void AdicionarTransferencias(List<TransferenciaPix> transferenciasPix)
        {
            if (transferenciasPix != null)
                TransferenciasPix = transferenciasPix;
        }

        public void AdicionarPeriodo(DateTime inicial, DateTime final)
        {
            DataInicial = inicial;
            DataFinal = final;
        }
    }
}