using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IModuloView : ICrudView
    {
        void PopularDllSistema();

        int ID { get; set; }
        int SISTEMA_ID { get; set; }
        bool ATIVO { get; set; }
        string SIGLA { get; set; }
        string DESCRICAO { get; set; }
        short? ORDEM { get; set; }
        string CAMINHO { get; set; }
        int? ID_PAI { get; set; }
    }
}
