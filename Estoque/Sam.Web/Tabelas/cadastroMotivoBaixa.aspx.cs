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
    public partial class cadastroMotivoBaixa : PageBase,  IMotivoBaixaView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MotivoBaixaPresenter motivoBaixa = new MotivoBaixaPresenter(this);
                motivoBaixa.Load();
            }

            ScriptManager.RegisterStartupScript(this.upnCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                //this.pnlEditar.Visible = value;
            }
        }

        public void PopularGrid()
        {
            gridMotivoBaixa.DataSourceID = "sourceGridMotivoBaixa";
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

        public string CodigoTransacao
        {
            get { return txtCodigoTransacao.Text; }
            set { txtCodigoTransacao.Text = value; }
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

        public bool BloqueiaCodigoTransacao
        {
            set { this.txtCodigoTransacao.Enabled = value; }
        }

        #endregion

        #region IEstruturaOrganizacionalView Members
        
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        
        protected void gridMotivoBaixa_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridMotivoBaixa.DataKeys[gridMotivoBaixa.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridMotivoBaixa.Rows[gridMotivoBaixa.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridMotivoBaixa.Rows[gridMotivoBaixa.SelectedIndex].Cells[2].Text);

            if (gridMotivoBaixa.Rows[gridMotivoBaixa.SelectedIndex].FindControl("lblCodigoTransacao") != null)
            {
                Label lbl = (Label)gridMotivoBaixa.Rows[gridMotivoBaixa.SelectedIndex].FindControl("lblCodigoTransacao");
                this.CodigoTransacao = lbl.Text;
            }

            MotivoBaixaPresenter MotivoBaixa = new MotivoBaixaPresenter(this);
            MotivoBaixa.RegistroSelecionado();
            UpdatePanels();
        }

        protected void gridUGE_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        public void UpdatePanels()
        {
            upnGridDados.Update();
            upnCodigo.Update();
            upnDescricao.Update();
            upnBotoes.Update();
            updInconsistencia.Update();
            upnCodigoTransacao.Update();
        }
        
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            MotivoBaixaPresenter MotivoBaixa = new MotivoBaixaPresenter(this);
            MotivoBaixa.Novo();
            UpdatePanels();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            MotivoBaixaPresenter MotivoBaixa = new MotivoBaixaPresenter(this);
            MotivoBaixa.Gravar();
            UpdatePanels();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            MotivoBaixaPresenter MotivoBaixa = new MotivoBaixaPresenter(this);
            MotivoBaixa.Excluir();
            UpdatePanels();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            MotivoBaixaPresenter MotivoBaixa = new MotivoBaixaPresenter(this);
            MotivoBaixa.Cancelar();
            UpdatePanels();
        }

        #region IMotivoBaixaView Members


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
            MotivoBaixaPresenter motivoBaixa = new MotivoBaixaPresenter(this);
            motivoBaixa.Imprimir();
        }
    }
}
