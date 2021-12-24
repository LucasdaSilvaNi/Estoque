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
    public class TransacaoPerfilPresenter : CrudPresenter<ITransacaoPerfilView>

    {
        ITransacaoPerfilView view;
        TransacaoPerfilBusiness estruturaPerfil = new TransacaoPerfilBusiness();        

        public ITransacaoPerfilView View
        {
            get { return view; }
            set { view = value; }
        }

        public TransacaoPerfilPresenter()
        {
        }

        public TransacaoPerfilPresenter(ITransacaoPerfilView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public List<TB_TRANSACAO_PERFIL> ListarTransacaoPerfil(int startRowIndexParameterName, int maximumRowsParameterName, int moduloId, int perfilId, bool ativo)
        {
            TransacaoPerfilBusiness business = new TransacaoPerfilBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarTransacaoPerfil(moduloId, perfilId, ativo);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public TB_TRANSACAO_PERFIL SelectOne(int id)
        {
            TransacaoPerfilBusiness business = new TransacaoPerfilBusiness();
            Expression<Func<TB_TRANSACAO_PERFIL, bool>> where = a => a.TB_TRANSACAO_PERFIL_ID == id;
            return business.SelectOne(where);
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
            this.View.PopularListaEdit();
            LimparCampos();
        }
        public override void RegistroSelecionado()
        {
            this.View.PopularListaEdit();
            base.RegistroSelecionado();
            View.BloqueiaExcluir = false;

            var trans = SelectOne(this.view.ID);

            if (trans != null)
            {
                this.view.ID = trans.TB_TRANSACAO_PERFIL_ID;
                this.view.ATIVO = trans.TB_TRANSACAO_ATIVO;
                this.view.TRANSACAO_ID = trans.TB_TRANSACAO_ID;
                this.view.PERFIL_ID = trans.TB_PERFIL_ID;
                this.view.EDITA = trans.TB_TRANSACAO_PERFIL_EDITA;
                this.view.FILTRA_COMBO = trans.TB_TRANSACAO_FILTRA_COMBO;

                this.View.BloqueiaGravar = false;
                this.View.BloqueiaNovo = true;
                this.View.BloqueiaCancelar = false;
            }
            
        }

        public override void Load()
        {
            this.View.MostrarPainelEdicao = false;
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = false;
            this.View.PopularListaModulo();
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
        }

        private bool Validar(TB_TRANSACAO_PERFIL transacao)
        {

            List<string> erros = new List<string>();

            if(transacao.TB_PERFIL_ID == 0)
            erros.Add("O campo perfil é de preenchimento obrigatório");

            if (transacao.TB_TRANSACAO_ID == 0)
                erros.Add("O campo Transação é de preenchimento obrigatório");

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

        public void Gravar()
        {
            TransacaoPerfilBusiness business = new TransacaoPerfilBusiness();
            TB_TRANSACAO_PERFIL transacao = new TB_TRANSACAO_PERFIL();

            try
            {
                transacao.TB_TRANSACAO_PERFIL_ID = this.view.ID;
                transacao.TB_TRANSACAO_ID = this.view.TRANSACAO_ID;
                transacao.TB_PERFIL_ID = this.view.PERFIL_ID;
                transacao.TB_TRANSACAO_ATIVO = this.view.ATIVO;
                transacao.TB_TRANSACAO_PERFIL_EDITA = this.view.EDITA;
                transacao.TB_TRANSACAO_FILTRA_COMBO = this.view.FILTRA_COMBO;

                if (!Validar(transacao))
                    return;

                if (transacao.TB_TRANSACAO_PERFIL_ID > 0)
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
             TB_TRANSACAO_PERFIL transacao = new TB_TRANSACAO_PERFIL();
            TransacaoPerfilBusiness business = new TransacaoPerfilBusiness();

            try
            {
                transacao.TB_TRANSACAO_PERFIL_ID = this.view.ID;

                if (transacao.TB_TRANSACAO_PERFIL_ID > 0)
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
            this.view.ID = 0;
            this.view.ATIVO =true;
            this.view.TRANSACAO_ID = 0;
            this.view.PERFIL_ID = 0;
            this.view.EDITA = false;
            this.view.FILTRA_COMBO = false;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int moduloId, int perfilId, bool ativo)
        {
            return this.TotalRegistrosGrid;
        }
    }

}
