using System.Collections.Generic;
using Sam.Entity;


namespace Sam.ServiceInfraestructure
{
    public interface IMaterialApoioService : ICatalogoBaseService, ICrudBaseService<MaterialApoioEntity>
    {
        IList<MaterialApoioEntity> ListarRecursosPorPerfil(int? Perfil_ID);
    }
}
