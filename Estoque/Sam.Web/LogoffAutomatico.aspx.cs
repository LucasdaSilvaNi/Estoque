using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Sam.Web.MasterPage;
using Sam.Entity;
using Sam.Presenter;

namespace Sam.Web
{
    public partial class LogoffAutomatico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var _logoff = Request.QueryString["lf"];
                var _logID = Request.QueryString["li"];

                // Caso seja fechada a janela por Duplicidade, não será encerrada a Sessão
                if (_logoff == "2")
                    return;

                if (Session.Count > 0)
                {
                    sairDoSistema();
                }
                else
                {
                    HttpContext.Current.Cache.Remove(string.Format("Perfil_{0}", PrincipalFull.Ticket));
                    FormsAuthentication.SignOut();

                    if (!string.IsNullOrEmpty(_logID))
                        UsuarioLogadoRemove(int.Parse(Request.QueryString["li"]));
                }

                if (!string.IsNullOrEmpty(_logoff) && (_logoff.Equals("0") || _logoff.Equals("1")))
                {
                    Response.Redirect("login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao realizar logoff do usuário");
            }
        }

        private void UsuarioLogadoRemove(Int32 id)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
            usuarioPresenter.RemoveUsuarioLogadoId(id);
        }

        public void sairDoSistema()
        {
            var IdLoginLogado = Convert.ToInt32(Session["IdLoginLogado"].ToString());

            UsuarioLogadoEntity usuario = new UsuarioLogadoEntity();
            UsuarioLogadoRemove(IdLoginLogado);

            if (Session["usuarioCahe"] != null)
            {
                HttpContext.Current.Cache.Remove(Session["usuarioCahe"].ToString());
                RemoverCache(Session["usuarioCahe"].ToString());
            }

            Session["usuarioCahe"] = null;
            HttpContext.Current.Request.Cookies.Clear();
            HttpContext.Current.Response.Cookies.Clear();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Cache.Remove(string.Format("Perfil_{0}", PrincipalFull.Ticket));

            FormsAuthentication.SignOut();
        }

        private void RemoverCache(string nome)
        {
            Cache.Remove(nome);
            Cache.Remove(string.Format("Perfil_{0}", nome));
            Application.Remove(nome);
            Session.Clear();
            HttpContext.Current.Cache.Remove(nome);
        }
    }
}
