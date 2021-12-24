using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IUnidadeFornecimentoConversaoView : ICrudView
    {
        int GestorId { get; }
        int OrgaoId  { get; }
        int UnidadeFornecimentoSistemaSamId { get; }
        int UnidadeFornecimentoSistemaSiafisicoId { get; }
        decimal FatorUnitario { get; set; }
        void ExibirRelatorio();
        void PopularListaUnidadeFornecimentoSistemaSAM(int orgaoId, int gestorId);
        void PopularListaUnidadeFornecimentoSistemaSiafisico();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
