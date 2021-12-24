using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class SEGPerfil : PageBase, IPerfilView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            PerfilPresenter presenter = new PerfilPresenter(this);
            if (!IsPostBack) 
            {
                presenter.Load();
                PopularGrid();
            }
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnCancelar.Attributes.Add("OnClick", "return confirm('Pressione OK para cancelar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para excluir.');");
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            PerfilPresenter perfil = new PerfilPresenter(this);
            perfil.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            PerfilPresenter perfil = new PerfilPresenter(this);
            perfil.Novo();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            PerfilPresenter perfil = new PerfilPresenter(this);
            perfil.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            PerfilPresenter perfil = new PerfilPresenter(this);
            //perfil.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (short)grdPerfil.SelectedDataKey.Value;

            PerfilPresenter presenter = new PerfilPresenter(this);
            presenter.RegistroSelecionado();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            PerfilPresenter perfil = new PerfilPresenter(this);
            perfil.Excluir();
        }

        protected void ddlModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerfilPresenter presenter = new PerfilPresenter(this);
            presenter.Cancelar();
        }

        #endregion

        #region Propriedades

        public bool Ativo
        {
            get
            {
                return Convert.ToBoolean(ddlAtividade.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlAtividade.SelectedValue = "True";
                else
                    ddlAtividade.SelectedValue = "False";
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

        public string Codigo { get; set; }

        public string Descricao
        {
            get { return txtDescricao.Text; }
            set { txtDescricao.Text = value; }
        }

        public void PopularGrid()
        {
            this.grdPerfil.DataSourceID = "sourceGridPerfil";
        }

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

        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }

        public bool BloqueiaGravar
        {
            set { btnGravar.Enabled = !value; }
        }

        public bool BloqueiaExcluir
        {
            set { btnExcluir.Enabled = !value; }
        }

        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaDescricao
        {
            set {  }
        }

        public bool MostrarPainelEdicao
        {
            set 
            { 
                if(value == true)
                    pnlEditar.CssClass = "mostrarControle";
                else
                    pnlEditar.CssClass = "esconderControle";
            }
        }

        #endregion

        #region Metodos
       

        #endregion

        #region View

        public int ID
        {
            get
            {
                int retorno = 0;
                if (Session["ID_Perfil"] != null)
                    retorno = Convert.ToInt32(Session["ID_Perfil"].ToString());
                return retorno;
            }
            set
            {
                Session["ID_Perfil"] = value;
            }
        }

        public bool ATIVO
        {
            get
            {
                return Convert.ToBoolean(ddlAtividade.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlAtividade.SelectedValue = "True";
                else
                    ddlAtividade.SelectedValue = "False";
            }
        }

        public string DESCRICAO
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

        public int? PESO
        {
            get
            {
                if (!String.IsNullOrEmpty(txtPeso.Text))
                    return Convert.ToInt32(txtPeso.Text);
                else
                    return null;
            }
            set
            {
                txtPeso.Text = value.ToString();
            }
        }

        public int? PERFILNIVEL
        {
            get
            {
                return Convert.ToInt32(ddlPerfilNivel.SelectedValue);
            }
            set
            {
                ddlPerfilNivel.SelectedValue = value.ToString();
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion


        int IPerfilView.PERFILNIVEL
        {
            get
            {
                return 1;
            }
            set
            {
                
            }
        }
    }
}
