using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Presenter;
using System.Collections;
using Sam.Common.Util;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroTipoIncorp : PageBase,  ITipoIncorpView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
                presenter.Load();
            }

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione Ok para Confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para Confirmar.');");
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

        public string CodigoTransacao
        {
            get
            {
                return txtCodigoTransacao.Text;
            }
            set
            {
                txtCodigoTransacao.Text = value;
            }
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
            set
            {
                btnNovo.Enabled = value;
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                btnGravar.Enabled = value;
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                btnExcluir.Enabled = value;
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                btnCancelar.Enabled = value;
            }
        }

        public bool BloqueiaCodigo
        {
            set
            {
                txtCodigo.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set
            {
                txtDescricao.Enabled = value;
            }
        }

        public bool BloqueiaCodigoTransacao
        {
            set
            {
                txtCodigoTransacao.Enabled = value;
            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                //this.pnlEditar.Visible = value;
            }
        }

        public void PopularGrid()
        {
            gridTipoIncorp.DataSourceID = "sourceGrid";
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
            presenter.Novo();
            UpdatePanels();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
            presenter.Gravar();
            PopularGrid();
            UpdatePanels();
        }

        private void UpdatePanels()
        {
            upnCodigo.Update();
            upnDescricao.Update();
            upnCodigotransacao.Update();
            upnGridDados.Update();
            upnIncosistencias.Update();
            upnBotoes.Update();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
            presenter.Excluir();
            PopularGrid();
            UpdatePanels();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
            presenter.Cancelar();
            UpdatePanels();
        }

        protected void gridTipoIncorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridTipoIncorp.DataKeys[gridTipoIncorp.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridTipoIncorp.Rows[gridTipoIncorp.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridTipoIncorp.Rows[gridTipoIncorp.SelectedIndex].Cells[2].Text);

            if (gridTipoIncorp.Rows[gridTipoIncorp.SelectedIndex].FindControl("lblCodigoTransacao") != null)
            {
                Label lbl = (Label)gridTipoIncorp.Rows[gridTipoIncorp.SelectedIndex].FindControl("lblCodigoTransacao");
                this.CodigoTransacao = lbl.Text;
            }

            TipoIncorpPresenter presenter = new TipoIncorpPresenter(this);
            presenter.RegistroSelecionado();
            UpdatePanels();
        }

        protected void gridTipoIncorp_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            TipoIncorpPresenter tipoIncorp = new TipoIncorpPresenter(this);
            tipoIncorp.Imprimir();
        }

        #region ITipoIncorpView Members


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
    }
}
