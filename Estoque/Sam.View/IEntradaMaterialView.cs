using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IEntradaMaterialView : ICrudView, IMovimentoView, IMovimentoItemView
    {

# region Controlar Botoes
        bool BloqueiaListaDivisao { set; }
        bool BloqueiaListaUGE { set; }
        bool BloqueiaNumeroDocumento { set; }
        bool BloqueiaListaIndicadorAtividade { set; }
        bool BloqueiaNovoItem { set; }
        bool BloqueiaEstornar { set; }
        bool BloqueiaDocumento { set; }
        bool ExibirGeradorDescricao { set; }
        bool ExibirDocumentoAvulso { set; }
        bool BloqueiaGravarItem { set; }
        bool BloqueiaExcluirItem { set; }
        bool BloqueiaItemEfetivado { set; }
        bool BloqueiaValorTotal { set; }
        bool BloqueiaDataRecebimento { set; }
        bool BloqueiaTipoMovimento { set; }
        void LimparGridSubItemMaterial();
# endregion

#region Tipo Movimento

        #region Tipo Movimento (inicializar)
            void HabilitarCompraDireta(bool Editar);
            void HabilitarAquisicaoAvulsa(bool Editar);
            void HabilitarAquisicaoRestosAPagar(bool Editar);
            void HabilitarDoacao(bool Editar);
            void HabilitarDoacaoImplantado(bool Editar);
            void HabilitarDevolucao(bool Editar);
            void HabilitarMaterialTransformado(bool Editar);
            void HabilitarTransferencia(bool Editar);
            void HabilitarOrgaoTransfSemImpl();
            void HabilitarInventario(bool Editar);
        #endregion

        #region Tipo Movimento Novo
            void HabilitarCompraDiretaNovo();
            void HabilitarAquisicaoAvulsaNovo();
            void HabilitarAquisicaoRestosAPagarNovo();
            void HabilitarTransferenciaNovo();
            void HabilitarDoacaoNovo();
            void HabilitarDevolucaoNovo();
            void HabilitarMaterialTransformadoNovo();
            void HabilitarAquisicaoInventario();
        #endregion
#endregion

            string UnidadeCodigo { set; }
            int CodigoUGE { get; set; }
            string InscricaoCE { get; set; }
            string InscricaoCEOld { get; set; }
            bool ModoGravacaoOuEstorno { get; }

        # region Controlar Combos/Grids
        void PopularListaFornecedor();
        void PopularListaUGE();
        void PopularListaUnidade();
        void PopularListaTipoMovimentoEntrada();
        void LimparListaEmpenho();
        void FocarFornecedor();
        void FocarDataDocumento();
        void FocarDataRecebimento();
        void FocarItemQtdeEntrar();
        void FocarItemDataVencLote();
        void FocarValorMovItem();
        void FocarFabricLoteItem();
        void SetarUGELogado();
        bool BloqueiaImprimir { set; }
        bool ExibirDocumentoAvulsoAnoMov { set; }

        string NumeroDocumentoCombo { get; set; }
        string NumeroEmpenhoCombo { get; set; }
        int? EmpenhoEventoId { get; set; }
        int? EmpenhoLicitacaoId { get; set; }
        string DataVctoLoteItemTexto { get; set; }
        decimal? ValorTotalMovimento { get; set; }
        string hiddenMovimentoId { get; set; }
        string ItemMaterialDescricao { get; set; }
        string AlmoxarifadoTransferencia { set; }
        string OrgaoTransferencia { get; set; }

#endregion

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        void RemoverSessao();
    }
}
