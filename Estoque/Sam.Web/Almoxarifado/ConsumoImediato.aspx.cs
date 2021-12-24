using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using System.Text.RegularExpressions;
using Sam.Entity;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Web.UI.HtmlControls;
using Sam.Domain.Business.SIAFEM;
using Sam.Common.Enums;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;
using TipoEmpenho = Sam.Common.Util.GeralEnum.TipoEmpenho;
using tipoOperacao = Sam.Common.Util.GeralEnum.OperacaoEntrada;
using tipoMovimentacao = Sam.Common.Util.GeralEnum.TipoMovimento;




namespace Sam.Web.Almoxarifado
{
    public partial class ConsumoImediato : PageBase, IConsumoImediatoView, IPostBackEventHandler
    {
        //Alt
        private MovimentoEntity movimento = new MovimentoEntity();
        private string sessaoMov = "movimento";
        private string loginSiafem = string.Empty;
        private string senhaSiafem = string.Empty;
        private bool dadosAcessoSiafemPreenchidos = false;
        private bool perfilEditar;
        private readonly string ChaveSessao_CampoTipoMaterial_LancamentoSIAFEM = "valorCampoTipoMaterial";
        private readonly string subItensMovimento = "subItensParaFiltro";
        private readonly string loginAcesso = new PageBase().GetAcesso.LoginBase;
        private readonly string cstListagemPTResAcao = "listagemPTResAcao";

        public void RaisePostBackEvent(string eventArgument) { }

        //Sessão do Movimento e MovimentoItens inseridos do grid


        public class GridItem
        {
            public int Item { get; set; }
            public string UniFornS { get; set; }
        }


        public void GetSessao()
        {
            bool atualizar = false;

            if (movimento == null)
                atualizar = true;
            else
            {
                if (movimento.MovimentoItem == null)
                    atualizar = true;
            }

            if (atualizar)
            {
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    movimento = GetSession<MovimentoEntity>(sessaoMov);
                }
            }
        }

        //Simulacro para alteração futura de comportamento da tela (referente a autenticação SIAFEM).
        public void ExecutaEvento()
        {
            RecuperarDadosSessao();

            var movimentacaoNovaOuEstorno = (this.TipoOperacao == (int)tipoOperacao.Nova || this.TipoOperacao == (int)tipoOperacao.Estorno);
            var tipoMovimentacaoEmpenhoOuRestosAPagar = ((this.TipoMovimento == (int)tipoMovimentacao.ConsumoImediatoEmpenho) || (this.TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho));

            if (movimentacaoNovaOuEstorno && tipoMovimentacaoEmpenhoOuRestosAPagar)
                btnListarEmpenhos_Click(null, null);
        }

        public void ExecutaEventoCancelar()
        {
            this.loginSiafem = this.senhaSiafem = null;
        }

        protected void RecuperarDadosSessao()
        {
            this.loginSiafem = GetSession<string>("loginWsSiafem");
            this.senhaSiafem = GetSession<string>("senhaWsSiafem");

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            ucAcessoSIAFEM.EvchamaEvento += new Controles.WSSenha.chamaEvento(ExecutaEvento);
            ucAcessoSIAFEM.EvchamaEventoCancelar += new Controles.WSSenha.EventoCancelar(ExecutaEventoCancelar);
            RecuperarDadosSessao();

            RegistraJavaScript();

            uc3SubItem.FiltrarAlmox = true;
            uc3SubItem.UsaSaldo = false;
            uc3SubItem.FiltrarNaturezasDespesaConsumoImediato = true;
            PesquisaDocumento.TipoOperacao = TipoOperacao.ToString();
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);

            // carregar grid (se hdfMovimentoId não estiver nulo)
            if (hdfMovimentoId.Value != "")
            {
                if (hdfMovimentoId.Value != "0")
                {
                    CarregaGridSubItemMaterial(Common.Util.TratamentoDados.TryParseInt32(this.Id));
                }
            }

            this.lblTipoEmpenho.Visible = false;
            this.lblTipoEmpenho.Text = String.Empty;

            if (!IsPostBack)
            {
                RemoveSession(sessaoMov);
                mat.Load();
                lblFornecedor.Visible = false;
                imgLupaFornecedor.Visible = false;
                btnNovo.Visible = false;
                ExibirGeradorDescricao = false;
                PerfilConsultar = true; //TODO DEBUGGING
                PopularDadosUGETodosCod();
                //PopularDadosSubItemClassif();
                PopularListaUAs(); //UA da UGE selecionada no combo
                PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial);
                hidAlmoxId.Value = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value.ToString();
                lblDocumentoAvulsoAnoMov.Text = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + "/";
                ExibirImprimir = false;
                fldSetOperacaoEntrada.Disabled = true;
                CarregaGridMovimentoItem(movimento);
                RemoveSession(subItensMovimento);
            }
            else
            {
                GetSessao();
                if (this.idSubItem.Value != null)
                {
                    if (this.idSubItemOld.Value != "")
                    {
                        if (this.idSubItemOld.Value != this.idSubItem.Value)
                        {
                            IdentificacaoLoteItem = string.Empty;
                            txtVencimentoLote.Text = string.Empty;
                            txtVencimentoLote.Enabled = false;
                        }
                    }

                    this.idSubItemOld.Value = this.idSubItem.Value;
                }
            }

            OcultaDescricaoItem();
            PageBase.SetSession<string>(loginAcesso, "loginAcesso");

            VerificaMesFechadoSIAFEM();
            VerificaRestricaoFechamentoExercicioFiscal();
            DesabilitaNovaEntradaeEstorno__EntradasPorEmpenho();
        }

        /// <summary>
        /// Caso o mês esteja fechado, permitir apenas reimpressão de notas.
        /// </summary>
        private void VerificaMesFechadoSIAFEM()
        {
            ConsumoImediatoPresenter objPresenter = null;
            string anoMesRef = null;
            bool mesJahFechado = false;


            anoMesRef = this.AnoMesReferencia;
            objPresenter = new ConsumoImediatoPresenter(this);
            mesJahFechado = objPresenter.VerificaStatusFechadoMesReferenciaSIAFEM(anoMesRef, true);

            if (mesJahFechado)
            {
                BloqueiaTipoOperacao = BloqueiaNovo = BloqueiaEstornar = true;
                TipoOperacao = (int)GeralEnum.OperacaoEntrada.NotaRecebimento;
            }
        }

        /// <summary>
        /// Caso o almoxarifado esteja situado no mês de referência 12/xxxx, e o mês-calendário esteja em 01/xxxx+1 (ano seguinte), não exibir as opções de 'Entrada Por Empenho'  e 'Restos a Pagar'
        /// </summary>
        private void VerificaRestricaoFechamentoExercicioFiscal()
        {
            int almoxID = -1;
            bool ehPeriodoFechamentoExercicioFiscal = false;
            FechamentoMensalPresenter objPresenter = null;

            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            var selecionadoEntradaPorEmpenho = (TipoMovimento == tipoMovimentacao.ConsumoImediatoEmpenho.GetHashCode());
            objPresenter = new FechamentoMensalPresenter();
            almoxID = almoxLogado.Id.Value;

            // Bloquear entrada Empenho - mesRef 12 e mesAtual 01
            //ehPeriodoFechamentoExercicioFiscal = objPresenter.VerificaRestricaoFechamentoExercicioFiscal(almoxID);
            //if (ehPeriodoFechamentoExercicioFiscal && selecionadoEntradaPorEmpenho)
            //    TipoOperacao = tipoOperacao.NotaRecebimento.GetHashCode();

            BloqueiaNovo = BloqueiaEstornar = (ehPeriodoFechamentoExercicioFiscal && selecionadoEntradaPorEmpenho);
            BloqueiaTipoOperacao = (ehPeriodoFechamentoExercicioFiscal && selecionadoEntradaPorEmpenho);
        }

        //VALIDACAO TEMPORARIA (MES-REFERENCIA EM 12/2020)
        private void DesabilitaNovaEntradaeEstorno__EntradasPorEmpenho()
        {
            int[] tiposEntradaPorEmpenho = null;
            bool ehHabilitadoEntradaPorEntradaPorEmpenho = false;


            tiposEntradaPorEmpenho = new int[] {
                                                   (int)tipoMovimentacao.EntradaPorEmpenho
                                                 , (int)tipoMovimentacao.ConsumoImediatoEmpenho
                                               };

            var almoxLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            if (almoxLogado.IsNotNull())
            {
                if (almoxLogado.MesRef == "202012")
                    if (!ehHabilitadoEntradaPorEntradaPorEmpenho && tiposEntradaPorEmpenho.Contains(TipoMovimento))
                        TipoOperacao = tipoOperacao.NotaRecebimento.GetHashCode();
            }
        }

        private void RegistraJavaScript()
        {
            btnGravarItem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluirItem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para concluir a entrada de materiais.');");
            btnEstornar.Attributes.Add("OnClick", "return confirm('Pressione OK para estornar a entrada de materiais.');");

            txtQtdeMov.Attributes.Add("onblur", "return calcularPrecoUnitario();");
            txtQtdeMov.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
            txtValorMovItem.Attributes.Add("onblur", "return calcularPrecoUnitario();");
            txtDocumentoAvulso.Attributes.Add("onblur", "preencheZeros(this,'8') ");


            ScriptManager.RegisterStartupScript(this.txtDataEmissao, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtDataReceb, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtVencimentoLote, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtValorTotal, GetType(), "numerico", "$('.numerico').floatnumber(',',2);", true);
            ScriptManager.RegisterStartupScript(this.txtPrecoUnit, GetType(), "numerico", "$('.numerico').floatnumber(','," + base.numCasasDecimaisValorUnitario + ");", true);
            ScriptManager.RegisterStartupScript(this.txtQtdeMov, GetType(), "numerico", "$('.numerico').floatnumber(','," + base.numCasasDecimaisMaterialQtde + ");", true);
            ScriptManager.RegisterStartupScript(this.txtValorTotalMovimento, GetType(), "numerico", "$('.numerico').floatnumber(',',2);", true);
            ScriptManager.RegisterStartupScript(this.txtDocumentoAvulso, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
        }

        protected void carregarPermissaoAcesso()
        {
            // carregar permissão
            switch (AutorizaTransacao())
            {
                case Enuns.AcessoTransacao.Edita:
                    perfilEditar = true;
                    break;
                default:
                    perfilEditar = false;
                    break;
            }
        }

        public void PopularListaUAs()
        {
            int codigoUGESelecionada = -1;
            ConsumoImediatoPresenter presenterTela = null;
            InicializarDropDownList(ddlUA);


            var ugeSelecionada = this.ddlUGE.SelectedItem.Text.BreakLine(0);
            if (!string.IsNullOrWhiteSpace(ugeSelecionada) && Int32.TryParse(ugeSelecionada, out codigoUGESelecionada))
            {
                presenterTela = new ConsumoImediatoPresenter(this);
                ddlUA.DataSource = presenterTela.ListarUAsPorUGE(codigoUGESelecionada);
                ddlUA.DataBind();

                this.CodigoUGE = codigoUGESelecionada;
            }
        }

        public void PopularListaAlmoxarifado()
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            InicializarDropDownList(ddlAlmoxarifadoTransfer);

            ddlAlmoxarifadoTransfer.AppendDataBoundItems = true;
            ddlAlmoxarifadoTransfer.DataSource = mat.PopularListaAlmoxarifado(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value).Where(a => a.Id != new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            ddlAlmoxarifadoTransfer.DataBind();
        }

        public void PopularDadosSubItemClassif()
        {
            throw new NotImplementedException("Funcionalidade não permitida para esta tela!");
        }

        #region Bloquear Controles

        public bool BloqueiaItemEfetivado
        {
            set
            {
                this.BloqueiaGravarItem = value;
                this.BloqueiaExcluirItem = value;
                this.BloqueiaItemQtdeMov = value;
                this.BloqueiaItemPrecoUnit = true;
                this.BloqueiaItemLoteFabric = value;
                this.BloqueiaItemLoteIdent = value;
                this.BloqueiaItemLoteDataVenc = value;
            }
        }

        public bool BloqueiaValorTotal { set { txtValorTotal.Enabled = !value; } }
        public bool BloqueiaTipoMovimento { set { rblTipoMovimento.Enabled = !value; } }
        public bool BloqueiaTipoOperacao { set { rblTipoOperacao.Enabled = !value; } }
        public bool BloqueiaGravarItem { set { btnGravarItem.Enabled = !value; } }
        public bool BloqueiaExcluirItem { set { btnExcluirItem.Enabled = !value; } }
        public bool BloqueiaItemQtdeMov { set { txtQtdeMov.Enabled = !value; } }
        public bool BloqueiaItemQtdeLiq { set { txtQtdeLiq.Enabled = !value; } }
        public bool BloqueiaItemPrecoUnit { set { txtPrecoUnit.Enabled = !value; } }
        public bool BloqueiaItemLoteFabric { set { txtFabricLoteItem.Enabled = !value; } }
        public bool BloqueiaItemLoteIdent { set { txtIdentLoteItem.Enabled = !value; } }
        public bool BloqueiaItemLoteDataVenc { set { txtVencimentoLote.Enabled = !value; } }
        public bool BloqueiaGeradorDescricao
        {
            set { txtGeradorDescricao.Enabled = !value; }
        }

        public bool ExibirGeradorDescricao
        {
            set
            {
                if (value == false)
                    txtGeradorDescricao.CssClass = "esconderControle";
                else
                    txtGeradorDescricao.CssClass = "mostrarControle";
            }
        }

        public bool ExibirListaEmpenho
        {
            set
            {
                if (value)
                    ddlEmpenho.CssClass = "mostrarControle";
                else
                    ddlEmpenho.CssClass = "esconderControle";
            }
            get
            {
                return (ddlEmpenho.CssClass == "mostrarControle");
            }
        }

        public bool ExibirNumeroEmpenho
        {
            set { lblEmpenho.Visible = txtEmpenho.Visible = value; }
            get { return (txtEmpenho.Visible &= lblEmpenho.Visible); }
        }

        public bool BloqueiaListaUGE
        {
            set
            {
                //bloqueia uge para todas as entradas
                //ddlUGE.Enabled = false;
                ddlUGE.Enabled = value;
            }
        }
        public bool BloqueiaListaUA
        {
            set { ddlUA.Enabled = value; }
        }
        public bool BloqueiaDocumento { set { txtDocumentoAvulso.Enabled = !value; } }
        public bool BloqueiaNumeroDocumento { set { txtDocumentoAvulso.Enabled = !value; } }
        public bool ExibirDocumentoAvulso { set { txtDocumentoAvulso.Visible = value; } }
        public bool BloqueiaListaIndicadorAtividade { set { throw new NotImplementedException(); } }

        public bool BloqueiaListaAlmoxarifado
        {
            set
            {
                ddlAlmoxarifadoTransfer.Enabled = !value;
                if (value == false)
                    ddlAlmoxarifadoTransfer.CssClass = "mostrarControle";
                else
                    ddlAlmoxarifadoTransfer.CssClass = "esconderControle";
            }
        }



        protected bool tipoMovimentoSelecionado()
        {
            for (int i = 0; i < rblTipoMovimento.Items.Count; i++)
            {
                if (rblTipoMovimento.Items[i].Selected == true)
                    return true;
            }
            return false;
        }

        #endregion

        #region Popular Combos

        public IList<MovimentoItemEntity> ListarMovimentoItens(int startRowIndexParameterName,
                int maximumRowsParameterName, string _documento)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            IList<MovimentoItemEntity> itens = mat.ListarMovimentoItens(startRowIndexParameterName, maximumRowsParameterName, _documento);
            gridSubItemMaterial.DataSource = itens;
            gridSubItemMaterial.DataBind();
            return itens;
        }

        public void PopularDadosUGETodosCod()
        {
            InicializarDropDownList(ddlUGE);

            ddlUGE.AppendDataBoundItems = true;
            UGEPresenter uge = new UGEPresenter();

            ddlUGE.DataSource = uge.PopularDadosTodosCodPorGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            ddlUGE.DataBind();
            int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            if (ddlUGE.Items.Count > 0)
                ddlUGE.SelectedValue = ugeLogado.ToString();
        }

        protected void carregarListaSubItens(DropDownList drp, string naturezaDespesa, int itemMaterialId)
        {
            drp.DataSource = null;
            drp.Items.Insert(0, "- Selecione -");
            drp.AppendDataBoundItems = true;
            drp.DataSource = new SubItemMaterialPresenter().ListarSubItemByAlmoxItemMaterial(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, itemMaterialId, naturezaDespesa);
            drp.DataBind();
        }

        /// <summary>
        /// Sobrecarga Experimental
        /// </summary>
        /// <param name="drp"></param>
        /// <param name="naturezaDespesa"></param>
        /// <param name="itemMaterialId"></param>
        protected void carregarListaSubItens(DropDownList drp, string naturezaDespesa, int itemMaterialId, bool NewMethod = true)
        {
            IList<SubItemMaterialEntity> listaSubItem = null;
            ListItem ItemLista = null;

            listaSubItem = new SubItemMaterialPresenter().ListarSubItemByAlmoxItemMaterial(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, itemMaterialId, naturezaDespesa);
            drp.DataSource = null;

            drp.Items.Insert(0, "- Selecione -");
            foreach (var item in listaSubItem)
            {
                ItemLista = new ListItem();
                ItemLista.Text = item.CodigoFormatado;
                ItemLista.Value = item.Id.Value.ToString();
                ItemLista.Attributes.Add("title", item.Descricao);

                drp.Items.Add(ItemLista);
            }

            if (drp.Items.Count >= 2)
                drp.SelectedIndex = 1;
        }

        public void PopularGrid() { throw new NotImplementedException(); }
        public void PopularListaFornecedor() { throw new NotImplementedException(); }
        public void PopularListaUGE() { throw new NotImplementedException(); }

        public void PopularDadosSubItemMaterial(string txtSubItemCodigo)
        {
            List<string> msgs = new List<string>();
            BloqueiaGravarItem = false;
            HabilitarLote = false;
        }

        private void OcultaDescricaoItem()
        {
            if (String.IsNullOrEmpty(lblItemMaterialDescricao.Text))
                ItemDescricao.Visible = false;
            else
                ItemDescricao.Visible = true;
        }

        //public void PopularListaTipoMovimentoEntrada()
        public void PopularListaTipoMovimentoConsumoImediato()
        {
            IList<TipoMovimentoEntity> listaMovimentacoesEntrada = null;
            TipoMovimentoPresenter objPresenter = new TipoMovimentoPresenter();

            listaMovimentacoesEntrada = objPresenter.PopularListaTipoMovimentoConsumoImediato();


            rblTipoMovimento.DataSource = listaMovimentacoesEntrada;
            rblTipoMovimento.AppendDataBoundItems = true;
            rblTipoMovimento.DataTextField = "Descricao";
            rblTipoMovimento.DataValueField = "Id";

            rblTipoMovimento.DataBind();
        }

        [Obsolete("Método usado apenas quando o combo de UGE é alterado, opção atualmente desativada.")]
        public void PopularDadosDocumentoTodosCodPorUge(int _ugeID, int _tipoMovimento, bool pBlnNewMethod)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            WebUtil webUtil = new WebUtil();

            string userName = string.Empty;
            string password = string.Empty;

            // pedir para o usuário inserir sua senha
            password = GetSession<string>("senhaWsSiafem");
            if (String.IsNullOrWhiteSpace(password))
            {
                webUtil.runJScript(this, "OpenModalSenhaWs();");
            }

            userName = GetSession<string>("loginWsSiafem");
            password = GetSession<string>("senhaWsSiafem");

            // carregar a lista de empenhos do mês corrente

            if (_tipoMovimento.Equals((int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho))
            {

                IList<MovimentoEntity> movEmpenho = mat.CarregarListaEmpenho(_ugeID, this.loginSiafem, this.senhaSiafem);

                // remove a sessão caso haja irregularidade com o usuário.
                if (movEmpenho != null)
                {
                    int contador = 0;

                    if (movEmpenho.Count > 0 && !String.IsNullOrWhiteSpace(movEmpenho[0].Observacoes))
                        return;

                    InicializarDropDownList(ddlEmpenho);
                    foreach (MovimentoEntity mov in movEmpenho)
                    {
                        contador++;
                        ddlEmpenho.Items.Add(mov.NumeroDocumento);
                        ddlEmpenho.Items[contador].Value = contador.ToString();
                    }
                }

                return;
            }
        }

        private void InicializarDropDownList(DropDownList comboLista)
        {
            comboLista.InserirElementoSelecione(true);
        }

        #endregion

        #region Propriedades

        public string Id
        {
            get
            {
                if (hdfMovimentoId != null)
                    return hdfMovimentoId.Value.ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (value != null)
                    hdfMovimentoId.Value = value.ToString();
                else
                    hdfMovimentoId.Value = string.Empty;
            }
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string NumeroDocumentoCombo { set; get; }
        public string NumeroEmpenhoCombo
        {
            set
            {
                ListItem item = ddlEmpenho.Items.FindByText(value.ToString());
                if (item != null)
                {
                    ddlEmpenho.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlEmpenho.ClearSelection();
            }
            get
            {
                string _strRetorno = null;

                if (ddlEmpenho.SelectedItem.IsNotNull())
                {
                    _strRetorno = ddlEmpenho.SelectedItem.Text;
                    if (_strRetorno.ToUpperInvariant().Contains("SELECIONE") || _strRetorno.ToUpperInvariant().Contains("TODOS"))
                        _strRetorno = "";
                }
                return _strRetorno;
            }
        }

        public int? DivisaoId { get; set; }

        public int TipoOperacao
        {
            get
            {
                int iRetorno = 0;
                if (rblTipoOperacao.SelectedIndex != -1)
                    iRetorno = Convert.ToInt32(rblTipoOperacao.SelectedValue);

                return iRetorno;
            }
            set
            {
                ListItem item = rblTipoOperacao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    rblTipoOperacao.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public int TipoMovimento
        {
            set
            {
                ListItem item = rblTipoMovimento.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    rblTipoMovimento.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                int iRetorno = 0;
                if (rblTipoMovimento.SelectedIndex != -1)
                    iRetorno = Convert.ToInt32(rblTipoMovimento.SelectedValue);

                return iRetorno;
            }
        }

        public int? AlmoxarifadoIdOrigem
        {
            set
            {
                hdfAlmoxTransId.Value = value.ToString();
            }
            get
            {
                return Convert.ToInt32(TratamentoDados.TryParseInt32(hdfAlmoxTransId.Value));
            }
        }

        public string OrgaoTransferencia
        {
            get { return txtOrgao_Transferencia.Text; }
            set { txtOrgao_Transferencia.Text = value; }
        }

        public string GeradorDescricao
        {
            get { return txtGeradorDescricao.Text; }
            set { txtGeradorDescricao.Text = value; }
        }

        public string SubItemMaterialDescricao
        {
            get { return txtDescricao.Text; }
            set { txtDescricao.Text = value; }
        }

        public string ItemMaterialDescricao
        {
            get { return lblItemMaterialDescricao.Text; }
            set { lblItemMaterialDescricao.Text = value; }
        }


        public int? EmpenhoEventoId { get; set; }
        public int? EmpenhoLicitacaoId { get; set; }
        public int OrgaoId { get; set; }

        public int UgeId
        {
            set
            {
                ListItem item = ddlUGE.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;

                    PopularListaUAs();
                }

            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlUGE.SelectedValue)); }
        }

        public int CodigoUGE
        {
            set
            {
                ListItem item = ddlUGE.Items.Cast<ListItem>().Where(_item => _item.Value.Contains(value.ToString())).FirstOrDefault();
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                int _tryparseCodigoUGE = -1;
                Int32.TryParse(ddlUGE.SelectedItem.Text.BreakLine(0), out _tryparseCodigoUGE);

                return _tryparseCodigoUGE;
            }
        }

        public int UaId
        {
            set
            {
                ListItem item = ddlUA.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUA.ClearSelection();
                    item.Selected = true;
                }

            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlUA.SelectedValue)); }
        }

        public int CodigoUA
        {
            set
            {
                ListItem item = ddlUA.Items.Cast<ListItem>().Where(_item => _item.Value.Contains(value.ToString())).FirstOrDefault();
                if (item != null)
                {
                    ddlUA.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                int _tryparseCodigoUA = -1;
                if (this.ddlUA.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(ddlUA.SelectedItem.Text.BreakLine(0), out _tryparseCodigoUA))
                    _tryparseCodigoUA = Int32.Parse(this.ddlUA.SelectedItem.Text.BreakLine(0));

                return _tryparseCodigoUA;
            }
        }

        public int? PTResCodigo
        {
            get
            {
                int? _codigoPTRes = null;
                var _tryParsePTResCodigo = -1;

                //if (this.ddlPTResItemMaterial.Items.Cast<ListItem>().ToList().HasElements())
                if (this.ddlPTResItemMaterial.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(this.ddlPTResItemMaterial.SelectedItem.Text.BreakLine(0), out _tryParsePTResCodigo))
                    _codigoPTRes = Int32.Parse(this.ddlPTResItemMaterial.SelectedItem.Text.BreakLine(0));
                else
                    _codigoPTRes = _tryParsePTResCodigo;

                return _codigoPTRes;
            }
            set
            {
                int idxOpcaoCombo = -1;

                if (!String.IsNullOrWhiteSpace(value.ToString()))
                {
                    ListItem _itemCombo = this.ddlPTResItemMaterial.Items.Cast<ListItem>().Where(_ptres => _ptres.Text.Contains(value.ToString())).FirstOrDefault();
                    idxOpcaoCombo = this.ddlPTResItemMaterial.Items.IndexOf(_itemCombo);

                    if (_itemCombo.IsNotNull() && idxOpcaoCombo != -1)
                        //this.ddlPTResItemMaterial.SelectedIndex = idxOpcaoCombo;
                        this.ddlPTResItemMaterial.SelectedValue = _itemCombo.Value;
                }
            }
        }

        public string PTResAcao
        {
            get
            {
                string _codigoPTResAcao = null;
                var _tryParsePTResAcao = -1;

                try
                {
                    if (this.ddlPTResAcao.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(this.ddlPTResAcao.SelectedItem.Text, out _tryParsePTResAcao))
                        _codigoPTResAcao = this.ddlPTResAcao.SelectedItem.Text;
                    else
                        _codigoPTResAcao = _tryParsePTResAcao.ToString();
                }
                catch
                {
                    //logErro.GravarMsgInfo("Tela Requisição Material", "Erro ao obter valor campo PTResCodigo");
                }

                return _codigoPTResAcao;
            }
            set
            {
                int idxOpcaoCombo = -1;

                if (!String.IsNullOrWhiteSpace(value))
                {
                    ListItem _itemCombo = this.ddlPTResAcao.Items.Cast<ListItem>().Where(_ptres => _ptres.Text.Contains(value)).FirstOrDefault();
                    idxOpcaoCombo = this.ddlPTResAcao.Items.IndexOf(_itemCombo);

                    if (_itemCombo.IsNotNull() && idxOpcaoCombo != -1)
                        this.ddlPTResAcao.SelectedIndex = idxOpcaoCombo;
                }
            }
        }

        public int? MovimentoItemId
        {
            get { return hidtxtMovimentoItemId.Value.ToString() == "" ? 0 : Convert.ToInt32(hidtxtMovimentoItemId.Value.ToString()); }
            set { hidtxtMovimentoItemId.Value = value.ToString(); }
        }

        public string UnidadeCodigo
        {
            set { txtUnidadeForn.Text = value; }
        }

        public int? FornecedorId
        {
            get
            {
                int valorConvertido = -1;
                int valorInteiro = -1;

                if (Int32.TryParse(txtCodFornecedor.Text, out valorInteiro))
                    valorConvertido = valorInteiro;

                return valorConvertido;
            }
            set { txtCodFornecedor.Text = value.ToString(); }
        }

        public int? ItemMaterialId
        {
            get { return hidtxtItemMaterialId.Value.ToString() == "" ? 0 : Convert.ToInt32(hidtxtItemMaterialId.Value.ToString()); }
            set { hidtxtItemMaterialId.Value = value.ToString(); }
        }

        public int? SubItemMaterialId
        {
            set { idSubItem.Value = value.ToString(); }
            get { return TratamentoDados.TryParseInt32(idSubItem.Value); }
        }


        public string SubItemMaterialTxt
        {
            set { hidtxtSubItemMaterialId.Value = value; }
            get { return hidtxtSubItemMaterialId.Value.ToString(); }
        }


        public long? SubItemMaterialCodigo
        {
            set { txtSubItem.Text = value.ToString(); }
            get { return TratamentoDados.TryParseLong(txtSubItem.Text); }
        }


        public int? ItemMaterialCodigo
        {
            get { return Convert.ToInt32(txtItemMaterial.Text); }
            set { txtItemMaterial.Text = value != null ? value.ToString() : null; }
        }

        public string NumeroDocumento
        {
            get { return txtDocumentoAvulso.Text; }
            set { txtDocumentoAvulso.Text = value; }
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

        public DateTime? DataDocumento
        {
            get { return TratamentoDados.TryParseDateTime(txtDataEmissao.Text); }
            set
            {
                if (value.HasValue)
                    txtDataEmissao.Text = Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro);
                else
                    txtDataEmissao.Text = "";
            }
        }

        public DateTime? DataMovimento
        {
            get { return TratamentoDados.TryParseDateTime(txtDataReceb.Text); }
            set
            {
                if (value.HasValue)
                    txtDataReceb.Text = Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro);
                else
                    txtDataReceb.Text = "";
            }
        }

        public void LimparGridSubItemMaterial()
        {
            gridSubItemMaterial.DataSourceID = null;
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
        }

        public string FonteRecurso { get; set; }

        public decimal? ValorDocumento
        {

            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtValorTotal.Text, true)); }
            set
            {
                decimal valor = 0;
                if (movimento != null)
                {

                    if (movimento.MovimentoItem != null)
                    {
                        foreach (var item in movimento.MovimentoItem)
                        {
                            if (item.SaldoLiq != null)
                            {
                                item.ValorMov = item.SaldoLiq;
                            }
                            else
                            {
                                item.ValorMov = item.ValorMov != null ? item.ValorMov : 0;
                            }
                            valor += Convert.ToDecimal(item.ValorMov).Truncar(2, true);
                        }

                        value = valor;
                    }
                }

                if (value.HasValue) txtValorTotal.Text = value.Value.Truncar(2, true).ToString(base.fmtValorFinanceiro); else txtValorTotal.Text = "";
            }
        }

        public decimal? ValorTotalMovimento
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtValorTotalMovimento.Text, true)); }
            set
            {
                if (value.HasValue) txtValorTotalMovimento.Text = value.Value.ToString(base.fmtValorFinanceiro);
                else txtValorTotalMovimento.Text = "";
            }
        }

        public string Observacoes
        {
            get { return txtObservacoes.Text; }
            set { txtObservacoes.Text = value; }
        }

        public string Instrucoes { get; set; }

        public string Empenho
        {
            get { return txtEmpenho.Text; }
            set { txtEmpenho.Text = value; }
        }

        public string hiddenMovimentoId
        {
            get { return hdfMovimentoId.Value; }
            set { hdfMovimentoId.Value = value; }
        }

        public string NlLiquidacao { get; set; }
        public string IdItem { get; set; }
        public int? SubItemMatItem { get; set; }

        public DateTime? DataVctoLoteItem
        {
            get
            {

                if (rblTipoMovimento.SelectedValue == Convert.ToString((int)GeralEnum.TipoMovimento.EntradaPorDevolucao) &&
                    ddlVencimentoLote.Items.Count > 1)
                    return TratamentoDados.TryParseDateTime(ddlVencimentoLote.SelectedItem.Text);
                else
                    return TratamentoDados.TryParseDateTime(txtVencimentoLote.Text);

            }
            set
            {
                if (value.HasValue)
                    txtVencimentoLote.Text = Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro);
                else
                    txtVencimentoLote.Text = "";
            }
        }

        public string DataVctoLoteItemTexto
        {
            get { return txtVencimentoLote.Text; }
            set
            {
                txtVencimentoLote.Text = Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro);

            }
        }

        public string IdentificacaoLoteItem
        {
            get { return txtIdentLoteItem.Text; }
            set { txtIdentLoteItem.Text = value; }

        }
        public string FabricLoteItem
        {
            get { return txtFabricLoteItem.Text; }
            set { txtFabricLoteItem.Text = value; }
        }

        public decimal QtdeItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtQtdeMov.Text, true)); }
            set
            {
                if (value != 0)
                    txtQtdeMov.Text = value.ToString(base.fmtFracionarioMaterialQtde);
                else
                    txtQtdeMov.Text = "0";
            }
        }

        public decimal QtdeLiqItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtQtdeLiq.Text, true)); }
            set
            {
                decimal _convNumero = _valorZero;
                if (!String.IsNullOrWhiteSpace(txtQtdeLiq.Text) && Decimal.TryParse(txtQtdeLiq.Text, out _convNumero)) txtQtdeLiq.Text = value.Truncar(3, true).ToString(base.fmtFracionarioMaterialQtde);
                else
                    txtQtdeLiq.Text = "0";
            }
        }

        public decimal SaldoQtdeItem { get; set; }

        public decimal PrecoUnitItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtPrecoUnit.Text, true)); }
            set
            {
                decimal _convNumero = _valorZero;
                //if (!String.IsNullOrWhiteSpace(txtPrecoUnit.Text) && Decimal.TryParse(txtPrecoUnit.Text, out _convNumero))
                if (value > 0.0000m)
                    txtPrecoUnit.Text = value.ToString(base.fmtFracionarioMaterialValorUnitario);
                else
                    txtPrecoUnit.Text = "0,0000";
            }
        }

        public decimal? SaldoValorItem { get; set; }

        public string AlmoxarifadoTransferencia
        {
            set { txtAlmoxarifadoTransf.Text = value.ToString(); }
        }

        public decimal ValorMovItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtValorMovItem.Text, true)); }
            set
            {
                decimal _convNumero = _valorZero;
                if (!String.IsNullOrWhiteSpace(txtValorMovItem.Text) && Decimal.TryParse(txtValorMovItem.Text, out _convNumero))
                    txtValorMovItem.Text = value.ToString();
                else
                    txtValorMovItem.Text = string.Empty;
            }
        }

        public int? NaturezaDespesaIdItem
        {
            set
            {
                ListItem item = ddlSubItemClassif.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlSubItemClassif.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlSubItemClassif.ClearSelection();
            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlSubItemClassif.SelectedValue)); }

        }

        public decimal? SaldoQtdeLoteItem { get; set; }
        public decimal DesdItem { get; set; }
        public bool? AtivoItem { get; set; }

        #endregion

        #region Mensagens de Erro

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

        #endregion

        #region Bloquear Controles Movimento

        public bool BloqueiaNovo { set { btnNovo.Enabled = !value; } }
        public bool ExibeBotaoNovoItem { set { btnNovo.Visible = value; btnNovo.CssClass = (value ? "mostrarControle" : "esconderControle"); } }
        public bool ExibeQtdeMovimentoItem
        {
            set
            {
                lblCampoInfoQtdeAEntrar.Visible = txtQtdeMov.Visible = value;
                lblCampoInfoQtdeAEntrar.CssClass = txtQtdeMov.CssClass = (value ? "mostrarControle" : "esconderControle");
            }
        }
        public bool ExibePrecoUnitarioMovimentoItem
        {
            set
            {
                lblCampoInfoPrecoUnitario.Visible = txtPrecoUnit.Visible = value;
                lblCampoInfoPrecoUnitario.CssClass = txtPrecoUnit.CssClass = (value ? "mostrarControle" : "esconderControle");
            }
        }
        public bool ExibirDocumentoAvulsoAnoMov { set { lblDocumentoAvulsoAnoMov.Visible = value; } }
        public bool BloqueiaGravar { set { btnGravar.Enabled = !value; } }
        public bool BloqueiaExcluir { set { btnExcluirItem.Enabled = !value; } }
        public bool BloqueiaBotaoCarregarEmpenho { set { btnListarEmpenhos.Enabled = !value; btnListarEmpenhos.Visible = !value; } }
        public bool BloqueiaCancelar { set { btnCancelar.Enabled = !value; } }
        public bool BloqueiaImprimir { set { btnImprimir.Enabled = !value; } }
        public bool ExibirImprimir { set { btnImprimir.Visible = value; } }
        public bool BloqueiaAjuda { set { btnAjuda.Enabled = !value; } }
        public bool BloqueiaCodigo { get; set; }
        public bool BloqueiaDescricao { set { txtDescricao.Enabled = value; } }
        public bool BloqueiaEmpenho
        {
            set
            {
                lblEmpenho.Visible = !value;
                txtEmpenho.Enabled = !value;
                txtEmpenho.Visible = !value;
            }
        }

        public bool BloqueiaNovoItem { set { throw new NotImplementedException(); } }
        public bool BloqueiaEstornar { set { btnEstornar.Enabled = !value; } }
        public bool HabilitaPesquisaFornecedor { set { imgLupaFornecedor.Visible = value; } }

        public bool HabilitarLote
        {
            set
            {
                if (value == true)
                    pnlLote.CssClass = "mostrarControle";
                else
                    pnlLote.CssClass = "esconderControle";
            }
        }

        public bool HabilitaPesquisaItemMaterial { set { } }//imgSubItemMaterial.Visible = value; } }

        public bool HabilitarBotoes
        {
            set
            {
                BloqueiaNovo = !perfilEditar;
                BloqueiaCancelar = !value;
                BloqueiaAjuda = !value;
            }
        }

        // usar controles baseado no perfil de consulta
        public bool HabilitarControles
        {
            set
            {
                txtDataEmissao.Enabled = true;
                txtEmpenho.Enabled = value;
                txtObservacoes.Enabled = value;
                BloqueiaListaUGE = !value;
                ExibirGeradorDescricao = value;
                BloqueiaEmpenho = !value;
                BloqueiaListaAlmoxarifado = true;
                //BloqueiaListaOrgao = true;
                // deixar sempre false
                MostrarPainelEdicao = false;
                HabilitaPesquisaFornecedor = false;
                ExibirGeradorDescricao = value;
                BloqueiaNovo = !perfilEditar; //BloqueiaNovo = false; //TODO ALTERADO
                BloqueiaDataRecebimento = false;
                ExibirListaEmpenho = false;
                ddlPTResAcao.Enabled = true;
                ddlPTResItemMaterial.Enabled = true;
                //btnNovo.Visible = (this.TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode());
            }
        }

        public void HabilitarControlesEdicao(bool Editar)
        {
            // conceder permissões

            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                BloqueiaNovo = true;
            else
                BloqueiaNovo = !perfilEditar; //TODO ALTERACAO

            txtValorTotal.Enabled = false;
            txtDataEmissao.Enabled = Editar;
            txtDataReceb.Enabled = Editar;
            txtObservacoes.Enabled = Editar;
            DataMovimento = DateTime.Now;
            BloqueiaCancelar = !Editar;
            BloqueiaItemQtdeLiq = true;
            ddlPTResAcao.Enabled = Editar;
            ddlPTResItemMaterial.Enabled = Editar;

            //btnNovo.Visible = true; //Editar; //TODO ALTERACAO
        }
        public bool MostrarPainelEdicao { set { pnlEditar.Visible = value; } }

        #endregion

        #region Compra Direta / BEC / Pregao
        public void HabilitarEmpenhoConsumoImediato(bool Editar)
        {
            lblFornecedor.Text = "Fornecedor:";
            HabilitarControles = true;
            HabilitarBotoes = false;
            ExibirGeradorDescricao = true;
            BloqueiaGeradorDescricao = true;
            // conceder permissões (de acordo com o perfil)
            BloqueiaNovo = !perfilEditar;
            txtEmpenho.Visible = false;
            ExibirListaEmpenho = true;
            lblValorTotal.Text = "Valor Doc.:";
            btnExcluirItem.Visible = false;
            btnNovo.Visible = false;
            //idDevolucao.Attributes["class"] = "esconderControle";
            idFornecedor.Attributes["class"] = "mostrarControle";
            idTransfer.Attributes["class"] = "esconderControle";
            BloqueiaBotaoCarregarEmpenho = false;
            divQtdLiq.Visible = true;
            idOrgaoTransf.Visible = false;
            idFornecedor.Visible = true;
            idEmpenho.Visible = true;
            imgLupaFornecedor.Visible = false;
            inscCE.Visible = true;
            txtInfoSiafemCE.Text = null;
            //divUge.Visible = true;
        }

        public void HabilitarEmpenhoConsumoImediatoNovo()
        {
            HabilitarControlesEdicao(true);
            BloqueiaItemQtdeLiq = true;
            HabilitaPesquisaItemMaterial = false;
            BloqueiaImprimir = true;
            BloqueiaGravarItem = true; // bloqueado por padrão para não permitir a inclusão de novo material no empenho.
            btnNovo.Visible = false;
        }

        public void HabilitarCompraDiretaEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
            BloqueiaNovo = !perfilEditar;
            btnNovo.Visible = false;
        }


        #endregion

        #region Aquisicao Restos a Pagar Consumo imediato

        public void HabilitarAquisicaoRestosAPagarConsumoImediatoNovo()
        {
            HabilitarControlesEdicao(true);
            HabilitaPesquisaFornecedor = true;
            BloqueiaGeradorDescricao = true;
            BloqueiaImprimir = true;
            BloqueiaGravarItem = false;
            btnNovo.Visible = true;
        }

        public void HabilitarAquisicaoRestosAPagarConsumoImediatoEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
        }

        public void HabilitarAquisicaoRestosAPagarConsumoImediato(bool Editar)
        {
            lblFornecedor.Text = "Fornecedor:";
            HabilitarControles = true;
            HabilitaPesquisaFornecedor = true;
            // perfil
            BloqueiaNovo = !perfilEditar; //BloqueiaNovo = false; //!perfilEditar; //TODO ALTERACAO
            txtEmpenho.Visible = false;
            ExibirListaEmpenho = true;
            lblValorTotal.Text = "Valor Doc.:";
            idFornecedor.Attributes["class"] = "mostrarControle";
            idTransfer.Attributes["class"] = "esconderControle";
            BloqueiaBotaoCarregarEmpenho = true;
            divQtdLiq.Visible = false;
            txtEmpenho.Enabled = false;
            idOrgaoTransf.Visible = false;
            idFornecedor.Visible = true;
            idEmpenho.Visible = true;
            imgLupaFornecedor.Visible = true;
            inscCE.Visible = true;
            txtInfoSiafemCE.Text = null;

        }

        public void HabilitarOrgaoTransfSemImpl()
        {
            throw new NotImplementedException("Funcionalidade não permitida para esta tela!");
        }



        public void criarNovoDocumento()
        {
            MovimentoPresenter mov = new MovimentoPresenter();

            //if (GetSession<MovimentoEntity>("movimento") != null)
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                //movimento = GetSession<MovimentoEntity>("movimento");
                movimento = GetSession<MovimentoEntity>(sessaoMov);
                movimento.Id = null;
            }

            movimento.AnoMesReferencia = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            this.AnoMesReferencia = movimento.AnoMesReferencia;

            movimento = mov.gerarNovoDocumento(movimento);
            txtDocumentoAvulso.Text = movimento.NumeroDocumento;
        }


        public void HabilitarAquisicaoAvulsaEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
        }

        #endregion



        #region Eventos

        protected void rblTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveSession(sessaoMov);
            VerificaMesFechadoSIAFEM();
            VerificaRestricaoFechamentoExercicioFiscal();
            DesabilitaNovaEntradaeEstorno__EntradasPorEmpenho();


            fldSetOperacaoEntrada.Disabled = false;
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();

            if (rblTipoMovimento.SelectedIndex >= 0)
            {
                lblFornecedor.Visible = true;
                pnlSideRight.Enabled = true;
                lblValorTotal.Text = "Valor Total:";
                hdfInfoSiafemCEOld.Text = string.Empty;
                ddlEmpenho.SelectedValue = "0";
                mat.LimparMovimento();
                mat.HabilitarControles(Convert.ToInt32(rblTipoMovimento.SelectedValue), perfilEditar);
                PesquisaDocumento.TipoMovimento = rblTipoMovimento.SelectedValue.ToString();

                int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;


                ddlUGE.SelectedValue = ugeLogado.ToString();

                switch (Convert.ToInt16(rblTipoOperacao.SelectedValue))
                {

                    case (int)GeralEnum.OperacaoEntrada.Nova:
                        optNovo();
                        painelSideRight(true);
                        break;
                    case (int)GeralEnum.OperacaoEntrada.Estorno:
                        optEstornar();
                        painelSideRight(false);
                        break;
                    case (int)GeralEnum.OperacaoEntrada.NotaRecebimento:
                        optNota();
                        painelSideRight(false);
                        break;
                    default:
                        painelSideRight(false);
                        break;
                }
                switch (TipoMovimento)
                {
                    case (int)tipoMovimentacao.ConsumoImediatoEmpenho:
                    case (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                        //PopularComboEmpenhos();
                        PopularComboEmpenhosConsumoImediato();
                        break;
                    default:
                        break;
                }
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NumeroEmpenhoCombo))
            {
                ExibirMensagem("Obrigatório selecionar empenho, antes de inserir subitens na movimentação!");
                return;
            }

            txtSubItem.Enabled = false;
            rblLote.Enabled = false;
            //imgSubItemMaterial.Visible = false;
            imgSubItemMaterial.Visible = true;
            //imgLupaFornecedor.Visible = false;
            //}


            AtivarBotaoFornecedor();

            if (string.IsNullOrEmpty(txtGeradorDescricao.Text) || string.IsNullOrEmpty(txtDataEmissao.Text)
               || string.IsNullOrEmpty(txtDataReceb.Text))
            {
                string alert1 = string.IsNullOrEmpty(txtGeradorDescricao.Text) ? "Fornecedor // " : "";
                string alert6 = (ddlUGE.SelectedValue) == "0" ? "UGE//" : "";
                string alert2 = string.IsNullOrEmpty(txtDataEmissao.Text) ? "Data Emissão// " : "";
                string alert3 = string.IsNullOrEmpty(txtDataReceb.Text) ? "Data Recebimento// " : "";
                string alert4 = (ddlUA.SelectedValue)=="0" ? "UA//" : "";
               // string alert5 = string.IsNullOrEmpty(txtDocumentoAvulso.Text) ? "Documento// " : "";
               
                this.ListaErros = new List<string>() { "Preencher o(s) campo(s) : " + alert6+ alert4+ alert1 +alert2 + alert3 };
                return;
            }

            BloqueiaTipoMovimento = true;
            //BloqueiaListaUGE = true;
            BloqueiaListaUGE = false; //Usuario pode escolher a UGE de Destino para este tipo de entrada
            MostrarPainelEdicao = true;
            imgLupaRequisicao.Visible = false;
            btnCarregarSubItem.Visible = false;


            ExibeQtdeMovimentoItem = ExibePrecoUnitarioMovimentoItem = !(TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode());
            AtualizaInscricaoCE();


            var objPresenter = new ConsumoImediatoPresenter(this);
            objPresenter.LimparItem();
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((rblTipoMovimento.SelectedIndex == -1))
                return;


            //PopularComboEmpenhosConsumoImediato();
            PopularListaUAs();

            AtualizaInscricaoCE();

            PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial);
        }

        protected void txtQtdeLiqGridRepeater_OnTextChanged(object sender, EventArgs e)
        {
            btnGravar.Enabled = true;
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            TextBox txtQtdeLiqGrid = (TextBox)rp1.FindControl("txtQtdeLiqGrid");
            TextBox txtQtdeMovGrid = (TextBox)rp1.FindControl("txtQtdeMovGrid");
            TextBox txtSaldoLiq = (TextBox)rp1.FindControl("txtSaldoLiq");
            Label lblPrecoUnitGridCalc = (Label)rp1.FindControl("lblPrecoUnitGridCalc");
            Label lblPrecoUnitGrid = (Label)rp1.FindControl("lblPrecoUnitGrid");
            //Label lblSaldoLiq = (Label)rp1.FindControl("lblSaldoLiq");


            txtQtdeLiqGrid.Text = string.IsNullOrWhiteSpace(txtQtdeLiqGrid.Text) ? "0,000" : txtQtdeLiqGrid.Text;
            txtQtdeMovGrid.Text = string.IsNullOrWhiteSpace(txtQtdeMovGrid.Text) ? "0,000" : txtQtdeMovGrid.Text;

            decimal QtdeLiqGrid = Decimal.Parse(txtQtdeLiqGrid.Text);
            decimal QtdeMovGrid = Decimal.Parse(txtQtdeMovGrid.Text);
            decimal PrecoUnitGrid = Decimal.Parse(lblPrecoUnitGrid.Text);



            decimal calculo = (QtdeLiqGrid > 0 ? (QtdeLiqGrid * PrecoUnitGrid) / QtdeLiqGrid : 0);

            lblPrecoUnitGridCalc.Text = calculo.truncarQuatroCasas().ToString();
            lblPrecoUnitGrid.Visible = false;
            lblPrecoUnitGridCalc.Visible = true;

            //decimal calculoSaldoLiq = (QtdeLiqGrid * PrecoUnitGrid);
            decimal valorMinimoItemEmpenho = PrecoUnitGrid;


            //lblSaldoLiq.Text = calculoSaldoLiq.truncarDuasCasas().ToString();
            if (TipoMovimento == tipoMovimentacao.ConsumoImediatoEmpenho.GetHashCode())
            {
                //lblSaldoLiq.Style.Add("display", "none");
                //txtSaldoLiq.Text = calculoSaldoLiq.ToString();
                txtSaldoLiq.Text = "1.00";
            }

            //if (txtQtdeLiqGrid.Text == "0,000")
            txtQtdeMovGrid.Text = txtQtdeLiqGrid.Text;

            //if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
            if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeLiqGrid.Text == "0,000"))
            {
                string strValorMinimo = valorMinimoItemEmpenho.ToString();
                btnGravar.Enabled = false;
                //this.ListaErros = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo de R$ {0} não são permitidas. Verifique qtde/preço.", strValorMinimo) };
                this.ListaErros = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo do valor do empenho não serão permitidas. Favor verificar.", strValorMinimo) };
                return;
            }

            double valorMin = 0.01;

            //if ((calculoSaldoLiq < Convert.ToDecimal(valorMin)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
            if (TipoMovimento == tipoMovimentacao.ConsumoImediatoEmpenho.GetHashCode())
            {
                //if ((Convert.ToDecimal(lblSaldoLiq.Text) < Convert.ToDecimal(valorMin)) && !(txtQtdeLiqGrid.Text == "0,000"))
                if ((Convert.ToDecimal(txtSaldoLiq.Text) < Convert.ToDecimal(valorMin)) && !(txtSaldoLiq.Text == "0,000"))
                {
                    btnGravar.Enabled = false;
                    this.ListaErros = new List<string>() { String.Format("O valor não pode ser menor que 0,01") };
                    return;
                }
            }

        }

        public void ddlSubItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gestorId = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            DropDownList ddlSubItemList = (DropDownList)sender;
            RepeaterItem ri = (RepeaterItem)ddlSubItemList.Parent;

            TextBox txtQtdeMovGrid = ri.FindControl("txtQtdeMovGrid") as TextBox;
            TextBox txtQtdeLiqGrid = ri.FindControl("txtQtdeLiqGrid") as TextBox;
            TextBox txtValorTotal = ri.FindControl("txtValorTotal") as TextBox;
            Label lblUnidFornecimentoGrid = ri.FindControl("lblUnidFornecimentoGrid") as Label;
            Label lblUnidFornecimentoGridSiafisico = ri.FindControl("lblUnidFornecimentoGridSiafisico") as Label;
            Label lblDescricaoGrid = ri.FindControl("lblDescricaoGrid") as Label;
            Label lblSubItemMaterialId = ri.FindControl("lblSubItemMaterialId") as Label;
            Label lblPrecoUnitGrid = ri.FindControl("lblPrecoUnitGrid") as Label;
            Label lblPosicao = ri.FindControl("lblPosicao") as Label;





            // trazer os dados do subitem (para EMPENHO)
            if (ddlSubItemList.SelectedIndex > 0)
            {
                ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
                SubItemMaterialEntity subItem = mat.SelectSubItemMaterialRetorno(Convert.ToInt32(ddlSubItemList.SelectedValue));
                MovimentoItemEntity MovimentoItem = new MovimentoItemEntity();
                UnidadeFornecimentoConversaoEntity unidadeConversao = null;

                if (subItem != null)
                {

                    unidadeConversao = (new UnidadeFornecimentoConversaoPresenter()).ObterDadosUnidadeFornecimentoConversao(lblUnidFornecimentoGridSiafisico.Text, subItem.Id.Value);

                    // linha substitui a coluna (do lote)
                    //System.Web.UI.HtmlControls.HtmlTableRow tbl = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("tblLote");


                    GetSessao();
                    MovimentoItem = movimento.MovimentoItem.Where(MovItem => MovItem.SubItemMaterial.Id.Value == Int32.Parse(lblSubItemMaterialId.Text) && MovItem.Posicao == Convert.ToInt32(lblPosicao.Text)).FirstOrDefault();
                    if (movimento.MovimentoItem.IsNotNull() && MovimentoItem.IsNotNull())
                    {
                        lblUnidFornecimentoGrid.Text = subItem.DescricaoUnidadeFornecimento;
                        lblDescricaoGrid.Text = subItem.Descricao;
                        lblSubItemMaterialId.Text = subItem.Id.Value.ToString();
                        ItemMaterialCodigo = subItem.ItemMaterial.Codigo;
                        ItemMaterialDescricao = subItem.ItemMaterial.Descricao;
                        SubItemMaterialCodigo = subItem.Codigo;
                        SubItemMaterialDescricao = subItem.Descricao;
                        NaturezaDespesaIdItem = subItem.NaturezaDespesa.Id;
                        UnidadeCodigo = subItem.UnidadeFornecimento.Codigo + "-" + subItem.UnidadeFornecimento.Descricao;
                        MovimentoItem.SubItemMaterial = subItem;
                        movimento.MovimentoItem[MovimentoItem.Posicao - 1] = MovimentoItem;

                        if (unidadeConversao.IsNotNull() && (movimento.EmpenhoEvento.Descricao != GeralEnum.GetEnumDescription(TipoEmpenho.BEC)))
                        {
                            var qtdeConvertida = (Decimal.Parse(txtQtdeLiqGrid.Text) * Decimal.Parse(unidadeConversao.FatorUnitario.ToString()));
                            var precoUnitarioConvertido = (movimento.ValorDocumento.Value.Truncar(base.numCasasDecimaisValorUnitario, true) / qtdeConvertida).Truncar(base.numCasasDecimaisValorUnitario, true);

                            lblDescricaoGrid.Text = String.Format("{0} <b><font color='red'>(Conversção automática efetuada para este Item)</font></b>.", lblDescricaoGrid.Text);

                            lblPrecoUnitGrid.Text = precoUnitarioConvertido.ToString(base.fmtFracionarioMaterialValorUnitario);
                            txtQtdeMovGrid.Text = qtdeConvertida.ToString(base.fmtFracionarioMaterialQtde);
                        }

                    }
                }

                unidadeConversao = null;
                txtQtdeMovGrid.Focus();
            }
        }

        protected void imgLupaFornecedor_Click(object sender, ImageClickEventArgs e)
        {
            SetSession(txtCodFornecedor, "fornecedorId");
            SetSession(txtGeradorDescricao, "fornecedorDados");
            txtInfoSiafemCE.Text = string.Empty;

        }

        protected void txtQtdeMovGrid_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQtdeMovGrid = (TextBox)sender;

            Label lblPrecoUnitGrid = txtQtdeMovGrid.Parent.FindControl("lblPrecoUnitGrid") as Label;
            Label lblValorMovItemGrid = txtQtdeMovGrid.Parent.FindControl("lblValorMovItemGrid") as Label;

            lblPrecoUnitGrid.ReformatarValorNumerico(base.fmtFracionarioMaterialValorUnitario);
            lblValorMovItemGrid.Text = (Decimal.Parse(lblPrecoUnitGrid.Text) * Decimal.Parse(txtQtdeMovGrid.Text)).ToString();
        }

        protected void txtItemMaterial_TextChanged(object sender, EventArgs e)
        {
            if (ddlUGE.SelectedIndex <= 0)
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('Informar a UGE');", true);
                return;
            }

            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            if (txtItemMaterial.Text != "")
            {
                mat.ProcurarItemMaterialId(Convert.ToInt32(txtItemMaterial.Text));
            }
        }

        private void CarregaGridMovimentoItem(MovimentoEntity movimento)
        {
            GetSessao();

            bool atualizar = true;

            if (movimento == null)
                atualizar = false;
            else
            {
                if (movimento.MovimentoItem == null)
                    atualizar = false;
            }

            if (atualizar)
            {
                gridSubItemMaterial.DataSource = movimento.MovimentoItem.OrderBy(a => a.Id).ToList();
                gridSubItemMaterial.DataBind();

                if (movimento.MovimentoItem.Count > 0)
                {
                    if (!string.IsNullOrEmpty(movimento.MovimentoItem.FirstOrDefault().DataVencimentoLote.ToString()))
                    {
                        rblLote.SelectedValue = "1";
                        DivLote.Visible = true;
                    }
                    else
                    {
                        rblLote.SelectedValue = "0";
                        DivLote.Visible = false;
                    }
                }
            }

            if ((movimento != null) && (movimento.MovimentoItem != null && movimento.MovimentoItem.Count > 0))
                PageBase.SetSession<IList<MovimentoItemEntity>>(movimento.MovimentoItem, subItensMovimento);
        }

        private void ReformatarValoresExibidosControlesGrid()
        {
            throw new NotImplementedException("Funcionalidade não permitida para esta tela!");
        }

        protected void btnGravarItem_Click(object sender, EventArgs e)
        {
            this.ListaErros = new List<string>();

            if (TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode())
            {
                QtdeItem = 1.000m;
                this.PrecoUnitItem = ValorMovItem;
            }


            if (PrecoUnitarioZeroEMilesimal())
            {
                //string strValorMinimo = Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString();
                string strValorMinimo = obterValorUnitarioMinimo();
                this.ListaErros = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo de R$ {0} não são permitidas. Verifique qtde/preço.", strValorMinimo) };
                return;
            }


            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);

            GetSessao();

            string botao = ViewState["botao"] == null ? "" : ViewState["botao"].ToString();
            //movimento = mat.GravarItem(movimento, Convert.ToInt32(rblLote.SelectedValue), botao);
            movimento = mat.GravarItem(movimento, 0, botao);

            if (movimento == null)
                return;

            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);

            mat.LimparItem();
            CarregaGridMovimentoItem(movimento);

            if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            {
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
            }
            else
            {
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.ValorMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.ValorMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
            }

            pnlGrid.Enabled = true;
            BloqueiaGravar = false;
            MostrarPainelEdicao = false;
            gridSubItemMaterial.Visible = true;

            AtivarBotaoFornecedor();
        }

        protected bool PrecoUnitarioZeroEMilesimal()
        {
            //return (this.PrecoUnitItem < 0.0001m);
            return (this.PrecoUnitItem < Decimal.Parse(Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString()));
        }
        protected bool PrecoUnitarioZeroEMilesimal(decimal valor)
        {
            return (valor < Decimal.Parse(Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString()));
        }


        protected void btnCancelarItem_Click(object sender, EventArgs e)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            mat.CancelarItem();
            this.MostrarPainelEdicao = false;
            pnlGrid.Enabled = true;
            BloqueiaTipoMovimento = false;
            CarregaGridMovimentoItem(movimento);
            AtivarBotaoFornecedor();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            mat.Cancelar();
            pnlGrid.Enabled = true;
            txtDataEmissao.Enabled = true;
            ddlPTResAcao.Enabled = true;
            ddlPTResItemMaterial.Enabled = true;
            if (ddlEmpenho.Items.Count > 0)
                ddlEmpenho.SelectedIndex = 0;
            hdfMovimentoId.Value = "";
            BloqueiaTipoMovimento = false;

            if ((TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho) || (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho))
            {
                ValorTotalMovimento = null;
                ExibirListaEmpenho = true;
                txtEmpenho.Visible = false;

                ddlEmpenho.Enabled = true;
            }

            mat.LimparCamposMovimento(false);
            CarregaGridMovimentoItem(movimento);
            AtivarBotaoFornecedor();
        }

        private bool ValidarLoteVencimentoEmpenho()
        {
            bool retorno = false;
            string SubItemDataInvalida = "";
            string SubItemDataInvalidaNula = "";
            string SubItens = "";
            string SubItensT = "";
            string SubItensValor = "";
            string SubItensV = "";
            string Itens = "";
            string ItensSIAFEM = "";
            int contMovItem = 0;
            string SubItensBEC = "";

            List<GridItem> listGridItem = new List<GridItem>();
            GridItem gridItem;

            foreach (RepeaterItem ri in gridSubItemMaterial.Items)
            {
                // instanciar cada item
                DropDownList drpSubItem = ri.FindControl("ddlSubItemList") as DropDownList;

                Label lblPrecoUnitGridCalc = ri.FindControl("lblPrecoUnitGridCalc") as Label;
                TextBox txtQtdeLiqGrid = ri.FindControl("txtQtdeLiqGrid") as TextBox;
                HiddenField hdfQtdeLiqGrid = ri.FindControl("hdfQtdeLiqGrid") as HiddenField;
                HiddenField hdfValorLiqGrid = ri.FindControl("hdfValorLiqGrid") as HiddenField;
                TextBox txtQtdeMovGrid = ri.FindControl("txtQtdeMovGrid") as TextBox;
                TextBox txtValorItemEmpenho = ri.FindControl("txtValorItemEmpenho") as TextBox;
                CheckBox chkLiquidarEmpenhoBEC = ri.FindControl("chkLiquidarEmpenhoBEC") as CheckBox;
                Label lblItemMatGrid = ri.FindControl("lblItemMatGrid") as Label;
                Label lblUnidFornecimentoGridSiafisico = ri.FindControl("lblUnidFornecimentoGridSiafisico") as Label;


                txtQtdeLiqGrid.Text = string.IsNullOrWhiteSpace(txtQtdeLiqGrid.Text) ? "0,000" : txtQtdeLiqGrid.Text;

                string QtdeLiqGridAnterior = hdfValorLiqGrid.Value.Replace(".", "");
                string QtdeLiqGridAtual = txtValorItemEmpenho.Text.Replace(".", "");

                gridItem = new GridItem();
                gridItem.Item = Convert.ToInt32(lblItemMatGrid.Text);
                gridItem.UniFornS = lblUnidFornecimentoGridSiafisico.Text.Trim();
                listGridItem.Add(gridItem);

                decimal QtdeLiqGrid = Decimal.Parse(txtQtdeLiqGrid.Text);
                decimal PrecoUnitGrid = Decimal.Parse(lblPrecoUnitGridCalc.Text);
                double valorMin = 0.01;
                decimal calculoSaldoLiq = (QtdeLiqGrid * PrecoUnitGrid).truncarDuasCasas();


                if ((GeralEnum.GetEnumDescription(TipoEmpenho.BEC) == ViewState["tipoEmpenho"].ToString()))
                {
                    if (chkLiquidarEmpenhoBEC.Checked)
                    {
                        if (drpSubItem.SelectedItem.Text == "- Selecione -")
                        {
                            Itens += lblItemMatGrid.Text + ",";
                        }
                        if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
                        {
                            SubItens += drpSubItem.SelectedItem.Text + ",";
                        }

                        if (calculoSaldoLiq < Convert.ToDecimal(valorMin))
                        {
                            SubItensValor += drpSubItem.SelectedItem.Text + ",";

                        }


                    }
                    else
                    {
                        if (!(txtQtdeMovGrid.Text == "0,000" || string.IsNullOrEmpty(txtQtdeMovGrid.Text)))
                        {
                            if (Decimal.Parse(txtQtdeMovGrid.Text) != 0)
                            {
                                SubItensBEC += drpSubItem.SelectedItem.Text + ",";
                            }


                        }
                    }

                }
                else
                {
                    if ((Decimal.Parse(QtdeLiqGridAtual) != 0))
                    {

                        if (QtdeLiqGridAnterior == QtdeLiqGridAtual)
                        {
                            if (!Convert.ToBoolean(ViewState["ValidaLiqTotal"]))
                            {
                                SubItensT += drpSubItem.SelectedItem.Text + ",";

                            }
                        }
                        else
                        {

                            if (Decimal.Parse(QtdeLiqGridAtual) > Decimal.Parse(QtdeLiqGridAnterior))
                            {
                                SubItensV += drpSubItem.SelectedItem.Text + ",";
                            }


                        }


                        if (drpSubItem.SelectedItem.Text == "- Selecione -")
                        {
                            Itens += lblItemMatGrid.Text + ",";

                        }
                    }


                    if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
                    {
                        SubItens += drpSubItem.SelectedItem.Text + ",";


                    }

                    if ((calculoSaldoLiq < Convert.ToDecimal(valorMin)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
                    {
                        SubItensValor += drpSubItem.SelectedItem.Text + ",";

                    }

                }





                if (!(txtQtdeMovGrid.Text == "0,000" || string.IsNullOrEmpty(txtQtdeMovGrid.Text)))
                    if (Decimal.Parse(txtQtdeMovGrid.Text) != 0)
                    {
                        contMovItem++;
                    }
            }

            var itemS = listGridItem.GroupBy(n => new { n.Item, n.UniFornS },
                (key, values) => new { ItemS = key, Count = values.Count() });

            foreach (var item in itemS)
            {
                if (item.Count > 1)
                {
                    ItensSIAFEM += item.ItemS.Item + " - " + item.ItemS.UniFornS + ",";
                }
            }


            List<string> listaErro = new List<string>();
            this.ListaErros = new List<string>();

            if (contMovItem == 0)
            {
                listaErro.Add("Não há movimento a ser enviado!!!");
            }

            if (!string.IsNullOrEmpty(Itens))
            {
                listaErro.Add(String.Format("Selecionar o subItem do(s) item(s) {0}", Itens));
            }

            if (!string.IsNullOrEmpty(SubItensBEC))
            {
                listaErro.Add(String.Format("A entrada do(s) subItem(ns) {0} - BEC deve(m) ser total", SubItensBEC));
            }


            if (!string.IsNullOrEmpty(ItensSIAFEM))
            {
                listaErro.Add(String.Format("Item(s) {0} duplicado no SIAFEM, por favor verificar", ItensSIAFEM));
            }

            if (!string.IsNullOrEmpty(SubItensV))
            {
                listaErro.Add(String.Format("O valor liquidar do(s) SubItem(ns) {0} está(ão) acima do valor disponível.", SubItensV));
            }

            if (!string.IsNullOrEmpty(SubItensT))
            {
                ViewState["ValidaLiqTotal"] = true;
                ExibirMensagem(String.Format("O(s) SubItem(ns) {0} será(ão) liquidado(s) o valor TOTAL.", SubItensT));
                retorno = true;
            }

            if (!string.IsNullOrEmpty(SubItens))
            {
                string strValorMinimo = obterValorUnitarioMinimo();
                listaErro.Add(String.Format("Entrada(s) de subitem(ns) {0} com preço unitário abaixo de R$ {1} não é(são) permitida(s). Verifique qtde/preço.", SubItens, strValorMinimo));
            }

            if (!string.IsNullOrEmpty(SubItensValor))
            {
                listaErro.Add(String.Format("O valor não pode ser menor que 0,01"));
            }


            if (!string.IsNullOrEmpty(SubItemDataInvalida))
                listaErro.Add("Data(s) Vencimento(s) inválida(s) do(s) Subitem(ns) " + SubItemDataInvalida + " ela(s) deve(m) ser maior que data atual.");

            if (!string.IsNullOrEmpty(SubItemDataInvalidaNula))
                listaErro.Add("Data(s) de Vencimento(s) do(s) Lote(s) " + SubItemDataInvalidaNula + " está(ão) vazia(s).");


            if (ddlEmpenho.SelectedValue == "0")
                listaErro.Add("Selecione o empenho.");           
            if (listaErro.Count > 0)
            {
                this.ListaErros = listaErro;
                retorno = true;
            }

            return retorno;
        }
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            var almoxLogado = (new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado);

            if (inscCE.Visible)
            {

                if (string.IsNullOrEmpty(txtInfoSiafemCE.Text))
                {
                    ExibirMensagem("Preencher Inscrição (CE)");
                    return;
                }
                else
                {
                    txtInfoSiafemCE.Text = txtInfoSiafemCE.Text.Replace(" ", "");
                    if (txtInfoSiafemCE.Text == "999")
                        txtInfoSiafemCE.Text = "CE999";
                }
            }


            if (dadosAcessoSiafemPreenchidos)
                efetuarAcessoSIAFEM();

            var _tempCodigoUGE = this.CodigoUGE;
            bool erro = false;

            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            {
                if (ValidarLoteVencimentoEmpenho())
                    return;

                if (!GravarMovimentoEmpenho())
                    return;
            }

            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                // salva o almoxarifado do perfil
                mov.Almoxarifado = almoxLogado;
                mov.AnoMesReferencia = almoxLogado.MesRef;
                mov.UGE = almoxLogado.Uge;
                mov.UA = new UAEntity() { Id = this.UaId, Codigo = this.CodigoUA };

                vincularPTRes_MovimentoItem(mov);

                this.AnoMesReferencia = mov.AnoMesReferencia;

                #region Verifica Evento Pagamento (CE)
                mov.InscricaoCE = this.InscricaoCE;
                #endregion

                switch (TipoMovimento)
                {
                    //case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar:
                    case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                        mov.Empenho = this.NumeroEmpenhoCombo;
                        mov.ValorDocumento = this.ValorDocumento;
                        mov.UA = (mov.UA ?? (new UAEntity() { Id = this.UaId, Codigo = CodigoUA }));

                        if (!ValidarValorMovimentacaoEntradaRestosAPagarConsumoImediato(mov))
                            return;

                        break;
                }

                MovimentoEntity movStatus = mat.Gravar(this.loginSiafem, this.senhaSiafem, mov);
                if (movStatus != null)
                {
                    RemoveSession(sessaoMov);
                    RemoveSession(subItensMovimento);
                }
                else
                {
                    mov = GetSession<MovimentoEntity>(sessaoMov);
                    erro = true;
                }
            }

            CarregaGridMovimentoItem(null);

            if (!erro)
            {
                hdfInfoSiafemCEOld.Text = string.Empty;
                this.LimparDadosDocumento(false);
            }

            AtivarBotaoFornecedor();
        }

        private void vincularPTRes_MovimentoItem(MovimentoEntity mov)
        {
            var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
            if (listaPTRes.HasElements())
            {
                var dadosPTRes = listaPTRes.Where(ptres => ptres.Codigo == this.PTResCodigo).FirstOrDefault();
                if (dadosPTRes.IsNotNull())
                    mov.MovimentoItem.ToList().ForEach(itemMovimentacao => itemMovimentacao.PTRes = dadosPTRes);
            }
        }

        private bool ValidarValorMovimentacaoEntradaRestosAPagarConsumoImediato(MovimentoEntity movimentacaoMaterial)
        {
            bool blnValorTotalEmpenhoOK = false;
            bool blnCodigoPTResMovimentacaoMaterialOK = false;
            decimal valorTotalEmpenhoSelecionado = 0.00m;
            string msgErro = null;


            Decimal.TryParse(ddlEmpenho.SelectedItem.Value.BreakLine(";", 1), out valorTotalEmpenhoSelecionado);
            if (movimentacaoMaterial.IsNotNull())
                blnValorTotalEmpenhoOK = (movimentacaoMaterial.ValorDocumento <= valorTotalEmpenhoSelecionado);

            if (!blnValorTotalEmpenhoOK)
            {
                msgErro = String.Format(@"Valor de movimentação para ""Restos a Pagar"" (R$ {2}), superior a valor disponível (R$ {1}) para empenho {0} no SIAFEM!", movimentacaoMaterial.Empenho, valorTotalEmpenhoSelecionado.ToString(base.fmtValorFinanceiro), movimentacaoMaterial.ValorDocumento.Value.ToString(base.fmtValorFinanceiro));
                ExibirMensagem(msgErro);
            }

            blnCodigoPTResMovimentacaoMaterialOK = movimentacaoMaterial.MovimentoItem.HasElements() && !String.IsNullOrWhiteSpace(movimentacaoMaterial.MovimentoItem[0].PTResCodigo);
            if (!blnCodigoPTResMovimentacaoMaterialOK)
            {
                msgErro = "Obrigatório informar PTRes para registrar movimentação de materiais!";
                ExibirMensagem(msgErro);
            }
  
            return (blnValorTotalEmpenhoOK && blnCodigoPTResMovimentacaoMaterialOK);
        }

        protected void btnHidQtdeMov_Click(object sender, EventArgs e)
        {
            SetFocus(txtValorMovItem);
        }

        protected void lnkCancelarQtdeMovEmpenho_Click(object sender, EventArgs e)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            movimento = mat.CancelarItemEmpenho(movimento);
            if (movimento == null)
                return;
            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);
            mat.LimparItem();
            gridSubItemMaterial.DataSource = movimento.MovimentoItem;
            gridSubItemMaterial.DataBind();

            if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            {
                this.MostrarPainelEdicao = false;
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral(), true);
            }
            else
            {
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
            }
            BloqueiaGravar = false;
        }

        protected void rblTipoOperacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlEmpenho.SelectedValue = "0";
            //HabilitarOrgaoTransfSemImpl();
            RemoveSession(sessaoMov);

            // nova entrada
            if (TipoOperacao == (int)GeralEnum.OperacaoEntrada.Nova)
                optNovo();
            if (TipoOperacao == (int)GeralEnum.OperacaoEntrada.Estorno)
                optEstornar();
            if (TipoOperacao == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                optNota();

            LimparDadosDocumento();
            MostrarPainelEdicao = false;
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
            this.Id = string.Empty;
        }

        protected void imgLupaRequisicao_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void btnEstornar_Click(object sender, EventArgs e)
        {
            string complementoMsg = null;

            if (inscCE.Visible)
            {

                if (string.IsNullOrEmpty(txtInfoSiafemCE.Text))
                {

                    if ((TipoMovimento == (int)tipoMovimentacao.ConsumoImediatoEmpenho) || (TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho))
                        complementoMsg = "Reclassificação";

                    ExibirMensagem(String.Format("Preencher Inscrição (CE) {0}", complementoMsg));
                    return;
                }
                else
                {
                    txtInfoSiafemCE.Text = txtInfoSiafemCE.Text.Replace(" ", "");
                    if (txtInfoSiafemCE.Text == "999")
                        txtInfoSiafemCE.Text = "CE999";
                }
            }

            if (dadosAcessoSiafemPreenchidos)
                efetuarAcessoSIAFEM();

            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                mov.Id = Common.Util.TratamentoDados.TryParseInt32(this.Id);
                //mov = mat.Estornar(mov);
                mov = mat.Estornar(this.loginSiafem, this.senhaSiafem, mov);
                if (mov != null)
                {
                    hdfInfoSiafemCEOld.Text = string.Empty;
                    RemoveSession(sessaoMov);
                    mat.HabilitarControles(Convert.ToInt32(rblTipoMovimento.SelectedValue), perfilEditar);
                    optEstornar();


                }
            }

            CarregaGridMovimentoItem(movimento);
        }

        protected void btnExcluirItem_Click(object sender, EventArgs e)
        {
            ConsumoImediatoPresenter objPresenter = new ConsumoImediatoPresenter(this);

            this.ValorDocumento = 0;
            movimento = objPresenter.ExcluirItem(movimento);

            if (movimento == null)
                return;

            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);

            objPresenter.LimparItem();
            MostrarPainelEdicao = false;

            // total do movimento
            if (movimento.MovimentoItem != null && movimento.MovimentoItem.Count > 0)
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov).Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);


            CarregaGridMovimentoItem(movimento);
            AtivarBotaoFornecedor();
        }

        #endregion

        #region Imprimir

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            mat.Imprimir();
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                paramList.Add("MovimentoId", hdfMovimentoId.Value);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region Habilitar Perfis
        public bool PerfilAlterar
        {
            set
            {
                // disponíveis
                rblTipoMovimento.Enabled = value;
                ddlUGE.Enabled = value;
                txtEmpenho.Enabled = value;
                txtGeradorDescricao.Enabled = value;
                txtDataEmissao.Enabled = value;
                txtDataReceb.Enabled = value;
                txtObservacoes.Enabled = value;
                txtItemMaterial.Enabled = value;

                txtQtdeMov.Enabled = value;
                txtPrecoUnit.Enabled = value;
                txtVencimentoLote.Enabled = value;
                txtFabricLoteItem.Enabled = value;
                txtIdentLoteItem.Enabled = value;
                // indisponíveis
                //ddlUnidade.Enabled = false;
                txtDescricao.Enabled = false;
                txtQtdeLiq.Enabled = false;
                ddlSubItemClassif.Enabled = false;
                ddlPTResAcao.Enabled = value;
                ddlPTResItemMaterial.Enabled = value;
            }
        }

        public bool PerfilConsultar
        {
            set
            {
                // disponíveis
                rblTipoMovimento.Enabled = value;
                ddlUGE.Enabled = value;
                txtEmpenho.Enabled = value;
                txtGeradorDescricao.Enabled = value;
                // indisponíveis
                txtDataEmissao.Enabled = false;
                txtDataReceb.Enabled = false;
                txtObservacoes.Enabled = false;
                txtItemMaterial.Enabled = false;

                txtQtdeMov.Enabled = false;
                txtPrecoUnit.Enabled = false;
                txtVencimentoLote.Enabled = false;
                txtFabricLoteItem.Enabled = false;
                txtIdentLoteItem.Enabled = false;
                //ddlUnidade.Enabled = false;
                txtDescricao.Enabled = false;
                txtQtdeLiq.Enabled = false;
                ddlSubItemClassif.Enabled = false;
                ddlPTResAcao.Enabled = false;
                ddlPTResItemMaterial.Enabled = false;
            }
        }
        #endregion

        #region Metodos

        private MovimentoEntity AdicionaNumeracaoMovimentoItem(MovimentoEntity movimento)
        {
            if (movimento != null)
            {
                if (movimento.MovimentoItem != null)
                {
                    for (int i = 1; i <= movimento.MovimentoItem.Count(); i++)
                    {
                        movimento.MovimentoItem[i - 1].Posicao = i;
                    }
                }
            }

            return movimento;
        }

        protected void CarregaGridSubItemMaterial(int? movimentoId)
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);

            if (movimentoId != null)
            {
                //Busca o movimento completo
                movimento = mat.GetMovimentoGrid(movimentoId);
                movimento = AdicionaNumeracaoMovimentoItem(movimento);
                //movimento = CalcularPrecoMedio(movimento);

                if (movimento != null)
                {
                    //Atualiza a sessão com o movimento
                    SetSession(movimento, sessaoMov);

                    BloqueiaCancelar = false;
                    //BloqueiaListaUGE = true;
                    BloqueiaListaUGE = false; //Usuario pode escolher a UGE de Destino para este tipo de entrada

                    ViewState["tipoEmpenho"] = movimento.EmpenhoEvento != null ? movimento.EmpenhoEvento.Descricao : null;

                    if (!String.IsNullOrEmpty(movimento.Empenho))
                        NumeroEmpenhoCombo = movimento.Empenho;

                    if (perfilEditar)
                    {
                        rblTipoOperacao.Items[1].Selected = true;
                        BloqueiaNovo = !perfilEditar;
                    }

                    if (movimento.MovimentoItem.Count > 0)
                    {
                        this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);

                        if (ddlPTResAcao.Items.Count == 1 && ddlPTResItemMaterial.Items.Count == 1)
                            PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial);

                        mat.CarregarMovimentoTela(movimento);


                        lblDocumentoAvulsoAnoMov.Text = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + "/";

                        BloqueiaGravar = false;
                        BloqueiaEstornar = false;
                        BloqueiaImprimir = false;
                    }

                    //if (movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                    //if (movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                    if ((movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho) || (movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho))
                    {
                        if (ddlEmpenho.Items.Count > 1)
                            NumeroEmpenhoCombo = movimento.Empenho;
                    }

                    //Atualiza o grid
                    gridSubItemMaterial.DataSource = movimento.MovimentoItem;
                    gridSubItemMaterial.DataBind();
                }
            }
        }


        #endregion

        #region Metodos do Empenho


        private Tuple<bool, bool, bool, int, List<string>, decimal, bool> AdicionarItemEmpenho(bool bNumeroDoc, bool bGravar, bool bDataRec, int selecioneItem, List<string> msgErro,
            RepeaterItem ri, MovimentoEntity mov, IList<MovimentoItemEntity> newList, TextBox qtdLiq, TextBox qtdMov,
            TextBox txtIdLote, TextBox txtVctoLote, bool UltimoMov, int countLote, decimal saldoItemlote, bool contemLote)
        {
            string strSubItemMaterialCodigo = null;
            bool gravarLinha = true;


            // instanciar cada item
            Label itemMaterialCodigo = ri.FindControl("lblItemMatGrid") as Label;
            DropDownList drpSubItem = ri.FindControl("ddlSubItemList") as DropDownList;
            Label qtdLiqSiafisico = ri.FindControl("lblQtdeLiqSiafisico") as Label;
            //Label SaldoLiq = ri.FindControl("lblSaldoLiq") as Label;
            //TextBox txtSaldoLiq = ri.FindControl("txtSaldoLiq") as TextBox;
            TextBox txtValorItemEmpenho = ri.FindControl("txtValorItemEmpenho") as TextBox;
            Label precoUnit = ri.FindControl("lblPrecoUnitGrid") as Label;
            Label ValorUnit = ri.FindControl("lblValorUnit") as Label;
            HiddenField hdfQtdeLiqGrid = ri.FindControl("hdfQtdeLiqGrid") as HiddenField;
            TextBox txtQtdeLiqGrid = ri.FindControl("txtQtdeLiqGrid") as TextBox;
            Label lblUnidFornecimentoGridSiafisico = ri.FindControl("lblUnidFornecimentoGridSiafisico") as Label;
            // TextBox txtFabLote = ri.FindControl("txtFabLote") as TextBox;
            bool bddlUA = false;


            //DropDownList ddlPTResAcao = ri.FindControl("ddlPTResAcao") as DropDownList;
            //DropDownList ddlPTResItemMaterial = ri.FindControl("ddlPTResItemMaterial") as DropDownList;


            Label lblPosicao = ri.FindControl("lblPosicao") as Label;
            CheckBox chkLiquidarEmpenhoBEC = ri.FindControl("chkLiquidarEmpenhoBEC") as CheckBox;
            strSubItemMaterialCodigo = drpSubItem.SelectedItem.Value;

            decimal QtdeLiqGrid = string.IsNullOrEmpty(txtQtdeLiqGrid.Text) ? 0 : Convert.ToDecimal(txtQtdeLiqGrid.Text.Replace(".", ""));
            decimal QtdeLiqGridT = string.IsNullOrEmpty(hdfQtdeLiqGrid.Value) ? 0 : Convert.ToDecimal(hdfQtdeLiqGrid.Value.Replace(".", ""));

            if (drpSubItem.SelectedItem.Value != "- Selecione -")
            {
                strSubItemMaterialCodigo = drpSubItem.SelectedItem.Value;
                if (GeralEnum.GetEnumDescription(TipoEmpenho.BEC) == movimento.EmpenhoEvento.Descricao)
                {
                    gravarLinha = false;
                   
                    //if (QtdeLiqGrid == QtdeLiqGridT)
                    //{
                    //    if (chkLiquidarEmpenhoBEC.Checked)
                    //        bGravar = true;
                    //}
                    //else
                    //{
                    //    gravarLinha = false;

                    //}

                    //if (!(!chkLiquidarEmpenhoBEC.Checked && ((TratamentoDados.TryParseDecimal(qtdMov.Text) == 0.0m) || qtdMov.Text == "")))
                    //    if (chkLiquidarEmpenhoBEC.Checked && (TratamentoDados.TryParseDecimal(qtdMov.Text) != 0.0m))
                    if (!(!chkLiquidarEmpenhoBEC.Checked && ((TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text) == 0.0m) || txtValorItemEmpenho.Text == "")))
                        if (chkLiquidarEmpenhoBEC.Checked && (TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text) != 0.0m))
                            gravarLinha = chkLiquidarEmpenhoBEC.Checked;

                    if (!gravarLinha)
                    {
                        MovimentoItemEntity movItem = new MovimentoItemEntity();
                        movItem = mov.MovimentoItem.Where(MovimentoItem => MovimentoItem.SubItemMaterial.Id.Value == TratamentoDados.TryParseLong(strSubItemMaterialCodigo) && MovimentoItem.UnidadeFornecimentoSiafisicoDescricao.Trim() == lblUnidFornecimentoGridSiafisico.Text.Trim()).FirstOrDefault();

                        if (movItem != null)
                        {
                            movItem.QtdeMov = 0;

                        }
                    }
                }


                if (gravarLinha)
                {

                    qtdLiq.Text = string.IsNullOrEmpty(qtdLiq.Text) ? "0" : qtdLiq.Text;
                    qtdMov.Text = string.IsNullOrEmpty(qtdMov.Text) ? "0" : qtdMov.Text;

                    bGravar = true;
                    string subitem = drpSubItem.SelectedValue;
                    MovimentoItemEntity movItem = new MovimentoItemEntity();
                    movItem = mov.MovimentoItem.Where(MovimentoItem => MovimentoItem.SubItemMaterial.Id.Value == TratamentoDados.TryParseLong(strSubItemMaterialCodigo) && MovimentoItem.UnidadeFornecimentoSiafisicoDescricao.Trim() == lblUnidFornecimentoGridSiafisico.Text.Trim()).FirstOrDefault();

                    if (movItem != null)
                    {
                        //movItem.QtdeLiqSiafisico = TratamentoDados.TryParseDecimal(qtdLiqSiafisico.Text);
                        movItem.UGE = new UGEEntity(UgeId);


                        movItem.QtdeLiq = TratamentoDados.TryParseDecimal(qtdLiq.Text, true);
                        movItem.QtdeLiqSiafisico = TratamentoDados.TryParseDecimal(qtdLiqSiafisico.Text, true);
                        movItem.QtdeMov = TratamentoDados.TryParseDecimal(qtdMov.Text, true);
                        //movItem.PrecoUnit = (TratamentoDados.TryParseDecimal(qtdMov.Text) == 0.0m) ? (TratamentoDados.TryParseDecimal(qtdMov.Text)) : (TratamentoDados.TryParseDecimal(qtdLiq.Text, true) * movItem.PrecoUnit) / TratamentoDados.TryParseDecimal(qtdMov.Text, true);
                        movItem.PrecoUnit = TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text, true);
                        movItem.ValorUnit = Convert.ToDecimal(ValorUnit.Text);
                        //movItem.ValorMov = TratamentoDados.TryParseDecimal(qtdLiq.Text, true) * TratamentoDados.TryParseDecimal(precoUnit.Text, true);
                        movItem.ValorMov = TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text, true);
                        movItem.ValorMov = movItem.ValorMov.Value.truncarDuasCasas();
                        //movItem.SaldoLiq = TratamentoDados.TryParseDecimal(SaldoLiq.Text, true);
                        //movItem.SaldoValor = movItem.SaldoLiq;
                        movItem.SaldoLiq = TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text, true);
                        if (ddlPTResAcao.SelectedValue != "0")
                        {
                            movItem.PTResAcao = ddlPTResAcao.SelectedItem.Text;
                            movItem.PTResCodigo = ddlPTResItemMaterial.SelectedItem.Text;
                        }

                        //Esta consistencia foi retirada. Tera de ser consistida pelo SIAFEM, pois o SAM Estoque nao devera fazer este tipo de verificacao
                        //if (movItem.SaldoLiq < movItem.ValorUnit && (movItem.ValorMov.Value != 0.00m))
                        //    msgErro.Add(String.Format("Valor a liquidar para o subitem #{0:D2} tem de ser maior ou igual a R$ {1:#,##0.##00}!", Int32.Parse(lblPosicao.Text), movItem.ValorUnit));

                        if (!(this.DataMovimento == null || this.DataDocumento == null))
                            movItem.PrecoUnit = movItem.PrecoUnit.Value.Truncar(this.DataMovimento.Value.RetornarAnoMesReferenciaNumeral().ToString(), true);
                        else
                            bDataRec = true;


                        if (string.IsNullOrEmpty(this.NumeroDocumento))
                            bNumeroDoc = true;


                        if (ddlUA.SelectedValue == "-Selecione-" || ddlPTResAcao.SelectedValue == "0" || ddlPTResItemMaterial.SelectedValue == "0")
                            bddlUA = true;


                        if (drpSubItem.SelectedIndex <= 0 && (movItem.QtdeMov.HasValue && movItem.QtdeMov > 0))
                        {
                            msgErro.Add("Selecionar um subitem para o item #" + Int32.Parse(lblPosicao.Text) + " - " + itemMaterialCodigo.Text + "!");
                        }
                    }
                }
                else
                    bGravar = true;
            }
            else
            {
                //if ((TratamentoDados.TryParseDecimal(qtdLiq.Text) != 0.0m))
                if ((TratamentoDados.TryParseDecimal(txtValorItemEmpenho.Text) != 0.0m))
                    selecioneItem++;
                else
                    bGravar = true;
            }

            return new Tuple<bool, bool, bool, int, List<string>, decimal, bool>(bNumeroDoc, bGravar, bDataRec, selecioneItem, msgErro, saldoItemlote, bddlUA);
        }

        protected bool GravarMovimentoEmpenho()
        {
            bool bGravar = false;
            bool bDataRec = false;
            bool bNumeroDoc = false;
            bool bddlUa = false;
            List<string> msgErro = new List<string>();
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                MovimentoEntity mov = new MovimentoEntity();
                mov = GetSession<MovimentoEntity>(sessaoMov);
                // salva o almoxarifado o perfil
                mov.Almoxarifado = new AlmoxarifadoEntity(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);
                mov.AnoMesReferencia = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                mov.UA = new UAEntity() { Id = this.UaId, Codigo = this.CodigoUA };

                vincularPTRes_MovimentoItem(mov);

                this.AnoMesReferencia = mov.AnoMesReferencia;

                IList<MovimentoItemEntity> newList = mov.MovimentoItem;
                int selecioneItem = 0;
                mov.ValorDocumento = 0;
                decimal saldoItemlote = 0;
                bool opcaoAtivaCheck = false;

                Tuple<bool, bool, bool, int, List<string>, decimal, bool> retorno = new Tuple<bool, bool, bool, int, List<string>, decimal, bool>(bNumeroDoc, bGravar, bDataRec, selecioneItem, msgErro, saldoItemlote, bddlUa);

                foreach (RepeaterItem ri in gridSubItemMaterial.Items)
                {
                    //Repeater rptItemLote = ri.FindControl("rptItemLote") as Repeater;
                    Label lblUnidFornecimentoGridSiafisico = ri.FindControl("lblUnidFornecimentoGridSiafisico") as Label;
                    CheckBox chkLiquidarEmpenhoBEC = ri.FindControl("chkLiquidarEmpenhoBEC") as CheckBox;

                    TextBox qtdLiq = new TextBox();
                    TextBox qtdMov = new TextBox();

                    qtdLiq = ri.FindControl("txtQtdeLiqGrid") as TextBox;
                    qtdMov = ri.FindControl("txtQtdeMovGrid") as TextBox;

                    

                    if (chkLiquidarEmpenhoBEC.Checked && opcaoAtivaCheck == false)
                        opcaoAtivaCheck = true;


                    retorno = AdicionarItemEmpenho(bNumeroDoc, bGravar, bDataRec, selecioneItem, msgErro, ri, mov, newList, qtdLiq, qtdMov, null, null, true, 0, 0, false);
                }

                if (retorno.Item1)
                    msgErro.Add("Preencher o Numero do Documento");

                if (retorno.Item7)
                    msgErro.Add("Selecionar UA e/ou PTRes.");

                if (!retorno.Item2)
                {
                    msgErro.Add("Não foi informada a quantidade no empenho corretamente!");

                    msgErro.Add("Selecionar o(s) item(ns) a ser(em) liquidado(s) ou preencher a(s) quantidade(s) a ser(em) liquidada(s)!");
                }

                if (retorno.Item3)
                    msgErro.Add("Preencher a Data de Recebimento e/ou Data do Documento corretamente!");

                if (retorno.Item4 > 0)
                    msgErro.Add("Há " + selecioneItem.ToString() + " SubItem(ns) para selecionar!");

                if(!opcaoAtivaCheck && (GeralEnum.GetEnumDescription(TipoEmpenho.BEC) == movimento.EmpenhoEvento.Descricao))
                    msgErro.Add("Selecionar item(ns) a ser(em) liquidado(s)");

                if (retorno.Item5.Count > 0)
                {
                    ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                    this.ListaErros = msgErro;
                    return false;
                }

                if (TipoMovimento == tipoMovimentacao.ConsumoImediatoEmpenho.GetHashCode())
                {
                    var valorAtualMovimentacao = mov.MovimentoItem.Sum(itemMovimentacao => itemMovimentacao.ValorMov);
                    if (this.ValorDocumento < valorAtualMovimentacao)
                    {
                        ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                        this.ListaErros = new List<string>() { String.Format("Valor da somatória de subitens da movimentação (R$ {0:#,##0.00}) é superior ao valor do empenho {1} (R$ {2:#,##0.00})", valorAtualMovimentacao, this.NumeroEmpenhoCombo, this.ValorDocumento) };

                        return false;
                    }
                }

                for (int i = 0; i < mov.MovimentoItem.Count(); i++)
                {
                    //if (mov.MovimentoItem[i].QtdeMov == 0 || mov.MovimentoItem[i].QtdeMov == null)
                    if (mov.MovimentoItem[i].ValorMov == 0 || mov.MovimentoItem[i].ValorMov == null)
                    {
                        mov.MovimentoItem.RemoveAt(i);
                        i = i - 1;
                    }
                }


                SetSession(mov, sessaoMov);

                return true;
            }
            return true;
        }

        public void LimparListaEmpenho() { ddlEmpenho.Items.Clear(); }
        //public void LimparListaLicitacao() { ddlEmpenhoEvento.Items.Clear(); }

        private void InstanciaObjetoMovimento()
        {
            if (movimento == null) movimento = new MovimentoEntity();
            movimento.MovimentoItem = null;
            if (movimento.TipoMovimento == null) movimento.TipoMovimento = new TipoMovimentoEntity();
            if (movimento.Almoxarifado == null) movimento.Almoxarifado = new AlmoxarifadoEntity();
            if (movimento.Divisao == null) movimento.Divisao = new DivisaoEntity();
            if (movimento.MovimAlmoxOrigemDestino == null) movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();
            if (movimento.UGE == null) movimento.UGE = new UGEEntity();
            if (movimento.UA.IsNull()) movimento.UA = new UAEntity();
        }

        protected void ddlEmpenho_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["ValidaLiqTotal"] = false;
            //if ((TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagar) && (ddlEmpenho.SelectedIndex > 0))
            if ((TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho) && (ddlEmpenho.SelectedIndex > 0))
            {
                ddlEmpenho.Enabled = false;

                decimal valorDoc = 0.00m;
                gridSubItemMaterial.DataSource = null;
                gridSubItemMaterial.DataBind();


                Decimal.TryParse(ddlEmpenho.SelectedItem.Value.BreakLine(";", 1), out valorDoc);
                ValorDocumento = valorDoc;

                //PopularListaUAs();
                PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial);
                return;
            }

            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            // Procurar itens por empenho
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
            // LimparDadosDocumento(true);


            var ugeAlmoxLogado = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Codigo.Value;
            var almoxID = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            var oldUgeID = this.UgeId;
            var oldCodigoUGE = this.CodigoUGE;

            var oldUaID = this.UaId;
            var oldCodigoUA = this.CodigoUA;

            if (!String.IsNullOrWhiteSpace(NumeroEmpenhoCombo) && this.dadosAcessoSiafemPreenchidos)
                movimento = mat.obterItensEmpenho(ugeAlmoxLogado, almoxID, NumeroEmpenhoCombo, this.loginSiafem, this.senhaSiafem);
            else if (!this.dadosAcessoSiafemPreenchidos)
            {
                efetuarAcessoSIAFEM();

                return;
            }

            //movimento.UA = new UAEntity() { Id = this.UaId, Codigo = this.CodigoUA, Uge = new UGEEntity() { Id = oldUgeID, Codigo = oldCodigoUGE } };
            //if (movimento != null && movimento.MovimentoItem.HasElements())
            if (movimento != null)
            {
                ViewState["tipoEmpenho"] = movimento.EmpenhoEvento != null ? movimento.EmpenhoEvento.Descricao : null;
                mat.CarregarMovimentoTela(movimento);

                foreach (var item in movimento.MovimentoItem)
                {
                    if (item.UnidadeFornecimentoSiafisico != null)
                    {
                        if (string.IsNullOrEmpty(item.Produtor) && string.IsNullOrEmpty(item.ItMeEpp))
                            item.UnidadeFornecimentoSiafisicoDescricao = item.UnidadeFornecimentoSiafisico.Descricao;
                        else
                            item.UnidadeFornecimentoSiafisicoDescricao = item.UnidadeFornecimentoSiafisicoDescricao = item.UnidadeFornecimentoSiafisico.Descricao.Trim() + "-" + item.Produtor + "" + item.ItMeEpp;
                    }
                    else
                    {
                        this.ListaErros = new List<string>() { String.Format("Unidade de Fornecimento Siafisico não cadastrada no SAM, entre em contato com o suporte - Item Siafisico {0}", item.ItemMaterialCodigo) };
                        return;
                    }

                    item.QtdeMov = 1.000m;
                }

                
                this.UgeId = oldUgeID;
                //this.CodigoUGE = oldCodigoUGE;
                this.UaId = oldUaID;
                //this.CodigoUA = oldCodigoUA;
                


                movimento.UA = new UAEntity() { Id = this.UaId, Codigo = this.CodigoUA, Uge = new UGEEntity() { Id = oldUgeID, Codigo = oldCodigoUGE } };

                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    RemoveSession(sessaoMov);
                    SetSession(movimento, sessaoMov);
                }
                else
                {
                    SetSession(movimento, sessaoMov);
                }

                PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial);

                gridSubItemMaterial.DataSource = movimento.MovimentoItem;
                gridSubItemMaterial.DataBind();

                if (ListInconsistencias.ErroCount() > 0)
                {
                    BloqueiaCancelar = false;
                    BloqueiaGravar = true;
                }
                else
                {
                    BloqueiaCancelar = false;
                    BloqueiaGravar = false;
                }

                AtualizaInscricaoCE();
            }
            else
            {
                limparDadosAcessoSIAFEM();
            }

        }

        //protected void PopularComboEmpenhos()
        protected void PopularComboEmpenhosConsumoImediato()
        {
            var listarRestosAPagarEmpenhoConsumoImediato = (this.TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho);
            //var ugeAlmoxLogado = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Codigo.Value;

            int codigoUGESelecionada = -1;
            var ugeSelecionada = this.ddlUGE.SelectedItem.Text.BreakLine(0);
            int almoxarifadoId = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            int gestorId = this.GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value;


            IList<string> lstEmpenhosUGE = null;

            LimparListaEmpenho();

            //if (this.dadosAcessoSiafemPreenchidos)
            if ((this.dadosAcessoSiafemPreenchidos) && ((!string.IsNullOrWhiteSpace(ugeSelecionada) && Int32.TryParse(ugeSelecionada, out codigoUGESelecionada))))
                {
                ConsumoImediatoPresenter objPresenter = new ConsumoImediatoPresenter(this);
                lstEmpenhosUGE = objPresenter.ObterListaEmpenhos(almoxarifadoId, gestorId, codigoUGESelecionada, this.loginSiafem, this.senhaSiafem, false, listarRestosAPagarEmpenhoConsumoImediato);

                if (lstEmpenhosUGE.HasElements())
                {
                    InicializarDropDownList(ddlEmpenho);

                    if (!listarRestosAPagarEmpenhoConsumoImediato)
                        lstEmpenhosUGE.ToList().ForEach(_empenho => ddlEmpenho.Items.Add(new ListItem(_empenho, _empenho)));
                    else
                        lstEmpenhosUGE.ToList().ForEach(_empenho => ddlEmpenho.Items.Add(new ListItem(_empenho.BreakLine(";", 0), _empenho)));

                    ddlEmpenho.Enabled = true;
                }
            }
            else if (!this.dadosAcessoSiafemPreenchidos)
            {
                efetuarAcessoSIAFEM();
            }

        }

        private static void limparDadosAcessoSIAFEM()
        {
            RemoveSession("loginWsSiafem");
            RemoveSession("senhaWsSiafem");
        }

        public void SetarUGELogado()
        {
            int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            if (ddlUGE.Items.Count > 0) ddlUGE.SelectedValue = ugeLogado.ToString();
        }

        override protected void OnInit(EventArgs e) { base.OnInit(e); }

        public void txtQtde_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQtde = (sender as TextBox);

            if (!txtQtde.IsNull())
                txtQtde.ReformatarValorNumerico(base.fmtFracionarioMaterialQtde);
        }

        public void LimparDadosDocumento()
        {
            ConsumoImediatoPresenter presenter = new ConsumoImediatoPresenter(this);
            presenter.LimparMovimento();
            presenter.LimparItem();
            OrgaoTransferencia = string.Empty;
            hdfInfoSiafemCEOld.Text = string.Empty;
            ddlPTResAcao.SelectedValue = "0";
            ddlPTResItemMaterial.SelectedValue = "0";
            txtDescricaoPTRes.Text = string.Empty;


            if (TipoMovimento == (int)tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho)
            {
                //PopularComboEmpenhos();
                PopularComboEmpenhosConsumoImediato();
            }
        }

        public void LimparDadosDocumento(bool limparDataRecebimento)
        {
            LimparDadosDocumento();

            if (limparDataRecebimento)
                txtDataReceb.Text = DateTime.Now.ToString(base.fmtDataFormatoBrasileiro);
        }

        #endregion

        #region Focar Objeto

        public void FocarFornecedor() { SetFocus(txtCodFornecedor); }
        public void FocarDataDocumento() { SetFocus(txtDataEmissao); }
        public void FocarItemDataVencLote() { SetFocus(txtVencimentoLote); }
        public void FocarDataRecebimento() { SetFocus(txtDataReceb); }
        public void FocarItemQtdeEntrar() { SetFocus(txtQtdeMov); }
        public void FocarValorMovItem() { SetFocus(txtValorMovItem); }
        public void FocarFabricLoteItem() { SetFocus(txtFabricLoteItem); }

        #endregion

        #region Operação Entrada

        private void painelSideRight(bool ativar)
        {
            //ddlUGE.Enabled = false;
            ddlUGE.Enabled = ativar;
            txtCodFornecedor.Enabled = ativar;
            ddlEmpenho.Enabled = ativar;
            //txtEmpenho.Enabled = ativar;
            btnListarEmpenhos.Enabled = false;
            txtOrgao_Transferencia.Enabled = ativar;
            txtGeradorDescricao.Enabled = false;
            txtDocumentoAvulso.Enabled = ativar;
            txtDataEmissao.Enabled = ativar;
            txtDataReceb.Enabled = ativar;
            txtValorTotal.Enabled = false;
            txtObservacoes.Enabled = ativar;
            ddlPTResAcao.Enabled = ativar;
            ddlPTResItemMaterial.Enabled = ativar;

            //BloqueiaNovo = !(this.TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode());
        }

        protected void optNota()
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            if (!tipoMovimentoSelecionado())
            {
                ExibirMensagem("Selecionar o tipo de movimento.");
                return;
            }


            LimparGridSubItemMaterial();
            imgLupaRequisicao.Visible = true;
            btnEstornar.Visible = false;
            btnGravar.Visible = false;
            btnCancelar.Visible = true;
            carregarPermissaoAcesso();
            mat.HabilitarControlesNovo(Convert.ToInt16(rblTipoMovimento.SelectedValue));
            imgLupaFornecedor.Visible = false;
            btnNovo.Visible = false;

            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                BloqueiaBotaoCarregarEmpenho = true;
            BloqueiaImprimir = true;
            ExibirImprimir = true;
            BloqueiaEstornar = true;
            BloqueiaGravar = true;
            painelSideRight(false);

            ExibirNumeroEmpenho = true;
            ExibirListaEmpenho = false;
        }

        protected void optEstornar()
        {
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            if (!tipoMovimentoSelecionado())
            {
                ExibirMensagem("Selecionar o tipo de movimento.");
                return;
            }


            LimparGridSubItemMaterial();
            imgLupaRequisicao.Visible = true;
            btnEstornar.Visible = true;
            BloqueiaEstornar = false;
            btnGravar.Visible = false;
            btnCancelar.Visible = true;
            carregarPermissaoAcesso();

            mat.HabilitarControlesNovo(Convert.ToInt16(rblTipoMovimento.SelectedValue));
            imgLupaFornecedor.Visible = false;
            btnNovo.Visible = false;

            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                BloqueiaBotaoCarregarEmpenho = true;
            BloqueiaImprimir = true;
            ExibirImprimir = false;
            BloqueiaEstornar = true;
            painelSideRight(false);

            ExibirNumeroEmpenho = true;
            ExibirListaEmpenho = false;

            //Limpeza Combos Tela
            InicializarDropDownList(ddlPTResItemMaterial);
            InicializarDropDownList(ddlPTResAcao);
            //PopularComboEmpenhos();
            PopularComboEmpenhosConsumoImediato();
        }

        protected void optNovo()
        {
            if (!tipoMovimentoSelecionado())
            {
                ExibirMensagem("Selecionar o tipo de movimento.");
                return;
            }


            LimparGridSubItemMaterial();
            btnNovo.Visible = true;
            btnGravar.Visible = true;
            BloqueiaGravar = false;
            btnEstornar.Visible = false;
            BloqueiaEstornar = true;
            btnCancelar.Visible = true;
            imgLupaRequisicao.Visible = false;
            List<String> listaErro = new List<string>();
            int UgeId = 0;
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            btnExcluirItem.Visible = true;
            if (ddlUGE.SelectedIndex == 0)
            {
                ExibirMensagem("Selecionar a UGE.");
                return;
            }
            carregarPermissaoAcesso();

            UgeId = TratamentoDados.TryParseInt32(ddlUGE.SelectedValue).Value;
            // não limpar o grid caso seja aquisição direta
            txtValorMovItem.Enabled = true;

            if (this.TipoMovimento != (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            {
                //RemoveSession("movimento");
                RemoveSession(sessaoMov);
                mat.LimparMovimento();
                BloqueiaBotaoCarregarEmpenho = true;

                BloqueiaNovo = !(this.TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode());
            }
            else
            {
                BloqueiaBotaoCarregarEmpenho = false;
                txtValorMovItem.Enabled = false;

                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    movimento = GetSession<MovimentoEntity>(sessaoMov);
                    movimento.Id = null;
                }

                if (listaErro.Count > 0)
                {
                    ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                    this.ListaErros = listaErro;
                    return;
                }
                mat.LimparMovimentoCompraDireta();
                BloqueiaNovo = true;
            }
            mat.LimparItem();
            this.UgeId = UgeId;
            this.BloqueiaItemEfetivado = false;
            HabilitaPesquisaItemMaterial = true;
            mat.HabilitarControlesNovo(Convert.ToInt16(rblTipoMovimento.SelectedValue));
            imgLupaRequisicao.Visible = false;
            ExibirImprimir = false;
            BloqueiaGravar = true;
            painelSideRight(true);

            txtEmpenho.Visible = false;
            ExibirListaEmpenho = true;
        }

        #endregion

        #region Grid

        private void HabilitaEditcao(RepeaterItemEventArgs e)
        {
            //Estorno não edita
            if (this.TipoOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Estorno || this.TipoOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.NotaFornecimento)
            {
                object controle = e.Item.FindControl("linkID");

                if (controle != null)
                    ((ImageButton)controle).Enabled = false;
            }
            else
            {
                object controle = e.Item.FindControl("linkID");

                if (controle != null)
                    ((ImageButton)controle).Enabled = true;
            }
        }

        protected void gridSubItemMaterial_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int subItemId = 0;

            if (e.CommandName == "Select")
            {
                ViewState["botao"] = "btnEditar";
                rblLote.Enabled = false;
                txtSubItem.Enabled = false;

                subItemId = Convert.ToInt32(e.CommandArgument);

                // atribuições para empenho

                if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                {
                    if (btnNovo.Enabled)
                    {
                        MostrarPainelEdicao = true;
                        BloqueiaGravarItem = true;
                        BloqueiaExcluirItem = true;
                    }
                    else
                    {
                        btnExcluirItem.Visible = true;
                        BloqueiaGravarItem = false;

                        if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                        {
                            btnExcluirItem.Visible = false;
                        }
                    }
                }
                Label lblMovimentoId = (Label)e.Item.FindControl("lblMovimentoId");
                Label lblMovimentoItemId = (Label)e.Item.FindControl("lblMovimentoItemId");
                Label lblItemMaterialId = (Label)e.Item.FindControl("lblItemMaterialId");
                Label lblUnidadeFornecimentoId = (Label)e.Item.FindControl("lblUnidadeFornecimentoId");
                this.Id = lblMovimentoId.Text;
                this.MovimentoItemId = subItemId;
                ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                    mat.LerRegistro(mov, this.MovimentoItemId.Value);
                    pnlGrid.Enabled = false;
                }

                if (!string.IsNullOrEmpty(this.IdentificacaoLoteItem))
                {
                    rblLote.SelectedValue = "1";
                    DivLote.Visible = true;
                }
                else
                {
                    rblLote.SelectedValue = "0";
                    DivLote.Visible = false;

                }
            }

        }

        protected void PreencherGridHeader(int tipoMovimento, RepeaterItemEventArgs e)
        {
            HtmlTableCell celHdrQtdeLiq = (HtmlTableCell)e.Item.FindControl("celHdrQtdeLiq");
            HtmlTableCell hdr2QtdLiq1 = (HtmlTableCell)e.Item.FindControl("hdr2QtdLiq1");
            HtmlTableCell hdr2QtdLiq2 = (HtmlTableCell)e.Item.FindControl("hdr2QtdLiq2");
            //HtmlTableCell rowLote = (HtmlTableCell)e.Item.FindControl("hdrLote");
            HtmlTableCell rowDescricao = (HtmlTableCell)e.Item.FindControl("hdrDescricao");
            HtmlTableCell hdrEdit = (HtmlTableCell)e.Item.FindControl("hdrEdit");
            HtmlTableCell hdrPrecoTotal = (HtmlTableCell)e.Item.FindControl("hdrPrecoTotal");
            HtmlTableCell hdrPrecoUnitSAM = (HtmlTableCell)e.Item.FindControl("hdrPrecoUnitSAM");
            HtmlTableCell hdrSaldoLiquidar = (HtmlTableCell)e.Item.FindControl("hdrSaldoLiquidar");
            HtmlTableCell celHdrUnidSiaf = (HtmlTableCell)e.Item.FindControl("celHdrUnidSiaf");
            HtmlTableCell HeaderDadosEmpenho = (HtmlTableCell)e.Item.FindControl("HeaderDadosEmpenho");
            HtmlTableCell celHdrUnidSub = (HtmlTableCell)e.Item.FindControl("celHdrUnidSub");
            HtmlTableCell celHdrQuant = (HtmlTableCell)e.Item.FindControl("celHdrQuant");
            HtmlTableRow celHeaderEmpenho = (HtmlTableRow)e.Item.FindControl("celHeaderEmpenho");


            Label lblItem = (Label)e.Item.FindControl("lblItem");
            Label lblUnidSiaf = (Label)e.Item.FindControl("lblUnidSiaf");
            Label lblSIAFEM = (Label)e.Item.FindControl("lblSIAFEM");
            Label lblUnidSubitem = (Label)e.Item.FindControl("lblUnidSubitem");
            Label lblUnidLiquidar = (Label)e.Item.FindControl("lblUnidLiquidar");
            Label lblQtdRecSubitem = (Label)e.Item.FindControl("lblQtdRecSubitem");
            Label lblPrecoUnit = (Label)e.Item.FindControl("lblPrecoUnit");

            //Liquidação Empenho Tipo BEC
            HtmlTableCell hdrLiquidar = (HtmlTableCell)e.Item.FindControl("hdrLiquidar");
            HtmlTableCell hdrQtdRecSubitem = (HtmlTableCell)e.Item.FindControl("hdrQtdRecSubitem");

            celHdrUnidSiaf.Style.Add("display", "none"); // Visible = false;
            celHdrUnidSub.Style.Add("display", ""); // Visible = false;
            hdrPrecoTotal.Style.Add("display", ""); // Visible = true;
            hdrPrecoUnitSAM.Style.Add("display", "none");
            hdrSaldoLiquidar.Style.Add("display", "none");
            celHdrQtdeLiq.Style.Add("display", "none"); // Visible = false;
            hdr2QtdLiq1.Style.Add("display", "none"); // Visible = false;
            hdr2QtdLiq2.Style.Add("display", "none"); // Visible = false;
            hdrEdit.Style.Add("display", ""); // Visible = true;
            hdrEdit.RowSpan = 2;


            if(rowDescricao.IsNotNull())
                rowDescricao.ColSpan = 6;

            celHdrQuant.Style.Add("display", "none");
            celHeaderEmpenho.Style.Add("display", "none");


            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            {

                lblItem.Text = "Item Siafisico";
                bool temProdutor = movimento.MovimentoItem.Any(l => l.Produtor != string.Empty);
                bool temMeEpp = movimento.MovimentoItem.Any(l => l.ItMeEpp == "S");

                if (temProdutor || temMeEpp)
                    lblUnidSiaf.Text = "Unid. For." + ((temProdutor == true) ? "-CPF" : string.Empty) + ((temMeEpp == true) ? "-ME/EPP" : string.Empty);
                else
                    lblUnidSiaf.Text = "Unid. For.";
                lblSIAFEM.Text = "Quant.";
                lblUnidSubitem.Text = "Unid For.";
                lblUnidLiquidar.Text = "Quant. a Liquidar - EMPENHO";
                lblQtdRecSubitem.Text = "Quant. a Liquidar c/ Conversão p/ o  SAM";
                lblPrecoUnit.Text = "Valor Unitário Convertido";

                celHeaderEmpenho.Style.Add("display", ""); // Visible = true;
                celHdrQtdeLiq.Style.Add("display", "none"); // Visible = false; Usuario entrara apenas com o valor a liquidar
                hdrSaldoLiquidar.Style.Add("display", "");
                celHdrQuant.Style.Add("display", "");
                if (Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                {
                    HeaderDadosEmpenho.ColSpan = 3;
                    hdr2QtdLiq1.Style.Add("display", "none"); // Visible = false;
                    celHdrUnidSiaf.Style.Add("display", "none"); // Visible = false;
                    // hdrSaldoLiquidar.Style.Add("display", "none"); 

                }
                else
                {
                    HeaderDadosEmpenho.ColSpan = 5;
                    celHdrUnidSiaf.Style.Add("display", ""); // Visible = false;
                    hdr2QtdLiq1.Style.Add("display", ""); // Visible = false;
                }

                //Se for Empenho do Tipo BEC, exibir o campo de liquidação
                if (GeralEnum.GetEnumDescription(TipoEmpenho.BEC) == movimento.EmpenhoEvento.Descricao)
                {
                    HeaderDadosEmpenho.ColSpan = 5;
                    hdrLiquidar.Style.Add("display", "");
                }
                else
                {
                    hdrLiquidar.Style.Add("display", "none");

                    celHeaderEmpenho.Style.Add("display", "none"); // Visible = false;
                    HeaderDadosEmpenho.Style.Add("display", "none");
                    HeaderDadosEmpenho.ColSpan = 0;
                }

                hdr2QtdLiq2.Style.Add("display", "none"); // Visible = false; 
                hdr2QtdLiq2.ColSpan = 1;

                celHdrUnidSub.Style.Add("display", ""); // Visible = false;
                celHdrUnidSub.ColSpan = 1;


                hdrPrecoTotal.Style.Add("display", "none"); // Visible = false;
                // hdrPrecoUnitSAM.Style.Add("display", ""); -TEMP P/ DOUGLAS
                hdrEdit.Style.Add("display", "none");  // Visible = false;
            }
            else if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
            {
                lblItem.Text = "Item";
                lblUnidSiaf.Text = "Unid. Siafisico";
                lblSIAFEM.Text = "SIAFEM";
                lblUnidSubitem.Text = "Unid. Subitem";
                lblUnidLiquidar.Text = "Qtd. Liquidar";
                lblQtdRecSubitem.Text = "Qtd. Rec. Subitem";
                lblPrecoUnit.Text = "Preço Unit.";

                celHeaderEmpenho.Style.Add("display", "none"); // Visible = false;
                HeaderDadosEmpenho.Style.Add("display", "none");
                HeaderDadosEmpenho.ColSpan = 0;

                hdrSaldoLiquidar.Style.Add("display", "none"); // Visible = false;
                hdrEdit.Style.Add("display", ""); // Visible = true;
            }

            lblUnidLiquidar.Style.Add("display", "none");
        }

        protected void PreencherGridItem(int tipoMovimento, RepeaterItemEventArgs e, string lblNaturezaDespesaEmpenho)
        {
            RepeaterItem ri = e.Item;
            ConsumoImediatoPresenter mat = new ConsumoImediatoPresenter(this);
            // fazer cast dos objetos dentro do grid
            DropDownList drpSubItens = ri.FindControl("ddlSubItemList") as DropDownList;

            //DropDownList ddlPTResAcao = ri.FindControl("ddlPTResAcao") as DropDownList;
            //DropDownList ddlPTResItemMaterial = ri.FindControl("ddlPTResItemMaterial") as DropDownList;

            Label lblItemMaterialId = ri.FindControl("lblItemMaterialId") as Label;
            Label lblSubItem = ri.FindControl("lblSubItem") as Label;
            Label lblUnidFornecimentoGridSiafisico = (Label)ri.FindControl("lblUnidFornecimentoGridSiafisico");
            Label lblDescricaoGrid = (Label)ri.FindControl("lblDescricaoGrid");
            Label lblItemMaterialCodigo = ri.FindControl("lblItemMatGrid") as Label;
            Label lblQtdeLiqGridSiafisico = (Label)ri.FindControl("lblQtdeLiqGridSiafisico");
            Label lblPrecoUnitGrid = (Label)ri.FindControl("lblPrecoUnitGrid");
            Label lblPrecoUnitGridCalc = (Label)ri.FindControl("lblPrecoUnitGridCalc");
            Label lblValorUnit = (Label)ri.FindControl("lblValorUnit");
            //Label lblSaldoLiq = (Label)ri.FindControl("lblSaldoLiq");
            TextBox txtValorItemEmpenho = (TextBox)ri.FindControl("txtValorItemEmpenho");
            HiddenField hdfQtdeLiqGrid = (HiddenField)e.Item.FindControl("hdfQtdeLiqGrid");
            HiddenField hdfQtdeMovGrid = (HiddenField)e.Item.FindControl("hdfQtdeMovGrid");
            HtmlTableCell celQtdeLiq = ri.FindControl("celQtdeLiq") as HtmlTableCell;
            HtmlTableCell celQtdeLiq2 = ri.FindControl("celQtdeLiq2") as HtmlTableCell;
            HtmlTableCell celValorItemEmpenho = ri.FindControl("celValorItemEmpenho") as HtmlTableCell;
            HtmlTableCell celUnidSiaf = ri.FindControl("celUnidSiaf") as HtmlTableCell;
            HtmlTableCell celUnidSub = ri.FindControl("celUnidSub") as HtmlTableCell;
            HtmlTableCell celQuant = ri.FindControl("celQuant") as System.Web.UI.HtmlControls.HtmlTableCell;
            //System.Web.UI.HtmlControls.HtmlTableCell celTdEmpenho = ri.FindControl("celTdEmpenho") as System.Web.UI.HtmlControls.HtmlTableCell;
            //System.Web.UI.HtmlControls.HtmlTableCell celTdEmpenho2 = ri.FindControl("celTdEmpenho2") as System.Web.UI.HtmlControls.HtmlTableCell;

            //Liquidação Empenho Tipo BEC
            HtmlTableCell celLiquidar = ri.FindControl("celLiquidar") as HtmlTableCell;
            HtmlTableCell celQtdeMov = ri.FindControl("celQtdeMov") as HtmlTableCell;
            CheckBox chkLiquidarEmpenhoBEC = ri.FindControl("chkLiquidarEmpenhoBEC") as CheckBox;
            Label lblLiquidado = (Label)ri.FindControl("lblLiquidado");


            // dados do lote
            TextBox txtQtdeLiqGrid = (TextBox)e.Item.FindControl("txtQtdeLiqGrid");
            TextBox txtQtdeMovGrid = (TextBox)e.Item.FindControl("txtQtdeMovGrid");
            txtQtdeMovGrid.Style.Add("text-align", "center");
            txtQtdeLiqGrid.Style.Add("text-align", "center");
            txtValorItemEmpenho.Style.Add("text-align", "center");

            ScriptManager.RegisterStartupScript((TextBox)txtValorItemEmpenho, GetType(), "numerico", "$('.numerico').mask('999.999,999');", true);
            ScriptManager.RegisterStartupScript((TextBox)txtQtdeLiqGrid, GetType(), "numerico", "$('.numerico').mask('999.999,999');", true);
            ScriptManager.RegisterStartupScript((TextBox)txtQtdeMovGrid, GetType(), "numerico", "$('.numerico').mask('999.999,999');", true);
            ScriptManager.RegisterStartupScript((HiddenField)hdfQtdeLiqGrid, GetType(), "numerico", "$('.numerico').mask('999.999,999');", true);
            ScriptManager.RegisterStartupScript((HiddenField)hdfQtdeMovGrid, GetType(), "numerico", "$('.numerico').mask('999.999,999');", true);

            // colunas
            System.Web.UI.HtmlControls.HtmlTableCell celPosicao = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celPosicao");
            System.Web.UI.HtmlControls.HtmlTableCell cellEdit = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("cellEdit");
            System.Web.UI.HtmlControls.HtmlTableCell cellVlTotal = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celPrecoTotal");
            System.Web.UI.HtmlControls.HtmlTableCell cellPrecoUnitSAM = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celPrecoUnitSAM");
            // linhas
            // System.Web.UI.HtmlControls.HtmlTableCell rowLote = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("rowLote");
            System.Web.UI.HtmlControls.HtmlTableCell rowDescricao = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("rowDescricao");
            //System.Web.UI.HtmlControls.HtmlTableRow tbl = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("tblLote");
            //System.Web.UI.HtmlControls.HtmlTableRow trEmpenhoLote = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("trEmpenhoLote");
            //System.Web.UI.HtmlControls.HtmlTableRow trEmpenhoLoteNovo = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("trEmpenhoLoteNovo");
            //System.Web.UI.HtmlControls.HtmlTable tbEmpenhoLoteNovo = (System.Web.UI.HtmlControls.HtmlTable)ri.FindControl("tbEmpenhoLoteNovo");
            //label
            //Label lblLote = (Label)e.Item.FindControl("lblLote");
            //Label lblVctoLote = (Label)e.Item.FindControl("lblVctoLote");

            SubItemMaterialPresenter item = new SubItemMaterialPresenter();

            lblSubItem.Style.Add("display", "");
            drpSubItens.Style.Add("display", "none");
            lblUnidFornecimentoGridSiafisico.Style.Add("display", "none");
            cellVlTotal.Style.Add("display", "");
            cellPrecoUnitSAM.Style.Add("display", "none");
            cellEdit.Style.Add("display", "");
            celQtdeLiq.Style.Add("display", "none");
            celQtdeLiq2.Style.Add("display", "none");
            celValorItemEmpenho.Style.Add("display", "none");
            celUnidSiaf.Style.Add("display", "none");
            celUnidSub.Style.Add("display", "");
            cellEdit.RowSpan = 3;
            celPosicao.RowSpan = 2;
            celQuant.Style.Add("display", "none");
            //celTdEmpenho.Style.Add("display", "none");
            //celTdEmpenho2.ColSpan = 4;

            Label lblSubItemMaterialId = (Label)ri.FindControl("lblSubItemMaterialId");

            int idSubitemMaterial = 0;
            Int32.TryParse(lblSubItemMaterialId.Text, out idSubitemMaterial);
            SubItemMaterialEntity subItem = mat.SelectSubItemMaterialRetorno(idSubitemMaterial);

            if (ri.DataItem.IsNotNull() && ((MovimentoItemEntity)ri.DataItem).IsNotNull() && ((MovimentoItemEntity)ri.DataItem).UnidadeFornecimentoSiafisico.IsNotNull())
                lblUnidFornecimentoGridSiafisico.Text = (string.IsNullOrEmpty(((MovimentoItemEntity)ri.DataItem).Produtor) && string.IsNullOrEmpty(((MovimentoItemEntity)ri.DataItem).ItMeEpp)) ?
                    ((MovimentoItemEntity)ri.DataItem).UnidadeFornecimentoSiafisico.Descricao : ((MovimentoItemEntity)ri.DataItem).UnidadeFornecimentoSiafisicoDescricao;

            if (idSubitemMaterial != 0 && subItem != null)
            {

                if (TipoMovimento != (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
                {
                    cellEdit.RowSpan = 3;
                    celPosicao.RowSpan = 3;
                    rowDescricao.ColSpan = 6;

                    //trEmpenhoLote.Visible = false;
                    //trEmpenhoLoteNovo.Visible = false;
                    //tbEmpenhoLoteNovo.Visible = false;


                }
                else if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
                {
                    rowDescricao.ColSpan = 15;
                    cellEdit.RowSpan = 3;
                    celPosicao.RowSpan = 3;

                    lblUnidFornecimentoGridSiafisico.Style.Add("display", "none");

                }
            }

            //if (lblLote != null)
            //{
            //    if (!string.IsNullOrEmpty(lblLote.Text))
            //        trEmpenhoLote.Visible = true;
            //}


            if (TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho)
            //if ((TipoMovimento == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho) || (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho))
            {

                //PreencherListaPTResAcao(ddlPTResAcao, ddlPTResItemMaterial); TODO TESTE PARA NAO FICAR POPULANDO TODA HORA COMBOS PTRES

                //trEmpenhoLoteNovo.Visible = false;
                //tbEmpenhoLoteNovo.Visible = false;

                if (movimento.EmpenhoEvento.IsNotNull())
                {
                    this.lblTipoEmpenho.Text = movimento.EmpenhoEvento.Descricao;
                    this.lblTipoEmpenho.Visible = true;
                    //Se for Empenho do Tipo BEC, exibir o campo de liquidação
                    if (GeralEnum.GetEnumDescription(TipoEmpenho.BEC) == movimento.EmpenhoEvento.Descricao)
                    {
                        //txtQtdeLiqGrid.Enabled = false;
                        txtValorItemEmpenho.Enabled = false;
                        celLiquidar.Style.Add("display", "");
                        //if (Convert.ToDecimal(txtQtdeLiqGrid.Text) == Convert.ToDecimal(0))
                        if (Convert.ToDecimal(txtValorItemEmpenho.Text) == Convert.ToDecimal(0))
                        {
                            chkLiquidarEmpenhoBEC.Visible = false;
                            lblLiquidado.Visible = true;
                            //txtQtdeMov.Text = "0,000";
                        }
                        else
                        {
                            chkLiquidarEmpenhoBEC.Visible = true;
                            lblLiquidado.Visible = false;
                        }
                    }
                    else
                    {
                        celLiquidar.Style.Add("display", "none");
                        //txtQtdeLiqGrid.Enabled = true;
                        txtValorItemEmpenho.Enabled = true;
                    }
                }
                //celTdEmpenho.Style.Add("display", "");
                //celTdEmpenho2.ColSpan = 6;
                celQuant.Style.Add("display", "");

                celValorItemEmpenho.Style.Add("display", "");
                if (Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                {
                    //celTdEmpenho.ColSpan = 2;
                    celUnidSiaf.Style.Add("display", "none");
                    celQtdeLiq.Style.Add("display", "none");
                    // celSaldoLiq.Style.Add("display", "none");
                }
                else
                {
                    //celTdEmpenho.ColSpan = 4;
                    celUnidSiaf.Style.Add("display", "");
                    celQtdeLiq.Style.Add("display", "");

                }

                lblSubItem.Style.Add("display", "none");
                drpSubItens.Style.Add("display", "");

                //celQtdeLiq2.Style.Add("display", "");
                if (TipoMovimento == tipoMovimentacao.ConsumoImediatoEmpenho.GetHashCode())
                    celQtdeLiq2.Style.Add("display", "none");

                //txtQtdeLiqGrid.BorderStyle = BorderStyle.Solid;
                txtValorItemEmpenho.BorderStyle = BorderStyle.Solid;
                //txtQtdeLiqGrid.Enabled = true;
                txtQtdeMovGrid.BorderStyle = BorderStyle.Solid;
                txtQtdeMovGrid.Enabled = true;

                celUnidSub.Style.Add("display", "");
                // desabilitar campos caso a quantidade recebida atingiu a qtde total do item
                //if (!String.IsNullOrWhiteSpace(txtQtdeLiqGrid.Text) && Decimal.Parse(txtQtdeLiqGrid.Text) == 0)
                if (!String.IsNullOrWhiteSpace(txtValorItemEmpenho.Text) && Decimal.Parse(txtValorItemEmpenho.Text) == 0)
                {
                    //txtQtdeLiqGrid.Enabled = false;
                    txtValorItemEmpenho.Enabled = false;
                    txtQtdeMovGrid.Enabled = false;
                }


                //if (Convert.ToInt16(rblTipoOperacao.SelectedValue) != (int)GeralEnum.OperacaoEntrada.Estorno && Convert.ToInt16(rblTipoOperacao.SelectedValue) != (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                //{

                //    //if (Convert.ToDecimal(txtQtdeLiqGrid.Text) != Convert.ToDecimal(0))
                //    //{
                //    //    trEmpenhoLoteNovo.Visible = true;
                //    //    tbEmpenhoLoteNovo.Visible = true;
                //    //}

                //    if (GeralEnum.GetEnumDescription(TipoEmpenho.BEC) != movimento.EmpenhoEvento.Descricao)
                //        txtQtdeMovGrid.Text = txtQtdeLiqGrid.Text; //Alteração solicitada pelo Celso.
                //}

                cellEdit.Style.Add("display", "none");
                cellVlTotal.Style.Add("display", "none");
                // cellPrecoUnitSAM.Style.Add("display", ""); -TEMP P/ DOUGLAS

                if (Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt16(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                {
                    lblNaturezaDespesaEmpenho = subItem.DescricaoNaturezaDespesa;
                    lblValorUnit.Text = Convert.ToString(movimento.MovimentoItem[e.Item.ItemIndex].PrecoUnitSiafem);
                    //lblSaldoLiq.Text = Convert.ToString(movimento.MovimentoItem[e.Item.ItemIndex].ValorMov);
                    txtValorItemEmpenho.Text = Convert.ToString(movimento.MovimentoItem[e.Item.ItemIndex].ValorMov);
                    txtValorItemEmpenho.Enabled = false;
                }

                if (!string.IsNullOrEmpty(lblItemMaterialId.Text))
                    carregarListaSubItens(drpSubItens, lblNaturezaDespesaEmpenho, Convert.ToInt32(lblItemMaterialId.Text), true);

                lblUnidFornecimentoGridSiafisico.Style.Add("display", "");

                txtQtdeLiqGrid.Text = "1,000"; //Valor serah sempre 1.000, pois usuario entrarah apenas com o valor a liquidar
                txtQtdeLiqGrid.Style.Add("display", "none");

                txtQtdeMovGrid.Text = "1,000"; //Valor serah sempre 1.000, pois usuario entrarah apenas com o valor a liquidar
                txtQtdeMovGrid.Style.Add("display", "none");
            }
            else if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
            {

                txtQtdeMovGrid.BorderStyle = BorderStyle.None;
                txtQtdeMovGrid.Enabled = false;
                txtQtdeMovGrid.Style.Add("background-color", "transparent");
                //lblUnidFornecimentoGridSiafisico.Visible = false;
                lblUnidFornecimentoGridSiafisico.Visible = true;
                lblDescricaoGrid.Visible = true;
                lblValorTotal.Visible = true;

                celValorItemEmpenho.ColSpan = 3;

                if (TipoOperacao != tipoOperacao.Nova.GetHashCode() && TipoMovimento == tipoMovimentacao.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode())
                {
                    HtmlTableCell hdrEdit = e.Item.Parent.FindControlRecursive("hdrEdit") as HtmlTableCell;
                    if (hdrEdit.IsNotNull())
                    {
                        hdrEdit.Style.Add("display", "none"); // Visible = false;
                        cellEdit.Style.Add("display", "none");
                    }
                }

            }
            ReformatarValoresExibidosControles(base.fmtFracionarioMaterialQtde, txtQtdeLiq, txtQtdeLiqGrid, txtQtdeMov, txtQtdeMovGrid, lblQtdeLiqGridSiafisico);
            ReformatarValoresExibidosControles(base.fmtFracionarioMaterialValorUnitario, this.txtPrecoUnit, lblPrecoUnitGrid, lblValorUnit, lblPrecoUnitGridCalc);
            //hdfQtdeLiqGrid.Value = txtQtdeLiqGrid.Text;
            hdfQtdeLiqGrid.Value = txtValorItemEmpenho.Text;
            hdfQtdeMovGrid.Value = txtQtdeMovGrid.Text;
        }

        private void ReformatarValoresExibidosControles(string strFormatoNumerico, params Control[] arrControlesWeb)
        {
            foreach (var objQueExibeTexto in arrControlesWeb)
            {
                if (objQueExibeTexto.GetType().GetInterfaces().Contains(typeof(ITextControl)))
                    ((ITextControl)objQueExibeTexto).ReformatarValorNumerico(strFormatoNumerico);
            }
        }

        private Tuple<decimal, decimal> RecalculoQtdeLiqItemSiafisico(int qtdeTotalItemSiafisico, int saldoTotalItemSiafisico, int itemMaterialCodigo, int iUge_ID, string strEmpenhoSiafisicoCodigo, int iAlmoxarifado_ID, int gestorId, int iNaturezaDespesaCodigo)
        {
            ConsumoImediatoPresenter objPresenter = null;
            decimal qtdeTotalItemMaterialSam = 0.0m;
            decimal saldoTotalItemMaterialSam = 0.0m;
            decimal qtdeTotalEmpenhoSiafisico = 0.0m;
            decimal saldoTotalEmpenhoSiafisico = 0.0m;

            objPresenter = new ConsumoImediatoPresenter();

            string unidadeFornSiafem = "";

            qtdeTotalEmpenhoSiafisico = qtdeTotalItemSiafisico;
            saldoTotalEmpenhoSiafisico = saldoTotalItemSiafisico;
            var somaLiq = objPresenter.RecalculoQtdeLiqItemSiafisico(itemMaterialCodigo, iUge_ID, strEmpenhoSiafisicoCodigo, iAlmoxarifado_ID, gestorId, iNaturezaDespesaCodigo, unidadeFornSiafem, true);
            qtdeTotalItemMaterialSam = somaLiq.Item1;
            saldoTotalItemMaterialSam = somaLiq.Item2;


            return new Tuple<decimal, decimal>(qtdeTotalEmpenhoSiafisico - qtdeTotalItemMaterialSam, saldoTotalEmpenhoSiafisico - saldoTotalItemMaterialSam);
        }

        private decimal RecalculoValorLiquidadoItemEmpenho(int qtdeTotalItemSiafisico, int saldoValorTotalItemEmpenho, int itemMaterialCodigo, int iUge_ID, string strEmpenhoSiafisicoCodigo, int iAlmoxarifado_ID, int gestorId, int iNaturezaDespesaCodigo)
        {
            ConsumoImediatoPresenter objPresenter = null;
            decimal saldoTotalItemMaterialSam = 0.0m;
            decimal saldoValorTotalItemEmpenhoSiafisico = 0.0m;

            objPresenter = new ConsumoImediatoPresenter();

            string unidadeFornSiafem = "";

            saldoValorTotalItemEmpenhoSiafisico = saldoValorTotalItemEmpenho;
            saldoTotalItemMaterialSam = objPresenter.RecalculoValorLiquidadoItemEmpenho(itemMaterialCodigo, iUge_ID, strEmpenhoSiafisicoCodigo, iAlmoxarifado_ID, gestorId, iNaturezaDespesaCodigo, unidadeFornSiafem);


            return (saldoValorTotalItemEmpenhoSiafisico - saldoTotalItemMaterialSam);
        }

        protected void gridSubItemMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HabilitaEditcao(e);

            string lblNaturezaDespesaEmpenho = movimento.NaturezaDespesaEmpenho;

            if (e.Item.ItemType == ListItemType.Header)
            {
                PreencherGridHeader(TipoMovimento, e);
            }
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                PreencherGridItem(TipoMovimento, e, lblNaturezaDespesaEmpenho);

                if (Convert.ToInt32(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt32(rblTipoOperacao.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                {
                    if (movimento.MovimentoItem[e.Item.ItemIndex].SubItemMaterial.Id != null)
                    {
                        ((DropDownList)e.Item.FindControl("ddlSubItemList")).SelectedValue = movimento.MovimentoItem[e.Item.ItemIndex].SubItemMaterial.Id.ToString();
                        ((DropDownList)e.Item.FindControl("ddlSubItemList")).Enabled = false;
                    }
                }
                else
                {
                    if (((DropDownList)e.Item.FindControl("ddlSubItemList")).Items.Count > 2)
                        ((DropDownList)e.Item.FindControl("ddlSubItemList")).SelectedValue = "- Selecione -";
                }

                if ((!string.IsNullOrEmpty(movimento.MovimentoItem[e.Item.ItemIndex].PTResAcao)) && (!string.IsNullOrEmpty(movimento.MovimentoItem[e.Item.ItemIndex].PTResCodigo)))
                {
                    var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
                    if (!listaPTRes.HasElements())
                    {
                        var objPresenter = new PTResPresenter();
                        var lstPTResAcao = objPresenter.CarregarListaPTResAcao(movimento.UA.Uge.Codigo.Value);
                        PageBase.SetSession<IList<PTResEntity>>(lstPTResAcao, cstListagemPTResAcao);
                        listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
                    }

                    var _ptresAcao = listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                            && (ptres.ProgramaTrabalho.ProjetoAtividade == movimento.MovimentoItem[e.Item.ItemIndex].PTResAcao)
                                                            && (ptres.Codigo.ToString() == movimento.MovimentoItem[e.Item.ItemIndex].PTResCodigo))
                                               .FirstOrDefault();


                    if (_ptresAcao != null)
                    {

                        ddlPTResAcao.SelectedValue = ((_ptresAcao.ProgramaTrabalho.IsNotNull()) ? _ptresAcao.ProgramaTrabalho.ProjetoAtividade : "0");
                        ddlPTResItemMaterial.SelectedValue = _ptresAcao.Id.ToString();
                        txtDescricaoPTRes.Text = _ptresAcao.Descricao;
                    }

                }
            }


        }

        #endregion

        protected void btnListarEmpenhos_Click(object sender, EventArgs e)
        {
            //PopularComboEmpenhos();
            PopularComboEmpenhosConsumoImediato();
        }

        public bool BloqueiaDataRecebimento
        {
            set { txtDataReceb.Enabled = !value; }
        }


        public void RemoverSessao()
        {
            PageBase.RemoveSession(sessaoMov);
            movimento = null;
        }

        protected void efetuarAcessoSIAFEM()
        {
            var opcaoNotaFornecimentoSelecionada = (TipoOperacao == (int)tipoOperacao.NotaRecebimento);
            if (!opcaoNotaFornecimentoSelecionada)
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
        }

        public void PopularListaUnidade() { }
        public int UnidadeId
        {
            get { return 0; }
            set { }
        }

        protected void btnCarregarSubItem_Click(object sender, EventArgs e)
        {
            CarregarSubItensPorCodigo();
        }

        //TODO: Mover método para a(s) camada(s) correta(s): PRESENTER -> BUSINESS
        private void CarregarSubItensPorCodigo()
        {
            long? codigoSubItem = Common.Util.TratamentoDados.TryParseLong(txtSubItem.Text.Trim());
            SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
            var gestorId = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            var subItem = presenter.CarregarSubItem(codigoSubItem, gestorId);

            if (subItem != null)
            {

                SubItemMaterialDescricao = subItem.Descricao;
                SubItemMaterialId = subItem.Id;
                UnidadeCodigo = subItem.UnidadeFornecimento.Descricao;
                ItemMaterialCodigo = subItem.ItemMaterial.Codigo;
            }
            else
            {
                List<String> erro = new List<string>();
                erro.Add("Subitem não encontrado");
                this.ListaErros = erro;

                SubItemMaterialDescricao = string.Empty;
                SubItemMaterialId = null;
                UnidadeCodigo = string.Empty;
                ItemMaterialCodigo = null;
            }
        }

        protected int NumeroCasasDecimais_QtdeItem()
        {
            return base.numCasasDecimaisMaterialQtde;
        }
        protected int NumeroCasasDecimais_ValorUnitario()
        {
            return base.numCasasDecimaisValorUnitario;
        }
        protected void rblLote_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (rblLote.SelectedValue == "0")
            //{
            //    DivLote.Visible = false;
            //}
            //else
            //{
            //    DivLote.Visible = true;
            //    if (string.IsNullOrEmpty(IdentificacaoLoteItem))
            //        txtVencimentoLote.Enabled = false;
            //    else
            //    {
            //        if (rblTipoMovimento.SelectedValue != Convert.ToString((int)GeralEnum.TipoMovimento.EntradaPorDevolucao))
            //        {
            //            if (ValidarLoteVencimento() || string.IsNullOrEmpty(txtVencimentoLote.Text))
            //            {
            //                txtVencimentoLote.Enabled = true;
            //                return;
            //            }
            //            else
            //                txtVencimentoLote.Enabled = false;
            //        }
            //        else
            //            txtVencimentoLote.Enabled = false;
            //    }
            //}
        }

        private bool ValidaNumerico(string valor)
        {
            Regex rgx = new Regex(@"^[0-9]{7,8}$");

            return rgx.IsMatch(valor) ? true : false;
        }

        protected void txtNLote_TextChanged(object sender, EventArgs e)
        {
            PreencherDataVencimento();
        }


        public void PreencherDataVencimento()
        {
            txtVencimentoLote.Text = string.Empty;
            ddlVencimentoLote.Visible = false;
            int idFornecedor = 0;

            idFornecedor = this.FornecedorId.Value;

            if (string.IsNullOrEmpty(txtSubItem.Text))
            {
                this.ListaErros = new List<string>() { "Selecionar o Subitem." };
                IdentificacaoLoteItem = string.Empty;
                txtVencimentoLote.Text = string.Empty;
                return;
            }

            txtSubItem.Enabled = false;
            rblLote.Enabled = false;
            imgSubItemMaterial.Visible = false;
            imgLupaFornecedor.Visible = false;

            if (!string.IsNullOrEmpty(IdentificacaoLoteItem))
            {
                if (this.FornecedorId <= 0)
                {
                    this.ListaErros = new List<string>() { "Selecionar o fornecedor." };
                    IdentificacaoLoteItem = string.Empty;
                    txtVencimentoLote.Text = string.Empty;
                    return;
                }


                txtVencimentoLote.Enabled = true;
            }
            else
            {
                txtSubItem.Enabled = true;
                rblLote.Enabled = true;
                imgSubItemMaterial.Visible = true;
                txtVencimentoLote.Enabled = false;
                if (movimento != null)
                    if (movimento.MovimentoItem != null)
                        imgLupaFornecedor.Visible = false;
                    else
                        imgLupaFornecedor.Visible = true;
                else
                    imgLupaFornecedor.Visible = true;
            }
        }

        protected void txtQtdeSubitemGridRepeater_OnTextChanged(object sender, EventArgs e)
        {
            btnGravar.Enabled = true;
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            TextBox txtQtdeLiqGrid = (TextBox)rp1.FindControl("txtQtdeLiqGrid");
            TextBox txtQtdeMovGrid = (TextBox)rp1.FindControl("txtQtdeMovGrid");
            Label lblPrecoUnitGridCalc = (Label)rp1.FindControl("lblPrecoUnitGridCalc");
            Label lblPrecoUnitGrid = (Label)rp1.FindControl("lblPrecoUnitGrid");

            txtQtdeLiqGrid.Text = string.IsNullOrWhiteSpace(txtQtdeLiqGrid.Text) ? "0,000" : txtQtdeLiqGrid.Text;
            txtQtdeMovGrid.Text = string.IsNullOrWhiteSpace(txtQtdeMovGrid.Text) ? "0,000" : txtQtdeMovGrid.Text;

            decimal QtdeLiqGrid = Decimal.Parse(txtQtdeLiqGrid.Text);
            decimal QtdeMovGrid = Decimal.Parse(txtQtdeMovGrid.Text);
            decimal PrecoUnitGrid = Decimal.Parse(lblPrecoUnitGrid.Text);




            decimal calculo = (QtdeMovGrid > 0 ? (QtdeLiqGrid * PrecoUnitGrid) / QtdeMovGrid : 0);

            lblPrecoUnitGridCalc.Text = calculo.truncarQuatroCasas().ToString();
            lblPrecoUnitGrid.Visible = false;
            lblPrecoUnitGridCalc.Visible = true;


            if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeMovGrid.Text == "0,000"))
            {
                //string strValorMinimo = Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString();
                string strValorMinimo = obterValorUnitarioMinimo();
                btnGravar.Enabled = false;
                this.ListaErros = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo de R$ {0} não são permitidas. Verifique qtde/preço.", strValorMinimo) };
                return;
            }


        }

        protected void txtIdLoteRepeater_OnTextChanged(object sender, EventArgs e)
        {
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            TextBox tbLote = (TextBox)rp1.FindControl("txtIdLote");
            DropDownList tbSubItem = (DropDownList)rp1.FindControl("ddlSubItemList");
            TextBox tbVencLote = (TextBox)rp1.FindControl("txtVctoLote");
            tbVencLote.Enabled = false;

            if (!string.IsNullOrEmpty(tbSubItem.SelectedValue))
            {
                if (!string.IsNullOrEmpty(tbLote.Text.Trim()))
                {
                    tbSubItem.Enabled = false;

                    tbVencLote.Text = string.Empty;
                    tbVencLote.Enabled = true;

                }
                else
                {
                    tbSubItem.Enabled = true;
                    tbVencLote.Text = string.Empty;
                    tbVencLote.Enabled = false;
                }
            }
            else
            {
                this.ListaErros = new List<string>() { "Selecionar o Subitem." };
                tbLote.Text = string.Empty;
                tbVencLote.Text = string.Empty;
                return;
            }
        }

        //public bool ValidarLoteVencimento()
        //{
        //    bool retorno = false;
        //    if (!string.IsNullOrEmpty(IdentificacaoLoteItem))
        //    {
        //        if (this.FornecedorId <= 0)
        //        {
        //            this.ListaErros = new List<string>() { "Selecionar o fornecedor." };
        //            IdentificacaoLoteItem = string.Empty;
        //            txtVencimentoLote.Text = string.Empty;
        //            retorno = true;
        //        }

        //        if (GetSession<MovimentoEntity>(sessaoMov) != null)
        //        {
        //            MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);

        //            foreach (var item in mov.MovimentoItem)
        //            {
        //                if (item.SubItemCodigo.HasValue)
        //                {
        //                    if ((item.IdentificacaoLote == IdentificacaoLoteItem) && (item.SubItemCodigo.Value.ToString() == txtSubItem.Text))
        //                    {
        //                        if (txtVencimentoLote.Text != item.DataVencimentoLote.Value.ToString(base.fmtDataFormatoBrasileiro))
        //                        {
        //                            this.ListaErros = new List<string>() { "Data Vencimento Inválida, a Data Vencimento deve ser igual do Lote cadastrado para este fornecedor e este Subitem " + item.DataVencimentoLote.Value.ToString(base.fmtDataFormatoBrasileiro) + "." };
        //                            retorno = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //var datalote = ListarMovimentoItensLote(IdentificacaoLoteItem, Convert.ToInt32(txtCodFornecedor.Text), this.SubItemMaterialId.Value);
        //            var datalote = ListarMovimentoItensLote(IdentificacaoLoteItem, this.FornecedorId.Value, this.SubItemMaterialId.Value);
        //            if (datalote.Count > 0)
        //            {
        //                if (txtVencimentoLote.Text != Convert.ToDateTime(datalote.FirstOrDefault().DataVencimentoLote.Value.ToShortDateString()).ToString(base.fmtDataFormatoBrasileiro))
        //                {
        //                    this.ListaErros = new List<string>() { "Data Vencimento Inválida, a Data Vencimento deve ser igual do Lote cadastrado para este fornecedor e este Subitem " + Convert.ToDateTime(datalote.FirstOrDefault().DataVencimentoLote.Value.ToShortDateString()).ToString(base.fmtDataFormatoBrasileiro) + "." };
        //                    retorno = true;
        //                }
        //            }
        //        }
        //    }

        //    return retorno;
        //}

        public void AtivarBotaoFornecedor()
        {
            if (rblTipoOperacao.SelectedValue == "1")
            {
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);

                    if (mov.MovimentoItem != null)
                    {
                        imgLupaFornecedor.Visible = false;
                    }
                    else
                    {
                        imgLupaFornecedor.Visible = true;
                    }
                }
                else
                {
                    imgLupaFornecedor.Visible = true;
                }
            }
        }

        protected void imgSubItemMaterial_Click(object sender, ImageClickEventArgs e)
        {
            Session["PesquisaSubitemRequisicao"] = false;
        }

        protected void chkLiquidarEmpenhoBEC_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox thisCheck = ((CheckBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(thisCheck.NamingContainer));
            TextBox txtQtdeLiqGrid = (TextBox)rp1.FindControl("txtQtdeLiqGrid");
            TextBox txtQtdeMovGrid = (TextBox)rp1.FindControl("txtQtdeMovGrid");
            Label lblPrecoUnitGridCalc = (Label)rp1.FindControl("lblPrecoUnitGridCalc");
            Label lblPrecoUnitGrid = (Label)rp1.FindControl("lblPrecoUnitGrid");
           // System.Web.UI.HtmlControls.HtmlTableRow trEmpenhoLoteNovo = (System.Web.UI.HtmlControls.HtmlTableRow)rp1.FindControl("trEmpenhoLoteNovo");
            System.Web.UI.HtmlControls.HtmlTable tbEmpenhoLoteNovo = (System.Web.UI.HtmlControls.HtmlTable)rp1.FindControl("tbEmpenhoLoteNovo");


            txtQtdeLiqGrid.Text = string.IsNullOrWhiteSpace(txtQtdeLiqGrid.Text) ? "0,000" : txtQtdeLiqGrid.Text;
            txtQtdeMovGrid.Text = string.IsNullOrWhiteSpace(txtQtdeMovGrid.Text) ? "0,000" : txtQtdeMovGrid.Text;

            decimal QtdeLiqGrid = Decimal.Parse(txtQtdeLiqGrid.Text);

            decimal QtdeMovGrid = 0;

            if (thisCheck.Checked)
            {
                QtdeMovGrid = Decimal.Parse(txtQtdeLiqGrid.Text);
               // trEmpenhoLoteNovo.Visible = false;
                tbEmpenhoLoteNovo.Visible = false;
            }
            else
            {
                QtdeMovGrid = 0;
                //trEmpenhoLoteNovo.Visible = false;
                tbEmpenhoLoteNovo.Visible = false;
            }

            txtQtdeMovGrid.Text = QtdeMovGrid.ToString();

            decimal PrecoUnitGrid = Decimal.Parse(lblPrecoUnitGrid.Text);

            decimal calculo = (QtdeMovGrid > 0 ? (QtdeLiqGrid * PrecoUnitGrid) / QtdeMovGrid : 0);

            lblPrecoUnitGridCalc.Text = calculo.truncarQuatroCasas().ToString();
            lblPrecoUnitGrid.Visible = false;
            lblPrecoUnitGridCalc.Visible = true;

            if (PrecoUnitarioZeroEMilesimal(Convert.ToDecimal(lblPrecoUnitGridCalc.Text)) && !(txtQtdeLiqGrid.Text == "0,000" && txtQtdeMovGrid.Text == "0,000"))
            {
                //string strValorMinimo = Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString();
                string strValorMinimo = obterValorUnitarioMinimo();
                this.ListaErros = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo de R$ {0} não são permitidas. Verifique qtde/preço.", strValorMinimo) };
                return;
            }
        }



        private string obterValorUnitarioMinimo()
        {
            return Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString();
        }

        public string InscricaoCE
        {
            get { return txtInfoSiafemCE.Text; }
            set { txtInfoSiafemCE.Text = value; }
        }

        public string InscricaoCEOld
        {
            get { return hdfInfoSiafemCEOld.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    hdfInfoSiafemCEOld.Text = "CE Original:" + value;
            }
        }

        public bool ModoGravacaoOuEstorno
        {
            get { return ((TipoOperacao == (int)tipoOperacao.Nova) || (TipoOperacao == (int)tipoOperacao.Estorno)); }
        }

        public class LoteEmpenho
        {
            private int indexMovItem;
            private int index;
            private decimal liqSiafisico;
            private decimal movEstoque;
            private string loteItem;
            private DateTime? vctoLoteItem;

            public LoteEmpenho(int indexMovItem, int index, decimal liqSiafisico, decimal movEstoque, string loteItem, DateTime? vctoLoteItem)
            {
                this.indexMovItem = indexMovItem;
                this.index = index;
                this.liqSiafisico = liqSiafisico;
                this.movEstoque = movEstoque;
                this.loteItem = loteItem;
                this.vctoLoteItem = vctoLoteItem;
            }

            public int IndexMovItem
            {
                get
                {
                    return indexMovItem;
                }
            }



            public int Index
            {
                get
                {
                    return index;
                }
            }


            public decimal LiqSiafisico
            {
                get
                {
                    return liqSiafisico;
                }
            }

            public decimal MovEstoque
            {
                get
                {
                    return movEstoque;
                }
            }
            public string LoteItem
            {
                get
                {
                    return loteItem;
                }
            }
            public DateTime? VctoLoteItem
            {
                get
                {
                    return vctoLoteItem;
                }
            }
        }

        protected void txtLoteItemRepeater_OnTextChanged(object sender, EventArgs e)
        {
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            DropDownList tbSubItem = (DropDownList)rp1.FindControl("ddlSubItemList");
            Label Posicao = (Label)rp1.FindControl("lblPosicao");

            int indexMovItem = Convert.ToInt32(Posicao.Text) - 1;

            Repeater rptItemLote = (Repeater)gridSubItemMaterial.Items[indexMovItem].FindControl("rptItemLote");
            TextBox tbLote = (TextBox)gridSubItemMaterial.Items[indexMovItem].FindControl("txtLoteItem");
            TextBox tbVencLote = (TextBox)gridSubItemMaterial.Items[indexMovItem].FindControl("txtVctoLoteItem");

            tbVencLote.Enabled = false;

            if (!string.IsNullOrEmpty(tbSubItem.SelectedValue))
            {
                if (!string.IsNullOrEmpty(tbLote.Text.Trim()))
                {
                    tbSubItem.Enabled = false;
                    tbVencLote.Text = string.Empty;
                    tbVencLote.Enabled = true;
                }
                else
                {
                    tbSubItem.Enabled = true;
                    tbVencLote.Text = string.Empty;
                    tbVencLote.Enabled = false;
                }
            }
            else
            {
                this.ListaErros = new List<string>() { "Selecionar o Subitem." };
                tbLote.Text = string.Empty;
                tbVencLote.Text = string.Empty;
                return;
            }
        }



        protected void txtLiqSiafisico_OnTextChanged(object sender, EventArgs e)
        {
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            Repeater rptItemLote = (Repeater)rp1.FindControl("rptItemLote");
            Label Posicao = (Label)rp1.FindControl("lblPosicao");
            TextBox txtMovEstoque = (TextBox)rp1.FindControl("txtMovEstoque");

            if (rptItemLote.Items.Count > 0)
            {
                TextBox txtLiqSiafisico = (TextBox)rp1.FindControl("txtLiqSiafisico");
                decimal lblLiqSiafisico = Convert.ToDecimal(((Label)rptItemLote.Items[0].FindControl("lblLiqSiafisico")).Text);
                decimal lblMovEstoque = Convert.ToDecimal(((Label)rptItemLote.Items[0].FindControl("lblMovEstoque")).Text);
                decimal LiqSiafisico = Convert.ToDecimal(txtLiqSiafisico.Text);
                decimal MovEstoque = 0;

                MovEstoque = (lblMovEstoque * LiqSiafisico) / lblLiqSiafisico;
                txtMovEstoque.Text = MovEstoque.Truncar(3, true).ToString(base.fmtFracionarioMaterialQtde);
                txtMovEstoque.Enabled = false;

            }
            else
                txtMovEstoque.Enabled = true;

        }

        protected void txtValorItemEmpenho_OnTextChanged(object sender, EventArgs e)
        {
            TextBox tb1 = ((TextBox)(sender));
            RepeaterItem rp1 = ((RepeaterItem)(tb1.NamingContainer));
            Repeater rptItemLote = (Repeater)rp1.FindControl("rptItemLote");
            Label Posicao = (Label)rp1.FindControl("lblPosicao");
            Label lblValorUnitarioItemEmpenho = (Label)rp1.FindControl("lblValorUnit");
            TextBox txtValorItemEmpenho = (TextBox)rp1.FindControl("txtValorItemEmpenho");
            TextBox txtValorTotal = ((TextBox)this.Page.FindControlRecursive("txtValorTotal"));
            decimal valorItemEmpenhoDigitado = -1.00m;
            decimal valorUnitarioItemEmpenho = -1.00m;
            decimal valortotalEmpenho = -1.00m;

            valorUnitarioItemEmpenho = Decimal.Parse(lblValorUnitarioItemEmpenho.Text);
            valorItemEmpenhoDigitado = Decimal.Parse(txtValorItemEmpenho.Text);

            //if ((valorItemEmpenhoDigitado != 0.00m) && (valorItemEmpenhoDigitado < valorUnitarioItemEmpenho))
            //{
            //    ExibirMensagem(String.Format("Valor a liquidar para este subitem tem de ser maior ou igual a R$ {0:#,##0.00}!", valorUnitarioItemEmpenho.ToString("#,##0.00")));
            //    txtValorItemEmpenho.Focus();
            //    return;
            //}

            txtValorItemEmpenho.Text = valorItemEmpenhoDigitado.ToString(fmtValorFinanceiro);
            txtValorItemEmpenho.Enabled = true;


            valortotalEmpenho = Decimal.Parse(txtValorTotal.Text);


            if (valorItemEmpenhoDigitado > valortotalEmpenho)
            {
                ExibirMensagem("Valor a liquidar digitado para este subitem é maior do que o valor total do empenho!");
                return;
            }

            ((TextBox)sender).Style.Add("text-align", "center");
        }

        public void PreencherListaPTResAcao(DropDownList drop, DropDownList dropItemMaterial)
        {

            //DropDownList drop = new DropDownList();
            //DropDownList dropItemMaterial = new DropDownList();
            drop.InserirElementoSelecione(true);
            dropItemMaterial.InserirElementoSelecione(true);


        //    var ugeCodigo = Int32.Parse(ddlUGE.SelectedItem.Text.BreakLine('-', 0).Trim());

            var ugeCodigo = (ddlUGE.SelectedItem.Text.BreakLine('-', 0)=="Todos" ?0:Int32.Parse(ddlUGE.SelectedItem.Text.BreakLine('-', 0).Trim()));

            PTResPresenter objPresenter = new PTResPresenter();
            var lstPTResAcao = objPresenter.CarregarListaPTResAcao(ugeCodigo);
            PageBase.SetSession<IList<PTResEntity>>(lstPTResAcao, cstListagemPTResAcao);

            if (lstPTResAcao.IsNotNullAndNotEmpty())
            {
                lstPTResAcao.ToList()
                    .OrderBy(f=>f.Codigo).ToList()
                    .ForEach(_ptRes => dropItemMaterial.Items.Add(new ListItem(_ptRes.Codigo.ToString(), _ptRes.Id.ToString())));

                lstPTResAcao.DistinctBy(a => a.ProgramaTrabalho.ProjetoAtividade).ToList()
                        .ForEach(_ptRes => drop.Items.Add(new ListItem(_ptRes.ProgramaTrabalho.ProjetoAtividade, _ptRes.ProgramaTrabalho.ProjetoAtividade)));
            }

        }

        protected void ddlPTResItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPTResItemMaterial.SelectedValue != "0")
            {
                var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);

                var _ptresVinculado = listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                            && !String.IsNullOrWhiteSpace(ptres.ProgramaTrabalho.ProjetoAtividade)
                                                            && ptres.Codigo.ToString() == ddlPTResItemMaterial.SelectedItem.Text)
                                           .FirstOrDefault();

                if (_ptresVinculado != null)
                {
                    ddlPTResAcao.SelectedValue = ((_ptresVinculado.ProgramaTrabalho.IsNotNull()) ? _ptresVinculado.ProgramaTrabalho.ProjetoAtividade : "0");
                    ddlPTResItemMaterial.SelectedValue = _ptresVinculado.Id.ToString();
                    txtDescricaoPTRes.Text = _ptresVinculado.Descricao;
                }


                AtualizaInscricaoCE();
            }
            else
            {
                ddlPTResAcao.SelectedValue = "0";
                txtDescricaoPTRes.Text = string.Empty;
            }

        }

        protected void ddlUA_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaInscricaoCE();


            GetSessao();
            movimento.UA = new UAEntity() { Id = this.UaId, Codigo = this.CodigoUA, Uge = new UGEEntity() { Id = this.UgeId, Codigo = this.CodigoUGE } };

            SetSession<MovimentoEntity>(movimento, sessaoMov);
        }

        private void AtualizaInscricaoCE()
        {
            string inscricaoEventoConsumoImediato = null;
            string codigoUGE = null;
            string codigoUA = null;
            string codigoPTRes = null;


            codigoUGE = ((this.CodigoUGE > 0) ? this.CodigoUGE.ToString("D6") : null);
            codigoUA = ((this.CodigoUA > 0) ? this.CodigoUA.ToString("D6") : null);
            codigoPTRes = ((this.PTResCodigo.HasValue && this.PTResCodigo.Value > 0) ? this.PTResCodigo.Value.ToString("D6") : null);

            inscricaoEventoConsumoImediato = String.Format("{0}{1}{2}", codigoUGE, codigoUA, codigoPTRes);
            InscricaoCE = inscricaoEventoConsumoImediato;


            GetSessao();

            if (movimento.IsNotNull())
            {
                movimento.InscricaoCE = this.InscricaoCE;
                movimento.UA = new UAEntity { Id = this.UaId, Codigo = this.CodigoUA, Uge = new UGEEntity { Id = this.UgeId, Codigo = this.CodigoUGE } };
            }
        }

        protected void ddlPTResAcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPTResAcao.SelectedValue != "0")
            {
                var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
                var _ptresAcao = listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                        && ptres.ProgramaTrabalho.ProjetoAtividade == ddlPTResAcao.SelectedItem.Text)
                                           .FirstOrDefault();

                if (_ptresAcao != null)
                {
                    ddlPTResAcao.SelectedValue = ((_ptresAcao.ProgramaTrabalho.IsNotNull()) ? _ptresAcao.ProgramaTrabalho.ProjetoAtividade : "0");
                    ddlPTResItemMaterial.SelectedValue = _ptresAcao.Id.ToString();
                    txtDescricaoPTRes.Text = _ptresAcao.Descricao;
                }

                AtualizaInscricaoCE();
            }
            else
            {
                ddlPTResItemMaterial.SelectedValue = "0";
                txtDescricaoPTRes.Text = string.Empty;
            }

            AtualizaInscricaoCE();
        }


    }
}
