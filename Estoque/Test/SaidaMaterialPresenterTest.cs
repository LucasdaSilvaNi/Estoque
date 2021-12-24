using Sam.Presenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Infrastructure;
using System.Threading;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SaidaMaterialPresenterTest and is intended
    ///to contain all SaidaMaterialPresenterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SaidaMaterialPresenterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private MovimentoEntity GetMovimento()
        {
            
            MovimentoEntity movimento = new MovimentoEntity(); 

            movimento.MovimentoItem = new List<MovimentoItemEntity>();

            var movimentoItem = new MovimentoItemEntity();
            movimentoItem.UGE = new UGEEntity(121);
            movimentoItem.ItemMaterial = new ItemMaterialEntity(69959);
            movimentoItem.SubItemMaterial = new SubItemMaterialEntity(25951);
            movimentoItem.QtdeMov = 1;
            movimentoItem.QtdeLiq = 0;
            movimentoItem.PrecoUnit = 10;
            movimentoItem.SaldoValor = 10;
            movimentoItem.SaldoQtde = 1;
            movimentoItem.ValorMov = 10;
            movimentoItem.Desd = 0;
            movimentoItem.Ativo = true;
            movimento.MovimentoItem.Add(movimentoItem);

            movimento.Almoxarifado = new AlmoxarifadoEntity(156);
            movimento.AnoMesReferencia = "201305";
            movimento.Ativo = true;
            movimento.CodigoDescricao = "123";
            movimento.CodigoFormatado = "123";
            movimento.DataMovimento = Convert.ToDateTime("21/05/2013");
            movimento.DataDocumento = Convert.ToDateTime("21/05/2013");
            movimento.DataOperacao = DateTime.Now;
            movimento.Empenho = "";
            movimento.FonteRecurso = String.Empty;

            movimento.IdLogin = 1;
            movimento.Instrucoes = "";

            movimento.NaturezaDespesaEmpenho = "";
            movimento.Fornecedor = new FornecedorEntity(1);

            //movimento.NlLiquidacao = "";
            movimento.NumeroDocumento = "201212454564";
            movimento.Observacoes = "";

            movimento.TipoMovimento = new TipoMovimentoEntity(5);
            movimento.UGE = new UGEEntity(121);
            movimento.ValorDocumento = 123;

            movimento.Divisao = new DivisaoEntity(641);

            return movimento;
        }



        /// <summary>
        ///A test for GravarMovimentoSaida
        ///</summary>
        [TestMethod()]
        public void GravarMovimentoSaidaTest()
        {
            MovimentoBusiness target = new MovimentoBusiness(); 

            target.Movimento = GetMovimento();
            target.SalvarMovimentoSaida();

            //EstornarTest(60708);
        }

        /// <summary>
        ///A test for Estornar
        ///</summary>
        [TestMethod()]
        public void EstornarTest()
        {
            MovimentoBusiness target = new MovimentoBusiness(); 

            MovimentoEntity movimento = GetMovimento();
            movimento.Id = 3652;
            movimento.NumeroDocumento = "";
            target.Movimento = movimento;

            target.EstornarMovimentoSaida(1);
        }

        /// <summary>
        ///A test for CarregarRascunho
        ///</summary>
        [TestMethod()]
        public void CarregarRascunhoTest()
        {
            SaidaMaterialPresenter target = new SaidaMaterialPresenter(); 
            int idRascunho = 3; 
            MovimentoEntity expected = null; 
            MovimentoEntity actual;
            actual = target.CarregarRascunho(idRascunho);
        }

        /// <summary>
        ///A test for ImprimirRelatorioSaida
        ///</summary>
        [TestMethod()]
        public void ImprimirRelatorioSaidaTest()
        {
            SaidaMaterialPresenter target = new SaidaMaterialPresenter(); 
            MovimentoEntity movimento = new MovimentoEntity(116822); 
            
            target.ImprimirRelatorioSaida(movimento);
        }
    }
}
