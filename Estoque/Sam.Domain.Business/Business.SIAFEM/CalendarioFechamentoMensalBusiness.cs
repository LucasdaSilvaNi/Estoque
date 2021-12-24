using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;




namespace Sam.Domain.Business
{
    public partial class CalendarioFechamentoMensalBusiness : BaseBusiness
    {
        private CalendarioFechamentoMensalEntity entity = new CalendarioFechamentoMensalEntity();
        public CalendarioFechamentoMensalEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public bool Salvar()
        {
            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<ICalendarioFechamentoMensalService>().Entity = this.Entity;
                    this.ConsistirDataFechamento();
                    if (this.Consistido)
                        this.Service<ICalendarioFechamentoMensalService>().Salvar();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao gravar data fechamento. Favor verificar dados fornecidos!" };
            }

            return this.Consistido;
        }

        public IList<CalendarioFechamentoMensalEntity> ListarDatasFechamento()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<ICalendarioFechamentoMensalService>().SkipRegistros = this.SkipRegistros;
                    lstRetorno = this.Service<ICalendarioFechamentoMensalService>().Listar();
                    this.TotalRegistros = this.Service<ICalendarioFechamentoMensalService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao gravar data fechamento. Favor verificar dados fornecidos!" };
            }

            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> ListarDatasFechamento(int Ano)
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<ICalendarioFechamentoMensalService>().SkipRegistros = this.SkipRegistros;
                    lstRetorno = this.Service<ICalendarioFechamentoMensalService>().Listar(Ano);
                    this.TotalRegistros = this.Service<ICalendarioFechamentoMensalService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao gravar data fechamento. Favor verificar dados fornecidos!" };
            }

            return lstRetorno;
        }

        public IList<int> ListarAnoFechamento()
        {
            IList<int> lstRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<ICalendarioFechamentoMensalService>().SkipRegistros = this.SkipRegistros;
                    lstRetorno = this.Service<ICalendarioFechamentoMensalService>().ListarAno();
                    this.TotalRegistros = this.Service<ICalendarioFechamentoMensalService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao carregar Ano Fechamento. Favor verificar dados fornecidos!" };
            }

            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> Imprimir()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;

            using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                lstRetorno = this.Service<ICalendarioFechamentoMensalService>().Imprimir();
                trOperacaoBancoDados.Complete();
            }

            return lstRetorno;
        }

        public bool Excluir()
        {
            this.Service<ICalendarioFechamentoMensalService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        this.Service<ICalendarioFechamentoMensalService>().Excluir();
                        trOperacaoBancoDados.Complete();
                    }
                }
                catch (Exception excErroOperacaoBancoDados)
                {
                    new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                    if (!this.ListaErro.IsNotNull())
                        this.ListaErro = new List<string>() { "Erro ao excluir data fechamento. Favor verificar dados fornecidos!" };
                }
            }
            return this.Consistido;
        }

        public bool StatusFechadoMesReferenciaSIAFEM(int mesReferencia, int anoReferencia, bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            CalendarioFechamentoMensalEntity dataFechamento = null;

            try
            {
                dataFechamento = this.Service<ICalendarioFechamentoMensalService>().ObterDataFechamentoMensal(mesReferencia, anoReferencia);

                if (dataFechamento.IsNotNull())
                {
                    dataFechamento.DataFechamentoDespesa = dataFechamento.DataFechamentoDespesa.AddHours(19);
                
                    blnRetorno = dataFechamento.DataFechamentoDespesa <= DateTime.Now;


                    if (exibirMensagemErro)
                        this.ListaErro.Add(String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2}.", mesReferencia, anoReferencia, dataFechamento.DataFechamentoDespesa.ToString("dd/MM/yyyy")));
                }
                else if (exibirMensagemErro && dataFechamento.IsNull())
                {
                    this.ListaErro.Add(String.Format("Mês/ano referência ({0:D2}/{1:D4}) não possui data de fechamento definida. Favor contatar Prodesp ou CSCC/SEFAZ.", mesReferencia, anoReferencia));
                }
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.ListaErro.Add("Módulo Financeiro: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
                return false;
            }


            return blnRetorno;
        }

        public bool StatusFechadoMesReferenciaSIAFEM2(int mesReferencia, int anoReferencia, bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            CalendarioFechamentoMensalEntity dataFechamento = null;

            try
            {
                dataFechamento = this.Service<ICalendarioFechamentoMensalService>().ObterDataFechamentoMensal(mesReferencia, anoReferencia);

                if (dataFechamento.IsNotNull())
                {
                    //dataFechamento.DataFechamentoDespesa = dataFechamento.DataFechamentoDespesa.AddHours(19);
                    blnRetorno = dataFechamento.DataFechamentoDespesa <= DateTime.Now;

                //    if (exibirMensagemErro)
                //        this.ListaErro.Add(String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2}.", mesReferencia, anoReferencia, dataFechamento.DataFechamentoDespesa.ToString("dd/MM/yyyy")));
                //}
                }
                //else if (exibirMensagemErro && dataFechamento.IsNull())
                //{
                //    this.ListaErro.Add(String.Format("Mês/ano referência ({0:D2}/{1:D4}) não possui data de fechamento definida. Favor contatar Prodesp ou CSCC/SEFAZ.", mesReferencia, anoReferencia));
                //}
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.ListaErro.Add("Módulo Financeiro: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
                return false;
            }


            return blnRetorno;
        }

        public CalendarioFechamentoMensalEntity ObterDataFechamentoMensalSIAFEM(int mesReferencia, int anoReferencia)
        {
            CalendarioFechamentoMensalEntity dtRetorno = null;
            ICalendarioFechamentoMensalService srvInfra = null;

            try
            {
                srvInfra = this.Service<ICalendarioFechamentoMensalService>();

                if (srvInfra.IsNotNull())
                    dtRetorno = srvInfra.ObterDataFechamentoMensal(mesReferencia, anoReferencia);
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.ListaErro.Add("Módulo Financeiro: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
            }

            return dtRetorno;
        }

        public bool ConsistirDataFechamento()
        {
            //Fazer validação
            if (this.Entity.AnoReferencia <= 0)
                this.ListaErro.Add("É obrigatório informar ano válido para registro.");

            if (this.Entity.MesReferencia <= 0)
                this.ListaErro.Add("É obrigatório selecionar mês de referência válido para registro.");

            //validar via M+1
            var dtFechamento = new DateTime(this.Entity.AnoReferencia, this.Entity.MesReferencia, 01);
            dtFechamento = dtFechamento.AddMonths(1);

            //if (!(this.Entity.DataFechamentoDespesa.Year == dtFechamento.Year && this.Entity.DataFechamentoDespesa.Month == dtFechamento.Month))
            //    this.ListaErro.Add("Data de Fechamento de Despesa inválida para ano/mês de referência informados.");

            if (this.Service<ICalendarioFechamentoMensalService>().ExisteMesReferenciaInformado() && !this.Entity.Id.HasValue)
                this.ListaErro.Add("Registro já existente para ano/mês de referência informado.");

            return (this.ListaErro.Count == 0);
        }


        public CalendarioFechamentoMensalEntity PegarPendenciaFechamento(int mesReferencia, int anoReferencia)
        {
            CalendarioFechamentoMensalEntity dtRetorno = null;
            ICalendarioFechamentoMensalService srvInfra = null;

            try
            {
                srvInfra = this.Service<ICalendarioFechamentoMensalService>();

                if (srvInfra.IsNotNull())
                    dtRetorno = srvInfra.ObterDataFechamentoMensal(mesReferencia, anoReferencia);
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.ListaErro.Add("Módulo Financeiro: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
            }

            return dtRetorno;
        }
    }
}
