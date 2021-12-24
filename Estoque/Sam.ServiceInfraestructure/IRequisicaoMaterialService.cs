using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IRequisicaoMaterial : ICatalogoBaseService,ICrudBaseService<DivisaoEntity>
    {
        IList<DivisaoEntity> Listar(int OrgaoId, int UaId);
        IList<DivisaoEntity> Imprimir(int OrgaoId, int UaId);
        DivisaoEntity Select(int _id);
    }
}
