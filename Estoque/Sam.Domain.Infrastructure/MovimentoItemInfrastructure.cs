using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using System.Linq.Expressions;
using System.Transactions;
using Sam.Common.Util;
using System.Data;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoLancamentoSIAFEM = Sam.Common.Util.GeralEnum.TipoLancamentoSIAF;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoAgrupamentoMovimento = Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento;
using IsolationLevel = System.Transactions.IsolationLevel;
using Sam.Common.LambdaExpression;
using Sam.Domain.Entity.DtoWs;
using Sam.Common.Enums;
using System.Data.Common;

namespace Sam.Domain.Infrastructure
{
    public partial class MovimentoItemInfrastructure : BaseInfraestructure, IMovimentoItemService
    {
        private const int codigoTipoMovimentoRequisicaoPendente = 20;
        private const int codigoTipoMovimentoRequisicaoFinalizada = 25;
        private const int codigoTipoMovimentoRequisicaoAprovada = 21;
        private const int codigoTipoMovimentoRequisicaoCancelada = 16;
        private const int codigoTipoMovimentoEmpenhoConsumoImediato = 40;
        private const int codigoTipoMovimentoRestosAPagarEmpenhoConsumoImediato = 41;

        public MovimentoItemEntity Entity { get; set; }

        public IList<MovimentoItemEntity> Listar()
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
                                                    select new MovimentoItemEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
                                                    Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                    DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                    Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                    FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                    IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                    Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                    PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                    QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                    QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                    SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                    SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                    SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                    SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoItemEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ITEM_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEntity> Listar(int MovimentoId)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    where a.TB_MOVIMENTO_ITEM_ID == MovimentoId
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
                                                    select new MovimentoItemEntity
                                                    {
                                                        Id = a.TB_MOVIMENTO_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoItemEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                   where a.TB_MOVIMENTO_ITEM_ID == MovimentoId
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEntity> ListarPorMovimento(MovimentoEntity mov)
        {
            IEnumerable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                          join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                          orderby a.TB_MOVIMENTO_ITEM_ID
                                                          select new MovimentoItemEntity
                                                          {
                                                              Id = a.TB_MOVIMENTO_ID,
                                                              Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                              DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                              Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                              FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                              IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                              Movimento = new MovimentoEntity
                                                              {
                                                                  Id = a.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                  Empenho = a.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                  NumeroDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                                              }

                                                              ,
                                                              PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                              QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                              QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                              SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                              SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                              SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                              SubItemMaterial = new SubItemMaterialEntity
                                                                  {
                                                                      Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                      Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                      CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,                                                               
                                                                      Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                      ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                                      join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                          on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                                      where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                                      select new ItemMaterialEntity
                                                                                      {
                                                                                          Id = i.TB_ITEM_MATERIAL_ID,
                                                                                          Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                          CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                          Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                                      }
                                                                                      ).FirstOrDefault(),
                                                                      UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                                              {
                                                                                                  Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                                  Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                                  Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                              },
                                                                      NaturezaDespesa = new NaturezaDespesaEntity() 
                                                                                                                    {
                                                                                                                        Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                                                                        Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                                                        Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                                                                    }
                                                                  },
                                                              UGE = new UGEEntity
                                                              {
                                                                  Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                                  Codigo = a.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                                                                  Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO,
                                                              },
                                                              ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                          })
                                                    .Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoItemEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                   join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                   where (m.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();

            if (mov.NumeroDocumento != null)
                resultado = resultado.Where(m => m.Movimento.NumeroDocumento == mov.NumeroDocumento);

            if (mov.Empenho != null)
                resultado = resultado.Where(m => m.Movimento.Empenho == mov.Empenho);

            return resultado.ToList();
        }

        public IList<MovimentoEntity> Imprimir(int MovimentoId)
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                where a.TB_MOVIMENTO_ID == MovimentoId
                                                orderby a.TB_MOVIMENTO_ID
                                                select new MovimentoEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
                                                    Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                    TipoMovimento = new TipoMovimentoEntity(a.TB_TIPO_MOVIMENTO_ID),
                                                    GeradorDescricao = a.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                    NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                    AnoMesReferencia = a.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                    DataDocumento = a.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                    DataMovimento = a.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                    FonteRecurso = a.TB_MOVIMENTO_FONTE_RECURSO,
                                                    ValorDocumento = a.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                    Observacoes = a.TB_MOVIMENTO_OBSERVACOES,
                                                    Instrucoes = a.TB_MOVIMENTO_INSTRUCOES,
                                                    Empenho = a.TB_MOVIMENTO_EMPENHO,
                                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where a.TB_MOVIMENTO_ID == MovimentoId
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEntity> ListarNotaRequisicao(int MovimentoId)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    where a.TB_MOVIMENTO.TB_MOVIMENTO_ID == MovimentoId
                                                    select new MovimentoItemEntity
                                                    {
                                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity
                                                        {
                                                            Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                            Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                            CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,                                                       
                                                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                            select new ItemMaterialEntity
                                                                            {
                                                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                            }
                                                                            ).FirstOrDefault(),
                                                            NaturezaDespesa = new NaturezaDespesaEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                            },
                                                            UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                            }
                                                        },
                                                        UGE = new UGEEntity
                                                        {
                                                            Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                            Codigo = a.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                                                            Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                                                        },
                                                        PTRes = new PTResEntity
                                                        {
                                                            Id = a.TB_PTRE.TB_PTRES_ID,
                                                            Codigo = a.TB_PTRE.TB_PTRES_CODIGO,
                                                            Descricao = a.TB_PTRE.TB_PTRES_DESCRICAO,

                                                            ProgramaTrabalho = new ProgramaTrabalho(a.TB_PTRE.TB_PTRES_PT_CODIGO)
                                                        },
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                        NL_Liquidacao = a.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO
                                                    })
                                            .ToList<MovimentoItemEntity>();

            return resultado;
        }

        public IList<MovimentoItemEntity> ListarTodosCod(int MovimentoId)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    where a.TB_MOVIMENTO.TB_MOVIMENTO_ID == MovimentoId
                                                    select new MovimentoItemEntity
                                                    {
                                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity
                                                        {
                                                            Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                            Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                            CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,                                                          
                                                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                            select new ItemMaterialEntity
                                                                            {
                                                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                            }
                                                                            ).FirstOrDefault(),
                                                            NaturezaDespesa = new NaturezaDespesaEntity
                                                                               {
                                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                               },
                                                            UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                                   {
                                                                                       Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                       Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                       Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                   }
                                                        }
                                                        ,
                                                        UGE = new UGEEntity
                                                        {
                                                            Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                            Codigo = a.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                                                            Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                                                        },
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                        NL_Liquidacao = a.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                                                        PTRes = (from b in this.Db.TB_PTREs
                                                                 //where a.TB_PTRES_ID == b.TB_PTRES_ID
                                                                 where a.TB_PTRES_CODIGO == b.TB_PTRES_CODIGO
                                                                 where a.TB_PTRES_PT_ACAO == b.TB_PTRES_PT_PROJ_ATV
                                                                 select new PTResEntity
                                                                 {
                                                                     Id = b.TB_PTRES_ID,
                                                                     Descricao = b.TB_PTRES_DESCRICAO,
                                                                     Codigo = b.TB_PTRES_CODIGO,
                                                                     ProgramaTrabalho = new ProgramaTrabalho(b.TB_PTRES_PT_CODIGO)
                                                                 }).First(),
                                                    })
                                           .ToList<MovimentoItemEntity>();

            this.totalregistros = resultado.Count();

            int num = 0;
            if (resultado.Count() > 0)
            {
                foreach (MovimentoItemEntity item in resultado)
                {
                    num++;
                    item.Posicao = num;
                }
            }

            return resultado;
        }

        public IList<MovimentoItemEntity> ImprimirMovimento()
        {
            return ImprimirMovimento(null, null);
        }

        /// <summary>
        /// Relatório Entrada Material
        /// </summary>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public IList<MovimentoItemEntity> ImprimirMovimento(DateTime? dataInicial, DateTime? dataFinal, bool? consultaTrasnf = false)
        {
            string _dataInicial = Convert.ToDateTime(dataInicial).ToString("yyyy-MM-dd HH':'mm':'ss");
            string _dataFinal = Convert.ToDateTime(dataFinal).ToString("yyyy-MM-dd HH':'mm':'ss");

            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                         select new MovimentoItemEntity
                                                         {
                                                             Id = a.TB_MOVIMENTO_ITEM_ID,
                                                             Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                             DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                             Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                             FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                             IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                             NL_Liquidacao = a.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                                                             NL_Reclassificacao = a.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO,
                                                             NL_Consumo = a.TB_MOVIMENTO_ITEM_NL_CONSUMO,
                                                             Movimento = new MovimentoEntity
                                                             {
                                                                 Id = a.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                 NumeroDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                 GeradorDescricao = a.TB_MOVIMENTO.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                                 ValorDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                                 Almoxarifado = new AlmoxarifadoEntity
                                                                 {
                                                                     Id = a.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                     Codigo = a.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                                                                     Descricao = a.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                 },
                                                                 TipoMovimento = new TipoMovimentoEntity
                                                                 {
                                                                     Id = a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                                     Codigo = a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                                     AgrupamentoId = a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                     Descricao = a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO
                                                                 },
                                                                 Fornecedor = new FornecedorEntity
                                                                 {
                                                                     Id = a.TB_MOVIMENTO.TB_FORNECEDOR.TB_FORNECEDOR_ID,
                                                                     CpfCnpj = a.TB_MOVIMENTO.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ,
                                                                     Nome = a.TB_MOVIMENTO.TB_FORNECEDOR.TB_FORNECEDOR_NOME.Trim()
                                                                 },
                                                                 Divisao = new DivisaoEntity
                                                                 {
                                                                     Id = a.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_ID,
                                                                     Descricao = a.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                                                                     Uf = new UFEntity
                                                                     {
                                                                         Id = a.TB_MOVIMENTO.TB_DIVISAO.TB_UF.TB_UF_ID,
                                                                         Sigla = a.TB_MOVIMENTO.TB_DIVISAO.TB_UF.TB_UF_SIGLA,
                                                                         Descricao = a.TB_MOVIMENTO.TB_DIVISAO.TB_UF.TB_UF_DESCRICAO,
                                                                     },
                                                                     Responsavel = new ResponsavelEntity
                                                                     {
                                                                         Id = a.TB_MOVIMENTO.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_ID,
                                                                         Descricao = a.TB_MOVIMENTO.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME
                                                                     }
                                                                 },
                                                                 Ativo = a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO,
                                                                 DataDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                 DataMovimento = a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                 DataOperacao = a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_OPERACAO,
                                                                 AnoMesReferencia = a.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                                 Observacoes = a.TB_MOVIMENTO.TB_MOVIMENTO_OBSERVACOES,
                                                                 Empenho = a.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                 MovimAlmoxOrigemDestino =
                                                                 (from i in Db.TB_ALMOXARIFADOs
                                                                  where i.TB_ALMOXARIFADO_ID == a.TB_MOVIMENTO.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO
                                                                  select
                                                                  new AlmoxarifadoEntity
                                                                  {
                                                                      Id = i.TB_ALMOXARIFADO_ID,
                                                                      Codigo = i.TB_ALMOXARIFADO_CODIGO,
                                                                      Descricao = i.TB_ALMOXARIFADO_DESCRICAO
                                                                  }).FirstOrDefault(),
                                                                 UGE = new UGEEntity
                                                                 {
                                                                     Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                                     Codigo = a.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                                                                     Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                                                                 }
                                                             },
                                                             PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                             QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                             QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                             SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                             SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                             SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                             SubItemMaterial = new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                 ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                                 join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                     on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                                 where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                                 select new ItemMaterialEntity
                                                                                 {
                                                                                     Id = i.TB_ITEM_MATERIAL_ID,
                                                                                     Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                     CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                     Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                                 }
                                                                                 ).FirstOrDefault(),
                                                                 NaturezaDespesa = new NaturezaDespesaEntity
                                                                 {
                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                 },
                                                                 UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                 {
                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                 }
                                                             }
                                                             ,
                                                             UGE = new UGEEntity
                                                             {
                                                                 Id = a.TB_UGE.TB_UGE_ID,
                                                                 Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                                 Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                                             },
                                                             ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                         }).AsQueryable();

            if (dataInicial.HasValue && dataFinal.HasValue)
            {
                resultado = resultado.Where(a => a.Movimento.DataMovimento >= Convert.ToDateTime(_dataInicial)).Where(a => a.Movimento.DataMovimento < Convert.ToDateTime(_dataFinal));
            }

            if (Entity.Movimento != null)
            {

                resultado = resultado.Where(a => a.Movimento.Ativo == true);

                if (Entity.Movimento.Id.HasValue)
                {
                    resultado = resultado.Where(a => a.Movimento.Id == Entity.Movimento.Id);
                }

                if (Entity.Movimento.TipoMovimento != null)
                {
                    if (Entity.Movimento.TipoMovimento.AgrupamentoId.HasValue)
                    {
                        if (Entity.Movimento.TipoMovimento.AgrupamentoId != 0)
                        {
                            resultado = resultado.Where(a => a.Movimento.TipoMovimento.AgrupamentoId == Entity.Movimento.TipoMovimento.AgrupamentoId);
                        }
                    }

                    if (Convert.ToBoolean(consultaTrasnf))
                        resultado = resultado.Where(a => a.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia || a.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia);
                }

                if (Entity.Movimento.Fornecedor != null && Entity.Movimento.Fornecedor.Id != 0)
                {
                    resultado = resultado.Where(a => a.Movimento.Fornecedor.Id == Entity.Movimento.Fornecedor.Id);
                }

                if (Entity.Movimento.UGE != null && Entity.Movimento.UGE.Id != 0)
                {
                    resultado = resultado.Where(a => a.Movimento.UGE.Id == Entity.Movimento.UGE.Id);
                }

                if (Entity.Movimento.Divisao != null && Entity.Movimento.Divisao.Id != 0)
                {
                    resultado = resultado.Where(a => a.Movimento.Divisao.Id == Entity.Movimento.Divisao.Id);
                }

                if (Entity.Movimento.NumeroDocumento != null && Entity.Movimento.NumeroDocumento != "")
                {
                    resultado = resultado.Where(a => a.Movimento.NumeroDocumento == Entity.Movimento.NumeroDocumento);
                }

                if (Entity.Movimento.Almoxarifado != null && Entity.Movimento.Almoxarifado.Id != 0)
                {
                    if (Convert.ToBoolean(consultaTrasnf))
                        resultado = resultado.Where(a => a.Movimento.Almoxarifado.Id == Entity.Movimento.Almoxarifado.Id);
                    else
                        resultado = resultado.Where(a => a.Movimento.Almoxarifado.Id == Entity.Movimento.Almoxarifado.Id);
                }


            }

            //ordena a lista
            IList<MovimentoItemEntity> result = (from a in resultado.ToList()
                                                 orderby a.TipoMovimentoId, a.NomeFornecedor, a.Movimento.NumeroDocumento, a.SubItemMaterial.Codigo
                                                 select a).ToList<MovimentoItemEntity>();

            if (result.Count > 0)
            {
                // truncado o valor unitário
                foreach (MovimentoItemEntity item in result)
                {
                    if (!item.PrecoUnit.HasValue)
                        item.PrecoUnit = 0;

                    //item.PrecoUnit = item.PrecoUnit.Value.Truncar(4, true);
                    item.PrecoUnit = ((item.PrecoUnit.HasValue) ? (item.PrecoUnit.Value.Truncar(4)) : 0.0000m);
                    item.Desd = ((item.Desd.HasValue) ? (item.Desd.Value.Truncar(4)) : 0.0000m);
                    item.ValorMov = ((item.ValorMov.HasValue) ? (item.ValorMov.Value.Truncar(4)) : 0.0000m);

                    item.TipoMovimentoDescricao = item.Movimento.TipoMovimento.Descricao;
                    if (Convert.ToBoolean(consultaTrasnf)) // Coloquei esse if pois não sei se isso pode interferir nos demais relatórios
                    {
                        item.ValorOriginalDocumento = item.Movimento.ValorDocumento;
                        item.DataMovimento = item.Movimento.DataOperacao;
                        if (item.Movimento != null)
                            item.DocumentoMovimento = item.Movimento.NumeroDocumento;
                    }


                }
            }

            return result;
        }


        const int CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO = 999;
        const int CST_CONSULTA_TODAS_AS_UGES_ORGAO = 999999;

        public IList<dtoWsMovimentacaoMaterial> GeraRelatorioMovimentacaoMaterialWs(int orgaoCodigo, int ugeCodigo, int almoxCodigo, int? uaCodigo, int? divisaoUaCodigo, int tipoMovimentacaoMaterialCodigo, int agrupamentoTipoMovimentacaoMaterialCodigo, string cpfCnpjFornecedor, DateTime dataInicial, DateTime dataFinal, bool? consultaTransf = false, int numeroPaginaConsulta = 0)
        {
            IList<dtoWsMovimentacaoMaterial> lstRetornoConsulta = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhereAdicional = null;
            int[] tiposMovimentacaoMaterialTransferencia = null;
            int[] codigosTipoMovimentacaoMaterialConsumoImediato = null;
            int[] codigosTipoRequisicaoMaterialConsultaNaoPermitida = null;
            int[] tiposMovimentacaoMaterial = null;
            int codigoTipoRequisicaoParaPesquisa = -1;

            //IQueryable<TB_MOVIMENTO> qryConsulta_Sombra = null;
            //Expression<Func<TB_MOVIMENTO, bool>> _expWhere_Sombra = null;

            //Filtro por Movimento/MovimentoItem Ativos
            //Filtro por DataInicial/DataFinal Relatorio
            //Filtro por CodigoAlmox e (CodigoOrgao + UG pertencente a UO pertencente a CodigoOrgao)
            //Filtro por CodigoUG Consultado



            if ((almoxCodigo != CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO) && (ugeCodigo != CST_CONSULTA_TODAS_AS_UGES_ORGAO))
            {
                expWhere = (movimentacaoMaterial => (movimentacaoMaterial.TB_MOVIMENTO_ATIVO == true)
                                                 && (movimentacaoMaterial.TB_MOVIMENTO_DATA_MOVIMENTO >= dataInicial && movimentacaoMaterial.TB_MOVIMENTO_DATA_MOVIMENTO <= dataFinal)
                                                 && (movimentacaoMaterial.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO == almoxCodigo &&
                                                    (movimentacaoMaterial.TB_ALMOXARIFADO.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO == orgaoCodigo && movimentacaoMaterial.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO == orgaoCodigo))
                                                 && (movimentacaoMaterial.TB_UGE.TB_UGE_CODIGO == ugeCodigo));
            }
            else if ((almoxCodigo == CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO) || (ugeCodigo == CST_CONSULTA_TODAS_AS_UGES_ORGAO) ||
                     (almoxCodigo != CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO) && (ugeCodigo == CST_CONSULTA_TODAS_AS_UGES_ORGAO) ||
                     (almoxCodigo == CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO) && (ugeCodigo != CST_CONSULTA_TODAS_AS_UGES_ORGAO))
            {
                expWhere = (movimentacaoMaterial => (movimentacaoMaterial.TB_MOVIMENTO_ATIVO == true)
                                                 && (movimentacaoMaterial.TB_MOVIMENTO_DATA_MOVIMENTO >= dataInicial && movimentacaoMaterial.TB_MOVIMENTO_DATA_MOVIMENTO <= dataFinal)
                                                 //&& (movimentacaoMaterial.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO == almoxCodigo &&
                                                 && (movimentacaoMaterial.TB_ALMOXARIFADO.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO == orgaoCodigo && movimentacaoMaterial.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO == orgaoCodigo)
                                                 //&& (movimentacaoMaterial.TB_UGE.TB_UGE_CODIGO == ugeCodigo));
                           );

                if (almoxCodigo != CST_CONSULTA_TODOS_OS_ALMOXARIFADOS_ORGAO)
                {
                    expWhereAdicional = (movimentacaoMaterial => movimentacaoMaterial.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO == almoxCodigo);
                    expWhere = expWhere.And(expWhereAdicional);
                }

                if (ugeCodigo != CST_CONSULTA_TODAS_AS_UGES_ORGAO)
                {
                    expWhereAdicional = (movimentacaoMaterial => movimentacaoMaterial.TB_UGE.TB_UGE_CODIGO == ugeCodigo);
                    expWhere = expWhere.And(expWhereAdicional);
                }
            }

            codigosTipoMovimentacaoMaterialConsumoImediato = new int[] { codigoTipoMovimentoEmpenhoConsumoImediato, codigoTipoMovimentoRestosAPagarEmpenhoConsumoImediato };
            codigosTipoRequisicaoMaterialConsultaNaoPermitida = new int[] { codigoTipoMovimentoRequisicaoCancelada, codigoTipoMovimentoRequisicaoPendente };

            //Se relacao consultada for de transferencia de materiais;
            if (consultaTransf.IsNotNull() && consultaTransf.HasValue && consultaTransf.Value)
            {
                tiposMovimentacaoMaterialTransferencia = new int[] { TipoMovimento.EntradaPorTransferencia.GetHashCode(), TipoMovimento.SaidaPorTransferencia.GetHashCode() };
                expWhereAdicional = (movimentacaoMaterial => tiposMovimentacaoMaterialTransferencia.Contains(movimentacaoMaterial.TB_TIPO_MOVIMENTO_ID));


                expWhere = expWhere.And(expWhereAdicional);
            }
            //Se for solicitado tipo generico de movimentacao de material, inves de tipo especifico
            else if (agrupamentoTipoMovimentacaoMaterialCodigo != 0)
            {
                tiposMovimentacaoMaterial = obterIDsTipoMovimentacaoMaterialPorAgrupamento(agrupamentoTipoMovimentacaoMaterialCodigo);
                expWhereAdicional = (movimentacaoMaterial => tiposMovimentacaoMaterial.Contains(movimentacaoMaterial.TB_TIPO_MOVIMENTO_ID));

                //Se TipoAgrupamento for para Consumo Imediato + UA especificada
                if (agrupamentoTipoMovimentacaoMaterialCodigo == TipoAgrupamentoMovimento.ConsumoImediato.GetHashCode() && (uaCodigo.IsNotNull() && uaCodigo.Value > 0))
                    expWhereAdicional = (movimentacaoMaterial => (movimentacaoMaterial.TB_UA.TB_UA_CODIGO == uaCodigo) && (tiposMovimentacaoMaterial.Contains(movimentacaoMaterial.TB_TIPO_MOVIMENTO_ID)));


                expWhere = expWhere.And(expWhereAdicional);
            }
            //Se for solicitado tipo especifico
            else if ((agrupamentoTipoMovimentacaoMaterialCodigo == 0) &&
                     //(tipoMovimentacaoMaterialCodigo != TipoMovimento.RequisicaoCancelada.GetHashCode() || tipoMovimentacaoMaterialCodigo != TipoMovimento.RequisicaoPendente.GetHashCode()))
                     //(tipoMovimentacaoMaterialCodigo != codigoTipoMovimentoRequisicaoCancelada || tipoMovimentacaoMaterialCodigo != codigoTipoMovimentoRequisicaoPendente))
                     (!codigosTipoRequisicaoMaterialConsultaNaoPermitida.Contains(tipoMovimentacaoMaterialCodigo)))
            {
                //Se relacao consultada for de saida por requisicao;
                if (tipoMovimentacaoMaterialCodigo == codigoTipoMovimentoRequisicaoFinalizada)
                {
                    codigoTipoRequisicaoParaPesquisa = codigoTipoMovimentoRequisicaoAprovada;


                    //Se DivisaoUA foi especificada
                    if (divisaoUaCodigo.HasValue && divisaoUaCodigo.Value > 0)
                        expWhereAdicional = (requisicaoMaterial => (requisicaoMaterial.TB_DIVISAO.TB_UA.TB_UA_CODIGO == uaCodigo && requisicaoMaterial.TB_DIVISAO.TB_DIVISAO_CODIGO == divisaoUaCodigo)
                                                                && (requisicaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == codigoTipoRequisicaoParaPesquisa));

                    //Se DivisaoUA nao foi especificada
                    else if (divisaoUaCodigo.IsNull() || (divisaoUaCodigo.HasValue && divisaoUaCodigo.Value == 0))
                        //expWhereAdicional = (requisicaoMaterial => (requisicaoMaterial.TB_DIVISAO.TB_UA.TB_UA_CODIGO == uaCodigo) && (requisicaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == codigoTipoMovimentoRequisicaoFinalizada));
                        expWhereAdicional = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == codigoTipoRequisicaoParaPesquisa));




                    //_expWhere_Sombra = expWhere;
                    //_expWhere_Sombra = _expWhere_Sombra.And(requisicaoMaterial02 => requisicaoMaterial02.TB_TIPO_MOVIMENTO_ID == codigoTipoMovimentoRequisicaoAprovada);
                }
                //Se relacao consultada for de consumo imediato;
                else if (codigosTipoMovimentacaoMaterialConsumoImediato.Contains(tipoMovimentacaoMaterialCodigo))
                {
                    expWhereAdicional = (movimentacaoMaterial => (movimentacaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == tipoMovimentacaoMaterialCodigo));

                    //Se UA foi especificada
                    if (uaCodigo.IsNotNull() && uaCodigo.Value > 0)
                        expWhereAdicional = (movimentacaoMaterial => (movimentacaoMaterial.TB_UA.TB_UA_CODIGO == uaCodigo) && (movimentacaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == tipoMovimentacaoMaterialCodigo));

                }
                //Se relacao consultada for de outros tipos (entrada/saida);
                else
                {
                    expWhereAdicional = (movimentacaoMaterial => (movimentacaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO == tipoMovimentacaoMaterialCodigo));
                }

                
                expWhere = expWhere.And(expWhereAdicional);
            }

            //Se fornecedor (CPF/CNPJ) for especificado
            if (!String.IsNullOrWhiteSpace(cpfCnpjFornecedor))
            {
                long iCNPJ_CPF = -1;

                expWhere = expWhere.And(expWhereAdicional);
                if (Int64.TryParse(cpfCnpjFornecedor, out iCNPJ_CPF))
                    expWhereAdicional = (movimentacaoMaterial => movimentacaoMaterial.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ.Contains(iCNPJ_CPF.ToString()));
                else //if (iCNPJ_CPF == 0 && !String.IsNullOrWhiteSpace(cpfCnpjFornecedor))
                    expWhereAdicional = (movimentacaoMaterial => movimentacaoMaterial.TB_FORNECEDOR.TB_FORNECEDOR_NOME.Contains(cpfCnpjFornecedor));

                expWhere = expWhere.And(expWhereAdicional);
            }

            qryConsulta = Db.TB_MOVIMENTOs.Where(expWhere)
                                          .OrderBy(movimentacaoMaterial => movimentacaoMaterial.TB_TIPO_MOVIMENTO_ID)
                                          .ThenBy(movimentacaoMaterial => movimentacaoMaterial.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ)
                                          .ThenBy(movimentacaoMaterial => movimentacaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                          //.ThenBy(movimentacaoMaterial => movimentacaoMaterial.TB_MOVIMENTO_ITEMs.OrderBy(itemMovimentacaoMaterial => itemMovimentacaoMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO))
                                          .AsQueryable();

            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<DbParameter>().Reverse().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            try
            {
                int numeroRegistros = qryConsulta.Count();
                if (numeroRegistros > Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS && ((numeroPaginaConsulta * Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS) < numeroRegistros))
                    qryConsulta = Queryable.Take(Queryable.Skip(qryConsulta, numeroPaginaConsulta * Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS), Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS);


                lstRetornoConsulta = qryConsulta.Select(MovimentoInfrastructure._instanciadorDtoWsMovimentacaoMaterial())
                                                .ToList();

                base.totalregistros = numeroRegistros;
                base.SkipRegistros = Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS;

                return lstRetornoConsulta;
            }
            catch (Exception excErroExecucao)
            {
                var msgErroExecucao = (excErroExecucao.InnerException.IsNotNull() ? String.Format("Exception Gerada: {0}, Mensagem: {1}, Detalhe: {1}, StackTrace: {2}", excErroExecucao.GetType().ToString(), excErroExecucao.Message, excErroExecucao.InnerException.Message, excErroExecucao.StackTrace) : String.Format("Exception Gerada: {0}, Mensagem: {1}, StackTrace: {2}", excErroExecucao.GetType().ToString(), excErroExecucao.Message, excErroExecucao.StackTrace));
                var logInfra = new LogErroInfraestructure();
                logInfra.GravarLogErro(excErroExecucao);
                logInfra.GravarMsgInfo("Exception @MovimentoItemInfrastructure.GeraRelatorioMovimentacaoMaterialWs", msgErroExecucao);
                throw excErroExecucao;
                //return null;
            }
        }

        public TB_ALMOXARIFADO obterAlmoxarifado(int codigoOrgao, int codigoAlmox)
        {
            TB_ALMOXARIFADO rowTabela = null;

            rowTabela = Db.TB_ALMOXARIFADOs.Where(almoxEstoque => almoxEstoque.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO == codigoOrgao
                                                               && almoxEstoque.TB_ALMOXARIFADO_CODIGO == codigoAlmox)
                                           .FirstOrDefault();

            return rowTabela;
        }
        static internal int[] obterIDsTipoMovimentacaoMaterialPorAgrupamento(int agrupamentoTipoMovimentacaoMaterialCodigo)
        {
            //dbSawDataContext _mDb = null;
            IQueryable<TB_TIPO_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_TIPO_MOVIMENTO, bool>> expWhere = null;
            //int[] IDsTipoMovimentacaoMaterial = null;
            List<int> IDsTipoMovimentacaoMaterial = null;


            //_mDb = new dbSawDataContext();
            var infraTabelaMovimentos = new MovimentoInfrastructure();
            var _mDb = infraTabelaMovimentos.Db;
            qryConsulta = _mDb.TB_TIPO_MOVIMENTOs.AsQueryable();
            switch (agrupamentoTipoMovimentacaoMaterialCodigo)
            {
                case (int)TipoAgrupamentoMovimento.Entrada:
                case (int)TipoAgrupamentoMovimento.Saida:
                case (int)TipoAgrupamentoMovimento.ConsumoImediato:
                                                                    {
                                                                        expWhere = (tiposMovimentacaoMaterial => tiposMovimentacaoMaterial.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == agrupamentoTipoMovimentacaoMaterialCodigo);
                                                                        IDsTipoMovimentacaoMaterial = qryConsulta.Where(expWhere)
                                                                                                                 .Select(tipoMovimentacaoMaterial => tipoMovimentacaoMaterial.TB_TIPO_MOVIMENTO_ID)
                                                                                                                 .ToList();

                                                                        if (IDsTipoMovimentacaoMaterial.Contains(TipoMovimento.RequisicaoAprovada.GetHashCode()))
                                                                            {
                                                                                IDsTipoMovimentacaoMaterial.Remove(TipoMovimento.RequisicaoAprovada.GetHashCode());
                                                                                IDsTipoMovimentacaoMaterial.Add(TipoMovimento.RequisicaoFinalizada.GetHashCode());
                                                                            }

                                                                    }
                                                                    break;
                case (int)TipoAgrupamentoMovimento.Requisicao:
                                                                    {
                                                                        IDsTipoMovimentacaoMaterial = new List<int>() { TipoMovimento.RequisicaoPendente.GetHashCode() };
                                                                    }
                                                                    break;

                case (int)TipoAgrupamentoMovimento.Todos:
                                                                    {
                                                                        IDsTipoMovimentacaoMaterial = new List<int>();
                                                                        //IDsTipoMovimentacaoMaterial = new int[] { };

                                                                        foreach (var tipoAgrupamento in Enum.GetValues(typeof(TipoAgrupamentoMovimento))
                                                                                                            .Cast<TipoAgrupamentoMovimento>()
                                                                                                            .Where(tipoAgrupamentoMovimentacaoMaterialCodigo => (tipoAgrupamentoMovimentacaoMaterialCodigo != TipoAgrupamentoMovimento.Todos)
                                                                                                                                                             //&& (tipoAgrupamentoMovimentacaoMaterialCodigo != TipoAgrupamentoMovimento.Requisicao)
                                                                                                                  )
                                                                                                            .ToList())
                                                                            //IDsTiposMovimentacaoMaterial.AddRange(obterIDsTipoMovimentacaoMaterialPorAgrupamento(tipoAgrupamento.GetHashCode()));
                                                                            IDsTipoMovimentacaoMaterial.AddRange(obterIDsTipoMovimentacaoMaterialPorAgrupamento(tipoAgrupamento.GetHashCode()));
                                                                    }
                                                                    break;
            }

            infraTabelaMovimentos = null;
            _mDb = null;
            return IDsTipoMovimentacaoMaterial.ToList().OrderBy(tipoMovimentacaoMaterialID => tipoMovimentacaoMaterialID).ToArray();
        }

        static internal Func<TB_MOVIMENTO_ITEM, dtoWsItemMovimentacaoMaterial> _instanciadorDtoWsItemMovimentacaoMaterial()
        {
            var tiposMovimentacaoMaterialTransferencia = new int[] { TipoMovimento.EntradaPorTransferencia.GetHashCode(), TipoMovimento.SaidaPorTransferencia.GetHashCode() };
            var tipoMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoAprovada.GetHashCode() };
            Func<TB_MOVIMENTO_ITEM, dtoWsItemMovimentacaoMaterial> _actionSeletor = null;

            _actionSeletor = (rowTabela => new dtoWsItemMovimentacaoMaterial()
                                                                                {
                                                                                    Codigo = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                                    Descricao = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                                    UnidadeFornecimento = String.Format("{0} - {1}", rowTabela.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, rowTabela.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO),
                                                                                    NaturezaDespesa = String.Format("{0:D8} - {1}", rowTabela.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO, rowTabela.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO),

                                                                                    Qtde_ItemMovimentacaoMaterialSolicitada = ((tipoMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_MOVIMENTO.TB_TIPO_MOVIMENTO_ID) && rowTabela.TB_MOVIMENTO_ITEM_QTDE_LIQ.IsNotNull()) ? rowTabela.TB_MOVIMENTO_ITEM_QTDE_LIQ.Value : 0.00m),
                                                                                    Qtde_ItemMovimentacaoMaterial = (rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV.IsNotNull() ? (rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV.Value.Truncar(3) as decimal?) : null),
                                                                                    Valor_ItemMovimentacaoMaterial = (rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV.IsNotNull() ? (rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV.Value.Truncar(2) as decimal?) : null),
                                                                                    PrecoUnitario = ((rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV.IsNotNull() && rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV.IsNotNull()) ? (rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV.Value / rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV.Value).Truncar(4) as decimal? : null),
                                                                                    PrecoMedio = (rowTabela.TB_MOVIMENTO_ITEM_PRECO_UNIT.IsNotNull() ? (rowTabela.TB_MOVIMENTO_ITEM_PRECO_UNIT.Value.Truncar(4) as decimal?) : null),

                                                                                    Desdobro = (rowTabela.TB_MOVIMENTO_ITEM_DESD.IsNotNull() ? (rowTabela.TB_MOVIMENTO_ITEM_DESD.Value.Truncar(4) as decimal?) : null),

                                                                                    Fabricante_Lote = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_LOTE_FABR)? rowTabela.TB_MOVIMENTO_ITEM_LOTE_FABR : null),
                                                                                    ID_Lote = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_LOTE_IDENT)? rowTabela.TB_MOVIMENTO_ITEM_LOTE_IDENT : null),
                                                                                    DataVencimento_Lote = (rowTabela.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.IsNotNull() ? (rowTabela.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC) : null as DateTime?),

                                                                                    NL_Consumo = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_NL_CONSUMO)? rowTabela.TB_MOVIMENTO_ITEM_NL_CONSUMO : null),
                                                                                    NL_Liquidacao = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO)? rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO : null),
                                                                                    NL_Reclassificacao = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO)? rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO : null),
                                                                                    //NL_Estorno_Liquidacao = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO)? rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO : null),
                                                                                    //NL_Estorno_Reclassificacao = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO)? rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO : null),

                                                                                    //DataMovimento = ((tiposMovimentacaoMaterialTransferencia.Contains(rowTabela.TB_MOVIMENTO.TB_TIPO_MOVIMENTO_ID)) ? ((rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_DATA_OPERACAO.IsNotNull())?rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_DATA_OPERACAO : null): rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO),
                                                                                    DataMovimento = rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO
                                                                                });

            return _actionSeletor;
        }
        static internal EntitySet<TB_MOVIMENTO_ITEM> processarItensMovimentacao(TB_MOVIMENTO rowTabela)
        {
            EntitySet<TB_MOVIMENTO_ITEM> itensMovimentacao = null;


            itensMovimentacao = rowTabela.TB_MOVIMENTO_ITEMs;
            return itensMovimentacao;
        }

        public IList<MovimentoEntity> ListarTodosCod()
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                orderby a.TB_MOVIMENTO_ID
                                                select new MovimentoEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
                                                    Almoxarifado = new AlmoxarifadoEntity
                                                    {
                                                        Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                        Descricao = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                    },
                                                    TipoMovimento = new TipoMovimentoEntity
                                                    {
                                                        Id = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                        Descricao = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                        AgrupamentoId = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                    },
                                                    GeradorDescricao = a.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                    NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                    AnoMesReferencia = a.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                    DataDocumento = a.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                    DataMovimento = a.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                    FonteRecurso = a.TB_MOVIMENTO_FONTE_RECURSO,
                                                    ValorDocumento = a.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                    Observacoes = a.TB_MOVIMENTO_OBSERVACOES,
                                                    Instrucoes = a.TB_MOVIMENTO_INSTRUCOES,
                                                    Empenho = a.TB_MOVIMENTO_EMPENHO,
                                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEntity> Imprimir()
        {
            return new List<MovimentoItemEntity>();
        }

        public void Excluir()
        {
        }

        public void Salvar()
        {
            TB_MOVIMENTO_ITEM mov = new TB_MOVIMENTO_ITEM();

            if (this.Entity.Id.HasValue)
                mov = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ITEM_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(mov);

            mov.TB_MOVIMENTO_ITEM_ATIVO = this.Entity.Ativo;
            mov.TB_MOVIMENTO_ITEM_DESD = this.Entity.Desd;
            mov.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = this.Entity.DataVencimentoLote;
            mov.TB_MOVIMENTO_ITEM_LOTE_FABR = this.Entity.FabricanteLote;
            mov.TB_MOVIMENTO_ITEM_LOTE_IDENT = this.Entity.IdentificacaoLote;
            mov.TB_MOVIMENTO_ITEM_PRECO_UNIT = this.Entity.PrecoUnit;
            mov.TB_MOVIMENTO_ITEM_QTDE_LIQ = this.Entity.QtdeLiq;
            mov.TB_MOVIMENTO_ITEM_QTDE_MOV = this.Entity.QtdeMov;
            mov.TB_MOVIMENTO_ITEM_SALDO_QTDE = this.Entity.SaldoQtde;
            mov.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = this.Entity.SaldoQtdeLote;
            mov.TB_MOVIMENTO_ITEM_SALDO_VALOR = this.Entity.SaldoValor;
            mov.TB_MOVIMENTO_ITEM_VALOR_MOV = this.Entity.ValorMov;
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_MOVIMENTO_ITEMs
                .Where(a => a.TB_MOVIMENTO_ID == this.Entity.Movimento.Id.Value)
                .Where(a => a.TB_MOVIMENTO_ITEM_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_MOVIMENTO_ITEMs
                .Where(a => a.TB_MOVIMENTO_ID == this.Entity.Movimento.Id.Value)
                .Count() > 0;
            }
            return retorno;
        }

        public MovimentoItemEntity LerRegistroItem(int MovimentoId)
        {
            MovimentoItemEntity resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                             where a.TB_MOVIMENTO_ITEM_ID == MovimentoId
                                             select new MovimentoItemEntity
                                             {
                                                 Id = a.TB_MOVIMENTO_ID,
                                                 Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                 DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                 Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                 FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                 Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                 QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                 QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                 SubItemMaterial = new SubItemMaterialEntity
                                                 {
                                                     Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                     CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,                                                 
                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                     ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                     join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                         on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                     where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                     select new ItemMaterialEntity
                                                                     {
                                                                         Id = i.TB_ITEM_MATERIAL_ID,
                                                                         Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                         CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                         Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                     }
                                                                     ).FirstOrDefault(),
                                                     UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                     {
                                                         Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                         Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                         Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                     }
                                                 }
                                                 ,
                                                 UGE = new UGEEntity
                                                 {
                                                     Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                     Codigo = a.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                                                     Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                                                 },
                                                 ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                             }).FirstOrDefault();

            return resultado;
        }

        public IList<MovimentoItemEntity> ListarPorMovimentoTodos(MovimentoEntity mov)
        {
            int tipoMovimentoId = 0;
            if (mov.TipoMovimento != null)
                tipoMovimentoId = mov.TipoMovimento.Id;

            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    join saldoSubItem in Db.TB_SALDO_SUBITEMs.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals saldoSubItem.TB_SUBITEM_MATERIAL_ID
                                                    where (a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                                    where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID == mov.TipoMovimento.Id)
                                                    where (a.TB_MOVIMENTO_ITEM_ATIVO == true)
                                                    orderby a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO
                                                    select new MovimentoItemEntity
                                                    {
                                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                        SaldoValor = saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity
                                                        {
                                                            Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                            Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                            CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                            select new ItemMaterialEntity
                                                                            {
                                                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                            }
                                                                            ).FirstOrDefault(),
                                                            NaturezaDespesa = new NaturezaDespesaEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                            },
                                                            UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                            }
                                                        }
                                                        ,
                                                        UGE = new UGEEntity
                                                            {
                                                                Id = a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                                                                Descricao = a.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                                                            },
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).ToList();

            return resultado;

        }

        public Tuple<decimal?, decimal?, decimal?> SaldoMovimentoItemDataMovimento(int? idSubItem, int? idAlmoxarifado, int? idUge, DateTime dataMovimento)
        {
            decimal? Qtde = 0;
            decimal? Valor = 0;
            decimal? PrecoUnit = 0;

            var resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             //join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                             where a.TB_SUBITEM_MATERIAL_ID == idSubItem
                           //  where m.TB_MOVIMENTO_DATA_MOVIMENTO<= dataMovimento
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == idAlmoxarifado
                             where a.TB_UGE_ID == idUge
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             //where a.TB_MOVIMENTO_ITEM_LOTE_IDENT == lote 
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                 PrecoUnitDtMov = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                 Movimento = (from m in Db.TB_MOVIMENTOs
                                              where a.TB_MOVIMENTO_ID == m.TB_MOVIMENTO_ID
                                              select new MovimentoEntity
                                              {
                                                  Id = m.TB_MOVIMENTO_ID,
                                                  TipoMovimento = (from t in Db.TB_TIPO_MOVIMENTOs
                                                                   where m.TB_TIPO_MOVIMENTO_ID == t.TB_TIPO_MOVIMENTO_ID
                                                                   select new TipoMovimentoEntity
                                                                   {
                                                                       Id = t.TB_TIPO_MOVIMENTO_ID,
                                                                       TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)t.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
                                                                   }).FirstOrDefault(),
                                                  DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                  Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID),
                                              }).FirstOrDefault()


                             }).AsQueryable();


            if (dataMovimento != DateTime.MinValue)
                resultado = resultado.Where(a => a.Movimento.DataMovimento.Value.Date <= dataMovimento );           

            var resultadoFiltro = resultado.OrderByDescending(a => a.Id).OrderByDescending(a => a.Movimento.DataMovimento);


            if (resultadoFiltro.FirstOrDefault() != null)
            {
                Qtde = resultadoFiltro.First().SaldoQtde;
                Valor = resultadoFiltro.FirstOrDefault().SaldoValor;
                PrecoUnit = resultadoFiltro.FirstOrDefault().PrecoUnitDtMov;
            }


            Tuple<decimal?, decimal?, decimal?> _retorno = Tuple.Create(Qtde, Valor, PrecoUnit);

            return _retorno;


        }

        public decimal? MediaUaRequisicao(int DivisaoId, int SubItemId, DateTime dataMovimento)
        {

            int MesFim = dataMovimento.Month;
             int MesInicio = 0;
             int AnoInicio = 0;

            switch (MesFim)
            {
                case 3:
                    MesInicio = 12;
                    AnoInicio = dataMovimento.Year - 1;
                    break;

                case 2:
                    MesInicio = 11;
                    AnoInicio = dataMovimento.Year - 1;
                    break;

                case 1:
                    MesInicio = 10;
                    AnoInicio = dataMovimento.Year - 1;
                    break;

                default:
                    MesInicio = MesFim - 3;
                    AnoInicio = dataMovimento.Year;
                    break;

            }

            string DtInicio = AnoInicio + "-" + MesInicio + "-01";
            string DtFim = dataMovimento.Year + "-" + MesFim + "-01";


            var resul = (from q in Db.TB_MOVIMENTO_ITEMs
                         join m in Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                         join t in Db.TB_DIVISAOs on m.TB_DIVISAO_ID equals t.TB_DIVISAO_ID
                         where m.TB_MOVIMENTO_DATA_MOVIMENTO >= Convert.ToDateTime(DtInicio)
                         where m.TB_MOVIMENTO_DATA_MOVIMENTO < Convert.ToDateTime(DtFim)
                         where m.TB_MOVIMENTO_ATIVO == true
                         where m.TB_TIPO_MOVIMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada
                         where m.TB_DIVISAO_ID == DivisaoId
                         where q.TB_SUBITEM_MATERIAL_ID == SubItemId
                         where q.TB_MOVIMENTO_ITEM_ATIVO == true
                         select new MovimentoItemEntity
                          {
                              QtdeMov = q.TB_MOVIMENTO_ITEM_QTDE_MOV
                          }).ToList().Sum(a => a.QtdeMov);

            return resul.Value / 3;

        }
        /// <summary>
        /// Relatório Saida de Material
        /// </summary>
        /// <param name="movimento"></param>
        /// <returns></returns>
        public IList<MovimentoEntity> ListarMovimentoItemSaldoTodos(MovimentoEntity movimento)
        {
            IQueryable<MovimentoEntity> resultado = (from mov in this.Db.TB_MOVIMENTOs
                                                     where (mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == movimento.NumeroDocumento)
                                                     where (mov.TB_TIPO_MOVIMENTO_ID == movimento.TipoMovimento.Id)
                                                     where (mov.TB_ALMOXARIFADO_ID == (int)movimento.Almoxarifado.Id)
                                                     where (mov.TB_MOVIMENTO_ATIVO == true)
                                                     select new MovimentoEntity
                                                     {
                                                         Id = mov.TB_MOVIMENTO_ID,
                                                         DataMovimento = mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                         DataDocumento = mov.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                         Bloquear = mov.TB_MOVIMENTO_BLOQUEAR,
                                                         SubTipoMovimentoId = mov.TB_SUBTIPO_MOVIMENTO_ID,
                                                         GeradorDescricao = mov.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                         MovimAlmoxOrigemDestino = (from almox in Db.TB_ALMOXARIFADOs
                                                                                    where almox.TB_ALMOXARIFADO_ID == (int)mov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO
                                                                                    select new AlmoxarifadoEntity
                                                                                    {
                                                                                        Id = almox.TB_ALMOXARIFADO_ID,
                                                                                        Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                                                                                    }).FirstOrDefault(),
                                                         Almoxarifado = (from almox in Db.TB_ALMOXARIFADOs
                                                                         where almox.TB_ALMOXARIFADO_ID == (int)mov.TB_ALMOXARIFADO_ID
                                                                         select new AlmoxarifadoEntity
                                                                         {
                                                                             Id = almox.TB_ALMOXARIFADO_ID,
                                                                             Codigo = almox.TB_ALMOXARIFADO_CODIGO,
                                                                             Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                                                                         }).FirstOrDefault(),
                                                         NumeroDocumento = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                         AnoMesReferencia = string.Format("{0}/{1}", mov.TB_MOVIMENTO_ANO_MES_REFERENCIA.Substring(0, 4), mov.TB_MOVIMENTO_ANO_MES_REFERENCIA.Substring(4, 2)),
                                                         UGE = (from uge in Db.TB_UGEs
                                                                where uge.TB_UGE_ID == mov.TB_UGE_ID
                                                                select new UGEEntity
                                                                {
                                                                    Id = uge.TB_UGE_ID,
                                                                    Descricao = uge.TB_UGE_DESCRICAO
                                                                }).FirstOrDefault(),
                                                         TipoMovimento = (from tipoMov in Db.TB_TIPO_MOVIMENTOs
                                                                          where mov.TB_TIPO_MOVIMENTO_ID == tipoMov.TB_TIPO_MOVIMENTO_ID
                                                                          select new TipoMovimentoEntity
                                                                          {
                                                                              Id = tipoMov.TB_TIPO_MOVIMENTO_ID,
                                                                              Descricao = tipoMov.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                              Codigo = tipoMov.TB_TIPO_MOVIMENTO_CODIGO,
                                                                              SubTipoMovimentoItem = (
                                                                                      from subtipo in Db.TB_SUBTIPO_MOVIMENTOs
                                                                                      where subtipo.TB_SUBTIPO_MOVIMENTO_ID == mov.TB_SUBTIPO_MOVIMENTO_ID
                                                                                      select new SubTipoMovimentoEntity
                                                                                      {
                                                                                          Id = subtipo.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                          Descricao = subtipo.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                      }).FirstOrDefault()
                                                                          }).FirstOrDefault(),
                                                         Divisao = (from divisao in Db.TB_DIVISAOs
                                                                    where mov.TB_DIVISAO_ID == divisao.TB_DIVISAO_ID
                                                                    select new DivisaoEntity
                                                                    {
                                                                        Id = divisao.TB_DIVISAO_ID,
                                                                        Descricao = divisao.TB_DIVISAO_DESCRICAO,
                                                                        EnderecoLogradouro = divisao.TB_DIVISAO_LOGRADOURO,
                                                                        EnderecoNumero = divisao.TB_DIVISAO_NUMERO,
                                                                        EnderecoCompl = divisao.TB_DIVISAO_COMPLEMENTO,
                                                                        EnderecoBairro = divisao.TB_DIVISAO_BAIRRO,
                                                                        EnderecoCep = divisao.TB_DIVISAO_CEP,
                                                                        EnderecoTelefone = divisao.TB_DIVISAO_TELEFONE,
                                                                        EnderecoFax = divisao.TB_DIVISAO_FAX,
                                                                        Responsavel = new ResponsavelEntity
                                                                        {
                                                                            Id = divisao.TB_RESPONSAVEL.TB_RESPONSAVEL_ID,
                                                                            Codigo = divisao.TB_RESPONSAVEL.TB_RESPONSAVEL_CODIGO,
                                                                            Descricao = divisao.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME
                                                                        }
                                                                    }).FirstOrDefault(),
                                                         MovimentoItem = (from movItem in Db.TB_MOVIMENTO_ITEMs
                                                                          where (movItem.TB_MOVIMENTO_ID == mov.TB_MOVIMENTO_ID)
                                                                          select new MovimentoItemEntity
                                                                          {
                                                                              UGE = (from uge in Db.TB_UGEs
                                                                                     where uge.TB_UGE_ID == movItem.TB_UGE_ID
                                                                                     select new UGEEntity
                                                                                     {
                                                                                         Id = uge.TB_UGE_ID,
                                                                                         Descricao = uge.TB_UGE_DESCRICAO
                                                                                     }).FirstOrDefault(),

                                                                              Id = movItem.TB_MOVIMENTO_ITEM_ID,
                                                                              Ativo = movItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                                              DataVencimentoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                                              Desd = movItem.TB_MOVIMENTO_ITEM_DESD,
                                                                              FabricanteLote = movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                                              IdentificacaoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                                              Movimento = new MovimentoEntity
                                                                              {
                                                                                  Id = movItem.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                                  DataMovimento = movItem.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                                  NumeroDocumento = movItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                                  AnoMesReferencia = movItem.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                                                  SubTipoMovimentoId = mov.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                  Divisao = new DivisaoEntity
                                                                                  {
                                                                                      Id = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_ID,
                                                                                      Descricao = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                                                                                      EnderecoLogradouro = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_LOGRADOURO,
                                                                                      EnderecoNumero = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_NUMERO == null ? string.Empty : movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_NUMERO,
                                                                                      EnderecoCompl = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_COMPLEMENTO,
                                                                                      EnderecoBairro = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_BAIRRO,
                                                                                      EnderecoMunicipio = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_MUNICIPIO,
                                                                                      EnderecoCep = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_CEP,
                                                                                      EnderecoTelefone = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_TELEFONE,
                                                                                      EnderecoFax = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_FAX,
                                                                                      Responsavel = new ResponsavelEntity
                                                                                      {
                                                                                          Id = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_ID,
                                                                                          Descricao = movItem.TB_MOVIMENTO.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME
                                                                                      },
                                                                                      Uf = (from uf in Db.TB_UFs
                                                                                            where uf.TB_UF_ID == movItem.TB_MOVIMENTO.TB_DIVISAO.TB_UF_ID
                                                                                            select new UFEntity
                                                                                            {
                                                                                                Id = uf.TB_UF_ID,
                                                                                                Sigla = uf.TB_UF_SIGLA,
                                                                                                Descricao = uf.TB_UF_DESCRICAO
                                                                                            }
                                                                                            ).FirstOrDefault()
                                                                                  },
                                                                                  GeradorDescricao = movItem.TB_MOVIMENTO.TB_MOVIMENTO_GERADOR_DESCRICAO
                                                                              },
                                                                              PrecoUnit = movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                                              QtdeLiq = movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                                              QtdeMov = movItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                                              SaldoQtde = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                                              SaldoQtdeLote = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                                              SaldoValor = movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                                              ValorMov = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                                              SubItemMaterial = (from subitem in Db.TB_SUBITEM_MATERIALs
                                                                                                 where subitem.TB_SUBITEM_MATERIAL_ID == movItem.TB_SUBITEM_MATERIAL_ID
                                                                                                 orderby subitem.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                                 select new SubItemMaterialEntity
                                                                                                 {
                                                                                                     Id = subitem.TB_SUBITEM_MATERIAL_ID,
                                                                                                     Codigo = subitem.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                                     CodigoFormatado = subitem.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                                                     Descricao = subitem.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                                                     SomaSaldoLote = (from saldoSubItem in this.Db.TB_SALDO_SUBITEMs
                                                                                                                      where saldoSubItem.TB_ALMOXARIFADO_ID == movimento.Almoxarifado.Id
                                                                                                                      where (saldoSubItem.TB_SUBITEM_MATERIAL_ID == subitem.TB_SUBITEM_MATERIAL_ID)
                                                                                                                      where (saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC || movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null)
                                                                                                                      where (saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT || movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT == null)
                                                                                                                      where (saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB == movItem.TB_MOVIMENTO_ITEM_LOTE_FABR || movItem.TB_MOVIMENTO_ITEM_LOTE_FABR == null)
                                                                                                                      group saldoSubItem by new { saldoSubItem.TB_UGE_ID, saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC, saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT, saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB }
                                                                                                                          into saldoGrup
                                                                                                                          select new SaldoSubItemEntity
                                                                                                                          {
                                                                                                                              Id = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_ID,
                                                                                                                              LoteDataVenc = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                                                                                                              LoteIdent = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_IDENT,
                                                                                                                              LoteFabr = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_FAB,
                                                                                                                              UGE = (from uge in Db.TB_UGEs
                                                                                                                                     where uge.TB_UGE_ID == saldoGrup.FirstOrDefault().TB_UGE_ID
                                                                                                                                     select new UGEEntity
                                                                                                                                     {
                                                                                                                                         Id = uge.TB_UGE_ID,
                                                                                                                                         Descricao = uge.TB_UGE_DESCRICAO
                                                                                                                                     }).FirstOrDefault(),
                                                                                                                              SaldoQtde = saldoGrup.Sum(a => a.TB_SALDO_SUBITEM_SALDO_QTDE)
                                                                                                                          }).FirstOrDefault(),

                                                                                                     SaldoSubItems = (from saldo in Db.TB_SALDO_SUBITEMs
                                                                                                                      where saldo.TB_ALMOXARIFADO_ID == movimento.Almoxarifado.Id
                                                                                                                      where (saldo.TB_SUBITEM_MATERIAL_ID == subitem.TB_SUBITEM_MATERIAL_ID)
                                                                                                                      where (saldo.TB_UGE_ID == movItem.TB_UGE.TB_UGE_ID)
                                                                                                                      where (saldo.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC || movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null)
                                                                                                                      where (saldo.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT || movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT == null)
                                                                                                                      where (saldo.TB_SALDO_SUBITEM_LOTE_FAB == movItem.TB_MOVIMENTO_ITEM_LOTE_FABR || movItem.TB_MOVIMENTO_ITEM_LOTE_FABR == null)
                                                                                                                      select new SaldoSubItemEntity
                                                                                                                      {
                                                                                                                          Id = saldo.TB_SALDO_SUBITEM_ID,
                                                                                                                          LoteDataVenc = saldo.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                                                                                                          LoteFabr = saldo.TB_SALDO_SUBITEM_LOTE_FAB,
                                                                                                                          LoteIdent = saldo.TB_SALDO_SUBITEM_LOTE_IDENT,
                                                                                                                          PrecoUnit = saldo.TB_SALDO_SUBITEM_PRECO_UNIT ?? 0,
                                                                                                                          SaldoQtde = saldo.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                                                                                          SaldoValor = saldo.TB_SALDO_SUBITEM_SALDO_VALOR,
                                                                                                                          SubItemMaterial = new SubItemMaterialEntity(saldo.TB_SUBITEM_MATERIAL_ID),
                                                                                                                          UGE = new UGEEntity(saldo.TB_UGE_ID),
                                                                                                                      }).ToList(),

                                                                                                     ItemMaterial = (from item in this.Db.TB_ITEM_MATERIALs
                                                                                                                     join itemSubItem in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                                                         on item.TB_ITEM_MATERIAL_ID equals itemSubItem.TB_ITEM_MATERIAL_ID
                                                                                                                     where subitem.TB_SUBITEM_MATERIAL_ID == itemSubItem.TB_SUBITEM_MATERIAL_ID
                                                                                                                     select new ItemMaterialEntity
                                                                                                                     {
                                                                                                                         Id = item.TB_ITEM_MATERIAL_ID,
                                                                                                                         Codigo = item.TB_ITEM_MATERIAL_CODIGO,
                                                                                                                         CodigoFormatado = item.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                                                         Descricao = item.TB_ITEM_MATERIAL_DESCRICAO
                                                                                                                     }
                                                                                                                     ).FirstOrDefault(),
                                                                                                     NaturezaDespesa = (from n in this.Db.TB_NATUREZA_DESPESAs
                                                                                                                        where n.TB_NATUREZA_DESPESA_ID == subitem.TB_NATUREZA_DESPESA_ID
                                                                                                                        select new NaturezaDespesaEntity
                                                                                                                        {
                                                                                                                            Id = n.TB_NATUREZA_DESPESA_ID,
                                                                                                                            Codigo = n.TB_NATUREZA_DESPESA_CODIGO,
                                                                                                                            Descricao = n.TB_NATUREZA_DESPESA_DESCRICAO
                                                                                                                        }).FirstOrDefault(),
                                                                                                     UnidadeFornecimento = (from un in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                                                                                            where un.TB_UNIDADE_FORNECIMENTO_ID == subitem.TB_UNIDADE_FORNECIMENTO_ID
                                                                                                                            select new UnidadeFornecimentoEntity
                                                                                                                            {
                                                                                                                                Id = un.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                                                                Codigo = un.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                                                                Descricao = un.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                                                            }
                                                                                                                             ).FirstOrDefault()
                                                                                                 }).FirstOrDefault(),
                                                                              PTRes = (from rowTabela in Db.TB_PTREs
                                                                                       //where movItem.TB_PTRES_ID == rowTabela.TB_PTRES_ID
                                                                                       where rowTabela.TB_PTRES_CODIGO == movItem.TB_PTRES_CODIGO
                                                                                       where rowTabela.TB_PTRES_PT_PROJ_ATV == movItem.TB_PTRES_PT_ACAO
                                                                                       select new PTResEntity
                                                                                       {
                                                                                           Id = rowTabela.TB_PTRES_ID,
                                                                                           Codigo = rowTabela.TB_PTRES_CODIGO,
                                                                                           Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                                                                           CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                                                                           CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                                                                           CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                                                                           AnoDotacao = rowTabela.TB_PTRES_ANO,
                                                                                           CodigoPT = rowTabela.TB_PTRES_PT_CODIGO,
                                                                                           //Ativo = rowTabela.TB_PTRES_STATUS,
                                                                                           //ProgramaTrabalho = new ProgramaTrabalho(Int64.Parse(rowTabela.TB_PTRES_PT_CODIGO)),
                                                                                           ProgramaTrabalho = new ProgramaTrabalho(rowTabela.TB_PTRES_PT_CODIGO),
                                                                                       }).FirstOrDefault()
                                                                          }).ToList(),
                                                         Observacoes = mov.TB_MOVIMENTO_OBSERVACOES
                                                     }).AsQueryable();
            var retorno = resultado.ToList();

            bool bloqueio = false;


            if (Constante.isSamWebDebugged)
            {
                string strSQL = resultado.ToString();
                this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            }

            if (retorno.Count > 0)
            {

                if (retorno[0].Bloquear != null)
                    bloqueio = ((bool)retorno[0].Bloquear);

                if (!bloqueio)
                {
                    foreach (var item in retorno[0].MovimentoItem)
                    {

                        //Calcula o saldo total dos menos a reserva       
                        Tuple<decimal, int, bool, decimal, decimal> result;
                        result = CalculaTotalSaldoUGEsReserva(movimento.Almoxarifado.Id, item.SubItemMaterial.Id, item.UGE.Id);
                        // CalculaTotalSaldoUGEsReserva(movimento.Almoxarifado.Id, item.SubItemMaterial.Id) + (item.SubItemMaterial.SomaSaldoTotal == null ? 0 : item.SubItemMaterial.SomaSaldoTotal);
                        Tuple<decimal?, decimal?, decimal?> retornoSaldo = SaldoMovimentoItemDataMovimento(item.SubItemMaterial.Id, movimento.Almoxarifado.Id, movimento.UGE.Id, Convert.ToDateTime(movimento.DataMovimento));              
                        item.SubItemMaterial.SomaSaldoTotal = result.Item1 + (item.SubItemMaterial.SomaSaldoTotal == null ? 0 : item.SubItemMaterial.SomaSaldoTotal);
                        item.SubItemMaterial.SomaSaldoTotal = item.SubItemMaterial.SomaSaldoTotal < 0 ? 0 : item.SubItemMaterial.SomaSaldoTotal;
                        item.SubItemMaterial.SomaSaldoTotalDataMovimento = retornoSaldo.Item1==null || retornoSaldo.Item1 < 0? 0 : retornoSaldo.Item1;
                       // item.SubItemMaterial.SomaSaldoTotalDataMovimento = item.SubItemMaterial.SomaSaldoTotal < item.SubItemMaterial.SomaSaldoTotalDataMovimento ? item.SubItemMaterial.SomaSaldoTotal : item.SubItemMaterial.SomaSaldoTotalDataMovimento;

                        item.SubItemMaterial.SomaSaldoValorTotal = result.Item4 == null ? 0 : result.Item4;
                        item.SubItemMaterial.SomaSaldoValorTotal = item.SubItemMaterial.SomaSaldoValorTotal < 0 ? 0 : item.SubItemMaterial.SomaSaldoValorTotal;
                        item.SubItemMaterial.SomaSaldoValorTotalDataMovimento = retornoSaldo.Item2;
                        item.SubItemMaterial.SomaSaldoValorTotalDataMovimento = item.SubItemMaterial.SomaSaldoValorTotal < item.SubItemMaterial.SomaSaldoValorTotalDataMovimento ? item.SubItemMaterial.SomaSaldoValorTotal : item.SubItemMaterial.SomaSaldoValorTotalDataMovimento;                       
                        item.SubItemMaterial.PrecoUnitAtual = result.Item5 == null ? 0 : result.Item5;
                        item.SubItemMaterial.PrecoUnitDataMovimento = retornoSaldo.Item3 == null ? 0 : retornoSaldo.Item3;

                        if (result.Item2 > 1 || result.Item3)
                            item.FabricanteLote = "Vários";

                        if (!item.QtdeMov.HasValue)
                        {
                            item.QtdeMov = item.QtdeLiq;

                            if (item.QtdeLiq > item.SubItemMaterial.SomaSaldoTotal)
                            {
                                item.QtdeMov = item.SubItemMaterial.SomaSaldoTotal;
                            }

                            if (item.QtdeMov > item.SubItemMaterial.SomaSaldoTotalDataMovimento)
                            {
                                item.QtdeMov = item.SubItemMaterial.SomaSaldoTotalDataMovimento;
                            }
                        }

                        //Calcula o saldo por lote menos a reserva
                        if (item.SubItemMaterial.SomaSaldoLote != null)
                        {
                            item.SubItemMaterial.SomaSaldoLote.SaldoQtde = CalculaTotalSaldoUGEsReserva(item.SubItemMaterial.SomaSaldoLote, item.SubItemMaterial.Id, movimento.Almoxarifado.Id);
                            item.SubItemMaterial.SomaSaldoLote.SaldoQtde = item.SubItemMaterial.SomaSaldoLote.SaldoQtde < 0 ? 0 : item.SubItemMaterial.SomaSaldoLote.SaldoQtde;
                        }

                        if (item.SubItemMaterial.SomaSaldoTotalDataMovimento > item.SubItemMaterial.SomaSaldoTotal)
                        {
                            item.SaldoQtde = item.SubItemMaterial.SomaSaldoTotal;
                            item.SaldoValor = item.SubItemMaterial.SomaSaldoValorTotal;
                            item.PrecoUnitDtMov = item.SubItemMaterial.PrecoUnitAtual;
                        }
                        else
                        {
                            item.SaldoQtde = item.SubItemMaterial.SomaSaldoTotalDataMovimento;
                            item.SaldoValor = item.SubItemMaterial.SomaSaldoValorTotalDataMovimento;
                            item.PrecoUnitDtMov = item.SubItemMaterial.PrecoUnitDataMovimento;
                        }

                        // calcula valor movel
                        if (item.SubItemMaterial.SaldoSubItems != null && item.SubItemMaterial.SaldoSubItems.Count > 0)
                        {
                            //item.PrecoUnit = item.SubItemMaterial.SaldoSubItems[0].PrecoUnit;
                            item.PrecoUnit = item.PrecoUnit;
                            //item.ValorMov = item.QtdeLiq * item.SubItemMaterial.SaldoSubItems[0].PrecoUnit;
                        }

                        if (retorno.FirstOrDefault().Divisao != null && Convert.ToDateTime(movimento.DataMovimento) > DateTime.MinValue)
                            item.QtdeMedia = MediaUaRequisicao(Convert.ToInt32(retorno.FirstOrDefault().Divisao.Id), Convert.ToInt32(item.SubItemMaterial.Id), Convert.ToDateTime(movimento.DataMovimento));

                    }


                }
            }

            return retorno;
        }

        private Tuple<decimal, int, bool, decimal, decimal> CalculaTotalSaldoUGEsReserva(int? almoxId, int? subItemId, int? ugeId)
        {

            var res = (from reserva in this.Db.TB_RESERVA_MATERIALs
                       where reserva.TB_ALMOXARIFADO_ID == almoxId
                       where reserva.TB_SUBITEM_MATERIAL_ID == subItemId
                       where reserva.TB_UGE_ID ==  ugeId
                       select new ReservaMaterialEntity
                       {
                           Quantidade = reserva.TB_RESERVA_MATERIAL_QUANT.Value
                       }).ToList().Sum(a => a.Quantidade);
           
            var saldo = (from saldoSubItem in this.Db.TB_SALDO_SUBITEMs
                         where saldoSubItem.TB_ALMOXARIFADO_ID == almoxId
                         where saldoSubItem.TB_SUBITEM_MATERIAL_ID == subItemId
                         where saldoSubItem.TB_UGE_ID ==ugeId
                         select new SaldoSubItemEntity
                         {
                             Id = saldoSubItem.TB_SALDO_SUBITEM_ID,
                             SaldoQtde = saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                             SaldoValor = saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR,
                             PrecoUnit = saldoSubItem.TB_SALDO_SUBITEM_PRECO_UNIT
                         }).ToList();

            int qdtelote = 0;
            bool Lote = false;

            if (saldo != null && saldo.Count > 0)
            {
                var listLote = (from saldoSubItem in this.Db.TB_SALDO_SUBITEM_LOTEs
                                where saldoSubItem.TB_SALDO_SUBITEM_ID == saldo.FirstOrDefault().Id
                                where saldoSubItem.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE != 0
                                select new SaldoSubItemEntity
                                {
                                    LoteIdent = saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT
                                }).ToList();

                qdtelote = listLote.Count();
                Lote = listLote.Any(a => a.LoteIdent != string.Empty) ? true : false;

            }



            decimal calculo = 0;
            decimal calculoValor = 0;
            decimal precoUnit = 0;
            if (saldo.Count > 0)
            {
                calculo = Convert.ToDecimal((saldo.FirstOrDefault().SaldoQtde - res));
                calculoValor = Convert.ToDecimal((saldo.FirstOrDefault().SaldoValor));
                precoUnit = Convert.ToDecimal((saldo.FirstOrDefault().PrecoUnit));
            }





            Tuple<decimal, int, bool, decimal, decimal> _cliente =
                        Tuple.Create(calculo, qdtelote, Lote, calculoValor, precoUnit);
            return _cliente;


        }

        private decimal? CalculaTotalSaldoUGEsReserva(SaldoSubItemEntity somaSaldoLote, int? subItemId, int? almoxId)
        {
            if (somaSaldoLote.IsNull())
                return _valorZero;

            var res = (from reserva in this.Db.TB_RESERVA_MATERIALs
                       where reserva.TB_ALMOXARIFADO_ID == almoxId
                       where reserva.TB_SUBITEM_MATERIAL_ID == subItemId
                       select new ReservaMaterialEntity
                       {
                           Quantidade = reserva.TB_RESERVA_MATERIAL_QUANT.Value
                       }).ToList().Sum(a => a.Quantidade);

            return (somaSaldoLote.SaldoQtde - res);
        }

        public MovimentoItemEntity SelectPorDocumentoItem(int ItemId, string Documento)
        {
            MovimentoItemEntity resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                             where a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO == Documento
                                             where a.TB_MOVIMENTO_ITEM_ID == ItemId
                                             orderby a.TB_MOVIMENTO_ITEM_ID
                                             select new MovimentoItemEntity
                                             {
                                                 Id = a.TB_MOVIMENTO_ID,
                                                 Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                 DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                 Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                 FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                 Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                 QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                 QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                 SubItemMaterial = new SubItemMaterialEntity
                                                 {
                                                     Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                     CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,                                              
                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                     ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                     join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                         on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                     where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                     select new ItemMaterialEntity
                                                                     {
                                                                         Id = i.TB_ITEM_MATERIAL_ID,
                                                                         Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                         CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                         Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                     }
                                                                     ).FirstOrDefault(),
                                                     UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                     {
                                                         Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                         Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                         Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                     }
                                                 }
                                                     ,
                                                 UGE = new UGEEntity(a.TB_MOVIMENTO.TB_UGE.TB_UGE_ID),
                                                 ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                             })
                                        .FirstOrDefault();
            return resultado;
        }

        public MovimentoItemEntity Estornar(int MovimentoId)
        {
            throw new NotImplementedException();
        }

        public MovimentoItemEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<MovimentoItemEntity> ListarSubItemLote(string Lote, int idFornecedor, int idSubItem)
        {
            IQueryable resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                    join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                    //join siAlmox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                                    //    on new { m.TB_ALMOXARIFADO_ID, a.TB_SUBITEM_MATERIAL_ID } equals new { siAlmox.TB_ALMOXARIFADO_ID, siAlmox.TB_SUBITEM_MATERIAL_ID }
                                    where a.TB_SUBITEM_MATERIAL_ID == idSubItem
                                    where a.TB_MOVIMENTO_ITEM_LOTE_IDENT == Lote
                                    where m.TB_FORNECEDOR_ID == idFornecedor || (idFornecedor == 0 || idFornecedor == null)
                                    where ((m.TB_MOVIMENTO_ATIVO == true && m.TB_MOVIMENTO_DATA_ESTORNO == null))
                                    where ((a.TB_MOVIMENTO_ITEM_ATIVO == true && a.TB_MOVIMENTO_ITEM_DATA_ESTORNO == null))
                                    where (m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                                    orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date, a.TB_MOVIMENTO_ITEM_ID
                                    select new MovimentoItemEntity
                                    {
                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                        DataVencimentoLoteFormatado = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue ?
                                        a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.Value.ToShortDateString() : "",
                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                        //EstoqueMinimo = siAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,


                                    });

            var retorno = Queryable.Cast<MovimentoItemEntity>(resultado).ToList();

            return retorno;
        }


        public IList<MovimentoItemEntity> ListarMovimentacaoItem(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime DtInicial, DateTime DtFinal, bool comEstorno)
        {
            IQueryable resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                    join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                    //join siAlmox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                                    //    on new { m.TB_ALMOXARIFADO_ID, a.TB_SUBITEM_MATERIAL_ID } equals new { siAlmox.TB_ALMOXARIFADO_ID, siAlmox.TB_SUBITEM_MATERIAL_ID }
                                    //where a.TB_UGE_ID == UgeId
                                    where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == SubItemMatId
                                    where m.TB_ALMOXARIFADO_ID == AlmoxId
                                    where (m.TB_MOVIMENTO_DATA_MOVIMENTO >= DtInicial && m.TB_MOVIMENTO_DATA_MOVIMENTO < DtFinal)
                                    where ((m.TB_MOVIMENTO_ATIVO == true && comEstorno == false) || comEstorno == true)
                                    where ((a.TB_MOVIMENTO_ITEM_ATIVO == true && comEstorno == false) || comEstorno == true)
                                    where (m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                    orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date, a.TB_MOVIMENTO_ITEM_ID 
                                    select new MovimentoItemEntity
                                    {
                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                        Movimento = new MovimentoEntity
                                        {
                                            Id = m.TB_MOVIMENTO_ID,
                                            Almoxarifado = new AlmoxarifadoEntity
                                            {
                                                Id = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                Codigo = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                                                Descricao = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                            },
                                            DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
                                            TipoMovimento = new TipoMovimentoEntity
                                            {
                                                Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                Codigo = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                AgrupamentoId = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                TipoAgrupamento = new TipoMovimentoAgrupamentoEntity
                                                {
                                                    Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                    Descricao = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                                                }
                                            },
                                            Divisao = new DivisaoEntity
                                            {
                                                Id = m.TB_DIVISAO.TB_DIVISAO_ID,
                                                Descricao = m.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                                                Uf = new UFEntity
                                                {
                                                    Id = m.TB_DIVISAO.TB_UF.TB_UF_ID,
                                                    Sigla = m.TB_DIVISAO.TB_UF.TB_UF_SIGLA,
                                                    Descricao = m.TB_DIVISAO.TB_UF.TB_UF_DESCRICAO,
                                                },
                                                Responsavel = new ResponsavelEntity
                                                {
                                                    Id = m.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_ID,
                                                    Descricao = m.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME
                                                }
                                            },
                                            Fornecedor = new FornecedorEntity
                                            {
                                                CpfCnpj = m.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ,
                                                Nome = m.TB_FORNECEDOR.TB_FORNECEDOR_NOME
                                            },
                                            DataDocumento = m.TB_MOVIMENTO_DATA_DOCUMENTO,
                                            NumeroDocumento = m.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                            Ativo = m.TB_MOVIMENTO_ATIVO,
                                            GeradorDescricao = m.TB_MOVIMENTO_GERADOR_DESCRICAO
                                        },
                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                        //EstoqueMinimo = siAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                                        SubItemMaterial = new SubItemMaterialEntity
                                        {
                                            Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                            Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                            CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                            Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                            ItemMaterial = (from isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                            select new ItemMaterialEntity
                                                            {
                                                                Id = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                Codigo = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                CodigoFormatado = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                Descricao = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                            }
                                                            ).FirstOrDefault(),
                                            NaturezaDespesa = new NaturezaDespesaEntity
                                            {
                                                Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                            },
                                            UnidadeFornecimento = new UnidadeFornecimentoEntity
                                            {
                                                Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                            }
                                        }
                                        ,
                                        UGE = new UGEEntity
                                        {
                                            Id = a.TB_UGE.TB_UGE_ID,
                                            Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                            Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                        },
                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                    });

            var strSQL = resultado.ToString();
            this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            var retorno = resultado.Cast<MovimentoItemEntity>().ToList(); //Queryable.Cast<MovimentoItemEntity>(resultado).ToList();


            #region Truncar Valores Retorno
            retorno.Cast<MovimentoItemEntity>().ToList().ForEach(_movItem =>
            {
                _movItem.PrecoMedio = ((_movItem.PrecoMedio.HasValue) ? ((decimal.Parse(_movItem.ValorMov.ToString()).truncarQuatroCasas()) / (decimal.Parse(_movItem.QtdeMov.ToString()).truncarQuatroCasas())).truncarQuatroCasas() : 0.0000m);
                _movItem.ValorMov = ((_movItem.ValorMov.HasValue) ? (decimal.Parse(_movItem.ValorMov.ToString()).truncarDuasCasas()) : 0.00m);
                _movItem.SaldoValor = ((_movItem.SaldoValor.HasValue) ? (decimal.Parse(_movItem.SaldoValor.ToString()).truncarDuasCasas()) : 0.00m);
            });

            #endregion Truncar Valores Retorno

            return AtualizaLastEstorno(retorno);
        }

        private IList<MovimentoItemEntity> AtualizaLastEstorno(IList<MovimentoItemEntity> resultado)
        {
            if (resultado.Where(a => a.Ativo == true).Count() > 0)
            {
                var linha = resultado.Where(a => a.Ativo == true).Last();
                resultado.Last().SaldoQtdeLastAtivo = linha.SaldoQtde;
                resultado.Last().PrecoUnitLastAtivo = linha.PrecoUnit;
                resultado.Last().SaldoValorLastAtivo = linha.SaldoValor;
            }

            return resultado;
        }

        /// <summary>
        /// Metodo utilizado para listar todos os Movimentos itens para serem recalculados por uma determinada data
        /// </summary>
        /// <param name="AlmoxId"></param>
        /// <param name="SubItemMatId"></param>
        /// <param name="UgeId"></param>
        /// <param name="DtInicial"></param>
        /// <param name="DtFinal"></param>
        /// <returns></returns>
        public IList<MovimentoItemEntity> ListarMovimentacaoItemRecalculo(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {
            IEnumerable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                          join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                          join siAlmox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                              on new { m.TB_ALMOXARIFADO_ID, a.TB_SUBITEM_MATERIAL_ID } equals new { siAlmox.TB_ALMOXARIFADO_ID, siAlmox.TB_SUBITEM_MATERIAL_ID }
                                                          where a.TB_UGE_ID == UgeId
                                                          where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == (int)SubItemMatId
                                                          where m.TB_ALMOXARIFADO_ID == AlmoxId
                                                          where m.TB_MOVIMENTO_ATIVO == true
                                                          where m.TB_TIPO_MOVIMENTO_ID != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada
                                                          where m.TB_TIPO_MOVIMENTO_ID != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente
                                                          where ((m.TB_MOVIMENTO_DATA_MOVIMENTO > DtInicial || !DtInicial.HasValue) && (m.TB_MOVIMENTO_DATA_MOVIMENTO < DtFinal || !DtFinal.HasValue))
                                                          orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO, a.TB_MOVIMENTO.TB_MOVIMENTO_ID
                                                          select new MovimentoItemEntity
                                                          {
                                                              Id = a.TB_MOVIMENTO_ITEM_ID,
                                                              Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                              DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                              Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                              FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                              IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                              PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                              QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                              QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                              SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                              SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                              SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                              EstoqueMinimo = siAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                              ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                              UGE = new UGEEntity(a.TB_UGE_ID),
                                                              SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                              Movimento = new MovimentoEntity
                                                              {
                                                                  Id = m.TB_MOVIMENTO_ID,
                                                                  DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                  TipoMovimento = new TipoMovimentoEntity
                                                                  {
                                                                      Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                                      Codigo = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                                      AgrupamentoId = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                      TipoAgrupamento = new TipoMovimentoAgrupamentoEntity
                                                                      {
                                                                          Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                          Descricao = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                                                                      }
                                                                  }
                                                              }
                                                          }).ToList();
            this.totalregistros = resultado.Count();
            return resultado.ToList();
        }

        //Listar movimentos para o estorno
        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorIdEstorno(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                    where a.TB_UGE_ID == UgeId
                                                    where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == (int)SubItemMatId
                                                    where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                    where m.TB_ALMOXARIFADO_ID == AlmoxId
                                                    where ((m.TB_TIPO_MOVIMENTO_ID != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada &&
                                                        m.TB_TIPO_MOVIMENTO_ID != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada &&
                                                        m.TB_TIPO_MOVIMENTO_ID != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente))
                                                    orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO, a.TB_MOVIMENTO.TB_MOVIMENTO_ID
                                                    select new MovimentoItemEntity
                                                    {
                                                        Id = a.TB_MOVIMENTO_ITEM_ID,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                        Movimento = new MovimentoEntity
                                                        {
                                                            Id = m.TB_MOVIMENTO_ID,
                                                            TipoMovimento = new TipoMovimentoEntity()
                                                        }
                                                    }).ToList();
            return resultado;
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorId(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {

            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                         where a.TB_UGE_ID == UgeId
                                                         where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == (int)SubItemMatId
                                                         where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                         where m.TB_ALMOXARIFADO_ID == AlmoxId
                                                         orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO, a.TB_MOVIMENTO.TB_MOVIMENTO_ID
                                                         select new MovimentoItemEntity
                                                         {
                                                             Id = a.TB_MOVIMENTO_ITEM_ID,
                                                             Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                             DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                             Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                             FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                             IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                             Movimento = new MovimentoEntity
                                                             {
                                                                 Id = m.TB_MOVIMENTO_ID,
                                                                 Almoxarifado = new AlmoxarifadoEntity
                                                                 {
                                                                     Id = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                     Codigo = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                                                                     Descricao = m.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                 },
                                                                 DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                 TipoMovimento = new TipoMovimentoEntity
                                                                 {
                                                                     Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                                     Codigo = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                                     AgrupamentoId = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                     TipoAgrupamento = new TipoMovimentoAgrupamentoEntity
                                                                     {
                                                                         Id = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                         Descricao = m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                                                                     }
                                                                 },
                                                                 Divisao = new DivisaoEntity
                                                                 {
                                                                     Id = m.TB_DIVISAO.TB_DIVISAO_ID,
                                                                     Descricao = m.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                                                                     Uf = new UFEntity
                                                                     {
                                                                         Id = m.TB_DIVISAO.TB_UF.TB_UF_ID,
                                                                         Sigla = m.TB_DIVISAO.TB_UF.TB_UF_SIGLA,
                                                                         Descricao = m.TB_DIVISAO.TB_UF.TB_UF_DESCRICAO,
                                                                     },
                                                                     Responsavel = new ResponsavelEntity
                                                                     {
                                                                         Id = m.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_ID,
                                                                         Descricao = m.TB_DIVISAO.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME
                                                                     }
                                                                 },
                                                                 Fornecedor = new FornecedorEntity
                                                                 {
                                                                     CpfCnpj = m.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ,
                                                                     Nome = m.TB_FORNECEDOR.TB_FORNECEDOR_NOME
                                                                 },
                                                                 DataDocumento = m.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                 NumeroDocumento = m.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                 Ativo = m.TB_MOVIMENTO_ATIVO
                                                             },
                                                             PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                             QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                             QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                             SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                             SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                             SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                             SubItemMaterial = new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                 ItemMaterial = (from isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                 where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                                 select new ItemMaterialEntity
                                                                                 {
                                                                                     Id = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                     Codigo = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                     CodigoFormatado = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                     Descricao = isi.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                                                 }
                                                                                 ).FirstOrDefault(),
                                                                 NaturezaDespesa = new NaturezaDespesaEntity
                                                                 {
                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                 },
                                                                 UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                 {
                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                 }
                                                             }
                                                             ,
                                                             UGE = new UGEEntity
                                                             {
                                                                 Id = a.TB_UGE.TB_UGE_ID,
                                                                 Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                                 Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                                             },
                                                             ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                         }).AsQueryable<MovimentoItemEntity>();

            if (DtInicial.HasValue)
                resultado = resultado.Where(m => m.Movimento.DataMovimento >= DtInicial);

            if (DtFinal.HasValue)
                resultado = resultado.Where(m => m.Movimento.DataMovimento <= DtFinal);


            //Faz um filtro por requisicoes pendentes e finalizadas

            resultado = resultado.Where
                (a => a.Movimento.TipoMovimento.Id != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada &&
                    a.Movimento.TipoMovimento.Id != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada &&
                    a.Movimento.TipoMovimento.Id != (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente);

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();
            this.totalregistros = result.Count();

            return result;
        }

        /// <summary>
        /// Retorna o saldo do Item de Material, por meio da lista de MovimentoItem's relacionados ao empenho.
        /// </summary>
        /// <param name="iItemMaterialCodigo">CÃƒÂ³digo do ItemMaterial do sistema SIAFISICO</param>
        /// <param name="strEmpenhoCodigo">CÃƒÂ³digo do Empenho do sistema SIAFISICO</param>
        /// <returns></returns>
        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo)
        {
            //IEnumerable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
            IEnumerable<MovimentoItemEntity> resultado = (from MovimentoItem in this.Db.TB_MOVIMENTO_ITEMs
                                                          //join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID into asd
                                                          join ItemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs on MovimentoItem.TB_SUBITEM_MATERIAL_ID equals ItemSubItemMaterial.TB_SUBITEM_MATERIAL_ID into grpMovimentos
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO == strEmpenhoCodigo
                                                          where MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == iItemMaterialCodigo
                                                          where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                          from ItemSubItemMaterial in grpMovimentos.DefaultIfEmpty()
                                                          orderby MovimentoItem.TB_MOVIMENTO_ITEM_ID
                                                          select new MovimentoItemEntity
                                                          {
                                                              Id = MovimentoItem.TB_MOVIMENTO_ID,
                                                              Ativo = MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                              DataVencimentoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                              Desd = MovimentoItem.TB_MOVIMENTO_ITEM_DESD,
                                                              FabricanteLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                              IdentificacaoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                              Movimento = new MovimentoEntity
                                                              {
                                                                  Id = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                  Empenho = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                  NumeroDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                  DataDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                  Ativo = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO
                                                              },
                                                              PrecoUnit = MovimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                              QtdeLiq = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                              QtdeMov = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV ?? 0,
                                                              SaldoQtde = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                              SaldoQtdeLote = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                              SaldoValor = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                              ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                              where ItemMaterial.TB_ITEM_MATERIAL_ID == ItemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                              select new ItemMaterialEntity
                                                                              {
                                                                                  Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                  Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                  Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                                                              }).FirstOrDefault(),
                                                              SubItemMaterial = (from SubItemMaterial in this.Db.TB_SUBITEM_MATERIALs
                                                                                 where SubItemMaterial.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                                 select new SubItemMaterialEntity
                                                                                 {
                                                                                     Id = SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                                     Codigo = SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                     Descricao = SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                 }).FirstOrDefault(),
                                                              UGE = new UGEEntity(MovimentoItem.TB_UGE_ID),
                                                              ValorMov = MovimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                          });

            string lStrSQL = resultado.ToString();
            this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();

            return result;
        }

        /// <summary>
        /// Retorna o saldo do Item de Material, por meio da lista de MovimentoItem's relacionados ao empenho.
        /// </summary>
        /// <param name="iItemMaterialCodigo">CÃƒÂ³digo do ItemMaterial do sistema SIAFISICO</param>
        /// <param name="strEmpenhoCodigo">CÃƒÂ³digo do Empenho do sistema SIAFISICO</param>
        /// <returns></returns>
        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID)
        {
            IEnumerable<MovimentoItemEntity> resultado = (from MovimentoItem in this.Db.TB_MOVIMENTO_ITEMs
                                                          join ItemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs on MovimentoItem.TB_SUBITEM_MATERIAL_ID equals ItemSubItemMaterial.TB_SUBITEM_MATERIAL_ID into grpMovimentos
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO == strEmpenhoCodigo
                                                          where MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == iItemMaterialCodigo
                                                          where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_UGE_ID == iUge_ID
                                                          where MovimentoItem.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == iAlmoxarifado_ID
                                                          from ItemSubItemMaterial in grpMovimentos.DefaultIfEmpty()
                                                          orderby MovimentoItem.TB_MOVIMENTO_ITEM_ID
                                                          select new MovimentoItemEntity
                                                          {
                                                              Id = MovimentoItem.TB_MOVIMENTO_ID,
                                                              Ativo = MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                              DataVencimentoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                              Desd = MovimentoItem.TB_MOVIMENTO_ITEM_DESD,
                                                              FabricanteLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                              IdentificacaoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                              Movimento = new MovimentoEntity
                                                              {
                                                                  Id = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                  Empenho = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                  NumeroDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                  DataDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                  Ativo = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO
                                                              },
                                                              PrecoUnit = MovimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                              QtdeLiq = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                              QtdeMov = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV ?? 0,
                                                              SaldoQtde = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                              SaldoQtdeLote = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                              SaldoValor = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                              ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                              where ItemMaterial.TB_ITEM_MATERIAL_ID == ItemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                              select new ItemMaterialEntity
                                                                              {
                                                                                  Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                  Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                  Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                                                              }).FirstOrDefault(),
                                                              SubItemMaterial = (from SubItemMaterial in this.Db.TB_SUBITEM_MATERIALs
                                                                                 where SubItemMaterial.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                                 select new SubItemMaterialEntity
                                                                                 {
                                                                                     Id = SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                                     Codigo = SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                     Descricao = SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                 }).FirstOrDefault(),
                                                              UGE = new UGEEntity(MovimentoItem.TB_UGE_ID),
                                                              ValorMov = MovimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                          });

            string lStrSQL = resultado.ToString();
            this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();

            return result;
        }

        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, int iSubItemMaterialCodigo_ID, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID)
        {
            IEnumerable<MovimentoItemEntity> resultado = (from MovimentoItem in this.Db.TB_MOVIMENTO_ITEMs
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO == strEmpenhoCodigo
                                                          where MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == iItemMaterialCodigo
                                                          where MovimentoItem.TB_SUBITEM_MATERIAL_ID == iSubItemMaterialCodigo_ID
                                                          where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_UGE_ID == iUge_ID
                                                          where MovimentoItem.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == iAlmoxarifado_ID
                                                          orderby MovimentoItem.TB_MOVIMENTO_ITEM_ID
                                                          select new MovimentoItemEntity
                                                          {
                                                              Id = MovimentoItem.TB_MOVIMENTO_ID,
                                                              Ativo = MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                              DataVencimentoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                              Desd = MovimentoItem.TB_MOVIMENTO_ITEM_DESD,
                                                              FabricanteLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                              IdentificacaoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                              Movimento = new MovimentoEntity
                                                              {
                                                                  Id = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                  Empenho = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                  NumeroDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                  DataDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                  Ativo = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO,
                                                                  
                                                                  TipoMovimento = new TipoMovimentoEntity() {
                                                                      AgrupamentoId = MovimentoItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                                  }
                                                              },
                                                              PrecoUnit = MovimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                              QtdeLiq = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                              QtdeMov = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV ?? 0,
                                                              SaldoQtde = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                              SaldoQtdeLote = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                              SaldoValor = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                              ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                              where ItemMaterial.TB_ITEM_MATERIAL_ID == MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID //ItemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                              select new ItemMaterialEntity
                                                                              {
                                                                                  Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                  Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                  Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                                                              }).FirstOrDefault(),
                                                              SubItemMaterial = (from SubItemMaterial in this.Db.TB_SUBITEM_MATERIALs
                                                                                 where SubItemMaterial.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                                 select new SubItemMaterialEntity
                                                                                 {
                                                                                     Id = SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                                     Codigo = SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                     Descricao = SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                 }).FirstOrDefault(),
                                                              UGE = new UGEEntity(MovimentoItem.TB_UGE_ID),
                                                              ValorMov = MovimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                          });

            string lStrSQL = resultado.ToString();
            this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();

            return result;
        }

        public IList<MovimentoItemEntity> ListarSaldoValorConsumoDeItemMaterial(int iItemMaterialCodigo, int iSubItemMaterialCodigo_ID, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID)
        {
            IEnumerable<MovimentoItemEntity> resultado = (from MovimentoItem in this.Db.TB_MOVIMENTO_ITEMs
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO == strEmpenhoCodigo
                                                          where MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == iItemMaterialCodigo
                                                          where MovimentoItem.TB_SUBITEM_MATERIAL_ID == iSubItemMaterialCodigo_ID
                                                          where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                          where MovimentoItem.TB_MOVIMENTO.TB_UGE_ID == iUge_ID
                                                          where MovimentoItem.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == iAlmoxarifado_ID
                                                          //where MovimentoItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO_ID == TipoMovimento.ConsumoImediatoEmpenho.GetHashCode()
                                                          where MovimentoItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == TipoAgrupamentoMovimento.ConsumoImediato.GetHashCode()
                                                          orderby MovimentoItem.TB_MOVIMENTO_ITEM_ID
                                                          select new MovimentoItemEntity
                                                                                          {
                                                                                              Id = MovimentoItem.TB_MOVIMENTO_ID,
                                                                                              Ativo = MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                                                              DataVencimentoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                                                              Desd = MovimentoItem.TB_MOVIMENTO_ITEM_DESD,
                                                                                              FabricanteLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                                                              IdentificacaoLote = MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                                                              Movimento = new MovimentoEntity
                                                                                              {
                                                                                                  Id = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                                                  Empenho = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                                                  NumeroDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                                                  DataDocumento = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                                                  Ativo = MovimentoItem.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO
                                                                                              },
                                                                                              PrecoUnit = MovimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                                                              QtdeLiq = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                                                              QtdeMov = MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV ?? 0,
                                                                                              SaldoQtde = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                                                              SaldoQtdeLote = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                                                              SaldoValor = MovimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                                                              ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                                              where ItemMaterial.TB_ITEM_MATERIAL_ID == MovimentoItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID //ItemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                                                              select new ItemMaterialEntity
                                                                                                              {
                                                                                                                  Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                                                  Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                                                  Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                                              }).FirstOrDefault(),
                                                                                              SubItemMaterial = (from SubItemMaterial in this.Db.TB_SUBITEM_MATERIALs
                                                                                                                 where SubItemMaterial.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                                                                 select new SubItemMaterialEntity
                                                                                                                 {
                                                                                                                     Id = SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                                                                     Codigo = SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                                                     Descricao = SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                                                 }).FirstOrDefault(),
                                                                                              UGE = new UGEEntity(MovimentoItem.TB_UGE_ID),
                                                                                              ValorMov = MovimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                                                          });

            string lStrSQL = resultado.ToString();
            this.Db.GetCommand(resultado as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();

            return result;
        }

        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID, bool blnOverload)
        {
            IList<MovimentoItemEntity> lstRetorno = new List<MovimentoItemEntity>();

            IQueryable<TB_MOVIMENTO_ITEM> qryConsulta = (from MovimentoItem in this.Db.TB_MOVIMENTO_ITEMs
                                                         join ItemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs on MovimentoItem.TB_SUBITEM_MATERIAL_ID equals ItemSubItemMaterial.TB_SUBITEM_MATERIAL_ID into grpMovimentos
                                                         from ItemSubItemMaterial in grpMovimentos.DefaultIfEmpty()
                                                         select MovimentoItem).AsQueryable<TB_MOVIMENTO_ITEM>();

            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhere = (ItemMovimentacao => ItemMovimentacao.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO == strEmpenhoCodigo
                                                                                   && ItemMovimentacao.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == iItemMaterialCodigo
                                                                                   && ItemMovimentacao.TB_MOVIMENTO_ITEM_ATIVO == true
                                                                                   && ItemMovimentacao.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                                                   && ItemMovimentacao.TB_MOVIMENTO.TB_UGE_ID == iUge_ID
                                                                                   && ItemMovimentacao.TB_UGE_ID == iUge_ID
                                                                                   && ItemMovimentacao.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == iAlmoxarifado_ID
                                                                 );

            qryConsulta = qryConsulta.Where(expWhere);

            string lStrSQL = qryConsulta.ToString();
            this.Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            qryConsulta.ToList().ForEach(rowTabela => lstRetorno.Add(
                                                                      new MovimentoItemEntity
                                                                      {
                                                                          Id = rowTabela.TB_MOVIMENTO_ITEM_ID,
                                                                          Ativo = rowTabela.TB_MOVIMENTO_ITEM_ATIVO,
                                                                          DataVencimentoLote = (rowTabela.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC : null,
                                                                          FabricanteLote = (rowTabela.TB_MOVIMENTO_ITEM_LOTE_FABR != "") ? rowTabela.TB_MOVIMENTO_ITEM_LOTE_FABR : null,
                                                                          IdentificacaoLote = (rowTabela.TB_MOVIMENTO_ITEM_LOTE_IDENT != "") ? rowTabela.TB_MOVIMENTO_ITEM_LOTE_IDENT : null,
                                                                          Desd = (rowTabela.TB_MOVIMENTO_ITEM_DESD.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_DESD : null,
                                                                          PrecoUnit = (rowTabela.TB_MOVIMENTO_ITEM_PRECO_UNIT.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_PRECO_UNIT : null,
                                                                          QtdeLiq = (rowTabela.TB_MOVIMENTO_ITEM_QTDE_LIQ.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_QTDE_LIQ : null,
                                                                          QtdeMov = (rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_QTDE_MOV : null,
                                                                          SaldoQtde = (rowTabela.TB_MOVIMENTO_ITEM_SALDO_QTDE.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_SALDO_QTDE : null,
                                                                          SaldoQtdeLote = (rowTabela.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE : null,
                                                                          SaldoValor = (rowTabela.TB_MOVIMENTO_ITEM_SALDO_VALOR.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_SALDO_VALOR : null,
                                                                          ValorMov = (rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV.HasValue) ? rowTabela.TB_MOVIMENTO_ITEM_VALOR_MOV : null,
                                                                          Movimento = new MovimentoEntity
                                                                          {
                                                                              Id = rowTabela.TB_MOVIMENTO_ID,
                                                                              Empenho = rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                              NumeroDocumento = rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                              DataDocumento = rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                              Ativo = rowTabela.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO
                                                                          },
                                                                          ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                          where ItemMaterial.TB_ITEM_MATERIAL_ID == rowTabela.TB_ITEM_MATERIAL_ID
                                                                                          select new ItemMaterialEntity
                                                                                          {
                                                                                              Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                              Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                              Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                          }).FirstOrDefault(),
                                                                          SubItemMaterial = (from SubItemMaterial in this.Db.TB_SUBITEM_MATERIALs
                                                                                             where SubItemMaterial.TB_SUBITEM_MATERIAL_ID == rowTabela.TB_SUBITEM_MATERIAL_ID
                                                                                             select new SubItemMaterialEntity
                                                                                             {
                                                                                                 Id = SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                                                 Codigo = SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                                 Descricao = SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                                             }).FirstOrDefault(),
                                                                          UGE = new UGEEntity(rowTabela.TB_UGE_ID),
                                                                      }));

            this.totalregistros = lstRetorno.Count;

            return lstRetorno;
        }

        IList<MovimentoItemEntity> ICrudBaseService<MovimentoItemEntity>.ListarTodosCod()
        {
            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         orderby a.TB_MOVIMENTO_ITEM_ID
                                                         select new MovimentoItemEntity
                                                         {
                                                             Id = a.TB_MOVIMENTO_ID,
                                                             Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                             DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                             Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                             FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                             IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                             Movimento = new MovimentoEntity
                                                             {
                                                                 Id = a.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                 Empenho = a.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO,
                                                                 NumeroDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                 DataDocumento = a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                 Ativo = a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO
                                                             },
                                                             PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                             QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                             QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                             SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                             SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                             SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                             SubItemMaterial = new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO
                                                             },
                                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                                             ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,

                                                         }).AsQueryable<MovimentoItemEntity>();

            resultado = resultado.Where(a => a.Movimento.Ativo == Entity.Movimento.Ativo);

            if (Entity.SubItemMaterial != null)
                resultado = resultado.Where(a => a.SubItemMaterial.Id == Entity.SubItemMaterial.Id);

            if (Entity.Movimento.Empenho != null && Entity.Movimento.Empenho != "")
                resultado = resultado.Where(a => a.Movimento.Empenho == Entity.Movimento.Empenho);

            IList<MovimentoItemEntity> result = resultado.ToList<MovimentoItemEntity>();

            return result;
        }

        public string SelectUnidFornecimentoSiafisico(int codUnidade)
        {
            return (from a in Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                    where a.TB_UNIDADE_FORNECIMENTO_CODIGO == codUnidade
                    select a.TB_UNIDADE_FORNECIMENTO_DESCRICAO)
                    .FirstOrDefault();
        }

        public void AtualizarItensMovimentacaoComNotaSIAF(int movItemID, string nlLiquidacao, Enum @TipoNotaSIAFEM, Enum @TipoLancamentoSIAFEM)
        {
            if (movItemID <= 0)
                throw new ArgumentException("ID informado deve ser maior que zero!");

            if (String.IsNullOrWhiteSpace(nlLiquidacao))
                throw new ArgumentException("NL Liquidação não informada!");

            TB_MOVIMENTO_ITEM rowMovimentoItem = null;
            var _tipoNotaSIAFEM = (TipoNotaSIAFEM)@TipoNotaSIAFEM;
            var _tipoLancamentoSIAFEM = (TipoLancamentoSIAFEM)@TipoLancamentoSIAFEM;

            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    rowMovimentoItem = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ITEM_ID == movItemID).FirstOrDefault();

                    if (rowMovimentoItem.IsNotNull())
                    {
                        //NL LIQUIDACAO
                        if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Normal)
                            rowMovimentoItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = nlLiquidacao;
                        else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Estorno)
                            rowMovimentoItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO = nlLiquidacao;
                        //NL RECLASSIFICACAO
                        else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Normal)
                            rowMovimentoItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO = nlLiquidacao;
                        else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Estorno)
                            rowMovimentoItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO = nlLiquidacao;
                        //NL CONSUMO
                        else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Consumo)
                            rowMovimentoItem.TB_MOVIMENTO_ITEM_NL_CONSUMO = nlLiquidacao;


                        this.Db.SubmitChanges();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Erro ao atualizar dados de nota de lançamento de item de movimentação.");
                }

                ts.Complete();
            }
        }

        public void AlterarMovimentoBloquear(int movimentoId, bool bloquer)
        {
            TB_MOVIMENTO rowTabela = null;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    rowTabela = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == movimentoId).FirstOrDefault();

                    if (rowTabela.IsNotNull())
                    {
                        rowTabela.TB_MOVIMENTO_BLOQUEAR = bloquer;

                        this.Db.SubmitChanges();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Erro ao atualizar dado de movimentação.");
                }

                ts.Complete();
            }

        }
        public void AtualizarItensMovimentacaoComNotaSIAF(int[] movItemIDs, string nlLiquidacao, Enum @TipoNotaSIAFEM, Enum @TipoLancamentoSIAFEM)
        {
            if (String.IsNullOrWhiteSpace(nlLiquidacao) && ((GeralEnum.TipoNotaSIAF)@TipoNotaSIAFEM != GeralEnum.TipoNotaSIAF.NL_Consumo))
                throw new ArgumentException("NL Lançamento não informada!");

            if (movItemIDs.Contains(0))
                throw new ArgumentException("Lista de ID's informada contém chave de valor zero!");



            if (movItemIDs.IsNotNullAndNotEmpty())
            {
                var _tipoNotaSIAFEM = (TipoNotaSIAFEM)TipoNotaSIAFEM;
                var _tipoLancamentoSIAFEM = (TipoLancamentoSIAFEM)TipoLancamentoSIAFEM;
                EntitySet<TB_MOVIMENTO_ITEM> movItems = new EntitySet<TB_MOVIMENTO_ITEM>();
                TB_MOVIMENTO_ITEM rowTabela = null;

                try
                {
                    foreach (var movItemID in movItemIDs)
                    {
                        rowTabela = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ITEM_ID == movItemID).FirstOrDefault();
                        if (rowTabela.IsNotNull())
                        {
                            //NL LIQUIDACAO
                            if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Normal)
                                rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = nlLiquidacao;
                            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Estorno)
                                rowTabela.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO = nlLiquidacao;
                            //NL RECLASSIFICACAO
                            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Normal)
                                rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO = nlLiquidacao;
                            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao && _tipoLancamentoSIAFEM == GeralEnum.TipoLancamentoSIAF.Estorno)
                                rowTabela.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO = nlLiquidacao;
                            //NL CONSUMO
                            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Consumo)
                                rowTabela.TB_MOVIMENTO_ITEM_NL_CONSUMO = nlLiquidacao;

                            movItems.Add(rowTabela);
                        }
                    }

                    this.Db.SubmitChanges();
                }
                catch (Exception)
                {
                    throw new Exception("Erro ao atualizar dados de nota de lançamento de item de movimentação.");
                }
            }
        }

        public string ObterNumerosDocumentoPorMovimentoItemIDs(IList<int> arrMovItemIDs)
        {
            IList<string> lstRetorno = null; ;
            string strRetorno = null;


            lstRetorno = new List<string>();
            foreach (var movItemID in arrMovItemIDs)
            {
                lstRetorno.Add(Db.TB_MOVIMENTO_ITEMs.AsQueryable()
                                                    .Where(rowMovItem => rowMovItem.TB_MOVIMENTO_ITEM_ID == movItemID)
                                                    .Select(rowMovItem => rowMovItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                                    .FirstOrDefault());
            }


            lstRetorno = lstRetorno.Distinct()
                                   .ToList();

            strRetorno = String.Join("\n", lstRetorno);
            return strRetorno;
        }
        /// <summary>
        /// Método para obter as NL's SIAFEM/CONSUMO geradas para pagar os itens da movimentação.
        /// </summary>
        /// <param name="movimentacaoMaterialID">ID da movimentação de material a consultar os itens.</param>
        /// <param name="TipoNotaSIAFEM">Se tipo de NL consultada será NL SIAFEM, ou NL CONSUMO</param>
        /// <param name="retornaNLEstorno">Se utilizado, parâmetro fará método retornar as NL's de estorno (caso TipoNotaSIAFEM seja tipo 'NL')</param>
        /// <returns></returns>
        public IList<string> ObterNLsMovimentacao(int movimentacaoMaterialID, Enum @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false)
        {
            List<string> notasLancamentoMovimentacao = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere = null;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereMovItemLiquidado = null;
            Expression<Func<TB_MOVIMENTO_ITEM, string>> expCampoInfoNL = null;
            var _tipoNotaSIAFEM = (TipoNotaSIAFEM)@TipoNotaSIAFEM;


            //filtro básico
            expWhere = (_movimentacao => _movimentacao.TB_MOVIMENTO_ID == movimentacaoMaterialID);

            //Se NL SIAFEM
            if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao)
            {
                if (!retornaNLEstorno)
                {
                    expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                            || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty)));

                    expCampoInfoNL = (movItem => movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO);
                }
                else
                {
                    expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO != null
                                                            || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO != string.Empty)));

                    expCampoInfoNL = (movItem => movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO);
                }
            }
            //Se NL CONSUMO
            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Consumo)
            {
                expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO != null
                                                         || _movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO != string.Empty)));

                expCampoInfoNL = (movItem => movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO);
            }
            //Se NL RECLASSIFICACAO SIAFEM
            if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao)
            {
                if (!retornaNLEstorno)
                {
                    expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO != null
                                                            || _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO != string.Empty)));

                    expCampoInfoNL = (movItem => movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO);
                }
                else
                {
                    expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO != null
                                                            || _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO != string.Empty)));

                    expCampoInfoNL = (movItem => movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO);
                }
            }



            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            notasLancamentoMovimentacao = qryConsulta.Where(expWhere)
                                                     .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                                     .Where(expWhereMovItemLiquidado)
                                                     .Select(expCampoInfoNL)
                                                     .ToList();

            notasLancamentoMovimentacao = notasLancamentoMovimentacao.Distinct()
                                                                     .ToList();

            return notasLancamentoMovimentacao;
        }
    }
}
