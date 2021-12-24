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
    public class SiglaPresenter : CrudPresenter<ISiglaView>
    {
        ISiglaView view;

        public ISiglaView View
        {
            get { return view; }
            set { view = value; }
        }

        public SiglaPresenter()
        {
        }

        public SiglaPresenter(ISiglaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<SiglaEntity> PopularDadosSigla(int startRowIndexParameterName,
                int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<SiglaEntity> retorno = estrutura.ListarSigla(_orgaoId, _gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<SiglaEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<SiglaEntity> retorno = estrutura.ImprimirSigla(_orgaoId, _gestorId);
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

        public IList<IndicadorBemProprioEntity> PopularListaIndicadorBemProprio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<IndicadorBemProprioEntity> retorno = estrutura.ListarIndicadorBemProprio();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.BloqueiaListaIndicadorBemProprio = false;
            this.View.PopularListaOrgao();
            this.View.PopularListaGestor(int.MinValue);
            this.View.PopularListaIndicadorBemProprio();
            this.View.PopularGrid();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Sigla.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.Sigla.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Sigla.Descricao = this.View.Descricao;
            estrutura.Sigla.Orgao = (new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId)));
            estrutura.Sigla.Gestor = (new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId)));
            estrutura.Sigla.IndicadorBemProprio = (new IndicadorBemProprioEntity(TratamentoDados.TryParseInt32(this.View.IndicadorBemProprioId)));

            if (estrutura.SalvarSigla())
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
            estrutura.Sigla.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirSigla())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaListaIndicadorBemProprio = true;
            base.RegistroSelecionado();
        }

        public override void GravadoSucesso()
        {
            this.View.IndicadorBemProprioId = " - Selecione - ";
            this.View.BloqueiaListaIndicadorBemProprio = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.IndicadorBemProprioId = " - Selecione - ";
            this.View.BloqueiaListaIndicadorBemProprio = false;
            base.ExcluidoSucesso();
        }

        public override void Novo()
        {
            this.View.IndicadorBemProprioId = " - Selecione - ";
            this.View.BloqueiaListaIndicadorBemProprio = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.IndicadorBemProprioId = " - Selecione - ";
            this.View.BloqueiaListaIndicadorBemProprio = false;
            
            base.Cancelar();
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.Sigla;
            //RelatorioEntity.Nome = "rptSigla.rdlc";
            //RelatorioEntity.DataSet = "dsSigla";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Sigla;
            relatorioImpressao.Nome = "rptSigla.rdlc";
            relatorioImpressao.DataSet = "dsSigla";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
