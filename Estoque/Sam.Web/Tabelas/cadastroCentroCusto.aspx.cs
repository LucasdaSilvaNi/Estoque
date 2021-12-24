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
    public partial class cadastroCentroCusto : PageBase,  ICentroCustoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
                centroCusto.Load();
            }

            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");

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
            gridCentroCusto.DataSourceID = "sourceGridCentroCusto";
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
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Cancelar();

            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }
                
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Cancelar();

            gridCentroCusto.PageIndex = 0;
            PopularGrid();
        }

        protected void gridCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridCentroCusto.DataKeys[gridCentroCusto.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridCentroCusto.Rows[gridCentroCusto.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridCentroCusto.Rows[gridCentroCusto.SelectedIndex].Cells[2].Text);
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.RegistroSelecionado();
            txtCodigo.Focus();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            CentroCustoPresenter centroCusto = new CentroCustoPresenter(this);
            centroCusto.Imprimir();
        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            gridCentroCusto.PageIndex = 0;
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
    }
}
