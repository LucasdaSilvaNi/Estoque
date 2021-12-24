using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    public class BaseSeguranca
    {
        public DateTime Criacao { get; set; }
        public Int32 IdOperador { get; set; }
        public Int64 IpOrigem { get; set; }
        public int IdTransacaoOrigem { get; set; }
        public int Id { get; set; } // adicionado por causa da exibição de grid
    }
}
