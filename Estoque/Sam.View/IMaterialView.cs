using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IMaterialView : ICrudView
    {
        void PopularListaGrupo();
        void PopularListaClasse();
        string ClasseId { get; }
        void ExibirRelatorio();
        SortedList ParametrosRelatorio {get;}
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
