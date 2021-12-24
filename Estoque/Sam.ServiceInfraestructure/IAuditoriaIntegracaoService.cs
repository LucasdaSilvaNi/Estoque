using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IAuditoriaIntegracaoService : ICrudBaseService<AuditoriaIntegracaoEntity>
    {
        bool InserirRegistro(AuditoriaIntegracaoEntity entidadeAuditoria);
        AuditoriaIntegracaoEntity ObterEventoAuditoria(int movimentacaoMaterialID);
    }
}
