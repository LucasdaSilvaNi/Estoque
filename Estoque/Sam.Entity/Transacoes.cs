using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Transacao: BaseSeguranca
    {
        
            public Int32 IdTransacao { get; set; }
            public Modulo Modulo { get; set; }
            public bool Ativa { get; set; }            
            public string Sigla { get; set; }
            public string Descricao { get; set; }
            public string Caminho { get; set; }
            public int Ordem { get; set; }
            public bool Edita { get; set; }
    }
}
