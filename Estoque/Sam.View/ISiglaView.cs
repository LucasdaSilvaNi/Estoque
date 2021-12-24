using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface ISiglaView : ICrudView
    {
        string OrgaoId { get; }
        string GestorId { get; }
        string IndicadorBemProprioId { get; set; }
        
        void PopularListaOrgao();
        void PopularListaGestor(int OrgaoId);
        void PopularListaIndicadorBemProprio();

        bool BloqueiaListaIndicadorBemProprio { set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
