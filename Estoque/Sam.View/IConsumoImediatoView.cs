﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IConsumoImediatoView : ICrudView, IMovimentoView, IMovimentoItemView
    {

        # region Controlar Botoes
        //bool BloqueiaListaDivisao { set; }
        bool BloqueiaListaUGE { set; }
        bool BloqueiaListaUA { set; }
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
        #endregion

        #region Tipo Movimento

        void HabilitarEmpenhoConsumoImediato(bool Editar);
        void HabilitarEmpenhoConsumoImediatoNovo();
        void HabilitarAquisicaoRestosAPagarConsumoImediatoNovo();
        void HabilitarAquisicaoRestosAPagarConsumoImediatoEdit();
        void HabilitarAquisicaoRestosAPagarConsumoImediato(bool Editar);

        #endregion


        string UnidadeCodigo { set; }
        int CodigoUA { set; get; }
        int UaId { set; get; }
        int CodigoUGE { get; set; }
        int? PTResCodigo { get; set; }
        string PTResAcao { get; set; }
        string InscricaoCE { get; set; }
        string InscricaoCEOld { get; set; }
        bool ModoGravacaoOuEstorno { get; }

        # region Controlar Combos/Grids
        void PopularListaFornecedor();
        void PopularListaUGE();
        void PopularListaUnidade();
        //void PopularListaTipoMovimentoEntrada();
        void PopularListaTipoMovimentoConsumoImediato();
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

        void RemoverSessao();
    }
}
