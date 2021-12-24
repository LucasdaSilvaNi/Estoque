using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sam.Web
{
    public partial class CSharp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            Captcha1.ValidateCaptcha(txtCaptcha.Text.Trim());
            if (Captcha1.UserValidated)
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Valid";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "InValid";
            }
        }
    }
}
