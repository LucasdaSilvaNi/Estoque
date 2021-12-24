using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IUAView : ICrudView
    {
       string OrgaoId { get; }
       string UoId { get; }
       string UgeId { get; }
       string UAVinculada { get; set; }
       bool IndicadorAtividadeId { get; set; }
       string UnidadeId { get; set; }
       string CentroCustoId { get; set; }
       string GestorId { get; set; }

       bool BloqueiaListaUnidade { set; }
       bool BloqueiaUAVinculada { set; }
       bool BloqueiaListaCentroCusto { set; }
       bool BloqueiaListaIndicadorAtividade { set; }

       void PopularListaOrgao();
       void PopularListaUO(int? OrgaoID);
       void PopularListaUGE(int? UoID);
       void PopularListaUnidade();
       void PopularListaCentroCusto();
       void PopularListaIndicadorAtividade();
       void ListarUasTodosCodPorOrgao(int? OrgaoID);

       void ExibirRelatorio();
       SortedList ParametrosRelatorio { get; }
       RelatorioEntity DadosRelatorio { get; set; }
    }
}
