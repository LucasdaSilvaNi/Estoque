using Sam.Presenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Collections.Generic;
using Sam.View;
using System.Diagnostics;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoEmpenhoPresenterTest and is intended
    ///to contain all LiquidacaoEmpenhoPresenterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoEmpenhoPresenterTest
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
        ///A test for ObterMovimento
        ///</summary>
        [TestMethod()]
        public void ObterMovimentoTest()
        {
            LiquidacaoEmpenhoPresenter target = new LiquidacaoEmpenhoPresenter(); 
            int movID = 119047; 
            MovimentoEntity objMovimento = null; 
            MovimentoEntity expected = null; 
            MovimentoEntity actual;
            objMovimento = target.ObterMovimento(movID);

            //string strRetorno = ExtensionMethods._xpObterDescricaoTipoEmpenho(objMovimento);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProcessarMovimentacoesEmpenho
        ///</summary>
        [TestMethod()]
        public void ProcessarMovimentacoesEmpenhoTest()
        {
            LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();
            IList<MovimentoEntity> lstMovimentacoes = null;
            IList<string> lstNlPagas = null;
            string loginSiafem = "PSIAFISIC15";
            string senhaSiafem = "13NOVEMBRO";
            string loginSAM = "loginSAM";
            int almoxID = 55;//175;
            string anoMesRef = "201411";//"201501";
            string codigoEmpenho = "2014NE00584";// "2015NE00002";


            lstMovimentacoes = objPresenter.ListarMovimentacoesEmpenho(almoxID, -1, anoMesRef, codigoEmpenho, true);
            lstMovimentacoes = Sam.Domain.Business.SIAFEM.ExtensionMethods.ParticionadorPorNumeroDocumento(lstMovimentacoes);

            lstNlPagas = objPresenter.ProcessarMovimentacoesEmpenho(lstMovimentacoes, loginSiafem, senhaSiafem, loginSAM);
            string nlLiquidacao = null;
            foreach (var _nlLiquidacao in lstNlPagas)
                nlLiquidacao += String.Format("\\r\\n{0},", _nlLiquidacao);//, _nlLiquidacao.BreakLine('|', 4));

            Debug.AutoFlush = true;
            Debug.Write(String.Format("Gerado(s) NL(s) {0} no SIAFEM.", nlLiquidacao));
            
            
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
