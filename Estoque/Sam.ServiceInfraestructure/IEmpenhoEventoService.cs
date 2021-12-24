using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Sam.ServiceInfraestructure
{
    public interface IEmpenhoEventoService: ICatalogoBaseService, ICrudBaseService<EmpenhoEventoEntity>
    {
        EmpenhoEventoEntity ObterEventoEmpenho(int codigoEvento);
        EmpenhoEventoEntity ObterEventoEmpenho(MovimentoEntity objMovimentacao);
        bool Excluir();
        IList<string> ListarEmpenhosPorAgrupamento(int almoxarifadoId, int gestorId, List<string> listaEmpenhos, bool consumoImediato);
    }
}
