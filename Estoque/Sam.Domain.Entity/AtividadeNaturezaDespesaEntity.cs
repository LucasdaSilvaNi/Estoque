using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class AtividadeNaturezaDespesaEntity : BaseEntity
    {
        public AtividadeNaturezaDespesaEntity() { }
        public AtividadeNaturezaDespesaEntity(int _atividadeId) { this.Id = _atividadeId; }
        public string Descricao { get; set; }

    }
}
