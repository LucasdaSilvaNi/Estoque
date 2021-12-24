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

namespace Sam.Web.Seguranca
{
    public partial class cadastroRelacaoItemSubItem : PageBase, IRelacaoMaterialItemSubItemView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();

        protected void Page_Load(object sender, EventArgs e)
        {
            PesquisaSubitem1.UsaSaldo = false;
            PesquisaSubitem1.FiltrarAlmox = false;

            if (!String.IsNullOrEmpty(idSubItem.Value))
            {
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialBySubItem((int)Common.Util.TratamentoDados.TryParseInt32(idSubItem.Value));
                PopularListaGrupo();
            }

            if (!IsPostBack)
            {
                RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
                classe.Load();
            }
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

            HabilitarBotoes();
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
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
            //ddlClasse.Items.Clear();
            ddlClasse.DataSourceID = "odsListaClasse";
        }

        public void PopularListaMaterial()
        {
            //ddlMaterial.Items.Clear();
            ddlMaterial.DataSourceID = "odsListaMaterial";
        }

        public void PopularListaItem()
        {
            //ddlItem.Items.Clear();
            ddlItem.DataSourceID = "odsListaItem";
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        public string Codigo
        {
            set;
            get;
        }

        public string ItemId
        {
            get
            {
                return ddlItem.SelectedValue;
            }
        }

        public string ItemEditId
        {
            get
            {
                return ddlItemEdit.SelectedValue;
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
            set;
            get;
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

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
                HabilitarBotoes();
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

            string subItemId = null;

            if (!string.IsNullOrEmpty(ddlSubitem.SelectedValue))
                subItemId = ddlSubitem.SelectedValue;

            if (ddlItem.Items.Count > 0)
                PopularListaSubItem();

            if (!string.IsNullOrEmpty(subItemId))
                ddlSubitem.SelectedValue = subItemId;
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
            classe.Novo();
            btnNovo.Enabled = false;
            ddlOrgao.Enabled = false;
            ddlGestor.Enabled = false;
            txtItem.Enabled = false;
            ddlSubitem.Enabled = false;
            imgLupaSubItem.Enabled = false;
            ddlItemEdit.Enabled = true;
            ddlClasseEdit.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
            classe.Gravar();
            btnNovo.Enabled = true;
            ddlOrgao.Enabled = true;
            ddlGestor.Enabled = true;
            txtItem.Enabled = true;
            ddlSubitem.Enabled = true;
            imgLupaSubItem.Enabled = true;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
            classe.Excluir();
            btnNovo.Enabled = true;
            ddlOrgao.Enabled = true;
            ddlGestor.Enabled = true;
            txtItem.Enabled = true;
            ddlSubitem.Enabled = true;
            imgLupaSubItem.Enabled = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Cancelar();
            HabilitarBotoes();
            ddlOrgao.Enabled = true;
            ddlGestor.Enabled = true;
            txtItem.Enabled = true;
            ddlSubitem.Enabled = true;
            imgLupaSubItem.Enabled = true;
            ddlClasseEdit.Items.Clear();
            ddlMaterialEdit.Items.Clear();
            ddlItemEdit.Items.Clear();
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlGestor.Items.Clear();
            ddlGrupo.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            HabilitarBotoes();
        }

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlGrupo.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            this.PopularListaGrupo();
            HabilitarBotoes();
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlClasse.Items.Clear();
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            ddlSubitem.Items.Clear();
            this.PopularListaClasse();
            btnNovo.Enabled = false;
        }

        protected void ddlClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMaterial.Items.Clear();
            ddlItem.Items.Clear();
            ddlSubitem.Items.Clear();
            this.PopularListaMaterial();
            HabilitarBotoes();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularListaItem();
            this.PopularListaSubItem();
            PopularGrid();
            HabilitarBotoes();
        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            this.PopularListaSubItem();
            PopularGrid();
            HabilitarBotoes();
        }

        protected void ddlSubItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSubItemMaterial.PageIndex = 0;
            PopularGrid();
            HabilitarBotoes();
            MostrarPainelEdicao = false;
        }

        protected void ddlSubitem_DataBound(object sender, EventArgs e)
        {
            if (ddlSubitem.Items.Count > 0)
            {
                PopularGrid();
                HabilitarBotoes();
            }

        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridSubItemMaterial.DataKeys[gridSubItemMaterial.SelectedIndex].Value.ToString();

            RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
            classe.View.Id = int.Parse(this.Id).ToString();
            classe.RegistroSelecionado();

            ddlGrupoEdit.SelectedValue = ((Label)(gridSubItemMaterial.Rows[gridSubItemMaterial.SelectedIndex].Cells[6].FindControl("GrupoId"))).Text;
            ddlClasseEdit.SelectedValue = this.Classe;
            ddlMaterialEdit.SelectedValue = this.Material;
            ddlItemEdit.SelectedValue = this.Item;

            btnNovo.Enabled = false;
            ddlOrgao.Enabled = false;
            ddlGestor.Enabled = false;
            txtItem.Enabled = false;
            ddlSubitem.Enabled = false;
            imgLupaSubItem.Enabled = false;
            ddlItemEdit.Enabled = false;
            //ddlClasseEdit.Focus();
        }

        protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubitem.Items.Clear();
            this.PopularListaSubItem();
            HabilitarBotoes();
            MostrarPainelEdicao = false;
        }

        #endregion

        #region IRelacaoItemSubItemView Members

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
                paramList.Add("CodigoSubItem", this.ddlSubitem.SelectedValue);
                if (this.ddlSubitem.Items.Count > 0)
                    paramList.Add("DescricaoSubItem", this.ddlSubitem.SelectedItem.Text);
                else
                    paramList.Add("DescricaoSubItem", "");
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            RelacaoMaterialItemSubItemPresenter subitensMaterial = new RelacaoMaterialItemSubItemPresenter(this);
            subitensMaterial.Imprimir();
        }

        public void PopularListaSubItem()
        {
            ddlSubitem.DataSourceID = "odsListaSubitem";
        }

        public bool BloqueiaGrupo
        {
            set { ddlGrupo.Enabled = !value; }
        }

        public bool BloqueiaClasse
        {
            set { ddlClasse.Enabled = !value; }
        }

        public bool BloqueiaMaterial
        {
            set { ddlMaterial.Enabled = !value; }
        }

        public bool BloqueiaItem
        {
            set { ddlItem.Enabled = !value; }
        }

        public string SubItemId
        {
            get
            {
                return ddlSubitem.SelectedValue;
            }
            set
            {
                ddlSubitem.SelectedValue = value;
            }
        }

        public string Grupo
        {
            get
            {
                return ddlGrupoEdit.SelectedValue;
            }
            set
            {
                ListItem item = ddlGrupoEdit.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlGrupoEdit.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlGrupoEdit.ClearSelection();
            }
        }

        public string Material
        {
            get
            {
                return ddlMaterialEdit.SelectedValue;
            }
            set
            {
                ListItem item = ddlMaterialEdit.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlMaterialEdit.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlMaterialEdit.ClearSelection();
            }
        }

        public string Classe
        {
            get
            {
                return ddlClasseEdit.SelectedValue;
            }
            set
            {
                ListItem item = ddlClasseEdit.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlClasseEdit.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlClasseEdit.ClearSelection();
            }
        }

        public string Item
        {
            set
            {
                ListItem item = ddlItemEdit.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlItemEdit.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlItemEdit.ClearSelection();
            }
            get
            {
                return ddlItemEdit.SelectedValue;
            }
        }

        public bool BloqueiaCodigo
        {
            get;
            set;
        }

        public bool BloqueiaDescricao
        {
            get;
            set;
        }

        #endregion

        #region Metodos

        private void Cancelar()
        {
            RelacaoMaterialItemSubItemPresenter classe = new RelacaoMaterialItemSubItemPresenter(this);
            classe.Cancelar();
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        private void HabilitarBotoes()
        {
            if (ddlSubitem.Items.Count > 0 && pnlEditar.Visible == false)
                btnNovo.Enabled = true;
            else
            {
                btnNovo.Enabled = false;
            }

            if (ddlItemEdit.Items.Count > 0 && pnlEditar.Visible == true)
                btnGravar.Enabled = true;
            else
                btnGravar.Enabled = false;

            bool habilitarEdit;

            if (pnlEditar.Visible == true)
                habilitarEdit = false;
            else
                habilitarEdit = true;

            for (int i = 0; i < gridSubItemMaterial.Rows.Count; i++)
            {
                ((ImageButton)gridSubItemMaterial.Rows[i].Cells[3].FindControl("linkID")).Enabled = habilitarEdit;
            }
        }

        #endregion        

        #region Eventos Edit

        protected void ddlItemEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            HabilitarBotoes();
        }

        protected void ddlItemEdit_DataBound(object sender, EventArgs e)
        {
            HabilitarBotoes();
        }


        protected void ddlGrupoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlClasseEdit.Items.Clear();
            PopularListaClasseEdit();
            ddlMaterialEdit.Items.Clear();
            ddlItemEdit.Items.Clear();
        }

        protected void ddlGrupoEdit_DataBound(object sender, EventArgs e)
        {
            if (ddlGrupoEdit.Items.Count > 0)
                PopularListaClasseEdit();
            else
                ddlClasseEdit.Items.Clear();
        }

        protected void ddlClasseEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMaterialEdit.Items.Clear();
            PopularListaMaterialEdit();
            ddlItemEdit.Items.Clear();
        }

        protected void ddlClasseEdit_DataBound(object sender, EventArgs e)
        {
            ddlMaterialEdit.Items.Clear();
            PopularListaMaterialEdit();
        }

        protected void ddlMaterialEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlItemEdit.Items.Clear();
            PopularListaItemEdit();
        }

        protected void ddlMaterialEdit_DataBound(object sender, EventArgs e)
        {
            ddlItemEdit.Items.Clear();
            PopularListaItemEdit();
        }


        public void PopularListaGrupoEdit()
        {
            GrupoPresenter grupo = new GrupoPresenter();

            ddlGrupoEdit.DataSource = grupo.PopularDadosGrupoTodosCod(0, 0);
            ddlGrupoEdit.DataBind();

            //Seleciona o mesmo Grupo da consulta e bloqueia para não trocar.
            if (ddlGrupo.Items.Count > 0)
            {
                foreach (ListItem i in ddlGrupoEdit.Items)
                {
                    if (i.Value == ddlGrupo.SelectedValue)
                    {
                        i.Selected = true;
                        ddlGrupoEdit.Enabled = false;
                        break;
                    }
                    else
                    {
                        i.Selected = false;
                    }
                }
            }
            PopularListaClasseEdit();
        }

        public void PopularListaClasseEdit()
        {
            ClassePresenter classe = new ClassePresenter();

            ddlClasseEdit.Items.Clear();
            if (ddlGrupo.Items.Count > 0)
            {
                ddlClasseEdit.DataSource = classe.PopularDadosClasseComCod(0, 0, int.Parse(ddlGrupoEdit.SelectedValue));
                ddlClasseEdit.DataBind();

                //Seleciona a mesma classe da consulta e bloqueia para não trocar.
                if (ddlClasse.Items.Count > 0)
                {
                    foreach (ListItem i in ddlClasseEdit.Items)
                    {
                        if (i.Value == ddlClasse.SelectedValue)
                        {
                            i.Selected = true;
                            ddlClasseEdit.Enabled = false;
                            break;
                        }
                        else
                        {
                            i.Selected = false;
                        }
                    }
                }
                PopularListaMaterialEdit();
            }
        }

        public void PopularListaMaterialEdit()
        {
            MaterialPresenter material = new MaterialPresenter();

            ddlMaterialEdit.Items.Clear();
            if (ddlClasseEdit.Items.Count > 0)
            {
                ddlMaterialEdit.SelectedIndex = -1;
                ddlMaterialEdit.DataSource = material.PopularDadosMaterialComCod(0, 0, int.Parse(ddlClasseEdit.SelectedValue));
                ddlMaterialEdit.DataBind();

                //Seleciona o mesmo Material da consulta e bloqueia para não trocar.
                if (ddlMaterial.Items.Count > 0)
                {
                    foreach (ListItem i in ddlMaterialEdit.Items)
                    {
                        if (i.Value == ddlMaterial.SelectedValue)
                        {
                            i.Selected = true;
                            ddlMaterialEdit.Enabled = false;
                            break;
                        }
                        else
                        {
                            i.Selected = false;
                        }
                    }
                }
            }
        }

        public void PopularListaItemEdit()
        {
            ItemMaterialPresenter itemMaterial = new ItemMaterialPresenter();

            ddlItemEdit.Items.Clear();
            if (ddlMaterialEdit.Items.Count > 0)
            {
                ddlItemEdit.DataSource = itemMaterial.PopularDadosItemMaterialComCod(0, 0, int.Parse(ddlMaterialEdit.SelectedValue));
                ddlItemEdit.DataBind();
            }
        }

        #endregion


        public void PopularListaOrgaoEdit()
        {
        }

        public void PopularListaGestorEdit()
        {
        }

        public void LimparPesquisaItem()
        {
            txtItem.Text = string.Empty;
            idSubItem.Value = string.Empty;
            this.ItemPesquisado = new ItemMaterialEntity();
        }


        public string idItemSubItem
        {
            get { throw new NotImplementedException(); }
        }

        protected void imgLupaSubItem_Click(object sender, ImageClickEventArgs e)
        {
            MostrarPainelEdicao = false;
        }
    }
}
