using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface INovaEntradaMaterialView : ICrudView, IMovimentoView, IMovimentoItemView
    {

        void PopularListaTipoMovimentoEntrada();        
        void GetSessao();
        void GetSessaoEdicao();
        void TratarEntrada();
        void PreprarEdicaoEntrada();
        void PrepararNovaEntrada();
        void PreencherDadosEdicao();
        void RemoveSessao();

        void PopularDadosUGETodosCod();
        void PopularDadosSubItemClassif();
        void PopularListaAlmoxarifado();
        void PopularListaEmpenhoEvento();
        void PopularUnidFornecimentoTodosPorGestor();
        void PopularListaDivisao();
        void NotPostBackInicialize();
        void ConfiguracaoPagina();
        void CarregarPermissaoAcesso();

        void PopularDadosSubItemMaterial(string txtSubItem);

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

        #region Mostrar Campos

        //bool VisibleFornecedor { set; }

        #endregion

        #region Tipo Movimento

        #region Tipo Movimento (inicializar)
        void HabilitarCompraDireta(bool Editar);
        void HabilitarAquisicaoAvulsa(bool Editar);
        void HabilitarDoacao(bool Editar);
        void HabilitarDevolucao(bool Editar);
        void HabilitarMaterialTransformado(bool Editar);
        void HabilitarTransferencia(bool Editar);
        #endregion

        #region Tipo Movimento Novo
        void HabilitarCompraDiretaNovo();
        void HabilitarAquisicaoAvulsaNovo();
        void HabilitarTransferenciaNovo();
        void HabilitarDoacaoNovo();
        void HabilitarDevolucaoNovo();
        void HabilitarMaterialTransformadoNovo();
        #endregion
        #endregion


        # region Controlar Combos/Grids
        void PopularListaFornecedor();
        void PopularListaUGE();
        void PopularListaUnidade();
        
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

        # endregion

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
