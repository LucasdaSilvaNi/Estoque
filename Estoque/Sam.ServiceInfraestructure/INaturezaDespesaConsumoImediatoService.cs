using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
namespace Sam.ServiceInfraestructure
{
    public interface INaturezaDespesaConsumoImediatoService : ICatalogoBaseService, ICrudBaseService<NaturezaDespesaConsumoImediatoEntity>
    {
        NaturezaDespesaConsumoImediatoEntity ObterNaturezaDespesaConsumoImediato(int codigoNaturezaDespesa);
        IList<String> ListarNaturezasDespesaConsumoImediato();

        bool ExcluirNaturezaDespesaConsumoImediato();
        bool ExisteItemMaterialRelacionado();
    }
}
