using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common;
using Sam.Entity;
using Sam.Infrastructure;
using Sam.Domain.Business;
using Sam.Common.Util;

namespace Sam.Business
{
    public class LoginBusiness: BaseBusinessSeguranca, ICrudBaseBusiness<TB_LOGIN>
    {
        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }


        private Sam.Entity.Login login = new Sam.Entity.Login();

        public Sam.Entity.Login Login
        {
            get { return login; }
            set { login = value; }
        }

        public LoginBusiness()
        {
        }

        public LoginBusiness(string login, string senha)
            :base(login, senha)
        {}

        /// <summary>
        /// Criptofraga a senha no padrão MD5
        /// </summary>
        /// <param name="senha"></param>
        /// <returns>Senha Criptografada</returns>
        public string CriptografarSenha(string senha)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(senha);
            byte[] hash = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Validar Usuario
        /// </summary>
        public void ValidarUsuario()
        {
            if (string.IsNullOrEmpty(base._usuario))
                throw new Exception(Msg.ErroUserOb);

            if(string.IsNullOrEmpty(base._senha))
                throw new Exception(Msg.ErroPassOb);
        }
        /// <summary>
        /// Novo Método Responsável por autenticar usuário
        /// </summary>
        public List<Login> Autenticar()
        {
            //Resolve Autenticação

            List<Login> resposta = null;

            ValidarUsuario();
            resposta = new Infrastructure.LoginInfrastructure().AutenticarUsuario(base._usuario, base._senha);

            return resposta;

        }
        /// <summary>
        /// Retorna Frase lembrete de senha
        /// </summary>
        public string LembreteSenha()
        {
            return Infrastructure.LoginInfrastructure.RecuperarDicaSenha(base._usuario);
        }
        /// <summary>
        /// Validar Se a dica de senha confere
        /// </summary>
        public bool ValidarDicaSenha(string dicaSenha)
        {
            string dicaServidor = Infrastructure.LoginInfrastructure.RecuperarDicaSenha(base._usuario);
            
            return dicaSenha.Equals(dicaServidor);   
        }
        /// <summary>
        /// Recuperar Objeto Usuario para enviar por email senha
        /// </summary>
        /// <returns></returns>
        public Usuario RecuperarSenha()
        {
            return Infrastructure.LoginInfrastructure.RecuperarSenha(base._usuario);
        }

        public bool SalvarLogin()
        {
            try
            {
                Infrastructure.LoginInfrastructure infra = new Infrastructure.LoginInfrastructure();
                infra.Entity = Login;
                infra.Entity.Senha = CriptografarSenha(infra.Entity.Senha);
                infra.Salvar();
                return true;
            }
            catch (Exception e) 
            {
                base.GravarLogErro(e);
                throw new Exception("Erro no sistema: " + e.Message);
                return false;
            }
        }

        public bool SalvarLoginComPerfis()
        {
            try
            {

                Infrastructure.LoginInfrastructure infra = new Infrastructure.LoginInfrastructure();
                infra.Entity = Login;
                infra.Perfil = Perfil;
                infra.Entity.Senha = CriptografarSenha(infra.Entity.Senha);
                //if (!ConsistirPerfilLogin())
                //    return false;

                infra.SalvarComPerfis();
                return true;
            }
            catch (Exception e) 
            {
                base.GravarLogErro(e);
                throw new Exception("Erro no sistema: " + e.Message);
                return false;
            }
        }

        protected bool ConsistirPerfilLogin() 
        {
            if (this.Perfil.OrgaoPadrao == null || Perfil.OrgaoPadrao.Id == 0)
                throw new Exception("Informar o órgão.");
            
            if (this.Perfil.AlmoxarifadoPadrao == null || Perfil.AlmoxarifadoPadrao.Id == 0)
                throw new Exception("Informar o almoxarifado.");

            if (this.Perfil.GestorPadrao == null || Perfil.GestorPadrao.Id == 0)
                throw new Exception("Informar o gestor.");

            if (this.ListaErro.Count > 0)
                return false;

            return true;
        }


        public Login RecuperarInformacoesLogin()
        {
            throw new NotImplementedException();
        }

        public Login RecuperarInformacoesLoginPorUserId(int UserId)
        {
            return new Infrastructure.LoginInfrastructure().ListarPorUserId(UserId);
        }

        public string ObterCPFNomeUsuarioSAM(int usuarioSamID)
        {
            string dadosUsuario = null;
            LoginBusiness objInfra = new LoginBusiness();

            var retornoQuery = objInfra.SelectOne(loginUsuarioSAM => loginUsuarioSAM.TB_LOGIN_ID == usuarioSamID);
            if (retornoQuery.IsNotNull())
                dadosUsuario = String.Format("{0} - {1}", retornoQuery.TB_USUARIO.TB_USUARIO_CPF, retornoQuery.TB_USUARIO.TB_USUARIO_NOME_USUARIO);

            return dadosUsuario;
        }

        public void Insert(TB_LOGIN entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TB_LOGIN entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TB_LOGIN entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelatedEntries(TB_LOGIN entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelatedEntries(TB_LOGIN entity, System.Collections.ObjectModel.ObservableCollection<string> keyListOfIgnoreEntites)
        {
            throw new NotImplementedException();
        }

        public TB_LOGIN SelectOne(System.Linq.Expressions.Expression<Func<TB_LOGIN, bool>> where)
        {
            try
            {
                LoginNovoInfrastructure infraestrutura = new LoginNovoInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public List<TB_LOGIN> SelectAll()
        {
            throw new NotImplementedException();
        }

        public List<TB_LOGIN> SelectAll(System.Linq.Expressions.Expression<Func<TB_LOGIN, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            throw new NotImplementedException();
        }

        public List<TB_LOGIN> SelectWhere(System.Linq.Expressions.Expression<Func<TB_LOGIN, bool>> where)
        {
            throw new NotImplementedException();
        }

        public List<TB_LOGIN> SelectWhere(System.Linq.Expressions.Expression<Func<TB_LOGIN, int>> sortExpression, bool desc, System.Linq.Expressions.Expression<Func<TB_LOGIN, bool>> where, int startRowIndex)
        {
            throw new NotImplementedException();
        }

        public List<TB_LOGIN> SelectWhere(System.Linq.Expressions.Expression<Func<TB_LOGIN, string>> sortExpression, bool desc, System.Linq.Expressions.Expression<Func<TB_LOGIN, bool>> where, int startRowIndex)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TB_LOGIN> QueryAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TB_LOGIN> QueryAll(System.Linq.Expressions.Expression<Func<TB_LOGIN, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }

        public int GetCount(System.Linq.Expressions.Expression<Func<TB_LOGIN, bool>> where)
        {
            throw new NotImplementedException();
        }

        public void Consistir(TB_LOGIN entity)
        {
            throw new NotImplementedException();
        }
    }
}
