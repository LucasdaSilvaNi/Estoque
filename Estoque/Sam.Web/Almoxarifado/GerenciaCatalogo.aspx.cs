using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Entity;
using helperTratamento = Sam.Common.Util.TratamentoDados;
using System.Data;

namespace Sam.Web
{
    public partial class GerenciaCatalogo : PageBase, View.ISubItemMaterialView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();

        private void RemoverCache(string nome)
        {
            Cache.Remove(nome);
            Cache.Remove(string.Format("Perfil_{0}", nome));
            Application.Remove(nome);
            Session.Clear();
            HttpContext.Current.Cache.Remove(nome);


        }
        private void ExpirePageCache()
        {
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
         
            PesquisaSubitemNova.UsaSaldo = false;
            PesquisaSubitemNova.FiltrarAlmox = false;
            PesquisaSubitemNova.FiltraGestor = true;
            
           

            if (!String.IsNullOrEmpty(idSubItem.Value))
            {
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialBySubItem((int)Common.Util.TratamentoDados.TryParseInt32(idSubItem.Value));
                PopularListaGrupo();
            }

            if (!IsPostBack)
            {
                SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
                classe.Load();
                PopularListaNaturezaDespeza();
                ComboPesquisaDesbalitar("");
            }

            RegistraJavascript();
            HabilitarBotoes();
        }
   

        private void RegistraJavascript()
        {
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

            txtEstoqueMinimo.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
            txtEstoqueMaximo.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));

            ScriptManager.RegisterStartupScript(this.txtEstoqueMaximo, GetType(), "numerico", "$('.numerico').floatnumber(','," + base.numCasasDecimaisValorUnitario + ");", true);
            ScriptManager.RegisterStartupScript(this.txtEstoqueMinimo, GetType(), "numerico", "$('.numerico').floatnumber(','," + base.numCasasDecimaisMaterialQtde + ");", true);
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridSubItemMaterial.DataSourceID = "sourceGridSubItemMaterial";

        }

        public void PopularListaOrgao()
        {
        }

        public void PopularListaGestor()
        {
        }


        public void PopularListaGrupo()
        {
            ddlGrupo.DataSourceID = "odsListaGrupo";
        }

        public void PopularListaClasse()
        {
            ddlClasse.DataSourceID = "odsListaClasse";
        }

        public void PopularListaMaterial()
        {
            ddlMaterial.DataSourceID = "odsListaMaterial";
        }

        public void PopularListaItem()
        {
            ddlItem.DataSourceID = "odsListaItem";
        }

        public void PopularListaIndicadorAtividade()
        {

        }

        //public void PopularListaNaturezaDespeza()
        //{

        //}

        public void PopularListaContaAuxiliar()
        {

        }

        public void PopularListaUnidadeFornecimento()
        {
        }

        public string Codigo
        {
            set
            {
                lblCod.Text = value;
            }
        }

        public string ItemId
        {
            get
            {
                return ddlItem.SelectedValue;
            }
        }

        public string GestorId
        {
            get
            {
                int iGestor_ID = 0;

                var perfilLogado = new PageBase().GetAcesso.Transacoes.Perfis[0];
                var gestorAlmoxarifadoLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;

                if (gestorAlmoxarifadoLogado.HasValue)
                    iGestor_ID = gestorAlmoxarifadoLogado.Value;
                else
                    iGestor_ID = perfilLogado.GestorPadrao.Id.Value;

                return iGestor_ID.ToString();
            }

            set { }
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
                return lblDescricao.Text;
            }
            set
            {
                lblDescricao.Text = value;
            }
        }

        public string GrupoId
        {
            get
            {
                return ddlGrupo.SelectedValue;
            }

        }


        public string CodigoBarras
        {
            get;
            set;
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
                this.btnNovo.Enabled = false;
                this.btnNovo.Visible = false;
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

            }
        }

        public bool BloqueiaDescricao
        {
            set
            {

            }
        }

        public bool BloqueiaCodBarras
        {
            set
            {

            }
        }

        public bool BloqueiaNaturezaDespesa
        {
            set
            {

            }
        }

        public bool BloqueiaContaAuxiliar
        {
            set
            {

            }
        }

        public bool BloqueiaControlaLote
        {
            set
            {

            }
        }

        public bool BloqueiaExpandeDecimais
        {
            set
            {

            }
        }

        public bool BloqueiaPermiteFacionamento
        {
            set
            {

            }
        }

        public bool BloqueiaAtividade
        {
            set
            {

            }
        }

        public bool BloqueiaUnidadeFornecimento
        {
            get;
            set;
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        #endregion

        #region Eventos


        protected void ddlNatureza_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            PopularGrid();
        }

        protected void ddlGrupo_DataBound(object sender, EventArgs e)
        {
            if (ddlGrupo.Items.Count > 0)
                PopularListaClasse();
            else
                ddlClasse.Items.Clear();

            if (ItemPesquisado.Id != null)
            {
                ddlGrupo.Items.FindByValue(ItemPesquisado.GrupoId.ToString()).Selected = true;
            }
        }

        protected void ddlClasse_DataBound(object sender, EventArgs e)
        {
            if (ddlClasse.Items.Count > 0)
            {
                PopularListaMaterial();
            }
            else
                ddlMaterial.Items.Clear();

            if (ItemPesquisado.Id != null)
            {
                ddlClasse.Items.FindByValue(ItemPesquisado.ClasseId.ToString()).Selected = true;
            }
        }

        protected void ddlMaterial_DataBound(object sender, EventArgs e)
        {
            if (ddlMaterial.Items.Count > 0)
            {
                PopularListaItem();
            }
            else
                ddlItem.Items.Clear();

            if (ItemPesquisado.Id != null)
            {
                ddlMaterial.Items.FindByValue(ItemPesquisado.MaterialId.ToString()).Selected = true;
            }
        }

        protected void ddlItem_DataBound(object sender, EventArgs e)
        {
            //após carregar os itens pela busca, lipa as variáveis                
            if (string.IsNullOrEmpty(idSubItem.Value))
                LimparPesquisaItem();

            if (ItemPesquisado.Id != null)
            {
                ddlItem.Items.FindByValue(ItemPesquisado.Id.ToString()).Selected = true;
            }
            HabilitarBotoes();
            PopularGrid();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {

        }

        //protected void imgLupaSubItem_Click(object sender, EventArgs e)
        //{
        //    gridSubItemMaterial.Visible = true;


        //    if (string.IsNullOrEmpty(txtSubItem.Text))
        //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModalItem();", true);

        //}
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.GravarAlmox();

            btnNovo.Enabled = true;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Excluir();
            btnNovo.Enabled = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
            btnNovo.Enabled = true;
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();

            gridSubItemMaterial.PageIndex = 0;

            this.PopularListaClasse();
            PopularGrid();

            btnNovo.Enabled = false;

            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.Cancelar();
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ddlItem.Items.Clear();

            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaMaterial();
            PopularGrid();

            HabilitarBotoes();

            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.Cancelar();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaItem();
            PopularGrid();

            HabilitarBotoes();

            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.Cancelar();
        }

        protected void ddlUnidadeFornecimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* gridSubItemMaterial.PageIndex = 0;
             PopularGrid();

             HabilitarBotoes();

             SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
             subitensMaterial.Cancelar();*/
        }

        protected void gridSubItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        
        }
        protected void lnkDelete(Object sender, CommandEventArgs e)
        {
            int iStID = int.Parse(e.CommandArgument.ToString());
        }


        protected void gridSubItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridSubItemMaterial.DataKeys[gridSubItemMaterial.SelectedIndex].Value.ToString();


            GridViewRow row = gridSubItemMaterial.SelectedRow;

            CheckBox chkIndicador = (CheckBox)row.FindControl("chkIndicador");
            CheckBox chkDisp = (CheckBox)row.FindControl("chkDisp");
            CheckBox chkDispZerado = (CheckBox)row.FindControl("chkDispZerado");
            TextBox txtsaldoitemunit = (TextBox)row.FindControl("txtsaldoitemunit");
            TextBox txtEstoqueMinimo = (TextBox)row.FindControl("txtEstoqueMinimo");
            TextBox txtEstoqueMaximo = (TextBox)row.FindControl("txtEstoqueMaximo");

          

            //SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            //classe.SubItemId = int.Parse(thsi.Id);
            //classe.RegistroSelecionadoAlmox();

            int chkDispx = chkDisp.Checked == true ? 2 : 0;
            int chkDispZeradox = chkDispZerado.Checked == true ? 2 : 0;

            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.GravarAlmox(chkIndicador.Checked, chkDispx, chkDispZeradox, Convert.ToDecimal(txtEstoqueMinimo.Text), Convert.ToDecimal(txtEstoqueMaximo.Text));



            //HabilitarBotoes();
            ddlClasse.Focus();
        }

        protected void lnbsalvar(object sender, System.EventArgs e)
        {
            //ClientScript.RegisterClientScriptBlock(GetType(), "sas", "<script> alert('Inserted successfully');</script>", true);
            //CheckBox ChkBoxHeader = (CheckBox)gridSubItemMaterial.("chkDispZerado");
            //if (ddlIndicador.SelectedValue == "-1")
            //    ChkBoxHeader.Attributes.Add("checked", "checked");
            //else
            //    ChkBoxHeader.Attributes.Remove("checked");
        }

        protected void gridSubItemMaterial_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            btnImprimir.Visible = gridSubItemMaterial.Rows.Count == 1 ? true : false;

            var utilizaFormatoSIAFEM = this.FormatoSIAFEM();
            FormatarGridItem(e, utilizaFormatoSIAFEM);
            btnAtivar.Enabled = (bool)Session["acessoBotao"];
            btnDesativar.Enabled = (bool)Session["acessoBotao"];


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkIndicador = (CheckBox)e.Row.FindControl("chkIndicador");
                CheckBox chkDisp = (CheckBox)e.Row.FindControl("chkDisp");
                TextBox txtsaldoitemunit = (TextBox)e.Row.FindControl("txtsaldoitemunit");
                TextBox txtEstoqueMinimo = (TextBox)e.Row.FindControl("txtEstoqueMinimo");                
                TextBox txtEstoqueMaximo = (TextBox)e.Row.FindControl("txtEstoqueMaximo");
                LinkButton lnbSalvar = (LinkButton)e.Row.FindControl("lnbSalvar");
                LinkButton lnbExcluir = (LinkButton)e.Row.FindControl("lnbExcluir");


                if (Session["acessoBotao"] != null)
                {
                    chkIndicador.Enabled = (bool)Session["acessoBotao"];
                    chkDisp.Enabled = (bool)Session["acessoBotao"];
                    txtEstoqueMinimo.Enabled = (bool)Session["acessoBotao"];
                    txtsaldoitemunit.Enabled = (bool)Session["acessoBotao"];
                    txtEstoqueMaximo.Enabled = (bool)Session["acessoBotao"];
                    lnbSalvar.Enabled = (bool)Session["acessoBotao"];
                    lnbExcluir.Enabled = (bool)Session["acessoBotao"];
                }
               
           

            }
           


        

        }

        protected void FormatarGridItem(GridViewRowEventArgs gridViewEventArgs, bool utilizaFormatoSIAFEM)
        {
            var gridExibicao = gridViewEventArgs.Row.NamingContainer as GridView;
            GridViewRow rowGrid = gridViewEventArgs.Row;

            if (gridExibicao.IsNotNull() && rowGrid.IsNotNull() && rowGrid.RowType == DataControlRowType.DataRow)
            {
                for (int iContador = 0; iContador < gridExibicao.Columns.Count; iContador++)
                {
                    if (gridExibicao.Columns[iContador].GetType() == typeof(BoundField))
                    {
                        BoundField boundField = (BoundField)(gridExibicao.Columns[iContador]);
                        if (boundField.DataField.ToLowerInvariant() == "EstoqueMinimo".ToLowerInvariant() ||
                            boundField.DataField.ToLowerInvariant() == "EstoqueMaximo".ToLowerInvariant())
                            boundField.DataFormatString = "{0:" + base.fmtFracionarioMaterialQtde + "}";
                    }
                }
            }
        }

        protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.Cancelar();
            HabilitarBotoes();
            PopularGrid();
        }

        #region ISubItemMaterialView Members


        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>((this as ISubItemMaterialView).DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());

                if (!string.IsNullOrEmpty(ddlGrupo.SelectedValue.ToString()))
                    paramList.Add("CodigoGrupo", ddlGrupo.SelectedValue.ToString());
                else
                    paramList.Add("CodigoGrupo", string.Empty);

                if (!string.IsNullOrEmpty(ddlGrupo.SelectedItem.Text))
                    paramList.Add("DescricaoGrupo", ddlGrupo.SelectedItem.Text);
                else
                    paramList.Add("DescricaoGrupo", string.Empty);

                if (!string.IsNullOrEmpty(ddlClasse.SelectedValue.ToString()))
                    paramList.Add("CodigoClasse", ddlClasse.SelectedValue.ToString());
                else
                    paramList.Add("CodigoClasse", string.Empty);

                if (!string.IsNullOrEmpty(this.ddlClasse.SelectedItem.Text))
                    paramList.Add("DescricaoClasse", this.ddlClasse.SelectedItem.Text);
                else
                    paramList.Add("DescricaoClasse", string.Empty);

                if (!string.IsNullOrEmpty(ddlMaterial.SelectedValue.ToString()))
                    paramList.Add("CodigoMaterial", ddlMaterial.SelectedValue.ToString());
                else
                    paramList.Add("CodigoMaterial", string.Empty);

                if (!string.IsNullOrEmpty(ddlMaterial.SelectedValue))
                    paramList.Add("DescricaoMaterial", ddlMaterial.SelectedItem.Text);
                else
                    paramList.Add("DescricaoMaterial", string.Empty);

                if (!string.IsNullOrEmpty(ddlItem.SelectedValue.ToString()))
                    paramList.Add("CodigoItem", ddlItem.SelectedValue.ToString());
                else
                    paramList.Add("CodigoItem", string.Empty);

                if (!string.IsNullOrEmpty(ddlItem.SelectedValue))
                    paramList.Add("DescricaoItem", ddlItem.SelectedItem.Text);
                else
                    paramList.Add("DescricaoItem", string.Empty);

                paramList.Add("CodigoGestor", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);

                paramList.Add("CodigoAlmoxarifado", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);

                return paramList;
            }
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.ImprimirGerenciaCatalogo();
        }

        #endregion

        #region Propriedades

        public string NaturezaDespesaId
        {
            get;
            set;
        }

        public string ContaAuxiliarId
        {
            get;
            set;
        }


        public int UnidadeFornecimentoId
        {
            get;
            set;
        }

        public bool IndicadorAtividadeId
        {
            get
            {

                return Convert.ToBoolean(ddlAtividade.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlAtividade.SelectedValue = "True";
                else
                    ddlAtividade.SelectedValue = "False";
            }
        }

        public bool ControlaLote
        {
            get;
            set;
        }

        public bool ExpandeDecimos
        {
            get;
            set;
        }

        public bool PermiteFracionamento
        {
            get;
            set;
        }

        #endregion

        #region Metodos


        public void PopularListaNaturezaDespeza()
        {
            SubItemMaterialPresenter lObjPresenter = new SubItemMaterialPresenter(this);

            IList<NaturezaDespesaEntity> lstNaturezaDespesa = lObjPresenter.PopularNaturezaDespesaComSubItem(Int32.Parse(GestorId));

            ddlNatureza.Items.Clear();

            ListItem todos = new ListItem();
            todos.Value = "0";
            todos.Text = "- Todas -";

            //ddlNatureza.Items.Add("- Selecione -");
            ddlNatureza.Items.Add(todos);
            ddlNatureza.AppendDataBoundItems = true;
            //ddlNatureza.DataTextField = "CodigoDescricao";
            //ddlNatureza.DataValueField = "Id";

            ddlNatureza.DataSource = lstNaturezaDespesa;
            ddlNatureza.DataBind();
        }

        private void Cancelar()
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Cancelar();
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        private void HabilitarBotoes()
        {
            if (ddlItem.Items.Count > 0)
                btnNovo.Enabled = true;
            else
            {
                btnNovo.Enabled = false;
            }
        }

        #endregion


        public int IndicadorDisponivelId
        {
            get
            {
                return (int)Common.Util.TratamentoDados.TryParseInt32(ddlDisponivel.SelectedValue);
            }
            set
            {
                ddlDisponivel.SelectedValue = value.ToString();
            }
        }

        public decimal EstoqueMinimo
        {
            get { return helperTratamento.TryParseDecimal(this.txtEstoqueMinimo.Text, true).Value; }
            set
            {
                if (value != 0.0m)
                    this.txtEstoqueMinimo.Text = value.ToString(base.fmtFracionarioMaterialQtde);
                else
                    this.txtEstoqueMinimo.Text = string.Empty;
            }
        }
        public decimal SaldoSubItemUnit
        {
            get { return helperTratamento.TryParseDecimal(this.txtsaldoitemunit.Text, true).Value; }
            set
            {
                if (value != 0.0m)
                    this.txtsaldoitemunit.Text = value.ToString(base.fmtFracionarioMaterialQtde);
                else
                    this.txtsaldoitemunit.Text = string.Empty;
            }
        }


        public decimal EstoqueMaximo
        {
            get { return helperTratamento.TryParseDecimal(this.txtEstoqueMaximo.Text, true).Value; }
            set
            {
                if (value != 0.0m)
                    this.txtEstoqueMaximo.Text = value.ToString(base.fmtFracionarioMaterialQtde);
                else
                    this.txtEstoqueMaximo.Text = string.Empty;
            }
        }

        public bool IndicadorAtividadeAlmox
        {
            get
            {
                return (bool)Common.Util.TratamentoDados.TryParseBool(ddlAtividade.SelectedValue);
            }
            set
            {
                if (value == false)
                    ddlAtividade.SelectedValue = "0";
                if (value == true)
                    ddlAtividade.SelectedValue = "1";
            }
        }

        string ICrudView.Codigo
        {
            get
            {
                return lblCod.Text;
            }
            set
            {
                lblCod.Text = value;
            }
        }

        public void PopularListaIndicadorDisponivel()
        {
            //ddlDisponivel.DataSourceID = "odsDisponivel";
        }
        public void LimparPesquisaItem()
        {
            txtSubItem.Text = string.Empty;
            idSubItem.Value = string.Empty;
            this.ItemPesquisado = new ItemMaterialEntity();
        }

        public bool FiltraAlmoxarifado
        {
            get { return false; }
        }

        public bool FiltraGestor
        {
            get { return false; }
        }

        public bool ComSaldo
        {
            get { return false; }
        }

        RelatorioEntity ISubItemMaterialView.DadosRelatorio { get; set; }

        //Todo Edu: Refatorar
        public bool BloqueiaListaOrgao
        {
            set { }
        }

        public bool BloqueiaListaUO
        {
            set { }
        }

        public bool BloqueiaListaUGE
        {
            set { }
        }

        public void ComboPesquisaDesbalitar(string tipo)
        {
            switch (tipo)
            {
                case "N":
                    chbGrupo.Checked = false;
                    chbSubItem.Checked = false;
                    chbItem.Checked = false;
                    ddlNatureza.Enabled = true;
                    ddlGrupo.Enabled = false;
                    ddlClasse.Enabled = false;
                    ddlMaterial.Enabled = false;
                    ddlItem.Enabled = false;
                    txtSubItem.Enabled = false;
                    txtItem.Enabled = false;
                    txtSubItem.Text = string.Empty;
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = false;
                    imgLupaItem.Visible = false;

                    break;

                case "S":
                    chbNatureza.Checked = false;
                    chbSubItem.Checked = false;
                    chbGrupo.Checked = false;
                    chbItem.Checked = false;
                    ddlNatureza.Enabled = false;
                    ddlGrupo.Enabled = false;
                    ddlClasse.Enabled = false;
                    ddlMaterial.Enabled = false;
                    ddlItem.Enabled = false;
                    txtSubItem.Enabled = true;
                    txtItem.Enabled = false;
                    //txtSubItem.Text = string.Empty;
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = true;
                    imgLupaItem.Visible = false;
                    break;

                case "G":
                    chbNatureza.Checked = false;
                    chbSubItem.Checked = false;
                    chbItem.Checked = false;
                    ddlNatureza.Enabled = false;
                    ddlGrupo.Enabled = true;
                    ddlClasse.Enabled = true;
                    ddlMaterial.Enabled = true;
                    ddlItem.Enabled = true;
                    txtSubItem.Enabled = false;
                    txtItem.Enabled = false;
                    txtSubItem.Text = string.Empty;
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = false;
                    imgLupaItem.Visible = false;
                    break;

                case "I":
                    chbNatureza.Checked = false;
                    chbGrupo.Checked = false;
                    chbSubItem.Checked = false;
                    ddlNatureza.Enabled = false;
                    ddlGrupo.Enabled = false;
                    ddlClasse.Enabled = false;
                    ddlMaterial.Enabled = false;
                    ddlItem.Enabled = false;
                    txtSubItem.Enabled = false;
                    txtItem.Enabled = true;
                   // txtSubItem.Text = string.Empty;
                    txtSubItem.Enabled = false; 
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = false;
                    imgLupaItem.Visible = true;
                    break;
                case "D":
                    chbGrupo.Checked = false;
                    chbSubItem.Checked = false;
                    chbItem.Checked = false;
                    ddlNatureza.Enabled = false;
                    ddlGrupo.Enabled = false;
                    ddlClasse.Enabled = false;
                    ddlMaterial.Enabled = false;
                    ddlItem.Enabled = false;
                    txtSubItem.Enabled = false;
                    txtItem.Enabled = false;
                    txtSubItem.Text = string.Empty;
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = false;
                    imgLupaItem.Visible = false;

                    break;

                default:
                    chbNatureza.Checked = false;
                    chbSubItem.Checked = false;
                    chbItem.Checked = false;
                    ddlNatureza.Enabled = false;
                    ddlGrupo.Enabled = false;
                    ddlClasse.Enabled = false;
                    ddlMaterial.Enabled = false;
                    ddlItem.Enabled = false;
                    txtSubItem.Enabled = false;
                    txtSubItem.Text = string.Empty;
                    txtItem.Text = string.Empty;
                    imgLupaSubItem.Visible = false;
                    imgLupaItem.Visible = false;
                    gridSubItemMaterial.DataSourceID = null;
                    gridSubItemMaterial.Visible = false;

                    break;
            }


            ddlNatureza.SelectedValue = "0";
            ddlGrupo.SelectedValue = "0";
            ddlClasse.SelectedValue = "0";
            ddlMaterial.SelectedValue = "0";
            ddlItem.SelectedValue = "0";

            if (ItemPesquisado != null)
                ItemPesquisado.Id = null;


            switch (tipo)
            {
                case "N":
                    PopularGrid();
                    break;
                case "G":
                    PopularGrid();
                    break;
                case "D":
                    PopularGrid();
                    break;
                default:
                    break;
            }
        }



        protected void chbNatureza_CheckedChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("N");
        }

        protected void chbGrupo_CheckedChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("G");
        }


        protected void chbSubItem_CheckedChanged(object sender, EventArgs e)
        {
            if (chbSubItem.Checked)
            { txtSubItem.Enabled = true; }
            else {
                txtSubItem.Text = string.Empty;
                }

            gridSubItemMaterial.Visible = false;
            ComboPesquisaDesbalitar("S");
        }

        protected void chbItem_CheckedChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = false;
            ComboPesquisaDesbalitar("I");
        }

        protected void gridSubItemMaterial_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                // *** Retreive the DataGridRow
                int row = -1;
                int.TryParse(e.CommandArgument as string, out row);

                SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
                classe.Excluir(row);
                btnNovo.Enabled = true;

            }

        }



        protected void imgLupaSubItem_Click(object sender, ImageClickEventArgs e)
       { 
           
            gridSubItemMaterial.Visible = true;
            var asd = txtSubItem.Text;

            ddlNatureza.SelectedValue = "0";
            ddlGrupo.SelectedValue = "0";
            ddlClasse.SelectedValue = "0";
            ddlMaterial.SelectedValue = "0";
            ddlItem.SelectedValue = "0";

            if (ItemPesquisado != null)
                ItemPesquisado.Id = null;

            if (!string.IsNullOrEmpty(txtSubItem.Text))
            {
                PopularGrid();

                if (gridSubItemMaterial.Rows.Count == 0)
                {
                    ComboPesquisaDesbalitar("S");
                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModalSubItem();", true);
                }


            }
            else
            {
                ComboPesquisaDesbalitar("S");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModalSubItem();", true);
            }
        }

        protected void imgLupaItem_Click(object sender, ImageClickEventArgs e)
        {
            gridSubItemMaterial.Visible = true;

            ddlNatureza.SelectedValue = "0";
            ddlGrupo.SelectedValue = "0";
            ddlClasse.SelectedValue = "0";
            ddlMaterial.SelectedValue = "0";
            ddlItem.SelectedValue = "0";

            if (ItemPesquisado != null)
                ItemPesquisado.Id = null;

            if (!string.IsNullOrEmpty(txtItem.Text))
            {
                PopularGrid();

                if (gridSubItemMaterial.Rows.Count == 0)
                {
                    ComboPesquisaDesbalitar("I");
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModalItem();", true);
                }


            }
            else
            {
                ComboPesquisaDesbalitar("I");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModalItem();", true);
            }
        }

        protected void ddlIndicador_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("D");

            // CheckBox ChkBoxHeader = (CheckBox)gridSubItemMaterial.HeaderRow.FindControl("chkb1");
            //if (ddlIndicador.SelectedValue == "1")
            //    ChkBoxHeader.Attributes.Add("checked", "checked");
            //else
            //    ChkBoxHeader.Attributes.Remove("checked");


        }

        protected void ddlSaldo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("D");
        }

        protected void sellectAll(object sender, EventArgs e)
        {
            //CheckBox ChkBoxHeader = (CheckBox)gridSubItemMaterial.HeaderRow.FindControl("chkb1");
            //foreach (GridViewRow row in gridSubItemMaterial.Rows)
            //{
            //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkDisp");
            //    if (ChkBoxHeader.Checked == true)
            //    {
            //        ChkBoxRows.Checked = true;
            //    }
            //    else
            //    {
            //        ChkBoxRows.Checked = false;
            //    }
            //}
        }

    


        protected void btnAtivar_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.atualizarAlmoxSaldo(true);
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("D");
           // Session["TESTE" ] = "x";

        }

        protected void btnDesativar_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.atualizarAlmoxSaldo(false);
            gridSubItemMaterial.Visible = true;
            ComboPesquisaDesbalitar("D");
        }
    }
}
