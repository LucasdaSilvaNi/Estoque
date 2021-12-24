using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IUnidadeService : ICatalogoBaseService,ICrudBaseService<UnidadeEntity>
    {
        IList<UnidadeEntity> Listar(int? GestorId);
        IList<UnidadeEntity> Listar(int OrgaoId, int GestorId);
        IList<UnidadeEntity> Imprimir(int OrgaoId, int GestorId);
        IList<UnidadeEntity> ListarTodosCod(int OrgaoId);
    }
}
