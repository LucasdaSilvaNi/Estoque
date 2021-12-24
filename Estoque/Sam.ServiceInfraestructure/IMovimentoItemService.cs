using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IMovimentoItemService : ICatalogoBaseService, ICrudBaseService<MovimentoItemEntity>
    {
        IList<MovimentoItemEntity> Listar(int MovimentoId);
        //IList<MovimentoItemEntity> Imprimir(int MovimentoId);
        MovimentoItemEntity Estornar(int MovimentoId);
        MovimentoItemEntity LerRegistroItem(int MovimentoId);
        IList<MovimentoItemEntity> ListarPorMovimento(MovimentoEntity mov);
        //IList<MovimentoItemEntity> ListarPorMovimentoTodos(MovimentoEntity mov);
        IList<MovimentoEntity> ListarMovimentoItemSaldoTodos(MovimentoEntity movimento);
        IList<MovimentoItemEntity> ListarMovimentacaoItem(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime DtInicial, DateTime DtFinal, bool comEstorno);
        IList<MovimentoItemEntity> ListarSubItemLote(string Lote, int idFornecedor, int idSubItem);
        IList<MovimentoItemEntity> ListarMovimentacaoItemRecalculo(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal);
        IList<MovimentoItemEntity> ListarMovimentacaoItemPorId(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal);
        IList<MovimentoItemEntity> ImprimirMovimento(DateTime? dataInicial, DateTime? dataFinal, bool? consultaTrasnf);
        IList<MovimentoItemEntity> ImprimirMovimento();        
        IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo);
        IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID);
        IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, int iSubItemMaterialCodigo_ID, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID);
        IList<MovimentoItemEntity> ListarSaldoValorConsumoDeItemMaterial(int iItemMaterialCodigo, int iSubItemMaterialCodigo_ID, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID);
        string SelectUnidFornecimentoSiafisico(int codUnidade);
        IList<MovimentoItemEntity> ListarNotaRequisicao(int MovimentoId);
        IList<MovimentoItemEntity> ListarMovimentacaoItemPorIdEstorno(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal);

        void AtualizarItensMovimentacaoComNotaSIAF(int movItemID, string nlLiquidacao, Enum TipoNotaSIAFEM, Enum TipoLancamentoSIAFEM);
        void AtualizarItensMovimentacaoComNotaSIAF(int[] movItemIDs, string nlLiquidacao, Enum TipoNotaSIAFEM, Enum TipoLancamentoSIAFEM);
        IList<string> ObterNLsMovimentacao(int movimentacaoMaterialID, Enum @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false);
        string ObterNumerosDocumentoPorMovimentoItemIDs(IList<int> arrMovItemIDs);
    }
}
