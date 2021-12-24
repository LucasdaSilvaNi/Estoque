using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IConsultasView : ICrudView
    {
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        SortedList ParametrosRelatorioMovimentacao { get; }
        SortedList ParametrosRelatorioConsumo { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        void ExibirRelatorioEstoqueAnalitico();
        string TipoMovimentacao { get; set; }
        string TipoConsumo { get; set; }
        string SubItemMaterialAnalitico { get; set; }
        bool AgrupadoPor { get;}
    }
}
