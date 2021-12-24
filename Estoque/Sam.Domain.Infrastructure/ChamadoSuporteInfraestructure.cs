using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Xml.Linq;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using statusAtendimentoChamado = Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado;
using System.Linq.Expressions;
using Sam.Common.Enums.ChamadoSuporteEnums;
using Sam.Common.Enums;



namespace Sam.Domain.Infrastructure
{
    public class ChamadoSuporteInfraestructure : BaseInfraestructure
    {
        private ChamadoSuporteEntity _chamadoSuporte = new ChamadoSuporteEntity();
        public ChamadoSuporteEntity Entity
        {
            get { return _chamadoSuporte; }
            set { _chamadoSuporte = value; }
        }

        #region Pesquisas Filtradas
        private Expression<Func<TB_CHAMADO_SUPORTE, bool>> processarStatusFiltroPesquisa(statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;

            if ((statusChamadoAtendimentoProdesp != statusAtendimentoChamado.Todos) && (statusChamadoAtendimentoUsuario != statusAtendimentoChamado.Todos))
                expWhereFiltroStatus = (chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_PRODESP == (int)statusChamadoAtendimentoProdesp
                                                       && chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_USUARIO == (int)statusChamadoAtendimentoUsuario);
            else if ((statusChamadoAtendimentoProdesp != statusAtendimentoChamado.Todos) && (statusChamadoAtendimentoUsuario == statusAtendimentoChamado.Todos))
                expWhereFiltroStatus = (chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_PRODESP == (int)statusChamadoAtendimentoProdesp);
            else if ((statusChamadoAtendimentoProdesp == statusAtendimentoChamado.Todos) && (statusChamadoAtendimentoUsuario != statusAtendimentoChamado.Todos))
                expWhereFiltroStatus = (chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_USUARIO == (int)statusChamadoAtendimentoUsuario);


            return expWhereFiltroStatus;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorAlmoxarifado(int almoxID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhere = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (almoxID > 0)
            {
                expWhere = (chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID == almoxID);

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    expWhere = expWhere.And(expWhereFiltroStatus);

                lstChamados = Db.TB_CHAMADO_SUPORTEs.Where(expWhere)
                                                   .Select(_instanciadorDTOChamadoSuporte())
                                                   .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorUsuario(long cpfUsuario, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhere = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (cpfUsuario > 0)
            {
                expWhere = (chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA == cpfUsuario);

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    expWhere = expWhere.And(expWhereFiltroStatus);


                lstChamados = Db.TB_CHAMADO_SUPORTEs.Where(expWhere)
                                                   .Select(_instanciadorDTOChamadoSuporte())
                                                   .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorUGE(int ugeID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhere = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (ugeID > 0)
            {
                qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                               join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                               join uge in Db.TB_UGEs on almox.TB_UGE_ID equals uge.TB_UGE_ID
                               orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                               select chamadoSuporte).AsQueryable();

                expWhere = (chamadoSuporte => chamadoSuporte.TB_ALMOXARIFADO.TB_UGE_ID == ugeID);

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    expWhere = expWhere.And(expWhereFiltroStatus);


                lstChamados = qryConsulta.Where(expWhere)
                                         .Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        //TODO [TELA CHAMADOS SUPORTE]: Finalizar/validar consulta
        private IList<ChamadoSuporteEntity> ObterChamadosPorUA(int uaID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (uaID > 0)
            {
                qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                               join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                               join ua in Db.TB_UAs on almox.TB_ALMOXARIFADO_ID equals ua.TB_UA_ID
                               join divisao in Db.TB_DIVISAOs on ua.TB_UA_ID equals divisao.TB_DIVISAO_ID
                               orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                               where ua.TB_UA_ID == uaID
                               //where _chamado.TB_CHAMADO_SUPORTE_ATIVO == true)
                               //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                               select chamadoSuporte).AsQueryable();

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    qryConsulta = qryConsulta.Where(expWhereFiltroStatus);

                lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        //TODO [TELA CHAMADOS SUPORTE]: Finalizar/validar consulta
        private IList<ChamadoSuporteEntity> ObterChamadosPorDivisao(int divisaoID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhere = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (divisaoID > 0)
            {
                IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                                                              join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                                                              join divisao in Db.TB_DIVISAOs on almox.TB_ALMOXARIFADO_ID equals divisao.TB_ALMOXARIFADO_ID
                                                              orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                                                              where divisao.TB_DIVISAO_ID == divisaoID
                                                              //where _chamado.TB_CHAMADO_SUPORTE_ATIVO == true)
                                                              //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                                                              select chamadoSuporte).AsQueryable();


                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    qryConsulta = qryConsulta.Where(expWhereFiltroStatus);


                lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorUO(int uoID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (uoID > 0)
            {
                IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                                                              join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                                                              join uge in Db.TB_UGEs on almox.TB_UGE_ID equals uge.TB_UGE_ID
                                                              join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                                              orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                                                              where uo.TB_UO_ID == uoID
                                                              //where _chamado.TB_CHAMADO_SUPORTE_ATIVO == true)
                                                              //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                                                              select chamadoSuporte).AsQueryable();

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    qryConsulta = qryConsulta.Where(expWhereFiltroStatus);


                lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorOrgao(int orgaoID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (orgaoID > 0)
            {
                IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                                                              join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                                                              join uge in Db.TB_UGEs on almox.TB_UGE_ID equals uge.TB_UGE_ID
                                                              join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                                              join orgao in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals orgao.TB_ORGAO_ID
                                                              orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                                                              where orgao.TB_ORGAO_ID == orgaoID
                                                              //where _chamado.TB_CHAMADO_SUPORTE_ATIVO == true)
                                                              //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                                                              select chamadoSuporte).AsQueryable();

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    qryConsulta = qryConsulta.Where(expWhereFiltroStatus);


                lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterChamadosPorGestor(int gestorID, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            if (gestorID > 0)
            {
                IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                                                              join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                                                              join gestor in Db.TB_GESTORs on almox.TB_GESTOR_ID equals gestor.TB_GESTOR_ID
                                                              orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                                                              where gestor.TB_GESTOR_ID == gestorID
                                                              //where _chamado.TB_CHAMADO_SUPORTE_ATIVO == true)
                                                              //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                                                              select chamadoSuporte).AsQueryable();

                expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
                if (expWhereFiltroStatus.IsNotNull())
                    qryConsulta = qryConsulta.Where(expWhereFiltroStatus);


                lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                         .ToList();
            }

            return lstChamados;
        }

        private IList<ChamadoSuporteEntity> ObterTodosOsChamados(statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario, bool? chamadosAtivos = true)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;
            IQueryable<TB_CHAMADO_SUPORTE> qryConsulta = null;
            Expression<Func<TB_CHAMADO_SUPORTE, bool>> expWhereFiltroStatus = null;


            qryConsulta = (from chamadoSuporte in Db.TB_CHAMADO_SUPORTEs
                           join almox in Db.TB_ALMOXARIFADOs on chamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                           join gestor in Db.TB_GESTORs on almox.TB_GESTOR_ID equals gestor.TB_GESTOR_ID
                           orderby chamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA descending
                           //where chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO == (int)statusChamadoPesquisa
                           select chamadoSuporte).AsQueryable();

            if (chamadosAtivos.HasValue && chamadosAtivos.Value == true)
                qryConsulta = qryConsulta.Where(chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_USUARIO != (byte)StatusAtendimentoChamado.Finalizado);

            expWhereFiltroStatus = processarStatusFiltroPesquisa(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario);
            if (expWhereFiltroStatus.IsNotNull())
                qryConsulta = qryConsulta.Where(expWhereFiltroStatus);


            lstChamados = qryConsulta.Select(_instanciadorDTOChamadoSuporte())
                                     .ToList();

            return lstChamados;
        }

        public ChamadoSuporteEntity ObterChamadoPorID(int chamadoSuporteID)
        {
            ChamadoSuporteEntity chamadoSuporte = null;

            if (chamadoSuporteID > 0)
            {
                chamadoSuporte = Db.TB_CHAMADO_SUPORTEs.Where(_chamado => _chamado.TB_CHAMADO_SUPORTE_ID == chamadoSuporteID)
                                                       .Select(_instanciadorDTOChamadoSuporte())
                                                       .FirstOrDefault();
            }

            return chamadoSuporte;
        }
        #endregion

        public IList<ChamadoSuporteEntity> SelecionarChamados(tipoPesquisa tipoPesquisa, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario, long tabelaPesquisaID, bool? chamadosAtivos = true)
        {
            IList<ChamadoSuporteEntity> lstChamados = null;

            switch (tipoPesquisa)
            {
                case GeralEnum.TipoPesquisa.SemFiltro: lstChamados = ObterTodosOsChamados(statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario, chamadosAtivos); break;
                case GeralEnum.TipoPesquisa.Gestor: lstChamados = ObterChamadosPorGestor((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.Orgao: lstChamados = ObterChamadosPorOrgao((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.UO: lstChamados = ObterChamadosPorUO((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.UGE: lstChamados = ObterChamadosPorUGE((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.Almox: lstChamados = ObterChamadosPorAlmoxarifado((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.Divisao: lstChamados = ObterChamadosPorDivisao((int)tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.Usuario: lstChamados = ObterChamadosPorUsuario(tabelaPesquisaID, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario); break;
                case GeralEnum.TipoPesquisa.ID: lstChamados = new List<ChamadoSuporteEntity>() { ObterChamadoPorID((int)tabelaPesquisaID) }; break;

                case GeralEnum.TipoPesquisa.UA:

                default: throw new Exception("Filtro de pesquisa não-implementado.");
            }

            if (lstChamados.HasElements())
                lstChamados = lstChamados.OrderByDescending(chamadoSuporte => chamadoSuporte.DataAbertura)
                                         .ToList();

            return lstChamados;
        }

        private Func<TB_CHAMADO_SUPORTE, ChamadoSuporteEntity> _instanciadorDTOChamadoSuporte()
        {
            Func<TB_CHAMADO_SUPORTE, ChamadoSuporteEntity> _actionSeletor = null;

            _actionSeletor = rowTabela => new ChamadoSuporteEntity
            {
                Id = rowTabela.TB_CHAMADO_SUPORTE_ID,
                //NumeroChamado = rowTabela.TB_CHAMADO_SUPORTE_ID,
                DataAbertura = rowTabela.TB_CHAMADO_SUPORTE_DATA_ABERTURA,
                DataFechamento = rowTabela.TB_CHAMADO_SUPORTE_DATA_FECHAMENTO,
                Almoxarifado = (rowTabela.TB_ALMOXARIFADO.IsNotNull() ? (new AlmoxarifadoEntity()
                {
                    Id = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                    Codigo = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                    Descricao = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                    Uge = (rowTabela.TB_ALMOXARIFADO.TB_UGE.IsNotNull() ? (new UGEEntity()
                    {
                        Id = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID,
                        Codigo = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO,
                        Uo = (rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.IsNotNull() ? (new UOEntity()
                        {
                            Id = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_UO_ID,
                            Codigo = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_UO_CODIGO,
                            Descricao = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_UO_DESCRICAO
                        }) : null),
                        Orgao = (rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_ORGAO.IsNotNull() ? (new OrgaoEntity()
                        {
                            Id = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                            Codigo = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                            Descricao = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                        }) : null)
                    }) : null),
                    Gestor = (rowTabela.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull() ? (new GestorEntity()
                    {
                        Id = rowTabela.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID,
                        NomeReduzido = rowTabela.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO,
                        Nome = rowTabela.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME
                    }) : null)
                }) : null),
                CpfUsuario = rowTabela.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA,
                EMailUsuario = rowTabela.TB_CHAMADO_SUPORTE_EMAIL_USUARIO,
                SistemaModulo = rowTabela.TB_CHAMADO_SUPORTE_MODULO_SISTEMA,
                Funcionalidade = (rowTabela.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA.IsNotNull() ? (new FuncionalidadeSistemaEntity()
                {
                    Id = rowTabela.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID,
                    Descricao = rowTabela.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_DESCRICAO
                }) : null),
                StatusChamadoAtendimentoProdesp = rowTabela.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_PRODESP,
                StatusChamadoAtendimentoUsuario = rowTabela.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_USUARIO,
                Responsavel = rowTabela.TB_CHAMADO_SUPORTE_RESPONSAVEL_ATENDIMENTO,
                TipoChamado = rowTabela.TB_CHAMADO_SUPORTE_TIPO_CHAMADO,
                PerfilUsuarioAberturaChamado = rowTabela.TB_PERFIL_ID,
                Observacoes = rowTabela.TB_CHAMADO_SUPORTE_OBSERVACOES,
                HistoricoAtendimento = rowTabela.TB_CHAMADO_SUPORTE_HISTORICO_ATENDIMENTO,
                LogHistoricoAtendimento = rowTabela.TB_CHAMADO_SUPORTE_HISTORICO_ATENDIMENTO,
                Anexos = gerarListaDeAnexos(rowTabela),
                UsuarioCancelador = rowTabela.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA_CANCELADOR,
                Divisao = (rowTabela.TB_DIVISAO.IsNotNull() ? (new DivisaoEntity()
                {
                    Id = rowTabela.TB_DIVISAO.TB_DIVISAO_ID,
                    Codigo = rowTabela.TB_DIVISAO.TB_DIVISAO_CODIGO,
                    Descricao = rowTabela.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                    Ua = (rowTabela.TB_DIVISAO.TB_UA.IsNotNull() ? (new UAEntity()
                    {
                        Id = rowTabela.TB_DIVISAO.TB_UA.TB_UA_ID,
                        Codigo = rowTabela.TB_DIVISAO.TB_UA.TB_UA_CODIGO,
                        Descricao = rowTabela.TB_DIVISAO.TB_UA.TB_UA_DESCRICAO
                    }) : null),
                    Almoxarifado = (rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.IsNotNull() ? (new AlmoxarifadoEntity()
                    {
                        Id = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                        Codigo = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                        Descricao = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                        Uge = (rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_UGE.IsNotNull() ? (new UGEEntity()
                        {
                            Id = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID,
                            Codigo = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO
                        }) : null),
                        Gestor = (rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_GESTOR.IsNotNull() ? (new GestorEntity()
                        {
                            Id = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID,
                            NomeReduzido = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO,
                            Nome = rowTabela.TB_DIVISAO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME
                        }) : null)
                    }) : null)
                }) : null),
            };

            return _actionSeletor;
        }

        private IList<AnexoChamadoSuporte> gerarListaDeAnexos(TB_CHAMADO_SUPORTE rowTabela)
        {
            IList<AnexoChamadoSuporte> listaAnexos = null;

            if (rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_001.IsNull() && rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_002.IsNull() && rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_003.IsNull())
                return listaAnexos;
            else
                listaAnexos = new List<AnexoChamadoSuporte>();


            if ((!String.IsNullOrWhiteSpace(rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_001)) && (rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_001.IsNotNull()))
                listaAnexos.Add(new AnexoChamadoSuporte() { NomeArquivo = rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_001, ConteudoArquivo = rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_001.ToArray() });

            if ((!String.IsNullOrWhiteSpace(rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_002)) && (rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_002.IsNotNull()))
                listaAnexos.Add(new AnexoChamadoSuporte() { NomeArquivo = rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_002, ConteudoArquivo = rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_002.ToArray() });

            if ((!String.IsNullOrWhiteSpace(rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_003)) && (rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_003.IsNotNull()))
                listaAnexos.Add(new AnexoChamadoSuporte() { NomeArquivo = rowTabela.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_003, ConteudoArquivo = rowTabela.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_003.ToArray() });


            return listaAnexos;
        }

        public void Excluir()
        {
            TB_CHAMADO_SUPORTE tbChamadoSuporte = this.Db.TB_CHAMADO_SUPORTEs
                                                      .Where(a => a.TB_CHAMADO_SUPORTE_ID == this.Entity.Id)
                                                      .FirstOrDefault();

            if (tbChamadoSuporte.IsNotNull())
            {
                this.Db.TB_CHAMADO_SUPORTEs.DeleteOnSubmit(tbChamadoSuporte);
                this.Db.SubmitChanges();
            }
            else
                throw new Exception("Registro não encontrado.");
        }

        public bool Salvar()
        {
            bool isUpdate = isUpdate = this.Entity.Id.HasValue;
            string _log = null;
            TB_CHAMADO_SUPORTE tbChamadoSuporte = null;

            using (TransactionScope tranScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    tbChamadoSuporte = new TB_CHAMADO_SUPORTE();

                    if (isUpdate)
                    {
                        tbChamadoSuporte = this.Db.TB_CHAMADO_SUPORTEs.Where(a => a.TB_CHAMADO_SUPORTE_ID == this.Entity.Id).FirstOrDefault();
                        _log = ProcessarHistoricoAtendimento(this.Entity);
                    }
                    else
                    {
                        Db.TB_CHAMADO_SUPORTEs.InsertOnSubmit(tbChamadoSuporte);
                        _log = ProcessarHistoricoAtendimento(this.Entity);
                    }

                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_DATA_ABERTURA = this.Entity.DataAbertura;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_DATA_FECHAMENTO = this.Entity.DataFechamento;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_ALMOXARIFADO_ID = ((this.Entity.Almoxarifado.IsNotNull() && this.Entity.Almoxarifado.Id.HasValue) ? (this.Entity.Almoxarifado.Id.Value) : -1);
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA = ((this.Entity.Id.HasValue) ? tbChamadoSuporte.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA : this.Entity.CpfUsuario);
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_EMAIL_USUARIO = this.Entity.EMailUsuario;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_MODULO_SISTEMA = this.Entity.SistemaModulo;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID = ((this.Entity.Funcionalidade.IsNotNull() && this.Entity.Funcionalidade.Id.HasValue) ? (this.Entity.Funcionalidade.Id.Value) : (byte)0);
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_PRODESP = this.Entity.StatusChamadoAtendimentoProdesp;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_USUARIO = this.Entity.StatusChamadoAtendimentoUsuario;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_RESPONSAVEL_ATENDIMENTO = this.Entity.Responsavel;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_TIPO_CHAMADO = this.Entity.TipoChamado;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_OBSERVACOES = this.Entity.Observacoes;
                    tbChamadoSuporte.TB_PERFIL_ID = this.Entity.PerfilUsuarioAberturaChamado;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_HISTORICO_ATENDIMENTO = _log;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_AMBIENTE_SISTEMA = this.Entity.AmbienteSistema;
                    tbChamadoSuporte.TB_DIVISAO_ID = ((this.Entity.Divisao.IsNotNull() && this.Entity.Divisao.Id.HasValue) ? (this.Entity.Divisao.Id.Value) : -1);

                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_USUARIO_SISTEMA_CANCELADOR = this.Entity.UsuarioCancelador;

                    ZerarColunasDosAnexos(tbChamadoSuporte);

                    if (this.Entity.Anexos.HasElements())
                    {
                        var possuiUmAnexo = this.Entity.Anexos.Count == 1;
                        var possuiDoisAnexos = this.Entity.Anexos.Count < 3 && !possuiUmAnexo;
                        var possuiTresAnexos = (!possuiUmAnexo && !possuiDoisAnexos);

                        if ((possuiUmAnexo || possuiDoisAnexos || possuiTresAnexos) && this.Entity.Anexos[0].IsNotNull())
                        {
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_001 = this.Entity.Anexos[0].NomeArquivo;
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_001 = this.Entity.Anexos[0].ConteudoArquivo;
                        }

                        if ((possuiDoisAnexos || possuiTresAnexos) && this.Entity.Anexos[1].IsNotNull())
                        {
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_002 = this.Entity.Anexos[1].NomeArquivo;
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_002 = this.Entity.Anexos[1].ConteudoArquivo;
                        }

                        if (possuiTresAnexos && this.Entity.Anexos[2].IsNotNull())
                        {
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_003 = this.Entity.Anexos[2].NomeArquivo;
                            tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_003 = this.Entity.Anexos[2].ConteudoArquivo;
                        }
                    }

                    this.Db.SubmitChanges();
                    this.Entity.Id = tbChamadoSuporte.TB_CHAMADO_SUPORTE_ID;
                }
                catch (Exception excErroGravacao)
                {
                    throw new Exception(excErroGravacao.Message);
                }

                tranScope.Complete();
            }

            return (tbChamadoSuporte.IsNotNull() && tbChamadoSuporte.TB_CHAMADO_SUPORTE_ID != 0);
        }

        private bool atualizarStatus()
        {
            TB_CHAMADO_SUPORTE tbChamadoSuporte = null;

            using (TransactionScope tranScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    tbChamadoSuporte = this.Db.TB_CHAMADO_SUPORTEs.Where(a => a.TB_CHAMADO_SUPORTE_ID == this.Entity.Id).FirstOrDefault();
                    var _log = ProcessarHistoricoAtendimento(this.Entity);

                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_STATUS_ATENDIMENTO_PRODESP = this.Entity.StatusChamadoAtendimentoProdesp;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_RESPONSAVEL_ATENDIMENTO = this.Entity.Responsavel;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_OBSERVACOES = this.Entity.Observacoes;
                    tbChamadoSuporte.TB_CHAMADO_SUPORTE_HISTORICO_ATENDIMENTO = _log;

                    this.Db.SubmitChanges();
                    this.Entity.Id = tbChamadoSuporte.TB_CHAMADO_SUPORTE_ID;
                }
                catch (Exception excErroGravacao)
                {
                    throw new Exception(excErroGravacao.Message);
                }

                tranScope.Complete();
            }
                return (tbChamadoSuporte.IsNotNull() && tbChamadoSuporte.TB_CHAMADO_SUPORTE_ID != 0);
        }

        private static void ZerarColunasDosAnexos(TB_CHAMADO_SUPORTE tbChamadoSuporte)
        {
            tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_001 = tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_002 = tbChamadoSuporte.TB_CHAMADO_SUPORTE_NOME_ARQUIVO_ANEXO_003 = null;
            tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_001 = tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_002 = tbChamadoSuporte.TB_CHAMADO_SUPORTE_ARQUIVO_ANEXO_003 = null;
        }

        public bool ExisteCodigoInformado()
        {
            bool blnRetorno = true;

            if (this.Entity.Id.HasValue)
                blnRetorno = Db.TB_CHAMADO_SUPORTEs.Select(chamadoSuporte => chamadoSuporte.TB_CHAMADO_SUPORTE_ID == this.Entity.Id).FirstOrDefault().IsNotNull();

            return blnRetorno;
        }

        public int Count()
        {
            int _numRegistros = -1;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                _numRegistros = Db.TB_CHAMADO_SUPORTEs.Count();
            }

            return _numRegistros;
        }

        public string ProcessarHistoricoAtendimento(ChamadoSuporteEntity rowParaUpdate)
        {
            string strRetorno = null;

            strRetorno = obterAlteracoesRowTabela(rowParaUpdate);
            rowParaUpdate.HistoricoAtendimento = obterAlteracoesAnterioresRowTabela(rowParaUpdate);

            if (String.IsNullOrWhiteSpace(rowParaUpdate.HistoricoAtendimento) || !XmlUtil.IsXML(rowParaUpdate.HistoricoAtendimento))
                rowParaUpdate.HistoricoAtendimento = null;


            if (!String.IsNullOrWhiteSpace(rowParaUpdate.HistoricoAtendimento) && XmlUtil.IsXML(strRetorno))
            {
                var xmlHistoricoAtendimento = XDocument.Parse(rowParaUpdate.HistoricoAtendimento);
                xmlHistoricoAtendimento.Element("TB_CHAMADO_SUPORTE").Add(XElement.Parse(strRetorno));

                strRetorno = xmlHistoricoAtendimento.ToString();
            }
            else if (String.IsNullOrWhiteSpace(rowParaUpdate.HistoricoAtendimento) && (!String.IsNullOrWhiteSpace(strRetorno) && XmlUtil.IsXML(strRetorno)))
            {
                var xmlHistoricoAtendimento = new XDocument();
                xmlHistoricoAtendimento.Add(new XElement("TB_CHAMADO_SUPORTE", XElement.Parse(strRetorno)));

                strRetorno = xmlHistoricoAtendimento.ToString();
            }

            return strRetorno;
        }

        private string obterAlteracoesRowTabela(ChamadoSuporteEntity rowParaUpdate)
        {
            int rowTabelaID = -1;
            string strRetorno = null;
            string descricaoAcaoEfetuada = null;
            ChamadoSuporteEntity rowOriginal = null;
            string[] arrCamposExcluidosDeTrilhamento = null;
            //Variável que armazena o valor das propriedades
            object valorAntigo, valorNovo;
            string nomeCampo = null;
            string fmtDescricaoAcaoEfetuada = null;
            int codigoEntidadeEstruturaOrganizacional = 0;
            int codigoSubEntidadeEstruturaOrganizacional = 0;


            arrCamposExcluidosDeTrilhamento = new string[] { "historicoatendimento", "loghistoricoatendimento", "anexos", "datafechamento" };
            XElement xml = new XElement("AlteracaoRegistro", new XAttribute("DataAlteracao", DateTime.Now.ToString("yyyy/MM/dd HH:mm")), new XAttribute("AlteradoPor", rowParaUpdate.CpfUsuarioLogado)); // Crio o elemente referente ao módulo
            //Pego as propriedades de uma das entidades (já que as duas são iguais).
            MemberInfo[] campos = rowParaUpdate.GetType()
                                               .GetMembers()
                                               .Cast<MemberInfo>()
                                               .Where(tipoMembroEntidade => tipoMembroEntidade.MemberType == MemberTypes.Field || tipoMembroEntidade.MemberType == MemberTypes.Property)
                                               .Where(tipoMembroEntidade => tipoMembroEntidade.DeclaringType == tipoMembroEntidade.ReflectedType)
                                               .Where(tipoMembroEntidade => !arrCamposExcluidosDeTrilhamento.Contains(tipoMembroEntidade.Name.ToLowerInvariant()))
                                               .ToArray();

            if (rowParaUpdate.Id.HasValue)
            {
                rowTabelaID = rowParaUpdate.Id.Value;
            }
            else if (!rowParaUpdate.Id.HasValue)
            {
                if (rowParaUpdate.Divisao.IsNotNull())
                {
                    fmtDescricaoAcaoEfetuada = "Chamado aberto por Divisão {0:D5}, UA {1:D7}.";

                    codigoEntidadeEstruturaOrganizacional = rowParaUpdate.Divisao.Ua.Codigo.Value;
                    codigoSubEntidadeEstruturaOrganizacional = rowParaUpdate.Divisao.Codigo.Value;
                }
                else if (rowParaUpdate.Almoxarifado.IsNotNull())
                {
                    fmtDescricaoAcaoEfetuada = "Chamado aberto por Almoxarifado {0:D3}, UGE {1:D6}.";

                    codigoEntidadeEstruturaOrganizacional = rowParaUpdate.Almoxarifado.Uge.Codigo.Value;
                    codigoSubEntidadeEstruturaOrganizacional = rowParaUpdate.Almoxarifado.Codigo.Value;
                }

                descricaoAcaoEfetuada = String.Format(fmtDescricaoAcaoEfetuada, codigoSubEntidadeEstruturaOrganizacional, codigoEntidadeEstruturaOrganizacional);


                xml.Add(new XElement("StatusAtendimento",
                        new XAttribute("ValorAnterior", "NULL"),
                        new XAttribute("ValorAtual", rowParaUpdate.StatusChamadoAtendimentoProdesp.ToString()),
                        new XAttribute("DescricaoAcaoEfetuada", descricaoAcaoEfetuada)));

                xml.Add(new XElement("Observacoes",
                        new XAttribute("ValorAnterior", "NULL"),
                        new XAttribute("ValorAtual", rowParaUpdate.Observacoes),
                        new XAttribute("DescricaoAcaoEfetuada", descricaoAcaoEfetuada)));

                var xmlDoc = XElement.Parse(xml.ToString());


                strRetorno = xmlDoc.ToString();
                return strRetorno;
            }

            rowOriginal = this.Db.TB_CHAMADO_SUPORTEs.Where(a => a.TB_CHAMADO_SUPORTE_ID == rowTabelaID)
                                                     .Select(_instanciadorDTOChamadoSuporte())
                                                     .FirstOrDefault();


            //passo por todas as propriedades da entidade
            foreach (MemberInfo campoEntidade in campos)
            {
                nomeCampo = campoEntidade.Name;

                //Atribuo valor as entidades.
                valorAntigo = (campoEntidade is PropertyInfo) ? ((PropertyInfo)campoEntidade).GetValue(rowOriginal, null) : ((FieldInfo)campoEntidade).GetValue(rowOriginal);
                valorNovo = (campoEntidade is PropertyInfo) ? ((PropertyInfo)campoEntidade).GetValue(rowParaUpdate, null) : ((FieldInfo)campoEntidade).GetValue(rowParaUpdate);

                //Catch NULL #001
                valorAntigo = valorAntigo == null ? "NULL" : valorAntigo;
                valorNovo = valorNovo == null ? "NULL" : valorNovo;

                if (valorAntigo.IsNotNull() && valorAntigo.GetType().BaseType == typeof(BaseEntity))
                {
                    var _nestedEntidadeAntiga = ((BaseEntity)((campoEntidade is PropertyInfo) ? ((PropertyInfo)campoEntidade).GetValue(rowOriginal, null) : ((FieldInfo)campoEntidade).GetValue(rowOriginal)));
                    var _nestedEntidade = ((BaseEntity)((campoEntidade is PropertyInfo) ? ((PropertyInfo)campoEntidade).GetValue(rowParaUpdate, null) : ((FieldInfo)campoEntidade).GetValue(rowParaUpdate)));

                    valorAntigo = (_nestedEntidadeAntiga.IsNotNull() ? _nestedEntidadeAntiga.Id : null);
                    valorNovo = (_nestedEntidade.IsNotNull() ? _nestedEntidade.Id : null);
                }

                //Catch NULL #002
                valorAntigo = valorAntigo == null ? "NULL" : valorAntigo;
                valorNovo = valorNovo == null ? "NULL" : valorNovo;

                if (rowParaUpdate.Id.HasValue)
                {
                    switch (nomeCampo)
                    {
                        case "SistemaModulo": break;
                        case "Responsavel": descricaoAcaoEfetuada = String.Format("Responsável atual pelo chamado: {0}.", valorNovo); break;
                        case "Observacao": descricaoAcaoEfetuada = String.Format("Campo 'Observação' alterado: {0}.", valorNovo); break;
                        case "StatusChamadoAtendimentoProdesp": descricaoAcaoEfetuada = String.Format("Status do chamado alterado para {0}, por atendente Prodesp.", EnumUtils.GetEnumDescription<statusAtendimentoChamado>(((statusAtendimentoChamado)(byte)valorNovo))); break;
                        case "StatusChamadoAtendimentoUsuario": descricaoAcaoEfetuada = String.Format("Status do chamado alterado para {0}, pelo usuário.", EnumUtils.GetEnumDescription<statusAtendimentoChamado>(((statusAtendimentoChamado)(byte)valorNovo))); break;
                        case "Funcionalidade": descricaoAcaoEfetuada = String.Format("Funcionalidade alterada de {0} para {1}.", EnumUtils.GetEnumDescription<FuncionalidadeSistema>(((FuncionalidadeSistema)(int)valorAntigo)), EnumUtils.GetEnumDescription<FuncionalidadeSistema>(((FuncionalidadeSistema)(int)valorNovo))); break;
                        case "TipoChamado": descricaoAcaoEfetuada = String.Format("Tipo de chamado alterado de {0} para {1}.", EnumUtils.GetEnumDescription<TipoChamadoSuporte>(((TipoChamadoSuporte)(byte)valorAntigo)), EnumUtils.GetEnumDescription<TipoChamadoSuporte>(((TipoChamadoSuporte)(byte)valorNovo))); break;
                    }
                }

                //Verifico se os valores são diferentes
                if (!valorAntigo.Equals(valorNovo))
                {
                    if (String.IsNullOrWhiteSpace(descricaoAcaoEfetuada))
                        descricaoAcaoEfetuada = String.Format("'{0}' de '{1}' para '{2}'.", nomeCampo, valorAntigo, valorNovo);

                    xml.Add(new XElement(nomeCampo, // Crio um elemento com o nome da propriedade
                                            new XAttribute("ValorAnterior", valorAntigo), // um atributo com o valor antigo
                                            new XAttribute("ValorAtual", valorNovo), // um atributo com o novo valor.
                                            new XAttribute("DescricaoAcaoEfetuada", descricaoAcaoEfetuada))); // um atributo com o novo valor.
                }

                descricaoAcaoEfetuada = null;
            }

            strRetorno = xml.ToString();
            return strRetorno;
        }

        private string obterAlteracoesAnterioresRowTabela(ChamadoSuporteEntity rowParaUpdate)
        {
            int rowTabelaID = -1;
            string strRetorno = null;
            ChamadoSuporteEntity rowOriginal = null;


            if (rowParaUpdate.Id.HasValue)
            {
                rowTabelaID = rowParaUpdate.Id.Value;
                rowOriginal = this.Db.TB_CHAMADO_SUPORTEs.Where(a => a.TB_CHAMADO_SUPORTE_ID == rowTabelaID)
                                         .Select(_instanciadorDTOChamadoSuporte())
                                         .FirstOrDefault();

                strRetorno = rowOriginal.HistoricoAtendimento;
            }

            return strRetorno;
        }

    }
}
