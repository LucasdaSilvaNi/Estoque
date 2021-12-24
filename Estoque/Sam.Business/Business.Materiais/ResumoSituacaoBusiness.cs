using Sam.Business.Business.Seguranca;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;
using Sam.Infrastructure;
using Sam.ServiceInfraestructure;
using Sam.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Sam.Business.ResumoSituacaoBusiness
{

    public class ResumoSituacaoBusiness
    {
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgao(int orgaoId)
        {
            AlmoxarifadoInfraestructure infraestructure = new AlmoxarifadoInfraestructure();
            IList<AlmoxarifadoEntity> retorno = infraestructure.ListarAlmoxarifadoPorOrgao(orgaoId);
            return retorno;
        }
        public IList<SubItemMaterialEntity> ListarItensEstoque(int filtro, int idAlmoxarifado)
        {
            SubItemMaterialInfraestructure infraestructure = new SubItemMaterialInfraestructure();
            IList<SubItemMaterialEntity> retorno = infraestructure.ListarItensEstoque(filtro, idAlmoxarifado);
            return retorno;
        }
        public List<Entity.UsuarioLogadoEntity> ListarUsuariosOnlinePorOrgao(int orgaoId)
        {
            return UsuarioLogadoBusiness.CreateInstance().ObterUsuarioLogadoPorOrgao(orgaoId);
        }
        public DataSet GerarExportacaoAlmoxarifadoStatusFechamento(int codigoOrgao, int idAlmoxarifado, string anoMesRef)
        {
            DataSet dsRetorno = new DataSet();
            try
            {
                AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
                dsRetorno = infra.GerarExportacaoAlmoxarifadoStatusFechamento(codigoOrgao, idAlmoxarifado, anoMesRef);

                return dsRetorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int idAlmoxarifado, int orgaoId)
        {
            AlmoxarifadoInfraestructure infraestructure = new AlmoxarifadoInfraestructure();
            IList<AlmoxarifadoEntity> retorno = infraestructure.ListarAlmoxarifadoStatusFechamento(idAlmoxarifado, orgaoId);
            return retorno;
        }
        public OrgaoEntity ListarCodigoOrgao(int orgaoId)
        {
            OrgaoEntity lista = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;
            eoBusiness = new EstruturaOrganizacionalBusiness();
            lista = eoBusiness.ListarCodigoOrgao(orgaoId);
            return lista;
        }


        public IList<UGEEntity> ListarUgeImplantada(int idOrgao)
        {
            UGEInfraestructure infraestructure = new UGEInfraestructure();
            IList<UGEEntity> retorno = infraestructure.ListarUgeImplantada(idOrgao);
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int orgaoId)
        {
            AlmoxarifadoInfraestructure infraestructure = new AlmoxarifadoInfraestructure();
            IList<AlmoxarifadoEntity> retorno = infraestructure.ListarInicioFechamento( orgaoId);
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarInicioFechamento(int orgaoId)
        {
            AlmoxarifadoInfraestructure infraestructure = new AlmoxarifadoInfraestructure();
            IList<AlmoxarifadoEntity> retorno = infraestructure.ListarInicioFechamento( orgaoId);
            return retorno;
        }

    }
}
