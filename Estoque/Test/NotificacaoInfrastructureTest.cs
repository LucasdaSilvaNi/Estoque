using Sam.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for NotificacaoInfrastructureTest and is intended
    ///to contain all NotificacaoInfrastructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NotificacaoInfrastructureTest
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
        ///A test for ListarNotificacao
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Sam.Infrastructure.dll")]
        public void ListarNotificacaoTest()
        {
            NotificacaoInfrastructure target = new NotificacaoInfrastructure(); 
            
            IList<TB_NOTIFICACAO> expected = null; 
            IList<TB_NOTIFICACAO> actual;
            actual = target.ListarNotificacao(a => a.TB_NOTIFICACAO_IND_ATIVO == true, 20);
            
        }
    }
}
