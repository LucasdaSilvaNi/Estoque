using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SaldoSubItemBusinessTest and is intended
    ///to contain all SaldoSubItemBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SaldoSubItemBusinessTest
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
        ///A test for ImprimirConsultaEstoqueSintetico
        ///</summary>
        [TestMethod()]
        public void ImprimirConsultaEstoqueSinteticoTest()
        {
            SaldoSubItemBusiness target = new SaldoSubItemBusiness(); 
            int UgeId = 0; 
            int AlmoxId = 174; 
            int GrupoId = 0; 
            int ComSemSaldo = 0; 
            IList<SaldoSubItemEntity> expected = null; 
            IList<SaldoSubItemEntity> actual;
            actual = target.ImprimirConsultaEstoqueSintetico(UgeId, AlmoxId, GrupoId, ComSemSaldo, 0);            
        }

        /// <summary>
        ///A test for VerificaSubItemUtilizado
        ///</summary>
        [TestMethod()]
        public void VerificaSubItemUtilizadoTest()
        {
            SaldoSubItemBusiness target = new SaldoSubItemBusiness(); 
            int subItemId = 26820;             
            bool actual;
            actual = target.VerificaSubItemUtilizado(subItemId);            
        }

        /// <summary>
        ///A test for AnalisarFechamentoMensal
        ///</summary>
        [TestMethod()]
        public void AnalisarFechamentoMensalTest()
        {
            SaldoSubItemBusiness target = new SaldoSubItemBusiness(); 
            Nullable<int> almoxId = new Nullable<int>(130); 
            Nullable<int> anomes = new Nullable<int>(201306); 

            IList<SaldoSubItemEntity> actual;
            actual = target.AnalisarFechamentoMensal(almoxId, anomes);            

            MovimentoItemEntity movimentoItem = new MovimentoItemEntity();
            movimentoItem.Movimento = new MovimentoEntity();
            movimentoItem.UGE = new UGEEntity(actual[0].UGE.Id);
            movimentoItem.SubItemMaterial = new SubItemMaterialEntity(actual[0].SubItemMaterial.Id);
            movimentoItem.Movimento.Almoxarifado = new AlmoxarifadoEntity(actual[0].Almoxarifado.Id);
            movimentoItem.Movimento.DataMovimento = new DateTime(2013, 06, 30);
            movimentoItem.Id = 999999999;

            var MovimentoItem = new MovimentoBusiness().ConsultaSaldoMovimentoItem(movimentoItem);

            foreach (var a in actual)
            {
                if (a.SubItemMaterial.Id == MovimentoItem.SubItemMaterial.Id)
                {
                    if (a.SaldoQtde == movimentoItem.SaldoQtde)
                    {
                    }
                    else
                    {
                    }
                }
            }
            
        }
    }
}
