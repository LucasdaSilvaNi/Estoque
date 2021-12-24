using System.Collections.Generic;
using Sam.Entity;



namespace Sam.ServiceInfraestructure
{
    public interface IUnidadeFornecimentoConversaoService : ICatalogoBaseService, ICrudBaseService<UnidadeFornecimentoConversaoEntity>
    {
        IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversaoPorGestor(int? Gestor_ID);
        IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversao();
        UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversaoPorID(int UnidadeFornecimentoConversao_ID);
        UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversaoPorCodigo(string strCodigoUnidadeConversao);
        IList<UnidadeFornecimentoConversaoEntity> Imprimir(int GestorId);
    }
}
