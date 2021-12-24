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
using Sam.Entity;
using Sam.Business;
using Sam.Common;

namespace Sam.Presenter
{
    public class DivisaoPresenter : CrudPresenter<IDivisaoView> 
    {
        IDivisaoView view;

        public IDivisaoView View
        {
            get { return view; }
            set { view = value; }
        }

        public int divisaoId { get; set; }

        public DivisaoPresenter()
        {
        }

        public DivisaoPresenter(IDivisaoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<DivisaoEntity> PopularDadosDivisao(int startRowIndexParameterName, int maximumRowsParameterName,
            int _orgaoId, int _uaId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<DivisaoEntity> retorno = estrutura.ListarDivisao(_orgaoId, _uaId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        public IList<DivisaoEntity> PopularDadosRelatorio(int _orgaoId, int _uaId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ImprimirDivisao(_orgaoId, _uaId);
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        public IList<UAEntity> PopularListaUA(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UAEntity> retorno = estrutura.ListarUAPorOrgao(_orgaoId);
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public IList<AlmoxarifadoEntity> PopularListaAlmoxarifado(int OrgaoId, int GestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            // para os perfils Adm. Geral  e  Adm. de Orgão, pegar as informações selecionadas pelo usuário na tela.

            //if (this.Perfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL || this.Perfil == (int)Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
            //{
                IList<AlmoxarifadoEntity> lista = estrutura.ListarComboAlmoxarifado(OrgaoId, GestorId);
                //lista = base.FiltrarNivelAcesso(lista);
                return lista;
            //}
            //else //Pega os almoxarifados cadastrados no nivel de acesso do perfil do usuário
            //{

            //    int idLogin = Acesso.Transacoes.Perfis[0].IdLogin;
            //    var business = new PerfilLoginNivelAcessoBusiness();
            //    return business.ListarPerfilLoginNivelAcesso(idLogin, GestorId);
            //}                     
            
        }

        public IList<AlmoxarifadoEntity> PopularListaAlmoxarifado(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifado(_orgaoId);
            return retorno;
        }

        public IList<DivisaoEntity> ListarPorAlmoxTodosCod(int almoxarifadoId)
        {
            almoxarifadoId =  Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ListarDivisaoPorAlmoxTodosCod(almoxarifadoId);
            return retorno.OrderBy(x => x.Descricao).ToList();
        }

        public IList<ResponsavelEntity> PopularListaResponsavel()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavel();
            return retorno;
        }

        public IList<UFEntity> PopularListaUf()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UFEntity> retorno = estrutura.ListarUF();
            return retorno;
        }

        public IList<IndicadorAtividadeEntity> PopularListaIndicadorAtividade()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<IndicadorAtividadeEntity> retorno = estrutura.ListarIndicadorAtividade();
            return retorno;
        }

        public IList<DivisaoEntity> PopularListaDivisao(int _almoxId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ListarDivisaoPorAlmoxTodosCod(_almoxId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno.OrderBy(x => x.Descricao).ToList();
        }

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ListarDivisaoByUA(uaId, gestorId);
            return retorno;
        }

        public IList<DivisaoEntity> ImprimirDivisaoByGestor(int gestorId,int? UOId, int? UGEId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarDivisaoByGestor(gestorId, UOId, UGEId);
        }

        public IList<DivisaoEntity> PopularDadosDivisao(int startRowIndexParameterName, int maximumRowsParameterName, int UoId, Int64 UgeId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<DivisaoEntity> retorno = estrutura.ListarDivisao(UoId, UgeId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<DivisaoEntity> ListarDivisaoByGestor(int gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Divisao = new DivisaoEntity();
            estrutura.Divisao.Ua = new UAEntity();
            estrutura.Divisao.Ua.Uge = new UGEEntity();
            estrutura.Divisao.Ua.Uge.Uo = new UOEntity();

            if (this.View.UGEId != null && this.View.UGEId != "")
                estrutura.Divisao.Ua.Uge.Id = TratamentoDados.TryParseInt32(this.View.UGEId);
            if (this.View.UOId != null && this.View.UOId != "")
                estrutura.Divisao.Ua.Uge.Uo.Id = TratamentoDados.TryParseInt32(this.View.UOId);

            //Refazer
            return estrutura.ListarDivisaoTodosCod().Where(a => a.Ua.Gestor.Id == gestorId).ToList();
        }

        public IList<DivisaoEntity> ListarDivisaoById(int divisaoId)
        {   
            return new DivisaoBusiness().SelectById(divisaoId);
        }

        
        public IList<DivisaoEntity> ListarDivisaoByUALogin(int uaId, int gestorId)
        {
            int loginId = Acesso.Transacoes.Perfis[0].IdLogin;

            PerfilLoginNivelAcessoBusiness business = new PerfilLoginNivelAcessoBusiness();
            return business.ListarDivisaoByUALogin(uaId, gestorId, loginId);
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _uaId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaArea = false;
            this.View.BloqueiaNumeroFuncionarios = false;
            this.View.BloqueiaListaAlmoxarifado = false;
            //this.View.BloqueiaListaUF = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            this.View.BloqueiaListaResponsavel = false;
            this.View.PopularListaOrgao();
            this.View.PopularListaUA(int.MinValue);
            this.View.PopularListaUF();
        }

        public void LoadConsulta()
        {
            base.Load();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Divisao.Orgao = new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId));
            estrutura.Divisao.Ua = new UAEntity(TratamentoDados.TryParseInt32(this.View.UAId));
            estrutura.Divisao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.Divisao.Responsavel =
                new ResponsavelEntity(TratamentoDados.TryParseInt32(this.View.ResponsavelId));
            estrutura.Divisao.Almoxarifado = new AlmoxarifadoEntity(TratamentoDados.TryParseInt32(this.View.AlmoxarifadoId));
            estrutura.Divisao.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Divisao.Descricao = this.View.Descricao;
            estrutura.Divisao.EnderecoLogradouro = this.View.EnderecoLogradouro;
            estrutura.Divisao.EnderecoNumero = this.View.EnderecoNumero;
            estrutura.Divisao.EnderecoCompl = this.View.EnderecoComplemento;
            estrutura.Divisao.EnderecoBairro = this.View.EnderecoBairro;
            estrutura.Divisao.EnderecoMunicipio = this.View.EnderecoMunicipio;
            estrutura.Divisao.Uf = new UFEntity(TratamentoDados.TryParseInt32(this.View.UfId));
            estrutura.Divisao.EnderecoCep = TratamentoDados.RetirarMascara(this.View.EnderecoCep);
            estrutura.Divisao.EnderecoTelefone = TratamentoDados.RetirarMascara(this.View.EnderecoTelefone);
            estrutura.Divisao.EnderecoFax = TratamentoDados.RetirarMascara(this.View.EnderecoFax);
            estrutura.Divisao.Area = this.View.Area != "" ? (int)TratamentoDados.TryParseInt32(this.View.Area) : 0;
            estrutura.Divisao.NumeroFuncionarios = TratamentoDados.TryParseInt32(this.View.NumeroFuncionarios);
            estrutura.Divisao.IndicadorAtividade = Convert.ToBoolean(this.View.IndicadorAtividadeId);
            
            if (estrutura.SalvarDivisao())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                this.View.BloqueiaListaUA = true;
            }
            else
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");

            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Divisao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            EstruturaBusiness estruturaPerfil = new EstruturaBusiness();



            if (estruturaPerfil.ConsultaEstruturaNivel(Convert.ToInt32(estrutura.Divisao.Id), (int)Enuns.NivelAcessoEnum.DIVISAO) == 0)
            {
                if (estrutura.ExcluirDivisao())
                {
                    this.View.PopularGrid();
                    this.ExcluidoSucesso();
                    this.View.ExibirMensagem("Registro excluído com Sucesso.");
                }
                else
                    estrutura.ListaErro.Add("Há registro associado a esta divisão");
            }else
                estrutura.ListaErro.Add("Há nivel associado a esta divisão");

            if (estrutura.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens.");
                this.View.ListaErros = estrutura.ListaErro;
            }
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.Divisao;
            //RelatorioEntity.Nome = "rptDivisao.rdlc";
            //RelatorioEntity.DataSet = "dsDivisao";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Divisao;
            relatorioImpressao.Nome = "rptDivisao.rdlc";
            relatorioImpressao.DataSet = "dsDivisao";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void ImprimirConsulta()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.ConsultaDivisao;
            //RelatorioEntity.Nome = "rptConsultaDivisao.rdlc";
            //RelatorioEntity.DataSet = "dsDivisao";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaDivisao;
            relatorioImpressao.Nome = "rptConsultaDivisao.rdlc";
            relatorioImpressao.DataSet = "dsDivisao";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void RegistroSelecionado()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            //this.View.PopularListaUF();
            estrutura.SelectDivisao(divisaoId);

            this.View.Codigo = String.Format("{0:D3}", estrutura.Divisao.Codigo);
            this.View.Descricao = estrutura.Divisao.Descricao;
            this.View.AlmoxarifadoId = estrutura.Divisao.Almoxarifado.Id.ToString();
            this.View.ResponsavelId = estrutura.Divisao.Responsavel.Id.ToString();

            if(estrutura.Divisao.EnderecoLogradouro != null)
                this.View.EnderecoLogradouro = estrutura.Divisao.EnderecoLogradouro;

            if (estrutura.Divisao.EnderecoNumero != null)
                this.View.EnderecoNumero = estrutura.Divisao.EnderecoNumero;

            if (estrutura.Divisao.EnderecoCompl != null)
                this.View.EnderecoComplemento = estrutura.Divisao.EnderecoCompl;

            if (estrutura.Divisao.EnderecoBairro != null)
                this.View.EnderecoBairro = estrutura.Divisao.EnderecoBairro;

            if (estrutura.Divisao.EnderecoMunicipio != null)
                this.View.EnderecoMunicipio = estrutura.Divisao.EnderecoMunicipio;

            if (estrutura.Divisao.Uf != null)
                this.View.UfId = estrutura.Divisao.Uf.Id.ToString();

            if (estrutura.Divisao.EnderecoCep != null)
                this.View.EnderecoCep = estrutura.Divisao.EnderecoCep;

            if (estrutura.Divisao.EnderecoTelefone != null)
                this.View.EnderecoTelefone = estrutura.Divisao.EnderecoTelefone;

            if (estrutura.Divisao.EnderecoFax != null)
                this.View.EnderecoFax = estrutura.Divisao.EnderecoFax;

            this.View.Area = estrutura.Divisao.Area.ToString();

            if (estrutura.Divisao.NumeroFuncionarios != null)
                this.View.NumeroFuncionarios = estrutura.Divisao.NumeroFuncionarios.ToString();

            this.View.IndicadorAtividadeId = Convert.ToString(estrutura.Divisao.IndicadorAtividade);

            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.BloqueiaEnderecoMunicipio = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoCep = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaEnderecoFax = true;
            this.View.BloqueiaArea = true;
            this.View.BloqueiaNumeroFuncionarios = true;

            this.View.BloqueiaListaUF = true;
            this.View.BloqueiaListaUA = true;
            this.View.BloqueiaListaAlmoxarifado = true;
            this.View.BloqueiaListaResponsavel = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
            base.RegistroSelecionado();

        }

        public override void GravadoSucesso()
        {
            this.View.AlmoxarifadoId = " - Selecione - ";
            this.View.ResponsavelId = " - Selecione - ";
            this.View.UfId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
            this.View.Area = string.Empty;
            this.View.BloqueiaArea = false;
            this.View.NumeroFuncionarios = string.Empty;
            this.View.BloqueiaNumeroFuncionarios = false;


            this.View.BloqueiaListaUA = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaListaAlmoxarifado = false;
            this.view.BloqueiaListaResponsavel = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.AlmoxarifadoId = " - Selecione - ";
            this.View.ResponsavelId = " - Selecione - ";
            this.View.UfId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
            this.View.Area = string.Empty;
            this.View.BloqueiaArea = false;
            this.View.NumeroFuncionarios = string.Empty;
            this.View.BloqueiaNumeroFuncionarios = false;

            this.View.BloqueiaListaUA = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaListaAlmoxarifado = false;
            this.view.BloqueiaListaResponsavel = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            base.ExcluidoSucesso();
        }
        
        public override void Novo()
        {
            this.View.AlmoxarifadoId = " - Selecione - ";
            this.View.ResponsavelId = " - Selecione - ";
            this.View.UfId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = true;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = true;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = true;
            this.View.Area = string.Empty;
            this.View.BloqueiaArea = true;
            this.View.NumeroFuncionarios = string.Empty;
            this.View.BloqueiaNumeroFuncionarios = true;
            
            this.View.BloqueiaListaUA = true;
            this.View.BloqueiaListaUF = true;
            this.View.BloqueiaListaAlmoxarifado = true;
            this.view.BloqueiaListaResponsavel = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.Area = string.Empty;
            this.View.NumeroFuncionarios = string.Empty;

            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaArea = false;
            this.View.BloqueiaNumeroFuncionarios = false;

            //this.View.AlmoxarifadoId = " - Selecione - ";
            //this.View.ResponsavelId = " - Selecione - ";
            //this.View.UfId = " - Selecione - ";
            //this.View.IndicadorAtividadeId = " - Selecione - ";

            this.View.BloqueiaListaResponsavel = false;
            this.view.BloqueiaListaAlmoxarifado = false;
            this.view.BloqueiaListaUF = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            base.Cancelar();
        }

        public DivisaoEntity ObterDivisao(int divisaoID)
        {
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();
            objBusiness.SelectDivisao(divisaoID);

            DivisaoEntity objEntidade = objBusiness.Divisao;

            return objEntidade;
        }
    }
}
