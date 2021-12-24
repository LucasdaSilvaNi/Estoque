using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;


namespace Sam.Presenter
{
    public class EmpenhoEventoPresenter : CrudPresenter<IEmpenhoEventoView>
    {
        IEmpenhoEventoView view;

        public IEmpenhoEventoView View
        {
            get { return view; }
            set { view = value; }
        }

        public EmpenhoEventoPresenter() { }
        public EmpenhoEventoPresenter(IEmpenhoEventoView _view) : base(_view) { this.View = _view;  }


        public IList<EmpenhoEventoEntity> PopularDadosEmpenhoEvento()
        {
            EmpenhoEventoBusiness objBusiness = null;
            IList<EmpenhoEventoEntity> lstRetorno = null;

            objBusiness = new EmpenhoEventoBusiness();
            //objBusiness.SkipRegistros = startRowIndexParameterName;
            lstRetorno = objBusiness.ListarEventos();
            
            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            
            return lstRetorno;
        }

        public IList<EmpenhoEventoEntity> CarregarListaEventos()
        {
            EmpenhoEventoBusiness objBusiness = null;
            IList<EmpenhoEventoEntity> lstRetorno = null;


            objBusiness = new EmpenhoEventoBusiness();
            lstRetorno = objBusiness.ListarEventos();
            
            
            return lstRetorno;           
        }

        public IList<EmpenhoEventoEntity> PopularDadosRelatorio()
        {
            IList<EmpenhoEventoEntity> lstRetorno = null;
            EmpenhoEventoBusiness objBusiness = null;
            
            objBusiness = new EmpenhoEventoBusiness();
            lstRetorno = objBusiness.Imprimir();
            
            return lstRetorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularGrid();
        }

        public void Gravar()
        {
            EmpenhoEventoBusiness objBusiness = new EmpenhoEventoBusiness();
            objBusiness.Entity.Id = this.View.Id;
            objBusiness.Entity.Codigo = this.View.Codigo;
            objBusiness.Entity.Descricao = this.View.Descricao;
            objBusiness.Entity.CodigoEstorno = this.View.CodigoEstorno;
            objBusiness.Entity.AnoBase = this.View.AnoBase;
            objBusiness.Entity.Ativo = this.View.Ativo;
            objBusiness.Entity.TipoMovimentoAssociado = this.View.TipoMovimentoAssociado;
            objBusiness.Entity.TipoMaterialAssociado = this.View.TipoMaterialAssociado;

            if (objBusiness.Salvar())
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
            EmpenhoEventoBusiness objBusiness = new EmpenhoEventoBusiness();           
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
            base.Cancelar();
        }

        public override void Novo()
        {
            limparCampos();
            this.View.PopularListaTiposMovimento();
            base.Novo();
        }
        public override void GravadoSucesso()
        {
            limparCampos();
            base.ExcluidoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            limparCampos();
            base.ExcluidoSucesso();
        }

        private void limparCampos()
        {
            this.View.Id = null;
            this.View.Codigo = 0;
            this.View.CodigoEstorno = null;
            this.View.Descricao = null;
            this.View.AnoBase = null;
            this.View.TipoMovimentoAssociado = new TipoMovimentoEntity(0);
            this.View.TipoMaterialAssociado = 0;
        }
    }
}
