using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IRelacaoMaterialItemSubItemService : ICatalogoBaseService, ICrudBaseService<RelacaoMaterialItemSubItemEntity>
    {
        IList<RelacaoMaterialItemSubItemEntity> Listar(int _itemId, int _subItem, int _gestorId);
        RelacaoMaterialItemSubItemEntity Select(int _relacaoId);
        IList<RelacaoMaterialItemSubItemEntity> Imprimir(int _itemId, int _subItemId, int _gestorId);
    }
}
