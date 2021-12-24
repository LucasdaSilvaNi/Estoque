using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{

    [Serializable]
    public class ItemNaturezaDespesaEntity : BaseEntity
    {
        public List<NaturezaDespesaEntity> NaturezasDespesa { get; set; }
        public ItemMaterialEntity ItemMaterial { get; set; }
    }
}
