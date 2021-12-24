using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;

namespace Test
{


    /// <summary>
    ///This is a test class for MovimentoInfrastructureTest and is intended
    ///to contain all MovimentoInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MovimentoInfrastructureTest
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
        ///A test for ConsultaSaldoMovimentoItem
        ///</summary>
        [TestMethod()]
        public void ConsultaSaldoMovimentoItemTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoItemEntity movItem = new MovimentoItemEntity();
            movItem.Movimento = new MovimentoEntity();
            movItem.SubItemMaterial = new SubItemMaterialEntity(31040);
            movItem.Movimento.Almoxarifado = new AlmoxarifadoEntity(93);
            movItem.UGE = new UGEEntity(86);

            MovimentoItemEntity actual;
            actual = target.ConsultaSaldoMovimentoItem(movItem);

            System.Diagnostics.Debug.WriteLine(actual);
        }

        /// <summary>
        ///A test for GetMovimento
        ///</summary>
        [TestMethod()]
        public void GetMovimentoTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoEntity expected = null; 
            MovimentoEntity actual;

            target.Entity = new MovimentoEntity();
            target.Entity.Id = 60264;

            actual = target.GetMovimento();
        }

        /// <summary>
        ///A test for ListarMovimentosRecalcular
        ///</summary>
        [TestMethod()]
        public void ListarMovimentosRecalcularTest()
        {
            Sam.Infrastructure.MovimentoInfrastructure target = new Sam.Infrastructure.MovimentoInfrastructure();

            Sam.Infrastructure.TB_MOVIMENTO movimento = new Sam.Infrastructure.TB_MOVIMENTO();
            movimento.TB_ALMOXARIFADO_ID = 130;
            IList<TB_MOVIMENTO> expected = null;
            IList<TB_MOVIMENTO> actual;

            target.ListarMovimentosRecalcular(130);

        }

        /// <summary>
        ///A test for BuscarItemMaterial
        ///</summary>
        [TestMethod()]
        public void BuscarItemMaterialTest()
        {
            Sam.Infrastructure.MovimentoInfrastructure target = new Sam.Infrastructure.MovimentoInfrastructure(); 
            int startIndex = 0; 
            string palavraChave = string.Empty; 
            Sam.Infrastructure.TB_MOVIMENTO movimento = new Sam.Infrastructure.TB_MOVIMENTO(); 

            movimento.TB_ALMOXARIFADO_ID = 130;
            movimento.TB_TIPO_MOVIMENTO_ID = 5;           

            IList<Sam.Infrastructure.TB_MOVIMENTO> actual;
            //actual = target.ListarMovimentos(startIndex, palavraChave, movimento);
        }

        /// <summary>
        ///A test for ListarTodosCod
        ///</summary>
        [TestMethod()]
        public void ListarTodosCodTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoEntity mov = new MovimentoEntity(); 
            mov.Id = 0;
            //mov.Fornecedor = new FornecedorEntity(1);
            //mov.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(1);
            //mov.Divisao = new DivisaoEntity(1);
            //mov.UGE = new UGEEntity(1);
            //mov.TipoMovimento = new TipoMovimentoEntity(1);
            mov.NumeroDocumento = "";
            mov.Ativo = true;
            mov.Empenho = "";

            IList<MovimentoEntity> actual;
            // actual = target.ListarTodosCod(mov);
            
        }

        /// <summary>
        ///A test for ConsultaSaldoMovimentoItem
        ///</summary>
        [TestMethod()]
        public void ConsultaSaldoMovimentoItemTest1()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoItemEntity movItem = null; 
            MovimentoItemEntity expected = null; 
            MovimentoItemEntity actual;
            actual = target.ConsultaSaldoMovimentoItem(movItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NumeroUltimoDocumento
        ///</summary>
        [TestMethod()]
        public void NumeroUltimoDocumentoTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            string expected = string.Empty; 
            string actual;
            actual = target.NumeroUltimoDocumento();
        }

        /// <summary>
        ///A test for VerificaMovimentoTransferencia
        ///</summary>
        [TestMethod()]
        public void VerificaMovimentoTransferenciaTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoItemEntity movItem = new MovimentoItemEntity();
            movItem.Movimento = new MovimentoEntity();
            movItem.Movimento.Almoxarifado = new AlmoxarifadoEntity(24);
            
            movItem.SubItemMaterial = new SubItemMaterialEntity(23770);
            movItem.Movimento.DataMovimento = new DateTime(2013, 01, 01);
            movItem.ValorMov = 350;
            movItem.QtdeMov = 300;            

            
            bool actual;
            actual = target.VerificaMovimentoModificaPrecoMedio(movItem, true);
            
        }

        /// <summary>
        ///A test for VerificaMovimentoTransferencia
        ///</summary>
        [TestMethod()]
        public void VerificaMovimentoTransferenciaTest1()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoItemEntity movItem = null; 
            bool expected = false; 
            bool actual;
            actual = target.VerificaMovimentoModificaPrecoMedio(movItem, true);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for VerificaMovimentoTransferencia
        ///</summary>
        [TestMethod()]
        public void VerificaMovimentoTransferenciaTest2()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoItemEntity movItem = null; 
            bool expected = false; 
            bool actual;
            actual = target.VerificaMovimentoModificaPrecoMedio(movItem, true);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ListarTodosCodSimplif
        ///</summary>
        [TestMethod()]
        public void ListarTodosCodSimplifTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure(); 
            MovimentoEntity mov = null; 
            
            IList<MovimentoEntity> actual;
            actual = target.ListarTodosCodSimplif(mov);            
        }

        /// <summary>
        ///A test for ListarMovimentacoesEmpenho
        ///</summary>
        [TestMethod()]
        public void ListarMovimentacoesEmpenhoTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure();
            int almoxID = 49;
            int fornecedorID = 13; 
            string codigoEmpenho = "2013NE00001";
            bool empenhosNaoLiquidados = true; 
            string anoMesRef = "201305";
            IList<MovimentoEntity> listaMovEmpenho;
            IList<MovimentoEntity> listaEmpenhoFornecedor;
            string[] arrEmpenhos = null;
            string[] arrDocumentos = null;


            listaMovEmpenho = target.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            arrDocumentos = listaMovEmpenho.ToList()
                                           .Select(_nomvimentacao => _nomvimentacao.NumeroDocumento)
                                           .Distinct()
                                           .ToArray();

            var oi = "";
            if (!arrDocumentos.IsNullOrEmpty())
                oi = arrDocumentos[0];

            codigoEmpenho = null;
            fornecedorID = 4;
            almoxID = 126;
            anoMesRef = "201308";
            listaEmpenhoFornecedor = target.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            arrEmpenhos = listaEmpenhoFornecedor.ToList()
                                         .Select(_nomvimentacao => _nomvimentacao.Empenho)
                                         .ToList()
                                         .Distinct()
                                         .ToArray();

            if (!arrEmpenhos.IsNullOrEmpty())
                oi = arrEmpenhos[0];
        }

        /// <summary>
        ///A test for ObterNLsPagamentoEmpenho
        ///</summary>
        [TestMethod()]
        public void ObterNLsPagamentoEmpenhoTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure();
            int almoxID = 178; 
            string anoMesRef = "201501";
            string codigoEmpenho = "2015NE00003";
            IList<string> expected = null;
            IList<string> actual;

            actual = target.ObterNLsPagamentoEmpenho(almoxID, anoMesRef, codigoEmpenho);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
