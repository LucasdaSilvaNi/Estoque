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
    public partial class cadastroUnidade : PageBase,  IUnidadeView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UnidadePresenter unidade = new UnidadePresenter(this);
                unidade.Load();
            }

            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'3') ");

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridUnidade.DataSourceID = "sourceGridUnidade";
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public void PopularListaGestor(int _orgaoId)
        {
            ddlGestor.DataSourceID = "sourceListaGestor";
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

        public string OrgaoId
        {
            get
            {
                return ddlOrgao.SelectedValue;
            }
        }
               
        public string GestorId
        {
            get
            {
                return ddlGestor.SelectedValue;
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

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());

                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);

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


        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Cancelar();

            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }
                
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Cancelar();
            gridUnidade.PageIndex = 0;
            PopularGrid();
        }

        protected void gridUnidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridUnidade.DataKeys[gridUnidade.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[2].Text);
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.RegistroSelecionado();
            txtCodigo.Focus();
        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Cancelar();
            gridUnidade.PageIndex = 0;
            PopularGrid();
            if (ddlGestor.Items.Count > 0)
            {
                btnNovo.Enabled = true;
                btnImprimir.Enabled = true;
            }
            else
            {
                btnNovo.Enabled = false;
                btnImprimir.Enabled = false;
            }

        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Novo();
            txtCodigo.Focus();

        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Imprimir();
        }

        protected void gridUnidade_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UnidadePresenter unidade = new UnidadePresenter(this);
            unidade.Cancelar();

        }

    }
}
