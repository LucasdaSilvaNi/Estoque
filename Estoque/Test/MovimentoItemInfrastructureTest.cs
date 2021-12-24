using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for MovimentoItemInfrastructureTest and is intended
    ///to contain all MovimentoItemInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MovimentoItemInfrastructureTest
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
        ///A test for ListarMovimentacaoItem
        ///</summary>
        [TestMethod()]
        public void ListarMovimentacaoItemTest()
        {
            MovimentoItemInfrastructure target = new MovimentoItemInfrastructure(); 
            Nullable<int> AlmoxId = new Nullable<int>(149); 
            Nullable<long> SubItemMatId = new Nullable<long>(27134); 
            Nullable<int> UgeId = new Nullable<int>(152); 
            DateTime DtInicial = new DateTime(2011,01,01); 
            DateTime DtFinal = new DateTime(2014,01,01); 
            bool comEstorno = false;             
            IList<MovimentoItemEntity> actual;
            actual = target.ListarMovimentacaoItem(AlmoxId, SubItemMatId, UgeId, DtInicial, DtFinal, comEstorno);
        }
    }

}
