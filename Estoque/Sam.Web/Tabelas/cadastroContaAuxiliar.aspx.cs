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
    public partial class cadastroContaAuxiliar : PageBase,  IContaAuxiliarView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ContaAuxiliarPresenter presenter = new ContaAuxiliarPresenter(this);
                presenter.Load();
            }
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridConta.DataSourceID = "sourceGridConta";
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        public string ContaContabil
        {
            get
            {
                return txtContaContabil.Text;
            }
            set
            {
                txtContaContabil.Text = value;
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


        public bool BloqueiaCContabil
        {
            set
            {
                this.txtContaContabil.Enabled = value;
            }
        }

        #endregion

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ContaAuxiliarPresenter uo = new ContaAuxiliarPresenter(this);
            uo.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ContaAuxiliarPresenter uo = new ContaAuxiliarPresenter(this);
            uo.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ContaAuxiliarPresenter uo = new ContaAuxiliarPresenter(this);
            uo.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ContaAuxiliarPresenter uo = new ContaAuxiliarPresenter(this);
            uo.Cancelar();
        }

        protected void gridConta_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }



        protected void gridConta_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridConta.DataKeys[gridConta.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridConta.Rows[gridConta.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridConta.Rows[gridConta.SelectedIndex].Cells[2].Text);

            if (gridConta.Rows[gridConta.SelectedIndex].FindControl("lblContaContabil") != null) { 
                Label lbl = (Label)gridConta.Rows[gridConta.SelectedIndex].FindControl("lblContaContabil");
                this.ContaContabil  = Server.HtmlDecode(lbl.Text).ToString();
                this.ContaContabil = String.Format("{0:D9}", Convert.ToInt64(this.ContaContabil));
            }
            ContaAuxiliarPresenter uo = new ContaAuxiliarPresenter(this);
            uo.RegistroSelecionado();
            txtCodigo.Focus();
        }

        #region IEstruturaOrganizacionalView Members


        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }


        #endregion







        #region IContaAuxiliarView Members


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
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ContaAuxiliarPresenter contaAuxiliar = new ContaAuxiliarPresenter(this);
            contaAuxiliar.Imprimir();
        }
    }
}
