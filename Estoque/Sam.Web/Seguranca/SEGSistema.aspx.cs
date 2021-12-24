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
    public partial class SEGSistema : PageBase, ISistemaView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            SistemaPresenter presenter = new SistemaPresenter(this);
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
            SistemaPresenter Sistema = new SistemaPresenter(this);
            Sistema.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            SistemaPresenter Sistema = new SistemaPresenter(this);
            Sistema.Novo();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            SistemaPresenter Sistema = new SistemaPresenter(this);
            Sistema.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SistemaPresenter Sistema = new SistemaPresenter(this);
            //Sistema.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (int)grdSistema.SelectedDataKey.Value;

            SistemaPresenter presenter = new SistemaPresenter(this);
            presenter.RegistroSelecionado();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            SistemaPresenter Sistema = new SistemaPresenter(this);
            Sistema.Excluir();
        }

        protected void ddlModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SistemaPresenter presenter = new SistemaPresenter(this);
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
            this.grdSistema.DataSourceID = "sourceGridSistema";
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
                if (Session["ID_Sistema"] != null)
                    retorno = Convert.ToInt32(Session["ID_Sistema"].ToString());
                return retorno;
            }
            set
            {
                Session["ID_Sistema"] = value;
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

        public string SIGLA
        {
            get
            {
                return txtSigla.Text;
            }
            set
            {
                txtSigla.Text = value;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion


        
    }
}
