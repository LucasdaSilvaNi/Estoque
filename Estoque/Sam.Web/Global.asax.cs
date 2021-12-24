using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Sam.Web.Seguranca;
using Sam.Presenter;
using System.Web.Caching;

namespace Sam.Web
{
    public class Global : System.Web.HttpApplication
    {

        private void setSessionUrlPatrimonio()
        {
            Uri uriReferencia = Request.UrlReferrer;

            if (uriReferencia != null && uriReferencia.Host.Equals(EstoqueToken.GetUrlPatrimonio()))
                Session.Add("patrimonioUrl", Request.Url.PathAndQuery);



        }

        private void SetInterationReportView()
        {
            var pathQuery = HttpContext.Current.Request.Url.PathAndQuery;
            var url = HttpContext.Current.Request.Url.ToString().ToLower();

            if (pathQuery.StartsWith("/Reserved.ReportViewerWebControl.axd") && !url.Contains("iteration"))
            {
                var resourceStreamId = HttpContext.Current.Request.QueryString["ResourceStreamId"];

                if (resourceStreamId == null) { return; }

                if (resourceStreamId.ToString().Equals("blank.gif", StringComparison.InvariantCultureIgnoreCase))
                {
                    Context.RewritePath(String.Concat(HttpContext.Current.Request.Url.PathAndQuery, "&IterationId=0"));
                }
            }
        }

        private void setApplicationUrlPatrimonio()
        {
            Uri uriReferencia = Request.UrlReferrer;

            if (uriReferencia != null && uriReferencia.Host.Equals(EstoqueToken.GetUrlPatrimonio()))
                Application.Add("patrimonioUrl", Request.Url.PathAndQuery);



        }
        protected void Application_Start(object sender, EventArgs e)
        {
#if DEBUG == false
            RemoveAllUsuarioLogado();
#endif
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            setSessionUrlPatrimonio();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            setApplicationUrlPatrimonio();
            SetInterationReportView();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
       
            Global aplicacao = (Global)sender;

            if (aplicacao.Context.Error.Message.Contains("Não é possível converter um objeto do tipo 'System.Security.Principal.GenericIdentity' no tipo 'System.Web.Security.FormsIdentity'.") || aplicacao.Context.Error.Message.Contains("Referência de objeto não definida para uma instância de um objeto.") || aplicacao.Context.Error.Message.Contains("Object reference not set to an instance of an object.") || aplicacao.Context.Error.Message.Contains("USUARIO NAO LOGADO"))
                Response.Redirect("~/login.aspx", true);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            RemoveUsuarioLogadoSessionId(Session.SessionID);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            RemoveAllUsuarioLogado();

        }

        private void RemoveUsuarioLogadoSessionId(String sessionId)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            usuarioPresenter.RemoveUsuarioLogadoSessionId(sessionId);

        }

        private void RemoveAllUsuarioLogado()
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            usuarioPresenter.RemoveAllUsuarioLogado();

        }
    }
}