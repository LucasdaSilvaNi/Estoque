using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Business.SIAFEM;
using Sam.Domain.Entity;
using Sam.View;
using Sam.Common.Util;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;



namespace Sam.Presenter
{
    public class NotaLancamentoPendenteSIAFEMPresenter : CrudPresenter<INotaLancamentoPendenteSIAFEMView>
    {
        INotaLancamentoPendenteSIAFEMView view;
        public INotaLancamentoPendenteSIAFEMView View
        {
            get { return view; }
            set { view = value; }
        }

        public NotaLancamentoPendenteSIAFEMPresenter() { }
        public NotaLancamentoPendenteSIAFEMPresenter(INotaLancamentoPendenteSIAFEMView _view) : base(_view) { this.View = _view; }


        public IList<NotaLancamentoPendenteSIAFEMEntity> ObterNotasLancamentosPendentes(tipoPesquisa tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            lstRetorno = objBusiness.ObterNotasLancamentosPendentes(tipoPesquisa, tabelaPesquisaID, pendenciasAtivas);
            
            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            
            return lstRetorno;
        }
        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarNotasLancamentosPendentes(tipoPesquisa tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            lstRetorno = objBusiness.ListarNotasLancamentosPendentes(tipoPesquisa, tabelaPesquisaID, pendenciasAtivas);

            this.TotalRegistrosGrid = objBusiness.TotalRegistros;

            return lstRetorno;
        }

        public IList<NotaLancamentoPendenteSIAFEMEntity> PopularDadosRelatorio(bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;
            int almoxID = View.AlmoxarifadoID;


            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            lstRetorno = objBusiness.ObterNotasLancamentosPendentes(tipoPesquisa.Almox, almoxID, pendenciasAtivas);

            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            return lstRetorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
        public void Imprimir() { throw new NotImplementedException("Relatório inexistente para esta Tabela."); }

        public override void Load()
        {
            this.View.PopularGrid();
        }

        public string ExecutaProcessamentoNotaPendenteSIAFEM(string loginSiafemUsuario, string senhaSiafemUsuario, string valorCampoCE, string tipo, int notaLancamentoPendenteID, int codigoGestor, int codigoUge,string loginUsuarioSAM)
        {
            var siafBusiness = new SiafemBusiness();

            siafBusiness.ExecutaProcessamentoNotaPendenteSIAFEM(loginSiafemUsuario, senhaSiafemUsuario, valorCampoCE, tipo, notaLancamentoPendenteID, codigoGestor, codigoUge, loginUsuarioSAM);

            return siafBusiness.ErroProcessamentoWs;
        }

        public void ExecutaProcessamentoManualNotaSIAFEM(string loginUsuarioSAM, string tipoLancamento, int notaLancamentoPendenteID, string nlSiafem, TipoNotaSIAF @TipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao)
        {
            var siafBusiness = new SiafemBusiness();

            siafBusiness.ExecutaProcessamentoManualNotaSIAFEM(loginUsuarioSAM, tipoLancamento, notaLancamentoPendenteID, nlSiafem, @TipoNotaSIAF);

            if (this.View.IsNotNull())
                this.View.ListaErros = siafBusiness.ListaErro;
        }
    }
}
