using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sam.Business;
using Sam.Common.Util;
using Sam.Domain.Business.Auditoria;
using Sam.Domain.Entity;
using Sam.Domain.Entity.DtoWs;
using Sam.Infrastructure;
using Sam.Integracao.ConsultaWs.Constante;
using Sam.Integracao.ConsultaWs.Interface;
using Sam.Integracao.ConsultaWs.Parametro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Globalization;
using static Sam.Common.Util.GeralEnum;
using cstConstante = Sam.Common.Util.Constante;
using oldBusiness = Sam.Domain.Business;
using oldInfra = Sam.Domain.Infrastructure;



namespace Sam.Integracao.ConsultaWs.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceWs" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServiceWs.svc or ServiceWs.svc.cs at the Solution Explorer and start debugging.
    //public class ConsultasWs : BaseBusiness, IConsultasWs, IHttpModule
    public class ConsultasWs : IConsultasWs, IHttpModule
    {
        internal IncomingWebRequestContext ctxRequisicaoWs = null;

        public static string obterEstimuloJson()
        {
            OperationContext oc = OperationContext.Current;

            if (oc == null)
                //throw new Exception("Objeto OperationContext em estado inválido");
                throw new Exception(MensagemErro.MSG_ERRO__OBJETO_OPERATIONCONTEXT_ESTADO_INVALIDO);

            System.ServiceModel.Channels.MessageEncoder encoder = oc.IncomingMessageProperties.Encoder;
            string contentType = encoder.ContentType;


            var operationContextType = oc.GetType();
            object bufferedMessage = operationContextType.InvokeMember("request", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, oc, null);
            var bufferedMessageType = bufferedMessage.GetType();

            object messageData = bufferedMessageType.InvokeMember("MessageData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, bufferedMessage, null);
            var jsonBufferedMessageDataType = messageData.GetType();

            object buffer = jsonBufferedMessageDataType.InvokeMember("Buffer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, messageData, null);

            ArraySegment<byte> arrayBuffer = (ArraySegment<byte>)buffer;

            //Encoding encoding = Encoding.GetEncoding(characterSet);
            Encoding encoding = Encoding.GetEncoding("utf-8");

            string requestMessage = encoding.GetString(arrayBuffer.Array, arrayBuffer.Offset, arrayBuffer.Count);

            return requestMessage;
        }

        public string BuscaSubItemMaterialRequisicaoParaWs(ConsultaSubitemWs dadosConsulta)
        {
            string respJson = null;
            //string fmtMsgErro = null;
            int numeroPaginaConsulta = -1;
            IList<dtoWsSubitemMaterial> lstRetornoConsultaRelatorio = null;
            IList<string> listaParametrosComErro = null;
            dtoWsRetornoConsulta dtoRetornoConsulta = new dtoWsRetornoConsulta();
            DateTime dataHoraRecebimentoMsg = DateTime.Now;



            this.ctxRequisicaoWs = WebOperationContext.Current.IncomingRequest;
            listaParametrosComErro = consisteParametrosWs_ConsultaCatalogo(dadosConsulta);
            if (listaParametrosComErro.HasElements())
            {
                //fmtMsgErro = "Parâmetro(s) {0} informado(s) incorretamente. Favor verificar dado(s) informado(s).";
                //dtoRetornoConsulta.MensagemErro = String.Format(fmtMsgErro, String.Join(", ", listaParametrosComErro));
                dtoRetornoConsulta.MensagemErro = String.Format(MensagemErro.FMT_MSGERRO__PARAMETRO_INFORMADO_INCORRETAMENTE, String.Join(", ", listaParametrosComErro));
            }
            else
            {
                if (consisteCredenciaisWs(dadosConsulta.cpf, dadosConsulta.senha, dadosConsulta.codigoOrgao))
                {

                    if (ValidarLoginUsuarioWs(dadosConsulta.cpf, dadosConsulta.senha, dadosConsulta.codigoOrgao))
                    {
                        SubItemMaterialBusiness objBusiness = new SubItemMaterialBusiness();

                        int.TryParse(dadosConsulta.NumeroPaginaConsulta, out numeroPaginaConsulta);
                        int codigoOrgao = int.Parse(dadosConsulta.codigoOrgao);
                        int codigoAlmox = int.Parse(dadosConsulta.codigoAlmox);

                        lstRetornoConsultaRelatorio = objBusiness.BuscaSubItemMaterialRequisicaoParaWs(dadosConsulta.termoParaPesquisa, codigoOrgao, codigoAlmox, true, numeroPaginaConsulta);
                        if (lstRetornoConsultaRelatorio.HasElements())
                        {
                            dtoRetornoConsulta.DadosConsulta = lstRetornoConsultaRelatorio.ToList<object>();
                            dtoRetornoConsulta.TotalPaginas = ((objBusiness.TotalRegistros / cstConstante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS) + 1);
                            dtoRetornoConsulta.NumeroPaginaConsulta = ((numeroPaginaConsulta == 0) ? ++numeroPaginaConsulta : numeroPaginaConsulta); //Se parâmetro de paginação não informado, sempre devolver 'Página 1';

                        }
                        else
                        {
                            //dtoRetornoConsulta.MensagemErro = "CONSULTA NAO RETORNOU DADOS";
                            dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__CONSULTA_NAO_RETORNOU_DADOS;
                        }
                    }
                }
                else
                {
                    //dtoRetornoConsulta.MensagemErro = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
                    dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS;
                }
            }
            respJson = JsonConvert.SerializeObject(dtoRetornoConsulta, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            JToken tokenizer = JToken.Parse(respJson);
            respJson = tokenizer.ToString(Formatting.Indented);

            var msgEstimuloConsulta = obterEstimuloJson();
            msgEstimuloConsulta = removeSenhaUsuario(msgEstimuloConsulta);
            var auditoriaIntegracao = gerarRegistroAuditoria(dadosConsulta.cpf, String.Empty, msgEstimuloConsulta, respJson, dataHoraRecebimentoMsg, DateTime.Now, "WS_SAM_EST");
            var statusRegistroAuditoria = inserirRegistroAuditoriaIntegracao(auditoriaIntegracao);

            return respJson;
        }

        public string GeraRelacaoMovimentacaoMaterialWs(DadosRelatorioWs dadosRelatorio)
        {
            string respJson = null;
            //string fmtMsgErro = null;
            int numeroPaginaConsulta = -1;
            IList<string> listaParametrosComErro = null;
            IList<dtoWsMovimentacaoMaterial> lstRetornoConsultaRelatorio = null;
            dtoWsRetornoConsulta dtoRetornoConsulta = null;
            DateTime dataHoraRecebimentoMsg = DateTime.Now;



            this.ctxRequisicaoWs = WebOperationContext.Current.IncomingRequest;
            listaParametrosComErro = consisteParametrosWs_RelatorioMovimentacao<DadosRelatorioWs>(dadosRelatorio);
            dtoRetornoConsulta = new dtoWsRetornoConsulta();
            if (listaParametrosComErro.HasElements())
            {
                //fmtMsgErro = "Parâmetro(s) {0} informado(s) incorretamente. Favor verificar dado(s) informado(s).";
                //dtoRetornoConsulta.MensagemErro = String.Format(fmtMsgErro, String.Join(", ", listaParametrosComErro));
                dtoRetornoConsulta.MensagemErro = String.Format(MensagemErro.FMT_MSGERRO__PARAMETRO_INFORMADO_INCORRETAMENTE, String.Join(", ", listaParametrosComErro));
            }
            else
            {
                if (consisteCredenciaisWs(dadosRelatorio.cpf, dadosRelatorio.senha, dadosRelatorio.codigoOrgao))
                {
                    if (ValidarLoginUsuarioWs(dadosRelatorio.cpf, dadosRelatorio.senha, dadosRelatorio.codigoOrgao))
                    {
                        oldBusiness.MovimentoItemBusiness objBusiness = new oldBusiness.MovimentoItemBusiness();

                        #region Variaveis
                        int loginUsuarioSamID = -1;
                        int loginUsuarioRequisitanteSamID = -1;
                        int loginUsuarioEstornoSamID = -1;
                        int loginUsuarioRequisitanteEstornoSamID = -1;
                        int.TryParse(dadosRelatorio.NumeroPaginaConsulta, out numeroPaginaConsulta);
                        int codigoOrgao = int.Parse(dadosRelatorio.codigoOrgao);
                        //int codigoUGE = int.Parse(dadosRelatorio.codigoUge);
                        //int codigoAlmox = int.Parse(dadosRelatorio.codigoAlmox);
                        int codigoUGE = -1;
                        int codigoAlmox = -1;
                        codigoUGE = (!Int32.TryParse(dadosRelatorio.codigoUge, out codigoUGE) ? 999999 : codigoUGE);
                        codigoAlmox = (!Int32.TryParse(dadosRelatorio.codigoAlmox, out codigoAlmox) ? 999 : codigoAlmox);
                        //bool _consultaTransferencia = false;
                        int _codigoUA = -1; int? codigoUA = ((int.TryParse(dadosRelatorio.codigoUa, out _codigoUA)) ? _codigoUA : (int?)null);
                        int _codigoDivisaoUA = -1; int? codigoDivisaoUA = ((int.TryParse(dadosRelatorio.codigoDivisaoUa, out _codigoDivisaoUA)) ? _codigoDivisaoUA : (int?)null);
                        int codigoTipoMovimentacaoMaterial = ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && int.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out codigoTipoMovimentacaoMaterial)) ? codigoTipoMovimentacaoMaterial : 0);
                        int codigoAgrupamentoTipoMovimentacaoMaterial = ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && int.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out codigoAgrupamentoTipoMovimentacaoMaterial)) ? codigoAgrupamentoTipoMovimentacaoMaterial : 0);
                        //bool consultaTransfs = (!String.IsNullOrWhiteSpace(dadosRelatorio.consultaTransfs) && bool.TryParse(dadosRelatorio.consultaTransfs, out _consultaTransferencia));
                        DateTime _dataInicial = DateTime.MinValue;
                        DateTime _dataFinal = DateTime.MinValue;
                        //DateTime.TryParse(String.Format("01-{0}", dadosRelatorio.dataFinal.Insert(2, "-")), out _dataFinal);
                        //DateTime.TryParse(String.Format("01-{0}", dadosRelatorio.dataInicial.Insert(2, "-")), out _dataInicial);
                        //_dataFinal = _dataFinal.MonthLastDay();
                        _dataFinal = (!String.IsNullOrWhiteSpace(dadosRelatorio.mesReferenciaFinal) ? dadosRelatorio.mesReferenciaFinal.RetornarMesAnoReferenciaDateTime().MonthLastDay() : dadosRelatorio.dataConsultaFinal.RetornarData());
                        _dataInicial = (!String.IsNullOrWhiteSpace(dadosRelatorio.mesReferenciaInicial) ? dadosRelatorio.mesReferenciaInicial.RetornarMesAnoReferenciaDateTime() : dadosRelatorio.dataConsultaInicial.RetornarData());
                        
                        #endregion Variaveis

                        //lstRetornoConsultaRelatorio = objBusiness.GeraRelacaoMovimentacaoMaterialWs(codigoOrgao, codigoUGE, codigoAlmox, codigoUA, codigoDivisaoUA, codigoTipoMovimentacaoMaterial, codigoAgrupamentoTipoMovimentacaoMaterial, dadosRelatorio.cpfCnpjFornecedor, _dataInicial, _dataFinal, consultaTransfs, numeroPaginaConsulta);
                        lstRetornoConsultaRelatorio = objBusiness.GeraRelacaoMovimentacaoMaterialWs(codigoOrgao, codigoUGE, codigoAlmox, codigoUA, codigoDivisaoUA, codigoTipoMovimentacaoMaterial, codigoAgrupamentoTipoMovimentacaoMaterial, dadosRelatorio.cpfCnpjFornecedor, _dataInicial, _dataFinal, null, numeroPaginaConsulta);
                        if (lstRetornoConsultaRelatorio.HasElements())
                        {
                            lstRetornoConsultaRelatorio.ToList()
                                                       .ForEach(registro => {
                                                                                registro.CPF_Nome_Usuario = (Int32.TryParse(registro.UsuarioSamID, out loginUsuarioSamID) ? ConsultasWsExtensions.ObterCPFNomeUsuarioSAM(loginUsuarioSamID) : null);
                                                                                registro.CPF_Nome_Requisitante = (Int32.TryParse(registro.RequisitanteSamID, out loginUsuarioRequisitanteSamID) ? ConsultasWsExtensions.ObterCPFNomeUsuarioSAM(loginUsuarioRequisitanteSamID) : null);

                                                                                registro.CPF_Nome_UsuarioEstorno = (Int32.TryParse(registro.UsuarioSamIDEstorno, out loginUsuarioEstornoSamID) ? ConsultasWsExtensions.ObterCPFNomeUsuarioSAM(loginUsuarioEstornoSamID) : null);
                                                                                registro.CPF_Nome_RequisitanteEstorno = (Int32.TryParse(registro.RequisitanteSamIDEstorno, out loginUsuarioRequisitanteEstornoSamID) ? ConsultasWsExtensions.ObterCPFNomeUsuarioSAM(loginUsuarioRequisitanteEstornoSamID) : null);
                                                                            });

                            dtoRetornoConsulta.DadosConsulta = lstRetornoConsultaRelatorio.ToList<object>();
                            dtoRetornoConsulta.TotalPaginas = ((objBusiness.TotalRegistros / cstConstante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS) + 1);
                            //dtoRetornoConsulta.NumeroPaginaConsulta = ((numeroPaginaConsulta == 0) ? 1 : numeroPaginaConsulta);
                            dtoRetornoConsulta.NumeroPaginaConsulta = ((numeroPaginaConsulta == 0) ? 1 : ((numeroPaginaConsulta > dtoRetornoConsulta.TotalPaginas) ? dtoRetornoConsulta.TotalPaginas : numeroPaginaConsulta));
                        }
                        else
                        {
                            //dtoRetornoConsulta.MensagemErro = "CONSULTA NAO RETORNOU DADOS";
                            dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__CONSULTA_NAO_RETORNOU_DADOS;
                        }
                    }
                    else
                    {
                        //dtoRetornoConsulta.MensagemErro = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
                        dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS;
                    }
                }
                else
                {
                    //dtoRetornoConsulta.MensagemErro = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
                    dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS;
                }
            }

            respJson = JsonConvert.SerializeObject(dtoRetornoConsulta, Formatting.Indented, new JsonSerializerSettings() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();
            JToken tokenizer = JToken.Parse(respJson);
            tokenizer = removeCamposID(tokenizer); //Retirada de campos utilizados como suporte, para inclusão do usuário de lançamento da movimentação de material;
            tokenizer = removeCamposCpfComValorNuloOuVazio(tokenizer); //Só exibir CPF/Nome de usuários, quando possuem dados (estorno, ou quem requisitou/estornou requisição tambem);
            tokenizer = removeCamposEspecificosRequisicaoValorNuloOuVazio(tokenizer); //Só exibir quantidade solicitada de subitens, quando for consulta para saida de material por requsicao;

            respJson = tokenizer.ToString(Formatting.Indented);


            var msgEstimuloConsulta = obterEstimuloJson();
            msgEstimuloConsulta = removeSenhaUsuario(msgEstimuloConsulta);


            var auditoriaIntegracao = gerarRegistroAuditoria(dadosRelatorio.cpf, String.Empty, msgEstimuloConsulta, respJson, dataHoraRecebimentoMsg, DateTime.Now, "WS_SAM_EST");
            var statusRegistroAuditoria = inserirRegistroAuditoriaIntegracao(auditoriaIntegracao);

            return respJson;
        }

        private string removeSenhaUsuario(string msgEstimuloConsulta)
        {
            string estimuloProcessado = null;

            if (!String.IsNullOrWhiteSpace(msgEstimuloConsulta))
            {
                var parsedJSON = JObject.Parse(msgEstimuloConsulta);
                parsedJSON["senha"] = "********";
                estimuloProcessado = parsedJSON.ToString();
            }

            return estimuloProcessado;
        }

        private JToken removeCamposID(JToken tokenJson)
        {
            JToken retornoTokenJson = null;

            string[] nomesCamposParaRemocao = new string[] { "UsuarioSamID", "UsuarioSamIDEstorno", "RequisitanteSamID", "RequisitanteSamIDEstorno" };
            retornoTokenJson = tokenJson.RemoveFields(nomesCamposParaRemocao, true, true);

            return retornoTokenJson;
        }

        private JToken removeCamposCpfComValorNuloOuVazio(JToken tokenJson)
        {
            JToken retornoTokenJson = null;

            string[] nomesCamposParaRemocao = new string[] { "CPF_Nome_UsuarioEstorno", "CPF_Nome_Requisitante", "CPF_Nome_RequisitanteEstorno" };
            retornoTokenJson = tokenJson.RemoveFields(nomesCamposParaRemocao, true, false);

            return retornoTokenJson;
        }

        private JToken removeCamposEspecificosRequisicaoValorNuloOuVazio(JToken tokenJson)
        {
            JToken retornoTokenJson = null;

            string[] nomesCamposParaRemocao = new string[] { "Qtde_ItemMovimentacaoMaterialSolicitada" };
            retornoTokenJson = tokenJson.RemoveFields(nomesCamposParaRemocao, true, false);

            return retornoTokenJson;
        }

        private IList<string> consisteParametrosWs_ConsultaCatalogo(ConsultaSubitemWs item)
        {
            if (!(System.Diagnostics.Debugger.IsAttached))
                if (this.ctxRequisicaoWs.IsNotNull() && this.ctxRequisicaoWs.UriTemplateMatch.Data.ToString().ToUpperInvariant() != NomeMensagem.BuscaSubitemMaterialRequisicaoParaWs.ToUpperInvariant())
                    //return new List<string>() { "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!" };
                    return new List<string>() { MensagemErro.MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA };

            IList<string> listaParametrosComErro = null;
            int _codigoAlmox = -1;
            int _codigoOrgao = -1;
            int _numeroPaginaConsulta = -1;



            listaParametrosComErro = new List<string>();
            if (String.IsNullOrWhiteSpace(item.termoParaPesquisa))
                listaParametrosComErro.Add("'Termo Para Pesquisa'");

            if (String.IsNullOrWhiteSpace(item.codigoOrgao) || !Int32.TryParse(item.codigoOrgao, out _codigoOrgao))
                listaParametrosComErro.Add("'Código Órgão'");

            if (String.IsNullOrWhiteSpace(item.codigoAlmox) || !Int32.TryParse(item.codigoAlmox, out _codigoAlmox))
                listaParametrosComErro.Add("'Código Almoxarifado'");

            if (!String.IsNullOrWhiteSpace(item.NumeroPaginaConsulta) && !Int32.TryParse(item.NumeroPaginaConsulta, out _numeroPaginaConsulta))
                listaParametrosComErro.Add("'Número Página Consulta'");


            return listaParametrosComErro;
        }

        private IList<string> consisteParametrosDatasConsultaInicialFinal(string dataConsultaInicial, string dataConsultaFinal)
        {
            IList<string> listaMsgErro = null;
            DateTime _dataConsultaInicial = DateTime.MinValue;
            DateTime _dataConsultaFinal = DateTime.MinValue;



            listaMsgErro = new List<string>();
            //DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _dataFinal);
            //DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _dataInicial);
            //if ((!String.IsNullOrWhiteSpace(mesReferenciaFinal) && mesReferenciaFinal.Length == 8) && !DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _dataFinal))
            DateTime.TryParse(dataConsultaFinal, out _dataConsultaFinal);
            DateTime.TryParse(dataConsultaInicial, out _dataConsultaInicial);
            if ((!String.IsNullOrWhiteSpace(dataConsultaFinal) && dataConsultaFinal.Length == 8) && !DateTime.TryParse(dataConsultaFinal, out _dataConsultaFinal))
            {
                if (_dataConsultaFinal == DateTime.MinValue)
                    listaMsgErro.Add("'Data Consulta Final'");
            }

            //if ((!String.IsNullOrWhiteSpace(mesReferenciaInicial) && mesReferenciaInicial.Length == 8) && !DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _dataInicial))
            if ((!String.IsNullOrWhiteSpace(dataConsultaInicial) && dataConsultaInicial.Length == 8) && !DateTime.TryParse(dataConsultaInicial, out _dataConsultaInicial))
            {
                if (_dataConsultaInicial == DateTime.MinValue)
                    listaMsgErro.Add("'Data Consulta Inicial'");
            }

            //if (((!String.IsNullOrWhiteSpace(mesReferenciaInicial) && mesReferenciaInicial.Length == 8) && DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _dataInicial)) &&
            //     ((!String.IsNullOrWhiteSpace(mesReferenciaFinal) && mesReferenciaFinal.Length == 8) && DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _dataFinal)) &&
            if (((!String.IsNullOrWhiteSpace(dataConsultaInicial) && dataConsultaInicial.Length == 8) && DateTime.TryParse(dataConsultaInicial, out _dataConsultaInicial)) &&
                 ((!String.IsNullOrWhiteSpace(dataConsultaFinal) && dataConsultaFinal.Length == 8) && DateTime.TryParse(dataConsultaFinal, out _dataConsultaFinal)) &&
                 (_dataConsultaInicial != DateTime.MinValue && _dataConsultaFinal != DateTime.MinValue))
            {
                if (_dataConsultaInicial > _dataConsultaFinal)
                    listaMsgErro.Add("'Data Consulta Inicial' e 'Data Consulta Final'");
            }

            return listaMsgErro;
        }
        private IList<string> consisteParametrosMesReferenciaInicialFinal(string mesReferenciaInicial, string mesReferenciaFinal)
        {
            IList<string> listaMsgErro = null;
            DateTime _mesReferenciaInicial = DateTime.MinValue;
            DateTime _mesReferenciaFinal = DateTime.MinValue;



            listaMsgErro = new List<string>();
            DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _mesReferenciaFinal);
            DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _mesReferenciaInicial);
            if ((!String.IsNullOrWhiteSpace(mesReferenciaFinal) && mesReferenciaFinal.Length == 6) && !DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _mesReferenciaFinal))
            {
                if (_mesReferenciaFinal == DateTime.MinValue)
                    listaMsgErro.Add("'Mês Referência Final'");
            }

            if ((!String.IsNullOrWhiteSpace(mesReferenciaInicial) && mesReferenciaInicial.Length == 6) && !DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _mesReferenciaInicial))
            {
                if (_mesReferenciaInicial == DateTime.MinValue)
                    listaMsgErro.Add("'Mês Referência Inicial'");
            }

            if (((!String.IsNullOrWhiteSpace(mesReferenciaInicial) && mesReferenciaInicial.Length == 6) && DateTime.TryParse(String.Format("01-{0}", mesReferenciaInicial.Insert(2, "-")), out _mesReferenciaInicial)) &&
                 ((!String.IsNullOrWhiteSpace(mesReferenciaFinal) && mesReferenciaFinal.Length == 6) && DateTime.TryParse(String.Format("01-{0}", mesReferenciaFinal.Insert(2, "-")), out _mesReferenciaFinal)) &&
                 (_mesReferenciaInicial != DateTime.MinValue && _mesReferenciaFinal != DateTime.MinValue))
            {
                if (_mesReferenciaInicial > _mesReferenciaFinal)
                    listaMsgErro.Add("'Mês de Referência Inicial' e 'Mês de Referência Final'");
            }

            return listaMsgErro;
        }

        private IList<string> consisteParametrosWs_PeriodoTemporalConsulta(DadosRelatorioWs parametrosRelatorio)
        {
            IList<string> listaParametrosComErro = null;
            DateTime inicioPeriodoConsulta = DateTime.MinValue;
            DateTime finalPeriodoConsulta = DateTime.MaxValue;
            //JObject parsedJSON = null;

            //parsedJSON = JObject.Parse(msgEstimuloWs);
            listaParametrosComErro = new List<string>();
            if (   ((!String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaFinal)) && ((DateTime.TryParse(parametrosRelatorio.dataConsultaInicial, out inicioPeriodoConsulta)) && (DateTime.TryParse(parametrosRelatorio.dataConsultaFinal, out finalPeriodoConsulta))))
                && ((!String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaFinal)) && ((DateTime.TryParse(String.Format("01-{0}", parametrosRelatorio.mesReferenciaInicial.Insert(2, "-")), out inicioPeriodoConsulta)) && (DateTime.TryParse(String.Format("01-{0}", parametrosRelatorio.mesReferenciaFinal.Insert(2, "-")), out finalPeriodoConsulta))))
               ) //se foram fornecidos os dois filtros de datas (MesRef e DataConsulta)
                listaParametrosComErro.Add("'Mês de Referência' e/ou 'Data de Consulta' (fornecer apenas uma das duas datas)");

            else if (((!String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaFinal)) && ((!DateTime.TryParse(parametrosRelatorio.dataConsultaInicial, out inicioPeriodoConsulta)) || (!DateTime.TryParse(parametrosRelatorio.dataConsultaFinal, out finalPeriodoConsulta))))
                            && ((!String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaFinal)) && ((!DateTime.TryParse(String.Format("01-{0}", parametrosRelatorio.mesReferenciaInicial.Insert(2, "-")), out inicioPeriodoConsulta)) && (!DateTime.TryParse(String.Format("01-{0}", parametrosRelatorio.mesReferenciaFinal.Insert(2, "-")), out finalPeriodoConsulta))))
                           ) //se foram fornecidos os dois filtros de datas (MesRef e DataConsulta)
                listaParametrosComErro.Add("'Mês de Referência' e/ou 'Data de Consulta' (valor(es) fornecido(s) incorretamente)");

            else if ((String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaInicial) && String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaFinal)) &&
                     (String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaInicial) && String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaFinal))) //se nao foi fornecido nenhum dos dois filtros de datas (MesRef e DataConsulta)
                listaParametrosComErro.Add("'Mês de Referência' e/ou 'Data de Consulta' (nenhum intervalo de consulta fornecido)");

            else if (   ((!String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaInicial) && String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaFinal)) ||
                         (String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.dataConsultaFinal))) 
                     || ((!String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaInicial) && String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaFinal)) ||
                         (String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaInicial) && !String.IsNullOrWhiteSpace(parametrosRelatorio.mesReferenciaFinal)))
                    )
                listaParametrosComErro.Add("'Mês de Referência' e/ou 'Data de Consulta' (intervalo fornecido incorretamente)");


            return listaParametrosComErro;
        }

        private IList<string> consisteParametrosWs_CredenciaisAcesso(string msgEstimuloWs)
        {
            IList<string> listaParametrosComErro = null;
            JObject parsedJSON = null;
            int codigoOrgao = -1;
            long cpfUsuario = -1;
            

            parsedJSON = JObject.Parse(msgEstimuloWs);
            listaParametrosComErro = new List<string>();
            if ((parsedJSON["cpf"].IsNull()) || ((String.IsNullOrWhiteSpace(parsedJSON["cpf"].ToString())) || (parsedJSON["cpf"].ToString().Length < 11) || (!Int64.TryParse(parsedJSON["cpf"].ToString(), out cpfUsuario))))
                listaParametrosComErro.Add("'CPF (em branco ou inválido)'");

            if ((parsedJSON["senha"].IsNull()) || (String.IsNullOrWhiteSpace(parsedJSON["senha"].ToString())))
                listaParametrosComErro.Add("Senha (em branco)");

            if ((parsedJSON["codigoOrgao"].IsNull()) || ((String.IsNullOrWhiteSpace(parsedJSON["codigoOrgao"].ToString()) || (!Int32.TryParse(parsedJSON["codigoOrgao"].ToString(), out codigoOrgao)))))
                listaParametrosComErro.Add("Código de órgão (em branco)");


            return listaParametrosComErro;
        }

        private IList<string> consisteParametrosWs_CriaRequisicaoMaterial(string msgEstimuloWs)
        {
            if (!(System.Diagnostics.Debugger.IsAttached))
                if (this.ctxRequisicaoWs.IsNotNull() && this.ctxRequisicaoWs.UriTemplateMatch.Data.ToString().ToUpperInvariant() != NomeMensagem.GeraRelacaoMovimentacaoMaterialWs.ToUpperInvariant())
                    //return new List<string>() { "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!" };
                    return new List<string>() { MensagemErro.MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA };


            DateTime _dataInicial = DateTime.MinValue;
            DateTime _dataFinal = DateTime.MinValue;
            int _codigoOrgao = -1;
            int _codigoUGE = -1;
            int _codigoUA = -1;
            int _codigoDivisaoUA = -1;
            string codigoOrgao = null;
            string codigoUge = null;
            string codigoUa = null;
            string codigoDivisaoUa = null;
            string observacoesRequisicaoMaterial = null;
            JObject parsedJSON = null;
            IList<string> listaParametrosComErro = null;
            IList<JToken> relacaoItensRequisicaoMaterial = null;



            parsedJSON = JObject.Parse(msgEstimuloWs);
            codigoOrgao = parsedJSON["codigoOrgao"].ToString();
            codigoUge = parsedJSON["codigoUge"].ToString();
            codigoUa = parsedJSON["codigoUa"].ToString();
            codigoDivisaoUa = parsedJSON["codigoDivisaoUa"].ToString();
            observacoesRequisicaoMaterial = parsedJSON["observacoes"].ToString();
            relacaoItensRequisicaoMaterial = ((parsedJSON["movimentoItem"].IsNotNull()) ? (parsedJSON["movimentoItem"].Children().ToList()) : null);
            listaParametrosComErro = new List<string>();
            if (String.IsNullOrWhiteSpace(codigoOrgao) || !Int32.TryParse(codigoOrgao, out _codigoOrgao))
                listaParametrosComErro.Add("'Código Órgão'");

            if (String.IsNullOrWhiteSpace(codigoUge) || !Int32.TryParse(codigoUge, out _codigoUGE))
                listaParametrosComErro.Add("'Código UGE'");

            if (String.IsNullOrWhiteSpace(codigoUa) || !Int32.TryParse(codigoUa, out _codigoUA))
                listaParametrosComErro.Add("'Código UA'");

            if (String.IsNullOrWhiteSpace(codigoDivisaoUa) || !Int32.TryParse(codigoDivisaoUa, out _codigoDivisaoUA))
                listaParametrosComErro.Add("'Código Divisão UA'");

            if (!String.IsNullOrWhiteSpace(observacoesRequisicaoMaterial) && (observacoesRequisicaoMaterial.Length > 154))
                listaParametrosComErro.Add("'Campo Observações'");

            if (!String.IsNullOrWhiteSpace(msgEstimuloWs) && (relacaoItensRequisicaoMaterial.HasElements() && ((relacaoItensRequisicaoMaterial.Count() == 0) || (relacaoItensRequisicaoMaterial.Count() > cstConstante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO))))
                listaParametrosComErro.Add("'Relação Subitens Requisição' (verificar qtde. informada)");


            consisteParametrosWs_CredenciaisAcesso(msgEstimuloWs).ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));

            return listaParametrosComErro;
        }

        private IList<string> consisteParametrosWs_RelatorioMovimentacao<T>(DadosRelatorioWs parametrosRelatorio) where T : dtoBaseWs
        {
            if (!(System.Diagnostics.Debugger.IsAttached))
                if (this.ctxRequisicaoWs.IsNotNull() && this.ctxRequisicaoWs.UriTemplateMatch.Data.ToString().ToUpperInvariant() != NomeMensagem.GeraRelacaoMovimentacaoMaterialWs.ToUpperInvariant())
                    //return new List<string>() { "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!" };
                    return new List<string>() { MensagemErro.MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA };


            IList<string> listaParametrosComErro = null;
            DateTime _dataInicial = DateTime.MinValue;
            DateTime _dataFinal = DateTime.MinValue;
            int _codigoOrgao = -1;
            int _codigoUGE = -1;
            int _codigoAlmox = -1;
            int _numeroPaginaConsulta = -1;



            listaParametrosComErro = new List<string>();
            if (!String.IsNullOrWhiteSpace(parametrosRelatorio.NumeroPaginaConsulta) && !Int32.TryParse(parametrosRelatorio.NumeroPaginaConsulta, out _numeroPaginaConsulta))
                listaParametrosComErro.Add("'Número Página Consulta'");

            if (String.IsNullOrWhiteSpace(parametrosRelatorio.codigoOrgao) || !Int32.TryParse(parametrosRelatorio.codigoOrgao, out _codigoOrgao))
                listaParametrosComErro.Add("'Código Órgão'");


            consisteParametrosCodigoUGE_CodigoAlmoxarifado(parametrosRelatorio).ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));

            consisteParametrosTipoMovimento_TipoAgrupamentoMovimento(parametrosRelatorio).ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));
            //consisteParametrosDatasInicialFinal(parametrosRelatorio.dataInicial, parametrosRelatorio.dataFinal).ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));
            consisteParametrosWs_PeriodoTemporalConsulta(parametrosRelatorio).ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));

            return listaParametrosComErro;
        }

        private IList<string> consisteParametrosCodigoUGE_CodigoAlmoxarifado(DadosRelatorioWs dadosRelatorio)
        {
            if (!(System.Diagnostics.Debugger.IsAttached))
                if (this.ctxRequisicaoWs.IsNotNull() && this.ctxRequisicaoWs.UriTemplateMatch.Data.ToString().ToUpperInvariant() != NomeMensagem.GeraRelacaoMovimentacaoMaterialWs.ToUpperInvariant())
                    //return new List<string>() { "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!" };
                    return new List<string>() { MensagemErro.MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA };


            IList<string> listaParametrosComErro = null;
            int _codigoUge = -1;
            int _codigoAlmox = -1;


            listaParametrosComErro = new List<string>();
            //VALIDACAO BASICA - PARAMETROS 'CODIGO UGE' / 'CODIGO ALMOXARIFADO'
            if (((String.IsNullOrWhiteSpace(dadosRelatorio.codigoUge) && (String.IsNullOrWhiteSpace(dadosRelatorio.codigoAlmox)))) || //se tipo e agrupamento forem nulos
                (((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoUge) && Int32.TryParse(dadosRelatorio.codigoUge, out _codigoUge)) && //se for fornecido valores validos aos dois parametros
                  (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAlmox) && Int32.TryParse(dadosRelatorio.codigoAlmox, out _codigoAlmox)))))
                listaParametrosComErro.Add("'Código de UGE' e/ou 'Código de Almoxarifado' (fornecer apenas um dos dois)");
            else if ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoUge) && !Int32.TryParse(dadosRelatorio.codigoUge, out _codigoUge)) || //se tipo e agrupamento forem nulos ou for fornecido valor invalido aos dois parametros
                     (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAlmox) && !Int32.TryParse(dadosRelatorio.codigoAlmox, out _codigoAlmox)))
                listaParametrosComErro.Add("'Código de UGE' e/ou 'Código de Almoxarifado' (fornecido(s) incorretamente)");

            return listaParametrosComErro;
        }

        private const int codigoTipoMovimentoRequisicaoPendente = 20;
        private const int codigoTipoMovimentoRequisicaoFinalizada = 25;
        private const int codigoTipoMovimentoRequisicaoAprovada = 21;
        private const int codigoTipoMovimentoRequisicaoCancelada = 16;

        private IList<string> consisteParametrosTipoMovimento_TipoAgrupamentoMovimento(DadosRelatorioWs dadosRelatorio)
        {
            if (!(System.Diagnostics.Debugger.IsAttached))
                if (this.ctxRequisicaoWs.IsNotNull() && this.ctxRequisicaoWs.UriTemplateMatch.Data.ToString().ToUpperInvariant() != NomeMensagem.GeraRelacaoMovimentacaoMaterialWs.ToUpperInvariant())
                    //return new List<string>() { "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!" };
                    return new List<string>() { MensagemErro.MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA };


            IList<string> listaParametrosComErro = null;
            int _codigoTipoMovimentacaoMaterial = -1;
            int _codigoAgrupamentoTipoMovimentacaoMaterial = -1;
            long cpfCnpjFornecedor = -1;
            //bool _consultaTransferencia = false;
            int _codigoUA = -1;
            int _codigoDivisaoUA = -1;
            int[] tiposMovimentacaoMaterialConsultaPermitida = null;
            int[] tiposMovimentacaoRequisicaoMaterialConsultaNaoPermitida = null;
            int[] tiposAgrupamentoMovimentacaoMaterialConsultaPermitida = null;



            listaParametrosComErro = new List<string>();
            tiposMovimentacaoMaterialConsultaPermitida = new int[] {
                                                                        //ENTRADAS
                                                                        10, //Entrada por Empenho
                                                                        13, //Empenho de Serv. Volta Mat. Consumo
                                                                        14, //Entrada por Transferência
                                                                        15, //Doação por Orgão Implantado
                                                                        16, //Entrada por Devolução
                                                                        17, //Material Transformado
                                                                        37, //Transferência de almoxarifado não implantado
                                                                        38, //Restos a Pagar
                                                                        39, //Entrada por Inventário
                                                                        40, //Doação
                                                                        41, //Consumo Imediato
                                                                        42, // Restos a Pagar de Empenho de Consumo Imediato
                                                                        //SAIDAS
                                                                        22, //Saída por Transferência
                                                                        23, //Saída por Doação
                                                                        24, //Outras Saídas
                                                                        30, //Transformação
                                                                        31, //Extravio / Furto / Roubo
                                                                        32, //Incorporação Indevida
                                                                        33, //Perda
                                                                        34, //Inservivel / Quebra
                                                                        35, //Transferência para almoxarifado não implantado
                                                                        36,  //Amostra / Exposição / Análise
                                                                        //REQUISICAO -- VAI RETORNAR ESTE TIPO DE SAIDA
                                                                        25 //Requisição Finalizada
                                                                    };

            tiposAgrupamentoMovimentacaoMaterialConsultaPermitida = new int[] {
                                                                                1, //Entrada
                                                                                2, //Saida
                                                                                3, //Requisição
                                                                                4, //ConsumoImediato

                                                                                9, //Todos os tipos de agrupamento
                                                                              };

            tiposMovimentacaoRequisicaoMaterialConsultaNaoPermitida = new int[] {
                                                                                    20, //Requisicao Pendente
                                                                                    //25, //Requisicao Finalizada
                                                                                    21, //Requisicao Aprovada
                                                                                    16  //Requisicao Cancelada
                                                                                };

            //VALIDACAO BASICA - PARAMETROS 'CODIGO TIPO MOVIMENTACAO' / 'CODIGO AGRUPAMENTO TIPO MOVIMENTACAO'
            if (((String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && (String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial)))) || //se tipo e agrupamento forem nulos
                (((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && Int32.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out _codigoAgrupamentoTipoMovimentacaoMaterial)) && //se for fornecido valores validos aos dois parametros
                  (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial)))))
                listaParametrosComErro.Add("'Tipo de agrupamento' e/ou 'Tipo de movimentação' (fornecer apenas um dos dois)");
            else if ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out _codigoAgrupamentoTipoMovimentacaoMaterial)) || //se tipo e agrupamento forem nulos ou for fornecido valor invalido aos dois parametros
                     (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial)))
                listaParametrosComErro.Add("'Tipo de agrupamento' e/ou 'Tipo de movimentação' (fornecido(s) incorretamente)");


            {
                //Se for solicitado tipo generico de movimentacao de material, inves de tipo especifico
                //Se for solicitado tipo especifico
                //{
                //Se relacao consultada for de saida por requisicao;
                //if (tipoMovimentacaoMaterialCodigo == TipoMovimento.RequisicaoAprovada.GetHashCode())
                //{
                //Se DivisaoUA foi especificada
                //Se DivisaoUA nao foi especificada
                //}
                //Se relacao consultada for de outros tipos (entrada/saida);
                //Se fornecedor (CPF/CNPJ) for especificado


                //Se for solicitado tipo generico de movimentacao de material, inves de tipo especifico
                if (((String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) || !Int32.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out _codigoAgrupamentoTipoMovimentacaoMaterial)) && //se tipo e agrupamento forem nulos ou for fornecido valor invalido aos dois parametros
                     (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial))) &&
                     (!listaParametrosComErro.Contains("'Tipo de agrupamento' e/ou 'Tipo de movimentação' (fornecer apenas um dos dois)")))
                    listaParametrosComErro.Add("'Tipo de agrupamento' obrigatório");
                //Se for solicitado tipo especifico
                else if ((String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && //se agrupamento for nulo
                         (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial))) //e tipo especifico OK
                         && (!listaParametrosComErro.Contains("'Tipo de agrupamento' e/ou 'Tipo de movimentação' (fornecer apenas um dos dois)") || !listaParametrosComErro.Contains("'Tipo de agrupamento' e/ou 'Tipo de movimentação' (fornecido incorretamente)")))
                    listaParametrosComErro.Add("'Tipo de movimentação' (obrigatório)");

                /******/
                else if ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && Int32.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out _codigoAgrupamentoTipoMovimentacaoMaterial)) && //se tipo agrupamento for valido e for fornecido valor invalido para tipo movimentacao
                         (String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial)) &&
                         (!tiposAgrupamentoMovimentacaoMaterialConsultaPermitida.Contains(_codigoAgrupamentoTipoMovimentacaoMaterial))) //codigo tipo agrupamento, consulta permitida
                    listaParametrosComErro.Add("'Tipo de agrupamento' (fornecido(s) incorretamente)");
                /******/

                //else if ((String.IsNullOrWhiteSpace(dadosRelatorio.agrupamentoTipoMovimentacaoMaterialCodigo) && //se agrupamento for nulo
                if ((String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && //se agrupamento for nulo
                         (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && !Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial))) && //e tipo especifico OK
                         //((_codigoTipoMovimentacaoMaterial == TipoMovimento.RequisicaoCancelada.GetHashCode() || _codigoTipoMovimentacaoMaterial == TipoMovimento.RequisicaoPendente.GetHashCode()))) //e informado codigo tipo requisicao incorreto
                         //((_codigoTipoMovimentacaoMaterial == codigoTipoMovimentoRequisicaoCancelada || _codigoTipoMovimentacaoMaterial == codigoTipoMovimentoRequisicaoPendente))) //e informado codigo tipo requisicao incorreto
                         (tiposMovimentacaoRequisicaoMaterialConsultaNaoPermitida.Contains(_codigoTipoMovimentacaoMaterial))) //e informado codigo tipo requisicao incorreto
                    listaParametrosComErro.Add("'Tipo de movimentação' (Código Requisição Material Inválido)");
                /******/
                else if ((String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && //se agrupamento for nulo
                         (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoTipoMovimentacaoMaterial) && Int32.TryParse(dadosRelatorio.codigoTipoMovimentacaoMaterial, out _codigoTipoMovimentacaoMaterial))) && //e tipo especifico OK
                         (!tiposMovimentacaoMaterialConsultaPermitida.Contains(_codigoTipoMovimentacaoMaterial))) //e informado codigo tipo movimentacao nao permitido consultar
                    listaParametrosComErro.Add("'Tipo de movimentação' (Código Movimentacao Material Inválido)");
                /******/
                else if (((String.IsNullOrWhiteSpace(dadosRelatorio.codigoUa) || ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoUa) && !Int32.TryParse(dadosRelatorio.codigoUa, out _codigoUA)) || _codigoUA <= 0)) && //codigoUA for nulo ou invalido
                        (!String.IsNullOrWhiteSpace(dadosRelatorio.codigoDivisaoUa) && Int32.TryParse(dadosRelatorio.codigoDivisaoUa, out _codigoDivisaoUA))) &&  //e codigoDivisaoUA OK
                        (tiposMovimentacaoRequisicaoMaterialConsultaNaoPermitida.Contains(Int32.Parse(dadosRelatorio.codigoTipoMovimentacaoMaterial)))                        ) 
                    listaParametrosComErro.Add("'Códigos UA e 'Código Divisão' (obrigatório informar código UA)");
                else if ((!String.IsNullOrWhiteSpace(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial) && (Int32.TryParse(dadosRelatorio.codigoAgrupamentoTipoMovimentacaoMaterial, out _codigoAgrupamentoTipoMovimentacaoMaterial))) && //se agrupamento nao for nulo
                         //(_codigoAgrupamentoTipoMovimentacaoMaterial == TipoMovimentoAgrupamento.Entrada.GetHashCode() || _codigoAgrupamentoTipoMovimentacaoMaterial == TipoMovimentoAgrupamento.Saida.GetHashCode())) //e nao for requisicao e CPF/CNPJ for informado
                         (tiposAgrupamentoMovimentacaoMaterialConsultaPermitida.Contains(_codigoAgrupamentoTipoMovimentacaoMaterial))) //e se for tipo de agrupamento movimentacao valido
                {
                    if (_codigoAgrupamentoTipoMovimentacaoMaterial == TipoMovimentoAgrupamento.Requisicao.GetHashCode()) //e se for requisicao e CPF/CNPJ for informado
                        if (!String.IsNullOrWhiteSpace(dadosRelatorio.cpfCnpjFornecedor) && !Int64.TryParse(dadosRelatorio.cpfCnpjFornecedor, out cpfCnpjFornecedor))
                            listaParametrosComErro.Add("'CPF/CNPJ de Fornecedor inválido'");
                }
            }

            return listaParametrosComErro;
        }

        private bool consisteCredenciaisWs(string cpf, string senha, string codigoOrgao)
        {
            IList<string> listaParametrosComErro = null;

            listaParametrosComErro = new List<string>();
            if (String.IsNullOrWhiteSpace(cpf) || cpf.ToString().Length < 11)
                listaParametrosComErro.Add("'Informado CPF em branco ou inválido'");

            if (String.IsNullOrWhiteSpace(senha))
                listaParametrosComErro.Add("Informada senha em branco!");

            if (String.IsNullOrWhiteSpace(codigoOrgao))
                listaParametrosComErro.Add("Informado código de órgão em branco");

            return !listaParametrosComErro.HasElements();
        }

        /// <summary>
        /// Autenticar usuário Carregando o Perfil
        /// </summary>
        /// <param name="login"></param>
        /// <param name="senha"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool ValidarLoginUsuarioWs(string cpf, string senha, string codigoOrgao)
        {
            bool possuiPerfilConsultaWS = false;
            string senhaCriptografada = Sam.Facade.FacadeLogin.CriptografarSenha(senha);

            //Valida usuário e verifica seu perfil
            possuiPerfilConsultaWS = new PerfilBusiness().ValidarPerfilConsultaWs(cpf, senhaCriptografada, codigoOrgao);

            return possuiPerfilConsultaWS;
        }

        public string BuscaSubItemMaterialRequisicaoParaWsOptions(ConsultaSubitemWs item)
        {
            return "";
        }

        public string GeraRelacaoMovimentacaoMaterialWsOptions(DadosRelatorioWs dadosRelatorio)
        {
            return "";
        }

        public string CriaRequisicaoMaterialWsOptions(DadosMovimentacaoWs dadosMovimentacaoWs)
        {
            return "";
        }

        public string CriaRequisicaoMaterialWs(DadosMovimentacaoWs dadosMovimentacaoWs)
        {
            string respJson = null;
            string msgEstimuloConsulta = null;
            //string fmtMsgErro = null;
            string fmtMsgRetornoCriacaoRequisicaoMaterial;
            dtoWsRetornoConsulta dtoRetornoConsulta = null;
            MovimentoEntity requisicaoMaterial = null;
            IList<string> dadosMsgInfoCriacaoRequisicaoMaterial = null;
            IList<string> listaParametrosComErro = null;


            msgEstimuloConsulta = obterEstimuloJson();
            dtoRetornoConsulta = new dtoWsRetornoConsulta() { MensagemErro = "" };
            listaParametrosComErro = consisteParametrosWs_CriaRequisicaoMaterial(msgEstimuloConsulta);
            if (!listaParametrosComErro.HasElements())
            {
                var processamentoWsRequisicao = ConsultasWsExtensions.ProcessaEstimuloRequisicaoMaterial(msgEstimuloConsulta);
                dadosMsgInfoCriacaoRequisicaoMaterial = processamentoWsRequisicao.Item1.ToList().ToArray();
                requisicaoMaterial = processamentoWsRequisicao.Item2;


                if (dadosMsgInfoCriacaoRequisicaoMaterial.HasElements())
                    dadosMsgInfoCriacaoRequisicaoMaterial.ToList().ForEach(msgErro => listaParametrosComErro.Add(msgErro));
            }


            if (listaParametrosComErro.HasElements())
            {
                //fmtMsgErro = "Parâmetro(s) {0} informado(s) incorretamente. Favor verificar dado(s) informado(s).";
                //dtoRetornoConsulta.MensagemErro = String.Format(fmtMsgErro, String.Join(", ", listaParametrosComErro));
                dtoRetornoConsulta.MensagemErro = String.Format(MensagemErro.FMT_MSGERRO__PARAMETRO_INFORMADO_INCORRETAMENTE, String.Join(", ", listaParametrosComErro));
            }
            else
            {
                if (consisteCredenciaisWs(dadosMovimentacaoWs.cpf, dadosMovimentacaoWs.senha, dadosMovimentacaoWs.codigoOrgao))
                {
                    if (ValidarLoginUsuarioWs(dadosMovimentacaoWs.cpf, dadosMovimentacaoWs.senha, dadosMovimentacaoWs.codigoOrgao))
                    {
                        oldBusiness.MovimentoBusiness objBusiness = new oldBusiness.MovimentoBusiness() { Movimento = requisicaoMaterial }; ;
                        bool reqGravadaOK = objBusiness.SalvarRequisicaoWs();

                        if (reqGravadaOK)
                        {
                            dadosMsgInfoCriacaoRequisicaoMaterial = new string[] { objBusiness.Movimento.NumeroDocumento };
                            fmtMsgRetornoCriacaoRequisicaoMaterial = "REQUISICAO {0} SALVA COM SUCESSO!";

                            dtoRetornoConsulta.DadosConsulta = new string[] { String.Format(fmtMsgRetornoCriacaoRequisicaoMaterial, String.Join("", dadosMsgInfoCriacaoRequisicaoMaterial)) };
                        }
                        else
                        {
                            //var msgErro = "ERRO AO REGISTRAR MOVIMENTACAO!";
                            var msgErro = MensagemErro.MSG_ERRO__REGISTRO_MOVIMENTACAO;

                            //fmtMsgErro = "Parâmetro(s) {0} informado(s) incorretamente. Favor verificar dado(s) informado(s).";
                            //dtoRetornoConsulta.MensagemErro = String.Format(fmtMsgErro, String.Join(", ", listaParametrosComErro));
                            dtoRetornoConsulta.MensagemErro = String.Format(MensagemErro.FMT_MSGERRO__PARAMETRO_INFORMADO_INCORRETAMENTE, String.Join(", ", listaParametrosComErro));
                        }
                    }
                    else
                    {
                        //dtoRetornoConsulta.MensagemErro = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
                        dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS;
                    }
                }
                else
                {
                    //dtoRetornoConsulta.MensagemErro = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
                    dtoRetornoConsulta.MensagemErro = MensagemErro.MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS;
                }
            }


            respJson = JsonConvert.SerializeObject(dtoRetornoConsulta, Formatting.Indented, new JsonSerializerSettings() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();
            JToken tokenizer = JToken.Parse(respJson);
            respJson = tokenizer.ToString(Formatting.Indented);

            return respJson;
        }

        private bool inserirRegistroAuditoriaIntegracao(AuditoriaIntegracaoEntity registroAuditoria)
        {
            bool blnRetorno = false;
            AuditoriaIntegracaoBusiness objBusiness = null;

            objBusiness = new AuditoriaIntegracaoBusiness();
            blnRetorno = objBusiness.InserirRegistro(registroAuditoria);

            //if (objBusiness.ListaErro.HasElements())
            //    this.ListaErro.AddRange(objBusiness.ListaErro);
            //else
            //    this.ListaErro = objBusiness.ListaErro;

            return blnRetorno;
        }

        private AuditoriaIntegracaoEntity gerarRegistroAuditoria(string loginUsuarioSAM, string loginSiafemUsuario, string msgEstimuloWs, string retornoMsgWs, DateTime dataHoraEnvioMsgWs, DateTime dataHoraRetornoMsgWs, string nomeSistemaIntegracao = "ConsultaWs")
        {
            AuditoriaIntegracaoEntity registroAuditoria = null;

            registroAuditoria = new AuditoriaIntegracaoEntity();
            registroAuditoria.MsgEstimuloWS = msgEstimuloWs;
            registroAuditoria.MsgRetornoWS = retornoMsgWs;
            registroAuditoria.NomeSistema = nomeSistemaIntegracao;
            registroAuditoria.DataEnvio = dataHoraEnvioMsgWs;
            registroAuditoria.DataRetorno = dataHoraRetornoMsgWs;
            registroAuditoria.UsuarioSAM = loginUsuarioSAM;
            registroAuditoria.UsuarioSistemaExterno = loginSiafemUsuario;

            return registroAuditoria;
        }

        static List<string> ServiceMap = new List<string>
        {
            "Sam.Integracao.ConsultaWs.FCasa",
            "ConsultaWs.FCasa",
            "ConsultaWs",
            "FCasa"
        };

        public void Init(HttpApplication app)
        {
            app.BeginRequest += delegate
            {
                HttpContext ctx = HttpContext.Current;
                string path = ctx.Request.AppRelativeCurrentExecutionFilePath.ToLower();

                foreach (string mapPath in ServiceMap)
                {
                    if (path.Contains("/" + mapPath + "/") || path.EndsWith("/" + mapPath))
                    {
                        string newPath = path.Replace("/" + mapPath + "/", "/" + mapPath + ".svc/");
                        ctx.RewritePath(newPath, null, ctx.Request.QueryString.ToString(), false);
                        return;
                    }
                }
            };

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public static partial class ConsultasWsExtensions
    {
        private static MovimentoEntity criaRequisicaoMaterialFromJSON(this string estimuloJSON)
        {
            if (String.IsNullOrWhiteSpace(estimuloJSON))
                return null;


            MovimentoEntity requisicaoMaterial = null;
            MovimentoItemEntity itemRequisicaoMaterial = null;
            IList<MovimentoItemEntity> itensRequisicaoMaterial = null;
            IList<JToken> parsedItensRequisicaoJSON = null;
            JObject parsedJSON = null;
            int codigoUGE = -1;
            int codigoUA = -1;
            int codigoDivisaoUA = -1;
            int codigoOrgao = -1;
            long codigoSubitemMaterial = -1;
            int codigoPTRes = -1;
            decimal qtdeRequisitada = -1.00m;
            string cpfUsuarioSAM = null;
            string observacoesRequisicaoMaterial = null;





            parsedJSON = JObject.Parse(estimuloJSON);
            Int32.TryParse(parsedJSON["codigoOrgao"].ToString(), out codigoOrgao);
            Int32.TryParse(parsedJSON["codigoUge"].ToString(), out codigoUGE);
            Int32.TryParse(parsedJSON["codigoUa"].ToString(), out codigoUA);
            Int32.TryParse(parsedJSON["codigoDivisaoUa"].ToString(), out codigoDivisaoUA);
            cpfUsuarioSAM = parsedJSON["cpf"].ToString();
            observacoesRequisicaoMaterial = parsedJSON["observacoes"].ToString();


            requisicaoMaterial = new MovimentoEntity()
                                                        {
                                                            TipoMovimento = new TipoMovimentoEntity() { Id = TipoMovimento.RequisicaoPendente.GetHashCode() },
                                                            DataDocumento = DateTime.Now,
                                                            DataMovimento = DateTime.Now,
                                                            DataOperacao = DateTime.Now,
                                                            ValorDocumento = 0.00m
                                                        };
            requisicaoMaterial.Ativo = true;
            requisicaoMaterial.IdLogin = ObterLoginUsuarioID_PorCPF(cpfUsuarioSAM);
            var ugeRequisitante = ObterRegistro<UGEEntity>(codigoUGE);
            requisicaoMaterial.Divisao = ObterDivisaoUA(codigoUA, codigoDivisaoUA);
            requisicaoMaterial.Almoxarifado = requisicaoMaterial.Divisao.Almoxarifado;
            requisicaoMaterial.UGE = requisicaoMaterial.Divisao.Almoxarifado.Uge;
            requisicaoMaterial.UGE.CodigoDescricao = ugeRequisitante.Codigo.ToString(); //Campo utilizado pelo gerador de numero de documento (codigo da UGE requisitante);
            requisicaoMaterial.AnoMesReferencia = requisicaoMaterial.Divisao.Almoxarifado.MesRef;
            requisicaoMaterial.GeradorDescricao = String.Format("{0} - {1}", requisicaoMaterial.Divisao.Codigo, requisicaoMaterial.Divisao.Descricao);
            requisicaoMaterial.Observacoes = observacoesRequisicaoMaterial;
            requisicaoMaterial.DataDocumento = DateTime.Now;
            requisicaoMaterial.DataMovimento = DateTime.Now;

            requisicaoMaterial.FonteRecurso = "";

            parsedItensRequisicaoJSON = parsedJSON["movimentoItem"].Children().ToList();
            itensRequisicaoMaterial = new List<MovimentoItemEntity>();
            foreach (var parsedItemRequisicaoJSON in parsedItensRequisicaoJSON)
            {
                itemRequisicaoMaterial = new MovimentoItemEntity();

                Int64.TryParse(parsedItemRequisicaoJSON["codigoSubitemMaterial"].ToString(), out codigoSubitemMaterial);
                Int32.TryParse(parsedItemRequisicaoJSON["codigoPTRes"].ToString(), out codigoPTRes);
                Decimal.TryParse(parsedItemRequisicaoJSON["qtdeMaterialRequisitada"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out qtdeRequisitada);


                itemRequisicaoMaterial.Ativo = true;
                itemRequisicaoMaterial.UGE = requisicaoMaterial.UGE;
                itemRequisicaoMaterial.PTRes = ObterPTRes(codigoPTRes, codigoUGE);
                itemRequisicaoMaterial.SubItemMaterial = ObterSubitemMaterial(codigoOrgao, requisicaoMaterial.Almoxarifado.Codigo.Value, codigoSubitemMaterial);
                itemRequisicaoMaterial.QtdeLiq = qtdeRequisitada;


                itensRequisicaoMaterial.Add(itemRequisicaoMaterial);
                itemRequisicaoMaterial = null;
            }


            requisicaoMaterial.MovimentoItem = itensRequisicaoMaterial;
            return requisicaoMaterial;
        }

        public static int ObterLoginUsuarioID_PorCPF(string cpfUsuarioSAM)
        {
            int loginID_UsuarioSAM = -1;

            var objInfra = new PerfilInfrastructureAntigo();
            loginID_UsuarioSAM = objInfra.ObterLoginUsuarioID_UsuarioAtivo(cpfUsuarioSAM);

            return loginID_UsuarioSAM;
        }
        public static string ObterCPFNomeUsuarioSAM(int loginUsuarioSamID)
        {
            string cpfNomeUsuarioSAM = null;

            var objBusiness = new LoginBusiness();
            cpfNomeUsuarioSAM = objBusiness.ObterCPFNomeUsuarioSAM(loginUsuarioSamID);

            
            return cpfNomeUsuarioSAM;
        }
        public static DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA)
        {
            DivisaoEntity divisaoUA = null;
            oldBusiness.EstruturaOrganizacionalBusiness objBusiness = null;



            objBusiness = new oldBusiness.EstruturaOrganizacionalBusiness();
            divisaoUA = objBusiness.ObterDivisaoUA(codigoUA, codigoDivisaoUA);
            return divisaoUA;
        }
        public static AlmoxarifadoEntity ObterAlmoxarifadoUGE(int codigoUGE, int codigoAlmoxarifado)
        {
            AlmoxarifadoEntity almoxUGE = null;
            oldBusiness.EstruturaOrganizacionalBusiness objBusiness = null;



            objBusiness = new oldBusiness.EstruturaOrganizacionalBusiness();
            almoxUGE = objBusiness.ObterAlmoxarifadoUGE(codigoUGE, codigoAlmoxarifado);
            return almoxUGE;
        }
        public static SubItemMaterialEntity ObterSubitemMaterial(int codigoOrgao, int codigoAlmox, long codigoSubitemMaterial)
        {
            IList<dtoWsSubitemMaterial> relacaoResultados = null;
            SubItemMaterialEntity subitemMaterialRequisitado = null;
            SubItemMaterialBusiness objBusiness = null;

            objBusiness = new SubItemMaterialBusiness();
            relacaoResultados = objBusiness.BuscaSubItemMaterialRequisicaoParaWs(codigoSubitemMaterial.ToString(), codigoOrgao, codigoAlmox);

            if (relacaoResultados.Count() == 1)
                subitemMaterialRequisitado = ObterRegistro<SubItemMaterialEntity>(codigoSubitemMaterial, true);
            else
                subitemMaterialRequisitado = null;


            return subitemMaterialRequisitado;
        }
        public static PTResEntity ObterPTRes(int codigoPTRes, int codigoUge)
        {
            PTResEntity ptresItemMovimentacao = null;
            oldBusiness.MovimentoBusiness objBusiness = null;



            objBusiness = new oldBusiness.MovimentoBusiness();
            ptresItemMovimentacao = objBusiness.ObterPTRes(codigoPTRes, codigoUge);
            return ptresItemMovimentacao;
        }


        public static T ObterRegistro<T>(int codigoRegistroTabela, bool obterViaCodigo = true) where T : BaseEntity
        {
            T objEntidade = null;

            var objInfra = new oldInfra.AlmoxarifadoInfraestructure();
            objEntidade = ((oldInfra.BaseInfraestructure)objInfra).ObterEntidade<T>(codigoRegistroTabela, obterViaCodigo);


            return objEntidade;
        }
        public static T ObterRegistro<T>(long codigoRegistroTabela, bool obterViaCodigo = true) where T : BaseEntity
        {
            T objEntidade = null;

            var objInfra = new oldInfra.AlmoxarifadoInfraestructure();
            objEntidade = ((oldInfra.BaseInfraestructure)objInfra).ObterEntidade<T>(codigoRegistroTabela, obterViaCodigo);


            return objEntidade;
        }

        public static Tuple<IList<string>, MovimentoEntity> ProcessaEstimuloRequisicaoMaterial(string estimuloJSON)
        {
            List<string> relacaoInconsistencias = null;
            MovimentoEntity requisicaoMaterial = null;

            try
            {
                var parsedJSON = JObject.Parse(estimuloJSON);
                var relacaoItensRequisicaoMaterial = parsedJSON["movimentoItem"].Children().ToList();



                relacaoInconsistencias = new List<string>();
                {
                    //01. Deve validar se a UGE pertence ao Orgão
                    {
                        int codigoOrgao = -1;
                        int codigoUGE = -1;


                        Int32.TryParse(parsedJSON["codigoOrgao"].ToString(), out codigoOrgao);
                        Int32.TryParse(parsedJSON["codigoUge"].ToString(), out codigoUGE);
                        var dadosOrgao = ObterRegistro<OrgaoEntity>(codigoOrgao);
                        var dadosUGE = ObterRegistro<UGEEntity>(codigoUGE);
                        if (dadosUGE.Orgao.Codigo != dadosOrgao.Codigo)
                            //relacaoInconsistencias.Add(String.Format("'Código Órgão' UGE INFORMADA ({0}) NAO PERTENCENTE AO ORGAO {1})", dadosUGE.Codigo.Value.ToString("D6"), dadosOrgao.Codigo.Value.ToString("D6")));
                            relacaoInconsistencias.Add(String.Format(MensagemErro.FMT_MSGERRO__UGE_INFORMADA_NAO_PERTENCE_A_ORGAO, dadosUGE.Codigo.Value.ToString("D6"), dadosOrgao.Codigo.Value.ToString("D6")));
                    }
                    //02. Deve validar se a Divisão pertence aquela UA
                    {
                        int codigoUA = -1;
                        int codigoDivisaoUA = -1;


                        Int32.TryParse(parsedJSON["codigoUa"].ToString(), out codigoUA);
                        Int32.TryParse(parsedJSON["codigoDivisaoUa"].ToString(), out codigoDivisaoUA);
                        var dadosUA = ObterRegistro<UAEntity>(codigoUA);
                        var dadosDivisaoUA = ObterDivisaoUA(codigoUA, codigoDivisaoUA);
                        if (dadosUA.IsNotNull() && dadosDivisaoUA.IsNull())
                            //relacaoInconsistencias.Add(String.Format("'Código Divisao' (DIVISAO INFORMADA ({0}) NAO PERTENCENTE A UA {1})", codigoDivisaoUA.ToString("D3"), dadosUA.Codigo.Value.ToString("D8")));
                            relacaoInconsistencias.Add(String.Format(MensagemErro.FMT_MSGERRO__DIVISAO_INFORMADA_NAO_PERTENCE_A_UA, codigoDivisaoUA.ToString("D3"), dadosUA.Codigo.Value.ToString("D8")));
                    }
                    //03. Deve validar se a UA pertence a UGE
                    {
                        int codigoUA = -1;
                        int codigoUGE = -1;
                        int codigoDivisaoUA = -1;


                        Int32.TryParse(parsedJSON["codigoUa"].ToString(), out codigoUA);
                        Int32.TryParse(parsedJSON["codigoUge"].ToString(), out codigoUGE);
                        Int32.TryParse(parsedJSON["codigoDivisaoUa"].ToString(), out codigoDivisaoUA);
                        var dadosUGE = ObterRegistro<UGEEntity>(codigoUGE);
                        var dadosUA = ObterRegistro<UAEntity>(codigoUA);
                        var dadosDivisaoUA = ObterDivisaoUA(codigoUA, codigoDivisaoUA);

                        if ((dadosUGE.IsNotNull() && dadosUA.IsNotNull()) && (dadosUA.Uge.Codigo != dadosUGE.Codigo))
                            //relacaoInconsistencias.Add(String.Format("'Código UA' (UA INFORMADA ({0}) NAO PERTENCENTE A UGE {1})", dadosUA.Codigo.Value.ToString("D8"), dadosUGE.Codigo.Value.ToString("D6")));
                            relacaoInconsistencias.Add(String.Format(MensagemErro.FMT_MSGERRO__UA_INFORMADA_NAO_PERTENCE_A_UGE, dadosUA.Codigo.Value.ToString("D8"), dadosUGE.Codigo.Value.ToString("D6")));
                        //else if ((dadosUGE.IsNotNull() && dadosDivisaoUA.IsNotNull()) && (dadosDivisaoUA.Almoxarifado.Uge.Codigo != dadosUGE.Codigo))
                        //    relacaoInconsistencias.Add(String.Format("'Código Almoxarifado' (ALMOXARIFADO INFORMADO ({0}) NAO RELACIONADO A UGE {1})", dadosDivisaoUA.Almoxarifado.Codigo.Value.ToString("D3"), dadosUGE.Codigo.Value.ToString("D6")));
                    }
                }

                if (!relacaoInconsistencias.HasElements())
                    requisicaoMaterial = estimuloJSON.criaRequisicaoMaterialFromJSON();

                if (requisicaoMaterial.IsNotNull() && !relacaoInconsistencias.HasElements() && relacaoItensRequisicaoMaterial.HasElements())
                {
                    //consiste qtde. subitens na movimentacao
                    if (relacaoItensRequisicaoMaterial.Count() > cstConstante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO)
                    {
                        //relacaoInconsistencias.Add(String.Format("NUMERO MAXIMO DE SUBITENS POR MOVIMENTACAO ({0}), EXCEDIDO!", cstConstante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                        relacaoInconsistencias.Add(String.Format(MensagemErro.FMT_MSGERRO__SUBITEM_NUMERO_MAXIMO_MOVIMENTACAO, cstConstante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                    }
                    else
                    {
                        {
                            //consiste Codigo Subitem repetido
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.GroupBy(itemMovimentacaoMaterial => new { codigoSubitemMaterial = itemMovimentacaoMaterial["codigoSubitemMaterial"] })
                                                                                          .Where(agrupamentoSubitemMaterial => agrupamentoSubitemMaterial.Count() > 1)
                                                                                          //.Select(itemMovimentacaoMaterial => String.Format("'Relação Subitens Requisição' (SUBITEM {0} REQUISITADO MAIS DE UMA VEZ. PARA REQUISITAR NOVAMENTE O MESMO MATERIAL, UTILIZAR NOVA REQUISICAO DE MATERIAL)", itemMovimentacaoMaterial.Key.codigoSubitemMaterial))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format(MensagemErro.FMT_MSGERRO__SUBITEM_CODIGO_REPETIDO, itemMovimentacaoMaterial.Key.codigoSubitemMaterial))
                                                                                          .ToList());
                        }
                        {
                            //consiste Qtde Requisitada
                            decimal vlDecimal = -1.00m;
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => (itemMovimentacaoMaterial["qtdeMaterialRequisitada"].IsNull() || (!Decimal.TryParse(itemMovimentacaoMaterial["qtdeMaterialRequisitada"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out vlDecimal) || vlDecimal <= 0.00m)))
                                                                                          //.Select(itemMovimentacaoMaterial => String.Format("'Relação Subitens Requisição' (QTDE. INVALIDA INFORMADA, PARA SUBITEM {0})", itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format(MensagemErro.FMT_MSGERRO__SUBITEM_QTDE_INVALIDA_MOVIMENTACAO, itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToList());

                            vlDecimal = -1.00m;
                        }
                        {
                            //consiste PTRes
                            int codigoPTResSubitemRequisicao = -1;
                            int[] codigosPTResSubitemEnviados = null;
                            int[] codigosPTResSubitemValidos = null;


                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => itemMovimentacaoMaterial["codigoPTRes"].IsNull() || !Int32.TryParse(itemMovimentacaoMaterial["codigoPTRes"].ToString(), out codigoPTResSubitemRequisicao))
                                                                                          //.Select(itemMovimentacaoMaterial => String.Format("'Relação Subitens Requisição' (CODIGO INVALIDO DE PTRES INFORMADO. VALOR INFORMADO: '{0}', PARA SUBITEM {1})", itemMovimentacaoMaterial["codigoPTRes"].ToString(), itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format(MensagemErro.FMT_MSGERRO__PTRES_CODIGO_INVALIDO_SUBITENS_MOVIMENTACAO, itemMovimentacaoMaterial["codigoPTRes"].ToString(), itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToList());

                            codigosPTResSubitemEnviados = relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => ((Int32.TryParse(itemMovimentacaoMaterial["codigoPTRes"].ToString(), out codigoPTResSubitemRequisicao))))
                                                                                              .Select(itemMovimentacaoMaterial => codigoPTResSubitemRequisicao)
                                                                                              .ToArray();

                            codigosPTResSubitemValidos = requisicaoMaterial.MovimentoItem.Where(itemMovimentacaoMaterial => itemMovimentacaoMaterial.PTRes.IsNotNull() && itemMovimentacaoMaterial.PTRes.Codigo.HasValue)
                                                                                                          .Select(itemMovimentacaoMaterial => itemMovimentacaoMaterial.PTRes.Codigo.Value)
                                                                                                          .ToArray();

                            relacaoInconsistencias.AddRange(codigosPTResSubitemEnviados.Except(codigosPTResSubitemValidos)
                                                                                       //.Select(codigoPTResInexistente => String.Format("'Relação Subitens Requisição' (PTRES {0} NAO-VALIDO, PARA UGE {1:D6})", codigoPTResInexistente, requisicaoMaterial.UGE.Codigo))
                                                                                       .Select(codigoPTResInexistente => String.Format(MensagemErro.FMT_MSGERRO__PTRES_NAO_VALIDO_SUBITENS_MOVIMENTACAO, codigoPTResInexistente, requisicaoMaterial.UGE.Codigo))
                                                                                       .ToList());


                        }
                        {
                            //consiste Codigo Subitem obtido
                            long codigoSubitemRequisicao = -1;
                            long[] codigosSubitensRequisicaoEnviados = null;
                            long[] codigosSubitensRequisicaoExistentesCatalogo = null;

                            //Codigo Subitem invalido
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => ((!Int64.TryParse(itemMovimentacaoMaterial["codigoSubitemMaterial"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out codigoSubitemRequisicao) || codigoSubitemRequisicao <= 0)))
                                                                                          //.Select(itemMovimentacaoMaterial => String.Format("'Relação Subitens Requisição' (CODIGO INVALIDO DE SUBITEM INFORMADO. VALOR INFORMADO: '{0}')", itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format(MensagemErro.FMT_MSGERRO__SUBITEM_CODIGO_INVALIDO_INFORMADO, itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToArray());

                            codigosSubitensRequisicaoEnviados = relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => ((Int64.TryParse(itemMovimentacaoMaterial["codigoSubitemMaterial"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out codigoSubitemRequisicao) || codigoSubitemRequisicao > 0)))
                                                                                              .Select(itemMovimentacaoMaterial => codigoSubitemRequisicao)
                                                                                              .ToArray();

                            codigosSubitensRequisicaoExistentesCatalogo = requisicaoMaterial.MovimentoItem.Where(itemMovimentacaoMaterial => itemMovimentacaoMaterial.SubItemMaterial.IsNotNull())
                                                                                                          .Select(itemMovimentacaoMaterial => itemMovimentacaoMaterial.SubItemMaterial.Codigo)
                                                                                                          .ToArray();

                            relacaoInconsistencias.AddRange(codigosSubitensRequisicaoEnviados.Except(codigosSubitensRequisicaoExistentesCatalogo)
                                                                                             //.Select(codigoSubitemMaterial => String.Format("'Relação Subitens Requisição' (SUBITEM {0} NAO EXISTENTE NO CATALOGO DO ALMOXARIFADO {1:D3})", codigoSubitemMaterial, requisicaoMaterial.Almoxarifado.Codigo))
                                                                                             .Select(codigoSubitemMaterial => String.Format(MensagemErro.FMT_MSGERRO__SUBITEM_NAO_EXISTE_CATALOGO_ALMOXARIFADO, codigoSubitemMaterial, requisicaoMaterial.Almoxarifado.Codigo))
                                                                                             .ToList());
                        }
                    }
                }
            }
            catch (Exception excErroExecucao)
            {
                //relacaoInconsistencias.Add("ERRO AO PROCESSAR SOLICITACAO");
                relacaoInconsistencias.Add(MensagemErro.MSG_ERRO__PROCESSAMENTO_SOLICITACAO);
            }

            return new Tuple<IList<string>, MovimentoEntity>(relacaoInconsistencias, requisicaoMaterial);
        }


        static internal JToken RemoveFields(this JToken token, string[] fields, bool removeIfEmptyValue, bool forceRemove)
        {
            JProperty p = null;
            JContainer container = token as JContainer;
            JTokenType[] JValues_Null = new JTokenType[] { JValue.CreateNull().Type, JValue.CreateString(null).Type };
            if (container == null) return token;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                p = el as JProperty;
                //if (p != null && fields.Contains(p.Name) && ((JValues_Null.Contains(p.Value.Type) && String.IsNullOrWhiteSpace(p.Value.ToString())) && removeIfEmptyValue))
                if (p != null && fields.Contains(p.Name) 
                && (JValues_Null.Contains(p.Value.Type)) 
                && (((String.IsNullOrWhiteSpace(p.Value.ToString())) || (!String.IsNullOrWhiteSpace(p.Value.ToString()) && (p.Value.ToString() == "-1" || p.Value.ToString() == "0")))) 
                && removeIfEmptyValue)
                    removeList.Add(el);
                else if ((p != null && fields.Contains(p.Name)) && forceRemove)
                    removeList.Add(el);

                el.RemoveFields(fields, removeIfEmptyValue, forceRemove);
            }

            foreach (JToken el in removeList)
                el.Remove();


            return token;
        }
    }

}


