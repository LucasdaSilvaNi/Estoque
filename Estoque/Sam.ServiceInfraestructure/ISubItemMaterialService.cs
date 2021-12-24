using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ISubItemMaterialService : ICatalogoBaseService,  ICrudBaseService<SubItemMaterialEntity>
    {
        IList<SubItemMaterialEntity> Listar(int _itemId, int _gestor_id, bool? IndAtividade);
        IList<SubItemMaterialEntity> ListarTodosCod(int _itemId, int _gestor_id);
        IList<SubItemMaterialEntity> ListarSubItemAlmox(SubItemMaterialEntity subItem);
        IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestor_id, int _almoxarifadoId);
        IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestor_id);
        IList<SubItemMaterialEntity> ListarSubItem(int _subItemId, int _gestor_id);        
        IList<SubItemMaterialEntity> ListarSubItemAlmox(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId);
        IList<SubItemMaterialEntity> ListarSubItemAlmox(int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorId, int _almoxarifadoId, int iNaturezaDespesa_ID, Int64? _SubItemCodigo, Int64?  _Itemcodigo, int _indicadorId, int _saldoId);
        bool atualizarAlmoxSaldo(int _gestorId, int _almoxarifadoId, bool _indicadorDisponivel);
        IList<SubItemMaterialEntity> Imprimir(int _itemId, int _gestor_id);
        IList<SubItemMaterialEntity> Imprimir(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId);
        SubItemMaterialEntity Select(int _id);
        SubItemMaterialEntity SelectAlmox(int _id, int _idAlmoxarifado);
        IList<SubItemMaterialEntity> ListarTodosCodPorItemUgeAlmox(int _itemId, int _ugeId, int _almoxId);
        //IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemId, int _ugeId, int _almoxId, int _codigonaturezaDespesa);
        IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemCodigo, int _ugeId, int _almoxId, int gestorId, int _codigoNaturezaDespesa);
        //IList<SubItemMaterialEntity> ListarSubItemPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado);
        //IList<SubItemMaterialEntity> ListarSubItemPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado, bool subitemComSaldo);
        IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado, int item);
        IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado);
        IList<SubItemMaterialEntity> ListarSubItemPorAlmox(int _itemId, int _almoxarifadoId);
        IList<SubItemMaterialEntity> ListarSubItemByAlmoxItemMaterial(int almoxarifado, int itemId, string natDespesa);
        IList<SubItemMaterialEntity> ListarSubItemAlmoxarifadoItemNatureza(int almoxarifado, int itemCodigo, string natDespesa);
        IList<SubItemMaterialEntity> ListarCatalogoGestor(int gestorId, int naturezaDespesaCodigo, int itemCod, long subitemCod);
        void SalvarAlmox();
        IList<SubItemMaterialEntity> Listar(System.Linq.Expressions.Expression<Func<SubItemMaterialEntity, bool>> where);
        IList<SubItemMaterialEntity> ListarSubItemMaterial(System.Linq.Expressions.Expression<Func<SubItemMaterialEntity, bool>> where);
        IList<SubItemMaterialEntity> ListarItensEstoque(int filtro, int idAlmoxarifado);
    }
}
