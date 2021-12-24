using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
namespace Sam.ServiceInfraestructure
{
    public interface INaturezaDespesaService : ICatalogoBaseService, ICrudBaseService<NaturezaDespesaEntity>
    {
        NaturezaDespesaEntity ObterNaturezaDespesa(int natDespesaID);
        NaturezaDespesaEntity ObterNaturezaDespesa(string codigoNaturezaDespesa);
        List<String> ListarNaturezaConsumoImediato();

        bool ExcluirNaturezaDespesa();
    }
}
