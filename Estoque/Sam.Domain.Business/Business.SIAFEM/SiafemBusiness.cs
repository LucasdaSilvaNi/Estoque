using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Common;
using Sam.Common.Enums;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Integracao.SIAF.Core;
using Sam.Domain.Business.Auditoria;
using Sam.Integracao.SIAF.Configuracao;
using System.Transactions;
using Sam.Integracao.SIAF.Simulacra;
using Sam.ServiceInfraestructure;
using tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoLancamentoSIAFEM = Sam.Common.Util.GeralEnum.TipoLancamentoSIAF;
using enumSistemaSIAF = Sam.Common.Util.GeralEnum.SistemaSIAF;
using TipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;



namespace Sam.Domain.Business.SIAFEM
{
    public class SiafemBusiness : BaseBusiness
    {
        private string erroProcessamentoSIAF;
        public string ErroProcessamentoWs
        {
            get { return erroProcessamentoSIAF; }
            set { erroProcessamentoSIAF = value; }
        }

        private List<string> gerarEstimuloNotaLancamentoSIAF(string tipo, MovimentoEntity movimentacaoMaterial, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao, string inscricaoCE = null)
        {
            if (movimentacaoMaterial.IsNull())
                throw new NullReferenceException("Movimentação de Materiais nula passada ao método.");

            #region Variaveis
            List<string> estimuloPagamentoSiafem = null;
            int? tipoMovimentacao = null;
            int ugeCodigo = 0;
            int codigoGestao = 0;
            string ugeFavorecida = "0";
            int gestaoFavorecida = 0;
            string strDataEmissao = null;
            string anoMesReferencia = null;
            decimal valorDocumento = 0m;
            IList<string> observacoesMovimentacao = null;
            string[] arrObservacoesMovimentacao = null;
            string[] codigosEvento = null;
            string[] codigosEventoEstorno = null;
            string[] codigosInscricaoEvento = null;
            string[] codigosClassificacaoEvento = null;
            string[] codigosRecDesp = null;

            string primeiroCodigoInscricao = null;
            string segundoCodigoInscricao = null;
            string terceiroCodigoInscricao = null;
            string observacaoSiafem = null;
            EventosPagamentoEntity eventosPagamentoSiafem = null;
            EventoSiafemEntity eventoSiafem = null;
            EventosPagamentoBusiness objBusiness = null;

            bool ehEstorno = false;
            bool preencherUGEFavorecida = false;
            bool persistirLogTxt = true;
            bool ehReclassificacao = false;
            #endregion

            var movimentacaoValida = (movimentacaoMaterial.IsNotNull() && movimentacaoMaterial.TipoMovimento.IsNotNull() && movimentacaoMaterial.TipoMovimento.Id != 0);
            if (movimentacaoValida)
            {
                estimuloPagamentoSiafem = new List<string>();
                movimentacaoMaterial.TipoMaterial = movimentacaoMaterial.ObterTipoMaterial();
                tipoMovimentacao = ((movimentacaoMaterial.IsNotNull() && movimentacaoMaterial.TipoMovimento.IsNotNull() && movimentacaoMaterial.TipoMovimento.Id != 0) ? movimentacaoMaterial.TipoMovimento.Id : (new int?()));
                ugeCodigo = movimentacaoMaterial.UGE.Codigo.Value;
                codigoGestao = movimentacaoMaterial.Almoxarifado.Gestor.CodigoGestao.Value;
                strDataEmissao = movimentacaoMaterial.DataMovimento.Value.ToString("ddMMMyyyy").ToUpperInvariant();
                anoMesReferencia = movimentacaoMaterial.Almoxarifado.MesRef.Substring(0, 4);
                valorDocumento = movimentacaoMaterial.ValorDocumento.Value;
                //ehEstorno = !movimentacaoMaterial.Ativo.Value;
                ehEstorno = (tipo.ToUpperInvariant() == "E");

                observacaoSiafem = EnumUtils.GetEnumExtraDescription<tipoMovimento>((tipoMovimento)movimentacaoMaterial.TipoMovimento.Id);
                observacoesMovimentacao = movimentacaoMaterial.Observacoes.SplitOn(Constante.CST_COMPRIMENTO_MAXIMO_CAMPO_OBSERVACAO_SIAFEM, true).ToList();

                //Truncamento de array string (max length 77 chr) gerado por campo TB_MOVIMENTO.TB_MOVIMENTO_OBSERVACOES, caso gere mais de 2 linhas
                if (observacoesMovimentacao.Count() > 2)
                    observacoesMovimentacao.RemoveAt(2);

                observacoesMovimentacao.Insert(0, observacaoSiafem);
                arrObservacoesMovimentacao = observacoesMovimentacao.ToArray();

                //if (((tipoMovimentacao == (int)tipoMovimento.EntradaPorEmpenho) || (tipoMovimentacao == (int)tipoMovimento.EntradaPorRestosAPagar) || (tipoMovimentacao == (int)tipoMovimento.ConsumoImediatoEmpenho)) && (TipoNotaSIAF == TipoNotaSIAF.NL_Liquidacao))
                if (((tipoMovimentacao == (int)tipoMovimento.EntradaPorEmpenho) || (tipoMovimentacao == (int)tipoMovimento.EntradaPorRestosAPagar) || (tipoMovimentacao == (int)tipoMovimento.ConsumoImediatoEmpenho) || (tipoMovimentacao == (int)tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)) && (TipoNotaSIAF == TipoNotaSIAF.NL_Liquidacao))
                {
                    //if (movimentacaoMaterial.ObterTipoMaterialEmpenho() == TipoMaterial.MaterialConsumo)
                    //#region Validacao contra MaterialPermanente (temporario)
                    //if (movimentacaoMaterial.ObterTipoMaterial(true) == TipoMaterial.MaterialConsumo)
                    estimuloPagamentoSiafem.Add(GeradorEstimuloSIAF.SiafemDocNLEmLiq(ugeCodigo, codigoGestao, strDataEmissao, movimentacaoMaterial.Empenho, movimentacaoMaterial.NaturezaDespesaEmpenho, movimentacaoMaterial.NumeroDocumento, arrObservacoesMovimentacao, valorDocumento, tipo, movimentacaoMaterial.Id.Value));
                    //else
                        //throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor entrar em contato com CSCC/SEFAZ.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                        //throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor utilizar a funcionalidade de inclusão manual.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                    //#endregion
                }
                else if ((tipoMovimentacao == (int)tipoMovimento.EntradaPorTransferencia ||
                         (tipoMovimentacao == (int)tipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado) ||
						 (tipoMovimentacao == (int)tipoMovimento.EntradaInventario)))
                {
                    estimuloPagamentoSiafem = null;
                }
                else
                {
                    objBusiness = new EventosPagamentoBusiness();

                    if (movimentacaoMaterial.SubTipoMovimentoId != null)
                    {
                        string SubTipoDesc = movimentacaoMaterial.TipoMovimento.SubTipoMovimentoItem != null ? movimentacaoMaterial.TipoMovimento.SubTipoMovimentoItem.Descricao : string.Empty;
                        eventosPagamentoSiafem = objBusiness.ObterEventoSiafem(movimentacaoMaterial);


                        if (eventosPagamentoSiafem.IsNotNull())
                        {
                            if (eventosPagamentoSiafem.NlPatrimonial && eventosPagamentoSiafem.EstimuloAtivo)
                            {

                                #region Campos Info Transferencia


                                if ((movimentacaoMaterial.MovimAlmoxOrigemDestino.IsNotNull() &&
                                        movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.IsNotNull() &&
                                        movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.IsNotNull()))
                                {
                                    preencherUGEFavorecida = true;
                                    int uge = movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value;
                                    ugeFavorecida = uge.ToString("D6");
                                    gestaoFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.CodigoGestao.Value;
                                }
                                else if (!string.IsNullOrEmpty(movimentacaoMaterial.UgeCPFCnpjDestino))
                                    ugeFavorecida = movimentacaoMaterial.UgeCPFCnpjDestino;

                                #endregion
                                #region Campos Info Saída Por Transferencia Para Almoxarifado Não Implantado
                                if (movimentacaoMaterial.TipoMovimento.Id == tipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode()
                                    && (movimentacaoMaterial.MovimAlmoxOrigemDestino.IsNotNull() &&
                                        movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.IsNotNull() &&
                                        movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.IsNotNull()))
                                {
                                    preencherUGEFavorecida = true;

                                    if (movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value == 0)
                                        ugeFavorecida = movimentacaoMaterial.GeradorDescricao.Split('/')[1].Trim();
                                    //int.TryParse(movimentacaoMaterial.GeradorDescricao.Split('/')[1].Trim(), out ugeFavorecida);
                                    else
                                        ugeFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value.ToString();

                                    gestaoFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.CodigoGestao.Value;
                                }
                                #endregion

                                Dictionary<string, decimal?> Item = new Dictionary<string, decimal?>();


                                string Observacao = "MovId= " + movimentacaoMaterial.Id.ToString() + " GERADO ATRAVÉS DO SAM";
                                string DtMovimento = movimentacaoMaterial.DataMovimento.Value.ToShortDateString();

                                int cont = 0;
                                int itemMax = 39;
                                int indexCont = movimentacaoMaterial.MovimentoItem.Count - 1;
                                int conttotal = indexCont > itemMax ? itemMax : indexCont;

                                foreach (var item in movimentacaoMaterial.MovimentoItem)
                                {

                                    Item.Add(item.SubItemMaterial.ItemMaterial.Codigo.ToString(), item.ValorMov);

                                    if (cont == conttotal)
                                    {

                                        cont = 0;
                                        conttotal = indexCont - Item.Count > itemMax ? itemMax : indexCont - Item.Count;


                                        estimuloPagamentoSiafem.Add(GeradorEstimuloSIAF.SiafemDocNLPatrimonial(ugeCodigo, codigoGestao,
                                                                           DtMovimento, movimentacaoMaterial.NumeroDocumento, Observacao, movimentacaoMaterial.InscricaoCE,
                                                                           preencherUGEFavorecida, ugeFavorecida, gestaoFavorecida, ehEstorno,
                                                                           eventosPagamentoSiafem.EventoSiafemItem, Item));
                                        Item = new Dictionary<string, decimal?>();

                                    }
                                    else
                                        conttotal--;


                                }



                            }
                            else
                            {
                                throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM. Favor verificar SubTipo {1}.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), SubTipoDesc));
                            }
                        }
                        else
                            throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM. Favor verificar SubTipo {1}.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), SubTipoDesc));

                    }
                    else
                    {

                        eventosPagamentoSiafem = objBusiness.ObterEventoPagamento(movimentacaoMaterial);

                    if (TipoNotaSIAF == TipoNotaSIAF.NL_Liquidacao)
                        eventosPagamentoSiafem = objBusiness.ObterEventoPagamento(movimentacaoMaterial);
                    else if (TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao)
                        eventosPagamentoSiafem = objBusiness.ObterEventoPagamentoReclassificacao(movimentacaoMaterial, inscricaoCE);


                    if (eventosPagamentoSiafem.IsNotNull() && eventosPagamentoSiafem.Ativo)
                    {

                        #region Campos Info Transferencia
                        //TODO [CORRECAO FASE 2] - Gerar método .ObterUGE() e transportar valor da UI de UGE/Gestão até camada de negócios
                        preencherUGEFavorecida = eventosPagamentoSiafem.UGFavorecida;

                        if (preencherUGEFavorecida
                            && (movimentacaoMaterial.MovimAlmoxOrigemDestino.IsNotNull() &&
                                movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.IsNotNull() &&
                                movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.IsNotNull()))
                        {
                              int uge = movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value;
                                ugeFavorecida = uge.ToString("D6");
                            gestaoFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.CodigoGestao.Value;
                        }
                        #endregion


                        #region Campos Info Saída Por Transferencia Para Almoxarifado Não Implantado
                        if (movimentacaoMaterial.TipoMovimento.Id == tipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode()
                            && (movimentacaoMaterial.MovimAlmoxOrigemDestino.IsNotNull() &&
                                movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.IsNotNull() &&
                                movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.IsNotNull()))
                        {
                            preencherUGEFavorecida = true;

                            if (movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value == 0)
                                ugeFavorecida = movimentacaoMaterial.GeradorDescricao.Split('/')[1].Trim();
                                //int.TryParse(movimentacaoMaterial.GeradorDescricao.Split('/')[1].Trim(), out ugeFavorecida);
                            else
                                 ugeFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value.ToString();

                            gestaoFavorecida = movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.CodigoGestao.Value;
                        }
                        #endregion

                        #region Binding CE
                        //Mesmo campo CE sendo de preenchimento obrigatório agora, checagem será mantida.
                        if (!String.IsNullOrWhiteSpace(movimentacaoMaterial.InscricaoCE))
                        {
                            if (@TipoNotaSIAF == TipoNotaSIAF.NL_Liquidacao)
                            {
                                //Não + CE
                                if (eventosPagamentoSiafem.PrimeiraInscricao.ToUpperInvariant() == "NÃO" &&
                                    eventosPagamentoSiafem.SegundaInscricao.ToUpperInvariant() == "CE")
                                {
                                    primeiroCodigoInscricao = null;
                                    segundoCodigoInscricao = movimentacaoMaterial.InscricaoCE;
                                }
                                //CE + Não
                                else if (eventosPagamentoSiafem.PrimeiraInscricao.ToUpperInvariant() == "CE" &&
                                         eventosPagamentoSiafem.SegundaInscricao.ToUpperInvariant() == "NÃO")
                                {
                                    primeiroCodigoInscricao = movimentacaoMaterial.InscricaoCE;
                                    segundoCodigoInscricao = null;
                                }
                                //'CE Padrao' + CE
                                else if (eventosPagamentoSiafem.PrimeiraInscricao.ToUpperInvariant() == "CE PADRÃO" && eventosPagamentoSiafem.SegundaInscricao.ToUpperInvariant() == "CE")
                                {
                                    primeiroCodigoInscricao = Constante.CST_SIAFEM_CE_PADRAO;
                                    segundoCodigoInscricao = movimentacaoMaterial.InscricaoCE;
                                }
                                //CE + 'CE Padrao'
                                else if (eventosPagamentoSiafem.PrimeiraInscricao.ToUpperInvariant() == "CE" && eventosPagamentoSiafem.SegundaInscricao.ToUpperInvariant() == "CE PADRÃO")
                                {
                                    primeiroCodigoInscricao = movimentacaoMaterial.InscricaoCE;
                                    segundoCodigoInscricao = Constante.CST_SIAFEM_CE_PADRAO;
                                }
                            }
                            else if (@TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao)
                            {
                                primeiroCodigoInscricao = eventosPagamentoSiafem.PrimeiraInscricao;
                                segundoCodigoInscricao = eventosPagamentoSiafem.SegundaInscricao;
                                terceiroCodigoInscricao = eventosPagamentoSiafem.TerceiroInscricao;
                            }
                        }
                        #endregion

                        #region Binding Arrays Codigos

                        if (eventosPagamentoSiafem.TerceiroCodigo.ToString() == "0")
                        {
                            codigosEvento = new string[] { eventosPagamentoSiafem.PrimeiroCodigo.ToString(), eventosPagamentoSiafem.SegundoCodigo.ToString() };
                            codigosEventoEstorno = new string[] { eventosPagamentoSiafem.PrimeiroCodigoEstorno.ToString(), eventosPagamentoSiafem.SegundoCodigoEstorno.ToString() };
                            codigosInscricaoEvento = new string[] { primeiroCodigoInscricao, segundoCodigoInscricao };
                            codigosClassificacaoEvento = new string[] { eventosPagamentoSiafem.PrimeiraClassificacao, eventosPagamentoSiafem.SegundaClassificacao };
                            codigosRecDesp = new string[] { null, null };
                        }
                        else
                        {
                            codigosEvento = new string[] { eventosPagamentoSiafem.PrimeiroCodigo.ToString(), eventosPagamentoSiafem.SegundoCodigo.ToString(), eventosPagamentoSiafem.TerceiroCodigo.ToString() };
                            codigosEventoEstorno = new string[] { eventosPagamentoSiafem.PrimeiroCodigoEstorno.ToString(), eventosPagamentoSiafem.SegundoCodigoEstorno.ToString(), eventosPagamentoSiafem.TerceiroCodigoEstorno.ToString() };
                            codigosInscricaoEvento = new string[] { primeiroCodigoInscricao, null, terceiroCodigoInscricao };
                            codigosClassificacaoEvento = new string[] { eventosPagamentoSiafem.PrimeiraClassificacao, eventosPagamentoSiafem.SegundaClassificacao, null };
                            codigosRecDesp = new string[] { null, null, movimentacaoMaterial.MovimentoItem[0].NaturezaDespesaCodigo.ToString() };
                        }

                        if (ehEstorno)
                            codigosEvento = codigosEventoEstorno;
                            #endregion

                            estimuloPagamentoSiafem.Add(GeradorEstimuloSIAF.SiafemDocNL(ugeCodigo, codigoGestao, strDataEmissao, movimentacaoMaterial.NumeroDocumento, codigosEvento, codigosInscricaoEvento, codigosClassificacaoEvento, arrObservacoesMovimentacao, valorDocumento, tipo, movimentacaoMaterial.Id.Value, preencherUGEFavorecida, ugeFavorecida, gestaoFavorecida, @TipoNotaSIAF, codigosRecDesp));
                    }
                        //else
                        else if (eventosPagamentoSiafem.IsNotNull() && (eventosPagamentoSiafem.TipoMaterialAssociado != (int)movimentacaoMaterial.TipoMaterial))
                        {
                            //throw new InvalidOperationException(String.Format(@"Eventos para lançamento de movimentação SAM, do tipo ""{0}"" no SIAFEM, não cadastrados no sistema", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial)));
                            //throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor entrar em contato com CSCC/SEFAZ.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                            throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor utilizar a funcionalidade de inclusão manual.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                        }
                        else if (eventosPagamentoSiafem.IsNull() || (eventosPagamentoSiafem.IsNotNull() && !eventosPagamentoSiafem.Ativo))
                        {
                            //throw new InvalidOperationException(String.Format(@"SAM|Eventos para lançamento de movimentação SAM, do tipo ""{0}"" no SIAFEM, não cadastrados (ou inativos) no sistema, para o ano {1}. Favor entrar em contato com CAU/SEFAZ.", movimentacaoMaterial.TipoMovimento.Descricao, anoMesReferencia));
                            //throw new InvalidOperationException(String.Format(@"SAM|Evento SIAFEM para {0} do tipo ""{1}"", não cadastrado ou inativo no sistema SAM, para o ano {2}. Favor entrar em contato com CSCC/SEFAZ.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), movimentacaoMaterial.TipoMovimento.Descricao, anoMesReferencia));
                            throw new InvalidOperationException(String.Format(@"SAM|Evento SIAFEM para {0} do tipo ""{1}"", não cadastrado ou inativo no sistema SAM, para o ano {2}. Favor utilizar a funcionalidade de inclusão manual.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), movimentacaoMaterial.TipoMovimento.Descricao, anoMesReferencia));
                        }
                    }

                    //TODO [EVOLUCAO INFRA] Criar Metodo Padrao de Persistencia de LOGs, e mover trecho para camada de infra.
                    //if (persistirLogTxt)
                    //{
                    //    System.IO.StreamWriter arquivoLog = System.IO.File.CreateText(String.Format(Constante.FullPhysicalPathApp.Replace(@"\\", @"\") + @"{0}\" + @"\{1}_{2}_{3}.LOG", "LOG", "chamadaWS_NL", movimentacaoMaterial.UGE.Codigo, movimentacaoMaterial.NumeroDocumento));
                    //    arquivoLog.Write(estimuloPagamentoSiafem);
                    //    arquivoLog.Flush();
                    //    arquivoLog.Close();
                    //}
                }
            }

            return estimuloPagamentoSiafem;
        }
        private string gerarEstimuloConsultaMonitoramentoSIAF(string strChaveConsulta, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            string estimuloWsConsultaMonitoramentoSIAF = null;

            estimuloWsConsultaMonitoramentoSIAF = GeradorEstimuloSIAF.SiafMonitora(strChaveConsulta);

            return estimuloWsConsultaMonitoramentoSIAF;
        }
        private string obterNotaLancamentoSIAF(string loginUsuarioSAM, string loginSiafemUsuario, string senhaSiafemUsuario, string tipoLancamento, MovimentoEntity movimentacaoMaterial, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao, string inscricaoCE = null)
        {
            #region Variaveis
            string strRetornoWS = null;
            string strNoLanSIAF = null;
            //string documentoSAM = null;
            List<string> estimuloNotaLancamentoSIAF = null;
            string anoBasePagamento = null;
            string ugeGestora = null;
            AuditoriaIntegracaoEntity registroAuditoria = null;
            ProcessadorServicoSIAF procWsSiafem = null;
            #endregion

            anoBasePagamento = movimentacaoMaterial.AnoMesReferencia.Substring(0, 4);
            ugeGestora = movimentacaoMaterial.UGE.Codigo.ToString();

            try
            {
                estimuloNotaLancamentoSIAF = this.gerarEstimuloNotaLancamentoSIAF(tipoLancamento, movimentacaoMaterial, @TipoNotaSIAF, movimentacaoMaterial.InscricaoCE);

                if (estimuloNotaLancamentoSIAF.IsNotNullAndNotEmpty())
                {

                    foreach (var item in estimuloNotaLancamentoSIAF)
                    {

                        procWsSiafem = new ProcessadorServicoSIAF();
                        procWsSiafem.ConsumirWS(loginSiafemUsuario, senhaSiafemUsuario, anoBasePagamento, ugeGestora, item, false);

                        //Auditoria Integracao SIAFEM
                        registroAuditoria = gerarRegistroAuditoria(loginUsuarioSAM, loginSiafemUsuario, procWsSiafem);
                        inserirRegistroAuditoriaIntegracao(registroAuditoria);
                        if (!procWsSiafem.ErroProcessamentoWs)
                        {
                            strRetornoWS = procWsSiafem.RetornoWsSIAF;
                            strNoLanSIAF = obterNLSiafem(strRetornoWS);
                        }
                        else
                        {
                            gerarNotaLancamentoPendencia(registroAuditoria, movimentacaoMaterial, procWsSiafem.ErroRetornoWs, @TipoNotaSIAF);
                            throw new Exception(procWsSiafem.ErroRetornoWs);
                        }

                    }
                    
                }
            }
            catch (InvalidOperationException excErroGeracaoEstimuloWsSIAF)
            {
                var erroProcessamento = excErroGeracaoEstimuloWsSIAF.Message;
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, movimentacaoMaterial);

                throw new Exception(erroProcessamento);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var msgErro = String.Format("{0}|{1}", procWsSiafem.NomeSistemaSIAF, excErroProcessamentoWsSIAF.Message);

                GerarNotaLancamentoPendenciaDeException(msgErro, loginUsuarioSAM, loginSiafemUsuario, movimentacaoMaterial);
                throw new Exception(msgErro);
            }

            return strNoLanSIAF;
        }
        private string obterNotaLancamentoSIAF(string loginSiafemUsuario, string senhaSiafemUsuario, string tipo, NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao, string inscricaoCE = null)
        {
            #region Variaveis
            string strRetornoWS = null;
            string nlSiafem = null;
            string documentoSAM = null;
            string hdrMensagemErroSIAF = null;
            List<string> estimuloNotaLancamentoSIAF = null;
            string anoBasePagamento = null;
            string ugeGestora = null;
            string loginUsuarioSAM = null;
            AuditoriaIntegracaoEntity registroAuditoria = null;
            ProcessadorServicoSIAF procWsSiafem = null;
            #endregion


            hdrMensagemErroSIAF = "Erro ao gerar NL SIAFEM para Movimentação SAM";
            var movimentacaoMaterial = notaLancamentoPendente.MovimentoVinculado;
            documentoSAM = movimentacaoMaterial.NumeroDocumento;
            loginUsuarioSAM = notaLancamentoPendente.AuditoriaIntegracaoVinculada.UsuarioSAM;
            anoBasePagamento = movimentacaoMaterial.AnoMesReferencia.Substring(0, 4);
            ugeGestora = movimentacaoMaterial.UGE.Codigo.ToString();

            try
            {
                estimuloNotaLancamentoSIAF = gerarEstimuloNotaLancamentoSIAF(tipo, notaLancamentoPendente.MovimentoVinculado, @TipoNotaSIAF, inscricaoCE);

                if (estimuloNotaLancamentoSIAF.IsNotNullAndNotEmpty())
                {

                    foreach (var item in estimuloNotaLancamentoSIAF)
                    {

                        procWsSiafem = new ProcessadorServicoSIAF();
                        procWsSiafem.Configuracoes = new ConfiguracoesSIAF() { ExibeMsgErroCompleta = false };
                        procWsSiafem.ConsumirWS(loginSiafemUsuario, senhaSiafemUsuario, anoBasePagamento, ugeGestora, item, false);

                        //Auditoria Integracao SIAFEM
                        registroAuditoria = gerarRegistroAuditoria(loginUsuarioSAM, loginSiafemUsuario, procWsSiafem);
                        inserirRegistroAuditoriaIntegracao(registroAuditoria);
                        InativarPendenciasPorMovimentacao(movimentacaoMaterial.Id.Value);

                        if (!procWsSiafem.ErroProcessamentoWs)
                        {
                            strRetornoWS = procWsSiafem.RetornoWsSIAF;
                            nlSiafem = obterNLSiafem(strRetornoWS);
                        }
                        else
                        {
                            gerarNotaLancamentoPendencia(registroAuditoria, movimentacaoMaterial, procWsSiafem.ErroRetornoWs, @TipoNotaSIAF);
                            throw new Exception(procWsSiafem.ErroRetornoWs);
                        }
                    }
                }

            }
            catch (InvalidOperationException excErroGeracaoEstimuloWsSIAF)
            {
                var erroProcessamento = String.Format("Operação inválida em processamento de dados com servidores SEFAZ [{0}]", excErroGeracaoEstimuloWsSIAF.Message);
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, movimentacaoMaterial);

                throw new Exception(erroProcessamento);
            }
            catch (TimeoutException excTimeoutConexao)
            {
                var erroProcessamento = String.Format("Timeout em conexão com servidores SEFAZ [{0}]", excTimeoutConexao.Message);
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, movimentacaoMaterial);

                throw new Exception(erroProcessamento); 
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var complementoMsgErro = String.Format("SIAFEM|{0}", procWsSiafem.ErroRetornoWs);
                var msgErro = String.Format("{0} {1} - {2}", hdrMensagemErroSIAF, documentoSAM, complementoMsgErro);
                if (ListaErro.IsNotNullAndNotEmpty())
                    this.ListaErro.Add(msgErro);
                else
                    this.ListaErro = new List<string>() { msgErro };

                GerarNotaLancamentoPendenciaDeException(msgErro, loginUsuarioSAM, loginSiafemUsuario, movimentacaoMaterial);

                throw new Exception(msgErro, excErroProcessamentoWsSIAF);
            }

            return nlSiafem;
        }

        private void GerarNotaLancamentoPendenciaDeException(string erroProcessamento, string loginUsuarioSAM, string loginSiafemUsuario, MovimentoEntity movimentacaoMaterial)
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;


            //registroAuditoria = new AuditoriaIntegracaoEntity() { NomeSistema = "SIAFEM", DataEnvio = DateTime.Now, MsgEstimuloWS = "\0", MsgRetornoWS = "Erro ao gerar NL SIAFEM para Movimentação SAM", UsuarioSAM = loginUsuarioSAM, UsuarioSistemaExterno = loginSiafemUsuario };
            //erroProcessamento = erroProcessamento.BreakLine("|", 1);
            string[] infosErroProcessamento = erroProcessamento.BreakLine("|");
            if (!infosErroProcessamento.HasElements())
            {
                erroProcessamentoSIAF = "Erro de sistema não identificado (na geração de NL SIAFEM).";
            }
            else if (infosErroProcessamento.Count() == 2)
            {
                var descErroSistema = infosErroProcessamento[0];
                var msgErroDetalhada = infosErroProcessamento[1];
                erroProcessamentoSIAF = String.Format("{0}: {1}", descErroSistema, msgErroDetalhada);
            }
            else if (infosErroProcessamento.Count() == 1)
            {
                erroProcessamentoSIAF = infosErroProcessamento[0];
            }

            registroAuditoria = new AuditoriaIntegracaoEntity() { NomeSistema = "SIAFEM", DataEnvio = DateTime.Now, MsgEstimuloWS = "\0", MsgRetornoWS = erroProcessamentoSIAF, UsuarioSAM = loginUsuarioSAM, UsuarioSistemaExterno = loginSiafemUsuario };

            InativarPendenciasPorMovimentacao(movimentacaoMaterial.Id.Value);
            inserirRegistroAuditoriaIntegracao(registroAuditoria);
            gerarNotaLancamentoPendencia(registroAuditoria, movimentacaoMaterial, erroProcessamentoSIAF);
        }


        private string obterNotaLancamentoDoMonitoramentoSIAF(string loginSiafemUsuario, string senhaSiafemUsuario, string tipoLancamento, NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            #region Variaveis
            string estimuloWsMonitoramentoSIAF = null;
            string anoBasePagamento = null;
            string ugeGestora = null;
            string strRetornoWS = null;
            string strNoLanSIAF = null;
            string strChaveConsulta = null;
            string loginUsuarioSAM = null;
            string tipoNotaSIAF = null;
            string hdrMensagemErroSIAF = null;
            string documentoSAM = null;
            AuditoriaIntegracaoEntity registroAuditoria = null;

            ProcessadorServicoSIAF procWsSiafem = null;
            #endregion

            try
            {
                if (@TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao)
                    tipoNotaSIAF = "R";

                hdrMensagemErroSIAF = "Erro ao gerar obter NL SIAFEM do WS de monitoramento, para Movimentação SAM";
                strChaveConsulta = String.Format("{0}{1}_samMovID:{2}", tipoLancamento, tipoNotaSIAF, notaLancamentoPendente.MovimentoVinculado.Id);

                documentoSAM = notaLancamentoPendente.MovimentoVinculado.NumeroDocumento;
                ugeGestora = notaLancamentoPendente.MovimentoVinculado.UGE.Codigo.Value.ToString();
                anoBasePagamento = notaLancamentoPendente.MovimentoVinculado.AnoMesReferencia.Substring(0, 4);
                //estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta);

                //#region Validacao contra MaterialPermanente (temporario)
                MovimentoEntity movimentacaoMaterial = notaLancamentoPendente.MovimentoVinculado;
                string anoMesReferencia = notaLancamentoPendente.MovimentoVinculado.Almoxarifado.MesRef.Substring(0, 4);

                //if (movimentacaoMaterial.ObterTipoMaterial(true) == TipoMaterial.MaterialConsumo)
                    estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta, notaLancamentoPendente.TipoNotaSIAF);
                //else
                //    //throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor entrar em contato com CSCC/SEFAZ.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                //    throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor utilizar a funcionalidade de inclusão manual.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                //#endregion

                if (!String.IsNullOrWhiteSpace(estimuloWsMonitoramentoSIAF))
                {
                    procWsSiafem = new ProcessadorServicoSIAF();
                    procWsSiafem.SistemaSIAF = GeralEnum.SistemaSIAF.SIAFEM;
                    procWsSiafem.ConsumirWS(anoBasePagamento, ugeGestora, estimuloWsMonitoramentoSIAF, true, true);

                    //Auditoria Integracao SIAFEM
                    loginUsuarioSAM = notaLancamentoPendente.AuditoriaIntegracaoVinculada.UsuarioSAM;
                    registroAuditoria = gerarRegistroAuditoria(loginUsuarioSAM, loginSiafemUsuario, procWsSiafem);
                    inserirRegistroAuditoriaIntegracao(registroAuditoria);

                    InativarPendenciasPorMovimentacao(notaLancamentoPendente.MovimentoVinculado.Id.Value);
                    if (!procWsSiafem.ErroProcessamentoWs)
                    {
                        strRetornoWS = procWsSiafem.RetornoWsSIAF;
                        strNoLanSIAF = obterNLSiafemDoMonitoramento(strRetornoWS);
                    }
                    else
                    {
                        gerarNotaLancamentoPendencia(registroAuditoria, notaLancamentoPendente.MovimentoVinculado, procWsSiafem.ErroRetornoWs, @TipoNotaSIAF);

                        throw new Exception(procWsSiafem.ErroRetornoWs);
                    }
                }
            }
            catch (InvalidOperationException excErroGeracaoEstimuloWsSIAF)
            {
                var erroProcessamento = String.Format("Operação inválida em processamento de dados com servidores SEFAZ [{0}]", excErroGeracaoEstimuloWsSIAF.Message);
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, notaLancamentoPendente.MovimentoVinculado);

                throw new Exception(erroProcessamento);
            }
            catch (TimeoutException excTimeoutConexao)
            {
                var erroProcessamento = String.Format("Timeout em conexão com servidores SEFAZ [{0}]", excTimeoutConexao.Message);
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, notaLancamentoPendente.MovimentoVinculado);

                throw new Exception(erroProcessamento);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var complementoMsgErro = String.Format("SIAFEM|{0}", procWsSiafem.ErroRetornoWs);
                var msgErro = String.Format(@"{0} {1}\n{2}", hdrMensagemErroSIAF, documentoSAM, complementoMsgErro);
                GerarNotaLancamentoPendenciaDeException(msgErro, loginUsuarioSAM, loginSiafemUsuario, notaLancamentoPendente.MovimentoVinculado);

                throw new Exception(excErroProcessamentoWsSIAF.Message);
            }

            return strNoLanSIAF;
        }
        private bool InativarPendenciasPorMovimentacao(int movimentacaoMaterialID)
        {
            bool statusRetorno = false;
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;

            //Se houver pendências SIAF ativas, tornar inativas
            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            statusRetorno = objBusiness.InativarPendenciasPorMovimentacao(movimentacaoMaterialID);

            if (objBusiness.ListaErro.HasElements() && ListaErro.HasElements())
                ListaErro.AddRange(objBusiness.ListaErro);
            else if (objBusiness.ListaErro.HasElements() && !ListaErro.HasElements())
                ListaErro = objBusiness.ListaErro;

            return statusRetorno;
        }
        private bool VerificaSeMovimentacaoSAMEstaPendenteSIAF(string tipoLancamentoSIAFEM, NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.NL_Liquidacao)
        {
            #region Variaveis
            bool statusRetorno = true;
            string estimuloWsMonitoramentoSIAF = null;
            string anoBasePagamento = null;
            string ugeGestora = null;
            string strRetornoWS = null;
            string strNoLanSIAF = null;
            string strChaveConsulta = null;
            string prefixoTipoNotaSIAF = null;

            ProcessadorServicoSIAF procWsSiafem = null;
            #endregion

            try
            {
                procWsSiafem = new ProcessadorServicoSIAF();

                if ((notaLancamentoPendente.TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao) || (tipoNotaSIAFEM == TipoNotaSIAF.NL_Reclassificacao))
                    prefixoTipoNotaSIAF = "R";

                strChaveConsulta = String.Format("{0}{1}_samMovID:{2}", tipoLancamentoSIAFEM, prefixoTipoNotaSIAF, notaLancamentoPendente.MovimentoVinculado.Id);
                
                ugeGestora = notaLancamentoPendente.MovimentoVinculado.UGE.Codigo.Value.ToString();
                anoBasePagamento = notaLancamentoPendente.MovimentoVinculado.AnoMesReferencia.Substring(0, 4);
                //estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta, notaLancamentoPendente.TipoNotaSIAF);

                //#region Validacao contra MaterialPermanente (temporario)
                MovimentoEntity movimentacaoMaterial = notaLancamentoPendente.MovimentoVinculado;
                string anoMesReferencia = notaLancamentoPendente.MovimentoVinculado.Almoxarifado.MesRef.Substring(0,4);

                //if (movimentacaoMaterial.ObterTipoMaterial(true) == TipoMaterial.MaterialConsumo)
                    estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta, notaLancamentoPendente.TipoNotaSIAF);
                //else
                //    //throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor entrar em contato com CSCC/SEFAZ.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                //    throw new InvalidOperationException(String.Format(@"SAM|Eventos SIAFEM para lançamento deste tipo de material ({0}), não foram cadastrados no SAM, para o ano {1}. Favor utilizar a funcionalidade de inclusão manual.", EnumUtils.GetEnumDescription<TipoMaterial>(movimentacaoMaterial.TipoMaterial), anoMesReferencia));
                //#endregion


                if (!String.IsNullOrWhiteSpace(estimuloWsMonitoramentoSIAF))
                {
                    procWsSiafem = new ProcessadorServicoSIAF();
                    procWsSiafem.SistemaSIAF = GeralEnum.SistemaSIAF.SIAFEM;
                    procWsSiafem.ConsumirWS(anoBasePagamento, ugeGestora, estimuloWsMonitoramentoSIAF, true, true);

                    if (!procWsSiafem.ErroProcessamentoWs)
                    {
                        if (!String.IsNullOrWhiteSpace(procWsSiafem.ErroRetornoWs))
                            throw new Exception(String.Format("Erro processando retorno SIAFEM: {0}", procWsSiafem.ErroRetornoWs));

                        strRetornoWS = procWsSiafem.RetornoWsSIAF;
                        strNoLanSIAF = obterNLSiafemDoMonitoramento(strRetornoWS);

                        statusRetorno = String.IsNullOrWhiteSpace(strNoLanSIAF);
                    }
                }
            }
            catch (TimeoutException excTimeoutConexao)
            {
                string erroProcessamento = String.Format("Timeout em conexão com servidores SEFAZ [{0}]", excTimeoutConexao.Message);
                string loginSiafemUsuario = ConfiguracoesSIAF.userNameConsulta;
                string loginUsuarioSAM = notaLancamentoPendente.AuditoriaIntegracaoVinculada.UsuarioSAM;
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, notaLancamentoPendente.MovimentoVinculado);

                throw new Exception(erroProcessamento);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                string erroProcessamento = String.Format("Operação inválida em processamento de dados com servidores SEFAZ [{0}]", excErroProcessamentoWsSIAF.Message);
                string loginSiafemUsuario = ConfiguracoesSIAF.userNameConsulta;
                string loginUsuarioSAM = notaLancamentoPendente.AuditoriaIntegracaoVinculada.UsuarioSAM;
                GerarNotaLancamentoPendenciaDeException(erroProcessamento, loginUsuarioSAM, loginSiafemUsuario, notaLancamentoPendente.MovimentoVinculado);

                throw new Exception(excErroProcessamentoWsSIAF.Message);
            }

            return statusRetorno;
        }

        public void AtualizarMovimentacaoMaterialComNLSiafemManual(string loginUsuarioSAM, MovimentoEntity movimentacaoMaterial, string numeroNL, string tipoLancamento, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;

            try
            {
                using (TransactionScope tsProcessamento = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    //Atualizar NLs da movimentacao de material na base
                    this.atualizarItensMovimentacaoComNotaSIAF(movimentacaoMaterial, numeroNL, tipoLancamento, @TipoNotaSIAF);

                    //Auditoria Integracao SIAFEM
                    registroAuditoria = gerarRegistroAuditoria(loginUsuarioSAM, movimentacaoMaterial, numeroNL, tipoLancamento, @TipoNotaSIAF);

                    var isEstorno = (tipoLancamento.ToUpperInvariant() == "E");
                    if (!string.IsNullOrWhiteSpace(movimentacaoMaterial.ObterNLsMovimentacao(false, isEstorno, @TipoNotaSIAF, true)))
                    {
                        InativarPendenciasPorMovimentacao(movimentacaoMaterial.Id.Value);
                        inserirRegistroAuditoriaIntegracao(registroAuditoria);
                    }

                    tsProcessamento.Complete();
                }
            }
            catch (Exception excErroExecucao)
            {
                erroProcessamentoSIAF = String.Format("Erro ao atualizar manualmente NL's SIAFEM, movimentação SAM documento {0}.: {1}", movimentacaoMaterial.NumeroDocumento, excErroExecucao.Message);
                throw new Exception(erroProcessamentoSIAF);
            }
        }

        /// <summary>
        /// Sobrecarga de método, para geração de auditoria quando efetuada atualizacao manual de NLs SIAFEM, pelo usuario.
        /// </summary>
        /// <param name="loginUsuarioSAM">CPF de usuario logado no sistema</param>
        /// <param name="movimentacaoMaterial">DTO da movimentacao de material, que sofrera atualizacao manual das NLs</param>
        /// <param name="numeroNL">NL SIAFEM para atualizacao</param>
        /// <param name="tipoLancamento">Tipo de lancamento da Pendencia, se Normal/Estorno</param>
        /// <param name="TipoNotaSIAF">Se Nota de Lancamento (Liquidacao), ou Reclassificacao</param>
        private AuditoriaIntegracaoEntity gerarRegistroAuditoria(string loginUsuarioSAM, MovimentoEntity movimentacaoMaterial, string numeroNL, string tipoLancamento, TipoNotaSIAF @TipoNotaSIAF)
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;
            string xmlAuditoriaAtualizacaoManual = null;


            xmlAuditoriaAtualizacaoManual = gerarXmlAuditoriaAlteracaoManual(loginUsuarioSAM, movimentacaoMaterial, numeroNL, tipoLancamento, @TipoNotaSIAF);
            registroAuditoria = new AuditoriaIntegracaoEntity() { NomeSistema = "SIAFEM", DataEnvio = DateTime.Now, MsgEstimuloWS = xmlAuditoriaAtualizacaoManual, MsgRetornoWS = "\0", UsuarioSAM = loginUsuarioSAM, UsuarioSistemaExterno = "\0" };

            return registroAuditoria;
        }

        private string gerarXmlAuditoriaAlteracaoManual(string loginUsuarioSAM, MovimentoEntity movimentacaoMaterial, string numeroNL, string tipoLancamento, TipoNotaSIAF @TipoNotaSIAF)
        {
            string strRetorno = null;
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;

            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            strRetorno = objBusiness.GerarXmlAuditoriaAlteracaoManual(loginUsuarioSAM, movimentacaoMaterial, numeroNL, tipoLancamento, @TipoNotaSIAF);
            
            return strRetorno;
        }

        private void atualizarItensMovimentacaoComNotaSIAF(MovimentoEntity movimentacaoMaterial, string numeroNL, string tipoLancamento, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            string nlLancamentoSIAF = null;
            var tipoLancamentoSIAFEM = default(TipoLancamentoSIAFEM);
            var srvInfra = this.Service<Sam.ServiceInfraestructure.IMovimentoItemService>();

            try
            {
                nlLancamentoSIAF = numeroNL;

                if (tipoLancamento.ToUpperInvariant() == "E")
                    tipoLancamentoSIAFEM = TipoLancamentoSIAFEM.Estorno;
                else
                    tipoLancamentoSIAFEM = TipoLancamentoSIAFEM.Normal;

                movimentacaoMaterial.MovimentoItem.ToList().ForEach(movItem => srvInfra.AtualizarItensMovimentacaoComNotaSIAF(movItem.Id.Value, nlLancamentoSIAF, @TipoNotaSIAF, tipoLancamentoSIAFEM));
            }
            catch (Exception excErroGravacao)
            {
                erroProcessamentoSIAF = String.Format("Erro ao atualizar itens de material, movimentação SAM documento {0}.: {1}", movimentacaoMaterial.NumeroDocumento, excErroGravacao.Message);
            }

        }
        private string obterNLSiafem(string msgRetornoWsSIAF)
        {
            string strXmlRetornoPattern = "/*/*/Doc_Retorno/*/*/*/";
            var nlLiquidacao = XmlUtil.getXmlValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "NumeroNL"));

            return nlLiquidacao;
        }
        private string obterNLSiafemDoMonitoramento(string msgRetornoWsSIAF)
        {
            string strXmlRetornoPattern = "/*/*/Doc_Retorno/*/*/*/";
            var nlLiquidacao = XmlUtil.getXmlValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "retorno"));

            // Verificado que não precisa comparar o ano para saber se NL existe
            //var dtRetorno = XmlUtil.getXmlValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "dtRetorno"));
            //var anoDataRetornoWS = dtRetorno.BreakLine('-', 0);
            //nlLiquidacao = ((nlLiquidacao.StartsWith(anoDataRetornoWS)) ? nlLiquidacao : null);

            return nlLiquidacao;
        }
        private AuditoriaIntegracaoEntity gerarRegistroAuditoria(string loginUsuarioSAM, string loginSiafemUsuario, ProcessadorServicoSIAF svcProcessadorSIAF)
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;

            registroAuditoria                       = new AuditoriaIntegracaoEntity();
            registroAuditoria.MsgEstimuloWS         = svcProcessadorSIAF.EstimuloWsSIAF;
            registroAuditoria.MsgRetornoWS          = svcProcessadorSIAF.RetornoWsSIAF;
            registroAuditoria.NomeSistema           = svcProcessadorSIAF.NomeSistemaSIAF;
            registroAuditoria.DataEnvio             = svcProcessadorSIAF.DataEnvioMsgWs;
            registroAuditoria.UsuarioSAM            = loginUsuarioSAM;
            registroAuditoria.UsuarioSistemaExterno = loginSiafemUsuario;

            return registroAuditoria;
        }
        private void auditarTransacaoSIAF(string loginUsuarioSAM, string loginSiafemUsuario, MovimentoEntity movimentacaoMaterial, ProcessadorServicoSIAF svcProcessadorSIAF)
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;
            AuditoriaIntegracaoBusiness businessAuditoriaIntegracao = null;
            NotaLancamentoPendenteSIAFEMEntity notaPendente = null;
            NotaLancamentoPendenteSIAFEMBusiness businessNotasPendentes = null;


            registroAuditoria = gerarRegistroAuditoria(loginUsuarioSAM, loginSiafemUsuario, svcProcessadorSIAF);


            businessAuditoriaIntegracao = new AuditoriaIntegracaoBusiness();
            notaPendente = new NotaLancamentoPendenteSIAFEMEntity();
            businessNotasPendentes = new NotaLancamentoPendenteSIAFEMBusiness();

            //log auditoria sistemas integrados (nota SIAF)
            businessAuditoriaIntegracao.InserirRegistro(registroAuditoria);

            //inserção de pendencia na tabela de processamento de pendencias
            notaPendente.MovimentoVinculado = movimentacaoMaterial;
            notaPendente.AuditoriaIntegracaoVinculada = registroAuditoria;
            businessNotasPendentes.InserirRegistro(notaPendente);
        }
        private void inserirRegistroAuditoriaIntegracao(AuditoriaIntegracaoEntity registroAuditoria)
        {
            AuditoriaIntegracaoBusiness objBusiness = null;

            objBusiness = new AuditoriaIntegracaoBusiness();
            objBusiness.InserirRegistro(registroAuditoria);

            if (ListaErro.IsNotNullAndNotEmpty())
                this.ListaErro.AddRange(objBusiness.ListaErro);
            else
                this.ListaErro = objBusiness.ListaErro;
        }
        private void gerarNotaLancamentoPendencia(AuditoriaIntegracaoEntity registroAuditoria, MovimentoEntity movimentacaoMaterial, string msgErroSIAF, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            NotaLancamentoPendenteSIAFEMEntity notaPendente = null;
            NotaLancamentoPendenteSIAFEMBusiness businessNotasPendentes = null;

            try
            {
                businessNotasPendentes = new NotaLancamentoPendenteSIAFEMBusiness();

                notaPendente = new NotaLancamentoPendenteSIAFEMEntity();
                notaPendente.DataEnvioMsgWs = registroAuditoria.DataEnvio;
                notaPendente.DocumentoSAM = movimentacaoMaterial.NumeroDocumento;
                notaPendente.AlmoxarifadoVinculado = movimentacaoMaterial.Almoxarifado;
                notaPendente.MovimentoVinculado = movimentacaoMaterial;
                notaPendente.AuditoriaIntegracaoVinculada = registroAuditoria;
                notaPendente.ErroProcessamentoMsgWS = msgErroSIAF;
                notaPendente.TipoNotaSIAF = @TipoNotaSIAF;

                //Se procedimento foi efetuado após horariocomercial
                if (notaPendente.ErroProcessamentoMsgWS.ToUpperInvariant().Contains("Mensagem fora do horario de processamento".ToUpperInvariant()))
                {
                    var dtReenvio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    notaPendente.DataReenvioMsgWs = dtReenvio.AddDays(1);
                }

                businessNotasPendentes.InserirRegistro(notaPendente);
            }
            catch (Exception excErroGravacaoNotaLancamentoPendente)
            {
                erroProcessamentoSIAF = String.Format("SAM|{0}{1}", (!String.IsNullOrWhiteSpace(excErroGravacaoNotaLancamentoPendente.Message) ? (excErroGravacaoNotaLancamentoPendente.Message + "; ") : ""), (excErroGravacaoNotaLancamentoPendente.InnerException.IsNotNull() ? (excErroGravacaoNotaLancamentoPendente.InnerException.Message) : ""));

                if (ListaErro.IsNotNullAndNotEmpty())
                    this.ListaErro.Add(erroProcessamentoSIAF);
                else
                    this.ListaErro = new List<string>(erroProcessamentoSIAF.BreakLine(new char[] { ';' })
                                                                           .Cast<string>()
                                                                           .Where(linhaErro => !String.IsNullOrWhiteSpace(linhaErro))
                                                                           .AsEnumerable());
            }
        }
        private void inativarNotaLancamentoPendenciaReexecutada(NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente, ProcessadorServicoSIAF svcProcessadorSIAF)
        {
            NotaLancamentoPendenteSIAFEMEntity notaPendente = null;
            NotaLancamentoPendenteSIAFEMBusiness businessNotasPendentes = null;

            try
            {
                businessNotasPendentes = new NotaLancamentoPendenteSIAFEMBusiness();

                notaPendente = notaLancamentoPendente;
                notaPendente.DataReenvioMsgWs = svcProcessadorSIAF.DataEnvioMsgWs;
                notaPendente.StatusPendencia = 1;

                businessNotasPendentes.InserirRegistro(notaPendente);
            }
            catch (Exception excErroGravacaoNotaLancamentoPendente)
            {
                erroProcessamentoSIAF = String.Format("SAM|{0}{1}", (!String.IsNullOrWhiteSpace(excErroGravacaoNotaLancamentoPendente.Message) ? (excErroGravacaoNotaLancamentoPendente.Message + "; ") : ""), (excErroGravacaoNotaLancamentoPendente.InnerException.IsNotNull() ? (excErroGravacaoNotaLancamentoPendente.InnerException.Message) : ""));

                if (ListaErro.IsNotNullAndNotEmpty())
                    this.ListaErro.Add(erroProcessamentoSIAF);
                else
                    this.ListaErro = new List<string>(erroProcessamentoSIAF.BreakLine(new char[] { ';' })
                                                                           .Cast<string>()
                                                                           .Where(linhaErro => !String.IsNullOrWhiteSpace(linhaErro))
                                                                           .AsEnumerable());
            }
        }


        /// <summary>
        /// Metodo para obtencao de NLs SIAFEM vinculadas a esta movimentacao de materiais.
        /// Quaisquer erros de lancamentos (tanto lancamentos normais (liquidacao), quanto 'compostos' (liquidacao/reclassificacao), serao listados na tela de pendencias.
        /// O método de processamento de pendencias, tratara quaisquer situacoes especiais, referentes a lancamentos 'compostos', no SIAFEM.
        /// </summary>
        /// <param name="loginUsuarioSAM"></param>
        /// <param name="loginSiafemUsuario"></param>
        /// <param name="senhaSiafemUsuario"></param>
        /// <param name="tipoLancamento"></param>
        /// <param name="movimentacaoMaterial"></param>
        public void ExecutaProcessamentoMovimentacaoNoSIAF(string loginUsuarioSAM, string loginSiafemUsuario, string senhaSiafemUsuario, string tipoLancamento, MovimentoEntity movimentacaoMaterial)
        {
            string nlSIAF = null;
            bool movimentacaoReclassifica = false;
            //bool movimentacaoPodeEstornarReclassificacao = false;
            var ehEstorno = (tipoLancamento.ToUpperInvariant() == "E");
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;

            
            try
            {

                if (ehEstorno)
                {

                    #region Verificacao Para Reclassificacao
                    //Verificar se eh Entrada por Empenho ou Restos a Pagar, caso sim, reclassificar.
                    //Verificar se movimentacao pode ser reclassificada.
                    //movimentacaoReclassifica = ((movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho));
                    movimentacaoReclassifica = ((movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho));

                    //Se movimentacao foi liquidada, mas nao reclassificada
                    var semNLsReclassificacao = (String.IsNullOrWhiteSpace(movimentacaoMaterial.ObterNLsMovimentacao(false, false, TipoNotaSIAF.NL_Reclassificacao)) &&
                                                 String.IsNullOrWhiteSpace(movimentacaoMaterial.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Reclassificacao)));

                   
                    if (movimentacaoReclassifica)
                    {
                        if (ehEstorno && semNLsReclassificacao)
                            return;

                        tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;

                        nlSIAF = this.obterNotaLancamentoSIAF(loginUsuarioSAM, loginSiafemUsuario, senhaSiafemUsuario, tipoLancamento, movimentacaoMaterial, tipoNotaSIAF, movimentacaoMaterial.InscricaoCE);
                        this.atualizarItensMovimentacaoComNotaSIAF(movimentacaoMaterial, nlSIAF, tipoLancamento, tipoNotaSIAF);
                    }
                    #endregion

                    tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao;

                    nlSIAF = null;
                    nlSIAF = this.obterNotaLancamentoSIAF(loginUsuarioSAM, loginSiafemUsuario, senhaSiafemUsuario, tipoLancamento, movimentacaoMaterial, tipoNotaSIAF, movimentacaoMaterial.InscricaoCE);
                    this.atualizarItensMovimentacaoComNotaSIAF(movimentacaoMaterial, nlSIAF, tipoLancamento, tipoNotaSIAF);


                }
                else
                {
                    tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao;

                    nlSIAF = this.obterNotaLancamentoSIAF(loginUsuarioSAM, loginSiafemUsuario, senhaSiafemUsuario, tipoLancamento, movimentacaoMaterial, tipoNotaSIAF, movimentacaoMaterial.InscricaoCE);
                    this.atualizarItensMovimentacaoComNotaSIAF(movimentacaoMaterial, nlSIAF, tipoLancamento, tipoNotaSIAF);

                    #region Verificacao Para Reclassificacao
                    //Verificar se eh Entrada por Empenho ou Restos a Pagar, caso sim, reclassificar.
                    //Verificar se movimentacao pode ser reclassificada.
                    //movimentacaoReclassifica = ((movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho));
                    movimentacaoReclassifica = ((movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho) || (movimentacaoMaterial.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho));

                    //Se movimentacao foi liquidada, mas nao reclassificada
                    var semNLsReclassificacao = (String.IsNullOrWhiteSpace(movimentacaoMaterial.ObterNLsMovimentacao(false, false, TipoNotaSIAF.NL_Reclassificacao)) &&
                                                 String.IsNullOrWhiteSpace(movimentacaoMaterial.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Reclassificacao)));

                    nlSIAF = null;
                    if (movimentacaoReclassifica)
                    {
                        if (ehEstorno && semNLsReclassificacao)
                            return;

                        tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;

                        nlSIAF = this.obterNotaLancamentoSIAF(loginUsuarioSAM, loginSiafemUsuario, senhaSiafemUsuario, tipoLancamento, movimentacaoMaterial, tipoNotaSIAF, movimentacaoMaterial.InscricaoCE);
                        this.atualizarItensMovimentacaoComNotaSIAF(movimentacaoMaterial, nlSIAF, tipoLancamento, tipoNotaSIAF);
                    }
                    #endregion
                }
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                erroProcessamentoSIAF = String.Format("{0}{1}", (!String.IsNullOrWhiteSpace(excErroProcessamentoWsSIAF.Message) ? (excErroProcessamentoWsSIAF.Message) : ""), (excErroProcessamentoWsSIAF.InnerException.IsNotNull() ? (";" + excErroProcessamentoWsSIAF.InnerException.Message) : ""));

                if (ListaErro.IsNotNullAndNotEmpty())
                    this.ListaErro.Add(erroProcessamentoSIAF);
                else
                    this.ListaErro = new List<string>(erroProcessamentoSIAF.BreakLine(new char[] { ';' })
                                                                           .Cast<string>()
                                                                           .Where(linhaErro => !String.IsNullOrWhiteSpace(linhaErro))
                                                                           .AsEnumerable());
            }
        }


        private bool processaMovimentacaoNoSIAFEM(string loginSiafemUsuario, string senhaSiafemUsuario, string valorCampoCE, string tipoLancamentoSIAFEM, NotaLancamentoPendenteSIAFEMEntity notaLancamentoPendente, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.NL_Liquidacao)
        {
            bool blnRetorno = false;
            string nlSIAF = null;
            TipoNotaSIAF _tipoNotaSIAFEM = TipoNotaSIAF.NL_Liquidacao;


            if (valorCampoCE != null)
                 notaLancamentoPendente.MovimentoVinculado.InscricaoCE = valorCampoCE;

            if (tipoNotaSIAFEM == TipoNotaSIAF.NL_Liquidacao)
                _tipoNotaSIAFEM = notaLancamentoPendente.TipoNotaSIAF;
            else
                _tipoNotaSIAFEM = tipoNotaSIAFEM;


            if (VerificaSeMovimentacaoSAMEstaPendenteSIAF(tipoLancamentoSIAFEM, notaLancamentoPendente, _tipoNotaSIAFEM))
                nlSIAF = this.obterNotaLancamentoSIAF(loginSiafemUsuario, senhaSiafemUsuario, tipoLancamentoSIAFEM, notaLancamentoPendente, _tipoNotaSIAFEM, notaLancamentoPendente.MovimentoVinculado.InscricaoCE);
            else
                nlSIAF = this.obterNotaLancamentoDoMonitoramentoSIAF(loginSiafemUsuario, senhaSiafemUsuario, tipoLancamentoSIAFEM, notaLancamentoPendente, _tipoNotaSIAFEM);


            this.atualizarItensMovimentacaoComNotaSIAF(notaLancamentoPendente.MovimentoVinculado, nlSIAF, tipoLancamentoSIAFEM, _tipoNotaSIAFEM);
            this.atualizarDadosMovimentacaoCampoCE(notaLancamentoPendente.MovimentoVinculado.Id.Value, valorCampoCE);

            blnRetorno = !String.IsNullOrWhiteSpace(nlSIAF);
            return blnRetorno;
        }


        /// <summary>
        /// Metodo para obtencao de NLs SIAFEM vinculadas a esta movimentacao de materiais, para atualizacao dos campos correlatos, quando o lancamento original no SAM, originou algum erro SIAFEM
        /// </summary>
        /// <param name="loginSiafemUsuario"></param>
        /// <param name="senhaSiafemUsuario"></param>
        /// <param name="valorCampoCE"></param>
        /// <param name="tipoLancamento"></param>
        /// <param name="notaLancamentoPendenteID"></param>
        /// <param name="codigoGestor"></param>
        /// <param name="codigoUge"></param>
        public void ExecutaProcessamentoNotaPendenteSIAFEM(string loginSiafemUsuario, string senhaSiafemUsuario, string valorCampoCE, string tipoLancamento, int notaLancamentoPendenteID, int codigoGestor, int codigoUge, string loginUsuarioSAM)
        {
            bool movimentacaoReclassifica = false;
            bool movimentacaoPodeEstornarReclassificacao = false;
            bool movimentacaoPodeEstornarLiquidacao = false;
            bool reclassificacaoEstornada = false;
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;

            try
            {
                var notaLancamentoPendente = (new NotaLancamentoPendenteSIAFEMBusiness()).ObterNotaLancamentoPendente(notaLancamentoPendenteID);

                notaLancamentoPendente.AuditoriaIntegracaoVinculada.UsuarioSAM = loginUsuarioSAM;

                var ehEstorno = (tipoLancamento.ToUpperInvariant() == "E");

                if (notaLancamentoPendente.IsNotNull() && notaLancamentoPendente.Id.HasValue)
                {
                    if (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == tipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode() && codigoGestor > 0 && codigoUge > 0)
                    {
                        var _almoxDestino = new AlmoxarifadoEntity { Uge = new UGEEntity { Codigo = codigoUge }, Gestor = new GestorEntity { CodigoGestao = codigoGestor } };
                        notaLancamentoPendente.MovimentoVinculado.MovimAlmoxOrigemDestino = _almoxDestino;
                    }

                    if (!ehEstorno)
                    {
                        #region Lancamento Normal
                        processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente);
                        #endregion

                        #region Verificacao Lancamento Reclassificacao
                        //Verificar se eh Entrada por Empenho ou Restos a Pagar, caso sim, reclassificar.
                        //movimentacaoReclassifica = ((notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho));
                        movimentacaoReclassifica = ((notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho) || (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho));

                        if (movimentacaoReclassifica && notaLancamentoPendente.TipoNotaSIAF != TipoNotaSIAF.NL_Reclassificacao)
                        {
                            tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;
                            processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente, tipoNotaSIAF);
                        }
                        #endregion
                    }
                    else if (ehEstorno)
                    {
                        //Verificar se movimentacao pode ser reclassificada.
                        //movimentacaoReclassifica = ((String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Liquidacao)) && ((notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) ||
                        //                                                                                                                                                                    (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar))) ||
                        movimentacaoReclassifica = ((String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Liquidacao)) && ((notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorEmpenho) ||
                                                                                                                                                                                            (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagar) ||
                                                                                                                                                                                            (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho) ||
                                                                                                                                                                                            (notaLancamentoPendente.MovimentoVinculado.TipoMovimento.Id == (int)tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho))) ||
                                                                              (notaLancamentoPendente.TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao));

                        //Verificar se eh Entrada por Empenho ou Restos a Pagar, jah foi reclassificada. Caso sim, estornar reclassificacao.
                        movimentacaoPodeEstornarReclassificacao = (String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Reclassificacao)) && movimentacaoReclassifica);


                        if (!movimentacaoReclassifica)
                        {
                            #region Lancamento Estorno Normal
                            tipoNotaSIAF = notaLancamentoPendente.TipoNotaSIAF;
                            processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente, tipoNotaSIAF);
                            #endregion
                        }
                        else if (movimentacaoReclassifica)
                        {
                            #region Lancamento Estorno Reclassificacao

                            reclassificacaoEstornada = !String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Reclassificacao));
                            movimentacaoPodeEstornarLiquidacao = !String.IsNullOrWhiteSpace(notaLancamentoPendente.MovimentoVinculado.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Liquidacao));

                            if (reclassificacaoEstornada && movimentacaoPodeEstornarLiquidacao)
                            {
                                processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente);
                                return;
                            }
                            else if (movimentacaoPodeEstornarReclassificacao)
                            {
                                //Estornar reclassificacao
                                {
                                    tipoNotaSIAF = notaLancamentoPendente.TipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;
                                    processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente, tipoNotaSIAF);
                                }

                                //Estornar liquidacao
                                {
                                    processaMovimentacaoNoSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipoLancamento, notaLancamentoPendente);
                                }
                            }
                            #endregion
                        }

                    }
                }
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var msgErroProcessamento = excErroProcessamentoWsSIAF.Message;
                string[] infosErroProcessamento = excErroProcessamentoWsSIAF.Message.BreakLine(@"\n");
                if (infosErroProcessamento.Count() == 2)
                {
                    var descErroSistema = msgErroProcessamento.BreakLine(@"\n", 0);
                    var msgErroDetalhada = msgErroProcessamento.BreakLine(@"\n", 1).BreakLine("|", 1);
                    erroProcessamentoSIAF = String.Format("{0}: {1}", descErroSistema, msgErroDetalhada);
                }
                else if (infosErroProcessamento.Count() == 1)
                {
                    erroProcessamentoSIAF = msgErroProcessamento.BreakLine("|", 0);
                }



            }

            if (ListaErro.Contains(erroProcessamentoSIAF))
                ListaErro.Remove(erroProcessamentoSIAF);
        }

        public void ExecutaProcessamentoManualNotaSIAFEM(string loginUsuarioSAM, string tipoLancamento, int notaLancamentoPendenteID, string nlSiafem, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;

            try
            {
                objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
                var notaLancamentoPendente = objBusiness.ObterNotaLancamentoPendente(notaLancamentoPendenteID);

                if (notaLancamentoPendente.IsNotNull() && notaLancamentoPendente.Id.HasValue)
                    AtualizarMovimentacaoMaterialComNLSiafemManual(loginUsuarioSAM, notaLancamentoPendente.MovimentoVinculado, nlSiafem, tipoLancamento, @TipoNotaSIAF);

                if (ListaErro.HasElements())
                    ListaErro = objBusiness.ListaErro;
                else
                    ListaErro = new List<string>(objBusiness.ListaErro);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var msgErroProcessamento = excErroProcessamentoWsSIAF.Message;
                string[] infosErroProcessamento = excErroProcessamentoWsSIAF.Message.BreakLine(@"\n");
                if (infosErroProcessamento.Count() == 2)
                {
                    var descErroSistema = msgErroProcessamento.BreakLine(@"\n", 0);
                    var msgErroDetalhada = msgErroProcessamento.BreakLine(@"\n", 1).BreakLine("|", 1);
                    erroProcessamentoSIAF = String.Format("{0}: {1}", descErroSistema, msgErroDetalhada);
                }
                else if (infosErroProcessamento.Count() == 1)
                {
                    erroProcessamentoSIAF = msgErroProcessamento.BreakLine("|", 0);
                }

            }

            if (ListaErro.Contains(erroProcessamentoSIAF))
                ListaErro.Remove(erroProcessamentoSIAF);
        }

        private void atualizarDadosMovimentacaoCampoCE(int movimentacaoMaterialID, string valorCampoCE)
        {
            try
            {
                this.Service<IMovimentoService>().AtualizaMovimentacaoCampoCE(movimentacaoMaterialID, valorCampoCE);
            }
            catch (Exception excErroRuntime)
            {
                this.ListaErro.Add(excErroRuntime.Message);
            }
        }
    }
}
