using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ISubItemMaterialAlmox : ICatalogoBaseService, ICrudBaseService<SubItemMaterialEntity>
    {
        IList<SubItemMaterialEntity> Listar(int _itemId, int _gestor_id);
        IList<SubItemMaterialEntity> ListarTodosCod(int _itemId, int _gestor_id);
        IList<SubItemMaterialEntity> Imprimir(int _itemId, int _gestor_id);
        SubItemMaterialEntity Select(int _id);
    }
}
