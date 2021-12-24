using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Transactions;
using Sam.Common;
using Sam.Domain.Entity.Relatorios;
using System.Diagnostics;
using Sam.Integracao.SIAF.Core;


namespace Sam.Domain.Business
{
    public class FechamentoMensalBusiness : BaseBusiness
    {
        private FechamentoMensalEntity fechamento = new FechamentoMensalEntity();

        public FechamentoMensalEntity Fechamento
        {
            get { return fechamento; }
            set { fechamento = value; }
        }

        public bool EstornarFechamentoMensal()
        {
            try
            {
                string lStrDataTratada = string.Empty;
                bool mesFiscalFechadoSIAFEM = true;
                this.Service<IFechamentoMensalService>().Entity = Fechamento;

                this.Fechamento.AnoMesRef = ListarUltimoFechamento();
                mesFiscalFechadoSIAFEM = statusFechadoMesReferenciaSIAFEM();

                if (Fechamento.AnoMesRef == null)
                {
                    this.ListaErro.Add("Não consta fechamento para reabertura!");
                    return false;
                }
                //else if (!mesFiscalFechadoSIAFEM)
                else if (!mesFiscalFechadoSIAFEM || this.Fechamento.Almoxarifado.IgnoraCalendarioSiafemParaReabertura)
                {

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IFechamentoMensalService>().Entity = Fechamento;
                        this.Service<IFechamentoMensalService>().Excluir();
                    }

                    //atualiza o almoxarifado
                    string mesRef = TratamentoDados.ValidarAnoMesRef(Fechamento.AnoMesRef.Value.ToString(), 0);
                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IFechamentoMensalService>().AtualizarMesRefAlmoxarifadoFechamento(Fechamento.Almoxarifado.Id.Value, mesRef);
                    }

                    this.fechamento.AnoMesRef = Convert.ToInt32(mesRef);
                }
                else if (mesFiscalFechadoSIAFEM)
                {
                    var anoReferencia = Int32.Parse(this.Fechamento.AnoMesRef.Value.ToString().Substring(0, 4));
                    var mesReferencia = Int32.Parse(this.Fechamento.AnoMesRef.Value.ToString().Substring(4, 2));
                    var dataFechamento = this.Service<ICalendarioFechamentoMensalService>().ObterDataFechamentoMensal(mesReferencia, anoReferencia);

                    this.ListaErro.Add(String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2}.", mesReferencia, anoReferencia, dataFechamento.DataFechamentoDespesa.ToString("dd/MM/yyyy")));
                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("truncated"))
                    return true;

                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return false;
            }

            return true;
        }

        private bool statusFechadoMesReferenciaSIAFEM()
        {
            bool blnRetorno = true;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = null;

            try
            {
                var anoReferencia = Int32.Parse(this.Fechamento.AnoMesRef.Value.ToString().Substring(0, 4));
                var mesReferencia = Int32.Parse(this.Fechamento.AnoMesRef.Value.ToString().Substring(4, 2));

                objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
                blnRetorno = objBusinessCalendarioSIAFEM.StatusFechadoMesReferenciaSIAFEM(mesReferencia, anoReferencia);
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.ListaErro.Add("Módulo Fechamento: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
                return false;
            }


            return blnRetorno;
        }

        public int? ListarUltimoFechamento()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return this.Service<IFechamentoMensalService>().ListarUltimoFechamento(Fechamento.Almoxarifado.Id);
            }
        }

        public bool ContemFechamento(int idAlmox)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return this.Service<IFechamentoMensalService>().ContemFechamento(idAlmox);
            }
        }

        public List<FechamentoAnualEntity> GerarBalanceteAnual(int idAlmoxarifado, string mesrefAnoAnterior, string mesRefInicial, string mesRefFinal)
        {
            try
            {

                Infrastructure.FechamentoMensalInfrastructure fechamento = new Infrastructure.FechamentoMensalInfrastructure();
                return fechamento.GerarBalanceteAnual(idAlmoxarifado, mesrefAnoAnterior, mesRefInicial, mesRefFinal); ;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public bool SalvarFechamentoMensal(IList<SaldoSubItemEntity> lstSaldoSubItem, GeralEnum.SituacaoFechamento situacaoFechamento)
        {
            this.Service<IFechamentoMensalService>().Entity = Fechamento;
            PTResMensalBusiness ptResBusiness = new PTResMensalBusiness();

            IList<int> retorno = (from s in lstSaldoSubItem select s.SubItemMaterial.NaturezaDespesa.Id.Value).Distinct().ToList();

            int[] naturezaDespesa = new int[retorno.Count];

            foreach (int subItem in retorno)
            {
                naturezaDespesa[retorno.IndexOf(subItem)] = subItem;
            }

            //if (ptResBusiness.getRetornaPtResMensalParaConsumo(fechamento.Almoxarifado.Id.Value, fechamento.AnoMesRef.Value, 0, 0, naturezaDespesa, 0) != null)
            //{

            //    this.ListaErro.Add("Existe Nota de Consumo Pendente, não será possível realizar o fechamento");
            //    return false;
            //}
            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    IList<FechamentoMensalEntity> listaFechamento = MontarFechamentoMensal(lstSaldoSubItem);

                    if (ListaErro.Count > 0)
                        return false;

                    //Adiciona a situação do fechamento para todos os itens
                    foreach (var list in listaFechamento)
                    {
                        list.SituacaoFechamento = (int)situacaoFechamento;
                    }

                    if (situacaoFechamento == GeralEnum.SituacaoFechamento.Executar)
                    {
                        var result = SalvarFechamentoMensal(lstSaldoSubItem, listaFechamento);
                        tras.Complete();
                        return result;

                    }
                    else if (situacaoFechamento == GeralEnum.SituacaoFechamento.Simular)
                    {
                        var result = SalvarSimulacaoFechamentoMensal(lstSaldoSubItem, listaFechamento);
                        tras.Complete();
                        return result;
                    }
                    else
                    {
                        throw new Exception("A situação do fechamento não foi implementada.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("truncated"))
                    return true;

                throw new Exception(e.Message);
            }
        }

        private bool SalvarSimulacaoFechamentoMensal(IList<SaldoSubItemEntity> lstSaldoSubItem, IList<FechamentoMensalEntity> listaFechamento)
        {
            try
            {
                this.Service<IFechamentoMensalService>().Entity = Fechamento;

                // salva todo o fechamento mensal
                this.Service<IFechamentoMensalService>().Salvar(listaFechamento);

                if (ListaErro.Count > 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("truncated"))
                    return true;

                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return false;
            }
        }

        public void ExecutarFechamento(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {
            try
            {
                this.Service<IFechamentoMensalService>().ExecutarFechamento(AlmoxarifadoId, mesAnoReferencia, usuarioSamLoginId);

                if (this.Service<IFechamentoMensalService>().fechamentoErro != null && this.Service<IFechamentoMensalService>().fechamentoErro.Count > 0)
                    this.ListaErro.AddRange(this.Service<IFechamentoMensalService>().fechamentoErro);
                else
                {
                    //adianta um mês
                    string mesRef = TratamentoDados.ValidarAnoMesRef(mesAnoReferencia.ToString(), 1);
                    //Atualiza o mês ref do almoxarifado
                    this.Service<IFechamentoMensalService>().AtualizarMesRefAlmoxarifadoFechamento(AlmoxarifadoId, mesRef);
                }
            }
            catch (Exception ex)
            {
                this.ListaErro.Add(ex.Message);
            }
        }

        public void ExecutarSimulacao(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {

            try
            {
                this.Service<IFechamentoMensalService>().ExecutarSimulacao(AlmoxarifadoId, mesAnoReferencia, usuarioSamLoginId);
                if (this.Service<IFechamentoMensalService>().fechamentoErro != null && this.Service<IFechamentoMensalService>().fechamentoErro.Count > 0)
                    this.ListaErro.AddRange(this.Service<IFechamentoMensalService>().fechamentoErro);
            }
            catch (Exception ex)
            {

                this.ListaErro.Add(ex.Message);
            }

        }

        private bool SalvarFechamentoMensal(IList<SaldoSubItemEntity> lstSaldoSubItem, IList<FechamentoMensalEntity> listaFechamento)
        {
            try
            {
                this.Service<IFechamentoMensalService>().Entity = Fechamento;

                // *** NÃO IMPLEMENTAR POR ENQUANTO!
                //ConsistirMesAtual();

                if (ListaErro.Count > 0)
                {
                    return false;
                }

                //salva todo o fechamento mensal
                this.Service<IFechamentoMensalService>().Salvar(listaFechamento);

                //adianta um mês
                string mesRef = TratamentoDados.ValidarAnoMesRef(Fechamento.AnoMesRef.Value.ToString(), 1);

                //Atualiza o mês ref do almoxarifado
                this.Service<IFechamentoMensalService>().AtualizarMesRefAlmoxarifadoFechamento((int)Fechamento.Almoxarifado.Id, mesRef);

                if (ListaErro.Count > 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("truncated"))
                {
                    new LogErro().GravarLogErro(e);
                    this.ListaErro.Add("Erro no sistema: " + e.Message);
                    return false;
                }
                else
                    return true;
            }
        }

        protected void ConsistirMesAtual()
        {
            DateTime data;
            DateTime dataCorrente = DateTime.Now;
            if (Fechamento.AnoMesRef.HasValue)
            {
                data = new DateTime(Convert.ToInt32(Fechamento.AnoMesRef.ToString().Substring(0, 4)), Convert.ToInt32(Fechamento.AnoMesRef.ToString().Substring(4, 2)), 1);

                // verificar se o anomês do fechamento encontra-se na data atual. caso encontre. não permitirá o fechamento
                if (data.Month == dataCorrente.Month && data.Year == dataCorrente.Year)
                {
                    this.ListaErro.Add("O ano/mês de referência não pode ser fechado (ano/mês corrente ainda não foi fechado).");
                }
            }
        }

        public IList<FechamentoMensalEntity> ImprimirFechamentoMensal()
        {
            try
            {
                this.Service<IFechamentoMensalService>().Entity = this.Fechamento;
                return this.Service<IFechamentoMensalService>().Imprimir();
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return new List<FechamentoMensalEntity>();
            }
        }


        public IList<relInventarioFechamentoMensalEntity> ImprimirInventarioMensal(int almoxID, int anoMesRef)
        {
            try
            {
                this.Service<IFechamentoMensalService>().Entity = this.Fechamento;

                return this.Service<IFechamentoMensalService>()._xpImprimirInventarioBalanceteMensal(almoxID, anoMesRef);
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return new List<relInventarioFechamentoMensalEntity>();
            }
        }

        public IList<relAnaliticoFechamentoMensalEntity> ImprimirAnaliticoBalanceteMensal(int almoxID, int anoMesRef)
        {
            try
            {
                this.Service<IFechamentoMensalService>().Entity = this.Fechamento;

                return this.Service<IFechamentoMensalService>().ImprimirAnaliticoBalanceteMensal(almoxID, anoMesRef);
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return new List<relAnaliticoFechamentoMensalEntity>();
            }
        }

        private void ValidarFechamentoMensal(SaldoSubItemEntity saldoSubItem)
        {
            if (saldoSubItem.SubItemMaterial == null)
                throw new Exception("O SubItem está nulo");

            if (saldoSubItem.QtdeFechamento < 0)
                this.ListaErro.Add(String.Format("Estoque negativo no subitem: {0} - {1}. Saldo Quantidade: {2}", saldoSubItem.SubItemMaterialCodigo, saldoSubItem.SubItemMaterial.Descricao, saldoSubItem.QtdeFechamento.ToString()));

            if (saldoSubItem.ValFechamento < 0)
                this.ListaErro.Add(String.Format("Saldo negativo no subitem: {0} - {1}. Saldo Valor: {2}", saldoSubItem.SubItemMaterialCodigo, saldoSubItem.SubItemMaterial.Descricao, saldoSubItem.ValFechamento.ToString()));

            if (saldoSubItem.Almoxarifado == null)
                this.ListaErro.Add(String.Format("Não há almoxarifado associado ao saldo do subitem de material {0} - {1}.", saldoSubItem.SubItemMaterialCodigo, saldoSubItem.SubItemMaterial.Descricao));

            if (saldoSubItem.UGE == null)
                this.ListaErro.Add("Não há UGE associada ao saldo do subitem de material {0} - {1}");
        }

        public IList<FechamentoMensalEntity> MontarFechamentoMensal(IList<SaldoSubItemEntity> lstSaldoSubItem)
        {
            List<FechamentoMensalEntity> listaFecha = new List<FechamentoMensalEntity>();

            try
            {
                foreach (SaldoSubItemEntity fecha in lstSaldoSubItem)
                {
                    FechamentoMensalEntity fechamentoMes = new FechamentoMensalEntity();

                    ValidarFechamentoMensal(fecha);

                    if (fecha.Almoxarifado != null)
                        fechamentoMes.Almoxarifado = new AlmoxarifadoEntity(fecha.Almoxarifado.Id);

                    if (fecha.SubItemMaterial != null)
                        fechamentoMes.SubItemMaterial = new SubItemMaterialEntity(fecha.SubItemMaterial.Id.Value);

                    if (fecha.UGE != null)
                        fechamentoMes.UGE = new UGEEntity(fecha.UGE.Id);

                    fechamentoMes.AnoMesRef = Fechamento.AnoMesRef;
                    fechamentoMes.QtdeEntrada = fecha.QtdeEntrada;
                    fechamentoMes.QtdeSaida = fecha.QtdeSaida;
                    fechamentoMes.ValorEntrada = fecha.ValEntrada;
                    fechamentoMes.ValorSaida = fecha.ValSaida;
                    fechamentoMes.SaldoQtde = fecha.QtdeFechamento;
                    fechamentoMes.SaldoValor = fecha.ValFechamento;

                    listaFecha.Add(fechamentoMes);
                }
            }
            catch (Exception e)
            {
                this.ListaErro.Add(e.Message);
            }

            return listaFecha;
        }

        public bool ExcluirFechamento()
        {
            this.Service<IFechamentoMensalService>().Entity = this.Fechamento;

            try
            {
                this.Service<IFechamentoMensalService>().Excluir();
            }
            catch (Exception ex)
            {
                TratarErro(ex);
            }

            return this.Consistido;
        }

        public IList<FechamentoMensalEntity> Listar(int pIntAlmoxarifado, bool pBlnAgruparResultados)
        {
            IList<FechamentoMensalEntity> lista = new List<FechamentoMensalEntity>();
            try
            {
                lista = this.Service<IFechamentoMensalService>().Listar(pIntAlmoxarifado, pBlnAgruparResultados);
            }
            catch (Exception ex)
            {
                TratarErro(ex);
            }

            return lista;

        }

        public bool PodeExecutarPagamentoConsumoAlmox(int almoxID, bool consideraAcaoParaFechamento = false)
        {
            bool blnRetorno = false;
            int anoMesReferenciaAlmox = -1;
            int anoMesReferenciaDataHoraAtual = -1;
            DateTime anoMesReferenciaDataLimite = Convert.ToDateTime("09/02/2018");
            DateTime anoMesReferenciaDataAtual = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
            AlmoxarifadoEntity almoxConsultado = null;
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();


            almoxConsultado = objBusiness.ObterAlmoxarifado(almoxID);
            Int32.TryParse(almoxConsultado.MesRef, out anoMesReferenciaAlmox);
            Int32.TryParse(DateTime.Now.ToString("yyyyMM"), out anoMesReferenciaDataHoraAtual);
            

            if (anoMesReferenciaAlmox.ToString().Substring(4, 2) == "12")
            {
               if(anoMesReferenciaDataAtual <= anoMesReferenciaDataLimite)
                    return true;
               else
                anoMesReferenciaAlmox += 89;
            }
            else
                anoMesReferenciaAlmox++;

            blnRetorno = (anoMesReferenciaAlmox == anoMesReferenciaDataHoraAtual);
            if(consideraAcaoParaFechamento)
                blnRetorno = (anoMesReferenciaAlmox <= anoMesReferenciaDataHoraAtual);

            return blnRetorno;
        }

        public bool VerificaRestricaoFechamentoExercicioFiscal(int almoxID)
        {
            int anoMesReferenciaAlmox = -1;
            int anoMesReferenciaDataHoraAtual = -1;
            bool ehPeriodoFechamentoExercicioFiscal = false;
            AlmoxarifadoEntity almoxConsultado = null;
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();


            almoxConsultado = objBusiness.ObterAlmoxarifado(almoxID);
            Int32.TryParse(almoxConsultado.MesRef, out anoMesReferenciaAlmox);
            Int32.TryParse(DateTime.Now.ToString("yyyyMM"), out anoMesReferenciaDataHoraAtual);
            ehPeriodoFechamentoExercicioFiscal = ((almoxConsultado.MesRef.ToString().Substring(4, 2) == "12") && (DateTime.Now.Month == 01));

            return ehPeriodoFechamentoExercicioFiscal;
        }

        internal bool AlmoxarifadoContemFechamentos(int almoxarifadoId)
        {
            using (TransactionScope trScope = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return this.Service<IFechamentoMensalService>().ContemFechamento(almoxarifadoId);
            }
        }

        public bool VerificaRestricaoEntradaPorInventario(int almoxID)
        {
            bool ehHabilitadoEntradaPorInventario = false;
            AlmoxarifadoEntity almoxConsultado = null;
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();

            almoxConsultado = objBusiness.ObterAlmoxarifado(almoxID);
            ehHabilitadoEntradaPorInventario = (almoxConsultado.MesRef == almoxConsultado.RefInicial);

            return ehHabilitadoEntradaPorInventario;
        }




        public bool PodeExecutarFechamentoMensal(int almoxID)
        {
            return PodeExecutarPagamentoConsumoAlmox(almoxID, true);
        }

        public IList<string> ListarMesesFechados(int almoxId)
        {
            IList<string> lstRetorno = new List<string>();
            try
            {
                lstRetorno = this.Service<IFechamentoMensalService>().ListarMesesFechados(almoxId);
            }
            catch (Exception excErroConsulta)
            {
                Exception excErroParaPropagacao = new Exception("Erro ao consultar relação de fechamentos para almoxarifado corrente.", excErroConsulta);
                throw excErroParaPropagacao;
            }
            return lstRetorno;
        }

        #region WS SIAFNLCONSUMO

        public bool AtualizarAlmoxarifado(int pIntAlmoxarifadoLogadoID, int pIntGestorAlmoxarifadoLogadoID)
        {
            bool lBlnRetorno = true;

            AlmoxarifadoEntity lObjAlmoxarifadoLogado = null;
            EstruturaOrganizacionalBusiness lObjBusiness = new EstruturaOrganizacionalBusiness();

            int lIntMesReferenciaAlmoxarifado = 0;
            int lIntAnoReferenciaAlmoxarifado = 0;
            DateTime lDtDataAtual = new DateTime(0);


            lObjAlmoxarifadoLogado = lObjBusiness.ListarAlmoxarifadoPorGestorTodosCod(pIntGestorAlmoxarifadoLogadoID, true)
                                                 .Where(Almoxarifado => Almoxarifado.Id == pIntAlmoxarifadoLogadoID)
                                                 .FirstOrDefault();

            lIntMesReferenciaAlmoxarifado = TratamentoDados.TryParseInt32(lObjAlmoxarifadoLogado.MesRef.Substring(4, 2)).Value;
            lIntAnoReferenciaAlmoxarifado = TratamentoDados.TryParseInt32(lObjAlmoxarifadoLogado.MesRef.Substring(0, 4)).Value;
            lDtDataAtual = new DateTime(lIntAnoReferenciaAlmoxarifado, lIntMesReferenciaAlmoxarifado, 01);


            lObjAlmoxarifadoLogado.Descricao = lObjAlmoxarifadoLogado.Descricao.Substring(lObjAlmoxarifadoLogado.Descricao.IndexOf(" - ") + 3);
            lObjAlmoxarifadoLogado.MesRef = lDtDataAtual.ToString("yyyyMM");

            lObjBusiness.Almoxarifado = lObjAlmoxarifadoLogado;
            lBlnRetorno &= lObjBusiness.SalvarAlmoxarifado();

            return lBlnRetorno;
        }
        public bool ExcluirFechamento(FechamentoMensalEntity pFechamentoMensal)
        {
            bool lBlnRetorno = false;

            FechamentoMensalBusiness businessFechamentoMensal = new FechamentoMensalBusiness();

            businessFechamentoMensal.Fechamento = pFechamentoMensal;
            lBlnRetorno = businessFechamentoMensal.ExcluirFechamento();

            return lBlnRetorno;
        }
        public bool ExcluirPtResMensalNaoConsumidas()
        {
            bool lBlnStatusGravacao = true;

            PTResMensalBusiness lObjBusiness = null;
            IList<PTResMensalEntity> lILstPtResMensal = null;

            if (lILstPtResMensal.Count == 0)
                return false;

            lObjBusiness = new PTResMensalBusiness();
            lILstPtResMensal = lObjBusiness.ListarPTResMensal()
                                           .Where(a => a.FlagLancamento.ToString().ToUpperInvariant() == "N")
                                           .ToList();

            foreach (PTResMensalEntity lObjPtResMensal in lILstPtResMensal)
            {
                lObjBusiness.PTResMensal = lObjPtResMensal;
                lBlnStatusGravacao &= lObjBusiness.ExcluirPTResMensal();

                lObjBusiness.PTResMensal = null;
            }

            return lBlnStatusGravacao;

        }

        #endregion WS SIAFNLCONSUMO

        #region WS SIAFNLCONSUMO (refactored code)

        /// <summary>
        /// Pagamento (ou estorno) de NL's em lote, a ser gerada com base no banco de dados.
        /// </summary>
        /// <param name="almoxID"></param>
        /// <param name="anoMesRefAlmoxarifado"></param>
        /// <param name="loginSiafem"></param>
        /// <param name="senhaSiafem"></param>
        /// <param name="isEstorno"></param>
        /// <returns></returns>
        public IList<PTResMensalEntity> processarConsumoUAsAlmoxarifadoEmLote(int almoxID, int anoMesRefAlmoxarifado, string loginSiafem, string senhaSiafem, bool isEstorno = false)
        {
            #region Variaveis
            AlmoxarifadoEntity almoxLogado = null;
            PTResMensalBusiness objBusiness = null;
            IList<PTResMensalEntity> lstLancamentosParaLiquidar = new List<PTResMensalEntity>();
            IList<PTResMensalEntity> lstPTResLiquidadas = new List<PTResMensalEntity>();
            IList<PTResMensalEntity> lstPTResInconsistentes = new List<PTResMensalEntity>();
            IList<string> lstMsgEstimuloSIAFEM = new List<string>();
            IList<string> lstRetornoEstimuloSIAFEM = new List<string>();

            string strUgeAlmox = string.Empty;
            string strUGE = string.Empty;
            string strUAConsumidora = string.Empty;
            string strPtRes = string.Empty;
            string strNatDespesa = string.Empty;
            string strCodigoGestao = string.Empty;
            string strUGConsumidora = string.Empty;
            string strErroTratado = string.Empty;
            string strLoginUsuario = string.Empty;
            string strSenhaUsuario = string.Empty;
            string strAnoBase = string.Empty;
            string strDataReferencia = string.Empty;
            string strMsgRetornoWs = string.Empty;
            string strMsgEstimuloWs = string.Empty;
            string strNomeMensagem = string.Empty;
            string strDescritivoErro = string.Empty;
            string strAcaoSIAFEM = string.Empty;

            string strXmlPattern = "/*/SiafemDocNLConsumo/*/";
            #endregion Variaveis

            #region Dados Login WS
            strLoginUsuario = loginSiafem;
            strSenhaUsuario = senhaSiafem;

            strDataReferencia = null;// DateTime.Today.ToString("ddMMMyyyy").ToUpper();
            #endregion Dados Login WS

            #region Processamento chave "UA|PT_Res|ND"

            lstPTResLiquidadas = new List<PTResMensalEntity>();
            objBusiness = new PTResMensalBusiness();

            almoxLogado = (new EstruturaOrganizacionalBusiness()).ObterAlmoxarifado(almoxID);
            //lstLancamentosParaLiquidar = objBusiness.ProcessarNLsConsumoAlmox(gestorId, anoMesRefAlmoxarifado, almoxID);

            strAnoBase = almoxLogado.MesRef.Substring(0, 4);
            strUgeAlmox = almoxLogado.Uge.Codigo.ToString();

            int mesRef = Int32.Parse(almoxLogado.MesRef.Substring(4, 2));
            int anoRef = Int32.Parse(almoxLogado.MesRef.Substring(0, 4));
            int ultimoDiaMes = DateTime.DaysInMonth(anoRef, mesRef);
            strDataReferencia = String.Format("{0}{1}{2}", ultimoDiaMes, MesExtenso.Mes[mesRef].ToUpperInvariant(), anoRef);

            //Geração Estímulos
            foreach (PTResMensalEntity PtResMensal in lstLancamentosParaLiquidar)
            {
                if (!PtResMensal.PtRes.Id.HasValue)
                {
                    lstPTResInconsistentes.Add(PtResMensal);
                    continue;
                }

                //strMsgEstimuloWs = Siafem.SiafemDocNLConsumo((int)PtResMensal.UgeAlmoxarifado.Codigo,
                //strMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocNLConsumo((int)PtResMensal.UgeAlmoxarifado.Codigo,
                strMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocNL((int)PtResMensal.UgeAlmoxarifado.Codigo,
                                                                    (int)PtResMensal.UGE.Codigo,
                                                                    (int)PtResMensal.UA.Codigo,
                                                                    (int)PtResMensal.PtRes.Codigo,
                                                                    (int)PtResMensal.Gestor.CodigoGestao,
                                                                    isEstorno,
                                                                    strDataReferencia,
                                                                    PtResMensal.NaturezaDespesa.Codigo,
                                                                    (decimal)PtResMensal.Valor,
                                                                    PtResMensal.DocumentoRelacionado);
                lstMsgEstimuloSIAFEM.Add(strMsgEstimuloWs);
            }

            //Pagamento Lote SIAFEM
            foreach (var msgEstimuloPagamentoConsumo in lstMsgEstimuloSIAFEM)
            {
                try
                {
                    strPtRes = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "PTRes"));
                    strUGConsumidora = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "UGConsumidora"));
                    strUAConsumidora = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "UAConsumidora"));
                    strAcaoSIAFEM = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "Lancamento"));

                    strAcaoSIAFEM = ((strAcaoSIAFEM == "E") ? "estorno de " : "");
                    strMsgRetornoWs = Siafem.recebeMsg(strLoginUsuario, strSenhaUsuario, strAnoBase, strUGE, strMsgEstimuloWs, false);
                    if (Siafem.VerificarErroMensagem(strMsgRetornoWs, out strNomeMensagem, out strErroTratado))
                    {
                        if (!String.IsNullOrWhiteSpace(strErroTratado) && this.ListaErro.Contains(strErroTratado))
                            continue;
                        else
                        {
                            strDescritivoErro = String.Format("Erro ao efetuar {0} pagamento de consumo referente a PTRes {1}, UGE: {2}, UA: {3}. [{4}].", strAcaoSIAFEM, strPtRes, strUGConsumidora, strUAConsumidora, strErroTratado);
                            this.ListaErro.Add(strDescritivoErro);

                            continue;
                        }
                    }

                    lstRetornoEstimuloSIAFEM.Add(strMsgRetornoWs);
                }
                catch (Exception excErroExecucao)
                {
                    bool existeDetalhe = excErroExecucao.InnerException.IsNotNull();
                    var strDescricaoDetalhe = (existeDetalhe) ? excErroExecucao.InnerException.Message : string.Empty;
                    var fmtMsgErro = (existeDetalhe) ? "\nException: {0}. Detalhe: {1}" : "Exception: {0}";

                    strDescritivoErro = String.Format("Erro ao efetuar {0} pagamento de consumo referente a PTRes {1}, UGE: {2}, UA: {3}.[{4}].", strAcaoSIAFEM, strPtRes, strUGConsumidora, strUAConsumidora, String.Format(fmtMsgErro, excErroExecucao.Message, strDescricaoDetalhe));

                    LogErro.GravarStackTrace(strDescritivoErro, true);
                    this.ListaErro.Add(strDescritivoErro);
                    continue;
                }
            }

            //Repactuação chamada SIAFEM x BD
            foreach (PTResMensalEntity ptResMensal in lstLancamentosParaLiquidar)
            {
                string _selectMsgRetornoEstimulo = null;
                string _selectMsgEstimulo = null;
                string _xmlScratch = null;
                string _xmlScratchUA = null;
                string _xmlScratchPTRes = null;
                string _xmlScratchND = null;

                if (ptResMensal.UA.Codigo.HasValue && ptResMensal.PtRes.Codigo.HasValue && ptResMensal.NaturezaDespesa.Codigo != 0)
                {
                    _xmlScratch = String.Format("<UAConsumidora>{0}</UAConsumidora><PTRes>{1}</PTRes><ClassificacaoDespesa>3{2:D8}</ClassificacaoDespesa>", ptResMensal.UA.Codigo.Value.ToString("D6"), ptResMensal.PtRes.Codigo.Value.ToString("D5"), ptResMensal.NaturezaDespesa.Codigo);
                    _xmlScratchUA = String.Format("<UAConsumidora>{0}</UAConsumidora>", ptResMensal.UA.Codigo.Value.ToString("D6"));
                    _xmlScratchPTRes = String.Format("<PTRes>{0}</PTRes>", ptResMensal.PtRes.Codigo.Value.ToString("D5"));
                    _xmlScratchND = String.Format("<ClassificacaoDespesa>3{0:D8}</ClassificacaoDespesa>", ptResMensal.NaturezaDespesa.Codigo);
                }
                else
                    continue;

                _selectMsgEstimulo = lstMsgEstimuloSIAFEM.Where(_estimuloWS => _estimuloWS.RetirarLinhasEEspacosEmBranco().Contains(_xmlScratch)).FirstOrDefault();

                if (!String.IsNullOrWhiteSpace(_selectMsgEstimulo))
                {
                    _selectMsgRetornoEstimulo = lstRetornoEstimuloSIAFEM.Where(_retornoSIAFEM => _retornoSIAFEM.RetirarLinhasEEspacosEmBranco().Contains(_selectMsgEstimulo.RetirarLinhasEEspacosEmBranco())).FirstOrDefault();

                    _selectMsgRetornoEstimulo = lstRetornoEstimuloSIAFEM.Where(_retornoSIAFEM => _retornoSIAFEM.Contains(_xmlScratchUA)
                                                                                          && _retornoSIAFEM.Contains(_xmlScratchPTRes)
                                                                                          && _retornoSIAFEM.Contains(_xmlScratchND))
                                                                        .FirstOrDefault();
                }
                else
                    continue;

                ptResMensal.MensagemWs = _selectMsgEstimulo;
                ptResMensal.Retorno = _selectMsgRetornoEstimulo;
                ptResMensal.NlLancamento = (!String.IsNullOrWhiteSpace(_selectMsgRetornoEstimulo)) ? XmlUtil.getXmlValue(_selectMsgRetornoEstimulo, String.Format("{0}{1}", strXmlPattern, "NumeroNL")) : null;

                lstPTResLiquidadas.Add(ptResMensal);
            }


            lstLancamentosParaLiquidar.ToList().ForEach(_ptresMensal =>
            {
                objBusiness.PTResMensal = _ptresMensal;
                objBusiness.SalvarPTResMensal();

                objBusiness.PTResMensal = null;
            });

            #endregion Processamento chave "UA|PT_Res|ND"


            return lstPTResLiquidadas;
        }
        /// <summary>
        /// Pagamento (ou estorno) de NL's em lote
        /// </summary>
        /// <param name="notasConsumoParaPagamento">Lista de NL's a pagar (ou estornar)</param>
        /// <param name="loginSiafem"></param>
        /// <param name="senhaSiafem"></param>
        /// <param name="isEstorno">Se a lista a ser passada como parâmetro será paga ou estornada.</param>
        /// <returns></returns>
        public IList<PTResMensalEntity> processarConsumoUAsAlmoxarifadoEmLote(IList<PTResMensalEntity> notasConsumoParaPagamento, string loginSiafem, string senhaSiafem, bool isEstorno = false)
        {
            #region Variaveis
            AlmoxarifadoEntity almoxLogado = null;
            PTResMensalBusiness objBusiness = null;
            IList<PTResMensalEntity> lstLancamentosParaLiquidar = new List<PTResMensalEntity>();
            IList<PTResMensalEntity> lstPTResLiquidadas = new List<PTResMensalEntity>();
            IList<PTResMensalEntity> lstPTResInconsistentes = new List<PTResMensalEntity>();
            IList<string> lstMsgEstimuloSIAFEM = new List<string>();
            IList<string> lstRetornoEstimuloSIAFEM = new List<string>();

            string strUgeAlmox = string.Empty;
            string strUAConsumidora = string.Empty;
            string strPtRes = string.Empty;
            string strNatDespesa = string.Empty;
            string strCodigoGestao = string.Empty;
            string strUGConsumidora = string.Empty;
            string strErroTratado = string.Empty;
            string strLoginUsuario = string.Empty;
            string strSenhaUsuario = string.Empty;
            string strAnoBase = string.Empty;
            string strDataReferencia = string.Empty;
            string strMsgRetornoWs = string.Empty;
            string strMsgEstimuloWs = string.Empty;
            string strNomeMensagem = string.Empty;
            string strDescritivoErro = string.Empty;
            string strAcaoSIAFEM = string.Empty;

            string strXmlPattern = "/*/SiafemDocNLConsumo/*/";
            #endregion Variaveis

            #region Dados Login WS
            strLoginUsuario = loginSiafem;
            strSenhaUsuario = senhaSiafem;

            strDataReferencia = null; // DateTime.Today.ToString("ddMMMyyyy").ToUpper();
            #endregion Dados Login WS

            #region Processamento chave "UA|PT_Res|ND"

            lstPTResLiquidadas = new List<PTResMensalEntity>();
            objBusiness = new PTResMensalBusiness();

            almoxLogado = (new EstruturaOrganizacionalBusiness()).ObterAlmoxarifado(this.Fechamento.Almoxarifado.Id.Value);
            // almoxLogado = (new EstruturaOrganizacionalBusiness()).ObterAlmoxarifado(notasConsumoParaPagamento[0].Almoxarifado.Id.Value);
            //almoxLogado = notasConsumoParaPagamento[0].Almoxarifado;
            lstLancamentosParaLiquidar = notasConsumoParaPagamento;

            strAnoBase = almoxLogado.MesRef.Substring(0, 4);
            strUgeAlmox = almoxLogado.Uge.Codigo.ToString();

            int mesRef = Int32.Parse(almoxLogado.MesRef.Substring(4, 2));
            int anoRef = Int32.Parse(almoxLogado.MesRef.Substring(0, 4));
            int ultimoDiaMes = DateTime.DaysInMonth(anoRef, mesRef);
            strDataReferencia = String.Format("{0}{1}{2}", ultimoDiaMes, MesExtenso.Mes[mesRef].ToUpperInvariant(), anoRef);

            //Geração Estímulos
            foreach (PTResMensalEntity PtResMensal in lstLancamentosParaLiquidar)
            {
                if (!PtResMensal.PtRes.Id.HasValue)
                {
                    lstPTResInconsistentes.Add(PtResMensal);
                    continue;
                }

                //strMsgEstimuloWs = Siafem.SiafemDocNLConsumo((int)PtResMensal.UgeAlmoxarifado.Codigo,
                //strMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocNLConsumo((int)PtResMensal.UgeAlmoxarifado.Codigo,
                strMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocNL((int)PtResMensal.UgeAlmoxarifado.Codigo,
                                                                    (int)PtResMensal.UGE.Codigo,
                                                                    (int)PtResMensal.UA.Codigo,
                                                                    (int)PtResMensal.PtRes.Codigo,
                                                                    (int)PtResMensal.Gestor.CodigoGestao,
                                                                    isEstorno,
                                                                    strDataReferencia,
                                                                    PtResMensal.NaturezaDespesa.Codigo,
                                                                    (decimal)PtResMensal.Valor,
                                                                    PtResMensal.DocumentoRelacionado);
                lstMsgEstimuloSIAFEM.Add(strMsgEstimuloWs);
            }

            //Pagamento Lote SIAFEM
            foreach (var msgEstimuloPagamentoConsumo in lstMsgEstimuloSIAFEM)
            {
                try
                {
                    strPtRes = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "PTRes"));
                    strUGConsumidora = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "UGConsumidora"));
                    strUAConsumidora = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "UAConsumidora"));
                    strAcaoSIAFEM = XmlUtil.getXmlValue(msgEstimuloPagamentoConsumo, String.Format("{0}{1}", strXmlPattern, "Lancamento"));

                    strAcaoSIAFEM = ((strAcaoSIAFEM == "E") ? "estorno de " : "");
                    strMsgRetornoWs = Siafem.recebeMsg(strLoginUsuario, strSenhaUsuario, strAnoBase, strUgeAlmox, strMsgEstimuloWs, false);
                    if (Siafem.VerificarErroMensagem(strMsgRetornoWs, out strNomeMensagem, out strErroTratado))
                    {
                        if (!String.IsNullOrWhiteSpace(strErroTratado) && this.ListaErro.Contains(strErroTratado))
                            continue;
                        else
                        {
                            strDescritivoErro = String.Format("Erro ao efetuar {0} pagamento de consumo referente a PTRes {1}, UGE: {2}, UA: {3}. [{4}].", strAcaoSIAFEM, strPtRes, strUGConsumidora, strUAConsumidora, strErroTratado);
                            this.ListaErro.Add(strDescritivoErro);

                            continue;
                        }
                    }

                    lstRetornoEstimuloSIAFEM.Add(strMsgRetornoWs);
                }
                catch (Exception excErroExecucao)
                {
                    bool existeDetalhe = excErroExecucao.InnerException.IsNotNull();
                    var strDescricaoDetalhe = (existeDetalhe) ? excErroExecucao.InnerException.Message : string.Empty;
                    var fmtMsgErro = (existeDetalhe) ? "\nException: {0}. Detalhe: {1}" : "Exception: {0}";

                    strDescritivoErro = String.Format("Erro ao efetuar pagamento de consumo referente a PTRes {0}, UGE: {1}, UA: {2}.[{3}].", strPtRes, strUGConsumidora, strUAConsumidora, String.Format(fmtMsgErro, excErroExecucao.Message, strDescricaoDetalhe));

                    LogErro.GravarStackTrace(strDescritivoErro, true);
                    this.ListaErro.Add(strDescritivoErro);
                    continue;
                }
            }

            //Repactuação chamada SIAFEM x BD
            foreach (PTResMensalEntity ptResMensal in lstLancamentosParaLiquidar)
            {
                string _selectMsgRetornoEstimulo = null;
                string _selectMsgEstimulo = null;
                string _xmlScratch = null;
                string _xmlScratchUA = null;
                string _xmlScratchPTRes = null;
                string _xmlScratchND = null;

                if (ptResMensal.UA.Codigo.HasValue && ptResMensal.PtRes.Codigo.HasValue && ptResMensal.NaturezaDespesa.Codigo != 0)
                {
                    _xmlScratch = String.Format("<UAConsumidora>{0}</UAConsumidora><PTRes>{1}</PTRes><ClassificacaoDespesa>3{2:D8}</ClassificacaoDespesa>", ptResMensal.UA.Codigo.Value.ToString("D6"), ptResMensal.PtRes.Codigo.Value.ToString("D5"), ptResMensal.NaturezaDespesa.Codigo);
                    _xmlScratchUA = String.Format("<UAConsumidora>{0}</UAConsumidora>", ptResMensal.UA.Codigo.Value.ToString("D6"));
                    _xmlScratchPTRes = String.Format("<PTRes>{0}</PTRes>", ptResMensal.PtRes.Codigo.Value.ToString("D5"));
                    _xmlScratchND = String.Format("<ClassificacaoDespesa>3{0:D8}</ClassificacaoDespesa>", ptResMensal.NaturezaDespesa.Codigo);
                }
                else
                    continue;

                _selectMsgEstimulo = lstMsgEstimuloSIAFEM.Where(_estimuloWS => _estimuloWS.RetirarLinhasEEspacosEmBranco().Contains(_xmlScratch)).FirstOrDefault();

                if (!String.IsNullOrWhiteSpace(_selectMsgEstimulo))
                {
                    _selectMsgRetornoEstimulo = lstRetornoEstimuloSIAFEM.Where(_retornoSIAFEM => _retornoSIAFEM.RetirarLinhasEEspacosEmBranco().Contains(_selectMsgEstimulo.RetirarLinhasEEspacosEmBranco())).FirstOrDefault();

                    _selectMsgRetornoEstimulo = lstRetornoEstimuloSIAFEM.Where(_retornoSIAFEM => _retornoSIAFEM.Contains(_xmlScratchUA)
                                                                                          && _retornoSIAFEM.Contains(_xmlScratchPTRes)
                                                                                          && _retornoSIAFEM.Contains(_xmlScratchND))
                                                                        .FirstOrDefault();
                }
                else
                    continue;

                ptResMensal.MensagemWs = _selectMsgEstimulo;
                ptResMensal.Retorno = _selectMsgRetornoEstimulo;
                ptResMensal.NlLancamento = (!String.IsNullOrWhiteSpace(_selectMsgRetornoEstimulo)) ? XmlUtil.getXmlValue(_selectMsgRetornoEstimulo, String.Format("{0}{1}", strXmlPattern, "NumeroNL")) : null;

                lstPTResLiquidadas.Add(ptResMensal);
            }


            lstLancamentosParaLiquidar.ToList().ForEach(_ptresMensal =>
            {
                objBusiness.PTResMensal = _ptresMensal;
                objBusiness.SalvarPTResMensal();

                objBusiness.PTResMensal = null;
            });

            #endregion Processamento chave "UA|PT_Res|ND"

            return lstPTResLiquidadas;
        }

        public PTResMensalEntity GerarNLConsumo(int usuarioSamID, string loginSiafem, string senhaSiafem, int almoxID, int uaID, int ptresID, int natDespesaID, decimal valorConsumo, string movimentoItemIDs, bool isEstorno = false)
        {
            #region Variaveis
            AlmoxarifadoEntity almoxConsumo = null;
            UAEntity uaConsumo = null;
            PTResEntity ptResConsumo = null;
            PTResMensalEntity ptResMensal = null;
            NaturezaDespesaEntity natDespesaConsumo = null;
            PTResMensalBusiness objBusiness = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;
            ProcessadorServicoSIAF svcSIAFEM = null; 

            string strUgeAlmox = string.Empty;
            string strUGE = string.Empty;
            string strUAConsumidora = string.Empty;
            string strPtRes = string.Empty;
            string strNatDespesa = string.Empty;
            string strCodigoGestao = string.Empty;
            string strUGConsumidora = string.Empty;
            string strErroTratado = string.Empty;
            string strLoginUsuario = string.Empty;
            string strSenhaUsuario = string.Empty;
            string strAnoBase = string.Empty;
            string strDataReferencia = string.Empty;
            string strMsgRetornoWs = string.Empty;
            string strMsgEstimuloWs = string.Empty;
            string strNomeMensagem = string.Empty;
            string strDescritivoErro = string.Empty;
            string strDocumentosNLConsumo = null;

            string strXmlPattern = "/*/SiafemDocNLConsumo/*/";
            #endregion Variaveis

            #region Dados Login WS
            strLoginUsuario = loginSiafem;
            strSenhaUsuario = senhaSiafem;

            strDataReferencia = null;// DateTime.Today.ToString("ddMMMyyyy").ToUpper();

            #endregion Dados Login WS

            #region Processamento chave "UA|PT_Res|ND"

            objBusiness = new PTResMensalBusiness();
            eoBusiness = new EstruturaOrganizacionalBusiness();

            almoxConsumo = eoBusiness.ObterAlmoxarifado(almoxID);
            uaConsumo = eoBusiness.ObterUA(uaID);
            ptResConsumo = eoBusiness.ObterPTRes(ptresID);
            natDespesaConsumo = (new CatalogoBusiness()).ObterNaturezaDespesa(natDespesaID);

            IList<int> movItems = new List<int>();
            movimentoItemIDs.BreakLine(Environment.NewLine.ToCharArray())
                            .ToList()
                            .ForEach(movItemID => movItems.Add(Int32.Parse(movItemID)));
            strDocumentosNLConsumo = this.ObterNumerosDocumentoPorMovimentoItemIDs(movItems);

            strUgeAlmox = almoxConsumo.Uge.Codigo.ToString();

            ptResMensal = new PTResMensalEntity();
            ptResMensal.UsuarioSamId = usuarioSamID;
            ptResMensal.Almoxarifado = almoxConsumo;
            ptResMensal.UA = uaConsumo;
            ptResMensal.UGE = uaConsumo.Uge;
            ptResMensal.Gestor = uaConsumo.Gestor;
            ptResMensal.PtRes = ptResConsumo;
            ptResMensal.NaturezaDespesa = natDespesaConsumo;
            ptResMensal.UgeAlmoxarifado = almoxConsumo.Uge;
            ptResMensal.AnoMesRef = Int32.Parse(almoxConsumo.MesRef);
            ptResMensal.Valor = valorConsumo;
            ptResMensal.TipoLancamento = (isEstorno) ? 'E' : 'N';
            ptResMensal.DocumentoRelacionado = strDocumentosNLConsumo;

            int mesRef = Int32.Parse(almoxConsumo.MesRef.Substring(4, 2));
            int anoRef = Int32.Parse(almoxConsumo.MesRef.Substring(0, 4));
            //TODO [CORRECAO NLCONSUMO] Validar correcao -> Obter ultimo dia util do mes, para pagar a NLCONSUMO
            int ultimoDiaMes = DateTime.Now.Day;
            if ((DateTime.Now.Month > mesRef && DateTime.Now.Year == anoRef) || (DateTime.Now.Month < mesRef && DateTime.Now.Year > anoRef))
                ultimoDiaMes = ObterUltimoDiaUtilMesAno(anoRef, mesRef);

            strDataReferencia = String.Format("{0:D2}{1}{2}", ultimoDiaMes, MesExtenso.Mes[mesRef].ToUpperInvariant(), anoRef);
            strAnoBase = anoRef.ToString();

            //Pagamento SIAFEM
            try
            {
                //Geração Estímulo
                //strMsgEstimuloWs = Siafem.SiafemDocNLConsumo((int)ptResMensal.UgeAlmoxarifado.Codigo,
                strMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocNL((int)ptResMensal.UgeAlmoxarifado.Codigo,
                                                                          (int)ptResMensal.UGE.Codigo,
                                                                          (int)ptResMensal.UA.Codigo,
                                                                          (int)ptResConsumo.Codigo,
                                                                          (int)ptResMensal.Gestor.CodigoGestao,
                                                                          isEstorno,
                                                                          strDataReferencia,
                                                                          ptResMensal.NaturezaDespesa.Codigo,
                                                                          (decimal)ptResMensal.Valor,
                                                                          ptResMensal.DocumentoRelacionado);


                strPtRes = ((int)ptResConsumo.Codigo).ToString("D5");
                strUGE = ((int)ptResMensal.UgeAlmoxarifado.Codigo).ToString("D6");
                strUGConsumidora = ((int)ptResMensal.UGE.Codigo).ToString("D6");
                strUAConsumidora = ((int)ptResMensal.UA.Codigo).ToString("D6");
                strNatDespesa = String.Format("{0:D8}", ptResMensal.NaturezaDespesa.Codigo);
                svcSIAFEM = new ProcessadorServicoSIAF();

                //Repactuação chamada SIAFEM x BD
                strXmlPattern = "/*/*/*/*/SiafemDocNL/*/";

                //strMsgRetornoWs = Siafem.recebeMsg(strLoginUsuario, strSenhaUsuario, strAnoBase, strUGE, strMsgEstimuloWs, false);
                //ptResMensal.NlLancamento = (!String.IsNullOrWhiteSpace(strMsgRetornoWs)) ? XmlUtil.getXmlValue(strMsgRetornoWs, String.Format("{0}{1}", strXmlPattern, "NumeroNL")) : null;
                svcSIAFEM.ConsumirWS(strLoginUsuario, strSenhaUsuario, strAnoBase, strUGE, strMsgEstimuloWs, false, true);

                strMsgRetornoWs = svcSIAFEM.RetornoWsSIAF;
                if (!svcSIAFEM.ErroProcessamentoWs)
                {
                    ptResMensal.NlLancamento = (!String.IsNullOrWhiteSpace(strMsgRetornoWs)) ? XmlUtil.getXmlValue(strMsgRetornoWs, String.Format("{0}{1}", strXmlPattern, "NumeroNL")) : null;
                    if (!String.IsNullOrEmpty(ptResMensal.NlLancamento))
                    {
                        //ptResMensal.Obs = strErroTratado;
                        ptResMensal.MensagemWs = strMsgEstimuloWs;
                        ptResMensal.Retorno = strMsgRetornoWs;

                        ptResMensal.FlagLancamento = (!String.IsNullOrWhiteSpace(ptResMensal.NlLancamento)) ? 'S' : 'N';
                        ptResMensal.MovimentoItemIDs = movimentoItemIDs;

                        objBusiness.PTResMensal = ptResMensal;
                        objBusiness.SalvarPTResMensal();
                    }
                }
                else
                {
                    strErroTratado = svcSIAFEM.ErroRetornoWs;

                    if (!this.ListaErro.Contains(Constante.CST_MSG_EXISTENCIA_ERRO_PAGAMENTO_NLCONSUMO))
                        this.ListaErro.Add(Constante.CST_MSG_EXISTENCIA_ERRO_PAGAMENTO_NLCONSUMO);

                    strErroTratado = strErroTratado.Replace("Mensagem retornada pelo sistema SIAFEM: ", "");
                    LogErro.InserirEntradaNoLog(new LogErroEntity()
                    {
                        Data = DateTime.Now,
                        Message = String.Format("{0}\n{1}\n{2}\n{3}", "Erro SIAFEM ao gerar NL Consumo", strErroTratado, strMsgEstimuloWs, strMsgRetornoWs),
                        StrackTrace = String.Format("FechamentoMensalBusiness.GerarNLConsumo(usuarioSamID: {0}, loginSiafem: {1}, almoxID: {2}, uaID: {3}, ptresID: {4}, natDespesaID: {5}, valorConsumo: {6}, movimentoItemIDs: {7}, isEstorno: {8})", usuarioSamID, loginSiafem, almoxID, uaID, ptresID, natDespesaID, valorConsumo.ToString(), movimentoItemIDs, isEstorno.ToString())
                    });

                    ptResMensal.Obs = strErroTratado;
                }
            }
            catch (Exception excErroExecucao)
            {
                bool existeDetalhe = excErroExecucao.InnerException.IsNotNull();
                var strDescricaoDetalhe = (existeDetalhe) ? excErroExecucao.InnerException.Message : string.Empty;


                strDescritivoErro = String.Format("Erro ao efetuar pagamento da NL Consumo referente a PTRes {0}, UGE: {1}, UA: {2}.", strPtRes, strUGConsumidora, strUAConsumidora);

                LogErro.GravarStackTrace(strDescritivoErro, true);
                this.ListaErro.Add(strDescritivoErro);
            }

            #endregion Processamento chave "UA|PT_Res|ND"

            if (objBusiness.ListaErro.HasElements())
                this.ListaErro.AddRange(objBusiness.ListaErro);


            return ptResMensal;
        }

        private DateTime ObterUltimaDataUtilMesAno(int anoRef, int mesRef)
        {
            int ultimoDiaMes = DateTime.DaysInMonth(anoRef, mesRef);
            DateTime ultimaDataUtilMesAno = new DateTime(anoRef, mesRef, ultimoDiaMes);

            if (ultimaDataUtilMesAno.DayOfWeek == DayOfWeek.Sunday)
                ultimaDataUtilMesAno = ultimaDataUtilMesAno.AddDays(-2);
            else if (ultimaDataUtilMesAno.DayOfWeek == DayOfWeek.Saturday)
                ultimaDataUtilMesAno = ultimaDataUtilMesAno.AddDays(-1);

            return ultimaDataUtilMesAno;
        }
        private int ObterUltimoDiaUtilMesAno(int anoRef, int mesRef)
        {
            int ultimoDiaUtilMes = -1;
            DateTime dtUltimoDiaUtilMesAno = this.ObterUltimaDataUtilMesAno(anoRef, mesRef);

            ultimoDiaUtilMes = dtUltimoDiaUtilMesAno.Day;
            return ultimoDiaUtilMes;
        }

        private string ObterNumerosDocumentoPorMovimentoItemIDs(IList<int> movItems)
        {
            string docsSAM = null;
            MovimentoItemBusiness objBusiness = null;



            objBusiness = new MovimentoItemBusiness();
            docsSAM = objBusiness.ObterNumerosDocumentoPorMovimentoItemIDs(movItems);


            return docsSAM;
        }

        #endregion WS SIAFNLCONSUMO (refactored code)

        public AlmoxarifadoEntity VerificarAlmoxarifadoInativos(int? almoxID)
        {
            AlmoxarifadoEntity listaFecha = null;

            listaFecha= Service<IFechamentoMensalService>().ListarStatusAlmoxarifado(almoxID);

            return listaFecha;
        }      
    }
}
