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
    public partial class cadastroUA : PageBase,  IUAView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UAPresenter UA = new UAPresenter(this);
                UA.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'7') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");            
        }

        #region IUAView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        public string UAVinculada
        {
            get
            {
                return txtUaVinculada.Text;
            }
            set
            {
                txtUaVinculada.Text = value;
            }
        }

        public string GestorId
        {
            get
            {
                GestorPresenter gestorPresenter = null;
                int gestorID = 0;
                string strGestor_ID = null;


                if (ddlOrgao.Items.Count > 0)
                {
                    gestorPresenter = new GestorPresenter();
                    gestorID = gestorPresenter.RetornaGestorOrganizacional(Int32.Parse(this.OrgaoId), Int32.Parse(this.UoId), Int32.Parse(this.UgeId));
                    strGestor_ID = gestorID.ToString();
                }

                return strGestor_ID;
            }

            set { }
        }

        public bool IndicadorAtividadeId
        {
            get
            {
                return Convert.ToBoolean(ddlIndicadorAtividade.SelectedValue);
            }
            set
            {

                ListItem item = ddlIndicadorAtividade.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlIndicadorAtividade.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public string UnidadeId
        {
            get
            {
                return ddlUnidade.SelectedValue;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    ddlUnidade.SelectedValue = value;
                }
            }
        }

        public string CentroCustoId
        {
            get
            {
                return ddlCentroCusto.SelectedValue;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    ddlCentroCusto.SelectedValue = value;
                }
            }
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public void PopularListaUO(int? OrgaoId)
        {
            ddlUO.DataSourceID = "sourceListaUO";
        }

        public void PopularListaUGE(int? UoId)
        {
            ddlUGE.DataSourceID = "sourceListaUGE";
        }

        public void PopularGrid()
        {
            gridUA.DataSourceID = "sourceGridUA";
        }

        public void PopularListaUnidade()
        {
            UnidadePresenter unid = new UnidadePresenter();
            ddlUnidade.Items.Clear();
            ddlUnidade.DataSource = unid.PopularDadosTodosCod(Convert.ToInt32(ddlOrgao.SelectedValue));
            //ddlUnidade.DataSource = unid.PopularDadosUnidade(0, 0, (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id);
            ddlUnidade.DataBind();

//            ddlUnidade.DataSourceID = "sourceListaUnidade";
        }

        public void PopularListaCentroCusto()
        {
            CentroCustoPresenter cc = new CentroCustoPresenter();
            ddlCentroCusto.Items.Clear();
//            ddlCentroCusto.DataSource = cc.PopularDadosCentroCusto(0, 0 , (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id);
            //    , new GestorPresenter().RetornaGestorOrganizacional(
            //Common.Util.TratamentoDados.TryParseInt32(ddlOrgao.SelectedValue),
            //Common.Util.TratamentoDados.TryParseInt32(ddlUO.SelectedValue),
            //Common.Util.TratamentoDados.TryParseInt32(ddlUGE.SelectedValue)));
//            ddlCentroCusto.DataBind();
            ddlCentroCusto.DataSourceID = "sourceListaCentroCusto";
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoUO",this.ddlUO.SelectedValue.ToString());
                paramList.Add("DescricaoUO", this.ddlUO.SelectedItem.Text);
                paramList.Add("CodigoUGE", this.ddlUGE.SelectedValue.ToString());
                paramList.Add("DescricaoUGE", this.ddlUGE.SelectedItem.Text);
                
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region ICrudView Members

        public string OrgaoId
        {
            get
            {
                return ddlOrgao.SelectedValue;
            }
        }

        public string UoId
        {
            get
            {
                return ddlUO.SelectedValue;
            }
        }

        public string UgeId
        {
            get
            {
                return ddlUGE.SelectedValue;
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
                this.ListInconsistencias.DataBind();
            }
        }

        public bool BloqueiaNovo
        {
            set
            {
                this.btnNovo.Enabled = value;
            }
        }

        public void VerificaNovo()
        {
            if (this.ddlOrgao.SelectedValue != "0" && this.ddlUO.SelectedValue != "0" && this.ddlUGE.SelectedValue != "0" && this.ddlOrgao.SelectedValue != "" && this.ddlUO.SelectedValue != "" && this.ddlUGE.SelectedValue != "")
            {
                this.btnNovo.Enabled = true;
                this.btnImprimir.Enabled = true;
            }
            else
            {
                this.btnNovo.Enabled = false;
                this.btnImprimir.Enabled = false;
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
                txtCodigo.Enabled = value;
            }
        }

        public bool BloqueiaListaUnidade
        {
            set
            {
                this.ddlUnidade.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set
            {
                this.txtDescricao.Enabled = value;
            }
        }

        public bool BloqueiaUAVinculada
        {
            set
            {
                this.txtUaVinculada.Enabled = value;
            }
        }

        public bool BloqueiaListaCentroCusto
        {
            set
            {
                this.ddlCentroCusto.Enabled = value;
            }
        }

        public bool BloqueiaListaIndicadorAtividade
        {
            set
            {
                this.ddlIndicadorAtividade.Enabled = value;
            }
        }

      
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

        protected void gridUA_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridUA.DataKeys[gridUA.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridUA.Rows[gridUA.SelectedIndex].Cells[1].Text);

            if (gridUA.Rows[gridUA.SelectedIndex].FindControl("lblDescricaoUa") != null)
                this.Descricao = ((Label)gridUA.Rows[gridUA.SelectedIndex].FindControl("lblDescricaoUa")).Text.Replace(" (Inativa)", ""); 

            if (gridUA.Rows[gridUA.SelectedIndex].FindControl("lblUnidadeId") != null)
            {
                Label lbl = (Label)gridUA.Rows[gridUA.SelectedIndex].FindControl("lblUnidadeId");
                this.UnidadeId = lbl.Text;
            }

            if (gridUA.Rows[gridUA.SelectedIndex].FindControl("lblUaVinculada") != null)
            {
                Label lbl = (Label)gridUA.Rows[gridUA.SelectedIndex].FindControl("lblUaVinculada");
                this.UAVinculada = lbl.Text;
            }

            if (gridUA.Rows[gridUA.SelectedIndex].FindControl("lblCentroCustoId") != null)
            {
                Label lbl = (Label)gridUA.Rows[gridUA.SelectedIndex].FindControl("lblCentroCustoId");
                this.CentroCustoId = lbl.Text;
            }

            if (gridUA.Rows[gridUA.SelectedIndex].FindControl("lblIndicadorAtividadeId") != null)
            {
                Label lbl = (Label)gridUA.Rows[gridUA.SelectedIndex].FindControl("lblIndicadorAtividadeId");
                this.IndicadorAtividadeId = Convert.ToBoolean(lbl.Text);
            }

            UAPresenter UA = new UAPresenter(this);
            UA.RegistroSelecionado();
            txtCodigo.Focus();

            this.PopularListaUnidade();
            this.PopularListaCentroCusto();
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Cancelar();
            this.PopularListaUO(int.MaxValue);
            this.PopularListaUGE(int.MaxValue);
            this.PopularGrid();
        }

        protected void ddlUO_SelectedIndexChanged(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Cancelar();
            this.PopularListaUGE(int.MaxValue);
            this.PopularGrid();
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Cancelar();
            this.PopularGrid();
        }

        protected void ddlOrgao_DataBound(object sender, EventArgs e)
        {
            if (ddlOrgao.Items.Count > 0){
                UAPresenter UA = new UAPresenter(this);
                UA.Cancelar();
                this.PopularListaUO(int.MaxValue);
                this.PopularListaUGE(int.MaxValue);
                //this.PopularListaUnidade();

                //this.PopularListaUnidade(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
                //this.PopularListaUnidade(int.MaxValue);
                //this.PopularListaCentroCusto(int.MaxValue);
                //this.PopularListaCentroCusto();
                this.PopularGrid();
            }
            this.VerificaNovo();
        }

        protected void ddlUO_DataBound(object sender, EventArgs e)
        {
            if (ddlUO.Items.Count > 0){
                UAPresenter UA = new UAPresenter(this);
                UA.Cancelar();
                this.PopularListaUGE(int.MaxValue);
                //this.PopularListaUnidade();
                //this.PopularListaCentroCusto();
                this.PopularGrid();
            }
            this.VerificaNovo();
        }

        protected void ddlUGE_DataBound(object sender, EventArgs e)
        {
            if (ddlUGE.Items.Count > 0){
                UAPresenter UA = new UAPresenter(this);
                UA.Cancelar();
                //this.PopularListaUnidade();
                //this.PopularListaCentroCusto();
                this.PopularGrid();
            }
            this.VerificaNovo();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Novo();
            this.PopularListaUnidade();
            this.PopularListaCentroCusto();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Gravar();
            PopularGrid();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Excluir();
            PopularGrid();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UAPresenter UA = new UAPresenter(this);
            UA.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UAPresenter ua = new UAPresenter(this);
            ua.Imprimir();
        }

        public void ListarUasTodosCodPorOrgao(int? OrgaoID)
        {
            throw new NotImplementedException();
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNumUA.Text))
                return;

            int _numUA = int.Parse(txtNumUA.Text);

            UAPresenter _presenter = new UAPresenter(this);
            var _ua = _presenter.ObterUAPorCodigo(_numUA, int.Parse(GestorId));

            if (_ua == null)
            {
                ExibirMensagem("Código de UA não existe.");
                return;
            }

            alterarSelecaoDropDown(ddlOrgao, _ua.Gestor.Orgao.Id);
            this.PopularListaUO(int.MaxValue);
            ddlUO.DataBind();

            alterarSelecaoDropDown(ddlUO, _ua.Uge.Uo.Id);
            this.PopularListaUGE(int.MaxValue);
            ddlUGE.DataBind();

            alterarSelecaoDropDown(ddlUGE, _ua.Uge.Id);
            this.PopularGrid();
        }

        public void alterarSelecaoDropDown(DropDownList lista, int? valor)
        {
            lista.SelectedValue = TratamentoDados.ParseIntNull(valor).ToString();
        }

    }
}
