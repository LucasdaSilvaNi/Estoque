using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{

    public interface IUnidadeFornecimentoSiafemService : ICatalogoBaseService, ICrudBaseService<UnidadeFornecimentoSiafemEntity>
    {
        //IList<UnidadeFornecimentoSiafemEntity> Listar(int? OrgaoId);
        //IList<UnidadeFornecimentoSiafemEntity> Listar(int OrgaoId, int GestorId);
        //IList<UnidadeFornecimentoSiafemEntity> Listar(int? OrgaoId, bool noSkipResultSet);
        //IList<UnidadeFornecimentoSiafemEntity> ListarTodosCod(int OrgaoId, int GestorId);
        //IList<UnidadeFornecimentoSiafemEntity> Imprimir(int OrgaoId, int GestorId);
        //IList<UnidadeFornecimentoSiafemEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId);
    }
}
