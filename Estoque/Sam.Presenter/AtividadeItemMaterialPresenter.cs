using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;


namespace Sam.Presenter
{
    [DataObject(true)]
    public class AtividadeItemMaterialPresenter: CrudPresenter<IAtividadeItemMaterialView>
    {
        IAtividadeItemMaterialView view;

        public IAtividadeItemMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public AtividadeItemMaterialPresenter()
        {
        }

        public AtividadeItemMaterialPresenter(IAtividadeItemMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<AtividadeItemMaterialEntity> PopularDadosAtividadeItemMaterial(int startRowIndexParameterName,
int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<AtividadeItemMaterialEntity> retorno = estrutura.ListarAtividadeMaterial();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        


        public int TotalRegistros(int startRowIndexParameterName,
int maximumRowsParameterName)
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

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.AtividadeMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            estrutura.AtividadeMaterial.Descricao = this.View.Descricao;

            if (estrutura.SalvarAtividadeMaterial())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;

        }

        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.AtividadeMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirAtividadeMaterial())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

    }
}
