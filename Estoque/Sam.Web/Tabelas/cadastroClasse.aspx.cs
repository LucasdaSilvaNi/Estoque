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
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.View;
namespace Sam.Web.Seguranca
{
    public partial class cadastroClasse : PageBase,  IClasseView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!IsPostBack)
            {
                ClassePresenter classe = new ClassePresenter(this);
                classe.Load();
            }
            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            if (ddlGrupo.SelectedIndex >= 0)
                this.CodigoGrupo = ddlGrupo.SelectedItem.Text.Remove(2);            
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
            gridClasse.DataSourceID = "sourceGridClasse";
        }

        public void PopularListaGrupo()
        {
            ddlGrupo.DataSourceID = "sourceListaGrupo";
        }

        public string CodigoGrupo
        {
            get
            {
                return txtCodGrupo.Text;
            }
            set
            {
                txtCodGrupo.Text = value;
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

        public string GrupoId
        {
            get
            {
                return ddlGrupo.SelectedValue;
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

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());

                paramList.Add("CodigoGrupo", ddlGrupo.SelectedValue.ToString());
                paramList.Add("DescricaoGrupo", this.ddlGrupo.SelectedItem.Text);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ClassePresenter classe = new ClassePresenter(this);
            classe.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ClassePresenter classe = new ClassePresenter(this);            
            this.Codigo = string.Format("{0}{1}", this.CodigoGrupo.Trim(), this.Codigo.Trim().PadLeft(2,'0'));

            classe.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ClassePresenter classe = new ClassePresenter(this);
            classe.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ClassePresenter classe = new ClassePresenter(this);
            classe.Cancelar();
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridClasse.PageIndex = 0;
            PopularGrid();
            ClassePresenter classe = new ClassePresenter(this);
            classe.Cancelar();
        }

        protected void gridClasse_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void gridClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridClasse.DataKeys[gridClasse.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridClasse.Rows[gridClasse.SelectedIndex].Cells[1].Text.Remove(0, 2));
            this.Descricao = Server.HtmlDecode(gridClasse.Rows[gridClasse.SelectedIndex].Cells[2].Text);
            ClassePresenter classe = new ClassePresenter(this);
            classe.RegistroSelecionado();
            txtCodigo.Focus();
        }

        #region IEstruturaOrganizacionalView Members


        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ClassePresenter classe = new ClassePresenter(this);
            classe.Imprimir();
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

    }
}
