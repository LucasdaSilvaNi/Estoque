using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using Sam.Common;
using Sam.Common.Util;
using System.Collections;
using System.Security;
using System.Web;
using Sam.Domain.Entity;
using tipoMaterialParaLiquidacao = Sam.Common.Util.GeralEnum.TipoMaterial;
using Sam.Integracao.SIAF.Core;

namespace Sam.Domain.Business
{
    public sealed class Siafem
    {
        #region Propriedades

        public static string enderecoProxy
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["enderecoProxy"];
            }
        }
        public static string userNameProxy
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["userNameProxy"];
            }
        }
        public static string passProxy
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["passProxy"];
            }
        }
        public static string wsURLConsulta
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["wsURLConsulta"];
            }
        }
        public static string userNameConsulta
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["userNameConsulta"];
            }
        }
        public static string passConsulta
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["passConsulta"];
            }
        }
        public static string wsURLEnvio
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["wsURLEnvio"];
            }
        }
        public static string userNameEnvio
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["userNameEnvio"];
            }
        }
        public static string passEnvio
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["passEnvio"];
            }
        }
        public static string[] CatalogoMensagensSIAFEM
        {
            get
            {
                if (_catalogoInterno.IsNullOrEmpty())
                    Load();

                return _catalogoInterno.Keys.ToArray<string>();
            }
        }


        private static IDictionary<string, string> _catalogoInterno
        { get; set; }
        private static SortedList tagsErroMsgSIAFEM
        { get; set; }
        #endregion

        #region Métodos

        private static XmlWriterSettings ObterFormatacaoPadraoParaXml()
        {
            return new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
        }

        /// <summary>
        /// Método responsável pelo catálogo de todas as mensagens processadas (a procura de erros), pelo sistema.
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, string> initMensagens()
        {
            IDictionary<string, string> catalogoMensagens = null;
            KeyValuePair<string, string>[] arrNomeMsgSiafemComTagErro = null;
            string[] arrNomeMsgSiafisico = null;

            List<string> lstMensagensSiafisico = new List<string>(); ;
            List<KeyValuePair<string, string>> lstMensagensSiafemComTagErro = new List<KeyValuePair<string, string>>();

            arrNomeMsgSiafisico = new string[] { "SiafisicoDocCONNLItem", 
                                                 "SiafisicoDocConItemDetalha", 
                                                 "SiafisicoDocNLPregaoExecuta", 
                                                 "SiafisicoDocCONNLPregaoItem", 
                                                 "SiafisicoDocContNLBec", 
                                                 "SiafisicoDocContNLBecDet",
                                                 "SFCOLiquidaNL" };

            arrNomeMsgSiafemComTagErro = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("SiafisicoDocConsultaI", "MsgRetorno"),
                                                                              new KeyValuePair<string, string>("SiafisicoDocConsultaF", "MsgRetorno"),
                                                                              new KeyValuePair<string, string>("SiafisicoLogin", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemLogin", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemDocDetPTRES", "MsgRetorno"), 
                                                                              new KeyValuePair<string, string>("SiafemDocConsultaEmpenhos", "MsgRetorno"), 
                                                                              new KeyValuePair<string, string>("SiafemDocNLConsumo", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemDocNL", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemDocNLEmLiq", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemDocDetaContaGen", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SFCOLiqNLBec", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SFCONLPregao", "MsgErro"),
                                                                              new KeyValuePair<string, string>("SiafemDetaContaGen", "MsgErro") };

            catalogoMensagens = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            lstMensagensSiafisico.AddRange(arrNomeMsgSiafisico);
            lstMensagensSiafemComTagErro.AddRange(arrNomeMsgSiafemComTagErro);


            foreach (var mensagemSiafisico in lstMensagensSiafisico)
                catalogoMensagens.Add(mensagemSiafisico, "Msg");


            foreach (var mensagemSiafemTagErro in lstMensagensSiafemComTagErro)
                catalogoMensagens.Add(mensagemSiafemTagErro.Key, mensagemSiafemTagErro.Value);

            return catalogoMensagens;
        }
        /// <summary>
        /// Método responsável pelo catálogo de todas as mensagens processadas (a procura de erros), pelo sistema.
        /// </summary>
        /// <returns></returns>
        public static void Load()
        {
            _catalogoInterno = initMensagens();
            tagsErroMsgSIAFEM = new SortedList(_catalogoInterno as IDictionary, StringComparer.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Método utilizado para ignorar validação de certificado SSL
        /// </summary>
        private static void InitiateSSLTrust()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        }

        /// <summary>
        /// Função de tratamento para erro de autenticação aos sistemas SIAFEM/SIAFISICO, retornados via mensagem XML
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private static bool LoginInvalido(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            #region Variaveis
            bool blnRetorno = false;
            string strNomeSistema = null;
            strMensagemErro = null;
            #endregion Variaveis

            #region Tratamento Erro Servidor VHI
            var lstXmlElements = (new XmlDocument()).LoadXmlDocument(strRetornoMsgEstimulo)
                                                    .GetElementsByTagName("SIAFDOC")
                                                    .Cast<System.Xml.XmlElement>()
                                                    .ToList();

            if (!lstXmlElements.IsNullOrEmpty() && lstXmlElements.Count > 1)
            {
                foreach (var item in lstXmlElements)
                {
                    if (item.NextSibling.IsNotNull() && item.NextSibling.InnerText.Contains("RobosSiafNetVHI"))
                    {
                        strNomeSistema = item.ChildNodes[1].Name.Replace("Login", "").ToUpperInvariant();
                        var strPrefixoMensagemErro = String.Format("Retorno (Sistema {0}): {1}", strNomeSistema, strMensagemErro);
                        //strMensagemErro = String.Format("Erro {0} (SEFAZ/VHI): {1}.", strNomeSistema, strMensagemErro);

                        strMensagemErro = String.Format("{0}.\nFalha no servidor VHI/{1}. Favor contatar equipe CAU/SEFAZ para regularização.", strPrefixoMensagemErro, strNomeSistema);
                        blnRetorno = true;
                        break;
                    }
                }

            }
            #endregion Tratamento Erro Servidor VHI

            #region Outros Erros Login
            var elementoLogin = (new XmlDocument()).LoadXmlDocument(strRetornoMsgEstimulo)
                                                   .GetElementsByTagName("MsgErro")
                                                   .Cast<System.Xml.XmlElement>()
                                                   .ToList()
                                                   .FirstOrDefault();
            if (elementoLogin.IsNotNull())
            {
                strMensagemErro = elementoLogin.InnerText.BreakLine('-',0).Trim();
                strNomeSistema = elementoLogin.ParentNode.ParentNode.Name.Replace("Login", "").ToUpperInvariant();

                string strPrefixoMensagemErro = String.Format("Retorno (Sistema {0}): {1}", strNomeSistema, strMensagemErro);
                strMensagemErro = String.Format("Retorno (Sistema {0}): {1}", strNomeSistema, strMensagemErro);

                blnRetorno = true;
            }
            #endregion Outros Erros Login

            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar erros de conexão (timeout, infra, etc), quando do envio de mensagem de estÃ­mulo ao servidor SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private static bool ErroAcessoOuConexao(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            bool blnRetorno = false;
            string strRetorno = string.Empty;

            #region Erros Infra
            //erros quaisquer retornados por servidor SEFAZ (conexão)
            IList<string> listaErros = Constante.ErrosDeConexao;
            listaErros.ToList().ForEach(DescricaoErro => { if ((!String.IsNullOrWhiteSpace(strRetorno)) && ( String.Format("{0}.", strRetorno).ToLowerInvariant() == DescricaoErro.ToLowerInvariant() || strRetorno.ToLowerInvariant() == DescricaoErro.ToLowerInvariant())) { strRetorno = DescricaoErro; } });

            blnRetorno = !String.IsNullOrWhiteSpace(strRetorno);
            if (blnRetorno)
            {
                strRetorno = strRetorno.Replace("..", ".");
                if ((strRetorno == Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US) || (strRetorno == Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR) ||
                     (strRetorno == Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_EN_US) || (strRetorno == Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_PT_BR))
                    strRetorno = String.Format("Erro no Webservice! {0} ('{1}')\n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, "Serviço SIAFISICO/SIAFEM indisponível no momento.", "Favor entrar em contato com setor DTI da Secretaria da Fazenda.");
                else
                    strRetorno = String.Format("Erro no Webservice! {0} ('{1}')\n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, strRetorno, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);

                strMensagemErro = strRetorno;
                return blnRetorno;
            }
            #endregion Erros Infra

            strMensagemErro = strRetorno;
            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar retornos não-padrão do servidor SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private static bool RetornoInvalido(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            bool blnRetorno = false;
            XmlNode elRetornoInvalido = null;
            strMensagemErro = string.Empty;

            //Nenhum texto retornado
            if (String.IsNullOrWhiteSpace(strRetornoMsgEstimulo))
            {
                strMensagemErro = "Servidor SIAFEM/SIAFISICO não retornou dados.";
                blnRetorno = true;
            }
            else if (strRetornoMsgEstimulo.ToLowerInvariant().Contains("tried to invoke method") || 
                     strRetornoMsgEstimulo.ToLowerInvariant().Contains("java.lang.classcastexception@")) //Erro de processamento no container web webservice
            {
                strRetornoMsgEstimulo = strRetornoMsgEstimulo.Replace("\r", " ").Replace("\t", "").Replace("\n", "");

                strMensagemErro = "Consulta aos servidores SIAFEM/SIAFISICO indisponível no momento.\nFavor enviar e-mail ao suporte do sistema, informando esta mensagem.";
                blnRetorno = true;
            }
            else if (strRetornoMsgEstimulo.IndexOf("->") > 0) //MudaPah (primeira linha não é válida, logo terá que ser omitido)
            {
                strMensagemErro = strRetornoMsgEstimulo.Substring(strRetornoMsgEstimulo.IndexOf("->") + 2, strRetornoMsgEstimulo.Length - strRetornoMsgEstimulo.IndexOf("->") - 2);
                elRetornoInvalido = Sam.Common.XmlUtil.lerXml(strRetornoMsgEstimulo, "SIAFDOC/SiafemMudapah/Mudapah/MsgErro");

                if (elRetornoInvalido.InnerText.ToUpperInvariant() == "===>  PARAMETROS ADICIONAIS ATUALIZADOS PARA ESTA SESSAO")
                    strMensagemErro = "Erro no Webservice. Procedimento efetuado com sucesso, porém com retorno SIAFEM/SIAFISICO inesperado (MUDAPAH).";
                else
                    strMensagemErro = String.Format("Erro no Webservice: {0}", elRetornoInvalido.InnerText);

                blnRetorno = true;
            }
            else
            {
                XmlUtil.IsXML(strRetornoMsgEstimulo, ref strMensagemErro);
                strMensagemErro = "Erro processando empenho {codigoUGE}/{numeroEmpenho}. Informar administrador do sistema.";

                blnRetorno = true;

                throw new XmlException(strMensagemErro);
            }

            return blnRetorno;
        }

        /// <summary>
        /// Função para verificar qualquer erro/má-formação de retorno de chamada do método .recebeMsg(string, string, string, string, string, bool)
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strNomeMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        public static bool VerificarErroMensagem(string strRetornoMsgEstimulo, out string strNomeMsgEstimulo, out string strMensagemErro)
        {
            #region Variaveis
            bool problemaRetornoSiafem = false;
            bool naoExisteTagStatusOperacao = false;

            string strStatusOperacao = null;
            string strTagErroMsgSEFAZ = null;

            XmlDocument docXml = null;

            strNomeMsgEstimulo = null;
            strMensagemErro = null;
            #endregion Variaveis

            if (_catalogoInterno.IsNull())
                Siafem.Load();

            #region Tratamento Erros Diversos
            //Se não há mensagem XML retornada...
            if(!XmlUtil.IsXML(strRetornoMsgEstimulo, ref strMensagemErro))
            {
                LogErro.GravarStackTrace(String.Format("Retorno SIAFEM (erro): {0}, Retorno Estímulo: {1}", strMensagemErro, strRetornoMsgEstimulo));

                if (!String.IsNullOrWhiteSpace(strMensagemErro))
                    //strMensagemErro = String.Format("Retorno SIAFEM/SIAFISICO (erro): {0}", strMensagemErro);
                    strMensagemErro = String.Format("Retorno SIAFEM/SIAFISICO (erro): {0}", "XML mal-formado retornado por SIAFEM.");

                    return true;
            }
            else if (!XmlUtil.IsXML(strRetornoMsgEstimulo))
            {
                return (RetornoInvalido(strRetornoMsgEstimulo, out strMensagemErro) ||
                        ErroAcessoOuConexao(strRetornoMsgEstimulo, out strMensagemErro));
            }
            else if(strRetornoMsgEstimulo.ToLowerInvariant().Contains("horarioacesso"))
            {
                return HorarioAcessoInvalido(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);
            }
            #endregion Tratamento Erros Diversos

            docXml = new XmlDocument();
            docXml.LoadXml(strRetornoMsgEstimulo);


            strNomeMsgEstimulo = docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
            var _sistemaSIAF = ((strNomeMsgEstimulo.Contains("Siafisico") || strNomeMsgEstimulo.Contains("SFCODOC")) ? "SIAFISICO" : "SIAFEM");

            if (Siafem.tagsErroMsgSIAFEM[strNomeMsgEstimulo].IsNotNull())
            {
                strTagErroMsgSEFAZ = Siafem.tagsErroMsgSIAFEM[strNomeMsgEstimulo].ToString();
            }
            else
            {
                var _descErro = String.Format("Erro SAM: Processamento Transação {1}: {0}", strNomeMsgEstimulo, _sistemaSIAF);

                LogErro.GravarMsgInfo("Erro Processamanto Retorno VHI/SEFAZ", String.Format("\nMensagem {0} não cadastrada.\n\n{1}", _sistemaSIAF, _descErro));
                strMensagemErro = _descErro;
            }                

            
            
            if (docXml.GetElementsByTagName("StatusOperacao").Cast<XmlNode>().FirstOrDefault().IsNotNull())
                strStatusOperacao = docXml.GetElementsByTagName("StatusOperacao").Cast<XmlNode>().FirstOrDefault().InnerText;

            naoExisteTagStatusOperacao = !Boolean.TryParse(strStatusOperacao, out problemaRetornoSiafem);

            if (!naoExisteTagStatusOperacao)
                problemaRetornoSiafem |= !Boolean.Parse(strStatusOperacao);

            if (problemaRetornoSiafem || naoExisteTagStatusOperacao)
            {
                if (Siafem.tagsErroMsgSIAFEM.ContainsKey(strNomeMsgEstimulo) &&
                    Siafem.tagsErroMsgSIAFEM[strNomeMsgEstimulo].ToString().IsNotNull() &&
                    (docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().Count() > 0) &&
                    !String.IsNullOrWhiteSpace(docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText))
                {
                    strMensagemErro = String.Format("Mensagem retornada pelo sistema {1}: {0}.", docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText, _sistemaSIAF).Replace("-", "").Trim();
                }

                // Erro Login
                if (strNomeMsgEstimulo.Contains("Login") && String.IsNullOrWhiteSpace(strMensagemErro))
                {
                    strMensagemErro = null;
                    LoginInvalido(strRetornoMsgEstimulo, out strMensagemErro);
                }

                //Retorno Tag Vazia
                if (docXml.GetElementsByTagName("Doc_Retorno").IsNotNull() &&
                    docXml.GetElementsByTagName("Doc_Retorno").Count == 1 &&
                    !docXml.GetElementsByTagName("Doc_Retorno")[0].HasChildNodes)
                {
                    strMensagemErro = String.Format("Sistema {0} não retornou dados para estímulo enviado [{1}].", _sistemaSIAF, strNomeMsgEstimulo);

                    return true;
                }

                if (String.IsNullOrWhiteSpace(strMensagemErro) && 
                    docXml.GetElementsByTagName("MsgErro").Cast<XmlNode>().FirstOrDefault().IsNotNull() &&
                    docXml.GetElementsByTagName("MsgRetorno").Cast<XmlNode>().FirstOrDefault().IsNotNull())
                {
                    var capturaMsgErro = (docXml.GetElementsByTagName("MsgErro").Cast<XmlNode>().FirstOrDefault() ?? docXml.GetElementsByTagName("MsgRetorno").Cast<XmlNode>().FirstOrDefault()).InnerText;
                }

                //problemaRetornoSiafem = (((problemaRetornoSiafem && naoExisteTagStatusOperacao) || !String.IsNullOrWhiteSpace(strMensagemErro)) && (!String.IsNullOrWhiteSpace(strStatusOperacao) && strStatusOperacao.ToLowerInvariant() == "false" && !String.IsNullOrWhiteSpace(strMensagemErro)));
                problemaRetornoSiafem = (((problemaRetornoSiafem && naoExisteTagStatusOperacao) || !String.IsNullOrWhiteSpace(strMensagemErro)) && ((!String.IsNullOrWhiteSpace(strStatusOperacao) && strStatusOperacao.ToLowerInvariant() == "false" && !String.IsNullOrWhiteSpace(strMensagemErro)) || (String.IsNullOrWhiteSpace(strStatusOperacao) && !String.IsNullOrWhiteSpace(strMensagemErro))));
            }

            return problemaRetornoSiafem;
        }

        private static bool HorarioAcessoInvalido(string strRetornoMsgEstimulo, out string strNomeMsgEstimulo, out string strMensagemErro)
        {
            var docXml = new XmlDocument();
            docXml.LoadXml(strRetornoMsgEstimulo);

            strNomeMsgEstimulo = strMensagemErro = "";

            if (docXml.IsNotNull())
            {
                strNomeMsgEstimulo = docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
                strMensagemErro = (docXml.GetElementsByTagName("MsgErro").Cast<XmlNode>().FirstOrDefault() ?? docXml.GetElementsByTagName("MsgRetorno").Cast<XmlNode>().FirstOrDefault()).InnerText;
            }

            return (!String.IsNullOrWhiteSpace(strMensagemErro));
        }

        /// <summary>
        /// Função para verificar qualquer erro/má-formação de retorno de chamada do método .recebeMsg(string, string, string, string, string, bool)
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strNomeMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        public static bool VerificarErroMensagem(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            if (_catalogoInterno.IsNull())
                Siafem.Load();

            string strNull = null;
            return Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNull, out strMensagemErro);
        }

        /// <summary>
        /// Método de consulta os webservices SEFAZ (SIAFEM/SIAFISICO)
        /// </summary>
        /// <param name="strLoginUsuario"></param>
        /// <param name="strSenhaUsuario"></param>
        /// <param name="strAnoBase"></param>
        /// <param name="strUnidadeGestora"></param>
        /// <param name="strMsgEstimulo"></param>
        /// <param name="isConsulta"></param>
        /// <returns></returns>
        public static string recebeMsg(string strLoginUsuario, string strSenhaUsuario, string strAnoBase, string strUnidadeGestora, string strMsgEstimulo, bool isConsulta)
        {
            try
            {
                Siafem.Load();
                string lStrRetorno = string.Empty;
                WSSiafem.RecebeMSG oWS = new WSSiafem.RecebeMSG();

                InitiateSSLTrust();
                oWS.Url = wsURLConsulta;

                if (isConsulta)
                {
                    strUnidadeGestora = String.Empty;
                    oWS.Url = wsURLConsulta;
                }

                if (oWS.Url.Contains("siafemhom.intra.fazenda.sp.gov.br"))
                    oWS.UseDefaultCredentials = true;
                else if (oWS.Url.Contains("www6.fazenda.sp.gov.br"))
                    oWS.Proxy = new System.Net.WebProxy(enderecoProxy); //IIS Proxy.


                lStrRetorno = oWS.Mensagem(strLoginUsuario, strSenhaUsuario, strAnoBase, strUnidadeGestora, strMsgEstimulo);
                lStrRetorno = lStrRetorno.RetirarGrandesEspacosEmBranco();

                return GeradorEstimuloSIAF.TratarDescricaoXml(lStrRetorno);
            }
            catch (TimeoutException excTimeoutSolicitacao)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                excErroParaPropagacao = new Exception(erroRetorno, excTimeoutSolicitacao);

                throw excErroParaPropagacao;
            }
            catch (WebException lWebExcErroRuntime)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                if (lWebExcErroRuntime.Message.Contains(" (404) "))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if (lWebExcErroRuntime.Message.Contains(" (504) "))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if ((lWebExcErroRuntime.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US)) || (lWebExcErroRuntime.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR)))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_AMIGAVEL_SERVIDORES_SIAF_INDISPONIVEIS, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else
                    erroRetorno = String.Format("{0}", lWebExcErroRuntime.Message);

                excErroParaPropagacao = new Exception(erroRetorno, lWebExcErroRuntime);

                throw excErroParaPropagacao;
            }
            catch (InvalidOperationException lExcErroExecucaoProcedimento)
            {
                new LogErro().GravarLogErro(lExcErroExecucaoProcedimento);

                return string.Format("{0}\n{1}", Constante.CST_MSG_ERRO_PROCESSAMENTO_OPERACAO, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
            }
            catch (Exception lExcErroRuntime)
            {
                //return string.Format("Erro: {0}{1}Acionar administrador do sistema!", lExcErroRuntime.Message, Environment.NewLine);
                return string.Format("{0}", lExcErroRuntime.Message);
            }
        }

        static internal string TratarErroLoginSistema(string strMensagemErro)
        {
            string strMsgParam = strMensagemErro;

            if (!String.IsNullOrWhiteSpace(strMsgParam) && (strMsgParam.IndexOf("(Senha SIAF") != -1))
            {
                int compMsgErro = strMsgParam.Length;
                int idxMsgSenha = strMsgParam.IndexOf("(Senha SIAF");

                strMsgParam = strMsgParam.Remove(idxMsgSenha - 1, compMsgErro - idxMsgSenha);
                strMsgParam = String.Format("{0}\n{1}", strMensagemErro, "Senha Sistema SAM expirada. Favor informar administrador do sistema!");
            }
            return strMsgParam;
        }
        #endregion

        #region Consultas Empenhos

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocDetaContaGen(int iCodigoUG, int iCodigoGestao, string strMes, string strContaContabil, string strOpcao)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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

            //Descarrega o conteúdo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocConsultaEmpenhos(int iCodigoUG, int iCodigoGestao, string strNumeroEmpenho)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocListaEmpenhos(string strCpfCnpj, int iCodigoGestao, int iAnoBase, int iCodigoUG)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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
        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocListaEmpenhos(SortedList parametrosConsulta)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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

        #endregion Consultas Empenhos

        #region Consulta Cadastro

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafisicoDocConsultaF(long lngCpfCnpjFornecedor)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConsultaF");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocConsultaF");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CgcCpf", lngCpfCnpjFornecedor.ToString());
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

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafisicoDocConsultaI(string strCodigoItemMaterial)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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

            return strRetorno;
        }

        #endregion Consulta Cadastro

        #region PTRes

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocDetPTRES(int iCodigoUG, int iCodigoGestao, string strAnoMesReferencia)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();

            DateTime dtMesRef = strAnoMesReferencia.RetornarMesAnoReferenciaDateTime();
            int ultimoDiaMes = dtMesRef.MonthTotalDays();
            string strMesAbreviado = MesExtenso.Mes[dtMesRef.Month];

            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFDetPTRES");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocDetPTRES");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UG", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("Mes", strMesAbreviado);
            xmlMontadorEstimulo.WriteElementString("DiaMesInicial", "01");
            xmlMontadorEstimulo.WriteElementString("DiaMesFinal", ultimoDiaMes.ToString("D2"));
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

        /// <summary>
        /// Método experimental
        /// </summary>
        /// <param name="iCodigoUG"></param>
        /// <param name="iCodigoGestao"></param>
        /// <param name="strMes"></param>
        /// <param name="strContaContabil"></param>
        /// <param name="strOpcoes"></param>
        /// <returns></returns>
        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocDetaContaGen(int iCodigoUG, int iCodigoGestao, string strMes, string strContaContabil, string[] strOpcoes)
        {
            string strRetorno = null;
            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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
            //xmlMontadorEstimulo.WriteElementString("Opcao", strOpcao);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteúdo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        #endregion PTRes

        #region Fechamento
        //Verificar motivo do erro
        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocNLConsumo(int iCodigoUG, int iUGConsumidora, int iUAConsumidora, int iPTRes, int iCodigoGestao, bool IsEstorno, string strDataEmissao, long iNaturezaDespesa, decimal decValorBaixaFechamento, string strRelacaoNotasSAM)
        {
            string strRetorno = null;
            string strTipoLancamento = ((IsEstorno) ? "E" : "N");
            string strValorBaixaFechamento = decValorBaixaFechamento.ToString("#.00").Replace(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, "").Trim().PadLeft(17, '0');
            string strNotasSAM = ((!String.IsNullOrWhiteSpace(strRelacaoNotasSAM))? String.Format("Notas Saida Material SAM: {0}", strRelacaoNotasSAM.Replace("\n", ", ")) : null);

            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
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

            //Descarrega o conteúdo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        #endregion Fechamento

        #region Incorporação/Reclassificação Empenhos
        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocNL(int iCodigoUG, int iCodigoGestao, string strDataEmissao, string strNumeroEmpenho, string naturezaDespesa, string documentoSAM, string eventoPagamento, string classificacaoPagamento, string[] observacoesMovimentacao, decimal valorDocumento, bool estorno = false)
        {

            string strRetorno = null;
            string strPrefixoNe = String.Format("{0}NE", strDataEmissao.Substring(5,4));
            string strNumeroEmpenhoValidado = (!String.IsNullOrWhiteSpace(strNumeroEmpenho) && strNumeroEmpenho.Contains(strPrefixoNe)) ? strNumeroEmpenho.Replace(strPrefixoNe, "") : strNumeroEmpenho;

            string[] numeroDecimal = null;


            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNL001");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNL");
            xmlMontadorEstimulo.WriteStartElement("documento");

            xmlMontadorEstimulo.WriteFullElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteFullElementString("CgcCpfUgfav", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("GestaoFav", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteEndElement();

            xmlMontadorEstimulo.WriteStartElement("Evento");
            numeroDecimal = valorDocumento.ToString(CultureInfo.GetCultureInfo("pt-BR")).Split(new char[] { ',' });
            numeroDecimal[1] = numeroDecimal[1].Substring(0, 2);

            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("desc");
            xmlMontadorEstimulo.WriteFullElementString("Evento", eventoPagamento);
            xmlMontadorEstimulo.WriteFullElementString("InscricaoEvento", strNumeroEmpenho);
            xmlMontadorEstimulo.WriteFullElementString("RecDesp", null); //Valor sempre será nulo, vide funcional.
            xmlMontadorEstimulo.WriteFullElementString("Classificacao", classificacaoPagamento);
            xmlMontadorEstimulo.WriteFullElementString("Fonte", null); //Valor sempre será nulo, vide funcional.
            
            //VALOR MOVIMENTACAO
            numeroDecimal = valorDocumento.ToString(CultureInfo.GetCultureInfo("pt-BR")).Split(new char[] { ',' });
            numeroDecimal[1] = numeroDecimal[1].Substring(0, 2);

            var _valorMovimentacao = String.Format("{0}{1}", numeroDecimal[0], numeroDecimal[1]);
            _valorMovimentacao = _valorMovimentacao.PadLeft(17, '0');
            xmlMontadorEstimulo.WriteFullElementString("Valor", _valorMovimentacao);

            xmlMontadorEstimulo.WriteEndElement();
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

            return strRetorno;
        }

        [Obsolete("Utilizar método de mesmo nome na classe Sam.Integracao.SIAF.Core.GeradorEstimuloSIAF", true)]
        public static string SiafemDocNLEmLiq(int iCodigoUG, int iCodigoGestao, string strDataEmissao, string strNumeroEmpenho, string naturezaDespesa, string documentoSAM, string[] observacoesMovimentacao, decimal valorDocumento, bool estornoPagamento = false)
        {
            string strRetorno = null;
            string anoNotaEmpenho = strDataEmissao.Substring(5, 4);
            string strPrefixoNe = String.Format("{0}NE", anoNotaEmpenho);
            string strNumeroEmpenhoValidado = (!String.IsNullOrWhiteSpace(strNumeroEmpenho) && strNumeroEmpenho.Contains(strPrefixoNe)) ? strNumeroEmpenho.Replace(strPrefixoNe, "") : strNumeroEmpenho;

            string[] numeroDecimal = null;

            #region Tipo Ação Pagamento
            bool isPagamento = !estornoPagamento;
            string strAcaoPagamentoEstorno, strAcaoPagamentoNormal;

            strAcaoPagamentoNormal = ((isPagamento) ? "x" : string.Empty);
            strAcaoPagamentoEstorno = ((!isPagamento) ? "x" : string.Empty);

            #endregion 

            StringBuilder sbXml = new StringBuilder();
            XmlWriter xmlMontadorEstimulo = null;
            XmlWriterSettings xmlSettings = Siafem.ObterFormatacaoPadraoParaXml();


            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNLEmLiq");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNLEmLiq");
            xmlMontadorEstimulo.WriteStartElement("documento");

            xmlMontadorEstimulo.WriteFullElementString("DataEmissao", strDataEmissao);
            xmlMontadorEstimulo.WriteFullElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteFullElementString("Normal", strAcaoPagamentoNormal);
            xmlMontadorEstimulo.WriteFullElementString("Estorno", strAcaoPagamentoEstorno);

            xmlMontadorEstimulo.WriteFullElementString("Ano", anoNotaEmpenho);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", strNumeroEmpenhoValidado);

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

            return strRetorno;
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

    public static class SiafemXmllWriterExtensionMethods
    {
        public static string ToStringDataFormatoSiafem(this DateTime dataParaConversao)
        {
            string strDataConvertida = null;

            int mesRef = dataParaConversao.Month;
            int anoRef = dataParaConversao.Year;
            int diaMes = dataParaConversao.Day;
            strDataConvertida = String.Format("{0}{1}{2}", diaMes, MesExtenso.Mes[mesRef].ToUpperInvariant(), anoRef);

            return strDataConvertida;
        }
    }
}
