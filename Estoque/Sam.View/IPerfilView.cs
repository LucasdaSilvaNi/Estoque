using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


namespace Sam.View
{
    public interface IPerfilView : ICrudView
    {
        int ID { get; set; }
        bool ATIVO { get; set; }
        string DESCRICAO { get; set; }
        int? PESO { get; set; }
        int PERFILNIVEL { get; set; }
    }
}
