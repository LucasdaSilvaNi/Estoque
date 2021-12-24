using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;

namespace Sam.Web.Seguranca
{
    public partial class cadastroOrgao : PageBase, IOrgaoView
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OrgaoPresenter orgao = new OrgaoPresenter(this);
                orgao.Load();
            }

            //SomenteNumero
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'5') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            // Associa a grid com o Id do objectdatasource utilizado
            gridOrgao.DataSourceID = "sourceDados";
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
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

        public IList ListaErros
        {

            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());                
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

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
                if (gridOrgao.Rows.Count > 0)
                    this.btnGravar.Enabled = true;
                else
                    this.btnGravar.Enabled = false;

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

        public bool Ativo
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

        public bool Implantado
        {
            get
            {
                return Convert.ToBoolean(ddlImplantado.SelectedValue);
            }

            set
            {
                ListItem item = ddlImplantado.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAtivo.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        #endregion


        protected void btnGravar_Click(object sender, EventArgs e)
        {
            OrgaoPresenter orgao = new OrgaoPresenter(this);
            orgao.Gravar();
            ddlAtivo.Enabled = true;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            OrgaoPresenter orgao = new OrgaoPresenter(this);
            orgao.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            OrgaoPresenter orgao = new OrgaoPresenter(this);
            orgao.Cancelar();
            ddlAtivo.Enabled = true;
        }

        protected void gridOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlAtivo.Enabled = true;                       
            this.Id = gridOrgao.DataKeys[gridOrgao.SelectedIndex].Value.ToString();
            if (gridOrgao.Rows[gridOrgao.SelectedIndex].FindControl("lblDescricaoOrgao") != null)                
            this.Descricao = ((Label)gridOrgao.Rows[gridOrgao.SelectedIndex].FindControl("lblDescricaoOrgao")).Text.Replace(" (Inativo)", "");
            this.Codigo = Server.HtmlDecode(gridOrgao.Rows[gridOrgao.SelectedIndex].Cells[1].Text);
          
            OrgaoPresenter orgao = new OrgaoPresenter(this);
            orgao.RegistroSelecionado();
            txtCodigo.Focus();

        }

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

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            OrgaoPresenter orgao = new OrgaoPresenter(this);
            orgao.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ddlAtivo.SelectedIndex = 0;
            ddlAtivo.Enabled = false;
            OrgaoPresenter ptOrgao = new OrgaoPresenter(this);           
            ptOrgao.Novo();
            txtCodigo.Focus();
        }
          
        public bool IntegracaoSIAFEM
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
    }
}
