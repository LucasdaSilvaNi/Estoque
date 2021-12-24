using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;
using Sam.Entity;
using System.Linq.Expressions;

namespace Sam.Presenter
{

    public class SistemaPresenter : CrudPresenter<ISistemaView>
    {
        ISistemaView view;
        SistemaBusiness estruturaPerfil = new SistemaBusiness();

        public ISistemaView View
        {
            get { return view; }
            set { view = value; }
        }

        public SistemaPresenter()
        {
        }

        public SistemaPresenter(ISistemaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<Sistema> PopularListaSistema()
        {
            return new Sam.Business.SistemaBusiness().ListarSistema2();
        }

        public List<TB_SISTEMA> ListarSistema(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            SistemaBusiness business = new SistemaBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarSistema();
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public TB_SISTEMA SelectOne(int id)
        {
            SistemaBusiness business = new SistemaBusiness();
            Expression<Func<TB_SISTEMA, bool>> where = a => a.TB_SISTEMA_ID == id;
            return business.SelectOne(where);
        }

        public List<TB_SISTEMA> SelectAll()
        {
            SistemaBusiness business = new SistemaBusiness();
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
                this.view.ID = trans.TB_SISTEMA_ID;                
                this.view.ATIVO = trans.TB_SISTEMA_ATIVO;
                this.view.SIGLA = trans.TB_SISTEMA_SIGLA;
                this.view.DESCRICAO = trans.TB_SISTEMA_DESCRICAO;                

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

        private bool Validar(TB_SISTEMA Sistema)
        {
            List<string> erros = new List<string>();

            if (String.IsNullOrEmpty(Sistema.TB_SISTEMA_SIGLA))
                erros.Add("O campo sigla é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(Sistema.TB_SISTEMA_DESCRICAO))
                erros.Add("O campo descrição  é de preenchimento obrigatório");

            if (String.IsNullOrEmpty(Sistema.TB_SISTEMA_SIGLA))
                erros.Add("O campo sigla é de preenchimento obrigatório");

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
            SistemaBusiness business = new SistemaBusiness();
            TB_SISTEMA Sistema = new TB_SISTEMA();

            try
            {
                Sistema.TB_SISTEMA_ID = this.view.ID;                
                Sistema.TB_SISTEMA_ATIVO = this.view.ATIVO;
                Sistema.TB_SISTEMA_SIGLA = this.view.SIGLA;
                Sistema.TB_SISTEMA_DESCRICAO = this.view.DESCRICAO;                

                if (!Validar(Sistema))
                    return;

                if (Sistema.TB_SISTEMA_ID > 0)
                {
                    //update        
                    business.Update(Sistema);
                }
                else
                {
                    //insert
                    business.Insert(Sistema);
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
            TB_SISTEMA Sistema = new TB_SISTEMA();
            SistemaBusiness business = new SistemaBusiness();

            try
            {
                Sistema.TB_SISTEMA_ID = this.view.ID;

                if (Sistema.TB_SISTEMA_ID > 0)
                {

                    business.Delete(Sistema);

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
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
    }
}
