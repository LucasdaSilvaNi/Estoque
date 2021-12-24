using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IUGEView : ICrudView
    {
        string OrgaoId { get; }
        string UoId { get; }
        string UgeTipoId { get; set; }

        bool BloqueiaListaTipoUge { set; }

        void PopularListaOrgao();
        void PopularListaUo(int _orgaoId);
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        bool UgeAtivo { get; set; }
        bool UgeIntegracaoSIAFEM { get; set; }
        bool UgeImplantado { get; set; }
    }
}
