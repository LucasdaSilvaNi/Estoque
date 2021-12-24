using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroUGE : PageBase, IUGEView
    {
        protected void Page_Load(object sender, EventArgs e)
        {        
            if (!IsPostBack)
            {

                UGEPresenter uge = new UGEPresenter(this);
                uge.Load();

                UGEEntity ugeEntity = new UGEEntity();
            }
           

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'6') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridUGE.DataSourceID = "sourceGridUGE";
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public void PopularListaUo(int _orgaoId)
        {
            ddlUo.DataSourceID = "sourceListaUo";
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

        public string Descricao
        {
            get
            {
                return txtDescricao.Text;
            }
            set
            {
                txtDescricao.Text = value;
            }
        }

        public string OrgaoId
        {
            get
            {
                return ddlOrgao.SelectedValue;
            }

        }

        public string UgeTipoId
        {
            set
            {
                ListItem item = ddlTipo.Items.FindByValue(value);
                if (value.ToString() != null)
                {
                    ddlTipo.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlTipo.SelectedValue;
            }
        }
        public string UoId
        {
            get
            {
                return ddlUo.SelectedValue;
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
            set
            {
                this.txtDescricao.Enabled = value;
            }
        }

        public bool BloqueiaListaTipoUge
        {
            set { this.ddlTipo.Enabled = value; }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
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
                paramList.Add("CodigoUO", ddlUo.SelectedValue.ToString());
                paramList.Add("DescricaoUO", this.ddlUo.SelectedItem.Text);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region IEstruturaOrganizacionalView Members

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


        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            UGEPresenter UGE = new UGEPresenter(this);
            UGE.Cancelar();

            this.PopularListaUo((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }

        protected void ddlUo_SelectedIndexChanged(object sender, EventArgs e)
        {

            gridUGE.PageIndex = 0;
            PopularGrid();

            UGEPresenter UGE = new UGEPresenter(this);
            UGE.Cancelar();
        }

        protected void gridUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            OcultarCampos(true);          
            this.Id = gridUGE.DataKeys[gridUGE.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUGE.Rows[gridUGE.SelectedIndex].Cells[1].Text);          
            this.Descricao = ((Label)gridUGE.Rows[gridUGE.SelectedIndex].FindControl("lblDescricaoUge")).Text.Replace(" (Inativa)", "");
            var IntegracaoSIAFEM = ((Label)gridUGE.Rows[gridUGE.SelectedIndex].FindControl("lblIntegracaoSIAFEM"));
            var Implantado = ((Label)gridUGE.Rows[gridUGE.SelectedIndex].FindControl("lblImplantado"));

            if (IntegracaoSIAFEM.Text == " Não") ddlIntegracaoSIAFEM.SelectedValue = "false";
            else ddlIntegracaoSIAFEM.SelectedValue = "true";

            if (Implantado.Text == " Não")ddlUgeImplantado.SelectedValue = "false";
            else ddlUgeImplantado.SelectedValue = "true";           
            if (gridUGE.Rows[gridUGE.SelectedIndex].FindControl("lblTipoUge") != null)
            {
                Label lbl = (Label)gridUGE.Rows[gridUGE.SelectedIndex].FindControl("lblTipoUge");
                if (!String.IsNullOrWhiteSpace(lbl.Text))
                    this.UgeTipoId = lbl.Text;
            }

            UGEPresenter uge = new UGEPresenter(this);
            uge.RegistroSelecionado();
            txtCodigo.Focus();

        }

        protected void gridUGE_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
           
            UGEPresenter uge = new UGEPresenter(this);
            OcultarCampos(false);
            ddlAtivo.SelectedIndex = 0;            
            ddlTipo.SelectedIndex = 0;
            uge.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UGEPresenter uge = new UGEPresenter(this);
            uge.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UGEPresenter uge = new UGEPresenter(this);
            uge.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UGEPresenter uge = new UGEPresenter(this);
            uge.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UGEPresenter uge = new UGEPresenter(this);
            uge.Imprimir();
        }

        protected void ddlUo_DataBound(object sender, EventArgs e)
        {
            gridUGE.PageIndex = 0;
            PopularGrid();
            if (ddlUo.Items.Count > 0)
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
        public bool UgeAtivo
        {
            get
            {
                return Convert.ToBoolean(ddlAtivo.SelectedValue);
            }
            set
            {

                ListItem item = ddlAtivo.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAtivo.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public bool UgeIntegracaoSIAFEM
        {
            get
            {
                return Convert.ToBoolean(ddlIntegracaoSIAFEM.SelectedValue);
            }
            set
            {

                ListItem item = ddlIntegracaoSIAFEM.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlIntegracaoSIAFEM.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public bool UgeImplantado
        {
            get
            {
                return Convert.ToBoolean(ddlUgeImplantado.SelectedValue);
            }
            set
            {

                ListItem item = ddlUgeImplantado.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUgeImplantado.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        private void OcultarCampos( bool filtro)
        {          
            ddlAtivo.Enabled = filtro;
        }
    }
}
