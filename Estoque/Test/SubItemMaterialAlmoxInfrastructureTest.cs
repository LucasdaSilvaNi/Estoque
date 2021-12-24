using Sam.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SubItemMaterialAlmoxInfrastructureTest and is intended
    ///to contain all SubItemMaterialAlmoxInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubItemMaterialAlmoxInfrastructureTest
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
        ///A test for SubItemMaterialAlmoxInfrastructure Constructor
        ///</summary>
        [TestMethod()]
        public void SubItemMaterialAlmoxInfrastructureConstructorTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for MapearDTO
        ///</summary>
        [TestMethod()]
        public void MapearDTOTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            TB_SUBITEM_MATERIAL_ALMOX rowTabela = null; 
            bool convertToSubItemMaterial = false; 
            SubItemMaterialEntity expected = null; 
            SubItemMaterialEntity actual;
            actual = target.MapearDTO(rowTabela, convertToSubItemMaterial);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MapearDTO
        ///</summary>
        [TestMethod()]
        public void MapearDTOTest1()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            TB_SUBITEM_MATERIAL_ALMOX rowTabela = null; 
            SubItemMaterialAlmoxEntity expected = null; 
            SubItemMaterialAlmoxEntity actual;
            actual = target.MapearDTO(rowTabela);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MapearEntity
        ///</summary>
        [TestMethod()]
        public void MapearEntityTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            SubItemMaterialAlmoxEntity objEntidade = null; 
            TB_SUBITEM_MATERIAL_ALMOX expected = null; 
            TB_SUBITEM_MATERIAL_ALMOX actual;
            actual = target.MapearEntity(objEntidade);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa
        ///</summary>
        [TestMethod()]
        public void ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesaTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int Almoxarifado_ID = 0; 
            int NaturezaDespesa_ID = 0; 
            int startRowIndex = 0; 
            int maximumRows = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            actual = target.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(Almoxarifado_ID, NaturezaDespesa_ID, startRowIndex, maximumRows);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa
        ///</summary>
        [TestMethod()]
        public void ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesaTest1()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int Almoxarifado_ID = 149; 
            int NaturezaDespesa_ID = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            actual = target.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(Almoxarifado_ID, NaturezaDespesa_ID);
        }

        /// <summary>
        ///A test for SelectSubItemAlmoxByDivisao
        ///</summary>
        [TestMethod()]
        public void SelectSubItemAlmoxByDivisaoTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int maximumRows = 0; 
            int startRowIndex = 0; 
            int divisaoId = 0; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> expected = null; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> actual;
            actual = target.SelectSubItemAlmoxByDivisao(maximumRows, startRowIndex, divisaoId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectSubItemAlmoxByDivisao
        ///</summary>
        [TestMethod()]
        public void SelectSubItemAlmoxByDivisaoTest1()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int divisaoId = 0; 
            string pesquisa = string.Empty; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> expected = null; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> actual;
            actual = target.SelectSubItemAlmoxByDivisao(divisaoId, pesquisa);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectSubItemAlmoxByDivisao
        ///</summary>
        [TestMethod()]
        public void SelectSubItemAlmoxByDivisaoTest2()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int divisaoId = 0; 
            string pesquisa = string.Empty; 
            bool possuiSaldo = false; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> expected = null; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> actual;
            actual = target.SelectSubItemAlmoxByDivisao(divisaoId, pesquisa, possuiSaldo);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa
        ///</summary>
        [TestMethod()]
        public void ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesaTest2()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int Almoxarifado_ID = 0; 
            int NaturezaDespesa_ID = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            actual = target.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(Almoxarifado_ID, NaturezaDespesa_ID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ObterSubItensMaterialAlmoxarifadoPorDivisao
        ///</summary>
        [TestMethod()]
        public void ObterSubItensMaterialAlmoxarifadoPorDivisaoTest()
        {
            SubItemMaterialAlmoxInfrastructure target = new SubItemMaterialAlmoxInfrastructure(); 
            int DivisaoId = 5066; 
            string textoPesquisa = string.Empty; 
            bool possuiSaldo = false; 

            IList<TB_SUBITEM_MATERIAL_ALMOX> expected = null; 
            IList<TB_SUBITEM_MATERIAL_ALMOX> actual;
            actual = target.ObterSubItensMaterialAlmoxarifadoPorDivisao(DivisaoId, textoPesquisa, null);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
