using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ISiglaService : ICatalogoBaseService,ICrudBaseService<SiglaEntity>
    {
        IList<SiglaEntity> Listar(int OrgaoId, int GestorId);
        IList<SiglaEntity> Imprimir(int OrgaoId, int GestorId);
    }
}
