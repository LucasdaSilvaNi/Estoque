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
    public class ConsultasPresenter : CrudPresenter<IConsultasView>
    {
        IConsultasView view;

        List<string> lstErro = new List<string>();

        public IConsultasView View
        {
            get { return view; }
            set { view = value; }
        }

        public ConsultasPresenter()
        {
        }

        public ConsultasPresenter(IConsultasView _view)
            : base(_view)
        {
            this.View = _view;
        }

        //Consulta para o relatório sintetico
        public IList<SaldoSubItemEntity> PopularDadosRelatorio(int _ugeId, int _almoxId, int _grupoId, int _ComSemSaldo, int? _ordenarPor)
        {
            SaldoSubItemBusiness estrutura = new SaldoSubItemBusiness();
            IList<SaldoSubItemEntity> retorno = estrutura.ImprimirConsultaEstoqueSintetico(_ugeId, _almoxId, _grupoId, _ComSemSaldo, _ordenarPor);

            if (retorno.Count == 0)
                throw new Exception();
            else return retorno;
        }

        //Sintetico
        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();

            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaEstoqueSintetico;

            if (this.View.AgrupadoPor)
                relatorioImpressao.Nome = "AlmoxConsultasSinteticoEstoqueUgeNatureza.rdlc";
            else
                relatorioImpressao.Nome = "ConsultaSintetica.rdlc";
                //relatorioImpressao.Nome = "AlmoxConsultasSinteticoEstoque.rdlc";

            relatorioImpressao.DataSet = "dsConsultasSinteticoEstoque";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void ImprmirEstoqueAnalitico()
        {
            if (this.View.SubItemMaterialAnalitico == "")
            {
                lstErro.Add("Informar o subitem de material.");
            }

            if (lstErro.Count > 0)
            {
                this.View.ListaErros = lstErro;
                return;
            }

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaEstoqueAnalitico;
            relatorioImpressao.Nome = "AlmoxConsultasAnaliticoEstoque.rdlc";
            relatorioImpressao.DataSet = "dsConsultasAnaliticoEstoque";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void ImprmirFichaPrateleira()
        {
            if (this.View.SubItemMaterialAnalitico == "")
            {
                lstErro.Add("Informar o subitem de material.");
            }

            if (lstErro.Count > 0)
            {
                this.View.ListaErros = lstErro;
                return;
            }

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaEstoqueAnaliticoFichaPrateleira;
            relatorioImpressao.Nome = "rptConsultaFichaPrateleira.rdlc";
            relatorioImpressao.DataSet = "dsConsultaFichaPrateleira";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        //ASPX que vai chamar
        public void ImprmirMovimento()
        {
            //Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia
            const string ConsultaMovimentacaoEntrada = "1";
            const string ConsultaMovimentacaoSaida = "2";
            const string ConsultaMovimentacaoTransferencia = "3";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();

            if (this.View.TipoMovimentacao != "0")
            {
                switch (this.View.TipoMovimentacao)
                {
                    case ConsultaMovimentacaoEntrada:
                        relatorioImpressao.Id = (int)RelatorioEnum.ConsultaMovimentacaoEntrada;
                        relatorioImpressao.Nome = "AlmoxConsultasMovimentacaoEntrada.rdlc";
                        break;
                    case ConsultaMovimentacaoSaida:
                        relatorioImpressao.Id = (int)RelatorioEnum.ConsultaMovimentacaoSaida;
                        relatorioImpressao.Nome = "AlmoxConsultasMovimentacaoSaida.rdlc";
                        break;
                    case ConsultaMovimentacaoTransferencia:
                        relatorioImpressao.Id = (int)RelatorioEnum.ConsultaMovimentacaoTransferencia;
                        relatorioImpressao.Nome = "ConsultaMovimentacaoTransferencia.rdlc";
                        break;
                    default:
                        break;
                }

                relatorioImpressao.DataSet = "dsConsultasMovimentacao";

                relatorioImpressao.Parametros = this.View.ParametrosRelatorioMovimentacao;
                this.View.DadosRelatorio = relatorioImpressao;

                this.View.ExibirRelatorio();
            }
        }

        public void ImprmirConsumo()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();

            if (this.View.TipoConsumo == "1")
            {
                relatorioImpressao.Id = (int)RelatorioEnum.ConsultaConsumoAlmox;
                relatorioImpressao.Nome = "AlmoxConsultasConsumoPorAlmoxarifado.rdlc";
                relatorioImpressao.DataSet = "dsConsultasConsumoAlmoxarifado";
            }
            if (this.View.TipoConsumo == "2")
            {
                relatorioImpressao.Id = (int)RelatorioEnum.ConsultaConsumoRequisitante;
                relatorioImpressao.Nome = "AlmoxConsultasConsumoPorRequisitante.rdlc";
                relatorioImpressao.DataSet = "dsConsultasConsumoRequisitante";
            }
            if (this.View.TipoConsumo == "3")
            {
                relatorioImpressao.Id = (int)RelatorioEnum.ConsulmoSubitemAlmox;
                relatorioImpressao.Nome = "ConsultaSubitemConsumo.rdlc";
                relatorioImpressao.DataSet = "dsConsulmoSubitemAlmox";
            }

            relatorioImpressao.Parametros = this.View.ParametrosRelatorioConsumo;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

    }
}
