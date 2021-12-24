using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Infrastructure
{
    /// <summary>
    /// interação com outra base de dados (SAMWEB -> DBSEG)
    /// </summary>
    public partial class TB_MOVIMENTO_ITEM
    {
        
        public string TB_USUARIO_NOME { get; set; }
        public string TB_USUARIO_NOME_ESTORNO { get; set; }
        public decimal? TB_MOVIMENTO_ITEM_SALDO_QTDE_REC { get; set; }
        public decimal? TB_MOVIMENTO_ITEM_SALDO_VALOR_REC { get; set; }
        
    }
}
