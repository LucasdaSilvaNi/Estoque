using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class SubItemMaterialAlmoxEntity : BaseEntity
    {
        public SubItemMaterialAlmoxEntity() { }
        public SubItemMaterialAlmoxEntity(int _id) { base.Id = _id; }        

        public int SubItemId { get; set; }

        public int SubItemMaterialId { get; set; }

        public int IndicadorDisponivelId { get; set; }

        public decimal EstoqueMinimo { get; set; }

        public decimal EstoqueMaximo { get; set; }

        public bool IndicadorAtividade { get; set; }
    }
}
