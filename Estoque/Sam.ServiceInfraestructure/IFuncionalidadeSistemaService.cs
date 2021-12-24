using Sam.Domain.Entity;
using System.Collections.Generic;



namespace Sam.ServiceInfraestructure
{
    public interface IFuncionalidadeSistemaService : ICatalogoBaseService, ICrudBaseService<FuncionalidadeSistemaEntity>
    {
        FuncionalidadeSistemaEntity ObterFuncionalidadeSistema(int funcionalidadeSistemaID);
        IList<FuncionalidadeSistemaEntity> Listar(int[] perfilIDs);

        bool Excluir();
    }
}
