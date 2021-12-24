using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using TipoEventoEmpenho = Sam.Common.Util.GeralEnum.EmpenhoEvento;
using TipoLicitacaoEmpenho = Sam.Common.Util.GeralEnum.EmpenhoLicitacao;
using tipoMaterialParaLiquidacao = Sam.Common.Util.GeralEnum.TipoMaterial;
using Sam.Integracao.SIAF.Core;
using System.Linq.Expressions;
using Sam.Integracao.SIAF.Configuracao;
using Sam.Common.LambdaExpression;
using System.Linq;



namespace Sam.Domain.Business
{
    public class EmpenhoBusiness : BaseBusiness
    {
        private MovimentoEntity movimento = new MovimentoEntity();

        public MovimentoEntity Movimento
        {
            get { return movimento; }
            set { movimento = value; }
        }

        public void AjustarItensEmpenho()
        {
            IList<MovimentoItemEntity> lstNewMovItem = new List<MovimentoItemEntity>();
            int i = 0;
            foreach (MovimentoItemEntity item in this.Movimento.MovimentoItem)
            {
                if (item.QtdeMov.HasValue || item.QtdeMov > 0)
                {
                    i++;
                    item.Id = i;
                    lstNewMovItem.Add(item);
                }
            }
            // retorna a nova lista de itens
            this.Movimento.MovimentoItem = lstNewMovItem;
        }

        #region Novos Metodos

        public bool GravarNovoEmpenhoEvento(EmpenhoEventoEntity pObjEmpenhoEvento)
        {
            bool lBlnGravadoComSucesso = false;

            try
            {
                this.Service<IEmpenhoEventoService>().Entity = pObjEmpenhoEvento;

                if (!String.IsNullOrEmpty(pObjEmpenhoEvento.Descricao) && (pObjEmpenhoEvento.Codigo != 0))
                {
                    this.Service<IEmpenhoEventoService>().Salvar();
                    lBlnGravadoComSucesso = true;
                }
            }
            catch (Exception lExcExcecao)
            {
                new LogErro().GravarLogErro(lExcExcecao);
                this.ListaErro.Add(lExcExcecao.Message);
                return lBlnGravadoComSucesso;
            }

            return lBlnGravadoComSucesso;
        }

        #endregion

        #region Consistencias

        public bool ConsistirEmpenho()
        {
            if (!Movimento.MovimentoItem.HasElements())
            {
                if (!this.ListaErro.Contains("Os itens da movimentação estão vazios."))
                    this.ListaErro.Add("Os itens da movimentação estão vazios.");
                return false;
            }

            //ConsistirMovimentacaoCompra();

            if (this.ListaErro.Count > 0)
            {
                return false;
            }

            int indice = 0;
            foreach (MovimentoItemEntity item in Movimento.MovimentoItem)
            {
                Movimento.MovimentoItem[indice].Ativo = true;
                indice++;
            }

            return true;
        }

        public bool ConsistirSubItem(MovimentoItemEntity movimentoItem)
        {
            bool valido = true;

            if (movimentoItem.QtdeLiq == null)
            {
                this.ListaErro.Add("Informe a quantidade do Subitem.");
                valido = false;
            }
            if (movimentoItem.QtdeLiq == 0)
            {
                this.ListaErro.Add("A quantidade do Subitem tem que ser maior que zero.");
                valido = false;
            }

            if (movimentoItem.SubItemMaterial.Codigo == 0)
            {
                this.ListaErro.Add("Informe um SubItem.");
                valido = false;
            }
            return valido;
        }

        #endregion

        #region Listagem (eliminar)
        public IList<EmpenhoEventoEntity> ListarEmpenhoEvento()
        {
            return this.Service<IEmpenhoEventoService>().Listar();
        }

        public IList<EmpenhoLicitacaoEntity> ListarEmpenhoLicitacao()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IEmpenhoLicitacaoService>().Listar();
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
        #endregion Listagem (eliminar)

        #region Webservice Empenho

        [Obsolete]
        public IList<MovimentoEntity> CarregarListaEmpenho(int pIntUgeId, string pStrAnoMesReferenciaAlmoxarifado, string pStrLoginUsuario, string pStrSenhaUsuario, bool pBlnNovoMetodo)
        {
            try
            {
                List<MovimentoEntity> lLstMovimentos = new List<MovimentoEntity>();
                XmlNodeList ndlElementos = null;

                MovimentoEntity lObjMovimento = null;
                UGEEntity lObjUGE = null;
                GestorEntity lObjGestor = null;
                string lStrUsuario = string.Empty;
                string lStrSenha = string.Empty;
                string lStrAnoBase = string.Empty;
                string lStrMes = string.Empty;
                string lStrCodigoUGE = string.Empty;
                string lStrCodigoGestao = string.Empty;
                string lStrMsgEstimuloWs = string.Empty;
                string lStrRetornoWs = string.Empty;
                string lStrStatusErroRetornoWs = string.Empty;

                bool lBlnDevAmbienteHomologacao = false;
                string lStrPatternConsultaDadosXml = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDetaconta/documento/Repete/Documento";
                string[] naturezasDespesaPermitidas = null;


                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lObjUGE = this.Service<IUGEService>().Listar(a => a.Id == pIntUgeId).FirstOrDefault();
                        lObjGestor = this.Service<IGestorService>().Listar(lObjUGE.Orgao.Id.Value).FirstOrDefault();
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

                lStrUsuario = pStrLoginUsuario;
                lStrSenha = pStrSenhaUsuario;
                lStrAnoBase = pStrAnoMesReferenciaAlmoxarifado.Substring(0, 4);
                lStrMes = MesExtenso.Mes[int.Parse(pStrAnoMesReferenciaAlmoxarifado.Substring(4, 2))];
                lStrCodigoUGE = lObjUGE.Codigo.ToString();
                //lStrCodigoGestao = (lStrCodigoGestao[2] != '0' && lObjGestor != null) ? lObjGestor.CodigoGestao.ToString().PadLeft(5, '0') : "00001";
                lStrCodigoGestao = lObjGestor.CodigoGestao.ToString().PadLeft(5, '0');


                lBlnDevAmbienteHomologacao = (Siafem.wsURLConsulta.ToUpperInvariant() == Constante.CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO.ToUpperInvariant());
                //lBlnDevAmbienteHomologacao = (ProcessadorServicoSIAF.IsAmbienteHomologacao);

                //if (Const.isSamWebDebugged && Siafem.wsURLConsulta == Constante.CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO)
                if (Constante.isSamWebDebugged && lBlnDevAmbienteHomologacao)
                {
                    //lStrUsuario = "PSIAFEM2012";
                    //lStrSenha   = "13NOVEMBRO";
                    lStrUsuario = Constante.LoginUsuarioPublicoSiafem;
                    lStrSenha = Constante.SenhaUsuarioPublicoSiafem;
                }

                if (String.IsNullOrWhiteSpace(lStrSenha))
                {
                    lObjMovimento = new MovimentoEntity();
                    lObjMovimento.Observacoes = "LOGINERROR";
                    lLstMovimentos.Add(lObjMovimento);
                    return lLstMovimentos;
                }

                //Consulta/Retorno WS SEFAZ
                //lStrMsgEstimuloWs = Siafem.SiafemDocDetaContaGen((int)lObjUGE.Codigo, (int)lObjGestor.CodigoGestao, lStrMes, Constante.CST_CONTA_CONTABIL_EMITIDOS_NAO_LIQUIDADOS[lStrAnoBase].ToString(), Constante.CST_CONTA_CONTABIL_OPCAO);
                lStrMsgEstimuloWs = GeradorEstimuloSIAF.SiafemDocDetaContaGen((int)lObjUGE.Codigo, (int)lObjGestor.CodigoGestao, lStrMes, Constante.CST_CONTA_CONTABIL_EMITIDOS_NAO_LIQUIDADOS[lStrAnoBase].ToString(), Constante.CST_CONTA_CONTABIL_OPCAO);
                lStrRetornoWs = Siafem.recebeMsg(lStrUsuario, lStrSenha, lStrAnoBase, lStrCodigoUGE, lStrMsgEstimuloWs, true);

                string strNomeMensagem = "";
                if (Siafem.VerificarErroMensagem(lStrStatusErroRetornoWs, out strNomeMensagem, out lStrStatusErroRetornoWs))
                {
                    //this.ListaErro.Add(lStrStatusErroRetornoWs);
                    if (!String.IsNullOrWhiteSpace(lStrStatusErroRetornoWs))
                    {
                        lStrStatusErroRetornoWs.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro => this.ListaErro.Add(linhaErro));
                        return null;
                    }
                }

                //lStrRetornoWs = XmlUtil.AgruparItensEmpenho(lStrRetornoWs);
                ndlElementos = XmlUtil.lerNodeListXml(lStrRetornoWs, lStrPatternConsultaDadosXml);

                //Naturezas de Despesas Permitidas para listagem
                naturezasDespesaPermitidas = new string[] { Constante.CST_NATUREZA_DESPESA__BEM_CONSUMO_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_EQUIP_TI__INICIO_SEIS_DIGITOS };

                // listar os documentos
                List<MovimentoEntity> listaMovimentos = new List<MovimentoEntity>();
                ndlElementos.Cast<XmlNode>()
                            .SelectMany(nodes => nodes.ChildNodes.Cast<XmlNode>())
                            .Where(node2 => (node2.Name == "ContaCorrente"))
                            //.Where(node2 => ((node2.InnerText.BreakLine(2).StartsWith("339030") || (node2.InnerText.BreakLine(2).StartsWith("449052")))))
                            .Where(node2 => (naturezasDespesaPermitidas.Any(naturezaDespesa => node2.InnerText.BreakLine(2).StartsWith(naturezaDespesa))))
                            .Select(Linha => new MovimentoEntity()
                            {
                                GeradorDescricao = Linha.InnerText.BreakLine(0),
                                NumeroDocumento = Linha.InnerText.BreakLine(1),
                                Empenho = Linha.InnerText.BreakLine(1),
                                NaturezaDespesaEmpenho = Linha.InnerText.BreakLine(2),
                                FonteRecurso = Linha.InnerText.BreakLine(3),
                            })
                            .OrderBy(Movimento => Movimento.Empenho)
                            .ToList()
                            .ForEach(Movimento => lLstMovimentos.Add(Movimento));

                return lLstMovimentos;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema ao carregar lista de empenhos.");
                return null;
            }
        }

        //Utilizado
        //public IList<string> obterListaEmpenhosSiafem(int codigoUGE, string almoxAnoMesRef, string loginSiafemUsuario, string senhaSiafemUsuario)
        public IList<string> obterListaEmpenhosSiafem(int almoxarifadoId, int gestorId, int codigoUGE, string almoxAnoMesRef, string loginSiafemUsuario, string senhaSiafemUsuario, bool listarRestosAPagar = false, bool consumoImediato = false)
        {
            try
            {
                List<string> listagemEmpenhos = new List<string>();
                UGEEntity ugeAlmox = null;
                GestorEntity gestorAlmox = null;
                ProcessadorServicoSIAF svcSIAFEM = null;
                string usuarioSiafem = string.Empty;
                string senhaSiafem = string.Empty;
                string anoBaseConsulta = string.Empty;
                string mesRefAlmox = string.Empty;
                string lStrCodigoUGE = string.Empty;
                string codigoGestao = string.Empty;
                string msgEstimuloWs = string.Empty;
                string retornoMsgEstimuloWs = string.Empty;
                bool isAmbienteHomologacao = false;
                string contaContabil = null;
                string tipoDeEmpenhoListado = null;


                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        ugeAlmox = this.Service<IUGEService>().Listar(a => a.Codigo == codigoUGE).FirstOrDefault();
                        gestorAlmox = this.Service<IGestorService>().Listar(ugeAlmox.Orgao.Id.Value).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                    finally
                    {
                        tras.Complete();
                    }
                }

                usuarioSiafem = loginSiafemUsuario;
                senhaSiafem = senhaSiafemUsuario;

                anoBaseConsulta = almoxAnoMesRef.Substring(0, 4);
                mesRefAlmox = MesExtenso.Mes[int.Parse(almoxAnoMesRef.Substring(4, 2))];
                codigoGestao = gestorAlmox.CodigoGestao.ToString().PadLeft(5, '0');

                isAmbienteHomologacao = (Siafem.wsURLConsulta.ToUpperInvariant() == Constante.CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO.ToUpperInvariant());

                if (Constante.isSamWebDebugged && isAmbienteHomologacao)
                {
                    usuarioSiafem = Constante.LoginUsuarioPublicoSiafem;
                    senhaSiafem = Constante.SenhaUsuarioPublicoSiafem;
                }

                if (String.IsNullOrWhiteSpace(senhaSiafem))
                    this.ListaErro.Add("Senha em branco");

                if (!listarRestosAPagar)
                {
                    tipoDeEmpenhoListado = "a liquidar";
                    if (Constante.CST_CONTA_CONTABIL_EMITIDOS_NAO_LIQUIDADOS.Keys.Cast<string>().Count(anoContaContabil => anoContaContabil == anoBaseConsulta) == 1)
                        contaContabil = Constante.CST_CONTA_CONTABIL_EMITIDOS_NAO_LIQUIDADOS[anoBaseConsulta].ToString();
                    else
                        throw new Exception(String.Format("Conta Contábil para listagem de empenhos {0}, exercício fiscal de {1} não registrada no sistema. Informar Prodesp, por gentileza.", tipoDeEmpenhoListado, anoBaseConsulta));

                    msgEstimuloWs = GeradorEstimuloSIAF.SiafisicoDocListaPrecoNE(codigoUGE, gestorAlmox.CodigoGestao.Value, null);
                }
                else if (listarRestosAPagar)
                {
                    int anoExercicio = Int32.Parse(anoBaseConsulta);
                    tipoDeEmpenhoListado = "de 'Restos a Pagar'";
                    anoExercicio -= 1;

                    if (Constante.CST_CONTA_CONTABIL_RESTOS_A_PAGAR.Keys.Cast<string>().Count(anoContaContabil => anoContaContabil == anoExercicio.ToString()) == 1)
                        contaContabil = Constante.CST_CONTA_CONTABIL_RESTOS_A_PAGAR[anoExercicio.ToString()].ToString();
                    else
                        throw new Exception(String.Format("Conta Contábil para listagem de empenhos {0}, exercício fiscal de {1} não registrada no sistema. Informar Prodesp, por gentileza.", tipoDeEmpenhoListado, anoExercicio));

                    msgEstimuloWs = GeradorEstimuloSIAF.SiafemDocDetaContaGen(codigoUGE, (int)gestorAlmox.CodigoGestao, mesRefAlmox, contaContabil, Constante.CST_CONTA_CONTABIL_OPCAO);
                }

                //Consulta/Retorno WS SEFAZ
                svcSIAFEM = new ProcessadorServicoSIAF();
                svcSIAFEM.Configuracoes = new ConfiguracoesSIAF();
                svcSIAFEM.ConsumirWS(loginSiafemUsuario, senhaSiafemUsuario, anoBaseConsulta, codigoUGE.ToString(), msgEstimuloWs, false, true);

                if (!svcSIAFEM.ErroProcessamentoWs)
                    retornoMsgEstimuloWs = svcSIAFEM.RetornoWsSIAF;
                else
                    throw new Exception(svcSIAFEM.ErroRetornoWs);

                // listar os documentos
                listagemEmpenhos = processarListaEmpenhos(almoxarifadoId, gestorId, retornoMsgEstimuloWs, anoBaseConsulta, listarRestosAPagar, consumoImediato).ToList();

                string complementoMsg = null;
                if (listarRestosAPagar && !consumoImediato)
                    complementoMsg = "de 'Restos a Pagar',";
                else if (listarRestosAPagar && consumoImediato)
                        complementoMsg = "de 'Restos a Pagar de Empenho de Consumo Imediato',";

                if (listagemEmpenhos.Count == 0)
                    this.ListaErro.Add(String.Format("Nenhum empenho válido {0} retornado por SIAFISICO/SIAFEM.", complementoMsg));

                return listagemEmpenhos;
            }
            catch (Exception excErroCargaEmpenhos)
            {
                new LogErro().GravarLogErro(excErroCargaEmpenhos);
                string cabecalhoMsgErro = "Erro no sistema: ";

                if (excErroCargaEmpenhos.Message.ToLowerInvariant().Contains("pelo sistema".ToLowerInvariant()))
                    cabecalhoMsgErro = null;

                if (excErroCargaEmpenhos.Message.Contains("Falha de comunicação"))
                    excErroCargaEmpenhos.Message.BreakLine("\n".ToCharArray()).ToList().ForEach(linhaAviso => this.ListaErro.Add(linhaAviso));
                else
                    this.ListaErro.Add(cabecalhoMsgErro + excErroCargaEmpenhos.Message);

                return null;
            }
        }

        /// <summary>
        /// Método criado para trazer lista de empenhos, ordenados.
        /// </summary>
        /// <param name="xmlListNodes"></param>
        /// <returns></returns>
        //private IList<string> processarListaEmpenhos(XmlNodeList xmlListNodes)
        private IList<string> processarListaEmpenhos(int almoxarifadoId, int gestorId, string retornoMsgEstimuloWs, string anoBaseConsulta, bool processarRestosAPagar = false, bool consumoImediato = false)
        {
            #region Variaveis
            string sufixoPatternPesquisa = null;
            string patternGenericoPesquisaXml = null;
            //string naturezaDespesaBemConsumo = null;
            //string naturezaDespesaBemPermanente = null;
            string[] naturezasDespesaBensConsumoPermamente = null;
            List<string> listaEmpenhos = null;
            List<string> templista = null;
            string valor = null;

            IQueryable<XmlNode> qryConsulta = null;
            Expression<Func<XmlNode, bool>> expWhere = null;
            Expression<Func<XmlNode, bool>> expWhereAuxiliar = null;
            Func<XmlNode, string> funcTransformacao = null;
            XmlNodeList xmlListNodes = null;
            #endregion



            if (String.IsNullOrWhiteSpace(retornoMsgEstimuloWs))
                throw new Exception("Retorno de consulta inválido, efetuado pela integração SAM/SIAFEM WS.");


            listaEmpenhos = new List<string>();

            if (!processarRestosAPagar)
                sufixoPatternPesquisa = "/tabela";

            //naturezaDespesaBemConsumo = "339030";
            //naturezaDespesaBemPermanente = "449052";
            naturezasDespesaBensConsumoPermamente = new string[] { Constante.CST_NATUREZA_DESPESA__BEM_CONSUMO_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_EQUIP_TI__INICIO_SEIS_DIGITOS };
            patternGenericoPesquisaXml = String.Format("/MSG/SISERRO/Doc_Retorno/*/*/*/Repete{0}", sufixoPatternPesquisa);
            xmlListNodes = XmlUtil.lerNodeListXml(retornoMsgEstimuloWs, patternGenericoPesquisaXml);
            expWhere = (nodeXml => (nodeXml.Name == "ContaCorrente"));


            if (xmlListNodes.Cast<XmlNode>().HasElements())
            {
                qryConsulta = xmlListNodes.Cast<XmlNode>()
                                          .AsQueryable<XmlNode>()
                                          .SelectMany(nodes => nodes.ChildNodes.Cast<XmlNode>());

                if (consumoImediato)
                {
                    var objBusiness = new CatalogoBusiness();

                    List<String> natDespesa = null;
                    //natDespesa = objBusiness.ListarNaturezaConsumoImediato().ToList();
                    natDespesa = objBusiness.ListarNaturezasDespesaConsumoImediato().ToList();

                    expWhere = (nodeXml => (nodeXml.Name == "Numero") && (nodeXml.NextSibling.Name == "Natureza"));



                    funcTransformacao = (nodeXml => String.Format("{0}NE{1}", anoBaseConsulta, nodeXml.InnerText));

                    foreach (var item in natDespesa)
                    {
                        templista = new List<string>();
                        valor = item.Trim().ToString();
                        expWhereAuxiliar = (node2 => (node2.NextSibling.InnerText.Contains(valor)));
                        expWhere = expWhere.And(expWhereAuxiliar);
                        templista = qryConsulta.Where(expWhere)
                                           .Select(funcTransformacao)
                                           .ToList();

                        foreach (var item2 in templista)
                        {
                            listaEmpenhos.Add(item2);
                        }
                    }

                }
                else
                {
                    if (processarRestosAPagar)
                    {
                        expWhere = (nodeXml => ((nodeXml.Name == "ContaCorrente") && (nodeXml.NextSibling.Name == "ValorConta")));
                        //expWhereAuxiliar = (node2 => ((node2.InnerText.Contains(naturezaDespesaBemConsumo) || (node2.InnerText.Contains(naturezaDespesaBemPermanente)))));
                        expWhereAuxiliar = (node2 => (naturezasDespesaBensConsumoPermamente.Any(naturezaDespesa => node2.InnerText.Contains(naturezaDespesa))));

                        expWhere = expWhere.And(expWhereAuxiliar);
                        //funcTransformacao = (nodeXml => String.Format("{0};{1}", nodeXml.InnerText.BreakLineForEmpenho(), Decimal.Parse(nodeXml.NextSibling.InnerText)));
                        funcTransformacao = (nodeXml => String.Format("{0};{1};{2}", nodeXml.InnerText.BreakLineForEmpenho(), Decimal.Parse(nodeXml.NextSibling.InnerText), nodeXml.InnerText.BreakLine(2)));
                        qryConsulta = qryConsulta.SelectMany(nodes => nodes.ChildNodes.Cast<XmlNode>())
                                                 .AsQueryable();
                    }
                    else
                    {
                        expWhere = (nodeXml => (nodeXml.Name == "Numero") && (nodeXml.NextSibling.Name == "Natureza"));
                        //expWhereAuxiliar = (node2 => ((node2.NextSibling.InnerText.Contains(naturezaDespesaBemConsumo) || (node2.NextSibling.InnerText.Contains(naturezaDespesaBemPermanente)))));
                        expWhereAuxiliar = (node2 => (naturezasDespesaBensConsumoPermamente.Any(naturezaDespesa => node2.NextSibling.InnerText.Contains(naturezaDespesa))));
                        expWhere = expWhere.And(expWhereAuxiliar);

                        funcTransformacao = (nodeXml => String.Format("{0}NE{1}", anoBaseConsulta, nodeXml.InnerText));
                    }

                    listaEmpenhos = qryConsulta.Where(expWhere)
                                          .Select(funcTransformacao)
                                          .ToList();
                }
               
            }

            listaEmpenhos = TrataListaDeEmpenhosPorAgrupamento(almoxarifadoId, gestorId, listaEmpenhos, consumoImediato);
            listaEmpenhos.Sort();

            return listaEmpenhos;
        }

        public List<string> TrataListaDeEmpenhosPorAgrupamento(int almoxarifadoId, int gestorId, List<string> listaEmpenhos, bool consumoImediato)
        {
            var srvEmpenho = this.Service<IEmpenhoEventoService>();
            IList<string> lstEmpenhos = null;


            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstEmpenhos = srvEmpenho.ListarEmpenhosPorAgrupamento(almoxarifadoId, gestorId, listaEmpenhos, consumoImediato).ToList();
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

            return lstEmpenhos.ToList();
        }

        /// <summary>
        /// Sobrecarga (refatorada) do método CarregarItensEmpenho(int, int, string, string, string, string, int?).
        /// </summary>
        /// <param name="pIntUgeID"></param>
        /// <param name="pIntAlmoxarifadoID"></param>
        /// <param name="pStrCodigoEmpenho"></param>
        /// <param name="pStrAnoMesReferencia"></param>
        /// <param name="pStrLoginUsuario"></param>
        /// <param name="pStrSenhaUsuario"></param>
        /// <param name="pNulIntGestorID"></param>
        /// <returns></returns>
        public MovimentoEntity ObterMovimentoEmpenho(int codigoUGE, int almoxID, string numeroEmpenho, string almoxAnoMesRef, string loginUsuarioSiafem, string senhaUsuarioSiafem, bool isConsumoImediato)
        {
            string strUGE = null;
            string strEmpenho = numeroEmpenho;
            string retornoEstimuloWs = null;
            string msgEstimuloWs = null;
            string anoBaseConsulta = null;


            try
            {
                AlmoxarifadoEntity almoxConsulta = null;
                MovimentoEntity movimentacaoMaterial = new MovimentoEntity();
                MovimentoItemEntity movItem = new MovimentoItemEntity();
                UGEEntity ugeAlmox = null;
                ProcessadorServicoSIAF svcSIAFISICO = null;


                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        almoxConsulta = this.Service<IAlmoxarifadoService>().ObterAlmoxarifado(almoxID);
                        ugeAlmox = this.Service<IUGEService>().Listar(uge => uge.Codigo == codigoUGE).FirstOrDefault();
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

                var codigoGestao = almoxConsulta.Gestor.CodigoGestao.Value;
                anoBaseConsulta = almoxConsulta.MesRef.Substring(0, 4);


                //Consulta/Retorno WS SEFAZ
                msgEstimuloWs = GeradorEstimuloSIAF.SiafisicoConPrecoNE(codigoUGE, codigoGestao, numeroEmpenho);

                svcSIAFISICO = new ProcessadorServicoSIAF();
                svcSIAFISICO.Configuracoes = new ConfiguracoesSIAF() { ExibeMsgErroCompleta = false };
                svcSIAFISICO.ConsumirWS(loginUsuarioSiafem, senhaUsuarioSiafem, anoBaseConsulta, codigoUGE.ToString(), msgEstimuloWs, false);

                if (!svcSIAFISICO.ErroProcessamentoWs)
                {
                    retornoEstimuloWs = svcSIAFISICO.RetornoWsSIAF;
                }
                else
                {
                    svcSIAFISICO.ErroRetornoWs.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro => this.ListaErro.Add(linhaErro));
                    return null;
                }

                retornoEstimuloWs = XmlUtil.AgruparDescricoesItemEmpenho(retornoEstimuloWs);
                movimentacaoMaterial = preencherMovimentacaoDoSiafisico(retornoEstimuloWs, ugeAlmox.Id.Value, almoxID, true, isConsumoImediato);

                if (movimentacaoMaterial != null)
                {
                    movimentacaoMaterial.Id = 0;
                    movimentacaoMaterial.Almoxarifado = new AlmoxarifadoEntity(almoxID);
                }

                return movimentacaoMaterial;
            }
            catch (XmlException excErroParsingXmlPropagado)
            {
                LogErroEntity erro = new LogErroEntity() { Data = DateTime.Now, StrackTrace = excErroParsingXmlPropagado.StackTrace, Message = excErroParsingXmlPropagado.ToString() };
                LogErro.InserirEntradaNoLog(erro);

                new LogErro().GravarLogErro(excErroParsingXmlPropagado);
                this.ListaErro.Add(excErroParsingXmlPropagado.Message.Replace("{codigoUGE}", strUGE).Replace("{numeroEmpenho}", strEmpenho));
                return null;
            }
            catch (Exception excErroPropagado)
            {
                var strMsgErroDetalhado = (excErroPropagado.InnerException.IsNotNull() ? excErroPropagado.InnerException.Message : excErroPropagado.Message);
                var fmtMsgErroDetalhado = (!String.IsNullOrWhiteSpace(strMsgErroDetalhado) ? "Erro ao carregar itens do empenho {0}: [{1}]\n" : "Erro ao carregar itens do empenho {0}.\n");

                LogErroEntity erro = new LogErroEntity() { Data = DateTime.Now, StrackTrace = excErroPropagado.StackTrace, Message = excErroPropagado.ToString() };
                LogErro.InserirEntradaNoLog(erro);

                new LogErro().GravarLogErro(excErroPropagado);
                this.ListaErro.Add(String.Format(fmtMsgErroDetalhado, numeroEmpenho, strMsgErroDetalhado));
                return null;
            }
        }

        public IList<MovimentoItemEntity> ProcessarItensEmpenho(string msgRetornoWs, int ugeID, int almoxID, bool isConsumoImediato = false)
        {
            XmlNodeList xndElementos = null;
            List<MovimentoItemEntity> lstItensMovimento = new List<MovimentoItemEntity>();
            MovimentoItemEntity movItem = null;
            MovimentoItemEntity movItemFiltro = null;

            ItemMaterialEntity itemMaterial = new ItemMaterialEntity();
            SubItemMaterialEntity subitemMaterial = new SubItemMaterialEntity();
            CatalogoBusiness objBusiness = new CatalogoBusiness();

            string codigoMaterial = string.Empty;
            string strSeparadorDec = string.Empty;
            string strXmlRetornoPattern = string.Empty;
            string codigoNaturezaDespesa = string.Empty;
            string codigoPT = string.Empty;
            //string produtor = string.Empty;
            string produtorPPais = string.Empty;
            string itMeEpp = string.Empty;
            int iContador = 0;
            int iCodigoItemMaterial = 0;
            int iCodigoNaturezaDespesa = 0;
            ICatalogoBaseService svcInfra = null;

            bool existeEstaNaturezaDespesaParaItemSiafisico = false;
            var qtdeTotalItemMaterial = _valorZero;
            var saldoTotalItemMaterial = _valorZero;
            int? gestorAlmoxID = 0;
            string tagCodigoItemMaterial = "Item";
            string tagQuantidadeItemMaterial = "Quantidade";
            string tagUnidadeFornecimento = "UF";
            string tagPrecoUnitarioItemMaterial = "ValorUnitario";
            string tagDescricaoItemMaterial = "DescricaoItem";
            string tagSaldoSiafem = "Saldo";
            //string tagProdutor = "Produtor";
            string tagProdutorPPais = "ProdutorPPais";
            string tagItMeEpp = "ItMeEpp";


            strXmlRetornoPattern = "/MSG/SISERRO/Doc_Retorno/SFCODOC/NE/";

            var codigoPTRes = Int32.Parse(XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_Ptres"))); // Numero Ptres
            codigoPT = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_ProgTrabalho")); // Numero Ptres
            var codigoEmpenho = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_NumNE")); // Numero Empenho

            xndElementos = XmlUtil.lerNodeListXml(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "RepeteDescricao/Descricao"));
            codigoNaturezaDespesa = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_ND")); // Natureza de Despesa
            iCodigoNaturezaDespesa = Int32.Parse(codigoNaturezaDespesa);

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    svcInfra = this.Service<IAlmoxarifadoService>();
                    gestorAlmoxID = (svcInfra as IAlmoxarifadoService).ObterAlmoxarifado(almoxID).Gestor.Id;
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


            foreach (XmlNode xmlElemento in xndElementos)
            {
                movItem = new MovimentoItemEntity();
                iContador++;

                itemMaterial = new ItemMaterialEntity();
                subitemMaterial = new SubItemMaterialEntity();
                codigoMaterial = xmlElemento[tagCodigoItemMaterial].InnerText;
                //produtor = xmlElemento[tagProdutor].InnerText;
                produtorPPais = xmlElemento[tagProdutorPPais].InnerText;
                itMeEpp = xmlElemento[tagItMeEpp].InnerText;

                try
                {
                    objBusiness = new CatalogoBusiness();

                    if (!String.IsNullOrWhiteSpace(codigoMaterial))
                    {
                        iCodigoItemMaterial = Int32.Parse(codigoMaterial);
                        itemMaterial = objBusiness.ListarItemMaterialPorCodigoSiafisico(iCodigoItemMaterial).FirstOrDefault();

                        if (itemMaterial == null) // salvar automaticamente o item de material siafisico (acrescentado o parâmetro de salvar automaticamente para "true")
                            itemMaterial = objBusiness.RecuperarCadastroItemMaterialDoSiafisico(codigoMaterial, true);

                        if (itemMaterial == null)
                        {
                            if (this.ListaErro.IsNullOrEmpty())
                                this.ListaErro = objBusiness.ListaErro;
                            else
                                this.ListaErro.AddRange(objBusiness.ListaErro);

                            return null;
                        }

                        //Se retornar 1, Natureza de Despesa cadastrada para item SIAFISICO, caso contrário, atualiza cadastro na base, via consulta WS SIAFISICO
                        existeEstaNaturezaDespesaParaItemSiafisico = itemMaterial.NaturezaDespesa.Where(NaturezaDespesa => NaturezaDespesa.Codigo == iCodigoNaturezaDespesa)
                                                                                                 .ToList()
                                                                                                 .Count == 1;
                        if (!existeEstaNaturezaDespesaParaItemSiafisico)
                        {
                            objBusiness.AtualizaRelacaoItemNaturezaDespesa(itemMaterial.Id.Value, codigoNaturezaDespesa);
                            itemMaterial = objBusiness.ListarItemMaterialPorCodigoSiafisico(iCodigoItemMaterial).FirstOrDefault();
                        }

                        if (itemMaterial != null)
                        {
                            var catalogoOrgao = objBusiness.ListarSubItemMaterial(itemMaterial.Id.Value, gestorAlmoxID.Value, true).ToList();
                            var catalogoAlmox = objBusiness.ListarSubItemMaterialTodosCodPorItemUgeAlmox(itemMaterial.Codigo, ugeID, almoxID).ToList();
                            var catalogoAlmoxSubitensAtivos = (catalogoAlmox.IsNotNull()) ? catalogoAlmox.Where(mSubitemMaterial => mSubitemMaterial.IndicadorAtividadeAlmox == true
                                                                                                                            && mSubitemMaterial.NaturezaDespesa.Codigo == iCodigoNaturezaDespesa).ToList() : null;

                            if (catalogoOrgao.IsNullOrEmpty())
                            {
                                this.ListaErro.Add(String.Format("O Item de Material {0} - {1} não possui subitens cadastrados no catálogo do gestor!", codigoMaterial, itemMaterial.Descricao));
                                continue;
                            }
                            else if (catalogoAlmox.IsNullOrEmpty())
                            {
                                this.ListaErro.Add(String.Format("O Item de Material {0} - {1} não possui subitens cadastrados no catálogo do almoxarifado!", codigoMaterial, itemMaterial.Descricao));
                                continue;
                            }
                            //TODO: Informar quando não houver subitem para a Natureza de Despesa do Empenho
                            else if (catalogoAlmox.HasElements() && catalogoAlmoxSubitensAtivos.IsNullOrEmpty())
                            {
                                this.ListaErro.Add(String.Format("O Item de Material {0} - {1} não possui subitens ativos, cadastrados para Natureza de Despesa {2}!", codigoMaterial, itemMaterial.Descricao, iCodigoNaturezaDespesa));
                                continue;
                            }
                            else
                            {
                                subitemMaterial = catalogoAlmoxSubitensAtivos.First();
                                movItem.SubItemMaterial = subitemMaterial;
                                movItem.PrecoUnit = Decimal.Parse(xmlElemento[tagPrecoUnitarioItemMaterial].InnerText);
                                //movItem.Produtor = produtor;
                                movItem.Produtor = produtorPPais;
                                movItem.ItMeEpp = itMeEpp;

                                movItemFiltro = new MovimentoItemEntity();
                                movItemFiltro.ItemMaterial = new ItemMaterialEntity { Codigo = itemMaterial.Codigo };
                                movItemFiltro.Movimento = new MovimentoEntity() { Ativo = true, Empenho = codigoEmpenho, Almoxarifado = new AlmoxarifadoEntity() { Id = almoxID } };
                                movItemFiltro.UGE = new UGEEntity(ugeID);
                                movItemFiltro.SubItemMaterial = subitemMaterial;


                                // procura a quantidade
                                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                                {
                                    try
                                    {
                                        var _codigoUnidadeFornecimentoSiafisico = Int32.Parse(xmlElemento[tagUnidadeFornecimento].InnerText);
                                        svcInfra = this.Service<IMovimentoService>();
                                        movItem.UnidadeFornecimentoSiafisico = (svcInfra as IMovimentoService).ObterUnidadeFornecimentoSiafisico(_codigoUnidadeFornecimentoSiafisico);
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

                                // qtd a liquidar foi trocada
                                movItem.QtdeLiqSiafisico = Decimal.Parse(xmlElemento[tagQuantidadeItemMaterial].InnerText);
                                movItem.SaldoLiqSiafisico = Decimal.Parse(xmlElemento[tagSaldoSiafem].InnerText);
                                //decimal qtdeFaltanteALiquidar = 0.000m;
                                //decimal saldoFaltanteALiquidar = 0.00m;

                                //Decimal.TryParse(xmlElemento[tagQuantidadeItemMaterial].InnerText, out qtdeFaltanteALiquidar);
                                //Decimal.TryParse(xmlElemento[tagQuantidadeItemMaterial].InnerText, out saldoFaltanteALiquidar);
                                //movItem.QtdeLiqSiafisico = qtdeFaltanteALiquidar;
                                //movItem.SaldoLiqSiafisico = saldoFaltanteALiquidar;

                                int cont = 0;
                                foreach (XmlNode xmlElementoItem in xndElementos)
                                {

                                    int ItemCompare = Convert.ToInt32(xmlElementoItem[tagCodigoItemMaterial].InnerText.Trim());
                                    if (movItemFiltro.ItemMaterial.Codigo == ItemCompare)
                                        cont++;
                                }

                                string unidadeFornSiafem = string.Empty;

                                if (cont > 1)
                                {

                                    if (string.IsNullOrEmpty(movItem.Produtor) && string.IsNullOrEmpty(movItem.ItMeEpp))
                                        unidadeFornSiafem = movItem.UnidadeFornecimentoSiafisico.Descricao != null ? movItem.UnidadeFornecimentoSiafisico.Descricao.Trim() : string.Empty;
                                    else
                                    {
                                        unidadeFornSiafem = movItem.UnidadeFornecimentoSiafisico.Descricao != null ? movItem.UnidadeFornecimentoSiafisico.Descricao.Trim() + "-" + movItem.Produtor + "" + movItem.ItMeEpp  : string.Empty;
                                    }
                                }
                                var somaLiq = RecalculoQtdeLiqItemSiafisico(movItemFiltro.ItemMaterial.Codigo, ugeID, codigoEmpenho, almoxID, gestorAlmoxID.Value, iCodigoNaturezaDespesa, unidadeFornSiafem, isConsumoImediato);
                                //TODO CRIAR VERSAO QUE CALCULE O SALDO CONTABIL RESTANTE
                                //var somatoriaValorItemEmpenho = RecalculoValorLiquidadoItemEmpenho(movItemFiltro.ItemMaterial.Codigo, ugeID, codigoEmpenho, almoxID, gestorAlmoxID.Value, iCodigoNaturezaDespesa, unidadeFornSiafem);
                                //decimal somatoriaValorItemEmpenho = 0.00m;
                                //if(isConsumoImediato)
                                    //somatoriaValorItemEmpenho = RecalculoValorLiquidadoItemEmpenho(movItemFiltro.ItemMaterial.Codigo, ugeID, codigoEmpenho, almoxID, gestorAlmoxID.Value, iCodigoNaturezaDespesa, unidadeFornSiafem);

                                qtdeTotalItemMaterial = somaLiq.Item1;
                                saldoTotalItemMaterial = somaLiq.Item2;

                                movItem.QtdeLiq = movItem.QtdeLiqSiafisico - qtdeTotalItemMaterial;
                                movItem.SaldoLiq = movItem.SaldoLiqSiafisico - saldoTotalItemMaterial;
                                //movItem.QtdeLiq = ((isConsumoImediato) ? (1.000m) : (movItem.QtdeLiqSiafisico - qtdeTotalItemMaterial));
                                //movItem.SaldoLiq = ((isConsumoImediato) ? ((movItem.QtdeLiqSiafisico * movItem.PrecoUnit) - somatoriaValorItemEmpenho) : (movItem.SaldoLiqSiafisico - saldoTotalItemMaterial));
                                movItem.SaldoValor = (isConsumoImediato ? movItem.SaldoLiq : null);

                                if (movItem.QtdeLiq == 0.00m)
                                    movItem.QtdeMov = 0.00m;

                                movItem.ItemMaterial = itemMaterial;
                                movItem.ValorMov = movItem.PrecoUnit * movItem.QtdeLiq;
                                //movItem.ValorMov = ((isConsumoImediato) ? movItem.SaldoLiq : (movItem.PrecoUnit * movItem.QtdeLiq));
                                movItem.Id = iContador;
                                movItem.Posicao = iContador;
                                movItem.PTRes = new PTResEntity() { Codigo = codigoPTRes, ProgramaTrabalho = new ProgramaTrabalho(codigoPT) };

                                lstItensMovimento.Add(movItem);
                            }
                        }
                        else
                        {
                            var strDescricaoItemMaterial = xmlElemento[tagDescricaoItemMaterial].InnerText.Replace("-", "");
                            this.ListaErro.Add(String.Format("O Item de Material {0} - {1} não está cadastrado no sistema!", codigoMaterial.Insert(codigoMaterial.Length - 1, "-"), strDescricaoItemMaterial));
                        }

                        existeEstaNaturezaDespesaParaItemSiafisico = false;
                        iCodigoItemMaterial = 0;
                        itemMaterial = null;
                    }
                }
                catch (Exception excErroParaPropagacao)
                {
                    var strDescricaoItemMaterial = xmlElemento[tagDescricaoItemMaterial].InnerText.Replace("-", "");
                    this.ListaErro.Add(String.Format("Não foi possível cadastrar automaticamente Item de Material {0} - {1} no sistema! [{2}]", codigoMaterial.Insert(codigoMaterial.Length - 1, "-"), strDescricaoItemMaterial, excErroParaPropagacao.Message));
                }
            }

            return lstItensMovimento;
        }
        private MovimentoEntity preencherMovimentacaoDoSiafisico(string msgRetornoWs, int ugeID, int almoxID, bool preencheMovimentoItens, bool isConsumoImediato = false)
        {
            MovimentoEntity movimentacaoMaterial = null;
            EstruturaOrganizacionalBusiness objBusiness = null;
            ProcessadorServicoSIAF svcSIAFEM = null;

            string eventoEmpenho = String.Empty;
            string numeroEmpenho = String.Empty;
            string strNaturezaDespesa = String.Empty;
            string strPtRes = String.Empty;
            string licitacaoEmpenho = String.Empty;
            string dataEmissao = String.Empty;
            string valorTotalEmpenho = String.Empty;
            string credorCpfCgC = String.Empty;
            string strServicoOuMaterial = String.Empty;
            string nomeFornecedor = String.Empty;
            string strXmlRetornoPattern = String.Empty;
            string strDataLancamento = String.Empty;
            //string strMsgRetornoWs = String.Empty;

            #region Parsing
            strXmlRetornoPattern = "/MSG/SISERRO/Doc_Retorno/SFCODOC/*/";



            eventoEmpenho = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_Evento")); // Evento
            numeroEmpenho = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_NumNE")); // Numero Empenho
            licitacaoEmpenho = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_Licitacao")); // Licitação
            strNaturezaDespesa = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_ND")); // Natureza de Despesa
            strPtRes = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_Ptres")); // PtRes vinculado ao empenho
            dataEmissao = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_DataEmissao")); // Data de emissão
            strDataLancamento = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_LancadoPorEm")); // Data de lançamento
            valorTotalEmpenho = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_Valor")); // Valor
            credorCpfCgC = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_DoctoUgCred")); // CNPJ/CPF - localiza fornecedorEmpenho 
            strServicoOuMaterial = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_ServMaterial")); // Serviço (1) ou Material (2) - Serviço é descartado do SIAFISICO
            nomeFornecedor = credorCpfCgC.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim(); //credorCpfCgC.BreakLine('-', 1).Trim();
            credorCpfCgC = credorCpfCgC.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim(); //credorCpfCgC.BreakLine('-', 0).Trim();
            #endregion Parsing

            if (String.IsNullOrWhiteSpace(strServicoOuMaterial) || strServicoOuMaterial == "1")
            {
                this.ListaErro.Add("Empenho não é de materiais.");
                return null;
            }

            svcSIAFEM = new ProcessadorServicoSIAF();
            objBusiness = new EstruturaOrganizacionalBusiness();
            movimentacaoMaterial = new MovimentoEntity();
            movimentacaoMaterial.GeradorDescricao = String.Format("{0} - {1}", credorCpfCgC, nomeFornecedor);
            //movimentacaoMaterial.DataDocumento = XmlUtil.converterDataEmpenho(dataEmissao);
            movimentacaoMaterial.DataDocumento = DateTime.Parse(dataEmissao);
            movimentacaoMaterial.NaturezaDespesaEmpenho = strNaturezaDespesa;

            #region Fornecedor
            if (credorCpfCgC.Length > 6)
                movimentacaoMaterial.Fornecedor = objBusiness.ListarFornecedorPorPalavraChave(credorCpfCgC).FirstOrDefault();
            else
                movimentacaoMaterial.Fornecedor = objBusiness.ListarFornecedorPorPalavraChave(nomeFornecedor).FirstOrDefault();

            if (movimentacaoMaterial.Fornecedor == null)
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        // criar um fornecedorEmpenho novo
                        FornecedorEntity fornecedorEmpenho = new FornecedorEntity();


                        fornecedorEmpenho.CpfCnpj = credorCpfCgC;
                        fornecedorEmpenho.Nome = nomeFornecedor;
                        fornecedorEmpenho.Logradouro = XmlUtil.getXmlValue(msgRetornoWs, String.Format("{0}{1}", strXmlRetornoPattern, "cd_LocalEntrega"));
                        fornecedorEmpenho.Bairro = null;
                        fornecedorEmpenho.Cidade = null;
                        fornecedorEmpenho.Uf = this.Service<IUFService>().Listar().Where(a => a.Sigla == "SP").FirstOrDefault();
                        fornecedorEmpenho.Cep = null;
                        fornecedorEmpenho.Telefone = "0000000000";
                        fornecedorEmpenho.InformacoesComplementares = "Cadastro Automático - Carga Empenho SIAFEM";


                        this.Service<IFornecedorService>().Entity = fornecedorEmpenho;
                        this.Service<IFornecedorService>().Salvar();

                        movimentacaoMaterial.Fornecedor = fornecedorEmpenho;
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
            #endregion Fornecedor

            #region Itens Movimentacao
            if (preencheMovimentoItens)
                movimentacaoMaterial.MovimentoItem = ProcessarItensEmpenho(msgRetornoWs, ugeID, almoxID, isConsumoImediato);

            var preencherListaSubitensOuNao = (preencheMovimentoItens ? false : false);
            if (!preencherListaSubitensOuNao && movimentacaoMaterial.MovimentoItem.IsNullOrEmpty())
            {
                if (this.ListaErro.IsNullOrEmpty())
                    this.ListaErro = new List<string>() { "Erro ao efetuar carga de empenho" };

                return null;
            }
            #endregion Itens Movimentacao

            #region Tipo Evento Empenho
            //Evento
            //Dado do Evento necessário para efetuar liquidação por empenho.
            if (!String.IsNullOrEmpty(eventoEmpenho))
            {
                var srvEmpenhoEvento = this.Service<IEmpenhoEventoService>();
                int _codigoEventoEmpenho = -1;
                EmpenhoEventoEntity objEventoEmpenho = null;

                eventoEmpenho = eventoEmpenho.RetirarGrandesEspacosEmBranco();
                string codigoEvento = eventoEmpenho.Substring(0, eventoEmpenho.IndexOf("-")).Trim().PadLeft(6, '0');
                string descEvento = eventoEmpenho.Substring(eventoEmpenho.IndexOf("-") + 2).Trim();

                Int32.TryParse(codigoEvento, out _codigoEventoEmpenho);
                objEventoEmpenho = srvEmpenhoEvento.ObterEventoEmpenho(_codigoEventoEmpenho);
                if (objEventoEmpenho != null)
                {
                    movimentacaoMaterial.EmpenhoEvento = objEventoEmpenho;
                }
                else
                {
                    objEventoEmpenho = new EmpenhoEventoEntity() { Codigo = Int32.Parse(codigoEvento), Descricao = descEvento, CodigoDescricao = eventoEmpenho };

                    if (this.GravarNovoEmpenhoEvento(objEventoEmpenho))
                        movimentacaoMaterial.EmpenhoEvento = objEventoEmpenho;
                }
            }
            #endregion Tipo Evento Empenho

            #region Tipo Licitacao Empenho
            // Tipo de Licitação do Empenho
            if (!String.IsNullOrWhiteSpace(licitacaoEmpenho))
            {
                short codigoLicitacao = 0;
                //licitacaoEmpenho = licitacaoEmpenho.Substring(0, 1);

                //Int16.TryParse(licitacaoEmpenho.BreakLine(0), out codigoLicitacao);
                //movimentacaoMaterial.EmpenhoLicitacao = new EmpenhoLicitacaoEntity() { Id = codigoLicitacao, Descricao = licitacaoEmpenho.BreakLine(1) };
                Int16.TryParse(licitacaoEmpenho.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0], out codigoLicitacao);
                movimentacaoMaterial.EmpenhoLicitacao = new EmpenhoLicitacaoEntity() { Id = codigoLicitacao, Descricao = licitacaoEmpenho.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1] };
            }
            #endregion Tipo Licitacao Empenho

            if (!String.IsNullOrWhiteSpace(valorTotalEmpenho))
                movimentacaoMaterial.ValorDocumento = Decimal.Parse(valorTotalEmpenho);

            return movimentacaoMaterial;
        }

        #endregion

        #region pseudo-mocks

        public string ObterDescricaoTipoEmpenho(MovimentoEntity pObjMovimento)
        {

            int lIntCodigoEmpenhoEvento = 0;
            int lIntCodigoEmpenhoLicitacao = 0;
            string lStrRetorno = string.Empty;
            bool lBlnEmpenhoEventoIsNull = false;
            bool lBlnEmpenhoLicitacaoIsNull = false;
            bool lBlnIsBEC = false;

            //lBlnEmpenhoEventoIsNull    = (pObjMovimento.EmpenhoEvento == null || pObjMovimento.EmpenhoEvento.Codigo == null);
            lBlnEmpenhoEventoIsNull = (pObjMovimento.EmpenhoEvento == null || pObjMovimento.EmpenhoEvento.Codigo <= 0);
            lBlnEmpenhoLicitacaoIsNull = (pObjMovimento.EmpenhoLicitacao == null || String.IsNullOrEmpty(pObjMovimento.EmpenhoLicitacao.CodigoFormatado));


            if (lBlnEmpenhoEventoIsNull || lBlnEmpenhoLicitacaoIsNull)
            {
                pObjMovimento.EmpenhoEvento = this.Service<IEmpenhoEventoService>().ObterEventoEmpenho(pObjMovimento.EmpenhoEvento.Id.Value);
                pObjMovimento.EmpenhoLicitacao = this.Service<IEmpenhoLicitacaoService>().ObterTipoLicitacao(pObjMovimento.EmpenhoLicitacao.Id.Value);

                lIntCodigoEmpenhoEvento = pObjMovimento.EmpenhoEvento.Codigo;
                lIntCodigoEmpenhoLicitacao = Int32.Parse(pObjMovimento.EmpenhoLicitacao.CodigoFormatado);
            }
            else if (!lBlnEmpenhoEventoIsNull)
            {
                lIntCodigoEmpenhoEvento = (int)pObjMovimento.EmpenhoEvento.Codigo;
                pObjMovimento.EmpenhoEvento.Codigo = lIntCodigoEmpenhoEvento;
            }
            else if (!lBlnEmpenhoLicitacaoIsNull)
            {
                lIntCodigoEmpenhoEvento = Int32.Parse(pObjMovimento.EmpenhoLicitacao.CodigoFormatado);
                pObjMovimento.EmpenhoLicitacao.CodigoFormatado = lIntCodigoEmpenhoEvento.ToString();
            }

            lBlnIsBEC = (lIntCodigoEmpenhoEvento == (int)GeralEnum.EmpenhoEvento.BEC);

            if (lBlnIsBEC)
                lStrRetorno = "BEC";
            else if (!lBlnIsBEC && !(lIntCodigoEmpenhoLicitacao == (int)GeralEnum.EmpenhoLicitacao.Pregao))
                lStrRetorno = "SIAFISICO";
            else
                lStrRetorno = pObjMovimento.EmpenhoLicitacao.Descricao;


            return lStrRetorno;
        }

        public List<MovimentoEntity> ListarEmpenhosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId, string pStrAnoMesReferencia, string pStrCPF_CNPJ_Fornecedor)
        {
            List<MovimentoEntity> lLstRetorno = new List<MovimentoEntity>();

            lLstRetorno = this.Service<IMovimentoService>().ListarMovimentosNaoLiquidados(pIntUgeId, pIntAlmoxarifadoId, pStrAnoMesReferencia)
                                                           .Where(Movimento => Movimento.AnoMesReferencia == pStrAnoMesReferencia)
                                                           .Where(Movimento => Movimento.Fornecedor.CpfCnpj == pStrCPF_CNPJ_Fornecedor)
                                                           .ToList();


            return lLstRetorno;
        }
        public List<MovimentoEntity> ListarEmpenhosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId, string pStrAnoMesReferencia, bool pBlnIgnorarAnoMesReferencia, string pStrCPF_CNPJ_Fornecedor)
        {
            List<MovimentoEntity> lLstRetorno = new List<MovimentoEntity>();

            if (pBlnIgnorarAnoMesReferencia || String.IsNullOrWhiteSpace(pStrAnoMesReferencia))
            {
                lLstRetorno = this.Service<IMovimentoService>().ListarMovimentosNaoLiquidados(pIntUgeId, pIntAlmoxarifadoId)
                                                               .Where(Movimento => Movimento.Fornecedor.CpfCnpj == pStrCPF_CNPJ_Fornecedor)
                                                               .ToList();
            }
            else
            {
                lLstRetorno = this.Service<IMovimentoService>().ListarMovimentosNaoLiquidados(pIntUgeId, pIntAlmoxarifadoId, pStrAnoMesReferencia)
                                                               .Where(Movimento => Movimento.AnoMesReferencia == pStrAnoMesReferencia)
                                                               .Where(Movimento => Movimento.Fornecedor.CpfCnpj == pStrCPF_CNPJ_Fornecedor)
                                                               .ToList();
            }
            return lLstRetorno;
        }

        public List<string> obterListaEmpenhos(int almoxID, string almoxAnoMesRef, bool listarLiquidados)
        {
            List<string> lstEmpenhos = new List<string>();


            if (listarLiquidados)
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lstEmpenhos = this.Service<IMovimentoService>().obterListaEmpenhosLiquidados(almoxID, almoxAnoMesRef)
                                                                       .ToList();
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
            else
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lstEmpenhos = this.Service<IMovimentoService>().obterListaEmpenhosNaoLiquidados(almoxID, almoxAnoMesRef)
                                                                       .ToList();
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

            return lstEmpenhos;
        }

        #endregion pseudo-mocks

        public IList<string> ListarEmpenhosFornecedor(int almoxID, int fornecedorID, string anoMesRef, bool empenhosNaoLiquidados = true)
        {
            var srvMovimento = this.Service<IMovimentoService>();

            IList<string> lstEmpenhos = null;
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstEmpenhos = srvMovimento.ListarEmpenhosFornecedor(almoxID, fornecedorID, anoMesRef, empenhosNaoLiquidados);
                    this.TotalRegistros = lstEmpenhos.Count;
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

            return lstEmpenhos;
        }
        public IList<string> ListarDocumentosEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            var srvMovimento = this.Service<IMovimentoService>();

            IList<string> lstDocumentos = null;
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstDocumentos = srvMovimento.ListarDocumentosEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
                    this.TotalRegistros = lstDocumentos.Count;
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

            return lstDocumentos;
        }
        public IList<MovimentoEntity> ListarMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            var srvMovimento = this.Service<IMovimentoService>();

            IList<MovimentoEntity> lstMovimentacoes = null;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstMovimentacoes = srvMovimento.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
                    this.TotalRegistros = lstMovimentacoes.Count;
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

            return lstMovimentacoes;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenhoAgrupadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            var srvMovimento = this.Service<IMovimentoService>();

            IList<MovimentoEntity> lstMovimentacoes = null;
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstMovimentacoes = srvMovimento.ListarMovimentacoesEmpenhoAgrupadas(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
                    this.TotalRegistros = lstMovimentacoes.Count;
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

            return lstMovimentacoes;
        }

        public MovimentoEntity ObterMovimento(int movID, bool isEmpenho = true)
        {
            MovimentoEntity objRetorno = null;

            using (TransactionScope transScope = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    var objBusiness = new MovimentoBusiness();

                    objRetorno = objBusiness.ObterMovimento(movID, isEmpenho);
                }
                catch (Exception excErro)
                {
                    throw excErro;
                }
                finally
                {
                    transScope.Complete();
                }
            }

            return objRetorno;
        }

        public Tuple<decimal, decimal> RecalculoQtdeLiqItemSiafisico(int iItemMaterialCodigo, int ugeID, string codigoEmpenho, int almoxID, int gestorID, int codigoNaturezaDespesa, string unidadeFornSiafem, bool consumoImediato)
        {
            IMovimentoItemService svcInfra = null;
            List<MovimentoItemEntity> lstMovItem = new List<MovimentoItemEntity>();

            IList<SubItemMaterialEntity> subitensMaterial = null;

            var qtdeLiquidoTotalItemMaterial = _valorZero;
            var saldoLiquidoTotalItemMaterial = _valorZero;
            subitensMaterial = new CatalogoBusiness().ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(iItemMaterialCodigo, ugeID, almoxID, gestorID, codigoNaturezaDespesa);

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    svcInfra = this.Service<IMovimentoItemService>();
                    foreach (var subitemMaterial in subitensMaterial)
                    {

                        lstMovItem = svcInfra.ListarSaldoQteDeItemMaterial(iItemMaterialCodigo,
                                                                                               subitemMaterial.Id.Value,
                                                                                               codigoEmpenho,
                                                                                               ugeID,
                                                                                               almoxID).ToList();

                        if (consumoImediato == true)
                        {
                            lstMovItem = lstMovItem.Where(x => x.Movimento.TipoMovimento.AgrupamentoId == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada || x.Movimento.TipoMovimento.AgrupamentoId == (int)GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato).ToList();
                        }
                        else
                        {
                            lstMovItem = lstMovItem.Where(x => x.Movimento.TipoMovimento.AgrupamentoId == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada).ToList();
                        }

                        if (!string.IsNullOrEmpty(unidadeFornSiafem))
                        {
                            var somaLiq = from p in lstMovItem
                                      .Where(a => a.FabricanteLote == unidadeFornSiafem)
                                          group p by 1 into g
                                          select new
                                          {
                                              SomaQtde = g.Sum(x => x.QtdeLiq).Value,
                                              SomaSaldo = g.Sum(x => x.ValorMov).Value
                                          };

                            if (somaLiq.FirstOrDefault() != null)
                            {
                                qtdeLiquidoTotalItemMaterial += somaLiq.FirstOrDefault().SomaQtde;
                                saldoLiquidoTotalItemMaterial += somaLiq.FirstOrDefault().SomaSaldo;
                            }

                        }
                        else
                        {
                            var somaLiq = from p in lstMovItem
                                          group p by 1 into g
                                          select new
                                          {
                                              SomaQtde = g.Sum(x => x.QtdeLiq).Value,
                                              SomaSaldo = g.Sum(x => x.ValorMov).Value
                                          };

                            if (somaLiq.FirstOrDefault() != null)
                            {
                                qtdeLiquidoTotalItemMaterial += somaLiq.FirstOrDefault().SomaQtde;
                                saldoLiquidoTotalItemMaterial += somaLiq.FirstOrDefault().SomaSaldo;
                            }
                        }


                        //if (!string.IsNullOrEmpty(unidadeFornSiafem))
                        //    saldoLiquidoTotalItemMaterial += lstMovItem.Where(a => a.FabricanteLote == unidadeFornSiafem).Sum(Somatoria => Somatoria.QtdeLiq).Value;
                        //else
                        //    saldoLiquidoTotalItemMaterial += lstMovItem.Sum(Somatoria => Somatoria.QtdeLiq).Value;
                    }
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

            return new Tuple<decimal, decimal>(qtdeLiquidoTotalItemMaterial, saldoLiquidoTotalItemMaterial);
            // return saldoLiquidoTotalItemMaterial;
        }

        public decimal RecalculoValorLiquidadoItemEmpenho(int iItemMaterialCodigo, int ugeID, string codigoEmpenho, int almoxID, int gestorID, int codigoNaturezaDespesa, string unidadeFornSiafem)
        {
            IMovimentoItemService svcInfra = null;
            List<MovimentoItemEntity> lstMovItem = new List<MovimentoItemEntity>();

            IList<SubItemMaterialEntity> subitensMaterial = null;

            var saldoValorLiquidadoTotalItemMaterialEmpenho = _valorZero;
            subitensMaterial = new CatalogoBusiness().ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(iItemMaterialCodigo, ugeID, almoxID, gestorID, codigoNaturezaDespesa);

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    svcInfra = this.Service<IMovimentoItemService>();
                    foreach (var subitemMaterial in subitensMaterial)
                    {

                        lstMovItem = svcInfra.ListarSaldoValorConsumoDeItemMaterial(iItemMaterialCodigo,
                                                                                    subitemMaterial.Id.Value,
                                                                                    codigoEmpenho,
                                                                                    ugeID,
                                                                                    almoxID).ToList();


                        if (!string.IsNullOrEmpty(unidadeFornSiafem))
                        {
                            var somatoriasValorLiquidado = from p in lstMovItem.Where(a => a.FabricanteLote == unidadeFornSiafem)
                                          group p by 1 into g
                                          select new { SomaSaldo = g.Sum(x => x.ValorMov).Value };

                            if (somatoriasValorLiquidado.HasElements())
                                saldoValorLiquidadoTotalItemMaterialEmpenho += somatoriasValorLiquidado.FirstOrDefault().SomaSaldo;

                        }
                        else
                        {
                            var somatoriasValorLiquidado = from p in lstMovItem
                                          group p by 1 into g
                                          select new { SomaSaldo = g.Sum(x => x.ValorMov).Value };

                            if (somatoriasValorLiquidado.HasElements())
                                saldoValorLiquidadoTotalItemMaterialEmpenho += somatoriasValorLiquidado.FirstOrDefault().SomaSaldo;
                        }
                    }
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

            return saldoValorLiquidadoTotalItemMaterialEmpenho;
        }


        public IList<MovimentoEntity> ListarMovimentacoesEmpenhoParticionadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados)
        {
            var srvMovimento = this.Service<IMovimentoService>();

            IList<MovimentoEntity> lstMovimentacoes = null;

            lstMovimentacoes = srvMovimento.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
            lstMovimentacoes = lstMovimentacoes.ParticionadorPorNumeroDocumento();

            this.TotalRegistros = lstMovimentacoes.Count;

            return lstMovimentacoes;
        }
    }

    #region Tipo Empenho
    public static partial class ExtensionMethods
    {
        public static string RetornarDescricaoEmpenho(this MovimentoEntity objMovimento)
        {
            string strDescricaoFormatada;

            object[] argsFormatacao = null;
            string tipoEmpenho = objMovimento.RetornarTipoEmpenho();
            string tipoLicitacaoEmpenho = objMovimento.RetornarTipoLicitacaoEmpenho();
            string fmtDescricao = null;

            if (!String.IsNullOrWhiteSpace(tipoEmpenho) && !String.IsNullOrWhiteSpace(tipoLicitacaoEmpenho))
                if (tipoEmpenho == tipoLicitacaoEmpenho)
                {
                    fmtDescricao = "{0}";
                    argsFormatacao = new object[] { tipoEmpenho };
                }
                else
                {
                    fmtDescricao = "{0} ({1})";
                    argsFormatacao = new object[] { tipoEmpenho, tipoLicitacaoEmpenho };
                }

            strDescricaoFormatada = String.Format(fmtDescricao, argsFormatacao);

            return strDescricaoFormatada;
        }

        public static bool IsTipoBEC(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_BEC);
        }
        public static bool IsTipoPregao(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_PREGAO_ELETRONICO);
        }
        public static bool IsTipoSiafisico(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_SIAFISICO);
        }

        private static string RetornarTipoEmpenho(this MovimentoEntity objMovimento)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");

            #region Variaveis
            int codigoEventoEmpenho = 0;
            int tipoLicitacaoEmpenho = 0;
            string strRetorno = string.Empty;
            bool eventoEmpenhoNulo = false;
            bool licitacaoEmpenhoNulo = false;
            #endregion Variaveis

            eventoEmpenhoNulo = (objMovimento.EmpenhoEvento.IsNull() || objMovimento.EmpenhoEvento.Codigo.IsNull());
            licitacaoEmpenhoNulo = (objMovimento.EmpenhoLicitacao.IsNull() || objMovimento.EmpenhoLicitacao.Id.IsNull());

            if ((!eventoEmpenhoNulo && licitacaoEmpenhoNulo) || (eventoEmpenhoNulo && !licitacaoEmpenhoNulo))
            {
                strRetorno = "NÃO FOI POSSÍVEL DEFINIR TIPO DE EMPENHO/LICITACAO.";
            }
            else
            {
                tipoLicitacaoEmpenho = (int)objMovimento.EmpenhoLicitacao.Id;
                codigoEventoEmpenho = (int)objMovimento.EmpenhoEvento.Codigo;
            }

            if (tipoLicitacaoEmpenho == (int)TipoLicitacaoEmpenho.Pregao)
            {
                strRetorno = "PREGAO";
            }
            else if (tipoLicitacaoEmpenho != (int)TipoLicitacaoEmpenho.Pregao)
            {
                codigoEventoEmpenho = (int)objMovimento.EmpenhoEvento.Codigo;

                switch (codigoEventoEmpenho)
                {
                    case (int)TipoEventoEmpenho.BEC: strRetorno = "BEC"; break;
                    case (int)TipoEventoEmpenho.DotacaoReservada: strRetorno = "SIAFISICO"; break;
                }
            }

            return strRetorno;
        }
        private static string RetornarTipoLicitacaoEmpenho(this MovimentoEntity objMovimento)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");


            #region Variaveis

            string strRetorno = string.Empty;
            bool eventoEmpenhoNulo = false;
            bool licitacaoEmpenhoNulo = false;
            #endregion Variaveis

            licitacaoEmpenhoNulo = (objMovimento.EmpenhoLicitacao.IsNull() || objMovimento.EmpenhoLicitacao.Id.IsNull());

            if ((!eventoEmpenhoNulo && licitacaoEmpenhoNulo) || (eventoEmpenhoNulo && !licitacaoEmpenhoNulo))
            {
                strRetorno = "NÃO FOI POSSÍVEL DEFINIR TIPO DE LICITACAO.";
            }
            else
            {
                strRetorno = GeralEnum.GetEnumDescription((TipoLicitacaoEmpenho)objMovimento.EmpenhoLicitacao.Id);
            }

            return strRetorno;
        }

        public static IList<MovimentoEntity> ParticionadorPorNumeroDocumento(this IList<MovimentoEntity> movimentosEmpenho, int numeroMaximoItens = 8)
        {
            IList<MovimentoItemEntity> listaParticionadaMovItem = new List<MovimentoItemEntity>();
            IList<MovimentoEntity> listaParticionada = new List<MovimentoEntity>();

            MovimentoEntity objEspelho = null;
            int _contadorMov = 0;

            int numElementos = movimentosEmpenho.Count;
            string[] arrNumerosDocumento = new string[] { };

            if (movimentosEmpenho.IsNotNullAndNotEmpty())
            {
                arrNumerosDocumento = movimentosEmpenho.Select(movEmpenho => movEmpenho.NumeroDocumento).ToArray();

                foreach (var numDocumento in arrNumerosDocumento)
                {
                    listaParticionadaMovItem = new List<MovimentoItemEntity>();
                    var lstMovItems = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento).SelectMany(movItem => movItem.MovimentoItem);

                    //objEspelho = null;
                    objEspelho = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento)
                                                  .Select<MovimentoEntity, MovimentoEntity>(movimentosEmpenho._instaciadorAuxiliarParticionadorDtoEmpenho())
                                                  .First();

                    foreach (MovimentoItemEntity movItem in lstMovItems)
                    {
                        listaParticionadaMovItem.Add(movItem);
                        if (listaParticionadaMovItem.Count == numeroMaximoItens)
                        {
                            objEspelho.MovimentoItem = listaParticionadaMovItem;

                            _contadorMov++;
                            objEspelho.Id = _contadorMov;
                            listaParticionada.Add(objEspelho);
                            listaParticionadaMovItem = new List<MovimentoItemEntity>(numeroMaximoItens);

                            objEspelho = null;
                        }
                    }

                    if (listaParticionadaMovItem.Count > 0)
                    {
                        objEspelho = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento)
                                                      .Select<MovimentoEntity, MovimentoEntity>(movimentosEmpenho._instaciadorAuxiliarParticionadorDtoEmpenho())
                                                      .First();

                        objEspelho.MovimentoItem = listaParticionadaMovItem;

                        _contadorMov++;
                        objEspelho.Id = _contadorMov;
                        listaParticionada.Add(objEspelho);

                        listaParticionadaMovItem = null;
                    }

                    objEspelho = null;
                }
            }

            listaParticionada = listaParticionada.Distinct().ToList();
            return listaParticionada;
        }

        /// <summary>
        /// Método interno a classe, auxiliar ao particionador de movimentações, customizado para a geração de estímulos para liquidação de empenhos.
        /// </summary>
        /// <param name="listaMovimentacoes"></param>
        /// <returns></returns>
        private static Func<MovimentoEntity, MovimentoEntity> _instaciadorAuxiliarParticionadorDtoEmpenho(this IList<MovimentoEntity> listaMovimentacoes)
        {
            Func<MovimentoEntity, MovimentoEntity> objRetorno;

            objRetorno = (movEmpenho => new MovimentoEntity()
            {
                NumeroDocumento = movEmpenho.NumeroDocumento,
                UGE = movEmpenho.UGE,
                Almoxarifado = movEmpenho.Almoxarifado,
                AnoMesReferencia = movEmpenho.AnoMesReferencia,
                DataMovimento = movEmpenho.DataMovimento,
                Empenho = movEmpenho.Empenho,
                EmpenhoEvento = movEmpenho.EmpenhoEvento,
                EmpenhoLicitacao = movEmpenho.EmpenhoLicitacao,
                NaturezaDespesaEmpenho = movEmpenho.NaturezaDespesaEmpenho,
                Observacoes = movEmpenho.Observacoes,
                ValorDocumento = movEmpenho.ValorDocumento,
                GeradorDescricao = movEmpenho.GeradorDescricao,
                Fornecedor = movEmpenho.Fornecedor,
                MovimentoItem = new List<MovimentoItemEntity>()
            });
            return objRetorno;
        }

    }
    #endregion Metodos Extensão

}
