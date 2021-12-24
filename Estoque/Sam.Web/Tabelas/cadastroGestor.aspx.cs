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
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;


namespace Sam.Web.Seguranca
{
    public partial class cadastroGestor : PageBase,  IGestorView 
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                GestorPresenter gestor = new GestorPresenter(this);
                gestor.Load();
            }
            ScriptManager.RegisterStartupScript(this.txtTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
            ScriptManager.RegisterStartupScript(this.txtCodigoGestao, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Cancelar();

            gridGestor.PageIndex = 0;
            PopularListaUo();
            PopularListaUge();
            PopularGrid();
        }

        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                if (value == false)
                    this.pnlEditar.CssClass = "esconderControle";
                else
                    this.pnlEditar.CssClass = "mostrarControle";
            }
        }

        public void PopularGrid()
        {
            ddlUo.CssClass = "esconderControle";
            ddlUGE.CssClass = "esconderControle";
            //lblTipo.CssClass = "esconderControle";
            this.gridGestor.DataSourceID = "sourceGridGestor";
        }
        
        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public void PopularListaUo()
        {
            ddlUo.DataSourceID = "sourceListaUo";
            ddlUo.CssClass = "mostrarControle";
            ddlUGE.CssClass = "esconderControle";
        }

        public void PopularListaUge()
        {
            ddlUGE.DataSourceID = "sourceListaUge";
            ddlUGE.CssClass = "mostrarControle";
            ddlUo.CssClass = "esconderControle";
        }

        public string Codigo
        {
            get;
            set;
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

        public string Nome
        {
            get
            {
                return txtNome.Text;
            }
            set
            {
                txtNome.Text = value;
            }
        }

        public string NomeReduzido
        {
            get { return txtNomeReduzido.Text; }
            set { txtNomeReduzido.Text = value; }
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

        public string EnderecoTelefone
        {
            get { return txtTelefone.Text; }
            set { txtTelefone.Text = value; }
        }

        public string CodigoGestao
        {
            get
            {return txtCodigoGestao.Text;}
            set 
            { txtCodigoGestao.Text = value;}
        }
                 
        public string Descricao
        {
            get { return txtNome.Text; }
            set { txtNome.Text = value; }
        }

        public string OrgaoId
        {
           get { return ddlOrgao.SelectedValue; }
            set { ddlOrgao.SelectedValue = value; }
        }

        public string LogotipoImgUrl
        {
            get { return imgGestor.ImageUrl; }
            set { imgGestor.ImageUrl = value; }
        }

        public string UoId
        {
            set
            {
                ddlUo.ClearSelection();
                ListItem item = ddlUo.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUo.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlUo.SelectedValue; }
        }

        public string UgeId
        {
            set
            {
                ddlUGE.ClearSelection();
                ListItem item = ddlUGE.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlUGE.SelectedValue; }
        }

        public string TipoId
        {
            set
            {
                ListItem item = rblTipo.Items.FindByValue(value);

                if (item != null)
                {
                    rblTipo.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return rblTipo.SelectedValue; }
        }

        public byte[] Logotipo
        {
            get { return fileUploadGestor.FileBytes; }
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
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
               
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

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
            set{}
        }

        public bool BloqueiaDescricao
        {
            set{ this.txtNome.Enabled = value;} 
        }

        public bool BloqueiaNomeReduzido
        {
            set { this.txtNomeReduzido.Enabled = value; }
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

        public bool BloqueiaEnderecoTelefone
        {
            set { this.txtTelefone.Enabled = value; }
        }
                
        public bool BloqueiaCodigoGestao
        {
            set { this.txtCodigoGestao.Enabled = value; }
        }

        public bool BloqueiaListaUo
        {
            set 
            {
                if (value == false)
                    this.ddlUo.CssClass = "esconderControle";
                else
                    this.ddlUo.CssClass = "mostrarControle";
            }
        }

        public bool BloqueiaListaUge
        {
            set 
            {
                if (value == false)
                    this.ddlUGE.CssClass = "esconderControle";
                else
                    this.ddlUGE.CssClass = "mostrarControle";
            }
        }

        public bool BloqueiaFileUploadGestor
        {
            set { this.fileUploadGestor.Enabled = value; }
        }

        public bool BloqueiaTipoGestor
        {
            set
            {
                this.rblTipo.Enabled = value; 
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

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Novo();
            txtNome.Focus();
            this.imgGestor.ImageUrl = "~/Imagens/imgNaoCadastrada.gif";
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
           GestorPresenter gestor = new GestorPresenter(this);
            gestor.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Cancelar();
        }

        protected void gridGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Cancelar();

            this.Id = gridGestor.DataKeys[gridGestor.SelectedIndex].Value.ToString();
            this.Nome = Server.HtmlDecode(gridGestor.Rows[gridGestor.SelectedIndex].Cells[1].Text);

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblTipoId") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblTipoId");
                this.TipoId = Server.HtmlDecode(lbl.Text);
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblIdUo") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblIdUo");
                if (lbl.Text != "")
                {
                    this.UoId = Server.HtmlDecode(lbl.Text);
                }
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblIdUge") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblIdUge");
                if (lbl.Text != "")
                {
                    this.UgeId = Server.HtmlDecode(lbl.Text);
                }
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblNomeReduzido") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblNomeReduzido");
                this.NomeReduzido = Server.HtmlDecode(lbl.Text);
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblLogradouro") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblLogradouro");
                this.EnderecoLogradouro = Server.HtmlDecode(lbl.Text);
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblNumero") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblNumero");
                this.EnderecoNumero = Server.HtmlDecode(lbl.Text);
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblCompl") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblCompl");
                this.EnderecoComplemento = Server.HtmlDecode(lbl.Text);
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblTelefone") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblTelefone");
                this.EnderecoTelefone = Server.HtmlDecode(lbl.Text);
                
            }

            if (gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblLogradouro") != null)
            {
                Label lbl = (Label)gridGestor.Rows[gridGestor.SelectedIndex].FindControl("lblCodigoGestao");
                int codigo = Convert.ToInt32(lbl.Text);
                lbl.Text = string.Format("{0:00000}", codigo);  
                this.CodigoGestao = Server.HtmlDecode(lbl.Text);
            }

            // adicionado um random para poder atualizar as imagens que acabaram de ser modificadas
            if (this.Id != null)
                this.LogotipoImgUrl = Constante.ImageUrl + "?id=" + this.Id + "&tipoImagem=" + (int)ImagemEnum.Gestor + "&n=" + new Random().Next(100000000, 999999999);

            gestor.RegistroSelecionado();
            txtNome.Focus();
        }

        protected void rblTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.TipoId = rblTipo.SelectedIndex.ToString();

            if (this.TipoId == "1")
            {
                PopularListaUo();
                lblTipo.CssClass = "labelFormulario";
            }
            else if (this.TipoId == "2")
            {
                PopularListaUge();
                lblTipo.CssClass = "labelFormulario";
            }
            else
            {
                ddlUGE.CssClass = "esconderControle";
                ddlUo.CssClass = "esconderControle";
                lblTipo.CssClass = "esconderControle";
            }

            GestorPresenter gestor = new GestorPresenter(this);
            gestor.RegistroSelecionado();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            GestorPresenter gestor = new GestorPresenter(this);
            gestor.Imprimir();
        }


    }
}
