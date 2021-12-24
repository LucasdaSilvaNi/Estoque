using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Business
{
    public class PerfilLoginBusiness : BaseBusiness
    {
        public bool ExcluirLoginPerfil(int PerfilLoginId) 
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            infra.Excluir(PerfilLoginId);
            return false;
            
        }

        public void SalvarLoginPerfil(Sam.Entity.PerfilLogin perfilLogin, bool IsPerfilPadrao)
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            try
            {
                infra.Gravar(perfilLogin, IsPerfilPadrao);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public void AtualizarLoginPerfil(Sam.Entity.PerfilLogin perfilLogin, bool IsPerfilPadrao)
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            try
            {
                infra.Atualizar(perfilLogin, IsPerfilPadrao);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void ExcluirPerfilNivelAcesso(int perfilLoginId)
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            try
            {
                infra.ExcluirPerfilNivelAcesso(perfilLoginId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Entity.PerfilLogin GetPerfilLogin(int PerfilId, int LoginId)
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            try
            {
                var resultado = infra.GetPerfilLogin(PerfilId, LoginId);

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Entity.PerfilLogin> GetPerfilLogins(int PerfilId, int LoginId)
        {
            Infrastructure.PerfilLoginInfrastructure infra = new Infrastructure.PerfilLoginInfrastructure();
            try
            {
                var resultado = infra.GetPerfilLogins(PerfilId, LoginId);

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
