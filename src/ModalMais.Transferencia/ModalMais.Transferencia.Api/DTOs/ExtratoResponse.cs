using System;
using System.Collections.Generic;

namespace ModalMais.Transferencia.Api.DTOs
{
    public class ExtratoResponse
    {
        public string NumeroConta { get; set; }
        public string NumeroAgencia { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public List<TransferenciaPixResponse> TransferenciasPix { get; set; }
    }
}