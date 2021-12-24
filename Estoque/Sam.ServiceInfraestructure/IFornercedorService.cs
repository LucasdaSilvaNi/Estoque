using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IFornecedorService : ICatalogoBaseService, ICrudBaseService<FornecedorEntity>
    {
        IList<FornecedorEntity> ListarFornecedorPorPalavraChave(string Chave);
        IList<FornecedorEntity> ListarFornecedorPorPalavraChave(string strChavePesquisa, bool blnConsultaOtimizada);
        IList<FornecedorEntity> ListarFornecedorPorPalavraChaveTodosCod(string Chave);

        IList<FornecedorEntity> ListarFornecedorComEmpenhosPendentes(int almoxID, string anoMesRef);
        FornecedorEntity LerRegistro(int pIntFornecedorId);
    }
}
