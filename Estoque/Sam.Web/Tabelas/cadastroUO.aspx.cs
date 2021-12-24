using System;
using System.Collections;
using System.Collections.Generic;
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
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroUO : PageBase, IUOView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UOPresenter uo = new UOPresenter(this);
                uo.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'5') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridUO.DataSourceID = "sourceGridUO";
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
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
                //paramList.Add("NomeGestor",Session["NameGestor"].ToString());

                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("NomeOrgao", this.ddlOrgao.SelectedItem.Text);

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
        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        #endregion

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ddlAtivo.SelectedIndex = 0;
            ddlAtivo.Enabled = false;
            UOPresenter uo = new UOPresenter(this);
            uo.Novo();
            txtCodigo.Text = ddlOrgao.Items[ddlOrgao.SelectedIndex].Text.Substring(0, 2);
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {

            UOPresenter uo = new UOPresenter(this);          
            uo.Gravar();
            ddlAtivo.Enabled = true;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UOPresenter uo = new UOPresenter(this);
            uo.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UOPresenter uo = new UOPresenter(this);
            uo.Cancelar();
        }

        protected void gridUO_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            UOPresenter UO = new UOPresenter(this);
            UO.Cancelar();

            gridUO.PageIndex = 0;
            PopularGrid();
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

        protected void gridUO_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlAtivo.Enabled = true;           
            this.Id = gridUO.DataKeys[gridUO.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUO.Rows[gridUO.SelectedIndex].Cells[1].Text);          
            this.Descricao = ((Label)gridUO.Rows[gridUO.SelectedIndex].FindControl("lblDescricaoUO")).Text.Replace(" (Inativa)", "");
            UOPresenter uo = new UOPresenter(this);
            uo.RegistroSelecionado();
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
            UOPresenter uo = new UOPresenter(this);
            uo.Imprimir();
        }

        public IList<UOEntity> ListarUgePorUo(int uaId)
        {
            throw new NotImplementedException();
        }
    }
}
