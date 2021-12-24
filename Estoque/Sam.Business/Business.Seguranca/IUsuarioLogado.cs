using Sam.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Sam.Common.Enums.PerfilNivelAcessoEnum;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Business.Business.Seguranca
{
    public interface IUsuarioLogado
    {
        void CreateUsuarioLogado(UsuarioLogadoEntity usuario);
        void RemoveUsuarioLogadoId(Int32 loginId);
        void RemoveUsuarioLogadoSessionId(String sessionId);
        void RemoveAllUsuarioLogado();
        bool ConsistirUsuariosOnline(int gestorId, PerfilNivelAcesso nivelAcesso);
        List<UsuarioLogadoEntity> UsuarioLogado(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf, int gestorId = default(int));
        IList<UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId = default(int));
        IList<UsuarioLogadoEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId, int perfilId = default(int));
        List<UsuarioLogadoEntity> ListarUsuarioLogadoPorGestor(int gestorId);
        List<UsuarioLogadoEntity> ObterUsuarioLogadoPorOrgao(int orgaoId);
        List<UsuarioLogadoEntity> ListarUsuarioLogadoPorGestor(int gestorId, PerfilNivelAcesso nivelAcesso);
        int ObterQtdeUsuarioLogadoPorGestorEPerfil(int gestorId, PerfilNivelAcesso nivelAcesso);
        int ObterQtdeUsuariosContratado(int gestorId, PerfilNivelAcesso nivelAcesso);
        UsuarioLogadoEntity Get(Int32 loginId);
        Int32 TotalRegistro { get; set; }
        IList<UsuarioLogadoEntity> ListaUsuarioLogadosSessions();
    }
}
