using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
namespace Sam.ServiceInfraestructure
{
    public interface IGrupoService : ICatalogoBaseService, ICrudBaseService<GrupoEntity>
    {
        IList<GrupoEntity> ListarTodosCod(AlmoxarifadoEntity almoxarifado);
        GrupoEntity ObterGrupoMaterial(int codigoGrupoMaterial);
        IList<GrupoEntity> Listar(System.Linq.Expressions.Expression<Func<GrupoEntity, bool>> where);
    }
}
