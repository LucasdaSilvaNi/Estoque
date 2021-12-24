using Sam.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using System.Linq;
using Sam.Domain.Infrastructure;
using Sam.Common.Util;
using System.Diagnostics;
using Sam.Common.Siafem;
using Sam.Domain.Business;
using Sam.Domain.Business.SIAFEM;
//using Sam.Domain.Business.SIAFEM;



namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoTest and is intended
    ///to contain all LiquidacaoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoTest
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
        /////A test for __xpSiafemMsgEstimulo
        /////</summary>
        //[TestMethod()]
        //public void @__xpSiafemMsgEstimuloTest()
        //{
        //    Liquidacao target = new Liquidacao(); 
        //    List<MovimentoEntity> lstMovimentosDoEmpenho = null; 
        //    List<MovimentoItemEntity> lstItensEmpenho = null; 
        //    List<MovimentoItemEntity> lstItensEmpenhoExpected = null; 
        //    string strChaveSiaf = string.Empty; 
        //    string strSenhaSiaf = string.Empty; 
        //    string strNumeroEmpenho = string.Empty; 
        //    string strAnoBase = string.Empty; 
        //    string strUnidadeGestora = string.Empty; 
        //    string strCodigoGestao = string.Empty; 
        //    string debugLog = string.Empty; 
        //    string debugLogExpected = string.Empty; 
        //    List<string> expected = null; 
        //    List<string> actual;

        //    //Sam.Domain.Infrastructure.MovimentoInfrastructure MovimentoInfra = new Sam.Domain.Infrastructure.MovimentoInfrastructure();
        //    //Sam.Domain.Infrastructure.MovimentoItemInfrastructure MovItemInfra = new Sam.Domain.Infrastructure.MovimentoItemInfrastructure();
        //    Sam.Infrastructure.MovimentoInfrastructure MovimentoInfra = new Sam.Infrastructure.MovimentoInfrastructure();
        //    Sam.Infrastructure.MovimentoItemInfrastructure MovItemInfra = new Sam.Infrastructure.MovimentoItemInfrastructure();
        //    Sam.Infrastructure.ItemMaterialInfrastructure ItemMaterialInfra = new Sam.Infrastructure.ItemMaterialInfrastructure();
        //    Sam.Infrastructure.SubItemMaterialInfrastructure SubItemMaterialInfra = new Sam.Infrastructure.SubItemMaterialInfrastructure();


        //    lstMovimentosDoEmpenho = new List<MovimentoEntity>();
        //    lstItensEmpenho = new List<MovimentoItemEntity>();


        //    strChaveSiaf = "35593145857";
        //    strSenhaSiaf = "cocacola";
        //    strAnoBase = "2014";
        //    strUnidadeGestora = "380184";
        //    strCodigoGestao = "00001";
        //    strNumeroEmpenho = "2014NE00029";

        //    //varredura na base
        //    MovimentoInfra.SelectWhere(Movimentacoes => Movimentacoes.TB_TIPO_MOVIMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimento.AquisicaoCompraEmpenho
        //                                            && Movimentacoes.TB_MOVIMENTO_ATIVO == true
        //                                            && (Movimentacoes.TB_MOVIMENTO_EMPENHO != null && Movimentacoes.TB_MOVIMENTO_EMPENHO != string.Empty))
        //                  .GroupBy(_movEmpenho => new {
        //                                                _movEmpenho.TB_MOVIMENTO_ID
        //                                               ,_movEmpenho.TB_MOVIMENTO_EMPENHO
        //                                               ,_movEmpenho.TB_ALMOXARIFADO_ID
        //                                               ,_movEmpenho.TB_MOVIMENTO_NUMERO_DOCUMENTO
        //                                              })
        //                  .Take(20)
        //                  .ToList()
        //                  .Select(_movimento => new MovimentoEntity()
        //                  {
        //                      Id = _movimento.Key.TB_MOVIMENTO_ID,
        //                      Almoxarifado = new AlmoxarifadoEntity(_movimento.Key.TB_ALMOXARIFADO_ID),

        //                      Empenho = _movimento.Key.TB_MOVIMENTO_EMPENHO,
        //                      NumeroDocumento = _movimento.Key.TB_MOVIMENTO_NUMERO_DOCUMENTO,
        //                      MovimentoItem = new List<MovimentoItemEntity>()
        //                  })
        //                  .ToList()
        //                  .ForEach(_movimento => lstMovimentosDoEmpenho.Add(_movimento));

        //    //MovItemInfra.LazyLoadingEnabled = true;
        //    foreach (var Movimento in lstMovimentosDoEmpenho)
        //    {
        //        MovItemInfra.SelectWhere(_movItem => _movItem.TB_MOVIMENTO_ID == Movimento.Id
        //                                          && _movItem.TB_MOVIMENTO_ITEM_ATIVO == true)
        //                    .ForEach(_itemMovimento => Movimento.MovimentoItem.Add(new MovimentoItemEntity()
        //                    {
        //                        Id = _itemMovimento.TB_MOVIMENTO_ITEM_ID,
        //                        ItemMaterial = new ItemMaterialEntity(_itemMovimento.TB_ITEM_MATERIAL_ID.Value) { Codigo = ItemMaterialInfra.SelectOne(itemMat => itemMat.TB_ITEM_MATERIAL_ID == _itemMovimento.TB_ITEM_MATERIAL_ID).TB_ITEM_MATERIAL_CODIGO, Descricao = ItemMaterialInfra.SelectOne(itemMat => itemMat.TB_ITEM_MATERIAL_ID == _itemMovimento.TB_ITEM_MATERIAL_ID).TB_ITEM_MATERIAL_DESCRICAO },
        //                        SubItemMaterial = new SubItemMaterialEntity(_itemMovimento.TB_SUBITEM_MATERIAL_ID) { Codigo = SubItemMaterialInfra.SelectOne(_subitemMat => _subitemMat.TB_SUBITEM_MATERIAL_ID == _itemMovimento.TB_SUBITEM_MATERIAL_ID).TB_SUBITEM_MATERIAL_CODIGO, Descricao = SubItemMaterialInfra.SelectOne(_subitemMat => _subitemMat.TB_SUBITEM_MATERIAL_ID == _itemMovimento.TB_SUBITEM_MATERIAL_ID).TB_SUBITEM_MATERIAL_DESCRICAO },
        //                        QtdeMov = _itemMovimento.TB_MOVIMENTO_ITEM_QTDE_MOV,
        //                        SaldoQtde = _itemMovimento.TB_MOVIMENTO_ITEM_SALDO_QTDE
        //                    }));
        //    }

        //    string _debugNL = string.Empty;
        //    foreach (var Movimento in lstMovimentosDoEmpenho)
        //    {
        //        strNumeroEmpenho = Movimento.Empenho;
        //        var lstSaida = target.@__xpSiafemMsgEstimulo(lstMovimentosDoEmpenho, out lstItensEmpenho, strChaveSiaf, strSenhaSiaf, strNumeroEmpenho, strAnoBase, strUnidadeGestora, strCodigoGestao, ref debugLog);
        //    }

        //    actual = target.@__xpSiafemMsgEstimulo(lstMovimentosDoEmpenho, out lstItensEmpenho, strChaveSiaf, strSenhaSiaf, strNumeroEmpenho, strAnoBase, strUnidadeGestora, strCodigoGestao, ref debugLog);
        //    //Assert.AreEqual(lstItensEmpenhoExpected, lstItensEmpenho);
        //    //Assert.AreEqual(debugLogExpected, debugLog);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        public IList<MovimentoEntity> gerarMassaMovimentosParaLiquidar()
        {
            IList<MovimentoEntity> lstMovimentos = new List<MovimentoEntity>();
            MovimentoInfrastructure objMovBusiness = new MovimentoInfrastructure();
            MovimentoEntity objEntidade = null;
            int[] IdMovs = new int[] { 21522, 21372, 21402, 24195, 41737, 44914 }; //UGE: 380112 , EMPENHO: 2013NE00001, MesRef: 201305

            foreach (var idMov in IdMovs)
            {
                objMovBusiness.Entity = new MovimentoEntity();
                objEntidade = objMovBusiness.GetMovimento();

                if (objEntidade.IsNotNull())
                    lstMovimentos.Add(objEntidade);
            }

            if (lstMovimentos.IsNullOrEmpty())
                lstMovimentos = null;


            return lstMovimentos;
        }

        public IList<MovimentoEntity> gerarMassaMovimentosParaLiquidar02()
        {
            IList<MovimentoEntity> lstMovimentos = new List<MovimentoEntity>();
            MovimentoInfrastructure objMovBusiness = new MovimentoInfrastructure();
            MovimentoEntity objEntidade = null;
            int[] IdMovs = new int[] { 590857 }; //UGE: 380118 , EMPENHO: 2014NE00584, MesRef: 201411

            foreach (var idMov in IdMovs)
            {
                objMovBusiness.Entity = new MovimentoEntity(idMov);
                objEntidade = objMovBusiness.GetMovimento();

                if (objEntidade.IsNotNull())
                    lstMovimentos.Add(objEntidade);
            }

            if (lstMovimentos.IsNullOrEmpty())
                lstMovimentos = null;


            return lstMovimentos;
        }

        private IList<MovimentoEntity> _gerarListaMovimentacoesEmpenho()
        {
            MovimentoInfrastructure target = new MovimentoInfrastructure();
            int almoxID = 55;//49;
            int fornecedorID = 13;
            string codigoEmpenho = "2014NE00392";// "2013NE00001";
            bool empenhosNaoLiquidados = true;
            string anoMesRef = "201407";// "201305";
            IList<MovimentoEntity> listaMovEmpenho;

            listaMovEmpenho = target.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

            return listaMovEmpenho;
        }

        ///// <summary>
        /////A test for AgrupaMovimentoItem
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Business.dll")]
        //public void AgrupaMovimentoItemTest()
        //{
        //    string strDeploySaida = null;
        //    Liquidacao_Accessor target = new Liquidacao_Accessor();
        //    List<MovimentoEntity> lstMovimentacoesEmpenho = this._gerarListaMovimentacoesEmpenho().ToList(); 
            
        //    List<MovimentoItemEntity> lstItensMovimentacoes;

        //    lstItensMovimentacoes = target.AgrupaMovimentoItem(lstMovimentacoesEmpenho);
        //    lstItensMovimentacoes.ForEach(_itemMov => {
        //                                                if (String.IsNullOrWhiteSpace(strDeploySaida))
        //                                                    strDeploySaida = String.Format("ItemMaterial: {0}\nQtde: {1}", _itemMov.ItemMaterial.CodigoDescricao, _itemMov.QtdeMov);
        //                                                else
        //                                                    strDeploySaida = String.Format("{0}\n\n{1}", strDeploySaida, String.Format("ItemMaterial: {0}\nQtde: {1}", _itemMov.ItemMaterial.CodigoDescricao, _itemMov.QtdeMov));
        //                                              });

        //    Debugger.Log(0, "", "AgrupaMovimentoItemTest" + strDeploySaida);
        //}

        ///// <summary>
        /////A test for AgrupaMovimentoItem02
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Business.dll")]
        //public void AgrupaMovimentoItem02Test()
        //{
        //    string strDeploySaida = null;
        //    Liquidacao_Accessor accessorLiquidacao = new Liquidacao_Accessor();
        //    List<MovimentoEntity> lstMovimentacoesEmpenho = this._gerarListaMovimentacoesEmpenho().ToList();

        //    List<MovimentoItemEntity> lstItensMovimentacoes;

        //    lstItensMovimentacoes = accessorLiquidacao.AgrupaMovimentoItem02(lstMovimentacoesEmpenho);
        //    lstItensMovimentacoes.ForEach(_itemMov =>
        //    {
        //        if (String.IsNullOrWhiteSpace(strDeploySaida))
        //            strDeploySaida = String.Format("ItemMaterial: {0}\nQtde: {1}", _itemMov.ItemMaterial.CodigoDescricao, _itemMov.QtdeMov);
        //        else
        //            strDeploySaida = String.Format("{0}\n\n{1}", strDeploySaida, String.Format("ItemMaterial: {0}\nQtde: {1}", _itemMov.ItemMaterial.CodigoDescricao, _itemMov.QtdeMov));
        //    });

        //    Debugger.Log(0, "", "AgrupaMovimentoItem02Test" + strDeploySaida);
        //}

        //[TestMethod()]
        //public void gerarObjetoLiquidacaoSIAFEM()
        //{
        //    List<MovimentoItemEntity> itensMovimentacoesEmpenho = null;
        //    IList<ItemEmpenhoRepete> lstItensLiquidacao = null;
        //    ItemEmpenhoRepete itemEmpenho = null;

        //    EmpenhoBusiness objBusiness = new EmpenhoBusiness();
        //    Liquidacao_Accessor accessorLiquidacao = new Liquidacao_Accessor();
        //    lstItensLiquidacao = new List<ItemEmpenhoRepete>();
        //    List<MovimentoEntity> lstMovimentacoesEmpenho = this._gerarListaMovimentacoesEmpenho().ToList();

        //    itensMovimentacoesEmpenho = accessorLiquidacao.AgrupaMovimentoItem02(lstMovimentacoesEmpenho);

            
        //    int codigoUGE = 380212;
        //    int almoxID = 191;
        //    string codigoEmpenho = "2014NE00219";
        //    string anoMesRef = "201411";

        //    var movEmpenhoSiafem = objBusiness.obterMovimentoEmpenho(codigoUGE, almoxID, codigoEmpenho, anoMesRef, "35593145857", "cocacola");

        //    foreach (var _itemEmpenho in itensMovimentacoesEmpenho)
        //    {
        //        itemEmpenho = new ItemEmpenhoRepete();
        //        itemEmpenho.CodigoMaterial = _itemEmpenho.ItemMaterialCodigo;
        //        itemEmpenho.CodigoUnidade = _itemEmpenho.UnidadeFornecimentoDes;
        //        itemEmpenho.QtdeItem = _itemEmpenho.QtdeMov.ToString();
        //        itemEmpenho.ValorUnitarioItem = _itemEmpenho.ValorMov.ToString();

        //        lstItensLiquidacao.Add(itemEmpenho);
        //        itemEmpenho = null;
        //    }

        //}

        /// <summary>
        ///A test for GerarMovimentacoesParaLiquidacao
        ///</summary>
        [TestMethod()]
        public void GerarMovimentacoesParaLiquidacaoTest()
        {
            LiquidacaoBusiness target = new LiquidacaoBusiness();
            IList<MovimentoEntity> lstMovimentacoesEmpenho = gerarMassaMovimentosParaLiquidar02();
            IList<MovimentoEntity> expected = null;
            IList<MovimentoEntity> actual;
            actual = target.GerarMovimentacoesParaLiquidacao(lstMovimentacoesEmpenho);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
