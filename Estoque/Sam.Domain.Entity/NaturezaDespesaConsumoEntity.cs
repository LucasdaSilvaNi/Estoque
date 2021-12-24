using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class NaturezaDespesaConsumoImediatoEntity : BaseEntity
    {
        public NaturezaDespesaConsumoImediatoEntity() { }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public bool Ativa { get; set; }
    }
}
