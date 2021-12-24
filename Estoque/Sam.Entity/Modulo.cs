using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Sam.Entity
{
    [Serializable]
    public class Modulo: BaseSeguranca
    {
        public Int32 ModuloId { get; set; }
        public Sistema Sistema { get; set; }
        public bool ModuloAtivo { get; set; }
        public string Sigla { get; set; }
        public string Descricao { get; set; }
        public short? Ordem { get; set; }        
        public string Caminho { get; set; }        
        public int? ModuloPaiId { get; set; }
        public List<Transacao> Transacoes { get; set; }
    }
}
