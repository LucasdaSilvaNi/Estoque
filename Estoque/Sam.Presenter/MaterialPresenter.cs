using System;
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
    
    public class MaterialPresenter : CrudPresenter<IMaterialView>
    {
        IMaterialView view;

        public IMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public MaterialPresenter()
        {
        }

        public MaterialPresenter(IMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<MaterialEntity> PopularDadosMaterial(int startRowIndexParameterName,int maximumRowsParameterName, int _classeId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<MaterialEntity> retorno = estrutura.ListarMaterial(_classeId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<MaterialEntity> PopularDadosMaterialComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _classeId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<MaterialEntity> retorno = estrutura.ListarTodosMaterialCod(_classeId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<MaterialEntity> PopularDadosMaterialComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _classeId, AlmoxarifadoEntity almoxarifado)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<MaterialEntity> retorno = estrutura.ListarTodosMaterialCod(_classeId, almoxarifado);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        public IList<MaterialEntity> PopularDadosRelatorio(int _classeId)
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<MaterialEntity> retorno = catalogo.ImprimirMaterial(_classeId);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName,int maximumRowsParameterName, int _classeId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.PopularListaGrupo();
            this.View.PopularListaClasse();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Material.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int codigo,  classe;

            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.ClasseId, out classe);
            
            estrutura.Material.Codigo = codigo;
            estrutura.Material.Descricao = this.View.Descricao;
            estrutura.Material.Classe = (new ClasseEntity(classe));

            if (estrutura.SalvarMaterial())
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
            estrutura.Material.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirMaterial())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Material;
            //RelatorioEntity.Nome = "rptMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Material;
            relatorioImpressao.Nome = "rptMaterial.rdlc";
            relatorioImpressao.DataSet = "dsMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public MaterialEntity ObterMaterial(int iCodigoMaterial)
        {
            MaterialEntity objRetorno = null;

            CatalogoBusiness lObjBusiness = new CatalogoBusiness();
            objRetorno = lObjBusiness.ObterMaterial(iCodigoMaterial);

            return objRetorno;
        }
    }
}
