using Sam.Domain.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using Sam.Presenter;
using System.Collections.Generic;
using Sam.Domain.Infrastructure;
using DTOs = Sam.Domain.Entity;
using System.Linq;
using _outrageous = Sam.Common.Util;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for ExtensionMethodsTest and is intended
    ///to contain all ExtensionMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExtensionMethodsTest
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
        ///A test for _xpObterDescricaoTipoEmpenho
        ///</summary>
        [TestMethod()]
        public void _xpObterDescricaoTipoEmpenhoTest()
        {
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            MovimentoEntity objMovimento = null; 
            string expected = string.Empty; 
            string actual;

            objMovimento = objBusiness.ObterMovimento(119047);
            //actual = ExtensionMethods._xpObterDescricaoTipoEmpenho(objMovimento);
            actual = objMovimento.ToString();
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for _xpObterDescricaoTipoEmpenho02
        ///</summary>
        [TestMethod()]
        public void _xpObterDescricaoTipoEmpenho02Test()
        {
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            MovimentoEntity objMovimento = null;
            int[] arrMovIDs = new int[] { 21372, 21402, 21522, 23608, 23781, 23822, 24123, 24139, 24195, 24307, 24327, 24349, 24370, 43644, 43653, 119075 };
            string[] arrTipoMovEmpenho = new string[arrMovIDs.Length];

            for (int _contador = 0; _contador < arrMovIDs.Length; _contador++)
			{
                objMovimento = objBusiness.ObterMovimento(arrMovIDs[_contador]);
                arrTipoMovEmpenho[_contador] = objMovimento.RetornarDescricaoEmpenho();
			}
        }

        /// <summary>
        ///A test for ObterDescricaoTipoLicitacao
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Sam.Domain.Business.dll")]
        public void ObterDescricaoTipoLicitacaoTest()
        {
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            MovimentoEntity objMovimento = null;
            int[] arrMovIDs = new int[] { 21372, 21402, 21522, 23608, 23781, 23822, 24123, 24139, 24195, 24307, 24327, 24349, 24370, 43644, 43653, 119075 };
            string[] arrTipoLicitacaoEmpenho = new string[arrMovIDs.Length];

            for (int _contador = 0; _contador < arrMovIDs.Length; _contador++)
            {
                objMovimento = objBusiness.ObterMovimento(arrMovIDs[_contador]);
                arrTipoLicitacaoEmpenho[_contador] = objMovimento.RetornarDescricaoEmpenho();
            }

        }

        /// <summary>
        ///A test for RetornarDescricaoEmpenho
        ///</summary>
        [TestMethod()]
        public void RetornarDescricaoEmpenhoTest()
        {
            MovimentoEntity objMovimento = null; 
            string expected = string.Empty; 
            string actual;
            actual = ExtensionMethods.RetornarDescricaoEmpenho(objMovimento);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for _xpParticionadorPorNumeroDocumento
        ///</summary>
        [TestMethod()]
        public void _xpParticionadorPorNumeroDocumentoTest()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure();
            int almoxID = 55;
            string codigoEmpenho = "2014NE00584";
            bool empenhosNaoLiquidados = true;
            string anoMesRef = "201411";
            IList<MovimentoEntity> listaMovEmpenho;
            IList<MovimentoEntity> listaMovEmpenhoTransformada;

            listaMovEmpenho = target.ListarMovimentacoesEmpenho(almoxID, -1, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            int numeroMaximoItens = 8;
            listaMovEmpenhoTransformada = this._xpParticionadorPorNumeroDocumento(listaMovEmpenho, numeroMaximoItens);

            listaMovEmpenhoTransformada.ToList().ForEach(movEmpenho => { System.Diagnostics.Debug.Write(String.Format("Documento: {0}, Numero MovItens: {1}\n", movEmpenho.NumeroDocumento, movEmpenho.MovimentoItem.Count)); });
        }

        public IList<DTOs.MovimentoEntity> _xpParticionadorPorNumeroDocumento(IList<DTOs.MovimentoEntity> movimentosEmpenho, int numeroMaximoItens = 8)
        {
            IList<DTOs.MovimentoItemEntity> listaParticionadaMovItem = new List<DTOs.MovimentoItemEntity>();
            IList<DTOs.MovimentoEntity> listaParticionada = new List<DTOs.MovimentoEntity>();

            MovimentoEntity objEspelho = null;


            int numElementos = movimentosEmpenho.Count;
            string[] arrNumerosDocumento = new string[] { };

            if (_outrageous.ExtensionMethods.IsNotNullAndNotEmpty(movimentosEmpenho))
            {
                arrNumerosDocumento = movimentosEmpenho.Select(movEmpenho => movEmpenho.NumeroDocumento).ToArray();

                foreach (var numDocumento in arrNumerosDocumento)
                {
                    listaParticionadaMovItem = new List<MovimentoItemEntity>();
                    var lstMovItems = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento).SelectMany(movItem => movItem.MovimentoItem);

                    objEspelho = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento)
                                                  .Select<MovimentoEntity, MovimentoEntity>(instaciadorAuxDtoEmpenho(movimentosEmpenho))
                                                  .First();

                    foreach (DTOs.MovimentoItemEntity movItem in lstMovItems)
                    {
                        listaParticionadaMovItem.Add(movItem);
                        if (listaParticionadaMovItem.Count == numeroMaximoItens)
                        {
                            objEspelho.MovimentoItem = listaParticionadaMovItem;

                            listaParticionada.Add(objEspelho);
                            listaParticionadaMovItem = new List<DTOs.MovimentoItemEntity>(numeroMaximoItens);

                            objEspelho = null;
                        }
                    }

                    if (listaParticionadaMovItem.Count > 0)
                    {
                        objEspelho = movimentosEmpenho.Where(movEmpenho => movEmpenho.NumeroDocumento == numDocumento)
                                                      .Select<MovimentoEntity, MovimentoEntity>(instaciadorAuxDtoEmpenho(movimentosEmpenho))
                                                      .First();

                        objEspelho.MovimentoItem = listaParticionadaMovItem;
                        listaParticionada.Add(objEspelho);

                        listaParticionadaMovItem = null;
                    }

                    objEspelho = null;
                }
            }

            return listaParticionada;
        }

        public Func<MovimentoEntity, MovimentoEntity> instaciadorAuxDtoEmpenho(IList<MovimentoEntity> listaMovimentacoes)
        {
            Func<MovimentoEntity, MovimentoEntity> objRetorno;

            objRetorno = (movEmpenho => new MovimentoEntity(){
                                                               NumeroDocumento = movEmpenho.NumeroDocumento,
                                                               UGE = movEmpenho.UGE,
                                                               Almoxarifado = movEmpenho.Almoxarifado,
                                                               AnoMesReferencia = movEmpenho.AnoMesReferencia,
                                                               DataMovimento = movEmpenho.DataMovimento,
                                                               Empenho = movEmpenho.Empenho,
                                                               EmpenhoEvento = movEmpenho.EmpenhoEvento,
                                                               EmpenhoLicitacao = movEmpenho.EmpenhoLicitacao,
                                                               NaturezaDespesaEmpenho = movEmpenho.NaturezaDespesaEmpenho,
                                                               Observacoes = movEmpenho.Observacoes,
                                                               ValorDocumento = movEmpenho.ValorDocumento,
                                                               GeradorDescricao = movEmpenho.GeradorDescricao,
                                                               Fornecedor = movEmpenho.Fornecedor,
                                                               MovimentoItem = new List<MovimentoItemEntity>()
                                                             });
            return objRetorno;
        }
    }
}
