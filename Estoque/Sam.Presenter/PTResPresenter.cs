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
    public class PTResPresenter : CrudPresenter<IPTResView>
    {
        IPTResView view;

        public IPTResView View
        {
            get { return view; }
            set { view = value; }
        }

        public PTResPresenter()
        {
        }

        public PTResPresenter(IPTResView _view):base(_view)
        {
            this.View = _view;
        }

        public IList<PTResEntity> PopularDadosPTRes(int startRowIndexParameterName,
                int maximumRowsParameterName)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<PTResEntity> retorno = estrutura.ListarPTRes();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<PTResEntity> CarregarListaPTRes(int ugeCodigo)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<PTResEntity> PTREsList = estrutura.CarregarListaPTRes(ugeCodigo);
            return PTREsList;           
        }

        public IList<PTResEntity> CarregarListaPTResAcao(int ugeCodigo)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<PTResEntity> PTREsList = estrutura.CarregarListaPTResAcao(ugeCodigo);
            return PTREsList;
        }

        public IList<PTResEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<PTResEntity> retorno = estrutura.ImprimirPTRes();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
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
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.PTRes.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.PTRes.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.PTRes.Descricao = this.View.Descricao;

            if (estrutura.SalvarPTRes())
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
            estrutura.PTResM.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirPTRes())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.PTRes;
            //RelatorioEntity.Nome = "rptPTRes.rdlc";
            //RelatorioEntity.DataSet = "dsPTRes";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;
            
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.PTRes;
            relatorioImpressao.Nome = "rptPTRes.rdlc";
            relatorioImpressao.DataSet = "dsPTRes";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
