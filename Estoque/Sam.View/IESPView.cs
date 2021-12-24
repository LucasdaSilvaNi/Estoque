using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IESPView : ICrudView
    {
        int ID { get; set; }
        int EspCodigo { get; set; }
        string EspSistema { get; set; }
        int GestorId { get; set; }
        DateTime? DataInicioVigencia { get; set; }
        DateTime? DataFimVigencia { get; set; }
        int QtdeRepositorioPrincipal { get; set; }
        int QtdeRepositorioComplementar { get; set; }
        int QtdeUsuarioNivelI { get; set; }
        int QtdeUsuarioNivelII { get; set; }
        byte TermoId { get; set; }
    }
}
