using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IUnidadeFornecimentoView : ICrudView
    {
        string OrgaoId { get; }
        string GestorId { get; }
        void PopularListaOrgao();
        void PopularListaGestor(int OrgaoId);
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
