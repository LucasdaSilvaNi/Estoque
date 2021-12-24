using System;
using System.Collections;
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
 
    public class GrupoPresenter : CrudPresenter<IGrupoView>
    {
         IGrupoView view;

         public IGrupoView View
        {
            get { return view; }
            set { view = value; }
        }

        public GrupoPresenter()
        {
        }

        public GrupoPresenter(IGrupoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<GrupoEntity> PopularDadosGrupo(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros  = startRowIndexParameterName;
            IList<GrupoEntity> retorno = estrutura.ListarGrupo();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<GrupoEntity> PopularDadosGrupoByAlmoxarifadoTodosCod(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            AlmoxarifadoEntity almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<GrupoEntity> retorno = estrutura.ListarGrupoTodosCod(almoxarifado);

            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<GrupoEntity> PopularDadosGrupoTodosCod(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<GrupoEntity> retorno = estrutura.ListarGrupoTodosCod();

            return retorno;
        }

        public IList<GrupoEntity> PopularDadosRelatorio()
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<GrupoEntity> retorno = catalogo.ImprimirGrupo();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName,int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.view.PopularGrid();

        }

        public void Gravar()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Grupo.Id = TratamentoDados.TryParseInt32(this.view.Id);

            int codigo; 

            int.TryParse(this.View.Codigo, out codigo);

            estrutura.Grupo.Codigo = codigo;
            estrutura.Grupo.Descricao = this.View.Descricao;
            
            if (estrutura.SalvarGrupo())
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
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Grupo.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirGrupo())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.GrupoMaterial;
            //RelatorioEntity.Nome = "rptGrupoMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsGrupoMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.GrupoMaterial;
            relatorioImpressao.Nome = "rptGrupoMaterial.rdlc";
            relatorioImpressao.DataSet = "dsGrupoMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public GrupoEntity ObterGrupoMaterial(int? iCodigoGrupoMaterial)
        {
            GrupoEntity objRetorno = null;

            CatalogoBusiness lObjBusiness = new CatalogoBusiness();
            objRetorno = lObjBusiness.ObterGrupoMaterial(iCodigoGrupoMaterial.Value);

            return objRetorno;
        }
    }
}
