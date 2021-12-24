using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Infrastructure;

namespace Sam.Web.Controles
{
    public partial class PesquisaItemNova : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public bool UsaSaldo { get; set; }

        public bool FiltrarAlmoxarifadoLogado { get; set; }

        public bool FiltrarGestorLogado { get; set; }

        public void PopularDadosGridItemMaterial()
        {
            gridItemMaterial.DataSourceID = "sourceGridItemMaterial";

        }

        public IList<TB_ITEM_MATERIAL> BuscarSubItemMaterial(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave)
        {
            var presenter = new ItemMaterialPresenter();
            var result = presenter.BuscarItemMaterial(startRowIndexParameterName, palavraChave);
            this.TotalRegistrosGrid = presenter.TotalRegistrosGrid;
            return result;
        }


        int TotalRegistrosGrid = 0;
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave)
        {
            return this.TotalRegistrosGrid;
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
            
        }

        public void SetFocusOnLoad() 
        {
            txtChave.Focus();
        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }

    }
}
