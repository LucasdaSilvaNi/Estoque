using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;
using System.Reflection;
using Sam.Entity;

namespace Sam.Web.Controles
{
    public partial class PesquisaRequisicao : System.Web.UI.UserControl
    {
        private const string sessionMovimento = "sessionMovimento";
  

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void PopularDadosGridRequisicao(MovimentoEntity movimento, int pageIndex)
        {
            MovimentoPresenter presenter = new MovimentoPresenter();

            if (movimento != null)
            {
                PageBase.SetSession<MovimentoEntity>(movimento, sessionMovimento);
            }
            else
            {
                movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);
            }
                     
            int tipoOperacao = Convert.ToInt32(Session["rblTipoOperacao"]);                      
            IList<MovimentoEntity> list = presenter.ListarRequisicaoByAlmoxarifado(20, pageIndex, 0, (int)movimento.Divisao.Id, movimento.TipoMovimento.Id, tipoOperacao, movimento.DataDigitada);


            grdRequisicao.PageSize = 20;
            grdRequisicao.PageIndex = pageIndex;
            grdRequisicao.AllowPaging = true;
            grdRequisicao.DataSource = list;
            grdRequisicao.DataBind();
        }

        public void PopularDadosGridRequisicao(MovimentoEntity movimento, int pageIndex, bool requisicoesParaEstorno)
        {
            if (movimento != null)
                this.lblTipoMovimento.Text = movimento.TipoMovimento.Id.ToString();
            if (!requisicoesParaEstorno)
            {
                PopularDadosGridRequisicao(movimento, pageIndex);
                return;
            }

            if (requisicoesParaEstorno)
                movimento.Divisao.Id = 0;

            if (movimento != null)
                PageBase.SetSession<MovimentoEntity>(movimento, sessionMovimento);
            else
                movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);

            MovimentoPresenter presenter = new MovimentoPresenter();

            IList<MovimentoEntity> list = presenter.ListarRequisicaoByAlmoxarifado(20, pageIndex, 0, (int)movimento.Divisao.Id, movimento.TipoMovimento.Id, requisicoesParaEstorno);
            grdRequisicao.PageSize = 20;
            grdRequisicao.PageIndex = pageIndex;
            grdRequisicao.AllowPaging = true;
            grdRequisicao.DataSource = list;
            grdRequisicao.DataBind();
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {

            MovimentoPresenter presenter = new MovimentoPresenter();
            string dataDigitada = string.Empty;
            string tipoOperacao = Session["rblTipoOperacao"].ToString();
           if (Session["txtDataMovimento"] != null) { 
             dataDigitada = Session["txtDataMovimento"].ToString();
            }
            int tipoMovimento = Convert.ToInt32(this.lblTipoMovimento.Text.Trim());
            grdRequisicao.DataSource = presenter.ListarNotasdeFornecimento(txtChave.Text.Trim(), tipoMovimento, tipoOperacao, dataDigitada);
            grdRequisicao.DataBind();
        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = grdRequisicao.Rows[grdRequisicao.SelectedIndex];

            LinkButton lnk = (LinkButton)grdRequisicao.Rows[grdRequisicao.SelectedIndex].FindControl("linkCodigo");

            TextBox texto = null;
            TextBox txtDesc = null;

            Sam.Presenter.SubItemMaterialPresenter subPresenter = new Presenter.SubItemMaterialPresenter();

            HiddenField txtID = null;
            if (PageBase.GetSession<HiddenField>("itemMaterialId") != null)
            {
                txtID = (HiddenField)PageBase.GetSession<HiddenField>("itemMaterialId");
            }

            DropDownList drpSubItem = null;
            if (PageBase.GetSession<DropDownList>("lstSubItemMaterial") != null)
            {
                drpSubItem = (DropDownList)PageBase.GetSession<DropDownList>("lstSubItemMaterial");
            }


            //HiddenField txtCod = (HiddenField)this.Parent.FindControl(txtID.ID);

            //Label matId = (Label)gridItemMaterial.Rows[gridItemMaterial.SelectedIndex].FindControl("lblId");
            //txtCod.Value = matId.Text;

            // verifica se existe o código para popular o combobox
            if (PageBase.GetSession<TextBox>("itemMaterialCodigo") != null)
            {
                txtDesc = (TextBox)PageBase.GetSession<TextBox>("itemMaterialCodigo");
                texto = (TextBox)this.Parent.FindControl(txtDesc.ID);
                texto.Text = lnk.Text;
                if (PageBase.GetSession<HiddenField>("itemMaterialId") != null && PageBase.GetSession<DropDownList>("lstSubItemMaterial") != null)
                {
                    drpSubItem = (DropDownList)this.Parent.FindControl(drpSubItem.ID);
                    drpSubItem.DataSourceID = null;
                    drpSubItem.Items.Clear();
                    drpSubItem.Items.Add("-Selecione-");
                    drpSubItem.AppendDataBoundItems = true;
                    drpSubItem.DataSource = subPresenter.PopularSubItemMaterialTodosPorItemUgeAlmox(Convert.ToInt32(texto.Text), 1007, 1);
                    drpSubItem.DataBind();
                }
            }

            txtChave.Text = "";

            // descarregar sessões
            PageBase.RemoveSession("itemMaterialId");
            PageBase.RemoveSession("itemMaterialCodigo");
            PageBase.RemoveSession("lstSubItemMaterial");
        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PopularDadosGridRequisicao(null, e.NewPageIndex);
            ViewState["pagina"] = e.NewPageIndex;
            if(ViewState["SortExpression"] != null)
                Sort(ViewState["SortExpression"].ToString());

        }

        #region Sort

        private void Sort(string SortExpression)
        {

            MovimentoEntity movimento = null;
            movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);

            MovimentoPresenter presenter = new MovimentoPresenter();
            string DataDigtada = movimento.DataDigitada;
            int pagina = ViewState["pagina"] == null ? 0 : Convert.ToInt32(ViewState["pagina"].ToString());
            string Sortdir = GetSortDirection(SortExpression);
            string SortExp = SortExpression;
            int tipoOperacao = Convert.ToInt32(Session["rblTipoOperacao"]);
            var list = presenter.ListarRequisicaoByAlmoxarifado(20, pagina, 0, (int)movimento.Divisao.Id, movimento.TipoMovimento.Id, tipoOperacao, DataDigtada).ToList();
            if (Sortdir == "ASC")
            {
                list = Sort<MovimentoEntity>(list, SortExp, SortDirection.Ascending);
            }
            else
            {
                list = Sort<MovimentoEntity>(list, SortExp, SortDirection.Descending);
            }
            this.grdRequisicao.DataSource = list;
            this.grdRequisicao.DataBind();
        }

        /// <summary>
        /// Gridview sorting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdRequisicao_Sorting(object sender, GridViewSortEventArgs e)
        {
            Sort(e.SortExpression);
        }
        /// <summary>
        /// GEt Sorting direction
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;
            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;
            return sortDirection;
        }
        /// <summary>
        /// Sort function
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public List<MovimentoEntity> Sort<TKey>(List<MovimentoEntity> list, string sortBy, SortDirection direction)
        {
            PropertyInfo property = list.GetType().GetGenericArguments()[0].GetProperty(sortBy);

            if (direction == SortDirection.Ascending)
            {
                return list.OrderBy(e => property.GetValue(e, null)).ToList<MovimentoEntity>();
            }
            else
            {
                return list.OrderByDescending(e => property.GetValue(e, null)).ToList<MovimentoEntity>();
            }


        }

        protected void grdRequisicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string imgAsc = @" <br/> <img src='..\Imagens\sort_asc.png' title='Ascending' />";
            string imgDes = @" <br/> <img src='..\Imagens\sort_desc.png' title='Descendng' />";

            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    LinkButton lbSort = (LinkButton)cell.Controls[0];

                    if (ViewState["SortExpression"] != null)
                        if (lbSort.CommandArgument == ViewState["SortExpression"].ToString())
                        {
                            if (ViewState["SortDirection"] == "ASC")
                                lbSort.Text += imgAsc;
                            else
                                lbSort.Text += imgDes;
                        }
                }
            }
        }

        #endregion Sort


    }
}
