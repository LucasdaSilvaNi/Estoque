using Sam.Presenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Infrastructure;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for ItemNaturezaDespesaPresenterTest and is intended
    ///to contain all ItemNaturezaDespesaPresenterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemNaturezaDespesaPresenterTest
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
        ///A test for PopularDadosItemNatureza
        ///</summary>
        [TestMethod()]
        public void PopularDadosItemNaturezaTest()
        {
            ItemNaturezaDespesaPresenter target = new ItemNaturezaDespesaPresenter(); 
            int ItemMaterialId = 26820; 
            int startRowIndexParameterName = 0; 
            int maximumRowsParameterName = 0; 
            
            IList<TB_ITEM_NATUREZA_DESPESA> actual;
            actual = target.PopularDadosItemNatureza(ItemMaterialId, startRowIndexParameterName, maximumRowsParameterName);
            
        }
    }
}
