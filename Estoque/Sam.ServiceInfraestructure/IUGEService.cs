using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IUGEService : ICatalogoBaseService, ICrudBaseService<UGEEntity>
    {
        IList<TipoUGEEntity> ListarTipoUGE();
        IList<UGEEntity> Listar(int? UoId);
        IList<UGEEntity> Listar(int OrgaoId, int UoId);
        IList<UGEEntity> Imprimir(int OrgaoId, int UoId);
        IList<UGEEntity> ListarUgesTodosCod(int? OrgaoId);
        IList<UGEEntity> ListarUgeComAlmoxarifado(int OrgaoId);
        IList<UGEEntity> ListarUgesTodosCodPorUo(int? UoId);
        IList<UGEEntity> ListarUgesTodosCodPorUo(int? UoId, IList<DivisaoEntity> divisaoList);
        IList<UGEEntity> ListarTodosCodPorGestor(int GestorId);
        IList<UGEEntity> ListarUGESaldoTodosCod(int gestorId, int almoxarifadoId);
        IList<UGEEntity> ListarUGESaldoTodosCod(int subItemId, int almoxarifadoId, int ugeId);
        IList<UGEEntity> ListarUGEsComSaldoParaSubitem(long subItemCodigo, int almoxarifadoId);
        UGEEntity LerRegistro(int pIntUgeID);
        IList<UGEEntity> Listar(System.Linq.Expressions.Expression<Func<UGEEntity, bool>> where);

        int ObterCodigoGestao(int codigoUGE);
        IList<UGEEntity> ListarUaAssociadaUge(int? uaId);
    }
}
