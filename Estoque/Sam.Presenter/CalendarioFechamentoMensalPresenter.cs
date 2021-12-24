using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;


namespace Sam.Presenter
{
    public class CalendarioFechamentoMensalPresenter : CrudPresenter<ICalendarioFechamentoMensalView>
    {
        ICalendarioFechamentoMensalView view;
        public ICalendarioFechamentoMensalView View
        {
            get { return view; }
            set { view = value; }
        }

        public CalendarioFechamentoMensalPresenter() { }
        public CalendarioFechamentoMensalPresenter(ICalendarioFechamentoMensalView _view) : base(_view) { this.View = _view; }


        public IList<CalendarioFechamentoMensalEntity> PopularDadosDatasFechamento()
        {
            CalendarioFechamentoMensalBusiness objBusiness = null;
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;

            objBusiness = new CalendarioFechamentoMensalBusiness();
            //objBusiness.SkipRegistros = startRowIndexParameterName;
            lstRetorno = objBusiness.ListarDatasFechamento();
            
            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            
            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> ListarDatasFechamento()
        {
            CalendarioFechamentoMensalBusiness objBusiness = null;
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;


            objBusiness = new CalendarioFechamentoMensalBusiness();
            lstRetorno = objBusiness.ListarDatasFechamento();


            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> ListarDatasFechamento(int Ano)
        {
            CalendarioFechamentoMensalBusiness objBusiness = null;
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;


            objBusiness = new CalendarioFechamentoMensalBusiness();
            lstRetorno = objBusiness.ListarDatasFechamento(Ano);


            return lstRetorno;
        }

        public IList<int> ListarAnoFechamento()
        {
            CalendarioFechamentoMensalBusiness objBusiness = null;
            IList<int> lstRetorno = null;


            objBusiness = new CalendarioFechamentoMensalBusiness();
            lstRetorno = objBusiness.ListarAnoFechamento();


            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> PopularDadosRelatorio()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;
            CalendarioFechamentoMensalBusiness objBusiness = null;
            
            objBusiness = new CalendarioFechamentoMensalBusiness();
            lstRetorno = objBusiness.Imprimir();
            
            return lstRetorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public void Gravar()
        {
            CalendarioFechamentoMensalBusiness objBusiness = new CalendarioFechamentoMensalBusiness();
            objBusiness.Entity.Id = this.View.Id;
            objBusiness.Entity.AnoReferencia = this.View.AnoReferencia;
            objBusiness.Entity.MesReferencia = this.View.MesReferencia;
            objBusiness.Entity.DataFechamentoDespesa = this.View.DataFechamentoDespesa;
            //objBusiness.Entity.DataFechamentoReceita = this.View.DataFechamentoReceita;

            if (objBusiness.ConsistirDataFechamento() && objBusiness.Salvar())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

            this.View.ListaErros = objBusiness.ListaErro;
        }

        public void Excluir()
        {
            CalendarioFechamentoMensalBusiness objBusiness = new CalendarioFechamentoMensalBusiness();           
            objBusiness.Entity.Id = this.View.Id;
            if (objBusiness.Excluir())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = objBusiness.ListaErro;
        }

        public void Imprimir() { throw new NotImplementedException("Relatório inexistente para esta Tabela."); }

        public override void Cancelar()
        {
            limparCampos();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;

        }

        public override void Load()
        {
            limparCampos();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;

            this.view.PopularDdlAno();
            this.View.PopularGrid();
        }
        public override void RegistroSelecionado()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = true;
        }
        public override void Novo()
        {
            limparCampos();

            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaExcluir = false;
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNovo = false;
        }
        public override void GravadoSucesso()
        {
            limparCampos();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;
        }

        public override void ExcluidoSucesso()
        {
            limparCampos();
            //base.ExcluidoSucesso();
        }

        private void limparCampos()
        {
            this.View.Id = null;
            this.View.AnoReferencia = 0;
            this.View.MesReferencia = 0;
            this.View.DataFechamentoDespesa = DateTime.MinValue;


            //Ações adicionais base.ExcluidoSucesso()
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;
        }
    }
}
