using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.Web.UI.WebControls;

namespace Sam.Presenter
{
    public class CombosPadroesPresenter : CrudPresenter<ICombosPadroesView>
    {
        #region Constructors

        public CombosPadroesPresenter() { }

        public CombosPadroesPresenter(ICombosPadroesView _view)
            : base(_view)
        {
            this.View = _view;
        }

        #endregion


        #region Properties

        int PerfilId;
        int LoginId;

        ICombosPadroesView _view;
        public ICombosPadroesView View
        {
            get { return _view; }
            set { _view = value; }
        }

        public bool ExecutadaPorChamadoSuporte { get; set; }

        #endregion


        #region Methods

        #region Métodos novos da tela de pesquisa de requisições

        public IList<OrgaoEntity> PopularOrgao()
        {
            IList<OrgaoEntity> retorno = new List<OrgaoEntity>() { new OrgaoEntity(0) };
            retorno = new Sam.Presenter.OrgaoPresenter().PopularOrgao();

            return retorno;
        }

        public IList<UOEntity> PopularUO(int orgaoSelecionado)
        {
            ObterAcessosPerfilLogado();

            //Seta um item para não quebrar o ddl.DataSource que é populado com o método.
            IList<UOEntity> retorno = new List<UOEntity>() { new UOEntity(0) };
            IList<UOEntity> listaUOs = new EstruturaOrganizacionalBusiness().ListarUOs(orgaoSelecionado);

            if (listaUOs.Count < 1)
                return retorno;

            switch (PerfilId)
            {
                case (int)GeralEnum.TipoPerfil.AdministradorGeral:
                case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                case (int)GeralEnum.TipoPerfil.AdministradorFinanceiroSEFAZ:
                    retorno = listaUOs;
                    break;
                case (int)GeralEnum.TipoPerfil.AdministradorGestor:
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    retorno = ObterUOPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);
                    break;
                case (int)GeralEnum.TipoPerfil.Requisitante:
                case (int)GeralEnum.TipoPerfil.RequisitanteGeral:
                    //Métodos facades estão retornando por nivel acesso, ou seja, somente a UGE que esta em sua visão(que fora atribuida a ele).
                    IList<UOEntity> uosByPerfil = new Sam.Facade.FacadePerfil().ListarUOByPerfil(LoginId);
                    List<UOEntity> uosOrgaoSelecionadoByPerfil = new List<UOEntity>();

                    //Filtra as UOs do orgaoId selecionado, pelo acesso do usuario logado.                                
                    foreach (var uoNivelAcesso in uosByPerfil)
                    {
                        if (uoNivelAcesso.Id.HasValue)
                        {
                            var uo = listaUOs.Where(a => a.Id == uoNivelAcesso.Id).FirstOrDefault();
                            if (uo != null)
                                uosOrgaoSelecionadoByPerfil.Add(uo);
                        }
                    }
                    retorno = OrdenaListaUOPadrao(Acesso.Transacoes.Perfis[0].UOPadrao, uosOrgaoSelecionadoByPerfil);
                    break;
                default:
                    break;
            }
            return retorno;
        }

        public IList<UGEEntity> PopularUGE(int uoSelecionado)
        {
            ObterAcessosPerfilLogado();

            IList<UGEEntity> ugesByUOId = new EstruturaOrganizacionalBusiness().ListarUgesTodosCodPorUo(uoSelecionado);
            IList<UGEEntity> retorno = new List<UGEEntity>() { new UGEEntity(0) };

            if (ugesByUOId.Count < 1) return retorno;

            switch (PerfilId)
            {
                case (int)GeralEnum.TipoPerfil.AdministradorGeral:
                case (int)GeralEnum.TipoPerfil.RequisitanteGeral:
                case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                case (int)GeralEnum.TipoPerfil.AdministradorFinanceiroSEFAZ:
                    retorno = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesByUOId);
                    break;
                case (int)GeralEnum.TipoPerfil.AdministradorGestor:
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    retorno = ObterUGEPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);
                    break;
                case (int)GeralEnum.TipoPerfil.Requisitante:
                    //Requisitante
                    IList<UGEEntity> ugesByPerfil = new Sam.Facade.FacadePerfil().ListarUGEByPerfil(LoginId).Distinct().ToList();
                    List<UGEEntity> ugesUOSelecionadoByPerfil = new List<UGEEntity>();

                    //join com a lista da UO logada.
                    foreach (var ugeNivelAcesso in ugesByPerfil)
                    {
                        if (ugeNivelAcesso.Id.HasValue)
                        {
                            var uge = ugesByUOId.Where(x => x.Id == ugeNivelAcesso.Id).FirstOrDefault();
                            if (uge != null)
                                ugesUOSelecionadoByPerfil.Add(uge);
                        }
                    }

                    retorno = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesUOSelecionadoByPerfil);
                    break;
                default:
                    break;
            }

            return retorno;
        }

        public IList<UGEEntity> PopularUGETodos(int OrgaoPadrao)
        {
            ObterAcessosPerfilLogado();

            IList<UGEEntity> ugesPorOrgao = new EstruturaOrganizacionalBusiness().ListarUgesTodosCod(OrgaoPadrao);
            IList<UGEEntity> retorno = new List<UGEEntity>() { new UGEEntity(0) };

            if (ugesPorOrgao.Count < 1) return retorno;

            switch (PerfilId)
            {
                case (int)GeralEnum.TipoPerfil.AdministradorGeral:
                case (int)GeralEnum.TipoPerfil.RequisitanteGeral:
                case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                case (int)GeralEnum.TipoPerfil.AdministradorFinanceiroSEFAZ:
                    retorno = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesPorOrgao);
                    break;
                case (int)GeralEnum.TipoPerfil.AdministradorGestor:
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    retorno = ObterUGEPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);
                    break;
                case (int)GeralEnum.TipoPerfil.Requisitante:
                    //Requisitante
                    IList<UGEEntity> ugesByPerfil = new Sam.Facade.FacadePerfil().ListarUGEByPerfil(LoginId).Distinct().ToList();
                    List<UGEEntity> ugesUOSelecionadoByPerfil = new List<UGEEntity>();

                    //join com a lista da UO logada.
                    foreach (var ugeNivelAcesso in ugesByPerfil)
                    {
                        if (ugeNivelAcesso.Id.HasValue)
                        {
                            var uge = ugesPorOrgao.Where(x => x.Id == ugeNivelAcesso.Id).FirstOrDefault();
                            if (uge != null)
                                ugesUOSelecionadoByPerfil.Add(uge);
                        }
                    }

                    retorno = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesUOSelecionadoByPerfil);
                    break;
                default:
                    break;
            }

            return retorno;
        }

        public IList<UAEntity> PopularUA(int ugeSelecionado)
        {
            ObterAcessosPerfilLogado();

            IList<UAEntity> uasByUGEId = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorUge(ugeSelecionado);
            IList<UAEntity> result = new List<UAEntity> { new UAEntity(0) };

            if (uasByUGEId.Count < 1) return result;

            if (PerfilId == Sam.Common.Perfil.ADMINISTRADOR_ORGAO || PerfilId == Sam.Common.Perfil.ADMINISTRADOR_GERAL
                || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                return uasByUGEId;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                AlmoxarifadoEntity almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                result = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorAlmoxarifado(almoxarifado, !ExecutadaPorChamadoSuporte);

                if (ugeSelecionado > 0 && result != null && result.Count > 0)
                    result = result.Where(a => a.Uge.Id == ugeSelecionado).ToList<UAEntity>();
            }

            else if (PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL || PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
                result = OrdenaListaUAPadrao(Acesso.Transacoes.Perfis[0].UAPadrao, uasByUGEId);

            else
            {
                //Requisitante
                IList<UAEntity> uasByPerfil = new Sam.Facade.FacadePerfil().ListarUAByPerfil(LoginId).Distinct().ToList();
                List<UAEntity> uasUGESelecionadoByPerfil = new List<UAEntity>();

                foreach (var uaNivelAcesso in uasByPerfil)
                {
                    if (uaNivelAcesso.Id.HasValue)
                    {
                        var ua = uasByUGEId.Where(x => x.Id == uaNivelAcesso.Id).FirstOrDefault();
                        if (ua != null)
                            uasUGESelecionadoByPerfil.Add(ua);
                    }
                }

                result = OrdenaListaUAPadrao(Acesso.Transacoes.Perfis[0].UAPadrao, uasUGESelecionadoByPerfil);
            }

            return result;
        }

        public IList<DivisaoEntity> PopularDivisao(int uaSelecionada)
        {
            ObterAcessosPerfilLogado();
            int gestorId = this.Acesso.Transacoes.Perfis[0].GestorPadrao.Id.HasValue ? Convert.ToInt32(this.Acesso.Transacoes.Perfis[0].GestorPadrao.Id) : 0;

            IList<DivisaoEntity> result = new List<DivisaoEntity>() { new DivisaoEntity(0) };
            IList<DivisaoEntity> divisaoByUAId = new EstruturaOrganizacionalBusiness().ListarDivisaoByUA(uaSelecionada, gestorId);

            if (divisaoByUAId.Count < 1)
                return result;

            if (PerfilId == Sam.Common.Perfil.ADMINISTRADOR_ORGAO || PerfilId == Sam.Common.Perfil.ADMINISTRADOR_GERAL
                || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                return divisaoByUAId;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                //Carrega a lista de divisões do gestor
                divisaoByUAId = new EstruturaOrganizacionalBusiness().ListarDivisaoByUA(uaSelecionada, gestorId, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);
                result = OrdenaListaDivisaoPadrao(Acesso.Transacoes.Perfis[0].DivisaoPadrao, divisaoByUAId);
            }

            else if (PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                result = OrdenaListaDivisaoPadrao(Acesso.Transacoes.Perfis[0].DivisaoPadrao, divisaoByUAId);

            else
            {
                //Retorna todas as divições do nivel de acesso  - REQUISITANTE            
                var divisoesByNivelAcesso = this.Acesso.Transacoes.Perfis[0].PerfilLoginNivelAcesso.Where(a => a.NivelAcesso.Id == (int)Sam.Common.NivelAcessoEnum.DIVISAO).ToList();
                IList<DivisaoEntity> divisoesUASelecionadaByPerfil = new List<DivisaoEntity>();

                foreach (var divisaoNivelAcesso in divisoesByNivelAcesso)
                {
                    if (divisaoNivelAcesso.Valor != null)
                    {
                        var divisao = divisaoByUAId.Where(a => a.Id == (int)divisaoNivelAcesso.Valor).FirstOrDefault();
                        if (divisao != null)
                            divisoesUASelecionadaByPerfil.Add(divisao);
                    }
                }

                //Requisitantes                    
                result = OrdenaListaDivisaoPadrao(Acesso.Transacoes.Perfis[0].DivisaoPadrao, divisoesUASelecionadaByPerfil);
            }

            return result;
        }

        #endregion


        #region Métodos para orquestrar o preenchimento dos Combos(Orgão, UO, UGE, UA e Divisão)

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            ObterAcessosPerfilLogado();
            IList<OrgaoEntity> result = new List<OrgaoEntity>() { new OrgaoEntity(0) };

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL && PerfilId != (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                result = OrdenaListaOrgaoPadrao(Acesso.Transacoes.Perfis[0].OrgaoPadrao, ObterOrgaosPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado));

            else if (PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                result = OrdenaListaOrgaoPadrao(Acesso.Transacoes.Perfis[0].OrgaoPadrao, new Sam.Presenter.OrgaoPresenter().PopularDadosTodosCod());

            else
                //Requisitantes                
                result = OrdenaListaOrgaoPadrao(Acesso.Transacoes.Perfis[0].OrgaoPadrao, new Sam.Facade.FacadePerfil().ListarOrgaoByPerfil(LoginId));

            return result;
        }

        //Obsoleto para a tela Requisição
        public IList<UOEntity> PopularComboUO()
        {
            ObterAcessosPerfilLogado();
            int orgaoSelecionado = this._view.OrgaoId.IsNotNull() ? this._view.OrgaoId : Convert.ToInt32(this.Acesso.Transacoes.Perfis[0].OrgaoPadrao.Id);

            //Seta um item para não quebrar o ddl.DataSource que é populado com o método.
            IList<UOEntity> result = new List<UOEntity>();
            IList<UOEntity> uosByOrgaoId = new EstruturaOrganizacionalBusiness().ListarUosTodosCod(orgaoSelecionado);

            if (uosByOrgaoId.Count < 1) return result;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != Sam.Common.Perfil.ADMINISTRADOR_GERAL && PerfilId != Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
                result = ObterUOPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);

            else if (PerfilId == Sam.Common.Perfil.ADMINISTRADOR_GERAL || PerfilId == Sam.Common.Perfil.ADMINISTRADOR_ORGAO
                || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                result = OrdenaListaUOPadrao(Acesso.Transacoes.Perfis[0].UOPadrao, uosByOrgaoId);

            else
            {
                //Métodos facades estão retornando por nivel acesso, ou seja, somente a UGE que esta em sua visão(que fora atribuida a ele).
                IList<UOEntity> uosByPerfil = new Sam.Facade.FacadePerfil().ListarUOByPerfil(LoginId);
                List<UOEntity> uosOrgaoSelecionadoByPerfil = new List<UOEntity>();

                //Filtra as UOs do orgaoId selecionado, pelo acesso do usuario logado.                                
                foreach (var uoNivelAcesso in uosByPerfil)
                {
                    if (uoNivelAcesso.Id.HasValue)
                    {
                        var uo = uosByOrgaoId.Where(a => a.Id == uoNivelAcesso.Id).FirstOrDefault();
                        if (uo != null)
                            uosOrgaoSelecionadoByPerfil.Add(uo);
                    }
                }

                result = OrdenaListaUOPadrao(Acesso.Transacoes.Perfis[0].UOPadrao, uosOrgaoSelecionadoByPerfil);
            }

            return result;
        }

        public IList<UGEEntity> PopularComboUGE()
        {
            ObterAcessosPerfilLogado();
            int uoSelecionado = this._view.UOId;
            IList<UGEEntity> ugesByUOId = new EstruturaOrganizacionalBusiness().ListarUgesTodosCodPorUo(uoSelecionado);
            IList<UGEEntity> result = new List<UGEEntity>() { new UGEEntity(0) };

            if (ugesByUOId.Count < 1) return result;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != Sam.Common.Perfil.ADMINISTRADOR_GERAL)
                result = ObterUGEPorAlmoxarifadoLogado(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado);

            else if (PerfilId == Sam.Common.Perfil.REQUISITANTE_GERAL || PerfilId == Sam.Common.Perfil.ADMINISTRADOR_GERAL
                || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                //lista UGEs por UO selecionada, sem checagem de permissão na TB_LOGIN_NIVEL_ACESSO                
                result = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesByUOId);

            else
            {
                //Requisitante
                IList<UGEEntity> ugesByPerfil = new Sam.Facade.FacadePerfil().ListarUGEByPerfil(LoginId).Distinct().ToList();
                List<UGEEntity> ugesUOSelecionadoByPerfil = new List<UGEEntity>();

                //join com a lista da UO logada.
                foreach (var ugeNivelAcesso in ugesByPerfil)
                {
                    if (ugeNivelAcesso.Id.HasValue)
                    {
                        var uge = ugesByUOId.Where(x => x.Id == ugeNivelAcesso.Id).FirstOrDefault();
                        if (uge != null)
                            ugesUOSelecionadoByPerfil.Add(uge);
                    }
                }

                result = OrdenaListaUGEPadrao(Acesso.Transacoes.Perfis[0].UGEPadrao, ugesUOSelecionadoByPerfil);
            }

            return result;
        }

        public IList<UAEntity> PopularComboUA()
        {
            ObterAcessosPerfilLogado();
            int ugeSelecionado = this._view.UGEId;

            IList<UAEntity> uasByUGEId = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorUge(ugeSelecionado);
            IList<UAEntity> result = new List<UAEntity> { new UAEntity(0) };

            if (uasByUGEId.Count < 1) return result;

            if (PerfilId == Sam.Common.Perfil.ADMINISTRADOR_ORGAO || PerfilId == Sam.Common.Perfil.ADMINISTRADOR_GERAL
                || PerfilId == (int)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
                return uasByUGEId;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
                    && PerfilId != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                AlmoxarifadoEntity almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                result = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorAlmoxarifado(almoxarifado, !ExecutadaPorChamadoSuporte);

                if (ugeSelecionado > 0 && result != null && result.Count > 0)
                    result = result.Where(a => a.Uge.Id == ugeSelecionado).ToList<UAEntity>();
            }

            else if (PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL || PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
                result = OrdenaListaUAPadrao(Acesso.Transacoes.Perfis[0].UAPadrao, uasByUGEId);

            else
            {
                //Requisitante
                IList<UAEntity> uasByPerfil = new Sam.Facade.FacadePerfil().ListarUAByPerfil(LoginId).Distinct().ToList();
                List<UAEntity> uasUGESelecionadoByPerfil = new List<UAEntity>();

                foreach (var uaNivelAcesso in uasByPerfil)
                {
                    if (uaNivelAcesso.Id.HasValue)
                    {
                        var ua = uasByUGEId.Where(x => x.Id == uaNivelAcesso.Id).FirstOrDefault();
                        if (ua != null)
                            uasUGESelecionadoByPerfil.Add(ua);
                    }
                }

                result = OrdenaListaUAPadrao(Acesso.Transacoes.Perfis[0].UAPadrao, uasUGESelecionadoByPerfil);
            }

            return result;
        }

        public IList<AlmoxarifadoEntity> PopularComboAlmoxarifado()
        {
            ObterAcessosPerfilLogado();
            int ugeSelecionado = this._view.UGEId;

            IList<AlmoxarifadoEntity> almoxsPorUGE = new EstruturaOrganizacionalBusiness().ListarAlmoxarifadoPorUGE(ugeSelecionado);
            IList<AlmoxarifadoEntity> result = new List<AlmoxarifadoEntity> { new AlmoxarifadoEntity(0) };

            if (almoxsPorUGE.Count < 1) 
                almoxsPorUGE = result;


            return almoxsPorUGE;
        }

        public IList<DivisaoEntity> PopularComboDivisao()
        {
            ObterAcessosPerfilLogado();
            int uaSelecionada = this._view.UAId;
            int gestorId = this.Acesso.Transacoes.Perfis[0].GestorPadrao.Id.HasValue ? Convert.ToInt32(this.Acesso.Transacoes.Perfis[0].GestorPadrao.Id) : 0;

            IList<DivisaoEntity> result = new List<DivisaoEntity>() { new DivisaoEntity(0) };
            IList<DivisaoEntity> divisaoByUAId;

            if (this._view.OrgaoId == 0)
                divisaoByUAId = new EstruturaOrganizacionalBusiness().ListarDivisaoByUA(uaSelecionada, gestorId);
            else
                divisaoByUAId = new EstruturaOrganizacionalBusiness().ListarDivisao(this._view.OrgaoId, uaSelecionada);

            if (divisaoByUAId.Count < 1)
                return result;

            if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE)
                //Carrega a lista de divisões do gestor                
                result = OrdenaListaDivisaoPadrao(Acesso.Transacoes.Perfis[0].DivisaoPadrao, divisaoByUAId);

            else
            {
                //Retorna todas as divições do nivel de acesso  - REQUISITANTE            
                var divisoesByNivelAcesso = this.Acesso.Transacoes.Perfis[0].PerfilLoginNivelAcesso.Where(a => a.NivelAcesso.Id == (int)Sam.Common.NivelAcessoEnum.DIVISAO).ToList();
                IList<DivisaoEntity> divisoesUASelecionadaByPerfil = new List<DivisaoEntity>();

                foreach (var divisaoNivelAcesso in divisoesByNivelAcesso)
                {
                    if (divisaoNivelAcesso.Valor != null)
                    {
                        var divisao = divisaoByUAId.Where(a => a.Id == (int)divisaoNivelAcesso.Valor).FirstOrDefault();
                        if (divisao != null)
                            divisoesUASelecionadaByPerfil.Add(divisao);
                    }
                }

                //Requisitantes                    
                result = OrdenaListaDivisaoPadrao(Acesso.Transacoes.Perfis[0].DivisaoPadrao, divisoesUASelecionadaByPerfil);
            }

            return result;
        }

        #endregion

        public IList<TipoMovimentoEntity> PopularComboStatusRequisicao()
        {
            IList<TipoMovimentoEntity> result = new List<TipoMovimentoEntity>();
            result = new TipoMovimentoPresenter().PopularListaTipoRequisicao();

            return result;
        }

        private void ObterAcessosPerfilLogado()
        {
            PerfilId = Acesso.Transacoes.Perfis[0].IdPerfil;
            LoginId = Acesso.Transacoes.Perfis[0].IdLogin;
        }

        #region ObterDados Almoxarifado LOGADO - Seta os combos Orgão, UO e UGE com o valor default.

        private IList<UAEntity> ListarUAsPorAlmoxarifadoLogado(AlmoxarifadoEntity almoxarifadoLogado)
        {
            IList<UAEntity> _result = null;

            if (almoxarifadoLogado != null)
                _result = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorAlmoxarifado(almoxarifadoLogado, !ExecutadaPorChamadoSuporte).ToList();

            if (_result.Count == 0)
                _result = new EstruturaOrganizacionalBusiness().ListarUasTodosCodPorAlmoxarifado(almoxarifadoLogado).ToList();

            return _result ?? new List<UAEntity>();
        }

        private IList<UGEEntity> ObterUGEPorAlmoxarifadoLogado(AlmoxarifadoEntity almoxarifadoLogado)
        {
            IList<UGEEntity> listaUGEs;
            IList<UAEntity> listaUAs = ListarUAsPorAlmoxarifadoLogado(almoxarifadoLogado);

            if (listaUAs.Count < 1)
                listaUGEs = new List<UGEEntity>() { new UGEEntity(0) };
            else
            {
                List<UGEEntity> tempList = new List<UGEEntity>();
                foreach (var item in listaUAs)
                {
                    if (item.Uge.IsNotNull() && !tempList.Exists(x => x.Id == item.Uge.Id))
                    {
                        tempList.Add(
                                    new UGEEntity()
                                    {
                                        Id = item.Uge.Id,
                                        Codigo = item.Uge.Codigo,
                                        Descricao = item.Uge.Descricao
                                    });
                    }
                }
                listaUGEs = tempList;
            }
            return listaUGEs;
        }

        private IList<UOEntity> ObterUOPorAlmoxarifadoLogado(AlmoxarifadoEntity almoxarifadoLogado)
        {
            IList<UOEntity> listaUOs;
            IList<UAEntity> listaUAs = ListarUAsPorAlmoxarifadoLogado(almoxarifadoLogado);

            if (listaUAs.Count < 1)
                listaUOs = new List<UOEntity>() { new UOEntity(0) };
            else
            {
                List<UOEntity> tempList = new List<UOEntity>();
                foreach (var item in listaUAs)
                {
                    if (item.Uo.IsNotNull() && !tempList.Exists(x => x.Id == item.Uo.Id))
                    {
                        tempList.Add(
                                    new UOEntity()
                                    {
                                        Id = item.Uo.Id,
                                        Codigo = item.Uo.Codigo,
                                        Descricao = item.Uo.Descricao,
                                        Orgao = item.Uo.Orgao
                                    });
                    }
                }
                listaUOs = tempList;
            }
            return listaUOs;
        }

        private IList<OrgaoEntity> ObterOrgaosPorAlmoxarifadoLogado(AlmoxarifadoEntity almoxarifadoLogado)
        {
            IList<OrgaoEntity> listaOrgaos;
            IList<UAEntity> listaUAs = ListarUAsPorAlmoxarifadoLogado(almoxarifadoLogado);

            if (listaUAs.Count < 1)
                listaOrgaos = new List<OrgaoEntity>() { new OrgaoEntity(0) };
            else
            {
                List<OrgaoEntity> tempList = new List<OrgaoEntity>();
                foreach (var item in listaUAs)
                {
                    if (item.Orgao.IsNotNull() && !tempList.Exists(x => x.Id == item.Orgao.Id))
                    {
                        tempList.Add(
                                    new OrgaoEntity()
                                    {
                                        Id = item.Orgao.Id,
                                        Codigo = item.Orgao.Codigo,
                                        Descricao = item.Orgao.Descricao,
                                        CodigoDescricao = item.Orgao.CodigoDescricao
                                    });
                    }
                }
                listaOrgaos = tempList;
            }

            return listaOrgaos;
        }

        #region Métodos que recebem uma lista e tentam colcoar como TOP 1 a entidade padrão (se existir).

        private IList<OrgaoEntity> OrdenaListaOrgaoPadrao(OrgaoEntity orgaoPadrao, IList<OrgaoEntity> listaOrgaos)
        {
            IList<OrgaoEntity> result = listaOrgaos;

            if (orgaoPadrao.IsNotNull())
            {
                int indexOrgao = listaOrgaos.ToList().FindIndex(x => x.Id == orgaoPadrao.Id);
                if (indexOrgao > 0)
                {
                    OrgaoEntity orgaoPrincipal = listaOrgaos.ToList().FirstOrDefault(x => x.Id == orgaoPadrao.Id);
                    listaOrgaos.RemoveAt(indexOrgao);
                    listaOrgaos.Insert(0, orgaoPrincipal);
                    result = listaOrgaos;
                }
            }

            return result;
        }

        private IList<UOEntity> OrdenaListaUOPadrao(UOEntity uoPadrao, IList<UOEntity> listaUOs)
        {
            IList<UOEntity> result = listaUOs;

            if (uoPadrao.IsNotNull())
            {
                int indexUO = listaUOs.ToList().FindIndex(x => x.Id == uoPadrao.Id);
                if (indexUO > 0)
                {
                    UOEntity uoPrincipal = listaUOs.ToList().FirstOrDefault(x => x.Id == uoPadrao.Id);
                    listaUOs.RemoveAt(indexUO);
                    listaUOs.Insert(0, uoPrincipal);
                    result = listaUOs;
                }
            }

            return result;
        }

        private IList<UGEEntity> OrdenaListaUGEPadrao(UGEEntity ugePadrao, IList<UGEEntity> listaUGEs)
        {
            IList<UGEEntity> result = listaUGEs;

            if (ugePadrao.IsNotNull())
            {
                int indexUGE = listaUGEs.ToList().FindIndex(x => x.Id == ugePadrao.Id);
                if (indexUGE > 0)
                {
                    UGEEntity ugePrincipal = listaUGEs.ToList().FirstOrDefault(x => x.Id == ugePadrao.Id);
                    listaUGEs.RemoveAt(indexUGE);
                    listaUGEs.Insert(0, ugePrincipal);
                    result = listaUGEs;
                }
            }

            return result;
        }

        private IList<UAEntity> OrdenaListaUAPadrao(UAEntity uaPadrao, IList<UAEntity> listaUAs)
        {
            IList<UAEntity> result = listaUAs;

            if (uaPadrao.IsNotNull())
            {
                int indexUA = listaUAs.ToList().FindIndex(x => x.Id == uaPadrao.Id);
                if (indexUA > 0)
                {
                    UAEntity uaPrincipal = listaUAs.ToList().FirstOrDefault(x => x.Id == uaPadrao.Id);
                    listaUAs.RemoveAt(indexUA);
                    listaUAs.Insert(0, uaPrincipal);
                    result = listaUAs;
                }
            }

            return result;
        }

        private IList<DivisaoEntity> OrdenaListaDivisaoPadrao(DivisaoEntity divisaoPadrao, IList<DivisaoEntity> listaDivisoes)
        {
            IList<DivisaoEntity> result = listaDivisoes;

            if (divisaoPadrao.IsNotNull())
            {
                int indexDivisao = listaDivisoes.ToList().FindIndex(x => x.Id == divisaoPadrao.Id);
                if (indexDivisao > 0)
                {
                    DivisaoEntity divisaoPrincipal = listaDivisoes.ToList().FirstOrDefault(x => x.Id == divisaoPadrao.Id);
                    listaDivisoes.RemoveAt(indexDivisao);
                    listaDivisoes.Insert(0, divisaoPrincipal);
                    result = listaDivisoes;
                }
            }

            return result;
        }

        #endregion

        #endregion

        #endregion


        #region Controllers

        public override void Load()
        {
            ObterAcessosPerfilLogado();
            this._view.PrepararVisaoDeCombosPorPerfil(PerfilId);
        }

        #endregion

    }
}
