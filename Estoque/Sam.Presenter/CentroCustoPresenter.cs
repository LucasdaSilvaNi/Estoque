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
    public class CentroCustoPresenter : CrudPresenter<ICentroCustoView>
    {
        ICentroCustoView view;

        public ICentroCustoView View
        {
            get { return view; }
            set { view = value; }
        }

        public CentroCustoPresenter()
        {
        }

        public CentroCustoPresenter(ICentroCustoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<CentroCustoEntity> PopularDadosCentroCusto(int startRowIndexParameterName,
                int maximumRowsParameterName, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<CentroCustoEntity> retorno = estrutura.ListarCentroCusto(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public IList<GestorEntity> PopularListaGestorTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> lista = estrutura.ListarGestorTodosCod(_orgaoId);
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }
        public IList<CentroCustoEntity> PopularDadosTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<CentroCustoEntity> retorno = estrutura.ListarCentroCustoTodosCodPorOrgao(_orgaoId);
            retorno.Insert(0, new CentroCustoEntity(null) { Descricao = "- Selecione -"});
            return retorno;
        }

        public IList<GestorEntity> PopularListaGestor(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            return retorno;
        }

        public IList<CentroCustoEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<CentroCustoEntity> retorno = estrutura.ImprimirCentroCusto(_orgaoId, _gestorId);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularListaGestor(int.MinValue);
            this.view.PopularGrid();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.CentroCusto.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.CentroCusto.Codigo = this.View.Codigo;
            estrutura.CentroCusto.Descricao = this.View.Descricao;
            estrutura.CentroCusto.Orgao = (new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId)));
            estrutura.CentroCusto.Gestor = (new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId)));

            if (estrutura.SalvarCentroCusto())
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
            estrutura.CentroCusto.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirCentroCusto())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.CentroCusto;
            //RelatorioEntity.Nome = "rptCentroCusto.rdlc";
            //RelatorioEntity.DataSet = "dsCentroCusto";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.CentroCusto;
            relatorioImpressao.Nome = "rptCentroCusto.rdlc";
            relatorioImpressao.DataSet = "dsCentroCusto";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
