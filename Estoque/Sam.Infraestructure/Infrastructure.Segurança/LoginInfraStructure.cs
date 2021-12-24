using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Common;
using Sam.Domain.Entity;

namespace Sam.Infrastructure
{
    public class LoginInfrastructure: BaseInfrastructure
    {

        private Sam.Entity.Login login = new Sam.Entity.Login();

        public Sam.Entity.Login Entity
        {
            get { return login; }
            set { login = value; }
        }

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }


        /// <summary>
        /// Verifica se o Login e Senha existem cadastrados para um usuário Ativo
        /// </summary>
        /// <param name="login">Login CPF</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <returns></returns>
        public List<Login> AutenticarUsuario(string login, string senha)
        {
            IList<Login> resposta = null;

            try
            {
                resposta = (from user in base.Db.TB_LOGIN
                            join user2 in base.Db.TB_USUARIO on user.TB_USUARIO_ID equals user2.TB_USUARIO_ID
                            where user.TB_USUARIO.TB_USUARIO_CPF.Equals(login)
                               && user.TB_LOGIN_SENHA.Equals(senha)                               
                            select new Login
                            {
                                AcessoBloqueado = user.TB_LOGIN_BLOQUEADO ?? false,
                                ID = user.TB_LOGIN_ID,
                               
                                Senha = user.TB_LOGIN_SENHA,
                                LoginAtivo = user.TB_LOGIN_ATIVO,
                                Cpf = login,
                                NumeroTentativasInvalidas = user.TB_LOGIN_TENTATIVAS_INVALIDAS ?? 0,
                                SenhaBloqueada = user.TB_LOGIN_BLOQUEADO ?? false,
                                TrocarSenha = user.TB_LOGIN_TROCAR_SENHA ?? false,
                                Usuario = new Sam.Entity.Usuario
                                {
                                  
                                    Cpf = user2.TB_USUARIO_CPF,
                                    Email = user2.TB_USUARIO_EMAIL,
                                    Fone = user2.TB_USUARIO_END_FONE,
                                    Id = user2.TB_USUARIO_ID,
                                    NomeUsuario = user2.TB_USUARIO_NOME_USUARIO
                                }

                            }).ToList<Login>();

                if (resposta.Count == 0)
                    throw new Exception(Msg.ErrUserPass.ToString());

                var _loginUsuario = (from _login in resposta
                                    where _login.LoginAtivo == true
                                    select _login).FirstOrDefault();

                if (_loginUsuario == null)
                    throw new Exception(Msg.ErrActiveLogin.ToString());

                if (_loginUsuario.AcessoBloqueado)
                    throw new Exception(Msg.ErrBlockedAccess.ToString());

            }
            catch (Exception ex)
            {
                if (ex.TargetSite.IsSpecialName == false)
                    throw new Exception(string.Format(ex.Message));
                else
                    throw new Exception(string.Format(Common.Msg.ErrAuthenticate, ex.Message));
            }

            return (from _login in resposta
                   where _login.LoginAtivo == true
                   select _login).ToList();
        }



      

        public static string RecuperarDicaSenha(string p)
        {
            throw new NotImplementedException();
        }
        
        public static Usuario RecuperarSenha(string p)
        {
            return new Usuario();
        }

        public Login Listar(string login)
        {
            return (from log in base.Db.TB_LOGIN
                    where log.TB_USUARIO.TB_USUARIO_CPF.Equals(login)
                    select new Login
                    {
                          AcessoBloqueado = log.TB_LOGIN_BLOQUEADO ?? false
                        , ID = log.TB_LOGIN_ID
                        , LoginAtivo = log.TB_LOGIN_ATIVO
                        , Senha = log.TB_LOGIN_SENHA
                      
                        , NumeroTentativasInvalidas = log.TB_LOGIN_TENTATIVAS_INVALIDAS ?? 0
                        , SenhaBloqueada = log.TB_LOGIN_BLOQUEADO ?? false
                    }).FirstOrDefault();
        }

        public void Salvar()
        {
            TB_LOGIN loginTable = new TB_LOGIN();

            if (this.Entity.ID != 0)
                loginTable = (from a in this.Db.TB_LOGIN where a.TB_LOGIN_ID.Equals(this.Entity.ID) select a).FirstOrDefault();
            else
                Db.TB_LOGIN.AddObject(loginTable);

            loginTable.TB_LOGIN_ATIVO = this.Entity.LoginAtivo;
            loginTable.TB_LOGIN_BLOQUEADO = this.Entity.AcessoBloqueado;
            loginTable.TB_LOGIN_DATA_CADASTRO = this.Entity.Criacao;
            
            loginTable.TB_LOGIN_SENHA = this.Entity.Senha;
            loginTable.TB_LOGIN_TENTATIVAS_INVALIDAS = 0;
            loginTable.TB_USUARIO_ID = this.Entity.Usuario.Id;
            this.Db.SaveChanges();
        }

        public void SalvarComPerfis()
        {
            TB_LOGIN loginTable = new TB_LOGIN();

            if (this.Entity.ID != 0)
            {
                loginTable = Db.TB_LOGIN.FirstOrDefault(a => a.TB_LOGIN_ID == this.Entity.ID);
                Db.Attach(loginTable);
            }
            else
                Db.TB_LOGIN.AddObject(loginTable);

            loginTable.TB_LOGIN_ATIVO = this.Entity.LoginAtivo;
            loginTable.TB_LOGIN_BLOQUEADO = this.Entity.AcessoBloqueado;
            loginTable.TB_LOGIN_DATA_CADASTRO = this.Entity.Criacao;

            loginTable.TB_LOGIN_SENHA = this.Entity.Senha;
            loginTable.TB_LOGIN_TENTATIVAS_INVALIDAS = this.Entity.NumeroTentativasInvalidas;
            loginTable.TB_USUARIO_ID = this.Entity.Usuario.Id;

            // salva a lista de perfis

            if(this.Perfil.IdPerfil != 0)

            {
                TB_PERFIL perfilTable = Db.TB_PERFIL.FirstOrDefault(a => a.TB_PERFIL_ID == Perfil.IdPerfil);

                if (perfilTable == null)
                    perfilTable = new TB_PERFIL();

                TB_PERFIL_LOGIN perfilLoginTable = (from a in Db.TB_PERFIL_LOGIN
                                                    where a.TB_LOGIN_ID == Entity.ID
                                                    where a.TB_PERFIL_ID == Perfil.IdPerfil
                                                    select a).FirstOrDefault();

                if (perfilLoginTable == null)
                {
                    perfilLoginTable = new TB_PERFIL_LOGIN();
                    Db.AddToTB_PERFIL_LOGIN(perfilLoginTable);
                }

                if (Perfil.AlmoxarifadoPadrao != null)
                    perfilLoginTable.TB_ALMOXARIFADO_ID_PADRAO = Perfil.AlmoxarifadoPadrao.Id;
                else
                    perfilLoginTable.TB_ALMOXARIFADO_ID_PADRAO = null;

                if (Perfil.DivisaoPadrao != null)
                    perfilLoginTable.TB_DIVISAO_ID_PADRAO = Perfil.DivisaoPadrao.Id;
                else
                    perfilLoginTable.TB_DIVISAO_ID_PADRAO = null;

                if (Perfil.GestorPadrao != null)
                    perfilLoginTable.TB_GESTOR_ID_PADRAO = Perfil.GestorPadrao.Id;
                else
                    perfilLoginTable.TB_GESTOR_ID_PADRAO = null;

                if (Perfil.OrgaoPadrao != null)
                    perfilLoginTable.TB_ORGAO_ID_PADRAO = Perfil.OrgaoPadrao.Id;
                else
                    perfilLoginTable.TB_ORGAO_ID_PADRAO = null;

                if (Perfil.UOPadrao != null)
                    perfilLoginTable.TB_UO_ID_PADRAO = Perfil.UOPadrao.Id;
                else
                    perfilLoginTable.TB_UO_ID_PADRAO = null;

                if (Perfil.UGEPadrao != null)
                    perfilLoginTable.TB_UGE_ID_PADRAO = Perfil.UGEPadrao.Id;
                else
                    perfilLoginTable.TB_UGE_ID_PADRAO = null;

                if (Perfil.UAPadrao != null)
                    perfilLoginTable.TB_UA_ID_PADRAO = Perfil.UAPadrao.Id;
                else
                    perfilLoginTable.TB_UA_ID_PADRAO = null;

                perfilLoginTable.TB_LOGINReference.Value = loginTable;
                perfilLoginTable.TB_PERFILReference.Value = perfilTable;

            }
            this.Db.SaveChanges();
        }

        public Login ListarPorUserId(int UserId)
        {
            Login result = (from log in base.Db.TB_LOGIN
                            where log.TB_USUARIO.TB_USUARIO_ID.Equals(UserId)
                            select new Login
                            {
                                AcessoBloqueado = log.TB_LOGIN_BLOQUEADO ?? false,
                                ID = log.TB_LOGIN_ID,
                                Id = log.TB_LOGIN_ID,
                                LoginAtivo = log.TB_LOGIN_ATIVO,
                                Senha = log.TB_LOGIN_SENHA,
 
                                NumeroTentativasInvalidas = log.TB_LOGIN_TENTATIVAS_INVALIDAS ?? 0,
                                SenhaBloqueada = log.TB_LOGIN_BLOQUEADO ?? false,
                                Criacao = log.TB_LOGIN_DATA_CADASTRO ?? new DateTime(1900,1,1),
                                Usuario = new Usuario {
                                    Id = log.TB_USUARIO.TB_USUARIO_ID,
                                    Cpf = log.TB_USUARIO.TB_USUARIO_CPF,
                                    UsuarioAtivo = log.TB_USUARIO.TB_USUARIO_IND_ATIVO 
                                }
                            }).FirstOrDefault();

            result.Perfis = (from perfilLogin in Db.TB_PERFIL_LOGIN
                             where perfilLogin.TB_LOGIN_ID.Equals(result.ID)
                             select new Sam.Entity.Perfil
                             {
                                 IdPerfil = perfilLogin.TB_PERFIL_ID,
                                 DivisaoPadrao = new DivisaoEntity { Id = perfilLogin.TB_DIVISAO_ID_PADRAO ?? 0},
                                 GestorPadrao = new GestorEntity { Id = perfilLogin.TB_GESTOR_ID_PADRAO ?? 0 },
                                 OrgaoPadrao = new OrgaoEntity { Id = perfilLogin.TB_ORGAO_ID_PADRAO ?? 0 },
                                 UOPadrao = new UOEntity { Id = perfilLogin.TB_UO_ID_PADRAO ?? 0 },
                                 UGEPadrao = new UGEEntity { Id = perfilLogin.TB_UGE_ID_PADRAO ?? 0 },
                                 UAPadrao = new UAEntity { Id = perfilLogin.TB_UA_ID_PADRAO ?? 0 },
                                 PerfilLoginId = perfilLogin.TB_PERFIL_LOGIN_ID
                             }).ToList();

            foreach (Sam.Entity.Perfil perfil in result.Perfis) 
            {
                perfil.PerfilLoginNivelAcesso = (from logNivAcesso in Db.TB_LOGIN_NIVEL_ACESSO
                                                 where logNivAcesso.TB_PERFIL_LOGIN_ID.Equals(perfil.PerfilLoginId)
                                                 select new PerfilLoginNivelAcesso
                                                 {
                                                     Id = logNivAcesso.TB_LOGIN_NIVEL_ACESSO_ID,
                                                     IdLoginNivelAcesso = logNivAcesso.TB_LOGIN_NIVEL_ACESSO_ID,
                                                     NivelAcesso = new NivelAcesso
                                                     {
                                                         Id = logNivAcesso.TB_NIVEL_ID,
                                                         NivelId = logNivAcesso.TB_NIVEL_ID,
                                                         IdNivelAcesso = logNivAcesso.TB_LOGIN_NIVEL_ACESSO_ID,
                                                         DescricaoNivel = logNivAcesso.TB_NIVEL.TB_NIVEL_DESCRICAO
                                                     },
                                                     PerfilLoginId = logNivAcesso.TB_PERFIL_LOGIN_ID,
                                                     Valor = logNivAcesso.TB_LOGIN_NIVEL_ACESSO_VALOR
                                                 }).ToList();
            }

            return result;
        }


    }
}
