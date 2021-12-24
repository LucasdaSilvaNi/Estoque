using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using Sam.Presenter;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for FechamentoMensalBusinessTest and is intended
    ///to contain all FechamentoMensalBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FechamentoMensalBusinessTest
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
        ///A test for EstornarFechamentoMensal
        ///</summary>
        [TestMethod()]
        public void EstornarFechamentoMensalTest()
        {
            FechamentoMensalBusiness target = new FechamentoMensalBusiness(); 
            Sam.Domain.Entity.FechamentoMensalEntity objEntidade = new Sam.Domain.Entity.FechamentoMensalEntity() { Almoxarifado = new Sam.Domain.Entity.AlmoxarifadoEntity(149) };
            target.Fechamento = objEntidade;

            target.EstornarFechamentoMensal();
        }

        /// <summary>
        ///A test for ProcessaConsumoPtResMensal
        ///</summary>
        [TestMethod()]
        public void ProcessaConsumoPtResMensalTest()
        {
            FechamentoMensalPresenter objPresenter = new FechamentoMensalPresenter();
            FechamentoMensalBusiness target = new FechamentoMensalBusiness(); 
            int pIntAlmoxarifadoLogadoID = 0; 
            int pIntGestorAlmoxarifadoLogadoID = 0; 
            int pIntAnoMesReferenciaAlmoxarifadoLogado = 0; 
            IList<PTResMensalEntity> expected = null; 
            IList<PTResMensalEntity> actual;
            actual = target.ProcessaConsumoPtResMensal(pIntAlmoxarifadoLogadoID, pIntGestorAlmoxarifadoLogadoID, pIntAnoMesReferenciaAlmoxarifadoLogado);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for _xpProcessaConsumoAlmoxarifado
        /////</summary>
        //[TestMethod()]
        //public void _xpProcessaConsumoAlmoxarifadoTest()
        //{
        //    FechamentoMensalBusiness target = new FechamentoMensalBusiness(); 
        //    int almoxID = 147; 
        //    int anoMesRefAlmoxarifado = 201309; 
        //    string loginSiafem = "35593145857"; 
        //    string senhaSiafem = "cocacola"; 
        //    IList<PTResMensalEntity> expected = null; 
        //    IList<PTResMensalEntity> actual;
        //    actual = target.(almoxID, anoMesRefAlmoxarifado, loginSiafem, senhaSiafem);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
