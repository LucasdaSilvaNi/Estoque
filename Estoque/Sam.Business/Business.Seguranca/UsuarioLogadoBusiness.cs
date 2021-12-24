using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using entity = Sam.Entity;
using static Sam.Common.Enums.PerfilNivelAcessoEnum;
using static Sam.Common.Util.GeralEnum;
using Sam.Infrastructure.Infrastructure.Segurança;
using System.Transactions;

namespace Sam.Business.Business.Seguranca
{
    public class UsuarioLogadoBusiness : IUsuarioLogado
    {
        private SegurancaAbstractContext repositorio = null;

        public Int32 TotalRegistro { get; set; }
        private UsuarioLogadoBusiness()
        {
            repositorio = UsuarioLogadoInfrastructue.CreateInstance();
        }

        //IOC
        public static IUsuarioLogado CreateInstance()
        {
            return new UsuarioLogadoBusiness();
        }

        public void CreateUsuarioLogado(entity.UsuarioLogadoEntity usuario)
        {
            repositorio.CreateUsuarioLogado(ConverterUsuarioLogadoEntityToTB_USUARIO_LOGADO(usuario));
        }

        public void RemoveUsuarioLogadoId(int loginId)
        {
            repositorio.RemoveUsuarioLogadoId(loginId);
        }

        public void RemoveUsuarioLogadoSessionId(string sessionId)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                try
                {
                    repositorio.RemoveUsuarioLogadoSessionId(sessionId);
                    transaction.Complete();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void RemoveAllUsuarioLogado()
        {
            repositorio.RemoveAllUsuarioLogado();
        }

        public List<entity.UsuarioLogadoEntity> UsuarioLogado(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf, int gestorId = default(int))
        {
            var usuarios = repositorio.UsuarioLogado(maximumRowsParameterName, StartRowIndexParameterName, cpf, gestorId).ConvertAll(new Converter<TB_USUARIO_LOGADO, entity.UsuarioLogadoEntity>(ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity));
            TotalRegistro = repositorio.TotalRegistros();
            return usuarios;
        }

        public IList<entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId = default(int))
        {
            var usuarios = repositorio.ListarTodosUsuariosLogadoPorGestor(gestorId);
            TotalRegistro = repositorio.TotalRegistros();
            return usuarios;
        }

        public IList<entity.UsuarioLogadoEntity> ListaUsuarioLogadosSessions()
        {
            IList<entity.UsuarioLogadoEntity> usuariosLogados = null;

            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    var retorno = (List<TB_USUARIO_LOGADO>)repositorio.ListaUsuarioLogadosSessions();

                    usuariosLogados = retorno.ConvertAll(new Converter<TB_USUARIO_LOGADO, entity.UsuarioLogadoEntity>(ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity));
                    transaction.Complete();


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                return usuariosLogados;

            }

        }

        public IList<entity.UsuarioLogadoEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId, int perfilId = default(int))
        {
            var usuarios = repositorio.ListarTodosUsuariosLogadoPorGestor(gestorId, perfilId);
            TotalRegistro = repositorio.TotalRegistros();
            return usuarios;
        }

        public List<entity.UsuarioLogadoEntity> ListarUsuarioLogadoPorGestor(int gestorId)
        {
            var _usuariosLogados = repositorio.ObterUsuarioLogadoPorGestor(gestorId).ToList<TB_USUARIO_LOGADO>().ConvertAll(new Converter<TB_USUARIO_LOGADO, entity.UsuarioLogadoEntity>(ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity));
            return _usuariosLogados;
        }

        public List<entity.UsuarioLogadoEntity> ListarUsuarioLogadoPorGestor(int gestorId, PerfilNivelAcesso nivelAcesso)
        {
            var _perfil = nivelAcesso.GetHashCode();
            var _usuariosLogados = repositorio.ObterUsuarioLogadoPorGestor(gestorId, nivelAcesso).ToList<TB_USUARIO_LOGADO>().ConvertAll(new Converter<TB_USUARIO_LOGADO, entity.UsuarioLogadoEntity>(ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity));

            return _usuariosLogados;
        }

        public int ObterQtdeUsuarioLogadoPorGestorEPerfil(int gestorId, PerfilNivelAcesso nivelAcesso)
        {
            return repositorio.ObterQtdeUsuarioLogadoPorGestorPerfil(gestorId, nivelAcesso);
        }

        public int ObterQtdeUsuariosContratado(int gestorId, PerfilNivelAcesso nivelAcesso)
        {
            return new ESPBusiness().ObterTotalUsuarios(gestorId, DateTime.Now, nivelAcesso);
        }

        /// <summary>
        /// Consiste a quantidade de usuários online seja menor do que a quantidade de usuários contratada para o perfil especificado
        /// </summary>
        /// <param name="gestorId">Código do Gestor</param>
        /// <param name="nivelAcesso">Perfil</param>
        /// <returns>[True] - Excedeu a quantidade contratada / [False] - Não excedeu o contrato</returns>
        public bool ConsistirUsuariosOnline(int gestorId, PerfilNivelAcesso nivelAcesso)
        {
            return ObterQtdeUsuariosContratado(gestorId, nivelAcesso) > ObterQtdeUsuarioLogadoPorGestorEPerfil(gestorId, nivelAcesso);
        }

        public Entity.UsuarioLogadoEntity Get(int loginId)
        {
            return ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity(repositorio.Get(loginId));
        }

        private TB_USUARIO_LOGADO ConverterUsuarioLogadoEntityToTB_USUARIO_LOGADO(entity.UsuarioLogadoEntity entity)
        {
            TB_USUARIO_LOGADO usuario = new TB_USUARIO_LOGADO();

            usuario.TB_DATA_HORA_LOGADO = entity.DataHoraLogado;
            usuario.TB_IP_LOGADO = entity.IpLogado;
            usuario.TB_LOGIN_ID = entity.LoginId;
            usuario.TB_SESSION_ID = entity.SessionIdLogado;

            return usuario;
        }

        private entity.UsuarioLogadoEntity ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity(TB_USUARIO_LOGADO usuario)
        {
            entity.UsuarioLogadoEntity _entity = null;
            if (usuario != null)
            {
                _entity = new entity.UsuarioLogadoEntity();

                _entity.DataHoraLogado = usuario.TB_DATA_HORA_LOGADO;
                _entity.IpLogado = usuario.TB_IP_LOGADO;
                _entity.LoginId = usuario.TB_LOGIN_ID.Value;
                _entity.SessionIdLogado = usuario.TB_SESSION_ID;
                if (usuario.TB_LOGIN != null)
                    _entity.UsuarioNome = usuario.TB_LOGIN.TB_USUARIO.TB_USUARIO_NOME_USUARIO;

                _entity.Usuario = usuario.TB_LOGIN.TB_USUARIO.TB_USUARIO_CPF;
                _entity.UsuarioId = usuario.TB_LOGIN.TB_USUARIO_ID;
                _entity.Login = new entity.Login()
                {
                    ID = usuario.TB_LOGIN.TB_LOGIN_ID,
                    Id = usuario.TB_LOGIN.TB_LOGIN_ID,
                    LoginAtivo = usuario.TB_LOGIN.TB_LOGIN_ATIVO
                };
            }
            return _entity;
        }

        public List<entity.UsuarioLogadoEntity> ObterUsuarioLogadoPorOrgao(int almoxarifadoId)
        {
            var _usuariosLogados = repositorio.ObterUsuarioLogadoPorOrgao(almoxarifadoId).ToList<TB_USUARIO_LOGADO>().ConvertAll(new Converter<TB_USUARIO_LOGADO, entity.UsuarioLogadoEntity>(ConverterTB_USUARIO_LOGADOToUsuarioLogadoEntity));
            return _usuariosLogados;
           
        }
    }
}