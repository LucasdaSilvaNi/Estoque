using Sam.Web.Almoxarifado;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoEmpenhoTest and is intended
    ///to contain all LiquidacaoEmpenhoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoEmpenhoTest
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
        ///A test for RetornarNaturezaDespesa
        ///</summary>
        [TestMethod()]
        public void RetornarNaturezaDespesaTest()
        {
            IList<MovimentoEntity> lstMovimentacoes = null; 
            string expected = string.Empty; 
            string actual;

            lstMovimentacoes = new List<MovimentoEntity>() { { new MovimentoEntity() { Id = 11, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 12, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 13, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 14, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 15, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                           };

            //actual = LiquidacaoEmpenho.RetornarNaturezaDespesa(lstMovimentacoes);
            actual = lstMovimentacoes.RetornarNaturezaDespesaEmpenho();


            lstMovimentacoes = new List<MovimentoEntity>() { { new MovimentoEntity() { Id = 11, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 12, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 13, NaturezaDespesaEmpenho = "33903011 - GENEROS ALIMENTICIOS PPAIS - LEI 14.591/11" } }, 
                                                             { new MovimentoEntity() { Id = 14, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 15, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                           };

            actual = lstMovimentacoes.RetornarNaturezaDespesaEmpenho();

            lstMovimentacoes = new List<MovimentoEntity>() { { new MovimentoEntity() { Id = 11, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 12, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 14, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 15, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 13, NaturezaDespesaEmpenho = "33903011 - GENEROS ALIMENTICIOS PPAIS - LEI 14.591/11" } }, 
                                                           };

            actual = lstMovimentacoes.RetornarNaturezaDespesaEmpenho();

            lstMovimentacoes = new List<MovimentoEntity>() { { new MovimentoEntity() { Id = 11, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 12, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 13, NaturezaDespesaEmpenho = "33903011 - GENEROS ALIMENTICIOS PPAIS - LEI 14.591/11" } }, 
                                                             { new MovimentoEntity() { Id = 15, NaturezaDespesaEmpenho = "33903010 - GENEROS ALIMENTICIOS" } }, 
                                                             { new MovimentoEntity() { Id = 13, NaturezaDespesaEmpenho = "33903011 - GENEROS ALIMENTICIOS PPAIS - LEI 14.591/11" } }, 
                                                           };

            actual = lstMovimentacoes.RetornarNaturezaDespesaEmpenho();

            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PagarEmpenhoSiafisico
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projetos\\Prodesp - SAMweb\\Fontes\\Desenvolvimento\\main\\Fontes\\Sam.Web", "/")]
        [UrlToTest("http://localhost:8818/")]
        [DeploymentItem("Sam.Web.dll")]
        public void PagarEmpenhoSiafisicoTest()
        {
            LiquidacaoEmpenho_Accessor target = new LiquidacaoEmpenho_Accessor();
            target.PagarEmpenhoSiafisico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
