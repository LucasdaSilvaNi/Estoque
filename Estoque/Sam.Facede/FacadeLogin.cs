using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Business;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sam.Domain.Entity;
using logErro = Sam.Domain.Business.LogErro;
using Sam.Common.Util;

namespace Sam.Facade
{
    public static class FacadeLogin
    {

        /// <summary>
        /// Autenticar Usuário sem carregar o perfil
        /// </summary>
        /// <param name="login">Usuário CPF</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static Login Autenticar(string login, string senha, string sessionId)
        {
            Login loginUsuario = new LoginBusiness(login, senha).Autenticar().FirstOrDefault();

            if(loginUsuario != null)
            {
                loginUsuario.LoginBase = Guid.NewGuid().ToString("N");                
                return loginUsuario;
            }
            else
            {
                throw new Exception("Login ou Senha inválido.");
            }
        }

        /// <summary>
        /// Autenticar usuário Carregando o Perfil
        /// </summary>
        /// <param name="login"></param>
        /// <param name="senha"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static Acesso AutenticarUsuarioComPerfil(string login, string senha, string sessionId)
        {
            Acesso acesso = new Acesso();

            Estrutura estrutura = null;

            //Autenticar usuário e senha
            var loginUsuario = new LoginBusiness(login, senha).Autenticar().FirstOrDefault();

            //Retorna Perfil do usuário
            var perfils = new PerfilBusiness(loginUsuario.ID).RecuperarPerfil();

            //Carregar informações do usuário                        
            var usuario = UsuarioBusiness.RecuperarInformacoesUsuario(login).FirstOrDefault();

                if (loginUsuario != null)
                {
                    loginUsuario.Perfis = perfils;

                    if (usuario != null)
                    {
                        //loginUsuario.Peso;
                        usuario.Login = loginUsuario;
                    }

                    //Carregar a estrutura de perfil
                    estrutura = new EstruturaBusiness().BuscarEstruturas(loginUsuario.ID);
                }            

            if (usuario != null)
            {
                acesso.Cpf = usuario.Cpf;
                acesso.NomeUsuario = usuario.NomeUsuario;
            }

            acesso.Estruturas = estrutura;
            acesso.LoginBase = new LoginBusiness().CriptografarSenha(login); //Guid.NewGuid().ToString("N");
            acesso.SessionId = sessionId;
            acesso.Transacoes = loginUsuario;

            AlmoxarifadoEntity almoxPadrao = null;
            if (estrutura.Almoxarifado != null)
               almoxPadrao =  estrutura.Almoxarifado
                        .Where(almox => almox.AlmoxDefault.Equals(true))
                        .Select(almox => almox).FirstOrDefault();

                acesso.TipoCorrente = almoxPadrao;


                //_strDescLog = String.Format("Inicio Execução Método Sam.Facade.FacadeLogin.AutenticarUsuarioComPerfil(string, string, string); args(login: {0}, senha: {1}, sessionId: {2})", (String.IsNullOrWhiteSpace(login) ? "NULL" : login), (String.IsNullOrWhiteSpace(senha) ? "NULL" : senha), (String.IsNullOrWhiteSpace(sessionId) ? "NULL" : sessionId));
                //logErro.GravarMsgInfo(Constante.CST_DEBUG_DEPLOY_HOMOLOGACAO, _strDescLog);

            return acesso;
        }

        private static bool AlterarSenha(string login
                                , string senhaAtual
                                , string novaSenha)
        {

            return true;
        }

        private static void ManterConectado(bool manterConectado)
        {

        }

        public static string CriptografarSenha(string senha)
        {
            string senhaCriptografada = new LoginBusiness().CriptografarSenha(senha);

            return senhaCriptografada;
        }

        private static void ReenviarSenha(string email)
        { }

        private static string DicaSenha(string login)
        {
            return string.Empty;
        }
    }
}
