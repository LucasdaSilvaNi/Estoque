using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface INaturezaDespesaView : ICrudView
    {
        bool AtividadeNaturezaDespesaId { set; get; }
        void PopularListaAtividade();
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
