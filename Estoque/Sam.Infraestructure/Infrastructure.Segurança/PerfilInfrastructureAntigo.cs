using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Common;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Infrastructure
{
    public class PerfilInfrastructureAntigo : BaseInfrastructure
    {

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Entity
        {
            get { return perfil; }
            set { perfil = value; }
        }

        public List<Almoxarifado> RecuperarAlmox(int AlmoxId)
        {
            var resposta = (from almox in DbSam.TB_ALMOXARIFADO
                            where almox.TB_ALMOXARIFADO_ID == AlmoxId
                            select new Almoxarifado
                            {
                                AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID,
                                AlmoxarifadoDescricao = almox.TB_ALMOXARIFADO_DESCRICAO
                            }
                       ).ToList();

            return resposta;
        }

        public List<Sam.Entity.Perfil> RecuperarPerfil_Novo(int loginId)
        {

            List<Sam.Entity.Perfil> resposta = null;
            resposta = (from loginPerfil in Db.TB_PERFIL_LOGIN
                        join perfil in Db.TB_PERFIL on loginPerfil.TB_PERFIL_ID equals perfil.TB_PERFIL_ID
                        join lg in Db.TB_LOGIN on loginPerfil.TB_LOGIN_ID equals lg.TB_LOGIN_ID
                        join ac in Db.TB_LOGIN_NIVEL_ACESSO on loginPerfil.TB_PERFIL_LOGIN_ID equals ac.TB_PERFIL_LOGIN_ID

                        where loginPerfil.TB_LOGIN_ID.Equals(loginId)
                        where (ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) ||
                        ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                        orderby perfil.TB_PERFIL_DESCRICAO
                        select
                            new
                                Sam.Entity.Perfil
                            {
                                Ativo = perfil.TB_PERFIL_ATIVO,
                                Descricao = perfil.TB_PERFIL_DESCRICAO,
                                IdPerfil = perfil.TB_PERFIL_ID,
                                OrgaoPadrao = new OrgaoEntity { Id = loginPerfil.TB_ORGAO_ID_PADRAO ?? 0 },
                                GestorPadrao = new GestorEntity { Id = loginPerfil.TB_GESTOR_ID_PADRAO ?? 0 },
                                AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = loginPerfil.TB_ALMOXARIFADO_ID_PADRAO },
                                UOPadrao = new UOEntity { Id = loginPerfil.TB_UO_ID_PADRAO ?? 0 },
                                UGEPadrao = new UGEEntity { Id = loginPerfil.TB_UGE_ID_PADRAO ?? 0 },
                                UAPadrao = new UAEntity { Id = loginPerfil.TB_UA_ID_PADRAO ?? 0 },
                                DivisaoPadrao = new DivisaoEntity { Id = loginPerfil.TB_DIVISAO_ID_PADRAO ?? 0 },
                                PerfilLoginId = loginPerfil.TB_PERFIL_LOGIN_ID,
                                IdLogin = loginPerfil.TB_LOGIN_ID
                            }).ToList();

            for (int i = 0; i < resposta.Count; i++)
            {
                //Recupera do nivel de acesso
                int perfilLoginId = resposta[i].PerfilLoginId;

                int almoxId = 0;
                var almox = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
            a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.ALMOXARIFADO).FirstOrDefault();

                if (almox != null)
                    almoxId = almox.TB_LOGIN_NIVEL_ACESSO_VALOR;

                int DivisaoId = 0;
                var Divisao = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
            a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO).FirstOrDefault();

                if (Divisao != null)
                    DivisaoId = Divisao.TB_LOGIN_NIVEL_ACESSO_VALOR;

                resposta[i].Almoxarifado = new AlmoxarifadoEntity(almoxId);
                resposta[i].Divisao = new DivisaoEntity(DivisaoId);

                if (resposta[i].AlmoxarifadoPadrao.Id > 0 ||
                    resposta[i].DivisaoPadrao.Id > 0)
                {
                    resposta[i].Descricao = "<font color=\"Red\">" + resposta[i].Descricao + " (Padrão)</font>";
                    resposta[i].IsAlmoxarifadoPadrao = true;
                }

                string nomeAlmox = "";
                if (resposta[i].Almoxarifado.Id != 0)
                {
                    var almoxList = RecuperarAlmox(Convert.ToInt32(resposta[i].Almoxarifado.Id));

                    if (almoxList.Count > 0)
                    {
                        nomeAlmox = almoxList[0].AlmoxarifadoDescricao;
                    }
                }
                else
                {
                    if (DivisaoId > 0)
                    {
                        var div = DbSam.TB_DIVISAO.Where(a => a.TB_DIVISAO_ID == DivisaoId).FirstOrDefault();

                        if (div != null)
                        {
                            resposta[i].DivisaoPadrao.Descricao = div.TB_DIVISAO_DESCRICAO;
                            nomeAlmox = resposta[i].DivisaoPadrao.Descricao;
                        }
                    }
                    else
                    {
                        nomeAlmox = "";
                    }
                }
                resposta[i].Descricao = resposta[i].Descricao.ToString().PadRight(30) + " -> " + nomeAlmox;
            }

            return resposta;
        }

        public List<Sam.Entity.Perfil> RecuperarPerfilExportacao(int loginId)
        {

            List<Sam.Entity.Perfil> resposta = null;
            resposta = (from loginPerfil in Db.TB_PERFIL_LOGIN
                        join perfil in Db.TB_PERFIL on loginPerfil.TB_PERFIL_ID equals perfil.TB_PERFIL_ID
                        join lg in Db.TB_LOGIN on loginPerfil.TB_LOGIN_ID equals lg.TB_LOGIN_ID
                        join ac in Db.TB_LOGIN_NIVEL_ACESSO on loginPerfil.TB_PERFIL_LOGIN_ID equals ac.TB_PERFIL_LOGIN_ID

                        where loginPerfil.TB_LOGIN_ID.Equals(loginId)
                        where (ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) ||
                        ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                        orderby perfil.TB_PERFIL_DESCRICAO
                        select
                            new
                                Sam.Entity.Perfil
                            {
                                Ativo = perfil.TB_PERFIL_ATIVO,
                                Descricao = perfil.TB_PERFIL_DESCRICAO,
                                IdPerfil = perfil.TB_PERFIL_ID,
                                OrgaoPadrao = new OrgaoEntity { Id = loginPerfil.TB_ORGAO_ID_PADRAO ?? 0 },
                                GestorPadrao = new GestorEntity { Id = loginPerfil.TB_GESTOR_ID_PADRAO ?? 0 },
                                AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = loginPerfil.TB_ALMOXARIFADO_ID_PADRAO },
                                UOPadrao = new UOEntity { Id = loginPerfil.TB_UO_ID_PADRAO ?? 0 },
                                UGEPadrao = new UGEEntity { Id = loginPerfil.TB_UGE_ID_PADRAO ?? 0 },
                                UAPadrao = new UAEntity { Id = loginPerfil.TB_UA_ID_PADRAO ?? 0 },
                                DivisaoPadrao = new DivisaoEntity { Id = loginPerfil.TB_DIVISAO_ID_PADRAO ?? 0 },
                                PerfilLoginId = loginPerfil.TB_PERFIL_LOGIN_ID,
                                IdLogin = loginPerfil.TB_LOGIN_ID
                            }).ToList();

            for (int i = 0; i < resposta.Count; i++)
            {
                //Recupera do nivel de acesso
                int perfilLoginId = resposta[i].PerfilLoginId;

                int almoxId = 0;
                var almox = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
            a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.ALMOXARIFADO).FirstOrDefault();

                if (almox != null)
                    almoxId = almox.TB_LOGIN_NIVEL_ACESSO_VALOR;

                int DivisaoId = 0;
                var Divisao = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
            a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO).FirstOrDefault();

                if (Divisao != null)
                    DivisaoId = Divisao.TB_LOGIN_NIVEL_ACESSO_VALOR;

                resposta[i].Almoxarifado = new AlmoxarifadoEntity(almoxId);
                resposta[i].Divisao = new DivisaoEntity(DivisaoId);

                if (resposta[i].AlmoxarifadoPadrao.Id > 0 ||
                    resposta[i].DivisaoPadrao.Id > 0)
                {
                    resposta[i].Descricao = resposta[i].Descricao;

                    resposta[i].IsAlmoxarifadoPadrao = true;
                }

                string nomeAlmox = "";
                if (resposta[i].Almoxarifado.Id != 0)
                {
                    var almoxList = RecuperarAlmox(Convert.ToInt32(resposta[i].Almoxarifado.Id));

                    if (almoxList.Count > 0)
                    {
                        nomeAlmox = almoxList[0].AlmoxarifadoDescricao;
                    }
                }
                else
                {
                    if (DivisaoId > 0)
                    {
                        var div = DbSam.TB_DIVISAO.Where(a => a.TB_DIVISAO_ID == DivisaoId).FirstOrDefault();

                        if (div != null)
                        {
                            resposta[i].DivisaoPadrao.Descricao = div.TB_DIVISAO_DESCRICAO;
                            nomeAlmox = resposta[i].DivisaoPadrao.Descricao;
                        }
                    }
                    else
                    {
                        nomeAlmox = "";
                    }
                }
                resposta[i].Descricao = resposta[i].Descricao.ToString() + " - " + nomeAlmox;
            }

            return resposta;
        }


        private List<Sam.Entity.Perfil> RetornaPerfilPadrao(int loginId)
        {
            List<Sam.Entity.Perfil> resposta = null;

            //retorna o perfil padrao do Orgão
            resposta = (from loginPerfil in Db.TB_PERFIL_LOGIN
                        join perfil in Db.TB_PERFIL on loginPerfil.TB_PERFIL_ID equals perfil.TB_PERFIL_ID
                        where loginPerfil.TB_LOGIN_ID.Equals(loginId)
                        where loginPerfil.TB_ORGAO_ID_PADRAO > 0
                        select
                            new
                                Sam.Entity.Perfil
                            {
                                Ativo = perfil.TB_PERFIL_ATIVO,
                                Peso = perfil.TB_PERFIL_PESO,
                                Descricao = perfil.TB_PERFIL_DESCRICAO,
                                IdPerfil = perfil.TB_PERFIL_ID,
                                OrgaoPadrao = new OrgaoEntity { Id = loginPerfil.TB_ORGAO_ID_PADRAO ?? 0 },
                                GestorPadrao = new GestorEntity { Id = loginPerfil.TB_GESTOR_ID_PADRAO ?? 0 },
                                AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = loginPerfil.TB_ALMOXARIFADO_ID_PADRAO ?? 0 },
                                UOPadrao = new UOEntity { Id = loginPerfil.TB_UO_ID_PADRAO ?? 0 },
                                UGEPadrao = new UGEEntity { Id = loginPerfil.TB_UGE_ID_PADRAO ?? 0 },
                                UAPadrao = new UAEntity { Id = loginPerfil.TB_UA_ID_PADRAO ?? 0 },
                                DivisaoPadrao = new DivisaoEntity { Id = loginPerfil.TB_DIVISAO_ID_PADRAO ?? 0 },
                                PerfilLoginId = loginPerfil.TB_PERFIL_LOGIN_ID,
                                IdLogin = loginPerfil.TB_LOGIN_ID,
                            }).ToList();
            return resposta;

        }

        private bool VerificaPerfilRequisitante(Sam.Entity.Perfil perfilPadrao)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          where a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO
                          where a.TB_PERFIL_LOGIN_ID == perfilPadrao.PerfilLoginId
                          select a).Count();

            if (result > 0)
                return true;
            else
                return false;
        }

        private AlmoxarifadoEntity RetornaAlmoxarifadoDivisao(int divisaoId)
        {
            var result = (from a in DbSam.TB_DIVISAO
                          where a.TB_DIVISAO_ID == divisaoId
                          select new AlmoxarifadoEntity
                          {
                              Id = a.TB_ALMOXARIFADO_ID
                          }).FirstOrDefault();

            return result;
        }


        public List<Sam.Entity.Perfil> RecuperarPerfil(int loginId)
        {
            List<Sam.Entity.Perfil> resposta = null;
            try
            {
                resposta = RetornaPerfilPadrao(loginId);

                if (resposta == null)
                {
                    throw new Exception("Não existe um perfil padrão para o usuário");
                }
                else
                {

                    for (int i = 0; i < resposta.Count; i++)
                    {

                        if (VerificaPerfilRequisitante(resposta[i]))
                        {
                            int idDivisaoPadrao = (int)resposta[i].DivisaoPadrao.Id;

                            //Perfil Requisitante
                            resposta[i].AlmoxarifadoPadrao = RetornaAlmoxarifadoDivisao(idDivisaoPadrao);
                        }

                        int almoxId = 0;
                        short perfilId = resposta[i].IdPerfil;
                        almoxId = resposta[i].AlmoxarifadoPadrao.Id.Value;


                        ///Adiciona o Nivel de Acesso
                        resposta[i].PerfilLoginNivelAcesso = (from loginNivel in Db.TB_LOGIN_NIVEL_ACESSO
                                                              where loginNivel.TB_PERFIL_LOGIN.TB_LOGIN_ID == loginId
                                                              select new PerfilLoginNivelAcesso
                                                              {
                                                                  Id = loginNivel.TB_PERFIL_LOGIN_ID,
                                                                  Valor = loginNivel.TB_LOGIN_NIVEL_ACESSO_VALOR,
                                                                  NivelAcesso = (from nivel in Db.TB_NIVEL
                                                                                 where nivel.TB_NIVEL_ID == loginNivel.TB_NIVEL_ID
                                                                                 select new NivelAcesso
                                                                                 {
                                                                                     Id = nivel.TB_NIVEL_ID,
                                                                                     DescricaoNivel = nivel.TB_NIVEL_DESCRICAO
                                                                                 }).FirstOrDefault()
                                                              }).ToList();

                        if (resposta[i].AlmoxarifadoPadrao.Id.Value == 0)
                        {

                            int divisaoIdAlmox = resposta[i].DivisaoPadrao.Id.Value;

                            resposta[i].DivisaoPadrao = (from divisao in DbSam.TB_DIVISAO
                                                         where divisao.TB_DIVISAO_ID == divisaoIdAlmox
                                                         select new DivisaoEntity
                                                         {
                                                             Id = divisao.TB_DIVISAO_ID,
                                                             Descricao = divisao.TB_DIVISAO_DESCRICAO,
                                                             Almoxarifado = new AlmoxarifadoEntity { Id = divisao.TB_ALMOXARIFADO_ID ?? 0 }
                                                         }).FirstOrDefault();

                            almoxId = resposta[i].DivisaoPadrao.Almoxarifado.Id.Value;


                        }
                        else
                        {
                            resposta[i].AlmoxarifadoPadrao = (from almox in DbSam.TB_ALMOXARIFADO
                                                              where almox.TB_ALMOXARIFADO_ID == almoxId
                                                              select new AlmoxarifadoEntity
                                                              {
                                                                  Id = almox.TB_ALMOXARIFADO_ID,
                                                                  Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                                                                  RefInicial = almox.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                                  MesRef = almox.TB_ALMOXARIFADO_MES_REF,
                                                                  Uge = new UGEEntity { Id = almox.TB_UGE_ID, Codigo = almox.TB_UGE.TB_UGE_CODIGO, Descricao = almox.TB_UGE.TB_UGE_DESCRICAO, Uo = new UOEntity { Id = almox.TB_UGE.TB_UO.TB_UO_ID, Codigo = almox.TB_UGE.TB_UO.TB_UO_CODIGO, Descricao = almox.TB_UGE.TB_UO.TB_UO_DESCRICAO } },
                                                                  Gestor = new GestorEntity { Id = almox.TB_GESTOR.TB_GESTOR_ID, CodigoGestao = almox.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = almox.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = almox.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO, Orgao = new OrgaoEntity() { Id = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_ID, Codigo = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO, Descricao = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_DESCRICAO } },
                                                                  Uf = new UFEntity { Id = almox.TB_UF_ID },
                                                                  Divisao = new DivisaoEntity { Id = almox.TB_DIVISAO.FirstOrDefault().TB_DIVISAO_ID, Codigo = almox.TB_DIVISAO.FirstOrDefault().TB_DIVISAO_CODIGO, Descricao = almox.TB_DIVISAO.FirstOrDefault().TB_DIVISAO_DESCRICAO },
                                                              }).FirstOrDefault();
                        }


                        //Setar o almoxarifado logado para o almoxarifado padrão
                        resposta[i].AlmoxarifadoLogado = resposta[i].AlmoxarifadoPadrao;

                        //Buscar gestor padrão
                        int gestorId = resposta[i].GestorPadrao.Id.Value;
                        resposta[i].GestorPadrao = (from gestor in DbSam.TB_GESTOR
                                                    where gestor.TB_GESTOR_ID.Equals(gestorId)
                                                    select new GestorEntity { Id = gestor.TB_GESTOR_ID, Nome = gestor.TB_GESTOR_NOME, NomeReduzido = gestor.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestor.TB_GESTOR_CODIGO_GESTAO }).FirstOrDefault();

                        //Buscar orgão padrão
                        int orgaoId = resposta[i].OrgaoPadrao.Id.Value;
                        resposta[i].OrgaoPadrao = (from orgao in DbSam.TB_ORGAO
                                                   where orgao.TB_ORGAO_ID.Equals(orgaoId)
                                                   select new OrgaoEntity { Id = orgao.TB_ORGAO_ID, Codigo = orgao.TB_ORGAO_CODIGO, Descricao = orgao.TB_ORGAO_DESCRICAO }).FirstOrDefault();

                        //Buscar UO padrão
                        int uoId = resposta[i].UOPadrao.Id.Value;
                        resposta[i].UOPadrao = (from uo in DbSam.TB_UO
                                                where uo.TB_UO_ID.Equals(uoId)
                                                select new UOEntity { Id = uo.TB_UO_ID, Codigo = uo.TB_UO_CODIGO, Descricao = uo.TB_UO_DESCRICAO }).FirstOrDefault();

                        //Buscar UGE padrão
                        int ugeId = resposta[i].UGEPadrao.Id.Value;
                        resposta[i].UGEPadrao = (from uge in DbSam.TB_UGE
                                                 where uge.TB_UGE_ID.Equals(ugeId)
                                                 select new UGEEntity { Id = uge.TB_UGE_ID, Codigo = uge.TB_UGE_CODIGO, Descricao = uge.TB_UGE_DESCRICAO }).FirstOrDefault();

                        //Buscar UGE padrão
                        int uaId = resposta[i].UAPadrao.Id.Value;
                        resposta[i].UAPadrao = (from ua in DbSam.TB_UA
                                                where ua.TB_UA_ID.Equals(uaId)
                                                select new UAEntity { Id = ua.TB_UA_ID, Codigo = ua.TB_UA_CODIGO, Descricao = ua.TB_UA_DESCRICAO }).FirstOrDefault();

                        //Buscar Divisão padrão
                        int divisaoId = resposta[i].DivisaoPadrao.Id.Value;
                        resposta[i].DivisaoPadrao = (from divisao in DbSam.TB_DIVISAO
                                                     where divisao.TB_DIVISAO_ID.Equals(divisaoId)
                                                     select new DivisaoEntity { Id = divisao.TB_DIVISAO_ID, Codigo = divisao.TB_DIVISAO_CODIGO, Descricao = divisao.TB_DIVISAO_DESCRICAO }).FirstOrDefault();
                        //Buscar transações
                        List<Sam.Entity.Transacao> transacoes = (from perfilTransacao in Db.TB_TRANSACAO_PERFIL
                                                                 join transacao in Db.TB_TRANSACAO on perfilTransacao.TB_TRANSACAO_ID equals transacao.TB_TRANSACAO_ID
                                                                 join modulo in Db.TB_MODULO on transacao.TB_MODULO_ID equals modulo.TB_MODULO_ID
                                                                 join sistema in Db.TB_SISTEMA on modulo.TB_SISTEMA_ID equals sistema.TB_SISTEMA_ID
                                                                 where perfilTransacao.TB_PERFIL_ID == perfilId
                                                                 //Como SAM vai compartilhar a base com SCPweb (patrimônio)
                                                                 where (modulo.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SAM || modulo.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SEG)
                                                                 select new Sam.Entity.Transacao
                                                                 {
                                                                     Ativa = transacao.TB_TRANSACAO_ATIVO,
                                                                     Caminho = transacao.TB_TRANSACAO_CAMINHO,
                                                                     Descricao = transacao.TB_TRANSACAO_DESCRICAO,
                                                                     Sigla = transacao.TB_TRANSACAO_SIGLA,
                                                                     Edita = perfilTransacao.TB_TRANSACAO_PERFIL_EDITA,
                                                                     Ordem = transacao.TB_TRANSACAO_ORDEM ?? 0,
                                                                     IdTransacao = transacao.TB_TRANSACAO_ID,
                                                                     Modulo = new Sam.Entity.Modulo
                                                                     {
                                                                         ModuloId = modulo.TB_MODULO_ID,
                                                                         Descricao = modulo.TB_MODULO_DESCRICAO,
                                                                         Sigla = modulo.TB_MODULO_SIGLA,
                                                                         ModuloAtivo = modulo.TB_MODULO_IND_ATIVO,
                                                                         Ordem = modulo.TB_MODULO_ORDEM ?? 0,
                                                                         Sistema = new Sam.Entity.Sistema
                                                                         {
                                                                             Ativo = sistema.TB_SISTEMA_ATIVO,
                                                                             Descricao = sistema.TB_SISTEMA_DESCRICAO,
                                                                             Sigla = sistema.TB_SISTEMA_SIGLA,
                                                                             SistemaId = sistema.TB_SISTEMA_ID
                                                                         }
                                                                     }
                                                                 }).ToList();
                        resposta[i].Transacoes = transacoes;

                        List<Sam.Entity.Modulo> modulos = (from modulo in Db.TB_MODULO
                                                           join sistema in Db.TB_SISTEMA on modulo.TB_SISTEMA_ID equals sistema.TB_SISTEMA_ID
                                                           where modulo.TB_MODULO_IND_ATIVO == true
                                                           //Como SAM vai compartilhar a base com SCPweb (patrimônio)
                                                           where (modulo.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SAM || modulo.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SEG)
                                                           select new Sam.Entity.Modulo
                                                           {
                                                               ModuloId = modulo.TB_MODULO_ID,
                                                               Descricao = modulo.TB_MODULO_DESCRICAO,
                                                               Sigla = modulo.TB_MODULO_SIGLA,
                                                               ModuloAtivo = modulo.TB_MODULO_IND_ATIVO,
                                                               Ordem = modulo.TB_MODULO_ORDEM ?? 0,
                                                               ModuloPaiId = modulo.TB_MODULO_ID_PAI,
                                                               Caminho = modulo.TB_MODULO_CAMINHO,
                                                               Sistema = new Sam.Entity.Sistema
                                                               {
                                                                   Ativo = sistema.TB_SISTEMA_ATIVO,
                                                                   Descricao = sistema.TB_SISTEMA_DESCRICAO,
                                                                   Sigla = sistema.TB_SISTEMA_SIGLA,
                                                                   SistemaId = sistema.TB_SISTEMA_ID
                                                               }
                                                           }).Distinct().ToList();

                        //_strDescLog = "Setar Modulos.";
                        //LogErro.GravarMsgInfo(Constante.CST_DEBUG_DEPLOY_HOMOLOGACAO, _strDescLog);

                        resposta[i].Modulos = (from modulosOrder in modulos
                                               orderby modulosOrder.Ordem
                                               select modulosOrder).ToList();

                        for (int a = 0; a < resposta[i].Modulos.Count; a++)
                        {
                            Int32 moduloId = resposta[i].Modulos[a].ModuloId;

                            List<Transacao> Transacoes = (from trans in Db.TB_TRANSACAO
                                                          join perfilTransacao in Db.TB_TRANSACAO_PERFIL on trans.TB_TRANSACAO_ID equals perfilTransacao.TB_TRANSACAO_ID
                                                          where trans.TB_MODULO_ID == moduloId && perfilTransacao.TB_PERFIL_ID == perfilId
                                                          && trans.TB_TRANSACAO_ATIVO == true
                                                          && perfilTransacao.TB_TRANSACAO_ATIVO == true
                                                          //where perfilTransacao.TB_TRANSACAO.TB_MODULO.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SAM
                                                          //Como SAM vai compartilhar a base com SCPweb (patrimônio)
                                                          where (trans.TB_MODULO.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SAM || trans.TB_MODULO.TB_SISTEMA_ID == (int)GeralEnum.Sistema.SEG)
                                                          orderby trans.TB_TRANSACAO_ORDEM ascending
                                                          select new Transacao
                                                          {
                                                              Ativa = trans.TB_TRANSACAO_ATIVO,
                                                              Caminho = trans.TB_TRANSACAO_CAMINHO,
                                                              Descricao = trans.TB_TRANSACAO_DESCRICAO,
                                                              Sigla = trans.TB_TRANSACAO_SIGLA,
                                                              Ordem = trans.TB_TRANSACAO_ORDEM ?? 0,
                                                          }).Distinct().ToList();

                            resposta[i].Modulos[a].Transacoes = (from transacoesOrder in Transacoes
                                                                 orderby transacoesOrder.Ordem
                                                                 select transacoesOrder).ToList();
                        }
                    }
                }

            }
            catch (DataException metaDataExc)
            {
                var debugDetail = (metaDataExc.InnerException.IsNotNull() ? String.Format("{0}{1}", metaDataExc.Message, metaDataExc.InnerException.Message) : metaDataExc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (DataException): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.RecuperarPerfil(int), args(loginId: {1})", debugDetail, loginId));

                throw metaDataExc;
            }
            catch (Exception exc)
            {
                var debugDetail = (exc.InnerException.IsNotNull() ? String.Format("{0}{1}", exc.Message, exc.InnerException.Message) : exc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (Generic Exception): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.RecuperarPerfil(int), args(loginId: {1})", debugDetail, loginId));

                throw exc;
            }

            return resposta;
        }

        /// <summary>
        /// Funcao auxiliar para autenticacao de usuarios do modulo de estoque, validando se o mesmo possui perfil N1, com a senha do mesmo, no sistema.
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public bool ValidarPerfilConsultaWs(string cpf, string senhaCriptografada, string codigoOrgao)
        {
            bool possuiPerfilN1 = false;
            int iCodigoOrgaoCliente = -1;
            try
            {
                if (Int32.TryParse(codigoOrgao, out iCodigoOrgaoCliente))
                {
                    var orgaoCliente = (DbSam.TB_ORGAO.Where(orgaoClienteSAM => orgaoClienteSAM.TB_ORGAO_CODIGO == iCodigoOrgaoCliente).FirstOrDefault());
                    if (orgaoCliente.IsNotNull())
                    {
                        //possuiPerfilN1 = (from TB_LOGIN in Db.TB_LOGIN
                        //                  join user2 in Db.TB_USUARIO on TB_LOGIN.TB_USUARIO_ID equals user2.TB_USUARIO_ID
                        //                  join perfilLogin in Db.TB_PERFIL_LOGIN on TB_LOGIN.TB_LOGIN_ID equals perfilLogin.TB_LOGIN_ID
                        //                  where perfilLogin.TB_PERFIL_ID == (int)EnumPerfil.ADMINISTRADOR_ORGAO
                        //                  where TB_LOGIN.TB_USUARIO.TB_USUARIO_CPF.Equals(cpf)
                        //                     && TB_LOGIN.TB_LOGIN_SENHA.Equals(senhaCriptografada)
                        //                  where user2.TB_ORGAO_ID == orgaoCliente.TB_ORGAO_ID
                        //                  where user2.TB_USUARIO_IND_ATIVO == true
                        //                  where TB_LOGIN.TB_LOGIN_ATIVO == true
                        //                  select perfilLogin).HasElements();

                        possuiPerfilN1 = (from usuarioSAM in Db.TB_USUARIO
                                          join loginUsuarioSAM in Db.TB_LOGIN on usuarioSAM.TB_USUARIO_ID equals loginUsuarioSAM.TB_USUARIO_ID
                                          join perfilUsuarioSAM in Db.TB_PERFIL_LOGIN on loginUsuarioSAM.TB_LOGIN_ID equals perfilUsuarioSAM.TB_LOGIN_ID
                                          join tipoPerfilSAM in Db.TB_PERFIL on perfilUsuarioSAM.TB_PERFIL_ID equals tipoPerfilSAM.TB_PERFIL_ID
                                          where usuarioSAM.TB_USUARIO_CPF == cpf && loginUsuarioSAM.TB_LOGIN_SENHA == senhaCriptografada
                                          where tipoPerfilSAM.TB_PERFIL_ID == (int)EnumPerfil.ADMINISTRADOR_ORGAO
                                          where usuarioSAM.TB_USUARIO_IND_ATIVO == true && loginUsuarioSAM.TB_LOGIN_ATIVO == true
                                          select perfilUsuarioSAM).HasElements();
                    }
                }
            }
            catch (DataException metaDataExc)
            {
                var debugDetail = (metaDataExc.InnerException.IsNotNull() ? String.Format("{0}{1}", metaDataExc.Message, metaDataExc.InnerException.Message) : metaDataExc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (DataException): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.RecuperarPerfil(int), args(loginId: {1})", debugDetail, cpf));

                throw metaDataExc;
            }
            catch (Exception exc)
            {
                var debugDetail = (exc.InnerException.IsNotNull() ? String.Format("{0}{1}", exc.Message, exc.InnerException.Message) : exc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (Generic Exception): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.RecuperarPerfil(int), args(loginId: {1})", debugDetail, cpf));

                throw exc;
            }

            return possuiPerfilN1;
        }

        /// <summary>
        /// Funcao auxiliar para autenticacao de usuarios do modulo de estoque, validando se o mesmo possui perfil N1, no sistema.
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public int ObterLoginUsuarioID_UsuarioAtivo(string cpf)
        {
            int loginUsuarioID = -1;

            Expression<Func<TB_LOGIN, bool>> expWhere = null;
            Expression<Func<TB_PERFIL_LOGIN, bool>> expWhereTipoPerfil = null;
            IQueryable<TB_LOGIN> qryConsulta = null;

            try
            {
                expWhere = (loginUsuarioSAM => loginUsuarioSAM.TB_USUARIO.TB_USUARIO_CPF.Equals(cpf) && loginUsuarioSAM.TB_LOGIN_ATIVO == true);
                expWhereTipoPerfil = (perfilLogin => perfilLogin.TB_PERFIL_ID == (short)EnumPerfil.ADMINISTRADOR_ORGAO);
                qryConsulta = Db.TB_LOGIN.AsQueryable();

                loginUsuarioID = qryConsulta.Where(expWhere)
                                            .SelectMany(loginUsuarioSAM => loginUsuarioSAM.TB_PERFIL_LOGIN.Cast<TB_PERFIL_LOGIN>())
                                            .Where(expWhereTipoPerfil)
                                            .Select(loginUsuarioSAM => loginUsuarioSAM.TB_LOGIN_ID)
                                            .FirstOrDefault();
            }
            catch (DataException metaDataExc)
            {
                var debugDetail = (metaDataExc.InnerException.IsNotNull() ? String.Format("{0}{1}", metaDataExc.Message, metaDataExc.InnerException.Message) : metaDataExc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (DataException): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.ObterLoginUsuarioID_UsuarioAtivo(string), args(loginId: {1})", debugDetail, cpf));

                throw metaDataExc;
            }
            catch (Exception exc)
            {
                var debugDetail = (exc.InnerException.IsNotNull() ? String.Format("{0}{1}", exc.Message, exc.InnerException.Message) : exc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (Generic Exception): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.ObterLoginUsuarioID_UsuarioAtivo(string), args(loginId: {1})", debugDetail, cpf));

                throw exc;
            }

            return loginUsuarioID;
        }

        /// <summary>
        /// Funcao auxiliar para geração de relatorios do modulo de estoque, retornando CPF/Nome de usuario.
        /// </summary>
        /// <param name="usuarioSamID"></param>
        /// <returns></returns>
        public string ObterCPFNomeUsuarioSAM(int usuarioSamID)
        {
            string cpfNomeUsuarioSAM = null;

            IQueryable<TB_LOGIN> qryConsulta = null;
            Expression<Func<TB_LOGIN, bool>> expWhere = null;

            try
            {
                expWhere = (loginUsuarioSAM => loginUsuarioSAM.TB_LOGIN_ID == usuarioSamID);
                qryConsulta = Db.TB_LOGIN.AsQueryable();

                cpfNomeUsuarioSAM = qryConsulta.Where(expWhere)
                                               .Select(loginUsuarioSAM => String.Format("{0} - {1}", loginUsuarioSAM.TB_USUARIO.TB_USUARIO_CPF, loginUsuarioSAM.TB_USUARIO.TB_USUARIO_NOME_USUARIO))
                                               .FirstOrDefault();
            }
            catch (DataException metaDataExc)
            {
                var debugDetail = (metaDataExc.InnerException.IsNotNull() ? String.Format("{0}{1}", metaDataExc.Message, metaDataExc.InnerException.Message) : metaDataExc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (DataException): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.ObterCPFNomeUsuarioSAM(int), args(loginId: {1})", debugDetail, usuarioSamID));

                throw metaDataExc;
            }
            catch (Exception exc)
            {
                var debugDetail = (exc.InnerException.IsNotNull() ? String.Format("{0}{1}", exc.Message, exc.InnerException.Message) : exc.Message);
                LogErro.GravarStackTrace(String.Format("Erro Execução (Generic Exception): {0}\n\n Sam.Infrastructure.PerfilInfrastructureAntigo.ObterCPFNomeUsuarioSAM(int), args(loginId: {1})", debugDetail, usuarioSamID));

                throw exc;
            }

            return cpfNomeUsuarioSAM;
        }

        public IList<PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int LoginPerfilId, int PerfilId)
        {
            IList<PerfilLoginNivelAcesso> result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                                                    where a.TB_PERFIL_LOGIN_ID.Equals(LoginPerfilId)
                                                    where a.TB_PERFIL_LOGIN.TB_PERFIL_ID == PerfilId
                                                    select new PerfilLoginNivelAcesso
                                                    {
                                                        Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                                                        IdLoginNivelAcesso = a.TB_LOGIN_NIVEL_ACESSO_ID,
                                                        PerfilLoginId = a.TB_PERFIL_LOGIN_ID,
                                                        Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                                                        NivelAcesso = new NivelAcesso
                                                        {
                                                            Id = a.TB_NIVEL.TB_NIVEL_ID,
                                                            IdNivelAcesso = a.TB_NIVEL.TB_NIVEL_ID,
                                                            DescricaoNivel = a.TB_NIVEL.TB_NIVEL_DESCRICAO,
                                                            NivelId = a.TB_NIVEL.TB_NIVEL_ID,
                                                        }
                                                    }
                    ).ToList();

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            foreach (PerfilLoginNivelAcesso p in result)
            {
                // orgao
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.Orgao)
                {
                    OrgaoEntity orgao = estrutura.ListarOrgaosTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (orgao != null)
                    {
                        p.DescricaoValor = string.Format("{0} - {1}", orgao.Codigo.ToString().PadLeft(2, '0'), orgao.Descricao);
                        p.OrgaoId = orgao.Id;
                    }
                }
                // uo
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UO)
                {
                    UOEntity uo = estrutura.ListarUosTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (uo != null)
                    {
                        p.DescricaoValor = string.Format("{0} - {1}", uo.Codigo.ToString().PadLeft(5, '0'), uo.Descricao) ?? "";
                        p.OrgaoId = uo.Orgao.Id;
                        p.UoId = uo.Id;
                    }
                }
                // uge
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UGE)
                {
                    UGEEntity uge = estrutura.ListarUgesTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (uge != null)
                    {
                        p.DescricaoValor = string.Format("{0} - {1}", uge.Codigo.ToString().PadLeft(6, '0'), uge.Descricao);
                        p.OrgaoId = uge.Orgao.Id;
                        p.UoId = uge.Uo.Id;
                        p.UgeId = uge.Id;
                    }
                }
                // ua
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UA)
                {
                    UAEntity ua = estrutura.ListarUasTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (ua != null)
                    {
                        p.DescricaoValor = string.Format("{0} - {1}", ua.Codigo.ToString().PadLeft(5, '0'), ua.Descricao);
                        p.OrgaoId = ua.Orgao.Id;
                        p.UoId = ua.Uo.Id;
                        p.UgeId = ua.Uge.Id;
                        p.UaId = ua.Id;
                    }
                }
                // gestor
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.GESTOR)
                {
                    GestorEntity gestor = estrutura.ListarGestorTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (gestor != null)
                    {
                        p.DescricaoValor = gestor.Nome;
                        p.OrgaoId = gestor.Orgao.Id;
                        p.GestorId = gestor.Id;
                    }
                }
                // almoxarifado
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.ALMOXARIFADO)
                {
                    AlmoxarifadoEntity almox = estrutura.AlmoxarifadoTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (almox != null)
                    {
                        GestorEntity gestor = new EstruturaOrganizacionalBusiness().ListarGestorTodosCod().
                            Where(a => a.Id == almox.Gestor.Id).FirstOrDefault();

                        p.DescricaoValor = almox.Descricao;
                        p.OrgaoId = gestor.Orgao.Id;
                        p.AlmoxId = almox.Id;
                        p.GestorId = almox.Gestor.Id;
                    }
                }
                // divisão
                if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.DIVISAO)
                {
                    DivisaoEntity div = estrutura.ListarDivisaoTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    if (div != null)
                    {
                        AlmoxarifadoEntity almox = new EstruturaOrganizacionalBusiness().AlmoxarifadoTodosCod().
                            Where(a => a.Id == div.Almoxarifado.Id).FirstOrDefault();

                        GestorEntity gestor = new EstruturaOrganizacionalBusiness().ListarGestorTodosCod().
                            Where(a => a.Id == almox.Gestor.Id).FirstOrDefault();

                        p.DescricaoValor = div.Descricao;
                        p.DivisaoId = div.Id;
                        p.OrgaoId = gestor.Orgao.Id;
                        p.GestorId = gestor.Id;
                        p.AlmoxId = div.Almoxarifado.Id;
                    }
                }
            }

            return result;
        }

        public IList<PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcessoExportacao__(int OrgaoId, int GestorId)
        {
            try
            {
                IList<PerfilLoginNivelAcesso> result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                                                        join pl in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals pl.TB_PERFIL_LOGIN_ID
                                                        join l in Db.TB_LOGIN on pl.TB_LOGIN_ID equals l.TB_LOGIN_ID
                                                        join u in Db.TB_USUARIO on l.TB_USUARIO_ID equals u.TB_USUARIO_ID
                                                        join p in Db.TB_PERFIL on pl.TB_PERFIL_ID equals p.TB_PERFIL_ID
                                                        where u.TB_ORGAO_ID == OrgaoId && u.TB_GESTOR_ID == GestorId
                                                        && u.TB_USUARIO_IND_ATIVO == true && pl.TB_PERFIL_ID > 0
                                                        //where a.TB_PERFIL_LOGIN_ID.Equals(LoginPerfilId)
                                                        //where a.TB_PERFIL_LOGIN.TB_PERFIL_ID == PerfilId
                                                        select new PerfilLoginNivelAcesso
                                                        {
                                                            Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                                                            IdLoginNivelAcesso = a.TB_LOGIN_NIVEL_ACESSO_ID,
                                                            PerfilLoginId = a.TB_PERFIL_LOGIN_ID,
                                                            Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                                                            NomeUsuario = u.TB_USUARIO_NOME_USUARIO,
                                                            Cpf = u.TB_USUARIO_CPF,
                                                            PerfilDescricao = p.TB_PERFIL_DESCRICAO,
                                                            NivelAcesso = new NivelAcesso
                                                            {
                                                                Id = a.TB_NIVEL.TB_NIVEL_ID,
                                                                IdNivelAcesso = a.TB_NIVEL.TB_NIVEL_ID,
                                                                DescricaoNivel = a.TB_NIVEL.TB_NIVEL_DESCRICAO,
                                                                NivelId = a.TB_NIVEL.TB_NIVEL_ID,
                                                            }
                                                        }
                        ).ToList();

                EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
                foreach (PerfilLoginNivelAcesso p in result)
                {
                    // orgao
                    if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.Orgao)
                    {
                        OrgaoEntity orgao = estrutura.ListarOrgaosTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                        if (orgao != null)
                        {
                            p.DescricaoValor = string.Format("{0} - {1}", orgao.Codigo.ToString().PadLeft(2, '0'), orgao.Descricao);
                            p.OrgaoId = orgao.Id;
                        }
                    }

                    // uo
                    if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UO)
                    {
                        UOEntity uo = estrutura.ListarUosTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                        if (uo != null)
                        {
                            p.DescricaoValor = string.Format("{0} - {1}", uo.Codigo.ToString().PadLeft(5, '0'), uo.Descricao) ?? "";
                            p.OrgaoId = uo.Orgao.Id;
                            p.UoId = uo.Id;
                        }
                    }

                    // uge
                    if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UGE)
                    {
                        UGEEntity uge = estrutura.ListarUgesTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                        if (uge != null)
                        {
                            p.DescricaoValor = string.Format("{0} - {1}", uge.Codigo.ToString().PadLeft(6, '0'), uge.Descricao);
                            p.OrgaoId = uge.Orgao.Id;
                            p.UoId = uge.Uo.Id;
                            p.UgeId = uge.Id;
                        }
                    }

                    // ua
                    //if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UA)
                    //{
                    //    UAEntity ua = estrutura.ListarUasTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    //    if (ua != null)
                    //    {
                    //        p.DescricaoValor = string.Format("{0} - {1}", ua.Codigo.ToString().PadLeft(5, '0'), ua.Descricao);
                    //        p.OrgaoId = ua.Orgao.Id;
                    //        p.UoId = ua.Uo.Id;
                    //        p.UgeId = ua.Uge.Id;
                    //        p.UaId = ua.Id;
                    //    }
                    //}
                    //    // gestor
                    if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.GESTOR)
                    {
                        GestorEntity gestor = estrutura.ListarGestorTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                        if (gestor != null)
                        {
                            p.DescricaoValor = gestor.Nome;
                            p.OrgaoId = gestor.Orgao.Id;
                            p.GestorId = gestor.Id;
                        }
                    }
                    //    // almoxarifado
                    if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.ALMOXARIFADO)
                    {
                        AlmoxarifadoEntity almox = estrutura.AlmoxarifadoTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                        if (almox != null)
                        {
                            GestorEntity gestor = new EstruturaOrganizacionalBusiness().ListarGestorTodosCod().
                                Where(a => a.Id == almox.Gestor.Id).FirstOrDefault();

                            p.DescricaoValor = almox.Descricao;
                            p.OrgaoId = gestor.Orgao.Id;
                            p.AlmoxId = almox.Id;
                            p.GestorId = almox.Gestor.Id;
                        }
                    }
                    // divisão
                    //if (p.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.DIVISAO)
                    //{
                    //    DivisaoEntity div = estrutura.ListarDivisaoTodosCod().Where(a => a.Id == p.Valor).FirstOrDefault();
                    //    if (div != null)
                    //    {
                    //        AlmoxarifadoEntity almox = new EstruturaOrganizacionalBusiness().AlmoxarifadoTodosCod().
                    //            Where(a => a.Id == div.Almoxarifado.Id).FirstOrDefault();

                    //        GestorEntity gestor = new EstruturaOrganizacionalBusiness().ListarGestorTodosCod().
                    //            Where(a => a.Id == almox.Gestor.Id).FirstOrDefault();

                    //        p.DescricaoValor = div.Descricao;
                    //        p.DivisaoId = div.Id;
                    //        p.OrgaoId = gestor.Orgao.Id;
                    //        p.GestorId = gestor.Id;
                    //        p.AlmoxId = div.Almoxarifado.Id;
                    //    }
                    //}
                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Sam.Entity.Perfil> ListarPerfilLoginNivelAcessoExportacao(int? orgaoId, int? gestorId, int? UgeId, int? AlmozarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            EstruturaOrganizacionalBusiness eoBusiness = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity dtoAlmox = null;
            DivisaoEntity dtoDivisao = null;
            int almoxID = 0;
            string[] estruturaSAMInfos = null;
            string dadosUge = "";
            string prefixoTipoEstruturaOrganizacional = null;
            string prefixoEstruturaSAM = null;
            string descricaoTipoPerfil = null;
            char chrSeparador = ',';
            PerfilId = (PerfilId == -1 ? 0 : PerfilId);
            List<Sam.Entity.Perfil> result = null;

            try
            {
                result = ((from loginPerfil2 in this.Db.TB_PERFIL_LOGIN
                           join perfil in this.Db.TB_PERFIL on loginPerfil2.TB_PERFIL_ID equals perfil.TB_PERFIL_ID into indice_perfil
                           from job_perfil in indice_perfil.DefaultIfEmpty()
                           join lg in this.Db.TB_LOGIN on loginPerfil2.TB_LOGIN_ID equals lg.TB_LOGIN_ID into indice_usuariologin
                           from job_usuariologin in indice_usuariologin.DefaultIfEmpty()

                           join loginPerfil in this.Db.TB_PERFIL_LOGIN on job_usuariologin.TB_LOGIN_ID equals loginPerfil.TB_LOGIN_ID into indice_perfilogin
                           from job_perfilogin in indice_perfilogin.DefaultIfEmpty()
                           join acesso in this.Db.TB_LOGIN_NIVEL_ACESSO on job_perfilogin.TB_PERFIL_LOGIN_ID equals acesso.TB_PERFIL_LOGIN_ID
                           join user in this.Db.TB_USUARIO on job_usuariologin.TB_USUARIO_ID equals user.TB_USUARIO_ID

                           join vw_usuario in this.Db.VW_USUARIO_LOGIN_ACESSOS on acesso.TB_PERFIL_LOGIN_ID equals vw_usuario.TB_PERFIL_LOGIN_ID

                           into indice_wv_usuario
                           from job_wv_usuario in indice_wv_usuario.DefaultIfEmpty()

                           where (acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) || acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                           && (job_wv_usuario.TB_ORGAO_ID == orgaoId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.Orgao && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @orgaoId))
                           && (job_wv_usuario.TB_GESTOR_ID == gestorId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.GESTOR && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @gestorId))
                           && ((PerfilId == 0 && job_wv_usuario.RowId == 1) || (PerfilId > 0 && job_wv_usuario.TB_PERFIL_ID == PerfilId) || (job_perfil == null))
                           && user.TB_USUARIO_CPF != login
                           && ((PerfilId > 0 && job_perfil.TB_PERFIL_ID == PerfilId) || (PerfilId == 0))
                           && (user.TB_USUARIO_NOME_USUARIO.Contains(pesquisa)
                            || user.TB_USUARIO_CPF.Contains(pesquisa)
                            || user.TB_USUARIO_EMAIL.Contains(pesquisa)
                            || pesquisa == string.Empty
                          )
                           select new Sam.Entity.Perfil
                           {
                               Descricao = job_perfil.TB_PERFIL_DESCRICAO,
                               IdPerfil = job_perfil.TB_PERFIL_ID,
                               Cpf = job_wv_usuario.TB_USUARIO_CPF,
                               NomeUsuario = job_wv_usuario.TB_USUARIO_NOME_USUARIO,
                               OrgaoPadrao = new OrgaoEntity { Id = job_perfilogin.TB_ORGAO_ID_PADRAO ?? 0 },
                               GestorPadrao = new GestorEntity { Id = job_perfilogin.TB_GESTOR_ID_PADRAO ?? 0 },
                               AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = job_perfilogin.TB_ALMOXARIFADO_ID_PADRAO },
                               UOPadrao = new UOEntity { Id = job_perfilogin.TB_UO_ID_PADRAO ?? 0 },
                               UGEPadrao = new UGEEntity { Id = job_perfilogin.TB_UGE_ID_PADRAO ?? 0 },
                               UAPadrao = new UAEntity { Id = job_perfilogin.TB_UA_ID_PADRAO ?? 0 },
                               DivisaoPadrao = new DivisaoEntity { Id = job_perfilogin.TB_DIVISAO_ID_PADRAO ?? 0 },
                               PerfilLoginId = job_perfilogin.TB_PERFIL_LOGIN_ID
                           }).ToList()).OrderBy(a => a.NomeUsuario).ToList();

                //var teste = result;
                int almoxId = 0;
                int DivisaoId = 0;
                int perfilLoginId = 0;
                Sam.Entity.Perfil perfilTratado;
                string _estruturaSAMInfos;
                string _estruturaSAMInfosA;

                for (int i = 0; i < result.Count; i++)
                {
                    perfilTratado = result[i];
                    //Recupera do nivel de acesso
                    perfilLoginId = perfilTratado.PerfilLoginId;

                    almoxId = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                    a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.ALMOXARIFADO).Select(nivel => nivel.TB_LOGIN_NIVEL_ACESSO_VALOR).FirstOrDefault();

                    if (almoxId != 0)
                    {
                        var almoxList = RecuperarAlmox(almoxId);
                        if (almoxList.Count > 0)
                        {
                            perfilTratado.Descricao = perfilTratado.Descricao.ToString() + " - " + almoxList[0].AlmoxarifadoDescricao;
                        }
                    }

                    DivisaoId = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                    a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO).Select(nivel => nivel.TB_LOGIN_NIVEL_ACESSO_VALOR).FirstOrDefault();

                    if (DivisaoId > 0)
                    {
                        var div = DbSam.TB_DIVISAO.Where(a => a.TB_DIVISAO_ID == DivisaoId).FirstOrDefault();
                        if (div != null)
                        {
                            perfilTratado.DivisaoPadrao.Descricao = div.TB_DIVISAO_DESCRICAO;
                            perfilTratado.DivisaoPadrao.Id = div.TB_DIVISAO_ID;
                            almoxID = div.TB_ALMOXARIFADO_ID.Value;
                            perfilTratado.Descricao = perfilTratado.Descricao.ToString() + " - " + perfilTratado.DivisaoPadrao.Descricao;
                        }
                    }


                    if (perfilTratado.AlmoxarifadoPadrao.Id > 0 ||
                        perfilTratado.DivisaoPadrao.Id > 0)
                    {
                        perfilTratado.Descricao = perfilTratado.Descricao;
                        perfilTratado.IsAlmoxarifadoPadrao = true;
                    }

                    descricaoTipoPerfil = perfilTratado.Descricao.BreakLine("->", 0).Trim();

                    //UA
                    switch (perfilTratado.IdPerfil)
                    {
                        case (int)TipoPerfil.OperadorAlmoxarifado:
                        case (int)TipoPerfil.AdministradorGestor:
                        case (int)TipoPerfil.AdministradorOrgao:
                        case (int)TipoPerfil.Financeiro:
                        case (int)TipoPerfil.ConsultaRelatorio:
                        case (int)TipoPerfil.AdministradorFinanceiroSEFAZ:
                        case (int)TipoPerfil.AdministradorGeral:
                            if (almoxId != 0)
                            {
                                dtoAlmox = eoBusiness.ObterAlmoxarifado(almoxId);
                                estruturaSAMInfos = new string[] { dtoAlmox.Uge.Codigo.Value.ToString("D6"), dtoAlmox.Descricao };
                                perfilTratado.UGEPadrao.Id = dtoAlmox.Uge.Id;
                                prefixoTipoEstruturaOrganizacional = "UGE";
                                prefixoEstruturaSAM = "Almoxarifado";
                            }
                            break;
                        case (int)TipoPerfil.Requisitante:
                        case (int)TipoPerfil.RequisitanteGeral:
                            if (almoxID != 0)
                            {
                                dtoAlmox = eoBusiness.ObterAlmoxarifado(almoxID);
                                estruturaSAMInfos = new string[] { dtoAlmox.Uge.Codigo.Value.ToString("D6"), dtoAlmox.Descricao };
                                perfilTratado.UGEPadrao.Id = dtoAlmox.Uge.Id;
                                prefixoTipoEstruturaOrganizacional = "UGE";
                                prefixoEstruturaSAM = "Almoxarifado";
                            }
                            break;
                        default:
                            break;
                    }

                    //Divisão
                    switch (perfilTratado.IdPerfil)
                    {
                        case (int)TipoPerfil.Requisitante:
                            if (perfilTratado.DivisaoPadrao.Id.Value != 0)
                            {
                                eoBusiness.SelectDivisao(perfilTratado.DivisaoPadrao.Id.Value);
                                dtoDivisao = eoBusiness.Divisao;
                                perfilTratado.UGEPadrao.Id = eoBusiness.Divisao.Ua.Uge.Id;
                                estruturaSAMInfos = new string[] { dtoDivisao.Ua.Codigo.Value.ToString("D8"), dtoDivisao.Descricao, dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6"), dtoDivisao.Almoxarifado.Descricao };
                                dadosUge = dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6") + " - " + dtoDivisao.Ua.Uge.Descricao;
                                prefixoTipoEstruturaOrganizacional = String.Format(@"{0}: {1}, {2}: ""{3}"", {4}", "UGE", estruturaSAMInfos[2], "Almoxarifado", estruturaSAMInfos[3], "UA");
                                prefixoEstruturaSAM = "Divisão";
                            }

                            break;
                        case (int)TipoPerfil.RequisitanteGeral:
                            if (perfilTratado.DivisaoPadrao.Id.Value != 0)
                            {
                                eoBusiness.SelectDivisao(perfilTratado.DivisaoPadrao.Id.Value);
                                dtoDivisao = eoBusiness.Divisao;
                                perfilTratado.UGEPadrao.Id = eoBusiness.Divisao.Ua.Uge.Id;
                                estruturaSAMInfos = new string[] { dtoDivisao.Ua.Uge.Uo.Codigo.ToString("D5"), dtoDivisao.Ua.Uge.Uo.Descricao };

                                prefixoTipoEstruturaOrganizacional = "UO";
                                prefixoEstruturaSAM = "";
                                chrSeparador = '\0';
                            }
                            break;
                        default:
                            break;
                    }

                    _estruturaSAMInfos = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[0];
                    _estruturaSAMInfosA = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[1];
                    perfilTratado.Descricao = String.Format(@"{0} ({1}: {2}{3} {4}: ""{5}"")", descricaoTipoPerfil, prefixoTipoEstruturaOrganizacional ?? "", _estruturaSAMInfos, chrSeparador, prefixoEstruturaSAM ?? "", _estruturaSAMInfosA);

                    result[i] = perfilTratado;
                    perfilLoginId = 0;
                    _estruturaSAMInfos = null;
                    _estruturaSAMInfosA = null;
                }

                return result.Distinct().ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Sam.Entity.Perfil> BuscaComplementoDosUsuarioASeremMostradosNaTabela(List<Usuario> mostradosNaTabela, int? orgaoId, int? gestorId, int? UgeId, int? AlmozarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            EstruturaOrganizacionalBusiness eoBusiness = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity dtoAlmox = null;
            DivisaoEntity dtoDivisao = null;
            int almoxID = 0;
            string[] estruturaSAMInfos = null;
            string dadosUge = "";
            string prefixoTipoEstruturaOrganizacional = null;
            string prefixoEstruturaSAM = null;
            string descricaoTipoPerfil = null;
            char chrSeparador = ',';
            PerfilId = (PerfilId == -1 ? 0 : PerfilId);
            List<Sam.Entity.Perfil> result = null;

            try
            {
                if (PerfilId == 0)
                {
                    result = ((from loginPerfil in this.Db.TB_PERFIL_LOGIN

                               join lg in this.Db.TB_LOGIN on loginPerfil.TB_LOGIN_ID equals lg.TB_LOGIN_ID into indice_usuariologin
                               from job_usuariologin in indice_usuariologin.DefaultIfEmpty()

                               join perfil in this.Db.TB_PERFIL on loginPerfil.TB_PERFIL_ID equals perfil.TB_PERFIL_ID into indice_perfil
                               from job_perfil in indice_perfil.DefaultIfEmpty()

                               join acesso in this.Db.TB_LOGIN_NIVEL_ACESSO on loginPerfil.TB_PERFIL_LOGIN_ID equals acesso.TB_PERFIL_LOGIN_ID
                               join user in this.Db.TB_USUARIO on job_usuariologin.TB_USUARIO_ID equals user.TB_USUARIO_ID

                               join vw_usuario in this.Db.VW_USUARIO_LOGIN_ACESSOS on acesso.TB_PERFIL_LOGIN_ID equals vw_usuario.TB_PERFIL_LOGIN_ID

                               into indice_wv_usuario
                               from job_wv_usuario in indice_wv_usuario.DefaultIfEmpty()

                               where (acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) || acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                               && (job_wv_usuario.TB_ORGAO_ID == orgaoId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.Orgao && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @orgaoId))
                               && (job_wv_usuario.TB_GESTOR_ID == gestorId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.GESTOR && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @gestorId))
                               && ((PerfilId == 0 && job_wv_usuario.RowId == 1) || (PerfilId > 0 && job_wv_usuario.TB_PERFIL_ID == PerfilId) || (job_perfil == null))
                               && user.TB_USUARIO_CPF != login
                               && ((PerfilId > 0 && job_perfil.TB_PERFIL_ID == PerfilId) || (PerfilId == 0))
                               && (user.TB_USUARIO_NOME_USUARIO.Contains(pesquisa)
                                || user.TB_USUARIO_CPF.Contains(pesquisa)
                                || user.TB_USUARIO_EMAIL.Contains(pesquisa)
                                || pesquisa == string.Empty
                              )
                               select new Sam.Entity.Perfil
                               {
                                   Descricao = job_perfil.TB_PERFIL_DESCRICAO,
                                   IdPerfil = job_perfil.TB_PERFIL_ID,
                                   Cpf = job_wv_usuario.TB_USUARIO_CPF,
                                   NomeUsuario = job_wv_usuario.TB_USUARIO_NOME_USUARIO,
                                   OrgaoPadrao = new OrgaoEntity { Id = loginPerfil.TB_ORGAO_ID_PADRAO ?? 0 },
                                   GestorPadrao = new GestorEntity { Id = loginPerfil.TB_GESTOR_ID_PADRAO ?? 0 },
                                   AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = loginPerfil.TB_ALMOXARIFADO_ID_PADRAO },
                                   UOPadrao = new UOEntity { Id = loginPerfil.TB_UO_ID_PADRAO ?? 0 },
                                   UGEPadrao = new UGEEntity { Id = loginPerfil.TB_UGE_ID_PADRAO ?? 0 },
                                   UAPadrao = new UAEntity { Id = loginPerfil.TB_UA_ID_PADRAO ?? 0 },
                                   DivisaoPadrao = new DivisaoEntity { Id = loginPerfil.TB_DIVISAO_ID_PADRAO ?? 0 },
                                   PerfilLoginId = loginPerfil.TB_PERFIL_LOGIN_ID
                               }).ToList()).OrderBy(a => a.NomeUsuario).ToList();
                }
                else
                {
                    result = ((from loginPerfil2 in this.Db.TB_PERFIL_LOGIN
                               join perfil in this.Db.TB_PERFIL on loginPerfil2.TB_PERFIL_ID equals perfil.TB_PERFIL_ID into indice_perfil
                               from job_perfil in indice_perfil.DefaultIfEmpty()
                               join lg in this.Db.TB_LOGIN on loginPerfil2.TB_LOGIN_ID equals lg.TB_LOGIN_ID into indice_usuariologin
                               from job_usuariologin in indice_usuariologin.DefaultIfEmpty()

                               join loginPerfil in this.Db.TB_PERFIL_LOGIN on job_usuariologin.TB_LOGIN_ID equals loginPerfil.TB_LOGIN_ID into indice_perfilogin
                               from job_perfilogin in indice_perfilogin.DefaultIfEmpty()
                               join acesso in this.Db.TB_LOGIN_NIVEL_ACESSO on job_perfilogin.TB_PERFIL_LOGIN_ID equals acesso.TB_PERFIL_LOGIN_ID
                               join user in this.Db.TB_USUARIO on job_usuariologin.TB_USUARIO_ID equals user.TB_USUARIO_ID

                               join vw_usuario in this.Db.VW_USUARIO_LOGIN_ACESSOS on acesso.TB_PERFIL_LOGIN_ID equals vw_usuario.TB_PERFIL_LOGIN_ID

                               into indice_wv_usuario
                               from job_wv_usuario in indice_wv_usuario.DefaultIfEmpty()

                               where (acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) || acesso.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                               && (job_wv_usuario.TB_ORGAO_ID == orgaoId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.Orgao && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @orgaoId))
                               && (job_wv_usuario.TB_GESTOR_ID == gestorId || (acesso.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.GESTOR && acesso.TB_LOGIN_NIVEL_ACESSO_VALOR == @gestorId))
                               && ((PerfilId == 0 && job_wv_usuario.RowId == 1) || (PerfilId > 0 && job_wv_usuario.TB_PERFIL_ID == PerfilId) || (job_perfil == null))
                               && user.TB_USUARIO_CPF != login
                               && ((PerfilId > 0 && job_perfil.TB_PERFIL_ID == PerfilId) || (PerfilId == 0))
                               && (user.TB_USUARIO_NOME_USUARIO.Contains(pesquisa)
                                || user.TB_USUARIO_CPF.Contains(pesquisa)
                                || user.TB_USUARIO_EMAIL.Contains(pesquisa)
                                || pesquisa == string.Empty
                              )
                               select new Sam.Entity.Perfil
                               {
                                   Descricao = job_perfil.TB_PERFIL_DESCRICAO,
                                   IdPerfil = job_perfil.TB_PERFIL_ID,
                                   Cpf = job_wv_usuario.TB_USUARIO_CPF,
                                   NomeUsuario = job_wv_usuario.TB_USUARIO_NOME_USUARIO,
                                   OrgaoPadrao = new OrgaoEntity { Id = job_perfilogin.TB_ORGAO_ID_PADRAO ?? 0 },
                                   GestorPadrao = new GestorEntity { Id = job_perfilogin.TB_GESTOR_ID_PADRAO ?? 0 },
                                   AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = job_perfilogin.TB_ALMOXARIFADO_ID_PADRAO },
                                   UOPadrao = new UOEntity { Id = job_perfilogin.TB_UO_ID_PADRAO ?? 0 },
                                   UGEPadrao = new UGEEntity { Id = job_perfilogin.TB_UGE_ID_PADRAO ?? 0 },
                                   UAPadrao = new UAEntity { Id = job_perfilogin.TB_UA_ID_PADRAO ?? 0 },
                                   DivisaoPadrao = new DivisaoEntity { Id = job_perfilogin.TB_DIVISAO_ID_PADRAO ?? 0 },
                                   PerfilLoginId = job_perfilogin.TB_PERFIL_LOGIN_ID
                               }).ToList()).OrderBy(a => a.NomeUsuario).ToList();
                }

                var seraoMostradosNaTabelaEContemPerfil = mostradosNaTabela.Where(m => m.Login.Perfil != null).ToList();

                result = (from r in result
                          join res in seraoMostradosNaTabelaEContemPerfil on r.PerfilLoginId equals res.Login.Perfil.PerfilLoginId
                          select r).ToList();

                int almoxId = 0;
                int DivisaoId = 0;
                int perfilLoginId = 0;
                Sam.Entity.Perfil perfilTratado;
                string _estruturaSAMInfos;
                string _estruturaSAMInfosA;

                for (int i = 0; i < result.Count; i++)
                {
                    perfilTratado = result[i];
                    //Recupera do nivel de acesso
                    perfilLoginId = perfilTratado.PerfilLoginId;

                    almoxId = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                    a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.ALMOXARIFADO).Select(nivel => nivel.TB_LOGIN_NIVEL_ACESSO_VALOR).FirstOrDefault();

                    if (almoxId != 0)
                    {
                        var almoxList = RecuperarAlmox(almoxId);
                        if (almoxList.Count > 0)
                        {
                            perfilTratado.Descricao = perfilTratado.Descricao.ToString() + " - " + almoxList[0].AlmoxarifadoDescricao;
                        }
                    }

                    DivisaoId = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                    a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO).Select(nivel => nivel.TB_LOGIN_NIVEL_ACESSO_VALOR).FirstOrDefault();

                    if (DivisaoId > 0)
                    {
                        var div = DbSam.TB_DIVISAO.Where(a => a.TB_DIVISAO_ID == DivisaoId).FirstOrDefault();
                        if (div != null)
                        {
                            perfilTratado.DivisaoPadrao.Descricao = div.TB_DIVISAO_DESCRICAO;
                            perfilTratado.DivisaoPadrao.Id = div.TB_DIVISAO_ID;
                            almoxID = div.TB_ALMOXARIFADO_ID.Value;
                            perfilTratado.Descricao = perfilTratado.Descricao.ToString() + " - " + perfilTratado.DivisaoPadrao.Descricao;
                        }
                    }


                    if (perfilTratado.AlmoxarifadoPadrao.Id > 0 ||
                        perfilTratado.DivisaoPadrao.Id > 0)
                    {
                        perfilTratado.Descricao = perfilTratado.Descricao;
                        perfilTratado.IsAlmoxarifadoPadrao = true;
                    }

                    descricaoTipoPerfil = perfilTratado.Descricao.BreakLine("->", 0).Trim();

                    //UA
                    switch (perfilTratado.IdPerfil)
                    {
                        case (int)TipoPerfil.OperadorAlmoxarifado:
                        case (int)TipoPerfil.AdministradorGestor:
                        case (int)TipoPerfil.AdministradorOrgao:
                        case (int)TipoPerfil.Financeiro:
                        case (int)TipoPerfil.ConsultaRelatorio:
                        case (int)TipoPerfil.AdministradorFinanceiroSEFAZ:
                        case (int)TipoPerfil.AdministradorGeral:
                            if (almoxId != 0)
                            {
                                dtoAlmox = eoBusiness.ObterAlmoxarifado(almoxId);
                                estruturaSAMInfos = new string[] { dtoAlmox.Uge.Codigo.Value.ToString("D6"), dtoAlmox.Descricao };
                                perfilTratado.UGEPadrao.Id = dtoAlmox.Uge.Id;
                                prefixoTipoEstruturaOrganizacional = "UGE";
                                prefixoEstruturaSAM = "Almoxarifado";
                            }
                            break;
                        case (int)TipoPerfil.Requisitante:
                        case (int)TipoPerfil.RequisitanteGeral:
                            if (almoxID != 0)
                            {
                                dtoAlmox = eoBusiness.ObterAlmoxarifado(almoxID);
                                estruturaSAMInfos = new string[] { dtoAlmox.Uge.Codigo.Value.ToString("D6"), dtoAlmox.Descricao };
                                perfilTratado.UGEPadrao.Id = dtoAlmox.Uge.Id;
                                prefixoTipoEstruturaOrganizacional = "UGE";
                                prefixoEstruturaSAM = "Almoxarifado";
                            }
                            break;
                        default:
                            break;
                    }

                    //Divisão
                    switch (perfilTratado.IdPerfil)
                    {
                        case (int)TipoPerfil.Requisitante:
                            if (perfilTratado.DivisaoPadrao.Id.Value != 0)
                            {
                                eoBusiness.SelectDivisao(perfilTratado.DivisaoPadrao.Id.Value);
                                dtoDivisao = eoBusiness.Divisao;
                                perfilTratado.UGEPadrao.Id = eoBusiness.Divisao.Ua.Uge.Id;
                                estruturaSAMInfos = new string[] { dtoDivisao.Ua.Codigo.Value.ToString("D8"), dtoDivisao.Descricao, dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6"), dtoDivisao.Almoxarifado.Descricao };
                                dadosUge = dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6") + " - " + dtoDivisao.Ua.Uge.Descricao;
                                prefixoTipoEstruturaOrganizacional = String.Format(@"{0}: {1}, {2}: ""{3}"", {4}", "UGE", estruturaSAMInfos[2], "Almoxarifado", estruturaSAMInfos[3], "UA");
                                prefixoEstruturaSAM = "Divisão";
                            }

                            break;
                        case (int)TipoPerfil.RequisitanteGeral:
                            if (perfilTratado.DivisaoPadrao.Id.Value != 0)
                            {
                                eoBusiness.SelectDivisao(perfilTratado.DivisaoPadrao.Id.Value);
                                dtoDivisao = eoBusiness.Divisao;
                                perfilTratado.UGEPadrao.Id = eoBusiness.Divisao.Ua.Uge.Id;
                                estruturaSAMInfos = new string[] { dtoDivisao.Ua.Uge.Uo.Codigo.ToString("D5"), dtoDivisao.Ua.Uge.Uo.Descricao };

                                prefixoTipoEstruturaOrganizacional = "UO";
                                prefixoEstruturaSAM = "";
                                chrSeparador = '\0';
                            }
                            break;
                        default:
                            break;
                    }

                    _estruturaSAMInfos = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[0];
                    _estruturaSAMInfosA = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[1];
                    perfilTratado.Descricao = String.Format(@"{0} ({1}: {2}{3} {4}: ""{5}"")", descricaoTipoPerfil, prefixoTipoEstruturaOrganizacional ?? "", _estruturaSAMInfos, chrSeparador, prefixoEstruturaSAM ?? "", _estruturaSAMInfosA);

                    result[i] = perfilTratado;
                    perfilLoginId = 0;
                    _estruturaSAMInfos = null;
                    _estruturaSAMInfosA = null;
                }

                return result.Distinct().ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Salvar()
        {
            TB_PERFIL perfilTable = new TB_PERFIL();

            if (this.Entity.IdPerfil != 0)
                perfilTable = this.Db.TB_PERFIL.Where(a => a.TB_PERFIL_ID == this.Entity.IdPerfil).FirstOrDefault();
            else
                Db.TB_PERFIL.AddObject(perfilTable);
            perfilTable.TB_PERFIL_DESCRICAO = this.Entity.Descricao;
            perfilTable.TB_PERFIL_ATIVO = this.Entity.Ativo;
            this.Db.SaveChanges();

        }

        public List<Sam.Entity.Perfil> ListarPerfis()
        {
            // associar o gestor e o orgao 

            return (from a in Db.TB_PERFIL
                    orderby a.TB_PERFIL_DESCRICAO
                    select new Sam.Entity.Perfil
                    {
                        IdPerfil = a.TB_PERFIL_ID,
                        Descricao = a.TB_PERFIL_DESCRICAO
                    }
                    )
                    .ToList();
        }


        public List<Sam.Entity.Perfil> ListarPerfisPeso(int? Peso)
        {
            // associar o gestor e o orgao 
            int _idEnumPerfil = EnumPerfil.ADMINISTRADOR_GERAL.GetHashCode();

            return (from a in Db.TB_PERFIL
                    where a.TB_PERFIL_PESO < Peso || (a.TB_PERFIL_ID == _idEnumPerfil && a.TB_PERFIL_PESO <= Peso)
                    orderby a.TB_PERFIL_DESCRICAO
                    select new Sam.Entity.Perfil
                    {
                        IdPerfil = a.TB_PERFIL_ID,
                        Descricao = a.TB_PERFIL_DESCRICAO
                    }
                    )
                    .ToList();
        }

        public static void CriarTransacoes(List<Sam.Entity.Transacao> transacao
                                         , Sam.Entity.Perfil perfil
                                         , bool abrirTransacao)
        {
            throw new NotImplementedException();
        }

        public override void ComitarTransacao()
        {
            base.ComitarTransacao();
        }

        public override void RollbackTransacao()
        {
            base.RollbackTransacao();
        }


        public static void CriarPerfil(Sam.Entity.Perfil destino)
        {
            throw new NotImplementedException();
        }

        public static void InativarUsuario(string p)
        {
            throw new NotImplementedException();
        }

        public static void AtribuirPermissoes(List<Sam.Entity.Perfil> perfil, Sam.Entity.Usuario login)
        {
            throw new NotImplementedException();
        }

        public List<Sam.Entity.Perfil> RecuperarPerfil_NovoTeste(int? loginId)
        {
            List<Sam.Entity.Perfil> resposta = null;
            try
            {
                resposta = (from loginPerfil in Db.TB_PERFIL_LOGIN
                            join perfil in Db.TB_PERFIL on loginPerfil.TB_PERFIL_ID equals perfil.TB_PERFIL_ID
                            join lg in Db.TB_LOGIN on loginPerfil.TB_LOGIN_ID equals lg.TB_LOGIN_ID
                            join ac in Db.TB_LOGIN_NIVEL_ACESSO on loginPerfil.TB_PERFIL_LOGIN_ID equals ac.TB_PERFIL_LOGIN_ID
                            join usuario in Db.TB_USUARIO on lg.TB_USUARIO_ID equals usuario.TB_USUARIO_ID
                            where loginPerfil.TB_LOGIN_ID.Equals(loginId)
                            where (ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.ALMOXARIFADO) ||
                            ac.TB_NIVEL_ID.Equals((Byte)Common.NivelAcessoEnum.DIVISAO))
                            orderby perfil.TB_PERFIL_DESCRICAO
                            select
                                new
                                    Sam.Entity.Perfil
                                {
                                    Ativo = perfil.TB_PERFIL_ATIVO,
                                    Descricao = perfil.TB_PERFIL_DESCRICAO,
                                    IdPerfil = perfil.TB_PERFIL_ID,
                                    OrgaoPadrao = new OrgaoEntity { Id = loginPerfil.TB_ORGAO_ID_PADRAO ?? 0 },
                                    GestorPadrao = new GestorEntity { Id = loginPerfil.TB_GESTOR_ID_PADRAO ?? 0 },
                                    AlmoxarifadoPadrao = new AlmoxarifadoEntity { Id = loginPerfil.TB_ALMOXARIFADO_ID_PADRAO },
                                    UOPadrao = new UOEntity { Id = loginPerfil.TB_UO_ID_PADRAO ?? 0 },
                                    UGEPadrao = new UGEEntity { Id = loginPerfil.TB_UGE_ID_PADRAO ?? 0 },
                                    UAPadrao = new UAEntity { Id = loginPerfil.TB_UA_ID_PADRAO ?? 0 },
                                    DivisaoPadrao = new DivisaoEntity { Id = loginPerfil.TB_DIVISAO_ID_PADRAO ?? 0 },
                                    PerfilLoginId = loginPerfil.TB_PERFIL_LOGIN_ID,
                                    IdLogin = loginPerfil.TB_LOGIN_ID,
                                    Cpf = usuario.TB_USUARIO_CPF,
                                    NomeUsuario = usuario.TB_USUARIO_NOME_USUARIO
                                    //qNomeUsuario=usuario.TB_USUARIO_NOME_USUARIO,                               
                                    //Usuario = new Usuario
                                    //{
                                    //    NomeUsuario=usuario.TB_USUARIO_NOME_USUARIO,
                                    //    Cpf=usuario.TB_USUARIO_CPF
                                    //}

                                }).ToList();

                //for (int i = 0; i < resposta.Count; i++)
                //{
                //    //Recupera do nivel de acesso
                //    int perfilLoginId = resposta[i].PerfilLoginId;

                //    int almoxId = 0;
                //    var almox = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                //a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.ALMOXARIFADO).FirstOrDefault();

                //    if (almox != null)
                //        almoxId = almox.TB_LOGIN_NIVEL_ACESSO_VALOR;

                //    int DivisaoId = 0;
                //    var Divisao = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId &&
                //a.TB_NIVEL_ID == (Byte)Common.NivelAcessoEnum.DIVISAO).FirstOrDefault();

                //    if (Divisao != null)
                //        DivisaoId = Divisao.TB_LOGIN_NIVEL_ACESSO_VALOR;

                //    resposta[i].Almoxarifado = new AlmoxarifadoEntity(almoxId);
                //    resposta[i].Divisao = new DivisaoEntity(DivisaoId);

                //    if (resposta[i].AlmoxarifadoPadrao.Id > 0 ||
                //        resposta[i].DivisaoPadrao.Id > 0)
                //    {
                //        resposta[i].Descricao = "<font color=\"Red\">" + resposta[i].Descricao + " (Padrão)</font>";
                //        resposta[i].IsAlmoxarifadoPadrao = true;
                //    }

                //    string nomeAlmox = "";
                //    if (resposta[i].Almoxarifado.Id != 0)
                //    {
                //        var almoxList = RecuperarAlmox(Convert.ToInt32(resposta[i].Almoxarifado.Id));

                //        if (almoxList.Count > 0)
                //        {
                //            nomeAlmox = almoxList[0].AlmoxarifadoDescricao;
                //        }
                //    }
                //    else
                //    {
                //        if (DivisaoId > 0)
                //        {
                //            var div = DbSam.TB_DIVISAO.Where(a => a.TB_DIVISAO_ID == DivisaoId).FirstOrDefault();

                //            if (div != null)
                //            {
                //                resposta[i].DivisaoPadrao.Descricao = div.TB_DIVISAO_DESCRICAO;
                //                nomeAlmox = resposta[i].DivisaoPadrao.Descricao;
                //            }
                //        }
                //        else
                //        {
                //            nomeAlmox = "";
                //        }
                //    }
                //    resposta[i].Descricao = resposta[i].Descricao.ToString().PadRight(30) + " -> " + nomeAlmox;
                //}

                return resposta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
