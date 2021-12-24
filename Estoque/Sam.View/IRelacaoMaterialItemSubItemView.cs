using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IRelacaoMaterialItemSubItemView : ICrudView
    {
        void PopularListaOrgao();
        void PopularListaGestor();
        void PopularListaGrupo();
        void PopularListaClasse();
        void PopularListaMaterial();
        void PopularListaItem();
        void PopularListaSubItem();

        void PopularListaOrgaoEdit();
        void PopularListaGestorEdit();
        void PopularListaGrupoEdit();
        void PopularListaClasseEdit();
        void PopularListaMaterialEdit();
        void PopularListaItemEdit();

        bool BloqueiaGrupo{ set; }
        bool BloqueiaClasse { set; }
        bool BloqueiaMaterial { set; }
        bool BloqueiaItem { set; }

        string idItemSubItem { get; }
        string ItemId { get; }
        string ItemEditId { get; }
        string SubItemId { get; }
        string GestorId { get; }

        string Grupo { get; set; }
        string Material { get; set; }
        string Classe { get; set; }
        string Item { get; set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        void LimparPesquisaItem();

    }
}
