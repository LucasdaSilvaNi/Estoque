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
    public partial class cadastroSubItemMaterial : PageBase, ISubItemMaterialView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();
        IList<NaturezaDespesaEntity> ItemNatureza = new List<NaturezaDespesaEntity>();
       

        protected void Page_Load(object sender, EventArgs e)
        {


            PesquisaSubitem.UsaSaldo = false;
            PesquisaSubitem.FiltrarAlmox = false;

            if (!String.IsNullOrEmpty(idSubItem.Value))
            {
               txtItem.Text = "";
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialBySubItem((int)Common.Util.TratamentoDados.TryParseInt32(idSubItem.Value));
                PopularListaGrupo();
            }

            PesquisaItem.UsaSaldo = false;
            PesquisaItem.FiltrarAlmoxarifadoLogado = false;
            if (!String.IsNullOrEmpty(itemMaterialId.Value))
            {
              txtSubItem.Text = "";
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialByItem((int)Common.Util.TratamentoDados.TryParseInt32(itemMaterialId.Value));
                PopularListaGrupo();
            }

            if (!IsPostBack)
            {
                SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
                classe.Load();
            }
            txtCodigo.Attributes.Add("onblur", "preencheZeros(this,'12') ");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            HabilitarBotoes();
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

        public void PopularListaNaturezaDespeza()
        {
            ddlNatureza.DataSourceID = "odsListaNatDespesa";
        }


        public void PopularListaNaturezaDespezaEdit()
        {
            ddlNaturezaEdit.DataSourceID = "odsListaNatDespesaEdit";
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

                if (Session["OrgaoId"] != null)
                {
                    int idOrgao = Convert.ToInt32(Session["OrgaoId"]);
                    //if (idOrgao == (int)GeralEnum.Orgao.FCASA)
                    //    this.txtDescricao.Enabled = true;
                    //else
                    this.txtDescricao.Enabled = value;
                }
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
                if (Session["OrgaoId"] != null)
                {
                    int idOrgao = Convert.ToInt32(Session["OrgaoId"]);
                    if (idOrgao == (int)GeralEnum.Orgao.FCASA)
                    {
                        this.ddlNatureza.Enabled = true;
                        this.ddlNaturezaEdit.Enabled = true;
                    }
                    else
                    {
                        this.ddlNatureza.Enabled = value;
                        this.ddlNaturezaEdit.Enabled = value;
                    }
                }
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
                PopularListaMaterial();
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
                PopularListaItem();
            else
                ddlItem.Items.Clear();

            if (ItemPesquisado.Id != null)
            {
                ddlMaterial.Items.FindByValue(ItemPesquisado.MaterialId.ToString()).Selected = true;
            }
        }

        protected void ddlItem_DataBound(object sender, EventArgs e)
        {
            if (ItemPesquisado.Id != null)
            {
                ddlItem.Items.FindByValue(ItemPesquisado.Id.ToString()).Selected = true;
            }

            HabilitarBotoes();
            PopularGrid();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ViewState["acao"] = "Novo";
            ddlNatureza.Visible = true;
            ddlNaturezaEdit.Visible = false;
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Novo();
            btnNovo.Enabled = false;
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);

            string codigo = ViewState["codigoSelecionado"] == null ? txtCodigo.Text : ViewState["codigoSelecionado"].ToString();

            classe.Gravar(codigo, ViewState["acao"].ToString());
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

        }

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            PopularGrid();
            ddlGrupo.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            this.PopularListaGrupo();
            HabilitarBotoes();
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaClasse();
            PopularGrid();
            btnNovo.Enabled = false;
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlItem.Items.Clear();
            gridSubItemMaterial.PageIndex = 0;
            this.PopularListaMaterial();
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
            int filtro = 0;
            hfdSubItemNatureza.Value = string.Empty;
            this.Id = gridSubItemMaterial.DataKeys[gridSubItemMaterial.SelectedIndex].Value.ToString();
            this.Codigo = Server.HtmlDecode(gridSubItemMaterial.Rows[gridSubItemMaterial.SelectedIndex].Cells[1].Text);

            ViewState["codigoSelecionado"] = gridSubItemMaterial.SelectedRow.Cells[1].Text;
            ViewState["acao"] = "Editar";

            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);

            classe.SubItemId = int.Parse(this.Id);
            bool isUsado = classe.VerificaSubItemUtilizado(classe.SubItemId);

            classe.RegistroSelecionado();
            HabilitarBotoes();
            txtCodigo.Focus();

            if (isUsado)
            { 
                DesabilitaCamposEdicao(false);
            }
            else
            { 
                DesabilitaCamposEdicao(true);
                filtro = 1;
            }
         

            ItemMaterialPresenter presenter = new ItemMaterialPresenter();

           // paramList.Add("CodigoItem", ddlItem.SelectedValue.ToString());

            ItemNatureza = presenter.PopularNaturezaDespesaTodosCodPorItemMaterial(Convert.ToInt32(ddlItem.SelectedValue));
            //ItemNatureza = presenter.PopularNaturezaDespesaTodosCodPorItemMaterial(Convert.ToInt32(itemMaterialId.Value));             
            ddlNaturezaEdit.Visible = true;
            ddlNatureza.Visible = false;


            var output = from x in ItemNatureza
                         where  !x.CodigoDescricao.Contains("- Selecione -")
                         select x;


            ddlNaturezaEdit.DataSource = output;
            ddlNaturezaEdit.DataBind();
            // }
            SubItemMaterialPresenter presenterItem = new SubItemMaterialPresenter();
            int id = Convert.ToInt32(this.Id);
            string SubItemPesquisado = presenterItem.NaturezaSubItem(id);
            //if (ItemPesquisado.NaturezaDespesa.Count > 0)
            hfdSubItemNatureza.Value = SubItemPesquisado;
            if (filtro !=1)
            {
              ddlNaturezaEdit.SelectedValue = hfdSubItemNatureza.Value;
               
            }
            ddlNaturezaEdit.SelectedValue =SubItemPesquisado;
        }

        private void DesabilitaCamposEdicao(bool enabled)
        {
            new SubItemMaterialPresenter(this).DesativaEdicao(enabled);
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
                if (ddlNatureza.Visible)
                    return ddlNatureza.SelectedValue;
                else
                    return ddlNaturezaEdit.SelectedValue;
            }
            set
            {
                if (ddlNatureza.Items.Count > 0)
                {
                    for (int i = 0; i < ddlNatureza.Items.Count; i++)
                    {
                        if (ddlNatureza.Items[i].Value.Contains(value.ToString()))
                        {
                            ddlNatureza.SelectedValue = value.ToString();
                            hfdSubItemNatureza.Value = value.ToString();
                            return;
                        }
                        else
                            if (hfdSubItemNatureza.Value != "0" && !string.IsNullOrEmpty(hfdSubItemNatureza.Value))
                            ddlNatureza.SelectedValue = hfdSubItemNatureza.Value;
                        else
                            ddlNatureza.SelectedValue = "0";
                    }
                }
            }
        }

        //public string ContaAuxiliarId
        //{
        //    get
        //    {
        //        return ddlConta.SelectedValue;
        //    }
        //    set
        //    {
        //        ddlConta.SelectedValue = value.ToString();
        //    }
        //}


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

        //public bool ControlaLote
        //{
        //    get
        //    {
        //        return Convert.ToBoolean(ddlControlaLote.SelectedValue);
        //    }
        //    set
        //    {
        //        if (value == true)
        //            ddlControlaLote.SelectedValue = "True";
        //        else
        //            ddlControlaLote.SelectedValue = "False";
        //    }
        //}

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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
            //itemMaterialId.Value = string.Empty;
            this.ItemPesquisado = new ItemMaterialEntity();
        }


        //void ISubItemMaterialView.LimparPesquisaItem()
        //{
        //    txtSubItemMaterial.Text = string.Empty;
        //    idSubItem.Value = string.Empty;
        //    this.ItemPesquisado = new ItemMaterialEntity();
        //}

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


        public string Item
        {
            set
            {
                ListItem item = ddlItem.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlItem.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlItem.ClearSelection();
            }
            get
            {
                return ddlItem.SelectedValue;
            }
        }
        protected void btnCarregarSubItem_Click(object sender, EventArgs e)
        {
            txtItem.Text="";
            CarregarSubItensPorCodigo();
        }
        private void CarregarSubItensPorCodigo()
        {
            long? codigoSubItem = Common.Util.TratamentoDados.TryParseLong(txtSubItem.Text.Trim());
            SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
            var gestorId = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            var subItem = presenter.CarregarSubItem(codigoSubItem, gestorId);         
        }

    }
}
