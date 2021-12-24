using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;


namespace Sam.Presenter
{
    public class FuncionalidadeSistemaPresenter : CrudPresenter<IFuncionalidadeSistemaView>
    {
        IFuncionalidadeSistemaView view;

        public IFuncionalidadeSistemaView View
        {
            get { return view; }
            set { view = value; }
        }

        public FuncionalidadeSistemaPresenter() { }
        public FuncionalidadeSistemaPresenter(IFuncionalidadeSistemaView _view) : base(_view) { this.View = _view; }


        public IList<FuncionalidadeSistemaEntity> PopularDadosFuncionalidadesSistema(int[] perfilIDs)
        {
            FuncionalidadeSistemaBusiness objBusiness = null;
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;

            objBusiness = new FuncionalidadeSistemaBusiness();
            lstRetorno = objBusiness.Listar(perfilIDs);
            
            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            
            return lstRetorno;
        }

        public IList<FuncionalidadeSistemaEntity> CarregarListaFuncionalidadesSistema(int[] perfilIDs)
        {
            FuncionalidadeSistemaBusiness objBusiness = null;
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;


            objBusiness = new FuncionalidadeSistemaBusiness();
            lstRetorno = objBusiness.Listar(perfilIDs);
            
            
            return lstRetorno;           
        }

        public IList<FuncionalidadeSistemaEntity> PopularDadosRelatorio()
        {
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;
            FuncionalidadeSistemaBusiness objBusiness = null;
            
            objBusiness = new FuncionalidadeSistemaBusiness();
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
            FuncionalidadeSistemaBusiness objBusiness = new FuncionalidadeSistemaBusiness();
            objBusiness.Entity.Id = this.View.Id;
            objBusiness.Entity.Descricao = this.View.Descricao;

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
            FuncionalidadeSistemaBusiness objBusiness = new FuncionalidadeSistemaBusiness();           
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
            this.View.Descricao = null;
        }
    }
}
