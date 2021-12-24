using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Facade;
using Sam.View;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Entity;

namespace Sam.Presenter
{
    public class LoginPresenter  
    {
        ILoginView _view;

        public ILoginView View
        {
            get { return _view;}
            set { _view = value; }
        }

        public LoginPresenter()
        {
        }

        public LoginPresenter(ILoginView view)
        {
            this.View = view;
        }
             
        public Acesso AutenticarComPerfil()
        {
            //return FacadeLogin.AutenticarUsuarioComPerfil(View.Usuario, View.Senha, View.SessionId);
            return FacadeLogin.AutenticarUsuarioComPerfil(View.Usuario, View.Senha, View.SessionId);
        }

        public Login AutenticarTrocarPerfil()
        {
            return FacadeLogin.Autenticar(View.Usuario, View.Senha, View.SessionId);
        }

        public Login ValidarSenhaUsuario(string usuario, string Senha)
        {
            return FacadeLogin.Autenticar(View.Usuario, View.Senha, null);
        }

        //public IList<UsuarioEntity> PopularDados(string cpf)
        //{
        //    UsuarioBusiness usuario = new UsuarioBusiness();
        //    IList<UsuarioEntity> retorno = usuario.ListarUsuario(cpf);
        //    return retorno;
        //}

        public Login PopularDadosLoginPorUserId(int _userId)
        {
            Sam.Business.LoginBusiness log = new Sam.Business.LoginBusiness("","");
            return log.RecuperarInformacoesLoginPorUserId(_userId);
        }

        public void GravarLogErro(Exception ex)
        {
            new LogErro().GravarLogErro(ex);
        }
    }
}
