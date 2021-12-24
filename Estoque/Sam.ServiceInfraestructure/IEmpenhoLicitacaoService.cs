using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IEmpenhoLicitacaoService : ICatalogoBaseService, ICrudBaseService<EmpenhoLicitacaoEntity>
    {
        EmpenhoLicitacaoEntity LerRegistro();
        EmpenhoLicitacaoEntity LerRegistro(MovimentoEntity pObjMovimento);
        EmpenhoLicitacaoEntity LerTipoLicitacaoDoMovimento(int pIntMovimentoID);
        EmpenhoLicitacaoEntity Listar(int _id);
        EmpenhoLicitacaoEntity ObterTipoLicitacao(int pIntLicitacaoID);
        IList<EmpenhoLicitacaoEntity> Listar(System.Linq.Expressions.Expression<Func<EmpenhoLicitacaoEntity, bool>> where);
    }
}
