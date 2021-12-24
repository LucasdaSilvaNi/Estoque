using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using Sam.Domain.Business.SIAFEM;
using System.ComponentModel;


namespace Sam.Presenter
{
    public class LiquidacaoEmpenhoPresenter : CrudPresenter<ILiquidacaoEmpenhoView>
    {
        ILiquidacaoEmpenhoView view;

        public ILiquidacaoEmpenhoView View
        {
            get { return view; }
            set { view = value; }
        }

        public LiquidacaoEmpenhoPresenter(){  }
        public LiquidacaoEmpenhoPresenter(ILiquidacaoEmpenhoView _view) : base(_view)
        {
            this.View = _view;
        }


        public IList<string> ListarEmpenhosFornecedor(int almoxID, int fornecedorID, string anoMesRef, bool empenhosNaoLiquidados = true)
        {
            IList<string> lstEmpenhos = null;
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();

            lstEmpenhos = objBusiness.ListarEmpenhosFornecedor(almoxID, fornecedorID, anoMesRef, empenhosNaoLiquidados);
            this.TotalRegistrosGrid = lstEmpenhos.Count;

            return lstEmpenhos;
        }
        public IList<string> ListarDocumentosEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<string> lstDocumentos = null;
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();

            lstDocumentos = objBusiness.ListarDocumentosEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
            this.TotalRegistrosGrid = lstDocumentos.Count;

            return lstDocumentos;
        
        }
        public IList<MovimentoEntity> ListarMovimentacoesEmpenho(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados = true)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();

            lstMovimentacoes = objBusiness.ListarMovimentacoesEmpenho(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
            this.TotalRegistrosGrid = lstMovimentacoes.Count;

            return lstMovimentacoes;
        }

        //TODO: Finalizar função-base : Liquidar Empenho (UIX)
        public IList<MovimentoEntity> EfetuarLiquidacaoEmpenho(int[] _movimentacoesID, string codigoEmpenho, string chaveSiafem, string senhaSiafem)
        {
            IList<MovimentoEntity> lstMovimentacoesPagas = null;


            return lstMovimentacoesPagas;
        }

        //TODO: Finalizar função-base : Estornar Empenho (UIX)
        public IList<MovimentoEntity> EstornarPagamentoEmpenho(int[] _movimentacoesID, string codigoEmpenho, string chaveSiafem, string senhaSiafem)
        {
            IList<MovimentoEntity> lstMovimentacoesEstornadas = null;


            return lstMovimentacoesEstornadas;
        }

        public MovimentoEntity ObterMovimento(int movID, bool isEmpenho = true)
        {
            MovimentoEntity objRetorno = null;
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();
            
            objRetorno = objBusiness.ObterMovimento(movID, isEmpenho);

            if (this.View.IsNotNull())
                this.View.ListaErros = objBusiness.ListaErro;

            return objRetorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int NumeroEmpenhoCombo)
        {
            return this.TotalRegistrosGrid;
        }

        public IList<string> ProcessarMovimentacoesEmpenho(IList<MovimentoEntity> lstMovimentacoes, string loginSiafem, string senhaSiafem, string loginSAM)
        {
            IList<string> listaNLs = null;
            LiquidacaoBusiness objBusiness = null;

            try
            {
                //string nlLiquidacao = null;

                objBusiness = new LiquidacaoBusiness();
                listaNLs = objBusiness.ProcessarMovimentacoesEmpenho(lstMovimentacoes, loginSiafem, senhaSiafem, loginSAM);
                
                if (this.View.IsNotNull())
                    this.View.ListaErros = objBusiness.ListaErro;

                return listaNLs;
            }
            catch (Exception erroCamadaNegocio)
            {
                if (this.view.IsNotNull())
                    this.View.ListaErros = objBusiness.ListaErro;

                throw erroCamadaNegocio;
            }
        }

        public IList<string> ObterNLsPagamentoEmpenho(int almoxID, string anoMesRef, string codigoEmpenho, bool detalharNLs = false)
        {
            var objBusiness = new LiquidacaoBusiness();
            IList<string> listaNLsEmpenho = null;

            listaNLsEmpenho = objBusiness.ObterNLsPagamentoEmpenho(almoxID, anoMesRef, codigoEmpenho, detalharNLs);

            return listaNLsEmpenho;
        }

        public IList<MovimentoEntity> ListarMovimentacoesEmpenhoParticionadas(int almoxID, int fornecedorID, string anoMesRef, string codigoEmpenho, bool empenhosNaoLiquidados)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;
            EmpenhoBusiness objBusiness = new EmpenhoBusiness();

            lstMovimentacoes = objBusiness.ListarMovimentacoesEmpenhoParticionadas(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);
            this.TotalRegistrosGrid = lstMovimentacoes.Count;

            return lstMovimentacoes;
        }
    }
}
