using Sam.Common;
using Sam.Common.EMail.Core;
using Sam.Common.Enums;
using Sam.Common.Enums.ChamadoSuporteEnums;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using statusAtendimentoChamado = Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;



namespace Sam.Domain.Business
{
    public class ChamadoSuporteBusiness : BaseBusiness
    {
        private ChamadoSuporteEntity entity = new ChamadoSuporteEntity();
        public bool? envioEmailOk = null;
        public ChamadoSuporteEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        private readonly string cstNomeTabela_ChamadosSuporte = "TB_CHAMADO_SUPORTE";
        private readonly string cstNomeAtributoCampo_AlteradoPor = "AlteradoPor";
        private readonly string cstNomeAtributoCampo_DataAlteracao = "DataAlteracao";
        private readonly string cstNomeAtributoCampo_ValorAtual = "ValorAtual";
        private readonly string cstNomeAtributoCampo_ValorAnterior = "ValorAnterior";
        private readonly string cstChaveAdminGeral = "99999999999";
        private const string cstRodapeMensagemEMail = "Não responder este e-mail diretamente. Responder APENAS via sistema";
        private const string cstSeparador = "==========";

        private readonly string cstEMailParaEnvioSuporteSAM = VariaveisWebConfigEnum.eMailParaEnvioSuporteSam;

        #region MIME Mappings
        private static readonly IDictionary<string, string> _mimeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) 
        {
            {".doc", "application/msword"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".jpe", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".odf", "application/vnd.oasis.opendocument.formula"},
            {".pdf", "application/pdf"},
            {".png", "image/png"},
            {".rtf", "application/rtf"},
            {".txt", "text/plain"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        };
        #endregion

        public IList<ChamadoSuporteEntity> SelecionarChamados(tipoPesquisa tipoPesquisa, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario, long tabelaPesquisaID, bool? chamadosAtivos)
        {
            IList<ChamadoSuporteEntity> lstRetorno = null;
            using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    //var srvInfra = this.Service<IChamadoSuporteService>();
                    var srvInfra = new ChamadoSuporteInfraestructure();
                    lstRetorno = srvInfra.SelecionarChamados(tipoPesquisa, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario, tabelaPesquisaID, chamadosAtivos);

                    if (lstRetorno.HasElements())
                        lstRetorno.ToList()
                                  .ForEach(chamado => { if (chamado.Anexos.IsNotNullAndNotEmpty()) 
                                                            chamado.Anexos.ToList()
                                                                          .ForEach(anexoChamado => anexoChamado.ContentType = this.ObterContentTypePorExtensao(anexoChamado.NomeArquivo)); 

                                                        chamado.DataHoraUltimaEdicao = this.ObterDataUltimaEdicaoAtendimento(chamado.LogHistoricoAtendimento);
                                                      });
                }
                catch (Exception excErroPesquisa)
                {
                    this.ListaErro.Add("Não foi possível obter resultados para esta solicitação.");
                    LogErro.GravarMsgInfo("Modulo ChamadoSuporte", excErroPesquisa.Message);
                }

                tsOperacao.Complete();
            }

            return lstRetorno;
        }

        public bool GravarChamado(ChamadoSuporteEntity chamadoSuporte, bool ehAdminGeral)
        {
            bool blnRetorno = false;
            ChamadoSuporteInfraestructure objInfra = null;

            this.Entity = chamadoSuporte;

            this.ConsistirChamadoSuporte();

            if (this.ListaErro.Count == 0)
            {
                using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        if (chamadoSuporte.StatusChamadoAtendimentoProdesp == (byte)StatusAtendimentoChamado.Finalizado)
                            chamadoSuporte.DataFechamento = DateTime.Now;
                        else
                            chamadoSuporte.DataFechamento = null;


                        objInfra = new ChamadoSuporteInfraestructure();
                        objInfra.Entity = chamadoSuporte;

                        blnRetorno = objInfra.Salvar();

                        if (blnRetorno)
                            PrepararMensagemEMail(chamadoSuporte, ehAdminGeral);
                    }
                    catch (Exception excErroGravacao)
                    {
                        var _descErro = String.Format("Erro ao salvar chamado suporte: {0}", excErroGravacao.Message);

                        ListaErro.Add(_descErro);
                        LogErro.GravarMsgInfo("Chamado Suporte", _descErro);
                    }

                    tsOperacao.Complete();
                }
            }

            return blnRetorno;
        }


        private bool ConsitirChamadoSuporteEmLote(List<ChamadoSuporteEntity> chamadosSuporte)
        {
            foreach(var _chamado in chamadosSuporte)
            {
                this.entity = _chamado;
                this.ConsistirChamadoSuporte();
                this.ConsistirChamadoSuporteAtualizarStatus();

                if (this.ListaErro.Count > 0)
                {
                    this.ListaErro.Insert(0, string.Format("Erro ao processar o chamado #{0:D7}", _chamado.Id));
                    break;
                }
            }

            return this.ListaErro.Count == 0;
        }


        public bool GravarChamadosEmLote(List<ChamadoSuporteEntity> chamadosSuporte)
        {
            bool _blnRetorno = false;
            ChamadoSuporteInfraestructure objInfra = null;

            if (!this.ConsitirChamadoSuporteEmLote(chamadosSuporte))
                return false;

            if (this.ListaErro.Count == 0)
            {
                {
                    using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        foreach(var _chamadoSuporte in chamadosSuporte)
                        {
                            if (_chamadoSuporte.StatusChamadoAtendimentoProdesp == (byte)StatusAtendimentoChamado.Finalizado)
                                _chamadoSuporte.DataFechamento = DateTime.Now;
                            else
                                _chamadoSuporte.DataFechamento = null;


                            objInfra = new ChamadoSuporteInfraestructure();
                            objInfra.Entity = _chamadoSuporte;

                            try
                            {
                                objInfra.Salvar();
                            }
                            catch (Exception excErroGravacao)
                            {
                                var _descErro = String.Format("Erro ao salvar chamado suporte: {0}", excErroGravacao.Message);

                                ListaErro.Add(_descErro);
                                LogErro.GravarMsgInfo("Chamado Suporte", _descErro);

                                break;
                            }
                        }

                        tsOperacao.Complete();
                    }
                }
            }

            return (this.ListaErro.Count == 0);
        }


        public void PrepararMensagemEMail(ChamadoSuporteEntity chamadoSuporte, bool ehAdminGeral)
        {
            string assuntoMensagem = null;
            string corpoMensagem = null;


            DespachanteEMail despachanteEMail = null;
            //ChamadoSuporteEntity chamadoSuporte = null;
            SortedList listagemEMails = null;

            //chamadoSuporte = this.ObterChamadoSuporte(chamadoSuporteID);
            if (chamadoSuporte.IsNotNull())
            {
                try
                {
                    assuntoMensagem = montarAssuntoMensagem(chamadoSuporte);
                    corpoMensagem = montarCorpoMensagem(chamadoSuporte, ehAdminGeral);
                    listagemEMails = gerarRelacaoEMails(chamadoSuporte);
                    despachanteEMail = new DespachanteEMail();


                    despachanteEMail.MontarMensagem(listagemEMails["De"] as string,
                                                    listagemEMails["Para"] as string[],
                                                    listagemEMails["CC"] as string[],
                                                    null,
                                                    assuntoMensagem,
                                                    corpoMensagem,
                                                    null);

                    despachanteEMail.EnviarMensagem();

                    this.envioEmailOk = true;
                }
                catch (Exception excErroEnvioEMail)
                {
                    LogErro.GravarMsgInfo("Chamado Suporte", new LogErroInfraestructure().MontaMsgCompletaErro(excErroEnvioEMail));

                    ListaErro.Add("Erro ao enviar e-mail chamado suporte.");
                    this.envioEmailOk = false;
                }
            }

        }

        private string montarCorpoMensagem(ChamadoSuporteEntity chamadoSuporte, bool ehAdminGeral)
        {
            string fmtTextoMensagem = null;
            string textoMensagem = null;
            string nomeUsuario;
            string statusProdesp = null;
            string statusUsuario = null;
            string statusDaMensagem = null;


            nomeUsuario = chamadoSuporte.NomeUsuario;
            statusProdesp = GeralEnum.GetEnumDescription((statusAtendimentoChamado)chamadoSuporte.StatusChamadoAtendimentoProdesp);
            statusUsuario = GeralEnum.GetEnumDescription((statusAtendimentoChamado)chamadoSuporte.StatusChamadoAtendimentoUsuario);

            if (ehAdminGeral)
                statusDaMensagem = statusProdesp;
            else
                statusDaMensagem = statusUsuario;

            fmtTextoMensagem = "{0}{1},{0}Seu chamado foi alterado para \"{2}\". Favor verificar.{0}{0}Email automático. Favor não responder.";
            if (chamadoSuporte.IsNotNull())
                textoMensagem = String.Format(fmtTextoMensagem, Environment.NewLine, nomeUsuario, statusDaMensagem);

            return textoMensagem;
        }

        private string montarAssuntoMensagem(ChamadoSuporteEntity chamadoSuporte)
        {
            string fmtAssuntoMensagem = "SAM - Chamado Suporte SAM {0} do(a) {1} foi atualizado";
            string assuntoMensagem = null;

            string numeroChamado = null;
            string ugeDescricao = null;

            AlmoxarifadoEntity almoxChamadoSuporte = null;

            if (chamadoSuporte.IsNotNull())
            {
                almoxChamadoSuporte = ((chamadoSuporte.Almoxarifado.IsNotNull() && chamadoSuporte.Almoxarifado.Id.HasValue) ? chamadoSuporte.Almoxarifado : ((chamadoSuporte.Divisao.Almoxarifado.IsNotNull() && chamadoSuporte.Divisao.Almoxarifado.Id.HasValue) ? chamadoSuporte.Divisao.Almoxarifado : null));
                numeroChamado = chamadoSuporte.Id.Value.ToString("D7");
                ugeDescricao = almoxChamadoSuporte.Descricao;

                assuntoMensagem = String.Format(fmtAssuntoMensagem, numeroChamado, ugeDescricao);
            }

            return assuntoMensagem;
        }

        private SortedList gerarRelacaoEMails(ChamadoSuporteEntity chamadoSuporte)
        {
            string[] enderecosDestinatarios = null;
            string[] enderecosEMailEmCopia = null;

            SortedList listagemEMails = new SortedList(StringComparer.InvariantCultureIgnoreCase);

            enderecosDestinatarios = new string[] { cstEMailParaEnvioSuporteSAM };
            enderecosEMailEmCopia = new string[] { chamadoSuporte.EMailUsuario };

            listagemEMails.InserirValor("De", cstEMailParaEnvioSuporteSAM);
            listagemEMails.InserirValor("Para", enderecosDestinatarios);
            listagemEMails.InserirValor("CC", enderecosEMailEmCopia);


            return listagemEMails;
        }

        public void Excluir()
        {
            ChamadoSuporteInfraestructure objInfra = null;

            using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    objInfra = new ChamadoSuporteInfraestructure();
                    objInfra.Entity = this.entity;

                    objInfra.Excluir();
                }
                catch (Exception excErroGravacao)
                {
                    var _descErro = String.Format("Erro ao salvar chamado suporte: {0}", excErroGravacao.Message);

                    ListaErro.Add(_descErro);
                    LogErro.GravarMsgInfo("Chamado Suporte", _descErro);
                }

                tsOperacao.Complete();
            }
        }

        public ChamadoSuporteEntity ObterChamadoSuporte(int chamadoSuporteID)
        {
            ChamadoSuporteEntity objEntidade = null;
            ChamadoSuporteInfraestructure objInfra;

            try
            {
                objInfra = new ChamadoSuporteInfraestructure();
                objEntidade = objInfra.ObterChamadoPorID(chamadoSuporteID);
            }
            catch (Exception excErroGravacao)
            {
                var _descErro = String.Format(excErroGravacao.Message);

                ListaErro.Add(_descErro);
                LogErro.GravarMsgInfo("Chamado Suporte", _descErro);
            }

            return objEntidade;
        }

        public int ObterNumeroChamados()
        {
            int _numRetorno = -1;
            ChamadoSuporteInfraestructure objInfra;

            try
            {
                objInfra = new ChamadoSuporteInfraestructure();
                _numRetorno = objInfra.Count();
            }
            catch (Exception excErroGravacao)
            {
                var _descErro = String.Format(excErroGravacao.Message);

                ListaErro.Add(_descErro);
                LogErro.GravarMsgInfo("Chamado Suporte", _descErro);
            }

            return _numRetorno;
        }

        public void ConsistirChamadoSuporte()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ChamadoSuporteEntity>(ref this.entity);


            if (this.Entity.AmbienteSistema < 0)
                this.ListaErro.Add("É obrigatório selecionar ambiente de sistema.");

            if (this.Entity.Funcionalidade.IsNull() || !this.Entity.Funcionalidade.Id.HasValue || this.Entity.Funcionalidade.Id.Value <= 0)
                this.ListaErro.Add("É obrigatório selecionar alguma funcionalidade do sistema.");

            if (String.IsNullOrWhiteSpace(this.Entity.Observacoes))
                this.ListaErro.Add("É obrigatório inserir alguma observação sobre este chamado.");

            if (this.Entity.SistemaModulo < 0)
                this.ListaErro.Add("É obrigatório selecionar módulo de sistema.");

            if (this.Entity.TipoChamado < 1)
                this.ListaErro.Add("É obrigatório selecionar tipo de chamado.");

            if (String.IsNullOrWhiteSpace(this.Entity.EMailUsuario))
                this.ListaErro.Add("Campo E-Mail não foi informado!");

            if (!TratamentoDados.ValidarEmail(this.Entity.EMailUsuario))
                this.ListaErro.Add("Campo E-Mail não se encontra no formado adequado.");
        }

        public void ConsistirChamadoSuporteAtualizarStatus()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ChamadoSuporteEntity>(ref this.entity);

            if (this.Entity.StatusChamadoAtendimentoProdesp.IsNull() || this.Entity.StatusChamadoAtendimentoProdesp <= 0)
                this.ListaErro.Add("É obrigatório selecionar alguma funcionalidade do sistema.");
            
            if (String.IsNullOrWhiteSpace(this.Entity.Responsavel))
                this.ListaErro.Add("Campo responsável não foi informado!");
        }

        protected string ObterUltimaAcaoHistoricoCampo(string _xmlLog, string _nomeCampoConsulta)
        {
            return ProcessarHistoricoAtendimento(_xmlLog, _nomeCampoConsulta);
        }
        public string ProcessarHistoricoAtendimento(string xmlLog, string nomeCampoConsulta = null)
        {
            if (!String.IsNullOrWhiteSpace(xmlLog) && !XmlUtil.IsXML(xmlLog))
                return "Formato inválido";

            #region Variaveis
            string strRetorno = "";
            string descricaoAcaoEfetuada = null;
            string fmtDescricao = null;
            string alteradoPor = null;
            string dataAlteracao = null;
            string valorCampoCpfAnterior = null;
            string valorAnteriorCampoConsultado = null;
            string valorAtualCampoConsultado = null;
            string dataAlteracaoAnterior = null;
            string nomeCampo = null;
            string[] arrCamposEnums = null;

            StringBuilder _retornoHistorico = new StringBuilder();

            int contador;
            int primeiraOcorrencia = 0;
            bool existeAlteracaoObservacao;
            bool campoEnums = false;
            IList<XElement> listaAlteracoes = null;
            XDocument docXml = null;
            #endregion


            nomeCampo = "Observacoes";
            if (!String.IsNullOrWhiteSpace(nomeCampoConsulta))
            {
                nomeCampo = nomeCampoConsulta;
                arrCamposEnums = new string[] { "StatusChamadoAtendimentoUsuario", "StatusChamadoAtendimentoProdesp", "Funcionalidade", "TipoChamado" };
            }

            docXml = XDocument.Parse(xmlLog);
            existeAlteracaoObservacao = false;
            listaAlteracoes = docXml.Element(cstNomeTabela_ChamadosSuporte).Elements().ToList();
            primeiraOcorrencia = listaAlteracoes.ToList().FindIndex(elXML => elXML.Element(nomeCampo).IsNotNull());
            campoEnums = arrCamposEnums.HasElements() && arrCamposEnums.Contains(nomeCampo);
            contador = listaAlteracoes.Count()-1;

            foreach (var relacaoAlteracoes in listaAlteracoes.OrderByDescending(e => (string)e.Attribute(cstNomeAtributoCampo_DataAlteracao)))
            {
                dataAlteracao = (relacaoAlteracoes.Attribute(cstNomeAtributoCampo_DataAlteracao).IsNotNull() ? ((relacaoAlteracoes as System.Xml.Linq.XElement).Attribute(cstNomeAtributoCampo_DataAlteracao).Value) : null);
                string dataAlteracaoFormatada = string.IsNullOrEmpty(dataAlteracao) ? string.Empty : DateTime.Parse(dataAlteracao).ToString(base.fmtDataHoraFormatoBrasileiro);
                alteradoPor = (relacaoAlteracoes.Attribute(cstNomeAtributoCampo_AlteradoPor).IsNotNull() ? (relacaoAlteracoes.Attribute(cstNomeAtributoCampo_AlteradoPor).Value) : "NULL");


                existeAlteracaoObservacao = listaAlteracoes[contador].Element(nomeCampo).IsNotNull();
                if (existeAlteracaoObservacao)
                {
                    valorAtualCampoConsultado = (listaAlteracoes[contador].Element(nomeCampo).Attribute(cstNomeAtributoCampo_ValorAtual).IsNotNull() ? ((listaAlteracoes[contador] as System.Xml.Linq.XElement).Element(nomeCampo).Attribute(cstNomeAtributoCampo_ValorAtual).Value) : null);

                    if (campoEnums)
                        valorAtualCampoConsultado = obterValorEnum(valorAtualCampoConsultado, nomeCampo);

                    if ((contador == primeiraOcorrencia) && (listaAlteracoes.Count() > 1) && ((contador - 1) > 1))
                    {
                        dataAlteracaoAnterior = (listaAlteracoes[contador - 1].Attribute(cstNomeAtributoCampo_DataAlteracao).IsNotNull() ? (listaAlteracoes[contador - 1].Attribute(cstNomeAtributoCampo_DataAlteracao).Value) : null);
                        valorCampoCpfAnterior = (listaAlteracoes[contador - 1].Attribute(cstNomeAtributoCampo_AlteradoPor).IsNotNull() ? (listaAlteracoes[contador - 1].Attribute(cstNomeAtributoCampo_AlteradoPor).Value) : null);
                        valorAnteriorCampoConsultado = (listaAlteracoes[contador].Element(nomeCampo).IsNotNull() ? (listaAlteracoes[contador].Element(nomeCampo).Attribute(cstNomeAtributoCampo_ValorAnterior).Value) : null);

                        if (campoEnums)
                            valorAnteriorCampoConsultado = obterValorEnum(valorAnteriorCampoConsultado, nomeCampo);

                        fmtDescricao = "{0} ({1}):\n{2}\n{3} ({4}):\n{5}";
                        descricaoAcaoEfetuada = String.Format(fmtDescricao, valorCampoCpfAnterior, dataAlteracaoAnterior, valorAnteriorCampoConsultado, alteradoPor, dataAlteracaoFormatada, valorAtualCampoConsultado);
                    }
                    else
                    {
                        fmtDescricao = "{0} ({1}):\n{2}";

                        if (campoEnums)
                            fmtDescricao = "Status de chamado de suporte alterado para '{2}' ({1})";

                        descricaoAcaoEfetuada = String.Format(fmtDescricao, alteradoPor, dataAlteracaoFormatada, valorAtualCampoConsultado);
                    }
                }

                string _logStyle = contador % 2 == 0 ? "linhaCinza" : "linhaBranca";
                string _formatString = "<div class=\"{0}\"><span class=\"tituloHistorico\">{1} ({2}):</span><div class=\"textoHistorico\">{3}</div></div>";

                _retornoHistorico.AppendLine(string.Format(_formatString, _logStyle, alteradoPor, dataAlteracaoFormatada, valorAtualCampoConsultado));

                descricaoAcaoEfetuada = null;
                contador--;
            }

            return string.Format("<div class=\"divScrollVertical\">{0}</div>", _retornoHistorico.ToString());
        }

        private string obterValorEnum(string valorNumerico, string nomeCampoEnum)
        {
            string descricaoValorEnum = null;
            int iValorNumerico = -1;


            if (!String.IsNullOrWhiteSpace(valorNumerico) && Int32.TryParse(valorNumerico, out iValorNumerico))
            {
                switch (nomeCampoEnum)
                {
                    case "StatusChamadoAtendimentoUsuario":
                    case "StatusChamadoAtendimentoProdesp": descricaoValorEnum = EnumUtils.GetEnumDescription<statusAtendimentoChamado>(((statusAtendimentoChamado)iValorNumerico)); break;
                    case "Funcionalidade":                  descricaoValorEnum = EnumUtils.GetEnumDescription<FuncionalidadeSistema>(((FuncionalidadeSistema)iValorNumerico)); break;
                    case "TipoChamado":                     descricaoValorEnum = EnumUtils.GetEnumDescription<TipoChamadoSuporte>(((TipoChamadoSuporte)iValorNumerico)); break;
                }
            }

            return descricaoValorEnum;
        }

        public DateTime ObterDataUltimaEdicaoAtendimento(string xmlLog)
        {
            if (!String.IsNullOrWhiteSpace(xmlLog) && !XmlUtil.IsXML(xmlLog))
                throw new Exception("Formato inválido");

            #region Variaveis
            DateTime dataAlteracao = new DateTime(0);

            IList<XElement> listaAlteracoes = null;
            XElement ultimaRelacaoAlteracoes = null;
            XDocument docXml = null;
            #endregion



            docXml = XDocument.Parse(xmlLog);
            listaAlteracoes = docXml.Element(cstNomeTabela_ChamadosSuporte).Elements().ToList();

            if (listaAlteracoes.HasElements())
            {
                ultimaRelacaoAlteracoes = listaAlteracoes[listaAlteracoes.Count() - 1];
                dataAlteracao = (ultimaRelacaoAlteracoes.Attribute(cstNomeAtributoCampo_DataAlteracao).IsNotNull() ? (DateTime.Parse((ultimaRelacaoAlteracoes as System.Xml.Linq.XElement).Attribute(cstNomeAtributoCampo_DataAlteracao).Value)) : DateTime.MinValue);
                
            }

            return dataAlteracao;
        }

        public bool ExisteHistoricoParaChamadoSuporte(string xmlLog)
        {
            if (!String.IsNullOrWhiteSpace(xmlLog) && !XmlUtil.IsXML(xmlLog))
                throw new Exception("Formato inválido");

            #region Variaveis
            DateTime dataAlteracao = new DateTime(0);

            IList<XElement> listaAlteracoes = null;
            XDocument docXml = null;
            bool blnRetorno = false;
            #endregion


            docXml = XDocument.Parse(xmlLog);
            listaAlteracoes = docXml.Element(cstNomeTabela_ChamadosSuporte).Elements().ToList();
            blnRetorno = (listaAlteracoes.HasElements() && listaAlteracoes.Count() >= 2);

            return blnRetorno;
        }

        //public bool UltimaInteracaoDoUsuario(string xmlLog)
        //{
        //    if (!String.IsNullOrWhiteSpace(xmlLog) && !XmlUtil.IsXML(xmlLog))
        //        throw new Exception("Formato inválido");

        //    #region Variaveis
        //    DateTime dataAlteracao = new DateTime(0);

        //    IList<XElement> listaAlteracoes = null;
        //    XElement ultimaRelacaoAlteracoes = null;
        //    XElement penultimaRelacaoAlteracoes = null;
        //    XDocument docXml = null;
        //    bool blnRetorno = false;
        //    #endregion



        //    docXml = XDocument.Parse(xmlLog);
        //    listaAlteracoes = docXml.Element(cstNomeTabela_ChamadosSuporte).Elements().ToList();

        //    if (listaAlteracoes.HasElements() && listaAlteracoes.Count() >= 3)
        //    {
        //        //Iteracao AdminGeral
        //        penultimaRelacaoAlteracoes = listaAlteracoes[listaAlteracoes.Count() - 2];
        //        //Iteracao Usuario
        //        ultimaRelacaoAlteracoes = listaAlteracoes[listaAlteracoes.Count() - 1];

        //        blnRetorno = ((penultimaRelacaoAlteracoes.Attribute(cstNomeAtributoCampo_AlteradoPor).IsNotNull() && ultimaRelacaoAlteracoes.Attribute(cstNomeAtributoCampo_AlteradoPor).IsNotNull()) ? 
        //                      (((penultimaRelacaoAlteracoes as XElement).Attribute(cstNomeAtributoCampo_AlteradoPor).Value == cstChaveAdminGeral) && 
        //                       ((ultimaRelacaoAlteracoes as XElement).Attribute(cstNomeAtributoCampo_AlteradoPor).Value != cstChaveAdminGeral)) : 
        //                      false);
        //    }

        //    return blnRetorno;
        //}

        public bool ExtensaoValida(string nomeArquivo)
        {
            bool blnRetorno = false;

            if (!String.IsNullOrWhiteSpace(nomeArquivo))
            {
                string[] extensoesPermitidas = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt", ".jpg", ".jpeg", ".png", ".rtf", ".odf", ".zip" };
                FileInfo fInfo = new FileInfo(nomeArquivo.ToLowerInvariant());
                var extensionFile = fInfo.Extension;

                blnRetorno = extensoesPermitidas.Contains(extensionFile);
            }

            return blnRetorno;
        }

        public string ObterContentTypePorExtensao(string nomeArquivo)
        {
            string mimeType, extensaoArquivo;

            if (nomeArquivo == null)
                throw new ArgumentNullException("nome arquivo");

            var idxExtensao = nomeArquivo.BreakLine(".".ToCharArray()).Count() - 1;
            extensaoArquivo = nomeArquivo.BreakLine(".".ToCharArray())[idxExtensao];

            if (!extensaoArquivo.StartsWith("."))
                extensaoArquivo = "." + extensaoArquivo;

            return _mimeMappings.TryGetValue(extensaoArquivo, out mimeType) ? mimeType : "application/octet-stream";
        }
    }


    public static class ChamadoSuporteBusinessExtensions
    {
        public static bool ExisteHistoricoParaChamado(this ChamadoSuporteEntity chamadoSuporte)
        {
            bool blnRetorno = false;
            ChamadoSuporteBusiness objBusiness = null;

            if (chamadoSuporte.IsNotNull() && (!String.IsNullOrWhiteSpace(chamadoSuporte.LogHistoricoAtendimento) && XmlUtil.IsXML(chamadoSuporte.LogHistoricoAtendimento)))
            {
                objBusiness = new ChamadoSuporteBusiness();
                blnRetorno = objBusiness.ExisteHistoricoParaChamadoSuporte(chamadoSuporte.LogHistoricoAtendimento);
            }


            return blnRetorno;
        }
    }
}
