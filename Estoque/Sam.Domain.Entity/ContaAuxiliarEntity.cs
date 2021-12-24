using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class ContaAuxiliarEntity : BaseEntity
    {
        public ContaAuxiliarEntity() { }
        public ContaAuxiliarEntity(int _contaId) { this.Id = _contaId; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int ContaContabil { get; set; }

    }
}
