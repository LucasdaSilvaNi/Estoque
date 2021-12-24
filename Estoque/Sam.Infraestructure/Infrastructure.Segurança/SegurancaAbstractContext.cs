using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Sam.Common.Enums.PerfilNivelAcessoEnum;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Infrastructure.Infrastructure.Segurança
{
    public abstract class SegurancaAbstractContext : BaseInfrastructure
    {

        public SegurancaAbstractContext()
        {

        }

        public abstract void CreateUsuarioLogado(TB_USUARIO_LOGADO usuario);
        public abstract void RemoveUsuarioLogadoId(Int32 loginId);
        public abstract void RemoveUsuarioLogadoSessionId(String sessionId);
        public abstract void RemoveAllUsuarioLogado();
        public abstract IList<Entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId = default(int));
        public abstract IList<Entity.UsuarioLogadoEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId, int perfilId = default(int));
        public abstract List<TB_USUARIO_LOGADO> UsuarioLogado(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf = null, int gestorId = default(int));
        public abstract TB_USUARIO_LOGADO Get(Int32 loginId);
        public abstract TB_USUARIO_LOGADO GetByLoginId(int loginId);
        public abstract IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogado();
        public abstract IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorGestor(int gestorId);
        public abstract IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorGestor(int gestorId, PerfilNivelAcesso perfilNivelAcesso);
        public abstract int ObterQtdeUsuarioLogadoPorGestorPerfil(int gestorId, PerfilNivelAcesso perfilNivelAcesso);
        public abstract IList<TB_USUARIO_LOGADO> ListaUsuarioLogadosSessions();
        protected abstract IList<Entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogado(int gestorId);
        public abstract IQueryable<TB_USUARIO_LOGADO> ObterUsuarioLogadoPorOrgao(int orgaoId);
    }
}
