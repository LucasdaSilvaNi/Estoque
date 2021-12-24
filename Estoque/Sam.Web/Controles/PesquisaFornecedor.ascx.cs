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
    public partial class PesquisaFornecedor : System.Web.UI.UserControl
    {
        protected const string sessionForn = "gridFornecedorSession";

        protected void Page_Load(object sender, EventArgs e)
        {
            //gridFornecedor.EmptyDataText = "";
        }

        public void PopularFornecedorPorPalavraChave() 
        {
            FornecedorPresenter forn = new FornecedorPresenter();            
                IList<FornecedorEntity> lstForn = forn.PopularFornecedorPorPalavraChave(txtFornecedor.Text.Trim());
                gridFornecedor.PageSize = 20;
                gridFornecedor.DataSource = lstForn;
                gridFornecedor.DataBind();
                PageBase.SetSession(lstForn, sessionForn);
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {
            PopularFornecedorPorPalavraChave();
        }

        protected void gridFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gridFornecedor.Rows[gridFornecedor.SelectedIndex];
            LinkButton lnk = (LinkButton)gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lnkCnpj");
            TextBox txtID = (TextBox)PageBase.GetSession<TextBox>("fornecedorId");
            TextBox txtDesc = (TextBox)PageBase.GetSession<TextBox>("fornecedorDados");
            TextBox texto = (TextBox)this.Parent.FindControl(txtDesc.ID);
            TextBox txtCod = (TextBox)this.Parent.FindControl(txtID.ID);
            TextBox textoNome = (TextBox)gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("txtNome_");
            texto.Text = lnk.Text + " - " + textoNome.Text;
            Label fornId = (Label)gridFornecedor.Rows[gridFornecedor.SelectedIndex].FindControl("lblCodFornecedor");
            txtCod.Text = fornId.Text;
            PageBase.RemoveSession("fornecedorId");
            PageBase.RemoveSession("fornecedorDados");
            PageBase.RemoveSession(sessionForn);
        }

        protected void gridFornecedor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridFornecedor.PageIndex = e.NewPageIndex;
            if (Session[sessionForn] != null)
            {
                gridFornecedor.DataSource = PageBase.GetSession<List<FornecedorEntity>>(sessionForn);
                gridFornecedor.DataBind();
            }
        }

    }
}
