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
    public partial class WSSenha : System.Web.UI.UserControl
    {
        public static string ChaveSessao_CampoLogin_WsSiafem = "loginWsSiafem";
        public static string ChaveSessao_CampoSenha_WsSiafem = "senhaWsSiafem";

        public delegate void chamaEvento();
        public event chamaEvento EvchamaEvento;
        public delegate void EventoCancelar();
        public event EventoCancelar EvchamaEventoCancelar;


        public string SenhaSiafem 
        {
            get { return txtSenha.Text; }
            set { txtSenha.Text = value; }
        }

        public string LoginSiafem
        {
            get { return txtUserName.Text; }
            set { txtUserName.Text = value; }
        }

        protected void Page_Init(object sender, EventArgs e) 
        {
            var objAcesso = new PageBase().GetAcesso;

            //if (PageBase.GetSession<string>("loginWsSiafem") == null)
            if (objAcesso.IsNotNull() && PageBase.GetSession<string>("loginWsSiafem") == null)
                PageBase.SetSession<string>(objAcesso.Cpf, "loginWsSiafem");

            txtUserName.Text = PageBase.GetSession<string>("loginWsSiafem");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(PageBase.GetSession<string>("loginWsSiafem") != txtUserName.Text)
                PageBase.SetSession<string>(txtUserName.Text, "loginWsSiafem");

            txtUserName.Focus();
        }

        protected void btnOKSenha_Click(object sender, EventArgs e)
        {
            WebUtil webUtil = new WebUtil();
            PageBase.SetSession<string>(txtUserName.Text, "loginWsSiafem");
            PageBase.SetSession<string>(txtSenha.Text, "senhaWsSiafem");
            webUtil.runJScript(this.Parent.Page, "CloseModalSenhaWs();");
            webUtil.runJScript(this.Parent.Page, "btnOkSenha();");

            EvchamaEvento();
        }

        public void SetFocusOnLoad()
        {
            txtUserName.Focus();
        }

        protected void btnCancelarSenha_Click(object sender, EventArgs e)
        {
            EvchamaEventoCancelar();
           
        }
    }
}
