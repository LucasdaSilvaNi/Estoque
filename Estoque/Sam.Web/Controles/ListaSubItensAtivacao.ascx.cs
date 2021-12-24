using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sam.Web.Controles
{
    public partial class ListaSubItensAtivacao : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ExibirLista(IList MsgList)
        {
            this.ListMensagem.DataSource = MsgList;
            this.ListMensagem.DataBind();

            if (this.ListMensagem.Items.Count > 0)
            {
                this.ListMensagem.Visible = true;
                this.divListMensagem.Visible = true;
            }
        }
        public int ErroCount()
        {
            return this.ListMensagem.Items.Count;
        }
    }
}
