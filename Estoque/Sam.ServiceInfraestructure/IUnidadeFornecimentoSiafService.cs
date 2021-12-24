using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;


namespace Sam.ServiceInfraestructure
{
    public interface IUnidadeFornecimentoSiafService : ICatalogoBaseService, ICrudBaseService<UnidadeFornecimentoEntity>
    {
        IList<UnidadeFornecimentoSiafEntity> Listar();
        IList<UnidadeFornecimentoSiafEntity> ObterUnidadeFornecimentoSiafisico(int iCodigo);
        IList<UnidadeFornecimentoSiafEntity> ObterUnidadeFornecimentoSiafisico(string strDescricao);
    }
}
