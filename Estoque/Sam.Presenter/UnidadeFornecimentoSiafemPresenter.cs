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
    public class UnidadeFornecimentoSiafemPresenter : CrudPresenter<IUnidadeFornecimentoSiafemView>
    {
        IUnidadeFornecimentoSiafemView view;

        public IUnidadeFornecimentoSiafemView View
        {
            get { return view; }
            set { view = value; }
        }

        public UnidadeFornecimentoSiafemPresenter()
        {
        }

        public UnidadeFornecimentoSiafemPresenter(IUnidadeFornecimentoSiafemView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<UnidadeFornecimentoSiafemEntity> PopularDadosUnidadeFornecimentoTodosCod()
        {
            CatalogoSiafemBusiness catalogo = new CatalogoSiafemBusiness();
            IList<UnidadeFornecimentoSiafemEntity> retorno = catalogo.ListarUnidadeFornecimentoTodosCod();
            return retorno;
        }

 
        public override void Load()
        {
            base.Load();
            this.view.PopularGrid();
        }

        public void Gravar()
        {
            CatalogoSiafemBusiness catalogo = new CatalogoSiafemBusiness();
            catalogo.UnidadeFornecimento.Id = TratamentoDados.TryParseInt32(this.view.Id);
            catalogo.UnidadeFornecimento.Codigo = this.View.Codigo;
            catalogo.UnidadeFornecimento.Descricao = this.View.Descricao;

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
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.UnidadeFornecimentoSiafem;
            relatorioImpressao.Nome = "rptUnidadeFornecimentoSiafem.rdlc";
            relatorioImpressao.DataSet = "dsUnidadeFornecimentoSiafem";
            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;
            this.View.ExibirRelatorio();
        }

        //public void Excluir()
        //{
        //    CatalogoSiafemBusiness catalogo = new CatalogoSiafemBusiness();
        //    catalogo.UnidadeFornecimento.Id = TratamentoDados.TryParseInt32(this.View.Id);
        //    if (catalogo.ExcluirUnidadeFornecimento())
        //    {
        //        this.View.PopularGrid();
        //        this.ExcluidoSucesso();
        //        this.View.ExibirMensagem("Registro excluído com sucesso!");
        //    }
        //    else
        //        this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
        //    this.View.ListaErros = catalogo.ListaErro;
        //}
    }
}
