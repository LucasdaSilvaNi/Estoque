using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IReservaMaterialService : ICatalogoBaseService, ICrudBaseService<ReservaMaterialEntity>
    {
        IList<ReservaMaterialEntity> Listar(int almoxarifado);
        IList<ReservaMaterialEntity> Imprimir(int almoxarifado);
        ReservaMaterialEntity Select(int id);
        IList<ReservaMaterialEntity> ListarReservaPorPeriodoAlmoxSubItem(int almoxarifado, long SubItemMaterialCodigo, DateTime[] periodo);
    }
}
