using System;
using System.Collections.Generic;
using System.Text;
using Sam.View;

namespace Sam.Presenter
{
    public abstract class EstruturaOrganizacionalPresenter : BasePresenter
    {
        public EstruturaOrganizacionalPresenter()
        { }

        IEstruturaOrganizacionalView view;

        public IEstruturaOrganizacionalView View
        {
            get { return view; }
            set { view = value; }
        }
        public EstruturaOrganizacionalPresenter(IEstruturaOrganizacionalView _view)
        {
            this.view = _view;
        }

        public int TotalRegistrosGrid
        {
            get;
            set;
        }
        public virtual void Load()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
        }

        public void RegistroSelecionado()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = true;
        }

        public void GravadoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
        }

        public void ExcluidoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
        }

        public void Novo()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaExcluir = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
        }

        public void Cancelar()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.Id = null;
        }
    }
}
