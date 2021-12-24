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
    
    public class ClassePresenter : CrudPresenter<IClasseView>
    {
        IClasseView view;

        public IClasseView View
        {
            get { return view; }
            set { view = value; }
        }

        public ClassePresenter()
        {
        }

        public ClassePresenter(IClasseView _view)
            : base(_view)
        {
            this.View = _view;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ClasseEntity> PopularDadosClasse(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<ClasseEntity> retorno = estrutura.ListarClasse(_grupoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ClasseEntity> PopularDadosClasseComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<ClasseEntity> retorno = estrutura.ListarClasseTodosCod(_grupoId);
            return retorno;
        }

        public IList<ClasseEntity> PopularDadosRelatorio(int _grupoId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<ClasseEntity> retorno = estrutura.ImprimirClasse(_grupoId);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.view.PopularListaGrupo();
            this.view.PopularGrid();
        }

        public void Gravar()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Classe.Id = TratamentoDados.TryParseInt32(this.view.Id);

            int codigo, grupo;           
            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.GrupoId, out grupo);

            estrutura.Classe.Codigo = codigo;
            estrutura.Classe.Descricao = this.View.Descricao;
            estrutura.Classe.Grupo = (new GrupoEntity(grupo));
            if (estrutura.SalvarClasse())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                if (this.View.Codigo.Length > 2)
                    this.View.Codigo = this.View.Codigo.Remove(0, 2);
            }
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Classe.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirClasse())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Classe;
            //RelatorioEntity.Nome = "rptClasse.rdlc";
            //RelatorioEntity.DataSet = "dsClasse";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Classe;
            relatorioImpressao.Nome = "rptClasse.rdlc";
            relatorioImpressao.DataSet = "dsClasse";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public ClasseEntity ObterClasseMaterial(int codigoClasseMaterial)
        {
            ClasseEntity objRetorno = null;

            CatalogoBusiness lObjBusiness = new CatalogoBusiness();
            objRetorno = lObjBusiness.ObterClasse(codigoClasseMaterial);

            return objRetorno;
        }
    }
}
