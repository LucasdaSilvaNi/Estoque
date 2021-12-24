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
using Sam.Domain.Entity;
using Sam.Presenter;

namespace Sam.Web.Seguranca
{
    public partial class cadastroNaturezaDespesa : PageBase,  INaturezaDespesaView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;

            if (!IsPostBack)
            {
                NaturezaDespesaPresenter presenter = new NaturezaDespesaPresenter(this);
                presenter.Load();
            }
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'8') ");

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridNatureza.DataSourceID = "sourceGridNatureza";
        }

        public void PopularListaAtividade()
        {
        }



        public bool AtividadeNaturezaDespesaId
        {
            set
            {
                ListItem item = ddlAtividade.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAtividade.ClearSelection();
                    item.Selected = true;
                }

            }
            get
            {
                return Convert.ToBoolean(ddlAtividade.SelectedValue);
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
            this.AtividadeNaturezaDespesaId = true;
            NaturezaDespesaPresenter uo = new NaturezaDespesaPresenter(this);
            uo.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            NaturezaDespesaPresenter uo = new NaturezaDespesaPresenter(this);
            uo.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            NaturezaDespesaPresenter uo = new NaturezaDespesaPresenter(this);
            uo.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            NaturezaDespesaPresenter uo = new NaturezaDespesaPresenter(this);
            uo.Cancelar();
        }

        protected void gridNatureza_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }



        protected void gridNatureza_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridNatureza.DataKeys[gridNatureza.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridNatureza.Rows[gridNatureza.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridNatureza.Rows[gridNatureza.SelectedIndex].Cells[2].Text);

            if (gridNatureza.Rows[gridNatureza.SelectedIndex].FindControl("lblId") != null)
            {
                Label lbl = (Label)gridNatureza.Rows[gridNatureza.SelectedIndex].FindControl("lblId");
                this.AtividadeNaturezaDespesaId = Convert.ToBoolean(Server.HtmlDecode(lbl.Text));
            }
            
            NaturezaDespesaPresenter uo = new NaturezaDespesaPresenter(this);
            uo.RegistroSelecionado();
            txtCodigo.Focus();
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
            NaturezaDespesaPresenter naturezaDespesa = new NaturezaDespesaPresenter(this);
            naturezaDespesa.Imprimir();
        }

        void INaturezaDespesaView.PopularListaAtividade()
        {

        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

    }
}
