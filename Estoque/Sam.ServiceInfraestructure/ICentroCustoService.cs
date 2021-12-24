using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ICentroCustoService : ICatalogoBaseService,ICrudBaseService<CentroCustoEntity>
    {
        IList<CentroCustoEntity> Listar(int GestorId);
        IList<CentroCustoEntity> Listar(int OrgaoId, int GestorId);
        IList<CentroCustoEntity> Imprimir(int OrgaoId, int GestorId);
        IList<CentroCustoEntity> ListarTodosCodPorOrgao(int OrgaoId);
    }
}
