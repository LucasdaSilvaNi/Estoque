using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common;
using Sam.Entity;
using Sam.Common.Util;
using Sam.Domain.Entity;

namespace Sam.Infrastructure
{
    public class EstruturasInfrastructure: BaseInfrastructure
    {

        public Sam.Entity.Estrutura BuscarEstruturas(int loginId)
        {

            List<NivelAcesso> nivelAcesso = (from perfil in Db.TB_PERFIL_LOGIN
                                                join perfilNivel in Db.TB_LOGIN_NIVEL_ACESSO
                                                    on perfil.TB_PERFIL_LOGIN_ID equals perfilNivel.TB_PERFIL_LOGIN_ID
                                                where perfil.TB_LOGIN_ID == loginId
                                                where perfil.TB_ORGAO_ID_PADRAO > 0
                                                orderby perfilNivel.TB_NIVEL_ID descending
                                               select new NivelAcesso
                                               {
                                                  IdNivelAcesso = perfilNivel.TB_LOGIN_NIVEL_ACESSO_ID
                                                , NivelId = perfilNivel.TB_NIVEL_ID
                                                , PerfilId = perfilNivel.TB_PERFIL_LOGIN_ID
                                                , ValorNivelAcesso = perfilNivel.TB_LOGIN_NIVEL_ACESSO_VALOR
                                                , Default = perfilNivel.TB_LOGIN_NIVEL_ACESSO_DEFAULT ?? false
                                               }).ToList();
            return MontaListaEstrutura(nivelAcesso);
        }

        public int ConsultaEstruturaNivel(int estruturaId, int nivelId)
        {

            int resultado = (from nivel in Db.TB_LOGIN_NIVEL_ACESSO
                                             where nivel.TB_LOGIN_NIVEL_ACESSO_VALOR == estruturaId
                                             where nivel.TB_NIVEL_ID == nivelId
                                             select new
                                             {
                                                 Id = nivel.TB_LOGIN_NIVEL_ACESSO_ID
                                             }).Count();


            return resultado;
        }

        private Sam.Entity.Estrutura MontaListaEstrutura(List<NivelAcesso> acessos)
        { 
            Estrutura resposta = new Estrutura();
            for(int i=0; i < 20; i++)
            {
                Dictionary<int, bool> nivelAcessos =
                        acessos.Where(ac => ac.NivelId == i)
                        .Select(ac => new { ac.ValorNivelAcesso, ac.Default})
                        .ToDictionary(ac => ac.ValorNivelAcesso, ac => ac.Default);

                if (nivelAcessos.Count > 0)
                    this.ListaEstruturaSam(Convert.ToInt16(i)
                                    , nivelAcessos
                                    , ref resposta);
            }

            return resposta;
        }
        
        private void ListaEstruturaSam(int nivelAcesso
                                        , Dictionary<int, bool> valorNivel
                                        , ref Estrutura estrutura)
        {
            string nivel = nivelAcesso.ToString();
            
                if(nivel == NivelAcessoEnum.Orgao.ToString())
                    estrutura.Orgao = this.ListarOrgaos(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.UO.ToString())
                    estrutura.Uo = this.ListarUo(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.UGE.ToString())
                    estrutura.Uge = this.ListarUge(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.UA.ToString())
                    estrutura.Ua = this.ListarUa(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.DIVISAO.ToString())
                    estrutura.Divisao = this.ListarDivisao(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.GESTOR.ToString())
                    estrutura.Gestor = this.ListarGestor(valorNivel);
                    
                if(nivel == Sam.Common.NivelAcessoEnum.ALMOXARIFADO.ToString())
                    estrutura.Almoxarifado = this.ListarAlmoxarifado(valorNivel);
                    
            }

            //Sam.Entity.Estrutura estrutura = (from orgao in DbSam.TB_ORGAO 
            //                join uo in DbSam.TB_UO
            //                    on orgao.TB_ORGAO_ID equals uo.TB_ORGAO_ID into uoLeft
            //                    from uo in uoLeft.DefaultIfEmpty()
            //                join uge in DbSam.TB_UGE
            //                    on uo.TB_UO_ID equals uge.TB_UO_ID into ugeLeft
            //                from uge in ugeLeft.DefaultIfEmpty()
            //                join ua in DbSam.TB_UA
            //                    on uge.TB_UGE_ID equals ua.TB_UGE_ID into uaLeft
            //                from ua in uaLeft.DefaultIfEmpty()
            //                join divisao in DbSam.TB_DIVISAO
            //                    on ua.TB_UA_ID equals divisao.TB_DIVISAO_ID into divisaoLeft
            //                from divisao in divisaoLeft.DefaultIfEmpty()
            //                join gestor in DbSam.TB_GESTOR 
            //                    on orgao.TB_ORGAO_ID equals gestor.TB_GESTOR_ID into gestLeft
            //                from gestor in gestLeft.DefaultIfEmpty()
            //                join alm in DbSam.TB_ALMOXARIFADO
            //                    on gestor.TB_GESTOR_ID equals alm.TB_GESTOR_ID into almLeft
            //                from alm in almLeft.DefaultIfEmpty()
            //                where ((divisaoId == 0) || (divisao.TB_DIVISAO_ID == divisaoId))
            //                  && ((orgaoId == 0) || (orgao.TB_ORGAO_ID == orgaoId))
            //                  && ((uoId == 0) || (uo.TB_UO_ID == uoId))
            //                  &&  ((ugeId == 0)|| (uge.TB_UGE_ID == ugeId))
            //                  && ((ugeId == 0) || (uge.TB_UGE_ID == ugeId))
            //                  && ((uaId == 0)    || (ua.TB_UA_ID == uaId))
            //                  && ((gestorId == 0) || (gestor.TB_GESTOR_ID == gestorId))
            //                  && ((almoxarifadoId == 0) || (alm.TB_ALMOXARIFADO_ID == almoxarifadoId))
            //                select new Estrutura 
            //                    {Orgao = new Orgao {OrgaoId = orgao.TB_ORGAO_ID
            //                                      , OrgaoDescricao = orgao.TB_ORGAO_DESCRICAO},
            //                     Uo = new Uo { UoId = uo.TB_UO_ID
            //                                 , UoDescricao = uo.TB_UO_DESCRICAO},
            //                     Uge = new Uge{ UgeId = uge.TB_UGE_ID
            //                                    ,   UgeDescricao = uge.TB_UGE_DESCRICAO},
            //                     Ua = new Ua{ UaId = ua.TB_UA_ID
            //                                ,  UaDescricao = ua.TB_UA_DESCRICAO},
            //                    Divisao = new Divisao{DivisaoId = divisao.TB_DIVISAO_ID
            //                                          , DivisaoDescricao = divisao.TB_DIVISAO_DESCRICAO},
            //                    Gestor = new Gestor{ GestorId = gestor.TB_GESTOR_ID
            //                                        , GestorDescricao = gestor.TB_GESTOR_NOME},
            //                    Almoxarifado = new Almoxarifado{   AlmoxarifadoId = alm.TB_ALMOXARIFADO_ID
            //                                                     , AlmoxarifadoDescricao = alm.TB_ALMOXARIFADO_DESCRICAO}}).FirstOrDefault();
        //}

        #region Estruturas

        private List<OrgaoEntity> ListarOrgaos(int orgaoId)
        {
            return (DbSam.TB_ORGAO.Where(t => ((orgaoId==0) || t.TB_ORGAO_ID == orgaoId))
                    .Select(t=> new OrgaoEntity(){ Id = t.TB_ORGAO_ID
                            , Codigo = t.TB_ORGAO_CODIGO
                            , Descricao = t.TB_ORGAO_DESCRICAO})).ToList();
        }

        private List<OrgaoEntity> ListarOrgaos(Dictionary<int, bool> orgaoId)
        {
            List<OrgaoEntity> resposta = new List<OrgaoEntity>();
            OrgaoEntity orgao = null;
            int orgaoValue = default(int);
            bool orgaoDefault= default(bool);

            
            foreach (KeyValuePair<int, bool> dicionario in orgaoId)
            {
                orgao = new OrgaoEntity();

                orgaoValue = dicionario.Key;
                orgaoDefault = dicionario.Value;

                orgao = (from org in DbSam.TB_ORGAO
                         where org.TB_ORGAO_ID.Equals(orgaoValue)
                   select new OrgaoEntity
                    {
                         Id = org.TB_ORGAO_ID
                        , Codigo = org.TB_ORGAO_CODIGO
                        , Descricao = org.TB_ORGAO_DESCRICAO
                        , OrgaoDefault = orgaoDefault
                    }).FirstOrDefault();
                
                if(orgao != null)
                    resposta.Add(orgao);
            }

            return resposta;

        }

        private List<UOEntity> ListarUo(Dictionary<int, bool> uoId)
        {
            List<UOEntity> resposta =  new List<UOEntity>();
            UOEntity uo = null;
            int uoValue = default(int);
            bool uoDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in uoId)
            {
                uo = new UOEntity();

                uoValue = dicionario.Key;
                uoDefault = dicionario.Value;

                uo = (DbSam.TB_UO.Where(org => org.TB_UO_ID.Equals(uoValue)).ToList()
                .Select(org => new UOEntity
                {
                    Id = org.TB_UO_ID
                    ,Codigo =org.TB_UO_CODIGO
                    ,Descricao =org.TB_UO_DESCRICAO
                    ,Orgao = (new OrgaoEntity(org.TB_ORGAO_ID))
                    ,UoDefault = uoDefault
                })).FirstOrDefault();
                
                if(uo != null)
                    resposta.Add(uo);
            }

            return resposta;
        }

        private List<UGEEntity> ListarUge(Dictionary<int, bool> ugeId)
        {
           List<UGEEntity> resposta = new List<UGEEntity>();
           UGEEntity uge = null;
            int ugeValue = default(int);
            bool ugeDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in ugeId)
            {
                uge = new UGEEntity();

                ugeValue = dicionario.Key;
                ugeDefault = dicionario.Value;

                uge =  (DbSam.TB_UGE.Where(org => org.TB_UGE_ID.Equals(ugeValue)).ToList()
                .Select(org => new UGEEntity
                {
                    Id = org.TB_UGE_ID
                    ,Codigo =org.TB_UGE_CODIGO
                    ,Descricao =org.TB_UGE_DESCRICAO
                    ,Uo =(new UOEntity(org.TB_UO_ID))
                    , UgeDefault = ugeDefault

                })).FirstOrDefault();

                if(uge != null)
                    resposta.Add(uge);
            }

            return resposta;
        }

        private List<UAEntity> ListarUa(Dictionary<int, bool> uaId)
        {
            List<UAEntity> resposta = new List<UAEntity>();
            UAEntity ua = null;
            int uaValue = default(int);
            bool uaDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in uaId)
            {
                ua = new UAEntity();

                uaValue = dicionario.Key;
                uaDefault = dicionario.Value;

                ua =  (DbSam.TB_UA.Where(org => org.TB_UA_ID.Equals(uaValue)).ToList()
                .Select(org => new UAEntity
                    {
                         Id = org.TB_UA_ID
                         ,Codigo =org.TB_UA_CODIGO
                         ,Descricao =org.TB_UA_DESCRICAO
                         ,Uge =(new UGEEntity(org.TB_UGE_ID))
                         ,Unidade = (new UnidadeEntity(org.TB_UNIDADE_ID))
                         ,UaDefault = uaDefault

                    })).FirstOrDefault();
                if(ua != null)
                    resposta.Add(ua);
            }

            return resposta;
        }

        private List<DivisaoEntity> ListarDivisao(Dictionary<int, bool>  divisaoId)
        {
            List<DivisaoEntity> resposta = new List<DivisaoEntity>();
            DivisaoEntity divisao = null;
            int divisaoValue = default(int);
            bool divisaoDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in divisaoId)
            {
                divisao = new DivisaoEntity();

                divisaoValue = dicionario.Key;
                divisaoDefault = dicionario.Value;

                divisao =   (DbSam.TB_DIVISAO.Where(org => org.TB_DIVISAO_ID.Equals(divisaoValue)).ToList()
                .Select(org => new DivisaoEntity
                {
                    Id =org.TB_DIVISAO_ID
                    ,Codigo =org.TB_DIVISAO_CODIGO
                    ,Descricao=org.TB_DIVISAO_DESCRICAO
                    ,Almoxarifado=(new AlmoxarifadoEntity(org.TB_ALMOXARIFADO_ID))
                    ,Ua = (new UAEntity(org.TB_UA_ID))
                    ,DivisaoDefault = divisaoDefault

                })).FirstOrDefault();

                if(divisao != null)
                    resposta.Add(divisao);
            }

            return resposta;
        }

        private List<GestorEntity> ListarGestor(Dictionary<int, bool> gestorId)
        {
            List<GestorEntity> resposta = new List<GestorEntity>();
            GestorEntity gestor = null;
            int gestorValue = default(int);
            bool gestorDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in gestorId)
            {
                gestor = new GestorEntity();

                gestorValue = dicionario.Key;
                gestorDefault = dicionario.Value;

                gestor = (DbSam.TB_GESTOR.Where(org => org.TB_GESTOR_ID.Equals(gestorValue)).ToList()
                    .Select(org => new GestorEntity()
                        {
                            Id =org.TB_GESTOR_ID
                            ,CodigoGestao = org.TB_GESTOR_CODIGO_GESTAO
                                ,Nome =org.TB_GESTOR_NOME
                                ,NomeReduzido=org.TB_GESTOR_NOME_REDUZIDO
                                ,Orgao =(new OrgaoEntity(org.TB_ORGAO_ID))
                                ,Uge =(new UGEEntity(org.TB_UGE_ID))
                                ,Uo = (new UOEntity(org.TB_UO_ID))
                            ,GestorDefault = gestorDefault

                        })).FirstOrDefault();
                if(gestor != null)
                    resposta.Add(gestor);
            }

            return resposta;
        }

        private List<AlmoxarifadoEntity> ListarAlmoxarifado(Dictionary<int, bool> almoxarifadoId)
        {
            List<AlmoxarifadoEntity> resposta = new List<AlmoxarifadoEntity>();
            AlmoxarifadoEntity almoxarifado = null;
            int almoxarifadoValue = default(int);
            bool almoxarifadoDefault= default(bool);
            
            foreach (KeyValuePair<int, bool> dicionario in almoxarifadoId)
            {
                almoxarifado = new AlmoxarifadoEntity();

                almoxarifadoValue = dicionario.Key;
                almoxarifadoDefault = dicionario.Value;

                almoxarifado =  (DbSam.TB_ALMOXARIFADO.Where(org => org.TB_ALMOXARIFADO_ID.Equals(almoxarifadoValue)).ToList()
                .Select(org => new AlmoxarifadoEntity
                           {
                               Id = org.TB_ALMOXARIFADO_ID
                               ,Descricao = org.TB_ALMOXARIFADO_DESCRICAO
                               ,Gestor = new GestorEntity(org.TB_GESTOR_ID)
                               ,AlmoxDefault = almoxarifadoDefault

                           })).FirstOrDefault();
                if(almoxarifado != null)
                    resposta.Add(almoxarifado);
            }

            return resposta;
        }
        
        #endregion

        #region Orgão
        private List<OrgaoEntity> ListarOrgaos()
        {
            return this.ListarOrgaos(0);
        }
        #endregion

    }
}
