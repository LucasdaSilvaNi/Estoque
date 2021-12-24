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
    public partial class SEGModulo : PageBase, IModuloView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            ModuloPresenter presenter = new ModuloPresenter(this);
            if (!IsPostBack) 
            {
                presenter.Load();
            }
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ModuloPresenter perfil = new ModuloPresenter(this);
            perfil.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ModuloPresenter perfil = new ModuloPresenter(this);
            perfil.Novo();
            ddlSistema.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ModuloPresenter perfil = new ModuloPresenter(this);
            perfil.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ModuloPresenter perfil = new ModuloPresenter(this);
            //perfil.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (int)grdModulo.SelectedDataKey.Value;

            ModuloPresenter presenter = new ModuloPresenter(this);
            presenter.RegistroSelecionado();
            ddlSistema.Focus();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ModuloPresenter perfil = new ModuloPresenter(this);
            perfil.Excluir();
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
            this.grdModulo.DataSourceID = "sourceGridModulo";
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

        public void PopularDllSistema()
        {
            ddlSistema.DataBind();
        }

        #endregion

        #region View

        public int ID
        {
            get
            {
                int retorno = 0;
                if (Session["ID_Transacao"] != null)
                    retorno = Convert.ToInt32(Session["ID_Transacao"].ToString());
                return retorno;
            }
            set
            {
                Session["ID_Transacao"] = value;
            }
        }

        public int SISTEMA_ID
        {
            get
            {
                if (ddlSistema.SelectedIndex >= 0)
                    return Convert.ToInt32(ddlSistema.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (ddlSistema.Items.Count > 0)
                {
                    ddlSistema.SelectedValue = value.ToString();                    
                }
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

        public string CAMINHO
        {
            get
            {
                return txtCaminho.Text;
            }
            set
            {
                txtCaminho.Text = value;
            }
        }

        short? IModuloView.ORDEM
        {
            get
            {
                if (!String.IsNullOrEmpty(txtOrdem.Text))
                    return Convert.ToInt16(txtOrdem.Text);
                else
                    return null;
            }
            set
            {
                txtOrdem.Text = value.ToString();
            }
        }
        public int? ID_PAI
        {
            get
            {
               if(!String.IsNullOrEmpty(txtPai.Text))
               {
                   return Convert.ToInt32(txtPai.Text);
               }
               else
               {
                   return null;
               }
            }
            set
            {
                txtPai.Text = value.ToString();
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
    }
}
