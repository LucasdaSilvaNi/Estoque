using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Sam.Presenter;
using Sam.Entity;
using Sam.View;
using Sam.Domain.Entity;
using System.Threading;
using System.Globalization;
using Sam.Facade;
using logErro = Sam.Domain.Business.LogErro;
using Sam.Common.Util;
using Sam.Web.Seguranca;
using System.Net;
using Sam.Common;
using System.Text;
using Sam.Common.Enums;
using Sam.Web.MasterPage;

namespace Sam.Web
{
    public partial class login : System.Web.UI.Page, ILoginView
    {
        private String urlPatrimonio = String.Empty;
        private Guid? token = null;
        private DateTime? dataDoToken = null;
        private Uri urlorigem = null;

        private bool verificarTokenAtivo()
        {
            if (this.dataDoToken.HasValue && (this.dataDoToken.Value - DateTime.UtcNow).TotalHours < 1)
                return false;
            else
                return true;
        }

        private void decodeToken()
        {
            EstoqueToken _token = EstoqueToken.getEstoqueToken();

            string[] urlToken = urlPatrimonio.Split('?');
            string[] parametro = urlToken[1].Split(';');

            _token.DecodeToken(parametro[0].Replace("Token=", ""));

            this.token = _token.getTokenKey();
            this.dataDoToken = _token.getDataDoToken();
        }

        private void decodeLogin()
        {
            EstoqueToken _token = EstoqueToken.getEstoqueToken();

            string[] urlToken = urlPatrimonio.Split('?');
            string[] parametro = urlToken[1].Split(';');

            _token.DecodeLogin(parametro[1].Replace("Id=", ""));
            Session.Add("senhaCriptografada", true);
            this.Usuario = _token.getUsuario();
            this.Senha = _token.getSenha();
        }

        private bool verificarIpPatrimonio()
        {
            try
            {
                urlorigem = Request.UrlReferrer;

                if (urlorigem != null && urlorigem.Host.Equals(EstoqueToken.GetUrlPatrimonio(), StringComparison.CurrentCultureIgnoreCase))
                {
                    this.urlPatrimonio = (Session["patrimonioUrl"] != null ? Session["patrimonioUrl"].ToString() : Application["patrimonioUrl"].ToString());
                    decodeToken();
                    decodeLogin();

                    AutenticarUsuarioComPerfil();
                    Session.Add("MenuPatrimonio", true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                new Presenter.LoginPresenter().GravarLogErro(ex);
                List<string> listaErros = new List<string>();
                ListInconsistencias.Attributes.Clear();
                listaErros.Add("Usuário sem perfil para acessar o Nível!");
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();

                return false;
            }
            return false;
        }
        protected override void InitializeCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            base.InitializeCulture();
        }

        CacheItemRemovedCallback _onRemove = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.txtUsuario, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            ((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;
            ((Panel)Master.FindControl("pnlUsuario")).Visible = false;
            ((Menu)Master.FindControl("mnSam")).Visible = false;

            if (!IsPostBack)
            {
                txtUsuario.Focus();

                if (verificarIpPatrimonio())
                    Response.Redirect(this.urlPatrimonio);
            }
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
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();
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

        private void AutenticarUsuarioComPerfil()
        {
            LoginPresenter login = new LoginPresenter(this);
            try
            {
                Acesso acesso = login.AutenticarComPerfil();

                if (acesso != null)
                {
                    if (acesso.Transacoes.Perfis.Count == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "habilitarBotaoSubmit", "desabilitaDuploEnter(false)", true);
                        throw new Exception("O usuário não possui perfis cadastrados no sistema!");
                    }

                    var usuarioLogado = UsuarioLogadoGet(acesso.Transacoes.Perfis[0].IdLogin);

                    if (usuarioLogado != null && acesso.Transacoes.Perfis[0].IdPerfil != EnumPerfil.ADMINISTRADOR_GERAL.GetHashCode())
                    {
                        var timeout = Helper.ObterTimeoutDoConfig();

                        if (usuarioLogado.DataHoraLogado.AddMinutes(timeout) > DateTime.Now)
                        {
                            var listaErro = new List<string>();
                            listaErro.Add("Usuário já está utilizando o Sistema");
                            ListInconsistencias.ExibirLista(listaErro);
                            ScriptManager.RegisterStartupScript(this, GetType(), "habilitarBotaoSubmit", "desabilitaDuploEnter(false)", true);
                            return;
                        }

                        UsuarioLogadoRemove(usuarioLogado);
                        usuarioLogado = null;
                    }

                    var _gestor = acesso.Transacoes.Perfis[0].GestorPadrao;
                    var _perfil = (GeralEnum.TipoPerfil)acesso.Transacoes.Perfis[0].IdPerfil;

                    // Não executa validação para o perfil Administrador_Geral
                    if (!_perfil.Equals(EnumPerfil.ADMINISTRADOR_GERAL.GetHashCode()))
                    {
                        // Bloqueia o acesso, conforme a quantidade de usuários 
                        // logados exceder a quantidade contratada na ESP
                        var _qtdeUsuariosOnlineValida = ConsistirUsuariosOnline(_gestor, _perfil);
                        if (_qtdeUsuariosOnlineValida.Equals(false))
                            return;
                    }

                    //Sempre limpa o cache ao fazer login.
                    RemoverCache(acesso.LoginBase);

                    this.CriaTicket(acesso);
                    CriarCache(acesso.LoginBase, acesso);

                    UsuarioLogadoEntity usuario = new UsuarioLogadoEntity();
                    usuario.DataHoraLogado = DateTime.Now;
                    usuario.LoginId = acesso.Transacoes.Perfis[0].IdLogin;
                    usuario.IpLogado = Request.UserHostAddress;
                    usuario.SessionIdLogado = acesso.SessionId;
                    UsuarioLogadoCreate(usuario);

                    Session.Add("IdLoginLogado", usuario.LoginId);
                    CriarCache(string.Format("Perfil_{0}", acesso.LoginBase), acesso.Transacoes.Perfis[0].IdPerfil);

                    Session.Add("usuarioCahe", acesso.LoginBase);
                    Session.Add("usuarioPerfil", acesso.Transacoes.Perfis[0].IdPerfil);

                    //Cria uma variável de aplicação para saber se o usuário está logado ou não
                    Application[acesso.LoginBase] = "true";
                    Application[acesso.Cpf] = "true";

                    Session.Add("RESET_TABCOUNTER", "1");
                    Response.Redirect(FormsAuthentication.GetRedirectUrl(acesso.NomeUsuario, true), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception exc)
            {
                new Presenter.LoginPresenter().GravarLogErro(exc);
                List<string> listaErros = new List<string>();
                listaErros.Add(exc.Message);
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();

                if (urlorigem != null)
                    throw new Exception(exc.Message, exc.InnerException);
            }
        }

        private bool ConsistirUsuariosOnline(GestorEntity gestor, GeralEnum.TipoPerfil perfil)
        {
            var _nivelAcesso = GeralEnum.ObterPerfilNivelAcessoPorPerfil(perfil);

            // Verifica se o usuário tem perfil Administrativo e não realiza a consistência de licenças contratadas
            if (_nivelAcesso == PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_III)
                return true;

            var gestorId = TratamentoDados.ParseIntNull(gestor.Id);

            var _usuarioLogado = new UsuarioPresenter();
            var _usuarioPresenter = _usuarioLogado.ConsistirUsuarioOnline(gestorId, _nivelAcesso);
            var _qtdeContratado = _usuarioLogado.ObterQtdeUsuariosContratadoPorPerfil(gestorId, _nivelAcesso);
            var _qtdeUsuariosOnline = _usuarioLogado.ObterQtdeUsuariosOnlinePorGestorEPerfil(gestorId, _nivelAcesso);
            var _usuariosOnline = _usuarioLogado.ListarUsuariosOnlinePorGestor(gestorId, _nivelAcesso);
            var _perfilDescricao = EnumUtils.GetEnumDescription<GeralEnum.TipoPerfil>(perfil);

            List<string> listaErros = new List<string>();

            if (!_usuarioPresenter)
            {
                if (_qtdeContratado == 0)
                    listaErros.Add(string.Format("Não há licenças contratadas para o perfil {0} pelo gestor SEFAZ / {1} ({2})", _perfilDescricao, gestor.Nome, gestor.NomeReduzido));
                else
                {
                    listaErros.Add(string.Format("Quantidade de usuários utilizando o sistema ({0}) com o perfil {1} excede o contrato ({2})", _qtdeUsuariosOnline, _perfilDescricao, _qtdeContratado));
                }
            }

            ListInconsistencias.ExibirLista(listaErros);

            return _usuarioPresenter;
        }

        private void AutenticarUsuarioTrocarPerfil()
        {
            try
            {
                LoginPresenter login = new LoginPresenter(this);
                var loginRetorno = login.AutenticarTrocarPerfil();

                if (loginRetorno != null)
                {
                    RemoverCache(loginRetorno.LoginBase);

                    //Cria o cache
                    CriarCache(loginRetorno);

                    Response.Redirect("SelecionarPerfil.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
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

        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            if(TrocarPerfil == "1") //Trocar perfil
                AutenticarUsuarioTrocarPerfil();
            else
                AutenticarUsuarioComPerfil();
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
                        ,Cache.NoAbsoluteExpiration
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

        private void UsuarioLogadoCreate(UsuarioLogadoEntity usuario)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            usuarioPresenter.UsuarioLogadoCreate(usuario);

        }

        private void UsuarioLogadoRemove(UsuarioLogadoEntity usuario)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            usuarioPresenter.RemoveUsuarioLogadoId(usuario.LoginId);

        }

        private UsuarioLogadoEntity UsuarioLogadoGet(Int32 id)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
            return usuarioPresenter.UsuarioLogadoGet(id);
      

        }

        #region ILoginView Members

        public string Usuario
        {
            get { return this.txtUsuario.Text; }
            set { this.txtUsuario.Text = value; }
        }

        public string Senha
        {
            get
            {
                string senha = this.txtSenha.Text;
                //return senha;
                if (Session["senhaCriptografada"] == null)
                {
                    return FacadeLogin.CriptografarSenha(senha);
                }
                else
                    return senha;
            }
            set { this.txtSenha.Text = value; }
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
            }
        }

        public string SessionId
        {
            get
            {
                var Pagina = new Page();

                return Pagina.Session.SessionID;
            }
        }

        public string TrocarPerfil
        {
            get
            {
                return rdoPerfil.SelectedValue;
            }
        }
        #endregion
    }
}
