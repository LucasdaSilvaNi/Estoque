using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class NaturezaDespesaEntity : BaseEntity
    {
        public NaturezaDespesaEntity() { }
        public NaturezaDespesaEntity(int? _naturezaId) { this.Id = _naturezaId; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public bool? Natureza { get; set; }

        //public AtividadeNaturezaDespesaEntity Natureza { get; set; }
    }
}
