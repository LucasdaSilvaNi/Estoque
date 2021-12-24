using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sam.Common.Util;
using System.Collections;
using System.Globalization;
using System.EnterpriseServices;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using Sam.Domain.Entity;



namespace Sam.Integracao.SIAF.Core
{
    public static class GeradorEstimuloSIAF
    {
        static StringBuilder sbXml = new StringBuilder();
        static XmlWriter xmlMontadorEstimulo = null;
        static XmlWriterSettings xmlSettings = null;


        private static void Init()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;

            sbXml = new StringBuilder();
            xmlSettings = GeradorEstimuloSIAF.ObterFormatacaoPadraoParaXml();
            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
        }
        private static void Dispose()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;
        }

        #region Consulta Cadastros SIAFISICO

        public static string SiafisicoDocConsultaF(long cpfCnpjFornecedor)
        {
            string strRetorno = null;

            
            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConsultaF");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocConsultaF");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CgcCpf", cpfCnpjFornecedor.ToString());
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }
        public static string SiafisicoDocConsultaI(string strCodigoItemMaterial)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConsultaI");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocConsultaI");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CodigoItem", strCodigoItemMaterial);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }

        #endregion

        #region Balancete Mensal (NL Consumo)
        [Obsolete("NL Consumo sendo gerada por meio da transacao NL no SIAFEM", true)]
        public static string SiafemDocNLConsumo(int iCodigoUG, int iUGConsumidora, int iUAConsumidora, int iPTRes, int iCodigoGestao, bool IsEstorno, string strDataEmissao, long iNaturezaDespesa, decimal decValorBaixaFechamento, string strRelacaoNotasSAM)
        {
            string strRetorno = null;
            string strTipoLancamento = ((IsEstorno) ? "E" : "N");
            string strValorBaixaFechamento = decValorBaixaFechamento.ToString("#.00").Replace(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, "").Trim().PadLeft(17, '0');
            string strNotasSAM = ((!String.IsNullOrWhiteSpace(strRelacaoNotasSAM)) ? String.Format("Notas Saida Material SAM: {0}", strRelacaoNotasSAM.Replace("\n", ", ")) : null);



            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFNLCONSUMO");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNLConsumo");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("ID", "0001");
            xmlMontadorEstimulo.WriteElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("Lancamento", strTipoLancamento);
            xmlMontadorEstimulo.WriteElementString("UGConsumidora", iUGConsumidora.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("UAConsumidora", iUAConsumidora.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("PTRes", iPTRes.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("ClassificacaoDespesa", String.Format("{0:D8}", iNaturezaDespesa));
            xmlMontadorEstimulo.WriteElementString("Valor", strValorBaixaFechamento);
            xmlMontadorEstimulo.WriteElementString("Obs01", "Consumo almoxarifado (Fechamento Sistema SAM)");
            xmlMontadorEstimulo.WriteElementString("Obs02", strNotasSAM);
            xmlMontadorEstimulo.WriteElementString("Obs03", null);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }
        #endregion

        #region Notas Lancamento (SIAFEM)
        public static string SiafemDocNLEmLiq(int iCodigoUG, int iCodigoGestao, string strDataEmissao, string strNumeroEmpenho, string naturezaDespesa, string documentoSAM, string[] observacoesMovimentacao, decimal valorDocumento, string tipoLancamentoSIAFEM, int movimentacaoMaterialID = 0)
        {
            string strRetorno = null;
            string anoNotaEmpenho = null;
            string anoPagamentoEmpenho = null;
            string anoRestosAPagar = null;
            string anoPrefixoNE = null;
            string numeroEmpenho = null;
            string acaoPagamentoEstorno = null;
            string acaoPagamentoNormal = null;
            bool isPagamento = false;

            string[] numeroDecimal = null;


            #region Verificação Ano Base / Tipo Lançamento SAM

            anoNotaEmpenho = strDataEmissao.Substring(5, 4);
            //prefixoNE = String.Format("{0}NE", anoNotaEmpenho);
            //strPrefixoNeRestosAPagar = String.Format("{0}NE", (Int32.Parse(anoNotaEmpenho) - 1).ToString());
            //strNumeroEmpenhoValidado = (!String.IsNullOrWhiteSpace(strNumeroEmpenho) && ((strNumeroEmpenho.Contains(strPrefixoNe)) || (strNumeroEmpenho.Contains(strPrefixoNeRestosAPagar)))) ? strNumeroEmpenho.Replace(strPrefixoNe, "").Replace(strPrefixoNeRestosAPagar, "") : strNumeroEmpenho;

            anoRestosAPagar = (Int32.Parse(anoNotaEmpenho) - 1).ToString();
            anoPrefixoNE = strNumeroEmpenho.Split(new string[] { "NE" }, StringSplitOptions.RemoveEmptyEntries)[0];
            numeroEmpenho = strNumeroEmpenho.Split(new string[] { "NE" }, StringSplitOptions.RemoveEmptyEntries)[1];
            anoPagamentoEmpenho = ((anoNotaEmpenho != anoPrefixoNE) ? anoRestosAPagar : anoNotaEmpenho);


            isPagamento = (tipoLancamentoSIAFEM.ToUpperInvariant() == "N");
            acaoPagamentoNormal = ((isPagamento) ? "x" : string.Empty);
            acaoPagamentoEstorno = ((!isPagamento) ? "x" : string.Empty);

            #endregion


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNLEmLiq");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNLEmLiq");
            xmlMontadorEstimulo.WriteStartElement("documento");

            //Monitoramento, via WS SIAF SIAFMONITORA
            if (movimentacaoMaterialID > 0)
                //xmlMontadorEstimulo.WriteAttributeString("id", String.Format("_samMovID:{0}", movimentacaoMaterialID));
                xmlMontadorEstimulo.WriteAttributeString("id", String.Format("{0}_samMovID:{1}", tipoLancamentoSIAFEM, movimentacaoMaterialID));


            xmlMontadorEstimulo.WriteFullElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteFullElementString("Normal", acaoPagamentoNormal);
            xmlMontadorEstimulo.WriteFullElementString("Estorno", acaoPagamentoEstorno);

            //xmlMontadorEstimulo.WriteFullElementString("Ano", anoNotaEmpenho);
            xmlMontadorEstimulo.WriteFullElementString("Ano", anoPagamentoEmpenho);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", numeroEmpenho);

            //VALOR EMPENHO
            numeroDecimal = valorDocumento.ToString(CultureInfo.GetCultureInfo("pt-BR")).Split(new char[] { ',' });
            numeroDecimal[1] = numeroDecimal[1].Substring(0, 2);

            var _valorMovimentacao = String.Format("{0}{1}", numeroDecimal[0], numeroDecimal[1]);
            _valorMovimentacao = _valorMovimentacao.PadLeft(17, '0');
            xmlMontadorEstimulo.WriteFullElementString("Valor", _valorMovimentacao);
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO OBSERVAÇÃO
            xmlMontadorEstimulo.WriteStartElement("Observacao");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            for (int iContador = 0; iContador < observacoesMovimentacao.Count(); iContador++)
            {
                xmlMontadorEstimulo.WriteStartElement("obs");
                xmlMontadorEstimulo.WriteFullElementString("Observacao", observacoesMovimentacao[iContador]);
                xmlMontadorEstimulo.WriteEndElement();
            }
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", documentoSAM);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }

        /// <summary>
        /// Metodo utilizado para a geracao de NL Consumo no SIAFEM.
        /// </summary>
        /// <param name="iCodigoUG"></param>
        /// <param name="iUGConsumidora"></param>
        /// <param name="iUAConsumidora"></param>
        /// <param name="iPTRes"></param>
        /// <param name="iCodigoGestao"></param>
        /// <param name="IsEstorno"></param>
        /// <param name="strDataEmissao"></param>
        /// <param name="iNaturezaDespesa"></param>
        /// <param name="decValorBaixaFechamento"></param>
        /// <param name="strRelacaoNotasSAM"></param>
        /// <returns></returns>
        public static string SiafemDocNL(int iCodigoUG, int iUGConsumidora, int iUAConsumidora, int iPTRes, int iCodigoGestao, bool IsEstorno, string strDataEmissao, long iNaturezaDespesa, decimal decValorBaixaFechamento, string strRelacaoNotasSAM)
        {
            string _ugeFavorecida = null;
            string _gestaoFavorecida = null;
            string strRetorno = null;
            string strTipoLancamento = null;
            string strValorBaixaFechamento = null;
            string strNotasSAM = null;
            string[] notas = null;
            IDictionary<string, string[]> eventosNLConsumo = null;
            string codigoPrimeiroEvento = null;
            string codigoSegundoEvento = null;


            strTipoLancamento = ((IsEstorno) ? "E" : "N");
            strValorBaixaFechamento = decValorBaixaFechamento.ToString("#.00").Replace(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, "").Trim().PadLeft(17, '0');
            strNotasSAM = ((!String.IsNullOrWhiteSpace(strRelacaoNotasSAM)) ? strRelacaoNotasSAM.Replace("\n", ",") : null);
            notas = strNotasSAM.Split(new Char[] { ',' });

            eventosNLConsumo = new Dictionary<string, string[]>();
            eventosNLConsumo.Add("N", new string[] { "540429", "540430" });
            eventosNLConsumo.Add("E", new string[] { "545429", "545430" });

            codigoPrimeiroEvento = eventosNLConsumo[strTipoLancamento][0];
            codigoSegundoEvento = eventosNLConsumo[strTipoLancamento][1];



            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNL001");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNL");
            xmlMontadorEstimulo.WriteStartElement("documento");


            xmlMontadorEstimulo.WriteAttributeString("id", String.Format("{0}_samMovID:{1}", strTipoLancamento, "0001"));

            xmlMontadorEstimulo.WriteFullElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", iCodigoGestao.ToString("D5"));



            xmlMontadorEstimulo.WriteFullElementString("CgcCpfUgfav", _ugeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFav", _gestaoFavorecida);
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteStartElement("Evento");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");

            string InscricaoEvento = iUGConsumidora.ToString("D6") + iUAConsumidora.ToString("D6") + iPTRes.ToString("D5");
            xmlMontadorEstimulo.WriteStartElement("desc");
            //xmlMontadorEstimulo.WriteFullElementString("Evento", "540429");
            xmlMontadorEstimulo.WriteFullElementString("Evento", codigoPrimeiroEvento);
            xmlMontadorEstimulo.WriteFullElementString("InscricaoEvento", InscricaoEvento);
            xmlMontadorEstimulo.WriteFullElementString("RecDesp", String.Format("{0:D8}", iNaturezaDespesa));
            xmlMontadorEstimulo.WriteFullElementString("Classificacao", null);
            xmlMontadorEstimulo.WriteFullElementString("Fonte", null); //Valor sempre será nulo, vide funcional.
            xmlMontadorEstimulo.WriteFullElementString("Valor", strValorBaixaFechamento);
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteStartElement("desc");
            //xmlMontadorEstimulo.WriteFullElementString("Evento", "540430");
            xmlMontadorEstimulo.WriteFullElementString("Evento", codigoSegundoEvento);
            xmlMontadorEstimulo.WriteFullElementString("InscricaoEvento", "CE999");
            xmlMontadorEstimulo.WriteFullElementString("RecDesp", null);
            xmlMontadorEstimulo.WriteFullElementString("Classificacao", null);
            xmlMontadorEstimulo.WriteFullElementString("Fonte", null); //Valor sempre será nulo, vide funcional.
            xmlMontadorEstimulo.WriteFullElementString("Valor", strValorBaixaFechamento);
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO OBSERVAÇÃO
            xmlMontadorEstimulo.WriteStartElement("Observacao");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("obs");
            xmlMontadorEstimulo.WriteFullElementString("Observacao", "NLDECONSUMO GERADA ATRAVÉS DO SAM");
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");

            int i = 1;
            foreach (string item in notas)
            {
                xmlMontadorEstimulo.WriteStartElement("NF");
                xmlMontadorEstimulo.WriteAttributeString("Id", String.Format("{0:D4}", i));
                xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", item);
                xmlMontadorEstimulo.WriteEndElement();
                i++;
            }

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }

        //Alteração Solicitacao Marcia HML SEFAZ 01/10/2015
        //public static string SiafemDocNL(int iCodigoUG, int iCodigoGestao, string strDataEmissao, string documentoSAM, string[] codigosEvento, string[] codigosInscricaoEvento, string[] codigosClassificacaoPagamento , string[] observacoesMovimentacao, decimal valorDocumento)
        //public static string SiafemDocNL(int codigoUG, int codigoGestao, string strDataEmissao, string documentoSAM, string[] codigosEvento, string[] codigosInscricaoEvento, string[] codigosClassificacaoPagamento, string[] observacoesMovimentacao, decimal valorDocumento, bool favoreceUGE = false, int ugeFavorecida = 0, int gestaoFavorecida = 0)
        public static string SiafemDocNL(int codigoUG, int codigoGestao, string strDataEmissao, string documentoSAM, string[] codigosEvento, string[] codigosInscricaoEvento, string[] codigosClassificacaoPagamento, string[] observacoesMovimentacao, decimal valorDocumento, string tipoLancamentoSIAFEM, int movimentacaoMaterialID = 0, bool favoreceUGE = false, string ugeFavorecida = "0", int gestaoFavorecida = 0, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao, string[] codigosRecDesp= null)
        {
            string strRetorno = null;
            string[] numeroDecimal = null;
            string _valorMovimentacao = null;
            string _ugeFavorecida = null;
            string _gestaoFavorecida = null;
            string tipoNotaSIAF = null;

            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNL001");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNL");
            xmlMontadorEstimulo.WriteStartElement("documento");

            //Chave de reclassificacao
            if (@TipoNotaSIAF == TipoNotaSIAF.NL_Reclassificacao)
                tipoNotaSIAF = "R";

            //Monitoramento, via WS SIAF SIAFMONITORA
            if (movimentacaoMaterialID > 0)
                xmlMontadorEstimulo.WriteAttributeString("id", String.Format("{0}{1}_samMovID:{2}", tipoLancamentoSIAFEM, tipoNotaSIAF, movimentacaoMaterialID));

            xmlMontadorEstimulo.WriteFullElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UnidadeGestora", codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", codigoGestao.ToString("D5"));


            if (favoreceUGE)
            {
                _ugeFavorecida = ugeFavorecida;
                _gestaoFavorecida = gestaoFavorecida.ToString("D5");
            }

            xmlMontadorEstimulo.WriteFullElementString("CgcCpfUgfav", _ugeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFav", _gestaoFavorecida);
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteStartElement("Evento");

            //VALOR MOVIMENTACAO
            numeroDecimal = valorDocumento.ToString(CultureInfo.GetCultureInfo("pt-BR")).Split(new char[] { ',' });
            numeroDecimal[1] = numeroDecimal[1].Substring(0, 2);
            _valorMovimentacao = String.Format("{0}{1}", numeroDecimal[0], numeroDecimal[1]);
            _valorMovimentacao = _valorMovimentacao.PadLeft(17, '0');

            xmlMontadorEstimulo.WriteStartElement("Repeticao");

            for (int iContador = 0; iContador < codigosEvento.Length; iContador++)
            {
                xmlMontadorEstimulo.WriteStartElement("desc");
                xmlMontadorEstimulo.WriteFullElementString("Evento", codigosEvento[iContador]);
                xmlMontadorEstimulo.WriteFullElementString("InscricaoEvento", codigosInscricaoEvento[iContador]);
                xmlMontadorEstimulo.WriteFullElementString("RecDesp", codigosRecDesp[iContador]); 
                xmlMontadorEstimulo.WriteFullElementString("Classificacao", codigosClassificacaoPagamento[iContador]);
                xmlMontadorEstimulo.WriteFullElementString("Fonte", null); //Valor sempre será nulo, vide funcional.
                xmlMontadorEstimulo.WriteFullElementString("Valor", _valorMovimentacao);
                xmlMontadorEstimulo.WriteEndElement();
            }

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO OBSERVAÇÃO
            xmlMontadorEstimulo.WriteStartElement("Observacao");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            for (int iContador = 0; iContador < observacoesMovimentacao.Count(); iContador++)
            {
                xmlMontadorEstimulo.WriteStartElement("obs");
                xmlMontadorEstimulo.WriteFullElementString("Observacao", observacoesMovimentacao[iContador]);
                xmlMontadorEstimulo.WriteEndElement();
            }
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteAttributeString("Id", String.Format("{0:D4}", 1));
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", documentoSAM);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }


        public static string SiafemDocNLPatrimonial(int codigoUG, int codigoGestao, string strDataEmissao, string documentoSAM, string observacoesMovimentacao, string InscricaoCE,  bool favoreceUGE = false, string ugeFavorecida = "0", int gestaoFavorecida = 0, bool IsEstorno = false, EventoSiafemEntity evento = null, Dictionary<string, decimal?> dic = null)
        {
            string strRetorno = null;
            string[] numeroDecimal = null;
            string _valorMovimentacao = null;
            string _ugeFavorecida = null;
            string _gestaoFavorecida = null;
            string tipoNotaSIAF = null;
            string strTipoLancamento = null;
            strTipoLancamento = ((IsEstorno) ? "S" : "N");

            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("documento");

            xmlMontadorEstimulo.WriteFullElementString("TipoMovimento", evento.EventoTipoMovimento);

            xmlMontadorEstimulo.WriteFullElementString("Data", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UgeOrigem", codigoUG == 0 ? string.Empty : codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", codigoGestao == 0 ? string.Empty : codigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteFullElementString("Tipo_Entrada_Saida_Reclassificacao_Depreciacao", evento.EventoTipoEntradaSaidaReclassificacaoDepreciacao);

            if (favoreceUGE)
            {
                _ugeFavorecida = ugeFavorecida;
                _gestaoFavorecida = (gestaoFavorecida == 0 ? string.Empty : gestaoFavorecida.ToString("D5"));
            }
            else
                if (!string.IsNullOrEmpty(ugeFavorecida) && ugeFavorecida !="0")
                _ugeFavorecida = ugeFavorecida;

            xmlMontadorEstimulo.WriteFullElementString("CpfCnpjUgeFavorecida", _ugeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFavorecida", _gestaoFavorecida);

            xmlMontadorEstimulo.WriteFullElementString("Item", dic.FirstOrDefault().Key);
            xmlMontadorEstimulo.WriteFullElementString("TipoEstoque", evento.EventoTipoEstoque);
            xmlMontadorEstimulo.WriteFullElementString("Estoque", evento.EventoEstoque);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueDestino", "");
            xmlMontadorEstimulo.WriteFullElementString("EstoqueOrigem", "");
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimentacao", evento.EventoTipoMovimentacao);

            //VALOR MOVIMENTACAO

            string valorDoc = dic.Sum(x => x.Value).Value.ToString("0,0.00");
           

            xmlMontadorEstimulo.WriteFullElementString("ValorTotal", valorDoc);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecifico", InscricaoCE);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoEntrada", "");
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoSaida", "");
            xmlMontadorEstimulo.WriteFullElementString("FonteRecurso", "");
            xmlMontadorEstimulo.WriteFullElementString("NLEstorno", strTipoLancamento);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", "");
            xmlMontadorEstimulo.WriteFullElementString("Observacao", observacoesMovimentacao);
            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", documentoSAM);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO ITEM
            xmlMontadorEstimulo.WriteStartElement("ItemMaterial");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");

            foreach (var item in dic)
            {
                xmlMontadorEstimulo.WriteStartElement("IM");
                xmlMontadorEstimulo.WriteFullElementString("ItemMaterial", item.Key);
                xmlMontadorEstimulo.WriteEndElement();
            }
            
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }

        #endregion

        #region Consultas Empenhos

        public static string SiafemDocDetaContaGen(int iCodigoUG, int iCodigoGestao, string strMes, string strContaContabil, string strOpcao)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFDetaContaGen");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocDetaContaGen");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CodigoUG", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("Mes", strMes);
            xmlMontadorEstimulo.WriteElementString("ContaContabil", strContaContabil);
            xmlMontadorEstimulo.WriteElementString("ContaCorrente", null);
            xmlMontadorEstimulo.WriteElementString("Opcao", strOpcao);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }
        public static string SiafemDocConsultaEmpenhos(int iCodigoUG, int iCodigoGestao, string strNumeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFConsultaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocConsultaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("NumeroNe", strNumeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string SiafemDocListaEmpenhos(string strCpfCnpj, int iCodigoGestao, int iAnoBase, int iCodigoUG)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CgcCpf", strCpfCnpj);
            xmlMontadorEstimulo.WriteElementString("Data", null);
            xmlMontadorEstimulo.WriteElementString("Fonte", null);
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("GestaoCredor", null);
            xmlMontadorEstimulo.WriteElementString("Licitacao", null);
            xmlMontadorEstimulo.WriteElementString("ModalidadeEmpenho", null);
            xmlMontadorEstimulo.WriteElementString("Natureza", null);
            xmlMontadorEstimulo.WriteElementString("NumeroNe", null);
            xmlMontadorEstimulo.WriteElementString("Prefixo", String.Format("{0}NE", iAnoBase.ToString("D4")));
            xmlMontadorEstimulo.WriteElementString("Processo", null);
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string SiafemDocListaEmpenhos(SortedList parametrosConsulta)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");

            foreach (var parametroConsulta in parametrosConsulta)
                xmlMontadorEstimulo.WriteElementString(parametroConsulta.ToString(), parametrosConsulta[parametroConsulta].ToString());

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }



        [Description("Metodo sob homologacao")]
        public static string SiafisicoConPrecoNE(int codigoUG, int codigGestao, string numeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoConPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", codigGestao.ToString("D5"));

            if (numeroEmpenho.Contains("NE"))
                numeroEmpenho = numeroEmpenho.Split(new string[] { "NE" }, StringSplitOptions.RemoveEmptyEntries)[1];

            xmlMontadorEstimulo.WriteElementString("NumNE", numeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        [Description("Metodo sob homologacao")]
        public static string SiafisicoDocListaPrecoNE(int codigoUG, int codigGestao, string numeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOListaPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocListaPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", codigGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("NumeroNe", numeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        #endregion

        #region Monitoramento
        public static string SiafMonitora(string strChaveConsulta)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFMONITORA");
            xmlMontadorEstimulo.WriteStartElement("SiafMonitora");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("id", strChaveConsulta);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }
        #endregion

        #region Metodos Auxiliares (XML)
        /// <summary>
        /// Formatacao padrao para geracao de estimulos para os webservices SIAF.
        /// </summary>
        /// <returns></returns>
        private static XmlWriterSettings ObterFormatacaoPadraoParaXml()
        {
            return new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
        }

        /// <summary>
        /// Função para tratamento de caracteres inválidos, na mensagem XML retornada pelos sistemas SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string TratarDescricaoXml(string xml)
        {
            //if (xml.Contains("<descricao>"))
            if ((xml.ToLowerInvariant().Contains("<descricao>")) || (xml.ToLowerInvariant().Contains("<tabela>")) || (xml.ToLowerInvariant().Contains("<documento>")))
            {
                xml = xml.Replace(" <=", " &lt;=");
                xml = xml.Replace(" <,", " &lt;,");
                xml = xml.Replace(" >=", " &gt;=");
                xml = xml.Replace(" >,", " &gt;,");
                xml = xml.Replace(" < ", " &lt; ");
                xml = xml.Replace(" > ", " &gt; ");
                xml = xml.Replace(" & ", " &amp; ");
                xml = xml.Replace("&", "&amp;");
                xml = xml.Replace(@" "" ", " &quot; ");
                xml = xml.Replace("'", "&#39;");

                for (int iContador = 0; iContador < 9; iContador++)
                {
                    if (xml.IndexOf(String.Format(" <{0}", iContador)) != -1)
                        xml = xml.Replace(String.Format(" <{0}", iContador), String.Format(" &lt;{0}", iContador));
                }
            }

            return xml;
        }

        /// <summary>
        /// Sistema para reverter a ação efetuada pela função .TratarDescricaoXml(string)
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ReverterTratarDescricaoXml(string xml)
        {
            //Remove os sinais de  < e >, ocorre exception ao montar o xmlDoc
            xml = xml.Replace(" &lt#61;", " <=");
            xml = xml.Replace(" &gt#61;", " >=");
            xml = xml.Replace(" &lt; ", " < ");
            xml = xml.Replace(" &gt; ", " > ");
            xml = xml.Replace(" &amp; ", " & ");
            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace(" &quot; ", @" "" ");
            xml = xml.Replace("&#39;", "'");
            xml = xml.Replace(" &lt#61;,", " <,");
            xml = xml.Replace(" &gt#61;,", " >,");

            return xml;
        }
        #endregion
    }

    public static class xmlWriterExtensionMethods
    {
        [System.Diagnostics.DebuggerStepThrough]
        [System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static void WriteFullElementString(this XmlWriter xmlWriter, string localName, string value)
        {
            value = (value.IsNull() ? string.Empty : value);

            xmlWriter.WriteStartElement(localName);
            xmlWriter.WriteValue(value);
            xmlWriter.WriteEndElement();
        }
    }
}
