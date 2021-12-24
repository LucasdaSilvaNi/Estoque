using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IReservaMaterialView: ICrudView
    {
        string ItemMaterialId { get; set; }
        string SubItemMaterialId { get; set; }
        string AlmoxarifadoId { get; set; }
        string UgeId { get; set; }
        string Quantidade { get; set; }
        string Data { get; set; }
        string Obs { get; set; }

        void PopularListaReservaMaterial();
        void PopularListaReservaMaterial(int AlmoxarifadoId);
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
