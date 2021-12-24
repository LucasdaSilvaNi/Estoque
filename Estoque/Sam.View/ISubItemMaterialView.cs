using System;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface ISubItemMaterialView : ICrudView
    {
        void PopularListaOrgao();
        void PopularListaGestor();
        void PopularListaGrupo();
        void PopularListaClasse();
        void PopularListaMaterial();
        void PopularListaItem();
        void PopularListaIndicadorAtividade();
        void PopularListaNaturezaDespeza();
        //void PopularListaContaAuxiliar();
        void ExibirRelatorio();
        void PopularListaUnidadeFornecimento();

        string ItemId { get; }
        int UnidadeFornecimentoId { get; set; }
        string GestorId { get; }
        string NaturezaDespesaId { get; set; }
        //string ContaAuxiliarId { get; set; }
        bool IndicadorAtividadeId { get; set; }
        string CodigoBarras { get; set; }
        bool ExpandeDecimos { get; set; }
        bool PermiteFracionamento { get; set; }
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }

        bool IndicadorAtividadeAlmox { get; set; }
        int IndicadorDisponivelId { get; set; }
        decimal EstoqueMaximo { get; set; }
        decimal EstoqueMinimo { get; set; }

        bool BloqueiaCodBarras { set; }
        bool BloqueiaNaturezaDespesa { set; }
        //bool BloqueiaContaAuxiliar { set; }
        bool BloqueiaExpandeDecimais { set; }
        bool BloqueiaPermiteFacionamento { set; }
        bool BloqueiaAtividade { set; }
        bool BloqueiaUnidadeFornecimento { set; }

        //Todo Edu: Refatorar
        bool BloqueiaListaOrgao { set; }
        bool BloqueiaListaUO { set; }
        bool BloqueiaListaUGE { set; }

        void LimparPesquisaItem();
        bool FiltraAlmoxarifado { get; }
        bool FiltraGestor { get; }
        bool ComSaldo { get; }
    }
}

