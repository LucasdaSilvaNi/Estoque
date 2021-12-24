using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.View;
using System.Collections;
using Sam.Common.Util;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroFontesRecurso : PageBase,  IFontesRecursoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FontesRecursoPresenter fontesRecurso = new FontesRecursoPresenter(this);
                gridFontesRecurso.PageIndex = 0;
                fontesRecurso.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        #region ICrudView Members

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

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        public void PopularGrid()
        {
            gridFontesRecurso.DataSourceID = "sourceGrid";
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        protected void gridFontesRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridFontesRecurso.DataKeys[gridFontesRecurso.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridFontesRecurso.Rows[gridFontesRecurso.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridFontesRecurso.Rows[gridFontesRecurso.SelectedIndex].Cells[2].Text);
            FontesRecursoPresenter fontesRecurso = new FontesRecursoPresenter(this);
            fontesRecurso.RegistroSelecionado();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            FontesRecursoPresenter fontesRecursos = new FontesRecursoPresenter(this);
            fontesRecursos.Novo();
            PopularGrid();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            FontesRecursoPresenter fontesRecursos = new FontesRecursoPresenter(this);
            fontesRecursos.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            FontesRecursoPresenter fontesRecursos = new FontesRecursoPresenter(this);
            fontesRecursos.Excluir();
            PopularGrid();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FontesRecursoPresenter fontesRecursos = new FontesRecursoPresenter(this);
            fontesRecursos.Cancelar();
        }

        protected void gridFontesRecurso_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PopularGrid();
        }

        #region IFontesRecursoView Members

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
            FontesRecursoPresenter fontesRecurso = new FontesRecursoPresenter(this);
            fontesRecurso.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }
    }
}
