using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IUOService : ICatalogoBaseService, ICrudBaseService<UOEntity>
    {
        IList<UOEntity> Listar(int OrgaoId);
        IList<UOEntity> ListarTodosCod(int OrgaoId);
        IList<UOEntity> Imprimir(int OrgaoId);
        IList<UOEntity> ListarTodosCod(int OrgaoId, IList<DivisaoEntity> divisaoList);
        UOEntity ObterUoPorCodigoUGE(int codigoUGE);
        IList<UOEntity> ListarUgePorUo(int ugeId);
    }
}
