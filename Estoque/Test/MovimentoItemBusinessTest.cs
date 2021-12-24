using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for MovimentoItemBusinessTest and is intended
    ///to contain all MovimentoItemBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MovimentoItemBusinessTest
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
        ///A test for ImprimirMovimentacao
        ///</summary>
        [TestMethod()]
        public void ImprimirMovimentacaoTest()
        {
            MovimentoItemBusiness target = new MovimentoItemBusiness(); 
            Nullable<int> _almoxId = new Nullable<int>(149); 
            Nullable<int> _tipoMovimento = new Nullable<int>(5); 
            Nullable<int> _tipoMovimentoAgrup = new Nullable<int>(1); 
            Nullable<int> _fornecedorId = new Nullable<int>(); 
            Nullable<int> _divisaoId = new Nullable<int>(); 
            DateTime _dtInicial = new DateTime(2011, 01, 01); 
            DateTime _dtFinal = new DateTime(2014, 01, 01); 
            IList<MovimentoItemEntity> expected = null; 
            IList<MovimentoItemEntity> actual;
            actual = target.ImprimirMovimentacao(_almoxId, _tipoMovimento, _tipoMovimentoAgrup, _fornecedorId, _divisaoId, _dtInicial, _dtFinal);
                        
        }
    }
}
