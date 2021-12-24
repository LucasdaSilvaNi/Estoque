using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Orgao
    {
        public int OrgaoId { get; set; }
        public int OrgaoCodigo { get; set; }
        public string OrgaoDescricao { get; set; }
    }
}
