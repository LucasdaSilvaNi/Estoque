using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.View;
using System.Data;
using System.Text;
using System.IO;
using System.Drawing;
using Sam.Common.Util;
using Sam.Domain.Entity;


namespace Sam.Web.Almoxarifado
{
    public partial class Exportacao : PageBase
    {

        #region Propriedades

        #region Exportação Analitica
        int p_orgaoId;
        string p_periodoDe = "";
        string p_periodoAte = "";
        string p_Fornecedor = "";
        string p_Lote = "";
        string p_NaturezaDespesaCodigo = "";
        string p_NumeroDocumento = "";
        string p_QuantidadeSaldo = "";
        string p_ValorSaldo = "";
        string p_SubitemCodigo = "";
        string p_SituacaoId = "";
        string p_TipoMovimento = "";
        string p_UGECodigo = "";
        string p_NLLiquidacao = "";
        string p_NLLiquidacaoEstorno = "";
        string p_NLConsumo = "";
        string p_UA_Id_Destino = "";
        string p_UGE_Id_Destino = "";
        #endregion

        #region Exportação Sintetica
        #endregion
        string p_Grupo = "";
        const int rowLimit = 65000;
        #endregion

        #region Métodos comuns
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.btnExportar);

            if (!this.IsPostBack)
            {
                PopularCombosPeriodoAnatitica("");
                PopularComboTipoExportacao();
            }
        }

        public void PopularDadosGrupoTodosCod()
        {
            GrupoPresenter grp = new GrupoPresenter();
            ddlGrupoFiltroExportaSintetico.Items.Clear();
            ddlGrupoFiltroExportaSintetico.DataTextField = "Descricao";
            ddlGrupoFiltroExportaSintetico.DataValueField = "Id";
            ddlGrupoFiltroExportaSintetico.Items.Add("- Todos -");
            ddlGrupoFiltroExportaSintetico.Items[0].Value = "0";
            ddlGrupoFiltroExportaSintetico.AppendDataBoundItems = true;
            ddlGrupoFiltroExportaSintetico.DataSource = grp.PopularDadosGrupoTodosCod(0, 0); // órgão do perfil
            ddlGrupoFiltroExportaSintetico.DataBind();

        }

        public void PopularComboTipoExportacao()
        {
            this.ddlTipoExportacao.Items.Add(new ListItem("Selecione um tipo de Exportação", "0"));
            this.ddlTipoExportacao.Items.Add(new ListItem("Analítica", ((int)GeralEnum.TipoExportacao.Analitica).ToString()));
            this.ddlTipoExportacao.Items.Add(new ListItem("Sintética", ((int)GeralEnum.TipoExportacao.Sintetica).ToString()));
        }

        private void GerarExcel(DataTable listaExportacao, string nomeDoArquivo)
        {
            var grdExport = new GridView();
            grdExport.DataSource = listaExportacao;
            grdExport.DataBind();

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + nomeDoArquivo + ".xls");
            Response.Charset = "";

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

            grdExport.RenderControl(htmlWrite);
            Response.Write(stringWrite.ToString());

            Response.End();
        }

        protected void ddlTipoExportacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Convert.ToInt32(this.ddlTipoExportacao.SelectedValue))
            {
                case (int)GeralEnum.TipoExportacao.Analitica:
                    this.pnlExportacaoAnalitica.Visible = true;
                    this.pnlExportacaoSintetico.Visible = false;
                    this.pnlExportacaoConsumoMedio.Visible = false;
                    break;
                case (int)GeralEnum.TipoExportacao.Sintetica:
                    this.pnlExportacaoAnalitica.Visible = false;
                    this.pnlExportacaoSintetico.Visible = true;
                    this.pnlExportacaoConsumoMedio.Visible = false;
                    break;
                case (int)GeralEnum.TipoExportacao.ConsumoMedio:
                    this.pnlExportacaoAnalitica.Visible = false;
                    this.pnlExportacaoSintetico.Visible = false;
                    this.pnlExportacaoConsumoMedio.Visible = true;
                    break;
                default:
                    this.pnlExportacaoAnalitica.Visible = false;
                    this.pnlExportacaoSintetico.Visible = false;
                    this.pnlExportacaoConsumoMedio.Visible = false;
                    break;
            }
        }
        protected void ddlUGEDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulaListaUA();
        }

        protected void btnGerarFiltros_Click(object sender, EventArgs e)
        {
            if (this.chkFiltroUGEAnalitica.Checked)
            {
                this.PopularListaUGE();
                this.divValorFiltroUGE.Visible = this.chkFiltroUGEAnalitica.Checked;
            }
            else
            {
                this.PopularListaUGE();
                this.divValorFiltroUGE.Visible = this.chkUgeFiltroExportaSintetico.Checked;
            }

            if (this.chkFiltroUaCodigoDestino.Checked)
            {
                this.PopulaListaUA();
                this.divValorFiltroUACodigo.Visible = true;
            }
            else
            {
                this.divValorFiltroUACodigo.Visible = false;
            }


            if (this.chkFiltroUGECodigoDestino.Checked)
            {
                this.PopularListaUGEDestino();
                this.divValorFiltroUGEDestino.Visible = true;
            }
            else
            {
                this.PopularListaUGEDestino();
                this.divValorFiltroUGEDestino.Visible = false;
            }

            switch (Convert.ToInt32(this.ddlTipoExportacao.SelectedValue))
            {
                case (int)GeralEnum.TipoExportacao.Analitica:
                    this.divValorFiltroFornecedorAnalitica.Visible = chkFiltroFornecedorAnalitica.Checked;
                    this.divValorFiltroloteAnalitica.Visible = chkFiltroLoteAnalitica.Checked;
                    this.divValorFiltroNaturezaDespesaAnalitica.Visible = chkFiltroNaturezaDespesaAnalitica.Checked;
                    this.divValorFiltroNumeroDocumentoAnalitica.Visible = chkFiltroNumeroDocumentoAnalitica.Checked;
                    this.divValorFiltroSaldoQuantidadeAnalitica.Visible = chkFiltroSaldoQuantidadeAnalitica.Checked;
                    this.divValorFiltroSaldoValorAnalitica.Visible = chkFiltroSaldoValorAnalitica.Checked;
                    this.divValorFiltroSituacaoAnalitica.Visible = chkFiltroSituacaoAnalitica.Checked;
                    this.divValorFiltroTipoMovimentoAnalitica.Visible = chkFiltroTipoMovimentoAnalitica.Checked;
                    this.divValorFiltroSubitem.Visible = chkFiltroSubitemAnalitica.Checked;
                    this.divValorFiltroNLConsumo.Visible = this.chkFiltroNLConsumoAnalitica.Checked;
                    this.divValorFiltroNLLiquidacao.Visible = this.chkFiltroNLLiquidacao.Checked;
                    this.divValorFiltroNLLiquidacaoEstorno.Visible = this.chkFiltroNLLiquidacaoEstorno.Checked;

                    if (this.chkFiltroUGEAnalitica.Checked)
                    {
                        this.PopularListaUGE();
                    }
                    if (this.chkFiltroUaCodigoDestino.Checked)
                    {
                        this.PopularComboUGEDestino();
                        this.PopulaListaUA();
                    }
                    break;
                case (int)GeralEnum.TipoExportacao.Sintetica:
                    if (this.chkGrupoFiltroExportaSintetico.Checked)
                        this.PopularDadosGrupoTodosCod();
                    this.divValorFiltroGrupoExportaSintetico.Visible = chkGrupoFiltroExportaSintetico.Checked;
                    break;
                default:
                    break;
            }

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            switch (Convert.ToInt32(ddlTipoExportacao.SelectedValue))
            {
                case (int)GeralEnum.TipoExportacao.Analitica:
                    this.MontarFiltrosExportacaoAnalitica();
                    this.GerarExportacaoAnalitica();
                    break;
                case (int)GeralEnum.TipoExportacao.Sintetica:
                    this.MontarFiltrosExportacaoSintetica();
                    this.GerarExportacaoSintetica();
                    break;
                default:
                    break;
            }

        }

        private void PopularListaUGE()
        {
            int orgaoPadrao = 0;
            int gestorPadrao = 0;

            if (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null)
                orgaoPadrao = (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;

            if (GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id != null)
                gestorPadrao = (int)GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id;


            this.lstUGEAnalitica.Items.Clear();
            if (orgaoPadrao != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UGEEntity> listaUge = presenter.PopularUGETodos(orgaoPadrao);

                if (listaUge.Count > 0)
                {

                    this.lstUGEAnalitica.DataValueField = "Codigo";
                    this.lstUGEAnalitica.DataTextField = "Descricao";
                    this.lstUGEAnalitica.DataSource = listaUge;
                    this.lstUGEAnalitica.DataBind();
                }
            }
            lstUGEAnalitica.DataSource = null;
            lstUGEAnalitica.DataBind();
        }

        private void PopularListaUGEDestino()
        {
            int orgaoPadrao = 0;
            int gestorPadrao = 0;

            if (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null)
                orgaoPadrao = (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;

            if (GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id != null)
                gestorPadrao = (int)GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id;

            this.lstUGECodigoDestinoDisponivel.Items.Clear();
            if (orgaoPadrao != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UGEEntity> listaUge = presenter.PopularUGETodos(orgaoPadrao);

                if (listaUge.Count > 0)
                {

                    this.lstUGECodigoDestinoDisponivel.DataValueField = "Id";
                    this.lstUGECodigoDestinoDisponivel.DataTextField = "Descricao";
                    this.lstUGECodigoDestinoDisponivel.DataSource = listaUge;
                    this.lstUGECodigoDestinoDisponivel.DataBind();
                }
            }
            lstUGECodigoDestinoDisponivel.DataSource = null;
            lstUGECodigoDestinoDisponivel.DataBind();
        }

        private void PopularComboUGEDestino()
        {
            int orgaoPadrao = 0;
            int gestorPadrao = 0;

            if (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null)
                orgaoPadrao = (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;

            if (GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id != null)
                gestorPadrao = (int)GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id;

            this.ddlUGEDestino.Items.Clear();
            if (orgaoPadrao != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UGEEntity> listaUge = presenter.PopularUGETodos(orgaoPadrao);

                if (listaUge.Count > 0)
                {

                    this.ddlUGEDestino.DataValueField = "Id";
                    this.ddlUGEDestino.DataTextField = "Descricao";
                    this.ddlUGEDestino.DataSource = listaUge;
                    this.ddlUGEDestino.DataBind();
                }
            }
            ddlUGEDestino.DataSource = null;
            ddlUGEDestino.DataBind();
        }

        private void PopulaListaUA()
        {
            int orgaoPadrao = 0;

            if (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null)
                orgaoPadrao = (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;

            this.lstUACodigoDestinoDisponivel.Items.Clear();
            this.lstUACodigoDestinoSelecionados.Items.Clear();
            if (orgaoPadrao != 0)
            {
                int UgeIdDestinoSelecionado = ddlUGEDestino.SelectedValue != "" ? int.Parse(ddlUGEDestino.SelectedValue) : 0;

                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UAEntity> listaUas = presenter.PopularUA(UgeIdDestinoSelecionado);

                if (listaUas.Count > 0)
                {

                    this.lstUACodigoDestinoDisponivel.DataValueField = "Id";
                    this.lstUACodigoDestinoDisponivel.DataTextField = "CodigoDescricao";
                    this.lstUACodigoDestinoDisponivel.DataSource = listaUas;
                    this.lstUACodigoDestinoDisponivel.DataBind();
                }
            }
            lstUACodigoDestinoDisponivel.DataSource = null;
            lstUACodigoDestinoDisponivel.DataBind();
        }

        #endregion

        #region Métodos Exportação Dinamica Analitica

        private void MontarFiltrosExportacaoAnalitica()
        {
            #region Orgão
            p_orgaoId = Convert.ToInt32(GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id);
            #endregion
            #region Periodo

            p_periodoDe = ddlPeriodoDeExpAnalitica.SelectedValue;
            p_periodoDe = p_periodoDe.Substring(0, 4) + "-" + p_periodoDe.Substring(4, 2) + "-" + "01 00:00:00.000";

            p_periodoAte = ddlPeriodoAteExpAnalitica.SelectedValue;
            p_periodoAte = p_periodoAte.Substring(0, 4) + "-" + p_periodoAte.Substring(4, 2) + "-" + DateTime.DaysInMonth(Convert.ToInt32(p_periodoAte.Substring(0, 4)), Convert.ToInt32(p_periodoAte.Substring(4, 2))) + " 23:59:59.999";

            #endregion
            #region Fornecedor
            if (chkFiltroFornecedorAnalitica.Checked && !String.IsNullOrEmpty(txtValorFiltroFornecedorAnalitica.Text))
            {
                p_Fornecedor = txtCodFornecedor.Text.Trim();
            }
            #endregion
            #region Lote
            if (chkFiltroLoteAnalitica.Checked)
            {
                if (!String.IsNullOrEmpty(txtValorFiltroLoteDescricaoAnalitica.Text))
                {
                    p_Lote = " = ";
                    p_Lote += txtValorFiltroLoteDescricaoAnalitica.Text.Trim();
                }
                if (!String.IsNullOrEmpty(txtValorFiltroLoteDataAnalitica.Text))
                {
                    //p_VencimentoDocumento = " = ";
                    //p_VencimentoDocumento += Convert.ToDateTime(txtValorFiltroLoteDataAnalitica.Text).ToString();
                }
            }
            #endregion
            #region Natureza Despesa
            if (chkFiltroNaturezaDespesaAnalitica.Checked && !String.IsNullOrEmpty(txtValorFiltroNaturezaDespesaAnalitica.Text))
            {
                p_NaturezaDespesaCodigo = " = ";
                p_NaturezaDespesaCodigo = txtValorFiltroNaturezaDespesaAnalitica.Text.Trim();
            }
            #endregion
            #region Numero Documento

            #endregion
            #region Saldo Quantidade
            //p_QuantidadeSaldo = "";
            #endregion
            #region Saldo Valor
            //p_ValorSaldo = "";
            #endregion
            #region Subitem
            if (chkFiltroSubitemAnalitica.Checked && !String.IsNullOrEmpty(txtSubItem.Text))
                p_SubitemCodigo += txtSubItem.Text.Trim();

            #endregion
            #region Tipo Movimento
            if (chkFiltroTipoMovimentoAnalitica.Checked)
            {
                if (CheckEntrada() || CheckSaida() || CheckRequisicao())
                {
                    p_TipoMovimento = " IN(";

                    #region Movimento Entrada

                    if (chkValorFiltroTMEntrada.Checked)
                    {
                        if (chkValorFiltroTMEEmpenho.Checked)
                            //p_TipoMovimento += ((int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho).ToString() + ",";
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorEmpenho).ToString() + ",";
                        if (chkValorFiltroTMEAvulsa.Checked)
                            //p_TipoMovimento += ((int)GeralEnum.TipoMovimento.AquisicaoAvulsa).ToString() + ",";
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaAvulsa).ToString() + ",";
                        if (chkValorFiltroTMETransferencia.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorTransferencia).ToString() + ",";
                        if (chkValorFiltroTMEDoacao.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorDoacao).ToString() + ",";
                        if (chkValorFiltroTMEDoacaoOrgaoImplantado.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado).ToString() + ",";
                        if (chkValorFiltroTMEDevolucao.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorDevolucao).ToString() + ",";
                        if (chkValorFiltroTMETransformado.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado).ToString() + ",";
                    }

                    #endregion

                    #region Movimento Saida

                    if (chkValorFiltroTMSaida.Checked)
                    {
                        if (chkValorFiltroTMSAprovada.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.RequisicaoAprovada).ToString() + ",";
                        if (chkValorFiltroTMSTransferencia.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.SaidaPorTransferencia).ToString() + ",";
                        if (chkValorFiltroTMSDoacao.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.SaidaPorDoacao).ToString() + ",";
                        if (chkValorFiltroTMSOutros.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.OutrasSaidas).ToString() + ",";
                    }


                    #endregion

                    #region Requisição
                    if (chkValorFiltroTMRequisicao.Checked)
                    {
                        if (chkValorFiltroTMRPendente.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.RequisicaoPendente).ToString() + ",";
                        if (chkValorFiltroTMRFinalizada.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.RequisicaoFinalizada).ToString() + ",";
                        if (chkValorFiltroTMRCancelada.Checked)
                            p_TipoMovimento += ((int)GeralEnum.TipoMovimento.RequisicaoCancelada).ToString() + ",";
                    }
                    #endregion

                    p_TipoMovimento += ")";
                    p_TipoMovimento = p_TipoMovimento.Remove(p_TipoMovimento.IndexOf(")") - 1, 1);
                }
            }
            else
                p_TipoMovimento = "";
            #endregion
            #region UGE
            if (chkFiltroUGEAnalitica.Checked && !String.IsNullOrEmpty(txtValorFiltroUGEAnalitica.Text))
            {
                p_UGECodigo = " IN(";
                p_UGECodigo += txtValorFiltroUGEAnalitica.Text.Trim();
                p_UGECodigo += ")";
                p_UGECodigo = p_UGECodigo.Remove(p_UGECodigo.IndexOf(")") - 1, 1);
            }

            #endregion
            #region NL Liquidação
            if (chkFiltroNLLiquidacao.Checked && !String.IsNullOrEmpty(txtValorFiltroNLLiquidacaoAnalitica.Text))
            {
                p_NLLiquidacao = " IN(";
                p_NLLiquidacao += "'" + txtValorFiltroNLLiquidacaoAnalitica.Text.Trim() + "'";
                p_NLLiquidacao += ")";
            }
            #endregion
            #region NL Consumo
            if (chkFiltroNLConsumoAnalitica.Checked && !String.IsNullOrEmpty(txtValorFiltroNLConsumoAnalitica.Text))
            {
                p_NLConsumo = " IN(";
                p_NLConsumo += "'" + txtValorFiltroNLConsumoAnalitica.Text.Trim() + "'";
                p_NLConsumo += ")";
            }
            #endregion
            #region NL Liquidação Estorno
            if (chkFiltroNLLiquidacaoEstorno.Checked && !String.IsNullOrEmpty(txtValorFiltroNLLiquidacaoEstornoAnalitica.Text))
            {
                p_NLLiquidacaoEstorno = " IN(";
                p_NLLiquidacaoEstorno += "'" + txtValorFiltroNLLiquidacaoEstornoAnalitica.Text.Trim() + "'";
                p_NLLiquidacaoEstorno += ")";
            }
            #endregion

            #region UA Código (Destino)
            if (chkFiltroUaCodigoDestino.Checked && !String.IsNullOrEmpty(txtValorFiltroUADestino.Text))
            {
                p_UA_Id_Destino = " IN(";
                p_UA_Id_Destino += txtValorFiltroUADestino.Text.Trim();
                p_UA_Id_Destino += ")";
                p_UA_Id_Destino = p_UA_Id_Destino.Remove(p_UA_Id_Destino.IndexOf(")") - 1, 1);
            }
            #endregion
            #region UGE Código (Destino)
            if (chkFiltroUGECodigoDestino.Checked && !String.IsNullOrEmpty(txtValorFiltroUGEDestino.Text))
            {
                p_UGE_Id_Destino = " IN(";
                p_UGE_Id_Destino += txtValorFiltroUGEDestino.Text.Trim();
                p_UGE_Id_Destino += ")";
                p_UGE_Id_Destino = p_UGE_Id_Destino.Remove(p_UGE_Id_Destino.IndexOf(")") - 1, 1);
            }
            #endregion
        }

        private void GerarExportacaoAnalitica()
        {
            MovimentoPresenter presenter = new MovimentoPresenter();
            DataSet dsRetorno = new DataSet();
            string mensagemExcessoReg = "A consulta retornou uma grande quantidade de registros, acrescente filtros para diminuir a consulta e possibilitar a geração da planilha.";
            string mensagemNenhumReg = "O retorno da consulta não retornou nenhum registro.";

            try
            {
                dsRetorno = presenter.GerarExportacaoAnalitica(p_orgaoId
                                          , Convert.ToInt16(chkExportaPeriodoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaUgeCodigoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaUgeDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNaturezaDespesaCodigoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNaturezaDespesaDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExpotaSubItemCodigoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaSubItemDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaUnidFornecimentoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaDataMovimentoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaTipoMovimentoDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaSituacaoDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaFornecedorDescricaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNumeroDocumentoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaLoteAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaVencimentoDocAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaQtdeDocAnalitica.Checked)

                                          , Convert.ToInt16(chkExportaPrecoUnitarioEMPAnalitica.Checked)

                                          , Convert.ToInt16(chkExportaValorUnitarioAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaDesdobroAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaTotalDocAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaQtdeSaldoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaValorSaldoAnalitica.Checked)

                                          , Convert.ToInt16(chkExportaUaCodigoRequisicao.Checked)
                                          , Convert.ToInt16(chkExportaUaDescricaoRequisicao.Checked)
                                          , Convert.ToInt16(chkExportaDivisao.Checked)
                                          , Convert.ToInt16(chkExportaUGECodigoDestino.Checked)
                                          , Convert.ToInt16(chkExportaUGEDescricaoDestino.Checked)

                                          , Convert.ToInt16(chkExportaObservacoesMovimentoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNLConsumoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNLLiquidacaoAnalitica.Checked)
                                          , Convert.ToInt16(chkExportaNLLiquidacaoEstornoAnalitica.Checked)
                                          , p_periodoDe
                                          , p_periodoAte
                                          , p_Fornecedor
                                          , p_Lote
                                          , p_NaturezaDespesaCodigo
                                          , p_NumeroDocumento
                                          , p_QuantidadeSaldo
                                          , p_ValorSaldo
                                          , p_SubitemCodigo
                                          , p_TipoMovimento
                                          , p_UGECodigo
                                          , p_NLConsumo
                                          , p_NLLiquidacao
                                          , p_NLLiquidacaoEstorno
                                          , p_UA_Id_Destino
                                          , p_UGE_Id_Destino
                                          );

                if (dsRetorno.Tables[0].Rows.Count > 0)
                {
                    if (dsRetorno.Tables[0].Rows.Count > 5000)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + mensagemExcessoReg + "');", true);
                    }
                    else
                    {
                        GerarExcel(dsRetorno.Tables[0], "ExportacaoAnalitica");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('"+ mensagemNenhumReg +"');", true);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && (ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("tempo limite")))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + mensagemExcessoReg + "');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + ex.Message + "');", true);
                }
            }
        }

        private bool CheckEntrada()
        {
            bool retorno = false;

            if (chkValorFiltroTMEEmpenho.Checked)
                retorno = true;
            if (chkValorFiltroTMEAvulsa.Checked)
                retorno = true;
            if (chkValorFiltroTMETransferencia.Checked)
                retorno = true;
            if (chkValorFiltroTMEDoacao.Checked)
                retorno = true;
            if (chkValorFiltroTMEDoacaoOrgaoImplantado.Checked)
                retorno = true;
            if (chkValorFiltroTMEDevolucao.Checked)
                retorno = true;
            if (chkValorFiltroTMETransformado.Checked)
                retorno = true;

            return retorno;
        }

        private bool CheckSaida()
        {
            bool retorno = false;

            if (chkValorFiltroTMSAprovada.Checked)
                retorno = true;
            if (chkValorFiltroTMSTransferencia.Checked)
                retorno = true;
            if (chkValorFiltroTMSDoacao.Checked)
                retorno = true;
            if (chkValorFiltroTMSOutros.Checked)
                retorno = true;

            return retorno;
        }

        private bool CheckRequisicao()
        {
            bool retorno = false;

            if (chkValorFiltroTMRPendente.Checked)
                retorno = true;
            if (chkValorFiltroTMRFinalizada.Checked)
                retorno = true;
            if (chkValorFiltroTMRCancelada.Checked)
                retorno = true;

            return retorno;
        }

        public void PopularCombosPeriodoAnatitica(string mesRefInicial)
        {
            DateTime data = DateTime.Now;
            DateTime dataIni;

            if (String.IsNullOrEmpty(mesRefInicial))
            {
                mesRefInicial = "201304";
                dataIni = new DateTime(Convert.ToInt32(mesRefInicial.Substring(0, 4)),
                    Convert.ToInt32(mesRefInicial.Substring(4, 2)), 1);

                this.ddlPeriodoAteExpAnalitica.Items.Clear();
                this.ddlPeriodoDeExpAnalitica.Items.Clear();

                for (DateTime dat = new DateTime(Convert.ToInt32(mesRefInicial.Substring(0, 4)),
                    Convert.ToInt32(mesRefInicial.Substring(4, 2)), 1); dat < DateTime.Now; dat = dat.AddMonths(1))
                {
                    this.ddlPeriodoDeExpAnalitica.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                    this.ddlPeriodoDeExpAnalitica.Items[this.ddlPeriodoDeExpAnalitica.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
                }

                for (DateTime dat = new DateTime(Convert.ToInt32(mesRefInicial.Substring(0, 4)),
                    Convert.ToInt32(mesRefInicial.Substring(4, 2)), 1); dat < dataIni.AddMonths(12); dat = dat.AddMonths(1))
                {
                    this.ddlPeriodoAteExpAnalitica.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                    this.ddlPeriodoAteExpAnalitica.Items[this.ddlPeriodoAteExpAnalitica.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
                }
            }
            else
            {
                dataIni = new DateTime(Convert.ToInt32(mesRefInicial.Substring(0, 4)),
                    Convert.ToInt32(mesRefInicial.Substring(4, 2)), 1);

                this.ddlPeriodoAteExpAnalitica.Items.Clear();
                for (DateTime dat = new DateTime(Convert.ToInt32(mesRefInicial.Substring(0, 4)),
                    Convert.ToInt32(mesRefInicial.Substring(4, 2)), 1); dat < dataIni.AddMonths(12); dat = dat.AddMonths(1))
                {
                    this.ddlPeriodoAteExpAnalitica.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                    this.ddlPeriodoAteExpAnalitica.Items[this.ddlPeriodoAteExpAnalitica.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
                }
            }
        }

        protected void btnFiltrosAnalitica_Click(object sender, EventArgs e)
        {
            divCamposFiltrosAnalitica.Visible = true;
        }

        protected void chkExportaTodosAnalitica_CheckedChanged(object sender, EventArgs e)
        {
            chkExportaTodosAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaPeriodoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaObservacoesMovimentoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUgeCodigoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUgeDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaDataMovimentoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNaturezaDespesaCodigoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNaturezaDespesaDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUnidFornecimentoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExpotaSubItemCodigoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaSubItemDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaTipoMovimentoDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            //chkExportaSituacaoDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaFornecedorDescricaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNumeroDocumentoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaLoteAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaVencimentoDocAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaQtdeDocAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaValorUnitarioAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaDesdobroAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaTotalDocAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaQtdeSaldoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaValorSaldoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNLConsumoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNLLiquidacaoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaNLLiquidacaoEstornoAnalitica.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUaCodigoRequisicao.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUaDescricaoRequisicao.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaDivisao.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUGECodigoDestino.Checked = chkExportaTodosAnalitica.Checked;
            chkExportaUGEDescricaoDestino.Checked = chkExportaTodosAnalitica.Checked;


            chkExportaPrecoUnitarioEMPAnalitica.Checked = chkExportaTodosAnalitica.Checked;
        }

        protected void chkFiltroSelecionaTodosAnalitica_CheckedChanged(object sender, EventArgs e)
        {
            chkFiltroFornecedorAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroLoteAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroNaturezaDespesaAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroNumeroDocumentoAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroSaldoQuantidadeAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroSaldoValorAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            //chkFiltroSituacaoAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroSubitemAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroTipoMovimentoAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroUGEAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroNLLiquidacao.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroNLLiquidacaoEstorno.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroNLConsumoAnalitica.Checked = chkFiltroSelecionaTodosAnalitica.Checked;

            chkFiltroUaCodigoDestino.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
            chkFiltroUGECodigoDestino.Checked = chkFiltroSelecionaTodosAnalitica.Checked;
        }

        protected void ImgLupaFornecedor_Click(object sender, ImageClickEventArgs e)
        {
            SetSession(txtCodFornecedor, "fornecedorId");
            SetSession(txtValorFiltroFornecedorAnalitica, "fornecedorDados");
        }

        protected void chkValorFiltroTMEntrada_CheckedChanged(object sender, EventArgs e)
        {
            divValorFiltroTMTodasEntradas.Visible = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMEEmpenho.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMEAvulsa.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMETransferencia.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMEDoacao.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMEDoacaoOrgaoImplantado.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMEDevolucao.Checked = chkValorFiltroTMEntrada.Checked;
            chkValorFiltroTMETransformado.Checked = chkValorFiltroTMEntrada.Checked;
        }

        protected void chkValorFiltroTMSaida_CheckedChanged(object sender, EventArgs e)
        {
            divValorFiltroTMTodasSaidas.Visible = chkValorFiltroTMSaida.Checked;
            chkValorFiltroTMSAprovada.Checked = chkValorFiltroTMSaida.Checked;
            chkValorFiltroTMSTransferencia.Checked = chkValorFiltroTMSaida.Checked;
            chkValorFiltroTMSDoacao.Checked = chkValorFiltroTMSaida.Checked;
            chkValorFiltroTMSOutros.Checked = chkValorFiltroTMSaida.Checked;
        }

        protected void chkValorFiltroTMRequisicao_CheckedChanged(object sender, EventArgs e)
        {
            divValorFiltroTMTodasRequisicao.Visible = chkValorFiltroTMRequisicao.Checked;
            chkValorFiltroTMRPendente.Checked = chkValorFiltroTMRequisicao.Checked;
            chkValorFiltroTMRFinalizada.Checked = chkValorFiltroTMRequisicao.Checked;
            chkValorFiltroTMRCancelada.Checked = chkValorFiltroTMRequisicao.Checked;
        }

        protected void imgSubItemMaterial_Click(object sender, ImageClickEventArgs e)
        {
            SetSession(txtSubItem, "descricaoSelecionado");
        }

        protected void btnSelecionar_Click(object sender, EventArgs e)
        {
            lstValorFiltroUGEAnalitica.Items.Add(new ListItem(lstUGEAnalitica.SelectedItem.Text, lstUGEAnalitica.SelectedItem.Value));

            string strValorUge = "";
            strValorUge = txtValorFiltroUGEAnalitica.Text;
            strValorUge = strValorUge + lstUGEAnalitica.SelectedItem.Text.Substring(0, 6) + ",";
            txtValorFiltroUGEAnalitica.Text = strValorUge;

            lstUGEAnalitica.SelectedItem.Enabled = false;
        }

        protected void btnSelecionarUA_Click(object sender, EventArgs e)
        {
            lstUACodigoDestinoSelecionados.Items.Add(new ListItem(lstUACodigoDestinoDisponivel.SelectedItem.Text, lstUACodigoDestinoDisponivel.SelectedItem.Value));

            string strValorUa = "";
            strValorUa = txtValorFiltroUADestino.Text;
            strValorUa = strValorUa + lstUACodigoDestinoDisponivel.SelectedValue + ",";
            txtValorFiltroUADestino.Text = strValorUa;

            lstUACodigoDestinoDisponivel.SelectedItem.Enabled = false;
        }

        protected void btnSelecionarUgeDestino_Click(object sender, EventArgs e)
        {
            lstUGECodigoDestinoSelecionados.Items.Add(new ListItem(lstUGECodigoDestinoDisponivel.SelectedItem.Text, lstUGECodigoDestinoDisponivel.SelectedItem.Value));

            string strValorUgeDestino = "";
            strValorUgeDestino = txtValorFiltroUGEDestino.Text;
            strValorUgeDestino = strValorUgeDestino + lstUGECodigoDestinoDisponivel.SelectedValue + ",";
            txtValorFiltroUGEDestino.Text = strValorUgeDestino;

            lstUGECodigoDestinoDisponivel.SelectedItem.Enabled = false;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            txtValorFiltroUGEAnalitica.Text = txtValorFiltroUGEAnalitica.Text.Replace(lstValorFiltroUGEAnalitica.SelectedItem.Text.Substring(0, 6) + ",", "");
            lstValorFiltroUGEAnalitica.Items.Remove(new ListItem(lstValorFiltroUGEAnalitica.SelectedItem.Text, lstValorFiltroUGEAnalitica.SelectedItem.Value));

        }

        protected void btnExcluirUA_Click(object sender, EventArgs e)
        {
            txtValorFiltroUADestino.Text = txtValorFiltroUADestino.Text.Replace(lstUACodigoDestinoSelecionados.SelectedItem.Text.Substring(0, 6) + ",", "");

            lstUACodigoDestinoDisponivel.Items.Add(new ListItem(lstUACodigoDestinoSelecionados.SelectedItem.Text, lstUACodigoDestinoSelecionados.SelectedItem.Value));
            lstUACodigoDestinoSelecionados.Items.Remove(new ListItem(lstUACodigoDestinoSelecionados.SelectedItem.Text, lstUACodigoDestinoSelecionados.SelectedItem.Value));
        }

        protected void btnExcluirUgeDestino_Click(object sender, EventArgs e)
        {
            txtValorFiltroUGEDestino.Text = txtValorFiltroUGEDestino.Text.Replace(lstUGECodigoDestinoSelecionados.SelectedItem.Text.Substring(0, 6) + ",", "");

            lstUGECodigoDestinoDisponivel.Items.Add(new ListItem(lstUGECodigoDestinoSelecionados.SelectedItem.Text, lstUGECodigoDestinoSelecionados.SelectedItem.Value));
            lstUGECodigoDestinoSelecionados.Items.Remove(new ListItem(lstUGECodigoDestinoSelecionados.SelectedItem.Text, lstUGECodigoDestinoSelecionados.SelectedItem.Value));
        }

        protected void ddlPeriodoDeExpAnalitica_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularCombosPeriodoAnatitica(ddlPeriodoDeExpAnalitica.SelectedValue);
        }
        #endregion

        #region Métodos Exportação Dinamica Sintetica

        protected void chkTodosExportaSintetico_CheckedChanged(object sender, EventArgs e)
        {
            chkUgeCodigoExportaSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkUgeDescricaoExportaSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkGrupoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkSubitemCodigoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkSubitemDescricaoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkUnidadeFornecExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkSaldoQtdeExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkSaldoValorExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkPrecoMedioExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkNDCodigoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkNDDescricaoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkLoteDescricaoExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
            chkLoteQtdeExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked; ;
            chkLoteDataVencExportacaoSintetico.Checked = chkTodosExportaSintetico.Checked;
        }

        private void MontarFiltrosExportacaoSintetica()
        {
            #region Orgão
            p_orgaoId = Convert.ToInt32(GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id);
            #endregion
            #region Periodo

            p_periodoDe = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            p_periodoDe = p_periodoDe.Substring(0, 4) + "-" + p_periodoDe.Substring(4, 2) + "-" + "01 00:00:00.000";

            p_periodoAte = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            p_periodoAte = p_periodoAte.Substring(0, 4) + "-" + p_periodoAte.Substring(4, 2) + "-" + DateTime.DaysInMonth(Convert.ToInt32(p_periodoAte.Substring(0, 4)), Convert.ToInt32(p_periodoAte.Substring(4, 2))) + " 23:59:59.999";

            #endregion
            #region UGE
            if (chkUgeFiltroExportaSintetico.Checked && !String.IsNullOrEmpty(txtValorFiltroUGEAnalitica.Text))
            {
                p_UGECodigo = " IN(";
                p_UGECodigo += txtValorFiltroUGEAnalitica.Text.Trim();
                p_UGECodigo += ")";
                p_UGECodigo = p_UGECodigo.Remove(p_UGECodigo.IndexOf(")") - 1, 1);
            }

            #endregion
            #region Grupo Material
            if (chkGrupoFiltroExportaSintetico.Checked && ddlGrupoFiltroExportaSintetico.SelectedIndex > 0)
                p_Grupo += ddlGrupoFiltroExportaSintetico.SelectedValue;

            #endregion

            
        }

        private void GerarExportacaoSintetica()
        {
            MovimentoPresenter presenter = new MovimentoPresenter();
            DataSet dsRetorno = new DataSet();

            dsRetorno = presenter.GerarExportacaoSintetica(p_orgaoId
                                        , Convert.ToInt16(chkUgeCodigoExportaSintetico.Checked)
                                        , Convert.ToInt16(chkUgeDescricaoExportaSintetico.Checked)
                                        , Convert.ToInt16(chkGrupoExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkSubitemCodigoExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkSubitemDescricaoExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkUnidadeFornecExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkSaldoQtdeExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkSaldoValorExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkPrecoMedioExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkLoteDescricaoExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkLoteQtdeExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkLoteDataVencExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkNDCodigoExportacaoSintetico.Checked)
                                        , Convert.ToInt16(chkNDDescricaoExportacaoSintetico.Checked)
                                        , p_periodoDe
                                        , p_periodoAte
                                        , p_Grupo
                                        , p_UGECodigo
                                        , Convert.ToInt32(chkComSemSaldo.Checked));

            if (dsRetorno.Tables[0].Rows.Count > 0)
            {
                GerarExcel(dsRetorno.Tables[0], "ExportacaoSintetica");
            }
        }

        #endregion
    }
}