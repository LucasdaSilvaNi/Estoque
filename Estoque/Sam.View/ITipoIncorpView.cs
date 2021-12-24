using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface ITipoIncorpView : ICrudView
    {
        string CodigoTransacao { get; set; }
        bool BloqueiaCodigoTransacao { set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
