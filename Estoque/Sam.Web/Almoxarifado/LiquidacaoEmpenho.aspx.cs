using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using Sam.Domain.Business.SIAFEM;
using CasaDecimais = Sam.Common.Util.GeralEnum.casasDecimais;

namespace Sam.Web.Almoxarifado
{
    public partial class LiquidacaoEmpenho : PageBase, ILiquidacaoEmpenhoView, IPostBackEventHandler
    {
        private string chaveEmpenhoParaLiquidar = "empenhoParaLiquidar";
        private string chaveMovEmpenhosParaLiquidacao = "chaveMovEmpenhosParaLiquidacao";
        private readonly string loginAcesso = new PageBase().GetAcesso.Cpf;

        private string loginSiafem = string.Empty;
        private string senhaSiafem = string.Empty;
        private bool dadosAcessoSiafemPreenchidos = false;

        public string chaveNomeBotaoExecutante = "botaoChamador_OnClick()";
        public void RaisePostBackEvent(string eventArgument) { }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarJavascript();
            //Modal de chave/senha para acesso ao SIAFEM.
            ucAcessoSIAFEM.EvchamaEvento += new Controles.WSSenha.chamaEvento(ExecutaEvento_ucAcessoSIAFEM);
            ucAcessoSIAFEM.EvchamaEventoCancelar += new Controles.WSSenha.EventoCancelar(ExecutaEventoCancelar_ucAcessoSIAFEM);
			//TODO TESTE PARA VER SE POSSO DESATIVAR AQUI.
            //RecuperarDadosSessao();

            if (!IsPostBack) 
            { 
                initDadosEmpenho();
                CarregarMovimentacaoPaginaAnterior();
            }
        }

		//Simulacro para alteração futura de comportamento da tela (referente a autenticação SIAFEM).
        public void ExecutaEvento_ucAcessoSIAFEM()
        {
            PagarEmpenhoSiafisico();
            //RecuperarDadosSessao();
            //ExecutarEventoBotaoChamador();
        }

        private void ExecutarEventoBotaoChamador()
        {
            Button botaoChamador = null;

            botaoChamador = PageBase.GetSession<Button>(chaveNomeBotaoExecutante);
            if(botaoChamador.IsNotNull())
                botaoChamador.RaiseEvent("Click");
        }

        public void ExecutaEventoCancelar_ucAcessoSIAFEM()
        {
            this.loginSiafem= this.senhaSiafem = null;
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
        protected void RecuperarDadosSessao()
        {
            this.loginSiafem = GetSession<string>("loginWsSiafem");
            this.senhaSiafem = GetSession<string>("senhaWsSiafem");

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }

        private void RegistrarJavascript()
        {
            btnPagarEmpenhoSiafem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar a liquidação (SIAFEM) de empenho.');");
            btnPagarEmpenhoSiafisico.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar a liquidação (SIAFISICO) de empenho.');");

            btnEstornarPagamentoEmpenhoSiafem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar o estorno de liquidação (SIAFEM) de empenho.');");
            btnEstornarPagamentoEmpenhoSiafisico.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar o estorno de liquidação (SIAFISICO) de empenho.');");
        }

        private void CarregarMovimentacaoPaginaAnterior()
        {
            #region Variaveis

            GridViewCommandEventArgs gvceArgs = null;
            string argDadosEmpenho = null;
            string codigoEmpenho = null;
            string anoMesRef = null;
            int almoxID = 0;
            int movID = 0;

            #endregion Variaveis

            #region Init

            gvceArgs = GetSession<GridViewCommandEventArgs>(chaveEmpenhoParaLiquidar);

            if (gvceArgs.IsNull())
            {
                gvceArgs = GetSession<GridViewCommandEventArgs>(chaveEmpenhoParaLiquidar);
            }
            else
            {
                argDadosEmpenho = gvceArgs.CommandArgument.ToString();
                SetSession<GridViewCommandEventArgs>(gvceArgs, chaveEmpenhoParaLiquidar);                
            }

            #endregion Init

            #region Parsing EvArgs

            //argDadosEmpenho = gvceArgs.CommandArgument.ToString();
            if (!String.IsNullOrWhiteSpace(argDadosEmpenho))
            {
                char[] CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO = new char[] { '|' };
                char[] CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO = new char[] { ':' };
                string[] argsEvento = argDadosEmpenho.BreakLine(CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO);

                almoxID = Int32.Parse(argsEvento[0].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                codigoEmpenho = argsEvento[1].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                anoMesRef = argsEvento[2].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                movID = Int32.Parse(argsEvento[3].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
            }

            #endregion Parsing EvArgs

            if (IsModoPagamento())
            {
                if (!String.IsNullOrEmpty(codigoEmpenho))
                {
                    LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();

                    var lstMovimentacoes = objPresenter.ListarMovimentacoesEmpenho(almoxID, -1, anoMesRef, codigoEmpenho, true);
                    lstMovimentacoes = Sam.Domain.Business.SIAFEM.ExtensionMethods.ParticionadorPorNumeroDocumento(lstMovimentacoes);
                    var empenhoPossuiMovimentos = lstMovimentacoes.IsNotNullAndNotEmpty();
                    
                    if (empenhoPossuiMovimentos)
                    {
                        gdvMovimentosEmpenho.DataSource = lstMovimentacoes;
                        gdvMovimentosEmpenho.AllowPaging = false;
                        gdvMovimentosEmpenho.DataBind();


                        ValorTotalMovimento = lstMovimentacoes.Sum(_movEmpenho => _movEmpenho.ValorDocumento);
                        btnPagarEmpenhoSiafisico.Visible = btnPagarEmpenhoSiafisico.Enabled = empenhoPossuiMovimentos;
                        btnEstornarPagamentoEmpenhoSiafisico.Visible = false;//empenhoPossuiMovimentos;

                        PageBase.SetSession<IList<MovimentoEntity>>(lstMovimentacoes, chaveMovEmpenhosParaLiquidacao);
                    }
                    else
                    {
                        string MensagemErro = String.Format("{0}\\nDocumento {1}", "Erro ao carregar dados do empenho!", codigoEmpenho);
                        PageBase.SetSession<string>(MensagemErro, "_transPage");
                        PageBase.RemoveSession(chaveEmpenhoParaLiquidar);
                        Response.Redirect("ConsultarLiquidacaoEmpenho.aspx", false);
                    }

                    {
                        var objMovimento = lstMovimentacoes[0];

                        this.CodigoEmpenho = objMovimento.Empenho;
                        this.TipoEmpenho = objMovimento.RetornarDescricaoEmpenho();
                        this.NaturezaDespesaEmpenho = objMovimento.NaturezaDespesaEmpenho;
                    }
                }
            }
            else
            {
                string MensagemErro = "Tela não pode ser acessada diretamente!\nVocê será direcionado para a página correta.";
                PageBase.SetSession<string>(MensagemErro, "_transPage");
                PageBase.RemoveSession(chaveEmpenhoParaLiquidar);
                Response.Redirect("ConsultarLiquidacaoEmpenho.aspx", false);
            }
        }

        protected void initDadosEmpenho()
        {
            UGEEntity ugeLogada = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge;

            if (ugeLogada.IsNotNull())
                lblUGEDescricao.Text = String.Format("{0} - {1}", ugeLogada.Codigo, ugeLogada.Descricao);
        }


        public void ExibirRelatorio() { throw new NotImplementedException();   }

        public System.Collections.SortedList ParametrosRelatorio { get { throw new NotImplementedException(); } }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public IList<MovimentoEntity> PopularGrid() 
        {
            IList<MovimentoEntity> listaMovEmpenhos;

            if (this.gdvMovimentosEmpenho.DataSource.IsNotNull())
            {
                listaMovEmpenhos = this.gdvMovimentosEmpenho.DataSource as IList<MovimentoEntity>;
            }
            else
            {
                listaMovEmpenhos = PageBase.GetSession<IList<MovimentoEntity>>(chaveMovEmpenhosParaLiquidacao);
                listaMovEmpenhos = AtualizarMovimentacoesComObservacoesTela(listaMovEmpenhos, gdvMovimentosEmpenho);

                this.gdvMovimentosEmpenho.DataSource = listaMovEmpenhos;
                this.gdvMovimentosEmpenho.DataBind();
            }

            return listaMovEmpenhos;
        }

        void ICrudView.PopularGrid() { throw new NotImplementedException(); }

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

        public bool BloqueiaCancelar
        {
            //set { btnCancelar.Enabled = !value; }
            set { }
        }

        public bool BloqueiaImprimir
        {
            //set { btnImprimir.Enabled = !value; }
            set { }
        }

        public bool BloqueiaAjuda
        {
            //set { btnAjuda.Enabled = !value; }
            set { }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        #endregion

        public bool MostrarPainelEdicao
        {
            set {  pnlEditar.Visible = value; }
        }

        private bool IsModoPagamento()
        {
            //return (String.IsNullOrWhiteSpace(PageBase.GetSession<string>(chaveEmpenhoParaLiquidar)));
            return (GetSession<GridViewCommandEventArgs>(chaveEmpenhoParaLiquidar).IsNotNull()) ? true : false;
        }

        public int OrgaoId { get; set; }

        public string NaturezaDespesaEmpenho {
            get { return this.lblNaturezaDespesa.Text; }
            set { this.lblNaturezaDespesa.Text = value; }
        }

        protected void PagarEmpenhoSiafisico()
        {
            //CadastrarInfoBotao(sender as Button, e);
            RecuperarDadosSessao();
            if (!this.dadosAcessoSiafemPreenchidos)
                efetuarAcessoSIAFEM();

            #region Popular Grid
            IList<MovimentoEntity> lstMovimentacoes = null;
            lstMovimentacoes = PopularGrid();
            #endregion Popular Grid

            var almoxLogado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

            LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter(this);
            var listaEmpenhosPagos = objPresenter.ProcessarMovimentacoesEmpenho(lstMovimentacoes, loginSiafem, senhaSiafem, loginAcesso);

            #region Retorno SIAFEM
            string nlLiquidacao = null;
            if (listaEmpenhosPagos.IsNotNullAndNotEmpty())
            {
                foreach (var _nlLiquidacao in listaEmpenhosPagos)
                    nlLiquidacao += String.Format("\\r\\n{0},", _nlLiquidacao);

                nlLiquidacao.Replace("\\r\\n\\r\\n", "\\r\\n");
            }

            if (!String.IsNullOrWhiteSpace(nlLiquidacao))
                ExibirMensagem(String.Format("Gerado(s) NL(s) {0}\\r\\nno SIAFEM.", nlLiquidacao));
            #endregion Retorno SIAFEM

        }

        private IList<MovimentoEntity> AtualizarMovimentacoesComObservacoesTela(IList<MovimentoEntity> lstMovimentacoes, GridView gdvMovimentosEmpenho)
        {
            IList<GridViewRow> lstLinhasGrid = null;
            TextBox txtObservacoesMovimentosEmpenho = null;
            Label lblMovimentoId = null;
            int idxLinhaGrid = -1;

            if (gdvMovimentosEmpenho.IsNotNull() && lstMovimentacoes.IsNotNullAndNotEmpty())
            {
                lstLinhasGrid = gdvMovimentosEmpenho.Rows.Cast<GridViewRow>().ToList();

                foreach (var movLinhaGrid in lstLinhasGrid)
                {
                    txtObservacoesMovimentosEmpenho = (TextBox)movLinhaGrid.FindControl("txtObservacoesMovimentosEmpenho");
                    lblMovimentoId = (Label)movLinhaGrid.FindControl("lblMovimentoId");

                    Int32.TryParse(lblMovimentoId.Text, out idxLinhaGrid);

                    var _mov = lstMovimentacoes.Where(movEmpenho => movEmpenho.Id == idxLinhaGrid).Select(mov => mov).FirstOrDefault();
                    if (_mov.IsNotNull())
                        _mov.Observacoes = txtObservacoesMovimentosEmpenho.Text;
                }
            }

            return lstMovimentacoes;
        }

        protected void btnEstornarPagamentoEmpenhoSiafem_Click(object sender, EventArgs e)
        {
            CadastrarInfoBotao(sender as Button, e);

			//Verificar login SIAFEM
            //if (!this.dadosAcessoSiafemPreenchidos)
            //    efetuarAcessoSIAFEM();
        }

        private void CadastrarInfoBotao(Button sender, EventArgs e)
        {
            this.chaveNomeBotaoExecutante = ((Button)sender).ID;

            if (sender.GetType() == typeof(Button) && sender.IsNotNull())
                PageBase.SetSession<Button>(sender, chaveNomeBotaoExecutante);
        }

        protected void btnPagarEmpenhoSiafisico_Click(object sender, EventArgs e)
        {
            //Verificar login SIAFEM
            if (!this.dadosAcessoSiafemPreenchidos)
            {
                efetuarAcessoSIAFEM();
                ExecutarEventoBotaoChamador();
            }

            BotaoEstornoEmpenhoSiafisicoAtivo = true;
            BotaoPagarEmpenhoSiafisicoAtivo = false;
        }

        protected void btnPagarEmpenhoSiafem_Click(object sender, EventArgs e)
        {
            ////Verificar login SIAFEM
            //if (!this.dadosAcessoSiafemPreenchidos)
            //{
            //    efetuarAcessoSIAFEM();
            //    ExecutarEventoBotaoChamador();
            //}

            //BotaoEstornoEmpenhoSiafisicoAtivo = true;
            //BotaoPagarEmpenhoSiafisicoAtivo = false;

            throw new NotImplementedException("Funcionalidade não implementada!");
        }

        protected void btnEstornarPagamentoEmpenhoSiafisico_Click(object sender, EventArgs e)
        {
			//Verificar login SIAFEM
            //if (!this.dadosAcessoSiafemPreenchidos)
            //    efetuarAcessoSIAFEM();
        }

        public bool BloqueiaNovo { get; set; }
        public bool BloqueiaGravar { get; set; }
        public bool BloqueiaExcluir { get; set; }
        public bool BloqueiaDescricao { get; set; }
        public decimal? ValorDocumento { get; set; }
        //public decimal? ValorDocumento
        //{
        //    get { return TratamentoDados.TryParseDecimal(lblValorDocumento.Text); }
        //    set { lblValorDocumento.Text = value.ToString(); }
        //}

        public decimal? ValorTotalMovimento
        {
            get { return TratamentoDados.TryParseDecimal(txtValorTotalMovimento.Text); }
            set { txtValorTotalMovimento.Text = string.Format("{0:0,0.00}", value); }
        }

        public string Id { get; set; }

        public string Observacoes { get; set; }
        public string Instrucoes { get; set; }
        public int? DivisaoId { get; set; }
        public int? AlmoxarifadoIdOrigem { get; set; }

        public int? AlmoxarifadoLogadoId
        {
            get { return new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value; }
        }

        public string CodigoEmpenho
        {
            get { return lblCodigoEmpenho.Text; }
            set { lblCodigoEmpenho.Text = value; }
        }

        public string TipoEmpenho
        {
            get { return lblTipoEmpenho.Text; }
            set { lblTipoEmpenho.Text = value; }
        }


        public bool BotaoPagarEmpenhoSiafisicoAtivo
        {
            get { return (btnPagarEmpenhoSiafisico.Enabled && btnPagarEmpenhoSiafisico.Visible); }
            set { btnPagarEmpenhoSiafisico.Enabled = btnPagarEmpenhoSiafisico.Visible = value; }
        }

        public bool BotaoEstornoEmpenhoSiafisicoAtivo
        {
            get { return (btnEstornarPagamentoEmpenhoSiafisico.Enabled && btnEstornarPagamentoEmpenhoSiafisico.Visible); }
            set { btnEstornarPagamentoEmpenhoSiafisico.Enabled = btnEstornarPagamentoEmpenhoSiafisico.Visible = value; }
        }

        public bool BotaoPagarEmpenhoSiafemAtivo
        {
            get { return (btnPagarEmpenhoSiafem.Enabled && btnPagarEmpenhoSiafem.Visible); }
            set { btnPagarEmpenhoSiafem.Enabled = btnPagarEmpenhoSiafem.Visible = value; }
        }

        public bool BotaoEstornoEmpenhoSiafemAtivo
        {
            get { return (btnEstornarPagamentoEmpenhoSiafem.Enabled && btnEstornarPagamentoEmpenhoSiafem.Visible); }
            set { btnEstornarPagamentoEmpenhoSiafem.Enabled = btnEstornarPagamentoEmpenhoSiafem.Visible = value; }
        }

        private void initModoPagamento()
        {
            #region Variaveis

            string strEvArgs = null;
            GridViewCommandEventArgs gvceArgs = null;

            int almoxID = 0;
            int movID = 0;
            string codigoEmpenho;

            #endregion Variaveis

            #region Init

            if (gvceArgs.IsNotNull())
                SetSession<GridViewCommandEventArgs>(gvceArgs, chaveEmpenhoParaLiquidar);
            else
                gvceArgs = GetSession<GridViewCommandEventArgs>(chaveEmpenhoParaLiquidar);

            #endregion Init

            #region Parsing EvArgs

            strEvArgs = gvceArgs.CommandArgument.ToString();
            if (!String.IsNullOrWhiteSpace(strEvArgs))
            {
                char[] CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO = new char[] { '|' };
                char[] CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO = new char[] { ':' };
                string[] argsEvento = strEvArgs.BreakLine(CST_SEPARADOR_MULTILPLOS_ARGS_EVENTO);

                almoxID = Int32.Parse(argsEvento[0].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
                codigoEmpenho = argsEvento[1].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1];
                movID = Int32.Parse(argsEvento[2].BreakLine(CST_SEPARADOR_PAR_CHAVE_VALOR_ARG_EVENTO)[1]);
            }

            #endregion Parsing EvArgs
        }

        //protected void gdvMovimentosEmpenho_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    GridView gdvItensMovimentacao = null;
        //    Label lblMovimentoId = null;


        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();
        //        IList<MovimentoItemEntity> itensMovimento = null;
        //        MovimentoEntity movEmpenho = null;

        //        lblMovimentoId = (Label)e.Row.FindControl("lblMovimentoId");
        //        var _movimentoID = Int32.Parse(lblMovimentoId.Text);

        //        movEmpenho = objPresenter.ObterMovimento(_movimentoID);
        //        itensMovimento = movEmpenho.MovimentoItem;

        //        gdvItensMovimentacao = (GridView)e.Row.FindControl("gdvItensMovimentacao");

        //        if (gdvItensMovimentacao.IsNotNull())
        //        {
        //            gdvItensMovimentacao.DataSource = itensMovimento;
        //            gdvItensMovimentacao.DataBind();
        //        }
        //    }
        //}

        protected void gdvMovimentosEmpenho_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();
            IList<MovimentoItemEntity> itensMovimento = null;
            MovimentoEntity movEmpenho = null;

            GridView gdvItensMovimentacao = null;
            Label lblMovimentoId = null;
            Label lblValorTotalGrupoParaPagar = null;
            TextBox txtObservacoesMovimentosEmpenho = null;
            int _movimentoID = -1;

            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem.IsNotNull())
            {
                movEmpenho = e.Row.DataItem as MovimentoEntity;
                
                txtObservacoesMovimentosEmpenho = (TextBox)e.Row.FindControl("txtObservaoesMovimentosEmpenho");
                if (txtObservacoesMovimentosEmpenho.IsNotNull() && !String.IsNullOrWhiteSpace(txtObservacoesMovimentosEmpenho.Text))
                    movEmpenho.Observacoes = txtObservacoesMovimentosEmpenho.Text.Substring(0 ,77);


                lblMovimentoId = (Label)e.Row.FindControl("lblMovimentoId");
                if (lblMovimentoId.IsNotNull() && !String.IsNullOrWhiteSpace(lblMovimentoId.Text) && Int32.TryParse(lblMovimentoId.Text, out _movimentoID))
                    itensMovimento = movEmpenho.MovimentoItem;

                gdvItensMovimentacao = (GridView)e.Row.FindControl("gdvItensMovimentacao");
                if (gdvItensMovimentacao.IsNotNull())
                {
                    gdvItensMovimentacao.DataSource = itensMovimento;
                    gdvItensMovimentacao.DataBind();

                    lblValorTotalGrupoParaPagar = (Label)e.Row.FindControl("lblValorTotalGrupoParaPagar");
                    if (lblValorTotalGrupoParaPagar.IsNotNull())
                        lblValorTotalGrupoParaPagar.Text = String.Format("R$ {0:0,0.00}", itensMovimento.Sum(movItem => movItem.ValorMov.Value.Truncar((int)CasaDecimais.paraValorMonetario)));
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private string generateRandomNL()
        {
            var mesRefAlmoxLogado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4);
            return String.Format("{0}NL{1:D5}", mesRefAlmoxLogado, (new Random()).Next(0, 10000));
        }
    }

    public static class ExecutaEventosBotaoChamador
    {

        public static void RaiseEvent(this object source, string eventName)
        {
            var infoEvent = source.GetType().GetEvent(eventName);
            var infoMethod = infoEvent.GetRaiseMethod();

            if (infoMethod != null)
                infoMethod.Invoke(source, null);
        }
    }

    public static class LiquidacaoExthensionsMethods
    {
        public static string RetornarNaturezaDespesaEmpenho(this IList<MovimentoEntity> lstMovimentacoes)
        {
            if (lstMovimentacoes.IsNull())
                throw new ArgumentNullException("Lista de Movimentações Nula!");

            string _descNaturezaDespesa = null;

            lstMovimentacoes.ToList()
                            .ForEach(_movimentacaoEmpenho =>
                            {
                                if (String.IsNullOrWhiteSpace(_descNaturezaDespesa))
                                    _descNaturezaDespesa = (_movimentacaoEmpenho.NaturezaDespesaEmpenho);
                                else if (_descNaturezaDespesa != _movimentacaoEmpenho.NaturezaDespesaEmpenho && !_descNaturezaDespesa.Split('|').Contains(_movimentacaoEmpenho.NaturezaDespesaEmpenho))
                                    _descNaturezaDespesa = String.Format("{0}|{1}", _descNaturezaDespesa, _movimentacaoEmpenho.NaturezaDespesaEmpenho);
                            });

            return _descNaturezaDespesa;
        }
        public static string NotasLiquidacaoSiafisico(this MovimentoEntity movimentacaoEmpenho)
        {
            if (movimentacaoEmpenho.IsNull() && !movimentacaoEmpenho.MovimentoItem.IsNotNullAndNotEmpty())
                throw new ArgumentNullException("Movimentação ou Lista de Itens Nula!");

            IList<string> lstNLsLiquidacao = new List<string>(movimentacaoEmpenho.MovimentoItem.Count);
            
            movimentacaoEmpenho.MovimentoItem.ToList()
                               .ForEach(_movItemEmpenho => {
                                   if (!String.IsNullOrWhiteSpace(_movItemEmpenho.NLiquidacao) && !lstNLsLiquidacao.Contains(_movItemEmpenho.NLiquidacao))
                                       lstNLsLiquidacao.Add(_movItemEmpenho.NLiquidacao);
                               });

            lstNLsLiquidacao.ToList().Sort();
            return String.Join("|", lstNLsLiquidacao).ToString();
        }
    }
}
