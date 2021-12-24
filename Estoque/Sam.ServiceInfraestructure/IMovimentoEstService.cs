using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IMovimentoEstService: ICatalogoBaseService,ICrudBaseService<MovimentoEstEntity>
    {
        IList<MovimentoEstEntity> ListarTodosCod(int MovimentoId);
    }
}
