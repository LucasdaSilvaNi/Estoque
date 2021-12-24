using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


namespace Sam.View
{
    public interface IUsuarioPerfilView : ICrudView
    {
        int? UserId { get; }
        int? PerfilId { get; set; }
        int? LoginId { get; set; }
        int? OrgaoId { get; set; }
        int? UoId { get; set; }
        int? UgeId { get; set; }
        int? UaId { get; set; }
        int? DivisaoId { get; set; }
        int? AlmoxId { get; set; }
        int? GestorId { get; set; }
        int? PerfilLoginId { get; set; }
        bool IsPerfilPadrao { get; set; }

        void ApagarSessaoIDs();
        List<string> ConsistirPerfil();
    }
}
