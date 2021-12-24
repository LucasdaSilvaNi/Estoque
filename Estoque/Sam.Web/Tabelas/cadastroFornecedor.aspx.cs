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
    public partial class cadastroFornecedor : PageBase,  IFornecedorView 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FornecedorPresenter fornecedor = new FornecedorPresenter(this);
                fornecedor.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
            ScriptManager.RegisterStartupScript(this.txtFax, GetType(), "fax", "$('.fax').mask('(99)9999-9999');", true);            
            ScriptManager.RegisterStartupScript(this.txtCep, GetType(), "cep", "$('.cep').mask('99999-999');", true);
           
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        private void AdicionarMascaraCNPJCPF()
        {
            if(rdoCNPJ.Checked == true)
                ScriptManager.RegisterStartupScript(this.txtCpfCnpj, GetType(), "cnpjcpf", "$('.cnpjcpf').mask('99.999.999/9999-99');", true);
            else if (rdoCPF.Checked == true)
                ScriptManager.RegisterStartupScript(this.txtCpfCnpj, GetType(), "cnpjcpf", "$('.cnpjcpf').mask('999.999.999-99');", true);
        }
        
        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridFornecedor.DataSourceID = "sourceGridFornecedor";
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
            get { return txtCpfCnpj.Text; }
            set { txtCpfCnpj.Text = value; }
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

        public string EnderecoCep
        {
            get { return txtCep.Text; }
            set { txtCep.Text = value; }
        }

        public string EnderecoCidade
        {
            get { return txtCidade.Text; }
            set { txtCidade.Text = value; }
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

        public string Email
        {
            get { return txtEmail.Text; }
            set { txtEmail.Text = value; }
        }

        public string CNPJCPF
        {
            get { return txtCpfCnpj.Text; }
            set { txtCpfCnpj.Text = value; }
        }

        public string InformacoesComplementares
        {
            get { return txtInfoComplementares.Text; }
            set { txtInfoComplementares.Text = value; }
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
                this.txtCpfCnpj.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set { this.txtNome.Enabled = value; }
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

        public bool BloqueiaEnderecoBairro
        {
            set { this.txtBairro.Enabled = value; }
        }

        public bool BloqueiaEnderecoCidade
        {
            set { this.txtCidade.Enabled = value; }
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

        public bool BloqueiaEmail
        {
            set { this.txtEmail.Enabled = value; }
        }

        public bool BloqueiaListaUF
        {
            set { this.ddlUf.Enabled = value; }
        }

        public bool BloqueiaInformacoesComplementares
        {
            set { this.txtInfoComplementares.Enabled = value; }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }
        
        public void PopularListaUF()
        {
            this.ddlUf.DataSourceID = "sourceListaUF";
            this.ddlUf.DataBind();
            this.ddlUf.Items.Insert(0, " - Selecione - ");
        }
        
        #endregion

        #region IEstruturaOrganizacionalView Members
        
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        #endregion

        protected void gridFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridFornecedor.DataKeys[gridFornecedor.SelectedIndex].Value.ToString();
            this.Descricao = Server.HtmlDecode(gridFornecedor.Rows[gridFornecedor.SelectedIndex].Cells[2].Text);

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCPFCNPJ") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCPFCNPJ");

                if (lbl.Text.Length == 11)
                {
                    rdoCNPJ.Checked = false;
                    rdoCPF.Checked = true;
                    txtCpfCnpj.MaxLength = 11;
                    ScriptManager.RegisterStartupScript(this.txtCpfCnpj, GetType(), "cnpjcpf", "$('.cnpjcpf').mask('999.999.999-99');", true);
                }
                else
                {
                    rdoCNPJ.Checked = true;
                    rdoCPF.Checked = false;
                    txtCpfCnpj.MaxLength = 14;
                    ScriptManager.RegisterStartupScript(this.txtCpfCnpj, GetType(), "cnpjcpf", "$('.cnpjcpf').mask('99.999.999/9999-99');", true);
                }

                this.Codigo = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblLogradouro") != null)
            {
                Label lbl = (Label)gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblLogradouro");
                this.EnderecoLogradouro = lbl.Text;
            }
            
            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblNumero") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblNumero");
                this.EnderecoNumero = lbl.Text;
            }
            
            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblComplemento") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblComplemento");
                this.EnderecoComplemento = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblBairro") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblBairro");
                this.EnderecoBairro = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCep") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCep");
                this.EnderecoCep = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCidade") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCidade");
                this.EnderecoCidade = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblUfId") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblUfId");
                this.UfId = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblTelefone") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblTelefone");
                this.EnderecoTelefone = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblFax") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblFax");
                this.EnderecoFax = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblEmail") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblEmail");
                this.Email = lbl.Text;
            }

            if (gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblInfoComplementares") != null)
            {
                Label lbl = (Label) gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblInfoComplementares");
                this.InformacoesComplementares = lbl.Text;
            }

            FornecedorPresenter fornercedor = new FornecedorPresenter(this);
            fornercedor.RegistroSelecionado();
        }

        protected void btnAjuda_Click(object sender, EventArgs e) { }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            FornecedorPresenter fornecedor = new FornecedorPresenter(this);
            fornecedor.Novo();
            AdicionarMascaraCNPJCPF();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            FornecedorPresenter fornecedor = new FornecedorPresenter(this);
            fornecedor.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            FornecedorPresenter fornecedor = new FornecedorPresenter(this);
            fornecedor.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FornecedorPresenter fornecedor = new FornecedorPresenter(this);
            fornecedor.Cancelar();
        }

        #region IFornecedorView Members


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

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            FornecedorPresenter fornecedor = new FornecedorPresenter(this);
            fornecedor.Imprimir();
        }

        protected void rdoCNPJ_CheckedChanged(object sender, EventArgs e)
        {
            CNPJCPF = string.Empty;
            txtCpfCnpj.MaxLength = 11;
            AdicionarMascaraCNPJCPF();
        }

        protected void rdoCPF_CheckedChanged(object sender, EventArgs e)
        {
            CNPJCPF = string.Empty;
            txtCpfCnpj.MaxLength = 14;
            AdicionarMascaraCNPJCPF();
        }

    }
}
