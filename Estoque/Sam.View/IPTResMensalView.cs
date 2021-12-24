using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IPTResView : ICrudView
    {
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        void ExibirRelatorio();
    }
}
