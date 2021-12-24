using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ISistemaView : ICrudView
    {
        int ID { get; set; }
        bool ATIVO { get; set; }
        string SIGLA { get; set; }
        string DESCRICAO { get; set; }
    }
}
