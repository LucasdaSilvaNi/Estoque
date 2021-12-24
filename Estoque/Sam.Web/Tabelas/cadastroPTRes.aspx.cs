using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroPTRes : PageBase,  IPTResView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PTResPresenter ptRes = new PTResPresenter(this);
                ptRes.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }
        public void PopularGrid()
        {
            gridPTRes.DataSourceID = "sourceGridPTRes";
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
                this.ListInconsistencias.ExibirLista(value);
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
                
        protected void gridPTRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridPTRes.DataKeys[gridPTRes.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridPTRes.Rows[gridPTRes.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridPTRes.Rows[gridPTRes.SelectedIndex].Cells[2].Text);

            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.RegistroSelecionado();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.Novo();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            PTResPresenter ptRes = new PTResPresenter(this);
            ptRes.Imprimir();
            
        }       
    }
}
