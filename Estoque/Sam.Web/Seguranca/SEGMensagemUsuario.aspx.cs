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
using System.Windows.Forms;
using System.IO;

namespace Sam.Web.Seguranca
{
    public partial class SEGMensagemUsuario : PageBase, INotificacaoView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            NotificacaoPresenter presenter = new NotificacaoPresenter(this);
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
            NotificacaoPresenter perfil = new NotificacaoPresenter(this);
            perfil.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            NotificacaoPresenter perfil = new NotificacaoPresenter(this);
            perfil.Novo();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            NotificacaoPresenter perfil = new NotificacaoPresenter(this);
            perfil.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            NotificacaoPresenter perfil = new NotificacaoPresenter(this);
            //perfil.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdNotificacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (int)grdNotificacao.SelectedDataKey.Value;

            NotificacaoPresenter presenter = new NotificacaoPresenter(this);
            presenter.RegistroSelecionado();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            NotificacaoPresenter perfil = new NotificacaoPresenter(this);
            perfil.Excluir();
        }

        protected void ddlModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            NotificacaoPresenter presenter = new NotificacaoPresenter(this);
            presenter.Cancelar();
        }

        #endregion

        #region Propriedades

        public bool Ativo
        {
            get
            {
                return Convert.ToBoolean(ddlAtivo.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlAtivo.SelectedValue = "True";
                else
                    ddlAtivo.SelectedValue = "False";
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
            get { return txtMensagem.Text; }
            set { txtMensagem.Text = value; }
        }

        public void PopularGrid()
        {
            this.grdNotificacao.DataSourceID = "sourceGridPerfil";
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
            set { }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                if (value == true)
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

        public bool? ATIVO
        {
            get
            {
                return Convert.ToBoolean(ddlAtivo.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlAtivo.SelectedValue = "True";
                else
                    ddlAtivo.SelectedValue = "False";
            }
        }

        public short? PERFIL_ID
        {
            get
            {
                if (ddlPerfilEdit.SelectedValue != "")
                {
                    if (ddlPerfilEdit.SelectedValue == "0")
                        return null;
                    else
                    {
                        return Convert.ToInt16(ddlPerfilEdit.SelectedValue);
                    }
                    
                }
                else
                    return null;
            }
            set
            {
                if (value == null)
                    value = 0;

                if (ddlPerfilEdit.Items.Count > 0)
                {
                    ddlPerfilEdit.SelectedItem.Selected = false;
                    ddlPerfilEdit.Items.FindByValue(value.ToString()).Selected = true;
                }
            }
        }

        public string TITULO
        {
            get
            {
                return txtTituloMensagem.Text;
            }
            set
            {
                txtTituloMensagem.Text = value;
            }
        }

        public string MENSAGEM
        {
            get
            {
                return txtMensagem.Text;
            }
            set
            {
                txtMensagem.Text = value;
            }
        }

        public DateTime DATA
        {
            get
            {
                return DateTime.Now;
            }
            set
            {
                
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion



    }

}
