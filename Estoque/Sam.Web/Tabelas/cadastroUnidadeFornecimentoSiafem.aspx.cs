using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;

namespace Sam.Web.Seguranca
{
    public partial class cadastroUnidadeFornecimentoSiafem : PageBase,  IUnidadeFornecimentoSiafemView
    {
       protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                UnidadeFornecimentoSiafemPresenter unidade = new UnidadeFornecimentoSiafemPresenter(this);
                unidade.Load();
                btnNovo.Enabled = true;
            }
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            //btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnNovo.Enabled = true;
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
            UnidadeFornecimentoSiafemPresenter unidadeFornecimento = new UnidadeFornecimentoSiafemPresenter(this);
            IList<UnidadeFornecimentoSiafemEntity> lista = unidadeFornecimento.PopularDadosUnidadeFornecimentoTodosCod();
            gridUnidade.DataSource = lista;
            gridUnidade.DataBind();

        }


        protected void grdItens_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridUnidade.PageIndex = e.NewPageIndex;
            PopularGrid();
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

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
                //this.btnNovo.Enabled = value;
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
                //this.btnExcluir.Enabled = value;
                //this.btnExcluir.Visible = false;
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

        protected void gridUnidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridUnidade.DataKeys[gridUnidade.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[2].Text);
            UnidadeFornecimentoSiafemPresenter unidade = new UnidadeFornecimentoSiafemPresenter(this);
            unidade.RegistroSelecionado();
            txtCodigo.Focus();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoSiafemPresenter unidade = new UnidadeFornecimentoSiafemPresenter(this);
            unidade.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoSiafemPresenter unidade = new UnidadeFornecimentoSiafemPresenter(this);
            unidade.Gravar();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoSiafemPresenter unidade = new UnidadeFornecimentoSiafemPresenter(this);
            unidade.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoSiafemPresenter unidadeFornecimento = new UnidadeFornecimentoSiafemPresenter(this);
            unidadeFornecimento.Imprimir();
        }
    }
}
