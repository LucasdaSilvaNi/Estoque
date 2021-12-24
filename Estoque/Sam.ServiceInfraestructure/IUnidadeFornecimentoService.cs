using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IUnidadeFornecimentoService : ICatalogoBaseService,ICrudBaseService<UnidadeFornecimentoEntity>
    {
        IList<UnidadeFornecimentoEntity> Listar(int? OrgaoId);
        IList<UnidadeFornecimentoEntity> Listar(int OrgaoId, int GestorId);
        IList<UnidadeFornecimentoEntity> Listar(int? OrgaoId, bool noSkipResultSet);
        IList<UnidadeFornecimentoEntity> ListarTodosCod(int OrgaoId, int GestorId);
        IList<UnidadeFornecimentoEntity> Imprimir(int OrgaoId, int GestorId);
        IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId);
    }
}
