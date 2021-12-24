using System;
using System.Globalization;
using System.Threading;
using System.Web.UI;
using Sam.View;
using Sam.Domain;
//using Sam.Domain.Business;

namespace Sam.Web
{

    public partial class TesteWebservice : Page, ITesteWebserviceView 
    {

        private clsTesteWebservice lObjTesterWsProd;
        private clsTesteWebservice lObjTesterWsHml;

        protected override void InitializeCulture()
        {
            Thread.CurrentThread.CurrentCulture   = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            base.InitializeCulture();
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TestarWebservices();
                LimparControles();
            }

        }

        public void TestarWebservices()
        {

            #region WS Produção

            lObjTesterWsProd = new clsTesteWebservice(clsTesteWebservice.PRODUCAO);
            lObjTesterWsProd.ObterStatusServico();

            if (!String.IsNullOrEmpty(lObjTesterWsProd.Erro))
            {
                lblMsgDesenvolvimento.Text += lObjTesterWsProd.Erro;
                lblMsgDesenvolvimento.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblMsgDesenvolvimento.Text += lObjTesterWsProd.Messagem;
                lblMsgDesenvolvimento.ForeColor = System.Drawing.Color.Blue;
            }

            #endregion WS Produção

            #region WS Homologação

            lObjTesterWsHml = new clsTesteWebservice(clsTesteWebservice.HOMOLOGACAO);
            lObjTesterWsHml.ObterStatusServico();

            if (!String.IsNullOrEmpty(lObjTesterWsHml.Erro))
            {
                lblMsgHomologacao.Text += lObjTesterWsHml.Erro;
                lblMsgHomologacao.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblMsgHomologacao.Text += lObjTesterWsHml.Messagem;
                lblMsgHomologacao.ForeColor = System.Drawing.Color.Blue;
            }

            #endregion WS Homologação

        }
        public void LimparControles()
        {
            Master.FindControl("pnlBarraGestor").Visible = false;
            Master.FindControl("pnlUsuario").Visible = false;
        }

    }

    
}
