using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Controles
{
    public partial class PesquisaItem : System.Web.UI.UserControl
    {
        protected const string sessionItem = "gridItemMaterialSession";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public bool UsaSaldo { get; set; }

        public bool FiltrarAlmoxarifadoLogado { get; set; }

        public bool FiltrarGestorLogado { get; set; }

        public void PopularDadosGridItemMaterial()
        {
            ItemMaterialPresenter item = new ItemMaterialPresenter();

            IList<ItemMaterialEntity> lstItem = item.ListarItemMaterialPorPalavraChave(2, txtChave.Text, FiltrarAlmoxarifadoLogado, FiltrarGestorLogado);

            //Filtrar somente os itens que tem saldo - Padrão UsaSaldo = false (Não usa saldo)
            if (UsaSaldo && lstItem != null)
            {
                AlmoxarifadoEntity almoxarifado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                ItemMaterialPresenter itemMaterialPresenter = new ItemMaterialPresenter();
                IList<ItemMaterialEntity> itemSaldoByAlmox = itemMaterialPresenter.ListarItemSaldoByAlmox(almoxarifado.Id.Value);
                lstItem = lstItem.Intersect(itemSaldoByAlmox, new BaseEntityIEqualityComparer()).Cast<ItemMaterialEntity>().ToList();
            }
            gridItemMaterial.PageSize = 20;
            gridItemMaterial.DataSource = lstItem;
            gridItemMaterial.DataBind();
            gridItemMaterial.PageIndex = 0;
            PageBase.SetSession(lstItem, sessionItem);

        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {
            PopularDadosGridItemMaterial();
        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

            gridItemMaterial.DataSource = null;
            gridItemMaterial.DataBind();
            gridItemMaterial.EmptyDataText = "";
            txtChave.Text = "";


            //GridViewRow row = gridItemMaterial.Rows[gridItemMaterial.SelectedIndex];

            //LinkButton lnk = (LinkButton)gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].FindControl("linkCodigo");

            //TextBox texto = null;
            //TextBox txtDesc = null;

            //Sam.Presenter.SubItemMaterialPresenter subPresenter = new Presenter.SubItemMaterialPresenter();

            //HiddenField txtID = null;
            //if (PageBase.GetSession<HiddenField>("itemMaterialId") != null)
            //{
            //    txtID = (HiddenField)PageBase.GetSession<HiddenField>("itemMaterialId");
            //}

            //DropDownList drpSubItem = null;
            //if (PageBase.GetSession<DropDownList>("lstSubItemMaterial") != null)
            //{
            //    drpSubItem = (DropDownList)PageBase.GetSession<DropDownList>("lstSubItemMaterial");
            //}

            //if (txtID != null)
            //{
            //    HiddenField txtCod = (HiddenField)this.Parent.FindControl(txtID.ID);

            //    Label matId = (Label)gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].FindControl("lblId");
            //    txtCod.Value = matId.Text;

            //    IList<SubItemMaterialEntity> listaSubItens = null;

            //    int gestorId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            //    // verifica se existe o código para popular o combobox
            //    if (PageBase.GetSession<TextBox>("itemMaterialCodigo") != null)
            //    {
            //        txtDesc = (TextBox)PageBase.GetSession<TextBox>("itemMaterialCodigo");
            //        texto = (TextBox)this.Parent.FindControl(txtDesc.ID);
            //        texto.Text = lnk.Text;

            //        int ugeId = 0;
            //        if (PageBase.GetSession<int>("ugeId") != 0)
            //        {
            //            ugeId = Convert.ToInt32(PageBase.GetSession<int>("ugeId"));
            //        }

            //        int almoxId = 0;
            //        if (PageBase.GetSession<int>("almoxId") != 0)
            //        {
            //            almoxId = Convert.ToInt32(PageBase.GetSession<int>("almoxId"));
            //        }

            //        drpSubItem = (DropDownList)this.Parent.FindControl(drpSubItem.ID);

            //        if (PageBase.GetSession<HiddenField>("itemMaterialId") != null && PageBase.GetSession<DropDownList>("lstSubItemMaterial") != null)
            //        {
            //            //Busca subItem do Saldo
            //            if (UsaSaldo)
            //            {
            //                drpSubItem.DataSourceID = null;
            //                drpSubItem.Items.Clear();
            //                drpSubItem.Items.Add("- Selecione -");
            //                drpSubItem.AppendDataBoundItems = true;
            //                listaSubItens = new SubItemMaterialPresenter().ListarSubItemSaldoByAlmox(Convert.ToInt32(txtCod.Value));
            //                drpSubItem.DataSource = listaSubItens;
            //                drpSubItem.DataBind();
            //                if (drpSubItem.Items.Count > 1)
            //                {
            //                    drpSubItem.SelectedIndex = 1;
            //                    drpSubItem.Focus();
            //                }
            //                gridItemMaterial.DataSource = null;
            //                gridItemMaterial.DataBind();
            //                gridItemMaterial.EmptyDataText = "";
            //            }
            //            else
            //            {
            //                if (ugeId != 0)
            //                {
            //                    drpSubItem.DataSourceID = null;
            //                    drpSubItem.Items.Clear();
            //                    drpSubItem.Items.Add("- Selecione -");
            //                    drpSubItem.AppendDataBoundItems = true;
            //                    listaSubItens = subPresenter.PopularDadosPorAlmox(Convert.ToInt32(texto.Text), almoxId);
            //                    drpSubItem.DataSource = listaSubItens;
            //                    drpSubItem.DataBind();
            //                    if (drpSubItem.Items.Count > 1)
            //                    {
            //                        drpSubItem.Focus();
            //                    }
            //                    gridItemMaterial.DataSource = null;
            //                    gridItemMaterial.DataBind();
            //                    gridItemMaterial.EmptyDataText = "";
            //                }
            //            }
            //        }
            //    }

            //    txtChave.Text = "";

            //    // descarregar sessões
            //    PageBase.RemoveSession("itemMaterialId");
            //    PageBase.RemoveSession("itemMaterialCodigo");
            //    PageBase.RemoveSession("lstSubItemMaterial");
            //    PageBase.RemoveSession("ugeId");
            //    PageBase.RemoveSession("almoxId");
            //    PageBase.RemoveSession(sessionItem);
            //}
        }

        public void SetFocusOnLoad() 
        {
            txtChave.Focus();
        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridItemMaterial.PageIndex = e.NewPageIndex;
            if (Session[sessionItem] != null)
            {
                gridItemMaterial.DataSource = PageBase.GetSession<List<ItemMaterialEntity>>(sessionItem);
                gridItemMaterial.DataBind();
            }

        }

    }
}
