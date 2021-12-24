using Sam.Web.Almoxarifado;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Sam.Domain.Entity;
using Sam.Domain.Business.SIAFEM;
using Sam.Presenter;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoExthensionsMethodsTest and is intended
    ///to contain all LiquidacaoExthensionsMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoExthensionsMethodsTest
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
        ///A test for NotasLiquidacaoSiafisico
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Projetos\\Prodesp - SAMweb\\Fontes\\Desenvolvimento\\main\\Fontes\\Sam.Web", "/")]
        //[UrlToTest("http://localhost:8818/")]
        public void NotasLiquidacaoSiafisicoTest()
        {
            //LiquidacaoBusiness_Accessor objBusiness = new LiquidacaoBusiness_Accessor();
            LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();
            IList<MovimentoEntity> lstMovimentacoes = null;
            int almoxID = 175;
            string anoMesRef = "201501";
            string codigoEmpenho = "2015NE00002";
            string _nlsEmpenho = null;

            //lstMovimentacoes = objPresenter.ListarMovimentacoesEmpenho(almoxID, -1, anoMesRef, codigoEmpenho, false);
            //lstMovimentacoes = Sam.Domain.Business.SIAFEM.ExtensionMethods.ParticionadorPorNumeroDocumento(lstMovimentacoes);

            var _listaNLs = objPresenter.ObterNLsPagamentoEmpenho(almoxID, anoMesRef, codigoEmpenho);
            foreach (var _nl in _listaNLs)
                _nlsEmpenho += _nl;

            System.Diagnostics.Debug.Write(_nlsEmpenho);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
