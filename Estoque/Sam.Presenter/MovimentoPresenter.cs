using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Business.SIAFEM;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using Sam.View;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;


namespace Sam.Presenter
{
    public class MovimentoPresenter : CrudPresenter<IMovimentoView>
    {
        private readonly Sam.Business.MovimentoBusiness businessMovimentoPendente = new Business.MovimentoBusiness();
        public IList<MovimentoItemEntity> listaMovimentoItem { set; get; }
        public void PopularDadosTodosCodMovimento(int _ugeId)
        {

        }

        public MovimentoEntity gerarNovoDocumento(MovimentoEntity movimento)
        {
            MovimentoBusiness mov = new MovimentoBusiness();
            return mov.GerarNovoDocumento(movimento);
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int maximumRowsParameterName, int startRowIndexParameterName, int almoxarifadoId, int divisaoId, int tipoMovimento, int tipoDeOperacao, string datadigitada)
        {
            string mesRef = null;
            bool requisicoesParaEstorno = false;

            if (tipoDeOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Estorno)
            {
                mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                requisicoesParaEstorno = true;
            }
            if (!((tipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente) && (tipoDeOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Nova)))
            {
                datadigitada = null;
            }
            if (((tipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente) && (tipoDeOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Nova)))
            {
               // divisaoId = 0;
            }
            //Trecho comentado, pois devido a solicitação originada por cliente, o sistema atenderá requisições de qualquer mês.
            //string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            string dataDigitada = datadigitada;
            MovimentoBusiness estrutura = new MovimentoBusiness();
            //IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef);
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, requisicoesParaEstorno, dataDigitada);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        //Lista as requisições pendetes pelo amloxarifado passando o almoxarifado
        public IList<MovimentoEntity> ListarRequisicaoByPendete(int maximumRowsParameterName, int startRowIndexParameterName, int almoxarifadoId, int divisaoId, int tipoMovimento, int tipoDeOperacao)
        {
            string mesRef = null;
            bool requisicoesParaEstorno = false;

            if (tipoDeOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Estorno)
            {
                mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                requisicoesParaEstorno = true;
            }
            MovimentoBusiness estrutura = new MovimentoBusiness();           
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, requisicoesParaEstorno,"" );
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }



        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int maximumRowsParameterName, int startRowIndexParameterName, int almoxarifadoId, int divisaoId, int tipoMovimento, bool requisicoesParaEstorno)
        {
            string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, requisicoesParaEstorno, "");
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUge(int maximumRowsParameterName, int startRowIndexParameterName, MovimentoEntity movimento)
        {
            string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            movimento.AnoMesReferencia = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            MovimentoBusiness estrutura = new MovimentoBusiness();
            estrutura.Movimento = movimento;
            IList<MovimentoEntity> retorno = estrutura.ListarDocumentoByAlmoxarifadoUGE();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUge(int maximumRowsParameterName, int startRowIndexParameterName, MovimentoEntity movimento, bool docsParaEstorno)
        {
            if (!docsParaEstorno)
                return ListarDocumentoByAlmoxarifadoUge(maximumRowsParameterName, startRowIndexParameterName, movimento);


            string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            movimento.AnoMesReferencia = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            MovimentoBusiness estrutura = new MovimentoBusiness();
            estrutura.Movimento = movimento;
            IList<MovimentoEntity> retorno = estrutura.ListarDocumentoByAlmoxarifadoUGE(docsParaEstorno);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int maximumRowsParameterName, int startRowIndexParameterName, int almoxarifadoId, int tipoMovimento)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, tipoMovimento, "");
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<MovimentoEntity> ListarRequisicaoByDivisao(int maximumRowsParameterName, int startRowIndexParameterName, int divisaoId, int AlmoxarifadoId)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByDivisao(divisaoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        //Query 1 - Default Divisao        
        public IList<MovimentoEntity> ListarRequisicaoPorDivisao(int maximumRowsParameterName, int startRowIndexParameterName, int divisaoId, int AlmoxarifadoId)
        {
            IList<MovimentoEntity> retorno = new List<MovimentoEntity>();
            MovimentoBusiness movimentoBusiness = new MovimentoBusiness();
            Expression<Func<MovimentoEntity, bool>> expression = null;

            movimentoBusiness.SkipRegistros = startRowIndexParameterName;
            expression = a => a.Ativo == true &&
                                   a.Divisao.Id == divisaoId &&
                                   ((a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente) ||
                                   (a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada) ||
                                   (a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada));
            retorno = movimentoBusiness.ListarRequisicaoByDivisaoAndTipoMovimentoId(divisaoId, expression);
            this.TotalRegistrosGrid = movimentoBusiness.TotalRegistros;

            return retorno;
        }

        //Query 2 - NumeroDocumento
        public IList<MovimentoEntity> ListarRequisicaoByNumeroDocumento(int maximumRowsParameterName, int startRowIndexParameterName, string numeroDocumento)
        {
            IList<MovimentoEntity> retorno = new List<MovimentoEntity>();
            MovimentoBusiness movimentoBusiness = new MovimentoBusiness();
            movimentoBusiness.SkipRegistros = startRowIndexParameterName;

            if (!string.IsNullOrEmpty(numeroDocumento))
            {
                string[] specialKeys = { "\\", "/" };
                numeroDocumento = RemoveSpecialKeyNumeroDocumento(numeroDocumento, specialKeys);

                var acessoPerfils = Acesso.Transacoes.Perfis[0];
                retorno = new Facade.FacadePerfil().ListarMovimentoEntityByPerfil(acessoPerfils.IdLogin, acessoPerfils.IdPerfil, numeroDocumento);
                this.TotalRegistrosGrid = retorno.Count;
            }

            return retorno;
        }

        //Query 3 - Default + Status (TipoMovimentoID)
        public IList<MovimentoEntity> ListarRequisicaoByTipoMovimentoId(int maximumRowsParameterName, int startRowIndexParameterName, int divisaoId, int AlmoxarifadoId, int statusId)
        {
            IList<MovimentoEntity> retorno = new List<MovimentoEntity>();
            MovimentoBusiness movimentoBusiness = new MovimentoBusiness();
            movimentoBusiness.SkipRegistros = startRowIndexParameterName;

            if (statusId > 0)
            {
                //Expression Status                    
                Expression<Func<MovimentoEntity, bool>> expression = null;
                expression = a => a.Ativo == true &&
                                    a.Divisao.Id == divisaoId &&
                                    a.TipoMovimento.Id.Equals(statusId);

                retorno = movimentoBusiness.ListarRequisicaoByDivisaoAndTipoMovimentoId(divisaoId, expression).ToList();
            }

            this.TotalRegistrosGrid = movimentoBusiness.TotalRegistros;

            return retorno;
        }

        public MovimentoEntity ListarDocumentosRequisicaoByDocumento(string numeroDocumento)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            MovimentoEntity retorno = estrutura.ListarDocumentosRequisicaoByDocumento(numeroDocumento);
            return retorno;
        }

        public MovimentoEntity ObterMovimentacaoConsumoImediato(int movimentoId)
        {
            MovimentoBusiness objBusiness = new MovimentoBusiness();
            return objBusiness.ObterMovimentacaoConsumoImediato(movimentoId);
        }

        public MovimentoEntity ListarDocumentosRequisicaoById(int id)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            MovimentoEntity retorno = estrutura.ListarDocumentosRequisicaoById(id);
            return retorno;
        }

        public int TotalRegistros(int almoxarifadoId, int divisaoId, int tipoMovimento)
        {
            return this.TotalRegistrosGrid;
        }

        //Query 1
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, int divisaoId, int AlmoxarifadoId)
        {
            return this.TotalRegistrosGrid;
        }

        //Query 2
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, string numeroDocumento)
        {
            return this.TotalRegistrosGrid;
        }

        //Query 3
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, int divisaoId, int AlmoxarifadoId, int statusId)
        {
            return this.TotalRegistrosGrid;
        }

        public IList<TB_MOVIMENTO> PesquisaDocumentosMesReferencia(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {
            Sam.Business.MovimentoBusiness business = new Business.MovimentoBusiness();

            movimento.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            movimento.TB_MOVIMENTO_ATIVO = true;
            movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            var result = business.PesquisaDocumentosMesReferencia(startRowIndexParameterName, palavraChave, movimento, tipoOperacao);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public IList<TB_MOVIMENTO> PesquisaDocumentos(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {
            Sam.Business.MovimentoBusiness business = new Business.MovimentoBusiness();

            movimento.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            movimento.TB_MOVIMENTO_ATIVO = true;
            movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            var result = business.PesquisaDocumentos(startRowIndexParameterName, palavraChave, movimento, tipoOperacao);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public IList<TB_MOVIMENTO_ITEM> ListarMovimentosRecalcular(TB_MOVIMENTO movimento)
        {
            Sam.Business.MovimentoBusiness business = new Business.MovimentoBusiness();

            var result = business.ListarMovimentosRecalcular(movimento);

            return result;
        }

        public int CorrigirSaldo()
        {
            try
            {
                Sam.Business.CorrecaoSaldolBusiness business = new Business.CorrecaoSaldolBusiness();
                return business.CorrigirSaldo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> ListaDeMovimentoPendente(int startIndex, Int32? almoxarifadoId, Int64? subtItemCodigo)
        {
            if (almoxarifadoId.Value < 1)
            {
                this.View.ExibirMensagem("Favor informar o almoxarifado!");
                return null;
            }

            IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> result;
            try
            {
                result = businessMovimentoPendente.ListaDeMovimentoPendente(startIndex, almoxarifadoId, subtItemCodigo);
                TotalRegistrosGrid = businessMovimentoPendente.TotalRegistros;
                return result;
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem(ex.Message);
                return null;
            }


        }

        public Int32 getSubItensCount(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> lista)
        {
            var result = businessMovimentoPendente.getSubItensCount(lista);
            return result;

        }

        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> getListaDeMovimentoPendente()
        {
            return businessMovimentoPendente.getListaDeMovimentoPendente();
        }

        public void setListaDeMovimentoPendente(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> ListaDeMovimentoPendente)
        {
            businessMovimentoPendente.setListaDeMovimentoPendente(ListaDeMovimentoPendente);
        }

        private string RemoveSpecialKeyNumeroDocumento(string numeroDocumento, string[] specialKeys)
        {
            string _numeroDocumento = numeroDocumento.Trim();
            foreach (var key in specialKeys)
            {
                _numeroDocumento = _numeroDocumento.Contains(key) ? _numeroDocumento.Replace(key, "") : _numeroDocumento;
            }

            return _numeroDocumento;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;

            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            lstMovimentacoes = objBusiness.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            return lstMovimentacoes;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenhoAgrupadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;

            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            lstMovimentacoes = objBusiness.ListarMovimentacoesEmpenhoAgrupadas(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            return lstMovimentacoes;
        }

        public void CorrigirSubItem(MovimentoItemEntity movimentoItem)
        {
            MovimentoPresenter presenter = new MovimentoPresenter();

            try
            {
                ExecutarCorrecao(movimentoItem);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        public Tuple<string, bool> AlterarMovimentoSubItem(MovimentoItemEntity movimentoItem)
        {
            MovimentoBusiness business = new MovimentoBusiness();

            try
            {
                return business.AtualizarMovimentoSubItem(movimentoItem);
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }

        }

        public Tuple<string, bool> AtualizarMovimentoBloquear(int movId, bool bloquear)
        {
            MovimentoBusiness business = new MovimentoBusiness();

            try
            {
                return business.AtualizarMovimentoBloquear(movId,  bloquear);
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }

        }


        public DataSet GerarExportacaoAnalitica(int w_OrgaoId
                                                            , int r_Periodo = 0
                                                            , int r_UGECodigo = 0
                                                            , int r_UGEDescricao = 0
                                                            , int r_NaturezaDespesaCodigo = 0
                                                            , int r_NaturezaDespesaDescricao = 0
                                                            , int r_SubitemCodigo = 0
                                                            , int r_SubitemDescricao = 0
                                                            , int r_UnidadeFornecimento = 0
                                                            , int r_DataMovimento = 0
                                                            , int r_TipoMovimentoDescricao = 0
                                                            , int r_SituacaoDescricao = 0
                                                            , int r_FornecedorDescricao = 0
                                                            , int r_NumeroDoc = 0
                                                            , int r_Lote = 0
                                                            , int r_VenctoDoc = 0
                                                            , int r_QtdeDoc = 0
                                                            , int r_ValorUnitario = 0

                                                            , int r_PrecoUnitario_EMP = 0

                                                            , int r_Desdobro = 0
                                                            , int r_TotalDoc = 0
                                                            , int r_QtdeSaldo = 0
                                                            , int r_ValorSaldo = 0

                                                            , int r_UaCodigoRequisicao = 0
                                                            , int r_UaDescricaoRequisicao = 0
                                                            , int r_Divisao = 0
                                                            , int r_UGECodigoDestino = 0
                                                            , int r_UGEDescricaoDestino = 0

                                                            , int r_InformacoesComplementares = 0
                                                            , int r_NLConsumo = 0
                                                            , int r_NLLiquidacao = 0
                                                            , int r_NLLiquidacaoEstorno = 0
                                                            , string w_AnoMesRefDe = ""
                                                            , string w_AnoMesRefAte = ""
                                                            , string w_Fornecedor = ""
                                                            , string w_LoteDescricao = ""
                                                            , string w_NaturezaDespesaCodigo = ""
                                                            , string w_NumeroDoc = ""
                                                            , string w_SaldoQuantidade = ""
                                                            , string w_SaldoValor = ""
                                                            , string w_SubitemCodigo = ""
                                                            , string w_TipoMovimentoId = ""
                                                            , string w_UgeCodigo = ""
                                                            , string w_NLConsumo = ""
                                                            , string w_NLLiquidacao = ""
                                                            , string w_NLLiquidacaoEstorno = ""
                                                            , string w_UA_Id_Destino = ""
                                                            , string w_UGE_Id_Destino = "")
        {
            DataSet dsRetorno = new DataSet();
            try
            {


                Sam.Business.MovimentoBusiness business = new Sam.Business.MovimentoBusiness();

                dsRetorno = business.GerarExportacaoAnalitica(w_OrgaoId
                                                            , r_Periodo
                                                            , r_UGECodigo
                                                            , r_UGEDescricao
                                                            , r_NaturezaDespesaCodigo
                                                            , r_NaturezaDespesaDescricao
                                                            , r_SubitemCodigo
                                                            , r_SubitemDescricao
                                                            , r_UnidadeFornecimento
                                                            , r_DataMovimento
                                                            , r_TipoMovimentoDescricao
                                                            , r_SituacaoDescricao
                                                            , r_FornecedorDescricao
                                                            , r_NumeroDoc
                                                            , r_Lote
                                                            , r_VenctoDoc
                                                            , r_QtdeDoc
                                                            , r_ValorUnitario

                                                            ,r_PrecoUnitario_EMP

                                                            , r_Desdobro
                                                            , r_TotalDoc
                                                            , r_QtdeSaldo
                                                            , r_ValorSaldo

                                                            , r_UaCodigoRequisicao
                                                            , r_UaDescricaoRequisicao
                                                            , r_Divisao
                                                            , r_UGECodigoDestino
                                                            , r_UGEDescricaoDestino

                                                            , r_InformacoesComplementares
                                                            , r_NLConsumo
                                                            , r_NLLiquidacao
                                                            , r_NLLiquidacaoEstorno
                                                            , w_AnoMesRefDe
                                                            , w_AnoMesRefAte
                                                            , w_Fornecedor
                                                            , w_LoteDescricao
                                                            , w_NaturezaDespesaCodigo
                                                            , w_NumeroDoc
                                                            , w_SaldoQuantidade
                                                            , w_SaldoValor
                                                            , w_SubitemCodigo
                                                            , w_TipoMovimentoId
                                                            , w_UgeCodigo
                                                            , w_NLConsumo
                                                            , w_NLLiquidacao
                                                            , w_NLLiquidacaoEstorno
                                                            , w_UA_Id_Destino
                                                            , w_UGE_Id_Destino);

                return dsRetorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet GerarExportacaoSintetica(int w_OrgaoId
                                                            , int r_UGECodigo = 0
                                                            , int r_UGEDescricao = 0
                                                            , int r_GrupoMaterial = 0
                                                            , int r_SubitemCodigo = 0
                                                            , int r_SubitemDescricao = 0
                                                            , int r_UnidadeFornecimento = 0
                                                            , int r_QtdeSaldo = 0
                                                            , int r_ValorSaldo = 0
                                                            , int r_PrecoMedio = 0
                                                            , int r_LoteDescricao = 0
                                                            , int r_LoteQtde = 0
                                                            , int r_LoteDataVencimento = 0
                                                            , int r_NaturezaDespesaCodigo = 0
                                                            , int r_NaturezaDespesaDescricao = 0
                                                            , string w_AnoMesRefDe = ""
                                                            , string w_AnoMesRefAte = ""
                                                            , string w_GrupoMaterial = ""
                                                            , string w_UgeCodigo = ""
                                                            , int w_ComSemSaldo = 0)
        {
            DataSet dsRetorno = new DataSet();
            try
            {


                Sam.Business.MovimentoBusiness business = new Sam.Business.MovimentoBusiness();

                dsRetorno = business.GerarExportacaoSintetico(w_OrgaoId
                                                            , r_UGECodigo
                                                            , r_UGEDescricao
                                                            , r_GrupoMaterial 
                                                            , r_SubitemCodigo 
                                                            , r_SubitemDescricao 
                                                            , r_UnidadeFornecimento 
                                                            , r_QtdeSaldo 
                                                            , r_ValorSaldo 
                                                            , r_PrecoMedio 
                                                            , r_LoteDescricao 
                                                            , r_LoteQtde 
                                                            , r_LoteDataVencimento 
                                                            , r_NaturezaDespesaCodigo 
                                                            , r_NaturezaDespesaDescricao 
                                                            , w_AnoMesRefDe 
                                                            , w_AnoMesRefAte 
                                                            , w_GrupoMaterial 
                                                            , w_UgeCodigo
                                                            , w_ComSemSaldo); 

                return dsRetorno;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ExecutarCorrecao(object correcao)
        {
            MovimentoBusiness business = new MovimentoBusiness();

            try
            {

                business.CorrigirSubItem(correcao as MovimentoItemEntity);
                listaMovimentoItem = business.listaMovimentoItem;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void recalculaSubtItem(Int32? almoxarifadoId, Int64? subtItemCodigo, int gestorId)
        {
            if (almoxarifadoId.Value < 1)
            {
                this.View.ExibirMensagem("Favor informar o almoxarifado!");
                return;
            }

            businessMovimentoPendente.recalculaSubtItem(almoxarifadoId, subtItemCodigo, gestorId);
        }

        public IList<MovimentoEntity> ListarNotasdeFornecimento(string palavraChave, int tipoMovimento, string tipoOperacao, string dataDigitada)
        {
            int tipoOper = (int)Common.Util.GeralEnum.TipoRequisicao.Estorno;
            string mesReferencia = string.Empty;
            string mesRef = tipoOperacao == Convert.ToString(tipoOper) ? Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef : string.Empty;
            if ((tipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente) && int.Parse( tipoOperacao) == (int)Common.Util.GeralEnum.TipoRequisicao.Nova)
            {
                mesReferencia = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            }
            else
            {
                mesReferencia = mesRef;
                dataDigitada = null;

            }
           
            int idAlmox = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            
            MovimentoBusiness business = new MovimentoBusiness();
            return business.ListarNotasdeFornecimento(palavraChave, tipoMovimento, idAlmox, mesReferencia, dataDigitada);
        }

        public TB_UGE RetornarMovimentoAlmoxUge(string numeroDocumento)
        {
            Sam.Business.MovimentoBusiness business = new Sam.Business.MovimentoBusiness();
            return business.RetornarMovimentoAlmoxUge(numeroDocumento);
        }

        public TB_MOVIMENTO RetornarMovimentoSaidaFornecimento(int divisao, string numeroDocumento)
        {
            Sam.Business.MovimentoBusiness business = new Sam.Business.MovimentoBusiness();
            return business.RetornarMovimentoSaidaFornecimento(divisao, numeroDocumento);
        }

        public IList<MovimentoEntity> ImprimirConsultaConsulmoAlmox(long _subitemCodigo, string _dataInicio, string _dataFinal, int _gestorId)
        {
            MovimentoBusiness _movBusiness = new MovimentoBusiness();
            return _movBusiness.ImprimirConsultaConsulmoAlmox(_subitemCodigo, _dataInicio, _dataFinal, _gestorId);
        }

        public IList<MovimentoEntity> ImprimirConsultaTransferencia(int _almoxId, DateTime _dataInicial, DateTime _dataFinal)
        {
            MovimentoBusiness _movBusiness = new MovimentoBusiness();
            return _movBusiness.ImprimirConsultaTransferencia(_almoxId, _dataInicial, _dataFinal);
        }

        public MovimentoEntity ObterMovimentacao(int movID, bool isEmpenho = false)
        {
            MovimentoEntity objMovimentacaoMaterial = null;
            EmpenhoBusiness objBusiness = null;


            objBusiness = new EmpenhoBusiness();
            objMovimentacaoMaterial = objBusiness.ObterMovimento(movID, isEmpenho);

            return objMovimentacaoMaterial;
        }

        public bool VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(MovimentoEntity movimentacaoMaterial, MovimentoItemEntity itemMovimentacao)
        {
            TipoMaterialParaLancamento tipoMaterialSubitem = TipoMaterialParaLancamento.Indeterminado;
            TipoMaterialParaLancamento tipoMaterialMovimentacao = TipoMaterialParaLancamento.Indeterminado;
            if (movimentacaoMaterial.MovimentoItem.HasElements())
            {
                tipoMaterialMovimentacao = movimentacaoMaterial.ObterTipoMaterial();
                tipoMaterialSubitem = itemMovimentacao.SubItemMaterial.ObterTipoMaterial();
            }

            return (tipoMaterialMovimentacao != tipoMaterialSubitem);
        }

        public decimal ConsultarSaldoTotalAlmoxarifado(int idAlmoxarifado) {

            MovimentoBusiness estrutura = new MovimentoBusiness();
            return estrutura.ConsultarSaldoTotalAlmoxarifado(idAlmoxarifado);
        }

        public decimal ConsultarSaldoTotalAlmoxarifado33(int idAlmoxarifado)
        {

            MovimentoBusiness estrutura = new MovimentoBusiness();
            return estrutura.ConsultarSaldoTotalAlmoxarifado33(idAlmoxarifado);
        }

        public decimal ConsultarSaldoTotalAlmoxarifado44(int idAlmoxarifado)
        {

            MovimentoBusiness estrutura = new MovimentoBusiness();
            return estrutura.ConsultarSaldoTotalAlmoxarifado44(idAlmoxarifado);
        }
    }


}
