using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IFechamentoMensalView : IBaseView, ICrudView
    {
        bool ExibeBotaoNLConsumo { set; }
        bool ExibeBotaoReabertura { set; }
        bool BloqueiaBotaoNLConsumo { set; }
        bool BloqueiaBotaoReabertura { set; }

        SortedList ParametrosRelatorio { get;}
        RelatorioEntity DadosRelatorio { get; set; }
        void ExibirRelatorio();
    }
}
