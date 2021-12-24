using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IUsuarioPerfilNivelAcessoView : ICrudView
    {
        int? PerfilId { get; set; }
        int? NivelAcessoId { get; set; }
        int? Valor { get; set; }
        int? PerfilLoginId { get; set; }
        int? OrgaoId { get; set; }
        int? UoId { get; set; }
        int? UgeId { get; set; }
        int? UaId { get; set; }
        int? GestorId { get; set; }
        int? DivisaoId { get; set; }
        int? AlmoxId { get; set; }
        bool BloqueiaNivel { set; }
    }
}


