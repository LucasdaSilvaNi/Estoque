using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;


namespace Test
{
    
    
    /// <summary>
    ///This is a test class for MovimentoBusinessTest and is intended
    ///to contain all MovimentoBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MovimentoBusinessTest
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


        /// <summary>
        ///A test for EstornarMovimentoSaida
        ///</summary>
        [TestMethod()]
        public void EstornarMovimentoSaidaTest()
        {
            MovimentoBusiness target = new MovimentoBusiness(); 
            int loginIdEstornante = 0; 
            bool expected = false; 
            bool actual;
            actual = target.EstornarMovimentoSaida(loginIdEstornante);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EstornarMovimentoEntrada
        ///</summary>
        [TestMethod()]
        public void EstornarMovimentoEntradaTest()
        {
            MovimentoBusiness target = new MovimentoBusiness(); 
            bool actual;

            target.Movimento = GetMovimento();
            target.Movimento.Id = 60717;
            actual = target.EstornarMovimentoEntrada(1);
        }

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

            movimento.Almoxarifado = new AlmoxarifadoEntity(130);
            movimento.AnoMesReferencia = "201212";
            movimento.Ativo = true;
            movimento.CodigoDescricao = "123";
            movimento.CodigoFormatado = "123";
            movimento.DataMovimento = Convert.ToDateTime("21/12/2012");
            movimento.DataDocumento = Convert.ToDateTime("21/12/2012");
            movimento.DataOperacao = DateTime.Now;
            movimento.Empenho = "";
            movimento.FonteRecurso = String.Empty;

            movimento.IdLogin = 1;
            movimento.Instrucoes = "";

            movimento.NaturezaDespesaEmpenho = "";
            movimento.Fornecedor = new FornecedorEntity(38);

            //movimento.NlLiquidacao = "";
            movimento.NumeroDocumento = "201288888888";
            movimento.Observacoes = "";

            movimento.TipoMovimento = new TipoMovimentoEntity(5);
            movimento.UGE = new UGEEntity(121);
            movimento.ValorDocumento = 123;

            //movimento.Divisao = new DivisaoEntity(641);

            return movimento;
        }

        /// <summary>
        ///A test for ListarMovimentosRecalcular
        ///</summary>
        [TestMethod()]
        public void ListarMovimentosRecalcularTest()
        {
            Sam.Business.MovimentoBusiness target = new Sam.Business.MovimentoBusiness(); 
            Sam.Infrastructure.TB_MOVIMENTO movimento = new Sam.Infrastructure.TB_MOVIMENTO();
            movimento.TB_ALMOXARIFADO = new Sam.Infrastructure.TB_ALMOXARIFADO();
            movimento.TB_ALMOXARIFADO_ID = 130;

            var result = target.ListarMovimentosRecalcular(movimento);
        }

        /// <summary>
        ///A test for VerificaMovimentoTransferencia
        ///</summary>
        [TestMethod()]
        public void VerificaMovimentoTransferenciaTest()
        {
            MovimentoBusiness target = new MovimentoBusiness(); 
            MovimentoEntity mov = new MovimentoEntity();
            bool isEstorno = false;             

            MovimentoItemEntity movItem = new MovimentoItemEntity();
            movItem.Movimento = new MovimentoEntity();
            movItem.UGE = new UGEEntity(14);
            movItem.Movimento.Almoxarifado = new AlmoxarifadoEntity(24);
            movItem.SubItemMaterial = new SubItemMaterialEntity(23770);
            movItem.Movimento.DataMovimento = new DateTime(2013, 01, 01);

            mov.MovimentoItem = new List<MovimentoItemEntity>();
            mov.MovimentoItem.Add(movItem);
            target.Movimento = movItem.Movimento;

            target.VerificaMovimentoModificaPrecoMedio(mov, isEstorno);
        }
    }
}
