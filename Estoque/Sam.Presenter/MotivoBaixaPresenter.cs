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
    public class MotivoBaixaPresenter : CrudPresenter<IMotivoBaixaView>
    {
        IMotivoBaixaView view;

        public IMotivoBaixaView View
        {
            get { return view; }
            set { view = value; }
        }

        public MotivoBaixaPresenter()
        {
        }

        public MotivoBaixaPresenter(IMotivoBaixaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<MotivoBaixaEntity> PopularDadosMotivoBaixa(int startRowIndexParameterName,
                int maximumRowsParameterName)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<MotivoBaixaEntity> retorno = estrutura.ListarMotivoBaixa();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<MotivoBaixaEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<MotivoBaixaEntity> retorno = estrutura.ImprimirMotivoBaixa();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.BloqueiaCodigoTransacao = false;
            this.view.PopularGrid();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.MotivoBaixa.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.MotivoBaixa.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.MotivoBaixa.Descricao = this.View.Descricao;
            estrutura.MotivoBaixa.CodigoTransacao = this.View.CodigoTransacao;

            if (estrutura.SalvarMotivoBaixa())
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
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.MotivoBaixa.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirMotivoBaixa())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public override void Novo()
        {
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = true;
            base.Novo();
        }

        public override void GravadoSucesso()
        {
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = false;
            base.ExcluidoSucesso();
        }

        public override void Cancelar()
        {
            this.View.CodigoTransacao = string.Empty;
            this.view.BloqueiaCodigoTransacao = false;
            base.Cancelar();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaCodigoTransacao = true;
            base.RegistroSelecionado();
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.MotivoBaixa;
            //RelatorioEntity.Nome = "rptMotivoBaixa.rdlc";
            //RelatorioEntity.DataSet = "dsMotivoBaixa";
            //RelatorioEntity.Parametros = this.View.ParametroRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.MotivoBaixa;
            relatorioImpressao.Nome = "rptMotivoBaixa.rdlc";
            relatorioImpressao.DataSet = "dsMotivoBaixa";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
