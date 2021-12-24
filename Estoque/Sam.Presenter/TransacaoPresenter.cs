using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using Sam.Business;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;
using System.Linq.Expressions;

namespace Sam.Presenter
{
    public class TransacaoPresenter : CrudPresenter<ITransacaoView>

    {
        ITransacaoView view;
        TransacaoBusiness estruturaPerfil = new TransacaoBusiness();        

        public ITransacaoView View
        {
            get { return view; }
            set { view = value; }
        }

        public TransacaoPresenter()
        {
        }

        public TransacaoPresenter(ITransacaoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public List<TB_TRANSACAO> ListarTransacao(int startRowIndexParameterName, int maximumRowsParameterName, int moduloId)
        {
            TransacaoBusiness business = new TransacaoBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarTransacao(moduloId);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public List<TB_TRANSACAO> ListarTransacao(int moduloId)
        {
            TransacaoBusiness business = new TransacaoBusiness();
            var result = business.ListarTransacao(moduloId);
            return result;
        }

        public TB_TRANSACAO SelectOne(int id)
        {
            TransacaoBusiness business = new TransacaoBusiness();
            Expression<Func<TB_TRANSACAO, bool>> where = a => a.TB_TRANSACAO_ID == id;
            return business.SelectOne(where);       
        }

        public List<TB_TRANSACAO> SelectAll()
        {
            TransacaoBusiness business = new TransacaoBusiness();
            return business.SelectAll();
        }

        public override void Cancelar()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaCancelar = true;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.MostrarPainelEdicao = false;
            LimparCampos();
        }

        public override void Novo()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaNovo = true;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaExcluir = true;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.MostrarPainelEdicao = true;
            this.View.PopularListaModuloEdit();
            LimparCampos();
        }

        public override void RegistroSelecionado()
        {
            this.View.PopularListaModuloEdit();
            base.RegistroSelecionado();
            View.BloqueiaExcluir = false;

            var trans = SelectOne(this.view.ID);

            if (trans != null)
            {
                this.view.ID = trans.TB_TRANSACAO_ID;
                this.view.MODULO_ID = trans.TB_MODULO_ID;
                this.view.ATIVO = trans.TB_TRANSACAO_ATIVO;
                this.view.SIGLA = trans.TB_TRANSACAO_SIGLA;
                this.view.DESCRICAO = trans.TB_TRANSACAO_DESCRICAO;
                this.view.CAMINHO = trans.TB_TRANSACAO_CAMINHO;
                this.view.ORDEM = trans.TB_TRANSACAO_ORDEM;

                this.View.BloqueiaGravar = false;
                this.View.BloqueiaNovo = true;
                this.View.BloqueiaCancelar = false;
            }
            
        }

        public override void Load()
        {
            this.View.MostrarPainelEdicao = false;
            this.View.PopularListaModulo();

            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = false;
            LimparCampos();
        }

        public override void GravadoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = false;
            LimparCampos();
            View.PopularGrid();
        }

        public override void ExcluidoSucesso()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.Codigo = string.Empty;
            this.View.BloqueiaCodigo = false;
            this.View.Descricao = string.Empty;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = false;
            this.View.PopularGrid();
        }

        private bool Validar(TB_TRANSACAO transacao)
        {
            List<string> erros = new List<string>();

            if(transacao.TB_MODULO_ID == 0)
            erros.Add("O campo módulo é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(transacao.TB_TRANSACAO_SIGLA))
                erros.Add("O campo sigla é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(transacao.TB_TRANSACAO_DESCRICAO))
                erros.Add("O campo descrição  é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(transacao.TB_TRANSACAO_CAMINHO))
                erros.Add("O campo caminho é de preenchimento obrigatório");

            if(erros.Count() > 0)
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

        public void Gravar()
        {
            TransacaoBusiness business = new TransacaoBusiness();
            TB_TRANSACAO transacao = new TB_TRANSACAO();

            try
            {
                transacao.TB_TRANSACAO_ID = this.view.ID;
                transacao.TB_MODULO_ID = this.view.MODULO_ID;
                transacao.TB_TRANSACAO_ATIVO = this.view.ATIVO;
                transacao.TB_TRANSACAO_SIGLA = this.view.SIGLA;
                transacao.TB_TRANSACAO_DESCRICAO = this.view.DESCRICAO;
                transacao.TB_TRANSACAO_CAMINHO = this.view.CAMINHO;
                transacao.TB_TRANSACAO_ORDEM = this.view.ORDEM;

                if (!Validar(transacao))
                    return;

                if (transacao.TB_TRANSACAO_ID > 0)
                {
                    //update        
                    business.Update(transacao);
                }
                else
                {
                    //insert
                    business.Insert(transacao);
                }

                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                GravadoSucesso();
            }
            catch (Exception ex)
            {
                business.ListaErro.Add(ex.Message);
                this.View.ListaErros = business.ListaErro;
            }
        }

        public void Excluir()
        {
             TB_TRANSACAO transacao = new TB_TRANSACAO();
            TransacaoBusiness business = new TransacaoBusiness();

            try
            {
                transacao.TB_TRANSACAO_ID = this.view.ID;

                if (transacao.TB_TRANSACAO_ID > 0)
                {
                    
                    business.Delete(transacao);

                    this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                    ExcluidoSucesso();
                }
            }
            catch (Exception ex)
            {
                business.ListaErro.Add(ex.Message);
                this.View.ListaErros = business.ListaErro;
            } 
        }

        private void LimparCampos()
        {
            view.Id = null;
            view.ID = 0;
            view.MODULO_ID = 0;
            view.ATIVO = true;
            view.SIGLA = string.Empty;
            view.DESCRICAO = string.Empty;
            view.CAMINHO = string.Empty;
            view.ORDEM = null;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int moduloId)
        {
            return this.TotalRegistrosGrid;
        }
    }
}
