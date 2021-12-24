using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Util;


namespace Sam.Web.Controles
{
    public partial class ucConversaoUnidadesEmpenho : System.Web.UI.UserControl
    {
        public delegate void chamaEvento();
        public event chamaEvento EvChamaEvento;
        public delegate void EventoCancelar();
        public event EventoCancelar EvChamaEventoCancelar;

        protected void Page_Init(object sender, EventArgs e) 
        {
            if (PageBase.GetSession<string>("dadosEmpenho") == null)
            {
                //PageBase.SetSession<string>(new PageBase().GetAcesso.Cpf, "loginWsSiafem");
            }
            //txtUserName.Text = PageBase.GetSession<string>("loginWsSiafem");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if(PageBase.GetSession<string>("loginWsSiafem") != txtUserName.Text)
            //    PageBase.SetSession<string>(txtUserName.Text, "loginWsSiafem");

            txtUnidadeFornecimentoSAM.Focus();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            WebUtil webUtil = new WebUtil();
            //PageBase.SetSession<string>(txtUserName.Text, "loginWsSiafem");
            //PageBase.SetSession<string>(txt.Text, "WsSiafem");
            webUtil.runJScript(this.Parent.Page, "CloseModalConversaoUnidadeFornecimento();");
            webUtil.runJScript(this.Parent.Page, "btnOk();");

            EvChamaEvento();
        }

        public void SetFocusOnLoad()
        {
            txtUnidadeFornecimentoSAM.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            EvChamaEventoCancelar();
           
        }
    }
}
