using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class cadastroTerceiro : PageBase,  ITerceiroView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TerceiroPresenter terceiro = new TerceiroPresenter(this);
                terceiro.Load();
            }

            ScriptManager.RegisterStartupScript(this.upnTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
            ScriptManager.RegisterStartupScript(this.upnCep, GetType(), "cep", "$('.cep').mask('99999-999');", true);
            ScriptManager.RegisterStartupScript(this.upnCnpj, GetType(), "cnpj","$('.cnpj').mask('99.999.999.9999-99');", true);
                       
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
                
        #region IEntidadesAuxiliaresView Members

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                //this.pnlEditar.Visible = value;
            }
        }
        
        public string OrgaoId
        {
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value);
                if (item != null)
                {
                    ddlOrgao.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlOrgao.SelectedValue; }
        }

        public void PopularListaGestor(int? _orgaoId)
        {
            ddlGestor.DataSourceID = "sourceListaGestor";
        }

        public string GestorId
        {
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value);
                if (item != null)
                {
                    ddlGestor.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlGestor.SelectedValue; }
        }
         
        public string Codigo
        {
            get
            {
                return txtCnpj.Text;
            }
            set
            {
                txtCnpj.Text = value;
            }
        }

        public string Descricao
        {
            get { return txtNome.Text; }
            set { txtNome.Text = value; }
        }

        public string EnderecoLogradouro
        {
            get { return txtLogradouro.Text; }
            set { txtLogradouro.Text = value; }
        }

        public string EnderecoNumero
        {
            get { return txtNumero.Text; }
            set { txtNumero.Text = value; }
        }

        public string EnderecoComplemento
        {
            get { return txtComplemento.Text; }
            set { txtComplemento.Text = value; }
        }

        public string EnderecoBairro
        {
            get { return txtBairro.Text; }
            set { txtBairro.Text = value; }
        }

        public string EnderecoCidade
        {
            get { return txtCidade.Text; }
            set { txtCidade.Text = value; }
        }

        public void PopularListaUF()
        {
            this.ddlUf.DataSourceID = "sourceListaUF";
            this.ddlUf.DataBind();
            this.ddlUf.Items.Insert(0, " - Selecione - ");
        }

        public string UfId
        {
            set
            {
                ListItem item = ddlUf.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUf.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlUf.SelectedValue; }
        }

        public string EnderecoCep
        {
            get { return txtCep.Text; }
            set { txtCep.Text = value; }
        }

        public string EnderecoTelefone
        {
            get { return txtTelefone.Text; }
            set { txtTelefone.Text = value; }
        }

        public string EnderecoFax
        {
            get { return txtFax.Text; }
            set { txtFax.Text = value; }
        }

        public void PopularGrid()
        {
            this.gridTerceiro.DataSourceID = "sourceGridTerceiro";
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
                this.txtCnpj.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set{ this.txtNome.Enabled = value;} 
        }
               
        public bool BloqueiaEnderecoLogradouro
        {
            set { this.txtLogradouro.Enabled = value; }
        }

        public bool BloqueiaEnderecoNumero
        {
            set { this.txtNumero.Enabled = value; }
        }

        public bool BloqueiaEnderecoComplemento
        {
            set { this.txtComplemento.Enabled = value; }
        }
                
        public bool BloqueiaEnderecoCidade
        {
            set { this.txtCidade.Enabled = value; }
        }

        public bool BloqueiaEnderecoBairro
        {
            set { this.txtBairro.Enabled = value; }
        }

        public bool BloqueiaEnderecoCep
        {
            set { this.txtCep.Enabled = value; }
        }

        public bool BloqueiaEnderecoTelefone
        {
            set { this.txtTelefone.Enabled = value; }
        }

        public bool BloqueiaEnderecoFax
        {
            set { this.txtFax.Enabled = value; }
        }
        
        public bool BloqueiaListaUF
        {
            set { this.ddlUf.Enabled = value; }
        }
        
        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", ddlOrgao.SelectedItem.ToString());
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", ddlGestor.SelectedItem.ToString());

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

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
            this.PopularListaGestor(TratamentoDados.TryParseInt32(this.OrgaoId));
            UpdatePanels();
        }
        
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.Cancelar();
            gridTerceiro.PageIndex = 0;
            PopularGrid();
            UpdatePanels();
        }

        public void UpdatePanels()
        {
            upnGridDados.Update();
            upnInconsistencia.Update();
            upnCnpj.Update();
            upnNome.Update();
            upnLogradouro.Update();
            upnComplemento.Update();
            upnNumero.Update();
            upnBairro.Update();
            upnMunicipio.Update();
            upnDropUF.Update();
            upnCep.Update();
            upnTelefone.Update();
            upnFax.Update();
            upnDropGestor.Update();
        }
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.Novo();
            UpdatePanels();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.Gravar();
            UpdatePanels(); 
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.Excluir();
            UpdatePanels();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.Cancelar();
            UpdatePanels();
        }

        protected void gridTerceiro_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridTerceiro.DataKeys[gridTerceiro.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridTerceiro.Rows[gridTerceiro.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridTerceiro.Rows[gridTerceiro.SelectedIndex].Cells[2].Text);

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblLogradouro") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblLogradouro");
                this.EnderecoLogradouro = Server.HtmlDecode(lbl.Text);
            }
            
            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblNumero") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblNumero");
                this.EnderecoNumero = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCompl") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCompl");
                this.EnderecoComplemento = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblBairro") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblbairro");
                this.EnderecoBairro = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCidade") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCidade");
                this.EnderecoCidade = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblUfId") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblUfId");
                this.UfId = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCep") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblCep");
                this.EnderecoCep = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblTelefone") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblTelefone");
                this.EnderecoTelefone = Server.HtmlDecode(lbl.Text);
            }

            if (gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblFax") != null)
            {
                Label lbl = (Label)gridTerceiro.Rows[gridTerceiro.SelectedIndex].FindControl("lblFax");
                this.EnderecoFax = Server.HtmlDecode(lbl.Text);
            }

            TerceiroPresenter Terceiro = new TerceiroPresenter(this);
            Terceiro.RegistroSelecionado();
            
            upnBotoes.Update();
            UpdatePanels();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            TerceiroPresenter terceiro = new TerceiroPresenter(this);
            terceiro.Imprimir();
        }
    }
}
