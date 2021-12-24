using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;
using System.Data.Objects.SqlClient;
using System.Transactions;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using Sam.Common.Util;

namespace Sam.Infrastructure
{
    public class MovimentoInfrastructure : AbstractCrud<TB_MOVIMENTO, SAMwebEntities>
    {
        public List<TB_MOVIMENTO> ListarMovimentos()
        {
            var result = (from mov in this.Context.TB_MOVIMENTO
                          where mov.TB_MOVIMENTO_ID == 1
                          select mov).ToList();
            return result;
        }

        /// <summary>
        /// Lista os documentos para a lupa de pesquisa
        /// </summary>
        /// <param name="startIndex">index da paginação</param>
        /// <param name="palavraChave">Palavra chave para pesquisa</param>
        /// <param name="movimento">informações sobre o movimento</param>
        /// <returns></returns>
        public IList<TB_MOVIMENTO> PesquisaDocumentos(int startIndex, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {


            IQueryable<TB_MOVIMENTO> result;

            Context.ContextOptions.LazyLoadingEnabled = true;
            result = (from m in Context.TB_MOVIMENTO
                      join almox in Context.TB_ALMOXARIFADO on m.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                      select m).AsQueryable<TB_MOVIMENTO>();


            //Filtra o almoxarifado
            if (movimento.TB_ALMOXARIFADO_ID == 0)
                throw new Exception("O almoxarifado está nulo");

            //Filtra os documentos pelo Tipo de movimento
            if (movimento.TB_TIPO_MOVIMENTO_ID == 0)
            {
                throw new Exception("O tipo de movimento não foi informado.");
            }
            else if (movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
            {
                if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Nova))
                {
                    result = result.Where(a => a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia);
                    result = result.Where(a => a.TB_MOVIMENTO_ATIVO == true);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
                else
                {
                    result = result.Where(a => a.TB_ALMOXARIFADO_ID == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
            }
            else if (movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)
            {
                if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Nova))
                {
                    result = result.Where(a => a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao);
                    result = result.Where(a => a.TB_MOVIMENTO_ATIVO == true);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
                else
                {
                    result = result.Where(a => a.TB_ALMOXARIFADO_ID == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
            }
            else
            {
                result = result.Where(a => a.TB_ALMOXARIFADO_ID == movimento.TB_ALMOXARIFADO_ID);
                result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == movimento.TB_TIPO_MOVIMENTO_ID);
            }

            //Só pode estornar documentos do mês/ano ref
            if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Estorno))
                result = result.Where(a => a.TB_MOVIMENTO_ANO_MES_REFERENCIA == movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA);

            //documentos estornados ou ativos
            result = result.Where(a => a.TB_MOVIMENTO_ATIVO == movimento.TB_MOVIMENTO_ATIVO);

            //Filtra os movimentos pelo número do documento
            if (!String.IsNullOrEmpty(palavraChave))
                result = result.Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO.Contains(palavraChave));

            //Ordena a consulta
            result = result = result.OrderByDescending(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO);

            this.TotalRegistros = result.Count();
            if (result.Count() < startIndex)
                startIndex = 0;
            var retorno = Queryable.Take(Queryable.Skip(result, startIndex), 20).ToList();

            //Executa a Quryable com paginação
            return retorno;


        }
        private Int32 getMesReferenciaInicial(int pIntAlmoxarifadoFechamento)
        {
            Context.ContextOptions.LazyLoadingEnabled = true;
            IQueryable<TB_ALMOXARIFADO> lQryRetorno = null;
            lQryRetorno = (from amoxarifado in Context.TB_ALMOXARIFADO where amoxarifado.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoFechamento select amoxarifado).AsQueryable<TB_ALMOXARIFADO>();

            var retorno = lQryRetorno.Cast<TB_ALMOXARIFADO>().FirstOrDefault();


            try
            {
                return Convert.ToInt32(retorno.TB_ALMOXARIFADO_MES_REF_INICIAL.ToString());

            }
            catch
            {
                return 0;

            }
            finally
            {
                lQryRetorno = null;
            }


        }

        public IList<TB_MOVIMENTO> PesquisaDocumentosMesReferenciaBckp(int startIndex, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {
            IQueryable<TB_MOVIMENTO> result;


            Context.ContextOptions.LazyLoadingEnabled = true;
            result = (from m in Context.TB_MOVIMENTO
                      join almox in Context.TB_ALMOXARIFADO on m.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                      select m).AsQueryable<TB_MOVIMENTO>();


            //Filtra o almoxarifado
            if (movimento.TB_ALMOXARIFADO_ID == 0)
                throw new Exception("O almoxarifado está nulo");

            //Filtra os documentos pelo Tipo de movimento
            if (movimento.TB_TIPO_MOVIMENTO_ID == 0)
            {
                throw new Exception("O tipo de movimento não foi informado.");
            }
            else if (movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
            {
                if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Nova))
                {
                    result = result.Where(a => a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia);
                    result = result.Where(a => a.TB_MOVIMENTO_ATIVO == true);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
                else
                {
                    result = result.Where(a => a.TB_ALMOXARIFADO_ID == movimento.TB_ALMOXARIFADO_ID);
                    result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia);
                    result = result.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == null);
                }
            }
            else
            {
                result = result.Where(a => a.TB_ALMOXARIFADO_ID == movimento.TB_ALMOXARIFADO_ID);
                result = result.Where(a => a.TB_TIPO_MOVIMENTO_ID == movimento.TB_TIPO_MOVIMENTO_ID);
            }

            //Só pode estornar documentos do mês/ano ref
            if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Estorno))
                result = result.Where(a => a.TB_MOVIMENTO_ANO_MES_REFERENCIA == movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA);

            //documentos estornados ou ativos
            result = result.Where(a => a.TB_MOVIMENTO_ATIVO == movimento.TB_MOVIMENTO_ATIVO);

            //Filtra os movimentos pelo número do documento
            if (!String.IsNullOrEmpty(palavraChave))
                result = result.Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO.Contains(palavraChave));

            //Ordena a consulta
            result = result = result.OrderByDescending(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO);

            this.TotalRegistros = result.Count();

            //Executa a Quryable com paginação
            return Queryable.Take(Queryable.Skip(result, startIndex), 20).ToList();
        }

        /// <summary>
        /// Lista os documentos para a lupa de pesquisa
        /// </summary>
        /// <param name="startIndex">index da paginação</param>
        /// <param name="palavraChave">Palavra chave para pesquisa</param>
        /// <param name="movimento">informações sobre o movimento</param>
        /// <returns></returns>
        public IList<TB_MOVIMENTO> PesquisaDocumentosMesReferencia(int startIndex, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {
            var mesReferenciaInicial = getMesReferenciaInicial(movimento.TB_ALMOXARIFADO_ID);
            Context.ContextOptions.LazyLoadingEnabled = true;

            StringBuilder mSql = new StringBuilder();
            int tipoTransferencia = 0;

            mSql.Append("SELECT [t0].[TB_MOVIMENTO_ID], [t0].[TB_ALMOXARIFADO_ID], [t0].[TB_TIPO_MOVIMENTO_ID], [t1].[TB_ALMOXARIFADO_DESCRICAO] AS [TB_MOVIMENTO_OBSERVACOES], [t0].[TB_MOVIMENTO_GERADOR_DESCRICAO], [t0].[TB_MOVIMENTO_NUMERO_DOCUMENTO], [t0].[TB_MOVIMENTO_ANO_MES_REFERENCIA], [t0].[TB_MOVIMENTO_DATA_DOCUMENTO], [t0].[TB_MOVIMENTO_DATA_MOVIMENTO], [t0].[TB_MOVIMENTO_FONTE_RECURSO], [t0].[TB_MOVIMENTO_VALOR_DOCUMENTO], [t0].[TB_MOVIMENTO_OBSERVACOES], [t0].[TB_MOVIMENTO_INSTRUCOES], [t0].[TB_MOVIMENTO_EMPENHO], [t0].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO], [t0].[TB_MOVIMENTO_DATA_APROVACAO], [t0].[TB_FORNECEDOR_ID], [t0].[TB_DIVISAO_ID], [t0].[TB_UGE_ID], [t0].[TB_MOVIMENTO_ATIVO], [t0].[TB_EMPENHO_EVENTO_ID], [t0].[TB_MOVIMENTO_DATA_OPERACAO], [t0].[TB_LOGIN_ID], [t0].[TB_EMPENHO_LICITACAO_ID], [t0].[TB_LOGIN_ID_ESTORNO], [t0].[TB_MOVIMENTO_DATA_ESTORNO], [t0].[TB_MOVIMENTO_TRANSFERENCIA_ID],[t1].TB_ALMOXARIFADO_CODIGO,[t1].[TB_ALMOXARIFADO_DESCRICAO]");
            mSql.Append(" FROM [dbo].[TB_MOVIMENTO] AS [t0] WITH(NOLOCK) ");
            mSql.Append(" INNER JOIN [dbo].[TB_ALMOXARIFADO] AS [t1]  WITH(NOLOCK) ON [t0].[TB_ALMOXARIFADO_ID] = [t1].[TB_ALMOXARIFADO_ID]");
            mSql.Append(" WHERE ([t0].[TB_MOVIMENTO_ATIVO] =" + ((movimento.TB_MOVIMENTO_ATIVO == true) ? 1 : 0).ToString() + ") ");
            mSql.Append("  AND (NOT (EXISTS( SELECT NULL AS [EMPTY] ");
            mSql.Append("                   FROM [dbo].[TB_TRANSFERENCIA_PENDENTE] AS [t2] WITH(NOLOCK)");
            mSql.Append("                   WHERE [t2].[TB_MOVIMENTO_ID] = [t0].[TB_MOVIMENTO_ID]");
            mSql.Append("     ))) ");


            //Filtra o almoxarifado
            if (movimento.TB_ALMOXARIFADO_ID == 0)
                throw new Exception("O almoxarifado está nulo");

            //Filtra os documentos pelo Tipo de movimento
            if (movimento.TB_TIPO_MOVIMENTO_ID == 0)
            {
                throw new Exception("O tipo de movimento não foi informado.");
            }
            else if (movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
            {
                if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Nova))
                {
                    tipoTransferencia = (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia;

                    mSql.Append("   AND ([t0].[TB_TIPO_MOVIMENTO_ID] = " + tipoTransferencia.ToString() + ") ");
                    mSql.Append("   AND ([t0].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] = " + movimento.TB_ALMOXARIFADO_ID.ToString() + ")");

                }
                else
                {
                    tipoTransferencia = (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia;
                    mSql.Append("   AND ([t0].[TB_TIPO_MOVIMENTO_ID] = " + tipoTransferencia.ToString() + ") ");
                    mSql.Append("   AND ([t0].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] = " + movimento.TB_ALMOXARIFADO_ID.ToString() + ")");
                }
            }
            else if (movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)
            {
                if (tipoOperacao == (int)(Common.Util.GeralEnum.OperacaoEntrada.Nova))
                {
                    tipoTransferencia = (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao;

                    mSql.Append("   AND ([t0].[TB_TIPO_MOVIMENTO_ID] = " + tipoTransferencia.ToString() + ") ");
                    mSql.Append("   AND ([t0].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] = " + movimento.TB_ALMOXARIFADO_ID.ToString() + ")");

                }
                else
                {
                    tipoTransferencia = (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado;
                    mSql.Append("   AND ([t0].[TB_TIPO_MOVIMENTO_ID] = " + tipoTransferencia.ToString() + ") ");
                    mSql.Append("   AND ([t0].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] = " + movimento.TB_ALMOXARIFADO_ID.ToString() + ")");
                }
            }
            else
            {
                mSql.Append("   AND ([t0].[TB_TIPO_MOVIMENTO_ID] = " + movimento.TB_TIPO_MOVIMENTO_ID.ToString() + ") ");
                mSql.Append("   AND ([t0].[TB_ALMOXARIFADO_ID] = " + movimento.TB_ALMOXARIFADO_ID.ToString() + ") ");
            }

            //Filtra os movimentos pelo número do documento
            if (!String.IsNullOrEmpty(palavraChave))
                mSql.Append("   [t0].[TB_MOVIMENTO_NUMERO_DOCUMENTO] LIKE '%" + palavraChave + "%'");


            mSql.Append("   AND ([t0].[TB_MOVIMENTO_TRANSFERENCIA_ID] IS NULL) ");
            mSql.Append("   AND (CONVERT(Int,[t0].[TB_MOVIMENTO_ANO_MES_REFERENCIA]) > " + mesReferenciaInicial.ToString() + ") ");
            mSql.Append("   AND (CONVERT(Int,[t0].[TB_MOVIMENTO_ANO_MES_REFERENCIA])) <=" + movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA + "");
            mSql.Append(" ORDER BY [t0].[TB_MOVIMENTO_NUMERO_DOCUMENTO] DESC ");

            IList<TB_MOVIMENTO> listaRetorno = new List<TB_MOVIMENTO>();

            var result = Context.ExecuteStoreQuery<TB_MOVIMENTO>((mSql.ToString())).Cast<TB_MOVIMENTO>().ToList();


            foreach (TB_MOVIMENTO item in result)
            {
                TB_MOVIMENTO novo = new TB_MOVIMENTO();
                TB_ALMOXARIFADO amoNovo = new TB_ALMOXARIFADO();

                novo = item;
                amoNovo.TB_ALMOXARIFADO_CODIGO = item.TB_ALMOXARIFADO_ID;
                amoNovo.TB_ALMOXARIFADO_DESCRICAO = item.TB_MOVIMENTO_OBSERVACOES;

                novo.TB_ALMOXARIFADO = amoNovo;
                listaRetorno.Add(novo);

                amoNovo = null;
                novo = null;

            }

            result = null;


            this.TotalRegistros = listaRetorno.Count;

            var retorno = Queryable.Take(Queryable.Skip(listaRetorno.AsQueryable(), startIndex), 20).ToList();



            return retorno;

        }

        public IList<TB_MOVIMENTO_ITEM> ListarMovimentosRecalcular(int almoxarifadoID)
        {
            var result = (from m in Context.TB_MOVIMENTO_ITEM.ToList()
                          join m2 in Context.TB_MOVIMENTO on m.TB_MOVIMENTO_ID equals m2.TB_MOVIMENTO_ID
                          join t in Context.TB_TIPO_MOVIMENTO on m2.TB_TIPO_MOVIMENTO_ID equals t.TB_TIPO_MOVIMENTO_ID
                          join ta in Context.TB_TIPO_MOVIMENTO_AGRUPAMENTO on t.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals ta.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                          join s in Context.TB_SUBITEM_MATERIAL on m.TB_SUBITEM_MATERIAL_ID equals s.TB_SUBITEM_MATERIAL_ID
                          where m.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == almoxarifadoID
                          where (m.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                              || m.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          select new TB_MOVIMENTO_ITEM()
                          {
                              TB_MOVIMENTO_ITEM_SALDO_VALOR = m.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                              TB_MOVIMENTO_ITEM_SALDO_QTDE = m.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                              TB_MOVIMENTO_ITEM_QTDE_MOV = m.TB_MOVIMENTO_ITEM_QTDE_MOV,
                              TB_MOVIMENTO_ITEM_VALOR_MOV = m.TB_MOVIMENTO_ITEM_VALOR_MOV,
                              TB_MOVIMENTO = new TB_MOVIMENTO()
                              {
                                  TB_TIPO_MOVIMENTO = new TB_TIPO_MOVIMENTO()
                                  {
                                      TB_TIPO_MOVIMENTO_AGRUPAMENTO = m.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO
                                  },
                                  TB_ALMOXARIFADO = m.TB_MOVIMENTO.TB_ALMOXARIFADO
                              },
                          }).ToList();

            return result;
        }


        public void DeletarRecalculo(Int32 almoxarifadoId, Int64 subItemCodigo)
        {
            IQueryable<TB_MOVIMENTO_ITEM_RECALCULADO> query;

            if (subItemCodigo < 1)
                query = (from q in this.Context.TB_MOVIMENTO_ITEM_RECALCULADO join a in this.Context.TB_MOVIMENTO on q.TB_MOVIMENTO_ID equals a.TB_MOVIMENTO_ID select q).AsQueryable();
            else
                query = (from q in this.Context.TB_MOVIMENTO_ITEM_RECALCULADO
                         join a in this.Context.TB_MOVIMENTO on q.TB_MOVIMENTO_ID equals a.TB_MOVIMENTO_ID
                         join m in this.Context.TB_MOVIMENTO_ITEM on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                         join s in this.Context.TB_SUBITEM_MATERIAL on m.TB_SUBITEM_MATERIAL_ID equals s.TB_SUBITEM_MATERIAL_ID
                         where s.TB_SUBITEM_MATERIAL_CODIGO == subItemCodigo
                         select q).AsQueryable();


            var retorno = query.ToList();

            foreach (TB_MOVIMENTO_ITEM_RECALCULADO item in retorno)
            {
                this.Context.TB_MOVIMENTO_ITEM_RECALCULADO.DeleteObject(item);
            }

            this.Context.SaveChanges();

        }


        public void InserirRecalculo(MovimentoItemEntity entity)
        {

            this.Context.AddToTB_MOVIMENTO_ITEM_RECALCULADO(converterMovimentoItemToMovimentoRecalculado(entity));
            //this.Context.TB_MOVIMENTO_ITEM_RECALCULADO.AddObject(converterMovimentoItemToMovimentoRecalculado(entity));
            this.Context.SaveChanges();
        }

        private TB_MOVIMENTO_ITEM_RECALCULADO converterMovimentoItemToMovimentoRecalculado(MovimentoItemEntity entity)
        {
            TB_MOVIMENTO_ITEM_RECALCULADO retorno = new TB_MOVIMENTO_ITEM_RECALCULADO();
            retorno.TB_ALTERAR = false;
            retorno.TB_ITEM_MATERIAL_ID = (entity.ItemMaterial != null ? entity.ItemMaterial.Id : null);
            retorno.TB_LOGIN_ID = (entity.Movimento != null ? entity.Movimento.IdLogin : null);
            retorno.TB_LOGIN_ID_ESTORNO = (entity.Movimento != null ? entity.Movimento.IdLoginEstorno : null);
            retorno.TB_MOVIMENTO_ID = entity.Movimento.Id.Value;
            retorno.TB_MOVIMENTO_ITEM_ATIVO = true;
            retorno.TB_MOVIMENTO_ITEM_DATA_ESTORNO = null;
            retorno.TB_MOVIMENTO_ITEM_DATA_OPERACAO = null;
            retorno.TB_MOVIMENTO_ITEM_DESD = entity.Desd;
            retorno.TB_MOVIMENTO_ITEM_ID = entity.Id.Value;
            retorno.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = null;
            retorno.TB_MOVIMENTO_ITEM_LOTE_FABR = null;
            retorno.TB_MOVIMENTO_ITEM_LOTE_IDENT = null;
            retorno.TB_MOVIMENTO_ITEM_PRECO_UNIT = entity.PrecoUnit;
            retorno.TB_MOVIMENTO_ITEM_QTDE_LIQ = Convert.ToInt32(entity.QtdeLiq.Value);
            retorno.TB_MOVIMENTO_ITEM_QTDE_MOV = Convert.ToInt32(entity.QtdeMov);
            retorno.TB_MOVIMENTO_ITEM_SALDO_QTDE = Convert.ToInt32(entity.SaldoQtde);
            retorno.TB_MOVIMENTO_ITEM_SALDO_VALOR = entity.SaldoValor;
            retorno.TB_MOVIMENTO_ITEM_VALOR_MOV = entity.ValorMov;
            retorno.TB_SUBITEM_MATERIAL_ID = entity.SubItemMaterial.Id.Value;
            retorno.TB_UGE_ID = entity.UGE.Id.Value;

            return retorno;
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
                SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SAM_RETORNA_EXPORTACAO_DINAMICA_ANALITICA ", conexao);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@w_Orgao", w_OrgaoId);
                cmd.Parameters.AddWithValue("@r_Periodo", r_Periodo);
                cmd.Parameters.AddWithValue("@r_UGECodigo", r_UGECodigo);
                cmd.Parameters.AddWithValue("@r_UGEDescricao", r_UGEDescricao);
                cmd.Parameters.AddWithValue("@r_NaturezaDespesaCodigo", r_NaturezaDespesaCodigo);
                cmd.Parameters.AddWithValue("@r_NaturezaDespesaDescricao", r_NaturezaDespesaDescricao);
                cmd.Parameters.AddWithValue("@r_SubitemCodigo", r_SubitemCodigo);
                cmd.Parameters.AddWithValue("@r_SubitemDescricao", r_SubitemDescricao);
                cmd.Parameters.AddWithValue("@r_UnidadeFornecimento", r_UnidadeFornecimento);
                cmd.Parameters.AddWithValue("@r_DataMovimento", r_DataMovimento);
                cmd.Parameters.AddWithValue("@r_TipoMovimentoDescricao", r_TipoMovimentoDescricao);
                cmd.Parameters.AddWithValue("@r_SituacaoDescricao", r_SituacaoDescricao);
                cmd.Parameters.AddWithValue("@r_FornecedorDescricao", r_FornecedorDescricao);
                cmd.Parameters.AddWithValue("@r_NumeroDoc", r_NumeroDoc);
                cmd.Parameters.AddWithValue("@r_Lote", r_Lote);
                cmd.Parameters.AddWithValue("@r_VenctoDoc", r_VenctoDoc);
                cmd.Parameters.AddWithValue("@r_QtdeDoc", r_QtdeDoc);
                cmd.Parameters.AddWithValue("@r_ValorUnitario", r_ValorUnitario);

                cmd.Parameters.AddWithValue("@r_PrecoUnitario_EMP", r_PrecoUnitario_EMP);

                cmd.Parameters.AddWithValue("@r_Desdobro", r_Desdobro);
                cmd.Parameters.AddWithValue("@r_TotalDoc", r_TotalDoc);
                cmd.Parameters.AddWithValue("@r_QtdeSaldo", r_QtdeSaldo);
                cmd.Parameters.AddWithValue("@r_ValorSaldo", r_ValorSaldo);

                cmd.Parameters.AddWithValue("@r_UaCodigoRequisicao", r_UaCodigoRequisicao);
                cmd.Parameters.AddWithValue("@r_UaDescricaoRequisicao", r_UaDescricaoRequisicao);
                cmd.Parameters.AddWithValue("@r_Divisao", r_Divisao);
                cmd.Parameters.AddWithValue("@r_UGECodigoDestino", r_UGECodigoDestino);
                cmd.Parameters.AddWithValue("@r_UGEDescricaoDestino", r_UGEDescricaoDestino);

                cmd.Parameters.AddWithValue("@r_InformacoesComplementares", r_InformacoesComplementares);
                cmd.Parameters.AddWithValue("@r_NLConsumo", r_NLConsumo);
                cmd.Parameters.AddWithValue("@r_NLLiquidacao", r_NLLiquidacao);
                cmd.Parameters.AddWithValue("@r_NLLiquidacaoEstorno", r_NLLiquidacaoEstorno);

                cmd.Parameters.AddWithValue("@w_AnoMesRefDe", w_AnoMesRefDe);
                cmd.Parameters.AddWithValue("@w_AnoMesRefAte", w_AnoMesRefAte);

                cmd.Parameters.AddWithValue("@w_Fornecedor", w_Fornecedor);
                cmd.Parameters.AddWithValue("@w_LoteDescricao", w_LoteDescricao);
                cmd.Parameters.AddWithValue("@w_NaturezaDespesaCodigo", w_NaturezaDespesaCodigo);
                cmd.Parameters.AddWithValue("@w_NumeroDoc", w_NumeroDoc);
                cmd.Parameters.AddWithValue("@w_SaldoQuantidade", w_SaldoQuantidade);
                cmd.Parameters.AddWithValue("@w_SaldoValor", w_SaldoValor);
                cmd.Parameters.AddWithValue("@w_SubitemCodigo", w_SubitemCodigo);
                cmd.Parameters.AddWithValue("@w_TipoMovimentoId", w_TipoMovimentoId);
                cmd.Parameters.AddWithValue("@w_UgeCodigo", w_UgeCodigo);
                cmd.Parameters.AddWithValue("@w_NLConsumo", w_NLConsumo);
                cmd.Parameters.AddWithValue("@w_NLLiquidacao", w_NLLiquidacao);
                cmd.Parameters.AddWithValue("@w_NLLiquidacaoEstorno", w_NLLiquidacaoEstorno);

                cmd.Parameters.AddWithValue("@w_Ua_Id_Destino", w_UA_Id_Destino);
                cmd.Parameters.AddWithValue("@w_Uge_Id_Destino", w_UGE_Id_Destino);

                string teste;

                teste = ConvertCommandParamatersToLiteralValues(cmd);
                SqlDataAdapter daRetorno = new SqlDataAdapter(cmd);
                daRetorno.Fill(dsRetorno);

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
                SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SAM_RETORNA_EXPORTACAO_DINAMICA_SINTETICA ", conexao);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@r_UGECodigo", r_UGECodigo);
                cmd.Parameters.AddWithValue("@r_UGEDescricao", r_UGEDescricao);
                cmd.Parameters.AddWithValue("@r_GrupoMaterial", r_GrupoMaterial);
                cmd.Parameters.AddWithValue("@r_SubitemCodigo", r_SubitemCodigo);
                cmd.Parameters.AddWithValue("@r_SubitemDescricao", r_SubitemDescricao);
                cmd.Parameters.AddWithValue("@r_UnidadeFornecimento", r_UnidadeFornecimento);
                cmd.Parameters.AddWithValue("@r_QtdeSaldo", r_QtdeSaldo);
                cmd.Parameters.AddWithValue("@r_ValorSaldo", r_ValorSaldo);
                cmd.Parameters.AddWithValue("@r_PrecoMedio", r_PrecoMedio);
                cmd.Parameters.AddWithValue("@r_LoteDescricao", r_LoteDescricao);
                cmd.Parameters.AddWithValue("@r_LoteQtde", r_LoteQtde);
                cmd.Parameters.AddWithValue("@r_LoteDataVencimento", r_LoteDataVencimento);
                cmd.Parameters.AddWithValue("@r_NaturezaDespesaCodigo", r_NaturezaDespesaCodigo);
                cmd.Parameters.AddWithValue("@r_NaturezaDespesaDescricao", r_NaturezaDespesaDescricao);

                cmd.Parameters.AddWithValue("@w_Orgao", w_OrgaoId);
                cmd.Parameters.AddWithValue("@w_AnoMesRefDe", w_AnoMesRefDe);
                cmd.Parameters.AddWithValue("@w_AnoMesRefAte", w_AnoMesRefAte);
                cmd.Parameters.AddWithValue("@w_GrupoMaterial", w_GrupoMaterial);
                cmd.Parameters.AddWithValue("@w_UgeCodigo", w_UgeCodigo);
                cmd.Parameters.AddWithValue("@w_ComSemSaldo", w_ComSemSaldo);

                string teste;

                teste = ConvertCommandParamatersToLiteralValues(cmd);
                SqlDataAdapter daRetorno = new SqlDataAdapter(cmd);
                daRetorno.Fill(dsRetorno);

                return dsRetorno;
            }
            catch (Exception)
            {

                throw;
            }


        }

        private static string ConvertCommandParamatersToLiteralValues(SqlCommand cmd)
        {
            string query = cmd.CommandText;
            foreach (SqlParameter prm in cmd.Parameters)
            {
                switch (prm.SqlDbType)
                {
                    case SqlDbType.Bit:
                        int boolToInt = (bool)prm.Value ? 1 : 0;
                        query += Convert.ToString((bool)prm.Value ? 1 : 0) + ", ";
                        break;
                    case SqlDbType.Int:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                    case SqlDbType.VarChar:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                    default:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                }
            }
            return query;
        }
        public TB_UGE RetornarMovimentoAlmoxUge(string numeroDocumento)
        {
            IQueryable<TB_UGE> query = (from m in Context.TB_MOVIMENTO
                                        join mi in Context.TB_MOVIMENTO_ITEM on m.TB_MOVIMENTO_ID equals mi.TB_MOVIMENTO_ID
                                        join div in Context.TB_DIVISAO on m.TB_DIVISAO_ID equals div.TB_DIVISAO_ID
                                        join ua in Context.TB_UA on div.TB_UA_ID equals ua.TB_UA_ID
                                        join uge in Context.TB_UGE on ua.TB_UGE_ID equals uge.TB_UGE_ID
                                        where m.TB_MOVIMENTO_NUMERO_DOCUMENTO == numeroDocumento
                                        select uge).AsQueryable();

            return query.FirstOrDefault();
        }

        public TB_MOVIMENTO RetornarMovimentoSaidaFornecimento(int divisao, string numeroDocumento)
        {
            var query = (from m in Context.TB_MOVIMENTO
                         where m.TB_DIVISAO_ID == divisao &&
                               m.TB_MOVIMENTO_ATIVO == true &&
                               m.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoAprovada &&
                               m.TB_MOVIMENTO_NUMERO_DOCUMENTO == numeroDocumento

                         select m).FirstOrDefault();

            return query;
        }
    }
}
