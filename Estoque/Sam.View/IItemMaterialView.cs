using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IItemMaterialView : ICrudView
    {
        string AtividadeItemMaterialId { get; set; }
        string MaterialId { get; }
        void PopularListaIndicadorAtividade();
        void PopularListaMaterial();
        void PopularListaClasse();
        void PopularListaGrupo();
        void ExibirRelatorio();
        string SiafCodGrupo { get; set; }
        string SiafDescGrupo { get; set; }
        string SiafCodClasse { get; set; }
        string SiafDescClasse { get; set; }
        string SiafCodMaterial { get; set; }
        string SiafDescMaterial { get; set; }
        string SiafCodItem { get; set; }
        string SiafDescItem { get; set; }
        string SiafNatDesp1 { get; set; }
        string SiafNatDesp2 { get; set; }
        string SiafNatDesp3 { get; set; }
        string SiafNatDesp4 { get; set; }
        string SiafNatDesp5 { get; set; }
        bool BloqueiaSiafisico { set; }
        void LimparDadosSiafisico();
        bool ExibeSiafisico { set; }

        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
