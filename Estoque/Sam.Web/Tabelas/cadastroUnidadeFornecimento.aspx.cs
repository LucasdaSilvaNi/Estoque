using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;

namespace Sam.Web.Seguranca
{
    public partial class cadastroUnidadeFornecimento : PageBase,  IUnidadeFornecimentoView
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);
                unidade.Load();
                btnNovo.Enabled = false;
            }
            //txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'12')");

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

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString() );
                paramList.Add("DescricaoOrgao", ddlOrgao.SelectedItem.ToString());
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", ddlGestor.SelectedItem.ToString()); 

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
            UnidadeFornecimentoPresenter unidadeFornecimento = new UnidadeFornecimentoPresenter(this);
            unidadeFornecimento.Cancelar();
            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
            if (ddlGestor.Items.Count > 0)
            {
                btnNovo.Enabled = true;
            }
            else 
            {
                btnNovo.Enabled = false;
            }

        }
                
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidadeFornecimento = new UnidadeFornecimentoPresenter(this);
            unidadeFornecimento.Cancelar();

            gridUnidade.PageIndex = 0;
            PopularGrid();
        }

        protected void gridUnidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridUnidade.DataKeys[gridUnidade.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridUnidade.Rows[gridUnidade.SelectedIndex].Cells[2].Text);
            UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);
            unidade.RegistroSelecionado();
            txtCodigo.Focus();
        }

        protected void gridUGE_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);

            if (ddlGestor.Items.Count == 0) { 
                
            }
            unidade.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);
            unidade.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);
            unidade.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidade = new UnidadeFornecimentoPresenter(this);
            unidade.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidadeFornecimento = new UnidadeFornecimentoPresenter(this);
            unidadeFornecimento.Imprimir();
        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            UnidadeFornecimentoPresenter unidadeFornecimento = new UnidadeFornecimentoPresenter(this);
            unidadeFornecimento.Cancelar();

            gridUnidade.PageIndex = 0;
            PopularGrid();
            if (ddlGestor.Items.Count > 0)
            {
                btnNovo.Enabled = true;
            }
            else
            {
                btnNovo.Enabled = false;
            }
        }

    }
}
