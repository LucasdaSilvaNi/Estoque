using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    public class PerfilLogin: BaseSeguranca
    {
        public int IdPerfilLogin { get; set; }
        public int PerfilId { get; set; }
        public int LoginId { get; set; }
        public int OrgaoPadraoId { get; set; }
        public int GestorPadraoId { get; set; }
        public int AlmoxarifadoPadraoId { get; set; }
        public int UOPadraoId { get; set; }
        public int UGEPadraoId { get; set; }
        public int UAPadraoId { get; set; }
        public int DivisaoPadraoId { get; set; }
        public List<PerfilLoginNivelAcesso> PerfilLoginNivelAcessoList { get; set; }
    }
}
