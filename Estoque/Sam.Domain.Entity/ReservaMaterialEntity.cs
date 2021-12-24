using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class ReservaMaterialEntity : BaseEntity
    {

       public ReservaMaterialEntity()
       {
       }

       public ReservaMaterialEntity(int? _id)
       {
           base.Id = _id;
       }

       public SubItemMaterialAlmoxEntity SubItemMaterialAlmox { get; set; }
       public SubItemMaterialEntity SubItemMaterial { get; set; }
       public ItemMaterialEntity ItemMaterial { get; set; }
       public UnidadeFornecimentoEntity UnidadeFornecimento { get; set; }
       public AlmoxarifadoEntity Almoxarifado { get; set; }
       public UGEEntity Uge { get; set; }
       public DateTime Data { get; set; }
       public string Obs { get; set; }
       public decimal? Quantidade { get; set; }

    }

}
