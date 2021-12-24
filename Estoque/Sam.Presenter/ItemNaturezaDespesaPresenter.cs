using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using Sam.Infrastructure;

namespace Sam.Presenter
{
    //[DataObject(true)]
    public class ItemNaturezaDespesaPresenter : CrudPresenter<IItemNaturezaDespesaView>
    {
        IItemNaturezaDespesaView view;

        public IItemNaturezaDespesaView View
        {
            get { return view; }
            set { view = value; }
        }

        public ItemNaturezaDespesaPresenter()
        {
        }

        public ItemNaturezaDespesaPresenter(IItemNaturezaDespesaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<TB_NATUREZA_DESPESA> PopularListaNatureza()
        {
            Sam.Business.NaturezaDespesaBusiness business = new Business.NaturezaDespesaBusiness();
            var result = business.SelectWhere(a => a.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == true);

            return result;
        }

        public IList<TB_ITEM_NATUREZA_DESPESA> PopularDadosItemNatureza(int ItemMaterialId, int startRowIndexParameterName, int maximumRowsParameterName)
        {
            Sam.Business.ItemNaturezaDespesaBusiness business = new Business.ItemNaturezaDespesaBusiness();
            var result = business.SelectWhere(a => a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO, false, a => a.TB_ITEM_MATERIAL_ID == ItemMaterialId, startRowIndexParameterName);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }
        
        public int TotalRegistros(int ItemMaterialId, int startRowIndexParameterName,int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaNatureza();
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

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.NaturezaDespesa;
            relatorioImpressao.Nome = "rptNaturezaDespesa.rdlc";
            //RelatorioEntity.DataSet = "dsNaturezaDespesa";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;
            //this.View.ExibirRelatorio();
        }

        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.NaturezaDespesa.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirNaturezaDespesa())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluÃ­do com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

    }
}
