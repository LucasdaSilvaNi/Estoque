using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using Sam.Web.Controles;
using Sam.Domain.Business.SIAFEM;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using tipoMovimentacao = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using Sam.Common.Enums;



namespace Sam.Web.Financeiro
{
    /// <summary>
    /// Página criada para visualização das datas de fechamento do SIAFEM, pelos usuários do SAM.
    /// </summary>
    public partial class NotaLancamentoPendenteSIAFEM : PageBase, INotaLancamentoPendenteSIAFEMView
    {
        private const string cmdReexecucaoPagamentoNotaLancamentoSiafem = "cmdEnviarComandoPagamentoNotaSIAFEM";
        private static string ChaveSessao_ArgumentEvent_GridViewRowCommand = "gvceArgs";
        private static string ChaveSessao_DescricaoTipoMovimentacao = "descTipoMovimentacao";
        private static string ChaveSessao_NumeroDocumentoMovimentacao = "numeroDocMovimentacao";
        private readonly string ChaveSessao_TextBox_NotaLancamento = "txtAlteracaoNotaLancamentoMovimentacao";
        private readonly string ChaveSessao_TextBox_NotaReclassificacao = "txtAlteracaoNotaReclassificacaoMovimentacao";
        private const string cmdAlteracaoManualNotaLancamentoMovimentacao = "cmdAlteracaoManualNotaLancamentoMovimentacao";
        private const string cmdAlteracaoManualNotaReclassificacaoMovimentacao = "cmdAlteracaoManualNotaReclassificacaoMovimentacao";
        private const string CAMPOCEPREENCHIDO = "CampoCEPreenchido";
        private const string CAMPOUGEPREENCHIDO = "CampoUGEPreenchido";

        protected void Page_Load(object sender, EventArgs e)
        {
            ucAcessoSIAFEM.EvchamaEvento += new WSSenha.chamaEvento(ExecutaEvento);
            ucAcessoSIAFEM.EvchamaEventoCancelar += new WSSenha.EventoCancelar(ExecutaEventoCancelar);
            ucEntradaCampoCE.EvchamaEvento += new EntradaCampoCE.chamaEvento(ExecutaEvento);
            ucEntradaCampoCE.EvchamaEventoCancelar += new EntradaCampoCE.EventoCancelar(ExecutaEventoCancelar);

            RecuperarDadosSessao();

            if (!IsPostBack)
            {
                NotaLancamentoPendenteSIAFEMPresenter objPresenter = new NotaLancamentoPendenteSIAFEMPresenter(this);
                objPresenter.Load();

                LimparDadosInformativosSessao();
            }
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            int almoxID = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;
            tipoPesquisa tipoPesquisa = tipoPesquisa.Almox;

            NotaLancamentoPendenteSIAFEMPresenter objPresenter = new NotaLancamentoPendenteSIAFEMPresenter();
            lstRetorno = objPresenter.ObterNotasLancamentosPendentes(tipoPesquisa, almoxID);

            //Processamento movido para camada de infra, quando o DTO da tela é instanciando.
            //Tal ação não é atribuição de camada de UI.
            //foreach (var item in lstRetorno)
            //{
            //    item.Tipo = item.MovimentoVinculado.Ativo == true ? "N" : "E";
            //}

            gdvNotaLancamentoPendenteSIAFEM.DataSource = lstRetorno;
            gdvNotaLancamentoPendenteSIAFEM.DataBind();
        }

        private void LimparDadosInformativosSessao()
        {

            SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorCE);
            SetSession<string>(null, ChaveSessao_DescricaoTipoMovimentacao);
            SetSession<string>(null, ChaveSessao_NumeroDocumentoMovimentacao);
        }

        string ICrudView.Id
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        string ICrudView.Descricao
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        string ICrudView.Codigo
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaCodigo
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaDescricao
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaNovo
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaGravar
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaExcluir
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaCancelar
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.MostrarPainelEdicao
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }

        public SortedList ParametrosRelatorio
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        public IList ListaErros
        {
            set { this.ListInconsistencias.ExibirLista(value); }
        }
        public RelatorioEntity DadosRelatorio { get; set; }


        #endregion

        #region INotaLancamentoPendenteSIAFEMView Members

        private bool dadosAcessoSiafemPreenchidos = false;
        private string valorCampoCE = string.Empty;
        private string valorCampoUge = string.Empty;
        private string loginSiafem = string.Empty;
        private string loginUsuarioSAM = string.Empty;
        private string senhaSiafem = string.Empty;
        private bool valorPreenchidoCampoCE = false;
        private bool valorPreenchidoCampoGestorUge = false;

        public int? Id
        {
            get { return null; }
            set { }
        }
        public int AlmoxarifadoID
        {
            get { return 0; }
            set { }
        }
        public int MovimentoID
        {
            get { return 0; }
            set { }
        }
        public int AuditoriaIntegracaoID
        {
            get { return 0; }
            set { }
        }
        public string DocumentoSAM
        {
            get { return null; }
            set { }
        }
        public string ErroSIAFEM
        {
            get { return null; }
            set { }
        }
        public DateTime DataReenvioMsgWs
        {
            get { return DateTime.MinValue; }
            set { }
        }
        public DateTime DataEnvioMsgWs
        {
            get { return DateTime.MinValue; }
            set { }
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }
        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        #endregion

        protected void gdvNotaLancamentoPendenteSIAFEM_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var gridRow = e.Row;

            if (gridRow.DataItem.IsNotNull() && gridRow.RowType == DataControlRowType.DataRow)
            {
                NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente = null;

                Label lblNLP_DocumentoSAM;
                Label lblNLP_TipoNotaSIAFEM;
                Label lblNLP_DescricaoErroSIAFEM;
                Label lblNLP_DataReenvioMsgWS;
                Label lblNLP_DataEnvioMsgWS;
                Label lblNLP_CampoCE;
                Label lblNLP_AlteracaoNotaLancamentoMovimentacao;
                Label lblNLP_AlteracaoNotaReclassificacaoMovimentacao;
                LinkButton lnkID;
                LinkButton lnkPagarSIAFEM;
                LinkButton lnkAlteracaoNotaLancamentoMovimentacao;
                LinkButton lnkAlteracaoNotaReclassificacaoMovimentacao;
                TextBox txtAlteracaoNotaLancamentoMovimentacao;
                TextBox txtAlteracaoNotaReclassificacaoMovimentacao;
                bool isEstorno = false;
                bool isEmpenho = false;
                bool movimentacaoReclassifica = false;
                string numeroNLEmLiq = null;

                notaLancamentoPendente = (NotaLancamentoPendenteSIAFEMEntity)gridRow.DataItem;
                lnkID = (LinkButton)gridRow.FindControl("lnkID");
                lblNLP_DocumentoSAM = (Label)gridRow.FindControl("lblNLP_DocumentoSAM");
                lblNLP_TipoNotaSIAFEM = (Label)gridRow.FindControl("lblNLP_TipoNotaSIAFEM");
                lblNLP_DescricaoErroSIAFEM = (Label)gridRow.FindControl("lblNLP_DescricaoErroSIAFEM");
                lblNLP_DataEnvioMsgWS = (Label)gridRow.FindControl("lblNLP_DataEnvioMsgWS");
                lblNLP_DataReenvioMsgWS = (Label)gridRow.FindControl("lblNLP_DataReenvioMsgWS");
                lblNLP_CampoCE = (Label)gridRow.FindControl("lblNLP_CampoCE");
                lblNLP_AlteracaoNotaLancamentoMovimentacao = (Label)gridRow.FindControl("lblNLP_AlteracaoNotaLancamentoMovimentacao");
                lblNLP_AlteracaoNotaReclassificacaoMovimentacao = (Label)gridRow.FindControl("lblNLP_AlteracaoNotaReclassificacaoMovimentacao");
                lnkAlteracaoNotaLancamentoMovimentacao = (LinkButton)gridRow.FindControl("lnkAlteracaoNotaLancamentoMovimentacao");
                lnkAlteracaoNotaReclassificacaoMovimentacao = (LinkButton)gridRow.FindControl("lnkAlteracaoNotaReclassificacaoMovimentacao");
                txtAlteracaoNotaLancamentoMovimentacao = (TextBox)gridRow.FindControl("txtAlteracaoNotaLancamentoMovimentacao");
                txtAlteracaoNotaReclassificacaoMovimentacao = (TextBox)gridRow.FindControl("txtAlteracaoNotaReclassificacaoMovimentacao");
                lnkPagarSIAFEM = (LinkButton)gridRow.FindControl("lnkPagarSIAFEM");


                if (txtAlteracaoNotaLancamentoMovimentacao.IsNotNull())
                    ScriptManager.RegisterStartupScript(txtAlteracaoNotaLancamentoMovimentacao, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

                if (txtAlteracaoNotaReclassificacaoMovimentacao.IsNotNull())
                    ScriptManager.RegisterStartupScript(txtAlteracaoNotaReclassificacaoMovimentacao, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

                if (lnkID.IsNotNull())
                    lnkID.Text = notaLancamentoPendente.Id.ToString();

                if (lblNLP_DocumentoSAM.IsNotNull())
                    lblNLP_DocumentoSAM.Text = notaLancamentoPendente.DocumentoSAM;

                movimentacaoReclassifica = (notaLancamentoPendente.TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao);
                isEmpenho = ((notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimentacao.EntradaPorEmpenho) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimentacao.EntradaPorRestosAPagar) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimentacao.ConsumoImediatoEmpenho) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho));
                if (lblNLP_TipoNotaSIAFEM.IsNotNull())
                {
                    string descricaoTipoNota = null;

                    if (movimentacaoReclassifica)
                        descricaoTipoNota = GeralEnum.GetEnumDescription((TipoNotaSIAF)notaLancamentoPendente.TipoNotaSIAF);
                    else
                    {
                        switch ((tipoMovimentacao)notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id)
                        {
                            case tipoMovimentacao.EntradaPorEmpenho:
                            case tipoMovimentacao.ConsumoImediatoEmpenho:
                            case tipoMovimentacao.EntradaPorRestosAPagar:
                            case tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                            case tipoMovimentacao.SaidaPorReclassificacao:
                                descricaoTipoNota = "NLEmLiq"; break;
                            case tipoMovimentacao.EntradaAvulsa:
                            case tipoMovimentacao.EntradaPorDoacaoImplantado:
                            case tipoMovimentacao.EntradaPorDevolucao:
                            case tipoMovimentacao.EntradaPorMaterialTransformado:
                            case tipoMovimentacao.SaidaPorTransferencia:
                            case tipoMovimentacao.SaidaPorDoacao:
                            case tipoMovimentacao.OutrasSaidas:
                            case tipoMovimentacao.SaidaPorMaterialTransformado:
                            case tipoMovimentacao.SaidaPorExtravioFurtoRoubo:
                            case tipoMovimentacao.SaidaPorIncorporacaoIndevida:
                            case tipoMovimentacao.SaidaPorPerda:
                            case tipoMovimentacao.SaidaInservivelQuebra:
                            case tipoMovimentacao.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                            case tipoMovimentacao.SaidaParaAmostraExposicaoAnalise:
                                descricaoTipoNota = GeralEnum.GetEnumDescription((TipoNotaSIAF)notaLancamentoPendente.TipoNotaSIAF); break;
                            default:
                                descricaoTipoNota = GeralEnum.GetEnumDescription((TipoNotaSIAF)notaLancamentoPendente.TipoNotaSIAF); break;
                        }
                    }

                    lblNLP_TipoNotaSIAFEM.Text = descricaoTipoNota;
                }

                if (lblNLP_DescricaoErroSIAFEM.IsNotNull())
                    lblNLP_DescricaoErroSIAFEM.Text = notaLancamentoPendente.ErroProcessamentoMsgWS.Replace("\\n", " - ").Replace("\n", " - ");

                if (lblNLP_DataReenvioMsgWS.IsNotNull() && notaLancamentoPendente.DataReenvioMsgWs.HasValue)
                    lblNLP_DataReenvioMsgWS.Text = notaLancamentoPendente.DataReenvioMsgWs.Value.ToString(base.fmtDataHoraFormatoBrasileiro);

                if (lblNLP_DataEnvioMsgWS.IsNotNull())
                    lblNLP_DataEnvioMsgWS.Text = notaLancamentoPendente.DataEnvioMsgWs.ToString(base.fmtDataHoraFormatoBrasileiro);

                if (lblNLP_CampoCE.IsNotNull())
                    lblNLP_CampoCE.Text = ((String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.InscricaoCE)) ? "" : notaLancamentoPendente.MovimentoVinculado.InscricaoCE);

                if (lnkPagarSIAFEM.IsNotNull())
                {
                    var _valorCampoCE = ((String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.InscricaoCE)) ? " " : notaLancamentoPendente.MovimentoVinculado.InscricaoCE);
                    lnkPagarSIAFEM.CommandArgument = String.Format("NotaLancamentoID:{0}|ValorCE:{1}|DescricaoTipoMov:{2}|NumeroDocMovimentacao:{3}|MovID:{4}|TipoLancamento:{5}|TipoNota:{6}|TipoMovID:{7}", notaLancamentoPendente.Id.Value.ToString(), _valorCampoCE, notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Descricao, notaLancamentoPendente.MovimentoVinculado.NumeroDocumento, notaLancamentoPendente.MovimentoVinculado.Id.Value, notaLancamentoPendente.Tipo, EnumUtils.GetEnumDescription<TipoNotaSIAF>(notaLancamentoPendente.TipoNotaSIAF), notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id);
                }

                isEstorno = !(notaLancamentoPendente.MovimentoVinculado.Ativo.Value);

                #region Inclusao Manual NLs
                if (lblNLP_AlteracaoNotaLancamentoMovimentacao.IsNotNull() && lblNLP_AlteracaoNotaReclassificacaoMovimentacao.IsNotNull())
                {
                    var fmtMsgAlteracaoManualNL = (isEstorno ? "<b>{0}{1}</b>: {2}NL" : "<b>{0}</b>: {2}NL");
                    var _descricaoTipoNotaSIAF = (movimentacaoReclassifica ? "NLEmLiq" : "NL");
                    var tipoLancamento = (isEstorno ? " (estorno)" : null);
                    var anoMesReferenciaAlmox = notaLancamentoPendente.MovimentoVinculado.AnoMesReferencia.Substring(0, 4);

                    numeroNLEmLiq = notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, isEstorno, TipoNotaSIAF.NL_Liquidacao);
                    var labelTipoNL = String.Format(fmtMsgAlteracaoManualNL, _descricaoTipoNotaSIAF, tipoLancamento, anoMesReferenciaAlmox);


                    if (lnkAlteracaoNotaLancamentoMovimentacao.IsNotNull())
                        lnkAlteracaoNotaLancamentoMovimentacao.CommandArgument = String.Format("NotaLancamentoID:{0}|DescricaoTipoMov:{1}|NumeroDocMovimentacao:{2}|MovID:{3}|TipoLancamento:{4}|TipoNota:{5}|TipoMovID:{6}", notaLancamentoPendente.Id.Value.ToString(), notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Descricao, notaLancamentoPendente.MovimentoVinculado.NumeroDocumento, notaLancamentoPendente.MovimentoVinculado.Id.Value, notaLancamentoPendente.Tipo, EnumUtils.GetEnumDescription<TipoNotaSIAF>(notaLancamentoPendente.TipoNotaSIAF), notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id);

                    lblNLP_AlteracaoNotaLancamentoMovimentacao.Text = labelTipoNL;

                    if (!String.IsNullOrWhiteSpace(numeroNLEmLiq))
                    {
                        txtAlteracaoNotaLancamentoMovimentacao.Text = numeroNLEmLiq.Replace(String.Format("{0}NL", anoMesReferenciaAlmox), "");
                        txtAlteracaoNotaLancamentoMovimentacao.Enabled = false;
                    }

                    if (isEmpenho)
                    {
                        var notaReclassificacaoMovMaterial = notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, isEstorno, TipoNotaSIAF.NL_Reclassificacao);

                        if (!String.IsNullOrWhiteSpace(notaReclassificacaoMovMaterial))
                        {
                            txtAlteracaoNotaReclassificacaoMovimentacao.Text = notaReclassificacaoMovMaterial.Replace(String.Format("{0}NL", anoMesReferenciaAlmox), "");
                            txtAlteracaoNotaReclassificacaoMovimentacao.Enabled = false;
                        }

                        lnkAlteracaoNotaLancamentoMovimentacao.Visible = !isEmpenho;
                        lnkAlteracaoNotaReclassificacaoMovimentacao.CommandArgument = String.Format("NotaLancamentoID:{0}|DescricaoTipoMov:{1}|NumeroDocMovimentacao:{2}|MovID:{3}|TipoLancamento:{4}|TipoNota:{5}|TipoMovID:{6}", notaLancamentoPendente.Id.Value.ToString(), notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Descricao, notaLancamentoPendente.MovimentoVinculado.NumeroDocumento, notaLancamentoPendente.MovimentoVinculado.Id.Value, notaLancamentoPendente.Tipo, EnumUtils.GetEnumDescription<TipoNotaSIAF>(notaLancamentoPendente.TipoNotaSIAF), notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id);

                        _descricaoTipoNotaSIAF = "NL Reclassificação";
                        labelTipoNL = String.Format(fmtMsgAlteracaoManualNL, _descricaoTipoNotaSIAF, tipoLancamento, anoMesReferenciaAlmox);
                        lblNLP_AlteracaoNotaReclassificacaoMovimentacao.Text = labelTipoNL;

                    }


                    lnkAlteracaoNotaReclassificacaoMovimentacao.Visible = isEmpenho;
                    lblNLP_AlteracaoNotaReclassificacaoMovimentacao.Visible = isEmpenho;
                    txtAlteracaoNotaReclassificacaoMovimentacao.Visible = isEmpenho;
                }

                #endregion

                gridRow.ToolTip = String.Format("Tipo Movimentação: {0}\nTipo Lançamento: {1}", notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Descricao, (!isEstorno ? "Normal" : "Estorno"));
            }
        }

        protected void gdvNotaLancamentoPendenteSIAFEM_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvNotaLancamentoPendenteSIAFEM.PageIndex = e.NewPageIndex;
            PopularGrid();
        }


        protected void gdvNotaLancamentoPendenteSIAFEM_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            #region Init

            GridViewCommandEventArgs gvceArgs = e;
            GridViewRow gvRow = null;


            if (gvceArgs.IsNotNull())
                SetSession<GridViewCommandEventArgs>(e, ChaveSessao_ArgumentEvent_GridViewRowCommand);
            else
                gvceArgs = GetSession<GridViewCommandEventArgs>(ChaveSessao_ArgumentEvent_GridViewRowCommand);

            var objPresenter = new NotaLancamentoPendenteSIAFEMPresenter(this);
            gvRow = (((gvceArgs.CommandSource as Control).Parent.NamingContainer as GridViewRow));

            #endregion


            if (gvceArgs.IsNotNull())
            {
                #region Variaveis

                string strEvArgs = null;

                int notaPendenteID = 0;
                int movMaterialID = 0;
                int tipoMovMaterialID = 0;
                string _valorCE = null;
                string documentoMovimentacaoSAM = null;
                string descricaoTipoMovimentacao = null;
                string msgGravacaoMovimentacaoSucesso = null;
                string strMovimentacaoMaterialID = null;
                string strTipoMovMaterialID = null;
                string tipoLancamentoSIAFEM = null;
                bool ehEstorno = false;
                string fmtMsgInformeAoUsuario = null;
                string tipoNotaSIAFEM = null;
                string tipoNotaSIAFEM_Adicional = null;
                string nlLiquidacaoMovimentacaoMaterial = null;
                string nlReclassificacaoMovimentacaoMaterial = null;
                string anoMesReferenciaAlmox = null;

                string msgErroProcessamentoSIAFEM = null;
                string msgRetornoProcessamentoSAFEM = null;
                TipoNotaSIAF enumTipoNotaSIAFEM;
                TextBox txtNL_LiquidacaoSIAFEM = null;
                TextBox txtNL_ReclassificacaoSIAFEM = null;

                char[] CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO = null;
                char[] CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO = null;
                string[] argsEvento = null;
                string nlSIAFEM = null;
                string nlLiquidacaoSIAFEM = null;
                string nlReclassicacaoSIAFEM = null;
                #endregion Variaveis

                #region Parsing EvArgs

                strEvArgs = gvceArgs.CommandArgument.ToString();
                if (!String.IsNullOrWhiteSpace(strEvArgs))
                {
                    fmtMsgInformeAoUsuario = @"Dados SIAFEM da movimentação do tipo ""{0}"", documento {1}, atualizados com sucesso!\n({2}: {3})";

                    CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO = new char[] { '|' };
                    CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO = new char[] { ':' };
                    argsEvento = strEvArgs.BreakLine(CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO);


                    if (gvceArgs.CommandName == cmdReexecucaoPagamentoNotaLancamentoSiafem)
                    {
                        notaPendenteID = Int32.Parse(argsEvento[0].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                        _valorCE = argsEvento[1].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        descricaoTipoMovimentacao = argsEvento[2].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        documentoMovimentacaoSAM = argsEvento[3].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        strMovimentacaoMaterialID = argsEvento[4].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        tipoLancamentoSIAFEM = argsEvento[5].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        tipoNotaSIAFEM = argsEvento[6].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        strTipoMovMaterialID = argsEvento[7].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                    }
                    else if (gvceArgs.CommandName == cmdAlteracaoManualNotaLancamentoMovimentacao || gvceArgs.CommandName == cmdAlteracaoManualNotaReclassificacaoMovimentacao)
                    {
                        //"NotaLancamentoID:{0}|DescricaoTipoMov:{1}|NumeroDocMovimentacao:{2}|MovID:{3}|TipoLancamento:{4}|TipoNota:{5}|NLSiafem:{6}"
                        notaPendenteID = Int32.Parse(argsEvento[0].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                        descricaoTipoMovimentacao = argsEvento[1].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        documentoMovimentacaoSAM = argsEvento[2].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        strMovimentacaoMaterialID = argsEvento[3].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        tipoLancamentoSIAFEM = argsEvento[4].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        tipoNotaSIAFEM = argsEvento[5].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                        strTipoMovMaterialID = argsEvento[6].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                    }

                    SetSession<string>(descricaoTipoMovimentacao, ChaveSessao_DescricaoTipoMovimentacao);
                    SetSession<string>(documentoMovimentacaoSAM, ChaveSessao_NumeroDocumentoMovimentacao);

                    if (!String.IsNullOrWhiteSpace(_valorCE) && String.IsNullOrWhiteSpace(valorCampoCE))
                        this.ucEntradaCampoCE.ValorCampoCE = _valorCE;
                    else if (String.IsNullOrWhiteSpace(_valorCE) && !String.IsNullOrWhiteSpace(valorCampoCE))
                        this.ucEntradaCampoCE.ValorCampoCE = valorCampoCE;
                    else if ((!String.IsNullOrWhiteSpace(_valorCE) && !String.IsNullOrWhiteSpace(valorCampoCE)) &&
                              (valorCampoCE != _valorCE))
                        this.ucEntradaCampoCE.ValorCampoCE = valorCampoCE;
                }

                objPresenter = new NotaLancamentoPendenteSIAFEMPresenter(this);
                ehEstorno = (tipoLancamentoSIAFEM == "E");
                enumTipoNotaSIAFEM = EnumUtils.ParseDescriptionToEnum<TipoNotaSIAF>(tipoNotaSIAFEM);
                Int32.TryParse(strMovimentacaoMaterialID, out movMaterialID);
                Int32.TryParse(strTipoMovMaterialID, out tipoMovMaterialID);
                //var movimentacaoReclassifica = ((tipoMovMaterialID == (int)tipoMovimentacao.EntradaPorEmpenho) || (tipoMovMaterialID == (int)tipoMovimentacao.EntradaPorRestosAPagar) || (tipoMovMaterialID == (int)tipoMovimentacao.ConsumoImediatoEmpenho));
                var movimentacaoReclassifica = ((tipoMovMaterialID == (int)tipoMovimentacao.EntradaPorEmpenho) || (tipoMovMaterialID == (int)tipoMovimentacao.EntradaPorRestosAPagar) || (tipoMovMaterialID == (int)tipoMovimentacao.ConsumoImediatoEmpenho) || (tipoMovMaterialID == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho));

                ucEntradaCampoCE.ExibirUge = tipoMovMaterialID.Equals(tipoMovimentacao.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode());
                #endregion

                if (gvRow.IsNotNull())
                {
                    txtNL_LiquidacaoSIAFEM = (TextBox)gvRow.FindControlRecursive("txtAlteracaoNotaLancamentoMovimentacao");
                    txtNL_ReclassificacaoSIAFEM = (TextBox)gvRow.FindControlRecursive("txtAlteracaoNotaReclassificacaoMovimentacao");

                    if (txtNL_LiquidacaoSIAFEM.IsNotNull())
                    {
                        Session.Add(ChaveSessao_TextBox_NotaLancamento, txtNL_LiquidacaoSIAFEM);
                        ScriptManager.RegisterStartupScript(txtNL_LiquidacaoSIAFEM, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
                    }

                    if (txtNL_ReclassificacaoSIAFEM.IsNotNull())
                    {
                        Session.Add(ChaveSessao_TextBox_NotaReclassificacao, txtNL_ReclassificacaoSIAFEM);
                        ScriptManager.RegisterStartupScript(txtNL_ReclassificacaoSIAFEM, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
                    }

                    switch (gvceArgs.CommandName)
                    {
                        case cmdReexecucaoPagamentoNotaLancamentoSiafem:
                            {
                                // if (!this.valorPreenchidoCampoCE)
                                if ((!EntradaCampoCE.valorPreenchidoCampoCE && !ucEntradaCampoCE.ExibirUge) || ucEntradaCampoCE.ExibirUge)
                                    efetuarPreenchimentoCampoCE((tipoMovimentacao)tipoMovMaterialID, documentoMovimentacaoSAM);

                                if (!this.dadosAcessoSiafemPreenchidos)
                                    efetuarAcessoSIAFEM();

                                var usuarioLogado = this.GetAcesso.Transacoes.Usuario;
                                if (usuarioLogado.IsNotNull() && !string.IsNullOrWhiteSpace(usuarioLogado.Cpf))
                                {
                                    this.loginUsuarioSAM = usuarioLogado.Cpf;
                                }
                                else
                                {
                                    ExibirMensagem("Erro ao obter informações do usuário logado");
                                    return;
                                }

                                if ((EntradaCampoCE.valorPreenchidoCampoCE && !this.ucEntradaCampoCE.ExibirUge || valorPreenchidoCampoGestorUge) && dadosAcessoSiafemPreenchidos)
                                {
                                    int _codigoGestor = GetAcesso.Transacoes.Perfis[0].GestorPadrao.CodigoGestao.Value;

                                    int _codigoUge = 0;
                                    int.TryParse(this.valorCampoUge, out _codigoUge);

                                    msgErroProcessamentoSIAFEM = objPresenter.ExecutaProcessamentoNotaPendenteSIAFEM(this.loginSiafem, this.senhaSiafem, this.valorCampoCE, tipoLancamentoSIAFEM, notaPendenteID, _codigoGestor, _codigoUge, this.loginUsuarioSAM);

                                    var statusProcessamentoMovimentacaoSIAFEM = String.IsNullOrWhiteSpace(msgErroProcessamentoSIAFEM);
                                    if (statusProcessamentoMovimentacaoSIAFEM)
                                    {
                                        descricaoTipoMovimentacao = GetSession<string>(ChaveSessao_DescricaoTipoMovimentacao);
                                        documentoMovimentacaoSAM = GetSession<string>(ChaveSessao_NumeroDocumentoMovimentacao);
                                        var movMaterial = (new MovimentoPresenter()).ObterMovimentacao(movMaterialID);


                                        //Atualizar NL, de acordo com o tipo de lancamento SIAFEM
                                        nlLiquidacaoMovimentacaoMaterial = movMaterial.ObterNLsMovimentacao(false, ehEstorno, TipoNotaSIAF.NL_Liquidacao);
                                        if ((enumTipoNotaSIAFEM != default(TipoNotaSIAF)) && (movimentacaoReclassifica))
                                        {
                                            nlReclassificacaoMovimentacaoMaterial = movMaterial.ObterNLsMovimentacao(false, ehEstorno, TipoNotaSIAF.NL_Reclassificacao);

                                            tipoNotaSIAFEM_Adicional = GeralEnum.GetEnumDescription(TipoNotaSIAF.NL_Reclassificacao);
                                            fmtMsgInformeAoUsuario = @"Dados SIAFEM da movimentação do tipo ""{0}"", documento {1}, atualizados com sucesso!\n({2}: {3} e {4}: {5})";
                                        }


                                        msgGravacaoMovimentacaoSucesso = String.Format(fmtMsgInformeAoUsuario, descricaoTipoMovimentacao, documentoMovimentacaoSAM, tipoNotaSIAFEM, nlLiquidacaoMovimentacaoMaterial, tipoNotaSIAFEM_Adicional, nlReclassificacaoMovimentacaoMaterial);
                                        msgRetornoProcessamentoSAFEM = ((String.IsNullOrWhiteSpace(nlLiquidacaoMovimentacaoMaterial)) ? msgErroProcessamentoSIAFEM : msgGravacaoMovimentacaoSucesso);
                                    }

                                    EntradaCampoCE.valorPreenchidoCampoCE = false;
                                }
                            }

                            break;

                        case cmdAlteracaoManualNotaLancamentoMovimentacao:
                        case cmdAlteracaoManualNotaReclassificacaoMovimentacao:
                            {
                                nlLiquidacaoSIAFEM = string.Empty;
                                nlReclassicacaoSIAFEM = string.Empty;
                                anoMesReferenciaAlmox = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4);

                                //if(gvRow.IsNotNull())
                                {
                                    if (txtNL_LiquidacaoSIAFEM.IsNotNull() && !String.IsNullOrWhiteSpace(txtNL_LiquidacaoSIAFEM.Text))
                                        nlLiquidacaoSIAFEM = String.Format("{0}NL{1}", anoMesReferenciaAlmox, txtNL_LiquidacaoSIAFEM.Text);

                                    if (txtNL_ReclassificacaoSIAFEM.IsNotNull() && !String.IsNullOrWhiteSpace(txtNL_ReclassificacaoSIAFEM.Text))
                                        nlReclassicacaoSIAFEM = String.Format("{0}NL{1}", anoMesReferenciaAlmox, txtNL_ReclassificacaoSIAFEM.Text);
                                }

                                var usuarioLogado = this.GetAcesso.Transacoes.Usuario;
                                if (usuarioLogado.IsNotNull() && !string.IsNullOrWhiteSpace(usuarioLogado.Cpf))
                                {
                                    this.loginUsuarioSAM = usuarioLogado.Cpf;
                                }
                                else
                                {
                                    ExibirMensagem("Erro ao obter informações do usuário logado");
                                    return;
                                }

                                var parametrosOK = ValidaNLsParametrosParaAlteracaoManual(nlLiquidacaoSIAFEM, nlReclassicacaoSIAFEM, enumTipoNotaSIAFEM, (tipoMovimentacao)tipoMovMaterialID);
                                if (!parametrosOK)
                                {
                                    ExibirMensagem("Favor informar NLs corretamente");
                                    return;
                                }

                                nlSIAFEM = nlLiquidacaoSIAFEM;
                                if (!String.IsNullOrWhiteSpace(nlSIAFEM))
                                    objPresenter.ExecutaProcessamentoManualNotaSIAFEM(loginUsuarioSAM, tipoLancamentoSIAFEM, notaPendenteID, nlSIAFEM, TipoNotaSIAF.NL_Liquidacao);

                                if (gvceArgs.CommandName == cmdAlteracaoManualNotaReclassificacaoMovimentacao)
                                {
                                    nlSIAFEM = nlReclassicacaoSIAFEM;
                                    if (!String.IsNullOrWhiteSpace(nlSIAFEM))
                                    {
                                        objPresenter.ExecutaProcessamentoManualNotaSIAFEM(loginUsuarioSAM, tipoLancamentoSIAFEM, notaPendenteID, nlSIAFEM, TipoNotaSIAF.NL_Reclassificacao);

                                        tipoNotaSIAFEM_Adicional = GeralEnum.GetEnumDescription(TipoNotaSIAF.NL_Reclassificacao);
                                        fmtMsgInformeAoUsuario = @"Dados SIAFEM da movimentação do tipo ""{0}"", documento {1}, atualizados MANUALMENTE com sucesso!\n({2}: {3} e {4}: {5})";
                                    }
                                }
                                var movMaterial = (new MovimentoPresenter()).ObterMovimentacao(movMaterialID);
                                nlLiquidacaoMovimentacaoMaterial = movMaterial.ObterNLsMovimentacao(false, ehEstorno, TipoNotaSIAF.NL_Liquidacao);
                                nlReclassificacaoMovimentacaoMaterial = movMaterial.ObterNLsMovimentacao(false, ehEstorno, TipoNotaSIAF.NL_Reclassificacao);

                               
                                msgGravacaoMovimentacaoSucesso = String.Format(fmtMsgInformeAoUsuario, descricaoTipoMovimentacao, documentoMovimentacaoSAM, tipoNotaSIAFEM, nlLiquidacaoMovimentacaoMaterial, tipoNotaSIAFEM_Adicional, nlReclassificacaoMovimentacaoMaterial);
                                msgRetornoProcessamentoSAFEM = ((String.IsNullOrWhiteSpace(nlLiquidacaoMovimentacaoMaterial)) ? msgErroProcessamentoSIAFEM : msgGravacaoMovimentacaoSucesso);
                            }

                            break;
                    }
                }

                if (!String.IsNullOrWhiteSpace(descricaoTipoMovimentacao) && !String.IsNullOrWhiteSpace(documentoMovimentacaoSAM) && !String.IsNullOrWhiteSpace(tipoNotaSIAFEM) && !String.IsNullOrWhiteSpace(msgRetornoProcessamentoSAFEM))
                    ExibirMensagem(msgRetornoProcessamentoSAFEM);

                PopularGrid();
            }
        }

        private bool ValidaNLsParametrosParaAlteracaoManual(string nlLiquidacaoSIAFEM, string nlReclassicacaoSIAFEM, TipoNotaSIAF tipoNotaSIAFEM, tipoMovimentacao tipoMovimentacaoMaterial)
        {
            bool numeroNLTemConteudo = false;
            bool comprimentoNumeroNL = false;
            bool ehNumero = false;
            int outParse = -1;

            //var movimentacaoReclassifica = ((tipoNotaSIAFEM == TipoNotaSIAF.NL_Reclassificacao) || ((tipoMovimentacaoMaterial == tipoMovimentacao.EntradaPorEmpenho) || (tipoMovimentacaoMaterial == tipoMovimentacao.EntradaPorRestosAPagar) || (tipoMovimentacaoMaterial == tipoMovimentacao.ConsumoImediatoEmpenho)));
            var movimentacaoReclassifica = ((tipoNotaSIAFEM == TipoNotaSIAF.NL_Reclassificacao) || ((tipoMovimentacaoMaterial == tipoMovimentacao.EntradaPorEmpenho) || (tipoMovimentacaoMaterial == tipoMovimentacao.EntradaPorRestosAPagar) || (tipoMovimentacaoMaterial == tipoMovimentacao.ConsumoImediatoEmpenho) || (tipoMovimentacaoMaterial == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho)));
            if (movimentacaoReclassifica)
            {
                numeroNLTemConteudo = (!String.IsNullOrWhiteSpace(nlLiquidacaoSIAFEM) && !String.IsNullOrWhiteSpace(nlReclassicacaoSIAFEM));
                comprimentoNumeroNL = (nlLiquidacaoSIAFEM.Length == 11 && nlReclassicacaoSIAFEM.Length == 11);
                ehNumero = (numeroNLTemConteudo && (Int32.TryParse(nlLiquidacaoSIAFEM.Substring(6), out outParse) && Int32.TryParse(nlReclassicacaoSIAFEM.Substring(6), out outParse)));
            }
            else
            {
                numeroNLTemConteudo = (!String.IsNullOrWhiteSpace(nlLiquidacaoSIAFEM));
                comprimentoNumeroNL = (nlLiquidacaoSIAFEM.Length == 11);
                ehNumero = (numeroNLTemConteudo && Int32.TryParse(nlLiquidacaoSIAFEM.Substring(6), out outParse));

            }

            return (numeroNLTemConteudo && comprimentoNumeroNL && ehNumero);
        }

        private string _obterDescricaoTipoMovimentacao(int tipoMovID)
        {
            GeralEnum.TipoMovimento tipoMovimento = (GeralEnum.TipoMovimento)tipoMovID;

            if (tipoMovimento == GeralEnum.TipoMovimento.RequisicaoPendente)
                tipoMovimento = GeralEnum.TipoMovimento.RequisicaoAprovada;

            return GeralEnum.GetEnumDescription(tipoMovimento);
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            NotaLancamentoPendenteSIAFEMPresenter objPresenter = new NotaLancamentoPendenteSIAFEMPresenter(this);
            objPresenter.Imprimir();
        }

        #region Integracao SIAFEM
        protected void efetuarAcessoSIAFEM()
        {

            WebUtil webUtil = new WebUtil();
            if (String.IsNullOrWhiteSpace(GetSession<string>(WSSenha.ChaveSessao_CampoSenha_WsSiafem)))
            {
                webUtil.runJScript(this, "OpenModalSenhaWs();");
                return;
            }

            if (!String.IsNullOrWhiteSpace(GetSession<string>(WSSenha.ChaveSessao_CampoLogin_WsSiafem)))
                loginSiafem = GetSession<string>(WSSenha.ChaveSessao_CampoLogin_WsSiafem);

            if (!String.IsNullOrWhiteSpace(GetSession<string>(WSSenha.ChaveSessao_CampoSenha_WsSiafem)))
                senhaSiafem = GetSession<string>(WSSenha.ChaveSessao_CampoSenha_WsSiafem);
            else
            {
                ExibirMensagem("Senha em branco não-permitida.");
                SetSession<string>(null, WSSenha.ChaveSessao_CampoSenha_WsSiafem);
            }

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }
        protected void RecuperarDadosSessao()
        {
            loginSiafem = GetSession<string>(WSSenha.ChaveSessao_CampoLogin_WsSiafem);
            senhaSiafem = GetSession<string>(WSSenha.ChaveSessao_CampoSenha_WsSiafem);
            valorCampoCE = GetSession<string>(EntradaCampoCE.ChaveSessao_CampoValorCE);
        }
        protected void LimparDadosSessao()
        {
            SetSession<string>(null, WSSenha.ChaveSessao_CampoLogin_WsSiafem);
            SetSession<string>(null, WSSenha.ChaveSessao_CampoSenha_WsSiafem);
            SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorCE);
            SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorGestor);
            SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorUge);
        }

        public void ExecutaEvento()
        {
            RecuperarDadosSessao();
            gdvNotaLancamentoPendenteSIAFEM_RowCommand(null, null);
        }

        public void ExecutaEventoCancelar()
        {
            TextBox txtNL_LiquidacaoSIAFEM = (TextBox)Session[ChaveSessao_TextBox_NotaLancamento];
            TextBox txtNL_ReclassificacaoSIAFEM = (TextBox)Session[ChaveSessao_TextBox_NotaReclassificacao];

            if (txtNL_LiquidacaoSIAFEM.IsNotNull())
                ScriptManager.RegisterStartupScript(txtNL_LiquidacaoSIAFEM, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            if (txtNL_ReclassificacaoSIAFEM.IsNotNull())
                ScriptManager.RegisterStartupScript(txtNL_ReclassificacaoSIAFEM, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
        }

        protected void efetuarPreenchimentoCampoCE(tipoMovimentacao TipoMovimentacao, string NumeroDocumento)
        {
            WebUtil webUtil = new WebUtil();
            var _mesmoDocumento = PageBase.GetSession<string>(EntradaCampoCE.ChaveSessao_CampoValorNumDoc) == NumeroDocumento;

            if (String.IsNullOrWhiteSpace(GetSession<string>(EntradaCampoCE.ChaveSessao_CampoValorCE)) || (ucEntradaCampoCE.ExibirUge && !_mesmoDocumento) || (!EntradaCampoCE.valorPreenchidoCampoCE))
            {
                PageBase.SetSession<string>(NumeroDocumento, EntradaCampoCE.ChaveSessao_CampoValorNumDoc);
                ucEntradaCampoCE.ProcessarFormulario = false;
                ucEntradaCampoCE.Uge = "";
                webUtil.runJScript(this, "OpenModalCampoSiafemCE();");
                return;
            }

            valorCampoCE = PageBase.GetSession<string>(EntradaCampoCE.ChaveSessao_CampoValorCE);
            valorCampoUge = PageBase.GetSession<string>(EntradaCampoCE.ChaveSessao_CampoValorUge);

            List<string> _msgErro = new List<string>();

            if (string.IsNullOrWhiteSpace(valorCampoCE))
            {
                _msgErro.Add("Campo SIAFEM \"CE\", de preenchimento obrigatório.");
                SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorCE);
            }

            if (IsPostBack && ucEntradaCampoCE.ExibirUge && ucEntradaCampoCE.ProcessarFormulario && _mesmoDocumento)
            {
                if (string.IsNullOrWhiteSpace(valorCampoUge))
                {
                    _msgErro.Add("Campo SIAFEM \"UGE Destino\" é de preenchimento obrigatório.");
                    SetSession<string>(null, EntradaCampoCE.ChaveSessao_CampoValorUge);
                }

                this.valorPreenchidoCampoGestorUge = _msgErro.Count == 0;
            }

            if (_msgErro.Count > 0)
            {
                ExibirMensagem(string.Join("\\n", _msgErro.ToArray()));
                webUtil.runJScript(this, "OpenModalCampoSiafemCE();");
                return;
            }

            //this.valorPreenchidoCampoCE = (!String.IsNullOrWhiteSpace(this.valorCampoCE));
        }
        #endregion

    }
}
