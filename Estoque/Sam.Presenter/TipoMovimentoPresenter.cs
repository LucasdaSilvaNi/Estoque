using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using tipoMovimentacao = Sam.Common.Util.GeralEnum.TipoMovimento;
using Sam.Infrastructure;

namespace Sam.Presenter
{
    public class TipoMovimentoPresenter : CrudPresenter<ITipoMovimentoView>
    {
        ITipoMovimentoView view;

        public ITipoMovimentoView View
        {
            get { return view; }
            set { view = value; }
        }

        public TipoMovimentoPresenter()
        {
        }

        public TipoMovimentoPresenter(ITipoMovimentoView _view) : base(_view)
        {
            this.View = _view;
        }

        public IList<TipoMovimentoEntity> PopularListaTipoMovimentoEntrada()
        {
            TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento = new TipoMovimentoAgrupamentoEntity();
            tipoMovimentoAgrupamento.Id = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada;

            TipoMovimentoBusiness movimento = new TipoMovimentoBusiness();
            IList<TipoMovimentoEntity> retorno = movimento.ListarTipoMovimento(tipoMovimentoAgrupamento).Where(x=>x.Ativo == true).ToList();

            return retorno;
        }

        public IList<TipoMovimentoEntity> PopularListaTipoMovimentoConsumoImediato()
        {
            TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento = new TipoMovimentoAgrupamentoEntity();
            tipoMovimentoAgrupamento.Id = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato;

            TipoMovimentoBusiness movimento = new TipoMovimentoBusiness();
            IList<TipoMovimentoEntity> retorno = movimento.ListarTipoMovimento(tipoMovimentoAgrupamento).Where(x=>x.Ativo == true).ToList();

            return retorno;
        }

        public IList<TipoMovimentoEntity> PopularListaTipoMovimentoEntradaOuSaida(int startRowIndexParameterName, int maximumRowsParameterName, int tipoMovimentoAgrupamentoId)
        {
            TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento = new TipoMovimentoAgrupamentoEntity();

            TipoMovimentoBusiness movimento = new TipoMovimentoBusiness();
            IList<TipoMovimentoEntity> retorno = movimento.ListarTipoMovimento(new TipoMovimentoAgrupamentoEntity() { Id = tipoMovimentoAgrupamentoId }).Where(x=>x.Id != (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada).ToList();

            this.TotalRegistrosGrid = retorno.Count();

            return retorno;
        }

        public IList<TipoMovimentoEntity> RetirarTipoMovimentoDaListaTipoMovimentoEntrada(int iTipoMovimentoEntrada)
        {
            TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento = new TipoMovimentoAgrupamentoEntity();
            tipoMovimentoAgrupamento.Id = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada;

            TipoMovimentoBusiness movimento = new TipoMovimentoBusiness();
            IList<TipoMovimentoEntity> retorno = movimento.RetirarTipoMovimentoEntrada(iTipoMovimentoEntrada).Where(TipoEntrada => TipoEntrada.AgrupamentoId == tipoMovimentoAgrupamento.Id).ToList();

            return retorno;
        }

        //public TipoMovimentoEntity ExibirTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID)
        public IList<TipoMovimentoEntity> ExibirTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID)
        {
            List<TipoMovimentoEntity> listaRetorno = new List<TipoMovimentoEntity>();

            TipoMovimentoEntity lObjRetorno = new TipoMovimentoEntity();
            TipoMovimentoBusiness lObjBusiness = new TipoMovimentoBusiness();

            lObjRetorno = lObjBusiness.ExibirTipoMovimentoEntrada(iTipoMovimentoEntrada_ID);
            listaRetorno.Add(lObjRetorno);

            return listaRetorno;
        }

        public List<SubTipoMovimentoEntity> ListarSubTipoMovimento()
        {
            List<SubTipoMovimentoEntity> listaRetorno = new List<SubTipoMovimentoEntity>();
            TipoMovimentoBusiness lObjBusiness = new TipoMovimentoBusiness();

            listaRetorno = lObjBusiness.ListarSubTipoMovimento();

            return listaRetorno;
        }


        public SubTipoMovimentoEntity ListarInserirSubTipoMovimento(SubTipoMovimentoEntity objSubTipo)
        {
            SubTipoMovimentoEntity listaRetorno = new SubTipoMovimentoEntity();
            TipoMovimentoBusiness lObjBusiness = new TipoMovimentoBusiness();

            listaRetorno = lObjBusiness.ListarInserirSubTipoMovimento(objSubTipo);

            return listaRetorno;


        }


        public List<TipoMovimentoEntity> ListarTipoMovimentoAtivoNl()
        {
            List<TipoMovimentoEntity> listaRetorno = new List<TipoMovimentoEntity>();
            TipoMovimentoBusiness lObjBusiness = new TipoMovimentoBusiness();

            listaRetorno = lObjBusiness.ListarTipoMovimentoAtivoNl();

            return listaRetorno;
        }



        public IList<TipoMovimentoEntity> PopularListaTipoMovimentoSaida()
        {
            TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento = new TipoMovimentoAgrupamentoEntity();
            tipoMovimentoAgrupamento.Id = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida;

            TipoMovimentoBusiness movimento = new TipoMovimentoBusiness();
            IList<TipoMovimentoEntity> retorno = movimento.ListarTipoMovimento(tipoMovimentoAgrupamento).Where(x=>x.Ativo == true).ToList();

            //TipoMovimentoEntity requisicao = new TipoMovimentoEntity();
            //requisicao.Codigo = 20;
            //requisicao.Descricao = "Requisição";
            //requisicao.Id = 10;

            //retorno = retorno.Where(a => (a.Codigo >= 20 && a.Codigo <= 24 &&  a.Codigo != 21)).ToList();
            //retorno = retorno.Where(a =>  a.Codigo != 21).ToList();
            //retorno = retorno.Where(tipoMovimentacaoSaida => tipoMovimentacaoSaida.Id != (int)tipoMovimentacao.RequisicaoAprovada).ToList();

            //if (retorno != null)
            //{
            //    if (retorno.Count > 0)
            //    {
            //        if (retorno[0] != null)
            //            retorno[0].Descricao = "Requisição";
            //    }
            //}
            //retorno.Insert(0, requisicao);

            retorno.Where(x => x.Id == 10).ToList().ForEach(y => y.Descricao = "Requisição");

            return retorno;
        }

        public IList<TipoMovimentoEntity> PopularListaTipoRequisicao()
        {
            TipoMovimentoBusiness tipoMovimentoBusiness = new TipoMovimentoBusiness();
            return tipoMovimentoBusiness.ListarTipoMovimento(new TipoMovimentoAgrupamentoEntity((int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Requisicao));
        }

        public IList<TipoMovimentoEntity> Listar()
        {
            IList<TipoMovimentoEntity> lstRetorno = null;
            TipoMovimentoBusiness objBusiness = null;

            objBusiness = new TipoMovimentoBusiness();
            lstRetorno = objBusiness.Listar();

            return lstRetorno;
        }

        public TipoMovimentoEntity Recupera(int Id)
        {
            TipoMovimentoBusiness business = new TipoMovimentoBusiness();
            TipoMovimentoEntity tipoMov = business.Recupera(Id);

            return tipoMov;
        }

        public void Gravar(TipoMovimentoEntity tipoMovimento)
        {
            try
            {

                if (!Validar(tipoMovimento))
                    return;

                var business = new TipoMovimentoBusiness();
                business.Salvar(tipoMovimento);

                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");

                this.View.BloqueiaGravar = false;
                this.View.BloqueiaCancelar = false;
                this.View.ListaErros = null;
                this.View.MostrarPainelEdicao = false;
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem(ex.Message);
            }
        }

        private bool Validar(TipoMovimentoEntity tipoMovimento)
        {
            List<string> erros = new List<string>();

            if (tipoMovimento.Id == 0)
                erros.Add("O registro se encntra com Id inválido");

            if (tipoMovimento.Codigo == 0)
                erros.Add("O registro se encontra com o código inválido");

            if (erros.Count() > 0)
            {
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
                this.View.ListaErros = erros;
                return false;
            }
            else
            {
                return true;
            }
        }

        public void LoadData()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.MostrarPainelEdicao = false;
        }

        public void EstadoEdicao()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.MostrarPainelEdicao = true;
        }

        public void EstadoCancelar()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.MostrarPainelEdicao = false;
        }
    }
}
