using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;
using System.Linq;
using Sam.Business;
using Sam.Common.Util;
using Sam.Presenter;


namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SiafemTest and is intended
    ///to contain all SiafemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SiafemTest
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

        public void SFCOLiquidaNL_UGE_380149_GESTAO_00001_NUMERONE_2014NE00731()
        {
            string descMsgErro = string.Empty;
            bool _tstSaida = Siafem.VerificarErroMensagem(FakeReturnXml.SFCOLiquidaNL_UGE_380149_GESTAO_00001_NUMERONE_2014NE00731(), out descMsgErro);
        }

        /// <summary>
        ///A test for VerificarErroMensagem
        ///</summary>
        [TestMethod()]
        public void VerificarErroMensagemTest()
        {
            string strRetornoMsgEstimulo = null;
            string strNomeMsgEstimulo = string.Empty;
            string strNomeMsgEstimuloExpected = string.Empty;
            string strMensagemErro = string.Empty;
            string strMensagemErroExpected = string.Empty;
            bool expected = false;
            bool actual;

            strRetornoMsgEstimulo = FakeReturnXml.SFCOLiquidaNL_UGE_380101_GESTAO_00001_NUMERONE_2015NE00003__NUMERO_NL_2015NL00008();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.errSiafemLogin_ACESSO_NAO_PERMITIDO();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.errSiafemLogin_ERRO_DE_EXECUCAO();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.gerarSFCONLPregao_UGE_380149_GESTAO_00001_NUMERONE_2014NE00024();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.SiafemDocDetaContaGen_UGE_441101_GESTAO_44047_MES_2012DEZ();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.SiafisicoDocConsultaI_ItemMaterial_004152603();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);

            strRetornoMsgEstimulo = FakeReturnXml.errRetornoVazio();
            actual = Siafem.VerificarErroMensagem(strRetornoMsgEstimulo, out strNomeMsgEstimulo, out strMensagemErro);
            //Assert.AreEqual(strNomeMsgEstimuloExpected, strNomeMsgEstimulo);
            //Assert.AreEqual(strMensagemErroExpected, strMensagemErro);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
