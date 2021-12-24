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
    public class ResponsavelPresenter : CrudPresenter<IResponsavelView>
    {
        IResponsavelView view;

        public IResponsavelView View
        {
            get { return view; }
            set { view = value; }
        }

        public ResponsavelPresenter()
        {
        }

        public ResponsavelPresenter(IResponsavelView _view)
            : base(_view)
        {
            this.View = _view;
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

        public IList<ResponsavelEntity> PopularDadosResponsavel(int startRowIndexParameterName,
                int maximumRowsParameterName, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavel(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<ResponsavelEntity> PopularDadosResponsavelTodosCodPorOrgao(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavelTodosCodPorOrgao(_orgaoId);
            return retorno;
        }

        public IList<ResponsavelEntity> PopularDadosResponsavelPorOrgaoGestor(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavelPorOrgaoGestor(_orgaoId,_gestorId);
            return retorno;
        }

        public IList<ResponsavelEntity> ListarResponsavelPorUa(int UaId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Responsavel = new ResponsavelEntity();
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavelPorUa(UaId);
            return retorno;
        }

        public IList<ResponsavelEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<ResponsavelEntity> retorno = estrutura.ImprimirResponsavel(_orgaoId, _gestorId);
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

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularListaGestor(int.MinValue);
            this.View.PopularGrid();
            this.View.BloqueiaCargo = false;
            this.View.BloqueiaEndereco = false;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Responsavel.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.Responsavel.Codigo = TratamentoDados.TryParseLong(this.View.Codigo);
            estrutura.Responsavel.Descricao = this.View.Descricao;
            estrutura.Responsavel.Gestor = (new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId)));
            estrutura.Responsavel.Cargo = this.View.Cargo;
            estrutura.Responsavel.Endereco = this.View.Endereco;

            if (estrutura.SalvarResponsavel())
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
            estrutura.Responsavel.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirResponsavel())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Responsavel;
            //RelatorioEntity.Nome = "rptResponsavel.rdlc";
            //RelatorioEntity.DataSet = "dsResponsavel";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Responsavel;
            relatorioImpressao.Nome = "rptResponsavel.rdlc";
            relatorioImpressao.DataSet = "dsResponsavel";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void GravadoSucesso()
        {
            this.View.Cargo = string.Empty;
            this.View.BloqueiaCargo = false;
            this.View.Endereco = string.Empty;
            this.View.BloqueiaEndereco = false;
            base.GravadoSucesso();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaCargo = true;
            this.View.BloqueiaEndereco = true;
            base.RegistroSelecionado();
        }

        public override void ExcluidoSucesso()
        {
            this.View.Cargo = string.Empty;
            this.View.BloqueiaCargo = false;
            this.View.Endereco = string.Empty;
            this.View.BloqueiaEndereco = false;
            base.ExcluidoSucesso();
        }

        public override void Novo()
        {
            this.View.Cargo = string.Empty;
            this.View.BloqueiaCargo = true;
            this.View.Endereco = string.Empty;
            this.View.BloqueiaEndereco = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.BloqueiaCargo = false;
            this.View.BloqueiaEndereco = false;
            base.Cancelar();
        }
    }
}
