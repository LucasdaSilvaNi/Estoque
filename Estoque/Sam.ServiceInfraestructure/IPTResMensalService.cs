using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using System.Linq.Expressions;


namespace Sam.ServiceInfraestructure
{
    public interface IPTResMensalService : ICatalogoBaseService,ICrudBaseService<PTResMensalEntity>
    {
        PTResMensalEntity getRetornaPtResMensalParaConsumo(int almoxarifadoId, int anoMesReferencia, int uaId, int ptResId, int[] naturezaDespesa, decimal ptResMensalValor);
        IList<PTResMensalEntity> ProcessarNLsConsumoAlmox(int gestorId, int anoMesRef, int? almoxId);
        IList<PTResMensalEntity> ProcessarNLsConsumoImediato(int almoxID, int anoMesRef, int orgaoId, int idPerfil);
        PTResMensalEntity ObterNLConsumoPaga(int almoxID, int uaID, int ptresID, int natDespesaID, int anoMesRef, decimal valorNotaConsumo);
        Tuple<string, bool> ObterNLConsumoAlterada(PTResMensalEntity ptresMensal);
        IList<PTResMensalEntity> ObterNLsConsumoPagas(int almoxID, int anoMesRef, bool retornaNLEstornadas = false);
        IList<PTResMensalEntity> ObterNLsConsumoNulas(int almoxID, int anoMesRef, bool retornaNLEstornadas = false);
        PTResMensalEntity ObterUltimaNLConsumoRegistradaParaAgrupamento(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, decimal valorNotaConsumo, bool consideraValorNotaConsumo = false, bool verificaStatusAtivoMovimentacoes = false);
        IList<PTResMensalEntity> ObterUltimasNLConsumoRegistradasParaAgrupamento(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, bool verificaStatusAtivoMovimentacoes = false);
        //Tuple<string, bool> AtualizarPTResNL(PTResMensalEntity ptresMensal);
        bool StatusMovimentacao_Por_MovimentoItem(IList<string> listaMovimentoItemIDs);
        IList<PTResMensalEntity> ListarNLsConsumo(Expression _expWhere);
    }
}
