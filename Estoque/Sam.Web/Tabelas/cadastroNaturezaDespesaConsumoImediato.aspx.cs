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
    public partial class cadastroNaturezaDespesaConsumoImediato : PageBase,  INaturezaDespesaConsumoImediatoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
                objPresenter.Load();
            }
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'8') ");

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridNaturezaDespesaConsumoImediato.DataSourceID = "sourceGridNaturezaDespesaConsumoImediato";
        }

        public void PopularListaAtividade()
        {
        }



        public bool Ativa
        {
            set
            {
                ListItem item = ddlStatus.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlStatus.ClearSelection();
                    item.Selected = true;
                }

            }
            get
            {
                return Convert.ToBoolean(ddlStatus.SelectedValue);
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
                if (Session["Codigo"] != null)
                    retorno = Session["Codigo"].ToString();
                return retorno;
            }
            set
            {
                Session["Codigo"] = value;
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
            this.Ativa = true;
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.Cancelar();
        }

        protected void gridNaturezaDespesaConsumoImediato_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridNaturezaDespesaConsumoImediato.PageIndex = e.NewPageIndex;
            PopularGrid();
        }

        protected void gridNaturezaDespesaConsumoImediato_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }



        protected void gridNaturezaDespesaConsumoImediato_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.Id = gridNaturezaDespesaConsumoImediato.DataKeys[gridNaturezaDespesaConsumoImediato.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridNaturezaDespesaConsumoImediato.Rows[gridNaturezaDespesaConsumoImediato.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridNaturezaDespesaConsumoImediato.Rows[gridNaturezaDespesaConsumoImediato.SelectedIndex].Cells[2].Text);

            //if (gridNaturezaDespesaConsumoImediato.Rows[gridNaturezaDespesaConsumoImediato.SelectedIndex].FindControl("lblCodigoNaturezaDespesaConsumoImediato") != null)
            //{
            //    Label lblCodigoNaturezaDespesaConsumoImediato = (Label)gridNaturezaDespesaConsumoImediato.Rows[gridNaturezaDespesaConsumoImediato.SelectedIndex].FindControl("lblCodigoNaturezaDespesaConsumoImediato");
            //    this.Ativa = Convert.ToBoolean(Server.HtmlDecode(lblCodigoNaturezaDespesaConsumoImediato.Text));
            //}
            
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.RegistroSelecionado();
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
            NaturezaDespesaConsumoImediatoPresenter objPresenter = new NaturezaDespesaConsumoImediatoPresenter(this);
            objPresenter.Imprimir();
        }

        void INaturezaDespesaConsumoImediatoView.PopularListaNaturezasDespesaConsumoImediato()
        {
            PopularGrid();
            //throw new NotImplementedException();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

    }
}
