using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Infrastructure;


namespace Sam.View
{
    public interface ITransacaoView : ICrudView
    {
        void PopularListaModulo();
        void PopularListaModuloEdit();

        int ID {get; set;}
        int MODULO_ID { get; set; }
        bool ATIVO { get; set; }
        string SIGLA { get; set; }
        string DESCRICAO { get; set; }
        string CAMINHO { get; set; }
        int? ORDEM { get; set; }
    }
}
