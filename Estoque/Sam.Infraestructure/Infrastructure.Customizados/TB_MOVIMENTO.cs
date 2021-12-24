using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Infrastructure
{

    /// <summary>
    /// interação com outra base de dados (SAMWEB -> DBSEG)
    /// </summary>
    public partial class TB_MOVIMENTO
    {
        public string TB_USUARIO_NOME { get; set; }
        public string TB_USUARIO_NOME_ESTORNO { get; set; }
        public string TB_ALMOXARIFADO_CODIGO_DESCRICAO { get; set; }
        public string TB_ALMOXARIFADO_DESCRICAO { get; set; }
    }
}
