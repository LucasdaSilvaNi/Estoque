using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IOrgaoService : ICatalogoBaseService, ICrudBaseService<OrgaoEntity>
    {
        IList<OrgaoEntity> ListarOrgaosPorGestao(int codigoGestao, bool excluirOrgaosGestaoDoRetorno = false, bool gerarComCodigoDescricao = true);
        IList<OrgaoEntity> ListarOrgaosPorGestaoImplantado(int codigoGestao, bool excluirOrgaosGestaoDoRetorno = false, bool gerarComCodigoDescricao = true);
        IList<OrgaoEntity> ListarUoPorOrgao(int? orgaoId);
        IList<OrgaoEntity> ListarTodosCod(int? orgaoId);
         OrgaoEntity ListarCodigoOrgao(int orgaoId);
    }
}
