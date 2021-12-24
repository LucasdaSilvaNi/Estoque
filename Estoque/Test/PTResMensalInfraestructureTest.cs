using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for PTResMensalInfraestructureTest and is intended
    ///to contain all PTResMensalInfraestructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PTResMensalInfraestructureTest
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
        ///A test for _xpGerarNotasFechamentoMensalParaConsumo
        ///</summary>
        [TestMethod()]
        public void ProcessarNLsConsumoAlmoxTest()
        {
            PTResMensalInfraestructure target = new PTResMensalInfraestructure(); 
            int almoxID = 174; 
            int anoMesRef = 201309; 
            IList<PTResMensalEntity> expected = null; 
            IList<PTResMensalEntity> actual;
            actual = target.ProcessarNLsConsumoAlmox(almoxID, anoMesRef);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
