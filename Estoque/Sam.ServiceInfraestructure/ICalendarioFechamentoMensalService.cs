using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Sam.ServiceInfraestructure
{
    public interface ICalendarioFechamentoMensalService : ICatalogoBaseService, ICrudBaseService<CalendarioFechamentoMensalEntity>
    {
        CalendarioFechamentoMensalEntity ObterDataFechamentoMensal(int mesReferencia, int anoReferencia);
        bool ExisteMesReferenciaInformado();

        bool Excluir();
        bool ExisteCodigoInformado();

        IList<int> ListarAno();
        IList<CalendarioFechamentoMensalEntity> Listar(int Ano);
    }
}
