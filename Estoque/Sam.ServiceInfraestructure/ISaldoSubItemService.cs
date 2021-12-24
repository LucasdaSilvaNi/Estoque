using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ISaldoSubItemService: ICatalogoBaseService,ICrudBaseService<SaldoSubItemEntity>
    {
        SaldoSubItemEntity Consultar(int? SubItemMaterialId, int? AlmoxId, int? UgeId, string _LoteIdent, string fabricanteLote, DateTime? dataVencimentoLote);
        SaldoSubItemEntity ConsultarSaldoSubItem(AlmoxarifadoEntity almoxarifado, UGEEntity uge, SubItemMaterialEntity subItem);
        void Salvar(List<SaldoSubItemEntity> SubItens);
        IList<SaldoSubItemEntity> ImprimirConsultaEstoqueSintetico(int UgeId, int AlmoxId, int GrupoId, int ComSemSaldo, int? _ordenarPor);
        IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId,int? ugeId, int? almoxId);
        IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? almoxId, string lote);
        IList<UGEEntity> ConsultarUgesBySubItemAlmox(int almoxarifado, int subItem, int ugeId);
        IList<SubItemMaterialEntity> ImprimirConsumoAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal);
        IList<SubItemMaterialEntity> ImprimirConsumoAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod);        
        IList<SubItemMaterialEntity> ImprimirConsumoDivisao(int? divisao, int? _almoxId, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod);
        IList<SaldoSubItemEntity> ListarFechamento(int? almoxId, int? anomes);
        decimal? CalculaTotalSaldoUGEsReserva(int? subItemId, int? almoxId);
        Tuple<decimal?, decimal?, decimal?> SaldoMovimentoItemDataMovimento(int? subItemId, int? almoxId, int? ugeId, DateTime dataMovimento);
        IList<SubItemMaterialEntity> ImprimirPrevisaoConsumoAlmox(int? pIntAlmoxarifado_ID, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod = true);
        bool VerificaSubItemUtilizado(int subItemId);
    }
}
