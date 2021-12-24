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
    public class NaturezaDespesaPresenter : CrudPresenter<INaturezaDespesaView>
    {
        INaturezaDespesaView view;

        public INaturezaDespesaView View
        {
            get { return view; }
            set { view = value; }
        }

        public NaturezaDespesaPresenter()
        {
        }

        public NaturezaDespesaPresenter(INaturezaDespesaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<NaturezaDespesaEntity> PopularDadosNatureza(int startRowIndexParameterName,
int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<NaturezaDespesaEntity> retorno = estrutura.ListarNaturezaDespesa();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<NaturezaDespesaEntity> PopularDadosNaturezaTodosCod()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<NaturezaDespesaEntity> retorno = estrutura.ListarNaturezaDespesaTodosCod();            
            return retorno;
        }

        public IList<NaturezaDespesaEntity> PopularDadosRelatorio()
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<NaturezaDespesaEntity> retorno = catalogo.ImprimirNaturezaDespesa();
            return retorno;
        }
        
        public int TotalRegistros(int startRowIndexParameterName,int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaAtividade();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.NaturezaDespesa.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int codigo;

            int.TryParse(this.View.Codigo, out codigo);

            estrutura.NaturezaDespesa.Codigo = codigo;
            estrutura.NaturezaDespesa.Descricao = this.View.Descricao;
            estrutura.NaturezaDespesa.Natureza = this.View.AtividadeNaturezaDespesaId;

            if (estrutura.SalvarNaturezaDespesa())
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
            estrutura.NaturezaDespesa.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirNaturezaDespesa())
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
