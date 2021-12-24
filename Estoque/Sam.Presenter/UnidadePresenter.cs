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
    public class UnidadePresenter : CrudPresenter<IUnidadeView>
    {
        IUnidadeView view;

        public IUnidadeView View
        {
            get { return view; }
            set { view = value; }
        }

        public UnidadePresenter()
        {
        }

        public UnidadePresenter(IUnidadeView _view):base(_view)
        {
            this.View = _view;
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

        public IList<UnidadeEntity> PopularDadosUnidade(int startRowIndexParameterName,
                int maximumRowsParameterName, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<UnidadeEntity> retorno = estrutura.ListarUnidade(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<UnidadeEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UnidadeEntity> retorno = estrutura.ImprimirUnidade(_orgaoId, _gestorId);
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        public IList<GestorEntity> PopularListaGestor(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            return retorno;
        }

        public IList<UnidadeEntity> PopularDadosTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UnidadeEntity> retorno = estrutura.ListarUnidadesTodosCod(_orgaoId);
            retorno.Insert(0, new UnidadeEntity(null){ Descricao = "- Selecione -"});
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
            estrutura.Unidade.Id = TratamentoDados.TryParseInt32(this.view.Id);
            if (this.View.Codigo == "")
                estrutura.Unidade.Codigo = null;
            else
                estrutura.Unidade.Codigo = Convert.ToInt64(this.View.Codigo);

            estrutura.Unidade.Descricao = this.View.Descricao;
            estrutura.Unidade.Orgao = (new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId)));
            estrutura.Unidade.Gestor = (new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId)));
            
            if (estrutura.SalvarUnidade())
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
            estrutura.Unidade.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirUnidade())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Unidade;
            //RelatorioEntity.Nome = "rptUnidade.rdlc";
            //RelatorioEntity.DataSet = "dsUnidade";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Unidade;
            relatorioImpressao.Nome = "rptUnidade.rdlc";
            relatorioImpressao.DataSet = "dsUnidade";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
