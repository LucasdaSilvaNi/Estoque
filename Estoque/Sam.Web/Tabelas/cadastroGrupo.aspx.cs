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
    public partial class cadastroGrupo : PageBase,  IGrupoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;

            if (!IsPostBack)
            {
                GrupoPresenter presenter = new GrupoPresenter(this);
                presenter.Load();
            }

            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

        }

        #region IEntidadesAuxiliaresView Members

        #region Entidades

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

        #region Eventos

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            GrupoPresenter uo = new GrupoPresenter(this);
            uo.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            pnlEditar.Visible = true;

            GrupoPresenter uo = new GrupoPresenter(this);
            uo.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            GrupoPresenter uo = new GrupoPresenter(this);
            uo.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            GrupoPresenter uo = new GrupoPresenter(this);
            uo.Cancelar();
        }

        protected void gridGrupo_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();            
        }

        protected void gridGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridGrupo.DataKeys[gridGrupo.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridGrupo.Rows[gridGrupo.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridGrupo.Rows[gridGrupo.SelectedIndex].Cells[2].Text);
            GrupoPresenter uo = new GrupoPresenter(this);
            uo.RegistroSelecionado();
            txtCodigo.Focus();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            GrupoPresenter grupo = new GrupoPresenter(this);
            grupo.Imprimir();
        }

        protected void gridGrupo_RowEditing(object sender, GridViewEditEventArgs e)
        {
        }


        #endregion

        #region Metodos

        public void PopularGrid()
        {
            gridGrupo.DataSourceID = "sourcegridGrupo";
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

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        #endregion        

        #endregion

        #region IEstruturaOrganizacionalView Members


        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion
        
    }
}
