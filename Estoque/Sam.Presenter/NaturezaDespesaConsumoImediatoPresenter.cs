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
    //[DataObject(true)]
    public class NaturezaDespesaConsumoImediatoPresenter : CrudPresenter<INaturezaDespesaConsumoImediatoView>
    {
        INaturezaDespesaConsumoImediatoView view;

        public INaturezaDespesaConsumoImediatoView View
        {
            get { return view; }
            set { view = value; }
        }

        public NaturezaDespesaConsumoImediatoPresenter()
        {
        }

        public NaturezaDespesaConsumoImediatoPresenter(INaturezaDespesaConsumoImediatoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<NaturezaDespesaConsumoImediatoEntity> PopularDadosNaturezasDepesaConsumoImediato(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<NaturezaDespesaConsumoImediatoEntity> retorno = estrutura.ListarNaturezasDespesaConsumoImediatoTodosCod();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<NaturezaDespesaConsumoImediatoEntity> PopularDadosNaturezasDespesaConsumoImediatoTodosCod()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<NaturezaDespesaConsumoImediatoEntity> retorno = estrutura.ListarNaturezasDespesaConsumoImediatoTodosCod();            
            return retorno;
        }

        public IList<NaturezaDespesaConsumoImediatoEntity> PopularDadosRelatorio()
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<NaturezaDespesaConsumoImediatoEntity> retorno = catalogo.ImprimirNaturezasDespesaConsumoImediato();
            return retorno;
        }
        
        public int TotalRegistros(int startRowIndexParameterName,int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            //this.View.PopularListaNaturezasDespesaConsumoImediato();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.NaturezaDespesaConsumoImediato.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo).Value;

            int codigo;

            int.TryParse(this.View.Codigo, out codigo);

            estrutura.NaturezaDespesaConsumoImediato.Codigo = codigo;
            estrutura.NaturezaDespesaConsumoImediato.Descricao = this.View.Descricao;
            estrutura.NaturezaDespesaConsumoImediato.Ativa = this.View.Ativa;

            if (estrutura.SalvarNaturezaDespesaConsumoImediato())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.NaturezaDespesa;
            //RelatorioEntity.Nome = "rptNaturezaDespesa.rdlc";
            //RelatorioEntity.DataSet = "dsNaturezaDespesa";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.NaturezaDespesa;
            relatorioImpressao.Nome = "rptNaturezaDespesa.rdlc";
            relatorioImpressao.DataSet = "dsNaturezaDespesa";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.NaturezaDespesaConsumoImediato.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo).Value;

            if (estrutura.ExcluirNaturezaDespesaConsumoImediato())
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
