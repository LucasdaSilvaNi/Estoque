using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Common;
using System.Web.UI.HtmlControls;
using System.Drawing;
using logErro = Sam.Domain.Business.LogErro;
using System.Linq.Expressions;
using System.Diagnostics;
using Sam.Web.Controles;
using Sam.Entity;

namespace Sam.Web.Almoxarifado
{
    public partial class FechamentoMensal : PageBase, IFechamentoMensalView
    {
        private MovimentoEntity movimento = new MovimentoEntity();
        private const string sessaoMov = "movimento";
        private string loginSiafem = string.Empty;
        private string senhaSiafem = string.Empty;
        private decimal valorTotalConsumoAlmox = 0.00m;
        private decimal valorTotalPagoSiafem = 0.00m;
        private bool dadosAcessoSiafemPreenchidos = false;
        private const string btnTextoOriginal = "Realizar NL de Consumo";
        private static string nomePrimeiraNota = string.Empty;
        private static string textoPrimeiraNota = string.Empty;
        private static string chkEstornoNLConsumoPrimeiro = string.Empty;
        //private static string btnPagamentoSIAFEMClientID = string.Empty;
        private static string webControlClientID = string.Empty;

        private readonly string cmdPagamentoNLConsumo = "cmdPagarSIAFEM";
        private readonly string cmdEstornoPagamentoNLConsumo = "cmdEstornarPagamentoSIAFEM";
        private readonly string ChaveSessao_ArgumentEvent_GridViewRowCommand = "gvceArgs";

        private readonly string ChaveSessao_ListaConsumoUA = "listaNLParaProcessamento";
        private readonly string ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM = "btnPagamentoSIAFEM";
        private readonly string ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM_TextoOriginal = "btnPagamentoSIAFEMTextoOriginal";
        private readonly string ChaveSessao_AcaoEstornoPagamento_CheckBoxEstornoPagamentoSIAFEM = "chkEstornoNLConsumo";




        public void RaisePostBackEvent(string eventArgument) { }

        protected void Page_Load(object sender, EventArgs e)
        {

            SetTimeoutProcessamentoAssincrono(1800);

            ucAcessoSIAFEM.EvchamaEvento += new WSSenha.chamaEvento(ExecutaEvento);
            ucAcessoSIAFEM.EvchamaEventoCancelar += new WSSenha.EventoCancelar(ExecutaEventoCancelar);
            RecuperarDadosSessaoSIAFEM();

            AtualizarJavaScriptBtn();
            ControlaExibicaoBotaoPagamentoConsumo();
            ControlaExibicaoBotaoFechamento();
            DesabilitaBotaoAlmoxarifadoInativo();
          
            if (!IsPostBack)
            { }
        }

        private void AtualizarJavaScriptBtn()
        {
            string mesRef = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
           
            string mesRefFormatado = string.Format("{0}/{1}", mesRef.ToString().Substring(4, 2), mesRef.ToString().Substring(0, 4));

            string mesRefAnterior = TratamentoDados.ValidarAnoMesRef(mesRef, -1);
            string mesRefAnteriorFormatado = string.Format("{0}/{1}", mesRefAnterior.ToString().Substring(4, 2), mesRefAnterior.ToString().Substring(0, 4));

            btnFechamento.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar o fechamento do mês " + mesRefFormatado + ".');");
            btnReabertura.Attributes.Add("OnClick", "return confirm('Pressione OK para estornar o fechamento do mês  " + mesRefAnteriorFormatado + ".');");
        }

        private void ControlaExibicaoBotaoPagamentoConsumo()
        {
            //int almoxID = -1;
            //bool podePagarConsumoAlmox = false;
            //FechamentoMensalPresenter objPresenter = null;
            //var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;         
            //almoxID = almoxLogado.Id.Value;
            //objPresenter = new FechamentoMensalPresenter(this);


            //podePagarConsumoAlmox = objPresenter.PodeExecutarPagamentoConsumoAlmox(almoxID);

            //ExibeBotaoNLConsumo = podePagarConsumoAlmox;
            //BloqueiaBotaoNLConsumo = !podePagarConsumoAlmox;

            //Se o o mês-referência do almoxarifado estiver aberto no calendario SIAFEM, permitir a exibição do botão de geração de NLs de Consumo.
            VerificaMesFechadoSIAFEM();
        }

        private void ControlaExibicaoBotaoFechamento()
        {
            int almoxID = -1;
            FechamentoMensalPresenter objPresenter = null;
            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

            almoxID = almoxLogado.Id.Value;
            objPresenter = new FechamentoMensalPresenter(this);

            ExibeBotaoFechamento = objPresenter.PodeExecutarFechamentoMensal(almoxID);
        }

        /// <summary>
        /// Caso o mês esteja aberto no calendario SIAFEM, permitir a exibição do botão de geração de NLs de Consumo.
        /// </summary>
        private void VerificaMesFechadoSIAFEM()
        {
            FechamentoMensalPresenter objPresenter = null;
            string anoMesRef = null;
            bool podePagarConsumoAlmox = false;


            anoMesRef = this.AnoMesReferencia;
            objPresenter = new FechamentoMensalPresenter(this);
            podePagarConsumoAlmox = !objPresenter.VerificaStatusFechadoMesReferenciaSIAFEM(anoMesRef, true);


            ExibeBotaoNLConsumo = podePagarConsumoAlmox;
            BloqueiaBotaoNLConsumo = !podePagarConsumoAlmox;
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                string anoMesRef = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                int? almoxId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;               
                FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);
                fecha.VerificarTransferenciasPendentes((int)almoxId, Convert.ToInt32(anoMesRef));
                fecha.VerificarSubitensInativos((int)almoxId);

                SortedList paramList = new SortedList();
                DateTime dataInicial = new DateTime(Convert.ToInt32(anoMesRef.Substring(0, 4)), Convert.ToInt32(anoMesRef.Substring(4, 2)), 1);
                DateTime dataFinal = new DateTime(Convert.ToInt32(anoMesRef.Substring(0, 4)), Convert.ToInt32(anoMesRef.Substring(4, 2)), 1);
                dataFinal.AddMonths(1);

                paramList.Add("AlmoxId", almoxId);
                paramList.Add("DataInicial", dataInicial);
                paramList.Add("DataFinal", dataFinal);
                paramList.Add("TituloRelatorio", "Simulação do Balancete - mês aberto: ");
                paramList.Add("TransacaoPendentes", fecha.GetPendenciasTransferencia().ToString().ToUpper());
                paramList.Add("SubitensInativos", fecha.SubitensInativosFechamento);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public string Id
        {
            get
            {
                string retorno = null;
                if (Session["ID"] != null)
                    retorno = Session["ID"].ToString();
                return retorno;
            }
            set
            {
                Session["ID"] = value;
            }

        }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public void PopularGrid()
        {
            throw new NotImplementedException();
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        #region Bloquear Controles Movimento

        public bool BloqueiaNovo
        {
            set
            {
                //btnNovo.Enabled = !value; 
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                //btnGravarMov.Enabled = !value; 
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                //btnExcluir.Enabled = !value; 
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                //btnCancelarMov.Enabled = !value; 
            }
        }

        public bool BloqueiaImprimir
        {
            set
            {
                //    btnImprimir.Enabled = !value; 

            }
        }

        public bool BloqueiaAjuda
        {
            set
            {
                //btnAjuda.Enabled = !value; 
            }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaBotaoNLConsumo
        {
            set { btnNLConsumo.Enabled = !value; }
        }

        public bool ExibeBotaoNLConsumo
        {
            set
            {
                string acaoExibicao = null;

                acaoExibicao = ((value) ? "mostrarControle" : "esconderControle");

                btnNLConsumo.CssClass = acaoExibicao;
                btnNLConsumo.Visible = value;
            }
        }

        public bool ExibeBotaoFechamento
        {
            set
            {
                string acaoExibicao = null;

                acaoExibicao = ((value) ? "mostrarControle" : "esconderControle");

                btnFechamento.CssClass = acaoExibicao;
                btnFechamento.Visible = value;
            }
        }

        public bool BloqueiaBotaoReabertura
        {
            set { btnReabertura.Enabled = !value; }
        }

        public bool ExibeBotaoReabertura
        {
            set { btnReabertura.Visible = value; }
        }

        #endregion

        public bool MostrarPainelEdicao
        {
            set
            {
                //pnlEditar.Visible = value; 
            }
        }

        public int UgeId { get; set; }

        protected void btnAnalise_Click(object sender, EventArgs e)
        {
            FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);

            rptFechamento.DataSource = fecha.AnalisarFechamentoMensal(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id, TratamentoDados.TryParseInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef));
            rptFechamento.DataBind();
            if (rptFechamento.Items.Count > 0)
            {
                pnlFechamento.CssClass = "mostrarControle";
                pnlFechamento.Visible = true;
            }
            else
            {
                pnlFechamento.CssClass = "esconderControle";
                pnlFechamento.Visible = false;
            }
        }

        public bool BloqueiaDescricao
        {
            set { throw new NotImplementedException(); }
        }

        public string AnoMesReferencia
        {
            get
            {
                string vlRetorno = string.Empty;

                if (this.GetAcesso.IsNotNull() &&
                   this.GetAcesso.Transacoes.IsNotNull() &&
                   this.GetAcesso.Transacoes.Perfis.IsNotNullAndNotEmpty() &&
                   !String.IsNullOrWhiteSpace(this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef))
                {
                    vlRetorno = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                }

                return vlRetorno;
            }
            set { }
        }

        protected void btnSimularFechamento_click(object sender, EventArgs e)
        {
            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            int idAlmox = (int)almoxLogado.Id;
            int iAnoMesRef = (int)TratamentoDados.TryParseInt32(almoxLogado.MesRef);
            int usuarioSamLoginId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].IdLogin;

            FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);
            //fecha.EfetuarFechamentoMensal(idAlmox, iAnoMesRef, GeralEnum.SituacaoFechamento.Simular);
            fecha.ExecutarSimulacao(idAlmox, iAnoMesRef, usuarioSamLoginId);
            fecha.Imprimir();
        }

        private void InicializarGridConsumo(bool executaSimulacao = true)
        {
            IList<PTResMensalEntity> lstPTResConsumoUAs = PageBase.GetSession<IList<PTResMensalEntity>>(ChaveSessao_ListaConsumoUA);

            if (lstPTResConsumoUAs.IsNullOrEmpty())
            {
                lstPTResConsumoUAs = processarNLsConsumoAlmox();
                gridConsumoUAsAlmox.DataSource = lstPTResConsumoUAs;

                PageBase.SetSession<IList<PTResMensalEntity>>(lstPTResConsumoUAs, ChaveSessao_ListaConsumoUA);
            }

            if (lstPTResConsumoUAs.HasElements())
            {
                gridConsumoUAsAlmox.DataSource = lstPTResConsumoUAs;

                #region Detalhes Totalização NL Consumo

                this.valorTotalConsumoAlmox = (lstPTResConsumoUAs.IsNotNullAndNotEmpty()) ? lstPTResConsumoUAs.Sum(nlConsumo => { return (!nlConsumo.NaturezaDespesa.Codigo.ToString().StartsWith("4490")) ? nlConsumo.Valor.Value : 0.00m; }) : 0.00m;
                this.valorTotalPagoSiafem = (lstPTResConsumoUAs.IsNotNullAndNotEmpty()) ? lstPTResConsumoUAs.Where(nlConsumo => !String.IsNullOrWhiteSpace(nlConsumo.NlLancamento)
                                                                                                                             && nlConsumo.NlLancamento.Contains("NL") && !nlConsumo.NlLancamento.Contains("Consumo") && !nlConsumo.TipoLancamento.ToString().Contains("E")
                                                                                                                             && nlConsumo.Status != "C")
                                                                                                            .Sum(nlConsumo => { return (!nlConsumo.NaturezaDespesa.Codigo.ToString().StartsWith("4490")) ? nlConsumo.Valor.Value : 0.00m; }) : 0.00m;

                PageBase.SetSession<decimal>(this.valorTotalConsumoAlmox, "valorTotalConsumoAlmox");
                PageBase.SetSession<decimal>(this.valorTotalPagoSiafem, "valorTotalPagoSiafem");


                #endregion Detalhes Totalização NL Consumo
            }

            gridConsumoUAsAlmox.AllowPaging = false;
            gridConsumoUAsAlmox.DataBind();
        }

        private IList<PTResMensalEntity> processarNLsConsumoAlmox()
        {
            #region Variaveis
            int almoxID = -1;
            int gestorId = -1;
            int anoMesRef = -1;

            FechamentoMensalPresenter objPresenter = null;
            IList<PTResMensalEntity> lstNotasConsumoAlmox = null;
            #endregion Variaveis

            almoxID = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            gestorId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id;
            anoMesRef = (int)TratamentoDados.TryParseInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef);
            objPresenter = new FechamentoMensalPresenter(this);

            lstNotasConsumoAlmox = objPresenter.ProcessarNLsConsumoAlmox(gestorId, anoMesRef, almoxID);

            return lstNotasConsumoAlmox;
        }

        protected void gridConsumoUAsAlmox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            #region Variaveis

            string strEvArgs = null;
            Button btnPagamentoSIAFEM = null;
            TemplateField colunaEstornaNLConsumo = null;
            CheckBox chkEstornoNLConsumo = null;
            GridViewCommandEventArgs gvceArgs = e;
            FechamentoMensalPresenter objPresenter = null;
            PTResMensalEntity notaConsumoPaga = null;
            Label lblNLConsumoEstornada = null;
            Label lblMensagemErroSIAFEM = null;
            Label lblMensagemErroEstornoSIAFEM = null;
            GridViewRow gvRow = null;

            int uaID = 0;
            int ptresID = 0;
            int natDespesaID = 0;
            decimal valorNotaConsumo = 0.00m;
            int almoxID = 0;
            string movItemIDs = null;
            bool efetuaEstorno = false;

            #endregion Variaveis

            #region Init

            if (!this.dadosAcessoSiafemPreenchidos)
                efetuarAcessoSIAFEM();


            if (gvceArgs.IsNotNull())
                SetSession<GridViewCommandEventArgs>(e, ChaveSessao_ArgumentEvent_GridViewRowCommand);
            else
                gvceArgs = GetSession<GridViewCommandEventArgs>(ChaveSessao_ArgumentEvent_GridViewRowCommand);


            #endregion Init

            #region Parsing EvArgs

            strEvArgs = gvceArgs.CommandArgument.ToString();
            if (!String.IsNullOrWhiteSpace(strEvArgs))
            {
                char[] CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO = new char[] { '|' };
                char[] CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO = new char[] { ':' };
                string[] argsEvento = strEvArgs.BreakLine(CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO);

                uaID = Int32.Parse(argsEvento[0].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                ptresID = Int32.Parse(argsEvento[2].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                natDespesaID = Int32.Parse(argsEvento[3].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                valorNotaConsumo = Decimal.Parse(argsEvento[4].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                almoxID = Int32.Parse(argsEvento[5].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                movItemIDs = argsEvento[6].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
            }

            #endregion Parsing EvArgs


            if (gvceArgs.CommandName == "cmdPagarConsumoTotalUA" || gvceArgs.CommandName == "cmdEstornarConsumoTotalUA")
            {
                var _efetuaEstorno = (gvceArgs.CommandName == "cmdEstornarConsumoTotalUA");
                PagarConsumoAlmox(_efetuaEstorno);

                return;
            }


            objPresenter = new FechamentoMensalPresenter(this);
            //efetuaEstorno = (gvceArgs.CommandName == "cmdEstornarPagamentoSIAFEM" && dadosAcessoSiafemPreenchidos);
            efetuaEstorno = (gvceArgs.CommandName == cmdEstornoPagamentoNLConsumo && dadosAcessoSiafemPreenchidos);
            colunaEstornaNLConsumo = gridConsumoUAsAlmox.Columns.Cast<TemplateField>().Where(_coluna => _coluna.HeaderText == "Estorno NL Consumo").FirstOrDefault();
            gvRow = (((gvceArgs.CommandSource as Control).Parent.NamingContainer as GridViewRow));


            chkEstornoNLConsumo = (CheckBox)gvRow.FindControl("chkEstornarNL");
            lblNLConsumoEstornada = gvRow.FindControl("lblNLConsumoEstornada") as Label;
            btnPagamentoSIAFEM = (gvceArgs.CommandSource as Button);

            chkEstornoNLConsumo.Visible = true;
            lblNLConsumoEstornada.Text = string.Empty;
            if (btnPagamentoSIAFEM != null)
                webControlClientID = btnPagamentoSIAFEM.ClientID;

            //if ((gvceArgs.CommandName == "cmdPagarSIAFEM" || gvceArgs.CommandName == "cmdEstornarPagamentoSIAFEM") && dadosAcessoSiafemPreenchidos)
            if ((gvceArgs.CommandName == cmdPagamentoNLConsumo || gvceArgs.CommandName == cmdEstornoPagamentoNLConsumo) && dadosAcessoSiafemPreenchidos)
            {
                notaConsumoPaga = objPresenter.GerarNLConsumo(this.loginSiafem, this.senhaSiafem, almoxID, uaID, ptresID, natDespesaID, valorNotaConsumo, movItemIDs, efetuaEstorno);

                #region Comportamento Tela

                if (!String.IsNullOrWhiteSpace(notaConsumoPaga.NlLancamento))
                {
                    if (btnPagamentoSIAFEM.IsNotNull())
                    {
                        if (btnPagamentoSIAFEM.Text.Trim().ToUpper().Equals(btnTextoOriginal.Trim().ToUpper()))
                        {
                            btnPagamentoSIAFEM.Text = notaConsumoPaga.NlLancamento;
                            btnPagamentoSIAFEM.Font.Underline = false;
                            colunaEstornaNLConsumo.Visible = true;
                            btnPagamentoSIAFEM.OnClientClick = "return false;";
                            btnPagamentoSIAFEM.Enabled = false;
                        }
                        else
                        {
                            btnPagamentoSIAFEM.Text = btnTextoOriginal;
                            btnPagamentoSIAFEM.Font.Underline = true;
                            colunaEstornaNLConsumo.Visible = false;
                        }
                    }
                    else
                    {
                        if (efetuaEstorno)
                        {
                            chkEstornoNLConsumo.Visible = false;
                            lblNLConsumoEstornada.Text = notaConsumoPaga.NlLancamento;
                            lblNLConsumoEstornada.Visible = true;
                        }
                    }
                }
                else
                {
                    btnPagamentoSIAFEM = (gvceArgs.CommandSource as Button);
                    lblNLConsumoEstornada = gvRow.FindControl("lblNLConsumoEstornada") as Label;

                    var existeNLEstorno = (!String.IsNullOrWhiteSpace(lblNLConsumoEstornada.Text) && lblNLConsumoEstornada.Text != "Realizar NL de Consumo" && lblNLConsumoEstornada.Text.Contains("NL"));
                    if (btnPagamentoSIAFEM.IsNotNull())
                    {
                        var existeNLPagamento = (!String.IsNullOrWhiteSpace(btnPagamentoSIAFEM.Text) && btnPagamentoSIAFEM.Text != "Realizar NL de Consumo" && btnPagamentoSIAFEM.Text.Contains("NL"));
                        if (!String.IsNullOrWhiteSpace(notaConsumoPaga.Obs))
                        {
                            lblMensagemErroSIAFEM = gvRow.FindControl("lblMensagemErroSIAFEM") as Label;
                            lblMensagemErroEstornoSIAFEM = gvRow.FindControl("lblMensagemErroEstornoSIAFEM") as Label;

                            if (existeNLPagamento && notaConsumoPaga.TipoLancamento == 'N')
                            {
                                lblMensagemErroEstornoSIAFEM.Text = "ERRO SIAFEM!" + "<br>" + notaConsumoPaga.Obs + "<br><br>";
                                lblMensagemErroEstornoSIAFEM.Visible = true;

                                lblMensagemErroSIAFEM.Text = null;
                                lblMensagemErroSIAFEM.Visible = false;

                                btnPagamentoSIAFEM.Font.Underline = existeNLEstorno;
                                btnPagamentoSIAFEM.Enabled = existeNLEstorno;
                                chkEstornoNLConsumo.Visible = !existeNLEstorno;
                            }
                            else if (!existeNLPagamento && !existeNLEstorno)
                            {
                                lblMensagemErroSIAFEM.Text = "ERRO SIAFEM!" + "<br>" + notaConsumoPaga.Obs + "<br><br>";
                                lblMensagemErroSIAFEM.Visible = true;

                                lblMensagemErroEstornoSIAFEM.Text = null;
                                lblMensagemErroEstornoSIAFEM.Visible = false;


                                btnPagamentoSIAFEM.Font.Underline = true;
                                btnPagamentoSIAFEM.Enabled = true;
                                chkEstornoNLConsumo.Visible = false;
                            }
                            else
                            {
                                lblMensagemErroSIAFEM.Text = null;
                                lblMensagemErroSIAFEM.Visible = false;

                                lblMensagemErroEstornoSIAFEM.Text = "ERRO SIAFEM!" + "<br>" + notaConsumoPaga.Obs + "<br><br>";
                                lblMensagemErroEstornoSIAFEM.Visible = true;

                                btnPagamentoSIAFEM.Font.Underline = existeNLEstorno;
                                btnPagamentoSIAFEM.Enabled = existeNLEstorno;
                                chkEstornoNLConsumo.Visible = !existeNLEstorno;
                            }


                        }
                        else
                        {
                            //btnPagamentoSIAFEM.Text = "ERRO SIAFEM!";
                            btnPagamentoSIAFEM.ToolTip = btnPagamentoSIAFEM.Text;
                            btnPagamentoSIAFEM.Font.Underline = existeNLEstorno;
                            //btnPagamentoSIAFEM.OnClientClick = "return false;";
                            btnPagamentoSIAFEM.Enabled = existeNLEstorno;
                            chkEstornoNLConsumo.Visible = !existeNLEstorno;
                        }
                        //LimparDadosSessaoSIAFEM();
                    }
                    else if (btnPagamentoSIAFEM.IsNull())
                    {
                        lblMensagemErroEstornoSIAFEM = gvRow.FindControl("lblMensagemErroEstornoSIAFEM") as Label;
                        lblMensagemErroEstornoSIAFEM.Text = "ERRO SIAFEM!" + "<br>" + notaConsumoPaga.Obs + "<br><br>";
                        lblMensagemErroEstornoSIAFEM.Visible = true;
                    }

                    LimparDadosSessaoSIAFEM();
                }

                if (efetuaEstorno)
                    this.valorTotalConsumoAlmox += valorNotaConsumo;
                else
                    this.valorTotalConsumoAlmox -= valorNotaConsumo;

                #endregion Comportamento Tela
            }
        }

        protected void chkEstornoNLConsumo_CheckedChanged(object sender, EventArgs e)
        {
            #region Variaveis

            CheckBox chkEstornoNLConsumo = null;
            GridViewCommandEventArgs gvceArgs = null;
            GridViewRow gvRow = null;
            Button btnPagamentoSIAFEM = null;
            string strNLEstorno = "nlTesteEstorno";
            CommandEventArgs cmdEvArgs = null;

            #endregion


            #region Init
            chkEstornoNLConsumo = (sender as CheckBox);
            gvceArgs = e as GridViewCommandEventArgs;
            gvRow = (chkEstornoNLConsumo.NamingContainer as GridViewRow);

            #endregion

            if (chkEstornoNLConsumo.Checked)
            {
                #region Comportamento CheckBox Estorno

                btnPagamentoSIAFEM = gvRow.FindControlRecursive("btnPagarSIAFEM") as Button;
                Session.Add(ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM, btnPagamentoSIAFEM);
                Session.Add(ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM_TextoOriginal, btnPagamentoSIAFEM.Text);
                Session.Add(ChaveSessao_AcaoEstornoPagamento_CheckBoxEstornoPagamentoSIAFEM, chkEstornoNLConsumo);

                //cmdEvArgs = new CommandEventArgs("cmdEstornarPagamentoSIAFEM", btnPagamentoSIAFEM.CommandArgument);
                cmdEvArgs = new CommandEventArgs(cmdEstornoPagamentoNLConsumo, btnPagamentoSIAFEM.CommandArgument);
                gvceArgs = new GridViewCommandEventArgs(gvRow, chkEstornoNLConsumo, cmdEvArgs);



                if (btnPagamentoSIAFEM.IsNotNull())
                {
                    //var lblMsgErroEstorno = gvRow.NamingContainer.FindControlRecursive("lblMensagemErroEstornoSIAFEM") as Label;
                    var lblMensagemErroEstornoSIAFEM = gvRow.FindControl("lblMensagemErroEstornoSIAFEM") as Label;
                    var lblMensagemErroSIAFEM = gvRow.FindControl("lblMensagemErroSIAFEM") as Label;
                    var lblNLConsumoEstornada = gvRow.FindControl("lblNLConsumoEstornada") as Label;

                    //strNLEstorno = (lblNLConsumoEstornada.Text.Contains("NL") ? lblNLConsumoEstornada.Text : "[NOTA SIAFEM DESCONHECIDA]");
                    gridConsumoUAsAlmox_RowCommand(chkEstornoNLConsumo, gvceArgs);

                    var existeNLEstorno = (!String.IsNullOrWhiteSpace(lblNLConsumoEstornada.Text) && lblNLConsumoEstornada.Text != "Realizar NL de Consumo" && lblNLConsumoEstornada.Text.Contains("NL"));
                    var existeMsgErro = (!String.IsNullOrWhiteSpace(lblMensagemErroEstornoSIAFEM.Text) && String.IsNullOrWhiteSpace(lblNLConsumoEstornada.Text)
                                                                                                       && btnPagamentoSIAFEM.Text.Contains("NL"));
                    if (existeNLEstorno)
                        btnPagamentoSIAFEM.Text = "Realizar NL de Consumo";


                    btnPagamentoSIAFEM.Enabled = existeNLEstorno;
                    btnPagamentoSIAFEM.Font.Underline = existeNLEstorno;
                    btnPagamentoSIAFEM.OnClientClick = "";
                    chkEstornoNLConsumo.Checked = false;
                    chkEstornoNLConsumo.Text = "Estornar NL";
                }
                else if (btnPagamentoSIAFEM.IsNull())
                {
                    var btnPagamentoSIAFEM_InMemory = PageBase.GetSession<Button>(ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM);
                    btnPagamentoSIAFEM = gvRow.FindControl(btnPagamentoSIAFEM_InMemory.ID) as Button;
                    chkEstornoNLConsumo.Visible = true;
                }
                #endregion Comportamento CheckBox Estorno
            }
        }

        protected void gridConsumoUAsAlmox_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string msgTooltip = null;
            Button btnNumeroNL = null;
            CheckBox chkEstornoNLConsumo = null;
            Label lblNLConsumoEstornada = null;

            Label lblNaturezaDespesa = null;
            Label lblPTRes = null;
            Label lblPTResAcao = null;
            Label lblUADescricao = null;
            Label lblValorConsumo = null;

            Label lblValorTotalPendentes = null;
            Label lblValorTotalPagoSiafem = null;
            Label lblValorTotalConsumo = null;
            Label lblNumeroDocumentoRelacionado = null;
            Label lblMensagemErroSIAFEM = null;
            Button btnPagarSIAFEM = null;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region DataRow
                PTResMensalEntity _notaConsumo = e.Row.DataItem as PTResMensalEntity;

                btnNumeroNL = (Button)e.Row.FindControl("btnPagarSIAFEM");
                chkEstornoNLConsumo = (CheckBox)e.Row.FindControl("chkEstornarNL");
                lblNLConsumoEstornada = (Label)e.Row.FindControl("lblNLConsumoEstornada");
                lblNumeroDocumentoRelacionado = e.Row.FindControl("lblNumeroDocumentoRelacionado") as Label;
                lblNaturezaDespesa = (Label)e.Row.FindControl("lblNaturezaDespesa");
                lblPTRes = (Label)e.Row.FindControl("lblPTRes");
                lblPTResAcao = (Label)e.Row.FindControl("lblPTResAcao");
                lblUADescricao = (Label)e.Row.FindControl("lblUADescricao");
                lblValorConsumo = (Label)e.Row.FindControl("lblValorConsumo");
                btnPagarSIAFEM = (Button)e.Row.FindControl("btnPagarSIAFEM");


                //Notas que compõem a NL (Tooltip)
                //if (!String.IsNullOrWhiteSpace(_notaConsumo.DocumentoRelacionado))
                if (!String.IsNullOrWhiteSpace(_notaConsumo.DocumentoRelacionado) && _notaConsumo.DocumentoRelacionado.BreakLine().Count() > (Constante.CST_NUMERO_MAXIMO_DOCUMENTOS_TRANSACAO_SIAFEM_NL + 1))
                {
                    lblNumeroDocumentoRelacionado.Text = _notaConsumo.DocumentoRelacionado;

                    // Botao Pagamento NL Consumo desabilitado
                    btnPagarSIAFEM.Visible = btnPagarSIAFEM.Enabled = false;

                    //Informe de que este agrupamento devera ser pago diretamente no SIAFEM (por ora)
                    lblMensagemErroSIAFEM = (Label)e.Row.FindControl("lblMensagemErroSIAFEM");
                    lblMensagemErroSIAFEM.Visible = true;
                    lblMensagemErroSIAFEM.Text = "Este agrupamento deverá ser pago diretamente no SIAFEM";
                    lblMensagemErroSIAFEM.Font.Bold = true;
                    lblMensagemErroSIAFEM.BorderColor = Color.Red;
                }
                else
                {
                    lblNumeroDocumentoRelacionado.Text = _notaConsumo.DocumentoRelacionado;
                }
                msgTooltip = String.Format("Nota(s) de Requisição vinculada(s)\n\n{0}.", _notaConsumo.DocumentoRelacionado);


                if (_notaConsumo.NaturezaDespesa.Codigo.ToString().StartsWith("4490"))
                {
                    lblNaturezaDespesa.ForeColor = lblPTRes.ForeColor = lblUADescricao.ForeColor = lblValorConsumo.ForeColor = Color.Red;
                    btnPagarSIAFEM.Enabled = btnPagarSIAFEM.Visible = false;

                    return;
                }
                else if (!String.IsNullOrWhiteSpace(_notaConsumo.NlLancamento))
                {
                    chkEstornoNLConsumo.Visible = (_notaConsumo.TipoLancamento == 'N') ? true : false;
                    chkEstornoNLConsumo.Enabled = chkEstornoNLConsumo.Visible;

                    //Se for NL Consumo 'Complementar'
                    if (_notaConsumo.FlagLancamento == 'I')
                    {
                        var corFundoOriginal = e.Row.BackColor;
                        var corFonteTextoOriginal = lblNaturezaDespesa.ForeColor;
                        e.Row.BackColor = Color.DarkKhaki;
                        msgTooltip = "Lançamento (ou estorno) de requisições após pagamento de NL Consumo detectado";

                        lblNaturezaDespesa.ForeColor = lblPTRes.ForeColor = lblUADescricao.ForeColor = lblValorConsumo.ForeColor = Color.Red;

                        //Se status 'C' (movimentação vinculada cancelada), desabilitar tanto estorno, quanto pagamento, para linha gerada
                        if (_notaConsumo.Status == "C")
                        {
                            //btnPagarSIAFEM.Enabled = false;
                            //btnPagarSIAFEM.Visible = true;
                            e.Row.BackColor = corFundoOriginal;
                            lblNaturezaDespesa.ForeColor = lblPTRes.ForeColor = lblUADescricao.ForeColor = lblValorConsumo.ForeColor = corFonteTextoOriginal;
                            lblNaturezaDespesa.ForeColor = lblPTRes.ForeColor = lblUADescricao.ForeColor = lblValorConsumo.ForeColor = lblNumeroDocumentoRelacionado.ForeColor = lblNLConsumoEstornada.ForeColor = Color.Gray;

                            btnPagarSIAFEM.Enabled = btnPagarSIAFEM.Visible = false;
                            chkEstornoNLConsumo.Enabled = chkEstornoNLConsumo.Visible = false;

                            msgTooltip = "Requisição(ões) vinculada(s) estornada(s) após estorno de pagamento de NL Consumo detectado";
                        }
                    }

                    if (_notaConsumo.TipoLancamento == 'N')
                    {
                        btnNumeroNL.Text = _notaConsumo.NlLancamento;
                        btnNumeroNL.Font.Underline = false;
                        btnNumeroNL.OnClientClick = "return false";
                    }
                    else if (_notaConsumo.TipoLancamento == 'E')
                    {
                        lblNLConsumoEstornada.Text = _notaConsumo.NlLancamento;
                        lblNLConsumoEstornada.Visible = true;
                    }
                }

                btnNumeroNL.CssClass = "simulateLinkButtonLook";
                btnNumeroNL.Style.Remove("width");
                btnNumeroNL.Style.Add("width", "150px");

                e.Row.ToolTip = msgTooltip;

                #endregion

            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                #region FooterRow
                lblValorTotalPendentes = (Label)e.Row.FindControl("lblValorTotalPendentes");
                lblValorTotalPagoSiafem = (Label)e.Row.FindControl("lblValorTotalPagoSiafem");
                lblValorTotalConsumo = (Label)e.Row.FindControl("lblValorTotalConsumo");

                if (lblValorTotalConsumo.IsNotNull() && lblValorTotalPagoSiafem.IsNotNull() && lblValorTotalPendentes.IsNotNull())
                {
                    lblValorTotalConsumo.Text = String.Format("R$ {0}", this.valorTotalConsumoAlmox.ToString(base.fmtValorFinanceiro));
                    lblValorTotalPagoSiafem.Text = String.Format("R$ {0}", this.valorTotalPagoSiafem.ToString(base.fmtValorFinanceiro));

                    lblValorTotalPendentes.Text = String.Format("R$ {0}", (this.valorTotalConsumoAlmox - this.valorTotalPagoSiafem).ToString(base.fmtValorFinanceiro));
                }
                #endregion FooterRow
            }

        }

        private void PagarConsumoAlmox(bool estornarPagamentos = false)
        {

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseModalSenha", "javascript:CloseModalSenhaWs();", true);

            #region Variaveis
            IList<PTResMensalEntity> listaNLParaProcessamento = null;
            IList<PTResMensalEntity> listaNLParaProcessamento1 = null;
            Func<PTResMensalEntity, bool> expAcaoSIAFEM = null;
            FechamentoMensalPresenter objPresenter = null;

            int idAlmox = -1;
            int iAnoMesRef = -1;
            #endregion Variaveis

            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            objPresenter = new FechamentoMensalPresenter(this);
            idAlmox = (int)almoxLogado.Id;
            iAnoMesRef = Int32.Parse(almoxLogado.MesRef);

            AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(idAlmox);
            FechamentoMensalEntity Fechamento = new FechamentoMensalEntity();
            Fechamento.Almoxarifado = almoxarifado;

            objPresenter.Fechamento = Fechamento;

            if (!estornarPagamentos)
                expAcaoSIAFEM = (nlConsumo => String.IsNullOrWhiteSpace(nlConsumo.NlLancamento) || nlConsumo.NlLancamento.Contains("Realizar NL de Consumo"));
            else
                expAcaoSIAFEM = (nlConsumo => !String.IsNullOrWhiteSpace(nlConsumo.NlLancamento) && nlConsumo.NlLancamento.Contains("NL") && !nlConsumo.NlLancamento.Contains("Consumo"));


            listaNLParaProcessamento = PageBase.GetSession<IList<PTResMensalEntity>>(ChaveSessao_ListaConsumoUA).ToList();
            listaNLParaProcessamento1 = PageBase.GetSession<IList<PTResMensalEntity>>(ChaveSessao_ListaConsumoUA).ToList();


            if (listaNLParaProcessamento.IsNotNullAndNotEmpty())
            {
                listaNLParaProcessamento1 = listaNLParaProcessamento1.Where(expAcaoSIAFEM).ToListNoLock();
                listaNLParaProcessamento = objPresenter.processarConsumoUAsAlmoxarifadoEmLote(listaNLParaProcessamento, loginSiafem, senhaSiafem, estornarPagamentos);

                gridConsumoUAsAlmox.DataSource = listaNLParaProcessamento;
                gridConsumoUAsAlmox.DataBind();

                if (listaNLParaProcessamento1.IsNullOrEmpty())
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NLConsumoAlerta", "javascript:alert('Não há notas pendentes para pagamento');", true);
                else
                    if (!this.dadosAcessoSiafemPreenchidos)
                    efetuarAcessoSIAFEM();
            }
        }

        protected void efetuarAcessoSIAFEM()
        {
            WebUtil webUtil = new WebUtil();
            if (String.IsNullOrWhiteSpace(GetSession<string>("senhaWsSiafem")))
            {
                webUtil.runJScript(this, "OpenModalSenhaWs();");
                return;
            }

            if (!String.IsNullOrWhiteSpace(GetSession<string>("loginWsSiafem")))
                loginSiafem = GetSession<string>("loginWsSiafem");

            if (!String.IsNullOrWhiteSpace(GetSession<string>("senhaWsSiafem")))
                senhaSiafem = GetSession<string>("senhaWsSiafem");
            else
            {
                ExibirMensagem("Senha em branco não-permitida.");
                SetSession<string>(null, "senhaWsSiafem");
            }

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }
        protected void RecuperarDadosSessaoSIAFEM()
        {
            loginSiafem = GetSession<string>("loginWsSiafem");
            senhaSiafem = GetSession<string>("senhaWsSiafem");
        }
        protected void LimparDadosSessaoSIAFEM()
        {
            SetSession<string>(null, WSSenha.ChaveSessao_CampoLogin_WsSiafem);
            SetSession<string>(null, WSSenha.ChaveSessao_CampoSenha_WsSiafem);

            this.loginSiafem = this.senhaSiafem = null;
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        private void AtualizarAnoMesRef(int idAlmox)
        {
            var acesso = GetAcesso;
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter();
          
            Label lblMesAnoRef = (Label)this.Master.FindControl("lblMesAnoRef");
            if (lblMesAnoRef != null)
            {
                acesso.Transacoes.Perfis[0].AlmoxarifadoLogado = almoxarifado.SelecionarAlmoxarifadoPorGestor(idAlmox) as AlmoxarifadoEntity;

                string mesRefAtual = acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

                string mesRefFormatado = string.Format("{0}/{1}", mesRefAtual.Substring(4, 2), mesRefAtual.Substring(0, 4));

                lblMesAnoRef.Text = mesRefFormatado;
            }
        }

        protected void btnFechamento_Click(object sender, EventArgs e)
        {
            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            int idAlmox = (int)almoxLogado.Id;
            int iAnoMesRef = (int)TratamentoDados.TryParseInt32(almoxLogado.MesRef);
            int usuarioSamLoginId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].IdLogin;

            DateTime mesCorrente = new DateTime(Convert.ToInt16(iAnoMesRef.ToString().Substring(0, 4))
                                           , Convert.ToInt16(iAnoMesRef.ToString().Substring(4, 2))
                                           , 1);


            DateTime dRef = mesCorrente.AddDays(-(mesCorrente.Day - 1)).AddMonths(1).AddDays(-1);

            if (dRef <= Convert.ToDateTime(DateTime.Now.ToShortDateString()))
            {
                FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);
                AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter();
                try
                {
                    fecha.ExecutarFechamento(idAlmox, iAnoMesRef, usuarioSamLoginId);

                    AtualizarAnoMesRef(idAlmox);
                    AtualizarJavaScriptBtn();

                    ControlaExibicaoBotaoPagamentoConsumo();
                    ControlaExibicaoBotaoFechamento();
                    BloqueiaBotaoReabertura = false;
                }
                catch (Exception ex)
                {
                    List<string> listaErros = new List<string>();
                    listaErros.Add(ex.Message);
                    ListInconsistencias.ExibirLista(listaErros);
                    ListInconsistencias.DataBind();
                }
            }
            else
            {
                List<string> listaErros = new List<string>();
                listaErros.Add("Fechamento de mês inválido, verificar mês referência.");
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();
            }
        }

        protected void btnReabertura_Click(object sender, EventArgs e)
        {
            int idAlmox = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;

            // estornará o fechamento do mês corrente...
            FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter();

            if (fecha.EstornarFechamentoMensal(idAlmox))
            {
                AtualizarAnoMesRef(idAlmox);
                AtualizarJavaScriptBtn();

                ControlaExibicaoBotaoPagamentoConsumo();
                ControlaExibicaoBotaoFechamento();
            }
        }

        protected void rptFechamento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var utilizaFormatoSIAFEM = this.FormatoSIAFEM();
            FormatarGridItem(e, utilizaFormatoSIAFEM);
        }

        protected void FormatarGridItem(RepeaterItemEventArgs repeaterItemEventArgs, bool utilizaFormatoSIAFEM)
        {
            RepeaterItem itemGrid = repeaterItemEventArgs.Item;

            if (itemGrid.IsNotNull() && (itemGrid.ItemType == ListItemType.Item || itemGrid.ItemType == ListItemType.AlternatingItem))
            {
                Label lblSaldoAnterior = itemGrid.FindControl("lblSaldoAnterior") as Label;
                Label lblSaldoAnteriorValor = itemGrid.FindControl("lblSaldoAnteriorValor") as Label;
                Label lblQtdeEntrada = itemGrid.FindControl("lblQtdeEntrada") as Label;
                Label lblQtdeSaida = itemGrid.FindControl("lblQtdeSaida") as Label;
                Label lblQtdeFechamento = itemGrid.FindControl("lblQtdeFechamento") as Label;
                Label lblValorFechamento = itemGrid.FindControl("lblValorFechamento") as Label;
                Label lblValorEntrada = itemGrid.FindControl("lblValorEntrada") as Label;
                Label lblValorSaida = itemGrid.FindControl("lblValorSaida") as Label;

                ReformatarValoresExibidosControles(base.fmtFracionarioMaterialQtde, lblSaldoAnterior, lblQtdeEntrada, lblQtdeSaida, lblQtdeFechamento);
                ReformatarValoresExibidosControles(base.fmtFracionarioMaterialValorUnitario, lblSaldoAnteriorValor, lblValorEntrada, lblValorSaida, lblValorFechamento);
            }
        }

        private void ReformatarValoresExibidosControles(string strFormatoNumerico, params Control[] arrControlesWeb)
        {
            foreach (var objQueExibeTexto in arrControlesWeb)
            {
                if (objQueExibeTexto.GetType().GetInterfaces().Contains(typeof(ITextControl)))
                    ((ITextControl)objQueExibeTexto).ReformatarValorNumerico(strFormatoNumerico);
            }
        }

        private void ReformatarValoresExibidosControles(string strFormatoNumerico, params HtmlTableCell[] arrControlesWeb)
        {
            foreach (var objHtmlExibidorDeTexto in arrControlesWeb)
            {
                if (objHtmlExibidorDeTexto.GetType().BaseType == typeof(HtmlContainerControl))
                    ((HtmlTableCell)objHtmlExibidorDeTexto).ReformatarValorNumerico(strFormatoNumerico);
            }
        }

        private void DesabilitarBotao(params WebControl[] arrButtonControls)
        {
            foreach (var _botaoWeb in arrButtonControls)
            {
                if (_botaoWeb.GetType().GetInterfaces().Contains(typeof(IButtonControl)))
                    _botaoWeb.Visible = _botaoWeb.Enabled = false;
            }
        }

        public void ExecutaEvento()
        {
            RecuperarDadosSessaoSIAFEM();

            //if (!String.IsNullOrWhiteSpace(webControlClientID))
            {
                //var btnPagamentoSIAFEM = PageBase.GetSession<Button>(ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM);
                var gvceArgs = PageBase.GetSession<GridViewCommandEventArgs>(ChaveSessao_ArgumentEvent_GridViewRowCommand);
                var webControlCommandSource = gvceArgs.CommandSource;
                if (webControlCommandSource.IsNotNull())
                    webControlClientID = (webControlCommandSource as Control).ClientID;

                if (webControlCommandSource is Button)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alterarTexto", "javascript:__doPostBack('" + webControlClientID.Replace("_", "$") + "','');", true);
                }
                else if (webControlCommandSource is CheckBox)
                {
                    (webControlCommandSource as WebControl).Attributes.Add("OnClick", "javascript:__doPostBack('" + webControlClientID.Replace("_", "$") + "','')");
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "executaEstorno", "javascript:__doPostBack('" + webControlClientID.Replace("_", "$") + "','')", true);
                }
            }
        }

        public void ExecutaEventoCancelar()
        {
            Button btnPagamentoSIAFEM = (Button)Session[ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM];
            if (btnPagamentoSIAFEM != null)
            {
                string btnPagamentoSIAFEMTextoOriginal = Session[ChaveSessao_AcaoPagamento_BotaoPagamentoSIAFEM_TextoOriginal].ToString();

                CheckBox chkEstornoNLConsumo = (CheckBox)Session[ChaveSessao_AcaoEstornoPagamento_CheckBoxEstornoPagamentoSIAFEM];
                chkEstornoNLConsumo.Checked = false;
                btnPagamentoSIAFEM.Text = btnPagamentoSIAFEMTextoOriginal;
                btnPagamentoSIAFEM.Font.Underline = false;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SiafemTexto", "javascript:AlterarTextoLinkPagamentoSiafem('" + btnPagamentoSIAFEM.ClientID + "','" + btnPagamentoSIAFEMTextoOriginal + "');", true);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UnCheckedCheckBox", "javascript:UnCheckedCheckBox('" + chkEstornoNLConsumo.ClientID + "');", true);
            }
        }

        protected void btnNLConsumo_Click(object sender, EventArgs e)
        {
            SetSession<IList<PTResMensalEntity>>(null, ChaveSessao_ListaConsumoUA);

            InicializarGridConsumo(false);
            DesabilitarBotao(btnAnalise, btnFechamento, btnSimularFechamento, btnReabertura, btnNLConsumo);
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("FechamentoMensal.aspx"), false);
        }

        //Desabilita botão se Almoxarifado estiver inativo.
        private void DesabilitaBotaoAlmoxarifadoInativo()
        {
           
            int? almoxId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;            
            FechamentoMensalPresenter fecha = new FechamentoMensalPresenter(this);
            var resultado =fecha.VerificarAlmoxarifadoInativos((int)almoxId);
          
            if (!resultado.IndicAtividade)
            {

                btnAnalise.Visible = false;
                btnSimularFechamento.Visible = false;
                btnReabertura.Visible = false;
                btnFechamento.Visible = false;
                btnNLConsumo.Visible = false;
            }

        }
    }
}