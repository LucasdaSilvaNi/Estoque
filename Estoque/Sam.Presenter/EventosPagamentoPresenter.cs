using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;
using Sam.Common.Util;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;



namespace Sam.Presenter
{
    public class EventosPagamentoPresenter : CrudPresenter<IEventosPagamentoView>
    {
        IEventosPagamentoView view;

        public IEventosPagamentoView View
        {
            get { return view; }
            set { view = value; }
        }

        public EventosPagamentoPresenter() { }
        public EventosPagamentoPresenter(IEventosPagamentoView _view) : base(_view) { this.View = _view;  }


        public EventosPagamentoEntity ObterEventoPagamento(TipoMovimento tipoMovimento)
        {
            EventosPagamentoBusiness objBusiness = null;
            EventosPagamentoEntity dadosPagamento = null;

            objBusiness = new EventosPagamentoBusiness();
            dadosPagamento = objBusiness.ObterEventoPagamento(tipoMovimento);

            return dadosPagamento;
        }

        public IList<EventosPagamentoEntity> PopularDadosEventosPagamento()
        {
            EventosPagamentoBusiness objBusiness = null;
            IList<EventosPagamentoEntity> lstRetorno = null;

            objBusiness = new EventosPagamentoBusiness();
            lstRetorno = objBusiness.ListarEventos();
            
            this.TotalRegistrosGrid = objBusiness.TotalRegistros;
            
            return lstRetorno;
        }

        public EventoSiafemEntity SalvarEventoSiafem(EventoSiafemEntity item)
        {
            EventosPagamentoBusiness objBusiness = null;
            EventoSiafemEntity itemRetorno = null;

            objBusiness = new EventosPagamentoBusiness();
            itemRetorno = objBusiness.SalvarSiafem(item);

       
            return itemRetorno;
        }

        public bool InativarItemEventoSiafem(int id, string usuario)
        {
            EventosPagamentoBusiness objBusiness = null;
            bool itemRetorno = false;

            objBusiness = new EventosPagamentoBusiness();
            itemRetorno = objBusiness.InativarItemEventoSiafem(id, usuario);


            return itemRetorno;
        }

        public bool AlterarItemEventoSiafem(int id, string usuario, string txt1, string txt2, int subtipo, int subtipoOld, bool estimulo)
        {
            EventosPagamentoBusiness objBusiness = null;
            bool itemRetorno = false;

            objBusiness = new EventosPagamentoBusiness();
            itemRetorno = objBusiness.AlterarItemEventoSiafem(id, usuario, txt1, txt2, subtipo, subtipoOld, estimulo);


            return itemRetorno;
        }

        public IList<EventosPagamentoEntity> CarregarListaEventos()
        {
            EventosPagamentoBusiness objBusiness = null;
            IList<EventosPagamentoEntity> lstRetorno = null;


            objBusiness = new EventosPagamentoBusiness();
            lstRetorno = objBusiness.ListarEventos();


            return lstRetorno;
        }

		public IList<EventoSiafemEntity> CarregarListaEventoPatrimonial()
        {
            EventosPagamentoBusiness objBusiness = null;
            IList<EventoSiafemEntity> lstRetorno = null;


            objBusiness = new EventosPagamentoBusiness();
            lstRetorno = objBusiness.ListarEventoPatrimonial();


            return lstRetorno;
        }

        public IList<EventosPagamentoEntity> CarregarListaEventos(string Ano)
        {
            EventosPagamentoBusiness objBusiness = null;
            IList<EventosPagamentoEntity> lstRetorno = null;


            objBusiness = new EventosPagamentoBusiness();
            lstRetorno = objBusiness.ListarEventos(Ano);


            return lstRetorno;
        }

        public IList<string> CarregarAnoListaEventos()
        {
            EventosPagamentoBusiness objBusiness = null;
            IList<string> lstRetorno = null;


            objBusiness = new EventosPagamentoBusiness();
            lstRetorno = objBusiness.ListarAnoEventos();

            return lstRetorno;
        }

        public IList<EventosPagamentoEntity> PopularDadosRelatorio()
        {
            IList<EventosPagamentoEntity> lstRetorno = null;
            EventosPagamentoBusiness objBusiness = null;
            
            objBusiness = new EventosPagamentoBusiness();
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
            this.View.PopularDdlAno();
            this.View.PopularGrid();
        }

        public void Gravar()
        {
            EventosPagamentoBusiness objBusiness = new EventosPagamentoBusiness();
            objBusiness.Entity.Id = this.View.Id;

            objBusiness.Entity.PrimeiroCodigo = this.View.PrimeiroCodigo;
            objBusiness.Entity.PrimeiroCodigoEstorno = this.View.PrimeiroCodigoEstorno;
            objBusiness.Entity.PrimeiraInscricao = this.View.PrimeiraInscricao;
            objBusiness.Entity.PrimeiraClassificacao = this.View.PrimeiraClassificacao;

            objBusiness.Entity.SegundoCodigo = this.View.SegundoCodigo;
            objBusiness.Entity.SegundoCodigoEstorno = this.View.SegundoCodigoEstorno;
            objBusiness.Entity.SegundaInscricao = this.View.SegundaInscricao;
            objBusiness.Entity.SegundaClassificacao = this.View.SegundaClassificacao;

            objBusiness.Entity.AnoBase = this.View.AnoBase;
            objBusiness.Entity.Ativo = this.View.Ativo;
            objBusiness.Entity.UGFavorecida = this.View.UGFavorecida;
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
            EventosPagamentoBusiness objBusiness = new EventosPagamentoBusiness();           
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
            this.View.PrimeiroCodigo = 0;
            this.View.SegundoCodigo = 0;
            this.View.PrimeiroCodigoEstorno = null;
            this.View.SegundoCodigoEstorno = null;
            this.View.Descricao = null;
            this.View.AnoBase = null;
            this.View.TipoMovimentoAssociado = new TipoMovimentoEntity(0);
            this.View.TipoMaterialAssociado = 0;
        }
    }
}
