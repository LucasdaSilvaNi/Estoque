using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Infrastructure
{

    /// <summary>
    /// interação com outra base de dados (SAMWEB -> DBSEG)
    /// </summary>
    public partial class TB_SUBITEM_MATERIAL
    {
        public int TB_ITEM_MATERIAL_ID { get; set; }
        public int TB_ITEM_MATERIAL_CODIGO { get; set; }
        public string TB_UNIDADE_FORNECIMENTO_CODIGO_DESCRICAO { get; set; }

    }
}
