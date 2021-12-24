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
    public class UnidadeFornecimentoPresenter : CrudPresenter<IUnidadeFornecimentoView>
    {
        IUnidadeFornecimentoView view;

        public IUnidadeFornecimentoView View
        {
            get { return view; }
            set { view = value; }
        }

        public UnidadeFornecimentoPresenter()
        {
        }

        public UnidadeFornecimentoPresenter(IUnidadeFornecimentoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<UnidadeFornecimentoEntity> PopularDadosUnidadeFornecimento(int startRowIndexParameterName,
                int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {

            CatalogoBusiness catalogo = new CatalogoBusiness();

            catalogo.SkipRegistros = startRowIndexParameterName;
            IList<UnidadeFornecimentoEntity> retorno = catalogo.ListarUnidadeFornecimento(_orgaoId, _gestorId);
            this.TotalRegistrosGrid = catalogo.TotalRegistros;
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

        public IList<UnidadeFornecimentoEntity> PopularDadosUnidadeFornecimentoTodosCod(int _orgaoId, int _gestorId)
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<UnidadeFornecimentoEntity> retorno = catalogo.ListarUnidadeFornecimentoTodosCod(_orgaoId, _gestorId);            
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {

            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<UnidadeFornecimentoEntity> retorno = catalogo.ImprimirUnidadeFornecimento(_orgaoId, _gestorId);
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorItemUge(int _ugeId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<UnidadeFornecimentoEntity> retorno = estrutura.PopularUnidFornecimentoTodosPorUge(_ugeId);
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

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId)
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
            CatalogoBusiness catalogo = new CatalogoBusiness();
            catalogo.UnidadeFornecimento.Id = TratamentoDados.TryParseInt32(this.view.Id);
            catalogo.UnidadeFornecimento.Codigo = this.View.Codigo;
            catalogo.UnidadeFornecimento.Descricao = this.View.Descricao;
            catalogo.UnidadeFornecimento.Gestor = (new GestorEntity(TratamentoDados.TryParseInt16(this.View.GestorId)));

            if (catalogo.SalvarUnidadeFornecimento())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = catalogo.ListaErro;

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.UnidadeFornecimento;
            //RelatorioEntity.Nome = "rptUnidadeFornecimento.rdlc";
            //RelatorioEntity.DataSet = "dsUnidadeFornecimento";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.UnidadeFornecimento;
            relatorioImpressao.Nome = "rptUnidadeFornecimento.rdlc";
            relatorioImpressao.DataSet = "dsUnidadeFornecimento";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Excluir()
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            catalogo.UnidadeFornecimento.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (catalogo.ExcluirUnidadeFornecimento())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = catalogo.ListaErro;
        }
    }
}
