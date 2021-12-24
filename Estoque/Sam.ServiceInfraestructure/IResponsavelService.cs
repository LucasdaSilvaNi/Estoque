using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IResponsavelService : ICatalogoBaseService,ICrudBaseService<ResponsavelEntity>
    {
        IList<ResponsavelEntity> Listar(int GestorId);
        IList<ResponsavelEntity> Imprimir(int OrgaoId, int GestorId);
        IList<ResponsavelEntity> ListarTodosCodPorOrgao(int OrgaoId);
        IList<ResponsavelEntity> ListarTodosPorOrgaoGestor(int OrgaoId, int gestorId);
        IList<ResponsavelEntity> ListarTodosCodUa();
    }
}
