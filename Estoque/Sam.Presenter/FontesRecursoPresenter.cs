using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.ComponentModel;

namespace Sam.Presenter
{
    //[DataObject(true)]
    public class FontesRecursoPresenter : CrudPresenter<IFontesRecursoView>
    {
        IFontesRecursoView view;

        public IFontesRecursoView View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
            }
        }

       public FontesRecursoPresenter()
        {

        }

       public FontesRecursoPresenter(IFontesRecursoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularGrid();
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<FontesRecursoEntity> PopularGrid(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<FontesRecursoEntity> retorno = estrutura.ListarFontesRecurso();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<FontesRecursoEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<FontesRecursoEntity> retorno = estrutura.ImprimirFontesRecurso();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.FontesRecurso.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.FontesRecurso.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.FontesRecurso.Descricao = this.View.Descricao;

            if (estrutura.SalvarFontesRecurso())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com Sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            }

            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.FontesRecurso.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirFontesRecurso())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            }
             
            this.View.ListaErros = estrutura.ListaErro;
            
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.FonteRecurso;
            //RelatorioEntity.Nome = "rptFonteRecurso.rdlc";
            //RelatorioEntity.DataSet = "dsFonteRecurso";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.FonteRecurso;
            relatorioImpressao.Nome = "rptFonteRecurso.rdlc";
            relatorioImpressao.DataSet = "dsFonteRecurso";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
