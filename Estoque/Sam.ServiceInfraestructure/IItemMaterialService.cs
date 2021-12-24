using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
namespace Sam.ServiceInfraestructure
{
    public interface IItemMaterialService : ICatalogoBaseService, ICrudBaseService<ItemMaterialEntity>
    {
        ItemMaterialEntity Select(int _materialId);
        IList<ItemMaterialEntity> Listar(int _materialId);
        IList<ItemMaterialEntity> ListarAlmox(int _materialId, int _gestorId, int _almoxarifadoId);
        IList<ItemMaterialEntity> ListarItemSaldoByAlmox(int almoxarifado);        
        IList<ItemMaterialEntity> ListarBySubItem(int _subItem);
        IList<ItemMaterialEntity> ListarTodosCod(int _materialId, bool todos);
        IList<ItemMaterialEntity> Imprimir(int _materialId);
        ItemMaterialEntity Select(ItemMaterialEntity itemMat);
        IList<ItemMaterialEntity> ListarSubItemCod(int _materialId);        
        IList<ItemMaterialEntity> ListarPorPalavraChaveTodosCod(int? Id, int? Codigo, string Descricao, int? AlmoxId, int? GestorId);
        IList<ItemMaterialEntity> ListarItemMaterialPorCodigoSiafisico(int? Codigo);
        IList<ItemMaterialEntity> ListarBySubItem(int _subItem, int _gestorId);
        ItemMaterialEntity GetItemMaterialByItem(int itemId);
        ItemMaterialEntity GetItemMaterialBySubItem(int subItemId);
        ItemMaterialEntity GetItemMaterialNaturezaDespesa();
        ItemMaterialEntity ObterItemMaterial(int iCodigoItemMaterial);
        string NaturezaSubItem(int _subItem);
        bool SalvarSiafisico();
        bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int iNaturezaDespesa_ID);
        bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa);
        bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa, bool throwSeNaoExistir = false);
        bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int iNaturezaDespesa_ID);
        bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa);
    }
}
