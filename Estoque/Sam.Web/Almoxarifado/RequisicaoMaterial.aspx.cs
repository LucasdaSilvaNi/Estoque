using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using Sam.Common;
using Sam.Common.Util;
using helperTratamento = Sam.Common.Util.TratamentoDados;
using enumTipoRequisicao = Sam.Common.Util.GeralEnum.TipoRequisicao;
using enumTipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using logErro = Sam.Domain.Business.LogErro;
using Sam.Common.Enums;
using Sam.Domain.Business.SIAFEM;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;


namespace Sam.Web.Almoxarifado
{
    public partial class RequisicaoMaterial : PageBase, IRequisicaoMaterialView, ICombosPadroesView
    {
        #region Propriedades
        bool novaRequisicao = true;
        private MovimentoEntity movimento = new MovimentoEntity();
        private string sessionListMovimento = "listMovimento";
        private string movimentoIdEdit = "movimentoIdEdit";
        private readonly string loginAcesso = new PageBase().GetAcesso.LoginBase;
        private readonly string subItensMovimento = "subItensParaFiltro";
        private readonly string cstListagemPTRes = "listagemPTRes";
        private readonly string cstListagemPTResAcao = "listagemPTResAcao";
        private string exibirMensagemConfirmacao = "exibirMensagem";

        public string Observacao
        {
            get { return txtObservacoes.Text; }
            set { txtObservacoes.Text = value; }
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
            set { this.ListInconsistencias.ExibirLista(value); }
        }

        public bool BloqueiaNovo
        {
            set { this.btnNovo.Enabled = value; }
        }

        public bool VisivelEditar
        {
            set { this.btnEditar.Visible = value; }
        }

        public bool VisivelAdicionar
        {
            set { btnAdicionar.Visible = value; }
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
                this.btnExcluir.Enabled = value;
                this.btnExcluir.Visible = value;
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                this.btnCancelar.Enabled = value;
            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                if (value == false)
                    this.pnlEditar.CssClass = "esconderControle";
                else
                    this.pnlEditar.CssClass = "mostrarControle";
            }
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public void InserirMensagemEmSessao(string chaveSessao, string msgSessao)
        {
            if (!String.IsNullOrWhiteSpace(chaveSessao) && !String.IsNullOrWhiteSpace(msgSessao))
                PageBase.SetSession<string>(msgSessao, chaveSessao);
        }

        public void InserirMensagemCancelada(string chaveSessao, string msgSessao)
        {
            if (!String.IsNullOrWhiteSpace(chaveSessao) && !String.IsNullOrWhiteSpace(msgSessao))
                PageBase.SetSession<string>(msgSessao, chaveSessao);
        }
        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public int? PTResId { get; set; }

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
                        this.ddlPTResItemMaterial.SelectedIndex = idxOpcaoCombo;
                }
            }
        }

        public string PTResAcao
        {
            get
            {
                string _codigoPTResAcao = null;
                var _tryParsePTResAcao = -1;

                _codigoPTResAcao = _tryParsePTResAcao.ToString();

                try
                {
                    if (this.ddlPTResAcao.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(this.ddlPTResAcao.SelectedItem.Text, out _tryParsePTResAcao))
                        _codigoPTResAcao = this.ddlPTResAcao.SelectedItem.Text;
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

        public string PTAssociadoPTRes { get; set; }

        public string PTResDescricao
        {
            get { return txtDescricaoPTRes.Text; }
            set { this.txtDescricaoPTRes.Text = value; }
        }

        public decimal Quantidade
        {
            get { return helperTratamento.TryParseDecimal(txtQuantidade.Text, true).Value.Truncar(base.numCasasDecimaisMaterialQtde, true); }
            set
            {
                decimal _convNumero = 0.000m;

                if (Decimal.TryParse(value.ToString(base.fmtFracionarioMaterialQtde), out _convNumero))
                    txtQuantidade.Text = value.ToString(base.fmtFracionarioMaterialQtde);
            }
        }

        public string Descricao
        {
            get { return txtDescricao.Text; }
            set { txtDescricao.Text = value.ToString(); }
        }

        public bool BloqueiaSubItem
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public bool BloqueiaQuantidade
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public bool BloqueiaObservacoes
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public bool BloqueiaInstrucoes
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public bool BloqueiaListaOrgao
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaOrgao = value; }
        }

        public bool BloqueiaListaUO
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUO = value; }
        }

        public bool BloqueiaListaUGE
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUGE = value; }
        }

        public bool BloqueiaListaUA
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUA = value; }
        }

        public bool BloqueiaListaDivisao
        {
            set
            { this.CombosHierarquiaPadrao1.BloqueiaDivisao = value; }
        }

        public string Codigo
        {
            get { return txtSubItem.Text; }
            set { txtSubItem.Text = value; }
        }

        public bool BloqueiaCodigo
        {
            set
            {
                txtSubItem.Enabled = value;
            }
        }

        public int SubItemId
        {
            get
            {
                if (!string.IsNullOrEmpty(idSubItem.Value))
                {
                    return (int)Common.Util.TratamentoDados.TryParseInt32(idSubItem.Value);
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                idSubItem.Value = value.ToString();
            }
        }

        public string UnidadeFornecimentoDescricao
        {
            get
            {
                return txtUnidadeFornecimento.Text;
            }
            set
            {
                txtUnidadeFornecimento.Text = value;
            }
        }

        public bool MostrarPainelRequisicao
        {
            set { }
        }

        public int OrgaoId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlOrgao.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlOrgao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.CombosHierarquiaPadrao1.DdlOrgao.SelectedValue = value.ToString();
            }
        }

        public int UOId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlUO.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUO.SelectedValue);
                else
                {
                    //this.CombosHierarquiaPadrao1.DdlUO.ClearSelection();
                    return 0;

                }
            }
            set
            {
                if (value > 0)
                    this.CombosHierarquiaPadrao1.DdlUO.SelectedValue = value.ToString();
            }
        }

        public int UGEId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlUGE.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUGE.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.CombosHierarquiaPadrao1.DdlUGE.SelectedValue = value == 0 ? string.Empty : value.ToString();
            }
        }

        public int UAId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlUA.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUA.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    //this.CombosHierarquiaPadrao1.DdlUA.ClearSelection();
                    this.CombosHierarquiaPadrao1.DdlUA.SelectedValue = value == 0 ? string.Empty : value.ToString();
            }

        }

        public int DivisaoId
        {
            get
            {
                if (!String.IsNullOrEmpty(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue))
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue = value == 0 ? string.Empty : value.ToString();
            }
        }

        public bool OcultaOrgao
        {
            set { this.CombosHierarquiaPadrao1.OcultaOrgao = value; }
        }

        public bool OcultaUO
        {
            set { this.CombosHierarquiaPadrao1.OcultaUO = value; }
        }

        public bool OcultaUGE
        {
            set { this.CombosHierarquiaPadrao1.OcultaUGE = value; }
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
                return this.CombosHierarquiaPadrao1.PreservarComboboxValues;
            }
            set
            {
                this.CombosHierarquiaPadrao1.PreservarComboboxValues = value;
            }
        }

        public int StatusId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int AlmoxarifadoId
        {
            get
            {
                throw new NotImplementedException();
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
                return (string) movimento.NumeroDocumento;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ResetarStartRowIndex
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        #region Controles ASPX

        public void RegistraJavascript()
        {
            //btnGravar.Attributes.Add("OnClick", String.Format("{0} {1}", base.generateScriptForClickOnceButton(btnGravar), "return confirm('Pressione OK para confirmar.');"));
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

            txtQuantidade.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
        }

        public void RegistraJavascriptCancelar()
        {          
            btnCancelarRequisicao.Attributes.Add("OnClick", string.Format("return confirm('Pressione OK para cancelar a Requisição {0}.');", movimento.NumeroDocumento));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistraJavascript();
            RegistraJavascriptCancelar();
            Master.FindControl("pnlBarraGestor").Visible = true;
            this.CombosHierarquiaPadrao1.View = this;

            if (!IsPostBack)
            {
                if (IsModoEdicao())
                    ConfigurarAlertDeConfirmacaoDDLs();

                if (Page.Session["orgaoId"].IsNotNull())
                    CarregarViewComDDLSelectedValues();

                RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
                requisicao.Load();

                if (!requisicao.VerificaSeAlmoxarifadoPossuiFechamentos())
                    return;

                CarregarSessionEdit();
                RemoveSession(subItensMovimento);
            }

            CarregarDadosRequisicao();
            ConfigurarPesquisaSubItem();
            ConfigurarAlertDeConfirmacaoBotoes();
            HabilitaBotaoGravar();
            PopularGrid();
            PageBase.SetSession<string>(loginAcesso, "loginAcesso");
            if (gridRequisicao.Rows.Count==0)
            {
                btnCancelarRequisicao.Visible = false;
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            #region PTRes

            var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
            var _ptres = listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull() && ptres.ProgramaTrabalho.ProjetoAtividade == this.PTResAcao && ptres.Codigo == this.PTResCodigo).FirstOrDefault();
            if (this.PTResCodigo.HasValue && listaPTResAcao.IsNotNullAndNotEmpty() && _ptres.IsNotNull())
            {
                this.PTResDescricao = _ptres.Descricao;
                this.PTResAcao = _ptres.ProgramaTrabalho.ProjetoAtividade;
                this.PTResId = _ptres.Id;
                this.PTAssociadoPTRes = _ptres.CodigoPT;
            }
            else
            {
                LimparCamposPTRes();
            }


            var _tryParsePTResCodigo = -1;
            //if ((this.PTResCodigo.IsNotNull() && !(this.PTResCodigo.Value > 0)) && !(Int32.Parse(this.PTResAcao) > 0))
            if ((this.PTResCodigo.IsNotNull() && (this.PTResCodigo.Value > 0)) && (!(Int32.TryParse(this.PTResAcao, out _tryParsePTResCodigo) || _tryParsePTResCodigo <= 0)))
            //if (this.PTResCodigo.IsNotNull() && _tryParsePTResCodigo <= 0)
            {
                ExibirMensagem("Selecionar valor válido para campo PTRes / PTRes Ação!");
                return;
            }
            if (!ValidaAdicionar())
            {
                return;
            }       
                        
            #endregion #region PTRes

            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Editar();
            movimento = requisicao.EditSubItem(movimento, _ptres);
            requisicao.EditarSucesso();
            HabilitaBotaoGravar();
            PopularGrid();
            btnCancelarRequisicao.Visible = false;
        }

        protected void btnCancelarRequisicao_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);

            if (!String.IsNullOrEmpty(txtObservacoes.Text.Trim()))
            {
                
                requisicao.CancelarRequisicao();

                if (this.ListInconsistencias.ErroCount() > 0)
                {
                    btnGravar.Enabled = true;
                    btnCancelarRequisicao.Enabled = false;
                    for (int i = 0; i < gridRequisicao.Rows.Count; i++)
                    {
                        ((ImageButton)gridRequisicao.Rows[i].FindControl("linkID")).Enabled = false;
                    }

                    return;
                }

                RemoveSession(movimentoIdEdit);
                RemoveSession(sessionListMovimento);

                //Set property para preservar os valores dos DDLs.
                PreservarComboboxValues = true;

                //Requisitante Geral - Novos Campos.
                PopulaSessionsComDDLSelectedValues();

                //string message = "Requisição cancelada com sucesso!";
                //string url = "ConsultarRequisicaoMaterial.aspx";
                //string script = "window.onload = function(){ alert('";
                //script += message;
                //script += "');";
                //script += "window.location = '";
                //script += url;
                //script += "'; }";
                //ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);                                            
                Response.Redirect("ConsultarRequisicaoMaterial.aspx");
            }
            else
            {
                string script = "alert(\"Informe o motivo do cancelamento no campo observação!\");";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }
                
        }
        protected void LimpaDadosRequisicaoCancelada()
        {
            gridRequisicao.DataSource = null;
            gridRequisicao.DataBind();
            gridRequisicao.Visible = false;
            txtObservacoes.Text = string.Empty;
            movimento.NumeroDocumento = string.Empty;
        }
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Imprimir();
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
            RemoveSession(movimentoIdEdit);
            RemoveSession(sessionListMovimento);

            //Set propertie para preservar os valores dos DDLs.
            PreservarComboboxValues = true;

            //Requisitante Geral - Novos Campos.
            PopulaSessionsComDDLSelectedValues();
            Response.Redirect("ConsultarRequisicaoMaterial.aspx", false);
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Novo();
            txtSubItem.Focus();

            if (this.ddlPTResItemMaterial.Items.Cast<ListItem>().ToList().IsNullOrEmpty())
                PreencherListaPTRes();

            if (this.ddlPTResAcao.Items.Cast<ListItem>().ToList().IsNullOrEmpty())
                PreencherListaPTResAcao();
            btnCancelarRequisicao.Visible = false;
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            GravarRequisicaoEditada();
           // this.PTResId = Convert.ToInt32(ddlPTResItemMaterial.SelectedItem.ToString());

            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Gravar(movimento);

            if (this.ListInconsistencias.ErroCount() == 0)
            {
                RemoveSession(sessionListMovimento);
                movimento = new MovimentoEntity();

                PopularGrid();

                //Set propertie para preservar os valores dos DDLs.
                PreservarComboboxValues = true;

                PopulaSessionsComDDLSelectedValues();
                Response.Redirect("ConsultarRequisicaoMaterial.aspx", true);
            }
           
            HabilitaBotaoGravar();

            if (this.ListInconsistencias.ErroCount() > 0)
            {
                btnGravar.Enabled = false;
                btnCancelarRequisicao.Enabled = false;
                for (int i = 0; i < gridRequisicao.Rows.Count; i++)
                {
                    ((ImageButton)gridRequisicao.Rows[i].FindControl("linkID")).Enabled = false;
                }                         
            }
            btnCancelarRequisicao.Visible = true;
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            //Não permite inseirir subItem que existe na lista
            var total = gridRequisicao.Rows.Count;
            for (Int32 i = 0; i < gridRequisicao.Rows.Count; i++)
            {
                var campo = (Label)gridRequisicao.Rows[i].Cells[0].FindControl("Codigo");
                var campo_ = gridRequisicao.Rows[i].Cells[0];
                if (Codigo == campo.Text)
                {
                    ExibirMensagem("Para lançar saída do mesmo material, utilizar um novo documento.");
                    return;
                }
            }
            if (!ValidaAdicionar())
            {
                return;
            }            
            #region PTRes

            var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);
            //var _ptres = listaPTRes.Where(ptres => ptres.Codigo == this.PTResCodigo).FirstOrDefault();
            var _ptres = listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull() && ptres.ProgramaTrabalho.ProjetoAtividade == this.PTResAcao && ptres.Codigo == this.PTResCodigo).FirstOrDefault();
            if (this.PTResCodigo.HasValue && listaPTResAcao.IsNotNullAndNotEmpty() && _ptres.IsNotNull())
            {
                this.PTResDescricao = _ptres.Descricao;
                this.PTResAcao = _ptres.ProgramaTrabalho.ProjetoAtividade;
                this.PTResId = _ptres.Id;
                this.PTAssociadoPTRes = _ptres.CodigoPT;
            }
            else
            {
                LimparCamposPTRes();
            }


            var _tryParsePTResCodigo = -1;
            //if ((this.PTResCodigo.IsNotNull() && !(this.PTResCodigo.Value > 0)) && !(Int32.Parse(this.PTResAcao) > 0))
            if ((this.PTResCodigo.IsNotNull() && (this.PTResCodigo.Value > 0)) && (!(Int32.TryParse(this.PTResAcao, out _tryParsePTResCodigo) || _tryParsePTResCodigo <= 0)))
            //if (this.PTResCodigo.IsNotNull() && _tryParsePTResCodigo <= 0)
            {
                ExibirMensagem("Selecionar valor válido para campo PTRes / PTRes Ação!");
                return;
            }
            #endregion #region PTRes

            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            //movimento = requisicao.AdicionarSubItem(movimento, _ptres);
			//utilização de parametro, para verificar tipo de material, de subitem a ser incluso na requisicao.
			movimento = requisicao.AdicionarSubItem(movimento, _ptres, true);

            SetSession<MovimentoEntity>(movimento, sessionListMovimento);

            if (this.ListInconsistencias.ErroCount() == 0)
            { requisicao.AdicionadoSucesso();
                btnCancelarRequisicao.Visible = false;
            }

            PopularGrid();
            HabilitaBotaoGravar();

        }

        private bool ValidaAdicionar()
        {
            bool validado = true;
            var alertSubitem = string.IsNullOrEmpty(txtSubItem.Text) ? "SubItem// " : "";
            var alertPtr = ddlPTResAcao.SelectedValue == "0" ? "Ação (PTRes)// " : "";
            var alertQtd = string.IsNullOrEmpty(txtQuantidade.Text) || txtQuantidade.Text == "0,000" ? "Quantidade// " : "";
            string mensagem = alertSubitem + alertPtr + alertQtd;
            if (!string.IsNullOrEmpty(mensagem))
            {
                this.ListaErros = new List<string>() { string.Format($"Preencha o(s) campo(s): " + mensagem) };
                validado= false;

            }
            return validado;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.RemoverSubItem(movimento);
            requisicao.ExcluidoSucesso();                                   
            PopularGrid();

            //Set propertie para preservar os valores dos DDLs.
            PreservarComboboxValues = true;

            PopulaSessionsComDDLSelectedValues();
            btnCancelarRequisicao.Visible = false;
            btnGravar.Enabled = true;
            btnNovo.Enabled = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            requisicao.Cancelar();
            HabilitaBotaoGravar();
        }


        private void LimparTodosOsCamposPTRes(IList<PTResEntity> listaPreenchimento)
        {
            LimparCamposPTRes();

            if (listaPreenchimento.IsNotNullAndNotEmpty())
            {
                this.ddlPTResAcao.InserirElementoSelecione(true);
                this.ddlPTResItemMaterial.InserirElementoSelecione(true);


                listaPreenchimento.Where(ptres => ptres.ProgramaTrabalho.IsNotNull())
                                  .OrderBy(x => x.Codigo).ToList()
                                  .ForEach(_ptresVinculado => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptresVinculado.Codigo.ToString())));

                //listaPreenchimento.Where(ptres => ptres.ProgramaTrabalho.IsNotNull())
                //                  .ToList()
                //                  .ForEach(_ptresVinculado => this.ddlPTResAcao.Items.Add(new ListItem(_ptresVinculado.ProgramaTrabalho.ProjetoAtividade, _ptresVinculado.Codigo.ToString())));

             
                listaPreenchimento.DistinctBy(a => a.ProgramaTrabalho.ProjetoAtividade).ToList()
                                .ForEach(_ptresVinculado => this.ddlPTResAcao.Items.Add(new ListItem(_ptresVinculado.ProgramaTrabalho.ProjetoAtividade, _ptresVinculado.ProgramaTrabalho.ProjetoAtividade)));
            }
            else
            {
                PreencherListaPTRes();
                PreencherListaPTResAcao();
            }
        }

        private void LimparCamposPTRes()
        {
            this.PTResId = -1;
            this.PTResCodigo = -1;
            this.PTResDescricao = null;
            this.PTResAcao = "Selecione";
            this.PTAssociadoPTRes = null;
        }

        protected void ddlPTResItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTRes);

            if (this.ddlPTResItemMaterial.SelectedIndex != 0 && listaPTRes.IsNotNullAndNotEmpty())
            {
                this.ddlPTResAcao.InserirElementoSelecione(true);

                listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                           && !String.IsNullOrWhiteSpace(ptres.ProgramaTrabalho.ProjetoAtividade)
                                           && ptres.Codigo.ToString() == this.ddlPTResItemMaterial.SelectedItem.Text)
                              .ToList()
                              .ForEach(_ptresAcaoVinculada => this.ddlPTResAcao.Items.Add(new ListItem(_ptresAcaoVinculada.ProgramaTrabalho.ProjetoAtividade)));

                if (this.ddlPTResAcao.Items.Count >= 2)
                {
                    var _ptresVinculado = listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                                && !String.IsNullOrWhiteSpace(ptres.ProgramaTrabalho.ProjetoAtividade)
                                                                && ptres.Codigo.ToString() == this.ddlPTResItemMaterial.SelectedItem.Text)
                                               .FirstOrDefault();

                    if (_ptresVinculado.IsNotNull())
                    {
                        this.PTResId = _ptresVinculado.Id;
                        this.PTResDescricao = _ptresVinculado.Descricao;
                        this.PTResCodigo = _ptresVinculado.Codigo;
                        this.PTResAcao = ((_ptresVinculado.ProgramaTrabalho.IsNotNull()) ? _ptresVinculado.ProgramaTrabalho.ProjetoAtividade : "");
                        this.PTAssociadoPTRes = _ptresVinculado.CodigoPT;
                    }
                    else
                    {
                        LimparCamposPTRes();
                    }

                }
            }
            else if (this.ddlPTResAcao.SelectedIndex == 0 && this.ddlPTResItemMaterial.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTRes);
            }
            else if (this.ddlPTResItemMaterial.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTRes);
            }
        }

        protected void ddlPTResAcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);

            if (this.ddlPTResAcao.SelectedIndex != 0 && listaPTResAcao.IsNotNullAndNotEmpty())
            {
                this.ddlPTResItemMaterial.InserirElementoSelecione(true);


                listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                           && ptres.ProgramaTrabalho.ProjetoAtividade == this.ddlPTResAcao.SelectedItem.Text)
                              .OrderBy(x => x.Codigo).ToList()
                              .ForEach(_ptresVinculado => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptresVinculado.Codigo.ToString())));


                if (this.ddlPTResItemMaterial.Items.Count >= 2)
                {
                    var _ptresAcao = listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                                && ptres.ProgramaTrabalho.ProjetoAtividade == this.ddlPTResAcao.SelectedItem.Text)
                                                   .FirstOrDefault();
                    if (_ptresAcao.IsNotNull())
                    {
                        this.PTResId = _ptresAcao.Id;
                        this.PTResDescricao = _ptresAcao.Descricao;
                        this.PTResCodigo = _ptresAcao.Codigo;
                        this.PTResAcao = ((_ptresAcao.ProgramaTrabalho.IsNotNull()) ? _ptresAcao.ProgramaTrabalho.ProjetoAtividade : "");
                        this.PTAssociadoPTRes = _ptresAcao.CodigoPT;
                    }
                }
                else
                {
                    LimparCamposPTRes();
                }
            }
            else if (this.ddlPTResAcao.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTResAcao);
            }
            btnCancelarRequisicao.Visible = false;
        }

        protected void dialog_link_Click(object sender, ImageClickEventArgs e)
        {
            PesquisaSubitem1.SetFocusOnLoad();
        }

        protected void gridRequisicao_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter(this);
            this.Id = ((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[0].FindControl("IdAleatorio")).Text;
            this.SubItemId = (int)Common.Util.TratamentoDados.TryParseInt32(((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[0].FindControl("Id")).Text);
            this.Codigo = ((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[0].FindControl("Codigo")).Text;
            this.UnidadeFornecimentoDescricao = ((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[3].FindControl("lblUnidadeFornecimento")).Text;
            this.Descricao = ((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[1].FindControl("lblDescricao")).Text;

            this.Quantidade = helperTratamento.TryParseDecimal(((Label)gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[2].FindControl("lblQtd")).Text).Value;

            var _labelPTRes = (gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[4].FindControl("lblPTRes") as Label);
            if (_labelPTRes.IsNotNull() && !String.IsNullOrWhiteSpace(_labelPTRes.Text))
                this.PTResCodigo = helperTratamento.TryParseInt32(_labelPTRes.Text).Value;

            var _labelPTResAcao = (gridRequisicao.Rows[gridRequisicao.SelectedIndex].Cells[4].FindControl("lblPTResAcao") as Label);
            if (_labelPTResAcao.IsNotNull() && !String.IsNullOrWhiteSpace(_labelPTResAcao.Text))
                this.PTResAcao = _labelPTResAcao.Text;

            RequisicaoMaterialPresenter classe = new RequisicaoMaterialPresenter(this);
            classe.View.Id = this.Id;
            classe.View.Quantidade = this.Quantidade;

            PopularGrid();
            requisicao.Editar();
            btnCancelarRequisicao.Visible = false;
        }

        protected void gridRequisicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ImageButton ImageButton = (ImageButton)e.Row.FindControl("linkID");
            Label lblQtde = (Label)e.Row.FindControl("lblQtd");

            if (lblQtde.IsNotNull())
            {
                decimal tmpValue = Decimal.Parse(lblQtde.Text);
                string tstValor = tmpValue.ToString(base.fmtFracionarioMaterialQtde);

                lblQtde.ReformatarValorNumerico(base.fmtFracionarioMaterialQtde);
            }

            ValidarEdicaoRequisicao(ImageButton);
        }

        protected void gridRequisicao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CarregaSessao();
            IList<MovimentoItemEntity> listaSubitensRequisicao = GetSession<IList<MovimentoItemEntity>>(subItensMovimento);

            gridRequisicao.PageIndex = e.NewPageIndex;

            gridRequisicao.PageSize = 50;
            gridRequisicao.AllowPaging = true;
            gridRequisicao.DataSource = listaSubitensRequisicao;
            gridRequisicao.DataBind();
        }

        #endregion


        #region Métodos

        private void ConfigurarAlertDeConfirmacaoDDLs()
        {
            this.CombosHierarquiaPadrao1.DdlOrgao.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.CombosHierarquiaPadrao1.DdlUO.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.CombosHierarquiaPadrao1.DdlUA.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.CombosHierarquiaPadrao1.DdlUGE.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.CombosHierarquiaPadrao1.DdlDivisao.Attributes.Add("onChange", "return ShowConfirm(this);");
        }

        private void SairModoEdicao()
        {
            RemoveSession(sessionListMovimento);
            movimento = null;
            this.txtObservacoes.Text = string.Empty;
            PopularGrid();

            this.CombosHierarquiaPadrao1.DdlOrgao.Attributes.Remove("onChange");
            this.CombosHierarquiaPadrao1.DdlUO.Attributes.Remove("onChange");
            this.CombosHierarquiaPadrao1.DdlUA.Attributes.Remove("onChange");
            this.CombosHierarquiaPadrao1.DdlUGE.Attributes.Remove("onChange");
            this.CombosHierarquiaPadrao1.DdlDivisao.Attributes.Remove("onChange");
        }

        private bool IsModoEdicao()
        {
            return (GetSession<string>(movimentoIdEdit) != null) ? true : false;
        }


        public void PreencherListaPTRes()
        {
            this.ddlPTResItemMaterial.InserirElementoSelecione(true);

            //Retornar a UGE selecionada no combo a cima.
            if (UGEId != 0)
            {
                var ugeCodigo = Int32.Parse(this.CombosHierarquiaPadrao1.DdlUGE.SelectedItem.Text.BreakLine('-', 0).Trim());

                PTResPresenter objPresenter = new PTResPresenter();
                var lstPTRes = objPresenter.CarregarListaPTRes(ugeCodigo);
                PageBase.SetSession<IList<PTResEntity>>(lstPTRes, cstListagemPTRes);

                if (lstPTRes.IsNotNullAndNotEmpty())
                {
                    lstPTRes.ToList()
                            .DistinctBy(ptRes => ptRes.Codigo.GetValueOrDefault())
                            .ToList()
                            .ForEach(_ptRes => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptRes.Codigo.ToString())));
                }
            }
        }

        public void PreencherListaPTResAcao()
        {
            this.ddlPTResAcao.InserirElementoSelecione(true);

            if (UGEId != 0)
            {
                var ugeCodigo = Int32.Parse(this.CombosHierarquiaPadrao1.DdlUGE.SelectedItem.Text.BreakLine('-', 0).Trim());

                PTResPresenter objPresenter = new PTResPresenter();
                var lstPTResAcao = objPresenter.CarregarListaPTResAcao(ugeCodigo);
                PageBase.SetSession<IList<PTResEntity>>(lstPTResAcao, cstListagemPTResAcao);

                if (lstPTResAcao.IsNotNullAndNotEmpty())
                {
                    lstPTResAcao.DistinctBy(a => a.ProgramaTrabalho.ProjetoAtividade).ToList()
                    //        .ForEach(_ptRes => this.ddlPTResAcao.Items.Add(new ListItem(_ptRes.ProgramaTrabalho.ProjetoAtividade, _ptRes.ProgramaTrabalho.ProjetoAtividade)));
                                .ForEach(_ptRes => this.ddlPTResAcao.Items.Add(new ListItem(_ptRes.ProgramaTrabalho.ProjetoAtividade, _ptRes.Codigo.ToString())));
                }
            }
        }

        private void CarregarSessionEdit()
        {
            if (IsModoEdicao())
            {
                string numeroDocumento = GetSession<string>(movimentoIdEdit);
                if (!string.IsNullOrEmpty(numeroDocumento))
                {
                    novaRequisicao = false;

                    MovimentoPresenter movimentoPresenter = new MovimentoPresenter();

                    int movimentoId = Convert.ToInt32(numeroDocumento);

                    movimento = movimentoPresenter.ListarDocumentosRequisicaoById(movimentoId);
                    if (movimento != null)
                    {
                        this.Id = movimento.Id.ToString();
                        SetSession<MovimentoEntity>(movimento, sessionListMovimento);
                    }
                    else
                    {
                        string MensagemErro = String.Format("{0}\\nDocumento {1}", "Erro ao carregar dados da requisição!", numeroDocumento);
                        SetSession<string>(MensagemErro, "_transPage");
                        Response.Redirect("ConsultarRequisicaoMaterial.aspx", false);
                        PageBase.RemoveSession(subItensMovimento);
                    }
                }
            }
        }

        //Se a requisição estiver no status de pendente, habilitar o botão cancelar requisição
        private void VisualizarBotaoCancelarRequisicao()
        {
            btnCancelarRequisicao.Visible = false;

            if (movimento == null) return;

            int idPerfil = Convert.ToInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil.ToString());
            if (idPerfil == Sam.Common.Perfil.REQUISITANTE || idPerfil == Sam.Common.Perfil.REQUISITANTE_GERAL || idPerfil == Sam.Common.Perfil.ADMINISTRADOR_GERAL || idPerfil == Sam.Common.Perfil.OPERADOR_ALMOXARIFADO)
                btnCancelarRequisicao.Visible = (movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente) ? true : false;
        }

        private void EditarRequisicao()
        {
            if (IsModoEdicao())
            {
                movimento = GetSession<MovimentoEntity>(sessionListMovimento);

                if (movimento != null)
                {
                    VisualizarBotaoCancelarRequisicao();
                    txtObservacoes.Text = movimento.Observacoes;

                    if (movimento.Divisao.Ua != null)
                    {
                        if (this.CombosHierarquiaPadrao1.DdlUA.Items.Count > 0)
                        {
                            if (this.CombosHierarquiaPadrao1.DdlUA.Items.FindByValue(movimento.Divisao.Ua.Id.ToString()) != null)
                                this.CombosHierarquiaPadrao1.DdlUA.SelectedValue = movimento.Divisao.Ua.Id.ToString();
                        }
                    }

                    if (movimento.Divisao != null)
                    {
                        if (this.CombosHierarquiaPadrao1.DdlDivisao.Items.Count > 0)
                        {
                            if (this.CombosHierarquiaPadrao1.DdlDivisao.Items.FindByValue(movimento.Divisao.Id.ToString()) != null)
                                this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue = movimento.Divisao.Id.ToString();
                        }
                    }
                }
            }
        }

        private void CarregarDadosRequisicao()
        {
            bool campoObservacoesVazio = false;
            bool campoPTResVazio = false;

            campoObservacoesVazio = String.IsNullOrWhiteSpace(txtObservacoes.Text);
            campoPTResVazio = String.IsNullOrWhiteSpace(this.CombosHierarquiaPadrao1.DdlPTRES.SelectedValue);

            if (IsModoEdicao())
            {
                if (this.ddlPTResItemMaterial.Items.Count <= 1)
                    PreencherListaPTRes();

                if (this.ddlPTResAcao.Items.Count <= 1)
                    PreencherListaPTResAcao();

                movimento = GetSession<MovimentoEntity>(sessionListMovimento);

                if (movimento != null)
                {
                    VisualizarBotaoCancelarRequisicao();

                    if (!String.IsNullOrWhiteSpace(txtObservacoes.Text) && (movimento.Observacoes != txtObservacoes.Text))
                        movimento.Observacoes = txtObservacoes.Text;
                    else
                        txtObservacoes.Text = movimento.Observacoes;
                }
            }
        }

        private void GravarRequisicaoEditada()
        {
            int idPerfil = Convert.ToInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil.ToString());
            bool isRequisitante = (idPerfil == Sam.Common.Perfil.REQUISITANTE);
            bool isRequisicaoPendente = false;

            if (IsModoEdicao())
            {
                movimento = GetSession<MovimentoEntity>(sessionListMovimento);

                if (movimento != null && (movimento.TipoMovimento != null && movimento.TipoMovimento.Id != 0))
                {
                    isRequisicaoPendente = (movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente);

                    if (!String.IsNullOrWhiteSpace(txtObservacoes.Text) && (movimento.Observacoes != txtObservacoes.Text))
                        movimento.Observacoes = txtObservacoes.Text;

                    if (movimento.Divisao.Ua != null && this.CombosHierarquiaPadrao1.DdlUA.Items.Count > 0)
                        movimento.Divisao.Ua.Id = Int32.Parse(this.CombosHierarquiaPadrao1.DdlUA.SelectedValue);

                    if (movimento.Divisao != null && this.CombosHierarquiaPadrao1.DdlDivisao.Items.Count > 0)
                    {
                        movimento.Divisao.Id = Int32.Parse(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue);
                        movimento.Divisao.Descricao = this.CombosHierarquiaPadrao1.DdlDivisao.SelectedItem.Text;
                    }
                }
            }
            else
            {
                if (movimento != null && this.CombosHierarquiaPadrao1.DdlDivisao.Items.Count > 0)
                {
                    movimento.Divisao = new DivisaoEntity();
                    movimento.UGE = new UGEEntity();
                    movimento.Divisao.Id = Int32.Parse(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue);
                    movimento.Divisao.Descricao = this.CombosHierarquiaPadrao1.DdlDivisao.SelectedItem.Text;
                    movimento.UGE.CodigoDescricao = this.CombosHierarquiaPadrao1.DdlUGE.SelectedItem.Text.ToString().Substring(0, 6);                   
                }
            }
        }

        public void PopularGrid()
        {
            CarregaSessao();

            if (movimento != null)
            {
                if (movimento.MovimentoItem != null)
                {
                    gridRequisicao.DataSource = movimento.MovimentoItem;
                    var documento =movimento.NumeroDocumento;
                }
                else
                    gridRequisicao.DataSource = null;

                if (!novaRequisicao)
                    btnCancelarRequisicao.Enabled = true;
            }
            else
                gridRequisicao.DataSource = null;

            gridRequisicao.EmptyDataText = "Insira pelo menos um Sub Item.";
            gridRequisicao.DataBind();

            if (movimento != null)
                SetSession<IList<MovimentoItemEntity>>((gridRequisicao.DataSource as IList<MovimentoItemEntity>), subItensMovimento);
        }

        private void HabilitaBotaoGravar()
        {
            btnGravar.Enabled = (gridRequisicao.Rows.Count > 0) ? true : false;
        }

        private void ValidarEdicaoRequisicao(ImageButton ImageButton)
        {
            if (ImageButton != null && movimento != null && movimento.TipoMovimento != null)
            {
                int tipoMovimentoId = movimento.TipoMovimento.Id;
                //Se a requisição finalizada ou cancelada, abre a tela apenas para visualização
                if ((tipoMovimentoId == (int)enumTipoMovimento.RequisicaoFinalizada) || (tipoMovimentoId == (int)enumTipoMovimento.RequisicaoCancelada))
                {
                    btnGravar.Enabled = false;
                    ImageButton.Enabled = false;
                    btnNovo.Enabled = false;
                }

                int idPerfil = Convert.ToInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil.ToString());
                //Caso o perfil não for de Requisitante, Requisitante Geral ou Adm. Geral, abre a tela apenas para visualização                
                if (idPerfil != Perfil.REQUISITANTE && idPerfil != Perfil.REQUISITANTE_GERAL && idPerfil != Perfil.ADMINISTRADOR_GERAL)
                {
                    //TODO: [Douglas Batista]: Retornar a este ponto posteriormente
                    //Operadores podem editar a requisição agora, na visualização, 
                    //if (idPerfil == Perfil.OPERADOR_ALMOXARIFADO)
                    btnGravar.Enabled = false;
                    ImageButton.Enabled = false;

                    btnNovo.Enabled = false;
                }
            }
        }

        private void CarregarViewComDDLSelectedValues()
        {
            this.OrgaoId = Convert.ToInt32(Session["orgaoId"]);
            this.UOId = Convert.ToInt32(Session["uoId"]);
            this.UGEId = Convert.ToInt32(Session["ugeId"]);
            this.UAId = Convert.ToInt32(Session["uaId"]);
            this.DivisaoId = Convert.ToInt32(Session["divId"]);

            PageBase.RemoveSession("orgaoId");
            PageBase.RemoveSession("uoId");
            PageBase.RemoveSession("ugeId");
            PageBase.RemoveSession("uaId");
            PageBase.RemoveSession("divId");
        }

        private void PopulaSessionsComDDLSelectedValues()
        {
            PageBase.SetSession<int>(OrgaoId, "orgaoId");
            PageBase.SetSession<int>(UOId, "uoId");
            PageBase.SetSession<int>(UGEId, "ugeId");
            PageBase.SetSession<int>(UAId, "uaId");
            PageBase.SetSession<int>(DivisaoId, "divId");
            PageBase.SetSession<bool>(PreservarComboboxValues, "PreservarCombosValues");
        }

        private void ConfigurarAlertDeConfirmacaoBotoes()
        {
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        private void ConfigurarPesquisaSubItem()
        {
            //PesquisaSubitem1.UsaSaldo = false;
            PesquisaSubitem1.UsaSaldo = true;
            PesquisaSubitem1.FiltrarAlmox = true;
            PesquisaSubitem1.DivisaoId = DivisaoId;
        }


        public void PrepararVisaoDeCombosPorPerfil(int perfilId)
        {
        }

        public void ResetGridViewPageIndex()
        {
            throw new NotImplementedException();
        }

        protected string FormatoFracionario()
        {
            return base.fmtFracionarioMaterialQtde;
        }


        #endregion
    }
}
