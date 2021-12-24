using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SaldoSubItemInfrastructureTest and is intended
    ///to contain all SaldoSubItemInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SaldoSubItemInfrastructureTest
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
        ///A test for ImprimirConsultaEstoqueSintetico
        ///</summary>
        [TestMethod()]
        public void ImprimirConsultaEstoqueSinteticoTest()
        {
            SaldoSubItemInfrastructure target = new SaldoSubItemInfrastructure(); 
            int UgeId = 0; 
            int AlmoxId = 0; 
            int GrupoId = 0; 
            int ComSemSaldo = 0; 
            IList<SaldoSubItemEntity> expected = null; 
            IList<SaldoSubItemEntity> actual;
            actual = target.ImprimirConsultaEstoqueSintetico(UgeId, AlmoxId, GrupoId, ComSemSaldo);
        }

        /// <summary>
        ///A test for ImprimirConsumoAlmox
        ///</summary>
        [TestMethod()]
        public void ImprimirConsumoAlmoxTest()
        {
            SaldoSubItemInfrastructure target = new SaldoSubItemInfrastructure(); 
            Nullable<int> pIntAlmoxarifado_ID = new Nullable<int>(0); 
            Nullable<DateTime> dataInicial = new Nullable<DateTime>(new DateTime(2011,01,01)); 
            Nullable<DateTime> dataFinal = new Nullable<DateTime>(new DateTime(2014,01,01)); 
            bool NewMethod = false; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;
            actual = target.ImprimirConsumoAlmox(pIntAlmoxarifado_ID, dataInicial, dataFinal, NewMethod);            
        }
    }
}
