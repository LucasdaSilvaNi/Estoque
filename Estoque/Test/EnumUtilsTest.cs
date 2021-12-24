using Sam.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for EnumUtilsTest and is intended
    ///to contain all EnumUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EnumUtilsTest
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
        ///A test for GetEnumDescription
        ///</summary>
        public void GetEnumDescriptionTestHelper<T>()
        {
            string expected = string.Empty;
            string actual;

            TipoMovimento value = TipoMovimento.EntradaEmpenho;


            actual = EnumUtils_Accessor.GetEnumDescription<TipoMovimento>(value);

            value = Sam.Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa;
            actual = EnumUtils_Accessor.GetEnumDescription<TipoMovimento>(value);

            value = Sam.Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada;
            actual = EnumUtils_Accessor.GetEnumDescription<TipoMovimento>(value);

        }

        [TestMethod()]
        [DeploymentItem("Sam.Common.dll")]
        public void GetEnumDescriptionTest()
        {
            GetEnumDescriptionTestHelper<GenericParameterHelper>();
        }
    }
}
