using Sam.Common.Util;
using Sam.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using static Sam.Common.Enums.PerfilNivelAcessoEnum;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Infrastructure.Infrastructure.Segurança
{
    public class UsuarioLogadoInfrastructue : SegurancaAbstractContext
    {

        private UsuarioLogadoInfrastructue()
        {

        }

        public static UsuarioLogadoInfrastructue CreateInstance()
        {
            return new UsuarioLogadoInfrastructue();
        }

        public override void CreateUsuarioLogado(TB_USUARIO_LOGADO usuario)
        {
            try
            {

                this.Db.TB_USUARIO_LOGADO.AddObject(usuario);
                this.Db.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }

        public override void RemoveUsuarioLogadoId(Int32 loginId)
        {
            var usuario = GetByLoginId(loginId);

            if (usuario != null)
            {
                this.Db.TB_USUARIO_LOGADO.DeleteObject(usuario);
                this.Db.SaveChanges();
            }
        }


        public override void RemoveUsuarioLogadoSessionId(string sessionId)
        {
            var usuarios = UsuarioLogados(sessionId);

            foreach (var item in usuarios)
            {
                this.Db.TB_USUARIO_LOGADO.DeleteObject(item);
                this.Db.SaveChanges();
            }
        }

        public override void RemoveAllUsuarioLogado()
        {
            var usuarios = UsuarioLogado(10000, 0, "");

            foreach (TB_USUARIO_LOGADO usuario in usuarios)
            {
                this.Db.TB_USUARIO_LOGADO.DeleteObject(usuario);
                this.Db.SaveChanges();
            }
        }

        private List<TB_USUARIO_LOGADO> UsuarioLogados(string sessionId)
        {
            try
            {
                IQueryable<TB_USUARIO_LOGADO> query = (from lo in this.Db.TB_USUARIO_LOGADO where lo.TB_SESSION_ID == sessionId select lo);

                var retorno = query.ToList();

                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<TB_USUARIO_LOGADO> UsuarioLogado(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf = null, int gestorId = default(int))
        {
            IQueryable<TB_USUARIO_LOGADO> query = (from lo in this.Db.TB_USUARIO_LOGADO select lo);

            this.totalregistros = query.Count();


            if (!string.IsNullOrWhiteSpace(cpf))
            {
                query = (from lo in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                         join li in this.Db.TB_LOGIN.Include("TB_USUARIO") on lo.TB_LOGIN_ID equals li.TB_LOGIN_ID
                         join pf in this.Db.TB_PERFIL_LOGIN on li.TB_LOGIN_ID equals pf.TB_LOGIN_ID
                         join us in this.Db.TB_USUARIO on li.TB_USUARIO_ID equals us.TB_USUARIO_ID
                         where us.TB_USUARIO_CPF.Contains(cpf)
                                && ((pf.TB_GESTOR_ID_PADRAO == gestorId && gestorId > 0)
                                      || (pf.TB_GESTOR_ID_PADRAO > 0 && gestorId == 0))
                         select lo);
            }
            else
            {
                query = (from lo in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                         join li in this.Db.TB_LOGIN.Include("TB_USUARIO") on lo.TB_LOGIN_ID equals li.TB_LOGIN_ID
                         join pf in this.Db.TB_PERFIL_LOGIN on li.TB_LOGIN_ID equals pf.TB_LOGIN_ID
                         join us in this.Db.TB_USUARIO on li.TB_USUARIO_ID equals us.TB_USUARIO_ID
                         where ((pf.TB_GESTOR_ID_PADRAO == gestorId && gestorId > 0)
                                 || (pf.TB_GESTOR_ID_PADRAO > 0 && gestorId == 0))
                         select lo);
            }

            var retorno = query.OrderBy(o => o.TB_DATA_HORA_LOGADO).Skip(StartRowIndexParameterName).Take(maximumRowsParameterName).ToList();

            return retorno;
        }

        public override TB_USUARIO_LOGADO Get(int loginId)
        {
            IQueryable<TB_USUARIO_LOGADO> query = (from q in this.Db.TB_USUARIO_LOGADO where q.TB_LOGIN_ID == loginId select q);

            return query.FirstOrDefault();
        }

        public override TB_USUARIO_LOGADO GetByLoginId(int loginId)
        {
            IQueryable<TB_USUARIO_LOGADO> query = (from q in this.Db.TB_USUARIO_LOGADO where q.TB_LOGIN_ID == loginId select q);

            return query.FirstOrDefault();
        }

        private TB_USUARIO_LOGADO GetSessionId(string sessionId)
        {
            IQueryable<TB_USUARIO_LOGADO> query = (from q in this.Db.TB_USUARIO_LOGADO where q.TB_SESSION_ID.Equals(sessionId) select q);

            return query.FirstOrDefault();
        }

        public override IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogado()
        {
            var _result = from _ul in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                          join _pl in this.Db.TB_PERFIL_LOGIN.Include("TB_PERFIL") on _ul.TB_LOGIN.TB_LOGIN_ID equals _pl.TB_LOGIN_ID
                          join _ge in this.DbSam.TB_GESTOR.Include("TB_ORGAO") on _pl.TB_GESTOR_ID_PADRAO equals _ge.TB_GESTOR_ID
                          join _u in this.Db.TB_USUARIO on _pl.TB_LOGIN.TB_USUARIO_ID equals _u.TB_USUARIO_ID
                          select _ul;

            return _result;
        }

        public override IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorGestor(int gestorId)
        {
            var _result = from _ul in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                          join _pl in this.Db.TB_PERFIL_LOGIN.Include("TB_PERFIL") on _ul.TB_LOGIN.TB_LOGIN_ID equals _pl.TB_LOGIN_ID
                          join _u in this.Db.TB_USUARIO on _pl.TB_LOGIN.TB_USUARIO_ID equals _u.TB_USUARIO_ID
                          where _pl.TB_GESTOR_ID_PADRAO == gestorId
                          select _ul;

            return _result;
        }

        public override IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorGestor(int gestorId, PerfilNivelAcesso perfilNivelAcesso)
        {
            var _perfil = GeralEnum.ObterPerfilPorPerfilNivelAcesso(perfilNivelAcesso);
            var _perfilIds = ObterCodigoListaEnum(_perfil);

            var _result = from _ul in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                          join _pl in this.Db.TB_PERFIL_LOGIN.Include("TB_PERFIL") on _ul.TB_LOGIN.TB_LOGIN_ID equals _pl.TB_LOGIN_ID
                          join _u in this.Db.TB_USUARIO on _pl.TB_LOGIN.TB_USUARIO_ID equals _u.TB_USUARIO_ID
                          where _pl.TB_GESTOR_ID_PADRAO == gestorId
                                && _perfilIds.Contains(_pl.TB_PERFIL_ID)
                          select _ul;

            return _result;
        }

        public override int ObterQtdeUsuarioLogadoPorGestorPerfil(int gestorId, PerfilNivelAcesso perfilNivelAcesso)
        {
            var _perfil = GeralEnum.ObterPerfilPorPerfilNivelAcesso(perfilNivelAcesso);
            var _perfilIds = ObterCodigoListaEnum(_perfil);

            var _result = from _ul in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                          join _pl in this.Db.TB_PERFIL_LOGIN.Include("TB_PERFIL") on _ul.TB_LOGIN.TB_LOGIN_ID equals _pl.TB_LOGIN_ID
                          join _u in this.Db.TB_USUARIO on _pl.TB_LOGIN.TB_USUARIO_ID equals _u.TB_USUARIO_ID
                          where _pl.TB_GESTOR_ID_PADRAO == gestorId
                                && _perfilIds.Contains(_pl.TB_PERFIL_ID)
                          select _ul;

            if (_result.FirstOrDefault() == null)
                return 0;

            return _result.Count();
        }

        private List<int> ObterCodigoListaEnum(List<TipoPerfil> listaEnum)
        {
            return (from _p in listaEnum
                    select _p.GetHashCode()).ToList<int>();
        }

        public override IList<Entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId = default(int))
        {
            IList<Entity.UsuarioLogadoPorGestorEntity> query = new List<Entity.UsuarioLogadoPorGestorEntity>();
            Entity.UsuarioLogadoPorGestorEntity _ulpg = null;
            int _idPerfilAdminGeral = Common.EnumPerfil.ADMINISTRADOR_GERAL.GetHashCode();


            foreach (var item in from _usuarioLogado in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                                 join _perfilLogin in this.Db.TB_PERFIL_LOGIN on _usuarioLogado.TB_LOGIN_ID equals _perfilLogin.TB_LOGIN_ID
                                 where ((_perfilLogin.TB_GESTOR_ID_PADRAO > 0 && gestorId == 0)
                                       || (_perfilLogin.TB_GESTOR_ID_PADRAO == gestorId && gestorId > 0))
                                       && _perfilLogin.TB_PERFIL_ID != _idPerfilAdminGeral
                                 orderby _perfilLogin.TB_GESTOR_ID_PADRAO, _usuarioLogado.TB_DATA_HORA_LOGADO descending
                                 select new
                                 {
                                     TB_GESTOR_ID_PADRAO = _perfilLogin.TB_GESTOR_ID_PADRAO,
                                     TB_LOGIN_ID = _usuarioLogado.TB_LOGIN_ID,
                                     TB_SESSION_ID = _usuarioLogado.TB_SESSION_ID,
                                     TB_IP_LOGADO = _usuarioLogado.TB_IP_LOGADO,
                                     TB_DATA_HORA_LOGADO = _usuarioLogado.TB_DATA_HORA_LOGADO
                                 })
            {
                if (_ulpg == null || item.TB_GESTOR_ID_PADRAO != _ulpg.GestorId)
                {
                    if (_ulpg != null)
                        query.Add(_ulpg);

                    _ulpg = (from _ge in this.DbSam.TB_GESTOR.Include("TB_ORGAO")
                             where _ge.TB_GESTOR_ID == item.TB_GESTOR_ID_PADRAO
                             select new Entity.UsuarioLogadoPorGestorEntity()
                             {
                                 GestorId = _ge.TB_GESTOR_ID,
                                 GestorDescricao = _ge.TB_GESTOR_NOME
                             }).FirstOrDefault<Entity.UsuarioLogadoPorGestorEntity>();

                    _ulpg.UsuarioLogado = new List<UsuarioLogadoEntity>();
                }

                _ulpg.UsuarioLogado.Add((from lo in this.Db.TB_LOGIN.Include("TB_USUARIO")
                                         where lo.TB_LOGIN_ID == item.TB_LOGIN_ID
                                         select new UsuarioLogadoEntity
                                         {
                                             LoginId = lo.TB_LOGIN_ID,
                                             Usuario = lo.TB_USUARIO.TB_USUARIO_CPF,
                                             UsuarioNome = lo.TB_USUARIO.TB_USUARIO_NOME_USUARIO,
                                             DataHoraLogado = item.TB_DATA_HORA_LOGADO,
                                             IpLogado = item.TB_IP_LOGADO,
                                             SessionIdLogado = item.TB_SESSION_ID,
                                             Login = (from _lo in this.Db.TB_LOGIN
                                                      where lo.TB_LOGIN_ID == _lo.TB_LOGIN_ID
                                                      select new Login
                                                      {
                                                          AcessoBloqueado = _lo.TB_LOGIN_BLOQUEADO ?? false,
                                                          ID = _lo.TB_LOGIN_ID,
                                                          Id = _lo.TB_LOGIN_ID,
                                                          LoginAtivo = _lo.TB_LOGIN_ATIVO,
                                                          Senha = _lo.TB_LOGIN_SENHA,
                                                         
                                                          NumeroTentativasInvalidas = _lo.TB_LOGIN_TENTATIVAS_INVALIDAS ?? 0,
                                                          SenhaBloqueada = _lo.TB_LOGIN_BLOQUEADO ?? false,
                                                          Criacao = _lo.TB_LOGIN_DATA_CADASTRO ?? new DateTime(1900, 1, 1),
                                                          Perfil = (from _pe in this.Db.TB_PERFIL
                                                                    join _pf in this.Db.TB_PERFIL_LOGIN on _pe.TB_PERFIL_ID equals _pf.TB_PERFIL_ID
                                                                    where _lo.TB_LOGIN_ID == _pf.TB_LOGIN_ID
                                                                          && _pf.TB_GESTOR_ID_PADRAO > 0
                                                                    select new Perfil
                                                                    {
                                                                        Id = _pe.TB_PERFIL_ID,
                                                                        Descricao = _pe.TB_PERFIL_DESCRICAO
                                                                    }).FirstOrDefault<Perfil>()
                                                      }).FirstOrDefault<Login>()
                                         }).FirstOrDefault<UsuarioLogadoEntity>());
            }

            if (!query.Contains(_ulpg))
                query.Add(_ulpg);

            if (query != null && query.Count > 0 && query[0] != null)
                query = query.OrderBy(a => a.GestorDescricao).ToList();

            return query.ToList();
        }

        public override IList<Entity.UsuarioLogadoEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId, int perfilId = default(int))
        {
            var _usuariosLogados = gestorId == 0 ? this.ListarTodosUsuariosLogadoPorGestor(gestorId) : this.ListarTodosUsuariosLogado(gestorId);

            if (_usuariosLogados.Count == 0)
                return new List<Entity.UsuarioLogadoEntity>();

            List<Entity.UsuarioLogadoEntity> _retorno = new List<UsuarioLogadoEntity>();

            foreach (var _item in _usuariosLogados[0].UsuarioLogado.OrderBy(a => a.UsuarioNome))
            {
                if (_item.Login.Perfil.Id == perfilId || perfilId == default(int))
                    _retorno.Add(_item);
            }

            return _retorno;
        }

        protected override IList<Entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogado(int gestorId)
        {
            return (from _ul in this.ListarTodosUsuariosLogadoPorGestor() where _ul.GestorId == gestorId select _ul).ToList<Entity.UsuarioLogadoPorGestorEntity>();
        }

        public override IList<TB_USUARIO_LOGADO> ListaUsuarioLogadosSessions()
        {
            IQueryable<TB_USUARIO_LOGADO> query = (from q in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                                                   join l in this.Db.TB_LOGIN.Include("TB_USUARIO") on q.TB_LOGIN_ID equals l.TB_LOGIN_ID 
                                                   select q).Distinct().AsQueryable();

            IList<TB_USUARIO_LOGADO> retorno = query.ToList();

            return retorno;
        }

        public override IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorOrgao(int orgaoId)
        {
            var _result = from _ul in this.Db.TB_USUARIO_LOGADO.Include("TB_LOGIN")
                          join _pl in this.Db.TB_PERFIL_LOGIN.Include("TB_PERFIL") on _ul.TB_LOGIN.TB_LOGIN_ID equals _pl.TB_LOGIN_ID
                          join _u in this.Db.TB_USUARIO on _pl.TB_LOGIN.TB_USUARIO_ID equals _u.TB_USUARIO_ID
                          where _pl.TB_ORGAO_ID_PADRAO == orgaoId
                          select _ul;

            return _result;
        }
    }
}