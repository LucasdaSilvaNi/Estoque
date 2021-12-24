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
    public class AtividadeNaturezaDespesaPresenter: CrudPresenter<IAtividadeNaturezaDespesaView>
    {
        IAtividadeNaturezaDespesaView view;

        public IAtividadeNaturezaDespesaView View
        {
            get { return view; }
            set { view = value; }
        }

        public AtividadeNaturezaDespesaPresenter()
        {
        }

        public AtividadeNaturezaDespesaPresenter(IAtividadeNaturezaDespesaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<AtividadeNaturezaDespesaEntity> PopularDadosAtividadeNaturezaDespesa(int startRowIndexParameterName,
int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<AtividadeNaturezaDespesaEntity> retorno = estrutura.ListarAtividadeNatureza();
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
            estrutura.AtividadeNatureza.Id = TratamentoDados.TryParseInt32(this.View.Id);

            estrutura.AtividadeNatureza.Descricao = this.View.Descricao;

            if (estrutura.SalvarAtividadeNatureza())
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
            estrutura.AtividadeNatureza.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirAtividadeNatureza())
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
