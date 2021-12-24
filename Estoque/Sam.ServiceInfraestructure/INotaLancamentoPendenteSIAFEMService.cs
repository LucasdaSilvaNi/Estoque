using System;
using System.Collections.Generic;
using Sam.Domain.Entity;


namespace Sam.ServiceInfraestructure
{
    public interface INotaLancamentoPendenteSIAFEMService : ICrudBaseService<NotaLancamentoPendenteSIAFEMEntity>
    {
        bool InserirRegistro(NotaLancamentoPendenteSIAFEMEntity pendenciaNotaLancamento);


        IList<NotaLancamentoPendenteSIAFEMEntity> ListarpendenciaSiafemPorAlmox(Enum tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true);
        NotaLancamentoPendenteSIAFEMEntity ObterNotaLancamentoPendente(int notaLancamentoPendenteID);
        NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true);
        //NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, string anoMesRef = null, bool movimentacaoEstornada = true);
        NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, bool consideraTambemMovimentacaoEstornada, string anoMesRef = null);
        IList<NotaLancamentoPendenteSIAFEMEntity> ObterPendenciasParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true);
        bool ExistePendenciaSiafemParaMovimentacao(int movimentacaoMaterialID, bool? pendenciaAtiva = true);
        bool InativarPendencia(int notaLancamentoPendencia);
        bool InativarPendenciasPorMovimentacao(int movimentacaoMaterialID);

        //NotaLancamentoPendenteSIAFEMEntity ExistePendenciaSiafemParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, string anoMesRef = null, bool movimentacaoEstornada = true);
        NotaLancamentoPendenteSIAFEMEntity ExistePendenciaSiafemParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, bool consideraTambemMovimentacaoEstornada, string anoMesRef = null);
        IList<NotaLancamentoPendenteSIAFEMEntity> ListarpendenciaSiafemPorAlmoxarifados(Enum tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true);
    }
}
