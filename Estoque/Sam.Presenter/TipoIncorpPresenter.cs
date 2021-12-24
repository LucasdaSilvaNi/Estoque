using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;

namespace Sam.Presenter
{
    public class TipoIncorpPresenter : CrudPresenter<ITipoIncorpView>
    {
        ITipoIncorpView view;
        public ITipoIncorpView View
        {
            get { return view; }
            set { view = value; }
        }

        public TipoIncorpPresenter()
        {

        }

        public TipoIncorpPresenter(ITipoIncorpView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<TipoIncorpEntity> PopularGrid(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<TipoIncorpEntity> retorno = estrutura.ListarTiposIncorp();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<TipoIncorpEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<TipoIncorpEntity> retorno = estrutura.ImprimirTiposIncorp();
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
            this.View.PopularGrid();
        }

        public override void Novo()
        {
            base.Novo();
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = true;
        }

        public override void Cancelar()
        {
            base.Cancelar();
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = false;
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
            this.View.BloqueiaCodigoTransacao = true;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = false;
        }

        public override void ExcluidoSucesso()
        {
            base.ExcluidoSucesso();
            this.View.CodigoTransacao = string.Empty;
            this.View.BloqueiaCodigoTransacao = false;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.TipoIncorp.Id = TratamentoDados.TryParseInt32(View.Id);
            estrutura.TipoIncorp.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.TipoIncorp.Descricao = this.View.Descricao;
            estrutura.TipoIncorp.CodigoTransacao = this.View.CodigoTransacao;

            if (estrutura.SalvarTipoIncorp())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Não foi possível salvar, verifique as mensagens!");
            }

            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.TipoIncorp.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirTipoIncorp())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
            {
                this.View.PopularGrid();
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            }

            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.TipoIncorp;
            //RelatorioEntity.Nome = "rptTipoIncorp.rdlc";
            //RelatorioEntity.DataSet = "dsTipoIncorp";
            //RelatorioEntity.Parametros = this.View.ParametroRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.TipoIncorp;
            relatorioImpressao.Nome = "rptTipoIncorp.rdlc";
            relatorioImpressao.DataSet = "dsTipoIncorp";
            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;

            this.View.ExibirRelatorio();
        }
    }
}
