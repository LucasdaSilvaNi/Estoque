using Sam.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for ItemMaterialInfrastructureTest and is intended
    ///to contain all ItemMaterialInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemMaterialInfrastructureTest
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
        ///A test for BuscarItemMaterial
        ///</summary>
        [TestMethod()]
        public void BuscarItemMaterialTest()
        {
            ItemMaterialInfrastructure target = new ItemMaterialInfrastructure(); 
            int startIndex = 0; 
            string palavraChave = string.Empty; 
            //IList<TB_ITEM_MATERIAL> expected = null; 
            IList<TB_ITEM_MATERIAL> actual;

            int count;

            actual = target.BuscarItemMaterial(startIndex, palavraChave);
            count = target.TotalRegistros;

            System.Diagnostics.Debug.WriteLine(String.Format("Total Busca: {0} - Count: {1}", actual.Count, count));
            
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
