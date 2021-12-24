using Sam.Common.Util;
using Sam.Domain.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace Sam.Domain.Infrastructure
{
    public abstract class BaseInfraestructure
    {
        private static IDictionary<string, BaseEntity> dicObjetoMemoria = new ConcurrentDictionary<string, BaseEntity>(StringComparer.InvariantCultureIgnoreCase);
        public IDictionary<string, BaseEntity> CacheEntidades { get { return dicObjetoMemoria; } }

        public static decimal _valorZero = 0.0000m; 

        dbSawDataContext db;

        public dbSawDataContext Db
        {
            get
            {
                if (this.db == null)
                    this.db = new dbSawDataContext(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                return db;
            }
        }

        public int RegistrosPagina
        {
            get
            {
                //return Convert.ToInt32(ConfigurationManager.AppSettings["TOP"].ToString());
                return 50;
            }
        }

        public int SkipRegistros
        {
            get;
            set;
        }

        internal int totalregistros;

        public int TotalRegistros()
        {
            return totalregistros;
        }

        public string fmtDataFormatoBrasileiro
        { get { return "dd/MM/yyyy"; } }
        public string fmtDataHoraFormatoBrasileiro
        { get { return "dd/MM/yyyy HH:mm:ss"; } }

        public T ObterEntidade<T>(string chaveTabelaDB, bool obterViaCodigo = false) where T : BaseEntity
        { return BaseInfraestructureExtensions.ObterEntidade<T>(dicObjetoMemoria, chaveTabelaDB, obterViaCodigo); }
        public T ObterEntidade<T>(int codigoRowOuID, bool obterViaCodigo = false) where T : BaseEntity
        { return BaseInfraestructureExtensions.ObterEntidade<T>(dicObjetoMemoria, codigoRowOuID, obterViaCodigo); }
        public T ObterEntidade<T>(long codigoRowOuID, bool obterViaCodigo = false) where T : BaseEntity
        { return BaseInfraestructureExtensions.ObterEntidade<T>(dicObjetoMemoria, codigoRowOuID, obterViaCodigo); }

        public static T ObterRegistro<T>(int codigoRegistroTabela, bool obterViaCodigo = true) where T : BaseEntity
        {
            return BaseInfraestructureExtensions.ObterEntidade<T>(dicObjetoMemoria, codigoRegistroTabela, obterViaCodigo);
        }
        public static T ObterRegistro<T>(long codigoRegistroTabela, bool obterViaCodigo = true) where T : BaseEntity
        {
            return BaseInfraestructureExtensions.ObterEntidade<T>(dicObjetoMemoria, codigoRegistroTabela, obterViaCodigo);
        }
    }

    public static class BaseInfraestructureExtensions
    {
        static public long Contador = 0;
        public static T ObterEntidade<T>(this IDictionary<string, BaseEntity> objMemoriaCache, string chaveTabelaDB, bool obterViaCodigo = false) where T : BaseEntity
        {
            int codigoRowOuID = -1;
            T objEntidade = null;


            try
            {
                if (!Int32.TryParse(chaveTabelaDB.BreakLine(new char[] { ':' })[1], out codigoRowOuID))
                {
                    long _codigoRowOuID = Int64.Parse(chaveTabelaDB.BreakLine(new char[] { ':' })[1]);
                    objEntidade = BaseInfraestructureExtensions.ObterEntidade<T>(objMemoriaCache, _codigoRowOuID, obterViaCodigo);
                }
                else
                {
                    codigoRowOuID = Int32.Parse(chaveTabelaDB.BreakLine(new char[] { ':' })[1]);
                    objEntidade = BaseInfraestructureExtensions.ObterEntidade<T>(objMemoriaCache, codigoRowOuID, obterViaCodigo);
                }
            }
            catch(Exception excErroUtilizacaoCache)
            {
                throw new Exception(String.Format("Erro ao utilizar estrutura genérica de cache (L2S), tipo {0}: {1}", typeof(T).ToString(), excErroUtilizacaoCache.Message));
            }

            return (T)objEntidade;
        }
        public static T ObterEntidade<T>(this IDictionary<string, BaseEntity> objMemoriaCache, int codigoRowOuID, bool obterViaCodigo = false) where T : BaseEntity
        {
            string chaveTabelaDB = null;
            string campoComplementarChave = null;
            string tipoEntidade = null;
            BaseEntity objEntidade = null;
            BaseInfraestructure moduloInfra = new OrgaoInfraestructure();


            tipoEntidade = typeof(T).ToString();
            chaveTabelaDB = tipoEntidade.Replace("Sam.Domain.Entity.", "");
            campoComplementarChave = ((obterViaCodigo) ? "codigo" : "id");
            chaveTabelaDB = String.Format("{0}_{1}:{2}", chaveTabelaDB, campoComplementarChave, codigoRowOuID);
            if (objMemoriaCache.ContainsKey(chaveTabelaDB))
            {
                objEntidade = objMemoriaCache[chaveTabelaDB];
            }
            else
            {
                tipoEntidade = typeof(T).ToString();
                codigoRowOuID = Int32.Parse(chaveTabelaDB.BreakLine(new char[] { ':' })[1]);
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    switch (tipoEntidade)
                    {
                        case "Sam.Domain.Entity.UAEntity":
                                                                            {
                                                                                Expression<Func<TB_UA, bool>> expConsultaUA = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaUA = (rowEntidade => rowEntidade.TB_UA_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaUA = (rowEntidade => rowEntidade.TB_UA_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.TB_UAs.Where(expConsultaUA)
                                                                                                                    //.Select(_ua => new UAEntity() { Id = _ua.TB_UA_ID, Codigo = _ua.TB_UA_CODIGO, Descricao = _ua.TB_UA_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _ua.TB_UA_CODIGO, _ua.TB_UA_DESCRICAO),  Gestor = new GestorEntity() { Id = _ua.TB_GESTOR.TB_GESTOR_ID, NomeReduzido = _ua.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = _ua.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }, Uo = new UOEntity() { Id = _ua.TB_UGE.TB_UO.TB_UO_ID, Codigo = _ua.TB_UGE.TB_UO.TB_UO_CODIGO, Descricao = _ua.TB_UGE.TB_UO.TB_UO_DESCRICAO }, Uge = new UGEEntity() { Id = _ua.TB_UGE.TB_UGE_ID, Codigo = _ua.TB_UGE.TB_UGE_CODIGO, Descricao = _ua.TB_UGE.TB_UGE_DESCRICAO } })
                                                                                                                    .Select(_ua => new UAEntity() { Id = _ua.TB_UA_ID, Codigo = _ua.TB_UA_CODIGO, Descricao = _ua.TB_UA_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _ua.TB_UA_CODIGO, _ua.TB_UA_DESCRICAO), Gestor = ObterEntidade<GestorEntity>(objMemoriaCache, _ua.TB_GESTOR_ID, false), Uo = ObterEntidade<UOEntity>(objMemoriaCache, _ua.TB_UGE.TB_UO.TB_UO_ID, false), Uge = ObterEntidade<UGEEntity>(objMemoriaCache, _ua.TB_UGE_ID, false) })
                                                                                                                    .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.UOEntity":
                                                                            {
                                                                                Expression<Func<TB_UO, bool>> expConsultaUO = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaUO = (rowEntidade => rowEntidade.TB_UO_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaUO = (rowEntidade => rowEntidade.TB_UO_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_UO>().Where(expConsultaUO)
                                                                                                                              .Select(_uo => new UOEntity() { Id = _uo.TB_UO_ID, Codigo = _uo.TB_UO_CODIGO, Descricao = _uo.TB_UO_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _uo.TB_UO_CODIGO, _uo.TB_UO_DESCRICAO) })
                                                                                                                              .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.PTResEntity":
                                                                            {
                                                                                Expression<Func<TB_PTRE, bool>> expConsultaPTRES = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaPTRES = (rowEntidade => rowEntidade.TB_PTRES_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaPTRES = (rowEntidade => rowEntidade.TB_PTRES_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_PTRE>().Where(expConsultaPTRES)
                                                                                                                                .Select(_ptres => new PTResEntity() { Id = _ptres.TB_PTRES_ID, Codigo = _ptres.TB_PTRES_CODIGO, Descricao = _ptres.TB_PTRES_DESCRICAO })
                                                                                                                                .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.NaturezaDespesaEntity":
                                                                            {
                                                                                Expression<Func<TB_NATUREZA_DESPESA, bool>> expConsultaNATDESPESA = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaNATDESPESA = (rowEntidade => rowEntidade.TB_NATUREZA_DESPESA_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaNATDESPESA = (rowEntidade => rowEntidade.TB_NATUREZA_DESPESA_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_NATUREZA_DESPESA>().Where(expConsultaNATDESPESA)
                                                                                                                                            .Select(_natDespesa => new NaturezaDespesaEntity() { Id = _natDespesa.TB_NATUREZA_DESPESA_ID, Codigo = _natDespesa.TB_NATUREZA_DESPESA_CODIGO, Descricao = _natDespesa.TB_NATUREZA_DESPESA_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _natDespesa.TB_NATUREZA_DESPESA_CODIGO, _natDespesa.TB_NATUREZA_DESPESA_DESCRICAO) })
                                                                                                                                            .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.UGEEntity":
                                                                            {
                                                                                Expression<Func<TB_UGE, bool>> expConsultaUGE = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaUGE = (rowEntidade => rowEntidade.TB_UGE_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaUGE = (rowEntidade => rowEntidade.TB_UGE_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_UGE>().Where(expConsultaUGE)
                                                                                                                                //.Select(_uge => new UGEEntity() { Id = _uge.TB_UGE_ID, Codigo = _uge.TB_UGE_CODIGO, Descricao = _uge.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _uge.TB_UGE_CODIGO, _uge.TB_UGE_DESCRICAO), Orgao = new OrgaoEntity() { Id = _uge.TB_UO.TB_ORGAO.TB_ORGAO_ID, Codigo = _uge.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO, Descricao = _uge.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO } })
                                                                                                                                .Select(_uge => new UGEEntity() { Id = _uge.TB_UGE_ID, Codigo = _uge.TB_UGE_CODIGO, Descricao = _uge.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _uge.TB_UGE_CODIGO, _uge.TB_UGE_DESCRICAO), Orgao = ObterEntidade<OrgaoEntity>(objMemoriaCache, _uge.TB_UO.TB_ORGAO_ID, false) })
                                                                                                                                .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.AlmoxarifadoEntity":
                                                                            {
                                                                                Expression<Func<TB_ALMOXARIFADO, bool>> expConsultaALMOX = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaALMOX = (rowEntidade => rowEntidade.TB_ALMOXARIFADO_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaALMOX = (rowEntidade => rowEntidade.TB_ALMOXARIFADO_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_ALMOXARIFADO>().Where(expConsultaALMOX)
                                                                                                                                        //.Select(_almox => new AlmoxarifadoEntity() { Id = _almox.TB_ALMOXARIFADO_ID, Codigo = _almox.TB_ALMOXARIFADO_CODIGO, Descricao = _almox.TB_ALMOXARIFADO_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _almox.TB_ALMOXARIFADO_CODIGO, _almox.TB_ALMOXARIFADO_DESCRICAO), Uge = new UGEEntity() { Id = _almox.TB_UGE.TB_UGE_ID, Codigo = _almox.TB_UGE.TB_UGE_CODIGO, Descricao = _almox.TB_UGE.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _almox.TB_UGE.TB_UGE_CODIGO, _almox.TB_UGE.TB_UGE_DESCRICAO) }, Gestor = new GestorEntity() { Id = _almox.TB_GESTOR.TB_GESTOR_ID, NomeReduzido = _almox.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = _almox.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO } })
                                                                                                                                        .Select(_almox => new AlmoxarifadoEntity() { Id = _almox.TB_ALMOXARIFADO_ID, Codigo = _almox.TB_ALMOXARIFADO_CODIGO, Descricao = _almox.TB_ALMOXARIFADO_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _almox.TB_ALMOXARIFADO_CODIGO, _almox.TB_ALMOXARIFADO_DESCRICAO), Uge = ObterEntidade<UGEEntity>(objMemoriaCache, _almox.TB_UGE_ID.Value, false), Gestor = ObterEntidade<GestorEntity>(objMemoriaCache, _almox.TB_GESTOR_ID, false) })
                                                                                                                                        .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.OrgaoEntity":
                                                                            {
                                                                                Expression<Func<TB_ORGAO, bool>> expConsultaORGAO = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaORGAO = (rowEntidade => rowEntidade.TB_ORGAO_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaORGAO = (rowEntidade => rowEntidade.TB_ORGAO_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_ORGAO>().Where(expConsultaORGAO)
                                                                                                                                    .Select(_orgao => new OrgaoEntity() { Id = _orgao.TB_ORGAO_ID, Codigo = _orgao.TB_ORGAO_CODIGO, Descricao = _orgao.TB_ORGAO_DESCRICAO })
                                                                                                                                    .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.MovimentoItemEntity":
                                                                            {
                                                                                objEntidade = moduloInfra.Db.GetTable<TB_MOVIMENTO_ITEM>().Where(rowEntidade => rowEntidade.TB_MOVIMENTO_ITEM_ID == codigoRowOuID)
                                                                                                                                            .Select(_itemMovimentacaoMaterial => new MovimentoItemEntity() { Id = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_ID, Ativo = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_ATIVO, ValorMov = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_VALOR_MOV, Movimento = new MovimentoEntity() { Id = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_ID, NumeroDocumento = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO, Ativo = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO } })
                                                                                                                                            .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.SubItemMaterialEntity":
                                                                            {
                                                                                Expression<Func<TB_SUBITEM_MATERIAL, bool>> expConsultaCodigoSubitemMaterial = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaCodigoSubitemMaterial = (rowEntidade => rowEntidade.TB_SUBITEM_MATERIAL_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaCodigoSubitemMaterial = (rowEntidade => rowEntidade.TB_SUBITEM_MATERIAL_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_SUBITEM_MATERIAL>().Where(expConsultaCodigoSubitemMaterial)
                                                                                                                                            //.Select(_subitemMaterial => new SubItemMaterialEntity() { Id = _subitemMaterial.TB_SUBITEM_MATERIAL_ID, Codigo = _subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO, Descricao = _subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, NaturezaDespesa = new NaturezaDespesaEntity() { Id = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID, Codigo = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO, Descricao = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO }, UnidadeFornecimento = new UnidadeFornecimentoEntity() { Id = _subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO } })
                                                                                                                                            .Select(_subitemMaterial => new SubItemMaterialEntity() { Id = _subitemMaterial.TB_SUBITEM_MATERIAL_ID, Codigo = _subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO, Descricao = _subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, NaturezaDespesa = ObterEntidade<NaturezaDespesaEntity>(objMemoriaCache, _subitemMaterial.TB_NATUREZA_DESPESA_ID, false), UnidadeFornecimento = ObterEntidade<UnidadeFornecimentoEntity>(objMemoriaCache, _subitemMaterial.TB_UNIDADE_FORNECIMENTO_ID, false) })
                                                                                                                                            .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.SaldoSubItemEntity":
                                                                            {
                                                                                Expression<Func<TB_SALDO_SUBITEM, bool>> expConsultaRowTabelaSaldoSubitem = null;
                                                                                expConsultaRowTabelaSaldoSubitem = (rowEntidade => rowEntidade.TB_SALDO_SUBITEM_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_SALDO_SUBITEM>().Where(expConsultaRowTabelaSaldoSubitem)
                                                                                                                                         //.Select(_rowSaldoSubitemMaterial => new SaldoSubItemEntity() { Id = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_ID, SubItemMaterial = new SubItemMaterialEntity() { Id = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, Codigo = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, Descricao = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO }, UGE = new UGEEntity() { Id = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_ID, Codigo = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_CODIGO, Descricao = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_DESCRICAO }, LoteIdent = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_IDENT, LoteDataVenc = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_DT_VENC, LoteFabr = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_FAB, SaldoQtde = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_QTDE, SaldoValor = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_VALOR })
                                                                                                                                         .Select(_rowSaldoSubitemMaterial => new SaldoSubItemEntity() { Id = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_ID, LoteIdent = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_IDENT, LoteDataVenc = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_DT_VENC, LoteFabr = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_FAB, SaldoQtde = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_QTDE, SaldoValor = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_VALOR, UGE = ObterEntidade<UGEEntity>(objMemoriaCache, _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_ID, false), SubItemMaterial = ObterEntidade<SubItemMaterialEntity>(objMemoriaCache, _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL_ID, false) })
                                                                                                                                         .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.FornecedorEntity":
                                                                            {
                                                                                Expression<Func<TB_FORNECEDOR, bool>> expConsultaFornecedor = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaFornecedor = (rowEntidade => rowEntidade.TB_FORNECEDOR_CPFCNPJ == codigoRowOuID.ToString());
                                                                                else
                                                                                    expConsultaFornecedor = (rowEntidade => rowEntidade.TB_FORNECEDOR_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_FORNECEDOR>().Where(expConsultaFornecedor)
                                                                                                                                      .Select(_rowFornecedor => new FornecedorEntity() { Id = _rowFornecedor.TB_FORNECEDOR_ID, CpfCnpj = _rowFornecedor.TB_FORNECEDOR_CPFCNPJ, Nome = _rowFornecedor.TB_FORNECEDOR_NOME, Cidade = _rowFornecedor.TB_FORNECEDOR_CIDADE, CodigoDescricao = String.Format("{0} - {1}", _rowFornecedor.TB_FORNECEDOR_CPFCNPJ, _rowFornecedor.TB_FORNECEDOR_NOME) })
                                                                                                                                      .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.UnidadeFornecimentoEntity":
                                                                            {
                                                                                Expression<Func<TB_UNIDADE_FORNECIMENTO, bool>> expConsultaUnidadeFornecimento = null;
                                                                                expConsultaUnidadeFornecimento = (rowEntidade => rowEntidade.TB_UNIDADE_FORNECIMENTO_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_UNIDADE_FORNECIMENTO>().Where(expConsultaUnidadeFornecimento)
                                                                                                                                                .Select(_unidadeFornecimento => new UnidadeFornecimentoEntity() { Id = _unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = _unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO, _unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO) })
                                                                                                                                                .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.DivisaoEntity":
                                                                            {
                                                                                Expression<Func<TB_DIVISAO, bool>> expConsultaDivisao = null;
                                                                                if (obterViaCodigo)
                                                                                    expConsultaDivisao = (rowEntidade => rowEntidade.TB_DIVISAO_CODIGO == codigoRowOuID);
                                                                                else
                                                                                    expConsultaDivisao = (rowEntidade => rowEntidade.TB_DIVISAO_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_DIVISAO>().Where(expConsultaDivisao)
                                                                                                                                                .Select(_divisao => new DivisaoEntity() { Id = _divisao.TB_DIVISAO_ID, Codigo = _divisao.TB_DIVISAO_CODIGO, Descricao = _divisao.TB_DIVISAO_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _divisao.TB_DIVISAO_CODIGO, _divisao.TB_DIVISAO_DESCRICAO) })
                                                                                                                                                .FirstOrDefault();
                                                                                break;
                                                                            }
                        case "Sam.Domain.Entity.GestorEntity":
                                                                            {
                                                                                Expression<Func<TB_GESTOR, bool>> expConsultaGestorSAM = null;
                                                                                expConsultaGestorSAM = (rowEntidade => rowEntidade.TB_GESTOR_ID == codigoRowOuID);

                                                                                objEntidade = moduloInfra.Db.GetTable<TB_GESTOR>().Where(expConsultaGestorSAM)
                                                                                                                                  .Select(_gestorSAM => new GestorEntity() { Id = _gestorSAM.TB_GESTOR_ID, NomeReduzido = _gestorSAM.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = _gestorSAM.TB_GESTOR_CODIGO_GESTAO })
                                                                                                                                  .FirstOrDefault();
                                                                                break;
                                                                            }
                    }
                }

                if (objEntidade.IsNotNull())
                    objMemoriaCache.Add(chaveTabelaDB, objEntidade);
            }

            return (T)objEntidade;
        }
        public static T ObterEntidade<T>(this IDictionary<string, BaseEntity> objMemoriaCache, long codigoRowOuID, bool obterViaCodigo = false) where T : BaseEntity
        {
            string chaveTabelaDB = null;
            string campoComplementarChave = null;
            string tipoEntidade = null;
            BaseEntity objEntidade = null;
            BaseInfraestructure moduloInfra = new OrgaoInfraestructure();


            tipoEntidade = typeof(T).ToString();
            chaveTabelaDB = tipoEntidade.Replace("Sam.Domain.Entity.", "");
            campoComplementarChave = ((obterViaCodigo) ? "codigo" : "id");
            chaveTabelaDB = String.Format("{0}_{1}:{2}", chaveTabelaDB, campoComplementarChave, codigoRowOuID);
            if (objMemoriaCache.ContainsKey(chaveTabelaDB))
            {
                objEntidade = objMemoriaCache[chaveTabelaDB];
            }
            else
            {
                tipoEntidade = typeof(T).ToString();
                codigoRowOuID = Int64.Parse(chaveTabelaDB.BreakLine(new char[] { ':' })[1]);
                switch (tipoEntidade)
                {
                    case "Sam.Domain.Entity.UAEntity":
                                                                        Expression<Func<TB_UA, bool>> expConsultaUA = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaUA = (rowEntidade => rowEntidade.TB_UA_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaUA = (rowEntidade => rowEntidade.TB_UA_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.TB_UAs.Where(expConsultaUA)
                                                                                                           .Select(_ua => new UAEntity() { Id = _ua.TB_UA_ID, Codigo = _ua.TB_UA_CODIGO, Descricao = _ua.TB_UA_DESCRICAO, Gestor = new GestorEntity() { Id = _ua.TB_GESTOR.TB_GESTOR_ID, NomeReduzido = _ua.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = _ua.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO }, Uo = new UOEntity() { Id = _ua.TB_UGE.TB_UO.TB_UO_ID, Codigo = _ua.TB_UGE.TB_UO.TB_UO_CODIGO, Descricao = _ua.TB_UGE.TB_UO.TB_UO_DESCRICAO } })
                                                                                                           .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.UOEntity":
                                                                        Expression<Func<TB_UO, bool>> expConsultaUO = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaUO = (rowEntidade => rowEntidade.TB_UO_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaUO = (rowEntidade => rowEntidade.TB_UO_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_UO>().Where(expConsultaUO)
                                                                                                                      .Select(_uo => new UOEntity() { Id = _uo.TB_UO_ID, Codigo = _uo.TB_UO_CODIGO, Descricao = _uo.TB_UO_DESCRICAO })
                                                                                                                      .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.PTResEntity":
                                                                        Expression<Func<TB_PTRE, bool>> expConsultaPTRES = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaPTRES = (rowEntidade => rowEntidade.TB_PTRES_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaPTRES = (rowEntidade => rowEntidade.TB_PTRES_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_PTRE>().Where(expConsultaPTRES)
                                                                                                                        .Select(_ptres => new PTResEntity() { Id = _ptres.TB_PTRES_ID, Codigo = _ptres.TB_PTRES_CODIGO, Descricao = _ptres.TB_PTRES_DESCRICAO })
                                                                                                                        .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.NaturezaDespesaEntity":
                                                                        Expression<Func<TB_NATUREZA_DESPESA, bool>> expConsultaNATDESPESA = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaNATDESPESA = (rowEntidade => rowEntidade.TB_NATUREZA_DESPESA_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaNATDESPESA = (rowEntidade => rowEntidade.TB_NATUREZA_DESPESA_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_NATUREZA_DESPESA>().Where(expConsultaNATDESPESA)
                                                                                                                                    .Select(_natDespesa => new NaturezaDespesaEntity() { Id = _natDespesa.TB_NATUREZA_DESPESA_ID, Codigo = _natDespesa.TB_NATUREZA_DESPESA_CODIGO, Descricao = _natDespesa.TB_NATUREZA_DESPESA_DESCRICAO, CodigoDescricao = String.Format("{0} - {1}", _natDespesa.TB_NATUREZA_DESPESA_CODIGO, _natDespesa.TB_NATUREZA_DESPESA_DESCRICAO) })
                                                                                                                                    .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.UGEEntity":
                                                                        Expression<Func<TB_UGE, bool>> expConsultaUGE = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaUGE = (rowEntidade => rowEntidade.TB_UGE_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaUGE = (rowEntidade => rowEntidade.TB_UGE_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_UGE>().Where(expConsultaUGE)
                                                                                                                       .Select(_uge => new UGEEntity() { Id = _uge.TB_UGE_ID, Codigo = _uge.TB_UGE_CODIGO, Descricao = _uge.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _uge.TB_UGE_CODIGO, _uge.TB_UGE_DESCRICAO) })
                                                                                                                       .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.AlmoxarifadoEntity":
                                                                        Expression<Func<TB_ALMOXARIFADO, bool>> expConsultaALMOX = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaALMOX = (rowEntidade => rowEntidade.TB_ALMOXARIFADO_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaALMOX = (rowEntidade => rowEntidade.TB_ALMOXARIFADO_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_ALMOXARIFADO>().Where(expConsultaALMOX)
                                                                                                                                .Select(_almox => new AlmoxarifadoEntity() { Id = _almox.TB_ALMOXARIFADO_ID, Codigo = _almox.TB_ALMOXARIFADO_CODIGO, Descricao = _almox.TB_ALMOXARIFADO_DESCRICAO, CodigoDescricao = String.Format("{0:D5} - {1}", _almox.TB_ALMOXARIFADO_CODIGO, _almox.TB_ALMOXARIFADO_DESCRICAO), Gestor = new GestorEntity() { Id = _almox.TB_GESTOR.TB_GESTOR_ID, NomeReduzido = _almox.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = _almox.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO } })
                                                                                                                                .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.MovimentoItemEntity":
                                                                        objEntidade = moduloInfra.Db.GetTable<TB_MOVIMENTO_ITEM>().Where(rowEntidade => rowEntidade.TB_MOVIMENTO_ITEM_ID == codigoRowOuID)
                                                                                                                                  .Select(_itemMovimentacaoMaterial => new MovimentoItemEntity() { Id = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_ID, Ativo = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_ATIVO, ValorMov = _itemMovimentacaoMaterial.TB_MOVIMENTO_ITEM_VALOR_MOV, Movimento = new MovimentoEntity() { Id = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_ID, NumeroDocumento = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO, Ativo = _itemMovimentacaoMaterial.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO } })
                                                                                                                                  .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.SubItemMaterialEntity":
                                                                        Expression<Func<TB_SUBITEM_MATERIAL, bool>> expConsultaCodigoSubitemMaterial = null;
                                                                        if (obterViaCodigo)
                                                                            expConsultaCodigoSubitemMaterial = (rowEntidade => rowEntidade.TB_SUBITEM_MATERIAL_CODIGO == codigoRowOuID);
                                                                        else
                                                                            expConsultaCodigoSubitemMaterial = (rowEntidade => rowEntidade.TB_SUBITEM_MATERIAL_ID == codigoRowOuID);

                                                                        objEntidade = moduloInfra.Db.GetTable<TB_SUBITEM_MATERIAL>().Where(expConsultaCodigoSubitemMaterial)
                                                                                                                                    .Select(_subitemMaterial => new SubItemMaterialEntity() { Id = _subitemMaterial.TB_SUBITEM_MATERIAL_ID, Codigo = _subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO, Descricao = _subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, NaturezaDespesa = new NaturezaDespesaEntity() { Id = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID, Codigo = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO, Descricao = _subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO },                                                                                                           UnidadeFornecimento = new UnidadeFornecimentoEntity() { Id = _subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, Codigo = _subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO } })
                                                                                                                                    .FirstOrDefault();
                                                                        break;

                    case "Sam.Domain.Entity.SaldoSubItemEntity":
                                                                        objEntidade = moduloInfra.Db.GetTable<TB_SALDO_SUBITEM>().Where(rowEntidade => rowEntidade.TB_SALDO_SUBITEM_ID == codigoRowOuID)
                                                                                                                                 .Select(_rowSaldoSubitemMaterial => new SaldoSubItemEntity() { Id = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_ID, SubItemMaterial = new SubItemMaterialEntity() { Id = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, Codigo = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, Descricao = _rowSaldoSubitemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO }, UGE = new UGEEntity() { Id = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_ID, Codigo = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_CODIGO, Descricao = _rowSaldoSubitemMaterial.TB_UGE.TB_UGE_DESCRICAO }, LoteIdent = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_IDENT, LoteDataVenc = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_DT_VENC, LoteFabr = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_LOTE_FAB, SaldoQtde = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_QTDE, SaldoValor = _rowSaldoSubitemMaterial.TB_SALDO_SUBITEM_SALDO_VALOR })
                                                                                                                                 .FirstOrDefault();
                                                                        break;
                }

                objMemoriaCache.Add(chaveTabelaDB, objEntidade);
            }

            return (T)objEntidade;
        }
    }
}
