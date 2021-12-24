using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IItemNaturezaDespesaView : ICrudView
    {
        void PopularListaNatureza();

        int NaturezaDespesaId { get; set; }
        int ItemMaterialId { get; }
        string ItemMaterialCodigo { get; }
        string ItemMaterialDescricao { get; }
    }
}
