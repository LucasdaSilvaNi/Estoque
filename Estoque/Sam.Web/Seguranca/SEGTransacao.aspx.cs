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
    public partial class SEGTransacao : PageBase, ITransacaoView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            TransacaoPresenter presenter = new TransacaoPresenter(this);
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
            TransacaoPresenter perfil = new TransacaoPresenter(this);
            perfil.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            TransacaoPresenter perfil = new TransacaoPresenter(this);
            perfil.Novo();
            ddlModuloEdit.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            TransacaoPresenter perfil = new TransacaoPresenter(this);
            perfil.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            TransacaoPresenter perfil = new TransacaoPresenter(this);
            //perfil.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdTransacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (int)grdTransacao.SelectedDataKey.Value;

            TransacaoPresenter presenter = new TransacaoPresenter(this);
            presenter.RegistroSelecionado();
            ddlModuloEdit.Focus();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            TransacaoPresenter perfil = new TransacaoPresenter(this);
            perfil.Excluir();
        }

        protected void ddlModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TransacaoPresenter presenter = new TransacaoPresenter(this);
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
            this.grdTransacao.DataSourceID = "sourceGridAlmoxarifado";
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
        
        public void PopularListaModulo()
        {
            ModuloPresenter presenter = new ModuloPresenter();
            ddlModulo.DataSource = presenter.SelectAll();
            ddlModulo.DataTextField = "TB_MODULO_DESCRICAO";
            ddlModulo.DataValueField = "TB_MODULO_ID";

            ddlModulo.DataBind();
        }

        public void PopularListaModuloEdit()
        {
            ModuloPresenter presenter = new ModuloPresenter();
            ddlModuloEdit.DataSource = presenter.SelectAll();
            ddlModuloEdit.DataTextField = "TB_MODULO_DESCRICAO";
            ddlModuloEdit.DataValueField = "TB_MODULO_ID";

            ddlModuloEdit.DataBind();
            ddlModuloEdit.Items.Insert(0, new ListItem("Selecione", "0"));
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

        public int MODULO_ID
        {
            get
            {
                if (ddlModuloEdit.SelectedIndex >= 0)
                    return Convert.ToInt32(ddlModuloEdit.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (ddlModuloEdit.Items.Count > 0)
                {
                    ddlModuloEdit.SelectedValue = value.ToString();                    
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

        public int? ORDEM
        {
            get
            {
                if (!String.IsNullOrEmpty(txtOrdem.Text))
                    return Convert.ToInt32(txtOrdem.Text);
                else
                    return null;
            }
            set
            {
                txtOrdem.Text = value.ToString();
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
    }
}
