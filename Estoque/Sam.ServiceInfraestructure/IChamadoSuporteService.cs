using System;
using System.Collections.Generic;
using Sam.Domain.Entity;



namespace Sam.ServiceInfraestructure
{
    public interface IChamadoSuporteService : ICrudBaseService<ChamadoSuporteEntity>
    {
        IList<ChamadoSuporteEntity> SelecionarChamados(Enum tipoPesquisa, long linhaRegistroID);
        ChamadoSuporteEntity ObterChamadoSuporte(int chamadoSuporteID);
        int ObterNumeroChamados();
    }
}
