using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Divisao
    {
        public int? DivisaoId { get; set; }
        public int? DivisaoCodigo { get; set; }
        public string DivisaoDescricao { get; set; }
        
    }
}
