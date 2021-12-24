using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Facade;


namespace Sam.Web.Controles
{
    public partial class ModalSenha : System.Web.UI.UserControl
    {    
        protected void Page_Init(object sender, EventArgs e) 
        {
            txtUserName.Text = new PageBase().GetAcesso.Cpf;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        protected void btnOKSenha_Click(object sender, EventArgs e)
        {

            WebUtil webUtil = new WebUtil();
            string usuario = txtUserName.Text;
            string senha = txtSenha.Text;
            string senhaCriptografada = FacadeLogin.CriptografarSenha(senha);

            Entity.Login login = new LoginPresenter().ValidarSenhaUsuario(usuario, senhaCriptografada);

            if (login != null)
            {
                webUtil.runJScript(this.Parent.Page, "btnOkSenha();");
                webUtil.runJScript(this.Parent.Page, "CloseModalSenhaWs();");
            }
            else
            {

            }
        }

        public void SetFocusOnLoad()
        {
            txtUserName.Focus();
        }
    }
}
