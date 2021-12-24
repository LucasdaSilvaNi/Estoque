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
namespace Sam.Web
{
    public partial class cadastroSubItemMaterialItemInativo : PageBase, ISubItemMaterialView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();

        protected void Page_Load(object sender, EventArgs e)
        {
            PesquisaSubitem1.UsaSaldo = false;
            PesquisaSubitem1.FiltrarAlmoxarifadoLogado = false;

            if (!String.IsNullOrEmpty(itemMaterialId.Value))
            {
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialByItem((int)Common.Util.TratamentoDados.TryParseInt32(itemMaterialId.Value));
            }

            if (!IsPostBack)
            {
                SubItemMaterialPresenter objPresenter = new SubItemMaterialPresenter(this);
                //objPresenter.Load(true);
                //PopularCombos(true);
                objPresenter.Load();
                PopularCombos(false);
            }
            
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'12') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            HabilitarBotoes();
            PopularGrid();
        }

        private void PopularCombos(bool blnCarregarDadosEdicao)
        {
            this.PopularListaOrgao();
            this.PopularListaGestor();
            
            this.PopularListaGrupo();
            this.PopularListaClasse();
            this.PopularListaMaterial();
            this.PopularListaItem();

            if (blnCarregarDadosEdicao)
            {
                this.PopularListaNaturezaDespeza();
                this.PopularListaContaAuxiliar();
                this.PopularListaUnidadeFornecimento();
                this.PopularListaIndicadorAtividade();
            }
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridSubItemMaterial.DataSourceID = "sourceGridSubItemMaterial";
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "odsListaOrgao";
        }

        public void PopularListaGestor()
        {
            ddlGestor.DataSourceID = "odsListaGestor";
        }

        public void PopularListaGrupo()
        {
            //ddlGrupo.DataSourceID = "odsListaGrupo";
            
            GrupoPresenter    lObjPresenter = new GrupoPresenter();
            List<GrupoEntity> lstRetorno    = new List<GrupoEntity>();
            lstRetorno.Add(lObjPresenter.ObterGrupoMaterial(99));

            this.ddlGrupo.DataSource = lstRetorno;
            this.ddlGrupo.DataValueField = "Id";
            this.ddlGrupo.DataTextField = "CodigoDescricao";
            this.ddlGrupo.DataBind();
        }

        public void PopularListaClasse()
        {
            //ddlClasse.DataSourceID = "odsListaClasse";

            ClassePresenter    lObjPresenter = new ClassePresenter();
            List<ClasseEntity> lstRetorno    = new List<ClasseEntity>();
            lstRetorno.Add(lObjPresenter.ObterClasseMaterial(9999));

            this.ddlClasse.DataSource = lstRetorno;
            this.ddlClasse.DataValueField = "Id";
            this.ddlClasse.DataTextField = "CodigoDescricao";
            this.ddlClasse.DataBind();
        }

        public void PopularListaMaterial()
        {
            //ddlMaterial.DataSourceID = "odsListaMaterial";

            MaterialPresenter lObjPresenter = new MaterialPresenter();
            List<MaterialEntity> lstRetorno = new List<MaterialEntity>();
            lstRetorno.Add(lObjPresenter.ObterMaterial(99999999));

            this.ddlMaterial.DataSource = lstRetorno;
            this.ddlMaterial.DataValueField = "Id";
            this.ddlMaterial.DataTextField = "CodigoDescricao";
            this.ddlMaterial.DataBind();
        }

        public void PopularListaItem()
        {
            //ddlItem.DataSourceID = "odsListaItem";

            ItemMaterialPresenter    lObjPresenter = new ItemMaterialPresenter();
            List<ItemMaterialEntity> lstRetorno    = new List<ItemMaterialEntity>();
            //lstRetorno.Add(lObjPresenter.ObterItemMaterial(99999999));

            ItemMaterialEntity itemInativoImplantacao = lObjPresenter.ObterItemMaterial(99999999);
            if (itemInativoImplantacao.IsNotNull())
            {
                lstRetorno.Add(lObjPresenter.ObterItemMaterial(99999999));

                this.ddlItem.DataSource = lstRetorno;
                this.ddlItem.DataValueField = "Id";
                this.ddlItem.DataTextField = "CodigoDescricao";
                this.ddlItem.DataBind();
            }
            else
            {
                this.ddlItem.DataSource = null;
                this.ddlItem.DataBind();

                this.ddlItem.Items.Add(new ListItem(@"ITEM MATERIAL - ""ITEM INATIVO - IMPLANTAÃ‡ÃƒO"" - NÃƒO ENCONTRADO", "00", true));
            }
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        public void PopularListaNaturezaDespeza()
        {
            //ddlNatureza.DataSourceID = "odsListaNatDespesa";
            int iGestor_ID = 0;
            if (!String.IsNullOrWhiteSpace(this.GestorId))
                iGestor_ID = Int32.Parse(this.GestorId);
            else
                iGestor_ID = new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value;

            SubItemMaterialPresenter lObjPresenter = new SubItemMaterialPresenter(this);
            IList<NaturezaDespesaEntity> lstNaturezaDespesa = lObjPresenter.PopularNaturezaDespesaComSubItem(iGestor_ID);

            ddlNatureza.DataSource = lstNaturezaDespesa;
            ddlNatureza.DataBind();

        }

        public void PopularListaContaAuxiliar()
        {
            ddlConta.DataSourceID = "odsListaConta";
        }

        public void PopularListaUnidadeFornecimento()
        {
            ddlUnidadeFornecimento.DataSourceID = "sourceListaUnidadeFornecimento";
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

                return ddlGestor.SelectedValue;
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

        public string CodigoBarras
        {
            get
            {
                return txtCodBarras.Text;
            }
            set
            {
                txtCodBarras.Text = value;
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

        public bool BloqueiaCodBarras
        {
            set
            {
                this.txtCodBarras.Enabled = value;
            }
        }

        public bool BloqueiaNaturezaDespesa
        {
            set
            {
                this.ddlNatureza.Enabled = value;
            }
        }

        public bool BloqueiaContaAuxiliar
        {
            set
            {
                this.ddlConta.Enabled = value;
            }
        }

        public bool BloqueiaControlaLote
        {
            set
            {
                this.ddlControlaLote.Enabled = value;
            }
        }

        public bool BloqueiaExpandeDecimais
        {
            set
            {
                this.ddlExpandeDecimos.Enabled = value;
            }
        }

        public bool BloqueiaPermiteFacionamento
        {
            set
            {
                this.ddlPermiteFracionamento.Enabled = value;
            }
        }

        public bool BloqueiaAtividade
        {
            set
            {
                this.ddlAtividade.Enabled = value;
            }
        }

        public bool BloqueiaUnidadeFornecimento
        {
            set { this.ddlUnidadeFornecimento.Enabled = value; }
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

        protected void ddlOrgao_DataBound(object sender, EventArgs e)
        {
            if (ddlOrgao.Items.Count > 0)
                PopularListaGestor();
            else
                ddlGestor.Items.Clear();
        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            if (ddlGestor.Items.Count > 0)
                PopularListaGrupo();
            else
                ddlGrupo.Items.Clear();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Novo();
            btnNovo.Enabled = false;

            
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Gravar(ViewState["codigoSelecionado"].ToString(), ViewState["acao"].ToString());
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

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaGestor();

            ddlGestor.Items.Clear();
            ddlGrupo.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            HabilitarBotoes();

            this.PopularListaGestor();
            this.PopularListaGrupo();
            this.PopularListaClasse();
            this.PopularListaMaterial();
            this.PopularListaItem();
        }

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            PopularGrid();
            ddlGrupo.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            this.PopularListaGrupo();
            this.PopularListaClasse();
            this.PopularListaMaterial();
            this.PopularListaItem();
            HabilitarBotoes();
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaClasse();
            this.PopularListaMaterial();
            this.PopularListaItem();
            PopularGrid();
            btnNovo.Enabled = false;
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlItem.Items.Clear();
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaMaterial();
            this.PopularListaItem();
            PopularGrid();
            HabilitarBotoes();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaItem();
            PopularGrid();
            HabilitarBotoes();
        }

        protected void ddlUnidadeFornecimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            PopularGrid();
            HabilitarBotoes();
        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridSubItemMaterial.DataKeys[gridSubItemMaterial.SelectedIndex].Value.ToString();

            ViewState["codigoSelecionado"] = gridSubItemMaterial.SelectedRow.Cells[1].Text;

            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.SubItemId = int.Parse(this.Id);
            classe.RegistroSelecionado();
            HabilitarBotoes();
            txtCodigo.Focus();
        }

        protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            HabilitarBotoes();
            PopularGrid();
            Cancelar();
        }

        #region ISubItemMaterialView Members


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
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);
                paramList.Add("CodigoGrupo", ddlGrupo.SelectedValue.ToString());
                paramList.Add("DescricaoGrupo", this.ddlGrupo.SelectedItem.Text);
                paramList.Add("CodigoClasse", ddlClasse.SelectedValue.ToString());
                paramList.Add("DescricaoClasse", this.ddlClasse.SelectedItem.Text);
                paramList.Add("CodigoMaterial", ddlMaterial.SelectedValue.ToString());
                paramList.Add("DescricaoMaterial", this.ddlMaterial.SelectedItem.Text);
                paramList.Add("CodigoItem", ddlItem.SelectedValue.ToString());
                paramList.Add("DescricaoItem", this.ddlItem.SelectedItem.Text);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
            subitensMaterial.Imprimir();
        }

        #endregion

        #region Propriedades

        public string NaturezaDespesaId
        {
            get
            {
                return ddlNatureza.SelectedValue;
            }
            set
            {
                ddlNatureza.SelectedValue = value.ToString();
            }
        }

        public string ContaAuxiliarId
        {
            get
            {
                return ddlConta.SelectedValue;
            }
            set
            {
                ddlConta.SelectedValue = value.ToString();
            }
        }

        public int UnidadeFornecimentoId
        {
            get
            {
                return Convert.ToInt32(Common.Util.TratamentoDados.TryParseInt32(ddlUnidadeFornecimento.SelectedValue));
            }
            set
            {
                if (value > 0)
                {
                    //ListItem DropDownListItem = ddlUnidadeFornecimento.Items.FindByValue(value.ToString());

                    ddlUnidadeFornecimento.SelectedValue = value.ToString();

                    //if (DropDownListItem != null)
                    //{ DropDownListItem.Selected = true; }

                }
            }
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
            get
            {
                return Convert.ToBoolean(ddlControlaLote.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlControlaLote.SelectedValue = "True";
                else
                    ddlControlaLote.SelectedValue = "False";
            }
        }

        public bool ExpandeDecimos
        {
            get
            {
                return Convert.ToBoolean(ddlExpandeDecimos.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlExpandeDecimos.SelectedValue = "True";
                else
                    ddlExpandeDecimos.SelectedValue = "False";
            }
        }

        public bool PermiteFracionamento
        {
            get
            {
                return Convert.ToBoolean(ddlPermiteFracionamento.SelectedValue);
            }
            set
            {
                if (value == true)
                    ddlPermiteFracionamento.SelectedValue = "True";
                else
                    ddlPermiteFracionamento.SelectedValue = "False";
            }
        }

        #endregion

        #region Metodos

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
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        public decimal EstoqueMinimo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public decimal EstoqueMaximo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

//        public int EstoqueMinimo
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public int EstoqueMaximo
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }


        public bool IndicadorAtividadeAlmox
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void PopularListaIndicardorDisponivel()
        {
            throw new NotImplementedException();
        }

        protected void ddlUnidadeFornecimento_DataBound(object sender, EventArgs e)
        {
            if (this.UnidadeFornecimentoId != 0 && this.UnidadeFornecimentoId != 1)
                ddlUnidadeFornecimento.Items.FindByValue(this.UnidadeFornecimentoId.ToString()).Selected = true;
        }

        void ISubItemMaterialView.LimparPesquisaItem()
        {
            //txtItem.Text = string.Empty;
            itemMaterialId.Value = string.Empty;
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
    }
}
