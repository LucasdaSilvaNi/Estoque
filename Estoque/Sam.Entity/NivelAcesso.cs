using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    public class NivelAcesso : BaseSeguranca
    {
        public int IdNivelAcesso { get; set; }
        public short NivelId { get; set; }
        public int PerfilId { get; set; }
        public int ValorNivelAcesso { get; set; }
        public bool Default { get; set; }
        public string DescricaoNivel { get; set; }
    }

    public class Nivel : BaseSeguranca
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

    }
}
