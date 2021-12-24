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
    public class ModuloPresenter : CrudPresenter<IModuloView>
    {
        IModuloView view;
        ModuloBusiness estruturaPerfil = new ModuloBusiness();

        public IModuloView View
        {
            get { return view; }
            set { view = value; }
        }

        public ModuloPresenter()
        {
        }

        public ModuloPresenter(IModuloView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public List<TB_MODULO> ListarModulo(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            ModuloBusiness business = new ModuloBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarModulo();
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public TB_MODULO SelectOne(int id)
        {
            ModuloBusiness business = new ModuloBusiness();
            Expression<Func<TB_MODULO, bool>> where = a => a.TB_MODULO_ID == id;
            return business.SelectOne(where);
        }

        public List<TB_MODULO> SelectAll()
        {
            ModuloBusiness business = new ModuloBusiness();
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
            this.View.PopularDllSistema();
        }

        public override void RegistroSelecionado()
        {
            this.View.PopularDllSistema();

            base.RegistroSelecionado();
            View.BloqueiaExcluir = false;

            var trans = SelectOne(this.view.ID);

            if (trans != null)
            {
                this.view.ID = trans.TB_MODULO_ID;

                this.view.ATIVO = trans.TB_MODULO_IND_ATIVO;
                this.view.SIGLA = trans.TB_MODULO_SIGLA;
                this.view.DESCRICAO = trans.TB_MODULO_DESCRICAO;
                this.view.CAMINHO = trans.TB_MODULO_CAMINHO;
                this.view.ORDEM = trans.TB_MODULO_ORDEM;
                this.view.ID_PAI = trans.TB_MODULO_ID_PAI;
                this.view.SISTEMA_ID = trans.TB_SISTEMA_ID;

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
            this.View.PopularGrid();
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

        private bool Validar(TB_MODULO Modulo)
        {
            List<string> erros = new List<string>();

            if (Modulo.TB_SISTEMA_ID == 0)
                erros.Add("O campo módulo é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(Modulo.TB_MODULO_SIGLA))
                erros.Add("O campo sigla é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(Modulo.TB_MODULO_DESCRICAO))
                erros.Add("O campo descrição  é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(Modulo.TB_MODULO_CAMINHO))
                erros.Add("O campo caminho é de preenchimento obrigatório");

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
            ModuloBusiness business = new ModuloBusiness();
            TB_MODULO modulo = new TB_MODULO();

            try
            {
                modulo.TB_MODULO_ID = this.view.ID;
                modulo.TB_MODULO_IND_ATIVO = this.view.ATIVO;
                modulo.TB_MODULO_SIGLA = this.view.SIGLA;
                modulo.TB_MODULO_DESCRICAO = this.view.DESCRICAO;
                modulo.TB_MODULO_CAMINHO = this.view.CAMINHO;
                modulo.TB_MODULO_ORDEM = this.view.ORDEM;
                modulo.TB_MODULO_ID_PAI = this.view.ID_PAI;
                modulo.TB_SISTEMA_ID = this.view.SISTEMA_ID;

                if (!Validar(modulo))
                    return;

                business.Save(modulo);
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
            TB_MODULO Modulo = new TB_MODULO();
            ModuloBusiness business = new ModuloBusiness();

            try
            {
                Modulo.TB_MODULO_ID = this.view.ID;

                if (Modulo.TB_MODULO_ID > 0)
                {

                    business.Delete(Modulo);

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
            view.SIGLA = string.Empty;
            view.DESCRICAO = string.Empty;
            view.CAMINHO = string.Empty;
            view.ORDEM = null;
            view.ID_PAI = null;


        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
    }

}
