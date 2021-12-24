using Newtonsoft.Json.Linq;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;
using Sam.Business;
using Sam.Domain.Entity.DtoWs;
using Sam.Common.Util;
using static Sam.Common.Util.GeralEnum;
using cstConstantes = Sam.Common.Util.Constante;
using oldInfra = Sam.Domain.Infrastructure;
using oldBusiness = Sam.Domain.Business;


namespace Sam.Integracao.ConsultaWs.Negocio
{

    public static class helperJSON_MovimentacoesWs
    {
        public static MovimentoEntity RequisicaoMaterialFromJSON(this string estimuloJSON)
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


            requisicaoMaterial = new MovimentoEntity() { TipoMovimento = new TipoMovimentoEntity() { Id = TipoMovimento.RequisicaoPendente.GetHashCode() },
                                                         DataDocumento = DateTime.Now,
                                                         DataMovimento = DateTime.Now,
                                                         DataOperacao = DateTime.Now,
                                                         ValorDocumento = 0.00m
                                                       };
            requisicaoMaterial.Ativo = true;
            requisicaoMaterial.IdLogin = ObterLoginUsuarioID_PorCPF(cpfUsuarioSAM);
            requisicaoMaterial.UGE = ObterRegistro<UGEEntity>(codigoUGE);
            requisicaoMaterial.UGE.CodigoDescricao = requisicaoMaterial.UGE.Codigo.ToString();
            requisicaoMaterial.Divisao = ObterDivisaoUA(codigoUA, codigoDivisaoUA);
            requisicaoMaterial.Almoxarifado = requisicaoMaterial.Divisao.Almoxarifado;
            requisicaoMaterial.AnoMesReferencia = requisicaoMaterial.Divisao.Almoxarifado.MesRef;
            requisicaoMaterial.GeradorDescricao = String.Format("{0} - {1}", requisicaoMaterial.Divisao.Codigo, requisicaoMaterial.Divisao.Descricao);
            requisicaoMaterial.Observacoes = observacoesRequisicaoMaterial;
            requisicaoMaterial.FonteRecurso = "";


            parsedItensRequisicaoJSON = parsedJSON["MovimentoItem"].Children().ToList();
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

            return loginID_UsuarioSAM; ;
        }
        public static DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA)
        {
            DivisaoEntity divisaoUA = null;
            oldBusiness.EstruturaOrganizacionalBusiness objBusiness = null;



            objBusiness = new oldBusiness.EstruturaOrganizacionalBusiness();
            divisaoUA = objBusiness.ObterUA(codigoUA, codigoDivisaoUA);
            return divisaoUA;
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

        public static Tuple<IList<string>, MovimentoEntity> consisteRelacaoItensRequisicao(string estimuloJSON)
        {
            List<string> relacaoInconsistencias = null;
            MovimentoEntity requisicaoMaterial = null;

            try
            {
                var parsedJSON = JObject.Parse(estimuloJSON);
                var relacaoItensRequisicaoMaterial = parsedJSON["MovimentoItem"].Children().ToList();
   


                relacaoInconsistencias = new List<string>();
                requisicaoMaterial = estimuloJSON.FromJSON();
                if (relacaoItensRequisicaoMaterial.HasElements())
                {
                    //consiste qtde. subitens na movimentacao
                    if (relacaoItensRequisicaoMaterial.Count() > cstConstantes.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO)
                    {
                        relacaoInconsistencias.Add(String.Format("NUMERO MAXIMO DE SUBITENS POR MOVIMENTACAO ({0}), EXCEDIDO!", cstConstantes.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                    }
                    else
                    {
                        {
                            //consiste Codigo Subitem repetido
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.GroupBy(itemMovimentacaoMaterial => new { codigoSubitemMaterial = itemMovimentacaoMaterial["codigoSubitemMaterial"] })
                                                                                          .Where(agrupamentoSubitemMaterial => agrupamentoSubitemMaterial.Count() > 1)
                                                                                          .Select(itemMovimentacaoMaterial => String.Format("SUBITEM {0} REQUISITADO MAIS DE UMA VEZ. PARA REQUISITAR NOVAMENTE O MESMO MATERIAL, UTILIZAR NOVA REQUISICAO DE MATERIAL.", itemMovimentacaoMaterial.Key.codigoSubitemMaterial))
                                                                                          .ToList());
                        }
                        {
                            //consiste Qtde Requisitada
                            decimal vlDecimal = -1.00m;
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => (itemMovimentacaoMaterial["qtdeMaterialRequisitada"].IsNull() || (!Decimal.TryParse(itemMovimentacaoMaterial["qtdeMaterialRequisitada"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out vlDecimal) || vlDecimal <= 0.00m)))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format("QTDE. INVALIDA INFORMADA, PARA SUBITEM {0}", itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToList());

                            vlDecimal = -1.00m;
                        }
                        {
                            //consiste PTRes
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => itemMovimentacaoMaterial["codigoPTRes"].IsNull())
                                                                                          .Select(itemMovimentacaoMaterial => String.Format("PTRES INFORMADO NAO-VALIDO, PARA SUBITEM {0} ({1})", itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToList());
                        }
                        {
                            //consiste Codigo Subitem obtido
                            long codigoSubitemRequisicao = -1;
                            long[] codigosSubitensRequisicaoEnviados = null;
                            long[] codigosSubitensRequisicaoExistentesCatalogo = null;

                            //Codigo Subitem invalido
                            relacaoInconsistencias.AddRange(relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => ((!Int64.TryParse(itemMovimentacaoMaterial["codigoSubitemMaterial"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out codigoSubitemRequisicao) || codigoSubitemRequisicao <= 0)))
                                                                                          .Select(itemMovimentacaoMaterial => String.Format("CODIGO INVALIDO DE SUBITEM INFORMADO. VALOR INFORMADO: '{0}'.", itemMovimentacaoMaterial["codigoSubitemMaterial"]))
                                                                                          .ToArray());

                            codigosSubitensRequisicaoEnviados = relacaoItensRequisicaoMaterial.Where(itemMovimentacaoMaterial => ((Int64.TryParse(itemMovimentacaoMaterial["codigoSubitemMaterial"].ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out codigoSubitemRequisicao) || codigoSubitemRequisicao > 0)))
                                                                                              .Select(itemMovimentacaoMaterial => codigoSubitemRequisicao)
                                                                                              .ToArray();

                            codigosSubitensRequisicaoExistentesCatalogo = requisicaoMaterial.MovimentoItem.Where(itemMovimentacaoMaterial => itemMovimentacaoMaterial.SubItemMaterial.IsNotNull())
                                                                                                          .Select(itemMovimentacaoMaterial => itemMovimentacaoMaterial.SubItemMaterial.Codigo)
                                                                                                          .ToArray();

                            relacaoInconsistencias.AddRange(codigosSubitensRequisicaoEnviados.Except(codigosSubitensRequisicaoExistentesCatalogo)
                                                                                             .Select(codigoSubitemMaterial => String.Format("SUBITEM {0} NAO EXISTENTE NO CATALOGO DO ALMOXARIFADO {1:D3}. FAVOR VERIFICAR MENSAGEM ENVIADA!", codigoSubitemMaterial, requisicaoMaterial.Almoxarifado.Codigo))
                                                                                             .ToList());
                        }
                    }
                }
                else
                {
                    relacaoInconsistencias.Add("ERRO AO PROCESSAR SOLICITACAO");
                }

            }
            catch (Exception excErroExecucao)
            {
                relacaoInconsistencias.Add("NAO FORAM ADICIONADOS SUBITENS DE MATERIAL A ESTA REQUISICAO DE MATERIAL");
            }

            return new Tuple<IList<string>, MovimentoEntity>(relacaoInconsistencias, requisicaoMaterial);
        }
    }
}
