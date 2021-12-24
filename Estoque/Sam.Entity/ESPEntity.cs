using Sam.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class ESPEntity : BaseSeguranca
    {
        public int ESPCodigo { get; set; }
        public byte TermoId { get; set; }
        public string ESPSistema { get; set; }
        public string ESPSistemaDescricao { get; set; }
        public Gestor Gestor { get; set; }
        public string DataInicioVigencia { get; set; }
        public string DataFimVigencia { get; set; }
        public int QtdeRepositorioPrincipal { get; set; }
        public int QtdeRepositorioComplementar { get; set; }
        public int QtdeUsuarioNivelI { get; set; }
        public int QtdeUsuarioNivelII { get; set; }
    }
}
