using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for ItemMaterialInfraestructureTest and is intended
    ///to contain all ItemMaterialInfraestructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemMaterialInfraestructureTest
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
        ///A test for ListarItemSaldoByAlmox
        ///</summary>
        [TestMethod()]
        public void ListarItemSaldoByAlmoxTest()
        {
            ItemMaterialInfraestructure target = new ItemMaterialInfraestructure(); 
            int almoxarifado = 130; 
            IList<ItemMaterialEntity> expected = null; 
            IList<ItemMaterialEntity> actual;
            actual = target.ListarItemSaldoByAlmox(almoxarifado);

            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ListarPorPalavraChaveTodosCod
        ///</summary>
        [TestMethod()] //metodo com problema
        public void ListarPorPalavraChaveTodosCodTest()
        {
            ItemMaterialInfraestructure target = new ItemMaterialInfraestructure(); 
            Nullable<int> Id = new Nullable<int>(); 
            Nullable<int> Codigo = new Nullable<int>(); 
            Codigo = 0;
            string Descricao = string.Empty; 
            Nullable<int> AlmoxId = 0; 
            Nullable<int> GestorId = 1; 
            IList<ItemMaterialEntity> expected = null; 
            IList<ItemMaterialEntity> actual;
            actual = target.ListarPorPalavraChaveTodosCod(Id, Codigo, Descricao, AlmoxId, GestorId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
