using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class AtividadeItemMaterialEntity : BaseEntity
    {
        public AtividadeItemMaterialEntity() { }
        public AtividadeItemMaterialEntity(int _atividadeId)
        {
            this.Id = _atividadeId;
        }
        public string Descricao { get; set; }
    }
}
