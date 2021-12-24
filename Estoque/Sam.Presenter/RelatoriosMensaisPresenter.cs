using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Domain.Business;
using System.Collections;
using System.ComponentModel;

namespace Sam.Presenter
{
    public class RelatoriosMensaisPresenter : CrudPresenter<IRelatoriosMensaisView>
    {
        IRelatoriosMensaisView view;

        public IRelatoriosMensaisView View
        {
            get { return view; }
            set { view = value; }
        }

        public RelatoriosMensaisPresenter()
        {
        }

        public RelatoriosMensaisPresenter(IRelatoriosMensaisView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<object> PopularDadosRelatorio(int _ugeId, int _almoxId, int _grupoId)
        {
            SaldoSubItemBusiness estrutura = new SaldoSubItemBusiness();
            IList<SaldoSubItemEntity> retorno = estrutura.ImprimirConsultaEstoqueSintetico(_ugeId, _almoxId, _grupoId, 0, 0);
            return (IList<Object>)retorno;
        }

        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            int idRelatorio = Convert.ToInt32(this.View.NomeRelatorio);
            switch (idRelatorio)
            {
                case (int)RelatorioEnum.MensalBalancete:
                case (int)RelatorioEnum.MensalBalanceteConsumo:
                case (int)RelatorioEnum.MensalBalancetePatrimonio:
                case (int)RelatorioEnum.BalanceteSimulacao:
                    relatorioImpressao.Nome = "rptRelMensalBalancete.rdlc";
                    break;
                case (int)RelatorioEnum.MensalAnalitico:
                case (int)RelatorioEnum.MensalAnaliticoConsumo:
                case (int)RelatorioEnum.MensalAnaliticoPatrimonio:
                    relatorioImpressao.Nome = "rptRelMensalAnalitico.rdlc";
                    break;
                case (int)RelatorioEnum.MensalGrupoClasseMaterial:
                    relatorioImpressao.Nome = "rptRelMensalGrupoClasseMaterial.rdlc";
                    break;
                case (int)RelatorioEnum.MensalInventario:
                    relatorioImpressao.Nome = "rptRelMensalInventario.rdlc";
                    break;
                case (int)RelatorioEnum.ExportacaoCustos:
                    relatorioImpressao.Nome = "ExportCustos.rdlc";
                    break;
                case (int)RelatorioEnum.ExportacaoCustosConsumoImediato:
                    relatorioImpressao.Nome = "ExportCustos.rdlc";
                    break;
                case (int)RelatorioEnum.BalanceteAnual:
                    relatorioImpressao.Nome = "Balancete_Anual.rdlc";
                    break;
                default:
                    break;
            }

            //RelatorioEntity.Id = Convert.ToInt32(this.View.NomeRelatorio);   
            //RelatorioEntity.DataSet = "dsFechamentoMensal";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;
            relatorioImpressao.Id = Int32.Parse(this.View.NomeRelatorio);

            switch (idRelatorio)
            {
                case (int)RelatorioEnum.ExportacaoCustos:
                    relatorioImpressao.DataSet = "dsPtRes";
                    break;
                case (int)RelatorioEnum.ExportacaoCustosConsumoImediato:
                    relatorioImpressao.DataSet = "dsPtRes";
                    break;
                case (int)RelatorioEnum.BalanceteAnual:
                    relatorioImpressao.DataSet = "dsFechamentoAnual";
                    break;
                default:
                    relatorioImpressao.DataSet = "dsFechamentoMensal";
                    break;
            }
                

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

    }
}
