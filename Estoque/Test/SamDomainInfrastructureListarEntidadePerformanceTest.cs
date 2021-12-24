using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sam.Domain.Infrastructure;
using Sam.Domain.Entity;
using System.Diagnostics;
using System.Configuration;
using Sam.ServiceInfraestructure;
using System.Linq.Expressions;
//using NUnit.Framework;


namespace Test
{
    /// <summary>
    /// Summary description for ListarEntidade_Iqueryable_Versus_Inumerable
    /// </summary>
    [TestClass]
    public class SamDomainInfrastructureListarEntidadePerformanceTest 
    {
        /*  TESTES         
         * 1 [x] - IQueryable<MovimentoItem> tempo consulta 
         * 2 [x] - IList<MovimentoItem> tempo consulta .ToList().filtros  X .filtros.ToList()         
         * 3 [x] - IQueryable<TB_MOVIMENTO_ITEMs> tempo consulta
         * 4 [x] - IList<TB_MOVIMENTO_ITEMs> tempo consulta .ToList().filtros X .filtros.ToList()          
         * 5 [x] - Sem DTO x Com DTO. */        

        //private static dbSawDataContext db = new dbSawDataContext(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);

        //public dbSawDataContext DB
        //{
        //    get
        //    {
        //        return db;
        //    }
        //    set
        //    {
        //        db = value;
        //    }
        //}

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        
        private TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }

        [TestCategory("IQueryable<T> - Com DTO")]
        [Priority(1)]
        [TestMethod]
        public void SamDomainInfrastructureFAKE_ListarIQueryable_TempoConsulta()
        {
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarTodosIQueryable()));
        }

        [TestMethod]
        [TestCategory("IQueryable<T> - Com DTO")]
        public void SamDomainInfrastructureFAKE_ListarIQueryableFiltros_TempoConsulta()
        {
            Expression<Func<MovimentoItemEntity, bool>> expression = (x => x.Id > 700000);

            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarIQueryableByMovimentoId(expression)));
        }

        [TestMethod]
        [TestCategory("IQueryable<T> - Sem DTO")]
        public void SamDomainInfrastructureFAKE_ListarIQueryableSemDTO_TempoConsulta()
        {
            //Assert.AreEqual(0, new DomainInfrastructureFake().ListarTodosIQueryableSemDTO().Count());
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarTodosIQueryableTBMovimentoItems()));
        }

        [TestMethod]
        [Ignore]
        public void SamDomainInfrastructureFAKE_ListarIList_TempoConsulta()
        {
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().Listar()));
        }

        [TestMethod]
        [TestCategory("IList<T> - Com DTO")]
        public void SamDomainInfrastructureFAKE_ListarIListToListFiltros_TempoConsulta()
        {
            Func<MovimentoItemEntity, bool> expression = (x => x.Id > 700000);
            //int amout = resultQuery.Count();

            //Assert.AreEqual(730031, amout);
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarToListFiltros(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - Sem DTO")]
        public void SamDomainInfrastructureFAKE_ListarIlistSemDTOToListFiltros_Tempoconsulta()
        {
            Func<TB_MOVIMENTO_ITEM, bool> expression = (x => x.TB_MOVIMENTO_ITEM_ID > 700000);
            //Assert.AreEqual(0, new DomainInfrastructureFake().ListarTodosIQueryableSemDTO().Count());
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarToListFiltrosTBMovimentoItems(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - Com DTO")]
        public void SamDomainInfrastructureFAKE_ListarIListFiltrosToList_TempoConsulta()
        {
            Func<MovimentoItemEntity, bool> expression = (x => x.Id > 700000);
            //int amout = resultQuery.Count();

            //Assert.AreEqual(730031, amout);
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarFiltrosToList(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - Sem DTO")]
        public void SamDomainInfrastructureFAKE_ListarIListSemDTOFiltrosToList_TempoConsulta()
        {
            Func<TB_MOVIMENTO_ITEM, bool> expression = (x => x.TB_MOVIMENTO_ITEM_ID > 700000);
            //int amout = resultQuery.Count();

            //Assert.AreEqual(730031, amout);
            Assert.AreEqual(TimeSpan.FromSeconds(0), Time(() => new DomainInfrastructureFake().ListarFiltrosToListTBMovimentoItemIds(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - Com DTO")]
        public void SamDomainInfrastructureFAKE_ListarIList_FiltrosToList_VS_ToListFiltros_TempoConsulta()
        {
            Func<MovimentoItemEntity, bool> expression = (x => x.Id > 700000);

            Assert.AreEqual(Time(() => new DomainInfrastructureFake().ListarToListFiltros(expression)), Time(() => new DomainInfrastructureFake().ListarFiltrosToList(expression)));
        }

        [TestMethod]        
        [TestCategory("IList<T> x IQueryable<T> - Com DTO")]
        public void SamDomainInfrastructureFAKE_IListFiltrosToList_VS_IQueryableFiltrosAsQueryable_TempoConsulta()
        {
            Func<MovimentoItemEntity, bool> expression = (x => x.Id > 700000);

            Assert.AreEqual(new DomainInfrastructureFake().ListarFiltrosToList(expression).Count, new DomainInfrastructureFake().ListarFiltrosIQueryable(expression).Count());
            Assert.AreEqual(Time(() => new DomainInfrastructureFake().ListarFiltrosToList(expression)), Time(() => new DomainInfrastructureFake().ListarFiltrosIQueryable(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - .ToList().Filtros() x .Filtros().ToList() - Sem DTO")]        
        public void SamDomainInfrastructureFAKE_ListarIListSemDTO_FiltrosToList_VS_ToListFiltros_TempoConsulta()
        {
            Func<TB_MOVIMENTO_ITEM, bool> expression = (x => x.TB_MOVIMENTO_ITEM_ID > 700000);

            Assert.AreEqual(Time(() => new DomainInfrastructureFake().ListarToListFiltrosTBMovimentoItems(expression)), Time(() => new DomainInfrastructureFake().ListarFiltrosToListTBMovimentoItemIds(expression)));
        }

        [TestMethod]
        [TestCategory("IList<T> - .Filtros().ToList() - Sem DTO x Com DTO")]
        public void SamDomainInfrastructureFAKE_Listar_IListSemDTOFiltrosToList_VS_IListComDTOFiltrosToList_TempoConsulta()
        {
            Func<TB_MOVIMENTO_ITEM, bool> expression = (x => x.TB_MOVIMENTO_ITEM_ID > 700000);
            Func<MovimentoItemEntity, bool> expressionEntity = (x => x.Id > 700000);

            Assert.AreEqual(Time(()=> new DomainInfrastructureFake().ListarFiltrosToListTBMovimentoItemIds(expression)), Time(()=>new DomainInfrastructureFake().ListarFiltrosToList(expressionEntity)));
        }


        [TestMethod]
        [TestCategory("IList<T> - .Filtros().ToList() - Com DTO x Sem DTO")]
        public void SamDomainInfrastructureFAKE_Listar_IListComDTOFiltrosToList_TempoConsulta_VS_IListSemDTOFiltrosToList()
        {
            Func<TB_MOVIMENTO_ITEM, bool> expression = (x => x.TB_MOVIMENTO_ITEM_ID > 700000);
            Func<MovimentoItemEntity, bool> expressionEntity = (x => x.Id > 700000);

            //Assert.AreEqual(new DomainInfrastructureFake().ListarFiltrosToList(expressionEntity).Count(), new DomainInfrastructureFake().ListarFiltrosToListTBMovimentoItemIds(expression).Count());
            Assert.AreEqual(Time(() => new DomainInfrastructureFake().ListarFiltrosToList(expressionEntity)), Time(() => new DomainInfrastructureFake().ListarFiltrosToListTBMovimentoItemIds(expression)));
        }

    }

    internal abstract class BaseInfraestructureFake        
    {
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
                return 1000000;                
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

    }

    internal class DomainInfrastructureFake : BaseInfraestructureFake//, IMovimentoItemService
    {
        public IQueryable<MovimentoItemEntity> ListarTodosIQueryable()
        {
            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                             SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                                             ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                         }).Take(this.RegistrosPagina); 

            return resultado;
        }


        internal IQueryable<TB_MOVIMENTO_ITEM> ListarTodosIQueryableTBMovimentoItems()
        {
            IQueryable<TB_MOVIMENTO_ITEM> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                       orderby a.TB_MOVIMENTO_ITEM_ID
                                                       select a);
            return resultado;
        }

        public IQueryable<MovimentoItemEntity> ListarIQueryableByMovimentoId(Expression<Func<MovimentoItemEntity, bool>> expression)
        {
            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                         orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                             SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                             UGE = new UGEEntity(a.TB_UGE_ID),
                                                             ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                         }).Take(this.RegistrosPagina).Where(expression);

            return resultado;
        }

        public IList<MovimentoItemEntity> Listar()
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                        SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).Take(this.RegistrosPagina).ToList<MovimentoItemEntity>();

            return resultado;
        }

        public IList<MovimentoItemEntity> ListarToListFiltros(Func<MovimentoItemEntity, bool> expression)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                        SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).Take(this.RegistrosPagina).ToList<MovimentoItemEntity>().Where(expression).ToList();

            return resultado;
        }

        internal IList<TB_MOVIMENTO_ITEM> ListarToListFiltrosTBMovimentoItems(Func<TB_MOVIMENTO_ITEM, bool> expression)
        {
            IList<TB_MOVIMENTO_ITEM> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                  orderby a.TB_MOVIMENTO_ITEM_ID
                                                  select a).ToList().Where(expression).ToList();

            return resultado;
        }

        public IList<MovimentoItemEntity> ListarFiltrosToList(Func<MovimentoItemEntity, bool> expression)
        {
            IList<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                        SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).Where(expression).ToList<MovimentoItemEntity>();

            return resultado;
        }

        public IQueryable<MovimentoItemEntity> ListarFiltrosIQueryable(Func<MovimentoItemEntity, bool> expression)
        {
            IQueryable<MovimentoItemEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                    orderby a.TB_MOVIMENTO_ITEM_ID
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
                                                        SubItemMaterial = new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID),
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).Where(expression).Take(this.RegistrosPagina).AsQueryable(); 

            return resultado;
        }

        internal IList<TB_MOVIMENTO_ITEM> ListarFiltrosToListTBMovimentoItemIds(Func<TB_MOVIMENTO_ITEM, bool> expression)
        {
            IList<TB_MOVIMENTO_ITEM> resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                                                  orderby a.TB_MOVIMENTO_ITEM_ID
                                                  select a).Where(expression).ToList(); 

            return resultado;
        }

        #region Not Impelmented

        public IList<MovimentoItemEntity> Listar(int MovimentoId)
        {
            throw new NotImplementedException();
        }

        public MovimentoItemEntity Estornar(int MovimentoId)
        {
            throw new NotImplementedException();
        }

        public MovimentoItemEntity LerRegistroItem(int MovimentoId)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarPorMovimento(MovimentoEntity mov)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoEntity> ListarMovimentoItemSaldoTodos(MovimentoEntity movimento)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItem(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime DtInicial, DateTime DtFinal, bool comEstorno)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItemRecalculo(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorId(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ImprimirMovimento(DateTime? dataInicial, DateTime? dataFinal)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ImprimirMovimento()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarSaldoQteDeItemMaterial(int iItemMaterialCodigo, int iSubItemMaterialCodigo_ID, string strEmpenhoCodigo, int iUge_ID, int iAlmoxarifado_ID)
        {
            throw new NotImplementedException();
        }

        public string SelectUnidFornecimentoSiafisico(int codUnidade)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarNotaRequicao(int MovimentoId)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorIdEstorno(int? AlmoxId, long? SubItemMatId, int? UgeId, DateTime? DtInicial, DateTime? DtFinal)
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoPendenteEntity> ImprimirConsultaMovimentosEntradaPendentes(int TipoConsulta, int? AlmoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            throw new NotImplementedException();
        }


        public MovimentoItemEntity Entity
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public IList<MovimentoItemEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public MovimentoItemEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
