using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for EmpenhoBusinessTest and is intended
    ///to contain all EmpenhoBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmpenhoBusinessTest
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
        ///A test for obterListaEmpenhosSiafem
        ///</summary>
        [TestMethod()]
        public void obterListaEmpenhosSiafemTest()
        {
            EmpenhoBusiness target = new EmpenhoBusiness(); 
            int codigoUGE = 380101; 
            string almoxAnoMesRef = "201410"; 
            string loginSiafemUsuario = "PSIAFEM2014"; 
            string senhaSiafemUsuario = "13NOVEMBRO"; 
            IList<string> expected = null; 
            IList<string> actual;
            actual = target.obterListaEmpenhosSiafem(codigoUGE, almoxAnoMesRef, loginSiafemUsuario, senhaSiafemUsuario);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
