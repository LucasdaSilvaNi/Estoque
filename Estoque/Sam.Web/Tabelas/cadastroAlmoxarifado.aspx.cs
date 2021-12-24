using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using System.Data;

namespace Sam.Web.Seguranca
{
    public partial class cadastroAlmoxarifado : PageBase, IAlmoxarifadoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
                almoxarifado.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
            ScriptManager.RegisterStartupScript(this.txtCep, GetType(), "cep", "$('.cep').mask('99999-999');", true);
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            ScriptManager.RegisterStartupScript(this.txtRefInicial, GetType(), "mesAno", "$('.mesAno').mask('9999/99');", true);

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region IEntidadesAuxiliaresView Members

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

        public void PopularListaOrgao()
        {
            ddlOrgao.Items.Add("- Selecione -");
            ddlOrgao.Items[0].Value = "0";
            ddlOrgao.AppendDataBoundItems = true;
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public string OrgaoId
        {
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value);
                if (item != null)
                {
                    ddlOrgao.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlOrgao.SelectedValue; }
        }

        public void PopularListaGestor(int _orgaoId)
        {
            ddlGestor.DataSourceID = "sourceListaGestor";
        }

        public string GestorId
        {
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value);
                if (item != null)
                {
                    ddlGestor.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlGestor.SelectedValue; }
        }

        public string EnderecoLogradouro
        {
            get { return txtLogradouro.Text; }
            set { txtLogradouro.Text = value; }
        }

        public string Codigo
        {
            get
            {
                return txtCodigo.Text;
            }
            set
            {
                txtCodigo.Text = value;
            }
        }

        public string Descricao
        {
            get { return txtNome.Text; }
            set { txtNome.Text = value; }
        }

        public string EnderecoNumero
        {
            get { return txtNumero.Text; }
            set { txtNumero.Text = value; }
        }

        public string EnderecoComplemento
        {
            get { return txtComplemento.Text; }
            set { txtComplemento.Text = value; }
        }

        public string EnderecoBairro
        {
            get { return txtBairro.Text; }
            set { txtBairro.Text = value; }
        }

        public string EnderecoMunicipio
        {
            get { return txtMunicipio.Text; }
            set { txtMunicipio.Text = value; }
        }

        public void PopularListaUf()
        {
            this.ddlUf.DataSourceID = "sourceListaUf";
            this.ddlUf.DataBind();
            this.ddlUf.Items.Insert(0, " - Selecione - ");
        }

        public string UfId
        {
            set
            {
                ListItem item = ddlUf.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUf.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlUf.SelectedValue; }
        }

        public string EnderecoCep
        {
            get { return txtCep.Text; }
            set { txtCep.Text = value; }
        }

        public string EnderecoTelefone
        {
            get { return txtTelefone.Text; }
            set { txtTelefone.Text = value; }
        }

        public string EnderecoFax
        {
            get { return txtFax.Text; }
            set { txtFax.Text = value; }
        }

        public string Responsavel
        {
            set
            {
                txtResponsavel.Text = value;
            }

            get { return txtResponsavel.Text; }
        }

        public void PopularListaUge()
        {

            UGEPresenter uge = new UGEPresenter();
            ddlUGE.Items.Clear();
            ddlUGE.Items.Insert(0, " - Selecione - ");
            ddlUGE.AppendDataBoundItems = true;

            if (this.GestorId != "")
            {
                int gestorId;
                if (int.TryParse(this.GestorId, out gestorId))
                {
                    ddlUGE.DataSource = uge.PopularDadosTodosCodPorGestor(gestorId);
                    ddlUGE.DataBind();
                }
            }

        }

        public void PopularListaAlmoxarifados()
        {
            int almoxSelecionadoID = -1;
            int gestorID = -1;
            List<AlmoxarifadoEntity> listaAlmoxarifados = null;
            AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter(this);

            if (Int32.TryParse(this.Id, out almoxSelecionadoID) && Int32.TryParse(GestorId, out gestorID))
                listaAlmoxarifados = objPresenter.PopularListaAlmoxarifado(gestorID)
                                                 .ToList();

            IList<ListItem> selecaoAlmoxarifados = new List<ListItem>();
            listaAlmoxarifados.RemoveAll(_almox => _almox.Id == almoxSelecionadoID);
            listaAlmoxarifados.ForEach(_almox => selecaoAlmoxarifados.Add(new ListItem() { Text = _almox.Descricao, Value = _almox.Id.ToString() }));
        }

        public string UgeId
        {
            set
            {
                ListItem item = ddlUGE.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;

                }
            }

            get { return ddlUGE.SelectedValue; }
        }

        public string IndicadorAtividadeId
        {
            set
            {
                ListItem item = ddlIndicadorAtividade.Items.FindByValue(value);
                if (item != null)
                {
                    ddlIndicadorAtividade.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlIndicadorAtividade.SelectedValue;
            }
        }

        public string TipoAlmoxarifado
        {
            set
            {
                ListItem item = ddlTipoAlmoxarifado.Items.FindByValue(value);
                if (item != null)
                {
                    ddlTipoAlmoxarifado.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlTipoAlmoxarifado.SelectedValue;
            }
        }

        public bool PermiteIgnorarCalendarioSiafemParaReabertura
        {
            set
            {
                string vlCampo = Convert.ToInt32(value).ToString();
                ListItem item = ddlIgnoraCalendarioSiafemParaReabertura.Items.FindByValue(vlCampo);
                if (item != null)
                {
                    ddlIgnoraCalendarioSiafemParaReabertura.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return (ddlIgnoraCalendarioSiafemParaReabertura.SelectedValue == "1");
            }
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        public string RefInicial
        {
            get { return txtRefInicial.Text; }
            set { txtRefInicial.Text = value; }
        }

        public string RefFaturamento
        {
            get { return txtRefFaturamento.Text; }
            set { txtRefFaturamento.Text = value; }
        }

        public void PopularGrid()
        {
            this.gridAlmoxarifado.DataSourceID = "sourceGridAlmoxarifado";
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
            {
                this.btnNovo.Enabled = value;
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                this.btnGravar.Enabled = value;
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                this.btnExcluir.Enabled = value;
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                this.btnCancelar.Enabled = value;
            }
        }

        public bool BloqueiaCodigo
        {
            set
            {
                this.txtCodigo.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set { this.txtNome.Enabled = value; }
        }

        public bool BloqueiaEnderecoLogradouro
        {
            set { this.txtLogradouro.Enabled = value; }
        }

        public bool BloqueiaEnderecoNumero
        {
            set { this.txtNumero.Enabled = value; }
        }

        public bool BloqueiaEnderecoComplemento
        {
            set { this.txtComplemento.Enabled = value; }
        }

        public bool BloqueiaEnderecoTelefone
        {
            set { this.txtTelefone.Enabled = value; }
        }

        public bool BloqueiaEnderecoMunicipio
        {
            set { this.txtMunicipio.Enabled = value; }
        }

        public bool BloqueiaEnderecoBairro
        {
            set { this.txtBairro.Enabled = value; }
        }

        public bool BloqueiaRefInicial
        {
            set { this.txtRefInicial.Enabled = value; }
        }

        public bool BloqueiaEnderecoFax
        {
            set { this.txtFax.Enabled = value; }
        }

        public bool BloqueiaEnderecoCep
        {
            set { this.txtCep.Enabled = value; }
        }


        public bool BloqueiaListaUge
        {
            set { this.ddlUGE.Enabled = value; }
        }

        public bool BloqueiaListaIndicadorAtividade
        {
            set { this.ddlIndicadorAtividade.Enabled = value; }
        }

        public bool BloqueiaResponsavel
        {
            set { this.txtResponsavel.Enabled = value; }
        }

        public bool BloqueiaListaUf
        {
            set { this.ddlUf.Enabled = value; }
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

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();

            PopularListaUge();
            //            PopularDadosTodosCod
            //            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();
            gridAlmoxarifado.PageIndex = 0;
            PopularGrid();
            this.PopularListaUge();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Novo();
            PopularListaUge();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            var i = 1;
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Gravar();
            //if (i == 1)
            //{ ResetGridPesquisa();
            //    this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
            //}

        }
        public void ResetGridPesquisa()
        {
            //metodo para restaurar o grid quando o usuario selecionar outro perfil na combo.
            DataSet ds = new DataSet();
            ds.Reset();
            gridAlmoxarifado.DataSource = ds;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();
        }

        protected void gridAlmoxarifado_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label lblValorCampo = null;

            this.Id = gridAlmoxarifado.DataKeys[gridAlmoxarifado.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].Cells[2].Text);

            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblLogradouro") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblLogradouro");
                this.EnderecoLogradouro = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblNumero") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblNumero");
                this.EnderecoNumero = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblCompl") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblCompl");
                this.EnderecoComplemento = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblBairro") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblbairro");
                this.EnderecoBairro = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblMunicipio") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblMunicipio");
                this.EnderecoMunicipio = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblUfId") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblUfId");
                this.UfId = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblCep") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblCep");
                this.EnderecoCep = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblTelefone") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblTelefone");
                this.EnderecoTelefone = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblFax") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblFax");
                this.EnderecoFax = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblResponsavel") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblResponsavel");
                this.Responsavel = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            this.PopularListaUge();
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIdUge") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIdUge");
                this.UgeId = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblRefInicial") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblRefInicial");
                this.RefInicial = Server.HtmlDecode(lblValorCampo.Text);

                //txtRefInicial.Enabled = String.IsNullOrWhiteSpace(this.RefInicial);
                txtRefInicial.Enabled = (String.IsNullOrWhiteSpace(this.RefInicial) || this.IsAdminGeral());
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblrefFaturamento") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblrefFaturamento");
                this.RefFaturamento = Server.HtmlDecode(lblValorCampo.Text);

                //txtRefInicial.Enabled = String.IsNullOrWhiteSpace(this.RefInicial);
                txtRefFaturamento.Enabled = (String.IsNullOrWhiteSpace(this.RefFaturamento) || this.IsAdminGeral());
            }


            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIndicadorAtividadeId") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIndicadorAtividadeId");
                this.IndicadorAtividadeId = Server.HtmlDecode(lblValorCampo.Text);
            }

            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblTipoAlmoxarifado") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblTipoAlmoxarifado");
                this.TipoAlmoxarifado = Server.HtmlDecode(lblValorCampo.Text);
            }


            lblValorCampo = null;
            if (gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIgnoraCalendarioSiafemParaReabertura") != null)
            {
                lblValorCampo = (Label)gridAlmoxarifado.Rows[gridAlmoxarifado.SelectedIndex].FindControl("lblIgnoraCalendarioSiafemParaReabertura");
                this.PermiteIgnorarCalendarioSiafemParaReabertura = bool.Parse(Server.HtmlDecode(lblValorCampo.Text));

                campoIgnoraCalendarioSiafemParaReabertura.Visible = (this.IsAdminGeral() || this.IsAdminFinanceiro());
            }

            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.RegistroSelecionado();

            txtCodigo.Focus();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Imprimir();
        }

        protected void ddlUGE_DataBound(object sender, EventArgs e)
        {
            if (ddlUGE.SelectedIndex > 0)
            {
                ddlUGE.SelectedValue = this.UgeId;
            }
            else
            {
                if (this.UgeId != "")
                {
                    ddlUGE.SelectedValue = this.UgeId;
                    this.UgeId = "";
                }
            }

        }

        protected void ddlOrgao_DataBound(object sender, EventArgs e)
        {
            VerificaNovo();
        }

        public void VerificaNovo()
        {
            if (this.ddlOrgao.SelectedValue != "0" && this.ddlGestor.SelectedValue != "0" && this.ddlOrgao.SelectedValue != "" && this.ddlGestor.SelectedValue != "")
            {
                btnNovo.Enabled = true;
                btnImprimir.Enabled = true;
            }
            else
            {
                btnNovo.Enabled = false;
                btnImprimir.Enabled = false;
            }

        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            VerificaNovo();
        }

        public bool IsAdminGeral()
        {
            var PerfilLogado = this.GetAcesso.Transacoes.Perfis[0];
            int iTipoPerfilLogado = PerfilLogado.IdPerfil;

            return (iTipoPerfilLogado == (int)GeralEnum.TipoPerfil.AdministradorGeral);
        }

        public bool IsAdminFinanceiro()
        {
            var PerfilLogado = this.GetAcesso.Transacoes.Perfis[0];
            int iTipoPerfilLogado = PerfilLogado.IdPerfil;

            return (iTipoPerfilLogado == (int)GeralEnum.TipoPerfil.AdministradorFinanceiroSEFAZ);
        }
    }
}
