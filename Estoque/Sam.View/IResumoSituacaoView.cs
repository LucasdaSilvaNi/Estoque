using Sam.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IResumoSituacaoView : ICrudView
    {
        IList<OrgaoEntity> PopularListaOrgaoTodosCod();
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int idAlmoxarifado, int orgaoId);
        OrgaoEntity ListarCodigoOrgao(int orgaoId);
    }
}

