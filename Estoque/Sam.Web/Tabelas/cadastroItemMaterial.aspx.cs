using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;


namespace Sam.Web.Seguranca
{
    public partial class cadastroItemMaterial : PageBase,  IItemMaterialView
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;
            ItemMaterialPresenter objPresenter = new ItemMaterialPresenter(this);

            if (!IsPostBack)
            {
                //ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
                //classe.Load();
                objPresenter.Load();
                btnConsultarSiafisico.CssClass = "esconderControle";
                pnlSiafisico.Visible = false;
            }
            txtCodigo.Attributes.Add("onkeypress", "return SomenteNumero(event);");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnGravarSiafisico.Attributes.Add("OnClick", "return confirm('Pressione OK para integrar o item Siafisico ao SAM.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            this.ExibeBotaoImportacaoSiafisico = objPresenter.IsAdministradorOrgaoOuSuperior();
                            
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
            gridItemMaterial.DataSourceID = "sourceGridItemMaterial";
        }

        public void PopularListaGrupo()
        {
            ddlGrupo.DataSourceID = "sourceListaGrupo";
        }

        public void PopularListaClasse()
        {
            ddlClasse.DataSourceID = "sourceListaClasse";
        }

        public void PopularListaMaterial()
        {
            ddlMaterial.DataSourceID = "sourceListaMaterial";
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

        public string MaterialId
        {
            get
            {

                return ddlMaterial.SelectedValue;
            }
        }

        public string AtividadeItemMaterialId
        {
            set
            {
                ListItem item = ddlAtividade.Items.FindByValue(value);
                if (item != null)
                {
                    ddlAtividade.ClearSelection();
                    item.Selected = true;
                }

            }
            get
            {
                return ddlAtividade.SelectedValue;
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

        public string GrupoId
        {
            get
            {
                return ddlGrupo.SelectedValue;
            }

        }

        public IList ListaErros
        {

            set
            {
                ListInconsistencias.ExibirLista(value);
            }
        }

        public bool BloqueiaNovo
        {
            set
            {
                // this.btnNovo.Enabled = value;
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

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.Novo();
            HabilitarControlesSiafisico = false;
            BloqueiaSiafisico = true;
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
            HabilitarControlesSiafisico = false;
        }


        private void Cancelar()
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.Cancelar();
        }
        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridItemMaterial.PageIndex = 0;

            this.PopularListaClasse();
            this.PopularListaMaterial();            
            PopularGrid();
            this.Cancelar();
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridItemMaterial.PageIndex = 0;
            this.PopularListaMaterial();
            PopularGrid();
            this.Cancelar();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridItemMaterial.PageIndex = 0;
            PopularGrid();
            this.Cancelar();            
        }

        


        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }



        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridItemMaterial.DataKeys[gridItemMaterial.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].Cells[1].Text);
            this.Descricao = Server.HtmlDecode(gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].Cells[2].Text);

            if (gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].FindControl("lblId") != null)
            {
                Label lblId = (Label)gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].FindControl("lblId");
                this.AtividadeItemMaterialId = lblId.Text;
            }
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.RegistroSelecionado();
            HabilitarControlesSiafisico = false;
            BloqueiaSiafisico = true;
            txtCodigo.Focus();
        }


        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());
                paramList.Add("CodigoGrupoMaterial", ddlGrupo.SelectedValue.ToString());
                paramList.Add("DescricaoGrupoMaterial", this.ddlGrupo.SelectedItem.Text);
                paramList.Add("CodigoClasseMaterial", ddlClasse.SelectedValue.ToString());
                paramList.Add("DescricaoClasseMaterial", this.ddlClasse.SelectedItem.Text);
                paramList.Add("CodigoMaterial", this.ddlMaterial.SelectedValue.ToString());
                paramList.Add("DescricaoMaterial", this.ddlMaterial.SelectedItem.ToString());

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public void LimparDadosSiafisico()
        {
            txtSiafCodClasse.Text = "";
            txtSiafDescClasse.Text = "";
            txtSiafCodGrupo.Text = "";
            txtSiafDescGrupo.Text = "";
            txtSiafCodItemMaterial.Text = "";
            txtSiafDescItemMaterial.Text = "";
            txtSiafCodMaterial.Text = "";
            txtSiafDescMaterial.Text = "";
            txtSiafItemNat1.Text = "";
            txtSiafItemNat2.Text = "";
            txtSiafItemNat3.Text = "";
            txtSiafItemNat4.Text = "";
            txtSiafItemNat5.Text = "";
        }

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

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter itemMaterial = new ItemMaterialPresenter(this);
            itemMaterial.Imprimir();
        }

        public void PopularListaIndicadorAtividade()
        {
            throw new NotImplementedException();
        }

        protected void btnSiafisico_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            classe.Novo();
            txtCodigo.Focus();
            HabilitarControlesSiafisico = true;
        }

        protected bool HabilitarControlesSiafisico
        {
            set
            {
                txtDescricao.Visible = !value;
                lblDescricao.Visible = !value;
                ddlAtividade.Visible = !value;
                lblAtividade.Visible = !value;
                if (value)
                {
                    btnConsultarSiafisico.CssClass = "mostrarControle";
                    btnSiafisico.Enabled = false;
                }
                else
                {
                    btnConsultarSiafisico.CssClass = "esconderControle";
                    pnlSiafisico.Visible = value;
                    btnSiafisico.Enabled = true;
                }
                btnGravarSiafisico.Visible = value;
                btnGravar.Visible = !value;
            }
        }

        protected void btnConsultarSiafisico_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter presenter = new ItemMaterialPresenter(this);
            if(presenter.ConsultarSiafisico(txtCodigo.Text))
                pnlSiafisico.Visible = true;
            else
                pnlSiafisico.Visible = false;
        }

        public string SiafCodGrupo
        {
            get { return txtSiafCodGrupo.Text; }
            set { txtSiafCodGrupo.Text = value; }
        }

        public string SiafDescGrupo
        {
            get { return txtSiafDescGrupo.Text; }
            set { txtSiafDescGrupo.Text = value; }
        }

        public string SiafCodClasse
        {
            get { return txtSiafCodClasse.Text; }
            set { txtSiafCodClasse.Text = value; }
        }

        public string SiafDescClasse
        {
            get { return txtSiafDescClasse.Text; }
            set { txtSiafDescClasse.Text = value; }
        }

        public string SiafCodMaterial
        {
            get { return txtSiafCodMaterial.Text; }
            set { txtSiafCodMaterial.Text = value; }
        }

        public string SiafDescMaterial
        {
            get { return txtSiafDescMaterial.Text; }
            set { txtSiafDescMaterial.Text = value; }
        }

        public string SiafCodItem
        {
            get { return txtSiafCodItemMaterial.Text; }
            set { txtSiafCodItemMaterial.Text = value; }
        }

        public string SiafDescItem
        {
            get { return txtSiafDescItemMaterial.Text; }
            set { txtSiafDescItemMaterial.Text = value; }
        }

        public string SiafNatDesp1
        {
            get { return txtSiafItemNat1.Text; }
            set { txtSiafItemNat1.Text = value; }
        }

        public string SiafNatDesp2
        {
            get { return txtSiafItemNat2.Text; }
            set { txtSiafItemNat2.Text = value; }
        }

        public string SiafNatDesp3
        {
            get { return txtSiafItemNat3.Text; }
            set { txtSiafItemNat3.Text = value; }
        }

        public string SiafNatDesp4
        {
            get { return txtSiafItemNat4.Text; }
            set { txtSiafItemNat4.Text = value; }
        }

        public string SiafNatDesp5
        {
            get { return txtSiafItemNat5.Text; }
            set { txtSiafItemNat5.Text = value; }
        }

        protected void btnGravarSiafisico_Click(object sender, EventArgs e)
        {
            ItemMaterialPresenter classe = new ItemMaterialPresenter(this);
            if (pnlSiafisico.Visible)
                classe.GravarSiafisico();
            else
                ExibirMensagem("Consultar o Código do item material primeiro!");
        }


        public bool BloqueiaSiafisico
        {
            set
            {
                this.btnSiafisico.Enabled = !value;
            }
        }

        public bool ExibeSiafisico
        {
            set
            {
                this.pnlSiafisico.Visible = value;
            }
        }

        public bool ExibeBotaoImportacaoSiafisico
        {
            set
            {
                this.btnSiafisico.Visible = value;
            }
        }
    }
}
