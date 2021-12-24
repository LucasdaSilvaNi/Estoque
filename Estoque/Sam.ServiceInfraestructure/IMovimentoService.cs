using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using System.Linq.Expressions;

namespace Sam.ServiceInfraestructure
{
    public interface IMovimentoService: ICatalogoBaseService,ICrudBaseService<MovimentoEntity>
    {
        ///Depois que resolver o problema de integração remover esse método
        int GetOrgaoPorUge(int ugeId);
        IList<MovimentoEntity> Listar(int MovimentoId);
        IList<MovimentoEntity> Listar(string Documento);
        MovimentoEntity ListarSolicitacaoMaterial(int UA, int divisao);
        IList<MovimentoEntity> Imprimir(int MovimentoId);
        MovimentoEntity ObterMovimentacao(int MovimentoId, bool isEmpenho = false);      
        string NumeroUltimoDocumento();
        IList<MovimentoEntity> ListarTodosCodSimplif(MovimentoEntity mov);
        IList<MovimentoEntity> ListarRequisicao(int startRow, int maximoRow, string orgaoId = "", string uoId = "", string ugeId = "", string uaId = "", string divisaoId = "", int statusId = 0, string numeroDocumento = "");
        IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef);
        IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef, bool requisicoesParaEstorno, string dataDigitada);
        IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int tipoMovimento, string dataDigitada);        
        IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE();
        int ListarDocumentoByDocTransf(string numDocumento);
        IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE(bool docsParaRetorno);
        bool VerificarMovimentoEntradaAnterior(MovimentoItemEntity movItem);        
        MovimentoEntity SalvarSaida();
        Tuple<string, bool> AtualizarMovimentoSubItem(MovimentoItemEntity movItem);
        Tuple<string, bool> AtualizarMovimentoBloquear(int MovId, bool bloquear);
        MovimentoEntity ListarDocumentosRequisicaoById(int id);

        MovimentoEntity GravarMovimento();
        MovimentoEntity GravarMovimentoConsumo();
        MovimentoEntity gravarMovimentoEntrada();
        bool ExisteSaldo(MovimentoItemEntity movItem);
        int ExisteSaldoId(MovimentoItemEntity movItem);
        bool ExisteSaldoLote(MovimentoItemEntity movItem, int IdSaldo);
        decimal CalcularPrecoMedioSaldo(decimal? SaldoValor, decimal? quantidade, String AnoMesReferencia);
        Decimal GetSaldoQuantidade(MovimentoItemEntity movItem);
        void RecalcularPrecoMedioSaldo(MovimentoItemEntity movItem);
        void AtualizarSaldo(MovimentoItemEntity movItem, bool somarSaldo);
        void InserirAtualizarSaldoLoteItemCorrigir(MovimentoItemEntity movItem, int? idSubItem);
        void AtualizarSaldoLote(MovimentoItemEntity movItem, int idSaldo);
        void InserirSaldo(MovimentoItemEntity movItem);
        void InserirAtualizarSaldoLoteItem(MovimentoItemEntity movItem, bool somarSaldo, int idSubItem);

        bool VerificarSaldoPositivo(MovimentoItemEntity movItem);
        MovimentoEntity GetMovimento();
        void AtualizarMovimentoEstorno(int loginIdEstornante, string InscricaoCE);
        decimal? CalcularDesdobro(decimal? precoAcum, decimal? precoMedio, decimal? qtdeAcum);
        void GravarMovimentoItemHistorico(MovimentoItemEntity movItem);
        void GravarMovimentoHistorico(MovimentoEntity pObjMovimento);

        void AtualizarMovimentoValorDocumento(MovimentoEntity movimento, decimal valorDocumento);
        Decimal getPrecoUnitarioSaldo(MovimentoItemEntity movItem);
        MovimentoItemEntity RetornaMovimentoEntradaDaSaida(MovimentoItemEntity movItem);
        decimal? AtualizarDesdobroSaldo(MovimentoItemEntity movimento, bool alterarSaldo);
        void AtualizarMovimentoItem(MovimentoItemEntity movItem);        
        void CalcularSaldoTotal(MovimentoItemEntity movItem);
        List<SaldoSubItemEntity> GetSaldo(MovimentoItemEntity movItem);
        decimal? CalcularDesdobro(List<SaldoSubItemEntity> saldoItens);
        MovimentoItemEntity ConsultaSaldoMovimentoItem(MovimentoItemEntity movItem);
        MovimentoItemEntity ConsultaSaldoMovimentoItemLote(MovimentoItemEntity movItem);
        decimal? SaldoMovimentoItemLote(int? idSubItem, int? idAlmoxarifado, DateTime dataMovimento, string lote, DateTime? dataLoteVenc, int? ugeId);
        IList<MovimentoEntity> ListarRequisicaoByDivisao(int divisaoId);
        IList<MovimentoEntity> ListarRequisicaoByExpression(Expression<Func<MovimentoEntity, bool>> _where);
        IList<MovimentoEntity> ListarNotasdeFornecimento(Expression<Func<MovimentoEntity, bool>> _where);
        IList<MovimentoEntity> ListarRequisicaoPendente(string palavraChave, int tipoMovimento, int idAlmox, DateTime dataDigitada);
        MovimentoEntity ListarDocumentosRequisicaoByDocumento(string numeroDocumento);        
        void SalvarRequisicao(bool pBlnNewMethod);
        List<MovimentoEntity> GetMovimentos();
        bool VerificarSaldoPositivoMovimento(MovimentoItemEntity movItem);
        IList<MovimentoEntity> ListarNFsPorEmpenho(string pStrMovimentoEmpenho);
        List<MovimentoEntity>  ListarMovimentosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId, string pStrAnoMesReferencia);
        List<MovimentoEntity>  ListarMovimentosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId);
        IList<string>          ListarEmpenhosDeMovimentosJaLiquidados(int pIntUgeId, int pIntAlmoxarifadoId);
        void CancelarRequisicao(int idRequisicao);
        bool VerificarRetroativo(MovimentoItemEntity movItem);
        bool VerificarRetroativoNovoItem(MovimentoItemEntity movItem);
        bool VerificarEntrada(MovimentoItemEntity movItem);
        bool VerificarSeEstoqueZerado(MovimentoItemEntity movItem);
        bool VerificarRetroativoEstorno(MovimentoItemEntity movItem);  
        bool VerificarRetroativoEstornoBloqueio(MovimentoItemEntity movItem);  
        MovimentoItemEntity RetornaPrecoMedioMovimentoItemRetroativo(MovimentoItemEntity movItem);
        MovimentoItemEntity RetornaPrecoMedioMovimentoItemRetroativoLote(MovimentoItemEntity movItem);
        IList<MovimentoItemEntity> ListMovimentoItemLoteAgrupamento(MovimentoItemEntity movItem);
        bool PermitirEstornarEntrada(MovimentoItemEntity movItem);
        IList<MovimentoEntity> VerificarTransferenciasPendentes(int pIntAlmoxarifadoFechamento, int pIntMesReferencia, bool NewMethod);
        IList<MovimentoEntity> Listar(System.Linq.Expressions.Expression<Func<MovimentoEntity, bool>> where);        
        IList<MovimentoItemEntity> ListarMovimentacaoItemPorIdEstorno(MovimentoItemEntity movItem, bool isEstorno);
        IList<MovimentoItemEntity> RetornaListaTodosMovimentos(MovimentoItemEntity movItem,Boolean estorno);
        IList<MovimentoItemEntity> RetornaListaTodosMovimentosLote(MovimentoItemEntity movItem, Boolean estorno);
        void AtualizaTransferencia(int? movimentoSaidaId, int? movimentoEntradaId, bool isEstorno);
        bool VerificaMovimentoModificaPrecoMedio(MovimentoItemEntity movItem, bool isEstorno);
        decimal? GetValorDocumento(int movimentoId);
        IList<MovimentoEntity> ImprimirConsultaTransferencia(int _almoxId, DateTime _dataInicial, DateTime _dataFinal);
        DateTime? primeiraDataMovimentoDoSubItemDoAlmoxarifado(Int32 almoxarifadoId, Int32 ugeId, Int32 subItemId);
        DateTime? primeiraDataMovimentoDoSubItemDoAlmoxarifado(Int32 almoxarifadoId, Int32 ugeId, Int32 subItemId, string IdentLote);
        IList<Int32> retornaIdDoSubItem(Int64 subItemCodigo);
        IList<Int32> retornaIdDoSubItem(Int64 subItemCodigo, int gestorId);
        IList<Int32> retornaIdDoSubItemPorAlmoxarifado(Int32 almoxarifadoId);
        Int32? retornaUgeDoAlmoxarifado(Int32 almoxarifadoId);
        void AtualizarSaldoMovimentoDoSubItem(MovimentoItemEntity movItem);
        decimal ConsultarSaldoTotalAlmoxarifado(int idAlmoxarifado);
        decimal ConsultarSaldoTotalAlmoxarifado33(int idAlmoxarifado);
        decimal ConsultarSaldoTotalAlmoxarifado44(int idAlmoxarifado);
        IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorMovimentoPendente(int GestorId);
        Int32 getTipoMovimento(Int32 movimentoId);
        #region Empenhos

        IList<MovimentoEntity> ListarMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true);
        IList<MovimentoEntity> ListarMovimentacoesEmpenhoAgrupadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true);

        IList<string> ListarEmpenhosFornecedor(int almoxID, int fornecedorID, string anoMesRef, bool empenhosNaoLiquidados = true);
        IList<string> ListarDocumentosEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true);

        IList<string> obterListaEmpenhosLiquidados(int almoxID, string almoxAnoMesRef);
        IList<string> obterListaEmpenhosNaoLiquidados(int almoxID, string almoxAnoMesRef);
        IList<string> ObterNLsPagamentoEmpenho(int almoxID, string anoMesRef, string codigoEmpenho, bool detalharNLs = false);
        IList<string> ObterNLsPagamentoMovimentoEmpenho(int movimentacaoEmpenhoID);

        UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiafisico(int codigoUnidFornSiafisico);
        UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiafisico(string siglaUnidFornSiafisico);
        #endregion Empenhos

        bool ExisteNotaSiafemVinculada(int movimentoID, bool verificaEstornoNotaSIAF = false);
        MovimentoEntity ExisteCodigoInformadoDados();
        MovimentoEntity ExisteCodigoConsumoDados();
        void AtualizaMovimentacaoCampoCE(int movimentacaoMaterialID, string valorCampoCE);
        bool ExecutarIntegracao(int movimentoId, bool estorno, bool consulta);
        MovimentoEntity GetMovimentacaoConsumoImediato(int movimentacaoMaterialID);
        void SetEntradaPorEmpenho();

        bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId);
        bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId, out int? numeroRequisicoesPendentes);
    }
}
