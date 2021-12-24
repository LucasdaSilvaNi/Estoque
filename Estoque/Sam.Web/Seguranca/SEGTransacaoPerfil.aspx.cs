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
    public partial class SEGTransacaoPerfil : PageBase, ITransacaoPerfilView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            

            TransacaoPerfilPresenter presenter = new TransacaoPerfilPresenter(this);
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
            TransacaoPerfilPresenter perfil = new TransacaoPerfilPresenter(this);
            perfil.Gravar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            TransacaoPerfilPresenter perfil = new TransacaoPerfilPresenter(this);
            perfil.Novo();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            TransacaoPerfilPresenter perfil = new TransacaoPerfilPresenter(this);
            perfil.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            TransacaoPerfilPresenter perfil = new TransacaoPerfilPresenter(this);
            //perfil.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void grdTransacaoPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ID = (int)grdTransacaoPerfil.SelectedDataKey.Value;

            TransacaoPerfilPresenter presenter = new TransacaoPerfilPresenter(this);
            presenter.RegistroSelecionado();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            TransacaoPerfilPresenter perfil = new TransacaoPerfilPresenter(this);
            perfil.Excluir();
        }

        protected void ddlModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TransacaoPerfilPresenter presenter = new TransacaoPerfilPresenter(this);
            //presenter.Cancelar();
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


        public void PopularGrid()
        {
            this.grdTransacaoPerfil.DataSourceID = "sourceGridAlmoxarifado";
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

        public void PopularListaEdit()
        {
            ddlTransacao.Items.Clear();
            ddlPerfilEdit.Items.Clear();

            InicializarCombos(ddlPerfilEdit);
            InicializarCombos(ddlTransacao);

            ddlTransacao.DataBind();
            ddlPerfilEdit.DataBind();
        }

        private void InicializarCombos(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("- Selecione -");
            ddl.Items[0].Value = "0";
            ddl.AppendDataBoundItems = true;
        }

        #endregion

        #region View

        public int ID
        {
            get
            {
                int retorno = 0;
                if (Session["ID_TransacaoPerfil"] != null)
                    retorno = Convert.ToInt32(Session["ID_TransacaoPerfil"].ToString());
                return retorno;
            }
            set
            {
                Session["ID_TransacaoPerfil"] = value;
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

        public int TRANSACAO_ID
        {
            get
            {
                if (ddlTransacao.SelectedIndex >= 0)
                    return Convert.ToInt32(ddlTransacao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (ddlTransacao.Items.Count > 0)
                {
                    ddlTransacao.SelectedValue = value.ToString();
                }
            }
        }

        public short PERFIL_ID
        {
            get
            {
                if (ddlPerfilEdit.SelectedIndex >= 0)
                    return Convert.ToInt16(ddlPerfilEdit.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (ddlPerfilEdit.Items.Count > 0)
                {
                    ddlPerfilEdit.SelectedValue = value.ToString();
                }
            }
        }

        public bool EDITA
        {
            get
            {
                return Convert.ToBoolean(ddlEdita.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlEdita.SelectedValue = "True";
                else
                    ddlEdita.SelectedValue = "False";
            }
        }

        public bool? FILTRA_COMBO
        {
            get
            {
                return Convert.ToBoolean(ddlFiltraCombo.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlFiltraCombo.SelectedValue = "True";
                else
                    ddlFiltraCombo.SelectedValue = "False";
            }
        }

        public string Descricao
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void ddlModuloEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTransacao.DataSourceID = "odsTransacao";
        }
    }
}
