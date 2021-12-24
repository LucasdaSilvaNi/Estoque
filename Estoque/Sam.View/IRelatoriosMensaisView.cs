using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IRelatoriosMensaisView : ICrudView
    {
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        int? Almoxarifado { get; set; }
        string NomeRelatorio { get; set; }
    }
}
