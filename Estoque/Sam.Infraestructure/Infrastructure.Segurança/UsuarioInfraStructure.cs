using Sam.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Infrastructure
{
    public class UsuarioInfrastructure : BaseInfrastructure, ICrudBase<Sam.Entity.Usuario>
    {
        private Sam.Entity.Usuario usuario = new Sam.Entity.Usuario();

        public Sam.Entity.Usuario Entity
        {
            get { return usuario; }
            set { usuario = value; }
        }


        private Sam.Entity.Login login = new Sam.Entity.Login();

        public Sam.Entity.Login Login
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

        public IList<Sam.Entity.Usuario> Listar(string login)
        {
            var _login = login.PadLeft(11, '0');

            IList<Sam.Entity.Usuario> resposta = (from user in this.Db.TB_USUARIO
                                                  where (login == null && user.TB_USUARIO_CPF == null)
                                                     || (user.TB_USUARIO_CPF == _login)
                                                  where user.TB_USUARIO_IND_ATIVO == true
                                                  orderby user.TB_USUARIO_NOME_USUARIO
                                                  select new Sam.Entity.Usuario
                                                  {
                                                      Id = user.TB_USUARIO_ID,
                                                      Cpf = user.TB_USUARIO_CPF,
                                                      NomeUsuario = user.TB_USUARIO_NOME_USUARIO,

                                                      Fone = user.TB_USUARIO_END_FONE
                                                   ,
                                                      UsuarioAtivo = user.TB_USUARIO_IND_ATIVO
                                                   ,
                                                      Email = user.TB_USUARIO_EMAIL
                                                   ,
                                                      OrgaoPadrao = user.TB_ORGAO_ID ?? 0
                                                   ,
                                                      GestorPadrao = user.TB_GESTOR_ID ?? 0
                                                    ,
                                                      OrgaoId = user.TB_ORGAO_ID ?? 0
                                                  }).ToList();


            this.totalregistros = resposta.Count();

            return resposta;
        }


        public int? GetIdUsuarioOrgao(int? usuarioId)
        {
            //criar consulta para retornar o id do orgão
            var resposta = (from user in this.Db.TB_USUARIO
                            where user.TB_USUARIO_ID == usuarioId
                            select user.TB_ORGAO_ID).FirstOrDefault();

            return resposta;
        }




        public IList<Sam.Entity.Usuario> Listar()
        {
            IList<Sam.Entity.Usuario> resposta;
            IList<Sam.Entity.Login> Login;
            try
            {
                resposta = (from user in this.Db.TB_USUARIO

                            orderby user.TB_USUARIO_NOME_USUARIO
                            select new Sam.Entity.Usuario
                            {
                                Id = user.TB_USUARIO_ID,

                                Cpf = user.TB_USUARIO_CPF,

                                Email = user.TB_USUARIO_EMAIL,

                                Fone = user.TB_USUARIO_END_FONE,
                                GestorId = user.TB_GESTOR_ID ?? 0,
                                GestorPadrao = user.TB_GESTOR_ID ?? 0,
                                GestorPdId = user.TB_GESTOR_ID ?? 0,

                                NomeUsuario = user.TB_USUARIO_NOME_USUARIO,

                                UsuarioAtivo = user.TB_USUARIO_IND_ATIVO,
                                OrgaoId = user.TB_ORGAO_ID ?? 0,
                                OrgaoPadrao = user.TB_ORGAO_ID ?? 0,
                                OrgaoPdId = user.TB_ORGAO_ID ?? 0,

                                Login = (from l in Db.TB_LOGIN
                                         where (l.TB_USUARIO_ID == user.TB_USUARIO_ID)
                                         select new Sam.Entity.Login
                                         {
                                             ID = l.TB_LOGIN_ID,
                                             //AcessoBloqueado = l.TB_LOGIN_BLOQUEADO.Value,
                                             LoginAtivo = l.TB_LOGIN_ATIVO,
                                             LoginBase = user.TB_USUARIO_CPF,
                                             NumeroTentativasInvalidas = l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value == null ? 0 : l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value,

                                             Senha = l.TB_LOGIN_SENHA,
                                             // SenhaBloqueada = l.TB_LOGIN_BLOQUEADO.Value,
                                         }).FirstOrDefault()

                            }).ToList();

                this.totalregistros = resposta.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resposta;

        }



        #region [ ListarPerfilPadrao ]
        /// <summary>
        /// Listar todos os usuários que tenham perfil Associado
        /// </summary>
        /// <param name="OrgaoId"></param>
        /// <param name="GestorId"></param>
        /// <param name="UgeId"></param>
        /// <param name="login"></param>
        /// <param name="Peso"></param>
        /// <param name="pesquisa"></param>
        /// <returns></returns>
        /// 

     


        public IList<Sam.Entity.Usuario> ListarPerfilPadrao(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, string login, int? Peso, string pesquisa, int perfilId = 0, bool complemento = false)
        {
            IList<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();
            SqlConnection conexao = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSEG"].ConnectionString);
            SqlCommand cmd = new SqlCommand("PRC_LISTAR_USUARIOS_LOGIN_ACESSOS ", conexao);

            IList<Sam.Entity.Perfil> _result = new List<Sam.Entity.Perfil>();

            pesquisa = pesquisa.Trim().ToLower();

            try
            {
                conexao.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cpfLogin", login);
                cmd.Parameters.AddWithValue("@orgaoId", OrgaoId);
                cmd.Parameters.AddWithValue("@gestorId", GestorId);
                cmd.Parameters.AddWithValue("@ugeId", UgeId);
                cmd.Parameters.AddWithValue("@almoxarifadoId", AlmoxarifadoId);
                cmd.Parameters.AddWithValue("@peso", Peso);
                cmd.Parameters.AddWithValue("@pesquisa", pesquisa.Trim());
                cmd.Parameters.AddWithValue("@perfilId", perfilId);

                IDataReader _resultado = cmd.ExecuteReader();

                while (_resultado.Read())
                {
                    var _usuario = new Sam.Entity.Usuario
                    {
                        Id = obterValorRegistro<int>(_resultado, "TB_USUARIO_ID").Value,
                        Cpf = obterTextoRegistro(_resultado, "TB_USUARIO_CPF") == "" ? "" : obterTextoRegistro(_resultado, "TB_USUARIO_CPF"),
                        NomeUsuario = obterTextoRegistro(_resultado, "TB_USUARIO_NOME_USUARIO"),
                        Complemento = obterTextoRegistro(_resultado, "DESCRICAO_PERFIL"),

                        Fone = obterTextoRegistro(_resultado, "TB_USUARIO_END_FONE") == "" ? "" : this.formatarTelefone(obterTextoRegistro(_resultado, "TB_USUARIO_END_FONE")),
                        OrgaoId = obterValorRegistro<int>(_resultado, "TB_ORGAO_ID").Value,
                        UsuarioAtivo = obterValorRegistro<bool>(_resultado, "TB_USUARIO_IND_ATIVO").Value,
                        Email = obterTextoRegistro(_resultado, "TB_USUARIO_EMAIL"),
                        Login = new Sam.Entity.Login
                        {
                            ID = obterValorRegistro<int>(_resultado, "TB_LOGIN_ID").Value,
                            AcessoBloqueado = obterValorRegistro<bool>(_resultado, "TB_LOGIN_BLOQUEADO").Value,
                            LoginAtivo = obterValorRegistro<bool>(_resultado, "TB_LOGIN_ATIVO").Value,
                            LoginBase = obterTextoRegistro(_resultado, "TB_USUARIO_CPF"),
                            NumeroTentativasInvalidas = obterValorRegistro<int>(_resultado, "TB_LOGIN_TENTATIVAS_INVALIDAS").Value,

                            Senha = obterTextoRegistro(_resultado, "TB_LOGIN_SENHA"),
                            SenhaBloqueada = obterValorRegistro<bool>(_resultado, "TB_LOGIN_BLOQUEADO").Value,
                            QtdAcessos = obterValorRegistro<int>(_resultado, "QTD_TOTAL_ACESSOS"),
                            QtdAcessos30Dias = obterValorRegistro<int>(_resultado, "QTD_ACESSOS_30DIAS"),
                            QtdAcessos90Dias = obterValorRegistro<int>(_resultado, "QTD_ACESSOS_90DIAS"),
                            QtdAcessos180Dias = obterValorRegistro<int>(_resultado, "QTD_ACESSOS_180DIAS"),
                            DataUltimoAcesso = obterValorRegistro<DateTime>(_resultado, "ULTIMO_ACESSO"),
                            Perfil = new Sam.Entity.Perfil
                            {
                                Id = obterValorRegistro<int>(_resultado, "TB_PERFIL_ID") == null ? 0 : obterValorRegistro<int>(_resultado, "TB_PERFIL_ID").Value,
                                IdPerfil = obterValorRegistro<short>(_resultado, "TB_PERFIL_ID") == null ? (short)0 : obterValorRegistro<short>(_resultado, "TB_PERFIL_ID").Value,
                                Descricao = obterTextoRegistro(_resultado, "TB_PERFIL_DESCRICAO_PADRAO"),
                                DescricaoComDescricaoEstrutura = obterTextoRegistro(_resultado, "DESCRICAO_PERFIL"),
                                DescricaoComCodigoEstrutura = obterTextoRegistro(_resultado, "CODIGOS_PERFIL"),
                                IdLogin = obterValorRegistro<int>(_resultado, "TB_LOGIN_ID").Value,
                                PerfilLoginId = obterValorRegistro<int>(_resultado, "TB_PERFIL_LOGIN_ID").Value
                            }
                        }
                    };

                    _resposta.Add(_usuario);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conexao.State == ConnectionState.Open)
                    conexao.Close();
            }

            this.totalregistros = _resposta.Count;
            return _resposta;
        }
        public void BuscaComplementosDosUsuarios(IList<Sam.Entity.Usuario> todosOsUsuarios, int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, string login, int? Peso, string pesquisa, int perfilId = 0, int pagina = 0, bool gerarExcel = true)
        {
            PerfilInfrastructureAntigo perfil = new PerfilInfrastructureAntigo();
          
            IList<Sam.Entity.Perfil> _result;
      
            

            try
                {
                    var osQueVaoSerMostradoNaTabela = todosOsUsuarios.Skip(pagina * 50).Take(50).ToList();

                    _result = (from _Perfil in perfil.BuscaComplementoDosUsuarioASeremMostradosNaTabela(osQueVaoSerMostradoNaTabela, OrgaoId, GestorId, UgeId, AlmoxarifadoId, perfilId, login, Peso, pesquisa)
                               select _Perfil).Distinct().ToList();

                    for (int i = 0; i < todosOsUsuarios.Count; i++)
                    {
                        for (int j = 0; j < _result.Count; j++)
                        {
                            //para casos os usuários sejam sem perfil
                            if (todosOsUsuarios[i].Login.Perfil == null)
                            {
                                break;
                            }

                            if (todosOsUsuarios[i].Login.Perfil.PerfilLoginId == _result[j].PerfilLoginId)
                            {
                                todosOsUsuarios[i].Login.Perfil.DescricaoComCodigoEstrutura = _result[j].Descricao;
                                //break;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    throw e;
                }
            }


        public void BuscaComplementosDosUsuariosEXCEL(IList<Sam.Entity.Usuario> todosOsUsuarios, int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, string login, int? Peso, string pesquisa, int perfilId = 0, int pagina = 0, bool gerarExcel = true)
        {
            PerfilInfrastructureAntigo perfil = new PerfilInfrastructureAntigo();

            IList<Sam.Entity.Perfil> _result;

            try
            {
                var osQueVaoSerMostradoNaTabela = todosOsUsuarios.ToList();

                _result = (from _Perfil in perfil.BuscaComplementoDosUsuarioASeremMostradosNaTabela(osQueVaoSerMostradoNaTabela, OrgaoId, GestorId, UgeId, AlmoxarifadoId, perfilId, login, Peso, pesquisa)
                           select _Perfil).Distinct().ToList();

                for (int i = 0; i < todosOsUsuarios.Count; i++)
                {
                    for (int j = 0; j < _result.Count; j++)
                    {
                        //para casos os usuários sejam sem perfil
                        if (todosOsUsuarios[i].Login.Perfil == null)
                        {
                            break;
                        }

                        if (todosOsUsuarios[i].Login.Perfil.PerfilLoginId == _result[j].PerfilLoginId)
                        {
                            todosOsUsuarios[i].Login.Perfil.DescricaoComCodigoEstrutura = _result[j].Descricao;
                            //break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }



        #endregion

        /// <summary>
        /// Listar todos os perfis associados aos usuários
        /// </summary>
        /// <param name="OrgaoId"></param>
        /// <param name="GestorId"></param>
        /// <returns></returns>
        public IList<Sam.Entity.Usuario> ListarTodosPerfis(int? OrgaoId, int? GestorId)
        {
            List<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();

            try
            {
                Sam.Entity.Usuario _novo = null;

                foreach (var _item in from _user in this.Db.VW_DADOS_USUARIO_PERFIS where _user.TB_ORGAO_ID == OrgaoId && _user.TB_GESTOR_ID == GestorId select _user)
                {
                    int _index = _resposta.FindIndex(n => n.Id == _item.TB_USUARIO_ID);

                    if (_index < 0)
                    {
                        _novo = new Sam.Entity.Usuario
                        {
                            Id = _item.TB_USUARIO_ID,

                            Cpf = _item.TB_USUARIO_CPF,

                            Fone = _item.TB_USUARIO_END_FONE,
                            GestorId = _item.TB_GESTOR_ID.HasValue ? _item.TB_GESTOR_ID.Value : 0,
                            GestorPadrao = _item.TB_GESTOR_ID,
                            GestorPdId = _item.TB_GESTOR_ID.HasValue ? _item.TB_GESTOR_ID.Value : 0,

                            NomeUsuario = _item.TB_USUARIO_NOME_USUARIO,

                            UsuarioAtivo = _item.TB_USUARIO_IND_ATIVO,
                            OrgaoId = _item.TB_ORGAO_ID.HasValue ? _item.TB_ORGAO_ID.Value : 0,
                            OrgaoPadrao = _item.TB_ORGAO_ID,
                            OrgaoPdId = _item.TB_ORGAO_ID.HasValue ? _item.TB_ORGAO_ID.Value : 0,
                            Login = (from l in Db.TB_LOGIN
                                     where (l.TB_USUARIO_ID == _item.TB_USUARIO_ID)
                                     select new Sam.Entity.Login
                                     {
                                         ID = l.TB_LOGIN_ID,
                                         AcessoBloqueado = l.TB_LOGIN_BLOQUEADO.Value,
                                         LoginAtivo = l.TB_LOGIN_ATIVO,
                                         LoginBase = _item.TB_USUARIO_CPF,
                                         NumeroTentativasInvalidas = l.TB_LOGIN_TENTATIVAS_INVALIDAS.HasValue ? l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value : 0,

                                         Senha = l.TB_LOGIN_SENHA,
                                         SenhaBloqueada = l.TB_LOGIN_BLOQUEADO.Value,
                                         Perfil = new Sam.Entity.Perfil
                                         {
                                             Id = _item.TB_PERFIL_ID,
                                             IdPerfil = _item.TB_PERFIL_ID,
                                             Descricao = _item.TB_PERFIL_DESCRICAO,
                                             DescricaoComDescricaoEstrutura = _item.DESCRICAO_PERFIL,
                                             DescricaoComCodigoEstrutura = _item.CODIGOS_PERFIL,
                                             IdLogin = _item.TB_LOGIN_ID
                                         },
                                         Perfis = new List<Sam.Entity.Perfil> {
                                            new Sam.Entity.Perfil  {
                                                    Id = _item.TB_PERFIL_ID,
                                                    IdPerfil = _item.TB_PERFIL_ID,
                                                    Descricao = _item.TB_PERFIL_DESCRICAO,
                                                    DescricaoComDescricaoEstrutura = _item.DESCRICAO_PERFIL,
                                                    DescricaoComCodigoEstrutura = _item.CODIGOS_PERFIL,
                                                    IdLogin = _item.TB_LOGIN_ID
                                                }
                                        }
                                     }).FirstOrDefault()
                        };

                        _resposta.Add(_novo);
                    }
                    else
                    {
                        _resposta[_index].Login.Perfis.Add(new Sam.Entity.Perfil
                        {
                            Id = _item.TB_PERFIL_ID,
                            IdPerfil = _item.TB_PERFIL_ID,
                            Descricao = _item.TB_PERFIL_DESCRICAO,
                            DescricaoComDescricaoEstrutura = _item.DESCRICAO_PERFIL,
                            DescricaoComCodigoEstrutura = _item.CODIGOS_PERFIL,
                            IdLogin = _item.TB_LOGIN_ID
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.totalregistros = _resposta.Count;
            return _resposta;
        }

        /// <summary>
        /// Listar todos os perfis associados aos usuários
        /// </summary>
        /// <param name="OrgaoId"></param>
        /// <param name="GestorId"></param>
        /// <returns></returns>
        public IList<Sam.Entity.UsuarioRelatorio> ListarTodosPerfisRelatorio(int? OrgaoId, int? GestorId, int? PerfilId, string pesquisa)
        {
            List<Sam.Entity.UsuarioRelatorio> _resposta = new List<Sam.Entity.UsuarioRelatorio>();
            pesquisa = pesquisa.ToLower().Trim();

            try
            {
                _resposta = (from _user in this.Db.VW_DADOS_USUARIO_PERFIS
                             join _l in this.Db.TB_LOGIN on _user.TB_LOGIN_ID equals _l.TB_LOGIN_ID
                             where _user.TB_ORGAO_ID == OrgaoId
                                 && _user.TB_GESTOR_ID == GestorId

                             where (_user.TB_USUARIO_NOME_USUARIO.ToLower().Contains(pesquisa)
                                 || _user.TB_USUARIO_CPF.Contains(pesquisa)
                                 || _user.TB_USUARIO_EMAIL.ToLower().Contains(pesquisa)

                                 || pesquisa == string.Empty)

                             where (PerfilId.Value <= 0
                                    || _user.TB_PERFIL_ID == PerfilId)

                             orderby _user.TB_USUARIO_NOME_USUARIO, _user.RowId
                             select new Sam.Entity.UsuarioRelatorio
                             {
                                 Id = _user.TB_USUARIO_ID,

                                 Cpf = _user.TB_USUARIO_CPF,

                                 Email = _user.TB_USUARIO_EMAIL,

                                 Fone = _user.TB_USUARIO_END_FONE,
                                 GestorId = _user.TB_GESTOR_ID.HasValue ? _user.TB_GESTOR_ID.Value : 0,
                                 GestorPadrao = _user.TB_GESTOR_ID,
                                 GestorPdId = _user.TB_GESTOR_ID.HasValue ? _user.TB_GESTOR_ID.Value : 0,

                                 NomeUsuario = _user.TB_USUARIO_NOME_USUARIO,

                                 UsuarioAtivo = _user.TB_USUARIO_IND_ATIVO,
                                 OrgaoId = _user.TB_ORGAO_ID.HasValue ? _user.TB_ORGAO_ID.Value : 0,
                                 //CodigoOrgao = _user.TB_ORGAO_CODIGO,
                                 //NomeOrgao = _user.TB_ORGAO_DESCRICAO,
                                 OrgaoPadrao = _user.TB_ORGAO_ID,
                                 OrgaoPdId = _user.TB_ORGAO_ID.HasValue ? _user.TB_ORGAO_ID.Value : 0,
                                 IdGestor = _user.TB_GESTOR_ID,
                                 //CodigoGestor = _user.TB_GESTOR_CODIGO_GESTAO,
                                 //NomeGestor = _user.TB_GESTOR_NOME,
                                 //NomeReduzidoGestor = _user.TB_GESTOR_NOME_REDUZIDO,
                                 IdLogin = _l.TB_LOGIN_ID,
                                 IdPerfilLogin = _user.TB_PERFIL_LOGIN_ID,
                                 LoginAtivo = _l.TB_LOGIN_ATIVO,
                                 LoginBase = _user.TB_USUARIO_CPF,
                                 LoginSenhaBloqueada = _l.TB_LOGIN_BLOQUEADO.Value,
                                 IdPerfil = _user.TB_PERFIL_ID,
                                 Descricao = _user.TB_PERFIL_DESCRICAO,
                                 DescricaoComDescricaoEstrutura = _user.DESCRICAO_PERFIL,
                                 DescricaoComCodigoEstrutura = _user.PERFIL_RELATORIO,
                                 IdentificadorRegistro = _user.RowId,
                                 DataUltimoAcesso = _user.ULTIMO_ACESSO,
                                 AcessosUltimos30Dias = _user.QTD_ACESSOS_30DIAS,
                                 AcessosUltimos90Dias = _user.QTD_ACESSOS_90DIAS,
                                 AcessosUltimos180Dias = _user.QTD_ACESSOS_180DIAS,
                                 TotalAcessos = _user.QTD_TOTAL_ACESSOS
                             }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.totalregistros = _resposta.Count;
            return _resposta;
        }

        public IList<Sam.Entity.Usuario> ListarAdminGeral()
        {
            IList<Sam.Entity.Usuario> resposta;
            var _idEnumPerfilAdminGeral = EnumPerfil.ADMINISTRADOR_GERAL.GetHashCode();

            try
            {
                resposta = (from _user in this.Db.TB_USUARIO
                            join _login in this.Db.TB_LOGIN on _user.TB_USUARIO_ID equals _login.TB_USUARIO_ID
                            join _perfilLogin in this.Db.TB_PERFIL_LOGIN on _login.TB_LOGIN_ID equals _perfilLogin.TB_LOGIN_ID
                            join _perfil in this.Db.TB_PERFIL on _perfilLogin.TB_PERFIL_ID equals _perfil.TB_PERFIL_ID
                            where _perfil.TB_PERFIL_ID == _idEnumPerfilAdminGeral
                            where _user.TB_USUARIO_IND_ATIVO == true
                            orderby _user.TB_USUARIO_NOME_USUARIO
                            select new Sam.Entity.Usuario
                            {
                                Id = _user.TB_USUARIO_ID,

                                Cpf = _user.TB_USUARIO_CPF,

                                Email = _user.TB_USUARIO_EMAIL,

                                Fone = _user.TB_USUARIO_END_FONE,
                                GestorId = _user.TB_GESTOR_ID.HasValue ? _user.TB_GESTOR_ID.Value : 0,
                                GestorPadrao = _user.TB_GESTOR_ID,
                                GestorPdId = _user.TB_GESTOR_ID.HasValue ? _user.TB_GESTOR_ID.Value : 0,

                                NomeUsuario = _user.TB_USUARIO_NOME_USUARIO,

                                UsuarioAtivo = _user.TB_USUARIO_IND_ATIVO,
                                OrgaoId = _user.TB_ORGAO_ID.HasValue ? _user.TB_ORGAO_ID.Value : 0,
                                OrgaoPadrao = _user.TB_ORGAO_ID,
                                OrgaoPdId = _user.TB_ORGAO_ID.HasValue ? _user.TB_ORGAO_ID.Value : 0,
                                Login = (from l in Db.TB_LOGIN
                                         where (l.TB_USUARIO_ID == _user.TB_USUARIO_ID)
                                         select new Sam.Entity.Login
                                         {
                                             ID = l.TB_LOGIN_ID,
                                             AcessoBloqueado = l.TB_LOGIN_BLOQUEADO.Value,
                                             LoginAtivo = l.TB_LOGIN_ATIVO,
                                             LoginBase = _user.TB_USUARIO_CPF,
                                             NumeroTentativasInvalidas = l.TB_LOGIN_TENTATIVAS_INVALIDAS.HasValue ? l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value : 0,

                                             Senha = l.TB_LOGIN_SENHA,
                                             SenhaBloqueada = l.TB_LOGIN_BLOQUEADO.Value,
                                         }).FirstOrDefault()

                            }).ToList();

                this.totalregistros = resposta.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resposta;
        }

        public IList<Sam.Entity.Usuario> ListarOrgarGestor(int _OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, string login, int? Peso, string pesquisa, int perfilId, bool gerarExcel = true)
        {
            IList<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();
            IList<Sam.Entity.Usuario> lista = new List<Sam.Entity.Usuario>();
            pesquisa = pesquisa.Trim().ToLower();

            _resposta = (from _usuario in this.ListarPerfilPadrao(_OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, perfilId, gerarExcel) // pesquisar query
                         select _usuario).ToList();

            this.totalregistros = _resposta.Count;
            return _resposta;
        }

        private string formatarTelefone(string telefone)
        {
            UInt64 _retorno;
            return (UInt64.TryParse(telefone.PadLeft(10, '0'), out _retorno)) ? string.Format("{0:(00)0###-####}", _retorno) : telefone;
        }

        private T? obterValorRegistro<T>(IDataReader reader, string nomeCampo) where T : struct
        {
            object _retorno = reader.GetValue(reader.GetOrdinal(nomeCampo));
            return _retorno as T?;
        }

        private string obterTextoRegistro(IDataReader reader, string nomeCampo)
        {
            object _retorno = reader.GetValue(reader.GetOrdinal(nomeCampo));
            return _retorno as string ?? string.Empty;
        }

        public IList<Sam.Entity.Usuario> ListarOrgarGestorPerfil(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            IList<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();
            pesquisa = pesquisa.Trim().ToLower();

            _resposta = (from _usuario in this.ListarPerfilPadrao(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, PerfilId.Value)
                         select _usuario).ToList();//consulta

            this.totalregistros = _resposta.Count;
            return _resposta;
        }
        public IList<Sam.Entity.Usuario> ListarOrgarGestorPerfilExcel(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, bool gerarExcel)
        {
            IList<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();
            pesquisa = pesquisa.Trim().ToLower();

            _resposta = (from _usuario in this.ListarPerfilPadrao(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, PerfilId.Value, gerarExcel)
                         select _usuario).ToList();//consulta

            this.totalregistros = _resposta.Count;
            return _resposta;
        }
        public IList<Sam.Entity.Usuario> ListarOrgarGestorPerfil2(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            IList<Sam.Entity.Usuario> _resposta = new List<Sam.Entity.Usuario>();
            pesquisa = pesquisa.Trim().ToLower();

            _resposta = (from _usuario in this.ListarPerfilPadrao(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, PerfilId.Value)
                         select _usuario).ToList();//consulta

            this.totalregistros = _resposta.Count;
            return _resposta;
        }

        public IList<Sam.Entity.Usuario> ListarOrgarGestorPerfil_SemPerfil(int OrgaoId, int GestorId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            try
            {
                IList<Sam.Entity.Usuario> resposta = ((from user in this.Db.TB_USUARIO
                                                       join log in this.Db.TB_LOGIN on user.TB_USUARIO_ID equals log.TB_USUARIO_ID into i_ul
                                                       from j_ul in i_ul.DefaultIfEmpty()

                                                       join per_log in this.Db.TB_PERFIL_LOGIN on j_ul.TB_LOGIN_ID equals per_log.TB_LOGIN_ID into i_pl
                                                       from j_pl in i_pl.DefaultIfEmpty()

                                                       join acesso in this.Db.TB_LOGIN_NIVEL_ACESSO on j_pl.TB_PERFIL_LOGIN_ID equals acesso.TB_PERFIL_LOGIN_ID into i_a
                                                       from j_a in i_a.DefaultIfEmpty()

                                                       join p in this.Db.TB_PERFIL on j_pl.TB_PERFIL_ID equals p.TB_PERFIL_ID into i_p
                                                       from j_p in i_p.DefaultIfEmpty()

                                                       where (user.TB_ORGAO_ID == OrgaoId || (j_a.TB_NIVEL_ID == 1 && j_a.TB_LOGIN_NIVEL_ACESSO_VALOR == OrgaoId)) &&
                                                           (user.TB_GESTOR_ID == GestorId || (j_a.TB_NIVEL_ID == 6 && j_a.TB_LOGIN_NIVEL_ACESSO_VALOR == GestorId))

                                                       where (j_p == null) //where (j_p.TB_PERFIL_ID == null) // verificar se o OBJETO não existe

                                                       where user.TB_USUARIO_CPF != login


                                                       where (user.TB_USUARIO_NOME_USUARIO.Contains(pesquisa)
                                                     || user.TB_USUARIO_CPF.Contains(pesquisa)
                                                     || user.TB_USUARIO_EMAIL.Contains(pesquisa)

                                                     || pesquisa == string.Empty)

                                                       //where (j_p.TB_PERFIL_PESO < Peso) // este filtro não deveria existir, uma vez que o usuário não tem perfil nenhum!

                                                       select new Sam.Entity.Usuario
                                                       {
                                                           Id = user.TB_USUARIO_ID,

                                                           Cpf = user.TB_USUARIO_CPF,

                                                           Fone = user.TB_USUARIO_END_FONE,
                                                           UgeId = j_pl.TB_UGE_ID_PADRAO,
                                                           NomeUsuario = user.TB_USUARIO_NOME_USUARIO,
                                                           UsuarioAtivo = user.TB_USUARIO_IND_ATIVO,
                                                           Email = user.TB_USUARIO_EMAIL,
                                                           Perfil = j_pl.TB_PERFIL_ID,// == 0 ? j_pl.TB_PERFIL_ID : j_pl.TB_PERFIL_ID,// olhar aqui
                                                           Login = (from l in Db.TB_LOGIN
                                                                    where
                                                                        (l.TB_USUARIO_ID == user.TB_USUARIO_ID)
                                                                    select new Sam.Entity.Login
                                                                    {
                                                                        ID = l.TB_LOGIN_ID,
                                                                        AcessoBloqueado = l.TB_LOGIN_BLOQUEADO.Value,
                                                                        LoginAtivo = l.TB_LOGIN_ATIVO,
                                                                        LoginBase = user.TB_USUARIO_CPF,
                                                                        NumeroTentativasInvalidas = l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value,

                                                                        Senha = l.TB_LOGIN_SENHA,
                                                                        SenhaBloqueada = l.TB_LOGIN_BLOQUEADO.Value,
                                                                    }).FirstOrDefault()
                                                       }).Distinct().ToList()
                                                                     ).OrderBy(a => a.NomeUsuario).ToList();

                this.totalregistros = resposta.Count();
                return resposta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Sam.Entity.Usuario> ListarOrgarGestorPerfil_SemPerfilEXCEL(int OrgaoId, int GestorId, int? PerfilId, string login, int? Peso, string pesquisa, bool gerarexcel)
        {
            try
            {
                IList<Sam.Entity.Usuario> resposta = ((from user in this.Db.TB_USUARIO
                                                       join log in this.Db.TB_LOGIN on user.TB_USUARIO_ID equals log.TB_USUARIO_ID into i_ul
                                                       from j_ul in i_ul.DefaultIfEmpty()

                                                       join per_log in this.Db.TB_PERFIL_LOGIN on j_ul.TB_LOGIN_ID equals per_log.TB_LOGIN_ID into i_pl
                                                       from j_pl in i_pl.DefaultIfEmpty()

                                                       join acesso in this.Db.TB_LOGIN_NIVEL_ACESSO on j_pl.TB_PERFIL_LOGIN_ID equals acesso.TB_PERFIL_LOGIN_ID into i_a
                                                       from j_a in i_a.DefaultIfEmpty()

                                                       join p in this.Db.TB_PERFIL on j_pl.TB_PERFIL_ID equals p.TB_PERFIL_ID into i_p
                                                       from j_p in i_p.DefaultIfEmpty()

                                                       where (user.TB_ORGAO_ID == OrgaoId || (j_a.TB_NIVEL_ID == 1 && j_a.TB_LOGIN_NIVEL_ACESSO_VALOR == OrgaoId)) &&
                                                           (user.TB_GESTOR_ID == GestorId || (j_a.TB_NIVEL_ID == 6 && j_a.TB_LOGIN_NIVEL_ACESSO_VALOR == GestorId))

                                                       where (j_p == null) //where (j_p.TB_PERFIL_ID == null) // verificar se o OBJETO não existe

                                                       where user.TB_USUARIO_CPF != login


                                                       where (user.TB_USUARIO_NOME_USUARIO.Contains(pesquisa)
                                                     || user.TB_USUARIO_CPF.Contains(pesquisa)
                                                     || user.TB_USUARIO_EMAIL.Contains(pesquisa)

                                                     || pesquisa == string.Empty)

                                                       //where (j_p.TB_PERFIL_PESO < Peso) // este filtro não deveria existir, uma vez que o usuário não tem perfil nenhum!

                                                       select new Sam.Entity.Usuario
                                                       {
                                                           Id = user.TB_USUARIO_ID,

                                                           Cpf = user.TB_USUARIO_CPF,

                                                           Fone = user.TB_USUARIO_END_FONE,
                                                           UgeId = j_pl.TB_UGE_ID_PADRAO,
                                                           NomeUsuario = user.TB_USUARIO_NOME_USUARIO,
                                                           UsuarioAtivo = user.TB_USUARIO_IND_ATIVO,
                                                           Email = user.TB_USUARIO_EMAIL,
                                                           Perfil = j_pl.TB_PERFIL_ID,// == 0 ? j_pl.TB_PERFIL_ID : j_pl.TB_PERFIL_ID,// olhar aqui
                                                           Login = (from l in Db.TB_LOGIN
                                                                    where
                                                                        (l.TB_USUARIO_ID == user.TB_USUARIO_ID)
                                                                    select new Sam.Entity.Login
                                                                    {
                                                                        ID = l.TB_LOGIN_ID,
                                                                        AcessoBloqueado = l.TB_LOGIN_BLOQUEADO.Value,
                                                                        LoginAtivo = l.TB_LOGIN_ATIVO,
                                                                        LoginBase = user.TB_USUARIO_CPF,
                                                                        NumeroTentativasInvalidas = l.TB_LOGIN_TENTATIVAS_INVALIDAS.Value,

                                                                        Senha = l.TB_LOGIN_SENHA,
                                                                        SenhaBloqueada = l.TB_LOGIN_BLOQUEADO.Value,
                                                                    }).FirstOrDefault()
                                                       }).Distinct().ToList()
                                                                     ).OrderBy(a => a.NomeUsuario).ToList();

                this.totalregistros = resposta.Count();
                return resposta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Sam.Entity.Usuario> ListAllCode()
        {
            throw new NotImplementedException();
        }

        public Sam.Entity.Usuario LerRegistro()
        {
            throw new NotImplementedException();
        }

        public Sam.Entity.Usuario LerRegistro(int Id)
        {
            return Listar().Where(a => a.Id == Id).FirstOrDefault();
        }

        public IList<Sam.Entity.Usuario> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            TB_USUARIO userTable = this.Db.TB_USUARIO.Where(a => a.TB_USUARIO_CPF == this.Entity.Cpf).FirstOrDefault();
            this.Db.TB_USUARIO.DeleteObject(userTable);
            this.Db.SaveChanges();
        }


        public void Salvar()
        {
            try
            {
                byte NivelId = 1;
                TB_USUARIO userTable = new TB_USUARIO();
                TB_LOGIN loginTable = new TB_LOGIN();
                TB_PERFIL_LOGIN Perfil = new TB_PERFIL_LOGIN();
                TB_PERFIL_NIVEL NivelAcesso = null;
                //  Int16 PerfilId = Int16.Parse(this.Entity.Login.Perfil.Id.ToString());

                if (this.Entity.Id > 0)
                    userTable = this.Db.TB_USUARIO.Where(a => a.TB_USUARIO_ID == this.Entity.Id).FirstOrDefault();
                else
                    Db.TB_USUARIO.AddObject(userTable);

                if (this.Entity.Login != null && this.Entity.Login.ID != 0)
                    loginTable = Db.TB_LOGIN.FirstOrDefault(a => a.TB_USUARIO_ID == Entity.Id && a.TB_LOGIN_ID == Entity.Login.ID);
                else
                    Db.AddToTB_LOGIN(loginTable);

                userTable.TB_USUARIO_CPF = this.Entity.Cpf;
                userTable.TB_USUARIO_END_FONE = this.Entity.Fone != null ? this.Entity.Fone.ToString() : this.Entity.Fone;
                userTable.TB_USUARIO_IND_ATIVO = this.Entity.UsuarioAtivo;
                userTable.TB_USUARIO_NOME_USUARIO = this.Entity.NomeUsuario;
                userTable.TB_USUARIO_EMAIL = this.Entity.Email;
                userTable.TB_ORGAO_ID = this.Entity.OrgaoPdId;
                userTable.TB_GESTOR_ID = this.Entity.GestorPdId;
                //userTable.TB_USUARIO_IND_ATIVO = true;

                if (!string.IsNullOrEmpty(this.Entity.Login.Senha))
                {
                    loginTable.TB_LOGIN_TROCAR_SENHA = (loginTable.TB_LOGIN_SENHA != this.Entity.Login.Senha);
                    loginTable.TB_LOGIN_SENHA = this.Entity.Login.Senha;
                    loginTable.TB_LOGIN_ATIVO = true;
                    loginTable.TB_LOGIN_DATA_CADASTRO = DateTime.Now;
                    loginTable.TB_LOGIN_TENTATIVAS_INVALIDAS = 0;
                    loginTable.TB_LOGIN_BLOQUEADO = false;
                }

                // if(Perfil ==null || Perfil.TB_PERFIL_LOGIN_ID == 0)
                //{
                //    Perfil.TB_PERFIL_ID = Int16.Parse(this.Entity.Login.Perfil.Id.ToString());
                //    Perfil.TB_ORGAO_ID_PADRAO = this.Entity.OrgaoPdId;
                //    Perfil.TB_GESTOR_ID_PADRAO = this.Entity.GestorPdId;
                //    Perfil.TB_PERFIL_LOGIN_ATIVO = true;
                //}
                //NivelAcesso = Db.TB_PERFIL_NIVEL.FirstOrDefault(a => a.TB_NIVEL_ID == NivelId && a.TB_PERFIL_ID == PerfilId);
                //if(NivelAcesso == null)
                //{
                //    NivelAcesso = new TB_PERFIL_NIVEL();
                //    NivelAcesso.TB_PERFIL_ID = Int16.Parse(this.Entity.Login.Perfil.Id.ToString());
                //    NivelAcesso.TB_NIVEL_ID = NivelId;
                //}
                // loginTable.TB_LOGIN_TENTATIVAS_INVALIDAS = this.Entity.Login.NumeroTentativasInvalidas;
                //loginTable.TB_LOGIN_DATA_CADASTRO = this.Entity.Login.Criacao;
                //loginTable.TB_LOGIN_ATIVO = this.Entity.Login.LoginAtivo;

                //loginTable.TB_LOGIN_BLOQUEADO = this.Entity.Login.AcessoBloqueado;
                //loginTable.TB_LOGIN_TROCAR_SENHA = true;


                loginTable.TB_USUARIOReference.Value = userTable;



                this.Db.SaveChanges();
                this.Entity.Login.Id = loginTable.TB_LOGIN_ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlterarSenha(string NovaSenha)
        {
            TB_USUARIO userTable = new TB_USUARIO();
            TB_LOGIN loginTable = new TB_LOGIN();

            if (this.Entity.Id > 0)
                userTable = this.Db.TB_USUARIO.Where(a => a.TB_USUARIO_ID == this.Entity.Id).FirstOrDefault();
            else
                Db.TB_USUARIO.AddObject(userTable);

            if (this.Entity.Login.ID != 0)
                loginTable = Db.TB_LOGIN.FirstOrDefault(a => a.TB_USUARIO_ID == Entity.Id && a.TB_LOGIN_ID == Entity.Login.ID);
            else
                Db.AddToTB_LOGIN(loginTable);


            userTable.TB_USUARIO_CPF = this.Entity.Cpf;

            userTable.TB_USUARIO_IND_ATIVO = this.Entity.UsuarioAtivo;
            userTable.TB_USUARIO_NOME_USUARIO = this.Entity.NomeUsuario;

            userTable.TB_USUARIO_EMAIL = this.Entity.Email;

            // login
            loginTable.TB_LOGIN_ID = this.Entity.Login.ID;
            loginTable.TB_USUARIO_ID = this.Entity.Id;

            loginTable.TB_LOGIN_SENHA = NovaSenha;
            loginTable.TB_LOGIN_TENTATIVAS_INVALIDAS = this.Entity.Login.NumeroTentativasInvalidas;
            loginTable.TB_LOGIN_ATIVO = this.Entity.Login.LoginAtivo;

            loginTable.TB_LOGIN_BLOQUEADO = this.Entity.Login.AcessoBloqueado;
            loginTable.TB_LOGIN_TROCAR_SENHA = false;

            this.Db.TB_LOGIN.ApplyCurrentValues(loginTable);
            this.Db.SaveChanges();

        }

        public bool PodeExcluir()
        {
            return true;
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        //Traz Nome/CPF passando o TB_Login_ID
        public Sam.Entity.Usuario SelecionaUsuarioPor_LoginID(int LoginId)
        {
            Sam.Entity.Usuario resposta = (from l in Db.TB_LOGIN
                                           where (l.TB_LOGIN_ID == LoginId)
                                           select new Sam.Entity.Usuario
                                           {
                                               Id = l.TB_USUARIO.TB_USUARIO_ID,
                                               NomeUsuario = l.TB_USUARIO.TB_USUARIO_NOME_USUARIO,
                                               Cpf = l.TB_USUARIO.TB_USUARIO_CPF
                                           }).FirstOrDefault();

            return resposta;
        }

    }
}
