using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for CatalogoBusinessTest and is intended
    ///to contain all CatalogoBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CatalogoBusinessTest
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
        ///A test for ListarSubItemAlmoxPorPalavraChave
        ///</summary>
        [TestMethod()]//Refazer 22 registros
        public void ListarSubItemAlmoxPorPalavraChaveTest()
        {
            CatalogoBusiness target = new CatalogoBusiness(); 
            Nullable<long> codigo = new Nullable<long>(); 
            string descricao = string.Empty; 
            AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(0); 
            almoxarifado.Gestor = new GestorEntity(0);
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            //actual = target.ListarSubItemAlmoxPorPalavraChave(codigo, descricao, almoxarifado);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ListarSubItemMaterialTodosCod
        ///</summary>
        [TestMethod()]
        public void ListarSubItemMaterialTodosCodTest()
        {
            CatalogoBusiness target = new CatalogoBusiness(); 

            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            //actual = target.ListarSubItemMaterialTodosCod();
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
