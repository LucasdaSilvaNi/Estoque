using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.Services;

namespace Sam.Web.Almoxarifado
{
    public partial class ConsultarRequisicaoMaterial : PageBase, IRequisicaoMaterialView, ICombosPadroesView
    {
        #region Propriedades

        private MovimentoEntity movimento = new MovimentoEntity();
        private string sessionListMovimento = "listMovimento";
        private string movimentoIdEdit = "movimentoIdEdit";
        public string exibirMensagemConfirmacao = "exibirMensagem";
        public string Observacao
        {
            get
            {
                return string.Empty;
            }
            set
            { }
        }

        #region IEntidadesAuxiliaresView Members

        public int OrgaoId
        {
            get
            {
                if (ddlOrgao.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.ddlOrgao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.ddlOrgao.SelectedValue = value.ToString();
            }
        }

        public int UOId
        {
            get
            {
                if (this.ddlUO.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.ddlUO.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.ddlUO.SelectedValue = value.ToString();
            }
        }

        public int UGEId
        {
            get
            {
                if (this.ddlUGE.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.ddlUGE.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.ddlUGE.SelectedValue = value.ToString();
            }
        }

        public int UAId
        {
            set
            {
                if (value > 0)
                    this.ddlUA.SelectedValue = value.ToString();
            }
            get
            {
                if (this.ddlUA.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.ddlUA.SelectedValue);
                else
                    return 0;
            }
        }

        public void CarregaSessao()
        {
            bool carregarSessao = false;
            if (movimento == null)
                carregarSessao = true;
            else
            {
                if (movimento.MovimentoItem == null)
                    carregarSessao = true;
            }

            if (carregarSessao == true)
                movimento = GetSession<MovimentoEntity>(sessionListMovimento);
        }

        public void PopularGrid()
        {

        }

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

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
            }
        }

        public bool BloqueiaNovo
        {
            set
            { }
        }

        public bool VisivelEditar
        {
            set
            {

            }
        }

        public bool VisivelAdicionar
        {
            set
            {

            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                //O controle é feito manualmente
                //btnGravar.Enabled = value;
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {

            }
        }

        public bool BloqueiaCancelar
        {
            set
            {

            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {

            }
        }

        public bool MostrarPainelRequisicao
        {
            set
            {

            }

        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public void InserirMensagemEmSessao(string chaveSessao, string msgSessao)
        {
            throw new NotImplementedException();
        }

        public void InserirMensagemCancelada(string chaveSessao, string msgSessao)
        {
            throw new NotImplementedException();
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public bool OcultaOrgao
        {
            set
            {
                this.OcultaOrgao = value;
            }
        }

        public bool OcultaUO
        {
            set
            {
                this.OcultaUO = value;
            }
        }

        public bool OcultaUGE
        {
            set
            {
                this.OcultaUGE = value;
            }
        }

        #endregion


        public int DivisaoId
        {
            get
            {
                if (this.ddlDivisao.Items.Count > 0)
                {
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.ddlDivisao.SelectedValue);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value > 0)
                    this.ddlDivisao.SelectedValue = value.ToString();
            }
        }

        public int StatusId
        {
            get
            {
                if (this.ddlStatus.Items.Count > 0)
                {
                    string statusSelected = this.ddlStatus.SelectedIndex > 0 ?
                                                this.ddlStatus.SelectedValue :
                                                "0";
                    return (int)Common.Util.TratamentoDados.TryParseInt32(statusSelected);
                }
                else
                    return 0;
            }
            set
            {
                this.ddlStatus.SelectedValue = value.ToString();
            }
        }

        public int? PTResId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int? PTResCodigo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string PTResAcao
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string PTAssociadoPTRes
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public decimal Quantidade
        {
            get { return 0.000m; }
            set { }
        }

        public string PTResDescricao
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Descricao
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public string Instrucao
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public bool BloqueiaSubItem
        {
            set
            {

            }
        }

        public bool BloqueiaDescricao
        {
            set
            {

            }
        }

        public bool BloqueiaQuantidade
        {
            set
            {

            }
        }

        public bool BloqueiaObservacoes
        {
            set
            {

            }
        }

        public bool BloqueiaInstrucoes
        {
            set
            {

            }
        }

        public bool BloqueiaListaOrgao
        {
            set { this.BloqueiaListaOrgao = value; }
            //set { this.BloqueiaOrgao = value; }
        }

        public bool BloqueiaListaUO
        {
            set { this.BloqueiaListaUO = value; }
            //set { this.BloqueiaUO = value; }
        }

        public bool BloqueiaListaUGE
        {
            //set { this.BloqueiaUGE = value; }
            set { this.BloqueiaListaUGE = value; }
        }

        public bool BloqueiaListaUA
        {
            //set { this.BloqueiaUA = value; }
            set { this.BloqueiaListaUA = value; }
        }

        public bool BloqueiaListaDivisao
        {
            //set { this.BloqueiaDivisao = value; }
            set { this.BloqueiaListaDivisao = value; }
        }


        public SortedList paramList;
        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                return paramList;
            }

            set
            {
                paramList = value;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        protected bool ElementIfTrue(int tipoMovimentoID)
        {
            if (tipoMovimentoID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                return true;
            else
                return false;
        }

        public string Codigo
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public bool BloqueiaCodigo
        {
            set
            {

            }
        }

        public int SubItemId
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public string UnidadeFornecimentoDescricao
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }



        private bool _cascatearDDLOrgao = true;
        public bool CascatearDDLOrgao
        {
            get
            {
                return _cascatearDDLOrgao;
            }
            set
            {
                _cascatearDDLOrgao = value;
            }
        }

        private bool _cascatearDDLUO = true;
        public bool CascatearDDLUO
        {
            get
            {
                return _cascatearDDLUO;
            }
            set
            {
                _cascatearDDLUO = value;
            }
        }

        private bool _cascatearDDLUGE = true;
        public bool CascatearDDLUGE
        {
            get
            {
                return _cascatearDDLUGE;
            }
            set
            {
                _cascatearDDLUGE = value;
            }
        }

        private bool _cascatearDDLUA = true;
        public bool CascatearDDLUA
        {
            get
            {
                return _cascatearDDLUA;
            }
            set
            {
                _cascatearDDLUA = value;
            }
        }

        private bool _cascatearDDLAlmoxarifado = true;
        public bool CascatearDDLAlmoxarifado
        {
            get { return _cascatearDDLAlmoxarifado; }
            set { _cascatearDDLAlmoxarifado = value; }
        }

        public bool PreservarComboboxValues
        {
            get
            {
                return this.PreservarComboboxValues;
            }
            set
            {
                this.PreservarComboboxValues = value;
            }
        }

        public int AlmoxarifadoId
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string NumeroDocumento
        {
            get
            {
                return this.txtNumRequisicao.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //private bool _resetarStartRowIndex;
        public bool ResetarStartRowIndex
        {
            get
            {
                bool result = false;

                if (ViewState["resetarViewStateKey"].IsNotNull())
                    result = Convert.ToBoolean(ViewState["resetarViewStateKey"]);

                return result;
            }
            set { ViewState["resetarViewStateKey"] = value; }
        }



        #endregion

        int _TotalRegistro;
        #region Controles ASPX

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.CombosHierarquiaPadrao1.View = this;

            if (!String.IsNullOrWhiteSpace(GetSession<string>("_transPage")))
            {
                ExibirMensagem(GetSession<string>("_transPage"));
                RemoveSession("_transPage");
            }
            else if (!String.IsNullOrWhiteSpace(GetSession<string>("_transPageNumDocRequisicao")))
            {
                ExibirMensagem(GetSession<string>("_transPageNumDocRequisicao"));
                RemoveSession("_transPageNumDocRequisicao");
            }

            Master.FindControl("pnlBarraGestor").Visible = true;
            RemoveSession(movimentoIdEdit);
            RemoveSession(sessionListMovimento);

            //RemoveSession(this.CombosHierarquiaPadrao1.sessionListUGEEntity);

            if (!IsPostBack)
            {
                this.PopularOrgao();
                this.PopularUO();
                this.PopularUGE();
                this.PopularUA();
                this.PopularDivisao();
                this.PopularStatus();
                //if (Page.Session["orgaoId"].IsNotNull())
                //    CarregaViewComDDLSelectedValues();

                //RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
                //requisicao.Load();
            }

            //PopularGrid();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Editar();
            movimento = requisicao.EditSubItem(movimento);
            requisicao.EditarSucesso();
            PopularGrid();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Imprimir();
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {
        }

        protected void btnNovo_Click1(object sender, EventArgs e)
        {
            RemoveSession(movimentoIdEdit);
            RemoveSession(sessionListMovimento);
            //RemoveSession(this.CombosHierarquiaPadrao1.sessionListUGEEntity);

            PopulaSessionsComDDLSelectedValues();
            Response.Redirect("RequisicaoMaterial.aspx", false);
        }

        protected void dialog_link_Click(object sender, ImageClickEventArgs e)
        { }

        protected void linkCodigo_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                string movimentoId = e.CommandArgument.ToString().Trim();

                if (!string.IsNullOrEmpty(movimentoId))
                {
                    SetSession<string>(movimentoId, movimentoIdEdit);

                    //Quando em modo edição não por via DDL (numero de requisição) popular as sessions com os valores do MOVIMENTO.
                    PopulaSessionsModoEdicao(Convert.ToInt32(movimentoId));
                    Response.Redirect("RequisicaoMaterial.aspx", false);
                }
            }

        }

        protected void grdDocumentos_RowDataBound(object sender, GridViewRowEventArgs e)
        { }

        protected void gridRequisicao_SelectedIndexChanged(object sender, EventArgs e)
        { }

        protected void grdDocumentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ImprimirRequisicao")
            {
                int movimentoId = int.Parse(e.CommandArgument.ToString());
                this.Id = movimentoId.ToString();

                this.ImprimirNotaRequisicao(movimentoId);
            }
            else if (e.CommandName == "ImprimirSaida")
            {

                string[] arg = new string[2];
                arg = e.CommandArgument.ToString().Split(';');
                string numeroDocumento = arg[0];
                string divisaoId = arg[1];                

                //string numeroDocumento = Convert.ToString(e.CommandArgument.ToString());
                this.ImprimirNotaSaida(numeroDocumento, divisaoId);
            }
        }

        /// <summary>
        /// Método sendo acionado no changed do userControl, populando o ViewState para informar a presenter. 
        /// Isso porque apenas o botão pesquisar que de fato deve resetar o pageIndex. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void ddlStatus_Changed(object sender, EventArgs e)
        //{
        //    if (grdDocumentos.PageIndex >= 0)
        //    {
        //        ResetarStartRowIndex = true;
        //    }
        //}

        public int TotalRegistros(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName)
        {
            return this._TotalRegistro;
        }

        protected void sourceGridRequisicoes_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            var presenter = new RequisicaoMaterialPresenter(this);
            //presenter.SetTipoDePesquisaEventoGridview_Selecting(this.ddlDivisao.SelectedValue, this.ddlStatus, this.TextNumeroRequisicao, e);
            presenter.SetTipoDePesquisaEventoGridview_Selecting(Convert.ToInt32(this.ddlUO.SelectedValue)
                                                                    , Convert.ToInt32(this.ddlUGE.SelectedValue)
                                                                    , Convert.ToInt32(this.ddlUA.SelectedValue)
                                                                    , Convert.ToInt32(this.ddlDivisao.SelectedValue)
                                                                    , this.ddlStatus
                                                                    , this.txtNumRequisicao.Text
                                                                    , e);
        }

        protected void grdDocumentos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdDocumentos.PageIndex = e.NewPageIndex;
            grdDocumentos.DataSourceID = "requisicao";
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (GetAcesso.Transacoes.Perfis[0].IdPerfil == (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado && ddlUGE.SelectedValue == "0")
            {
                ExibirMensagem("Obrigatória a seleção da UGE");
            }
            else if (!String.IsNullOrEmpty(this.ddlDivisao.Items[0].Text))
            {
                string strOrgao = "";
                string strUO = "";
                string strUGE = "";
                string strUA = "";
                string strDivisao = "";

                if (Convert.ToInt32(this.ddlOrgao.SelectedValue) == 0 && this.ddlOrgao.Items.Count > 0)
                    for (int itemOrgao = 1; itemOrgao < this.ddlOrgao.Items.Count; itemOrgao++)
                    {
                        strOrgao = strOrgao + Convert.ToString(this.ddlOrgao.Items[itemOrgao].Value);
                        strOrgao += ",";

                    }
                else
                    strOrgao = Convert.ToString(this.ddlOrgao.SelectedValue);

                if (Convert.ToInt32(this.ddlUO.SelectedValue) == 0 && this.ddlUO.Items.Count > 0)
                    for (int itemUO = 1; itemUO < this.ddlUO.Items.Count; itemUO++)
                    {
                        strUO = strUO + Convert.ToString(this.ddlUO.Items[itemUO].Value);
                        strUO += ",";

                    }
                else
                    strUO = Convert.ToString(this.ddlUO.SelectedValue);

                if (Convert.ToInt32(this.ddlUGE.SelectedValue) == 0 && this.ddlUGE.Items.Count > 0)
                    for (int itemUGE = 1; itemUGE < this.ddlUGE.Items.Count; itemUGE++)
                    {
                        strUGE = strUGE + Convert.ToString(this.ddlUGE.Items[itemUGE].Value);
                        strUGE += ",";
                    }
                else
                    strUGE = Convert.ToString(this.ddlUGE.SelectedValue);



                if (Convert.ToInt32(this.ddlUA.SelectedValue) == 0 && this.ddlUA.Items.Count > 0)
                    for (int itemUA = 1; itemUA < this.ddlUA.Items.Count; itemUA++)
                    {
                        strUA = strUA + Convert.ToString(this.ddlUA.Items[itemUA].Value);
                        strUA += ",";
                    }
                else
                    strUA = Convert.ToString(this.ddlUA.SelectedValue);



                if (Convert.ToInt32(this.ddlDivisao.SelectedValue) == 0 && this.ddlDivisao.Items.Count > 0)
                    for (int itemDivisao = 1; itemDivisao < this.ddlDivisao.Items.Count; itemDivisao++)
                    {
                        strDivisao = strDivisao + Convert.ToString(this.ddlDivisao.Items[itemDivisao].Value);
                        strDivisao += ",";
                    }
                else 
                    strDivisao = Convert.ToString(this.ddlDivisao.SelectedValue);


                Session.Add("orgaoId", strOrgao);
                Session.Add("uoId", strUO);
                Session.Add("ugeId", strUGE);
                Session.Add("uaId", strUA);
                Session.Add("divisaoId", strDivisao);
                Session.Add("statusId", Convert.ToInt32(this.ddlStatus.SelectedValue));
                Session.Add("numeroDocumento", Convert.ToString(this.txtNumRequisicao.Text.Trim()));
                grdDocumentos.DataSourceID = "requisicao";
                grdDocumentos.Visible = true;
            }
            else
                ExibirMensagem("Essa UA não possui divisões, por isso não é permitido a pesquisa ou inclusão de requisições");
        }

        #endregion


        #region Métodos

        #region Popular combos
        public void PopularOrgao()
        {
            CombosPadroesPresenter presenter = new CombosPadroesPresenter();
            IList<OrgaoEntity> listaOrgao = presenter.PopularOrgao();

            this.ddlOrgao.Items.Clear();
            this.ddlOrgao.Items.Insert(0, new ListItem("Todos", "0"));

            if (listaOrgao.Count > 0)
            {
                this.ddlOrgao.Items.Clear();
                this.ddlOrgao.DataValueField = "Id";
                this.ddlOrgao.DataTextField = "CodigoDescricao";
                this.ddlOrgao.DataSource = listaOrgao;
                this.ddlOrgao.DataBind();
            }

            if (ddlOrgao.Items.Count > 1 && presenter.Perfil == (int)GeralEnum.TipoPerfil.AdministradorGeral)
            {
                this.ddlOrgao.Items.Insert(0, new ListItem("Todos", "0"));
                this.ddlOrgao.SelectedIndex = 0;
            }




        }
        public void PopularUO()
        {
            int orgaoSelecionado = 0;
            if (!String.IsNullOrEmpty(ddlOrgao.SelectedValue))
                orgaoSelecionado = Convert.ToInt32(ddlOrgao.SelectedValue);

            this.ddlUO.Items.Clear();
            this.ddlUO.Items.Insert(0, new ListItem("Todos", "0"));
            if (orgaoSelecionado != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UOEntity> listaUo = presenter.PopularUO(orgaoSelecionado);

                if (listaUo.Count > 0)
                {
                    this.ddlUO.Items.Clear();
                    this.ddlUO.DataValueField = "Id";
                    this.ddlUO.DataTextField = "CodigoDescricao";
                    this.ddlUO.DataSource = listaUo;
                    this.ddlUO.DataBind();
                    if (ddlUO.Items.Count > 1)
                    {
                        this.ddlUO.Items.Insert(0, new ListItem("Todos", "0"));
                        this.ddlUO.SelectedIndex = 0;
                    }
                }
            }
        }
        public void PopularUGE()
        {
            int uoSelecionado = 0;
            if (!String.IsNullOrEmpty(ddlUO.SelectedValue))
                uoSelecionado = Convert.ToInt32(ddlUO.SelectedValue);

            this.ddlUGE.Items.Clear();
            this.ddlUGE.Items.Insert(0, new ListItem("Todos", "0"));
            if (uoSelecionado != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UGEEntity> listaUge = presenter.PopularUGE(uoSelecionado);

                if (listaUge.Count > 0)
                {
                    this.ddlUGE.Items.Clear();
                    this.ddlUGE.DataValueField = "Id";
                    this.ddlUGE.DataTextField = "CodigoDescricao";
                    this.ddlUGE.DataSource = listaUge;
                    this.ddlUGE.DataBind();
                    if (ddlUGE.Items.Count > 1)
                    {
                        this.ddlUGE.Items.Insert(0, new ListItem("Todos", "0"));
                        this.ddlUGE.SelectedIndex = 0;
                    }
                }

            }
        }
        public void PopularUA()
        {
            int ugeSelecionado = 0;
            if (!String.IsNullOrEmpty(ddlUGE.SelectedValue))
                ugeSelecionado = Convert.ToInt32(ddlUGE.SelectedValue);

            this.ddlUA.Items.Clear();
            this.ddlUA.Items.Insert(0, new ListItem("Todos", "0"));

            if (ugeSelecionado != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                IList<UAEntity> listaUa = presenter.PopularUA(ugeSelecionado);
                listaUa = listaUa.Where(a => a.IndicadorAtividade == true).ToList();


                if (listaUa.Count > 0)
                {
                    this.ddlUA.Items.Clear();
                    this.ddlUA.DataValueField = "Id";
                    this.ddlUA.DataTextField = "CodigoDescricao";
                    this.ddlUA.DataSource = listaUa;
                    this.ddlUA.DataBind();
                    if (ddlUA.Items.Count > 1)
                    {
                        this.ddlUA.Items.Insert(0, new ListItem("Todos", "0"));
                        this.ddlUA.SelectedIndex = 0;
                    }
                }
            }
        }
        public void PopularDivisao()
        {
            int uaSelecionado = 0;
            if (!String.IsNullOrEmpty(ddlUA.SelectedValue))
                uaSelecionado = Convert.ToInt32(ddlUA.SelectedValue);

            this.ddlDivisao.Items.Clear();
            this.ddlDivisao.Items.Insert(0, new ListItem("Todos", "0"));

            if (uaSelecionado != 0)
            {
                CombosPadroesPresenter presenter = new CombosPadroesPresenter();
                //IList<DivisaoEntity> listaDivisao = presenter.PopularDivisao(int.Parse(ddlOrgao.SelectedValue), uaSelecionado);
                IList<DivisaoEntity> listaDivisao = presenter.PopularDivisao(uaSelecionado);
                listaDivisao = listaDivisao.Where(a => a.IndicadorAtividade == true).ToList();

                if (listaDivisao.Count > 0)
                {
                    this.ddlDivisao.Items.Clear();
                    this.ddlDivisao.DataValueField = "Id";
                    this.ddlDivisao.DataTextField = "CodigoDescricao";
                    this.ddlDivisao.DataSource = listaDivisao;
                    this.ddlDivisao.DataBind();
                    if (ddlDivisao.Items.Count > 1)
                    {
                        this.ddlDivisao.Items.Insert(0, new ListItem("Todos", "0"));
                        this.ddlDivisao.SelectedIndex = 0;
                    }
                }
            }
        }
        public void PopularStatus()
        {
            CombosPadroesPresenter presenter = new CombosPadroesPresenter();

            this.ddlStatus.Items.Clear();
            this.ddlStatus.DataValueField = "Id";
            this.ddlStatus.DataTextField = "Descricao";
            this.ddlStatus.DataSource = presenter.PopularComboStatusRequisicao();
            this.ddlStatus.DataBind();
            this.ddlStatus.Items.Insert(0, new ListItem("Todos", "0"));

        }
        #endregion

        private void ImprimirNotaRequisicao(int movimentoId)
        {
            RequisicaoMaterialPresenter presenter = new RequisicaoMaterialPresenter(this);
            TB_MOVIMENTO movimento = presenter.SelectOneMovimento(movimentoId);
            string erroMessage = "Erro ao executar o relatório: {0}";

            SortedList paramList = new SortedList();

            if (movimentoId != 0)
                paramList.Add("movimentoId", movimentoId.ToString());


            //paramList.Add("solicitadoPor", new PageBase().GetAcesso.NomeUsuario);
            //paramList.Add("cfp", new PageBase().GetAcesso.Cpf);
            //Seleciona Usuário que fez a Requisição
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
            var usuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(movimento.TB_LOGIN_ID.Value);
            paramList.Add("solicitadoPor", usuario.NomeUsuario);
            paramList.Add("cfp", usuario.Cpf);


            if (movimento != null)
                paramList.Add("Situacao", RetornaToolTipStatusRequisicao(movimento.TB_TIPO_MOVIMENTO_ID).Replace("Requisição", "").ToUpper());

            paramList.Add("AlmoxarifadoDescricao", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao);

            if (movimento.TB_DIVISAO.TB_UA != null)
            {

                paramList.Add("UaDescricao", String.Format("{0} - {1}", movimento.TB_DIVISAO.TB_UA.TB_UA_CODIGO, movimento.TB_DIVISAO.TB_UA.TB_UA_DESCRICAO));
                paramList.Add("UaRequisitante", movimento.TB_DIVISAO.TB_DIVISAO_DESCRICAO);

                if (movimento.TB_DIVISAO.TB_RESPONSAVEL == null)
                    paramList.Add("Contato", string.Empty);
                else if (String.IsNullOrEmpty(movimento.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME))
                    paramList.Add("Contato", string.Empty);
                else
                    paramList.Add("Contato", movimento.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME);
            }
            else
            {
                paramList.Add("UaDescricao", "");
                paramList.Add("UaRequisitante", "");
                paramList.Add("Contato", "");
            }

            if (movimento.TB_DIVISAO != null)
            {
                paramList.Add("Telefone", movimento.TB_DIVISAO.TB_DIVISAO_TELEFONE);
                paramList.Add("Logradouro", movimento.TB_DIVISAO.TB_DIVISAO_LOGRADOURO);
                paramList.Add("Numero", movimento.TB_DIVISAO.TB_DIVISAO_NUMERO);
                paramList.Add("Fax", movimento.TB_DIVISAO.TB_DIVISAO_FAX);
                paramList.Add("Complemento", movimento.TB_DIVISAO.TB_DIVISAO_COMPLEMENTO);
                paramList.Add("infoComplementar", movimento.TB_DIVISAO.TB_DIVISAO_COMPLEMENTO);
            }
            else
            {
                paramList.Add("Telefone", "");
                paramList.Add("Logradouro", "");
                paramList.Add("Numero", "");
                paramList.Add("Fax", "");
                paramList.Add("Complemento", "");
                paramList.Add("infoComplementar", "");
            }

            paramList.Add("DataRequisicao", movimento.TB_MOVIMENTO_DATA_MOVIMENTO.ToShortDateString().ToString());
            paramList.Add("NumeroDocumento", movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO);
            paramList.Add("Observacoes", movimento.TB_MOVIMENTO_OBSERVACOES);

            ParametrosRelatorio = paramList;
            presenter.imprimirRequisicao(ParametrosRelatorio);
            ExibirRelatorio();

        }

        public void ImprimirNotaSaida(string numeroDocumento, string divisaoId)
        {
            RequisicaoMaterialPresenter presenter = new RequisicaoMaterialPresenter(this);
            MovimentoPresenter movimentoPresenter = new MovimentoPresenter();
            SortedList paramList = new SortedList();


            var movimentoSaidaFornecimento = movimentoPresenter.RetornarMovimentoSaidaFornecimento(int.Parse(divisaoId), numeroDocumento);
            paramList.Add("MovimentoId", movimentoSaidaFornecimento.TB_MOVIMENTO_ID.ToString());

            ParametrosRelatorio = paramList;
            presenter.imprimirSaida(ParametrosRelatorio);
            ExibirRelatorio();
        }

        protected string RetornaIconeStatusRequisicao(int tipoMovimentoId)
        {
            string urlIconestatusRequisicao = null;

            if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                urlIconestatusRequisicao = "~/Imagens/Hourglass.gif";
            else if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada)
                urlIconestatusRequisicao = "~/Imagens/OK.gif";
            else if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada)
                urlIconestatusRequisicao = "~/Imagens/Close.gif";
            else
                urlIconestatusRequisicao = "~/Imagens/Warning.gif";

            return urlIconestatusRequisicao;
        }

        protected bool MostraIconeNotaSaida(int tipoMovimentoId)
        {
            if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada)
                return true;
            else
                return false;
        }

        protected string RetornaToolTipStatusRequisicao(int tipoMovimentoId)
        {
            if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                return "Requisição Pendente";
            else if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada)
                return "Requisição Finalizada";
            else if (tipoMovimentoId == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada)
                return "Requisição Cancelada";
            else
                return "";
        }

        public void PopularGridDocumentos()
        {
        }



        public IList<MovimentoEntity> ListarRequisicao(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            RequisicaoMaterialPresenter presenter = new RequisicaoMaterialPresenter();
            string _orgaoId = Session["orgaoId"].ToString().Trim();
            string _uoId = Session["uoId"].ToString().Trim();
            string _ugeId = Session["ugeId"].ToString().Trim();
            string _uaId = Session["uaId"].ToString().Trim();
            string _divisaoId = Session["divisaoId"].ToString().Trim();
            int _statusId = (int)Session["statusId"];
            string _numeroDocumento = Session["numeroDocumento"].ToString().Trim();

            IList<MovimentoEntity> listaRetorno = presenter.ListarRequisicao(startRowIndexParameterName, maximumRowsParameterName, _orgaoId, _uoId, _ugeId, _uaId, _divisaoId, _statusId, _numeroDocumento);

            this._TotalRegistro = presenter.TotalRegistrosGrid;
            return listaRetorno.OrderByDescending(y => y.DataDocumento).ToList();
        }

        private void CarregaViewComDDLSelectedValues()
        {
            this.OrgaoId = Convert.ToInt32(Session["orgaoId"]);
            this.UOId = Convert.ToInt32(Session["uoId"]);
            this.UGEId = Convert.ToInt32(Session["ugeId"]);
            this.UAId = Convert.ToInt32(Session["uaId"]);
            this.DivisaoId = Convert.ToInt32(Session["divId"]);
            this.PreservarComboboxValues = Convert.ToBoolean(Session["PreservarCombosValues"]);

            PageBase.RemoveSession("orgaoId");
            PageBase.RemoveSession("uoId");
            PageBase.RemoveSession("ugeId");
            PageBase.RemoveSession("uaId");
            PageBase.RemoveSession("divId");
        }

        private void PopulaSessionsModoEdicao(int movimentoId)
        {
            //Quando em modo ediÃƒÂ§ão não por via DDL (numero de requisiÃƒÂ§ão) popular as sessions com os valores do MOVIMENTO.
            var movimento = new MovimentoPresenter().ListarDocumentosRequisicaoById(movimentoId);

            DivisaoEntity divisaoEntity = new DivisaoPresenter().ListarDivisaoById(Convert.ToInt32(movimento.Divisao.Id)).SingleOrDefault();
            if (divisaoEntity == null) return;
            UAEntity uaEntity = new UAPresenter().ListarUaById(Convert.ToInt32(divisaoEntity.Ua.Id)).FirstOrDefault();
            //var org = new OrgaoPresenter().PopularDadosTodosCod

            PageBase.SetSession<int>(Convert.ToInt32(uaEntity.Orgao.Id), "orgaoId");
            PageBase.SetSession<int>(Convert.ToInt32(uaEntity.Uo.Id), "uoId");
            PageBase.SetSession<int>(Convert.ToInt32(uaEntity.Uge.Id), "ugeId");
            PageBase.SetSession<int>(Convert.ToInt32(uaEntity.Id), "uaId");
            PageBase.SetSession<int>(divisaoEntity.Id.Value, "divId");
        }

        private void PopulaSessionsComDDLSelectedValues()
        {
            PageBase.SetSession<int>(OrgaoId, "orgaoId");
            PageBase.SetSession<int>(UOId, "uoId");
            PageBase.SetSession<int>(UGEId, "ugeId");
            PageBase.SetSession<int>(UAId, "uaId");
            PageBase.SetSession<int>(DivisaoId, "divId");
        }

        public void PrepararVisaoDeCombosPorPerfil(int perfilId)
        {
            //Se o botão Cancelar RequisiÃƒÂ§ão for clicado, ou seja, voltar para pagina de Consulta, traz os valores dos DDLS.
            if (PreservarComboboxValues) return;

            int idPerfil = perfilId;


            //if (idPerfil != (int)Sam.Common.Perfil.REQUISITANTE && idPerfil != (int)Sam.Common.Perfil.REQUISITANTE_GERAL && idPerfil != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            //{
            //    CascatearDDLOrgao = true;
            //    CascatearDDLUO = true;
            //    CascatearDDLUGE = true;
            //    CascatearDDLUA = false;

            //    //Selecione Campos UA/ DIVISAO
            //    CombosHierarquiaPadrao1.ddlUA.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUA.ClearSelection();
            //    CombosHierarquiaPadrao1.ddlUA.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlDivisao.Items.Insert(0, new ListItem("Selecione", "0"));
            //}

            //else if (idPerfil == (int)Sam.Common.Perfil.REQUISITANTE || idPerfil == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
            //{
            //    CascatearDDLOrgao = true;
            //    CascatearDDLUO = false;

            //    //Selecione Campos UO/ UGE/ UA; DIVISAO
            //    CombosHierarquiaPadrao1.ddlUO.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUO.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlUGE.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUGE.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlUA.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUA.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlDivisao.Items.Insert(0, new ListItem("Selecione", "0"));
            //}
            //else
            //{
            //    CascatearDDLOrgao = false;

            //    //Selecione Campos ORGAO/ UO/ UGE/ UA; DIVISAO
            //    CombosHierarquiaPadrao1.ddlOrgao.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUO.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUO.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlUGE.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUGE.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlUA.Items.Insert(0, new ListItem("Selecione", "0"));
            //    CombosHierarquiaPadrao1.ddlUA.SelectedIndex = 0;
            //    CombosHierarquiaPadrao1.ddlDivisao.Items.Insert(0, new ListItem("Selecione", "0"));
            //}

        }

        public void ResetGridViewPageIndex()
        {
            grdDocumentos.PageIndex = 0;
        }

        #endregion

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularUO();
            this.PopularUGE();
            this.PopularUA();
            this.PopularDivisao();
        }

        protected void ddlUO_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularUGE();
            this.PopularUA();
            this.PopularDivisao();
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularUA();
            this.PopularDivisao();
        }

        protected void ddlUA_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularDivisao();
        }
    }

}
