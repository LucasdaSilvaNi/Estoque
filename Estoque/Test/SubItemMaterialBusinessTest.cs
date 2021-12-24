using Sam.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Infrastructure;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SubItemMaterialBusinessTest and is intended
    ///to contain all SubItemMaterialBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubItemMaterialBusinessTest
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
        ///A test for BuscaSubItemMaterial
        ///</summary>
        [TestMethod()]
        public void BuscaSubItemMaterialTest()
        {
            SubItemMaterialBusiness target = new SubItemMaterialBusiness(); 
            int startIndex = 0; 
            string palavraChave = "ALCOOL"; 
            bool filtraAlmoxarifado = false; 
            bool filtraGestor = false; 
            bool comSaldo = false; 

            var almox = new TB_ALMOXARIFADO();
            almox.TB_ALMOXARIFADO_ID = 130;
            almox.TB_GESTOR_ID = 1;


            IList<TB_SUBITEM_MATERIAL> expected = null;
            IList<TB_SUBITEM_MATERIAL> actual;

            int count1;
            int count2;
            int count3;
            int count4;
            int total;
            

            //teste1 : 17374
            actual = target.BuscaSubItemMaterial(startIndex, palavraChave, filtraAlmoxarifado, filtraGestor, comSaldo, almox);
            count1 = actual.Count;
            total = target.TotalRegistros;

            actual = target.BuscaSubItemMaterial(startIndex, palavraChave, true, filtraGestor, comSaldo, almox);
            count2 = actual.Count;
            total = target.TotalRegistros;

            actual = target.BuscaSubItemMaterial(startIndex, palavraChave, filtraAlmoxarifado, true, comSaldo, almox);
            count3 = actual.Count;
            total = target.TotalRegistros;

            actual = target.BuscaSubItemMaterial(startIndex, palavraChave, filtraAlmoxarifado, false, true, almox);
            count4 = actual.Count;
            total = target.TotalRegistros;
            
        }
    }
}
