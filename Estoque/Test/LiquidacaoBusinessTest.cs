using Sam.Domain.Business.SIAFEM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using Sam.Presenter;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for LiquidacaoBusinessTest and is intended
    ///to contain all LiquidacaoBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LiquidacaoBusinessTest
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
        ///A test for AtualizarItensMovimentacaoEmpenho
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Sam.Domain.Business.dll")]
        public void AtualizarItensMovimentacaoEmpenhoTest()
        {
            LiquidacaoBusiness_Accessor objBusiness = new LiquidacaoBusiness_Accessor();
            LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();
            IList<MovimentoEntity> lstMovimentacoes = null;
            string loginSiafem = "PSIAFISIC15";
            string senhaSiafem = "13NOVEMBRO";
            string loginSAM = "loginSAM";
            int almoxID = 175;
            string anoMesRef = "201501";
            string codigoEmpenho = "2015NE00002";


            lstMovimentacoes = objPresenter.ListarMovimentacoesEmpenho(almoxID, -1, anoMesRef, codigoEmpenho, true);
            lstMovimentacoes = Sam.Domain.Business.SIAFEM.ExtensionMethods.ParticionadorPorNumeroDocumento(lstMovimentacoes);

            var movimentacoesParaLiquidar = objBusiness.GerarMovimentacoesParaLiquidacao(lstMovimentacoes);
            var lstMsgEstimuloSIAFEM = objBusiness.GerarMsgEstimuloLiquidacao(movimentacoesParaLiquidar, loginSiafem, senhaSiafem, loginSAM);            
            
            IList<string> lstRetornoMsgEstimuloSIAFEM = null;
            lstRetornoMsgEstimuloSIAFEM = listaRetornoSiafem();
            objBusiness.AtualizarItensMovimentacaoEmpenho(lstMovimentacoes, lstRetornoMsgEstimuloSIAFEM);
        }

        private IList<string> listaRetornoSiafem()
        {
            var listaRetornoSIAF = new List<string>(2);
            var _msg01 = "<MSG><BCMSG><Doc_Estimulo><SFCODOC><cdMsg>SFCOLiquidaNL</cdMsg><SFCOLiquidaNL><documento><DataEmissao>15JAN2015</DataEmissao><UnidadeGestora>380193</UnidadeGestora><Gestao>00001</Gestao><EmpenhoOriginal>00002</EmpenhoOriginal><ContratoOriginal></ContratoOriginal><SERVICOSEMGERAL></SERVICOSEMGERAL><SEGUROSEMGERAL></SEGUROSEMGERAL><MATERIALDECONSUMO>x</MATERIALDECONSUMO><MATERIALPERMANENTE></MATERIALPERMANENTE><ALUGUEIS></ALUGUEIS><IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO><IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE><ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA><ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM><repeticaoItem><linha><Item>00013197-0</Item><UnidForn>00001</UnidForn><QtdInteiro>500</QtdInteiro><QtdDecimal>000</QtdDecimal></linha><linha><Item>00307246-0</Item><UnidForn>00001</UnidForn><QtdInteiro>100</QtdInteiro><QtdDecimal>000</QtdDecimal></linha></repeticaoItem><Observacao>EntradaEmpenhoParaLiquidar 001</Observacao><repeticaoNf><NotaFiscal>201519300001</NotaFiscal></repeticaoNf></documento></SFCOLiquidaNL></SFCODOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>";
            _msg01 += "<SFCODOC>";
            _msg01 += "             <SiafisicoDocLiquidaNL>";
            _msg01 += "        <cdMsg>SFCOLiquidaNL</cdMsg>";
            _msg01 += "        <StatusOperacao>true</StatusOperacao>";
            _msg01 += "        <Msg>Liquidacao realizada com sucesso</Msg>";
            _msg01 += "<documento>";
            _msg01 += "<NL>2015NL00005</NL>";
            _msg01 += "<NE>2015NE00002</NE>";
            _msg01 += "<CT>2015CT00002</CT>";
            _msg01 += "<OC></OC>";
            _msg01 += "</documento>";
            _msg01 += "        <ItemNaoEncontrado>";
            _msg01 += "        </ItemNaoEncontrado>";
            _msg01 += "    </SiafisicoDocLiquidaNL>";
            _msg01 += "</SFCODOC>";
            _msg01 += "</Doc_Retorno></SISERRO></MSG>";


            var _msg02 = "<MSG><BCMSG><Doc_Estimulo><SFCODOC><cdMsg>SFCOLiquidaNL</cdMsg><SFCOLiquidaNL><documento><DataEmissao>15JAN2015</DataEmissao><UnidadeGestora>380193</UnidadeGestora><Gestao>00001</Gestao><EmpenhoOriginal>00002</EmpenhoOriginal><ContratoOriginal></ContratoOriginal><SERVICOSEMGERAL></SERVICOSEMGERAL><SEGUROSEMGERAL></SEGUROSEMGERAL><MATERIALDECONSUMO>x</MATERIALDECONSUMO><MATERIALPERMANENTE></MATERIALPERMANENTE><ALUGUEIS></ALUGUEIS><IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO><IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE><ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA><ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM><repeticaoItem><linha><Item>00013197-0</Item><UnidForn>00001</UnidForn><QtdInteiro>500</QtdInteiro><QtdDecimal>000</QtdDecimal></linha><linha><Item>00307246-0</Item><UnidForn>00001</UnidForn><QtdInteiro>100</QtdInteiro><QtdDecimal>000</QtdDecimal></linha></repeticaoItem><Observacao>EntradaEmpenhoParaLiquidar 001</Observacao><repeticaoNf><NotaFiscal>201519300001</NotaFiscal></repeticaoNf></documento></SFCOLiquidaNL></SFCODOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>";
            _msg02 += "<SFCODOC>";
            _msg02 += "             <SiafisicoDocLiquidaNL>";
            _msg02 += "        <cdMsg>SFCOLiquidaNL</cdMsg>";
            _msg02 += "        <StatusOperacao>true</StatusOperacao>";
            _msg02 += "        <Msg>Liquidacao realizada com sucesso</Msg>";
            _msg02 += "<documento>";
            _msg02 += "<NL>2015NL00005</NL>";
            _msg02 += "<NE>2015NE00002</NE>";
            _msg02 += "<CT>2015CT00002</CT>";
            _msg02 += "<OC></OC>";
            _msg02 += "</documento>";
            _msg02 += "        <ItemNaoEncontrado>";
            _msg02 += "        </ItemNaoEncontrado>";
            _msg02 += "    </SiafisicoDocLiquidaNL>";
            _msg02 += "</SFCODOC>";
            _msg02 += "</Doc_Retorno></SISERRO></MSG>";

            listaRetornoSIAF.Add(_msg01);
            listaRetornoSIAF.Add(_msg02);

            return listaRetornoSIAF;
        }
    }
}
