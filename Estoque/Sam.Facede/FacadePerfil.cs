using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Facade
{
    public class FacadePerfil
    {
        public Perfil RecuperarPerfil(string login)
        {
            return new Perfil();
        }

        public void LiberarPerfil(List<Transacao> transacoes, Perfil perfil)
        {
        }

        public void AtribuirPermissoes(List<Transacao> transacoes, Usuario usuario)
        {}

        public void CopiarPerfil(Perfil origem, Perfil destino)
        {}

        public void InativarUsuario(string login)
        { }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcessoUser(int IdUsuario)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarPerfilLoginNivelAcessoUser(IdUsuario);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int perfilLoginId)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarPerfilLoginNivelAcesso(perfilLoginId);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idPerfil, string NivelAcessoDesc)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarPerfilLoginNivelAcesso(idPerfil, NivelAcessoDesc);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idPerfil, string NivelAcessoDesc, short perfil)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarPerfilLoginNivelAcesso(idPerfil, NivelAcessoDesc, perfil);
        }

        public IList<UAEntity> ListarUAByPerfil(int idLogin)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarUAByPerfil(idLogin);
        }

        // #Requisitante Geral.
        public IList<UAEntity> ListarUAByUgeId(int idUGE)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarUAByUgeId(idUGE);
        }

        // #Requisitante Geral.
        public IList<UGEEntity> ListarUGEByPerfil(int idLogin)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarUGEByPerfil(idLogin);
        }

        // #Requisitante Geral.
        public IList<UOEntity> ListarUOByPerfil(int idLogin)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarUOByPerfil(idLogin).Distinct().ToList();
        }

        // #Requisitante Geral.
        public IList<OrgaoEntity> ListarOrgaoByPerfil(int idLogin)
        {
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarOrgaoByPerfil(idLogin);
        }

        //# New Query Requisição por numeroDocumento.
        public IList<MovimentoEntity> ListarMovimentoEntityByPerfil(int idLogin, short idPerfil, string numeroDocumento)
        { 
            return new Sam.Business.PerfilLoginNivelAcessoBusiness().ListarMovimentoByPerfil(idLogin, idPerfil, numeroDocumento);
        }
    }
}
