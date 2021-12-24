using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using System.Web;
using Sam.Infrastructure;
using Sam.Business;

namespace Sam.Presenter
{
    public class GestorPresenter : CrudPresenter<IGestorView> 
    {
        IGestorView view;

        public IGestorView View
        {
            get { return view; }
            set { view = value; }
        }

        public GestorPresenter()
        {
        }

        public GestorPresenter(IGestorView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public int RetornaGestorOrganizacional(int? orgaoId, int? uoId, int? ugeId)
        {
            return new EstruturaOrganizacionalBusiness().RetornaGestorOrganizacional(orgaoId, uoId, ugeId);
        }

        public IList<GestorEntity> PopularDadosGestor(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<TB_GESTOR> ListarGestor(int OrgaoId)
        {
            GestorBusiness business = new GestorBusiness();
            return business.ListarGestor(OrgaoId);
        }

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public IList<GestorEntity> PopularDadosGestorTodos()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestorTodosCod();
            retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }

        public IList<GestorEntity> PopularDadosGestorTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestorTodosCod(_orgaoId);
            //retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }

        public IList<GestorEntity> PopularDadosRelatorio(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ImprimirGestor(_orgaoId);
            
            return retorno;
        }
        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        public IList<UGEEntity> PopularListaUge()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUGE();

            return retorno;
        }

        public IList<UOEntity> PopularListaUo()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUO();
            
            return retorno;
        }

        public GestorEntity SelecionarRegistro(int GestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            GestorEntity retorno = estrutura.SelecionarGestor(GestorId);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, 
            int _orgaoId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.BloqueiaNomeReduzido = false;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaCodigoGestao = false;
            this.View.BloqueiaFileUploadGestor = false;
            this.View.BloqueiaTipoGestor = false;
            this.View.PopularListaOrgao();
            this.View.PopularGrid();
            this.View.PopularListaUo();
            this.View.PopularListaUge();
            this.View.BloqueiaListaUo = false;
            this.View.BloqueiaListaUge = false;
            this.View.MostrarPainelEdicao = false;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Gestor.Id = TratamentoDados.TryParseInt32(this.View.Id);
            //estrutura.Gestor.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Gestor.Nome = this.View.Nome;
            estrutura.Gestor.NomeReduzido = this.View.NomeReduzido;
            estrutura.Gestor.EnderecoLogradouro = this.View.EnderecoLogradouro;
            estrutura.Gestor.EnderecoNumero = this.View.EnderecoNumero;
            estrutura.Gestor.EnderecoCompl = this.View.EnderecoComplemento;
            estrutura.Gestor.EnderecoTelefone = TratamentoDados.RetirarMascara(this.View.EnderecoTelefone);
            estrutura.Gestor.Orgao = new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId));
            estrutura.Gestor.CodigoGestao = TratamentoDados.TryParseInt32(this.View.CodigoGestao);
            estrutura.Gestor.Logotipo = this.View.Logotipo.Length == 0 ? null : this.View.Logotipo;
            estrutura.Gestor.Uge = new UGEEntity();
            estrutura.Gestor.Uo = new UOEntity();
                                             
            if (this.View.TipoId.Equals("1"))
            {
                estrutura.Gestor.Uo = new UOEntity(TratamentoDados.TryParseInt32(this.View.UoId));
                estrutura.Gestor.Uge = new UGEEntity();
            }
            else if (this.View.TipoId.Equals("2"))
            {
                estrutura.Gestor.Uo = new UOEntity();
                estrutura.Gestor.Uge = new UGEEntity(TratamentoDados.TryParseInt32(this.View.UgeId));
            }
            
            if (estrutura.SalvarGestor())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
            }
            else
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
            
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Gestor.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirGestor())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com Sucesso.");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens.");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = 4;
            //RelatorioEntity.Nome = "rptGestor.rdlc";
            //RelatorioEntity.DataSet = "dsGestor";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Gestor;
            relatorioImpressao.Nome = "rptGestor.rdlc";
            relatorioImpressao.DataSet = "dsGestor";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaNomeReduzido = true;
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaCodigoGestao = true;
            this.View.BloqueiaListaUo = true;
            this.View.BloqueiaFileUploadGestor = true;
            this.View.BloqueiaTipoGestor = true;
            
            if (this.View.TipoId.Equals("0"))
            {
                this.View.BloqueiaListaUo = false;
                this.View.BloqueiaListaUge = false;
            }
            else if (this.View.TipoId.Equals("1"))
            {
                this.View.BloqueiaListaUo = true;
                this.View.BloqueiaListaUge = false;
            }
            else if (this.View.TipoId.Equals("2"))
            {
                this.View.BloqueiaListaUo = false;
                this.View.BloqueiaListaUge = true;
            }
            base.RegistroSelecionado();
        }

        public override void GravadoSucesso()
        {
            this.View.NomeReduzido = string.Empty;
            this.View.BloqueiaNomeReduzido = false;
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.CodigoGestao = string.Empty;
            this.View.BloqueiaCodigoGestao = false;
            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaListaUo = false;
            this.View.BloqueiaFileUploadGestor = false;
            this.View.BloqueiaTipoGestor = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.NomeReduzido = string.Empty;
            this.View.BloqueiaNomeReduzido = false;
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.CodigoGestao = string.Empty;
            this.View.BloqueiaCodigoGestao = false;
            this.View.BloqueiaListaUo = false;
            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaFileUploadGestor = false;
            this.View.BloqueiaTipoGestor = false;
            base.ExcluidoSucesso();
        }

        public override void Novo()
        {
            this.View.TipoId = "0";
            this.View.NomeReduzido = string.Empty;
            this.View.BloqueiaNomeReduzido = true;
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.CodigoGestao = string.Empty;
            this.View.BloqueiaCodigoGestao = true;
            this.View.BloqueiaListaUo = false;
            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaFileUploadGestor = true;
            this.View.BloqueiaTipoGestor = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.NomeReduzido = string.Empty;
            this.View.BloqueiaNomeReduzido = false;
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.CodigoGestao = string.Empty;
            this.View.BloqueiaCodigoGestao = false;
            this.View.BloqueiaListaUo = false;
            this.View.BloqueiaListaUge = false;
            this.View.LogotipoImgUrl = string.Empty;
            this.View.BloqueiaFileUploadGestor = false;
            this.View.BloqueiaTipoGestor = false;
            base.Cancelar();
        }
    }
}
