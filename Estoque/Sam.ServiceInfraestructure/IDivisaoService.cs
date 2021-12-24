using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IDivisaoService : ICatalogoBaseService,ICrudBaseService<DivisaoEntity>
    {
        IList<DivisaoEntity> Listar(int UoId, Int64 UgeId = default(int));
        IList<DivisaoEntity> Listar(int OrgaoId, int UaId);
        IList<DivisaoEntity> Imprimir(int OrgaoId, int UaId);
        DivisaoEntity Select(int _id);
        IList<DivisaoEntity> ListarPorUgeTodosCod(int UgeId);
        IList<DivisaoEntity> ListarPorAlmoxTodosCod(int AlmoxId);
        IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId);
        IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId, AlmoxarifadoEntity almoxarifado);
        IList<DivisaoEntity> ListarDivisaoByGestor(int gestorId, int? UOId, int? UGEId);

        DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA);
    }
}
