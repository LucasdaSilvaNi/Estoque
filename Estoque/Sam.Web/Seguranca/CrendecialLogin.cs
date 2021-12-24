using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Sam.Entity;
using System.Web.Caching;
using Sam.Presenter;
using Sam.View;
using Sam.Facade;

namespace Sam.Web.Seguranca
{
    public class CrendecialLogin : System.Web.UI.Page, ILoginView
    {
        private String senhaLogin = string.Empty;
        private String usuarioLogin = string.Empty;

        private CrendecialLogin()
        {
        }

        public static CrendecialLogin getCrendecialLogin()
        {
            return new CrendecialLogin();
        }
        private CacheItemRemovedCallback _onRemove = null;

        private FormsAuthenticationTicket CriaTicket(Acesso acesso)
        {
            var ticket = new FormsAuthenticationTicket(1,
                acesso.Cpf,
                DateTime.Now,
                DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout),
                true,
                acesso.LoginBase,
                FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return ticket;
        }

        private FormsAuthenticationTicket CriaTicket(Sam.Entity.Login LoginUsuario)
        {
            var ticket = new FormsAuthenticationTicket(1,
                LoginUsuario.Usuario.Cpf,
                DateTime.Now,
                DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout),
                true,
                LoginUsuario.LoginBase,
                FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return ticket;
        }

        public void RemovedCallback(String key, Object value, CacheItemRemovedReason reason)
        {
        }

        private void CriarCache(string nome, object valor)
        {
            _onRemove = new CacheItemRemovedCallback(this.RemovedCallback);

            Cache.Add(nome
                        , valor
                        , null
                        , Cache.NoAbsoluteExpiration
                        , new TimeSpan(0, HttpContext.Current.Session.Timeout, 0)
                        , CacheItemPriority.Default
                        , _onRemove);
        }

        private void RemoverCache(string nome)
        {
            Cache.Remove(nome);
            Cache.Remove(string.Format("Perfil_{0}", nome));
            Application.Remove(nome);
            Session.Clear();
            HttpContext.Current.Cache.Remove(nome);


        }

        private void CriarCache(Sam.Entity.Login loginUsuario)
        {
            try
            {
                //Sempre limpa o cache ao fazer login.
                RemoverCache(loginUsuario.LoginBase);

                if (Cache[loginUsuario.LoginBase] == null)
                {
                    this.CriaTicket(loginUsuario);
                    CriarCache(loginUsuario.LoginBase, loginUsuario);
                }
            }
            catch (Exception ex)
            {
                new Presenter.LoginPresenter().GravarLogErro(ex);
                List<string> listaErros = new List<string>();
                listaErros.Add(ex.Message);
            }
        }

        private void VerifiarUsuarioLogado(string usuario)
        {
            if (Cache[usuario] != null)
            {
                RemoverCache(usuario);
                Exception ex = new Exception(String.Format("Usuário: {0} - O sistema ainda identificou o seu ultimo login. Tente novamente.", usuario));
                new Sam.Presenter.LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message);
            }
        }

     
        public void AutenticarUsuarioComPerfil()
        {
            LoginPresenter login = new LoginPresenter(this);
            
            try
            {
                Acesso acesso = login.AutenticarComPerfil();

                if (acesso != null)
                {

                    //Sempre limpa o cache ao fazer login.
                    RemoverCache(acesso.LoginBase);
                    if (Cache[acesso.LoginBase] == null || acesso.Transacoes.Perfis[0].IdPerfil == (int)Common.Util.GeralEnum.TipoPerfil.AdministradorGeral)
                    {
                        this.CriaTicket(acesso);
                        CriarCache(acesso.LoginBase, acesso);

                        if (acesso.Transacoes.Perfis.Count == 0)
                        {
                            throw new Exception("O usuário não possui perfis cadastrados no sistema!");
                        }

                        CriarCache(string.Format("Perfil_{0}", acesso.LoginBase)
                                    , acesso.Transacoes.Perfis[0].IdPerfil);

                        Session.Add("usuarioCahe", acesso.LoginBase);
                        Session.Add("usuarioPerfil", acesso.Transacoes.Perfis[0].IdPerfil);
                    }
                    else
                    {

                        throw new Exception("O usuário já está logado!");

                    }

                    //Cria uma variável de aplicação para saber se o usuário está logado ou não
                    Application[acesso.LoginBase] = "true";
                    Application[acesso.Cpf] = "true";
                    Response.Redirect(FormsAuthentication.GetRedirectUrl(acesso.NomeUsuario, true), false);
                }
            }
            catch (Exception exc)
            {
                new Presenter.LoginPresenter().GravarLogErro(exc);
                List<string> listaErros = new List<string>();
                listaErros.Add(exc.Message);
            }


        }

        private void AutenticarUsuarioTrocarPerfil()
        {
            try
            {
                LoginPresenter login = new LoginPresenter(this);
                var loginRetorno = login.AutenticarTrocarPerfil();

                if (loginRetorno != null)
                {
                    //VerifiarUsuarioLogado(loginRetorno.LoginBase);

                    RemoverCache(loginRetorno.LoginBase);

                    //Cria o cache
                    CriarCache(loginRetorno);

                    Response.Redirect("SelecionarPerfil.aspx", false);
                }
            }
            catch (Exception ex)
            {
                new Presenter.LoginPresenter().GravarLogErro(ex);
                List<string> listaErros = new List<string>();
                listaErros.Add(ex.Message);
              
            }
        }
        public string Usuario
        {
            get { return this.usuarioLogin; }
            set { this.usuarioLogin = value; }
        }

        public string Senha
        {
            get
            {
                string senha = this.senhaLogin;
                //return senha;
                string senhaCriptografada = FacadeLogin.CriptografarSenha(senha);
                return senhaCriptografada;
            }
            set { this.senhaLogin = value; }
        }

        public System.Collections.IList ListaErros
        {
            set { throw new NotImplementedException(); }
        }

        public string SessionId
        {
            get { return Page.Session.SessionID; }
        }

        public string TrocarPerfil
        {
            get { throw new NotImplementedException(); }
        }
    }


}