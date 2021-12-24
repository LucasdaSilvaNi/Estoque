using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
   public interface IUAService : ICatalogoBaseService, ICrudBaseService<UAEntity>
    {
       IList<UAEntity> ListarPorUGE(int UgeId);
       IList<UAEntity> ListarPorUGE(int UgeId, int UaId);
       IList<UAEntity> ListarPorOrgao(int OrgaoId);
       IList<UAEntity> Imprimir(int UgeId);
       IList<UAEntity> ListarTodas();
       IList<UAEntity> ListarUasTodosCod(int? OrgaoId);
       IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId);
       IList<UAEntity> ListarUasTodosCodPorUo(int? UoId);
       IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almoxarifado);
       IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almoxarifado, bool mostraDivisaoEspecial);

        IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId, IList<DivisaoEntity> divisaoList);
       IList<UAEntity> Listar(System.Linq.Expressions.Expression<Func<UAEntity, bool>> where);

       UAEntity ObterUA(int uaID);
       UAEntity ObterUAPorCodigo(int uaCodigo, int gestorId);
       UAEntity ObterUAAtivaPorCodigo(int uaCodigo);
       IList<UAEntity> ListarUAsPorUO(int codigoUO);
       IList<UAEntity> ListarUAsPorUGE(int codigoUGE);
    }
}
