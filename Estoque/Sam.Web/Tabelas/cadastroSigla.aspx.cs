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
    public partial class cadastroSigla : PageBase,  ISiglaView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SiglaPresenter sigla = new SiglaPresenter(this);
                sigla.Load();
            }

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            this.gridSigla.DataSourceID = "sourceGridSigla";
        }

        public void PopularListaOrgao()
        {
            this.ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public void PopularListaGestor(int _orgaoId)
        {
            this.ddlGestor.DataSourceID = "sourceListaGestor";
        }

        public void PopularListaIndicadorBemProprio()
        {
            this.ddlIndicadorBemProprio.DataSourceID = "sourceListaIndicadorBemProprio";
            this.ddlIndicadorBemProprio.DataBind();
            this.ddlIndicadorBemProprio.Items.Insert(0, " - Selecione - ");
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

        public string IndicadorBemProprioId
        {
            set
            {
                ListItem item = ddlIndicadorBemProprio.Items.FindByValue(value);

                if (item != null)
                {
                    ddlIndicadorBemProprio.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlIndicadorBemProprio.SelectedValue; }
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

        public bool BloqueiaListaIndicadorBemProprio
        {
            set 
            { 
                this.ddlIndicadorBemProprio.Enabled = value; 
            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                //this.pnlEditar.Visible = value;
            }
        }

        #endregion

        #region IEstruturaOrganizacionalView Members
        
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion


        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
            upnDropGestor.Update();
            UpdatePanels();
        }
                
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiglaPresenter sigla = new SiglaPresenter(this);
            sigla.Cancelar();
            gridSigla.PageIndex = 0;
            PopularGrid();
            UpdatePanels();
        }

        public void UpdatePanels()
        {
            upnCodigo.Update();
            upnDescricao.Update();
            upnDropIndicadorBemProprio.Update();
            upnGridDados.Update();
            updInconsistencia.Update();
            upnBotoes.Update();
            
        }

        protected void gridSigla_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridSigla.DataKeys[gridSigla.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridSigla.Rows[gridSigla.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridSigla.Rows[gridSigla.SelectedIndex].Cells[2].Text);

            if (gridSigla.Rows[gridSigla.SelectedIndex].FindControl("lblIndicadorBemProprioId") != null)
            {
                Label lbl = (Label)gridSigla.Rows[gridSigla.SelectedIndex].FindControl("lblIndicadorBemProprioId");
                this.IndicadorBemProprioId = Server.HtmlDecode(lbl.Text);
            }

            SiglaPresenter sigla = new SiglaPresenter(this);
            sigla.RegistroSelecionado();
            UpdatePanels();
            upnBotoes.Update();         
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            SiglaPresenter Sigla = new SiglaPresenter(this);
            Sigla.Novo();
            UpdatePanels();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            SiglaPresenter Sigla = new SiglaPresenter(this);
            Sigla.Gravar();

            upnBotoes.Update();
            UpdatePanels();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            SiglaPresenter Sigla = new SiglaPresenter(this);
            Sigla.Excluir();
            UpdatePanels();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            SiglaPresenter Sigla = new SiglaPresenter(this);
            Sigla.Cancelar();
            UpdatePanels();
        }

        protected void gridSigla_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SiglaPresenter sigla = new SiglaPresenter(this);
            sigla.Imprimir();
        }

        #region ISiglaView Members


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
                paramList.Add("CodigoOrgao", this.ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", this.ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
    }
}
