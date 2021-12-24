using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IOrgaoView : ICrudView
    {
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        bool Ativo { get; set; }
        bool Implantado { get; set; }
        bool IntegracaoSIAFEM { get; set; }
    }
}
