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
    public class NotificacaoPresenter : CrudPresenter<INotificacaoView>

    {
        INotificacaoView view;
        NotificacaoBusiness estruturaPerfil = new NotificacaoBusiness();        

        public INotificacaoView View
        {
            get { return view; }
            set { view = value; }
        }

        public NotificacaoPresenter()
        {
        }

        public NotificacaoPresenter(INotificacaoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<TB_NOTIFICACAO> ListarNotificacao(int startRowIndexParameterName, int maximumRowsParameterName, bool ativo, int? perfilId)
        {
            NotificacaoBusiness business = new NotificacaoBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            if (perfilId == 0)
                perfilId = null; //No banco de dados null é todos os perfirs

            var result = business.ListarNotificacao(a => (a.TB_NOTIFICACAO_IND_ATIVO == ativo) && (a.TB_PERFIL_ID == perfilId || a.TB_PERFIL_ID == null && perfilId == null));

            foreach (var r in result)
            {
                if (r.TB_PERFIL == null)
                {
                    var perfilTodos = new TB_PERFIL();
                    perfilTodos.TB_PERFIL_ID = 0;
                    perfilTodos.TB_PERFIL_DESCRICAO = "Todos os Perfis";

                    r.TB_PERFIL = perfilTodos;
                }
            }

            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public List<TB_NOTIFICACAO> CarregarNotificacoesUsuario()
        {

            //Resgata o perfil do usuário logado
            int perfilId = Acesso.Transacoes.Perfis[0].IdPerfil;


            NotificacaoBusiness business = new NotificacaoBusiness();
            var result = business.SelectWhere(a => (a.TB_NOTIFICACAO_IND_ATIVO == true) && (a.TB_PERFIL_ID == perfilId || a.TB_PERFIL_ID == null));

            return result;
        }

        public TB_NOTIFICACAO SelectOne(int id)
        {
            NotificacaoBusiness business = new NotificacaoBusiness();
            Expression<Func<TB_NOTIFICACAO, bool>> where = a => a.TB_NOTIFICACAO_ID == id;
            return business.SelectOne(where);       
        }

        public List<TB_NOTIFICACAO> SelectAll()
        {
            NotificacaoBusiness business = new NotificacaoBusiness();
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
            LimparCampos();
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
            View.BloqueiaExcluir = false;

            var trans = SelectOne(this.view.ID);

            if (trans != null)
            {
                this.view.ID = trans.TB_NOTIFICACAO_ID;
                this.view.PERFIL_ID = trans.TB_PERFIL_ID;
                this.view.ATIVO = trans.TB_NOTIFICACAO_IND_ATIVO;                
                this.view.TITULO = trans.TB_NOTIFICACAO_TITULO;
                this.view.MENSAGEM = trans.TB_NOTIFICACAO_MENSAGEM;
                this.view.DATA = trans.TB_NOTIFICACAO_DATA;

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

        public void Gravar()
        {
            NotificacaoBusiness business = new NotificacaoBusiness();
            TB_NOTIFICACAO Notificacao = new TB_NOTIFICACAO();

            try
            {
                Notificacao.TB_NOTIFICACAO_ID = this.view.ID;
                Notificacao.TB_PERFIL_ID = this.view.PERFIL_ID;
                Notificacao.TB_NOTIFICACAO_DATA = this.view.DATA;
                Notificacao.TB_NOTIFICACAO_TITULO = this.view.TITULO;
                Notificacao.TB_NOTIFICACAO_MENSAGEM = this.view.MENSAGEM;
                Notificacao.TB_NOTIFICACAO_IND_ATIVO = (bool)this.view.ATIVO;

                if (Notificacao.TB_NOTIFICACAO_ID > 0)
                {
                    //update        
                    business.Update(Notificacao);
                }
                else
                {
                    //insert
                    business.Insert(Notificacao);
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
             TB_NOTIFICACAO Notificacao = new TB_NOTIFICACAO();
            NotificacaoBusiness business = new NotificacaoBusiness();

            try
            {
                Notificacao.TB_NOTIFICACAO_ID = this.view.ID;

                if (Notificacao.TB_NOTIFICACAO_ID > 0)
                {
                    
                    business.Delete(Notificacao);

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
            view.PERFIL_ID = null;
            view.ATIVO = true;
            view.DATA = DateTime.MinValue;
            view.TITULO = string.Empty;
            view.MENSAGEM = string.Empty;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, bool ativo, int? perfilId)
        {
            return this.TotalRegistrosGrid;
        }
    }
}
