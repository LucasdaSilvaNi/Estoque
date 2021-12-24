using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for FechamentoMensalInfrastructureTest and is intended
    ///to contain all FechamentoMensalInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FechamentoMensalInfrastructureTest
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
        ///A test for Imprimir
        ///</summary>
        [TestMethod()]
        public void ImprimirTest()
        {
            FechamentoMensalInfrastructure target = new FechamentoMensalInfrastructure(); 
            IList<FechamentoMensalEntity> expected = null; 
            IList<FechamentoMensalEntity> actual;

            target.Entity = new FechamentoMensalEntity();
            target.Entity.SituacaoFechamento = 1;
            target.Entity.Almoxarifado = new AlmoxarifadoEntity(149);
            target.Entity.SubItemMaterial = new SubItemMaterialEntity(27134);
            target.Entity.SubItemMaterial.NaturezaDespesa = new NaturezaDespesaEntity(0);
            target.Entity.AnoMesRef = 201209;

            actual = target.Imprimir();
        }

        /// <summary>
        ///A test for ListarUltimoFechamento
        ///</summary>
        [TestMethod()]
        public void ListarUltimoFechamentoTest()
        {
            FechamentoMensalInfrastructure target = new FechamentoMensalInfrastructure(); 
            target.ListarUltimoFechamento(149);
            
        }

        /// <summary>
        ///A test for ListarMesesFechados
        ///</summary>
        [TestMethod()]
        public void ListarMesesFechadosTest()
        {
            FechamentoMensalInfrastructure target = new FechamentoMensalInfrastructure(); 
            int almoxId = 24; 
            IList<string> actual;
            actual = target.ListarMesesFechados(almoxId);
        }
    }
}
