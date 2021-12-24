using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IAlmoxarifadoService : ICatalogoBaseService, ICrudBaseService<AlmoxarifadoEntity>
    {
        IList<UGEEntity> ListarUGEGestor(int GestorId);
        IList<AlmoxarifadoEntity> Listar(int OrgaoId, int GestorId);

        IList<AlmoxarifadoEntity> ListarCodigo(int OrgaoId, int GestorId, string AlmoxCodigo);
        IList<AlmoxarifadoEntity> Imprimir(int OrgaoId, int GestorId);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoTodosCod(int OrgaoId);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoMesRef(int OrgaoId, string AnoMesReferencia);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId, bool pBlnPreencherObjetos);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoGestor(int OrgaoId, int GestorId);
        IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod();
        IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod(int _OrgaoId, int _GestorId);
        IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifado(int OrgaoId, int GestorId, int AlmoxarifadoId);
        IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifadoTake(int OrgaoId, int GestorId, int AlmoxarifadoId);        
        AlmoxarifadoEntity SelecionarAlmoxarifadoPorGestor(int almoxarifadoId);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadosNivelAcesso(int idGestor, List<AlmoxarifadoEntity> almoxarifadosNivelAcesso);
        AlmoxarifadoEntity GetAlmoxarifadoByDivisao(int divisaoId);
        AlmoxarifadoEntity ObterAlmoxarifado(int? idAlmoxarifado);
        IList<AlmoxarifadoEntity> Listar(System.Linq.Expressions.Expression<Func<AlmoxarifadoEntity, bool>> where);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorUGE(int UGEId);
        AlmoxarifadoEntity ObterAlmoxarifadoUGE(int codigoUGE, int codigoAlmoxarifado);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgao(int OrgaoId);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int idAlmoxarifado, int OrgaoId);
        bool ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(int almoxarifadoId);
        string ObtemMesReferenciaAtual(int almoxarifadoId);
    }
}
