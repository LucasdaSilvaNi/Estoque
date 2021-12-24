using Sam.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for FakeReturnXmlTest and is intended
    ///to contain all FakeReturnXmlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FakeReturnXmlTest
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
        ///A test for SiafemDocNLConsumo_UGE_200162_UA_CONSUMIDORA_022522_GESTAO_00001_DATA_EMISSAO_07_MAI_2008
        ///</summary>
        [TestMethod()]
        public void SiafemDocNLConsumo_UGE_200162_UA_CONSUMIDORA_022522_GESTAO_00001_DATA_EMISSAO_07_MAI_2008Test()
        {
            string expected = string.Empty; 
            string actual;
            actual = FakeReturnXml.SiafemDocNLConsumo_UGE_200162_UA_CONSUMIDORA_022522_GESTAO_00001_DATA_EMISSAO_07_MAI_2008();
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SiafemDocNLConsumo_OpFlex
        ///</summary>
        [TestMethod()]
        public void SiafemDocNLConsumo_OpFlexTest()
        {
            string pStrMsgEstimulo = string.Empty; 
            string expected = string.Empty; 
            string actual;
            actual = FakeReturnXml.SiafemDocNLConsumo_OpFlex(pStrMsgEstimulo);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}