using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Sistema
    {
        public Int32 SistemaId { get; set; }
        public bool Ativo { get; set; }
        public string Sigla { get; set; }
        public string Descricao { get; set; }
    }
}
