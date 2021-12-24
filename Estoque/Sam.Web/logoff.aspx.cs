using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Sam.Web.MasterPage;
using Sam.Facade;

namespace Sam.Web
{
    public partial class logoff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int perfil = Convert.ToInt32(Session["usuarioPerfil"].ToString());

                if (perfil != (int)Common.Util.GeralEnum.TipoPerfil.AdministradorGeral)
                {
                    Response.Redirect(FormsAuthentication.GetRedirectUrl(Session["usuarioCahe"].ToString(), true), false);
                }
                
            }
        }

        public void sairDoSistema()
        {

            HttpContext.Current.Cache.Remove(Session["usuarioCahe"].ToString());
            RemoverCache(Session["usuarioCahe"].ToString());
            Response.Redirect(FormsAuthentication.GetRedirectUrl(Session["usuarioCahe"].ToString(), true), false);
        }
        public void sairDoSistemaBckp()
        {
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
            Session.Abandon();

        }
        public void sairDoSistema(string usuario)
        {
            try
            {

                HttpContext.Current.Cache.Remove(FacadeLogin.CriptografarSenha(usuario));
                RemoverCache(FacadeLogin.CriptografarSenha(usuario));

               
                List<string> listaLogoff = new List<string>();
                listaLogoff.Add("Usuário desbloqueado com sucesso!");
                ListInconsistencias.ExibirLista(listaLogoff);
                ListInconsistencias.DataBind();
                listaLogoff = null;
            }
            catch (Exception ex)
            {
                new Presenter.LoginPresenter().GravarLogErro(ex);
                List<string> listaErros = new List<string>();
                listaErros.Add(ex.Message);
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();
            }
           


            

        }
        private void RemoverCache(string nome)
        {
            Cache.Remove(nome);
            Cache.Remove(string.Format("Perfil_{0}", nome));
            Application.Remove(nome);
            Session.Clear();
            HttpContext.Current.Cache.Remove(nome);


        }

        protected void btnLogoff_Click(object sender, EventArgs e)
        {
            if (this.txtUsuario.Text.Trim().Length > 0)
            {
                sairDoSistema(this.txtUsuario.Text);
            }
        }
    }
}
