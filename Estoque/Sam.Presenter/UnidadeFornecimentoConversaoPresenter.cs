using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Business;
using System.Collections;
using System.ComponentModel;
using Sam.Domain.Entity;
using Sam.Business;
using Sam.Entity;



namespace Sam.Presenter
{
    public class UnidadeFornecimentoConversaoPresenter : CrudPresenter<IUnidadeFornecimentoConversaoView>
    {
        new IUnidadeFornecimentoConversaoView view;

        public new IUnidadeFornecimentoConversaoView View
        {
            get { return view; }
            set { view = value; }
        }

        public UnidadeFornecimentoConversaoPresenter()
        {
        }

        public UnidadeFornecimentoConversaoPresenter(IUnidadeFornecimentoConversaoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<UnidadeFornecimentoEntity>          ListarUnidadeFornecimentoSistemaSAM(int _OrgaoId)
        {

            CatalogoBusiness catalogo = new CatalogoBusiness();

            IList<UnidadeFornecimentoEntity> retorno = catalogo.ListarUnidadeFornecimento(_OrgaoId, true).OrderBy(UnidadeFornecimento => UnidadeFornecimento.Codigo).ToList();
            this.TotalRegistrosGrid = catalogo.TotalRegistros;
            return retorno;
        }
        public IList<UnidadeFornecimentoSiafEntity>      ListarUnidadeFornecimentoSistemaSIAFISICO()
        {

            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            IList<UnidadeFornecimentoSiafEntity> objRetorno = objBusiness.ListarUnidadeFornecimentoSiafisico();
            this.TotalRegistrosGrid = objRetorno.Count;

            return objRetorno;
        }
        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadeFornecimentodeConversao(int gestorID)
        {
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            IList<UnidadeFornecimentoConversaoEntity> objRetorno = objBusiness.ListarUnidadesDeConversaoPorGestor(gestorID);
            this.TotalRegistrosGrid = objRetorno.Count;

            return objRetorno;
        }
        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadeFornecimentodeConversao(int gestorID, string strUnidadeSiafisico, int unidadeSamId)
        {
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            IList<UnidadeFornecimentoConversaoEntity> objRetorno = objBusiness.ListarUnidadesDeConversaoPorGestor(gestorID);
            this.TotalRegistrosGrid = objRetorno.Count;

            return objRetorno;
        }

        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(int? gestorID, string strUnidadeSiafisico, int subitemMaterialId)
        {
            UnidadeFornecimentoConversaoEntity objRetorno = null;
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            objRetorno = objBusiness.ObterDadosUnidadeFornecimentoConversao(gestorID, strUnidadeSiafisico, subitemMaterialId);

            return objRetorno;
        }

        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(string strDescricaoUnidadeSIAFISICO, int subitemMaterialId)
        {
            UnidadeFornecimentoConversaoEntity objRetorno = null;
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            objRetorno = objBusiness.ObterDadosUnidadeFornecimentoConversao(strDescricaoUnidadeSIAFISICO, subitemMaterialId);

            return objRetorno;
        }

        public UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiaifisico(string strCodigoUnidadeFornecimentoSiafisico)
        {
            UnidadeFornecimentoSiafEntity objRetorno = null;
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();

            objRetorno = objBusiness.ObterUnidadeFornecimentoSiafisico(strCodigoUnidadeFornecimentoSiafisico);

            return objRetorno;
        }

        public IList<UnidadeFornecimentoConversaoEntity> PopularDadosRelatorio(int _gestorId)
        {

            UnidadeFornecimentoConversaoBusiness      objBusiness = new UnidadeFornecimentoConversaoBusiness();
            IList<UnidadeFornecimentoConversaoEntity> objRetorno = objBusiness.ListarUnidadesDeConversaoPorGestor(_gestorId);

            return objRetorno;
        }
        public IList<GestorEntity>                       PopularListaGestorPorOrgao(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            return retorno;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaUnidadeFornecimentoSistemaSAM(this.View.OrgaoId, this.View.GestorId);
            this.View.PopularListaUnidadeFornecimentoSistemaSiafisico();
            //this.view.PopularGrid();
            //this.View.BloqueiaDescricao = false;
            //this.View.BloqueiaCodigo    = false;
            //this.View.BloqueiaGravar    = false;
            //this.View.BloqueiaNovo      = false;
            //this.View.BloqueiaCancelar  = false;
        }

        public void Gravar()
        {
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();
            objBusiness.UnidadeFornecimentoConversao.Id                     = TratamentoDados.TryParseInt32(this.view.Id);
            objBusiness.UnidadeFornecimentoConversao.Codigo                 = this.View.Codigo;
            objBusiness.UnidadeFornecimentoConversao.Descricao              = this.View.Descricao;
            objBusiness.UnidadeFornecimentoConversao.SistemaSamId           = this.View.UnidadeFornecimentoSistemaSamId;
            objBusiness.UnidadeFornecimentoConversao.SistemaSiafisicoCodigo = this.View.UnidadeFornecimentoSistemaSiafisicoId;
            objBusiness.UnidadeFornecimentoConversao.FatorUnitario          = this.View.FatorUnitario;

            if (objBusiness.SalvarUnidadeFornecimentoDeConversao())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = objBusiness.ListaErro;

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.UnidadeFornecimento;
            //RelatorioEntity.Nome = "rptUnidadeFornecimento.rdlc";
            //RelatorioEntity.DataSet = "dsUnidadeFornecimento";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;
            //this.View.ExibirRelatorio();
        }

        public void Excluir()
        {
            UnidadeFornecimentoConversaoBusiness objBusiness = new UnidadeFornecimentoConversaoBusiness();
            objBusiness.UnidadeFornecimentoConversao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (objBusiness.ExcluirUnidadeFornecimentoDeConversao())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = objBusiness.ListaErro;
        }
    }
}
