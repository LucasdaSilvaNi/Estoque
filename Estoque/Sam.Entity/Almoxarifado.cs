using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Almoxarifado
    {
        public int? AlmoxarifadoId { get; set; }
        public int? AlmoxarifadoCodigo { get; set; }
        public string AlmoxarifadoDescricao { get; set; }
    }
}   
