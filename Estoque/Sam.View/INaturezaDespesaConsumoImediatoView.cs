using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface INaturezaDespesaConsumoImediatoView : ICrudView
    {
        //bool AtividadeNaturezaDespesaId { set; get; }
        //void PopularListaAtividade();
        //void ExibirRelatorio();
        //SortedList ParametrosRelatorio { get; }
        //RelatorioEntity DadosRelatorio { get; set; }

        bool Ativa { set; get; }
        void PopularListaNaturezasDespesaConsumoImediato();
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
