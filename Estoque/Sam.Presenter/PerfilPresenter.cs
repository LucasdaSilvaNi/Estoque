using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;
using System.Linq.Expressions;

namespace Sam.Presenter
{
    public class PerfilPresenter : CrudPresenter<IPerfilView>
    {
        IPerfilView view;
        Sam.Business.PerfilBusiness estruturaPerfil = new Sam.Business.PerfilBusiness();

        public IPerfilView View
        {
            get { return view; }
            set { view = value; }
        }

        public PerfilPresenter()
        {
        }

        public PerfilPresenter(IPerfilView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<Perfil> PopularDadosPerfil(int? Peso)
        {
            return new Sam.Business.PerfilBusiness("").ListarPerfis(Peso);
        }

        public List<TB_PERFIL> ListarPerfil(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            PerfilBusiness business = new PerfilBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarPerfil();
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public TB_PERFIL SelectOne(int id)
        {
            PerfilBusiness business = new PerfilBusiness();
            Expression<Func<TB_PERFIL, bool>> where = a => a.TB_PERFIL_ID == id;

            return business.SelectOne(where);
        }

        public List<TB_PERFIL> SelectAll()
        {
            PerfilBusiness business = new PerfilBusiness();
            return business.SelectAll();
        }

        /// <summary>
        /// Seleciona todos os perfils no sistema, sem filtro mais o indice 0 Todos
        /// </summary>
        /// <returns></returns>
        public List<TB_PERFIL> SelectAllComTodos()
        {
            PerfilBusiness business = new PerfilBusiness();

            var perfilTodos = new TB_PERFIL();
            perfilTodos.TB_PERFIL_ID = 0;
            perfilTodos.TB_PERFIL_DESCRICAO = "Todos os perfis";

            var result = business.SelectAll();
            result.Insert(0, perfilTodos);

            return result;
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
                this.view.ID = trans.TB_PERFIL_ID;                
                this.view.ATIVO = trans.TB_PERFIL_ATIVO;                
                this.view.DESCRICAO = trans.TB_PERFIL_DESCRICAO;                
                this.view.PESO = trans.TB_PERFIL_PESO;
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

        private bool Validar(TB_PERFIL Perfil)
        {
            List<string> erros = new List<string>();

            if (String.IsNullOrEmpty(Perfil.TB_PERFIL_DESCRICAO))
                erros.Add("O campo descrição  é de preenchimento obrigatório");

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
            PerfilBusiness business = new PerfilBusiness();
            TB_PERFIL Perfil = new TB_PERFIL();

            try
            {
                Perfil.TB_PERFIL_ID = (short)view.ID;
                Perfil.TB_PERFIL_ATIVO = this.view.ATIVO;
                Perfil.TB_PERFIL_DESCRICAO = this.view.DESCRICAO;
                Perfil.TB_PERFIL_PESO = this.view.PESO;                

                if (!Validar(Perfil))
                    return;

                if (Perfil.TB_PERFIL_ID > 0)
                {
                    //update        
                    business.Update(Perfil);
                }
                else
                {
                    //insert
                    business.Insert(Perfil, view.PERFILNIVEL);

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
            TB_PERFIL Perfil = new TB_PERFIL();
            PerfilBusiness business = new PerfilBusiness();

            try
            {
                Perfil.TB_PERFIL_ID = (short)view.ID;

                if (Perfil.TB_PERFIL_ID > 0)
                {

                    business.DeleteRelatedEntries(Perfil);                    

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
            view.ATIVO = true;            
            view.DESCRICAO = string.Empty;            
            view.PESO = null;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
    }
}
