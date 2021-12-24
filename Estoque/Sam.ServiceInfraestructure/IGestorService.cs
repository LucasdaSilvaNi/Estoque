using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IGestorService : ICatalogoBaseService,ICrudBaseService<GestorEntity>
    {
        IList<GestorEntity> Listar(int OrgaoId);
        IList<GestorEntity> ListarTodosCod(int OrgaoId);
        GestorEntity Selecionar(int GestorId);
        IList<GestorEntity> Imprimir(int OrgaoId);
        int RetornaGestorOrganizacional(int? orgaoId, int? uoId, int? ugeId);
        IList<GestorEntity> Listar(System.Linq.Expressions.Expression<Func<GestorEntity, bool>> where);
    }
}
