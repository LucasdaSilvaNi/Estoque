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
    
    public class ContaAuxiliarPresenter : CrudPresenter<IContaAuxiliarView>
    {

        IContaAuxiliarView view;

        public IContaAuxiliarView View
        {
            get { return view; }
            set { view = value; }
        }

        public ContaAuxiliarPresenter()
        {
        }

        public ContaAuxiliarPresenter(IContaAuxiliarView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ContaAuxiliarEntity> PopularDadosConta(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<ContaAuxiliarEntity> retorno = estrutura.ListarConta();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }        
        public IList<ContaAuxiliarEntity> PopularDadosContaTodosCod()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<ContaAuxiliarEntity> retorno = estrutura.ListarContaTodosCod();            
            return retorno;
        }

        public IList<ContaAuxiliarEntity> PopularDadosRelatorio()
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<ContaAuxiliarEntity> retorno = catalogo.ImprimirContaAuxiliar();
            return retorno;
        }


        public int TotalRegistros(int startRowIndexParameterName,
int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            this.View.BloqueiaCContabil = false;
            base.Load();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.Conta.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int codigo, ccontabil;

            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.ContaContabil, out ccontabil);


            estrutura.Conta.Codigo = codigo;
            estrutura.Conta.Descricao = this.View.Descricao;
            estrutura.Conta.ContaContabil = ccontabil;

            if (estrutura.SalvarConta())
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
            estrutura.Conta.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirConta())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        #region Override Functions



        public override void RegistroSelecionado()
        {
            this.View.BloqueiaCContabil = true;
            base.RegistroSelecionado();
        }

        public override void GravadoSucesso()
        {
            this.View.ContaContabil = string.Empty;
            this.View.BloqueiaCContabil = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.BloqueiaCContabil = false;
            base.ExcluidoSucesso();
        }

        public override void Novo()
        {
            this.View.ContaContabil = string.Empty;
            this.View.BloqueiaCContabil = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.BloqueiaCContabil = false;
            base.Cancelar();
        }

        #endregion

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.ContaAuxiliar;
            //RelatorioEntity.Nome = "rptContaAuxiliar.rdlc";
            //RelatorioEntity.DataSet = "dsContaAuxiliar";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ContaAuxiliar;
            relatorioImpressao.Nome = "rptContaAuxiliar.rdlc";
            relatorioImpressao.DataSet = "dsContaAuxiliar";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;
            
            this.View.ExibirRelatorio();
        }
    }
}
