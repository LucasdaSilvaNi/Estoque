using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ITerceiroService : ICatalogoBaseService,ICrudBaseService<TerceiroEntity>
    {
        IList<TerceiroEntity> Listar(int OrgaoId, int GestorId);
        IList<TerceiroEntity> Imprimir(int OrgaoId, int GestorId);
    }
}
