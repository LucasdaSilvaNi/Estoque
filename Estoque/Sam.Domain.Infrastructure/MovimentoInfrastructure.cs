using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Transactions;
using System.Data.Common;
using Sam.Common.LambdaExpression;
using enumTipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using Sam.Domain.Entity.DtoWs;
using static Sam.Common.Util.GeralEnum;
using System.Data.SqlClient;




namespace Sam.Domain.Infrastructure
{
    public partial class MovimentoInfrastructure : BaseInfraestructure, IMovimentoService
    {
        MovimentoItemEntity lista;
        private MovimentoEntity Movimento = new MovimentoEntity();
        internal readonly string fmtFracionarioMaterialQtde = "#,#0.000";
        private bool entradaPorEmpenho = false;
        public void SetEntradaPorEmpenho()
        {
            this.entradaPorEmpenho = true;
        }
        public MovimentoEntity Entity
        {
            get { return Movimento; }
            set { Movimento = value; }
        }

        public IList<MovimentoEntity> Listar()
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
                                                }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoEntity> Listar(string Documento)
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                where a.TB_MOVIMENTO_NUMERO_DOCUMENTO == Documento
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
                                                }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<MovimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where a.TB_MOVIMENTO_NUMERO_DOCUMENTO == Documento
                                   orderby a.TB_MOVIMENTO_ID
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoEntity> Listar(int MovimentoId)
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
                                                    EmpenhoEvento = new EmpenhoEventoEntity(a.TB_EMPENHO_EVENTO_ID),
                                                    EmpenhoLicitacao = new EmpenhoLicitacaoEntity(a.TB_EMPENHO_LICITACAO_ID),
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
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

        /// <summary>
        /// Método para retornar requisições desconsiderando o filtro por mês de referência do almoxarifado desejado.
        /// </summary>
        /// <param name="almoxarifadoId"></param>
        /// <param name="divisaoId"></param>
        /// <param name="tipoMovimento"></param>
        /// <param name="desconsiderarMesRef"></param>
        /// <param name="mesRef"></param>
        /// <returns></returns>
        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string dataDigitada)
        {
            Expression<Func<TB_MOVIMENTO, bool>> expClausulaWhere = null;
            TB_TIPO_MOVIMENTO rowTipoMovimento = null;
            TB_DIVISAO rowDivisaoAlmox = null;
            IList<MovimentoEntity> lstRetorno = null;

            if (!string.IsNullOrEmpty(dataDigitada) && tipoMovimento == (int)GeralEnum.TipoMovimento.RequisicaoPendente)
            {
                DateTime dataDig = new DateTime();
                dataDig = Convert.ToDateTime(dataDigitada + " 23:59:00.193");
                expClausulaWhere = Movimentacao => ((Movimentacao.TB_DIVISAO_ID == divisaoId || divisaoId == 0) &&
                                                (Movimentacao.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0) &&
                                                (Movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovimento) &&
                                                (Movimentacao.TB_MOVIMENTO_ATIVO == true) && Movimentacao.TB_MOVIMENTO_DATA_DOCUMENTO <= dataDig &&
                                                (Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != null || Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != "" || Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != " "));
            }
            else
            {
                expClausulaWhere = Movimentacao => ((Movimentacao.TB_DIVISAO_ID == divisaoId || divisaoId == 0) &&
                                                    (Movimentacao.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0) &&
                                                    (Movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovimento) &&
                                                    (Movimentacao.TB_MOVIMENTO_ATIVO == true) &&
                                                    (Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != null || Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != "" || Movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO != " "));

            }
            IQueryable<TB_MOVIMENTO> qryConsulta = this.Db.TB_MOVIMENTOs.Where(expClausulaWhere).OrderByDescending(Movimentacao => Movimentacao.TB_MOVIMENTO_ID);

            this.totalregistros = qryConsulta.Count();

            string strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            if (this.totalregistros > 0)
            {
                lstRetorno = new List<MovimentoEntity>();

                rowTipoMovimento = this.Db.TB_TIPO_MOVIMENTOs.Where(TipoMovimento => TipoMovimento.TB_TIPO_MOVIMENTO_ID == tipoMovimento).FirstOrDefault();
                rowDivisaoAlmox = this.Db.TB_DIVISAOs.Where(DivisaoAlmoxarifado => DivisaoAlmoxarifado.TB_DIVISAO_ID == divisaoId).FirstOrDefault();

                qryConsulta.ToList().ForEach(linhaTabela => lstRetorno.Add(materializarRequisicao(linhaTabela, rowTipoMovimento, rowDivisaoAlmox)));
            }

            return lstRetorno;
        }

        /// <summary>
        /// Método retornante de objeto MovimentoEnity preenchido com os dados da linha (row) da tabela TB_MOVIMENTO passada como parãmetro
        /// </summary>
        /// <param name="rowTabela"></param>
        /// <param name="rowTipoMovimento"></param>
        /// <param name="rowDivisaoAlmoxarifado"></param>
        /// <returns></returns>
        internal MovimentoEntity materializarRequisicao(TB_MOVIMENTO rowTabela, TB_TIPO_MOVIMENTO rowTipoMovimento, TB_DIVISAO rowDivisaoAlmoxarifado)
        {
            MovimentoEntity objRetorno = null;
            TipoMovimentoEntity objTipoMovimento = null;
            DivisaoEntity objDivisao = null;

            if (!rowTabela.IsNull())
            {
                objRetorno = new MovimentoEntity(rowTabela.TB_MOVIMENTO_ID)
                {
                    GeradorDescricao = rowTabela.TB_MOVIMENTO_GERADOR_DESCRICAO,
                    NumeroDocumento = rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                    AnoMesReferencia = rowTabela.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                    DataDocumento = rowTabela.TB_MOVIMENTO_DATA_DOCUMENTO,
                    DataMovimento = rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO,
                    FonteRecurso = rowTabela.TB_MOVIMENTO_FONTE_RECURSO,
                    ValorDocumento = rowTabela.TB_MOVIMENTO_VALOR_DOCUMENTO,
                    Observacoes = rowTabela.TB_MOVIMENTO_OBSERVACOES,
                    Instrucoes = rowTabela.TB_MOVIMENTO_INSTRUCOES,
                    Empenho = rowTabela.TB_MOVIMENTO_EMPENHO,
                    Ativo = rowTabela.TB_MOVIMENTO_ATIVO,
                    Almoxarifado = new AlmoxarifadoEntity(rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID),
                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(rowTabela.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                };

                if (!rowTipoMovimento.IsNull())
                {
                    objTipoMovimento = new TipoMovimentoEntity(rowTipoMovimento.TB_TIPO_MOVIMENTO_ID)
                    {
                        AgrupamentoId = rowTipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                        Codigo = rowTipoMovimento.TB_TIPO_MOVIMENTO_CODIGO,
                        Descricao = rowTipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO
                    };
                }

                if (!rowDivisaoAlmoxarifado.IsNull())
                    objDivisao = new DivisaoEntity(rowDivisaoAlmoxarifado.TB_DIVISAO_ID)
                    {
                        Descricao = rowDivisaoAlmoxarifado.TB_DIVISAO_DESCRICAO,
                        Codigo = rowDivisaoAlmoxarifado.TB_DIVISAO_CODIGO,
                        Almoxarifado = new AlmoxarifadoEntity(rowDivisaoAlmoxarifado.TB_ALMOXARIFADO_ID),
                    };
            }

            objRetorno.TipoMovimento = objTipoMovimento;
            objRetorno.Divisao = objDivisao;

            return objRetorno;
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef, string dataDigitada)
        {
            if (String.IsNullOrWhiteSpace(mesRef))
                return this.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, dataDigitada);

            string formatAnoMesRef = "01/" + mesRef.Substring(4, 2) + "/" + mesRef.Substring(0, 4);
            string AnoMesRefMenos1 = Convert.ToDateTime(formatAnoMesRef).AddMonths(-1).ToString("yyyyMM");

            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                where (a.TB_DIVISAO_ID == divisaoId || divisaoId == 0)
                                                where (a.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0)
                                                where (a.TB_TIPO_MOVIMENTO_ID == tipoMovimento)
                                                where (a.TB_MOVIMENTO_NUMERO_DOCUMENTO != "")
                                                where (a.TB_MOVIMENTO_ATIVO == true)
                                                // Listar os documentos pendentes do mês de ref atual do almoxarifado ou do Mês anterior.
                                                where (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesRef) //|| (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == AnoMesRefMenos1))
                                                orderby a.TB_MOVIMENTO_NUMERO_DOCUMENTO descending
                                                select new MovimentoEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
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
                                                    Ativo = a.TB_MOVIMENTO_ATIVO,
                                                    SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,
                                                    Almoxarifado = new AlmoxarifadoEntity() { Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Gestor = ((a.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull()) ? (new GestorEntity() { Id = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }) : null) },
                                                    UGE = ((a.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = a.TB_UGE.TB_UGE_ID, Codigo = a.TB_UGE.TB_UGE_CODIGO, Descricao = a.TB_UGE.TB_UGE_DESCRICAO }) : null),
                                                    IdLogin = a.TB_LOGIN_ID,
                                                    IdLoginEstorno = a.TB_LOGIN_ID_ESTORNO,
                                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                    TipoMovimento = (from tpMov in this.Db.TB_TIPO_MOVIMENTOs
                                                                     where a.TB_TIPO_MOVIMENTO_ID == tpMov.TB_TIPO_MOVIMENTO_ID
                                                                     select new TipoMovimentoEntity
                                                                     {
                                                                         Id = tpMov.TB_TIPO_MOVIMENTO_ID,
                                                                         AgrupamentoId = tpMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                         Codigo = tpMov.TB_TIPO_MOVIMENTO_CODIGO,
                                                                         CodigoDescricao = tpMov.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                         Descricao = tpMov.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                         SubTipoMovimentoItem = (from subtipo in Db.TB_SUBTIPO_MOVIMENTOs
                                                                                                 where subtipo.TB_SUBTIPO_MOVIMENTO_ID == a.TB_SUBTIPO_MOVIMENTO_ID
                                                                                                 select new SubTipoMovimentoEntity
                                                                                                 {
                                                                                                     Id = subtipo.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                                     Descricao = subtipo.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                                 }).FirstOrDefault()
                                                                     }).FirstOrDefault(),

                                                    Divisao = (from div in this.Db.TB_DIVISAOs
                                                               where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                               select new DivisaoEntity()
                                                               {
                                                                   Id = div.TB_DIVISAO_ID,
                                                                   Descricao = div.TB_DIVISAO_DESCRICAO
                                                               }).First<DivisaoEntity>(),

                                                })
                               .ToList<MovimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where (a.TB_DIVISAO_ID == divisaoId || divisaoId == 0)
                                   && (a.TB_TIPO_MOVIMENTO_ID == tipoMovimento)
                                   && a.TB_MOVIMENTO_NUMERO_DOCUMENTO != null
                                   && (a.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0)
                                   where (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesRef)//|| (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == AnoMesRefMenos1))
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef, bool requisicoesParaEstorno, string dataDigitada)
        {
            if (!requisicoesParaEstorno)
                return ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, dataDigitada);


            int AnoMesDataReferenciaAlmox = Int32.Parse(mesRef.Substring(0, 4));
            int MesDataReferenciaAlmox = Int32.Parse(mesRef.Substring(4, 2));
            DateTime? dtMesRefAlmox = new DateTime(AnoMesDataReferenciaAlmox, MesDataReferenciaAlmox, 01);
            DateTime? dtMesRefAlmoxMaisUm = dtMesRefAlmox.Value.AddMonths(1);


            IQueryable<MovimentoEntity> lQryConsulta = (from Movimento in this.Db.TB_MOVIMENTOs
                                                        where (Movimento.TB_DIVISAO_ID == divisaoId || divisaoId == 0)
                                                        where (Movimento.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0)
                                                        where (Movimento.TB_TIPO_MOVIMENTO_ID == tipoMovimento)
                                                        where (Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO != "")
                                                        where (Movimento.TB_MOVIMENTO_ATIVO == true)
                                                        // Listar os documentos pendentes do mês de ref atual do almoxarifado.
                                                        where (Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesRef)
                                                        where (Movimento.TB_MOVIMENTO_DATA_MOVIMENTO >= dtMesRefAlmox)
                                                        where (Movimento.TB_MOVIMENTO_DATA_MOVIMENTO < dtMesRefAlmoxMaisUm)
                                                        orderby Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO descending
                                                        select new MovimentoEntity
                                                        {
                                                            Id = Movimento.TB_MOVIMENTO_ID,
                                                            GeradorDescricao = Movimento.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                            NumeroDocumento = Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                            AnoMesReferencia = Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                            DataDocumento = Movimento.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                            DataMovimento = Movimento.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                            FonteRecurso = Movimento.TB_MOVIMENTO_FONTE_RECURSO,
                                                            ValorDocumento = Movimento.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                            Observacoes = Movimento.TB_MOVIMENTO_OBSERVACOES,
                                                            Instrucoes = Movimento.TB_MOVIMENTO_INSTRUCOES,
                                                            Empenho = Movimento.TB_MOVIMENTO_EMPENHO,
                                                            Ativo = Movimento.TB_MOVIMENTO_ATIVO,
                                                            Almoxarifado = new AlmoxarifadoEntity() { Id = Movimento.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Gestor = ((Movimento.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull()) ? (new GestorEntity() { Id = Movimento.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = Movimento.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }) : null) },
                                                            UGE = ((Movimento.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = Movimento.TB_UGE.TB_UGE_ID, Codigo = Movimento.TB_UGE.TB_UGE_CODIGO, Descricao = Movimento.TB_UGE.TB_UGE_DESCRICAO }) : null),
                                                            IdLogin = Movimento.TB_LOGIN_ID,
                                                            IdLoginEstorno = Movimento.TB_LOGIN_ID_ESTORNO,
                                                            SubTipoMovimentoId = Movimento.TB_SUBTIPO_MOVIMENTO_ID,
                                                            MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                            TipoMovimento = (from tpMov in this.Db.TB_TIPO_MOVIMENTOs
                                                                             where Movimento.TB_TIPO_MOVIMENTO_ID == tpMov.TB_TIPO_MOVIMENTO_ID
                                                                             select new TipoMovimentoEntity
                                                                             {
                                                                                 Id = tpMov.TB_TIPO_MOVIMENTO_ID,
                                                                                 AgrupamentoId = tpMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                                 Codigo = tpMov.TB_TIPO_MOVIMENTO_CODIGO,
                                                                                 CodigoDescricao = tpMov.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                                 Descricao = tpMov.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                                 SubTipoMovimentoItem = (from subtipo in Db.TB_SUBTIPO_MOVIMENTOs
                                                                                                         where subtipo.TB_SUBTIPO_MOVIMENTO_ID == Movimento.TB_SUBTIPO_MOVIMENTO_ID
                                                                                                         select new SubTipoMovimentoEntity
                                                                                                         {
                                                                                                             Id = subtipo.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                                             Descricao = subtipo.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                                         }).FirstOrDefault()
                                                                             }).FirstOrDefault<TipoMovimentoEntity>(),
                                                            Divisao = (from div in this.Db.TB_DIVISAOs
                                                                       where Movimento.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                                       select new DivisaoEntity()
                                                                       {
                                                                           Id = div.TB_DIVISAO_ID,
                                                                           Descricao = div.TB_DIVISAO_DESCRICAO
                                                                       }).FirstOrDefault<DivisaoEntity>(),
                                                        }).AsQueryable<MovimentoEntity>();

            string strSQL = lQryConsulta.ToString();
            Db.GetCommand(lQryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            this.totalregistros = lQryConsulta.Count();

            return lQryConsulta.ToList();
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int tipoMovimento, string dataDigitada)
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                where (a.TB_ALMOXARIFADO_ID == almoxarifadoId || almoxarifadoId == 0)
                                                && (a.TB_TIPO_MOVIMENTO_ID == tipoMovimento)
                                                && (a.TB_MOVIMENTO_NUMERO_DOCUMENTO != "")
                                                orderby a.TB_MOVIMENTO_ID
                                                select new MovimentoEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
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
                                                    Ativo = a.TB_MOVIMENTO_ATIVO,
                                                    Almoxarifado = new AlmoxarifadoEntity() { Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Gestor = ((a.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull()) ? (new GestorEntity() { Id = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }) : null) },
                                                    UGE = ((a.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = a.TB_UGE.TB_UGE_ID, Codigo = a.TB_UGE.TB_UGE_CODIGO, Descricao = a.TB_UGE.TB_UGE_DESCRICAO }) : null),
                                                    IdLogin = a.TB_LOGIN_ID,
                                                    IdLoginEstorno = a.TB_LOGIN_ID_ESTORNO,
                                                    SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,
                                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                    Divisao = (from div in this.Db.TB_DIVISAOs
                                                               where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                               select new DivisaoEntity()
                                                               {
                                                                   Id = div.TB_DIVISAO_ID,
                                                                   Descricao = div.TB_DIVISAO_DESCRICAO
                                                               }).First<DivisaoEntity>(),

                                                }).ToList<MovimentoEntity>();

            return resultado;
        }

        public IList<MovimentoEntity> ListarRequisicaoByDivisao(int divisaoId)
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                where (a.TB_DIVISAO_ID == divisaoId)
                                                && ((a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                || (a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada)
                                                || (a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada))
                                                where (a.TB_MOVIMENTO_ATIVO == true)
                                                orderby a.TB_MOVIMENTO_ID
                                                select new MovimentoEntity
                                                {
                                                    Id = a.TB_MOVIMENTO_ID,
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
                                                    Ativo = a.TB_MOVIMENTO_ATIVO,
                                                    Almoxarifado = new AlmoxarifadoEntity() { Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Gestor = ((a.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull()) ? (new GestorEntity() { Id = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }) : null) },
                                                    UGE = ((a.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = a.TB_UGE.TB_UGE_ID, Codigo = a.TB_UGE.TB_UGE_CODIGO, Descricao = a.TB_UGE.TB_UGE_DESCRICAO }) : null),
                                                    IdLogin = a.TB_LOGIN_ID,
                                                    IdLoginEstorno = a.TB_LOGIN_ID_ESTORNO,
                                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                    Divisao = (from div in this.Db.TB_DIVISAOs
                                                               where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                               select new DivisaoEntity()
                                                               {
                                                                   Id = div.TB_DIVISAO_ID,
                                                                   Descricao = div.TB_DIVISAO_DESCRICAO
                                                               }).First<DivisaoEntity>(),
                                                    TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                                                     where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                                                     select new TipoMovimentoEntity()
                                                                     {
                                                                         Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                                         Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                                         (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                                         ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                                         : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                                                     }).First<TipoMovimentoEntity>()

                                                }).Skip(this.SkipRegistros)
                               .Take(this.RegistrosPagina)
                               .ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where (a.TB_DIVISAO_ID == divisaoId)
                                                && ((a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                 || (a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada)
                                                || (a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada))
                                   where (a.TB_MOVIMENTO_ATIVO == true)
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<MovimentoEntity> ListarRequisicaoByExpression(Expression<Func<MovimentoEntity, bool>> _where)
        {
            IList<MovimentoEntity> resultado = null;

            resultado = (from a in this.Db.TB_MOVIMENTOs
                         orderby a.TB_MOVIMENTO_ID
                         select new MovimentoEntity
                         {
                             Id = a.TB_MOVIMENTO_ID,
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
                             Ativo = a.TB_MOVIMENTO_ATIVO,
                             Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                             MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                             Divisao = (from div in this.Db.TB_DIVISAOs
                                        where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                        select new DivisaoEntity()
                                        {
                                            Id = div.TB_DIVISAO_ID,
                                            Descricao = div.TB_DIVISAO_DESCRICAO
                                        }).First<DivisaoEntity>() ?? new DivisaoEntity(0),
                             TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                              where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                              select new TipoMovimentoEntity()
                                              {
                                                  Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                  Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                  (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                  ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                  : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                              }).First<TipoMovimentoEntity>()

                         })
                                                .Where(_where)
                                                .Skip(this.SkipRegistros)
                                                .Take(this.RegistrosPagina)
                                                .ToList();

            //lStrSQL = lObjEnumRetorno.ToString();    
            //Db.GetCommand(lObjEnumRetorno as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   orderby a.TB_MOVIMENTO_ID
                                   select new MovimentoEntity
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                       NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                       Ativo = a.TB_MOVIMENTO_ATIVO,
                                       Divisao = (from div in this.Db.TB_DIVISAOs
                                                  where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                  select new DivisaoEntity()
                                                  {
                                                      Id = div.TB_DIVISAO_ID,
                                                      Descricao = div.TB_DIVISAO_DESCRICAO
                                                  }).First<DivisaoEntity>() ?? new DivisaoEntity(0),
                                       TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                                        where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                                        select new TipoMovimentoEntity()
                                                        {
                                                            Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                            Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                            (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                            ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                            : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                                        }).First<TipoMovimentoEntity>()

                                   })
                                                .Where(_where)
                                                .ToList()
                                                .Count;

            return resultado;
        }

        public IList<MovimentoEntity> ListarNotasdeFornecimento(Expression<Func<MovimentoEntity, bool>> _where)
        {
            IList<MovimentoEntity> resultado = null;

            resultado = (from a in this.Db.TB_MOVIMENTOs
                         orderby a.TB_MOVIMENTO_ID
                         select new MovimentoEntity
                         {
                             Id = a.TB_MOVIMENTO_ID,
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
                             Ativo = a.TB_MOVIMENTO_ATIVO,
                             Almoxarifado = (from alm in this.Db.TB_ALMOXARIFADOs
                                             where a.TB_ALMOXARIFADO_ID == alm.TB_ALMOXARIFADO_ID
                                             select new AlmoxarifadoEntity()
                                             {
                                                 Id = alm.TB_ALMOXARIFADO_ID
                                             }).First<AlmoxarifadoEntity>() ?? new AlmoxarifadoEntity(0),
                             //Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                             MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                             Divisao = (from div in this.Db.TB_DIVISAOs
                                        where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                        select new DivisaoEntity()
                                        {
                                            Id = div.TB_DIVISAO_ID,
                                            Descricao = div.TB_DIVISAO_DESCRICAO
                                        }).First<DivisaoEntity>() ?? new DivisaoEntity(0),
                             TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                              where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                              select new TipoMovimentoEntity()
                                              {
                                                  Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                  Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                  (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                  ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                  : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                              }).First<TipoMovimentoEntity>()

                         })
                                                .Where(_where)
                                                .Skip(this.SkipRegistros)
                                                .Take(this.RegistrosPagina)
                                                .ToList();

            //lStrSQL = lObjEnumRetorno.ToString();    
            //Db.GetCommand(lObjEnumRetorno as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   orderby a.TB_MOVIMENTO_ID
                                   select new MovimentoEntity
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                       NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                       AnoMesReferencia = a.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                       Ativo = a.TB_MOVIMENTO_ATIVO,
                                       Almoxarifado = (from alm in this.Db.TB_ALMOXARIFADOs
                                                       where a.TB_ALMOXARIFADO_ID == alm.TB_ALMOXARIFADO_ID
                                                       select new AlmoxarifadoEntity()
                                                       {
                                                           Id = alm.TB_ALMOXARIFADO_ID
                                                       }).First<AlmoxarifadoEntity>() ?? new AlmoxarifadoEntity(0),
                                       Divisao = (from div in this.Db.TB_DIVISAOs
                                                  where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                                  select new DivisaoEntity()
                                                  {
                                                      Id = div.TB_DIVISAO_ID,
                                                      Descricao = div.TB_DIVISAO_DESCRICAO
                                                  }).First<DivisaoEntity>() ?? new DivisaoEntity(0),
                                       TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                                        where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                                        select new TipoMovimentoEntity()
                                                        {
                                                            Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                            Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                            (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                            ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                            : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                                        }).First<TipoMovimentoEntity>()

                                   })
                                                .Where(_where)
                                                .ToList()
                                                .Count;

            return resultado;
        }

        //Busca as requisições pelo número do documento ou todos
        public IList<MovimentoEntity> ListarRequisicaoPendente(string palavraChave, int tipoMovimento, int idAlmox, DateTime dataDigitada)
        {
            IList<MovimentoEntity> resultado = null;
            resultado = (from a in this.Db.TB_MOVIMENTOs
                         where (a.TB_MOVIMENTO_NUMERO_DOCUMENTO == palavraChave || palavraChave == "")
                          && a.TB_MOVIMENTO_ATIVO == true
                          && a.TB_ALMOXARIFADO_ID == idAlmox
                          && a.TB_TIPO_MOVIMENTO_ID == tipoMovimento
                          && a.TB_MOVIMENTO_DATA_DOCUMENTO <= Convert.ToDateTime(dataDigitada)
                         orderby a.TB_MOVIMENTO_ID

                         select new MovimentoEntity
                         {
                             Id = a.TB_MOVIMENTO_ID,
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
                             Ativo = a.TB_MOVIMENTO_ATIVO,
                             Almoxarifado = (from alm in this.Db.TB_ALMOXARIFADOs
                                             where a.TB_ALMOXARIFADO_ID == alm.TB_ALMOXARIFADO_ID
                                             select new AlmoxarifadoEntity()
                                             {
                                                 Id = alm.TB_ALMOXARIFADO_ID
                                             }).First<AlmoxarifadoEntity>() ?? new AlmoxarifadoEntity(0),
                             //Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                             MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                             Divisao = (from div in this.Db.TB_DIVISAOs
                                        where a.TB_DIVISAO_ID == div.TB_DIVISAO_ID
                                        select new DivisaoEntity()
                                        {
                                            Id = div.TB_DIVISAO_ID,
                                            Descricao = div.TB_DIVISAO_DESCRICAO
                                        }).First<DivisaoEntity>() ?? new DivisaoEntity(0),
                             TipoMovimento = (from mov in this.Db.TB_TIPO_MOVIMENTOs
                                              where a.TB_TIPO_MOVIMENTO_ID == mov.TB_TIPO_MOVIMENTO_ID
                                               && mov.TB_TIPO_MOVIMENTO_ID == tipoMovimento
                                              select new TipoMovimentoEntity()
                                              {
                                                  Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                  Descricao = ((a.TB_TIPO_MOVIMENTO_ID ==
                                                  (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                  ? mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Pendente", "Em Andamento")
                                                  : mov.TB_TIPO_MOVIMENTO_DESCRICAO.Replace("Requisição Finalizada", "Atendida"))
                                              }).First<TipoMovimentoEntity>()

                         })
                                                .Skip(this.SkipRegistros)
                                                .Take(this.RegistrosPagina)
                                                .ToList();
            this.totalregistros = resultado.ToList().Count;

            return resultado;
        }


        public IList<MovimentoEntity> ListarRequisicao(int starRow, int maximoRow, string orgaoId = "", string uoId = "", string ugeId = "", string uaId = "", string divisaoId = "", int statusId = 0, string numeroDocumento = "")
        {

            ISingleResult<SAM_LISTAR_REQUISICAOResult> procedure = Db.SAM_LISTAR_REQUISICAO(orgaoId, uoId, ugeId, uaId, divisaoId, statusId, numeroDocumento);


            var lista = procedure.ToList();
            List<MovimentoEntity> listaRetorno = new List<MovimentoEntity>();

            foreach (SAM_LISTAR_REQUISICAOResult result in lista)
            {
                MovimentoEntity movimento = new MovimentoEntity();
                movimento.Id = result.TB_MOVIMENTO_ID;
                movimento.GeradorDescricao = result.TB_MOVIMENTO_GERADOR_DESCRICAO;
                movimento.NumeroDocumento = result.TB_MOVIMENTO_NUMERO_DOCUMENTO;

                movimento.AnoMesReferencia = result.TB_MOVIMENTO_ANO_MES_REFERENCIA;
                movimento.DataDocumento = result.TB_MOVIMENTO_DATA_DOCUMENTO;
                movimento.DataMovimento = result.TB_MOVIMENTO_DATA_MOVIMENTO;
                movimento.FonteRecurso = result.TB_MOVIMENTO_FONTE_RECURSO;
                movimento.ValorDocumento = result.TB_MOVIMENTO_VALOR_DOCUMENTO;
                movimento.Observacoes = result.TB_MOVIMENTO_OBSERVACOES;
                movimento.Instrucoes = result.TB_MOVIMENTO_INSTRUCOES;
                movimento.Empenho = result.TB_MOVIMENTO_EMPENHO;
                movimento.Ativo = result.TB_MOVIMENTO_ATIVO;
                movimento.Bloquear = result.TB_MOVIMENTO_BLOQUEAR == null ? false : result.TB_MOVIMENTO_BLOQUEAR;

                AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(result.TB_ALMOXARIFADO_ID);
                movimento.Almoxarifado = almoxarifado;
                movimento.Almoxarifado.Id = result.TB_ALMOXARIFADO_ID;

                AlmoxarifadoEntity almoxarifadoOrigemDestino = new AlmoxarifadoEntity(result.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO);
                movimento.MovimAlmoxOrigemDestino = almoxarifadoOrigemDestino;
                movimento.MovimAlmoxOrigemDestino.Id = result.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO;

                DivisaoEntity divisao = new DivisaoEntity(result.TB_DIVISAO_ID);
                movimento.Divisao = divisao;
                movimento.Divisao.Descricao = result.TB_DIVISAO_DESCRICAO;
                movimento.Divisao.Id = result.TB_DIVISAO_ID;

                TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity(result.TB_TIPO_MOVIMENTO_ID);
                movimento.TipoMovimento = tipoMovimento;
                movimento.TipoMovimento.Id = result.TB_TIPO_MOVIMENTO_ID;
                movimento.TipoMovimento.Descricao = result.TB_TIPO_MOVIMENTO_DESCRICAO;

                listaRetorno.Add(movimento);
                movimento = null;
            }
            this.totalregistros = listaRetorno.Count;


            if (listaRetorno.Count > 0)
                if (starRow + maximoRow <= listaRetorno.Count)
                    return listaRetorno.GetRange(starRow, maximoRow);
                else
                    return listaRetorno.GetRange(starRow, listaRetorno.Count - starRow);
            else
                return listaRetorno;



        }

        public MovimentoEntity ListarSolicitacaoMaterial(int UA, int divisao)
        {
            MovimentoEntity resultado = (from a in this.Db.TB_MOVIMENTOs
                                         where a.TB_MOVIMENTO_ID == divisao
                                         orderby a.TB_MOVIMENTO_ID
                                         select new MovimentoEntity
                                         {
                                             Id = a.TB_MOVIMENTO_ID,
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
                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                             Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                             Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                             Ativo = a.TB_MOVIMENTO_ATIVO,
                                             MovimentoItem = (from item in this.Db.TB_MOVIMENTO_ITEMs
                                                              where a.TB_MOVIMENTO_ID == item.TB_MOVIMENTO_ID
                                                              select new MovimentoItemEntity()
                                                              {
                                                                  Id = item.TB_MOVIMENTO_ID,
                                                                  Ativo = item.TB_MOVIMENTO_ITEM_ATIVO,
                                                                  //SubItemMaterial = item.TB_SUBITEM_MATERIAL_ID,
                                                                  Desd = item.TB_MOVIMENTO_ITEM_DESD,
                                                                  QtdeLiq = item.TB_MOVIMENTO_ITEM_QTDE_LIQ,

                                                              }).ToList(),

                                         }).Skip(this.SkipRegistros)
                               .Take(this.RegistrosPagina)
                               .First<MovimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where a.TB_MOVIMENTO_ID == divisao
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
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

        public IList<MovimentoEntity> ListarTodosCod(int MovimentoId)
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
                                                }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   where a.TB_MOVIMENTO_ID == MovimentoId
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoEntity> ListarTodosCodSimplif(MovimentoEntity mov)
        {

            MovimentoItemInfrastructure items = new MovimentoItemInfrastructure();

            IEnumerable<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
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
                                                          Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                          Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                          UGE = new UGEEntity(a.TB_UGE_ID),
                                                          Ativo = a.TB_MOVIMENTO_ATIVO,
                                                      });
            ;
            if (mov.Id.HasValue)
                resultado = resultado.Where(a => a.Id == mov.Id);

            if (mov.Almoxarifado != null && mov.Almoxarifado.Id != 0)
                resultado = resultado.Where(a => a.Almoxarifado.Id == mov.Almoxarifado.Id);

            if (mov.Fornecedor != null)
                resultado = resultado.Where(a => a.Fornecedor.Id == mov.Fornecedor.Id);

            if (mov.MovimAlmoxOrigemDestino != null)
                resultado = resultado.Where(a => a.MovimAlmoxOrigemDestino.Id == mov.MovimAlmoxOrigemDestino.Id);

            if (mov.Divisao != null)
                resultado = resultado.Where(a => a.Divisao.Id == mov.Divisao.Id);

            if (mov.UGE != null)
                resultado = resultado.Where(a => a.UGE.Id == mov.UGE.Id);

            if (mov.NumeroDocumento != null)
                resultado = resultado.Where(a => a.NumeroDocumento == mov.NumeroDocumento);

            if (mov.TipoMovimento != null)
                resultado = resultado.Where(a => a.TipoMovimento.Id == mov.TipoMovimento.Id);
            if (mov.Empenho != null && mov.Empenho != "")
                resultado = resultado.Where(a => a.Empenho == mov.Empenho);
            if (mov.Ativo.HasValue)
                resultado = resultado.Where(a => a.Ativo == mov.Ativo);

            IList<MovimentoEntity> result = resultado.ToList();

            this.totalregistros = result.Count();
            return result;
        }

        public IList<MovimentoEntity> ListarTodosCod()
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
                                                }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoEntity> Listar(System.Linq.Expressions.Expression<Func<MovimentoEntity, bool>> where)
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
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
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO
                                                }).Where(where).ToList();
            return resultado;
        }

        public MovimentoEntity LerRegistro(int MovimentoId)
        {

            MovimentoEntity resultado = (from a in this.Db.TB_MOVIMENTOs
                                         where a.TB_MOVIMENTO_ID == MovimentoId
                                         orderby a.TB_MOVIMENTO_ID
                                         select new MovimentoEntity
                                         {
                                             Id = a.TB_MOVIMENTO_ID,
                                             Almoxarifado = (from b in this.Db.TB_ALMOXARIFADOs
                                                             where a.TB_ALMOXARIFADO_ID == b.TB_ALMOXARIFADO_ID
                                                             select new AlmoxarifadoEntity
                                                             {
                                                                 Id = b.TB_ALMOXARIFADO_ID,
                                                                 Descricao = b.TB_ALMOXARIFADO_DESCRICAO
                                                             }).First()
                                             ,
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
                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                             Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                             Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID) { Nome = a.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = a.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                             MovimAlmoxOrigemDestino = (from b in this.Db.TB_ALMOXARIFADOs
                                                                        where a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == b.TB_ALMOXARIFADO_ID
                                                                        select new AlmoxarifadoEntity
                                                                        {
                                                                            Id = b.TB_ALMOXARIFADO_ID,
                                                                            Descricao = b.TB_ALMOXARIFADO_DESCRICAO
                                                                        }).First(),
                                             Ativo = a.TB_MOVIMENTO_ATIVO,
                                             MovimentoItem = new MovimentoItemInfrastructure().ListarTodosCod(a.TB_MOVIMENTO_ID),
                                             EmpenhoEvento = new EmpenhoEventoEntity
                                             {
                                                 Id = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID,
                                                 Codigo = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO,
                                                 Descricao = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO
                                             },
                                             EmpenhoLicitacao = new EmpenhoLicitacaoEntity
                                             {
                                                 Id = a.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID,
                                                 Descricao = a.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO
                                             }
                                         }).FirstOrDefault();
            return resultado;
        }

        public IList<MovimentoEntity> Imprimir()
        {
            IList<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                orderby a.TB_MOVIMENTO_DATA_MOVIMENTO
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
                                                    Divisao = new DivisaoEntity(a.TB_DIVISAO_ID),
                                                    Fornecedor = new FornecedorEntity(a.TB_FORNECEDOR_ID),
                                                    UGE = new UGEEntity(a.TB_UGE_ID),
                                                    Ativo = a.TB_MOVIMENTO_ATIVO,
                                                    EmpenhoEvento = new EmpenhoEventoEntity
                                                    {
                                                        Id = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID,
                                                        Codigo = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO,
                                                        Descricao = a.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO
                                                    },
                                                    EmpenhoLicitacao = new EmpenhoLicitacaoEntity
                                                    {
                                                        Id = a.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID,
                                                        Descricao = a.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO
                                                    }
                                                }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTOs
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ID,
                                   }).Count();
            return resultado;
        }

        public string NumeroUltimoDocumento()
        {
            var tipoSaida = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida;
            var tipoRequisicao = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Requisicao;

            //IQueryable<MovimentoEntity> query = (from a in this.Db.TB_MOVIMENTOs
            //                                     orderby a.TB_MOVIMENTO_DATA_OPERACAO descending, a.TB_MOVIMENTO_ID descending
            //                                     //where a.TB_MOVIMENTO_NUMERO_DOCUMENTO.Length == 12
            //                                     where (a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida ||
            //                                     a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Requisicao)
            //                                     select new MovimentoEntity
            //                                     {
            //                                         NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO
            //                                     }).Take(1);

            var retorno = Db.TB_MOVIMENTOs.Where(_movimentacao => (_movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoSaida ||
                                                                  _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoRequisicao)
                                                                  && _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO.Substring(0, 4).Contains("2014")) //Pega apenas os registros que iniciam com 2014
                                          .Max(_movimentacao => _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO);

            //var retorno = query.FirstOrDefault();

            return retorno.Remove(0, 4);
        }

        public void Excluir()
        {
            TB_MOVIMENTO mov
                = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_MOVIMENTOs.DeleteOnSubmit(mov);
            this.Db.SubmitChanges();
        }

        public MovimentoItemEntity RetornaPrecoMedioMovimentoItemRetroativo(MovimentoItemEntity movItem)
        {
            string lote = movItem.IdentificacaoLote == null ? string.Empty : movItem.IdentificacaoLote;

            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= Movimento.DataMovimento.Value
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                               || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          select new MovimentoItemEntity
                          {
                              Id = i.TB_MOVIMENTO_ITEM_ID,
                              SubItemMaterial = new SubItemMaterialEntity(i.TB_SUBITEM_MATERIAL_ID),
                              UGE = new UGEEntity(i.TB_UGE_ID),
                              Movimento = (from m in Db.TB_MOVIMENTOs
                                           where m.TB_MOVIMENTO_ID == i.TB_MOVIMENTO_ID
                                           select new MovimentoEntity
                                           {
                                               Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID)
                                           }).First(),
                              SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                              SaldoValor = i.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                              DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                              PrecoUnit = i.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                              Desd = i.TB_MOVIMENTO_ITEM_DESD
                          }).OrderByDescending(a => a.DataMovimento).ThenByDescending(a => a.Id).FirstOrDefault();

            return result;
        }

        public MovimentoItemEntity RetornaPrecoMedioMovimentoItemRetroativoLote(MovimentoItemEntity movItem)
        {
            movItem.IdentificacaoLote = movItem.IdentificacaoLote != null ? movItem.IdentificacaoLote : string.Empty;

            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= Movimento.DataMovimento.Value
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where i.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote
                          where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                               || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          select new MovimentoItemEntity
                          {
                              Id = i.TB_MOVIMENTO_ITEM_ID,
                              SubItemMaterial = new SubItemMaterialEntity(i.TB_SUBITEM_MATERIAL_ID),
                              UGE = new UGEEntity(i.TB_UGE_ID),
                              Movimento = (from m in Db.TB_MOVIMENTOs
                                           where m.TB_MOVIMENTO_ID == i.TB_MOVIMENTO_ID
                                           select new MovimentoEntity
                                           {
                                               Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID)
                                           }).First(),
                              SaldoQtdeLote = i.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                              // SaldoValor = i.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                              DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                              //PrecoUnit = i.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                              DataVencimentoLote = i.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                              IdentificacaoLote = i.TB_MOVIMENTO_ITEM_LOTE_IDENT
                          }).OrderByDescending(a => a.DataMovimento).ThenByDescending(a => a.Id).FirstOrDefault();


            return result;
        }

        public MovimentoItemEntity RetornaMovimentoEntradaDaSaida(MovimentoItemEntity movItemSaida)
        {
            IQueryable<MovimentoItemEntity> query = (from i in Db.TB_MOVIMENTO_ITEMs
                                                     where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= Movimento.DataMovimento.Value
                                                     where i.TB_SUBITEM_MATERIAL_ID == (int)movItemSaida.SubItemMaterial.Id
                                                     where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                                                     where i.TB_UGE_ID == (int)movItemSaida.UGE.Id
                                                     where i.TB_MOVIMENTO_ITEM_ATIVO == true
                                                     where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                                                     orderby i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO descending, i.TB_MOVIMENTO_ITEM_ID descending
                                                     select new MovimentoItemEntity
                                                     {
                                                         Id = i.TB_MOVIMENTO_ITEM_ID,
                                                         SubItemMaterial = new SubItemMaterialEntity(i.TB_SUBITEM_MATERIAL_ID),
                                                         UGE = new UGEEntity(i.TB_UGE_ID),
                                                         SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                         SaldoValor = i.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                         DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                         PrecoUnit = i.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                         QtdeMov = i.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                         ValorMov = i.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                         Movimento = new MovimentoEntity
                                                         {
                                                             Id = i.TB_MOVIMENTO_ID,
                                                             AnoMesReferencia = i.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA
                                                         }
                                                     }).AsQueryable();

            var resultado = query.FirstOrDefault();

            return resultado;
        }

        public void AlterarMovimentoBloquear(int movimentoId, bool bloquear)
        {
            TB_MOVIMENTO rowTabela = null;

            try
            {
                rowTabela = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == movimentoId).FirstOrDefault();

                if (rowTabela.IsNotNull())
                {
                    rowTabela.TB_MOVIMENTO_BLOQUEAR = bloquear;

                    this.Db.SubmitChanges();
                }
            }
            catch (Exception)
            {
                throw new Exception("Erro ao atualizar dado de movimentação.");
            }


        }


        public void SalvarRequisicao(bool pBlnNewMethod)
        {

            try
            {


                if (!this.Db.ObjectTrackingEnabled)
                    this.Db.ObjectTrackingEnabled = true;

                TB_MOVIMENTO mov = new TB_MOVIMENTO();
                EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();
                IList<TB_MOVIMENTO_ITEM> itensParaExclusao = new EntitySet<TB_MOVIMENTO_ITEM>();
                string strNumeroDocumento = String.Empty;

                if (Convert.ToInt64(this.Entity.AnoMesReferencia) >= Convert.ToInt64("201501"))
                {
                    string strAnoDocumento = DateTime.Now.Year.ToString();
                    string strUgeCodigoDescricao = this.Entity.UGE.CodigoDescricao;
                    string strPrefixo = "";

                    if (String.IsNullOrWhiteSpace(this.Entity.NumeroDocumento))
                    {
                        strPrefixo = strAnoDocumento + strUgeCodigoDescricao;
                        strNumeroDocumento = BuscaUltimoDocumento(strPrefixo);
                    }
                    else
                        strNumeroDocumento = this.Entity.NumeroDocumento;
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(this.Entity.NumeroDocumento))
                        strNumeroDocumento = getMaximoNumeroDocumento(this.Entity.AnoMesReferencia);
                    else
                        strNumeroDocumento = this.Entity.NumeroDocumento;
                }

                string strAnoReferencia = string.Empty;
                bool updateEfetuado = false;
                bool isRequisicaoEspelho = false;
                bool isAtualizacaoReq = false;
                bool isNumeroDocumentoExistente = false;

                //Se existir o movimento, irá desativar o atual e gravar um novo
                if (this.Entity.Id.HasValue)
                {

                    //Carrega movimento existente
                    mov = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value && a.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoPendente).FirstOrDefault();

                    if (mov == null)
                        throw new Exception("Requisição não está pendente");
                    else
                    {
                        bool? bloquear = mov.TB_MOVIMENTO_BLOQUEAR = mov.TB_MOVIMENTO_BLOQUEAR != null ? (bool?)mov.TB_MOVIMENTO_BLOQUEAR : false;

                        if ((bool)bloquear)
                            throw new Exception("Operador está efetuado o saída esta requisição.");
                    }

                    if (this.Entity.Id != null)
                        AlterarMovimentoBloquear(this.Entity.Id.Value, true);
                }
                else
                    //apenas insere o novo Movimento
                    this.Db.TB_MOVIMENTOs.InsertOnSubmit(mov);

                int codigoPTRes = 0;
                foreach (MovimentoItemEntity view in this.Entity.MovimentoItem)
                {
                    TB_MOVIMENTO_ITEM itemAdd;

                    //Verifica que o item existe cadastrado
                    if (this.Entity.Id != null)
                    {
                        itemAdd = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value).
                                Where(a => a.TB_MOVIMENTO_ITEM_ID == view.Id.Value).FirstOrDefault();
                    }
                    else
                    {
                        itemAdd = null;
                    }

                    if (itemAdd == null)//Novo Item
                    {
                        itemAdd = new TB_MOVIMENTO_ITEM();
                        this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(itemAdd);
                        itemAdd.TB_MOVIMENTO_ITEM_ID = view.Id.HasValue ? view.Id.Value : 0;
                    }
                    if (view.UGE != null)
                        itemAdd.TB_UGE_ID = view.UGE.Id.HasValue ? view.UGE.Id.Value : 0;

                    itemAdd.TB_MOVIMENTO_ITEM_ATIVO = view.Ativo;
                    itemAdd.TB_MOVIMENTO_ITEM_DESD = view.Desd;

                    if (!(DateTime.MinValue == view.DataVencimentoLote))
                        itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = view.DataVencimentoLote;

                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = view.FabricanteLote;
                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = view.IdentificacaoLote;
                    itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = view.PrecoUnit;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = view.QtdeLiq;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = view.QtdeMov;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE = view.SaldoQtde;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = view.SaldoQtdeLote;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = view.SaldoValor;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = view.ValorMov;
                    itemAdd.TB_SUBITEM_MATERIAL_ID = (int)view.SubItemMaterial.Id;
                    itemAdd.TB_MOVIMENTO_ITEM_ATIVO = true;

                    itemAdd.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = view.NL_Liquidacao;

                    if (view.PTRes.IsNull())
                        throw new Exception("Dados referentes a PTRES obrigatórios para gravação de requisição!");
                    else
                    {
                        itemAdd.TB_PTRES_ID = view.PTRes.Id;


                        if (Int32.TryParse(view.PTResCodigo, out codigoPTRes) && !String.IsNullOrWhiteSpace(view.PTResAcao))
                        {
                            itemAdd.TB_PTRES_CODIGO = codigoPTRes;
                            itemAdd.TB_PTRES_PT_ACAO = view.PTResAcao;
                        }
                        else if (!Int32.TryParse(view.PTResCodigo, out codigoPTRes) || String.IsNullOrWhiteSpace(view.PTResAcao))
                        {
                            throw new Exception("Dados referentes a PTRES obrigatórios para gravação de requisição!");
                        }
                    }

                    //itemAdd.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO = view.CodigoUnidadeFornecimentoSiafisico;
                    item.Add(itemAdd);
                }

                #region Tratamento gravacao historico
                itensParaExclusao = mov.TB_MOVIMENTO_ITEMs.Except(item).ToList();

                if (!updateEfetuado && (mov.TB_MOVIMENTO_ITEMs.Count != item.Count))
                    updateEfetuado = true;

                this.Db.TB_MOVIMENTO_ITEMs.DeleteAllOnSubmit(itensParaExclusao);
                #endregion Tratamento gravacao historico

                mov.TB_MOVIMENTO_ITEMs = item;
                mov.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;

                mov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
                mov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.DataDocumento.Value;
                mov.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value;
                mov.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
                mov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
                mov.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
                mov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
                mov.TB_LOGIN_ID = this.Entity.IdLogin;
                mov.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;
                // mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
                //mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = getMaximoNumeroDocumento(mov.TB_MOVIMENTO_ANO_MES_REFERENCIA);
                mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = strNumeroDocumento;
                mov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
                mov.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.ValorDocumento.Value.truncarDuasCasas();
                mov.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;

                if (this.Entity.UGE != null)
                {
                    if (this.Entity.UGE.Id.HasValue)
                        mov.TB_UGE_ID = this.Entity.UGE.Id.Value;
                    else
                    {
                        mov.TB_UGE_ID = null;
                    }
                }
                else
                {
                    mov.TB_UGE_ID = null;
                }

                if (this.Entity.MovimAlmoxOrigemDestino != null)
                {
                    if (this.Entity.MovimAlmoxOrigemDestino.Id.HasValue)
                        mov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino.Id.Value;
                }

                if (this.Entity.EmpenhoEvento != null)
                {
                    if (this.Entity.EmpenhoEvento.Id.HasValue)
                        mov.TB_EMPENHO_EVENTO_ID = this.Entity.EmpenhoEvento.Id.Value;
                }

                if (this.Entity.EmpenhoLicitacao != null)
                {
                    if (this.Entity.EmpenhoLicitacao.Id.HasValue)
                        mov.TB_EMPENHO_LICITACAO_ID = this.Entity.EmpenhoLicitacao.Id.Value;
                }

                if (this.Entity.Divisao != null)
                {
                    if (this.Entity.Divisao.Id != null)
                        mov.TB_DIVISAO_ID = this.Entity.Divisao.Id.Value;
                }

                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id != null)
                        mov.TB_FORNECEDOR_ID = this.Entity.Fornecedor.Id.Value;
                }

                mov.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;

                //TODO MELHORIA INFRA - DESATIVADO
                //if (updateEfetuado)
                //    GravarMovimentoHistorico(mov);



                this.Db.SubmitChanges();

                if (this.Entity.Id != null)
                    AlterarMovimentoBloquear(this.Entity.Id.Value, false);

                this.Entity.NumeroDocumento = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex.InnerException);
            }


        }
        private String BuscaUltimoDocumento(String Prefixo)
        {
            try
            {

                string numeroDocumento = string.Empty;
                String ultimoDocumento = String.Empty;

                var verificaDocumento = Db.TB_GERA_NUMERO_DOCUMENTOs.Where(x => x.TB_GERA_NUMERO_DOCUMENTO_CODIGO.Substring(0, 10) == Prefixo).FirstOrDefault();
                if (verificaDocumento == null)
                {
                    //Início do ano inicia nova contagem.
                    ultimoDocumento = Db.TB_MOVIMENTOs.Where(_movimentacao => (_movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Saida ||
                                                                 _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Requisicao)
                                                                 && _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO.Substring(0, 10) == Prefixo)
                                         .Max(_movimentacao => _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO);
                }
                else
                {
                    ultimoDocumento = verificaDocumento.TB_GERA_NUMERO_DOCUMENTO_CODIGO;
                }

                //Tratamento para não gerar requisição duplicada
                using (TransactionScope transacao = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var retonoDocumento = string.Empty;
                    if (ultimoDocumento == null)
                    {
                        var numero = Convert.ToInt64(Prefixo + "0001");
                        retonoDocumento = numero.ToString();
                    }
                    else
                    {
                        retonoDocumento = GerarDocumento(ultimoDocumento);
                    }

                    TB_GERA_NUMERO_DOCUMENTO doc = new TB_GERA_NUMERO_DOCUMENTO();
                    var listaDocumento = new TB_GERA_NUMERO_DOCUMENTO();
                    string retorno = string.Empty;

                    TB_GERA_NUMERO_DOCUMENTO documento = new TB_GERA_NUMERO_DOCUMENTO();

                    if (verificaDocumento != null)
                    {
                        verificaDocumento.TB_GERA_NUMERO_DOCUMENTO_CODIGO = retonoDocumento;
                        retorno = retonoDocumento;
                        retonoDocumento = retorno;
                    }
                    else
                    {
                        var documentoUgeIncrementa = retonoDocumento;
                        listaDocumento.TB_GERA_NUMERO_DOCUMENTO_CODIGO = retonoDocumento;
                        this.Db.TB_GERA_NUMERO_DOCUMENTOs.InsertOnSubmit(listaDocumento);
                        retonoDocumento = listaDocumento.TB_GERA_NUMERO_DOCUMENTO_CODIGO;
                    }
                    this.Db.SubmitChanges();
                    transacao.Complete();
                    if (retonoDocumento == "" || retonoDocumento == null)
                    {
                        throw new Exception("Erro ao gerar o numero do Documento.");
                    }

                    return numeroDocumento = retonoDocumento;

                }

            }
            catch (Exception ex)
            {
                var descErro = (ex.InnerException.IsNotNull() ? ex.InnerException.Message : ex.Message);
                throw ex;
            }

        }
        //Tratamento para quando ultrapassar 9999
        private string GerarDocumento(string NumeroDocumento)
        {
            try
            {
                var letra = "";
                var posfixo = "";
                int ultimoDigito = 0;
                string retorno = string.Empty;
                var prefixo = NumeroDocumento.Substring(0, 10);
                var ehLetra = false;


                if (NumeroDocumento.Length == 14)
                {
                    letra = NumeroDocumento.Substring(10, 1);
                    ultimoDigito = int.Parse(NumeroDocumento.Substring(11, 3));
                    posfixo = NumeroDocumento.Substring(10, 4);
                    ehLetra = VerificaString(letra);

                }

                if (ehLetra == true && posfixo != letra + "999")
                {
                    ultimoDigito = (ultimoDigito + 1);
                    var digito = letra + ultimoDigito.ToString().PadLeft(3, '0');
                    retorno = string.Format("{0}{1}", prefixo, digito);
                }
                else if (posfixo == "9999")
                {
                    var sequencial = "A000";
                    retorno = string.Format("{0}{1}", prefixo, sequencial);
                }
                //Caso seja letra + 999              
                else if (posfixo == letra + "999" && letra != "0" && ehLetra == true)
                {
                    string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ@#$%&";
                    int pos = alfabeto.IndexOf(letra);
                    pos++;
                    letra = Convert.ToString(alfabeto[pos]);
                    retorno = prefixo + letra + "000";
                }
                else
                {
                    var _numero = Convert.ToInt64(NumeroDocumento) + 1;
                    retorno = _numero.ToString();
                }

                return retorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public bool VerificaString(string str)
        {
            char[] c = str.ToCharArray();
            char le = ' ';
            for (int cont = 0; cont < c.Length; cont++)
            {
                le = c[cont];
                if (char.IsLetter(le) || char.IsPunctuation(le))
                    return true;
            }
            return false;
        }

        private String BuscaUltimoDocumento_old(String Prefixo)
        {
            try
            {
                long numeroDocumento;
                var ultimoDocumento = Db.TB_MOVIMENTOs.Where(_movimentacao => (_movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Saida ||
                                                                  _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Requisicao)
                                                                  && _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO.Substring(0, 10) == Prefixo)
                                          .Max(_movimentacao => _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO);

                if (ultimoDocumento == null)
                    numeroDocumento = Convert.ToInt64(Prefixo + "0001");
                else
                    numeroDocumento = Convert.ToInt64(ultimoDocumento) + 1;

                if (!String.IsNullOrEmpty(numeroDocumento.ToString()))
                    return numeroDocumento.ToString();
                else
                    throw new Exception("Erro ao gerar o numero do Documento.");
            }
            catch (Exception ex)
            {
                var descErro = (ex.InnerException.IsNotNull() ? ex.InnerException.Message : ex.Message);
                throw ex;
            }

        }

        private String getMaximoNumeroDocumento(String AnoMesReferencia, bool sobrecarga = false)
        {
            try
            {
                string strRetorno = null;
                string ultimoDocumento = NumeroUltimoDocumento();

                long? numeroMaximo;
                if (string.IsNullOrEmpty(ultimoDocumento))
                    throw new Exception("Erro ao gerar o numero do Documento (Mes de referência passado: nulo).");

                numeroMaximo = Common.Util.TratamentoDados.TryParseLong(ultimoDocumento);
                long numeroDocumento = numeroMaximo.Value + 1;

                if (!string.IsNullOrEmpty(AnoMesReferencia))
                    strRetorno = AnoMesReferencia.Substring(0, 4) + numeroDocumento.ToString().PadLeft(8, '0');

                return strRetorno;
            }
            catch (Exception ex)
            {
                var descErro = (ex.InnerException.IsNotNull() ? ex.InnerException.Message : ex.Message);
                //logger.GravarMsgInfo("Sam.Domain.MovimentoInfrastructure.getMaximoNumeroDocumento()", descErro);

                throw new Exception("Não foi possível gerar o numero do requisição.");
            }

        }

        // Salva o movimento e seus itens de uma vez (fecha a entrada de materiais)
        public void Salvar()
        {

            TB_MOVIMENTO mov = new TB_MOVIMENTO();
            EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();
            //EntitySet<TB_MOVIMENTO_ITEM> itensParaExclusao = new EntitySet<TB_MOVIMENTO_ITEM>();
            IList<TB_MOVIMENTO_ITEM> itensParaExclusao = new EntitySet<TB_MOVIMENTO_ITEM>();
            string strAnoReferencia = string.Empty;
            bool updateEfetuado = false;
            bool isRequisicaoEspelho = false;
            bool isAtualizacaoReq = false;
            bool isNumeroDocumentoExistente = false;

            if (this.Entity.Id.HasValue)
                mov = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_MOVIMENTOs.InsertOnSubmit(mov);

            int codigoPTRes = 0;
            foreach (MovimentoItemEntity view in this.Entity.MovimentoItem)
            {
                TB_MOVIMENTO_ITEM itemAdd = new TB_MOVIMENTO_ITEM();

                itemAdd.TB_MOVIMENTO_ITEM_ID = 0;
                if (this.Entity.Id.HasValue)
                {
                    itemAdd = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value).
                        Where(a => a.TB_MOVIMENTO_ITEM_ID == view.Id.Value).FirstOrDefault();
                    if (itemAdd != null)
                        itemAdd.TB_MOVIMENTO_ITEM_ATIVO = view.Ativo;
                }
                else
                {
                    this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(itemAdd);
                    itemAdd.TB_MOVIMENTO_ITEM_ID = view.Id.HasValue ? view.Id.Value : 0;
                    if (view.UGE != null)
                        itemAdd.TB_UGE_ID = view.UGE.Id.HasValue ? view.UGE.Id.Value : 0;

                    itemAdd.TB_MOVIMENTO_ITEM_ATIVO = view.Ativo;
                    //itemAdd.TB_MOVIMENTO_ITEM_DESD = view.Desd;

                    if (!(DateTime.MinValue == view.DataVencimentoLote))
                        itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = view.DataVencimentoLote;

                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = view.FabricanteLote;
                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = view.IdentificacaoLote;
                    itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = view.PrecoUnit;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = view.QtdeLiq;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = view.QtdeMov;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE = view.SaldoQtde;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = view.SaldoQtdeLote;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = view.SaldoValor;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = view.ValorMov;
                    itemAdd.TB_SUBITEM_MATERIAL_ID = (int)view.SubItemMaterial.Id;

                    itemAdd.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = view.NL_Liquidacao;

                    if (view.PTRes.IsNotNull() && view.PTRes.Id.HasValue)
                    {
                        itemAdd.TB_PTRES_ID = view.PTRes.Id.Value;

                        if (Int32.TryParse(view.PTResCodigo, out codigoPTRes) && !String.IsNullOrWhiteSpace(view.PTResAcao))
                        {
                            itemAdd.TB_PTRES_CODIGO = codigoPTRes;
                            itemAdd.TB_PTRES_PT_ACAO = view.PTResAcao;
                        }
                        else if (!Int32.TryParse(view.PTResCodigo, out codigoPTRes) || String.IsNullOrWhiteSpace(view.PTResAcao))
                        {
                            throw new Exception("Dados referentes a PTRES obrigatórios para gravação de requisição!");
                        }
                    }

                    //itemAdd.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO = view.CodigoUnidadeFornecimentoSiafisico;
                }
                item.Add(itemAdd);
            }

            #region Tratamento gravacao historico
            itensParaExclusao = mov.TB_MOVIMENTO_ITEMs.Except(item).ToList();

            if (!updateEfetuado && (mov.TB_MOVIMENTO_ITEMs.Count != item.Count))
                updateEfetuado = true;

            Db.TB_MOVIMENTO_ITEMs.DeleteAllOnSubmit(itensParaExclusao);
            #endregion Tratamento gravacao historico

            mov.TB_MOVIMENTO_ITEMs = item;
            mov.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;

            mov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
            mov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.DataDocumento.Value;
            mov.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value;
            mov.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
            mov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
            mov.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
            mov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
            mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
            mov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
            mov.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.ValorDocumento.Value.truncarDuasCasas();
            mov.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;
            mov.TB_LOGIN_ID = this.Entity.IdLogin;
            mov.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;

            if (this.Entity.UGE != null)
            {
                if (this.Entity.UGE.Id.HasValue)
                    mov.TB_UGE_ID = this.Entity.UGE.Id.Value;
                else
                {
                    mov.TB_UGE_ID = null;
                }
            }
            else
            {
                mov.TB_UGE_ID = null;
            }

            if (this.Entity.MovimAlmoxOrigemDestino != null)
            {
                if (this.Entity.MovimAlmoxOrigemDestino.Id.HasValue)
                    mov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino.Id.Value;
            }

            if (this.Entity.EmpenhoEvento != null)
            {
                if (this.Entity.EmpenhoEvento.Id.HasValue)
                    mov.TB_EMPENHO_EVENTO_ID = this.Entity.EmpenhoEvento.Id.Value;
            }

            if (this.Entity.EmpenhoLicitacao != null)
            {
                if (this.Entity.EmpenhoLicitacao.Id.HasValue)
                    mov.TB_EMPENHO_LICITACAO_ID = this.Entity.EmpenhoLicitacao.Id.Value;
            }

            if (this.Entity.Divisao != null)
            {
                if (this.Entity.Divisao.Id != null)
                    mov.TB_DIVISAO_ID = this.Entity.Divisao.Id.Value;
            }

            if (this.Entity.Fornecedor != null)
            {
                if (this.Entity.Fornecedor.Id != null)
                    mov.TB_FORNECEDOR_ID = this.Entity.Fornecedor.Id.Value;
            }

            //updateEfetuado = (Db.GetChangeSet().Updates.OfType<TB_MOVIMENTO>().Count() > 0);
            //TODO MELHORIA INFRA - DESATIVADO
            //if (updateEfetuado)
            //    GravarMovimentoHistorico(mov);

            #region Tratamento anti-duplicidade NumeroDocumento
            //Validar não-existência do nÃºmero do documento na base.
            strAnoReferencia = mov.TB_MOVIMENTO_ANO_MES_REFERENCIA.Substring(0, 4);
            isAtualizacaoReq |= this.Entity.Id.HasValue;
            isRequisicaoEspelho = (mov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoAprovada || mov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoFinalizada) && (mov.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id.Value);
            isNumeroDocumentoExistente = (this.Listar(Movimentacao => Movimentacao.NumeroDocumento == mov.TB_MOVIMENTO_NUMERO_DOCUMENTO).Count == 0);

            //if ( ( (!isRequisicaoEspelho) || (!updateEfetuado && (mov.TB_MOVIMENTO_NUMERO_DOCUMENTO.Replace(strAnoReferencia, "") == this.NumeroUltimoDocumento()) )) && !isAtualizacaoReq ) 
            if (((!isRequisicaoEspelho) || (!updateEfetuado) || (!isNumeroDocumentoExistente)) && (!(mov.TB_MOVIMENTO_NUMERO_DOCUMENTO.Replace(strAnoReferencia, "") == this.NumeroUltimoDocumento()) && (!isAtualizacaoReq)))
            {
                string strUltimoDocumento = string.Empty;
                int iUltimoDocumentoInserido = Int32.Parse(this.NumeroUltimoDocumento()) + 1;

                strUltimoDocumento = string.Format("{0}{1}", strAnoReferencia, iUltimoDocumentoInserido.ToString().PadLeft(8, '0'));
                mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = strUltimoDocumento;
            }
            #endregion Tratamento anti-duplicidade NumeroDocumento

            mov.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;
            this.Db.SubmitChanges();

        }

        private TB_SALDO_SUBITEM ConsultarSaldo(MovimentoItemEntity movimentoItem)
        {
            TB_SALDO_SUBITEM saldoList = new TB_SALDO_SUBITEM();
            saldoList = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == movimentoItem.Movimento.Almoxarifado.Id).
            Where(a => a.TB_UGE_ID == movimentoItem.UGE.Id).
            Where(a => a.TB_SUBITEM_MATERIAL_ID == movimentoItem.SubItemMaterial.Id).
            Where(a => a.TB_SALDO_SUBITEM_SALDO_QTDE != 0).FirstOrDefault();

            return saldoList;
        }
        private TB_SALDO_SUBITEM_LOTE ConsultarSaldoLote(MovimentoItemEntity movimentoItem, int IdSaldo)
        {
            IQueryable<TB_SALDO_SUBITEM_LOTE> result;

            if (movimentoItem.DataVencimentoLote != null)
            {
                result = (from saldolote in Db.TB_SALDO_SUBITEM_LOTEs
                          where saldolote.TB_SALDO_SUBITEM_ID == IdSaldo &&
                                saldolote.TB_SALDO_SUBITEM_LOTE_DT_VENC == movimentoItem.DataVencimentoLote &&
                                saldolote.TB_SALDO_SUBITEM_LOTE_IDENT == movimentoItem.IdentificacaoLote
                          select saldolote);
            }
            else
            {
                movimentoItem.IdentificacaoLote = movimentoItem.IdentificacaoLote == null ? string.Empty : movimentoItem.IdentificacaoLote;
                result = (from saldolote in Db.TB_SALDO_SUBITEM_LOTEs
                          where saldolote.TB_SALDO_SUBITEM_ID == IdSaldo &&
                                saldolote.TB_SALDO_SUBITEM_LOTE_IDENT == movimentoItem.IdentificacaoLote
                          select saldolote);
            }

            return result.FirstOrDefault();
        }



        private decimal ConsultarSaldoAlmoxarifado(int idAlmoxarifado)
        {
            //idAlmoxarifado = 147;
            String periodo = this.Db.TB_ALMOXARIFADOs.Where(b => b.TB_ALMOXARIFADO_ID == idAlmoxarifado).Max(b => b.TB_ALMOXARIFADO_MES_REF);


            decimal? saldo = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == idAlmoxarifado && a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF == periodo).Sum(x => x.TB_SALDO_SUBITEM_SALDO_VALOR == null ? 0 : x.TB_SALDO_SUBITEM_SALDO_VALOR);

            if (saldo != null)
                return (decimal)saldo;
            else
                return 0;
        }

        private decimal ConsultarSaldoAlmoxarifado33(int idAlmoxarifado)
        {
            //idAlmoxarifado = 147;
            String periodo = this.Db.TB_ALMOXARIFADOs.Where(b => b.TB_ALMOXARIFADO_ID == idAlmoxarifado).Max(b => b.TB_ALMOXARIFADO_MES_REF);


            // decimal? saldo33 = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == idAlmoxarifado && a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF == periodo && a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Substring(0, 2) == "33").Sum(x => x.TB_SALDO_SUBITEM_SALDO_VALOR == null ? 0 : x.TB_SALDO_SUBITEM_SALDO_VALOR);
            decimal? saldo33 = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == idAlmoxarifado && a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF == periodo && a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "33").Sum(x => x.TB_SALDO_SUBITEM_SALDO_VALOR == null ? 0 : x.TB_SALDO_SUBITEM_SALDO_VALOR);

            if (saldo33 != null)
                return (decimal)saldo33;
            else
                return 0;
        }

        private decimal ConsultarSaldoAlmoxarifado44(int idAlmoxarifado)
        {
            //idAlmoxarifado = 147;
            String periodo = this.Db.TB_ALMOXARIFADOs.Where(b => b.TB_ALMOXARIFADO_ID == idAlmoxarifado).Max(b => b.TB_ALMOXARIFADO_MES_REF);

            // decimal? saldo44 = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == idAlmoxarifado && a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF == periodo  && a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Substring(0, 2) == "44").Sum(x => x.TB_SALDO_SUBITEM_SALDO_VALOR == null ? 0 : x.TB_SALDO_SUBITEM_SALDO_VALOR);
            decimal? saldo44 = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == idAlmoxarifado && a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF == periodo && a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "44").Sum(x => x.TB_SALDO_SUBITEM_SALDO_VALOR == null ? 0 : x.TB_SALDO_SUBITEM_SALDO_VALOR);

            if (saldo44 != null)
                return (decimal)saldo44;
            else
                return 0;
        }


        private TB_SALDO_SUBITEM_LOTE ConsultarSaldosSaida(MovimentoItemEntity movimentoItem)
        {

            TB_SALDO_SUBITEM_LOTE saldoList = new TB_SALDO_SUBITEM_LOTE();
            int id = (from saldo in Db.TB_SALDO_SUBITEMs
                      where saldo.TB_UGE.TB_UGE_ID == movimentoItem.UGE.Id
                      where saldo.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == movimentoItem.Movimento.Almoxarifado.Id
                      where saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == movimentoItem.SubItemMaterial.Id
                      select saldo).FirstOrDefault().TB_SALDO_SUBITEM_ID;

            if (id != null)
            {
                movimentoItem.IdentificacaoLote = movimentoItem.IdentificacaoLote == null ? string.Empty : movimentoItem.IdentificacaoLote;
                saldoList = this.Db.TB_SALDO_SUBITEM_LOTEs.Where(a => a.TB_SALDO_SUBITEM_LOTE_DT_VENC == movimentoItem.DataVencimentoLote).
                Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == movimentoItem.IdentificacaoLote).
                Where(a => a.TB_SALDO_SUBITEM_ID == id).
                Where(a => a.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE != 0).FirstOrDefault();
            }

            return saldoList;
        }

        #region Métodos Saida de Material

        private void ValidaSaldoPositivo(MovimentoItemEntity movimentoItemSaldo, TB_SALDO_SUBITEM saldoList, MovimentoItemEntity itemMovimentoConsulta)
        {
            //if (movimentoItemSaldo == null)
            //if (movimentoItemSaldo == null || movimentoItemSaldo.SaldoQtde == 0)
            if (movimentoItemSaldo == null || movimentoItemSaldo.SaldoQtde == 0.000m)
                //throw new Exception(String.Format("Saldo Insuficiente para a saída do Subitem. Código SubItem: {0} - Descrição: {1} - Saldo Disponível na data digitada: {2}", saldoList.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, saldoList.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO, (movimentoItemSaldo.IsNotNull() ? movimentoItemSaldo.SaldoQtde.Value.ToString(this.fmtFracionarioMaterialQtde) : 0.0m.ToString(this.fmtFracionarioMaterialQtde))));
                throw new Exception(String.Format("Saldo Insuficiente para a saída do Subitem. Código SubItem: {0} - Descrição: {1} - Saldo Disponível em {2}: {3}", itemMovimentoConsulta.SubItemMaterial.Codigo.ToString(), itemMovimentoConsulta.SubItemMaterial.Descricao, itemMovimentoConsulta.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), (itemMovimentoConsulta.SaldoQtde.IsNotNull() ? itemMovimentoConsulta.SaldoQtde.Value.ToString(this.fmtFracionarioMaterialQtde) : 0.0m.ToString(this.fmtFracionarioMaterialQtde))));

            //if (saldoList == null)
            //if (saldoList == null || saldoList.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
            //TODO: Inserir tratamento contra null-value [DEF PROG]
            if (saldoList == null || saldoList.TB_SALDO_SUBITEM_SALDO_QTDE == 0.000m)
                //throw new Exception(String.Format("Saldo Insuficiente para a saída do Subitem. Código SubItem: {0} - Descrição: {1} - Saldo Disponível na data digitada: {2}", saldoList.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, saldoList.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO, saldoList.TB_SALDO_SUBITEM_SALDO_QTDE.Value.ToString(this.fmtFracionarioMaterialQtde)));
                throw new Exception(String.Format("Saldo Insuficiente para a saída do Subitem. Código SubItem: {0} - Descrição: {1} - Saldo Disponível em {2}: {3}", itemMovimentoConsulta.SubItemMaterial.Codigo.ToString(), itemMovimentoConsulta.SubItemMaterial.Descricao, itemMovimentoConsulta.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), (itemMovimentoConsulta.SaldoQtde.IsNotNull() ? itemMovimentoConsulta.SaldoQtde.Value.ToString(this.fmtFracionarioMaterialQtde) : 0.0m.ToString(this.fmtFracionarioMaterialQtde))));
        }

        private void updateDesdobroSaldo(TB_SALDO_SUBITEM saldo)
        {
            IQueryable<TB_SALDO_SUBITEM> query = (from q in Db.TB_SALDO_SUBITEMs
                                                  where q.TB_ALMOXARIFADO_ID == saldo.TB_ALMOXARIFADO_ID
                                                      && q.TB_SUBITEM_MATERIAL_ID == saldo.TB_SUBITEM_MATERIAL_ID
                                                      && q.TB_UGE_ID == saldo.TB_UGE_ID
                                                  select q).AsQueryable();
            var retorno = query.FirstOrDefault();

            if (retorno != null)
            {
                retorno.TB_SALDO_SUBITEM_DESDOBRO = 0;
                Db.TB_SALDO_SUBITEMs.InsertOnSubmit(retorno);
            }
        }
        private MovimentoItemEntity AtualizarSaldoSubItemSaida(MovimentoItemEntity movItem)
        {
            //Retorna a lista de tabelas TB_SALDO_SUBITEM atualizado do banco para update
            TB_SALDO_SUBITEM saldoList = this.ConsultarSaldo(movItem);

            TB_SALDO_SUBITEM_LOTE saldoListLote = this.ConsultarSaldoLote(movItem, saldoList.TB_SALDO_SUBITEM_ID);

            var saldoTotalSubItem = RetornaPrecoMedioMovimentoItemRetroativo(movItem);
            var saldoTotalSubItemLote = RetornaPrecoMedioMovimentoItemRetroativoLote(movItem);
            //Se não possui saldo, retorna mensagem.
            // ValidaSaldoPositivo(saldoTotalSubItem, saldoList, movItem);
            ValidaSaldoPositivo(saldoTotalSubItemLote, saldoList, movItem);

            movItem.PrecoUnit = saldoTotalSubItem.PrecoUnit;

            var qtdLiquidar = movItem.QtdeMov;

            if (movItem.QtdeMov != null)
            {
                //double dd;
                decimal? dd;
                decimal desdobro = 0.00m;

                //desdobro na última saída
                // if (saldoList.TB_SALDO_SUBITEM_SALDO_QTDE >= qtdLiquidar)
                if (saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE >= qtdLiquidar)
                {
                    //Saldo QTD
                    saldoList.TB_SALDO_SUBITEM_SALDO_QTDE -= qtdLiquidar;
                    saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE -= Convert.ToDecimal(qtdLiquidar);
                    saldoListLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;


                    //if (VerificarRetroativo(movItem) && saldoTotalSubItem != null)
                    //{
                    //    //dd = (double)saldoTotalSubItem.PrecoUnit;
                    //    //dd = saldoTotalSubItem.PrecoUnit.Value.Truncar(movItem.AnoMesReferencia, true);
                    //   // desdobro = ((saldoTotalSubItem.SaldoValor / saldoTotalSubItem.SaldoQtde) - saldoTotalSubItem.PrecoUnit) * saldoTotalSubItem.SaldoQtde;
                    //    //desdobro = (saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value > 0 ? saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value : 0);
                    //    desdobro = ((saldoList.TB_SALDO_SUBITEM_DESDOBRO.HasValue && saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value > 0) ? saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value : 0);
                    //    saldoList.TB_SALDO_SUBITEM_DESDOBRO = 0;

                    //}
                    //else
                    //{
                    //    //dd = ((double)((int)((double)(saldoTotalSubItem.SaldoValor / saldoTotalSubItem.SaldoQtde) * 100.0))) / 100.0;
                    //    //dd = (saldoTotalSubItem.SaldoValor / saldoTotalSubItem.SaldoQtde).Value.Truncar(movItem.AnoMesReferencia, true);

                    //    //desdobro = saldoTotalSubItem.SaldoValor - (dd * saldoTotalSubItem.SaldoQtde);
                    //    //desdobro = (saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value > 0 ? saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value : 0);
                    //    desdobro = ((saldoList.TB_SALDO_SUBITEM_DESDOBRO.HasValue && saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value > 0) ? saldoList.TB_SALDO_SUBITEM_DESDOBRO.Value : 0);
                    //    saldoList.TB_SALDO_SUBITEM_DESDOBRO = 0;


                    //}

                    ////saldoList.TB_SALDO_SUBITEM_PRECO_UNIT = dd;
                    ////movItem.Desd = desdobro;


                    decimal? saldoQtde = (saldoTotalSubItem.SaldoQtde - qtdLiquidar);
                    decimal? saldoQtdeLote = (saldoTotalSubItemLote.SaldoQtdeLote - qtdLiquidar);
                    if (saldoQtde < 0)
                    {
                        string lote = movItem.IdentificacaoLote == string.Empty ? "Sem Lote" : "Lote: " + movItem.IdentificacaoLote;
                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1} - {2}, verifique a quantidade disponível para o SubItem e se o Movimento é Retroativo.", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao, lote));
                    }
                    movItem.SaldoQtde = saldoQtde.Value;
                    movItem.SaldoQtdeLote = saldoQtdeLote.Value;

                    //Saldo Valor

                    //decimal valorSaldoCalculado = saldoList.TB_SALDO_SUBITEM_PRECO_UNIT.Value * qtdLiquidar.Value;
                    decimal valorSaldoCalculado = saldoTotalSubItem.PrecoUnit.Value * qtdLiquidar.Value;

                    // +float.Parse(desdobro.ToString());
                    decimal valorSaldoQtde = valorSaldoCalculado + desdobro;

                    movItem.ValorMov = valorSaldoQtde;
                    decimal calcularSaldo = 0;

                    if (saldoList.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
                    {
                        Decimal valorSaida = saldoTotalSubItem.PrecoUnit.Value * qtdLiquidar.Value;
                        Decimal valorSubtracao = saldoList.TB_SALDO_SUBITEM_SALDO_VALOR.Value - valorSaida;
                        movItem.Desd = valorSubtracao;

                        calcularSaldo = saldoList.TB_SALDO_SUBITEM_SALDO_VALOR.Value - (valorSaida + valorSubtracao);
                    }
                    else
                        calcularSaldo = saldoList.TB_SALDO_SUBITEM_SALDO_VALOR.Value - valorSaldoQtde;

                    //if(calcularSaldo < 0 )
                    //    throw new Exception("Quantidade Indisponível para o SubItem, Verifique a quantidade disponível para o SubItem e se o Movimento é Retroativo");

                    saldoList.TB_SALDO_SUBITEM_SALDO_VALOR = calcularSaldo;

                    decimal calcularSaldoSubItem = saldoTotalSubItem.SaldoValor.Value - valorSaldoQtde;

                    movItem.SaldoValor = calcularSaldoSubItem;

                    qtdLiquidar = 0;
                    if (movItem.SaldoValor < 0 || movItem.SaldoQtde < 0 || movItem.SaldoQtdeLote < 0)
                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível para o SubItem na data do movimento!", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao));

                }
                //else if (saldoList.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
                else if (saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE == 0)
                {
                    //throw new Exception("Saldo Indisponível para o SubItem");
                    throw new Exception(String.Format("Saldo Indisponível para o SubItem {0} - {1}, para esta data de movimento!", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao));
                }
                //Se a quantidade solicitada for maior que o saldo, retorna mensagem.
                // else if (saldoList.TB_SALDO_SUBITEM_SALDO_QTDE < movItem.QtdeMov)
                else if (saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE < movItem.QtdeMov)
                {
                    //throw new Exception("Saldo Indisponível para o SubItem");
                    throw new Exception(String.Format("Saldo Indisponível para o SubItem {0} - {1}, para esta data de movimento!", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao));
                }
                else
                {
                    qtdLiquidar -= saldoList.TB_SALDO_SUBITEM_SALDO_QTDE;
                    saldoList.TB_SALDO_SUBITEM_SALDO_QTDE = 0;
                }
            }

            return saldoTotalSubItem;
        }

        public decimal ConsultarSaldoTotalAlmoxarifado(int idAlmoxarifado)
        {
            return this.ConsultarSaldoAlmoxarifado(idAlmoxarifado);
        }
        public decimal ConsultarSaldoTotalAlmoxarifado33(int idAlmoxarifado)
        {
            return this.ConsultarSaldoAlmoxarifado33(idAlmoxarifado);
        }
        public decimal ConsultarSaldoTotalAlmoxarifado44(int idAlmoxarifado)
        {
            return this.ConsultarSaldoAlmoxarifado44(idAlmoxarifado);
        }


        public void AtualizarSaldoMovimentoDoSubItem(MovimentoItemEntity movItem)
        {
            //Retorno o objeto Saldo do subItem
            IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                   where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                   saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                   saldo.TB_UGE_ID == movItem.UGE.Id
                                                   select saldo).AsQueryable();

            TB_SALDO_SUBITEM resultado = result.FirstOrDefault();

            //Se encontrou o Saldo
            if (resultado != null)
            {
                resultado.TB_SALDO_SUBITEM_SALDO_QTDE = movItem.SaldoQtde;

                resultado.TB_SALDO_SUBITEM_SALDO_VALOR = movItem.SaldoValor;

                resultado.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit;

                resultado.TB_SALDO_SUBITEM_DESDOBRO = movItem.Desd;

                Db.SubmitChanges();
            }
            else
            {
                throw new Exception("Não foi possivel Atualizar o saldo");
            }
        }

        private MovimentoItemEntity CalculaSaldoDoSubItem(MovimentoItemEntity movimentoItemSaida, TB_SALDO_SUBITEM saldoAtual)
        {



            MovimentoItemEntity movimentoItem = getMovimentoItem(movimentoItemSaida);

            //ValidaSaldoPositivo(movimentoItem, saldoAtual, movimentoItemSaida);
            verificarSeExisteSaldoParaMovimento(saldoAtual, movimentoItemSaida);

            if (movimentoItemSaida.QtdeMov != null)
            {
                if (saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE >= movimentoItemSaida.QtdeMov)
                {
                    saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE -= movimentoItemSaida.QtdeMov;

                    if (saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
                    {
                        Decimal valorSaldoMenosMovimento = saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR.Value - (movimentoItemSaida.QtdeMov.Value * saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT.Value);

                        movimentoItemSaida.ValorMov = (movimentoItemSaida.QtdeMov * saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT) + valorSaldoMenosMovimento;

                        saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR -= movimentoItemSaida.ValorMov;
                        movimentoItemSaida.Desd = valorSaldoMenosMovimento;
                    }
                    else
                    {
                        movimentoItemSaida.ValorMov = (movimentoItemSaida.QtdeMov * saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT).Value.truncarDuasCasas();
                        saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR -= (movimentoItemSaida.QtdeMov.Value * saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT.Value).truncarDuasCasas();
                    }



                    movimentoItemSaida.SaldoQtde = saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE;
                    movimentoItemSaida.SaldoValor = saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR;

                }

                if (saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR < 0 || saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE < 0)
                    throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível para o SubItem na data do movimento!", movimentoItemSaida.SubItemMaterial.Codigo, movimentoItemSaida.SubItemMaterial.Descricao));




            }
            else if (saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
            {
                throw new Exception(String.Format("Saldo Indisponível para o SubItem {0} - {1}, para esta data de movimento!", movimentoItemSaida.SubItemMaterial.Codigo, movimentoItemSaida.SubItemMaterial.Descricao));
            }
            else if (saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE < movimentoItemSaida.QtdeMov)
            {
                throw new Exception(String.Format("Saldo Indisponível para o SubItem {0} - {1}, para esta data de movimento!", movimentoItemSaida.SubItemMaterial.Codigo, movimentoItemSaida.SubItemMaterial.Descricao));
            }

            return movimentoItem;
        }
        public MovimentoItemEntity getMovimentoItem(MovimentoItemEntity movItem)
        {
            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= Movimento.DataMovimento.Value
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                               || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                               (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          orderby i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO descending, i.TB_MOVIMENTO_ITEM_ID descending
                          select new MovimentoItemEntity
                          {
                              Id = i.TB_MOVIMENTO_ITEM_ID,
                              SubItemMaterial = new SubItemMaterialEntity(i.TB_SUBITEM_MATERIAL_ID),
                              UGE = new UGEEntity(i.TB_UGE_ID),
                              SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                              SaldoValor = i.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                              DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                              PrecoUnit = i.TB_MOVIMENTO_ITEM_PRECO_UNIT
                          }).FirstOrDefault();

            return result;
        }
        private void verificarSeExisteSaldoParaMovimento(TB_SALDO_SUBITEM saldoItem, MovimentoItemEntity movimentoItemSaida)
        {
            //if (movimentoItemSaida.QtdeMov > saldoItem.TB_SALDO_SUBITEM_SALDO_QTDE || movimentoItemSaida.ValorMov > saldoItem.TB_SALDO_SUBITEM_SALDO_VALOR)
            if (movimentoItemSaida.QtdeMov > saldoItem.TB_SALDO_SUBITEM_SALDO_QTDE)
                throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível para o SubItem na data do movimento!", movimentoItemSaida.SubItemMaterial.Codigo, movimentoItemSaida.SubItemMaterial.Descricao));

        }

        /// <summary>
        /// Para os movimentos de saida retroativos, verifica se existe saldo no MovimentoItem
        /// </summary>
        /// <returns></returns>
        public bool VerificarSaldoPositivoMovimento()
        {
            //this.Service<IMovimentoService>().Entity = this.Movimento;

            //bool retorno = true;
            //foreach (var movItem in Movimento.MovimentoItem)
            //{
            //    retorno = retorno && this.Service<IMovimentoService>().VerificarSaldoPositivoMovimento(movItem);
            //}

            //if (!retorno)
            //{
            //    this.ListaErro.Add("Saldo Histórico insuficiente para executar o Estorno retroativo.");
            //    return false;
            //}
            //else
            //    return true;

            return true;
        }

        private TB_MOVIMENTO RetornaMovimentoSaida()
        {
            TB_MOVIMENTO mov = new TB_MOVIMENTO();

            //Inserir Movimento
            this.Db.TB_MOVIMENTOs.InsertOnSubmit(mov);

            mov.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;
            mov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
            mov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataMovimento.Value.Date;
            mov.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value.Date;
            mov.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
            mov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
            mov.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
            mov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
            mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
            mov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
            mov.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;
            mov.TB_LOGIN_ID = this.Entity.IdLogin;
            mov.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;
            mov.TB_UGE_ID = (this.Entity.UGE != null) ? this.Entity.UGE.Id : null;
            mov.TB_DIVISAO_ID = (this.Entity.Divisao != null) ? this.Entity.Divisao.Id : null;
            mov.TB_EMPENHO_EVENTO_ID = (this.Entity.EmpenhoEvento != null) ? this.Entity.EmpenhoEvento.Id : null;
            mov.TB_EMPENHO_LICITACAO_ID = (this.Entity.EmpenhoLicitacao != null) ? this.Entity.EmpenhoLicitacao.Id : null;
            mov.TB_FORNECEDOR_ID = (this.Entity.Fornecedor != null) ? this.Entity.Fornecedor.Id : null;
            mov.TB_DIVISAO_ID = (this.Entity.Divisao != null) ? this.Entity.Divisao.Id : null;
            mov.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;
            mov.TB_SUBTIPO_MOVIMENTO_ID = this.Entity.SubTipoMovimentoId != null ? this.Entity.SubTipoMovimentoId : null;
            if (this.Entity.MovimAlmoxOrigemDestino != null)
            {
                if (this.Entity.MovimAlmoxOrigemDestino.Id.HasValue)
                    mov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino.Id.Value;
            }

            mov.TB_MOVIMENTO_BLOQUEAR = AtualizaStatusRequisicaoAprovada();

            return mov;
        }

        private bool AtualizaStatusRequisicaoAprovada()
        {
            bool retorno = false;
            //Saida para nova requisição irá atualizar o status da solicitação para Aprovada.
            if (this.Entity.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada)
            {
                var movUpdate = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value && a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente).FirstOrDefault();

                if (movUpdate != null)
                {
                    //Atualiza a requisição solicitada para Aprovada
                    movUpdate.TB_TIPO_MOVIMENTO_ID = (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada;
                    retorno = movUpdate.TB_MOVIMENTO_BLOQUEAR == null ? false : (bool)movUpdate.TB_MOVIMENTO_BLOQUEAR;
                }
            }

            return retorno;
        }

        private TB_PTRE RetornaPtResSaida(PTResEntity ptRes)
        {
            TB_PTRE PtRes = null;

            if (ptRes.IsNotNull())
            {
                PtRes = new TB_PTRE();

                PtRes.TB_PTRES_ANO = ptRes.AnoDotacao;
                PtRes.TB_PTRES_CODIGO = ptRes.Codigo.Value;
                PtRes.TB_PTRES_CODIGO_GESTAO = ptRes.CodigoGestao;
                PtRes.TB_PTRES_DESCRICAO = ptRes.Descricao;
                PtRes.TB_PTRES_ID = ptRes.Id.Value;
                PtRes.TB_PTRES_PT_CODIGO = ptRes.CodigoPT;
                PtRes.TB_PTRES_UGE_CODIGO = ptRes.CodigoUGE;
                PtRes.TB_PTRES_UO_CODIGO = ptRes.CodigoUO;
                PtRes.TB_PTRES_ANO = ptRes.AnoDotacao;
                PtRes.TB_PTRES_PT_ACAO = ptRes.ProgramaTrabalho.ProjetoAtividade;
            }

            return PtRes;
        }

        private TB_MOVIMENTO_ITEM RetornaMovimentoItemSaida(TB_MOVIMENTO mov, MovimentoItemEntity movItem, MovimentoItemEntity MovimentoItemsaldo, PTResEntity ptRes)
        {
            TB_MOVIMENTO_ITEM itemAdd = new TB_MOVIMENTO_ITEM();

            itemAdd.TB_MOVIMENTO_ITEM_ID = 0;

            this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(itemAdd);
            itemAdd.TB_MOVIMENTO_ITEM_ID = movItem.Id.HasValue ? movItem.Id.Value : 0;
            itemAdd.TB_UGE_ID = (int)movItem.UGE.Id;
            itemAdd.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
            itemAdd.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;
            itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItem.DataVencimentoLote;
            itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = movItem.FabricanteLote;
            itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = movItem.IdentificacaoLote;
            itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = movItem.QtdeLiq;
            itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov;
            itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE = movItem.SaldoQtde;
            itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItem.SaldoQtdeLote;

            #region PTRes
            bool isRequisicao = ((movItem.Movimento.TipoMovimento.Id == (int)enumTipoMovimento.RequisicaoPendente) ||
                                            (movItem.Movimento.TipoMovimento.Id == (int)enumTipoMovimento.RequisicaoAprovada) ||
                                 (movItem.Movimento.TipoMovimento.Id == (int)enumTipoMovimento.RequisicaoFinalizada));

            if (isRequisicao && movItem.PTRes.IsNull())
                throw new Exception("PTRes obrigatório para persistir requisição!");
            else if (isRequisicao && movItem.PTRes.IsNotNull())
            {
                itemAdd.TB_PTRES_ID = movItem.PTRes.Id;

                itemAdd.TB_PTRES_CODIGO = movItem.PTRes.Codigo;
                itemAdd.TB_PTRES_PT_ACAO = movItem.PTRes.ProgramaTrabalho.ProjetoAtividade;
            }
            #endregion PTRes

            itemAdd.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = movItem.NL_Liquidacao;
            //itemAdd.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO = movItem.CodigoUnidadeFornecimentoSiafisico;

            if (movItem.PrecoUnit.HasValue)
                itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
            else
                itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = MovimentoItemsaldo.PrecoUnit;

            if (movItem.SaldoValor.HasValue)
                itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = movItem.SaldoValor;
            else
                itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = MovimentoItemsaldo.SaldoValor;

            itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
            itemAdd.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;

            if (movItem.SubItemMaterial.ItemMaterial != null)
                itemAdd.TB_ITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.ItemMaterial.Id;

            //Trecho já existente no region PTRes, pouco acima.
            //if (isRequisicao && movItem.PTRes.IsNotNull())
            //    itemAdd.TB_PTRE = RetornaPtResSaida(ptRes);

            if (itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT == 0)
                throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");

            return itemAdd;
        }

        #endregion

        public DateTime RetornaDataDocumentoRequisicao(string numDocumento)
        {
            int tpMovPendente = 10;
            var data = (from a in Db.TB_MOVIMENTOs
                        where a.TB_MOVIMENTO_NUMERO_DOCUMENTO == numDocumento
                        where a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID == tpMovPendente
                        select a.TB_MOVIMENTO_DATA_DOCUMENTO).FirstOrDefault();

            if (data != DateTime.MinValue)
                return data;
            else
                return DateTime.Now;
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// verifica se existe o documento selecionado
        /// </summary>
        /// <returns></returns>
        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id.HasValue && this.Entity.Fornecedor.Id != 0)
                        retorno = this.Db.TB_MOVIMENTOs
                        .Where(a => a.TB_MOVIMENTO_DATA_DOCUMENTO == this.Entity.DataDocumento)
                        .Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                        .Where(a => a.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                        .Where(a => a.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                        .Where(a => a.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                        .Where(a => a.TB_MOVIMENTO_ATIVO == true)
                        .Count() > 0;
                    else
                        retorno = this.Db.TB_MOVIMENTOs
                        .Where(a => a.TB_MOVIMENTO_DATA_DOCUMENTO == this.Entity.DataDocumento)
                        .Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                        .Where(a => a.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                        .Where(a => a.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                        .Where(a => a.TB_MOVIMENTO_ATIVO == true)
                        .Count() > 0;
                }
            }
            else
            {
                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id.HasValue && this.Entity.Fornecedor.Id != 0)
                        retorno = this.Db.TB_MOVIMENTOs
                        .Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                        .Where(a => a.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                        .Where(a => a.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                        .Where(a => a.TB_MOVIMENTO_ATIVO == true)
                        .Count() > 0;
                    else
                        retorno = this.Db.TB_MOVIMENTOs
                        .Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                        .Where(a => a.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                        .Where(a => a.TB_MOVIMENTO_ATIVO == true)
                        .Count() > 0;
                }
            }
            return retorno;
        }



        public MovimentoEntity ExisteCodigoConsumoDados()
        {
            MovimentoEntity retorno = new MovimentoEntity();
            //bool contemEmpenho = false;

            if (!this.Entity.Id.HasValue || (this.Entity.Id.HasValue && this.Entity.Id.Value == 0))
            {
                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id.HasValue && this.Entity.Fornecedor.Id != 0)
                    {
                        if (!string.IsNullOrEmpty(this.Entity.Empenho))
                        {
                            List<MovimentoEntity> retornoList = (from Mov in this.Db.TB_MOVIMENTOs
                                                                 join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                                                 where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                                                 //where (Mov.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                                                                 //where (Mov.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                                                                 where (Mov.TB_MOVIMENTO_ATIVO == true)
                                                                 where (Mov.TB_UGE_ID == this.Entity.UGE.Id)
                                                                 where (Mov.TB_UA_ID == this.Entity.UA.Id)
                                                                 where (Mov.TB_MOVIMENTO_EMPENHO == this.Entity.Empenho)
                                                                 select new MovimentoEntity
                                                                 {
                                                                     //CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                                                     DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                     DataDocumento = Mov.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                     Empenho = Mov.TB_MOVIMENTO_EMPENHO,
                                                                     Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                                                     UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                                                     UA = new UAEntity { Id = Mov.TB_UA.TB_UA_ID, Codigo = Mov.TB_UA.TB_UA_CODIGO },
                                                                     IdLogin = Mov.TB_LOGIN_ID,
                                                                     IdLoginEstorno = Mov.TB_LOGIN_ID_ESTORNO,
                                                                     NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                                                 }).ToList();


                            /*
                            if (retornoList != null)
                            {
                                foreach (var item in retornoList)
                                {
                                    if (item.Fornecedor.Id == this.Entity.Fornecedor.Id.Value)
                                    {
                                        if (item.UA.Id == this.Entity.UA.Id && item.Empenho != this.Entity.Empenho)
                                        {
                                            retorno = null;
                                        }
                                        else if (item.Empenho == this.Entity.Empenho && item.UA.Id != this.Entity.UA.Id)
                                        {
                                            retorno = null;
                                        }
                                        else if (item.Empenho == this.Entity.Empenho && item.UA.Id == this.Entity.UA.Id)
                                        {
                                            retorno = retornoList.Where(a => a.DataDocumento.Value.ToShortDateString() != this.Entity.DataDocumento.Value.ToShortDateString() || a.DataMovimento.Value.ToShortDateString() != this.Entity.DataMovimento.Value.ToShortDateString() || a.Empenho == this.Entity.Empenho).FirstOrDefault();

                                            if (retorno != null)
                                                return retorno;
                                        }
                                    }
                                    else {
                                        if (item.Empenho == this.Entity.Empenho)
                                            contemEmpenho = true;
                                    }
                                     
                                }

                                if (!contemEmpenho)
                                    return retorno = null; 
                                                               
                                retorno = retornoList.Where(a => a.DataDocumento.Value.ToShortDateString() != this.Entity.DataDocumento.Value.ToShortDateString() || a.DataMovimento.Value.ToShortDateString() != this.Entity.DataMovimento.Value.ToShortDateString() || a.Empenho == this.Entity.Empenho).FirstOrDefault();

                            }
                            else
                                retorno = null;

                            */

                            if (retornoList.Any())
                            {
                                retorno = retornoList.Where(a => a.DataDocumento.Value.ToShortDateString() != this.Entity.DataDocumento.Value.ToShortDateString() || a.DataMovimento.Value.ToShortDateString() != this.Entity.DataMovimento.Value.ToShortDateString() || a.Empenho == this.Entity.Empenho).FirstOrDefault();
                            }
                            else
                            {
                                retorno = null;
                            }
                        }

                    }

                }
            }

            return retorno;
        }

        public MovimentoEntity ExisteCodigoInformadoDados()
        {
            MovimentoEntity retorno = new MovimentoEntity();

            if (this.Entity.Id.HasValue)
            {
                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id.HasValue && this.Entity.Fornecedor.Id != 0)
                    {
                        if (!string.IsNullOrEmpty(this.Entity.Empenho))
                        {
                            List<MovimentoEntity> retornoList = (from Mov in this.Db.TB_MOVIMENTOs
                                                                 join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                                                 where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                                                 where (Mov.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                                                                 where (Mov.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                                                                 where (Mov.TB_MOVIMENTO_ATIVO == true)
                                                                 where (Mov.TB_UGE_ID == this.Entity.UGE.Id)
                                                                 select new MovimentoEntity
                                                                 {
                                                                     CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                                                     DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                     DataDocumento = Mov.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                                     Empenho = Mov.TB_MOVIMENTO_EMPENHO,
                                                                     Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                                                     UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                                                     NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                                                 }).ToList();


                            if (retornoList != null)

                                retorno = retornoList.Where(a => a.DataDocumento.Value.ToShortDateString() != this.Entity.DataDocumento.Value.ToShortDateString() ||
                                a.DataMovimento.Value.ToShortDateString() != this.Entity.DataMovimento.Value.ToShortDateString() ||
                                a.Empenho == this.Entity.Empenho).FirstOrDefault();
                            else
                                retorno = null;

                        }
                        else
                        {
                            retorno = (from Mov in this.Db.TB_MOVIMENTOs
                                       join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                       where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                       where (Mov.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                                       where (Mov.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                                       where (Mov.TB_MOVIMENTO_ATIVO == true)
                                       select new MovimentoEntity
                                       {
                                           CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                           DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                           Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                           UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                           NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                       }).FirstOrDefault();
                        }
                    }
                    else
                    {

                        retorno = (from Mov in this.Db.TB_MOVIMENTOs
                                   join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                   where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                   where (Mov.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                                   where (Mov.TB_MOVIMENTO_ATIVO == true)
                                   where (Mov.TB_MOVIMENTO_GERADOR_DESCRICAO == this.Entity.GeradorDescricao)
                                   select new MovimentoEntity
                                   {
                                       CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                       DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                       Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                       UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                       NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                   }).FirstOrDefault();


                    }

                }
                else
                {

                    retorno = (from Mov in this.Db.TB_MOVIMENTOs
                               join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                               where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                               where (Mov.TB_MOVIMENTO_ID != this.Entity.Id.Value)
                               where (Mov.TB_MOVIMENTO_ATIVO == true)
                               where (Mov.TB_MOVIMENTO_GERADOR_DESCRICAO == this.Entity.GeradorDescricao)
                               select new MovimentoEntity
                               {
                                   CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                   DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                   Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                   UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                   NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                               }).FirstOrDefault();

                }
            }
            else
            {
                if (this.Entity.Fornecedor != null)
                {
                    if (this.Entity.Fornecedor.Id.HasValue && this.Entity.Fornecedor.Id != 0)
                    {
                        retorno = (from Mov in this.Db.TB_MOVIMENTOs
                                   join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                   where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                   where (Mov.TB_FORNECEDOR_ID == this.Entity.Fornecedor.Id.Value)
                                   where (Mov.TB_MOVIMENTO_ATIVO == true)
                                   select new MovimentoEntity
                                   {
                                       CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                       DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                       Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                       UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                       NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                   }).FirstOrDefault();


                    }
                    else
                    {
                        retorno = (from Mov in this.Db.TB_MOVIMENTOs
                                   join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                                   where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                                   where (Mov.TB_MOVIMENTO_ATIVO == true)
                                   where (Mov.TB_MOVIMENTO_GERADOR_DESCRICAO == this.Entity.GeradorDescricao)
                                   select new MovimentoEntity
                                   {
                                       CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                       DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                       Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                       UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                       NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                   }).FirstOrDefault();

                    }

                }
                else
                {
                    retorno = (from Mov in this.Db.TB_MOVIMENTOs
                               join Uge in Db.TB_UGEs on Mov.TB_UGE_ID equals Uge.TB_UGE_ID
                               where (Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.Entity.NumeroDocumento)
                               where (Mov.TB_MOVIMENTO_ATIVO == true)
                               where (Mov.TB_MOVIMENTO_GERADOR_DESCRICAO == this.Entity.GeradorDescricao)
                               select new MovimentoEntity
                               {
                                   CodigoFormatado = Uge.TB_UGE_CODIGO + " - " + Uge.TB_UGE_DESCRICAO,
                                   DataMovimento = Mov.TB_MOVIMENTO_DATA_MOVIMENTO,
                                   Fornecedor = new FornecedorEntity { Id = Mov.TB_FORNECEDOR_ID, Nome = Mov.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = Mov.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ },
                                   UGE = new UGEEntity { Id = Mov.TB_UGE.TB_UGE_ID, Codigo = Mov.TB_UGE.TB_UGE_CODIGO },
                                   NumeroDocumento = Mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                               }).FirstOrDefault();

                }
            }


            return retorno;
        }

        public bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId)
        {
            int? parametroNulo = null;
            return ExisteRequisicaoMaterialPendenteEAtiva(almoxarifadoId, out parametroNulo);
        }

        public bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId, out int? numeroRequisicoesPendentes)
        {
            bool existeRequisicoesPendentes = false;
            int? _numeroRequisicoesPendentes = null;
            IQueryable qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere = null;


            expWhere = (movMaterial => movMaterial.TB_TIPO_MOVIMENTO_ID == enumTipoMovimento.RequisicaoPendente.GetHashCode()
                                    && movMaterial.TB_MOVIMENTO_ATIVO == true
                                    && movMaterial.TB_ALMOXARIFADO_ID == almoxarifadoId);

            qryConsulta = this.Db.TB_MOVIMENTOs.Where(expWhere);
            _numeroRequisicoesPendentes = qryConsulta.Cast<TB_MOVIMENTO>().Count();


            existeRequisicoesPendentes = (_numeroRequisicoesPendentes > 0);
            numeroRequisicoesPendentes = _numeroRequisicoesPendentes;
            return existeRequisicoesPendentes;
        }


        public MovimentoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        #region Métodos importantes de calculos de Movimento

        /// <summary>
        /// **********************
        /// **Método Importante
        /// ***********************
        /// Realiza o processamento da saida
        /// </summary>
        /// <returns></returns>
        public MovimentoEntity SalvarSaida()
        {
            TB_MOVIMENTO tbMovimento = new TB_MOVIMENTO();
            EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();

            string strNumeroDocumento = String.Empty;

            if (Convert.ToInt64(this.Entity.AnoMesReferencia) >= Convert.ToInt64("201501"))
            {
                string strAnoDocumento = DateTime.Now.Year.ToString();
                string strUgeCodigoDescricao = Convert.ToString(this.Entity.UGE.Codigo ?? 0);
                string strPrefixo = "";

                if (String.IsNullOrWhiteSpace(this.Entity.NumeroDocumento))
                {
                    strPrefixo = strAnoDocumento + strUgeCodigoDescricao;
                    strNumeroDocumento = BuscaUltimoDocumento(strPrefixo);
                }
                else
                    strNumeroDocumento = this.Entity.NumeroDocumento;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(this.Entity.NumeroDocumento))
                    strNumeroDocumento = getMaximoNumeroDocumento(this.Entity.AnoMesReferencia);
                else
                    strNumeroDocumento = this.Entity.NumeroDocumento;
            }



            string strAnoReferencia = string.Empty;

            tbMovimento = RetornaMovimentoSaida();

            if (this.Entity.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada)
            {
                if ((bool)tbMovimento.TB_MOVIMENTO_BLOQUEAR)
                    throw new Exception("Requisição está sendo editada pelo Requisitante.");

                if (this.Entity.Id != null)
                    AlterarMovimentoBloquear((int)this.Entity.Id, true);
            }

            string SubItem = "";
            decimal SubItemSaldoQtde = 0;
            decimal SubItemSaldoValor = 0;
            //Consulta se existe saldo disponível
            foreach (MovimentoItemEntity movItem in this.Entity.MovimentoItem)
            {


                movItem.Movimento = this.Entity;

                //var saldoTotalSubItem = AtualizarSaldoSubItemSaida(movItem);
                TB_SALDO_SUBITEM saldoAtual = getSaldoDoSubItem(movItem);
                TB_SALDO_SUBITEM_LOTE saldoListLote = this.ConsultarSaldoLote(movItem, saldoAtual.TB_SALDO_SUBITEM_ID);
                MovimentoItemEntity saldoAtualMovimentoItem = new MovimentoItemEntity();

                if (SubItem != movItem.SubItemCodigoFormatado)
                {
                    saldoAtualMovimentoItem = getUltimoMovimentoItemDoSubItem(movItem);
                    SubItem = movItem.SubItemCodigoFormatado;
                }
                else
                {
                    saldoAtualMovimentoItem.SaldoQtde = SubItemSaldoQtde;
                    saldoAtualMovimentoItem.SaldoValor = SubItemSaldoValor;
                }




                saldoAtual.TB_SALDO_SUBITEM_SALDO_QTDE = saldoAtualMovimentoItem.SaldoQtde;
                saldoAtual.TB_SALDO_SUBITEM_SALDO_VALOR = saldoAtualMovimentoItem.SaldoValor;

                if (movItem.PrecoUnit != null)
                    saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit != 0 ? movItem.PrecoUnit : saldoAtual.TB_SALDO_SUBITEM_PRECO_UNIT;

                decimal valor = movItem.QtdeMov == null ? 0 : Convert.ToDecimal(movItem.QtdeMov);
                saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE -= valor;
                saldoListLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;
                movItem.SaldoQtdeLote = saldoListLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE;

                var saldoTotalSubItem = CalculaSaldoDoSubItem(movItem, saldoAtual);

                SubItemSaldoQtde = Convert.ToDecimal(movItem.SaldoQtde);
                SubItemSaldoValor = Convert.ToDecimal(movItem.SaldoValor);



                tbMovimento.TB_MOVIMENTO_ITEMs.Add(RetornaMovimentoItemSaida(tbMovimento, movItem, saldoTotalSubItem, movItem.PTRes));
            }

            //Atualiza o valor do documento
            tbMovimento.TB_MOVIMENTO_VALOR_DOCUMENTO = tbMovimento.TB_MOVIMENTO_ITEMs.Sum(a => a.TB_MOVIMENTO_ITEM_VALOR_MOV).Value.truncarDuasCasas();

            //Descrição da origem da saída
            tbMovimento.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;

            if (this.Entity.Divisao != null)
                tbMovimento.TB_DIVISAO_ID = this.Entity.Divisao.Id;


            tbMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = strNumeroDocumento;
            tbMovimento.TB_MOVIMENTO_CE = this.Entity.InscricaoCE;

            this.Db.SubmitChanges();

            if (this.Entity.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada)
            {
                if (this.Entity.Id != null)
                    AlterarMovimentoBloquear((int)this.Entity.Id, false);
            }

            //Retorna o objeto que inseriu atualizado
            Entity = new MovimentoEntity();
            Entity.Id = tbMovimento.TB_MOVIMENTO_ID;
            Entity.NumeroDocumento = tbMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO;



            return GetMovimento();
        }

        public bool ExecutarIntegracao(int movimentoId, bool estorno, bool consulta)
        {
            List<String> erro = new List<String>();
            DateTime dataExecucaoFechamento = DateTime.Now;
            Int32 retornoParametro = 1;

            SqlConnection cnn = new SqlConnection(this.Db.Connection.ConnectionString);
            cnn.Open();
            SqlCommand cmd;
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction(System.Data.IsolationLevel.Serializable, "INTEGRACAO");

            try
            {

                cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = "[dbo].[SAM_INTEGRACAO_PATRIMONIO]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Transaction = transaction;




                cmd.Parameters.AddWithValue("@TB_MOVIMENTO_ID", movimentoId);
                cmd.Parameters.AddWithValue("@ESTORNO", estorno);
                cmd.Parameters.AddWithValue("@CONSULTA", consulta);

                cmd.Parameters.AddWithValue("@RETORNO", 1).Direction = System.Data.ParameterDirection.Output;


                cmd.ExecuteNonQuery();
                retornoParametro = Convert.ToInt32(cmd.Parameters["@RETORNO"].Value);

                //retornoParametro igual a "0" foi gerado pela procedure "SAM_CORRIGIR_FECHAMENTO" um erro durante o processo de fechamento   
                if (retornoParametro == 0)
                {
                    //  SubItemMaterialInfraestructure subItem = new SubItemMaterialInfraestructure();
                    //SubItemMaterialEntity subitem = subItem.Select(item.TB_SUBITEM_MATERIAL_ID);

                    erro.Add("Erro");

                    // throw new Exception("Saldo negativo, por favor rever os movimentos para o SubItem : " + subitem.Codigo + "  Saldo quantidade : " + Convert.ToDecimal(cmd.Parameters["@SALDOQTD"].Value) + "  Saldo Valor : " + Convert.ToDecimal(cmd.Parameters["@SALDOVALOR"].Value).ToString("#.###,##"));
                }

                cmd.Parameters.Clear();


                if (erro.Count > 0)
                    transaction.Rollback();
                else
                    transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                erro.Add(ex.Message);
            }
            finally
            {

                transaction.Dispose();
                transaction = null;
                cmd = null;
                cnn.Close();
            }

            return Convert.ToBoolean(retornoParametro);
        }


        public MovimentoEntity gravarMovimentoEntrada()
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable }))
            {
                if (Db.Connection.State == System.Data.ConnectionState.Closed)
                    Db.Connection.Open();

                DbTransaction transaction = Db.Connection.BeginTransaction();
                try
                {
                    TB_MOVIMENTO movimento = new TB_MOVIMENTO();
                    EntitySet<TB_MOVIMENTO_ITEM> listaMovimentoItem = new EntitySet<TB_MOVIMENTO_ITEM>();

                    this.Db.TB_MOVIMENTOs.InsertOnSubmit(movimento);

                    foreach (MovimentoItemEntity movItem in this.Entity.MovimentoItem)
                    {

                        if (existeSaldoParaMovimento(movItem))
                        {
                            atualizarSaldoMovimentoEntrada(movItem, true);
                        }
                        else
                        {
                            inserirSaldoMovimentoEntrada(movItem);
                        }


                        TB_MOVIMENTO_ITEM movimentoItem = new TB_MOVIMENTO_ITEM();

                        movimentoItem.TB_MOVIMENTO_ITEM_ID = 0;

                        this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(movimentoItem);
                        movimentoItem.TB_MOVIMENTO_ITEM_ID = movItem.Id.HasValue ? movItem.Id.Value : 0;
                        if (movItem.UGE != null)
                            movimentoItem.TB_UGE_ID = movItem.UGE.Id.HasValue ? movItem.UGE.Id.Value : 0;

                        movimentoItem.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
                        //movimentoItem.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;

                        if (!(DateTime.MinValue == movItem.DataVencimentoLote))
                            movimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItem.DataVencimentoLote;

                        movimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR = movItem.FabricanteLote;
                        movimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT = movItem.IdentificacaoLote;
                        movimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ = movItem.QtdeLiq;
                        movimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov;
                        movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = movItem.SaldoQtde;
                        movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItem.SaldoQtdeLote;
                        movimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = movItem.SaldoValor;
                        movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
                        movimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
                        movimentoItem.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;
                        movimentoItem.TB_ITEM_MATERIAL_ID = (movItem.SubItemMaterial.ItemMaterial != null) ? movItem.SubItemMaterial.ItemMaterial.Id : null;

                        movimentoItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = movItem.NL_Liquidacao;
                        movimentoItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO = movItem.NL_Reclassificacao;
                        //movimentoItem.TB_PTRES_ID = (movItem.PTRes.Id.HasValue ? movItem.PTRes.Id : null);
                        //movimentoItem.TB_PTRES_CODIGO = (movItem.PTRes.Id.HasValue ? movItem.PTRes.Codigo : null);
                        //movimentoItem.TB_PTRES_PT_ACAO = (movItem.PTRes.Id.HasValue ? movItem.PTRes.ProgramaTrabalho.Acao : null);
                        //movimentoItem.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO = movItem.CodigoUnidadeFornecimentoSiafisico;
                        listaMovimentoItem.Add(movimentoItem);

                    }

                    movimento.TB_MOVIMENTO_ITEMs = listaMovimentoItem;
                    movimento.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;

                    movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
                    movimento.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.DataDocumento.Value;
                    movimento.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value.Date;
                    movimento.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
                    movimento.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
                    movimento.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
                    movimento.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
                    movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
                    movimento.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
                    movimento.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.ValorDocumento.Value.truncarDuasCasas();
                    movimento.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;
                    movimento.TB_LOGIN_ID = this.Entity.IdLogin;
                    movimento.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;
                    movimento.TB_UGE_ID = (this.Entity.UGE != null ? (this.Entity.UGE.Id.HasValue ? movimento.TB_UGE_ID = this.Entity.UGE.Id.Value : null) : null);
                    movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino != null ? this.Entity.MovimAlmoxOrigemDestino.Id.HasValue ? this.Entity.MovimAlmoxOrigemDestino.Id : null : null;
                    movimento.TB_EMPENHO_EVENTO_ID = this.Entity.EmpenhoEvento != null ? this.Entity.EmpenhoEvento.Id.HasValue ? this.Entity.EmpenhoEvento.Id : null : null;
                    movimento.TB_EMPENHO_LICITACAO_ID = this.Entity.EmpenhoLicitacao != null ? this.Entity.EmpenhoLicitacao.Id.HasValue ? this.Entity.EmpenhoLicitacao.Id : null : null;
                    movimento.TB_DIVISAO_ID = this.Entity.Divisao != null ? this.Entity.Divisao.Id != null ? this.Entity.Divisao.Id : null : null;
                    movimento.TB_FORNECEDOR_ID = this.Entity.Fornecedor != null ? this.Entity.Fornecedor.Id.HasValue ? this.Entity.Fornecedor.Id : null : null;

                    movimento.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;
                    this.Db.SubmitChanges();

                    //retorna o Movimento Atualizado depois de inserir
                    this.Movimento.Id = movimento.TB_MOVIMENTO_ID;
                    Entity.Id = movimento.TB_MOVIMENTO_ID;
                    transaction.Commit();
                    ts.Complete();
                    return this.Entity;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }



            }


        }
        private TB_SALDO_SUBITEM getSaldoDoSubItem(MovimentoItemEntity movimentoItem)
        {
            TB_SALDO_SUBITEM saldo = new TB_SALDO_SUBITEM();
            saldo = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == movimentoItem.Movimento.Almoxarifado.Id).
            Where(a => a.TB_UGE_ID == movimentoItem.UGE.Id).
            Where(a => a.TB_SUBITEM_MATERIAL_ID == movimentoItem.SubItemMaterial.Id).FirstOrDefault();
            // Where(a => a.TB_SALDO_SUBITEM_SALDO_QTDE != 0).FirstOrDefault();

            return saldo;
        }

        private MovimentoItemEntity getUltimoMovimentoItemDoSubItem(MovimentoItemEntity movimentoItemSaida)
        {
            //IQueryable<MovimentoItemEntity> query = (from q in Db.TB_MOVIMENTO_ITEMs
            //                                         join m in Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
            //                                         join t in Db.TB_TIPO_MOVIMENTOs on m.TB_TIPO_MOVIMENTO_ID equals t.TB_TIPO_MOVIMENTO_ID
            //                                         join g in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on t.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
            //                                         where m.TB_ALMOXARIFADO_ID == movimentoItemSaida.Movimento.Almoxarifado.Id
            //                                         where m.TB_MOVIMENTO_ATIVO == true
            //                                         where q.TB_UGE_ID == movimentoItemSaida.UGE.Id
            //                                         where q.TB_SUBITEM_MATERIAL_ID == movimentoItemSaida.SubItemMaterial.Id
            //                                         where q.TB_MOVIMENTO_ITEM_ATIVO == true
            //                                         where (g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
            //                                                || g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
            //                                         orderby m.TB_MOVIMENTO_DATA_MOVIMENTO descending, q.TB_MOVIMENTO_ITEM_ID descending

            //  ----- NUNCA FAÇA ISSO QUE A QUERY GERADA PELO ENTITY CRIA SELECT DE OUTRO SELECT
            //                                         select new MovimentoItemEntity
            //                                         {
            //                                             Id = q.TB_MOVIMENTO_ITEM_ID,
            //                                             SubItemMaterial = new SubItemMaterialEntity(q.TB_SUBITEM_MATERIAL_ID),
            //                                             UGE = new UGEEntity(q.TB_UGE_ID),
            //                                             SaldoQtde = q.TB_MOVIMENTO_ITEM_SALDO_QTDE,
            //                                             SaldoValor = q.TB_MOVIMENTO_ITEM_SALDO_VALOR,
            //                                             DataMovimento = q.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
            //                                             PrecoUnit = q.TB_MOVIMENTO_ITEM_PRECO_UNIT

            IQueryable<TB_MOVIMENTO_ITEM> query = (from q in Db.TB_MOVIMENTO_ITEMs
                                                   join m in Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                   join t in Db.TB_TIPO_MOVIMENTOs on m.TB_TIPO_MOVIMENTO_ID equals t.TB_TIPO_MOVIMENTO_ID
                                                   join g in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on t.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                   where m.TB_ALMOXARIFADO_ID == movimentoItemSaida.Movimento.Almoxarifado.Id
                                                   where m.TB_MOVIMENTO_ATIVO == true
                                                   where q.TB_UGE_ID == movimentoItemSaida.UGE.Id
                                                   where q.TB_SUBITEM_MATERIAL_ID == movimentoItemSaida.SubItemMaterial.Id
                                                   where q.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   where (g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                                          || g.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                   orderby m.TB_MOVIMENTO_DATA_MOVIMENTO descending, q.TB_MOVIMENTO_ITEM_ID descending
                                                   select q).Take(1);

            var retorno = query.FirstOrDefault();

            if (retorno == null)
                throw new Exception(String.Format("Saldo Indisponível para o SubItem {0} - {1} !", movimentoItemSaida.SubItemMaterial.Codigo, movimentoItemSaida.SubItemMaterial.Descricao));

            return new MovimentoItemEntity
            {
                Id = retorno.TB_MOVIMENTO_ITEM_ID,
                SubItemMaterial = new SubItemMaterialEntity(retorno.TB_SUBITEM_MATERIAL_ID),
                UGE = new UGEEntity(retorno.TB_UGE_ID),
                SaldoQtde = retorno.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                SaldoValor = retorno.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                DataMovimento = retorno.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                PrecoUnit = retorno.TB_MOVIMENTO_ITEM_PRECO_UNIT
            };

        }


        protected bool existeSaldoParaMovimento(MovimentoItemEntity movItem)
        {
            IEnumerable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                    where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                    saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                    saldo.TB_UGE_ID == movItem.UGE.Id
                                                    select saldo);

            /*
                implementação para lotes 
            */

            //result = result.Where(a => a.TB_SALDO_SUBITEM_LOTE_FAB == movItem.FabricanteLote &&
            //    a.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.DataVencimentoLote &&
            //    a.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote).ToList();

            int contador = result.Count();

            if (contador > 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Faz updata do saldo para cada SubItem
        /// </summary>
        protected void atualizarSaldoMovimentoEntrada(MovimentoItemEntity movItem, bool somarSaldo)
        {
            //Retorno o objeto Saldo do subItem
            IEnumerable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                    where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                    saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                    saldo.TB_UGE_ID == movItem.UGE.Id
                                                    select saldo);

            TB_SALDO_SUBITEM resultado = result.FirstOrDefault();

            //Se encontrou o Saldo
            if (resultado != null)
            {

                resultado.TB_SALDO_SUBITEM_SALDO_QTDE += movItem.QtdeMov;
                resultado.TB_SALDO_SUBITEM_SALDO_VALOR += movItem.ValorMov;

                resultado.TB_SALDO_SUBITEM_PRECO_UNIT = CalcularPrecoMedioSaldo(resultado.TB_SALDO_SUBITEM_SALDO_VALOR, resultado.TB_SALDO_SUBITEM_SALDO_QTDE, movItem.AnoMesReferencia);

            }
        }

        protected Boolean getMovimentoRetroativo(MovimentoItemEntity movItem)
        {

            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                          || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          select new MovimentoItemEntity
                          {
                              Id = i.TB_MOVIMENTO_ITEM_ID
                          }).Count();

            if (result > 0)
                return true;
            else
                return false;
        }


        protected void inserirSaldoMovimentoEntrada(MovimentoItemEntity movItem)
        {
            TB_SALDO_SUBITEM saldoSubItem = new TB_SALDO_SUBITEM();
            saldoSubItem.TB_ALMOXARIFADO_ID = (int)Movimento.Almoxarifado.Id;

            saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC = movItem.DataVencimentoLote;
            saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB = movItem.FabricanteLote;
            saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT = movItem.IdentificacaoLote;
            // calcula preço médio
            saldoSubItem.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit;

            saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE = movItem.QtdeMov;
            saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR = movItem.ValorMov;
            saldoSubItem.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;
            saldoSubItem.TB_UGE_ID = (int)movItem.UGE.Id;

            Db.TB_SALDO_SUBITEMs.InsertOnSubmit(saldoSubItem);

        }

        public MovimentoEntity GravarMovimento()
        {
            TB_MOVIMENTO tbMov = new TB_MOVIMENTO();
            EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();

            this.Db.TB_MOVIMENTOs.InsertOnSubmit(tbMov);


            foreach (MovimentoItemEntity movItem in this.Entity.MovimentoItem)
            {
                if (movItem.QtdeMov.HasValue && movItem.QtdeMov != 0)
                {
                    if (movItem.PrecoUnit == 0)
                        throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");


                    //if (VerificarSeEstoqueZerado(movItem))
                    //{
                    //    throw new Exception("Estoque zerado, não é permitido uma inclusão retroativa!");
                    //}
                    TB_MOVIMENTO_ITEM itemAdd = new TB_MOVIMENTO_ITEM();

                    itemAdd.TB_MOVIMENTO_ITEM_ID = 0;

                    this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(itemAdd);
                    itemAdd.TB_MOVIMENTO_ITEM_ID = movItem.Id.HasValue ? movItem.Id.Value : 0;
                    if (movItem.UGE != null)
                        itemAdd.TB_UGE_ID = movItem.UGE.Id.HasValue ? movItem.UGE.Id.Value : 0;

                    itemAdd.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
                    // itemAdd.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;

                    if (!(DateTime.MinValue == movItem.DataVencimentoLote))
                        itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItem.DataVencimentoLote;

                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = movItem.FabricanteLote;
                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = (String.IsNullOrWhiteSpace(movItem.IdentificacaoLote) ? string.Empty : movItem.IdentificacaoLote);
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = movItem.QtdeLiq;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE = movItem.SaldoQtde;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItem.SaldoQtdeLote;
                    itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = movItem.SaldoValor;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
                    itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP = movItem.ValorUnit;
                    itemAdd.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;
                    itemAdd.TB_ITEM_MATERIAL_ID = (movItem.SubItemMaterial.ItemMaterial != null) ? movItem.SubItemMaterial.ItemMaterial.Id : null;

                    itemAdd.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = movItem.NL_Liquidacao;
                    itemAdd.TB_PTRES_ID = (movItem.PTRes.IsNotNull()) ? movItem.PTRes.Id : null;
                    itemAdd.TB_PTRES_CODIGO = ((movItem.PTRes.IsNotNull() && movItem.PTRes.Id.HasValue) ? movItem.PTRes.Codigo : null);
                    itemAdd.TB_PTRES_PT_ACAO = ((movItem.PTRes.IsNotNull() && movItem.PTRes.Id.HasValue) ? movItem.PTRes.ProgramaTrabalho.ProjetoAtividade : null);
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_LIQUIDAR = movItem.SaldoLiq;
                    //Campo utilizado por empenho
                    //itemAdd.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO = movItem.CodigoUnidadeFornecimentoSiafisico;
                    item.Add(itemAdd);
                }
            }

            tbMov.TB_MOVIMENTO_ITEMs = item;
            tbMov.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;

            tbMov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
            tbMov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.DataDocumento.Value;
            tbMov.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value.Date;
            tbMov.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
            tbMov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
            tbMov.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
            tbMov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
            tbMov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
            tbMov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
            tbMov.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.ValorDocumento.Value.truncarDuasCasas();
            tbMov.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;
            tbMov.TB_LOGIN_ID = this.Entity.IdLogin;
            tbMov.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;
            tbMov.TB_UGE_ID = (this.Entity.UGE != null ? (this.Entity.UGE.Id.HasValue ? tbMov.TB_UGE_ID = this.Entity.UGE.Id.Value : null) : null);
            tbMov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino != null ? this.Entity.MovimAlmoxOrigemDestino.Id.HasValue ? this.Entity.MovimAlmoxOrigemDestino.Id : null : null;
            tbMov.TB_EMPENHO_EVENTO_ID = this.Entity.EmpenhoEvento != null ? this.Entity.EmpenhoEvento.Id.HasValue ? this.Entity.EmpenhoEvento.Id : null : null;
            tbMov.TB_EMPENHO_LICITACAO_ID = this.Entity.EmpenhoLicitacao != null ? this.Entity.EmpenhoLicitacao.Id.HasValue ? this.Entity.EmpenhoLicitacao.Id : null : null;
            tbMov.TB_DIVISAO_ID = this.Entity.Divisao != null ? this.Entity.Divisao.Id != null ? this.Entity.Divisao.Id : null : null;
            tbMov.TB_FORNECEDOR_ID = this.Entity.Fornecedor != null ? this.Entity.Fornecedor.Id.HasValue ? this.Entity.Fornecedor.Id : null : null;
            tbMov.TB_SUBTIPO_MOVIMENTO_ID = this.Entity.SubTipoMovimentoId != null ? this.Entity.SubTipoMovimentoId : null;

            tbMov.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;
            tbMov.TB_MOVIMENTO_CE = this.Entity.InscricaoCE;
            this.Db.SubmitChanges();

            //retorna o Movimento Atualizado depois de inserir
            this.Movimento.Id = tbMov.TB_MOVIMENTO_ID;
            Entity.Id = tbMov.TB_MOVIMENTO_ID;

            return GetMovimento();
        }


        public MovimentoEntity GravarMovimentoConsumo()
        {
            TB_MOVIMENTO tbMov = new TB_MOVIMENTO();
            EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();

            this.Db.TB_MOVIMENTOs.InsertOnSubmit(tbMov);


            foreach (MovimentoItemEntity movItem in this.Entity.MovimentoItem)
            {
                if (movItem.QtdeMov.HasValue && movItem.QtdeMov != 0)
                {
                    if (movItem.PrecoUnit == 0)
                        throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");



                    TB_MOVIMENTO_ITEM itemAdd = new TB_MOVIMENTO_ITEM();

                    itemAdd.TB_MOVIMENTO_ITEM_ID = 0;

                    this.Db.TB_MOVIMENTO_ITEMs.InsertOnSubmit(itemAdd);
                    itemAdd.TB_MOVIMENTO_ITEM_ID = movItem.Id.HasValue ? movItem.Id.Value : 0;
                    if (movItem.UGE != null)
                        itemAdd.TB_UGE_ID = movItem.UGE.Id.HasValue ? movItem.UGE.Id.Value : 0;

                    itemAdd.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
                    // itemAdd.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;

                    if (!(DateTime.MinValue == movItem.DataVencimentoLote))
                        itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItem.DataVencimentoLote;

                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = movItem.UnidadeFornecimentoSiafisico != null ? movItem.UnidadeFornecimentoSiafisicoDescricao : string.Empty;
                    itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = (String.IsNullOrWhiteSpace(movItem.IdentificacaoLote) ? string.Empty : movItem.IdentificacaoLote);
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = movItem.QtdeLiq;
                    itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
                    itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP = movItem.ValorUnit;
                    itemAdd.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;
                    itemAdd.TB_ITEM_MATERIAL_ID = (movItem.SubItemMaterial.ItemMaterial != null) ? movItem.SubItemMaterial.ItemMaterial.Id : null;

                    itemAdd.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = movItem.NL_Liquidacao;

                    //itemAdd.TB_PTRES_CODIGO = ((movItem.PTRes.IsNotNull() && movItem.PTRes.Id.HasValue) ? movItem.PTRes.Codigo : null);
                    itemAdd.TB_PTRES_CODIGO = ((movItem.PTRes.IsNotNull() && movItem.PTRes.Codigo.HasValue) ? movItem.PTRes.Codigo : null);
                    itemAdd.TB_PTRES_PT_ACAO = ((movItem.PTRes.IsNotNull() && !String.IsNullOrWhiteSpace(movItem.PTRes.ProgramaTrabalho.ProjetoAtividade)) ? movItem.PTRes.ProgramaTrabalho.ProjetoAtividade : null);
                    itemAdd.TB_MOVIMENTO_ITEM_VALOR_LIQUIDAR = movItem.SaldoLiq;
                    item.Add(itemAdd);
                }
            }

            tbMov.TB_MOVIMENTO_ITEMs = item;
            tbMov.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;

            tbMov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.AnoMesReferencia;
            tbMov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.DataDocumento.Value;
            tbMov.TB_MOVIMENTO_DATA_MOVIMENTO = this.Entity.DataMovimento.Value.Date;
            tbMov.TB_MOVIMENTO_EMPENHO = this.Entity.Empenho;
            tbMov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.FonteRecurso;
            tbMov.TB_MOVIMENTO_GERADOR_DESCRICAO = this.Entity.GeradorDescricao;
            tbMov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Instrucoes;
            tbMov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.NumeroDocumento;
            tbMov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Observacoes;
            tbMov.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.ValorDocumento.Value.truncarDuasCasas();
            tbMov.TB_MOVIMENTO_ATIVO = this.Entity.Ativo;
            tbMov.TB_LOGIN_ID = this.Entity.IdLogin;
            tbMov.TB_MOVIMENTO_DATA_OPERACAO = this.Entity.DataOperacao;
            tbMov.TB_UGE_ID = (this.Entity.UGE != null ? (this.Entity.UGE.Id.HasValue ? tbMov.TB_UGE_ID = this.Entity.UGE.Id.Value : null) : null);
            tbMov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.MovimAlmoxOrigemDestino != null ? this.Entity.MovimAlmoxOrigemDestino.Id.HasValue ? this.Entity.MovimAlmoxOrigemDestino.Id : null : null;
            tbMov.TB_EMPENHO_EVENTO_ID = this.Entity.EmpenhoEvento != null ? this.Entity.EmpenhoEvento.Id.HasValue ? this.Entity.EmpenhoEvento.Id : null : null;
            tbMov.TB_EMPENHO_LICITACAO_ID = this.Entity.EmpenhoLicitacao != null ? this.Entity.EmpenhoLicitacao.Id.HasValue ? this.Entity.EmpenhoLicitacao.Id : null : null;
            tbMov.TB_DIVISAO_ID = this.Entity.Divisao != null ? this.Entity.Divisao.Id != null ? this.Entity.Divisao.Id : null : null;
            tbMov.TB_FORNECEDOR_ID = this.Entity.Fornecedor != null ? this.Entity.Fornecedor.Id.HasValue ? this.Entity.Fornecedor.Id : null : null;

            tbMov.TB_TIPO_MOVIMENTO_ID = this.Entity.TipoMovimento.Id;
            tbMov.TB_MOVIMENTO_CE = this.Entity.InscricaoCE;
            tbMov.TB_UA_ID = this.Entity.UA.Id;
            this.Db.SubmitChanges();

            //retorna o Movimento Atualizado depois de inserir
            this.Movimento.Id = tbMov.TB_MOVIMENTO_ID;
            Entity.Id = tbMov.TB_MOVIMENTO_ID;

            return GetMovimento();
        }


        public void CalcularSaldoTotal(MovimentoItemEntity movItem)
        {
            SaldoSubItemEntity result = (from saldo in Db.TB_SALDO_SUBITEMs
                                         where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id
                                         where saldo.TB_ALMOXARIFADO_ID == movItem.Movimento.Almoxarifado.Id
                                         where saldo.TB_UGE_ID == movItem.UGE.Id
                                         group saldo by new
                                         {
                                             saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                             saldo.TB_UGE.TB_UGE_ID,
                                             saldo.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID
                                         } into grupo
                                         select new SaldoSubItemEntity
                                         {
                                             UGE = new UGEEntity(grupo.Key.TB_UGE_ID),
                                             SubItemMaterial = new SubItemMaterialEntity(grupo.Key.TB_SUBITEM_MATERIAL_ID),
                                             Almoxarifado = new AlmoxarifadoEntity(grupo.Key.TB_ALMOXARIFADO_ID),
                                             SaldoQtde = grupo.Sum(a => a.TB_SALDO_SUBITEM_SALDO_QTDE) ?? 0,
                                             SaldoValor = grupo.Sum(a => a.TB_SALDO_SUBITEM_SALDO_VALOR) ?? 0
                                         }
                          ).FirstOrDefault();
            movItem.SaldoQtde = result != null ? result.SaldoQtde : 0;
            movItem.SaldoValor = result != null ? result.SaldoValor : 0;
        }

        public bool ExisteSaldo(MovimentoItemEntity movItem)
        {
            IEnumerable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                        //IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                    where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                    saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                    saldo.TB_UGE_ID == movItem.UGE.Id
                                                    select saldo).AsEnumerable();

            //result = result.Where(a => a.TB_SALDO_SUBITEM_LOTE_FAB == movItem.FabricanteLote &&
            //    a.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.DataVencimentoLote &&
            //    a.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote);//.ToList();

            int contador = result.Count();

            //string _strSQL = result.ToString();
            //Db.GetCommand(result as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            if (contador > 0)
                return true;
            else
                return false;
        }

        public int ExisteSaldoId(MovimentoItemEntity movItem)
        {
            int? IdAlmoxarifado = this.Movimento.Almoxarifado == null ? movItem.Movimento.Almoxarifado.Id : this.Movimento.Almoxarifado.Id;

            TB_SALDO_SUBITEM result = (from saldo in Db.TB_SALDO_SUBITEMs
                                           //IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                       where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                       saldo.TB_ALMOXARIFADO_ID == IdAlmoxarifado &&
                                       saldo.TB_UGE_ID == movItem.UGE.Id
                                       select saldo).FirstOrDefault();

            if (result != null)
                return result.TB_SALDO_SUBITEM_ID;
            else
                return 0;

        }



        public bool ExisteSaldoLote(MovimentoItemEntity movItem, int IdSaldo)
        {
            int cont = (from saldolote in Db.TB_SALDO_SUBITEM_LOTEs
                            //IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                        where saldolote.TB_SALDO_SUBITEM_ID == IdSaldo &&
                          saldolote.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.DataVencimentoLote &&
                          saldolote.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote
                        select saldolote).Count();

            return cont > 0 ? true : false;



        }

        /// <summary>
        /// Movimento Item que atualizará o saldoItem
        /// </summary>
        /// <param name="movItem"></param>
        public void RecalcularPrecoMedioSaldo(MovimentoItemEntity movimentoItemRecalculado)
        {
            try
            {
                //retorna o SubItemMaterial por lote para ser atualizado
                var saldoSubItem = this.Db.TB_SALDO_SUBITEMs.Where(
                    a => a.TB_ALMOXARIFADO_ID == movimentoItemRecalculado.Movimento.Almoxarifado.Id
                    && a.TB_UGE_ID == movimentoItemRecalculado.UGE.Id
                    && a.TB_SUBITEM_MATERIAL_ID == movimentoItemRecalculado.SubItemMaterial.Id
                    ).FirstOrDefault();

                if (saldoSubItem == null)
                    throw new Exception("Não foi possível atualizar o saldo.");

                //Atualiza o preço medio
                saldoSubItem.TB_SALDO_SUBITEM_PRECO_UNIT = movimentoItemRecalculado.PrecoUnit;

                //Atualiza o saldo valor
                saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR = movimentoItemRecalculado.SaldoValor;
                saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE = movimentoItemRecalculado.SaldoQtde;
                Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public decimal CalcularPrecoMedioSaldo(decimal? SaldoValor, decimal? quantidade, String AnoMesReferencia)
        {
            if (SaldoValor == null)
                throw new Exception("Não foi possível calcular o preço médio. A quantidade o saldo está nulo");

            if (quantidade == null)
                throw new Exception("Não foi possível calcular o preço médio. A quantidade está nula");

            //Se o saldo em valor ou em qtde estiver zerado, retorna 0 como preço médio
            if (SaldoValor == 0.00m || quantidade == _valorZero)
                return 0;

            try
            {
                if ((quantidade.HasValue) && (quantidade != 0))
                {
                    decimal? precoUnit = (SaldoValor / quantidade);

                    return precoUnit.Value.Truncar(AnoMesReferencia, true);
                }
                else
                    throw new Exception("Não foi possível calcular o preço médio.");
            }
            catch
            {
                throw new Exception("Não foi possível calcular o preço médio.");

            }
        }

        private decimal calculaDesdobro(decimal valorSaldo, decimal qtdSaldo, decimal precoUnitario)
        {
            return valorSaldo - (qtdSaldo * precoUnitario.truncarQuatroCasas());
        }
        /// <summary>
        /// Faz updata do saldo para cada SubItem
        /// </summary>
        public void AtualizarSaldo(MovimentoItemEntity movItem, bool somarSaldo)
        {
            //Retorno o objeto Saldo do subItem
            //IEnumerable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs

            TB_SALDO_SUBITEM_LOTE saldoSubItemLote = new TB_SALDO_SUBITEM_LOTE();
            IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                   where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                   saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                   saldo.TB_UGE_ID == movItem.UGE.Id
                                                   select saldo);

            TB_SALDO_SUBITEM resultado = result.FirstOrDefault();

            string _strSQL = result.ToString();
            Db.GetCommand(result).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            //Se encontrou o Saldo
            if (resultado != null)
            {
                if (somarSaldo)
                {
                    if (movItem.QtdeMov != null)
                        resultado.TB_SALDO_SUBITEM_SALDO_QTDE += movItem.QtdeMov;
                    if (movItem.ValorMov != null)
                        resultado.TB_SALDO_SUBITEM_SALDO_VALOR += movItem.ValorMov;

                }
                else //Usado no estorno da entrada, subtrai o saldo.
                {
                    if (movItem.QtdeMov != null)
                        resultado.TB_SALDO_SUBITEM_SALDO_QTDE -= movItem.QtdeMov;
                    if (movItem.ValorMov != null)
                        resultado.TB_SALDO_SUBITEM_SALDO_VALOR -= movItem.ValorMov;
                }

                if (movItem.AgrupamentoId == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                    resultado.TB_SALDO_SUBITEM_PRECO_UNIT = CalcularPrecoMedioSaldo(resultado.TB_SALDO_SUBITEM_SALDO_VALOR, resultado.TB_SALDO_SUBITEM_SALDO_QTDE, movItem.AnoMesReferencia);

                resultado.TB_SALDO_SUBITEM_DESDOBRO = calculaDesdobro(resultado.TB_SALDO_SUBITEM_SALDO_VALOR.Value, resultado.TB_SALDO_SUBITEM_SALDO_QTDE.Value, resultado.TB_SALDO_SUBITEM_PRECO_UNIT.Value);

                if (!movItem.Retroativo)
                {
                    var ultimoMovimentoSaidaAnterior = this.RetornaPrecoMedioMovimentoItemRetroativo(movItem);

                    if (ultimoMovimentoSaidaAnterior != null)
                    {
                        resultado.TB_SALDO_SUBITEM_PRECO_UNIT = ultimoMovimentoSaidaAnterior.PrecoUnit;
                        resultado.TB_SALDO_SUBITEM_DESDOBRO = ultimoMovimentoSaidaAnterior.Desd;
                    }

                }


                if (resultado.TB_SALDO_SUBITEM_PRECO_UNIT == 0 && resultado.TB_SALDO_SUBITEM_SALDO_QTDE != 0)
                    throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");


                InserirAtualizarSaldoLoteItem(movItem, somarSaldo, resultado.TB_SALDO_SUBITEM_ID);

                Db.SubmitChanges();
            }
            else
            {
                throw new Exception("Não foi possivel Atualizar o saldo");
            }
        }

        public void AtualizarSaldoLote(MovimentoItemEntity movItem, int IdSaldo)
        {
            //Retorno o objeto Saldo do subItem
            //IEnumerable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
            IQueryable<TB_SALDO_SUBITEM_LOTE> result = (from saldolote in Db.TB_SALDO_SUBITEM_LOTEs
                                                        where saldolote.TB_SALDO_SUBITEM_ID == IdSaldo &&
                                                              saldolote.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.DataVencimentoLote &&
                                                              saldolote.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote
                                                        select saldolote);

            TB_SALDO_SUBITEM_LOTE resultado = result.FirstOrDefault();

            string _strSQL = result.ToString();
            Db.GetCommand(result).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            //Se encontrou o Saldo
            if (resultado != null)
            {

                if (movItem.QtdeMov != null)
                    resultado.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE += Convert.ToDecimal(movItem.QtdeMov);



                Db.SubmitChanges();
            }
            else
            {
                throw new Exception("Não foi possivel Atualizar o saldo");
            }


        }

        public void AtualizarSaldoMovimentoRetroativo(MovimentoItemEntity movItem)
        {
            //Retorno o objeto Saldo do subItem
            IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                   where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                   saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                   saldo.TB_UGE_ID == movItem.UGE.Id
                                                   select saldo).AsQueryable();

            TB_SALDO_SUBITEM resultado = result.FirstOrDefault();

            //Se encontrou o Saldo
            if (resultado != null)
            {
                resultado.TB_SALDO_SUBITEM_SALDO_QTDE = movItem.SaldoQtde;

                resultado.TB_SALDO_SUBITEM_SALDO_VALOR = movItem.SaldoValor;

                resultado.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit;

                resultado.TB_SALDO_SUBITEM_DESDOBRO = movItem.Desd;

                Db.SubmitChanges();
            }
            else
            {
                throw new Exception("Não foi possivel Atualizar o saldo");
            }


        }

        public Decimal getPrecoUnitarioSaldo(MovimentoItemEntity movimento)
        {
            //Retorno o objeto Saldo do subItem
            IQueryable<Decimal> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                          where saldo.TB_SUBITEM_MATERIAL_ID == movimento.SubItemMaterial.Id &&
                                           saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                           saldo.TB_UGE_ID == movimento.UGE.Id
                                          select saldo.TB_SALDO_SUBITEM_PRECO_UNIT.Value);

            Decimal resultado = result.FirstOrDefault();

            return resultado;

        }
        public decimal? AtualizarDesdobroSaldo(MovimentoItemEntity movimento, bool alterarSaldo)
        {
            IQueryable<TB_SALDO_SUBITEM> result;

            if (!ExisteSaldo(movimento))
                InserirSaldo(movimento);
            else if (alterarSaldo == true)
                AtualizarSaldoMovimentoRetroativo(movimento);

            //Retorno o objeto Saldo do subItem
            result = (from saldo in Db.TB_SALDO_SUBITEMs
                      where saldo.TB_SUBITEM_MATERIAL_ID == movimento.SubItemMaterial.Id &&
                       saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                       saldo.TB_UGE_ID == movimento.UGE.Id
                      select saldo);

            TB_SALDO_SUBITEM resultado = result.FirstOrDefault();

            resultado.TB_SALDO_SUBITEM_DESDOBRO = calculaDesdobro(movimento.SaldoValor.Value, movimento.SaldoQtde.Value, movimento.PrecoUnit.Value);

            Db.SubmitChanges();
            return resultado.TB_SALDO_SUBITEM_DESDOBRO;
            //resultado.TB_SALDO_SUBITEM_DESDOBRO = calculaDesdobro(resultado.TB_SALDO_SUBITEM_SALDO_VALOR.Value, resultado.TB_SALDO_SUBITEM_SALDO_QTDE.Value, resultado.TB_SALDO_SUBITEM_PRECO_UNIT.Value).Truncar(movItem.AnoMesReferencia, true);


        }


        public void InserirSaldo(MovimentoItemEntity movItem)
        {
            TB_SALDO_SUBITEM saldoSubItem = new TB_SALDO_SUBITEM();
            TB_SALDO_SUBITEM_LOTE saldoSubItemLote = new TB_SALDO_SUBITEM_LOTE();
            saldoSubItem.TB_ALMOXARIFADO_ID = (int)Movimento.Almoxarifado.Id;


            // calcula preço médio
            saldoSubItem.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit;

            saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE = movItem.QtdeMov;
            saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR = movItem.ValorMov;
            saldoSubItem.TB_SUBITEM_MATERIAL_ID = (int)movItem.SubItemMaterial.Id;
            saldoSubItem.TB_UGE_ID = (int)movItem.UGE.Id;
            var desdobro = calculaDesdobro(movItem.ValorMov.Value, movItem.QtdeMov.Value, movItem.PrecoUnit.Value);
            saldoSubItem.TB_SALDO_SUBITEM_DESDOBRO = (desdobro > 0 ? desdobro : 0);

            Db.TB_SALDO_SUBITEMs.InsertOnSubmit(saldoSubItem);


            IQueryable<TB_SALDO_SUBITEM> result = (from saldo in Db.TB_SALDO_SUBITEMs
                                                   where saldo.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id &&
                                                   saldo.TB_ALMOXARIFADO_ID == this.Movimento.Almoxarifado.Id &&
                                                   saldo.TB_UGE_ID == movItem.UGE.Id
                                                   select saldo);


            saldoSubItemLote.TB_SALDO_SUBITEM = saldoSubItem;
            if (!String.IsNullOrWhiteSpace(movItem.IdentificacaoLote))
            {
                if (movItem.DataVencimentoLote != null)
                {
                    saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DT_VENC = Convert.ToDateTime(movItem.DataVencimentoLote);
                }
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = movItem.IdentificacaoLote;
            }
            else
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = string.Empty;


            saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE = Convert.ToDecimal(movItem.QtdeMov);
            if (movItem.Movimento.TipoMovimento.Id == (int)TipoMovimento.EntradaPorEmpenho)
            {
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = movItem.Movimento.DataMovimento.Value; //DateTime.Now;
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = String.Format("{0}", movItem.IdentificacaoLote, movItem.Movimento.NumeroDocumento);
            }

            else
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;

            if (this.entradaPorEmpenho == true)
                saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_ENTRADA = movItem.Movimento.DataMovimento.Value;

            Db.TB_SALDO_SUBITEM_LOTEs.InsertOnSubmit(saldoSubItemLote);

            AtualizaPrecoMedioLote(movItem);


        }

        public void AtualizaPrecoMedioLote(MovimentoItemEntity movItem)
        {
            if (Movimento.Almoxarifado == null)
                Movimento.Almoxarifado = movItem.Movimento.Almoxarifado;

            //Atualiza o preço Médio de todos os lotes
            var saldoLote = Db.TB_SALDO_SUBITEMs.Where(a => a.TB_ALMOXARIFADO_ID == Movimento.Almoxarifado.Id)
                .Where(a => a.TB_UGE_ID == movItem.UGE.Id)
                .Where(a => a.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id).ToList();

            foreach (var s in saldoLote)
            {
                s.TB_SALDO_SUBITEM_PRECO_UNIT = movItem.PrecoUnit;
            }

            if (movItem.PrecoUnit == 0 && movItem.QtdeMov != 0)
                throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");

            Db.SubmitChanges();


        }

        public TB_SALDO_SUBITEM_LOTE SaldoLote(MovimentoItemEntity movItem, int idSubItem)
        {
            var saldoSubItemLote = new TB_SALDO_SUBITEM_LOTE();

            // var saldoLote = new TB_SALDO_SUBITEM_LOTE();

            if (movItem.DataVencimentoLote != null)
            {
                saldoSubItemLote = Db.TB_SALDO_SUBITEM_LOTEs.Where(a => a.TB_SALDO_SUBITEM_ID == idSubItem)
                           .Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote)
                           .Where(a => a.TB_SALDO_SUBITEM_LOTE_DT_VENC == movItem.DataVencimentoLote).FirstOrDefault();
            }
            else
            {
                if (string.IsNullOrEmpty(movItem.IdentificacaoLote))
                    saldoSubItemLote = Db.TB_SALDO_SUBITEM_LOTEs.Where(a => a.TB_SALDO_SUBITEM_ID == idSubItem)
                                .Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == string.Empty).FirstOrDefault();
                else
                    saldoSubItemLote = Db.TB_SALDO_SUBITEM_LOTEs.Where(a => a.TB_SALDO_SUBITEM_ID == idSubItem)
                            .Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == movItem.IdentificacaoLote).FirstOrDefault();
            }
            return saldoSubItemLote;

        }

        public Tuple<string, bool> AtualizarMovimentoSubItem(MovimentoItemEntity movItem)
        {

            var movItemSubItem = new TB_MOVIMENTO_ITEM();
            movItemSubItem = Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ITEM_ID == movItem.Id)
                .Where(a => a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == movItem.Movimento.Almoxarifado.Id).FirstOrDefault();

            try
            {
                if (movItemSubItem != null)
                {
                    if (movItem.QtdeMov != null)
                    {
                        movItemSubItem.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov;
                    }
                    else if (movItem.ValorMov != null)
                    {
                        movItemSubItem.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
                    }
                    else if (movItem.NL_Consumo != null)
                    {
                        movItemSubItem.TB_MOVIMENTO_ITEM_NL_CONSUMO = movItem.NL_Consumo;
                    }
                    else if (movItem.Ativo != null)
                    {
                        movItemSubItem.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
                    }



                    Db.Refresh(RefreshMode.KeepChanges, movItemSubItem);
                    Db.SubmitChanges();

                    return Tuple.Create("MovimentoItem: " + movItem.Id + " atualizado.", true);


                }
                else
                {
                    return Tuple.Create("MovimentoItem não encontrado para este Almoxarifado.", false);

                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }




        }


        public Tuple<string, bool> AtualizarMovimentoBloquear(int idMov, bool bloquear)
        {

            var mov = new TB_MOVIMENTO();
            mov = Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == idMov).FirstOrDefault();

            try
            {
                if (mov != null)
                {
                    if (bloquear != null)
                    {
                        mov.TB_MOVIMENTO_BLOQUEAR = bloquear;
                    }


                    Db.Refresh(RefreshMode.KeepChanges, mov);
                    Db.SubmitChanges();

                    return Tuple.Create("MovimentoId: " + idMov + " atualizado.", true);


                }
                else
                {
                    return Tuple.Create("MovimentoId não encontrado.", false);

                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }




        }

        public void InserirAtualizarSaldoLoteItem(MovimentoItemEntity movItem, bool somarSaldo, int idSubItem)
        {
            TB_SALDO_SUBITEM_LOTE saldoSubItemLote = new TB_SALDO_SUBITEM_LOTE();

            if (idSubItem != null)
            {
                var saldoLote = SaldoLote(movItem, idSubItem);


                if (saldoLote != null)
                {
                    if (movItem.QtdeMov != null)
                    {
                        if (somarSaldo)
                            saldoLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE += Convert.ToDecimal(movItem.QtdeMov);
                        else
                            saldoLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE -= Convert.ToDecimal(movItem.QtdeMov);
                        //entrada por empenho
                        if (movItem.Movimento.TipoMovimento.Id == (int)TipoMovimento.EntradaPorEmpenho)
                        {
                            saldoLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = movItem.Movimento.DataMovimento.Value; //DateTime.Now;
                            saldoLote.TB_SALDO_SUBITEM_LOTE_IDENT = String.Format("{0}", movItem.IdentificacaoLote);
                        }
                        else
                            saldoLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;

                        if (this.entradaPorEmpenho == true)
                            saldoLote.TB_SALDO_SUBITEM_LOTE_DATA_ENTRADA = movItem.Movimento.DataMovimento.Value;

                        if (saldoLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE < 0)
                        {
                            throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível no Analítico do SubItem! Data Movimento: {2},  Saldo Ficará: {3}, Lote: {4}", movItem.SubItemCodigo, movItem.SubItemDescricao, movItem.DataMovimento.Value.ToString("dd/MM/yyyy"), saldoLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE, movItem.IdentificacaoLote));
                        }
                        Db.Refresh(RefreshMode.KeepChanges, saldoLote);
                    }

                }
                else
                {
                    saldoSubItemLote.TB_SALDO_SUBITEM_ID = Convert.ToInt32(idSubItem.ToString());
                    if (!string.IsNullOrEmpty(movItem.IdentificacaoLote))
                    {
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = String.Format("{0}", movItem.IdentificacaoLote);

                        if (movItem.DataVencimentoLote != null)
                            saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DT_VENC = Convert.ToDateTime(movItem.DataVencimentoLote);

                    }
                    else
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = String.Empty;

                    saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE = Convert.ToDecimal(movItem.QtdeMov);
                    //entrada por empenho
                    if (movItem.Movimento.TipoMovimento.Id == (int)TipoMovimento.EntradaPorEmpenho)
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = movItem.Movimento.DataMovimento.Value; //DateTime.Now;
                    else
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;

                    if (this.entradaPorEmpenho == true)
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_ENTRADA = movItem.Movimento.DataMovimento.Value;

                    Db.TB_SALDO_SUBITEM_LOTEs.InsertOnSubmit(saldoSubItemLote);
                }

            }

            //Db.SubmitChanges();
        }

        public void InserirAtualizarSaldoLoteItemCorrigir(MovimentoItemEntity movItem, int? idSubItem)
        {
            TB_SALDO_SUBITEM_LOTE saldoSubItemLote = new TB_SALDO_SUBITEM_LOTE();

            if (idSubItem != null)
            {
                var saldoLote = SaldoLote(movItem, Convert.ToInt32(idSubItem));

                if (saldoLote != null)
                {
                    if (movItem.QtdeMov != null)
                    {
                        saldoLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE = Convert.ToDecimal(movItem.QtdeMov);
                        saldoLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;

                        Db.Refresh(RefreshMode.KeepChanges, saldoLote);
                    }

                }
                else
                {
                    saldoSubItemLote.TB_SALDO_SUBITEM_ID = Convert.ToInt32(idSubItem.ToString());
                    if (!string.IsNullOrEmpty(movItem.IdentificacaoLote))
                    {
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DT_VENC = Convert.ToDateTime(movItem.DataVencimentoLote);
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = movItem.IdentificacaoLote;
                    }
                    else
                        saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_IDENT = string.Empty;

                    saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE = Convert.ToDecimal(movItem.QtdeMov);
                    saldoSubItemLote.TB_SALDO_SUBITEM_LOTE_DATA_MOVIMENTO = DateTime.Now;
                    Db.TB_SALDO_SUBITEM_LOTEs.InsertOnSubmit(saldoSubItemLote);
                }

            }

            Db.SubmitChanges();
        }

        //READEQUADO PARA NL RECLASSIFICACAO
        public bool ExisteNotaSiafemVinculada(int movimentoID, bool verificaEstornoNotaSIAF = false)
        {
            if (movimentoID <= 0)
                throw new ArgumentException("ID informado deve ser maior que zero!");


            bool existeNotaSIAF = false;
            int tipoMovimentacao = 0;
            TB_MOVIMENTO movimentacaoMaterial = null;
            EntitySet<TB_MOVIMENTO_ITEM> movItems = null;

            try
            {
                movimentacaoMaterial = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == movimentoID).FirstOrDefault();

                if (movimentacaoMaterial.IsNotNull())
                {
                    tipoMovimentacao = movimentacaoMaterial.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID;
                    movItems = movimentacaoMaterial.TB_MOVIMENTO_ITEMs;

                    if (movItems.IsNotNullAndNotEmpty())
                    {
                        if (tipoMovimentacao == (int)enumTipoMovimento.EntradaPorTransferencia)
                        {
                            //Entrada por transferencia sempre retornará falso, pois o tipo de entrada nao gera NL SIAFEM.
                            existeNotaSIAF = false;
                        }
                        else if (tipoMovimentacao == (int)enumTipoMovimento.RequisicaoAprovada)
                        {
                            if (verificaEstornoNotaSIAF)
                                foreach (var movItem in movItems)
                                    existeNotaSIAF |= ((String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) && !String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO)) ||
                                                       (String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) && !String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO)) ||
                                                       (!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) && String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO)) ||
                                                       (!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) && String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO)));
                            else
                                foreach (var movItem in movItems)
                                    existeNotaSIAF |= ((!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) || !String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO)) ||
                                                       (!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO) || !String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO)));
                        }
                        else
                        {
                            if (verificaEstornoNotaSIAF)
                                foreach (var movItem in movItems)
                                    existeNotaSIAF |= ((!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO)) ||
                                                       (!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO)));
                            else
                                foreach (var movItem in movItems)
                                    existeNotaSIAF |= ((!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO)) ||
                                                       (!String.IsNullOrWhiteSpace(movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO)));
                        }
                    }
                }
            }
            catch (Exception excErroLeituraBD)
            {
                throw new Exception(String.Format("Erro ao verificar se movimentação {0} possui NL SIAFEM associada (Erro: {1}).", movimentacaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO, excErroLeituraBD.Message));
            }

            return existeNotaSIAF;
        }

        // Entrada Estorno
        public bool VerificarSaldoPositivo(MovimentoItemEntity movItem)
        {
            // verifica por LOTE o saldo do subitem de material
            IList<TB_SALDO_SUBITEM> resultado = (from saldo in Db.TB_SALDO_SUBITEMs
                                                 where saldo.TB_UGE.TB_UGE_ID == movItem.UGE.Id
                                                 where saldo.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id
                                                 where saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id
                                                 select saldo).ToList();

            var result = SaldoLote(movItem, resultado.FirstOrDefault().TB_SALDO_SUBITEM_ID);

            if (resultado.Count > 0 && result != null)
            {
                if ((result.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE) < 0)
                    return false;
                else
                    return true;
            }
            else
                return false;

        }

        /// <summary>
        /// Não permite estornar entrada caso haja anteriormente sido efetuada alguma saída.
        /// </summary>
        /// <param name="movItem">Movimento Item da entrada a ser estornada</param>
        /// <returns></returns>
        public bool PermitirEstornarEntrada(MovimentoItemEntity movItem)
        {
            var ultimoMovimentoSaidaAnterior = this.RetornaPrecoMedioMovimentoItemRetroativo(movItem);

            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= Movimento.DataMovimento.Value
                          where i.TB_MOVIMENTO_ID > ultimoMovimentoSaidaAnterior.Id
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where (i.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == movItem.DataVencimentoLote || movItem.DataVencimentoLote == null)
                          where (i.TB_MOVIMENTO_ITEM_LOTE_FABR == movItem.FabricanteLote || String.IsNullOrEmpty(movItem.FabricanteLote))
                          where (i.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote || String.IsNullOrEmpty(movItem.IdentificacaoLote))
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                          select new MovimentoItemEntity
                          {
                              Id = i.TB_MOVIMENTO_ITEM_ID,
                              DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                              Movimento = new MovimentoEntity(i.TB_MOVIMENTO.TB_MOVIMENTO_ID) { DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO },
                              SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                              SaldoQtdeLote = i.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE
                          }).OrderBy(a => a.DataMovimento).ThenBy(a => a.Id).ToList();

            if (ultimoMovimentoSaidaAnterior != null)
                result.Insert(0, ultimoMovimentoSaidaAnterior);

            bool IsValid = true;
            foreach (var item in result)
            {
                if (item.SaldoQtde < movItem.QtdeMov || item.SaldoQtdeLote < movItem.QtdeMov)
                    IsValid = false;
            }

            return IsValid;

        }

        /// <summary>
        /// Verifica se existe algum movimento de entrada antes da saida do material
        /// </summary>
        /// <param name="mov"></param>
        /// <returns></returns>
        public bool VerificarMovimentoEntradaAnterior(MovimentoItemEntity movItem)
        {
            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date <= Movimento.DataMovimento.Value.Date
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          where i.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == movItem.DataVencimentoLote || movItem.DataVencimentoLote == null
                          where i.TB_MOVIMENTO_ITEM_LOTE_FABR == movItem.FabricanteLote || movItem.FabricanteLote == null
                          where i.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote || movItem.IdentificacaoLote == null
                          where i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                          (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                          select new MovimentoEntity
                          {
                              Id = i.TB_MOVIMENTO_ID
                          }).Count();

            if (result > 0)
                return true;
            else
                return false;
        }

        public IList<MovimentoItemEntity> ListMovimentoItemLoteAgrupamento(MovimentoItemEntity movItem)
        {
            IList<MovimentoItemEntity> result = (from i in Db.TB_MOVIMENTO_ITEMs
                                                 where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                                                 where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)movItem.Movimento.Almoxarifado.Id
                                                 where i.TB_UGE_ID == (int)movItem.UGE.Id
                                                 where i.TB_MOVIMENTO_ITEM_ATIVO == true
                                                 where i.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == movItem.DataVencimentoLote || movItem.DataVencimentoLote == null
                                                 where i.TB_MOVIMENTO_ITEM_LOTE_FABR == movItem.FabricanteLote || movItem.FabricanteLote == null
                                                 where i.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote || movItem.IdentificacaoLote == null
                                                 where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                                 (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                                 || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID ==
                                                 (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                 where i.TB_MOVIMENTO_ITEM_ATIVO == true
                                                 select new MovimentoItemEntity
                                                 {
                                                     Id = i.TB_MOVIMENTO_ITEM_ID,
                                                     ValorMov = i.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                     QtdeMov = i.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                     SaldoValor = i.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                     SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                     PrecoUnit = i.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                     SubItemMaterial = new SubItemMaterialEntity(i.TB_SUBITEM_MATERIAL_ID),
                                                     Movimento = (from mov in Db.TB_MOVIMENTOs
                                                                  where mov.TB_MOVIMENTO_ID == i.TB_MOVIMENTO_ID
                                                                  select new MovimentoEntity
                                                                  {
                                                                      Almoxarifado = new AlmoxarifadoEntity(mov.TB_ALMOXARIFADO_ID),
                                                                      UGE = new UGEEntity(mov.TB_UGE_ID),
                                                                      TipoMovimento = (from tipoMov in Db.TB_TIPO_MOVIMENTOs
                                                                                       where mov.TB_TIPO_MOVIMENTO_ID == tipoMov.TB_TIPO_MOVIMENTO_ID
                                                                                       select new TipoMovimentoEntity
                                                                                       {
                                                                                           Id = tipoMov.TB_TIPO_MOVIMENTO_ID,
                                                                                           TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
                                                                                       }).FirstOrDefault(),
                                                                  }).FirstOrDefault(),

                                                 }).ToList();

            return result;
        }

        /// <summary>
        /// Verifica se existe algum movimento de entrada antes da saida do material
        /// </summary>
        /// <param name="mov"></param>
        /// <returns></returns>
        public bool VerificarSaldoPositivoMovimento(MovimentoItemEntity movItem)
        {
            var result = (from i in Db.TB_MOVIMENTO_ITEMs
                          where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= Movimento.DataMovimento.Value
                          where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where i.TB_UGE_ID == (int)movItem.UGE.Id
                          where i.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == movItem.DataVencimentoLote || movItem.DataVencimentoLote == null
                          where i.TB_MOVIMENTO_ITEM_LOTE_FABR == movItem.FabricanteLote || movItem.FabricanteLote == null
                          where i.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote || movItem.IdentificacaoLote == null
                          where i.TB_MOVIMENTO_ITEM_ATIVO == true
                          select new MovimentoItemEntity
                          {
                              SaldoQtde = i.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                              DataMovimento = i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO
                          }).OrderByDescending(a => a.DataMovimento).FirstOrDefault();

            if (result == null)
                return false;
            else
            {
                if (result.SaldoQtde >= movItem.QtdeMov)
                    return true;
                else
                    return false;
            }
        }

        //Verifica se o movimento é retroativo
        public bool VerificarRetroativo(MovimentoItemEntity movItem)
        {
            //var result = (from i in Db.TB_MOVIMENTO_ITEMs
            var qryConsulta = (from i in Db.TB_MOVIMENTO_ITEMs
                               where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                               where ((i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date == Movimento.DataMovimento.Value.Date && i.TB_MOVIMENTO_ITEM_ID > movItem.Id) ||
                               (i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date))
                               where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                               where i.TB_UGE_ID == (int)movItem.UGE.Id
                               where i.TB_MOVIMENTO_ITEM_ATIVO == true
                               where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                               select i).Take(1);
            //select new MovimentoItemEntity
            //{
            //    Id = i.TB_MOVIMENTO_ITEM_ID
            //}).Count();

            var numRetroativos = qryConsulta.Count();

            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            if (numRetroativos > 0)
                return true;
            else
                return false;
        }

        public bool VerificarRetroativoNovoItem(MovimentoItemEntity movItem)
        {
            //var result = (from i in Db.TB_MOVIMENTO_ITEMs
            var qryConsulta = (from i in Db.TB_MOVIMENTO_ITEMs
                               where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                               where (i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date)
                               where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                               where i.TB_UGE_ID == (int)movItem.UGE.Id
                               where i.TB_MOVIMENTO_ITEM_ATIVO == true
                               where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                               select i).Take(1);
            //select new MovimentoItemEntity
            //{
            //    Id = i.TB_MOVIMENTO_ITEM_ID
            //}).Count();

            var numRetroativos = qryConsulta.Count();

            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            if (numRetroativos > 0)
                return true;
            else
                return false;
        }



        public bool VerificarEntrada(MovimentoItemEntity movItem)
        {
            //var result = (from i in Db.TB_MOVIMENTO_ITEMs
            var qryConsulta = (from i in Db.TB_MOVIMENTO_ITEMs
                               where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                               where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date
                               where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                               where i.TB_UGE_ID == (int)movItem.UGE.Id
                               where i.TB_MOVIMENTO_ITEM_ATIVO == true
                               where i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                               select i).Take(1);

            int numRetroativos = qryConsulta.Count();

            if (numRetroativos > 0)
                return true;
            else
            {
                return VerificarSaida(movItem);
            }
        }


        public bool VerificarSaida(MovimentoItemEntity movItem)
        {
            //var result = (from i in Db.TB_MOVIMENTO_ITEMs
            var qryConsulta = (from i in Db.TB_MOVIMENTO_ITEMs
                               where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                               where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date
                               where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                               where i.TB_UGE_ID == (int)movItem.UGE.Id
                               where i.TB_MOVIMENTO_ITEM_ATIVO == true
                               where i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia
                               select i).Take(1);

            int numRetroativos = qryConsulta.Count();

            if (numRetroativos > 0)
                return true;
            else
                return false;
        }

        //Verifica se o movimento é retroativo
        public bool VerificarSeEstoqueZerado(MovimentoItemEntity movItem)
        {
            //var result = (from i in Db.TB_MOVIMENTO_ITEMs
            var qryConsulta = (from i in Db.TB_MOVIMENTO_ITEMs
                               where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                               where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date > Movimento.DataMovimento.Value.Date
                               where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                               where i.TB_UGE_ID == (int)movItem.UGE.Id
                               where i.TB_MOVIMENTO_ITEM_ATIVO == true
                               // where i.TB_MOVIMENTO_ITEM_SALDO_QTDE.Value  == 0
                               where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                               orderby i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO descending, i.TB_MOVIMENTO_ID descending
                               select i).Take(1);

            var movRetorno = qryConsulta.FirstOrDefault();

            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            if (movRetorno != null)
            {
                if (movRetorno.TB_MOVIMENTO_ITEM_SALDO_QTDE == 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        //Verifica se o movimento é retroativo para estorno
        public bool VerificarRetroativoEstorno(MovimentoItemEntity movItem)
        {
            IQueryable<TB_MOVIMENTO_ITEM> result = (from i in Db.TB_MOVIMENTO_ITEMs
                                                    where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                                                    where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= Movimento.DataMovimento.Value
                                                    where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                                                    //where i.TB_MOVIMENTO.TB_UGE_ID == (int)Movimento.UGE.Id
                                                    where i.TB_UGE_ID == (int)movItem.UGE.Id
                                                    where i.TB_MOVIMENTO_ITEM_ATIVO == true
                                                    where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                    select i).Take(1);
            var retorno = result.Count();

            if (retorno > 0)
                return true;
            else
                return false;
        }

        public bool VerificarRetroativoEstornoBloqueio(MovimentoItemEntity movItem)
        {
            IQueryable<TB_MOVIMENTO_ITEM> result = (from i in Db.TB_MOVIMENTO_ITEMs
                                                    where i.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                                                    where i.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= Movimento.DataMovimento.Value
                                                    where i.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                                                    where i.TB_MOVIMENTO_ITEM_ID > (int)movItem.Id
                                                    //where i.TB_MOVIMENTO.TB_UGE_ID == (int)Movimento.UGE.Id
                                                    where i.TB_UGE_ID == (int)movItem.UGE.Id
                                                    where i.TB_MOVIMENTO_ITEM_ATIVO == true
                                                    where (i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                || i.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                    select i).Take(1);
            var retorno = result.Count();

            if (retorno > 0)
                return true;
            else
                return false;
        }




        /// <summary>
        /// *********************
        /// **Método Importante**
        /// Utilizado para retornar o saldo do movimento item anterior ao movimentoItem informado
        /// </summary>
        /// <param name="movItem">MovimentoItem posterior que irá retornar o saldo</param>
        /// <returns></returns>
        public MovimentoItemEntity ConsultaSaldoMovimentoItem(MovimentoItemEntity movItem)
        {
            var resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)movItem.Movimento.Almoxarifado.Id
                             where a.TB_UGE_ID == (int)movItem.UGE.Id
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 //SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                 SubItemMaterial = new SubItemMaterialEntity() { Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO },
                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                 QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                 ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                 Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                 UGE = new UGEEntity(a.TB_UGE_ID),
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

            //Adiciona na Quaryable o where para verificar se existem movimentos no mesmo dia com MovimentoItemId menor
            var resultadoMesmoDia = resultado.Where(a => a.Movimento.DataMovimento.Value.Date == movItem.Movimento.DataMovimento.Value.Date);
            resultadoMesmoDia = resultadoMesmoDia.Where(a => a.Id < movItem.Id);
            var resultadoMesmoDiaList = resultadoMesmoDia.FirstOrDefault();

            if (resultadoMesmoDiaList != null)
            {
                return resultadoMesmoDiaList;
            }
            else
            {
                //Adiciona na Quaryable o where para verificar se existem movimentos nos dias anteriores com MovimentoItemId diferente
                var resultadoDiaAnterior = resultado.Where(a => a.Movimento.DataMovimento.Value.Date < movItem.Movimento.DataMovimento.Value.Date);
                resultadoDiaAnterior = resultadoDiaAnterior.Where(a => a.Id != movItem.Id);
                var resultadoDiaAnteriorList = resultadoDiaAnterior.FirstOrDefault();

                if (resultadoDiaAnteriorList != null)
                {
                    return resultadoDiaAnteriorList;
                }
                else
                {
                    //Caso não tenha saldo anterior, retorna os dados do movimento com o saldo e qtd zerados
                    movItem.SaldoQtde = 0;
                    movItem.SaldoValor = 0;
                    movItem.Desd = 0;
                    movItem.PrecoUnit = 0;
                    movItem.SaldoQtdeLote = 0;

                    return movItem;
                }
            }

        }

        public MovimentoItemEntity ConsultaSaldoMovimentoItemLote(MovimentoItemEntity movItem)
        {
            var resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)movItem.Movimento.Almoxarifado.Id
                             where a.TB_UGE_ID == (int)movItem.UGE.Id
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 //SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                 SubItemMaterial = new SubItemMaterialEntity() { Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO },
                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                 QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                 ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                 Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                 DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                 UGE = new UGEEntity(a.TB_UGE_ID),
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

            //Adiciona na Quaryable o where para verificar se existem movimentos no mesmo dia com MovimentoItemId menor
            var resultadoMesmoDia = resultado.Where(a => a.Movimento.DataMovimento.Value.Date == movItem.Movimento.DataMovimento.Value.Date &&
                                                         a.IdentificacaoLote == movItem.IdentificacaoLote);


            if (movItem.DataVencimentoLote.HasValue)
            {
                resultadoMesmoDia = resultadoMesmoDia.Where(a => a.Id < movItem.Id && a.DataVencimentoLote == movItem.DataVencimentoLote.Value);
            }
            else
            {
                resultadoMesmoDia = resultadoMesmoDia.Where(a => a.Id < movItem.Id);
            }

            var resultadoMesmoDiaList = resultadoMesmoDia.FirstOrDefault();

            if (resultadoMesmoDiaList != null)
            {
                return resultadoMesmoDiaList;
            }
            else
            {
                //Adiciona na Quaryable o where para verificar se existem movimentos nos dias anteriores com MovimentoItemId diferente
                var resultadoDiaAnterior = resultado.Where(a => a.Movimento.DataMovimento.Value.Date < movItem.Movimento.DataMovimento.Value.Date &&
                                                                a.IdentificacaoLote == movItem.IdentificacaoLote);



                if (movItem.DataVencimentoLote.HasValue)
                {
                    resultadoDiaAnterior = resultadoDiaAnterior.Where(a => a.Id != movItem.Id && a.DataVencimentoLote == movItem.DataVencimentoLote.Value);
                }
                else
                {
                    resultadoDiaAnterior = resultadoDiaAnterior.Where(a => a.Id != movItem.Id);
                }


                var resultadoDiaAnteriorList = resultadoDiaAnterior.FirstOrDefault();

                if (resultadoDiaAnteriorList != null)
                {
                    return resultadoDiaAnteriorList;
                }
                else
                {
                    //Caso não tenha saldo anterior, retorna os dados do movimento com o saldo e qtd zerados
                    movItem.SaldoQtde = 0;
                    movItem.SaldoValor = 0;
                    movItem.Desd = 0;
                    movItem.PrecoUnit = 0;
                    movItem.SaldoQtdeLote = 0;

                    return movItem;
                }
            }

        }

        public decimal? SaldoMovimentoItemLote(int? idSubItem, int? idAlmoxarifado,DateTime dataMovimento, string lote, DateTime? dataLoteVenc, int? ugeId)
        {
            IQueryable<MovimentoItemEntity> resultado = null;

            if (dataLoteVenc != null)
            {
                resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == idSubItem
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == idAlmoxarifado
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             where a.TB_MOVIMENTO_ITEM_LOTE_IDENT == lote
                             where a.TB_UGE_ID == ugeId
                             where a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == dataLoteVenc
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
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
            }
            else
            {
                resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == idSubItem
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == idAlmoxarifado
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             where a.TB_MOVIMENTO_ITEM_LOTE_IDENT == lote
                             where a.TB_UGE_ID == ugeId
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
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

            }

            var resultadoFiltro = resultado.Where(a => a.Movimento.DataMovimento.Value.Date <= dataMovimento).OrderByDescending(a => a.Id).OrderByDescending(a => a.Movimento.DataMovimento);

            var retorno = resultadoFiltro.FirstOrDefault();
            if (retorno != null)
                return retorno.SaldoQtdeLote;
            else
                return 0;


        }

        /// <summary>
        /// Retorna o Movimento e os itens pelo ID ou documento
        /// </summary>
        /// <returns></returns>
        public MovimentoEntity GetMovimento()
        {
            if (this.Movimento.Id == null)
                throw new Exception("Fornecer o id do documento na consulta de movimentos");

            IQueryable<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                     orderby a.TB_MOVIMENTO_ID
                                                     where a.TB_MOVIMENTO_ID == this.Movimento.Id
                                                     select new MovimentoEntity
                                                     {
                                                         Id = a.TB_MOVIMENTO_ID,
                                                         Almoxarifado = (from almox in Db.TB_ALMOXARIFADOs
                                                                         where almox.TB_ALMOXARIFADO_ID == a.TB_ALMOXARIFADO_ID
                                                                         select new AlmoxarifadoEntity
                                                                         {
                                                                             Id = almox.TB_ALMOXARIFADO_ID,
                                                                             Codigo = almox.TB_ALMOXARIFADO_CODIGO,
                                                                             Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                                                                             MesRef = almox.TB_ALMOXARIFADO_MES_REF,
                                                                             Gestor = new GestorEntity() { Id = almox.TB_GESTOR.TB_GESTOR_ID, Nome = almox.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = almox.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = almox.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }
                                                                         }).FirstOrDefault(),
                                                         MovimAlmoxOrigemDestino = (from almox in Db.TB_ALMOXARIFADOs
                                                                                    where almox.TB_ALMOXARIFADO_ID == a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO
                                                                                    select new AlmoxarifadoEntity
                                                                                    {
                                                                                        Id = almox.TB_ALMOXARIFADO_ID,
                                                                                        Codigo = almox.TB_ALMOXARIFADO_CODIGO,
                                                                                        Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                                                                                    }).FirstOrDefault(),
                                                         IdTransferencia = a.TB_MOVIMENTO_TRANSFERENCIA_ID,
                                                         TipoMovimento = (from tipo in Db.TB_TIPO_MOVIMENTOs
                                                                          where tipo.TB_TIPO_MOVIMENTO_ID == a.TB_TIPO_MOVIMENTO_ID
                                                                          select new TipoMovimentoEntity
                                                                          {
                                                                              Id = tipo.TB_TIPO_MOVIMENTO_ID,
                                                                              AgrupamentoId = tipo.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                              Descricao = tipo.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                              SubTipoMovimentoItem = (
                                                                                      from subtipo in Db.TB_SUBTIPO_MOVIMENTOs
                                                                                      where subtipo.TB_SUBTIPO_MOVIMENTO_ID == a.TB_SUBTIPO_MOVIMENTO_ID
                                                                                      select new SubTipoMovimentoEntity
                                                                                      {
                                                                                          Id = subtipo.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                          Descricao = subtipo.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                      }).FirstOrDefault()

                                                                          }).FirstOrDefault(),
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
                                                         EmpenhoLicitacao = new EmpenhoLicitacaoEntity(a.TB_EMPENHO_LICITACAO_ID),
                                                         EmpenhoEvento = new EmpenhoEventoEntity(a.TB_EMPENHO_EVENTO_ID),
                                                         SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,

                                                         UGE = new UGEEntity()
                                                         {
                                                             Id = a.TB_UGE.TB_UGE_ID,
                                                             Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                             Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                                         },
                                                         Divisao = (from d in Db.TB_DIVISAOs
                                                                    where a.TB_DIVISAO_ID == d.TB_DIVISAO_ID
                                                                    select new DivisaoEntity
                                                                    {
                                                                        Id = d.TB_DIVISAO_ID,
                                                                        Codigo = d.TB_DIVISAO_CODIGO,
                                                                        Descricao = d.TB_DIVISAO_DESCRICAO,
                                                                        EnderecoTelefone = d.TB_DIVISAO_TELEFONE,
                                                                        EnderecoLogradouro = d.TB_DIVISAO_LOGRADOURO,
                                                                        EnderecoCep = d.TB_DIVISAO_CEP,
                                                                        EnderecoCompl = d.TB_DIVISAO_COMPLEMENTO,
                                                                        EnderecoFax = d.TB_DIVISAO_FAX,
                                                                        EnderecoNumero = d.TB_DIVISAO_NUMERO,
                                                                        Responsavel = (from r in Db.TB_RESPONSAVELs
                                                                                       where r.TB_RESPONSAVEL_ID == d.TB_RESPONSAVEL_ID
                                                                                       select new ResponsavelEntity
                                                                                       {
                                                                                           Descricao = r.TB_RESPONSAVEL_NOME
                                                                                       }).FirstOrDefault(),
                                                                        Ua = (from u in Db.TB_UAs
                                                                              where d.TB_UA_ID == u.TB_UA_ID
                                                                              select new UAEntity
                                                                              {
                                                                                  Id = u.TB_UA_ID,
                                                                                  Codigo = u.TB_UA_CODIGO,
                                                                                  Descricao = u.TB_UA_DESCRICAO,
                                                                                  Uge = (from uge in Db.TB_UGEs
                                                                                         where u.TB_UGE_ID == uge.TB_UGE_ID
                                                                                         select new UGEEntity
                                                                                         {
                                                                                             Id = uge.TB_UGE_ID,
                                                                                             Codigo = uge.TB_UGE_CODIGO,
                                                                                             Descricao = uge.TB_UGE_DESCRICAO
                                                                                         }).FirstOrDefault(),
                                                                              }).FirstOrDefault(),
                                                                    }).FirstOrDefault(),
                                                         Fornecedor = new FornecedorEntity
                                                         {
                                                             Id = a.TB_FORNECEDOR_ID,
                                                             Nome = a.TB_FORNECEDOR.TB_FORNECEDOR_NOME,
                                                         },
                                                         Ativo = a.TB_MOVIMENTO_ATIVO,
                                                         IdLogin = a.TB_LOGIN_ID,
                                                         IdLoginEstorno = a.TB_LOGIN_ID_ESTORNO,
                                                         DataOperacao = a.TB_MOVIMENTO_DATA_OPERACAO,

                                                         MovimentoItem = (from movItem in Db.TB_MOVIMENTO_ITEMs
                                                                          where a.TB_MOVIMENTO_ID == movItem.TB_MOVIMENTO_ID
                                                                          select new MovimentoItemEntity
                                                                          {
                                                                              Id = movItem.TB_MOVIMENTO_ITEM_ID,
                                                                              Ativo = movItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                                              DataVencimentoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                                              Desd = movItem.TB_MOVIMENTO_ITEM_DESD,
                                                                              FabricanteLote = movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                                              IdentificacaoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                                              PrecoUnit = (movItem.TB_MOVIMENTO_ITEM_VALOR_MOV / movItem.TB_MOVIMENTO_ITEM_QTDE_MOV),
                                                                              PrecoUnitSiafem = movItem.TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP,
                                                                              PrecoMedio = movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT, //Preço médio do estoque
                                                                              QtdeLiq = movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                                              QtdeMov = movItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                                              SaldoQtde = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                                              SaldoQtdeLote = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                                              SaldoValor = movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                                              NL_Liquidacao = movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                                                                              NL_Reclassificacao = movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO,
                                                                              NL_Consumo = movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO,
                                                                              SubItemMaterial = new SubItemMaterialEntity
                                                                              {
                                                                                  Id = movItem.TB_SUBITEM_MATERIAL_ID,
                                                                                  Codigo = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                  Descricao = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                                  UnidadeFornecimento = (from unid in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                                         where unid.TB_UNIDADE_FORNECIMENTO_ID == movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO_ID
                                                                                                         select new UnidadeFornecimentoEntity
                                                                                                         {
                                                                                                             Id = unid.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                                             Codigo = unid.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                                             Descricao = unid.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                                         }).FirstOrDefault(),
                                                                                  CodigoFormatado = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(8),
                                                                                  ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                                  where ItemMaterial.TB_ITEM_MATERIAL_ID == movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                                                  select new ItemMaterialEntity()
                                                                                                  {
                                                                                                      Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                                      Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                                      CodigoFormatado = ItemMaterial.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(4),
                                                                                                      Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO
                                                                                                  }).FirstOrDefault(),
                                                                                  NaturezaDespesa = new NaturezaDespesaEntity
                                                                                  {
                                                                                      Id = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA_ID,
                                                                                      Codigo = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                      Descricao = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                                                  },

                                                                              },
                                                                              ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                              where ItemMaterial.TB_ITEM_MATERIAL_ID == movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                                              select new ItemMaterialEntity()
                                                                                              {
                                                                                                  Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                                  Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                                  CodigoFormatado = ItemMaterial.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(4),
                                                                                                  Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO
                                                                                              }).FirstOrDefault(),
                                                                              UGE = (from uge in this.Db.TB_UGEs
                                                                                     where uge.TB_UGE_ID == movItem.TB_UGE_ID
                                                                                     select new UGEEntity()
                                                                                     {
                                                                                         Id = uge.TB_UGE_ID,
                                                                                         Codigo = uge.TB_UGE_CODIGO,
                                                                                         Descricao = uge.TB_UGE_DESCRICAO,
                                                                                     }).FirstOrDefault(),
                                                                              ValorMov = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                                              Movimento = new MovimentoEntity
                                                                              {
                                                                                  Id = a.TB_MOVIMENTO_ID,
                                                                                  DataMovimento = a.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                                                  NumeroDocumento = a.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                                  Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                                                  AnoMesReferencia = a.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                                                  TipoMovimento = (from tipo in Db.TB_TIPO_MOVIMENTOs
                                                                                                   where tipo.TB_TIPO_MOVIMENTO_ID == a.TB_TIPO_MOVIMENTO_ID
                                                                                                   select new TipoMovimentoEntity
                                                                                                   {
                                                                                                       Id = tipo.TB_TIPO_MOVIMENTO_ID,
                                                                                                       AgrupamentoId = tipo.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                                                                       Descricao = tipo.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                                                                       SubTipoMovimentoItem = (
                                                                                                               from subtipo in Db.TB_SUBTIPO_MOVIMENTOs
                                                                                                               where subtipo.TB_SUBTIPO_MOVIMENTO_ID == a.TB_SUBTIPO_MOVIMENTO_ID
                                                                                                               select new SubTipoMovimentoEntity
                                                                                                               {
                                                                                                                   Id = subtipo.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                                                   Descricao = subtipo.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                                               }).FirstOrDefault()

                                                                                                   }).FirstOrDefault(),
                                                                              },
                                                                              PTRes = new PTResEntity()
                                                                              {
                                                                                  //Id = movItem.TB_PTRE.TB_PTRES_ID,
                                                                                  //Codigo = movItem.TB_PTRE.TB_PTRES_CODIGO,
                                                                                  Codigo = movItem.TB_PTRES_CODIGO,
                                                                                  //artificio para carga do campo PTRes_Acao em tela, para ConsumoImediato
                                                                                  ProgramaTrabalho = new ProgramaTrabalho("0000000000" + movItem.TB_PTRES_PT_ACAO + "0000")
                                                                              }
                                                                          }).ToList<MovimentoItemEntity>(),
                                                         InscricaoCE = a.TB_MOVIMENTO_CE,
                                                         UA = new UAEntity() { Id = a.TB_UA.TB_UA_ID, Codigo = a.TB_UA.TB_UA_CODIGO, Uge = new UGEEntity() { Id = a.TB_UA.TB_UGE.TB_UGE_ID, Codigo = a.TB_UA.TB_UGE.TB_UGE_CODIGO } }
                                                         //UA = ( ((a.TB_UA_ID != null && a.TB_DIVISAO_ID == null) ? (new UAEntity() { Id = a.TB_UA.TB_UA_ID, Codigo = a.TB_UA.TB_UA_CODIGO, Uge = new UGEEntity() { Id = a.TB_UA.TB_UGE.TB_UGE_ID, Codigo = a.TB_UA.TB_UGE.TB_UGE_CODIGO } }) : ((a.TB_UA_ID == null && a.TB_DIVISAO_ID != null) ? (new UAEntity() { Id = a.TB_DIVISAO.TB_UA.TB_UA_ID, Codigo = a.TB_DIVISAO.TB_UA.TB_UA_CODIGO, Uge = new UGEEntity() { Id = a.TB_DIVISAO.TB_UA.TB_UGE.TB_UGE_ID, Codigo = a.TB_DIVISAO.TB_UA.TB_UGE.TB_UGE_CODIGO } }) : null)))
                                                     }).AsQueryable();




            var retorno = resultado.FirstOrDefault();

            return retorno;

        }

        /// <summary>
        /// Retorna o Movimento e os itens pelo ID ou documento
        /// </summary>
        /// <returns></returns>
        public MovimentoEntity GetMovimentacaoConsumoImediato(int movimentacaoMaterialID)
        {
            if (movimentacaoMaterialID <= 0)
                throw new Exception("Fornecer o id do documento na consulta de movimentos");


            Expression<Func<TB_MOVIMENTO, bool>> expClausulaWhere = null;
            Func<TB_MOVIMENTO, MovimentoEntity> fMaterializadora = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            MovimentoEntity registroConsumoImediato = null;


            fMaterializadora = _instanciadorDTOMovimentacoesConsumoImediato();

            qryConsulta = Db.TB_MOVIMENTOs.AsQueryable();
            expClausulaWhere = (movimentacaoMaterial => movimentacaoMaterial.TB_MOVIMENTO_ID == movimentacaoMaterialID);

            registroConsumoImediato = qryConsulta.Where(expClausulaWhere)
                                                 .Select(fMaterializadora)
                                                 .FirstOrDefault();

            return registroConsumoImediato;
        }

        /// <summary>
        /// Retorna o Movimento e os itens pelo ID ou documento
        /// </summary>
        /// <returns></returns>
        public List<MovimentoEntity> GetMovimentos()
        {

            Expression<Func<MovimentoEntity, bool>> where;

            if (!string.IsNullOrEmpty(this.Movimento.NumeroDocumento))
            {
                where = a => a.NumeroDocumento == this.Movimento.NumeroDocumento;
            }
            else if (this.Movimento.Id != null)
            {
                where = a => a.Id == this.Movimento.Id;
            }
            else
            {
                return new List<MovimentoEntity>();
            }


            List<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
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
                                                   IdLogin = a.TB_LOGIN_ID,
                                                   EmpenhoLicitacao = new EmpenhoLicitacaoEntity(a.TB_EMPENHO_LICITACAO_ID),
                                                   EmpenhoEvento = new EmpenhoEventoEntity(a.TB_EMPENHO_EVENTO_ID),
                                                   MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                   UGE = new UGEEntity(a.TB_UGE_ID),
                                                   Divisao = (from d in Db.TB_DIVISAOs
                                                              where a.TB_DIVISAO_ID == d.TB_DIVISAO_ID
                                                              select new DivisaoEntity
                                                              {
                                                                  Id = d.TB_DIVISAO_ID,
                                                                  Codigo = d.TB_DIVISAO_CODIGO,
                                                                  Descricao = d.TB_DIVISAO_DESCRICAO,
                                                                  EnderecoTelefone = d.TB_DIVISAO_TELEFONE,
                                                                  EnderecoLogradouro = d.TB_DIVISAO_LOGRADOURO,
                                                                  EnderecoCep = d.TB_DIVISAO_CEP,
                                                                  EnderecoCompl = d.TB_DIVISAO_COMPLEMENTO,
                                                                  EnderecoFax = d.TB_DIVISAO_FAX,
                                                                  EnderecoNumero = d.TB_DIVISAO_NUMERO,
                                                                  Responsavel = (from r in Db.TB_RESPONSAVELs
                                                                                 where r.TB_RESPONSAVEL_ID == d.TB_RESPONSAVEL_ID
                                                                                 select new ResponsavelEntity
                                                                                 {
                                                                                     Descricao = r.TB_RESPONSAVEL_NOME
                                                                                 }).FirstOrDefault(),
                                                                  Ua = (from u in Db.TB_UAs
                                                                        where d.TB_UA_ID == u.TB_UA_ID
                                                                        select new UAEntity
                                                                        {
                                                                            Id = u.TB_UA_ID,
                                                                            Codigo = u.TB_UA_CODIGO,
                                                                            Descricao = u.TB_UA_DESCRICAO
                                                                        }).FirstOrDefault(),
                                                              }).FirstOrDefault(),
                                                   Fornecedor = new FornecedorEntity
                                                   {
                                                       Id = a.TB_FORNECEDOR_ID,
                                                       Nome = a.TB_FORNECEDOR.TB_FORNECEDOR_NOME,
                                                   },
                                                   Ativo = a.TB_MOVIMENTO_ATIVO,
                                                   MovimentoItem = (from movItem in Db.TB_MOVIMENTO_ITEMs
                                                                    where a.TB_MOVIMENTO_ID == movItem.TB_MOVIMENTO_ID
                                                                    select new MovimentoItemEntity
                                                                    {
                                                                        Id = movItem.TB_MOVIMENTO_ITEM_ID,
                                                                        Ativo = movItem.TB_MOVIMENTO_ITEM_ATIVO,
                                                                        DataVencimentoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                                        Desd = movItem.TB_MOVIMENTO_ITEM_DESD,
                                                                        FabricanteLote = movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                                        IdentificacaoLote = movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                                        PrecoUnit = movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                                        QtdeLiq = movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                                        QtdeMov = movItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                                        SaldoQtde = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                                        SaldoQtdeLote = movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                                        SaldoValor = movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                                        SubItemMaterial = new SubItemMaterialEntity
                                                                        {
                                                                            Id = movItem.TB_SUBITEM_MATERIAL_ID,
                                                                            Codigo = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                            Descricao = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                            UnidadeFornecimento = (from unid in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                                   where unid.TB_UNIDADE_FORNECIMENTO_ID == movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO_ID
                                                                                                   select new UnidadeFornecimentoEntity
                                                                                                   {
                                                                                                       Id = unid.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                                       Codigo = unid.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                                       Descricao = unid.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                                   }).FirstOrDefault(),
                                                                            CodigoFormatado = movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(8),
                                                                            ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                            where ItemMaterial.TB_ITEM_MATERIAL_ID == movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                                            select new ItemMaterialEntity()
                                                                                            {
                                                                                                Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                                Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                                CodigoFormatado = ItemMaterial.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(4),
                                                                                                Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO
                                                                                            }).FirstOrDefault(),
                                                                            NaturezaDespesa = new NaturezaDespesaEntity
                                                                            {
                                                                                Id = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA_ID,
                                                                                Codigo = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                Descricao = movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                                            },

                                                                        },
                                                                        ItemMaterial = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                                                                                        where ItemMaterial.TB_ITEM_MATERIAL_ID == movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID
                                                                                        select new ItemMaterialEntity()
                                                                                        {
                                                                                            Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                                                            Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                                                            CodigoFormatado = ItemMaterial.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(4),
                                                                                            Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO
                                                                                        }).FirstOrDefault(),
                                                                        UGE = new UGEEntity(movItem.TB_UGE_ID),
                                                                        ValorMov = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                                        Movimento = new MovimentoEntity
                                                                        {
                                                                            Id = movItem.TB_MOVIMENTO_ID
                                                                        },
                                                                        PTRes = (from PTRes in this.Db.TB_PTREs
                                                                                 where PTRes.TB_PTRES_ID == movItem.TB_PTRES_ID
                                                                                 select new PTResEntity()
                                                                                 {
                                                                                     Id = PTRes.TB_PTRES_ID,
                                                                                     Codigo = PTRes.TB_PTRES_CODIGO,
                                                                                     Descricao = PTRes.TB_PTRES_DESCRICAO,
                                                                                     CodigoGestao = PTRes.TB_PTRES_CODIGO_GESTAO,
                                                                                     CodigoPT = PTRes.TB_PTRES_PT_CODIGO,
                                                                                     CodigoUGE = PTRes.TB_PTRES_UGE_CODIGO,
                                                                                     AnoDotacao = PTRes.TB_PTRES_ANO,
                                                                                     CodigoUO = PTRes.TB_PTRES_UO_CODIGO,
                                                                                     ProgramaTrabalho = new ProgramaTrabalho(PTRes.TB_PTRES_PT_CODIGO)
                                                                                 }).FirstOrDefault(),
                                                                    }).ToList<MovimentoItemEntity>(),
                                                   InscricaoCE = a.TB_MOVIMENTO_CE
                                               }).Where(where).ToList();



            return resultado;

        }

        public List<SaldoSubItemEntity> GetSaldo(MovimentoItemEntity movItem)
        {
            var result = (from saldo in Db.TB_SALDO_SUBITEMs
                          where saldo.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                          where saldo.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                          where saldo.TB_UGE_ID == (int)Movimento.UGE.Id
                          select new SaldoSubItemEntity
                          {
                              PrecoUnit = saldo.TB_SALDO_SUBITEM_PRECO_UNIT,
                              SaldoQtde = saldo.TB_SALDO_SUBITEM_SALDO_QTDE,
                              SaldoValor = saldo.TB_SALDO_SUBITEM_SALDO_VALOR,

                          }).ToList();

            return result;
        }

        public Decimal GetSaldoQuantidade(MovimentoItemEntity movItem)
        {
            IQueryable<Decimal> query = (from saldo in Db.TB_SALDO_SUBITEMs
                                         where saldo.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                                         where saldo.TB_ALMOXARIFADO_ID == (int)Movimento.Almoxarifado.Id
                                         where saldo.TB_UGE_ID == (int)Movimento.UGE.Id
                                         select saldo.TB_SALDO_SUBITEM_SALDO_QTDE.Value).AsQueryable();
            return query.FirstOrDefault();
        }

        public decimal? GetValorDocumento(int movimentoId)
        {
            MovimentoEntity resultado = (from a in this.Db.TB_MOVIMENTOs
                                         where a.TB_MOVIMENTO_ID == movimentoId
                                         select new MovimentoEntity
                                         {
                                             ValorDocumento = a.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                         }).FirstOrDefault();

            return resultado.ValorDocumento;
        }

        //Desativa o movimento e os itens ao ser estornado
        public void AtualizarMovimentoEstorno(int loginIdEstornante, string InscricaoCE)
        {
            var movimentacaoMaterialID = this.Entity.Id.Value;

            DesativarMovimentoItemParaEstorno(loginIdEstornante, movimentacaoMaterialID);
            DesativarMovimentoParaEstorno(loginIdEstornante, movimentacaoMaterialID, InscricaoCE);

            //TODO MELHORIA INFRA - DESATIVADO
            this.GravarMovimentoHistorico(this.Entity);

            Db.SubmitChanges();
        }

        public void DesativarMovimentoParaEstorno(int loginIdEstornante, int movimentacaoMaterialID, string InscricaoCE)
        {
            TB_MOVIMENTO rowTabelaMovimentacao = null;
            TB_MOVIMENTO rowTabelaMovimentacaoRequisicao = null;
            int tipoMovRequisicaoFinalizada = -1;
            int tipoMovRequisicaoPendente = -1;
            int tipoMovRequisicaoAprovada = -1;



            tipoMovRequisicaoFinalizada = (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada;
            tipoMovRequisicaoPendente = (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente;
            tipoMovRequisicaoAprovada = (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada;
            rowTabelaMovimentacao = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == movimentacaoMaterialID).FirstOrDefault();
            rowTabelaMovimentacaoRequisicao = this.Db.TB_MOVIMENTOs.Where(MovRequisicao => MovRequisicao.TB_MOVIMENTO_NUMERO_DOCUMENTO == rowTabelaMovimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO
                                                                                        && MovRequisicao.TB_ALMOXARIFADO_ID == rowTabelaMovimentacao.TB_ALMOXARIFADO_ID
                                                                                        && MovRequisicao.TB_TIPO_MOVIMENTO_ID == tipoMovRequisicaoFinalizada).FirstOrDefault();
            if (rowTabelaMovimentacao.IsNotNull())
            {
                rowTabelaMovimentacao.TB_LOGIN_ID_ESTORNO = loginIdEstornante;

                //Desativa o Movimento Estornado
                rowTabelaMovimentacao.TB_MOVIMENTO_ATIVO = false;

                if ((rowTabelaMovimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovRequisicaoAprovada) && rowTabelaMovimentacaoRequisicao.IsNotNull())
                    //reativa requisição - Define status inicial da requisição "pendente"
                    rowTabelaMovimentacaoRequisicao.TB_TIPO_MOVIMENTO_ID = tipoMovRequisicaoPendente;

                //Atualiza a data da operação para a data atual
                rowTabelaMovimentacao.TB_MOVIMENTO_DATA_ESTORNO = DateTime.Now;
                if (!string.IsNullOrEmpty(InscricaoCE))
                    rowTabelaMovimentacao.TB_MOVIMENTO_CE = InscricaoCE;

                Db.SubmitChanges();
            }
        }

        public void DesativarMovimentoItemParaEstorno(int loginIdEstornante, int movimentacaoMaterialID)
        {
            if ((movimentacaoMaterialID == 0) || (loginIdEstornante == 0))
                throw new ArgumentException("ID informado inválido.");

            TB_MOVIMENTO rowTabelaMovimentacao = null;
            EntitySet<TB_MOVIMENTO_ITEM> rowsTabelaItemMovimentacao = null;
            IQueryable<TB_MOVIMENTO_ITEM> qryItensMovimentacaoEntrada = null;
            IQueryable<TB_MOVIMENTO_ITEM> qryItensMovimentacaoSaida = null;
            IList<TB_MOVIMENTO_ITEM> lstItensMovimentacaoEntradaComLote = null;
            IList<TB_MOVIMENTO_ITEM> lstItensMovimentacaoSaidaComLote = null;
            IList<TB_MOVIMENTO_ITEM> lstItensMovimentacaoEntrada = null;
            IList<TB_MOVIMENTO_ITEM> lstItensMovimentacaoSaida = null;
            IQueryable<TB_SALDO_SUBITEM> lstSaldoSubItem = null;
            int numeroMovimentacoesEntrada = 0;
            int numeroMovimentacoesSaida = 0;
            int tipoAgrupamentoMovimentacoesSaida = -1;
            int tipoAgrupamentoMovimentacoesEntrada = -1;


            tipoAgrupamentoMovimentacoesSaida = (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida;
            tipoAgrupamentoMovimentacoesEntrada = (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada;
            rowTabelaMovimentacao = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == movimentacaoMaterialID).FirstOrDefault();
            rowsTabelaItemMovimentacao = this.Db.TB_MOVIMENTO_ITEMs.Where(movItem => movItem.TB_MOVIMENTO_ID == movimentacaoMaterialID).ToEntitySet();

            if (rowTabelaMovimentacao.IsNotNull())
            {
                // atualiza os itens para ativo = false
                //foreach (TB_MOVIMENTO_ITEM movItem in rowTabelaMovimentacao.TB_MOVIMENTO_ITEMs)
                foreach (TB_MOVIMENTO_ITEM rowItemMovimentacao in rowsTabelaItemMovimentacao)
                {
                    //IQueryable<TB_MOVIMENTO_ITEM> query = (from a in this.Db.TB_MOVIMENTO_ITEMs
                    qryItensMovimentacaoEntrada = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                   where a.TB_SUBITEM_MATERIAL_ID == (int)rowItemMovimentacao.TB_SUBITEM_MATERIAL_ID
                                                   where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)rowTabelaMovimentacao.TB_ALMOXARIFADO_ID
                                                   where a.TB_UGE_ID == (int)rowTabelaMovimentacao.TB_UGE_ID
                                                   where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                   //where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                                   //where a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= this.Entity.DataMovimento
                                                   where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoAgrupamentoMovimentacoesEntrada
                                                   where a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= rowTabelaMovimentacao.TB_MOVIMENTO_DATA_MOVIMENTO
                                                   orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO ascending, a.TB_MOVIMENTO_ID ascending
                                                   select a).AsQueryable();



                    lstItensMovimentacaoEntrada = qryItensMovimentacaoEntrada.ToList();
                    numeroMovimentacoesEntrada = qryItensMovimentacaoEntrada.Count();

                    //IQueryable<TB_MOVIMENTO_ITEM> querySaida = (from a in this.Db.TB_MOVIMENTO_ITEMs
                    qryItensMovimentacaoSaida = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                 where a.TB_SUBITEM_MATERIAL_ID == (int)rowItemMovimentacao.TB_SUBITEM_MATERIAL_ID
                                                 where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)rowTabelaMovimentacao.TB_ALMOXARIFADO_ID
                                                 where a.TB_UGE_ID == (int)rowTabelaMovimentacao.TB_UGE_ID
                                                 where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                 where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                 //where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida
                                                 where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoAgrupamentoMovimentacoesSaida
                                                 select a).AsQueryable();

                    //IList<TB_MOVIMENTO_ITEM> listaSaida = querySaida.ToList();
                    //Int32 countSaida = querySaida.Count();
                    lstItensMovimentacaoSaida = qryItensMovimentacaoSaida.ToList();
                    numeroMovimentacoesSaida = qryItensMovimentacaoSaida.Count();

                    lstSaldoSubItem = (from s in this.Db.TB_SALDO_SUBITEMs
                                       where s.TB_SUBITEM_MATERIAL_ID == (int)rowItemMovimentacao.TB_SUBITEM_MATERIAL_ID &&
                                       s.TB_ALMOXARIFADO_ID == (int)rowTabelaMovimentacao.TB_ALMOXARIFADO_ID
                                       select s);
                    var saldoTotal = lstSaldoSubItem.Sum(a => a.TB_SALDO_SUBITEM_SALDO_QTDE);
                    //if (lista.Count > 0)
                    if (lstItensMovimentacaoEntrada.Count > 0)
                    {
                        if (numeroMovimentacoesEntrada > 1)
                        {
                            //if (lista[0].TB_MOVIMENTO_ID == this.Entity.Id && countSaida > 0)
                            if (lstItensMovimentacaoEntrada[0].TB_MOVIMENTO_ID == movimentacaoMaterialID && numeroMovimentacoesSaida > 0)
                                throw new Exception("Não é Permitido Estornar o Primeiro Movimento de Entrada!");
                        }
                        else
                        {
                            //IList<TB_MOVIMENTO_ITEM> mquery = (from q in lista where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC != null select q).ToList();
                            //IList<TB_MOVIMENTO_ITEM> mqueryS = (from q in listaSaida where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC != null select q).ToList();
                            lstItensMovimentacaoEntradaComLote = (from q in lstItensMovimentacaoEntrada where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC != null select q).ToList();
                            lstItensMovimentacaoSaidaComLote = (from q in lstItensMovimentacaoSaida where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC != null select q).ToList();

                            //if (mquery != null && mquery.Count > 0 && mquery[0].TB_MOVIMENTO_ID == this.Entity.Id && mqueryS.Count > 0)
                            if (lstItensMovimentacaoEntradaComLote.HasElements() && lstItensMovimentacaoEntradaComLote[0].TB_MOVIMENTO_ID == movimentacaoMaterialID && lstItensMovimentacaoSaidaComLote.Count > 0)
                                throw new Exception("Não é Permitido Estornar o Primeiro Movimento de Entrada!");

                            //mquery = (from q in lista where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null select q).ToList();
                            //mqueryS = (from q in listaSaida where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null select q).ToList();
                            lstItensMovimentacaoEntradaComLote = (from q in lstItensMovimentacaoEntrada where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null select q).ToList();
                            lstItensMovimentacaoSaidaComLote = (from q in lstItensMovimentacaoSaida where q.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null select q).ToList();

                            //if (mquery != null && mquery.Count > 0 && mquery[0].TB_MOVIMENTO_ID == this.Entity.Id && mqueryS.Count > 0)
                            if (lstItensMovimentacaoEntradaComLote.HasElements() && lstItensMovimentacaoEntradaComLote[0].TB_MOVIMENTO_ID == movimentacaoMaterialID && lstItensMovimentacaoSaidaComLote.Count > 0)
                            {
                                var subItemCodigo = rowItemMovimentacao.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO;
                                var subItemDescricao = rowItemMovimentacao.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO;
                                var saldoAtual = rowItemMovimentacao.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                                //throw new Exception("Não é Permitido Estornar o Primeiro Movimento de Entrada!");                            
                                throw new Exception((String.Format("Quantidade Indisponível para o SubItem {0}, Quantidade disponível: {1}, Quantidade da saída: {2} verifique a quantidade disponível no Relatório Analítico para o(s) SubItem(ns) !"
                                    , subItemCodigo, saldoTotal, saldoAtual)));
                            }
                        }
                    }

                    rowItemMovimentacao.TB_MOVIMENTO_ITEM_ATIVO = false;
                    rowItemMovimentacao.TB_LOGIN_ID_ESTORNO = loginIdEstornante;
                    rowItemMovimentacao.TB_MOVIMENTO_ITEM_DATA_ESTORNO = DateTime.Now;
                }

                try
                {
                    Db.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException excConflitoUpdate)
                {
                    foreach (ObjectChangeConflict rowComUpdateDivergente in Db.ChangeConflicts)
                        foreach (MemberChangeConflict colunaComValorDivergente in rowComUpdateDivergente.MemberConflicts)
                            if ((colunaComValorDivergente.Member.DeclaringType.Name.ToUpperInvariant() == "TB_MOVIMENTO_ITEM") && (colunaComValorDivergente.Member.Name.ToUpperInvariant() == "TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO"))
                                if (String.IsNullOrWhiteSpace((string)colunaComValorDivergente.CurrentValue) && !String.IsNullOrWhiteSpace((string)colunaComValorDivergente.DatabaseValue))
                                    colunaComValorDivergente.Resolve(RefreshMode.OverwriteCurrentValues);


                    Db.SubmitChanges();
                }

            }
        }

        public void AtualizarMovimentoLoteEstorno(int loginIdEstornante)
        {
            TB_MOVIMENTO mov = new TB_MOVIMENTO();
            EntitySet<TB_MOVIMENTO_ITEM> item = new EntitySet<TB_MOVIMENTO_ITEM>();
            TB_MOVIMENTO req = new TB_MOVIMENTO();

            mov = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            mov.TB_LOGIN_ID_ESTORNO = loginIdEstornante;
            req = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.TB_MOVIMENTO_NUMERO_DOCUMENTO
                && a.TB_ALMOXARIFADO_ID == mov.TB_ALMOXARIFADO_ID &&
                a.TB_TIPO_MOVIMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada).FirstOrDefault();

            // atualiza os itens para ativo = false
            foreach (TB_MOVIMENTO_ITEM movItem in mov.TB_MOVIMENTO_ITEMs)
            {
                IQueryable<TB_MOVIMENTO_ITEM> query = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                       where a.TB_SUBITEM_MATERIAL_ID == (int)movItem.TB_SUBITEM_MATERIAL_ID
                                                       where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)mov.TB_ALMOXARIFADO_ID
                                                       where a.TB_UGE_ID == (int)mov.TB_UGE_ID
                                                       where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                       where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                                       where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                                                       // where a.TB_MOVIMENTO.TB_MOVIMENTO_ID < this.Entity.Id.Value
                                                       where a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO <= this.Entity.DataMovimento
                                                       where a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue
                                                       orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO ascending, a.TB_MOVIMENTO_ID ascending
                                                       select a
                          ).AsQueryable();

                IList<TB_MOVIMENTO_ITEM> lista = query.ToList();

                if (lista.Count > 0)
                {
                    query = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == (int)movItem.TB_SUBITEM_MATERIAL_ID
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)mov.TB_ALMOXARIFADO_ID
                             where a.TB_UGE_ID == (int)mov.TB_UGE_ID
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             where a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null
                             where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada

                             // where a.TB_MOVIMENTO.TB_MOVIMENTO_ID < this.Entity.Id.Value
                             select a
                         ).AsQueryable();

                    Int32 countEntrada = query.Count();

                    if (countEntrada > 1)
                        if (lista[0].TB_MOVIMENTO_ID == this.Entity.Id)
                            throw new Exception("Não é Permetido Estorna o Primeiro Movimento de Entrada!");
                        else

                            query = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                     where a.TB_SUBITEM_MATERIAL_ID == (int)movItem.TB_SUBITEM_MATERIAL_ID
                                     where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == (int)mov.TB_ALMOXARIFADO_ID
                                     where a.TB_UGE_ID == (int)mov.TB_UGE_ID
                                     where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                     where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                                     where a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC != null
                                     where a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada

                                     // where a.TB_MOVIMENTO.TB_MOVIMENTO_ID < this.Entity.Id.Value
                                     select a
                         ).AsQueryable();

                    countEntrada = query.Count();

                    if (countEntrada > 1)
                        if (lista[0].TB_MOVIMENTO_ID == this.Entity.Id)
                            throw new Exception("Não é Permetido Estorna o Primeiro Movimento de Entrada!");

                    //TB_MOVIMENTO_ITEM retorno = (from q in lista where q.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO  < this.Entity.DataMovimento  select q).FirstOrDefault();

                    //if (retorno == null)
                    //    throw new Exception("Não é Permetido Estorna o Primeiro Movimento de Entrada!");
                }
                movItem.TB_MOVIMENTO_ITEM_ATIVO = false;
                movItem.TB_LOGIN_ID_ESTORNO = loginIdEstornante;
                movItem.TB_MOVIMENTO_ITEM_DATA_ESTORNO = DateTime.Now;
            }

            //TODO MELHORIA INFRA - DESATIVADO
            //this.GravarMovimentoHistorico(mov);

            //Desativa o Movimento Estornado
            mov.TB_MOVIMENTO_ATIVO = false;
            if (mov.TB_TIPO_MOVIMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada)
                //reativa requisição - Define status inicial da requisição "pendente"
                req.TB_TIPO_MOVIMENTO_ID = (int)Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente;
            //Atualiza a data da operação para a data atual
            mov.TB_MOVIMENTO_DATA_ESTORNO = DateTime.Now;

            Db.SubmitChanges();
        }

        /// <summary>
        /// Calcula o desdobro dos itens
        /// </summary>
        /// <param name="saldoItens"></param>
        /// <returns></returns>
        public decimal? CalcularDesdobro(List<SaldoSubItemEntity> saldoItens)
        {
            //Total do saldo
            var SaldoValorTotal = (from saldo in saldoItens
                                   select (saldo.SaldoQtde)).Sum();

            //Total de quantidade de itens
            var saldoQtdTotal = (from saldo in saldoItens
                                 select (saldo.SaldoQtde)).Sum();

            //Retorna o desdobro total
            return ((SaldoValorTotal / saldoQtdTotal) - saldoItens[0].PrecoUnit) * saldoQtdTotal;
        }

        public void GravarMovimentoItemHistorico(MovimentoItemEntity movItens)
        {
            TB_HIST_MOVIMENTO_ITEM histMovimentoItem = new TB_HIST_MOVIMENTO_ITEM();

            histMovimentoItem.TB_MOVIMENTO_ITEM_ID = (int)movItens.Id;
            histMovimentoItem.TB_HIST_MOVIMENTO_ID = (int)Movimento.Id;
            histMovimentoItem.TB_UGE_ID = (int)movItens.UGE.Id;
            histMovimentoItem.TB_SUBITEM_MATERIAL_ID = (int)movItens.SubItemMaterial.Id;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItens.DataVencimentoLote;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_LOTE_IDENT = movItens.IdentificacaoLote;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_LOTE_FABR = movItens.FabricanteLote;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_QTDE_MOV = movItens.QtdeMov;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_QTDE_LIQ = movItens.QtdeLiq;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_PRECO_UNIT = movItens.PrecoUnit;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_SALDO_VALOR = movItens.SaldoValor;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_SALDO_QTDE = movItens.SaldoQtde;

            if (movItens.SaldoQtdeLote != null)
                histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItens.SaldoQtdeLote;

            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_VALOR_MOV = movItens.ValorMov;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_DESD = movItens.Desd;
            histMovimentoItem.TB_HIST_MOVIMENTO_ITEM_ATIVO = movItens.Ativo;

            Db.TB_HIST_MOVIMENTO_ITEMs.InsertOnSubmit(histMovimentoItem);
            Db.SubmitChanges();
        }

        public void GravarMovimentoHistorico(MovimentoEntity _lMovimento)
        {
            TB_HIST_MOVIMENTO HistoricoMovimento = new TB_HIST_MOVIMENTO();

            HistoricoMovimento.TB_MOVIMENTO_ID = _lMovimento.Id.Value;
            HistoricoMovimento.TB_ALMOXARIFADO_ID = _lMovimento.Almoxarifado.Id.Value;
            HistoricoMovimento.TB_TIPO_MOVIMENTO_ID = _lMovimento.TipoMovimento.Id;
            HistoricoMovimento.TB_MOVIMENTO_GERADOR_DESCRICAO = _lMovimento.GeradorDescricao;
            HistoricoMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = _lMovimento.NumeroDocumento;
            HistoricoMovimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = _lMovimento.AnoMesReferencia;
            HistoricoMovimento.TB_MOVIMENTO_DATA_DOCUMENTO = _lMovimento.DataDocumento.Value;
            HistoricoMovimento.TB_MOVIMENTO_DATA_MOVIMENTO = _lMovimento.DataMovimento.Value;
            HistoricoMovimento.TB_MOVIMENTO_FONTE_RECURSO = _lMovimento.FonteRecurso;
            HistoricoMovimento.TB_MOVIMENTO_VALOR_DOCUMENTO = _lMovimento.ValorDocumento.Value.truncarDuasCasas();
            HistoricoMovimento.TB_MOVIMENTO_OBSERVACOES = _lMovimento.Observacoes;
            HistoricoMovimento.TB_MOVIMENTO_INSTRUCOES = _lMovimento.Instrucoes;
            HistoricoMovimento.TB_MOVIMENTO_EMPENHO = _lMovimento.Empenho;
            HistoricoMovimento.TB_MOVIMENTO_ATIVO = _lMovimento.Ativo;
            HistoricoMovimento.TB_LOGIN_ID = _lMovimento.IdLogin;
            HistoricoMovimento.TB_LOGIN_ID_ESTORNO = _lMovimento.IdLoginEstorno;

            if (_lMovimento.DataOperacao != null)
            {
                if (_lMovimento.DataOperacao.HasValue)
                    HistoricoMovimento.TB_MOVIMENTO_DATA_OPERACAO = _lMovimento.DataOperacao.Value;
            }

            if (_lMovimento.MovimAlmoxOrigemDestino != null)
            {
                if (_lMovimento.MovimAlmoxOrigemDestino.Id.HasValue)
                    HistoricoMovimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = _lMovimento.MovimAlmoxOrigemDestino.Id.Value;
            }

            if (_lMovimento.Fornecedor != null)
            {
                if (_lMovimento.Fornecedor.Id.HasValue)
                    HistoricoMovimento.TB_FORNECEDOR_ID = _lMovimento.Fornecedor.Id.Value;
            }

            if (_lMovimento.Divisao != null)
            {
                if (_lMovimento.Divisao.Id.HasValue)
                    HistoricoMovimento.TB_DIVISAO_ID = _lMovimento.Divisao.Id.Value;
            }

            if (_lMovimento.UGE != null)
            {
                if (_lMovimento.UGE.Id.HasValue)
                    HistoricoMovimento.TB_UGE_ID = _lMovimento.UGE.Id.Value;
            }

            if (_lMovimento.EmpenhoEvento != null)
            {
                if (_lMovimento.EmpenhoEvento.Id.HasValue)
                    HistoricoMovimento.TB_EMPENHO_EVENTO_ID = _lMovimento.EmpenhoEvento.Id.Value;
            }

            if (_lMovimento.EmpenhoLicitacao != null)
            {
                if (_lMovimento.EmpenhoLicitacao.Id.HasValue)
                    HistoricoMovimento.TB_EMPENHO_LICITACAO_ID = _lMovimento.EmpenhoLicitacao.Id.Value;
            }

            this.Db.TB_HIST_MOVIMENTOs.InsertOnSubmit(HistoricoMovimento);
            this.Db.SubmitChanges();
        }

        [Obsolete("Método desativado")]
        public void GravarMovimentoHistorico(TB_MOVIMENTO pRow_TB_Movimento)
        {
            TB_MOVIMENTO Movimento = new TB_MOVIMENTO();
            TB_HIST_MOVIMENTO HistoricoMovimento = new TB_HIST_MOVIMENTO();

            //if (this.Entity.Id.HasValue)
            //    HistoricoMovimento = this.Db.TB_HIST_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == pRow_TB_Movimento.TB_MOVIMENTO_ID).FirstOrDefault();
            //else
            //    this.Db.TB_HIST_MOVIMENTOs.InsertOnSubmit(HistoricoMovimento);

            HistoricoMovimento.TB_MOVIMENTO_ID = pRow_TB_Movimento.TB_MOVIMENTO_ID;
            HistoricoMovimento.TB_ALMOXARIFADO_ID = pRow_TB_Movimento.TB_ALMOXARIFADO_ID;
            HistoricoMovimento.TB_TIPO_MOVIMENTO_ID = pRow_TB_Movimento.TB_TIPO_MOVIMENTO_ID;
            HistoricoMovimento.TB_MOVIMENTO_GERADOR_DESCRICAO = pRow_TB_Movimento.TB_MOVIMENTO_GERADOR_DESCRICAO;
            HistoricoMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = pRow_TB_Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO;
            HistoricoMovimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = pRow_TB_Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA;
            HistoricoMovimento.TB_MOVIMENTO_DATA_DOCUMENTO = pRow_TB_Movimento.TB_MOVIMENTO_DATA_DOCUMENTO;
            HistoricoMovimento.TB_MOVIMENTO_DATA_MOVIMENTO = pRow_TB_Movimento.TB_MOVIMENTO_DATA_MOVIMENTO;
            HistoricoMovimento.TB_MOVIMENTO_FONTE_RECURSO = pRow_TB_Movimento.TB_MOVIMENTO_FONTE_RECURSO;
            HistoricoMovimento.TB_MOVIMENTO_VALOR_DOCUMENTO = pRow_TB_Movimento.TB_MOVIMENTO_VALOR_DOCUMENTO.truncarDuasCasas();
            HistoricoMovimento.TB_MOVIMENTO_OBSERVACOES = pRow_TB_Movimento.TB_MOVIMENTO_OBSERVACOES;
            HistoricoMovimento.TB_MOVIMENTO_INSTRUCOES = pRow_TB_Movimento.TB_MOVIMENTO_INSTRUCOES;
            HistoricoMovimento.TB_MOVIMENTO_EMPENHO = pRow_TB_Movimento.TB_MOVIMENTO_EMPENHO;
            HistoricoMovimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = pRow_TB_Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO;
            HistoricoMovimento.TB_MOVIMENTO_DATA_APROVACAO = pRow_TB_Movimento.TB_MOVIMENTO_DATA_APROVACAO;
            HistoricoMovimento.TB_FORNECEDOR_ID = pRow_TB_Movimento.TB_FORNECEDOR_ID;
            HistoricoMovimento.TB_DIVISAO_ID = pRow_TB_Movimento.TB_DIVISAO_ID;
            HistoricoMovimento.TB_UGE_ID = pRow_TB_Movimento.TB_UGE_ID;
            HistoricoMovimento.TB_MOVIMENTO_ATIVO = pRow_TB_Movimento.TB_MOVIMENTO_ATIVO;
            HistoricoMovimento.TB_EMPENHO_EVENTO_ID = pRow_TB_Movimento.TB_EMPENHO_EVENTO_ID;
            HistoricoMovimento.TB_MOVIMENTO_DATA_OPERACAO = pRow_TB_Movimento.TB_MOVIMENTO_DATA_OPERACAO.Value;
            HistoricoMovimento.TB_LOGIN_ID = pRow_TB_Movimento.TB_LOGIN_ID;
            HistoricoMovimento.TB_EMPENHO_LICITACAO_ID = pRow_TB_Movimento.TB_EMPENHO_LICITACAO_ID;
            HistoricoMovimento.TB_LOGIN_ID_ESTORNO = pRow_TB_Movimento.TB_LOGIN_ID_ESTORNO;

            this.Db.TB_HIST_MOVIMENTOs.InsertOnSubmit(HistoricoMovimento);
            this.Db.SubmitChanges();
        }

        public void AtualizarMovimentoValorDocumento(MovimentoEntity movimento, decimal valorDocumento)
        {
            TB_MOVIMENTO mov = new TB_MOVIMENTO();

            mov = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == (int)movimento.Id.Value).FirstOrDefault();
            mov.TB_MOVIMENTO_VALOR_DOCUMENTO = valorDocumento.truncarDuasCasas();
            mov.TB_LOGIN_ID = movimento.IdLogin;
            mov.TB_MOVIMENTO_DATA_OPERACAO = DateTime.Now;
            Db.SubmitChanges();
        }

        public void AtualizarMovimentoItem(MovimentoItemEntity movItem)
        {
            TB_MOVIMENTO mov = new TB_MOVIMENTO();
            TB_MOVIMENTO_ITEM movimentoItem = new TB_MOVIMENTO_ITEM();

            movimentoItem = this.Db.TB_MOVIMENTO_ITEMs.Where(a => a.TB_MOVIMENTO_ITEM_ID == movItem.Id.Value).FirstOrDefault();
            movimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
            movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = movItem.SaldoQtde;
            movimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = movItem.SaldoValor;
            movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
            movimentoItem.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;
            movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItem.SaldoQtdeLote;
            Db.SubmitChanges();
        }

        /// <summary>
        /// Retorna a lista de MovimentoItens para serem recalculados
        /// </summary>
        /// <param name="movItem">Movimento a patir que será recalculado</param>
        /// <param name="isEstorno">Recalcular com estorno ou movimento normal</param>
        /// <returns></returns>
        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorIdEstorno(MovimentoItemEntity movItem, bool isEstorno)
        {
            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                         where m.TB_ALMOXARIFADO_ID == (int)movItem.Movimento.Almoxarifado.Id
                                                         where a.TB_UGE_ID == movItem.UGE.Id
                                                         where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
                                                         where ((m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                                                         || (m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida))
                                                         where a.TB_MOVIMENTO_ITEM_ATIVO == true
                                                         where m.TB_MOVIMENTO_ATIVO == true
                                                         orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date ascending, a.TB_MOVIMENTO_ITEM_ID ascending
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
                                                             SubItemMaterial = new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL_ID,
                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                             },
                                                             FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                             IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                             DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                                             Movimento = new MovimentoEntity
                                                             {
                                                                 Id = m.TB_MOVIMENTO_ID,
                                                                 Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID),
                                                                 NumeroDocumento = m.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                                 TipoMovimento = new TipoMovimentoEntity
                                                                 {
                                                                     Id = m.TB_TIPO_MOVIMENTO_ID,
                                                                     TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
                                                                 },
                                                                 DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO
                                                             }
                                                         }).AsQueryable();
            if (isEstorno)
            {
                resultado = resultado.Where(i => i.Movimento.DataMovimento.Value.Date == movItem.Movimento.DataMovimento.Value.Date && i.Id > movItem.Id
                    || i.Movimento.DataMovimento.Value.Date > movItem.Movimento.DataMovimento.Value.Date);
            }
            else
            {
                resultado = resultado.Where(i => i.Movimento.DataMovimento.Value.Date > movItem.Movimento.DataMovimento.Value.Date);
            }

            return resultado.ToList();
        }

        public IList<MovimentoItemEntity> RetornaListaTodosMovimentosLote(MovimentoItemEntity movItem, Boolean estorno)
        {
            ISingleResult<SAM_RETORNA_MOVIMENTO_RETROATIVOResult> procedure;
            if (!estorno)
                procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(movItem.DataMovimento.Value.Date.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, movItem.IdentificacaoLote);
            else
            {
                DateTime? dataRecalcular = getDataDoEstornoParaRecalcular(movItem);
                if (dataRecalcular != null && dataRecalcular.Value > DateTime.MinValue)
                    procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(dataRecalcular.Value.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, movItem.IdentificacaoLote);
                else
                    procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(movItem.DataMovimento.Value.Date.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, movItem.IdentificacaoLote);
            }

            //ISingleResult<SAM_RETORNA_MOVIMENTO_RETROATIVOResult> procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(movItem.DataMovimento.Value.Date.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, movItem.IdentificacaoLote);


            var lista = procedure.ToList();
            IList<MovimentoItemEntity> listaRetorno = new List<MovimentoItemEntity>();

            foreach (SAM_RETORNA_MOVIMENTO_RETROATIVOResult item in lista)
            {
                MovimentoItemEntity movimentoItem = new MovimentoItemEntity();
                movimentoItem.Id = item.TB_MOVIMENTO_ITEM_ID;
                movimentoItem.DataMovimento = Convert.ToDateTime(item.TB_MOVIMENTO_DATA_MOVIMENTO);
                TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity();
                tipoMovimento.Id = item.TB_TIPO_MOVIMENTO_ID;
                tipoMovimento.TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)item.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID);
                MovimentoEntity movimento = new MovimentoEntity(item.TB_MOVIMENTO_ID);
                movimento.TipoMovimento = tipoMovimento;
                movimento.AnoMesReferencia = movItem.AnoMesReferencia;
                movimento.DataMovimento = Convert.ToDateTime(item.TB_MOVIMENTO_DATA_MOVIMENTO);
                movimento.NumeroDocumento = item.TB_MOVIMENTO_NUMERO_DOCUMENTO;
                movimentoItem.Movimento = movimento;

                UGEEntity uge = new UGEEntity(movItem.UGE.Id);
                SubItemMaterialEntity subItem = new SubItemMaterialEntity(movItem.SubItemMaterial.Id);
                movimentoItem.UGE = uge;
                movimentoItem.SubItemMaterial = subItem;
                movimentoItem.PrecoUnit = item.TB_MOVIMENTO_ITEM_PRECO_UNIT;
                movimentoItem.Desd = item.TB_MOVIMENTO_ITEM_DESD;
                movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                movimentoItem.SaldoQtdeLote = item.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE;
                movimentoItem.QtdeMov = item.TB_MOVIMENTO_ITEM_QTDE_MOV;
                movimentoItem.ValorMov = item.TB_MOVIMENTO_ITEM_VALOR_MOV;
                movimentoItem.IdentificacaoLote = item.TB_MOVIMENTO_ITEM_LOTE_IDENT;
                movimentoItem.DataVencimentoLote = item.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC;

                listaRetorno.Add(movimentoItem);
                movimento = null;
            }

            //return listaRetorno;
            return listaRetorno.OrderBy(a => a.Movimento.Id).OrderBy(b => b.DataMovimento).ToList();
        }

        public IList<MovimentoItemEntity> RetornaListaTodosMovimentos(MovimentoItemEntity movItem, Boolean estorno)
        {
            //IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
            //                                             join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
            //                                             where m.TB_ALMOXARIFADO_ID == (int)movItem.Movimento.Almoxarifado.Id
            //                                             where a.TB_UGE_ID == movItem.UGE.Id
            //                                             where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == (int)movItem.SubItemMaterial.Id
            //                                             where (a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= movItem.DataMovimento.Value 
            //                                                    || (a.TB_MOVIMENTO_ITEM_DESD !=null)) 
            //                                             where ((m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
            //                                             || (m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida))
            //                                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
            //                                             where m.TB_MOVIMENTO_ATIVO == true
            //                                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date ascending, a.TB_MOVIMENTO_ITEM_ID ascending
            //                                             select new MovimentoItemEntity
            //                                             {
            //                                                 Id = a.TB_MOVIMENTO_ITEM_ID,
            //                                                 Desd = a.TB_MOVIMENTO_ITEM_DESD,
            //                                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
            //                                                 QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
            //                                                 QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
            //                                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
            //                                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
            //                                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
            //                                                 ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV,
            //                                                 SubItemMaterial = new SubItemMaterialEntity
            //                                                 {
            //                                                     Id = a.TB_SUBITEM_MATERIAL_ID,
            //                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
            //                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
            //                                                 },
            //                                                 FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
            //                                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
            //                                                 DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
            //                                                 UGE = new UGEEntity(a.TB_UGE_ID),
            //                                                 Movimento = new MovimentoEntity
            //                                                 {
            //                                                     Id = m.TB_MOVIMENTO_ID,
            //                                                     Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID),
            //                                                     NumeroDocumento = m.TB_MOVIMENTO_NUMERO_DOCUMENTO,
            //                                                     TipoMovimento = new TipoMovimentoEntity
            //                                                     {
            //                                                         Id = m.TB_TIPO_MOVIMENTO_ID,
            //                                                         TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)m.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
            //                                                     },
            //                                                     DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
            //                                                     AnoMesReferencia = m.TB_MOVIMENTO_ANO_MES_REFERENCIA
            //                                                 }
            //                                             }).AsQueryable();


            //return resultado.ToList();
            ISingleResult<SAM_RETORNA_MOVIMENTO_RETROATIVOResult> procedure;
            if (!estorno)
                procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(movItem.DataMovimento.Value.Date.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, null);
            else
            {
                DateTime? dataRecalcular = getDataDoEstornoParaRecalcular(movItem);
                if (dataRecalcular != null && dataRecalcular.Value > DateTime.MinValue)
                    procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(dataRecalcular.Value.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, null);
                else
                    procedure = Db.SAM_RETORNA_MOVIMENTO_RETROATIVO(movItem.DataMovimento.Value.Date.ToShortDateString(), movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value, null);
            }



            var lista = procedure.ToList();
            IList<MovimentoItemEntity> listaRetorno = new List<MovimentoItemEntity>();

            foreach (SAM_RETORNA_MOVIMENTO_RETROATIVOResult item in lista)
            {
                MovimentoItemEntity movimentoItem;
                TipoMovimentoEntity tipoMovimento;
                MovimentoEntity movimento;
                UGEEntity uge;
                SubItemMaterialEntity subItem;

                movimentoItem = new MovimentoItemEntity();

                //if (estorno)
                //{
                //    movItem.DataMovimento = DateTime.Parse(item.TB_MOVIMENTO_DATA_MOVIMENTO);

                //    TB_MOVIMENTO_ITEM movSaldoAnteriorEstorno = setSaldoAnteriorDoEstorno(movItem);

                //    if (movSaldoAnteriorEstorno != null)
                //    {
                //        movimentoItem.SaldoQtde = movSaldoAnteriorEstorno.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                //        movimentoItem.SaldoValor = movSaldoAnteriorEstorno.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                //    }
                //    else
                //    {
                //        movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                //        movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                //    }
                //    estorno = false;
                //}
                //else
                //{
                //    movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                //    movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                //}

                movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;

                movimentoItem.Id = item.TB_MOVIMENTO_ITEM_ID;
                movimentoItem.DataMovimento = Convert.ToDateTime(item.TB_MOVIMENTO_DATA_MOVIMENTO);
                tipoMovimento = new TipoMovimentoEntity();
                tipoMovimento.Id = item.TB_TIPO_MOVIMENTO_ID;
                tipoMovimento.TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)item.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID);
                movimento = new MovimentoEntity(item.TB_MOVIMENTO_ID);
                movimento.TipoMovimento = tipoMovimento;
                movimento.NumeroDocumento = item.TB_MOVIMENTO_NUMERO_DOCUMENTO;
                movimento.AnoMesReferencia = movItem.AnoMesReferencia;
                movimento.DataMovimento = Convert.ToDateTime(item.TB_MOVIMENTO_DATA_MOVIMENTO);
                movimento.IdTemp = item.TB_MOVIMENTO_ID;
                movimentoItem.Movimento = movimento;

                uge = new UGEEntity(movItem.UGE.Id);
                subItem = new SubItemMaterialEntity
                {
                    Id = movItem.SubItemMaterial.Id,
                    Codigo = movItem.SubItemMaterial.Codigo
                };

                movimentoItem.UGE = uge;
                movimentoItem.SubItemMaterial = subItem;
                movimentoItem.PrecoUnit = item.TB_MOVIMENTO_ITEM_PRECO_UNIT;
                movimentoItem.Desd = item.TB_MOVIMENTO_ITEM_DESD;
                movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                movimentoItem.SaldoQtdeLote = item.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE;
                movimentoItem.QtdeMov = item.TB_MOVIMENTO_ITEM_QTDE_MOV;
                movimentoItem.ValorMov = item.TB_MOVIMENTO_ITEM_VALOR_MOV;
                movimentoItem.IdentificacaoLote = item.TB_MOVIMENTO_ITEM_LOTE_IDENT;
                movimentoItem.DataVencimentoLote = item.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC;
                movimentoItem.NL_Consumo = item.TB_MOVIMENTO_ITEM_NL_CONSUMO;
                movimentoItem.NL_Liquidacao = item.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO;
                movimentoItem.NL_LiquidacaoEstorno = item.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO;
                movimentoItem.NL_Reclassificacao = item.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO;
                movimentoItem.NL_ReclassificacaoEstorno = item.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO;

                listaRetorno.Add(movimentoItem);
                movimento = null;
            }
            //return listaRetorno;
            return listaRetorno.OrderBy(a => a.Movimento.Id).OrderBy(b => b.DataMovimento).ToList();
        }

        private TB_MOVIMENTO_ITEM setSaldoAnteriorDoEstorno(MovimentoItemEntity movItem)
        {
            IQueryable<TB_MOVIMENTO_ITEM> query = (from q in this.Db.TB_MOVIMENTO_ITEMs
                                                   join m in this.Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                   where m.TB_MOVIMENTO_DATA_MOVIMENTO < movItem.DataMovimento.Value
                                                   && m.TB_ALMOXARIFADO_ID == movItem.Almoxarifado.Id
                                                   && m.TB_MOVIMENTO_ATIVO == true
                                                   && q.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id.Value
                                                   && q.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   orderby m.TB_MOVIMENTO_DATA_MOVIMENTO descending
                                                   orderby q.TB_MOVIMENTO_ITEM_ID descending
                                                   select q).Take(1);

            return query.FirstOrDefault();


        }

        private DateTime? getDataDoEstornoParaRecalcular(MovimentoItemEntity movItem)
        {
            IQueryable<DateTime> query = (from q in this.Db.TB_MOVIMENTO_ITEMs
                                          join m in this.Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                          join a in this.Db.TB_TIPO_MOVIMENTOs on m.TB_TIPO_MOVIMENTO_ID equals a.TB_TIPO_MOVIMENTO_ID
                                          where m.TB_MOVIMENTO_DATA_MOVIMENTO < movItem.DataMovimento.Value
                                          && m.TB_ALMOXARIFADO_ID == movItem.Movimento.Almoxarifado.Id.Value
                                          && m.TB_MOVIMENTO_ATIVO == true
                                          && q.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id.Value
                                          && q.TB_MOVIMENTO_ITEM_ATIVO == true
                                          && ((a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                          || (a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada))
                                          orderby m.TB_MOVIMENTO_DATA_MOVIMENTO descending
                                          orderby q.TB_MOVIMENTO_ITEM_ID descending
                                          select m.TB_MOVIMENTO_DATA_MOVIMENTO).Take(1);

            return query.FirstOrDefault();


        }

        private DateTime? getDataDoEstornoParaRecalcularLote(MovimentoItemEntity movItem)
        {
            IQueryable<DateTime> query = (from q in this.Db.TB_MOVIMENTO_ITEMs
                                          join m in this.Db.TB_MOVIMENTOs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                          where m.TB_MOVIMENTO_DATA_MOVIMENTO < movItem.DataMovimento.Value
                                          && m.TB_ALMOXARIFADO_ID == movItem.Movimento.Almoxarifado.Id.Value
                                          && m.TB_MOVIMENTO_ATIVO == true
                                          && q.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id.Value
                                          && q.TB_MOVIMENTO_ITEM_ATIVO == true
                                          && q.TB_MOVIMENTO_ITEM_LOTE_IDENT == movItem.IdentificacaoLote
                                          orderby m.TB_MOVIMENTO_DATA_MOVIMENTO descending
                                          orderby q.TB_MOVIMENTO_ITEM_ID descending
                                          select m.TB_MOVIMENTO_DATA_MOVIMENTO).Take(1);

            return query.FirstOrDefault();


        }

        public Int32 getTipoMovimento(Int32 movimentoId)
        {

            IQueryable<Int32> query = (from q in this.Db.TB_MOVIMENTOs where q.TB_MOVIMENTO_ATIVO == true && q.TB_MOVIMENTO_ID == movimentoId select q.TB_TIPO_MOVIMENTO_ID).AsQueryable();

            return query.FirstOrDefault();
        }


        public DateTime? primeiraDataMovimentoDoSubItemDoAlmoxarifado(Int32 almoxarifadoId, Int32 ugeId, Int32 subItemId)
        {
            IQueryable<TB_MOVIMENTO> query = (from q in Db.TB_MOVIMENTOs
                                              join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                              where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                                              where m.TB_SUBITEM_MATERIAL_ID == subItemId
                                              where m.TB_UGE_ID == ugeId
                                              select q).AsQueryable();

            try
            {
                var retorno = query.Min(mo => mo.TB_MOVIMENTO_DATA_MOVIMENTO);
                return retorno;
            }
            catch (Exception ex)
            {

                query = (from q in Db.TB_MOVIMENTOs
                         join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                         where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                         where m.TB_SUBITEM_MATERIAL_ID == subItemId
                         select q).AsQueryable();

                var retorno = query.Min(mo => mo.TB_MOVIMENTO_DATA_MOVIMENTO);

                return retorno;


            }


        }

        public DateTime? primeiraDataMovimentoDoSubItemDoAlmoxarifado(Int32 almoxarifadoId, Int32 ugeId, Int32 subItemId, string IdentLote)
        {
            IQueryable<TB_MOVIMENTO> query = (from q in Db.TB_MOVIMENTOs
                                              join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                              where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                                              where m.TB_SUBITEM_MATERIAL_ID == subItemId
                                              where m.TB_UGE_ID == ugeId
                                              where m.TB_MOVIMENTO_ITEM_LOTE_IDENT == IdentLote
                                              select q).AsQueryable();

            try
            {
                var retorno = query.Min(mo => mo.TB_MOVIMENTO_DATA_MOVIMENTO);
                return retorno;
            }
            catch (Exception ex)
            {

                query = (from q in Db.TB_MOVIMENTOs
                         join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                         where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                         where m.TB_SUBITEM_MATERIAL_ID == subItemId
                         where m.TB_MOVIMENTO_ITEM_LOTE_IDENT == IdentLote
                         select q).AsQueryable();

                var retorno = query.Min(mo => mo.TB_MOVIMENTO_DATA_MOVIMENTO);

                return retorno;


            }


        }
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorMovimentoPendente(int GestorId)
        {
            IQueryable<TB_ALMOXARIFADO> resultado = (from a in Db.TB_ALMOXARIFADOs select a);
            var retorno = resultado.Cast<TB_ALMOXARIFADO>().ToList();

            var retornoLista = (from r in retorno
                                group r by new
                                {
                                    r.TB_ALMOXARIFADO_ID,
                                    r.TB_ALMOXARIFADO_CODIGO,
                                    r.TB_ALMOXARIFADO_DESCRICAO,

                                } into am
                                select new AlmoxarifadoEntity()
                                {
                                    Id = am.Key.TB_ALMOXARIFADO_ID,
                                    Codigo = am.Key.TB_ALMOXARIFADO_CODIGO,
                                    //TB_ALMOXARIFADO_DESCRICAO = am.Key.TB_ALMOXARIFADO_CODIGO.ToString() + " - " +  am.Key.TB_ALMOXARIFADO_DESCRICAO,
                                    Descricao = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.Id).ToList();
        }
        public IList<Int32> retornaIdDoSubItem(Int64 subItemCodigo)
        {
            IQueryable<Int32> query = (from q in Db.TB_SUBITEM_MATERIALs
                                       where q.TB_SUBITEM_MATERIAL_CODIGO == subItemCodigo
                                       select q.TB_SUBITEM_MATERIAL_ID).AsQueryable();



            return query.ToList();
        }

        public IList<Int32> retornaIdDoSubItem(Int64 subItemCodigo, int gestorId)
        {
            IQueryable<Int32> query = (from q in Db.TB_SUBITEM_MATERIALs
                                       where q.TB_SUBITEM_MATERIAL_CODIGO == subItemCodigo
                                       where q.TB_GESTOR_ID == gestorId
                                       select q.TB_SUBITEM_MATERIAL_ID).AsQueryable();



            return query.ToList();
        }
        public IList<Int32> retornaIdDoSubItemPorAlmoxarifado(Int32 almoxarifadoId)
        {
            IQueryable<TB_MOVIMENTO_ITEM> query = (from q in Db.TB_MOVIMENTOs
                                                   join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                   where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                                                   where q.TB_MOVIMENTO_ATIVO == true
                                                   where m.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   select m).AsQueryable();



            return query.DistinctBy(d => d.TB_SUBITEM_MATERIAL_ID).Select(q => q.TB_SUBITEM_MATERIAL_ID).ToList();
        }


        public Int32? retornaUgeDoAlmoxarifado(Int32 almoxarifadoId)
        {
            IQueryable<Int32?> query = (from q in Db.TB_ALMOXARIFADOs
                                        where q.TB_ALMOXARIFADO_ID == almoxarifadoId && q.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true
                                        select q.TB_UGE_ID).AsQueryable();

            return query.FirstOrDefault();
        }
        public decimal? CalcularDesdobro(decimal? precoAcum, decimal? precoMedio, decimal? qtdeAcum)
        {

            return precoAcum - (precoMedio * qtdeAcum);
        }
        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE()
        {
            string formatAnoMesRef = "01/" + this.Entity.AnoMesReferencia.Substring(4, 2) + "/" + this.Entity.AnoMesReferencia.Substring(0, 4);
            string AnoMesRefMenos1 = Convert.ToDateTime(formatAnoMesRef).AddMonths(-1).ToString("yyyyMM");

            // CodigoFormatado = usado para descrever nome do fornecedor, almoxarifado ou divisão
            IEnumerable<MovimentoEntity> resultado = (from a in this.Db.TB_MOVIMENTOs
                                                      where (a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id || this.Entity.Almoxarifado.Id == 0)
                                                      where (a.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                                                      where (a.TB_MOVIMENTO_NUMERO_DOCUMENTO != "")
                                                      where (a.TB_MOVIMENTO_ATIVO == true)
                                                      // Listar os documentos pendentes do mÃƒÂªs de ref atual do almoxarifado ou do MÃƒÂªs anterior.
                                                      where (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == this.Entity.AnoMesReferencia || (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == AnoMesRefMenos1))
                                                      orderby a.TB_MOVIMENTO_NUMERO_DOCUMENTO descending
                                                      select new MovimentoEntity
                                                      {
                                                          Id = a.TB_MOVIMENTO_ID,
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
                                                          Ativo = a.TB_MOVIMENTO_ATIVO,
                                                          Almoxarifado = new AlmoxarifadoEntity
                                                          {
                                                              Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID
                                                          },
                                                          Fornecedor = new FornecedorEntity
                                                          {
                                                              Id = a.TB_FORNECEDOR.TB_FORNECEDOR_ID,
                                                              Nome = a.TB_FORNECEDOR.TB_FORNECEDOR_NOME
                                                          },
                                                          CodigoFormatado =
                                                            a.TB_FORNECEDOR != null ? a.TB_FORNECEDOR.TB_FORNECEDOR_NOME :
                                                                (a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO != null && a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO != 0) ? (from ald in Db.TB_ALMOXARIFADOs where ald.TB_ALMOXARIFADO_ID == a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO select ald.TB_ALMOXARIFADO_DESCRICAO).FirstOrDefault() :
                                                                    a.TB_DIVISAO != null ? a.TB_DIVISAO.TB_DIVISAO_DESCRICAO :
                                                                    "",

                                                          UGE = new UGEEntity(a.TB_UGE.TB_UGE_ID),
                                                          MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(a.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                          TipoMovimento = new TipoMovimentoEntity
                                                          {
                                                              Id = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                              AgrupamentoId = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                              Codigo = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                              CodigoDescricao = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                              Descricao = a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO
                                                          },

                                                          Divisao = new DivisaoEntity()
                                                          {
                                                              Id = a.TB_DIVISAO.TB_DIVISAO_ID,
                                                              Descricao = a.TB_DIVISAO.TB_DIVISAO_DESCRICAO
                                                          },

                                                      })
                               ;

            if (this.Entity.UGE.Id != 0)
                resultado = resultado.Where(a => a.UGE.Id == this.Entity.UGE.Id);

            if (this.Entity.MovimAlmoxOrigemDestino != null && this.Entity.MovimAlmoxOrigemDestino.Id.HasValue && this.Entity.MovimAlmoxOrigemDestino.Id != 0)
                resultado = resultado.Where(a => a.MovimAlmoxOrigemDestino.Id == this.Entity.MovimAlmoxOrigemDestino.Id);

            this.totalregistros = resultado.Count();
            return resultado.ToList();
        }

        #endregion

        /// <summary>
        /// Sobrecarga criada para retornar apenas documentos que podem ser estornar (movimentações cadastradas no mês corrente do almoxarifado).
        /// </summary>
        /// <param name="docsParaRetorno">ParÃ¢metro para indicar se a lista de documentos retornados será utilizadas para estorno ou não.</param>
        /// <returns></returns>
        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE(bool docsParaRetorno)
        {
            if (!docsParaRetorno)
                return ListarDocumentoByAlmoxarifadoUGE();

            string formatAnoMesRef = "01/" + this.Entity.AnoMesReferencia.Substring(4, 2) + "/" + this.Entity.AnoMesReferencia.Substring(0, 4);

            // CodigoFormatado = usado para descrever nome do fornecedor, almoxarifado ou divisão
            IQueryable<MovimentoEntity> resultado = (from Movimento in this.Db.TB_MOVIMENTOs
                                                     where (Movimento.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id || this.Entity.Almoxarifado.Id == 0)
                                                     where (Movimento.TB_TIPO_MOVIMENTO_ID == this.Entity.TipoMovimento.Id)
                                                     where (Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO != "")
                                                     where (Movimento.TB_MOVIMENTO_ATIVO == true)
                                                     // Listar os documentos pendentes do mês de ref atual.
                                                     where (Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA == this.Entity.AnoMesReferencia)
                                                     orderby Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO descending
                                                     select new MovimentoEntity
                                                     {
                                                         Id = Movimento.TB_MOVIMENTO_ID,
                                                         GeradorDescricao = Movimento.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                         NumeroDocumento = Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                         AnoMesReferencia = Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                         DataDocumento = Movimento.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                         DataMovimento = Movimento.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                         FonteRecurso = Movimento.TB_MOVIMENTO_FONTE_RECURSO,
                                                         ValorDocumento = Movimento.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                         Observacoes = Movimento.TB_MOVIMENTO_OBSERVACOES,
                                                         Instrucoes = Movimento.TB_MOVIMENTO_INSTRUCOES,
                                                         Empenho = Movimento.TB_MOVIMENTO_EMPENHO,
                                                         Ativo = Movimento.TB_MOVIMENTO_ATIVO,
                                                         Almoxarifado = new AlmoxarifadoEntity(Movimento.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID),
                                                         Fornecedor = new FornecedorEntity()
                                                         {
                                                             Id = Movimento.TB_FORNECEDOR.TB_FORNECEDOR_ID,
                                                             Nome = Movimento.TB_FORNECEDOR.TB_FORNECEDOR_NOME
                                                         },
                                                         CodigoFormatado = Movimento.TB_FORNECEDOR != null ? Movimento.TB_FORNECEDOR.TB_FORNECEDOR_NOME :
                                                                               (Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO != null && Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO != 0) ? (from ald in Db.TB_ALMOXARIFADOs where ald.TB_ALMOXARIFADO_ID == Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO select ald.TB_ALMOXARIFADO_DESCRICAO).FirstOrDefault() :
                                                                                   Movimento.TB_DIVISAO != null ? Movimento.TB_DIVISAO.TB_DIVISAO_DESCRICAO : "",

                                                         UGE = new UGEEntity(Movimento.TB_UGE.TB_UGE_ID),
                                                         MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                                         TipoMovimento = new TipoMovimentoEntity()
                                                         {
                                                             Id = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                                                             AgrupamentoId = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                             Codigo = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                                                             CodigoDescricao = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                             Descricao = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO
                                                         },

                                                         Divisao = new DivisaoEntity()
                                                         {
                                                             Id = Movimento.TB_DIVISAO.TB_DIVISAO_ID,
                                                             Descricao = Movimento.TB_DIVISAO.TB_DIVISAO_DESCRICAO
                                                         },
                                                     }).AsQueryable<MovimentoEntity>();

            if (this.Entity.UGE.Id != 0)
                resultado = resultado.Where(a => Movimento.UGE.Id == this.Entity.UGE.Id);

            if (this.Entity.MovimAlmoxOrigemDestino != null && this.Entity.MovimAlmoxOrigemDestino.Id.HasValue && this.Entity.MovimAlmoxOrigemDestino.Id != 0)
                resultado = resultado.Where(a => Movimento.MovimAlmoxOrigemDestino.Id == this.Entity.MovimAlmoxOrigemDestino.Id);

            this.totalregistros = resultado.Count();

            return resultado.ToList();
        }

        public int ListarDocumentoByDocTransf(string numDocumento)
        {


            // CodigoFormatado = usado para descrever nome do fornecedor, almoxarifado ou divisão
            IQueryable<MovimentoEntity> resultado = (from Movimento in this.Db.TB_MOVIMENTOs
                                                         //where (Movimento.TB_ALMOXARIFADO_ID == almoxDestino)
                                                     where (Movimento.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                                                     where (Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO == numDocumento)
                                                     where (Movimento.TB_MOVIMENTO_ATIVO == true)
                                                     select new MovimentoEntity
                                                     {
                                                         Id = Movimento.TB_MOVIMENTO_ID,
                                                         GeradorDescricao = Movimento.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                                         NumeroDocumento = Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                         AnoMesReferencia = Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                         DataDocumento = Movimento.TB_MOVIMENTO_DATA_DOCUMENTO,
                                                         DataMovimento = Movimento.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                         FonteRecurso = Movimento.TB_MOVIMENTO_FONTE_RECURSO,
                                                         ValorDocumento = Movimento.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                         Almoxarifado = new AlmoxarifadoEntity(Movimento.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID),
                                                     }).AsQueryable<MovimentoEntity>();


            return resultado.Count();
        }

        /// <summary>
        /// Retorna o documento pelo numero
        /// </summary>
        /// <param name="numeroDocumento">Numero do documento</param>
        /// <returns></returns>
        public MovimentoEntity ListarDocumentosRequisicaoById(int id)
        {
            this.Movimento = new MovimentoEntity();
            this.Movimento.Id = id;
            return GetMovimentos().Where(a => a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente ||
                a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada).FirstOrDefault();
        }

        public MovimentoEntity ListarDocumentosRequisicaoByDocumento(string numeroDocumento)
        {
            this.Movimento = new MovimentoEntity();
            this.Movimento.Id = null;
            this.Movimento.NumeroDocumento = numeroDocumento;
            return GetMovimentos().Where(a => a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente ||
                a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada).FirstOrDefault();
        }

        [Obsolete]
        public List<MovimentoEntity> ListarMovimentosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId, string pStrAnoMesReferencia)
        {
            IEnumerable<MovimentoItemEntity> lIEnumItensMovimentacao = null;

            HashSet<MovimentoEntity> lHsMovimentos = new HashSet<MovimentoEntity>();
            List<MovimentoItemEntity> lLstMovItem = new List<MovimentoItemEntity>();
            List<MovimentoEntity> lLstMovimentos = new List<MovimentoEntity>();
            string lStrSQLStatement = string.Empty;

            lLstMovItem = new List<MovimentoItemEntity>();
            lHsMovimentos = new HashSet<MovimentoEntity>();

            lIEnumItensMovimentacao = (from MovItem in this.Db.TB_MOVIMENTO_ITEMs
                                       join Mov in Db.TB_MOVIMENTOs on MovItem.TB_MOVIMENTO_ID equals Mov.TB_MOVIMENTO_ID
                                       where (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty || MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null)
                                       where (MovItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                                       where (Mov.TB_MOVIMENTO_ATIVO == true)
                                       where (Mov.TB_UGE_ID == pIntUgeId)
                                       where (Mov.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoId)
                                       where (Mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == pStrAnoMesReferencia)
                                       orderby MovItem.TB_MOVIMENTO_ID
                                       select new MovimentoItemEntity
                                       {
                                           Id = MovItem.TB_MOVIMENTO_ITEM_ID,
                                           Movimento = this.LerRegistro(MovItem.TB_MOVIMENTO_ID)
                                       });

            lLstMovItem = lIEnumItensMovimentacao.Where(Movimento => Movimento.AnoMesReferencia == pStrAnoMesReferencia)
                                                 .Distinct()
                                                 .ToList();

            lLstMovItem.ForEach(MovItem => lHsMovimentos.Add(MovItem.Movimento));
            lLstMovimentos = lHsMovimentos.ToList();

            return lLstMovimentos;
        }

        public List<MovimentoEntity> ListarMovimentosNaoLiquidados(int pIntUgeId, int pIntAlmoxarifadoId)
        {
            IEnumerable<MovimentoItemEntity> lIEnumItensMovimentacao = null;

            HashSet<MovimentoEntity> lHsMovimentos = new HashSet<MovimentoEntity>();
            List<MovimentoItemEntity> lLstMovItem = new List<MovimentoItemEntity>();
            List<MovimentoEntity> lLstMovimentos = new List<MovimentoEntity>();
            string lStrSQLStatement = string.Empty;

            lLstMovItem = new List<MovimentoItemEntity>();
            lHsMovimentos = new HashSet<MovimentoEntity>();

            lIEnumItensMovimentacao = (from MovItem in this.Db.TB_MOVIMENTO_ITEMs
                                       join Mov in Db.TB_MOVIMENTOs on MovItem.TB_MOVIMENTO_ID equals Mov.TB_MOVIMENTO_ID
                                       where (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty || MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null)
                                       where (MovItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                                       where (Mov.TB_MOVIMENTO_ATIVO == true)
                                       where (Mov.TB_UGE_ID == pIntUgeId)
                                       where (Mov.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoId)
                                       orderby MovItem.TB_MOVIMENTO_ID
                                       select new MovimentoItemEntity
                                       {
                                           Id = MovItem.TB_MOVIMENTO_ITEM_ID,
                                           Movimento = this.LerRegistro(MovItem.TB_MOVIMENTO_ID)
                                       });

            lLstMovItem = lIEnumItensMovimentacao.Distinct()
                                                 .ToList();

            lLstMovItem.ForEach(MovItem => lHsMovimentos.Add(MovItem.Movimento));
            lLstMovimentos = lHsMovimentos.ToList();

            return lLstMovimentos;
        }

        public IList<string> ListarEmpenhosDeMovimentosJaLiquidados(int pIntUgeId, int pIntAlmoxarifadoId)
        {
            List<string> lLstStrEmpenhos = new List<string>();
            //IEnumerable<MovimentoEntity> lIEnumMovimentos = null;
            IQueryable<MovimentoEntity> lIEnumMovimentos = null;
            List<MovimentoItemEntity> lLstMovItem = new List<MovimentoItemEntity>();
            List<MovimentoEntity> lLstMovimentos = new List<MovimentoEntity>();
            string lStrPrefixoNL = String.Format("{0}{1}", DateTime.Now.Year, "NL");


            lIEnumMovimentos = (from MovItem in this.Db.TB_MOVIMENTO_ITEMs
                                join Mov in Db.TB_MOVIMENTOs on MovItem.TB_MOVIMENTO_ID equals Mov.TB_MOVIMENTO_ID
                                where (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO.Contains(lStrPrefixoNL) || (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null || MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty))
                                where (MovItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                                where (Mov.TB_MOVIMENTO_ATIVO == true)
                                where (Mov.TB_UGE_ID == pIntUgeId)
                                where (Mov.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoId)
                                orderby MovItem.TB_MOVIMENTO_ID
                                select new MovimentoEntity
                                {
                                    //Id = MovItem.TB_MOVIMENTO_ITEM_ID,
                                    //Movimento = this.LerRegistro(MovItem.TB_MOVIMENTO_ID)
                                    Empenho = MovItem.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO
                                }).AsQueryable();
            //.Select(MovItem => MovItem.Movimento);


            lLstStrEmpenhos = lIEnumMovimentos.OrderBy(Movimento => Movimento.Empenho)
                                              .Select(Movimento => Movimento.Empenho)
                                              .Distinct()
                                              .ToList();

            return lLstStrEmpenhos;
        }

        //TODO: Unir com o método que retorna empenhos não-liquidados (generalizar)
        public IList<string> obterListaEmpenhosLiquidados(int almoxID, string almoxAnoMesRef)
        {
            IList<string> lstEmpenhos = new List<string>();
            IQueryable<string> qryConsulta = null;
            string lStrPrefixoNL = String.Format("{0}{1}", almoxAnoMesRef.Substring(0, 4), "NL");


            qryConsulta = (from MovItem in this.Db.TB_MOVIMENTO_ITEMs
                           join Mov in Db.TB_MOVIMENTOs on MovItem.TB_MOVIMENTO_ID equals Mov.TB_MOVIMENTO_ID
                           where (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO.StartsWith(lStrPrefixoNL) || (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null || MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty))
                           where (MovItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                           where (Mov.TB_MOVIMENTO_ATIVO == true)
                           //where (Mov.TB_UGE.TB_UGE_CODIGO == codigoUGE)
                           where (Mov.TB_ALMOXARIFADO_ID == almoxID)
                           where (Mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == almoxAnoMesRef)
                           orderby MovItem.TB_MOVIMENTO_ID
                           select Mov.TB_MOVIMENTO_EMPENHO).AsQueryable();

            lstEmpenhos = qryConsulta.Distinct().ToList();
            ((List<string>)lstEmpenhos).Sort();

            return lstEmpenhos;
        }
        public IList<string> obterListaEmpenhosNaoLiquidados(int almoxID, string almoxAnoMesRef)
        {
            List<string> lstEmpenhos = new List<string>();
            IQueryable<string> qryConsulta = null;
            string lStrPrefixoNL = String.Format("{0}{1}", almoxAnoMesRef.Substring(0, 4), "NL");


            qryConsulta = (from MovItem in this.Db.TB_MOVIMENTO_ITEMs
                           join Mov in Db.TB_MOVIMENTOs on MovItem.TB_MOVIMENTO_ID equals Mov.TB_MOVIMENTO_ID
                           where (MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null || MovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty)
                           where (MovItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                           where (Mov.TB_MOVIMENTO_ATIVO == true)
                           //where (Mov.TB_UGE.TB_UGE_CODIGO == codigoUGE)
                           where (Mov.TB_ALMOXARIFADO_ID == almoxID)
                           where (Mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == almoxAnoMesRef)
                           orderby MovItem.TB_MOVIMENTO_ID
                           select Mov.TB_MOVIMENTO_EMPENHO).AsQueryable();

            lstEmpenhos = qryConsulta.Distinct().ToList();
            ((List<string>)lstEmpenhos).Sort();

            return lstEmpenhos;
        }
        internal IList<string> obterListaEmpenhosSistema(int almoxID, string anoMesRefAlmox, bool empenhosNaoLiquidados)
        {
            IList<string> lstMovimentacoes = null;

            IQueryable<string> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereLiquidadoOuNao;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal;


            //filtro básico
            //var tipoMovimento = (int)enumTipoMovimento.AquisicaoCompraEmpenho;
            var tipoMovimento = (int)enumTipoMovimento.EntradaPorEmpenho;
            expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovimento
                                               && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                               && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRefAlmox
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            expWhere = expWherePrincipal;

            if (!String.IsNullOrWhiteSpace(anoMesRefAlmox)) //MesRef
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRefAlmox));

            //Retornar liquidados ou não
            if (empenhosNaoLiquidados)
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty);
            else
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty);

            #region Consulta Refactored
            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable()
                                               .Where(expWhere)
                                               .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                               .Where(expWhereLiquidadoOuNao)
                                               .Select(_movItem => _movItem.TB_MOVIMENTO)
                                               .DistinctBy(movEmpenho => movEmpenho.TB_MOVIMENTO_ID)
                                               .Select(movEmpenho => movEmpenho.TB_MOVIMENTO_EMPENHO)
                                               .ToList()
                                               .AsQueryable();

            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion Consulta Refactored

            lstMovimentacoes = qryConsulta.ToList();

            return lstMovimentacoes;
        }
        #region Empenhos

        public MovimentoEntity ObterMovimentacao(int _movID, bool isEmpenho = false)
        {
            MovimentoEntity objMovimentacao = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Func<TB_MOVIMENTO, MovimentoEntity> _actionSeletor = null;

            _actionSeletor = _instanciadorDTOMovimentacoes(isEmpenho);

            //filtro básico
            expWhere = (_movimentacao => _movimentacao.TB_MOVIMENTO_ID == _movID);

            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            qryConsulta = qryConsulta.Where(expWhere);


            objMovimentacao = qryConsulta.ToList()
                                         .Select(_actionSeletor)
                                         .ToList()
                                         .FirstOrDefault();

            //TODO [CORRECAO FASE 2] - Gerar método .ObterUGE() e transportar valor da UI de UGE/Gestão até camada de negócios
            //if(  ((objMovimentacao.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorTransferencia) || (objMovimentacao.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado))
            //   &&  objMovimentacao.MovimAlmoxOrigemDestino.Id.HasValue)
            //{
            //    var objInfraAux = new AlmoxarifadoInfraestructure();
            //    objMovimentacao.MovimAlmoxOrigemDestino = objInfraAux.ObterAlmoxarifado(objMovimentacao.MovimAlmoxOrigemDestino.Id);
            //}

            return objMovimentacao;
        }

        public UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiafisico(string siglaUnidadeFornecimentoSiafisico)
        {
            UnidadeFornecimentoSiafEntity objUnidadeFornecimentoSiafisico = null;

            IQueryable<TB_UNIDADE_FORNECIMENTO_SIAF> qryConsulta = null;
            Expression<Func<TB_UNIDADE_FORNECIMENTO_SIAF, bool>> expWhere;
            Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _actionSeletor = null;

            //filtro básico
            var _siglaUnidadeFornecimentoSiafisico = siglaUnidadeFornecimentoSiafisico.ToUpperInvariant();
            expWhere = (_unidFornSiaf => _unidFornSiaf.TB_UNIDADE_FORNECIMENTO_DESCRICAO == _siglaUnidadeFornecimentoSiafisico);

            qryConsulta = (from unidFornSiaf in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                           select unidFornSiaf).AsQueryable<TB_UNIDADE_FORNECIMENTO_SIAF>();

            qryConsulta = qryConsulta.Where(expWhere);

            _actionSeletor = _instanciadorDTOUnidadeFornecimentoSiafisico(_siglaUnidadeFornecimentoSiafisico);

            objUnidadeFornecimentoSiafisico = qryConsulta.ToList()
                                                         .Select(_actionSeletor)
                                                         .ToList()
                                                         .FirstOrDefault();

            return objUnidadeFornecimentoSiafisico;
        }
        public UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiafisico(int codigoUnidFornSiafisico)
        {
            UnidadeFornecimentoSiafEntity objUnidadeFornecimentoSiafisico = null;

            IQueryable<TB_UNIDADE_FORNECIMENTO_SIAF> qryConsulta = null;
            Expression<Func<TB_UNIDADE_FORNECIMENTO_SIAF, bool>> expWhere;
            Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _actionSeletor = null;

            //filtro básico
            var _codigoUnidFornSiafisico = codigoUnidFornSiafisico;
            expWhere = (_unidFornSiaf => _unidFornSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO == _codigoUnidFornSiafisico);

            qryConsulta = (from unidFornSiaf in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                           select unidFornSiaf).AsQueryable<TB_UNIDADE_FORNECIMENTO_SIAF>();

            qryConsulta = qryConsulta.Where(expWhere);

            _actionSeletor = _instanciadorDTOUnidadeFornecimentoSiafisico(_codigoUnidFornSiafisico);

            objUnidadeFornecimentoSiafisico = qryConsulta.ToList()
                                                         .Select(_actionSeletor)
                                                         .ToList()
                                                         .FirstOrDefault();

            return objUnidadeFornecimentoSiafisico;
        }

        public IList<NaturezaDespesaEntity> ListarNaturezaDespesaSubitensMovimento(int movimentacaoID)
        {
            IList<NaturezaDespesaEntity> lstNaturezasDespesa = null;
            IQueryable<NaturezaDespesaEntity> qryConsulta = null;
            //int tipoMovID = (int)enumTipoMovimento.AquisicaoCompraEmpenho;
            int[] tiposMovimentacaoEmpenho = new int[] {  (int)enumTipoMovimento.EntradaPorEmpenho
                                                         ,(int)enumTipoMovimento.EntradaPorRestosAPagar
                                                         ,(int)enumTipoMovimento.ConsumoImediatoEmpenho
                                                         ,(int)enumTipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho
                                                       };

            qryConsulta = (from natDespesa in this.Db.TB_NATUREZA_DESPESAs
                           join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on natDespesa.TB_NATUREZA_DESPESA_ID equals subItemMaterial.TB_NATUREZA_DESPESA_ID into _left_subItemMaterial

                           from _subItemMaterial in _left_subItemMaterial.DefaultIfEmpty()
                           join movItem in this.Db.TB_MOVIMENTO_ITEMs on _subItemMaterial.TB_SUBITEM_MATERIAL_ID equals movItem.TB_SUBITEM_MATERIAL_ID into _left_movItem

                           from _movItem in _left_movItem.DefaultIfEmpty()
                           join movimentacao in this.Db.TB_MOVIMENTOs on _movItem.TB_MOVIMENTO_ID equals movimentacao.TB_MOVIMENTO_ID into _left_movimentacao

                           from _movimentacao in _left_movimentacao.DefaultIfEmpty()
                           join tipoMov in this.Db.TB_TIPO_MOVIMENTOs on _movimentacao.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov

                           where (_movimentacao.TB_MOVIMENTO_ID == movimentacaoID)
                           //where (_movimentacao.TB_MOVIMENTO_ATIVO == true)
                           //where (_movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovID)
                           //where (tiposMovimentacaoEmpenho.Contains(_movimentacao.TB_TIPO_MOVIMENTO_ID))
                           select new Sam.Domain.Entity.NaturezaDespesaEntity
                           {
                               Id = natDespesa.TB_NATUREZA_DESPESA_ID,
                               Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                               Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO,
                               CodigoDescricao = String.Format("{0} - {1}", natDespesa.TB_NATUREZA_DESPESA_CODIGO, natDespesa.TB_NATUREZA_DESPESA_DESCRICAO),
                               Natureza = natDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                           }).AsQueryable();


            qryConsulta = qryConsulta.DistinctBy(_natDespesa => _natDespesa.Codigo)
                                     .AsQueryable();

            lstNaturezasDespesa = qryConsulta.ToList();

            return lstNaturezasDespesa;
        }

        public IList<string> ListarNaturezaDespesaSubitensMovimento(int movimentacaoID, out bool unicaNaturezaDespesa)
        {
            IList<string> lstNaturezasDespesa = null;
            IQueryable<string> qryConsulta = null;
            //int tipoMovID = (int)enumTipoMovimento.AquisicaoCompraEmpenho;
            int tipoMovID = (int)enumTipoMovimento.EntradaPorEmpenho;

            qryConsulta = (from natDespesa in this.Db.TB_NATUREZA_DESPESAs
                           join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on natDespesa.TB_NATUREZA_DESPESA_ID equals subItemMaterial.TB_NATUREZA_DESPESA_ID into _left_subItemMaterial

                           from _subItemMaterial in _left_subItemMaterial.DefaultIfEmpty()
                           join movItem in this.Db.TB_MOVIMENTO_ITEMs on _subItemMaterial.TB_SUBITEM_MATERIAL_ID equals movItem.TB_SUBITEM_MATERIAL_ID into _left_movItem

                           from _movItem in _left_movItem.DefaultIfEmpty()
                           join movimentacao in this.Db.TB_MOVIMENTOs on _movItem.TB_MOVIMENTO_ID equals movimentacao.TB_MOVIMENTO_ID into _left_movimentacao

                           from _movimentacao in _left_movimentacao.DefaultIfEmpty()
                           join tipoMov in this.Db.TB_TIPO_MOVIMENTOs on _movimentacao.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov

                           where (_movimentacao.TB_MOVIMENTO_ID == movimentacaoID)
                           where (_movimentacao.TB_MOVIMENTO_ATIVO == true)
                           where (_movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovID)
                           select
                                String.Format("{0} - {1}", natDespesa.TB_NATUREZA_DESPESA_CODIGO, natDespesa.TB_NATUREZA_DESPESA_DESCRICAO)
                           ).AsQueryable();


            qryConsulta = qryConsulta.Distinct()
                                     .AsQueryable();

            lstNaturezasDespesa = qryConsulta.ToList();
            unicaNaturezaDespesa = (lstNaturezasDespesa.Count == 1); //se possui apenas uma única NatDespesa, esta é a (provável) NatDespesa do empenho.

            return lstNaturezasDespesa;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereLiquidadoOuNao;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal;
            Func<TB_MOVIMENTO, MovimentoEntity> _seletor = null;


            //filtro básico
            //expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.AquisicaoCompraEmpenho
            expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.EntradaPorEmpenho
                                               && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                               && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            expWhere = expWherePrincipal;

            if (fornecedorID > 0) //se fornecedor for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_FORNECEDOR_ID == fornecedorID));

            if (!String.IsNullOrWhiteSpace(anoMesRef)) //MesRef
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef));

            if (!String.IsNullOrWhiteSpace(codigoEmpenho)) //se empenho for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho));


            //Retornar liquidados ou não
            if (empenhosNaoLiquidados)
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty);
            else
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty);


            #region Consulta Refactored
            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable()
                                       .Where(expWhere)
                                       .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                       .Where(expWhereLiquidadoOuNao)
                                       .Select(_movItem => _movItem.TB_MOVIMENTO)
                                       .AsQueryable();

            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion Consulta Refactored

            qryConsulta = qryConsulta.OrderBy(_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO);

            var isEmpenho = true;
            _seletor = _instanciadorDTOMovimentacoes(isEmpenho);
            lstMovimentacoes = qryConsulta.DistinctBy(_seletorMov => _seletorMov.TB_MOVIMENTO_ID)
                                          .Select(_seletor)
                                          .ToList();

            lstMovimentacoes.ToList().ForEach(_movimentacao =>
            {
                //TODO RETIRAR
                _movimentacao.NaturezaDespesaEmpenho = _movimentacao.NaturezaDespesaEmpenho.BreakLine(0);

                if (!empenhosNaoLiquidados && !String.IsNullOrWhiteSpace(_movimentacao.Observacoes) && (_movimentacao.Observacoes.Length > 77))
                    _movimentacao.Observacoes = _movimentacao.Observacoes.Substring(0, 77);

                //_movimentacao.NaturezaDespesaEmpenho = _movimentacao.NaturezaDespesaEmpenho.Split(new char[] { '-', ' ' })[0];
            });

            return lstMovimentacoes;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenhoAgrupadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereLiquidadoOuNao;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal;
            Func<TB_MOVIMENTO, MovimentoEntity> _seletor = null;


            //filtro básico
            //expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.AquisicaoCompraEmpenho
            expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.EntradaPorEmpenho
                                               && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                               && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            expWhere = expWherePrincipal;

            if (fornecedorID > 0) //se fornecedor for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_FORNECEDOR_ID == fornecedorID));

            if (!String.IsNullOrWhiteSpace(anoMesRef)) //MesRef
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef));

            if (!String.IsNullOrWhiteSpace(codigoEmpenho)) //se empenho for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho));


            //Retornar liquidados ou não
            if (empenhosNaoLiquidados)
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty);
            else
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty);


            #region Consulta Refactored
            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable()
                                               .Where(expWhere)
                                               .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                               .Where(expWhereLiquidadoOuNao)
                                               .Select(_movItem => _movItem.TB_MOVIMENTO)
                                               .DistinctBy(movEmpenho => movEmpenho.TB_MOVIMENTO_ID)
                                               .ToList()
                                               .AsQueryable();

            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion Consulta Refactored

            qryConsulta = qryConsulta.OrderBy(_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO);

            var isEmpenho = true;
            _seletor = _instanciadorDTOMovimentacoes(isEmpenho);
            lstMovimentacoes = qryConsulta.DistinctBy(_seletorMov => _seletorMov.TB_MOVIMENTO_ID)
                                          .Select(_seletor)
                                          .ToList();

            lstMovimentacoes.ToList().ForEach(_movimentacao =>
            {
                _movimentacao.NaturezaDespesaEmpenho = _movimentacao.NaturezaDespesaEmpenho.BreakLine(0);
                _movimentacao.DataMovimento = null;
                _movimentacao.NumeroDocumento = String.Join(" <br> ", this.ListarDocumentosEmpenho(almoxID, fornecedorID, anoMesRef, _movimentacao.Empenho, empenhosNaoLiquidados));

                _movimentacao.ValorDocumento = lstMovimentacoes.Where(__movimentacao => __movimentacao.Empenho == _movimentacao.Empenho).Sum(__movimentacao => __movimentacao.ValorDocumento).Value;
            });

            lstMovimentacoes = lstMovimentacoes.DistinctBy(movEmpenho => movEmpenho.NumeroDocumento).ToListNoLock();

            return lstMovimentacoes;
        }

        public IList<string> ListarDocumentosEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            List<string> lstDocumentos = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereLiquidadoOuNao;


            //filtro básico
            //expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.AquisicaoCompraEmpenho
            expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.EntradaPorEmpenho
                                               && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                               && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            expWhere = expWherePrincipal;

            if (fornecedorID > 0) //se fornecedor for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_FORNECEDOR_ID == fornecedorID));

            if (!String.IsNullOrWhiteSpace(anoMesRef)) //MesRef
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef));

            if (!String.IsNullOrWhiteSpace(codigoEmpenho)) //se empenho for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho));

            if (!String.IsNullOrWhiteSpace(codigoEmpenho))
                expWhere = expWhere.And(_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho);

            //Retornar liquidados ou não
            if (empenhosNaoLiquidados)
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO == string.Empty);
            else
                expWhereLiquidadoOuNao = (_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                  || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty);


            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            qryConsulta = qryConsulta.OrderBy(_movimentacao => _movimentacao.TB_MOVIMENTO_ID);

            lstDocumentos = qryConsulta.Where(expWhere)
                                       .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                       .Where(expWhereLiquidadoOuNao)
                                       .Select(_movItem => _movItem.TB_MOVIMENTO)
                                       .DistinctBy(_movimentacaoEmpenho => _movimentacaoEmpenho.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                       .Select(mov => mov.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                       .ToList();

            return lstDocumentos;
        }
        public IList<string> ListarEmpenhosFornecedor(int almoxID, int fornecedorID, string anoMesRef, bool empenhosNaoLiquidados = true)
        {
            List<string> lstEmpenhos = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;

            //expWhere = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.AquisicaoCompraEmpenho
            expWhere = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == (int)enumTipoMovimento.EntradaPorEmpenho
                                      && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                      && _movimentacao.TB_FORNECEDOR_ID == fornecedorID
                                      && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                      && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            //if (empenhosNaoLiquidados)
            //    expWhere = (_movimentacao => _movimentacao.TB_ALMOXARIFADO_ID == almoxID
            //                                && _movimentacao.TB_FORNECEDOR_ID == fornecedorID
            //                                && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
            //                                && _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho
            //                                && _movimentacao.TB_MOVIMENTO_ATIVO == true);
            //                                //&& _movimentacao.TB_MOVIMENTO_LIQUIDADO_SIAFEM == true);


            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            qryConsulta = qryConsulta.OrderBy(_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO);

            lstEmpenhos = qryConsulta.Where(expWhere)
                                          .Select(_movimentacaoEmpenho => _movimentacaoEmpenho.TB_MOVIMENTO_EMPENHO)
                                          .Distinct()
                                          .ToList();

            return lstEmpenhos;
        }

        public IList<string> ObterNLsPagamentoEmpenho(int almoxID, string anoMesRef, string codigoEmpenho, bool detalharNLs = false)
        {
            List<string> lstDocumentos = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereMovItemLiquidado;


            //filtro básico
            //var tipoMovimento = (int)enumTipoMovimento.AquisicaoCompraEmpenho;
            var tipoMovimento = (int)enumTipoMovimento.EntradaPorEmpenho;
            expWherePrincipal = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovimento
                                               && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                               && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            expWhere = expWherePrincipal;

            if (!String.IsNullOrWhiteSpace(anoMesRef)) //MesRef
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef));

            if (!String.IsNullOrWhiteSpace(codigoEmpenho)) //se empenho for informado
                expWhere = LambaExpressionHelper.And(expWherePrincipal, (_movimentacao => _movimentacao.TB_MOVIMENTO_EMPENHO == codigoEmpenho));

            //Retornar liquidados
            expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                     || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty)
                                                 && _movItem.TB_MOVIMENTO_ITEM_ATIVO == true));


            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            qryConsulta = qryConsulta.OrderBy(_movimentacao => _movimentacao.TB_MOVIMENTO_ID);

            Expression<Func<TB_MOVIMENTO_ITEM, string>> _seletorDisplay = null;

            if (detalharNLs)
                //se for para detalhar
                _seletorDisplay = (_movItem => String.Format("MovItemID:{0}||MovID:{1}||MovDoc:{2}||ValorMovSubitem:{3}||NL: {4}", _movItem.TB_MOVIMENTO_ITEM_ID, _movItem.TB_MOVIMENTO_ID, _movItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO, _movItem.TB_MOVIMENTO_ITEM_VALOR_MOV, _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO));
            else
                //se não for detalhar
                _seletorDisplay = (_movItem => String.Format("MovDoc:{0}||ValorMov:{1}||NL: {2}", _movItem.TB_MOVIMENTO_ITEM_ID, _movItem.TB_MOVIMENTO_ID, _movItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO, _movItem.TB_MOVIMENTO_ITEM_VALOR_MOV, _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO));

            lstDocumentos = qryConsulta.Where(expWhere)
                                       .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                       .Where(expWhereMovItemLiquidado)
                                       //.Select(_movItem => String.Format("Doc: {0}||ValorMov: {3}||NL: {4}", _movItem.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO, _movItem.TB_MOVIMENTO.TB_MOVIMENTO_VALOR_DOCUMENTO, _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO))
                                       .Select(_seletorDisplay)
                                       .ToList();

            lstDocumentos = lstDocumentos.Distinct().ToList();

            return lstDocumentos;
        }

        public IList<string> ObterNLsPagamentoMovimentoEmpenho(int movimentacaoEmpenhoID)
        {
            List<string> notasLancamentoEmpenho = null;

            IQueryable<TB_MOVIMENTO> qryConsulta = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWhere;
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> expWhereMovItemLiquidado;


            //filtro básico
            expWhere = (_movimentacao => _movimentacao.TB_MOVIMENTO_ID == movimentacaoEmpenhoID
                                               && _movimentacao.TB_MOVIMENTO_ATIVO == true);
            //Retornar liquidados
            expWhereMovItemLiquidado = (_movItem => ((_movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != null
                                                     || _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO != string.Empty)
                                                 && _movItem.TB_MOVIMENTO_ITEM_ATIVO == true));


            qryConsulta = (from Movimentacao in this.Db.TB_MOVIMENTOs
                           select Movimentacao).AsQueryable<TB_MOVIMENTO>();

            notasLancamentoEmpenho = qryConsulta.Where(expWhere)
                                       .SelectMany(movimento => movimento.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                       .Where(expWhereMovItemLiquidado)
                                       .Select(_movItem => _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO)
                                       .ToList();

            notasLancamentoEmpenho = notasLancamentoEmpenho.Distinct().ToList();

            return notasLancamentoEmpenho;
        }

        static internal Expression<Func<TB_MOVIMENTO, bool>> obterMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef)
        {
            //int tipoMovimento = (int)enumTipoMovimento.AquisicaoCompraEmpenho;
            int tipoMovimento = (int)enumTipoMovimento.EntradaPorEmpenho;

            Expression<Func<TB_MOVIMENTO, bool>> expWhereRetorno = (_movimentacao => _movimentacao.TB_TIPO_MOVIMENTO_ID == tipoMovimento
                                                                                  && _movimentacao.TB_ALMOXARIFADO_ID == almoxID
                                                                                  && _movimentacao.TB_FORNECEDOR_ID == fornecedorID
                                                                                  && _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef
                                                                                  && _movimentacao.TB_MOVIMENTO_ATIVO == true);

            return expWhereRetorno;
        }

        static internal int obterLoginSamID_Requisitante(string numeroDocumentoSAM, bool isRequisitanteEstornante = false)
        {
            //var tiposRequisicaoMaterial = new int[] { enumTipoMovimento.RequisicaoPendente.GetHashCode(), enumTipoMovimento.RequisicaoFinalizada.GetHashCode() };
            var tiposRequisicaoMaterial = new int[] { enumTipoMovimento.RequisicaoAprovada.GetHashCode() };
            int loginUsuarioSamID_Requisitante = -1;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal = null;
            Expression<Func<TB_MOVIMENTO, int>> expWhereComplementar = null;
            IQueryable<int> qryConsulta = null;





            expWherePrincipal = (movimentacao => tiposRequisicaoMaterial.Contains(movimentacao.TB_TIPO_MOVIMENTO_ID)
                                              && movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO == numeroDocumentoSAM);

            if (!isRequisitanteEstornante)
                expWhereComplementar = (requisicaoMaterial => (requisicaoMaterial.TB_LOGIN_ID.HasValue ? requisicaoMaterial.TB_LOGIN_ID.Value : 0));
            else
                expWhereComplementar = (requisicaoMaterial => (requisicaoMaterial.TB_LOGIN_ID_ESTORNO.HasValue ? requisicaoMaterial.TB_LOGIN_ID_ESTORNO.Value : 0));



            var infraTabelaMovimentos = new MovimentoItemInfrastructure();
            var contextoDB = infraTabelaMovimentos.Db;
            qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                  .Select(expWhereComplementar)
                                                  .AsQueryable();

            var _qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                  //.Select(expWhereComplementar)
                                                  .AsQueryable();

            infraTabelaMovimentos = null;
            contextoDB = null;
            loginUsuarioSamID_Requisitante = qryConsulta.FirstOrDefault();
            return loginUsuarioSamID_Requisitante;
        }

        static internal Func<TB_MOVIMENTO, dtoWsMovimentacaoMaterial> _instanciadorDtoWsMovimentacaoMaterial()
        {
            Func<TB_MOVIMENTO, dtoWsMovimentacaoMaterial> _actionSeletor = null;

            var tiposMovimentacaoMaterialTransferencia = new int[] { TipoMovimento.EntradaPorTransferencia.GetHashCode(), TipoMovimento.SaidaPorTransferencia.GetHashCode() };
            var tiposMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoAprovada.GetHashCode() };
            var tiposMovimentacaoMaterialConsumoImediato = new int[] { TipoMovimento.ConsumoImediatoEmpenho.GetHashCode(), TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho.GetHashCode() };
            TB_MOVIMENTO movRequisicaoMaterialOriginal = null;

            _actionSeletor = (rowTabela =>
                                            {
                                                if (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
                                                    movRequisicaoMaterialOriginal = obterMovimentacaoOriginalRequisicaoMaterial(rowTabela);

                                                return new dtoWsMovimentacaoMaterial()
                                                {
                                                    Almoxarifado = ((!tiposMovimentacaoMaterialTransferencia.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID)) ? (BaseInfraestructure.ObterRegistro<AlmoxarifadoEntity>(rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, false).CodigoDescricao) : null),
                                                    AlmoxarifadoOrigem = obterAlmoxarifadoOuUgeTransferencia(rowTabela),
                                                    AlmoxarifadoDestino = obterAlmoxarifadoOuUgeTransferencia(rowTabela, false),
                                                    UgeOrigem = obterAlmoxarifadoOuUgeTransferencia(rowTabela, true, true),
                                                    UgeDestino = obterAlmoxarifadoOuUgeTransferencia(rowTabela, false, true),

                                                    NumeroEmpenho = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_EMPENHO) ? rowTabela.TB_MOVIMENTO_EMPENHO : null),
                                                    NumeroDocumento = (!String.IsNullOrWhiteSpace(rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO) ? rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO : null),
                                                    TipoMovimentacao = rowTabela.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,


                                                    UsuarioSamID = ((rowTabela.TB_LOGIN_ID.HasValue && rowTabela.TB_LOGIN_ID.Value != 0) ? rowTabela.TB_LOGIN_ID.Value.ToString() : null),
                                                    UsuarioSamIDEstorno = ((rowTabela.TB_LOGIN_ID_ESTORNO.HasValue && rowTabela.TB_LOGIN_ID_ESTORNO.Value != 0) ? rowTabela.TB_LOGIN_ID_ESTORNO.Value.ToString() : null),
                                                    RequisitanteSamID = ((movRequisicaoMaterialOriginal.IsNotNull() && movRequisicaoMaterialOriginal.TB_LOGIN_ID.HasValue && movRequisicaoMaterialOriginal.TB_LOGIN_ID.Value != 0) ? movRequisicaoMaterialOriginal.TB_LOGIN_ID.Value.ToString() : null),
                                                    RequisitanteSamIDEstorno = ((movRequisicaoMaterialOriginal.IsNotNull() && movRequisicaoMaterialOriginal.TB_LOGIN_ID_ESTORNO.HasValue && movRequisicaoMaterialOriginal.TB_LOGIN_ID.Value == 0) ? movRequisicaoMaterialOriginal.TB_LOGIN_ID_ESTORNO.Value.ToString() : null),



                                                    DataEmissao = ((rowTabela.TB_MOVIMENTO_DATA_DOCUMENTO != DateTime.MinValue) ? rowTabela.TB_MOVIMENTO_DATA_DOCUMENTO : null as DateTime?),
                                                    DataMovimento = ((rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO != DateTime.MinValue) ? rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO : null as DateTime?),
                                                    DataRecebimento = ((rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO != DateTime.MinValue) ? rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO : null as DateTime?),
                                                    ValorTotalMovimentacaoMaterial = ((rowTabela.TB_MOVIMENTO_VALOR_DOCUMENTO != 0.00m) ? rowTabela.TB_MOVIMENTO_VALOR_DOCUMENTO.Truncar(2) : null as Decimal?),
                                                    CPF_CNPJ_Fornecedor = ((rowTabela.TB_FORNECEDOR.IsNotNull()) ? (BaseInfraestructure.ObterRegistro<FornecedorEntity>(rowTabela.TB_FORNECEDOR_ID.Value, false).CodigoDescricao) : null),

                                                    UA = (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID) ? ((rowTabela.TB_DIVISAO_ID.HasValue && rowTabela.TB_DIVISAO_ID.Value != 0) ? String.Format("{0} - {1}", rowTabela.TB_DIVISAO.TB_UA.TB_UA_CODIGO.ToString("D5"), rowTabela.TB_DIVISAO.TB_UA.TB_UA_DESCRICAO) : null) //Se for RequisicaoMaterial
                                                                                                                                                                                                                   : ((tiposMovimentacaoMaterialConsumoImediato.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID) ? ((rowTabela.TB_UA_ID.HasValue && rowTabela.TB_UA_ID.Value != 0) ? (BaseInfraestructure.ObterRegistro<UAEntity>(rowTabela.TB_UA_ID.Value, false).CodigoDescricao) : null) : null))),  //Se for ConsumoImediato

                                                    UGE = ((rowTabela.TB_UGE_ID.HasValue && rowTabela.TB_UGE_ID.Value != 0) ? (BaseInfraestructure.ObterRegistro<UGEEntity>(rowTabela.TB_UGE_ID.Value, false).CodigoDescricao) : null),

                                                    //MovimentoItem = rowTabela.TB_MOVIMENTO_ITEMs.OrderBy(itemMovimentacaoMaterial => itemMovimentacaoMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO)
                                                    MovimentoItem = MovimentoItemInfrastructure.processarItensMovimentacao(rowTabela)
                                                                                               .OrderBy(itemMovimentacaoMaterial => itemMovimentacaoMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO)
                                                                                               .Select(MovimentoItemInfrastructure._instanciadorDtoWsItemMovimentacaoMaterial())
                                                                                               .ToList()
                                                };
                                            });

            return _actionSeletor;
        }

        static internal string obterAlmoxarifadoOuUgeTransferencia(TB_MOVIMENTO rowTabela, bool almoxarifadoOrigem = true, bool retornaDadosUGE = false)
        {
            var tiposMovimentacaoMaterialTransferencia = new int[] { TipoMovimento.EntradaPorTransferencia.GetHashCode(), TipoMovimento.SaidaPorTransferencia.GetHashCode() };
            int almoxTransferenciaID = -1;
            string codigoNomeAlmoxarifado = null;
            IQueryable<TB_ALMOXARIFADO> qryConsulta = null;
            Func<TB_ALMOXARIFADO, string> fRetornoConsulta = null;


            if (tiposMovimentacaoMaterialTransferencia.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
            {
                var infraTabelaMovimentos = new MovimentoItemInfrastructure();
                var contextoDB = infraTabelaMovimentos.Db;

                if ((rowTabela.TB_TIPO_MOVIMENTO_ID == TipoMovimento.EntradaPorTransferencia.GetHashCode() && almoxarifadoOrigem) ||
                    (rowTabela.TB_TIPO_MOVIMENTO_ID == TipoMovimento.SaidaPorTransferencia.GetHashCode() && almoxarifadoOrigem))
                    almoxTransferenciaID = rowTabela.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO.Value;
                else if ((rowTabela.TB_TIPO_MOVIMENTO_ID == TipoMovimento.EntradaPorTransferencia.GetHashCode() && !almoxarifadoOrigem) ||
                         (rowTabela.TB_TIPO_MOVIMENTO_ID == TipoMovimento.SaidaPorTransferencia.GetHashCode() && almoxarifadoOrigem))
                    almoxTransferenciaID = rowTabela.TB_ALMOXARIFADO_ID;


                fRetornoConsulta = (almoxConsultado => (retornaDadosUGE ? (String.Format("{0:D5} - {1}", almoxConsultado.TB_ALMOXARIFADO_CODIGO, almoxConsultado.TB_ALMOXARIFADO_DESCRICAO)) : (String.Format("{0:D6} - {1}", almoxConsultado.TB_UGE.TB_UGE_CODIGO, almoxConsultado.TB_UGE.TB_UGE_DESCRICAO))));

                qryConsulta = contextoDB.TB_ALMOXARIFADOs.AsQueryable();
                codigoNomeAlmoxarifado = qryConsulta.Where(almoxConsultado => almoxConsultado.TB_ALMOXARIFADO_ID == almoxTransferenciaID)
                                                    .Select(fRetornoConsulta)
                                                    .FirstOrDefault();
            }


            return codigoNomeAlmoxarifado;
        }

        static internal DateTime? obterDataMovimentoOuDataDocumento(TB_MOVIMENTO rowTabela, bool retornaDataDocumento = false)
        {
            var tiposMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoFinalizada.GetHashCode() };
            DateTime? dataCorretaParaRetorno = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;

            if (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
            {
                //dataCorretaParaRetorno = new EntitySet<TB_MOVIMENTO_ITEM>();
                expWherePrincipal = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO_ID == TipoMovimento.RequisicaoAprovada.GetHashCode())
                                                        && (requisicaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO == rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                                        && requisicaoMaterial.TB_MOVIMENTO_ATIVO == true);



                var infraTabelaMovimentos = new MovimentoItemInfrastructure();
                var contextoDB = infraTabelaMovimentos.Db;
                qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();

                var _qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();

                if (qryConsulta.Count() > 0)
                    rowTabela = qryConsulta.Where(expWherePrincipal)
                                           .FirstOrDefault();

                infraTabelaMovimentos = null;
                contextoDB = null;
            }

            dataCorretaParaRetorno = (!retornaDataDocumento ? rowTabela.TB_MOVIMENTO_DATA_MOVIMENTO : rowTabela.TB_MOVIMENTO_DATA_DOCUMENTO);
            return dataCorretaParaRetorno;
        }

        static internal TB_MOVIMENTO obterMovimentacaoSaidaDeRequisicao(TB_MOVIMENTO rowTabela)
        {
            var tiposMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoFinalizada.GetHashCode() };
            TB_MOVIMENTO movRequisicaoMaterialAtendida = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;

            if (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
            {
                //dataCorretaParaRetorno = new EntitySet<TB_MOVIMENTO_ITEM>();
                expWherePrincipal = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO_ID == TipoMovimento.RequisicaoAprovada.GetHashCode())
                                                        && (requisicaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO == rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                                        && requisicaoMaterial.TB_MOVIMENTO_ATIVO == true);



                var infraTabelaMovimentos = new MovimentoItemInfrastructure();
                var contextoDB = infraTabelaMovimentos.Db;
                qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();


                if (qryConsulta.Count() > 0)
                    movRequisicaoMaterialAtendida = qryConsulta.Where(expWherePrincipal)
                                                                  .FirstOrDefault();

                infraTabelaMovimentos = null;
                contextoDB = null;
            }


            return movRequisicaoMaterialAtendida;
        }
        static internal TB_MOVIMENTO obterMovimentacaoOriginalRequisicaoMaterial(TB_MOVIMENTO rowTabela)
        {
            var tiposMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoAprovada.GetHashCode() };
            TB_MOVIMENTO movRequisicaoMaterialOriginal = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;

            if (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
            {
                //dataCorretaParaRetorno = new EntitySet<TB_MOVIMENTO_ITEM>();
                //expWherePrincipal = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO_ID == TipoMovimento.RequisicaoAprovada.GetHashCode())
                expWherePrincipal = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO_ID == TipoMovimento.RequisicaoFinalizada.GetHashCode())
                                                        && (requisicaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO == rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                                        && requisicaoMaterial.TB_MOVIMENTO_ATIVO == true);



                var infraTabelaMovimentos = new MovimentoItemInfrastructure();
                var contextoDB = infraTabelaMovimentos.Db;
                qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();


                if (qryConsulta.Count() > 0)
                    movRequisicaoMaterialOriginal = qryConsulta.Where(expWherePrincipal)
                                                                  .FirstOrDefault();

                infraTabelaMovimentos = null;
                contextoDB = null;
            }


            return movRequisicaoMaterialOriginal;
        }

        static internal decimal? obterValorDocumentoMovimentacao(TB_MOVIMENTO rowTabela)
        {
            var tiposMovimentacaoRequisicaoMaterial = new int[] { TipoMovimento.RequisicaoFinalizada.GetHashCode() };
            decimal? valorDocumentoMovimentacao = null;
            Expression<Func<TB_MOVIMENTO, bool>> expWherePrincipal = null;
            IQueryable<TB_MOVIMENTO> qryConsulta = null;

            if (tiposMovimentacaoRequisicaoMaterial.Contains(rowTabela.TB_TIPO_MOVIMENTO_ID))
            {
                //dataCorretaParaRetorno = new EntitySet<TB_MOVIMENTO_ITEM>();
                expWherePrincipal = (requisicaoMaterial => (requisicaoMaterial.TB_TIPO_MOVIMENTO_ID == TipoMovimento.RequisicaoAprovada.GetHashCode())
                                                        && (requisicaoMaterial.TB_MOVIMENTO_NUMERO_DOCUMENTO == rowTabela.TB_MOVIMENTO_NUMERO_DOCUMENTO)
                                                        && requisicaoMaterial.TB_MOVIMENTO_ATIVO == true);



                var infraTabelaMovimentos = new MovimentoItemInfrastructure();
                var contextoDB = infraTabelaMovimentos.Db;
                qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();

                var _qryConsulta = contextoDB.TB_MOVIMENTOs.Where(expWherePrincipal)
                                                      .AsQueryable();

                if (qryConsulta.Count() > 0)
                    rowTabela = qryConsulta.Where(expWherePrincipal)
                                           .FirstOrDefault();

                infraTabelaMovimentos = null;
                contextoDB = null;
            }

            valorDocumentoMovimentacao = rowTabela.TB_MOVIMENTO_VALOR_DOCUMENTO;
            return valorDocumentoMovimentacao;
        }

        private Func<TB_MOVIMENTO, MovimentoEntity> _instanciadorDTOMovimentacoes(bool isEmpenho = false)
        {
            var _unicaNaturezaDespesa = false;

            Func<TB_MOVIMENTO, MovimentoEntity> _actionSeletor = null;

            if (isEmpenho)
            {
                _actionSeletor = _movimentacaoEmpenho => new MovimentoEntity()
                {
                    Id = _movimentacaoEmpenho.TB_MOVIMENTO_ID,
                    Almoxarifado = ((_movimentacaoEmpenho.TB_ALMOXARIFADO.IsNotNull()) ? (new AlmoxarifadoEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, MesRef = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF, Uge = new UGEEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID, Codigo = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_DESCRICAO }, Gestor = new GestorEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO } }) : null),
                    AnoMesReferencia = _movimentacaoEmpenho.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                    Ativo = _movimentacaoEmpenho.TB_MOVIMENTO_ATIVO,
                    DataMovimento = _movimentacaoEmpenho.TB_MOVIMENTO_DATA_MOVIMENTO,
                    Empenho = _movimentacaoEmpenho.TB_MOVIMENTO_EMPENHO,
                    //Inserir validaçãop contra null do banco.
                    EmpenhoEvento = ((_movimentacaoEmpenho.TB_EMPENHO_EVENTO.IsNotNull()) ? (new EmpenhoEventoEntity() { Id = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID, Codigo = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO, Descricao = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO }) : null),
                    EmpenhoLicitacao = ((_movimentacaoEmpenho.TB_EMPENHO_LICITACAO.IsNotNull()) ? (new EmpenhoLicitacaoEntity() { Id = _movimentacaoEmpenho.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID, Descricao = _movimentacaoEmpenho.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO }) : null),

                    Fornecedor = ((_movimentacaoEmpenho.TB_FORNECEDOR.IsNotNull()) ? (new FornecedorEntity() { Id = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_ID, Nome = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ }) : null),
                    NumeroDocumento = _movimentacaoEmpenho.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                    Observacoes = _movimentacaoEmpenho.TB_MOVIMENTO_OBSERVACOES,
                    MovimentoItem = _movimentacaoEmpenho.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                    {
                        Id = _movItem.TB_MOVIMENTO_ITEM_ID,
                        SubItemMaterial = (_movItem.TB_SUBITEM_MATERIAL.IsNotNull() ? (new SubItemMaterialEntity()
                        {
                            Id = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                            Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                            Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                            UnidadeFornecimento = ((_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull()) ? (new UnidadeFornecimentoEntity() { Id = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO }) : null),
                            UnidadeFornecimentoSiafisico = (_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull() ? ObterUnidadeFornecimentoSiafisico(_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO) : null)
                        }) : null),
                        ItemMaterial = ((_movItem.TB_ITEM_MATERIAL.IsNotNull()) ? (new ItemMaterialEntity() { Id = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID, Codigo = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO, Descricao = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO }) : null),

                        PrecoUnit = _movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                        SaldoQtde = _movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                        QtdeMov = _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                        QtdeLiq = _movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                        ValorMov = (_movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT * _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV),

                        NL_Liquidacao = _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                        NL_LiquidacaoEstorno = _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO,
                        NL_Reclassificacao = _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO,
                        NL_ReclassificacaoEstorno = _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO,
                        //CodigoUnidadeFornecimentoSiafisico = _movItem.TB_UNIDADE_FORNECIMENTO_SIAF_CODIGO
                    })
                                                                                                                                           .ToList(),

                    //TODO: Verificar como otimizar depois!
                    ValorDocumento = _movimentacaoEmpenho.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                    {
                        ValorMov = (_movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT * _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV) ?? 0
                    })
                                                                                                                                            .Sum(movItem => ((movItem.ValorMov.HasValue) ? movItem.ValorMov : 0.00m)),

                    UGE = ((_movimentacaoEmpenho.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = _movimentacaoEmpenho.TB_UGE.TB_UGE_ID, Codigo = _movimentacaoEmpenho.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacaoEmpenho.TB_UGE.TB_UGE_DESCRICAO }) : null),
                    NaturezaDespesaEmpenho = String.Join(" | ", this.ListarNaturezaDespesaSubitensMovimento(_movimentacaoEmpenho.TB_MOVIMENTO_ID, out _unicaNaturezaDespesa)),

                    TipoMovimento = ((_movimentacaoEmpenho.TB_TIPO_MOVIMENTO.IsNotNull()) ? (new TipoMovimentoEntity()
                    {
                        Id = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                        Descricao = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                        AgrupamentoId = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                        TipoAgrupamento = ((_movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.IsNotNull()) ? (new TipoMovimentoAgrupamentoEntity()
                        {
                            Id = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                            Descricao = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                        }) : null)
                    }) : null),
                    IdLogin = _movimentacaoEmpenho.TB_LOGIN_ID,
                    IdLoginEstorno = _movimentacaoEmpenho.TB_LOGIN_ID_ESTORNO,
                    InscricaoCE = _movimentacaoEmpenho.TB_MOVIMENTO_CE
                };
            }
            else
            {
                _actionSeletor = (_movimentacao => new MovimentoEntity()
                {
                    Id = _movimentacao.TB_MOVIMENTO_ID,
                    Almoxarifado = ((_movimentacao.TB_ALMOXARIFADO.IsNotNull()) ? (new AlmoxarifadoEntity() { Id = _movimentacao.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, MesRef = _movimentacao.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF, Uge = new UGEEntity() { Id = _movimentacao.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID, Codigo = _movimentacao.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacao.TB_ALMOXARIFADO.TB_UGE.TB_UGE_DESCRICAO }, Gestor = new GestorEntity() { Id = _movimentacao.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = _movimentacao.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = _movimentacao.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = _movimentacao.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO } }) : null),

                    AnoMesReferencia = _movimentacao.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                    Ativo = _movimentacao.TB_MOVIMENTO_ATIVO,
                    DataMovimento = _movimentacao.TB_MOVIMENTO_DATA_MOVIMENTO,
                    Empenho = _movimentacao.TB_MOVIMENTO_EMPENHO,

                    EmpenhoEvento = ((_movimentacao.TB_EMPENHO_EVENTO.IsNotNull()) ? (new EmpenhoEventoEntity() { Id = _movimentacao.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID, Codigo = _movimentacao.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO, Descricao = _movimentacao.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO }) : null),
                    EmpenhoLicitacao = ((_movimentacao.TB_EMPENHO_LICITACAO.IsNotNull()) ? (new EmpenhoLicitacaoEntity() { Id = _movimentacao.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID, Descricao = _movimentacao.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO }) : null),

                    Fornecedor = ((_movimentacao.TB_FORNECEDOR.IsNotNull()) ? (new FornecedorEntity() { Id = _movimentacao.TB_FORNECEDOR.TB_FORNECEDOR_ID, Nome = _movimentacao.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = _movimentacao.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ }) : null),
                    NumeroDocumento = _movimentacao.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                    Observacoes = _movimentacao.TB_MOVIMENTO_OBSERVACOES,
                    ValorDocumento = _movimentacao.TB_MOVIMENTO_VALOR_DOCUMENTO,
                    MovimentoItem = _movimentacao.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                    {
                        Id = _movItem.TB_MOVIMENTO_ITEM_ID,
                        SubItemMaterial = (_movItem.TB_SUBITEM_MATERIAL.IsNotNull() ? (new SubItemMaterialEntity()
                        {
                            Id = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                            Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                            Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                            UnidadeFornecimento = ((_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull()) ? (new UnidadeFornecimentoEntity() { Id = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO }) : null),
                            UnidadeFornecimentoSiafisico = (_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull() ? ObterUnidadeFornecimentoSiafisico(_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO) : null)
                        }) : null),

                        ItemMaterial = ((_movItem.TB_ITEM_MATERIAL.IsNotNull()) ? (new ItemMaterialEntity() { Id = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID, Codigo = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO, Descricao = _movItem.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO }) : null),
                        PrecoUnit = _movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                        SaldoQtde = _movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                        QtdeMov = _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                        ValorMov = _movItem.TB_MOVIMENTO_ITEM_VALOR_MOV
                        //QtdeMov = _movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                        //ValorMov = (_movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT * _movItem.TB_MOVIMENTO_ITEM_QTDE_LIQ)
                    }).ToList(),

                    UGE = ((_movimentacao.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = _movimentacao.TB_UGE.TB_UGE_ID, Codigo = _movimentacao.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacao.TB_UGE.TB_UGE_DESCRICAO }) : null),
                    NaturezaDespesaEmpenho = String.Join(" | ", this.ListarNaturezaDespesaSubitensMovimento(_movimentacao.TB_MOVIMENTO_ID, out _unicaNaturezaDespesa)),

                    TipoMovimento = ((_movimentacao.TB_TIPO_MOVIMENTO.IsNotNull()) ? (new TipoMovimentoEntity()
                    {
                        Id = _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                        Descricao = _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                        AgrupamentoId = _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                        TipoAgrupamento = ((_movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.IsNotNull()) ? (new TipoMovimentoAgrupamentoEntity()
                        {
                            Id = _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                            Descricao = _movimentacao.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                        }) : null)
                    }) : null),
                    IdLogin = _movimentacao.TB_LOGIN_ID,
                    IdLoginEstorno = _movimentacao.TB_LOGIN_ID_ESTORNO,
                    InscricaoCE = _movimentacao.TB_MOVIMENTO_CE
                });
            }

            return _actionSeletor;
        }

        private Func<TB_MOVIMENTO, MovimentoEntity> _instanciadorDTOMovimentacoesConsumoImediato()
        {

            Func<TB_MOVIMENTO, MovimentoEntity> _actionSeletor = null;



            _actionSeletor = _movimentacaoEmpenho => new MovimentoEntity()
            {
                Id = _movimentacaoEmpenho.TB_MOVIMENTO_ID,
                Almoxarifado = ((_movimentacaoEmpenho.TB_ALMOXARIFADO.IsNotNull()) ? (new AlmoxarifadoEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Codigo = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO, Descricao = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO, MesRef = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF, Uge = new UGEEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID, Codigo = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_UGE.TB_UGE_DESCRICAO }, Gestor = new GestorEntity() { Id = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = _movimentacaoEmpenho.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO } }) : null),
                AnoMesReferencia = _movimentacaoEmpenho.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                Ativo = _movimentacaoEmpenho.TB_MOVIMENTO_ATIVO,
                DataMovimento = _movimentacaoEmpenho.TB_MOVIMENTO_DATA_MOVIMENTO,
                Empenho = _movimentacaoEmpenho.TB_MOVIMENTO_EMPENHO,
                //Inserir validaçãoo contra null do banco.
                EmpenhoEvento = ((_movimentacaoEmpenho.TB_EMPENHO_EVENTO.IsNotNull()) ? (new EmpenhoEventoEntity() { Id = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID, Codigo = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO, Descricao = _movimentacaoEmpenho.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO }) : null),
                EmpenhoLicitacao = ((_movimentacaoEmpenho.TB_EMPENHO_LICITACAO.IsNotNull()) ? (new EmpenhoLicitacaoEntity() { Id = _movimentacaoEmpenho.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID, Descricao = _movimentacaoEmpenho.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO }) : null),

                Fornecedor = ((_movimentacaoEmpenho.TB_FORNECEDOR.IsNotNull()) ? (new FornecedorEntity() { Id = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_ID, Nome = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_NOME, CpfCnpj = _movimentacaoEmpenho.TB_FORNECEDOR.TB_FORNECEDOR_CPFCNPJ }) : null),
                NumeroDocumento = _movimentacaoEmpenho.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                Observacoes = _movimentacaoEmpenho.TB_MOVIMENTO_OBSERVACOES,
                MovimentoItem = _movimentacaoEmpenho.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                {
                    Id = _movItem.TB_MOVIMENTO_ITEM_ID,
                    SubItemMaterial = (_movItem.TB_SUBITEM_MATERIAL.IsNotNull() ? (new SubItemMaterialEntity()
                    {
                        Id = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                        Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                        Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                        UnidadeFornecimento = ((_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull()) ? (new UnidadeFornecimentoEntity() { Id = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO }) : null),
                        UnidadeFornecimentoSiafisico = (_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.IsNotNull() ? ObterUnidadeFornecimentoSiafisico(_movItem.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO) : null),
                        NaturezaDespesa = (_movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.IsNotNull() ? (new NaturezaDespesaEntity() { Id = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID, Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO, Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO }) : null)
                    }) : null),

                    PrecoUnit = _movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                    ValorMov = (_movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT * _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV),

                    NL_Liquidacao = _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                    NL_LiquidacaoEstorno = _movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO_ESTORNO,
                    NL_Reclassificacao = _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO,
                    NL_ReclassificacaoEstorno = _movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO_ESTORNO,
                }).ToList(),

                //TODO: Verificar como otimizar depois!
                ValorDocumento = _movimentacaoEmpenho.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                {
                    ValorMov = (_movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT * _movItem.TB_MOVIMENTO_ITEM_QTDE_MOV) ?? 0
                }).Sum(movItem => ((movItem.ValorMov.HasValue) ? movItem.ValorMov : 0.00m)),

                UGE = ((_movimentacaoEmpenho.TB_UGE.IsNotNull()) ? (new UGEEntity() { Id = _movimentacaoEmpenho.TB_UGE.TB_UGE_ID, Codigo = _movimentacaoEmpenho.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacaoEmpenho.TB_UGE.TB_UGE_DESCRICAO }) : null),
                UA = ((_movimentacaoEmpenho.TB_UA.IsNotNull()) ? (new UAEntity() { Id = _movimentacaoEmpenho.TB_UA.TB_UA_ID, Codigo = _movimentacaoEmpenho.TB_UA.TB_UA_CODIGO, Descricao = _movimentacaoEmpenho.TB_UA.TB_UA_DESCRICAO, Uge = new UGEEntity() { Id = _movimentacaoEmpenho.TB_UA.TB_UGE.TB_UGE_ID, Codigo = _movimentacaoEmpenho.TB_UA.TB_UGE.TB_UGE_CODIGO, Descricao = _movimentacaoEmpenho.TB_UA.TB_UGE.TB_UGE_DESCRICAO } }) : null),
                NaturezaDespesaEmpenho = this.ListarNaturezaDespesaSubitensMovimento(_movimentacaoEmpenho.TB_MOVIMENTO_ID)[0].Codigo.ToString(),
                TipoMovimento = ((_movimentacaoEmpenho.TB_TIPO_MOVIMENTO.IsNotNull()) ? (new TipoMovimentoEntity()
                {
                    Id = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                    Descricao = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                    AgrupamentoId = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                    TipoAgrupamento = ((_movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.IsNotNull()) ? (new TipoMovimentoAgrupamentoEntity()
                    {
                        Id = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                        Descricao = _movimentacaoEmpenho.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                    }) : null)
                }) : null),
                IdLogin = _movimentacaoEmpenho.TB_LOGIN_ID,
                IdLoginEstorno = _movimentacaoEmpenho.TB_LOGIN_ID_ESTORNO,
                InscricaoCE = _movimentacaoEmpenho.TB_MOVIMENTO_CE
            };

            return _actionSeletor;
        }

        private Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _instanciadorDTOUnidadeFornecimentoSiafisico(string siglaUnidadeFornecimentoSiafisico)
        {
            Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _seletor = null;

            var _siglaUnidadeFornecimentoSiafisico = siglaUnidadeFornecimentoSiafisico.Trim().ToUpperInvariant();
            var rowTabela = Db.TB_UNIDADE_FORNECIMENTO_SIAFs.Where(unidForSiaf => unidForSiaf.TB_UNIDADE_FORNECIMENTO_DESCRICAO == _siglaUnidadeFornecimentoSiafisico)
                                                             .FirstOrDefault();

            var haDados = (!String.IsNullOrWhiteSpace(siglaUnidadeFornecimentoSiafisico) && rowTabela.IsNotNull());
            _seletor = (_unidadeFornecimentoSiafisico => new UnidadeFornecimentoSiafEntity()
            {
                Codigo = rowTabela.TB_UNIDADE_FORNECIMENTO_CODIGO,
                Descricao = rowTabela.TB_UNIDADE_FORNECIMENTO_DESCRICAO
            });

            return _seletor;
        }

        private Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _instanciadorDTOUnidadeFornecimentoSiafisico(int codigoUnidadeFornecimento)
        {
            Func<TB_UNIDADE_FORNECIMENTO_SIAF, UnidadeFornecimentoSiafEntity> _seletor = null;

            var _codigoUnidadeFornecimento = codigoUnidadeFornecimento;
            var rowTabela = Db.TB_UNIDADE_FORNECIMENTO_SIAFs.Where(unidForSiaf => unidForSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO == _codigoUnidadeFornecimento)
                                                             .FirstOrDefault();

            var haDados = (codigoUnidadeFornecimento > 0 && rowTabela.IsNotNull());
            _seletor = (_unidadeFornecimentoSiafisico => new UnidadeFornecimentoSiafEntity()
            {
                Codigo = rowTabela.TB_UNIDADE_FORNECIMENTO_CODIGO,
                Descricao = rowTabela.TB_UNIDADE_FORNECIMENTO_DESCRICAO
            });

            return _seletor;
        }

        #endregion

        public void CancelarRequisicao(int idRequisicao)
        {
            try
            {
                int? usuarioLogado_ID = this.Entity.IdLogin;
                bool? bloquear = false;

                TB_MOVIMENTO mov = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == idRequisicao && a.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoPendente).FirstOrDefault();


                if (mov == null)
                    throw new Exception("Requisição não está pendente");
                else
                {
                    bloquear = mov.TB_MOVIMENTO_BLOQUEAR = mov.TB_MOVIMENTO_BLOQUEAR != null ? (bool)mov.TB_MOVIMENTO_BLOQUEAR : false;

                    if ((bool)bloquear)
                        throw new Exception("Requisição está sendo executada.");
                }


                AlterarMovimentoBloquear(idRequisicao, true);

                mov.TB_TIPO_MOVIMENTO_ID = (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada;
                mov.TB_MOVIMENTO_ATIVO = false;
                mov.TB_LOGIN_ID_ESTORNO = usuarioLogado_ID;
                mov.TB_MOVIMENTO_DATA_ESTORNO = DateTime.Now;
                mov.TB_MOVIMENTO_BLOQUEAR = false;

                this.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public IList<MovimentoEntity> ListarNFsPorEmpenho(string pStrMovimentoEmpenho)
        {

            MovimentoItemInfrastructure lObjInfrastructure = null;
            IList<MovimentoEntity> lILstMovimentos = null;
            IEnumerable<MovimentoEntity> lIEnumMovimentos = null;

            lObjInfrastructure = new MovimentoItemInfrastructure();
            lIEnumMovimentos = (from Movimento in this.Db.TB_MOVIMENTOs
                                where (Movimento.TB_MOVIMENTO_EMPENHO == pStrMovimentoEmpenho)
                                select new MovimentoEntity
                                {
                                    Id = Movimento.TB_MOVIMENTO_ID,
                                    Almoxarifado = new AlmoxarifadoEntity(Movimento.TB_ALMOXARIFADO_ID),
                                    TipoMovimento = (from TipoMovimento in Db.TB_TIPO_MOVIMENTOs
                                                     join AgrupamentoTipoMovimento in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals AgrupamentoTipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                     where TipoMovimento.TB_TIPO_MOVIMENTO_ID == Movimento.TB_TIPO_MOVIMENTO_ID
                                                     select new TipoMovimentoEntity
                                                     {
                                                         Id = TipoMovimento.TB_TIPO_MOVIMENTO_ID,
                                                         AgrupamentoId = AgrupamentoTipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                     }
                                                      ).FirstOrDefault(),
                                    GeradorDescricao = Movimento.TB_MOVIMENTO_GERADOR_DESCRICAO,
                                    NumeroDocumento = Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                    AnoMesReferencia = Movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                    DataDocumento = Movimento.TB_MOVIMENTO_DATA_DOCUMENTO,
                                    DataMovimento = Movimento.TB_MOVIMENTO_DATA_MOVIMENTO,
                                    FonteRecurso = Movimento.TB_MOVIMENTO_FONTE_RECURSO,
                                    ValorDocumento = Movimento.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                    Observacoes = Movimento.TB_MOVIMENTO_OBSERVACOES,
                                    Instrucoes = Movimento.TB_MOVIMENTO_INSTRUCOES,
                                    Empenho = Movimento.TB_MOVIMENTO_EMPENHO,
                                    MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO),
                                    Divisao = new DivisaoEntity(Movimento.TB_DIVISAO_ID),
                                    Fornecedor = new FornecedorEntity(Movimento.TB_FORNECEDOR_ID),
                                    UGE = new UGEEntity(Movimento.TB_UGE_ID),
                                    Ativo = Movimento.TB_MOVIMENTO_ATIVO,
                                    MovimentoItem = lObjInfrastructure.ListarTodosCod(Movimento.TB_MOVIMENTO_ID),
                                    EmpenhoEvento = new EmpenhoEventoEntity
                                    {
                                        Id = Movimento.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_ID,
                                        Codigo = Movimento.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_CODIGO,
                                        Descricao = Movimento.TB_EMPENHO_EVENTO.TB_EMPENHO_EVENTO_DESCRICAO
                                    },
                                    EmpenhoLicitacao = new EmpenhoLicitacaoEntity
                                    {
                                        Id = Movimento.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_ID,
                                        Descricao = Movimento.TB_EMPENHO_LICITACAO.TB_EMPENHO_LICITACAO_DESCRICAO
                                    }
                                });
            //.Where(Movimento => ((Movimento.Fornecedor != null) && (Movimento.Fornecedor.Id != null) && !String.IsNullOrWhiteSpace(Movimento.Fornecedor.Nome)))
            //.Where(Movimento => ((Movimento.Divisao != null) && (Movimento.Divisao.Id != null) && !String.IsNullOrWhiteSpace(Movimento.Divisao.Descricao)))
            //.Where(Movimento => ((Movimento.UGE != null) && (Movimento.UGE.Id != null) && !String.IsNullOrWhiteSpace(Movimento.UGE.Descricao)))
            //.Where(Movimento => ((Movimento.TipoMovimento != null) && Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho))
            //.Where(Movimento => ((Movimento.Fornecedor != null) && (Movimento.Fornecedor.Id != null) && !String.IsNullOrWhiteSpace(Movimento.Fornecedor.Nome)))
            //.Where(Movimento => Movimento.Ativo.HasValue);

            lILstMovimentos = lIEnumMovimentos.ToList();

            return lILstMovimentos;
        }

        public IList<MovimentoEntity> VerificarTransferenciasPendentes(int pIntAlmoxarifadoFechamento, int pIntMesReferencia, bool NewMethod)
        {
            List<MovimentoEntity> lLstRetorno = null;

            int lIntDiscrepancias = 0;
            string lStrSqlTracing = null;
            string lStrSqlTracingValued = null;
            IQueryable lQryRetorno = null;
            TB_ALMOXARIFADO almoxarifado = new TB_ALMOXARIFADO();

            try
            {
                lQryRetorno = (from rowMovimento in Db.TB_MOVIMENTOs
                               join rowAlmoxarifado in Db.TB_ALMOXARIFADOs on pIntAlmoxarifadoFechamento equals rowAlmoxarifado.TB_ALMOXARIFADO_ID
                               where !(from p in Db.TB_TRANSFERENCIA_PENDENTEs select p.TB_MOVIMENTO_ID).Contains(rowMovimento.TB_MOVIMENTO_ID)
                               where ((rowMovimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == pIntAlmoxarifadoFechamento))
                               where (rowMovimento.TB_MOVIMENTO_ATIVO == true)
                               where ((rowMovimento.TB_TIPO_MOVIMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia))
                               where (rowMovimento.TB_MOVIMENTO_DATA_ESTORNO == null)
                               where (rowMovimento.TB_MOVIMENTO_TRANSFERENCIA_ID == null)
                               where (Convert.ToInt32(rowMovimento.TB_MOVIMENTO_ANO_MES_REFERENCIA) == Convert.ToInt32(pIntMesReferencia.ToString()))
                               group rowMovimento by rowMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO into MovimentosAgrupados
                               let cCount = MovimentosAgrupados.Count()
                               where cCount < 2
                               select new MovimentoEntity
                               {
                                   NumeroDocumento = MovimentosAgrupados.Key
                               });

                lStrSqlTracing = lQryRetorno.ToString();
                lStrSqlTracingValued = lStrSqlTracing;

                Db.GetCommand(lQryRetorno).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSqlTracingValued = lStrSqlTracingValued.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

                lLstRetorno = lQryRetorno.Cast<MovimentoEntity>().ToList();
                lIntDiscrepancias = lQryRetorno.Cast<MovimentoEntity>().Count();

                List<MovimentoEntity> _lstTemp = new List<MovimentoEntity>();
                lLstRetorno.ForEach(Movimentacao => _lstTemp.AddRange(this.ObterMovimentoPorNumeroDocumento(Movimentacao.NumeroDocumento, pIntAlmoxarifadoFechamento, 12)));
                lLstRetorno = _lstTemp;
                _lstTemp.ForEach(Movimentacao => { if (String.IsNullOrWhiteSpace(Movimentacao.Observacoes)) { lLstRetorno.Remove(Movimentacao); } });
            }
            catch (Exception lExcExcecao)
            {
                throw new Exception(String.Format("Erro ao executar instrução SQL {0}", lStrSqlTracingValued), lExcExcecao);
            }

            return lLstRetorno;

        }

        private Int32 getMesReferenciaInicial(int pIntAlmoxarifadoFechamento)
        {
            IQueryable lQryRetorno = null;
            lQryRetorno = (from amoxarifado in Db.TB_ALMOXARIFADOs where amoxarifado.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoFechamento select amoxarifado.TB_ALMOXARIFADO_MES_REF_INICIAL);

            var retorno = Db.GetCommand(lQryRetorno);

            try
            {
                retorno.Connection = Db.Connection;
                retorno.Connection.Open();
                return Convert.ToInt32(retorno.ExecuteScalar().ToString());


            }
            catch
            {
                return 0;

            }
            finally
            {
                retorno.Connection.Close();
                retorno.Connection = null;
            }


        }

        /// <summary>
        /// RETORNA APENAS OS REGISTROS DE UM DETERMINADO TIPO DE MOVIMENTO
        /// </summary>
        /// <param name="pStrNumeroDocumento"></param>
        /// <param name="pIntAlmoxarifado_ID"></param>
        /// <param name="pIntTipoMovimento"></param>
        /// <returns></returns>
        public IList<MovimentoEntity> ObterMovimentoPorNumeroDocumento(string strNumeroDocumento, int almoxID, int tipoMovimentoID)
        {
            List<MovimentoEntity> lstRetorno = null;

            int numDivergencias = 0;
            int movAuxiliarID = 0;
            string strSQL = null;
            string nomeTipoMovimentacao = null;
            IQueryable qryConsulta = null;
            AlmoxarifadoInfraestructure infraAlmox = null;
            AlmoxarifadoEntity objEntidade = null;


            try
            {

                infraAlmox = new AlmoxarifadoInfraestructure();
                qryConsulta = (from Movimento in Db.TB_MOVIMENTOs
                               where (Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO == strNumeroDocumento)
                               where (Movimento.TB_TIPO_MOVIMENTO_ID == tipoMovimentoID)
                               select new MovimentoEntity
                               {
                                   Id = Movimento.TB_MOVIMENTO_ID,
                                   NumeroDocumento = Movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                   TipoMovimento = new TipoMovimentoEntity(Movimento.TB_TIPO_MOVIMENTO_ID) { Descricao = Movimento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO },
                                   Almoxarifado = infraAlmox.ObterAlmoxarifado(Movimento.TB_ALMOXARIFADO_ID),
                                   MovimAlmoxOrigemDestino = infraAlmox.ObterAlmoxarifado(Movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO.Value),
                                   DataMovimento = Movimento.TB_MOVIMENTO_DATA_MOVIMENTO,
                               });


                strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
                lstRetorno = qryConsulta.Cast<MovimentoEntity>().ToList();


                foreach (var objMovimento in lstRetorno)
                {
                    //if ((objMovimento.Almoxarifado.Id != almoxID) && (objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorTransferencia))
                    if ((objMovimento.Almoxarifado.Id != almoxID) && ((objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorTransferencia) || (objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorDoacao)))
                    {
                        if (tipoMovimentoID == (int)enumTipoMovimento.SaidaPorTransferencia)
                        {
                            nomeTipoMovimentacao = "Transferência";
                            movAuxiliarID = (int)enumTipoMovimento.EntradaPorTransferencia;
                        }
                        else if (tipoMovimentoID == (int)enumTipoMovimento.SaidaPorDoacao)
                        {
                            nomeTipoMovimentacao = "Doação";
                            movAuxiliarID = (int)enumTipoMovimento.EntradaPorDoacaoImplantado;
                        }


                        objEntidade = objMovimento.MovimAlmoxOrigemDestino;
                        objMovimento.MovimAlmoxOrigemDestino = objMovimento.Almoxarifado;
                        objMovimento.Almoxarifado = objEntidade;

                        objMovimento.TipoMovimento = new TipoMovimentoEntity(movAuxiliarID) { Descricao = String.Format("Entrada por {0}", nomeTipoMovimentacao) };
                    }


                    objMovimento.NumeroDocumento = String.Format("{0}/{1}", objMovimento.NumeroDocumento.Substring(0, 4), objMovimento.NumeroDocumento.Substring(4));

                    nomeTipoMovimentacao = nomeTipoMovimentacao.ToLowerInvariant();
                    if ((objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorTransferencia) || (objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.SaidaPorDoacao))
                        //objMovimento.Observacoes = String.Format("Foi realizada uma saída por transferência na data de {0} para o Almoxarifado {1}, que ainda não foi recebida no destino.", objMovimento.DataMovimento.Value.ToString("dd/MM/yyyy"), objMovimento.MovimAlmoxOrigemDestino.Descricao);
                        objMovimento.Observacoes = String.Format("Foi realizada uma saída por {0} na data de {1} para o Almoxarifado {2}, que ainda não foi recebida no destino.", nomeTipoMovimentacao, objMovimento.DataMovimento.Value.ToString(base.fmtDataFormatoBrasileiro), objMovimento.MovimAlmoxOrigemDestino.Descricao);
                    else if ((objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.EntradaPorTransferencia) || (objMovimento.TipoMovimento.Id == (int)enumTipoMovimento.EntradaPorDoacaoImplantado))
                        //objMovimento.Observacoes = String.Format("Existe uma transferência pendente do Almoxarifado {0}, documento {1}, realizada na data de {2} destinada a esse almoxarifado que ainda não foi recebida.", objMovimento.MovimAlmoxOrigemDestino.Descricao, objMovimento.NumeroDocumento, objMovimento.DataMovimento.Value.ToString("dd/MM/yyyy"));
                        objMovimento.Observacoes = String.Format("Existe uma {0} pendente do Almoxarifado {1}, documento {2}, realizada na data de {3} destinada a esse almoxarifado que ainda não foi recebida.", nomeTipoMovimentacao, objMovimento.MovimAlmoxOrigemDestino.Descricao, objMovimento.NumeroDocumento, objMovimento.DataMovimento.Value.ToString("dd/MM/yyyy"));

                }
            }
            catch (Exception excErroExecucao)
            {
                throw new Exception(String.Format("Erro ao executar instrução SQL {0}", strSQL), excErroExecucao);
            }

            return lstRetorno;

        }

        public void AtualizaTransferencia(int? movimentoSaidaId, int? movimentoEntradaId, bool isEstorno)
        {
            TB_MOVIMENTO mov = new TB_MOVIMENTO();
            TB_MOVIMENTO movimentoItem;

            if (isEstorno == false)
            {
                movimentoItem = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_ID == (int)movimentoSaidaId).FirstOrDefault();

                if (movimentoItem != null)
                    movimentoItem.TB_MOVIMENTO_TRANSFERENCIA_ID = movimentoEntradaId;
            }
            else
            {
                movimentoItem = this.Db.TB_MOVIMENTOs.Where(a => a.TB_MOVIMENTO_TRANSFERENCIA_ID == (int)movimentoEntradaId).FirstOrDefault();

                if (movimentoItem != null)
                    movimentoItem.TB_MOVIMENTO_TRANSFERENCIA_ID = null;
            }
            Db.SubmitChanges();
        }

        public void AtualizaMovimentacaoCampoCE(int movimentacaoMaterialID, string valorCampoCE)
        {
            TB_MOVIMENTO rowTabela = null;

            try
            {
                rowTabela = this.Db.TB_MOVIMENTOs.Where(movMaterial => movMaterial.TB_MOVIMENTO_ID == movimentacaoMaterialID).FirstOrDefault();
                if (rowTabela.IsNotNull())
                    rowTabela.TB_MOVIMENTO_CE = valorCampoCE;

                Db.SubmitChanges();
            }
            catch (Exception excErroExecucao)
            {
                throw new Exception(String.Format("Erro ao atualizar campo CE. Excecao: {0} ", excErroExecucao.Message));
            }

        }

        /// <summary>
        /// Verifica se o movimento irá modificar o preço Medio do SubItem
        /// </summary>
        /// <param name="movItem"></param>
        /// <returns></returns>
        public bool VerificaMovimentoModificaPrecoMedio(MovimentoItemEntity movItem, bool isEstorno)
        {
            decimal? pu = movItem.QtdeMov > 0 ? Convert.ToDecimal((movItem.ValorMov / movItem.QtdeMov)).truncarQuatroCasas() : movItem.QtdeMov;

            IQueryable<MovimentoItemEntity> result = (from mov in Db.TB_MOVIMENTOs
                                                      join item in Db.TB_MOVIMENTO_ITEMs on mov.TB_MOVIMENTO_ID equals item.TB_MOVIMENTO_ID
                                                      where mov.TB_ALMOXARIFADO_ID == movItem.Movimento.Almoxarifado.Id
                                                      where item.TB_SUBITEM_MATERIAL_ID == movItem.SubItemMaterial.Id
                                                      where mov.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == ((int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                      where mov.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID == ((int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia)
                                                      where mov.TB_MOVIMENTO_TRANSFERENCIA_ID != null
                                                      where mov.TB_MOVIMENTO_ATIVO == true
                                                      where item.TB_MOVIMENTO_ITEM_ATIVO == true
                                                      select new MovimentoItemEntity
                                                      {
                                                          Id = item.TB_MOVIMENTO_ITEM_ID,
                                                          SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                          SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                          PrecoMedio = item.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                          ValorMov = item.TB_MOVIMENTO_ITEM_VALOR_MOV,
                                                          QtdeMov = item.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                          Movimento = new MovimentoEntity
                                                          {
                                                              Id = mov.TB_MOVIMENTO_ID,
                                                              Almoxarifado = new AlmoxarifadoEntity(mov.TB_ALMOXARIFADO_ID),
                                                              NumeroDocumento = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                              TipoMovimento = new TipoMovimentoEntity
                                                              {
                                                                  Id = mov.TB_TIPO_MOVIMENTO_ID,
                                                                  TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)mov.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
                                                              },
                                                              DataMovimento = mov.TB_MOVIMENTO_DATA_MOVIMENTO
                                                          }
                                                      }).AsQueryable();

            if (isEstorno)
            {
                result = result.Where(i => i.Movimento.DataMovimento.Value.Date == movItem.Movimento.DataMovimento.Value.Date && i.Id > movItem.Id
                    || i.Movimento.DataMovimento.Value.Date > movItem.Movimento.DataMovimento.Value.Date);
            }
            else
            {
                result = result.Where(i => i.Movimento.DataMovimento.Value.Date > movItem.Movimento.DataMovimento.Value.Date);
            }

            var resultList = result.ToList();

            foreach (var r in resultList)
            {


                if (r.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
                {
                    decimal? valorMov = Convert.ToDecimal((r.QtdeMov * r.PrecoMedio)).truncarDuasCasas();

                    //if (r.PrecoMedio != pu || r.ValorMov != movItem.ValorMov)
                    if (r.ValorMov != valorMov)
                    {
                        return true;
                    }


                }
                //if (r.SaldoQtde != 0)
                //{
                //    if (Convert.ToDecimal(r.SaldoValor / r.SaldoQtde) != pu)
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }

        public TB_MOVIMENTO_ITEM getMovimentoEntradaPorTransferencia(Int32 subItemId, MovimentoItemEntity mov)
        {
            //IQueryable<Int32?> queryId = (from q in Db.TB_MOVIMENTOs where q.TB_MOVIMENTO_ATIVO == true && q.TB_MOVIMENTO_ID == mov.Movimento.Id select q.TB_MOVIMENTO_TRANSFERENCIA_ID).AsQueryable();

            //Int32? TB_MOVIMENTO_TRANSFERENCIA_ID = queryId.FirstOrDefault();

            //if (TB_MOVIMENTO_TRANSFERENCIA_ID != null)
            //{

            IQueryable<TB_MOVIMENTO_ITEM> query = (from q in Db.TB_MOVIMENTOs
                                                   join m in Db.TB_MOVIMENTO_ITEMs on q.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                   where q.TB_MOVIMENTO_ATIVO == true
                                                   where q.TB_TIPO_MOVIMENTO_ID == 6
                                                   where q.TB_MOVIMENTO_NUMERO_DOCUMENTO.Equals(mov.Movimento.NumeroDocumento)
                                                   where q.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO == mov.Movimento.Almoxarifado.Id
                                                   where m.TB_SUBITEM_MATERIAL_ID == subItemId
                                                   where m.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   where (m.TB_MOVIMENTO_ITEM_QTDE_MOV != mov.QtdeMov || m.TB_MOVIMENTO_ITEM_VALOR_MOV != mov.ValorMov)
                                                   select m).AsQueryable();

            TB_MOVIMENTO_ITEM retorno = query.FirstOrDefault();

            if (retorno != null)
                setMovimentoEntradaPorTransferencia(retorno);

            return retorno;

            //}
            //else
            //    return null;

        }

        private void setMovimentoEntradaPorTransferencia(TB_MOVIMENTO_ITEM item)
        {
            IQueryable<TB_MOVIMENTO_ITEM> query = (from q in Db.TB_MOVIMENTO_ITEMs
                                                   where q.TB_MOVIMENTO_ITEM_ID == item.TB_MOVIMENTO_ITEM_ID
                                                   where q.TB_SUBITEM_MATERIAL_ID == item.TB_SUBITEM_MATERIAL_ID
                                                   where q.TB_MOVIMENTO_ITEM_ATIVO == true
                                                   select q).AsQueryable();


            TB_MOVIMENTO_ITEM movimentoItem = query.FirstOrDefault();
            if (movimentoItem != null)
            {
                movimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV = item.TB_MOVIMENTO_ITEM_QTDE_MOV;
                movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV = item.TB_MOVIMENTO_ITEM_VALOR_MOV;

                this.Db.SubmitChanges();
            }


        }

        public IList<MovimentoEntity> ImprimirConsultaConsulmoAlmox(long _subitemCodigo, string _dataInicio, string _dataFinal, int _gestorId)
        {


            try
            {
                IQueryable<MovimentoEntity> iConsulta = (from tmi in Db.TB_MOVIMENTO_ITEMs
                                                         join tsm in Db.TB_SUBITEM_MATERIALs on tmi.TB_SUBITEM_MATERIAL_ID equals tsm.TB_SUBITEM_MATERIAL_ID
                                                         join tm in Db.TB_MOVIMENTOs on tmi.TB_MOVIMENTO_ID equals tm.TB_MOVIMENTO_ID
                                                         join ta in Db.TB_ALMOXARIFADOs on tm.TB_ALMOXARIFADO_ID equals ta.TB_ALMOXARIFADO_ID
                                                         join tss in Db.TB_SALDO_SUBITEMs on ta.TB_ALMOXARIFADO_ID equals tss.TB_ALMOXARIFADO_ID
                                                         join ttm in Db.TB_TIPO_MOVIMENTOs on tm.TB_TIPO_MOVIMENTO_ID equals ttm.TB_TIPO_MOVIMENTO_ID
                                                         join ttma in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on ttm.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals ttma.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                         where (tsm.TB_SUBITEM_MATERIAL_ID == tss.TB_SUBITEM_MATERIAL_ID)
                                                         where (tsm.TB_SUBITEM_MATERIAL_CODIGO == _subitemCodigo)
                                                         where (ttma.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Saida)
                                                         where (tm.TB_MOVIMENTO_DATA_MOVIMENTO >= Convert.ToDateTime(_dataInicio) && tm.TB_MOVIMENTO_DATA_MOVIMENTO < Convert.ToDateTime(_dataFinal))
                                                         where (ta.TB_GESTOR_ID == _gestorId)
                                                         where tm.TB_MOVIMENTO_ATIVO == true
                                                         where tmi.TB_MOVIMENTO_ITEM_ATIVO == true
                                                         select new MovimentoEntity
                                                         {
                                                             Id = tm.TB_MOVIMENTO_ID,
                                                             AnoMesReferencia = tm.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                                                             DataMovimento = tm.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                             ValorDocumento = tm.TB_MOVIMENTO_VALOR_DOCUMENTO,
                                                             GeradorDescricao = tsm.TB_SUBITEM_MATERIAL_CODIGO + " - " + tsm.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                             AlmoxConsumo = tm.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO + " - " + tm.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                                                             TotalQtdeSaidasSubitem = tmi.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                             TotalQtdeSaidasGestor = tmi.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                             SaldoAtualAlmox = tss.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                             SaldoAtualGestor = tss.TB_SALDO_SUBITEM_SALDO_QTDE

                                                         }).AsQueryable<MovimentoEntity>();

                string strSQL = iConsulta.ToString();
                Db.GetCommand(iConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

                return iConsulta.ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<MovimentoEntity> ImprimirConsultaTransferencia(int _almoxId, DateTime _dataInicial, DateTime _dataFinal)
        {
            try
            {

                IQueryable<MovimentoEntity> iConsulta = (from tm in Db.TB_MOVIMENTOs
                                                         join tmi in Db.TB_MOVIMENTO_ITEMs on tm.TB_MOVIMENTO_ID equals tmi.TB_MOVIMENTO_ID
                                                         join tsm in Db.TB_SUBITEM_MATERIALs on tmi.TB_SUBITEM_MATERIAL_ID equals tsm.TB_SUBITEM_MATERIAL_ID
                                                         join tuf in Db.TB_UNIDADE_FORNECIMENTOs on tsm.TB_UNIDADE_FORNECIMENTO_ID equals tuf.TB_UNIDADE_FORNECIMENTO_ID
                                                         where (tm.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia || tm.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
                                                         where (tm.TB_MOVIMENTO_DATA_MOVIMENTO >= _dataInicial && tm.TB_MOVIMENTO_DATA_MOVIMENTO < _dataFinal)
                                                         where (tm.TB_MOVIMENTO_ATIVO == true)
                                                         where (tm.TB_ALMOXARIFADO_ID == _almoxId)
                                                         select new MovimentoEntity
                                                         {
                                                             NumeroDocumento = tm.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                                                             DataMovimento = tm.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                             Almoxarifado = new AlmoxarifadoEntity
                                                             {
                                                                 Id = tm.TB_ALMOXARIFADO_ID
                                                             },
                                                             MovimAlmoxOrigemDestino = new AlmoxarifadoEntity
                                                             {
                                                                 Id = tm.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO
                                                             },
                                                             TipoMovimento = new TipoMovimentoEntity
                                                             {
                                                                 Id = tm.TB_TIPO_MOVIMENTO_ID
                                                             },
                                                             MovimentoItem2 = new MovimentoItemEntity
                                                             {
                                                                 SubItemDescricao = tsm.TB_SUBITEM_MATERIAL_CODIGO.ToString() + "' - '" + tsm.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                 UnidadeFornecimento = tuf.TB_UNIDADE_FORNECIMENTO_CODIGO.ToString() + "' - '" + tuf.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                 QtdeMov = tmi.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                                 ValorMov = tmi.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                             }
                                                         }).AsQueryable<MovimentoEntity>();


                string strSQL = iConsulta.ToString();
                Db.GetCommand(iConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

                return iConsulta.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int GetOrgaoPorUge(int ugeId)
        {
            IQueryable<int> query = (from q in Db.TB_MOVIMENTOs
                                     join uge in Db.TB_UGEs on q.TB_UGE_ID equals uge.TB_UGE_ID
                                     join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                     join orgao in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals orgao.TB_ORGAO_ID
                                     where uge.TB_UGE_ID == ugeId
                                     select orgao.TB_ORGAO_CODIGO
                                                 );
            var _orgao = query.FirstOrDefault();
            return _orgao;
        }
    }
  
}
