using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Transactions;
using Sam.Domain.Infrastructure;
using System.Linq;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoLancamentoSIAFEM = Sam.Common.Util.GeralEnum.TipoLancamentoSIAF;
using System.Diagnostics;




namespace Sam.Domain.Business
{
    //public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    public partial class PTResMensalBusiness : BaseBusiness
    {
        #region PTResMensal

        private PTResMensalEntity pTResMensal = new PTResMensalEntity();    
        public PTResMensalEntity PTResMensal
        {
            get { return pTResMensal; }
            set { pTResMensal = value; }
        }


        public Tuple<string, bool> AtualizarPTResM(PTResMensalEntity ptresMensal)
        {
            try
            {

                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    Tuple<string, bool> retorno = this.Service<IPTResMensalService>().ObterNLConsumoAlterada(ptresMensal);

                    tras.Complete();

                    return retorno;
                }
            }
            catch (Exception e)
            {
                return Tuple.Create(e.Message, false);

            }
        }

        public bool SalvarPTResMensal()
        {
            this.Service<IPTResMensalService>().Entity = this.PTResMensal;
            this.ConsistirPTResMensal();
            
            if (this.Consistido)
                this.Service<IPTResMensalService>().Salvar();

            this.AtualizarItensMovimentacaoComNotaNLCONSUMO();

            return this.Consistido;
        }

        private void AtualizarItensMovimentacaoComNotaNLCONSUMO()
        {
            try 
            {
                string[] arrIDs = null;
                string nlConsumo = null;
                IList<int> movItemIDs = null;
                var svcMovimentoItem = new MovimentoItemInfrastructure();


                if (!String.IsNullOrWhiteSpace(this.PTResMensal.MovimentoItemIDs))
                {
                    arrIDs = this.PTResMensal.MovimentoItemIDs.BreakLine(new char[] { '\n' });
                    movItemIDs = new List<int>(arrIDs.Length);

                    arrIDs.ToList().ForEach(strId => movItemIDs.Add(Int32.Parse(strId)));
                }

                var tipoLancamentoSIAFEM = default(TipoLancamentoSIAFEM);
                if (this.PTResMensal.TipoLancamento == 'N')
                {
                    tipoLancamentoSIAFEM = TipoLancamentoSIAFEM.Normal;
                    nlConsumo = this.PTResMensal.NlLancamento; 
                }
                else if (this.PTResMensal.TipoLancamento == 'E')
                {
                    tipoLancamentoSIAFEM = TipoLancamentoSIAFEM.Estorno;
                    nlConsumo = null; 
                }

                svcMovimentoItem.AtualizarItensMovimentacaoComNotaSIAF(movItemIDs.ToArray(), nlConsumo, TipoNotaSIAFEM.NL_Consumo, tipoLancamentoSIAFEM);

            }
            catch(Exception excErroGravacaoDados)
            {
                this.ListaErro.Add(String.Format("Erro ao atualizar itens de material, NL Consumo {0}.: {1}", this.PTResMensal.NlLancamento, excErroGravacaoDados.Message));
            }
        }

        public bool ExcluirPTResMensal()
        {
            this.Service<IPTResMensalService>().Entity = this.pTResMensal;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IPTResMensalService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirPTResMensal()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<PTResMensalEntity>(ref this.pTResMensal);

            if (this.pTResMensal.PtRes == null || !this.PTResMensal.PtRes.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o PTRes referente a este registro.");

            if (this.pTResMensal.UA == null || !this.PTResMensal.UA.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UA referente a este registro.");

            if (this.pTResMensal.UGE == null || !this.PTResMensal.UGE.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UGE referente a este registro.");

            if (this.pTResMensal.UgeAlmoxarifado == null || !this.PTResMensal.UgeAlmoxarifado.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UGE do almoxarifado referente a este registro.");

            if (this.pTResMensal.AnoMesRef == 0)
                this.ListaErro.Add("É obrigatório informar o ano de referência para este registro.");

            if (this.ListaErro.HasElements())
                this.ListaErro.Insert(0, "Erro ao gravar registro PTRes.");
        }
        
        public IList<PTResMensalEntity> ListarPTResMensal()
        {
            this.Service<IPTResMensalService>().SkipRegistros = this.SkipRegistros;
            IList<PTResMensalEntity> retorno = this.Service<IPTResMensalService>().Listar();
            this.TotalRegistros = this.Service<IPTResMensalService>().TotalRegistros();
            return retorno;
        }
        public IList<PTResMensalEntity> ObterMassaDadosParaGeracaoNLConsumo(int gestorId, int anoMesRef, int? almoxId = null)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {

                    return this.Service<IPTResMensalService>().ProcessarNLsConsumoAlmox(gestorId, anoMesRef, almoxId);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<PTResMensalEntity> ObterMassaDadosParaGeracaoNLConsumoImediato(int almoxID, int anoMesRef, int orgaoId, int idPerfil)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {

                    return this.Service<IPTResMensalService>().ProcessarNLsConsumoImediato(almoxID, anoMesRef, orgaoId, idPerfil);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<PTResMensalEntity> ObterNLsConsumoPagas(int almoxID, int anoMesRef, bool retornaEstornadas = false)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterNLsConsumoPagas(almoxID, anoMesRef, retornaEstornadas);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }
        public IList<PTResMensalEntity> ObterNLsConsumoNulas(int almoxID, int anoMesRef, bool retornaEstornadas = false)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterNLsConsumoNulas(almoxID, anoMesRef, retornaEstornadas);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public PTResMensalEntity ObterUltimaNLConsumoRegistradaParaAgrupamento(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, decimal valorNotaConsumo, bool consideraValorNotaConsumo = false, bool verificaStatusAtivoMovimentacoes = false)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterUltimaNLConsumoRegistradaParaAgrupamento(almoxID, uaID, PTResID, naturezaDespesaID, anoMesRef, valorNotaConsumo);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }
        public PTResMensalEntity ObterNLConsumoPaga(int almoxID, int anoMesRef, int uaID, int ptresID, int natDespesaID, decimal valorNotaConsumo)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterNLConsumoPaga(almoxID, anoMesRef, uaID, ptresID, natDespesaID, valorNotaConsumo);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }
        public PTResMensalEntity getRetornaPtResMensalParaConsumo(int almoxarifadoId, int anoMesReferencia, int uaId, int ptResId, int[] naturezaDespesa, decimal ptResMensalValor)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().getRetornaPtResMensalParaConsumo(almoxarifadoId, anoMesReferencia, uaId, ptResId, naturezaDespesa, ptResMensalValor);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        #endregion PTResMensal

        #region Consolidacao Massa NLConsumo
        public bool StatusMovimentacao_Por_MovimentoItem(IList<string> listaMovimentoItemIDs)
        {
            //using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            using (TransactionScope transScope = base.ObterConfiguracoesPadraoDeConsulta())
            {
                try
                {
                    return this.Service<IPTResMensalService>().StatusMovimentacao_Por_MovimentoItem(listaMovimentoItemIDs);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    transScope.Complete();
                }
            }
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoAlmox(int gestorId, int anoMesRef, int? almoxId = null)
        {
            IList<PTResMensalEntity> lstNotasGeradasParaConsumo = null;
            List<PTResMensalEntity> listaGeral = null;

            try
            {
                listaGeral = new List<PTResMensalEntity>();
                lstNotasGeradasParaConsumo = this.ObterMassaDadosParaGeracaoNLConsumo(gestorId, anoMesRef, almoxId);
                listaGeral.AddRange(lstNotasGeradasParaConsumo);

                listaGeral = listaGeral.OrderBy(nlConsumo => nlConsumo.UA.Codigo.Value)
                                       .ThenBy(nlConsumo => nlConsumo.PtRes.Codigo.Value)
                                       .ThenBy(nlConsumo => nlConsumo.NaturezaDespesa.Codigo)
                                       .ToList();
            }
            catch (Exception ex)
            {
                this.ListaErro = new List<string>() { ex.Message };
            }


            return listaGeral;
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoImediato(int almoxID, int anoMesRef, int orgaoId, int idPerfil)
        {
            IList<PTResMensalEntity> lstNotasGeradasParaConsumo = null;
            List<PTResMensalEntity> listaGeral = null;


            listaGeral = new List<PTResMensalEntity>();
            lstNotasGeradasParaConsumo = this.ObterMassaDadosParaGeracaoNLConsumoImediato(almoxID, anoMesRef, orgaoId, idPerfil);
            listaGeral.AddRange(lstNotasGeradasParaConsumo);

            listaGeral = listaGeral.OrderBy(nlConsumo => nlConsumo.UA.Codigo)
                                   .ThenBy(nlConsumo => nlConsumo.PtRes.Codigo)
                                   .ThenBy(nlConsumo => nlConsumo.NaturezaDespesa.Codigo)
                                   .ToList();

            return listaGeral;
        }

        private string ObterNumerosDocumentoPorMovimentoItemIDs(IList<int> movItems)
        {
            string docsSAM = null;
            MovimentoItemBusiness objBusiness = null;



            objBusiness = new MovimentoItemBusiness();
            docsSAM = objBusiness.ObterNumerosDocumentoPorMovimentoItemIDs(movItems);


            return docsSAM;
        }

        private PTResMensalEntity obterUltimaNotaRegistradaParaAgrupamento(PTResMensalEntity _notaGerada, bool consideraValorNotaConsumo = false, bool verificaStatusAtivoMovimentacoes = false)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterUltimaNLConsumoRegistradaParaAgrupamento(_notaGerada.Almoxarifado.Id.Value, _notaGerada.UA.Id.Value, _notaGerada.PtRes.Id.Value, _notaGerada.NaturezaDespesa.Id.Value, _notaGerada.AnoMesRef, _notaGerada.Valor.Value, consideraValorNotaConsumo, verificaStatusAtivoMovimentacoes);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        private IList<PTResMensalEntity> obterUltimasNotasRegistradasParaAgrupamento(PTResMensalEntity notaGerada)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IPTResMensalService>().ObterUltimasNLConsumoRegistradasParaAgrupamento(notaGerada.Almoxarifado.Id.Value, notaGerada.UA.Id.Value, notaGerada.PtRes.Id.Value, notaGerada.NaturezaDespesa.Id.Value, notaGerada.AnoMesRef);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        private IList<PTResMensalEntity> NLConsumoSemelhantes(PTResMensalEntity nlConsumoGerada, IList<PTResMensalEntity> listaParaConsulta)
        {
            IList<PTResMensalEntity> NLsSemelhantes = null;


            NLsSemelhantes = new List<PTResMensalEntity>();

            listaParaConsulta.ToList().ForEach(notaConsultada => { 
                                                                    if(notaConsultada.UA.Codigo              == nlConsumoGerada.UA.Codigo &&
                                                                       notaConsultada.PtRes.Codigo           == nlConsumoGerada.PtRes.Codigo &&
                                                                       notaConsultada.NaturezaDespesa.Codigo == nlConsumoGerada.NaturezaDespesa.Codigo)
                                                                          NLsSemelhantes.Add(notaConsultada); 
                                                                 });

            //NLsSemelhantes = NLsSemelhantes.OrderByDescending(notaConsumo => notaConsumo.Id.Value)
            NLsSemelhantes = NLsSemelhantes.OrderBy(nlConsumo => nlConsumo.UA.Codigo.Value)
                                           .ThenBy(nlConsumo => nlConsumo.PtRes.Codigo.Value)
                                           .ThenBy(nlConsumo => nlConsumo.NaturezaDespesa.Codigo)
                                           .DistinctBy(nlConsumo => new { nlConsumo.NlLancamento, nlConsumo.Valor })
                                           .ToList();

            return NLsSemelhantes;
        }
        private bool PossuiNLConsumoSemelhante(PTResMensalEntity nlConsumoGerada, IList<PTResMensalEntity> listaParaConsulta)
        {
            bool NLsParaRecalculo = false;
            PTResMensalEntity nlConsumoSemelhante = null;

            listaParaConsulta.ToList().ForEach(notaConsultada => {
                                                                    if (notaConsultada.UA.Codigo == nlConsumoGerada.UA.Codigo &&
                                                                        notaConsultada.PtRes.Codigo == nlConsumoGerada.PtRes.Codigo &&
                                                                        notaConsultada.NaturezaDespesa.Codigo == nlConsumoGerada.NaturezaDespesa.Codigo)
                                                                    {
                                                                        NLsParaRecalculo = true;
                                                                        nlConsumoSemelhante = nlConsumoGerada;
                                                                        return;
                                                                    }
                                                                 });

            return NLsParaRecalculo;
        }
        private bool NLConsumoIguaisComValoresDiferentes(PTResMensalEntity nlConsumoGerada, PTResMensalEntity nlConsumoPaga)
        {
            bool NLsComValorDivergente = false;

            NLsComValorDivergente = (nlConsumoGerada.UA.Codigo == nlConsumoPaga.UA.Codigo &&
                                     nlConsumoGerada.PtRes.Codigo == nlConsumoPaga.PtRes.Codigo &&
                                     nlConsumoGerada.NaturezaDespesa.Codigo == nlConsumoPaga.NaturezaDespesa.Codigo &&
                                     //nlConsumoGerada.TipoLancamento == nlConsumoGerada.TipoLancamento &&
                                     nlConsumoGerada.Valor != nlConsumoPaga.Valor);

            return NLsComValorDivergente;
        }
        private bool NLConsumoIguais(PTResMensalEntity nlConsumoGerada, PTResMensalEntity nlConsumoPaga, bool desconsideraTipoLancamento = true)
        {
            bool NLsEspelho = false;

            NLsEspelho = (nlConsumoGerada.UA.Codigo == nlConsumoPaga.UA.Codigo &&
                          nlConsumoGerada.PtRes.Codigo == nlConsumoPaga.PtRes.Codigo &&
                          nlConsumoGerada.NaturezaDespesa.Codigo == nlConsumoPaga.NaturezaDespesa.Codigo &&
                          nlConsumoGerada.Valor == nlConsumoPaga.Valor);

            if (!desconsideraTipoLancamento)
                NLsEspelho &= (nlConsumoGerada.TipoLancamento == nlConsumoGerada.TipoLancamento);

            return NLsEspelho;
        }
        private bool EhParPagamentoEstorno_NLConsumo(PTResMensalEntity nlConsumoGerada, PTResMensalEntity nlConsumoPaga)
        {
            bool NLsComValorDivergente = false;

            NLsComValorDivergente = (nlConsumoGerada.UA.Codigo == nlConsumoPaga.UA.Codigo &&
                                     nlConsumoGerada.PtRes.Codigo == nlConsumoPaga.PtRes.Codigo &&
                                     nlConsumoGerada.NaturezaDespesa.Codigo == nlConsumoPaga.NaturezaDespesa.Codigo &&
                                     nlConsumoGerada.Valor == nlConsumoPaga.Valor &&
                                     nlConsumoGerada.TipoLancamento != nlConsumoPaga.TipoLancamento);

            return NLsComValorDivergente;
        }

        #endregion
    }

    public static class PTResMensalBusinessExtensions
    {
        public static bool StatusMovimentacao_Por_MovimentoItem(this PTResMensalEntity nlConsumo)
        {
            bool blnRetorno = false;
            PTResMensalBusiness objBusiness = null;


            objBusiness = new PTResMensalBusiness();
            if (nlConsumo.IsNotNull())
                blnRetorno = objBusiness.StatusMovimentacao_Por_MovimentoItem(nlConsumo.MovimentoItemIDs.BreakLine());


            return blnRetorno;
        }

        public static string ObterNumerosDocumentoPorMovimentoItemIDs(this PTResMensalEntity nlConsumo)
        {
            string docsSAM = null;
            IList<int> movItemIDs = null;
            MovimentoItemBusiness objBusiness = null;



            objBusiness = new MovimentoItemBusiness();
            movItemIDs = new List<int>();
            nlConsumo.MovimentoItemIDs.BreakLine().ToList().ForEach(movItemID => movItemIDs.Add(int.Parse(movItemID)));
            docsSAM = objBusiness.ObterNumerosDocumentoPorMovimentoItemIDs(movItemIDs);


            return docsSAM;
        }
    }
}
