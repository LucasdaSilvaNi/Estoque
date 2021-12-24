using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Xml.Linq;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;



namespace Sam.Domain.Business
{
    public partial class NotaLancamentoPendenteSIAFEMBusiness : BaseBusiness
    {
        private NotaLancamentoPendenteSIAFEMEntity entity = new NotaLancamentoPendenteSIAFEMEntity();
        public NotaLancamentoPendenteSIAFEMEntity Entity
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
                    this.Service<INotaLancamentoPendenteSIAFEMService>().Entity = this.Entity;
                    if (this.Consistido)
                        this.Service<INotaLancamentoPendenteSIAFEMService>().Salvar();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao gravar pendência de nota de lançamento. Favor verificar dados fornecidos!" };
            }

            return this.Consistido;
        }
        public bool InativarPendencia(int notaLancamentoPendenciaID)
        {
            this.Service<INotaLancamentoPendenteSIAFEMService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        this.Service<INotaLancamentoPendenteSIAFEMService>().InativarPendencia(notaLancamentoPendenciaID);
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

        public bool InativarPendenciasPorMovimentacao(int movimentacaoMaterialID)
        {
            this.Service<INotaLancamentoPendenteSIAFEMService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        this.Service<INotaLancamentoPendenteSIAFEMService>().InativarPendenciasPorMovimentacao(movimentacaoMaterialID);
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

        public IList<NotaLancamentoPendenteSIAFEMEntity> ObterNotasLancamentosPendentes(tipoPesquisa tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<INotaLancamentoPendenteSIAFEMService>().SkipRegistros = this.SkipRegistros;
                    lstRetorno = this.Service<INotaLancamentoPendenteSIAFEMService>().ListarpendenciaSiafemPorAlmox(tipoPesquisa, tabelaPesquisaID, pendenciasAtivas);
                    this.TotalRegistros = this.Service<INotaLancamentoPendenteSIAFEMService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao obter pendência(s) de nota(s) de lançamento. Favor verificar dados fornecidos!" };
            }

            return lstRetorno;
        }



        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarNotasLancamentosPendentes(tipoPesquisa tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<INotaLancamentoPendenteSIAFEMService>().SkipRegistros = this.SkipRegistros;
                    lstRetorno = this.Service<INotaLancamentoPendenteSIAFEMService>().ListarpendenciaSiafemPorAlmoxarifados(tipoPesquisa, tabelaPesquisaID, pendenciasAtivas);
                    this.TotalRegistros = this.Service<INotaLancamentoPendenteSIAFEMService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao obter pendência(s) de nota(s) de lançamento. Favor verificar dados fornecidos!" };
            }

            return lstRetorno;
        }


        public NotaLancamentoPendenteSIAFEMEntity ObterNotaLancamentoPendente(int notaLancamentoPendenteID)
        {
            NotaLancamentoPendenteSIAFEMEntity objRetorno = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    objRetorno = this.Service<INotaLancamentoPendenteSIAFEMService>().ObterNotaLancamentoPendente(notaLancamentoPendenteID);
                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao obter pendência(s) de nota(s) de lançamento. Favor verificar dados fornecidos!" };
            }

            return objRetorno;
        }

        public NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true)
        {
            NotaLancamentoPendenteSIAFEMEntity notaPendente = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<INotaLancamentoPendenteSIAFEMService>().SkipRegistros = this.SkipRegistros;
                    notaPendente = this.Service<INotaLancamentoPendenteSIAFEMService>().ObterPendenciaParaMovimentacao(movimentacaoMaterialID, pendenciasAtivas);
                    this.TotalRegistros = this.Service<INotaLancamentoPendenteSIAFEMService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao obter pendência(s) de nota(s) de lançamento. Favor verificar dados fornecidos!" };
            }

            return notaPendente;
        }

        public IList<NotaLancamentoPendenteSIAFEMEntity> ObterPendenciasParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> notasPendentes = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    this.Service<INotaLancamentoPendenteSIAFEMService>().SkipRegistros = this.SkipRegistros;
                    notasPendentes = this.Service<INotaLancamentoPendenteSIAFEMService>().ObterPendenciasParaMovimentacao(movimentacaoMaterialID, pendenciasAtivas);
                    this.TotalRegistros = this.Service<INotaLancamentoPendenteSIAFEMService>().TotalRegistros();

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (!this.ListaErro.IsNotNull())
                    this.ListaErro = new List<string>() { "Erro ao obter pendência(s) de nota(s) de lançamento. Favor verificar dados fornecidos!" };
            }

            return notasPendentes;
        }

        public bool InserirRegistro(NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente)
        {
            bool blnRetorno = false;

            using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                blnRetorno = this.Service<INotaLancamentoPendenteSIAFEMService>().InserirRegistro(notaLancamentoPendente);
                trOperacaoBancoDados.Complete();
            }

            return blnRetorno;
        }
        public IList<NotaLancamentoPendenteSIAFEMEntity> Imprimir()
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                lstRetorno = this.Service<INotaLancamentoPendenteSIAFEMService>().Imprimir();
                trOperacaoBancoDados.Complete();
            }

            return lstRetorno;
        }

        public string GerarXmlAuditoriaAlteracaoManual(string loginUsuarioSAM, MovimentoEntity movimentacaoMaterial, string numeroNL, string tipoLancamento, TipoNotaSIAF @TipoNotaSIAF)
        {
            string strRetorno = null;
            string nomeCampoTipoNL = null;
            string sufixoTipoLancamento = null;
            XElement xml = null;
            XElement xmlTB_MOVIMENTO = null;
            XElement xmlTB_MOVIMENTO_ITEMs = null;
            XElement xmlTB_MOVIMENTO_ITEM = null;




            sufixoTipoLancamento = ((tipoLancamento.ToUpperInvariant() == "E") ? "_ESTORNO" : null);
            nomeCampoTipoNL = String.Format("{0}{1}", nomeCampoTipoNL, sufixoTipoLancamento);
            switch (@TipoNotaSIAF)
            {
                case TipoNotaSIAF.NL_Liquidacao: nomeCampoTipoNL = "TB_MOVIMENTO_ITEM_NL_LIQUIDACAO"; break;
                case TipoNotaSIAF.NL_Reclassificacao: nomeCampoTipoNL = "TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO"; break;
                case TipoNotaSIAF.NL_Consumo: nomeCampoTipoNL = "TB_MOVIMENTO_ITEM_NL_CONSUMO"; break;
            }


            xmlTB_MOVIMENTO_ITEMs = new XElement("TB_MOVIMENTO_ITEMs");
            foreach (var itemMovimento in movimentacaoMaterial.MovimentoItem)
            {
                xmlTB_MOVIMENTO_ITEM = new XElement("TB_MOVIMENTO_ITEM"
                                                                       , new XAttribute("TB_MOVIMENTO_ITEM_ID", itemMovimento.Id.Value)
                                                                       , new XAttribute(nomeCampoTipoNL, numeroNL));

                xmlTB_MOVIMENTO_ITEMs.Add(xmlTB_MOVIMENTO_ITEM);
                xmlTB_MOVIMENTO_ITEM = null;
            }

            xmlTB_MOVIMENTO = new XElement("TB_MOVIMENTO", xmlTB_MOVIMENTO_ITEMs);
            xml = new XElement("TB_AUDITORIA_INTEGRACAO"
                                                         , new XElement("AlteracaoRegistro_TelaPendenciaSIAFEM"
                                                                                                                , new XElement("AlteracaoRegistros"
                                                                                                                                                    , new XAttribute("DataAlteracao", DateTime.Now.ToString(base.fmtDataHoraFormatoBrasileiro))
                                                                                                                                                    , new XAttribute("AlteradoPor", loginUsuarioSAM)
                                                                                                                                                    , xmlTB_MOVIMENTO)));
            var xmlDoc = XElement.Parse(xml.ToString());
            strRetorno = xmlDoc.ToString();

            return strRetorno;
        }
    }
}
