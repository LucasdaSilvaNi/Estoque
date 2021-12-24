using Sam.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Domain.Business;
using System.Diagnostics;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoEmpenhoExthensionMethodsTest and is intended
    ///to contain all LiquidacaoEmpenhoExthensionMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoEmpenhoExthensionMethodsTest
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


        ///// <summary>
        /////A test for _xpConverterEmpenhoEventoParaLiquidacaoEvento
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Business.dll")]
        //public void _xpConverterEmpenhoEventoParaLiquidacaoEventoTest()
        //{
        //    GeralEnum.LiquidacaoEvento eventoLiquidacao;
        //    EmpenhoBusiness objBusiness = new EmpenhoBusiness();
        //    MovimentoEntity objMovimento = null;
        //    int[] arrMovIDs = new int[] { 21372, 21402, 21522, 23608, 23781, 23822, 24123, 24139, 24195, 24307, 24327, 24349, 24370, 43644, 43653, 119075 };
        //    string[] arrTipoMovEmpenho = new string[arrMovIDs.Length];

        //    for (int _contador = 0; _contador < arrMovIDs.Length; _contador++)
        //    {
        //        objMovimento = objBusiness.ObterMovimento(arrMovIDs[_contador]);
        //        //arrTipoMovEmpenho[_contador] = objMovimento.RetornarDescricaoEmpenho();

        //        objMovimento.MovimentoItem[0].Movimento = objMovimento;
        //        eventoLiquidacao = LiquidacaoEmpenhoExtensionMethods_Accessor._xpObterEventoEmpenhoParaLiquidacao(objMovimento.MovimentoItem[0]);

        //        Debugger.Log(0, "", "_xpConverterEmpenhoEventoParaLiquidacaoEventoTest" + (" - EventoLiquidacao: " + GeralEnum.GetEnumDescription(eventoLiquidacao)+"\n"));
        //    }

        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for _xpConverterEmpenhoEventoParaLiquidacaoEvento
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Business.dll")]
        //public void _xpConverterEmpenhoEventoParaLiquidacaoEventoTest1()
        //{
        //    MovimentoItemEntity _itemMovimentacao = null; 
        //    GeralEnum.LiquidacaoEvento expected = new GeralEnum.LiquidacaoEvento(); 
        //    GeralEnum.LiquidacaoEvento actual;
        //    actual = LiquidacaoEmpenhoExtensionMethods_Accessor._xpObterEventoEmpenhoParaLiquidacao(_itemMovimentacao);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
