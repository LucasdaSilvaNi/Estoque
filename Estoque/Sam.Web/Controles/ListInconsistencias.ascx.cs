using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sam.Web.Controles
{
    public partial class ListInconsistencias : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        
        public void ExibirLista(IList MsgList)
        {
            this.ListErros.DataSource = MsgList;
            this.ListErros.DataBind();

            if (this.ListErros.Items.Count > 0)
            {
                this.ListErros.Visible = true;
                this.divListErro.Visible = true;
            }
        }
        public int ErroCount()
        {
            return this.ListErros.Items.Count;
        }
    }

    
}
