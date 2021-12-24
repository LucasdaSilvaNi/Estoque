using Sam.Presenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Business.MovimentoFactory;
using Sam.Domain.Entity;
using System.Collections.Generic;
using Sam.Business;
using System.Diagnostics;
using Sam.Infrastructure;
using System.Linq;
using Sam.Common.Util;
using System.Threading;

namespace Test
{


    /// <summary>
    ///This is a test class for EntradaMaterialPresenterTest and is intended
    ///to contain all EntradaMaterialPresenterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EntradaMaterialPresenterTest
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
        ///A test for TesteFactory
        ///</summary>
        ///
        #region Métodos do Novo Movimento
        [TestMethod()]
        public void TesteFactoryTest()
        {
            try
            {
                for (int i = 1; i <= 1; i++)
                {
                    EntradaAvulsaTeste(i);
                    //SaidaAvulsaTeste(i + 3);

                    TestContext.WriteLine("fim Iteração:" + i);
                }

                TestContext.WriteLine("Sucesso final ");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Erro:" + ex.Message);
            }

        }

        [TestMethod()]
        public void EntradaAvulsaTeste(int iteracao)
        {
            //Massa de dados
            MassaDados massa = new MassaDados();
            massa.Iteracao = iteracao;
            massa.AlmoxId = 130;
            massa.DivisaoId = 348;
            massa.FornecedorId = 38;
            massa.UGEID = 18;
            massa.SubItemId = 17001; //17001
            //massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa;
            massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.EntradaPorEmpenho;
            massa.Qtd = 11;
            massa.Valor = 12;
            massa.MultQtd = 1;
            massa.MultValor = 0.25m;
            massa.ComLote = true;

            var massaMovimento = massa.GetMovimentoMassa(1);

            //Execução da entrada
            EntradaMaterialPresenter target = new EntradaMaterialPresenter(); 
            ParametrizacaoEntrada param = new ParametrizacaoEntrada();
            //param.tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa;
            param.tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.EntradaPorEmpenho;

            MovimentoFactory.GetIMovimentoFactory().EntrarMaterial(param, massaMovimento);
        }

        [TestMethod()]
        public void SaidaAvulsaTeste(int iteracao)
        {
            //Massa de dados
            MassaDados massa = new MassaDados();
            massa.Iteracao = iteracao + 2;
            massa.AlmoxId = 130;
            massa.DivisaoId = 348;
            massa.FornecedorId = 38;
            massa.UGEID = 18;
            massa.SubItemId = 17001;
            massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.OutrasSaidas;
            massa.Qtd = 5;
            massa.Valor = 12;
            massa.MultQtd = 1;
            massa.MultValor = 0.1m;
            massa.ComLote = true;

            EntradaMaterialPresenter target = new EntradaMaterialPresenter(); 
            ParametrizacaoEntrada param = new ParametrizacaoEntrada();
            param.tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.OutrasSaidas;

            var massaMovimento = massa.GetMovimentoMassa(1);
            MovimentoFactory.GetIMovimentoFactory().SairMaterial(param, massaMovimento);
        }

        [TestMethod()]
        public void TestarAgrupamento()
        {
            //Massa de dados
            MassaDados massa = new MassaDados();
            massa.Iteracao = 1;
            massa.AlmoxId = 130;
            massa.DivisaoId = 348;
            massa.FornecedorId = 38;
            massa.UGEID = 18;
            massa.SubItemId = 17001;
            massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.OutrasSaidas;
            massa.Qtd = 1;
            massa.Valor = 12;
            massa.MultQtd = 1;
            massa.MultValor = 0.1m;
            massa.ComLote = false;
            massa.DataMovimento = new DateTime(2012, 09, 01);

            var movimento = massa.GetMovimentoMassa(1);

            MovimentoItemInfrastructure infra = new MovimentoItemInfrastructure();
            infra.LazyLoadingEnabled = true;

            MovimentoBusiness business = new MovimentoBusiness();
            //var tbMov = business.SetEntidade(movimento);

            IQueryable<TB_MOVIMENTO_ITEM> tbMov = new MovimentoItemBusiness().QueryMovimentoBy(movimento.MovimentoItem.First());
            TB_MOVIMENTO mov = new TB_MOVIMENTO();

            if (tbMov.Count() == 0)
                throw new Exception("Zero");

            IList<TB_MOVIMENTO_ITEM> result = business.CalcularSaldoByMovimentoItem(tbMov.ToList());

            foreach (var r in result)
            {
                TestContext.WriteLine("---------------------------------------------------------------");
                TestContext.WriteLine(String.Format("SUBITEM: {0}", r.TB_SUBITEM_MATERIAL_ID));
                TestContext.WriteLine(String.Format("QTD: {0}", r.TB_MOVIMENTO_ITEM_QTDE_MOV));
                TestContext.WriteLine(String.Format("VALOR: {0}", r.TB_MOVIMENTO_ITEM_VALOR_MOV));
                TestContext.WriteLine(String.Format("PRECO UNIT.: {0}", r.TB_MOVIMENTO_ITEM_PRECO_UNIT));
                TestContext.WriteLine(String.Format("IDENTIFICAÇÃO.: {0}", r.TB_MOVIMENTO_ITEM_LOTE_IDENT));
                TestContext.WriteLine(String.Format("FABRICANTE.: {0}", r.TB_MOVIMENTO_ITEM_LOTE_FABR));
                TestContext.WriteLine(String.Format("VENCIMENTO.: {0}", r.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC));
                TestContext.WriteLine("---------------------------------------------------------------");

                TestContext.WriteLine("Outro Lote");
            }
            
        }

        #endregion

        #region Métodos do Movimento Atual

        [TestMethod()]
        public void ExecutarEntradaSaidaSimples()
        {
            try
            {
                for (int i = 1; i <= 18; i++)
                {
                    if (i == 1)
                    {
                        GravarMovimentoEntrada(5, 1);
                    }

                    if (i == 2)
                    {
                        GravarMovimentoEntrada(5, 1);
                    }

                    if (i == 3)
                    {
                        GravarMovimentoEntrada(6, 1);
                    }

                    if (i == 4)
                    {
                        GravarMovimentoEntrada(6, 1);
                    }
                    if (i == 5)
                    {
                        GravarMovimentoEntrada(6, 1);
                    }
                    if (i == 6)
                    {
                        GravarMovimentoSaida(6, 1);
                    }
                    if (i == 7)
                    {
                        GravarMovimentoEntrada(6, 1);                        
                    }
                    if (i == 8)
                    {
                        GravarMovimentoEntrada(6, 1);
                    }
                    if (i == 9)
                    {
                        GravarMovimentoSaida(7, 1);
                    }
                    
                    if (i == 10)
                    {
                        GravarMovimentoSaida(6, 1);

                        
                    }
                    if (i == 11)
                    {
                        GravarMovimentoSaida(8, 1);
                    }
                    if (i == 12)
                    {
                        GravarMovimentoEntrada(6, 1);
                    }
                    if (i == 13)
                    {
                        GravarMovimentoEntrada(7, 1);
                    }
                    if (i == 14)
                    {
                        GravarMovimentoSaida(7, 1);
                    }
                    if (i == 15)
                    {
                        GravarMovimentoSaida(5, 1);
                    }
                    if (i == 16)
                    {
                        var result = new Sam.Business.MovimentoBusiness().SelectWhere(a => a.TB_TIPO_MOVIMENTO_ID == 5).OrderByDescending(a => a.TB_MOVIMENTO_ID).ElementAt(6);
                        EstornoEntrada(result.TB_MOVIMENTO_ID);
                    }
                    if (i == 17)
                    {
                        var result = new Sam.Business.MovimentoBusiness().SelectWhere(a => a.TB_TIPO_MOVIMENTO_ID == 5).OrderByDescending(a => a.TB_MOVIMENTO_ID).ElementAt(3);
                        EstornoEntrada(result.TB_MOVIMENTO_ID);
                    }
                    if (i == 18)
                    {
                        var result = new Sam.Business.MovimentoBusiness().SelectWhere(a => a.TB_TIPO_MOVIMENTO_ID == 14).OrderByDescending(a => a.TB_MOVIMENTO_ID).ElementAt(3);
                        EstornoSaida(result.TB_MOVIMENTO_ID);
                    }
                }
                
                TestContext.WriteLine("Sucesso final ");
                TestarAgrupamento();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Erro:" + ex.Message);
            }

        }
        [TestMethod()]        
        public void ExecutarEntrada()
        {
            for (int i = 1; i <= 10000; i++)
            {
                System.Diagnostics.Debug.WriteLine("Thread: " + i + " - Data: " + DateTime.Now);
                var t1 = new Thread(StartThread);                
                t1.Start(i);                
                Thread.Sleep(5000);                

            }
        }
        

        public void StartThread(object i)
        {
            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(i + " Inicio: " + dataInicio);

            GravarMovimentoEntrada(1, 1);
            //GravarMovimentoEntrada(3, 1);
            //GravarMovimentoEntrada(2, 1);
            //GravarMovimentoSaida(3, 1);

            DateTime dataFim = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(i + " Fim: " + dataFim);
            System.Diagnostics.Debug.WriteLine(dataFim - dataInicio);
        }

        [TestMethod()]
        public void ExecutarEstornoEntrada()
        {
            EstornoEntrada(49319);

            //for (int i = 1; i <= 3; i++)
            //{
            //    GravarMovimentoEntrada(1);
            //    var movimentoID = new Sam.Business.MovimentoBusiness().QueryAll().OrderByDescending(a => a.TB_MOVIMENTO_ID).FirstOrDefault().TB_MOVIMENTO_ID;
            //    EstornoEntrada(48367);
            //}
        }

        [TestMethod()]
        public void ExecutarEstornoSaida()
        {
            for (int i = 1; i <= 1; i++)
            {
                GravarMovimentoEntrada(1, i);
                GravarMovimentoSaida(1, i);
                var movimentoID = new Sam.Business.MovimentoBusiness().QueryAll().OrderByDescending(a => a.TB_MOVIMENTO_ID).FirstOrDefault().TB_MOVIMENTO_ID;
                EstornoSaida(movimentoID);
            }
        }

        public void EstornoEntrada(int movimentoId)
        {
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();            
            business.Movimento = new MovimentoEntity(movimentoId);

            business.EstornarMovimentoEntrada(1);
        }

        public void EstornoSaida(int movimentoId)
        {
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();
            business.Movimento = new MovimentoEntity(movimentoId);

            business.EstornarMovimentoSaida(1);
        }

        [TestMethod()]
        public void GravarMovimentoEntrada(int data, int iteracao)
        {
            //Instancia movimento business
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();

            //Massa de dados
            MassaDados massa = new MassaDados();
            massa.Iteracao = iteracao;
            massa.AlmoxId = 149;
            massa.DivisaoId = 3471;
            massa.FornecedorId = 38;
            massa.UGEID = 152;

            massa.Qtd = 1;
            massa.Valor = 100;
            massa.SubItemId = 27134;
            //massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa;
            massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.EntradaAvulsa;
            massa.MultQtd = 1;
            massa.MultValor = 1.00m;
            massa.ComLote = false;
            massa.DataMovimento = new DateTime(2012, 09, data);
            massa.QtdItens = 1;

            business.Movimento = massa.GetMovimentoMassa(1);
            business.GravarMovimento();
        }
         
        [TestMethod()]
        public void GravarMovimentoSaida(int data, int iteracao)
        {
            //Instancia movimento business
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();

            //Massa de dados
            MassaDados massa = new MassaDados();
            massa.Iteracao = iteracao;
            massa.AlmoxId = 149;
            massa.DivisaoId = 3471;
            massa.FornecedorId = 38;
            massa.UGEID = 152;
            massa.SubItemId = 27134; //17001
            massa.TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento.OutrasSaidas;
            massa.Qtd = 1;
            massa.Valor = 10;
            massa.MultQtd = 1;
            massa.MultValor = 1.0m;
            massa.ComLote = true;
            massa.DataMovimento = new DateTime(2012, 09, data);
            massa.QtdItens = 1;
            massa.Fabricante = string.Empty;
            massa.DataVencimento = new DateTime(2012, 09, 01);
            massa.Identificador = string.Empty;

            business.Movimento = massa.GetMovimentoMassa(1);
            business.SalvarMovimentoSaida();
        }

        [TestMethod()]
        public void ExecutarRelatorioAnalitico()
        {
            Sam.Domain.Business.SaldoSubItemBusiness business = new Sam.Domain.Business.SaldoSubItemBusiness();
            var result = business.ImprimirConsultaEstoqueSintetico(18, 130, 0, 1,0);
        }

        [TestMethod()]
        public void Teste()
        {
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();
            business.SomarMovimentoItemPorLote(null);
        }



        #endregion

    }
}
