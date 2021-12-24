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
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroMaterial : PageBase,  IMaterialView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;

            if (!IsPostBack)
            {
                MaterialPresenter material = new MaterialPresenter(this);
                material.Load();
            }
            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridMaterial.DataSourceID = "sourcegridMaterial";
        }

        public void PopularListaGrupo()
        {
            ddlGrupo.DataSourceID = "sourceListaGrupo";
        }

        public void PopularListaClasse()
        {
            ddlClasse.DataSourceID = "sourceListaClasse";
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

        public string ClasseId
        {
            get
            {

                return ddlClasse.SelectedValue;
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

        public string GrupoId
        {
            get
            {
                return ddlGrupo.SelectedValue;
            }

        }

        public IList ListaErros
        {

            set
            {
                ListInconsistencias.ExibirLista(value);
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
            MaterialPresenter material = new MaterialPresenter(this);
            material.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            MaterialPresenter material = new MaterialPresenter(this);
            material.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            MaterialPresenter material = new MaterialPresenter(this);
            material.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private void Cancelar()
        {
            MaterialPresenter material = new MaterialPresenter(this);
            material.Cancelar();
        }
        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMaterial.PageIndex = 0;

            this.PopularListaClasse();
            PopularGrid();
            this.Cancelar();
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMaterial.PageIndex = 0;

            PopularGrid();
            this.Cancelar();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMaterial.PageIndex = 0;
            PopularGrid();
        }

        protected void gridMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void gridMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridMaterial.DataKeys[gridMaterial.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridMaterial.Rows[gridMaterial.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridMaterial.Rows[gridMaterial.SelectedIndex].Cells[2].Text);

            MaterialPresenter material = new MaterialPresenter(this);
            material.RegistroSelecionado();
            txtCodigo.Focus();
        }

        #region IEstruturaOrganizacionalView Members


        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            MaterialPresenter material = new MaterialPresenter(this);
            material.Imprimir();
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
                paramList.Add("CodigoGrupo", this.ddlGrupo.SelectedValue.ToString());
                paramList.Add("DescricaoGrupo", this.ddlGrupo.SelectedItem.Text);
                paramList.Add("CodigoClasse", this.ddlClasse.SelectedValue.ToString());
                paramList.Add("DescricaoClasse", this.ddlClasse.SelectedItem.Text);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

    }
}
