using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;


namespace Sam.Domain.Infrastructure
{
    public class UAInfraestructure : BaseInfraestructure, IUAService
    {
        private UAEntity ua = new UAEntity();

        public UAEntity Entity
        {
            get { return ua; }
            set { ua = value; }
        }

        public void Excluir()
        {
            TB_UA tbUA
                = this.Db.TB_UAs.Where(a => a.TB_UA_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UAs.DeleteOnSubmit(tbUA);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            //TB_UA tbUA = new TB_UA();
            var tbUA = new TB_UA();

            if (this.Entity.Id.HasValue)
            {
                tbUA = this.Db.TB_UAs.Where(a => a.TB_UA_ID == this.Entity.Id.Value).FirstOrDefault();
            }
            else
                Db.TB_UAs.InsertOnSubmit(tbUA);

            tbUA.TB_UA_CODIGO = this.Entity.Codigo.Value;
            tbUA.TB_UGE_ID = this.Entity.Uge.Id.Value;

            if (Entity.Gestor.IsNotNull() && Entity.Gestor.Id.HasValue)
                    tbUA.TB_GESTOR_ID = Entity.Gestor.Id.Value;
            
            if (this.Entity.Unidade.Id.HasValue)
                tbUA.TB_UNIDADE_ID = this.Entity.Unidade.Id.Value;
            
            if (this.Entity.CentroCusto.Id.HasValue)
                tbUA.TB_CENTRO_CUSTO_ID = this.Entity.CentroCusto.Id.Value;

            tbUA.TB_UA_DESCRICAO = this.Entity.Descricao;
            tbUA.TB_UA_VINCULADA = this.Entity.UaVinculada;
            tbUA.TB_UA_INDICADOR_ATIVIDADE = this.Entity.IndicadorAtividade;

            this.Db.SubmitChanges();
        }
        
        public IList<UAEntity> Listar()
        {
            IList<UAEntity> resultado = (from a in Db.TB_UAs
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(a.TB_UGE.TB_UGE_ID)),
                                             Uo = new UOEntity(a.TB_UGE.TB_UO.TB_UO_ID),
                                             Gestor = new GestorEntity(a.TB_GESTOR.TB_GESTOR_ID)
                                         }).Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<UAEntity>();

            this.totalregistros = (from a in Db.TB_UAs
                                   select new
                                   {
                                       id = a.TB_UA_ID,
                                   }).Count();
            return resultado;
        }

        public IList<UAEntity> ListarTodas()
        {
            IList<UAEntity> resultado = (from a in Db.TB_UAs
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(a.TB_UGE.TB_UGE_ID)),
                                             Uo = new UOEntity(a.TB_UGE.TB_UO.TB_UO_ID),
                                             Gestor = new GestorEntity(a.TB_GESTOR.TB_GESTOR_ID)
                                         }).ToList<UAEntity>();
            return resultado;
        }

        public IList<UAEntity> Imprimir()
        {
            //IList<UAEntity> resultado = (from a in this.Db.VW_UAs
            //                             orderby (a.TB_UA_ID)
            //                             select new UAEntity
            //                             {
            //                                 Id = a.TB_UA_ID,
            //                                 Codigo = a.TB_UA_CODIGO,
            //                                 Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
            //                                 Descricao = a.TB_UA_DESCRICAO,
            //                                 UaVinculada = a.TB_UA_VINCULADA,
            //                                 CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
            //                                 IndicadorAtividade = (new IndicadorAtividadeEntity(a.TB_INDICADOR_ATIVIDADE_ID)),
            //                                 Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                 Uge = (new UGEEntity(a.TB_UGE_ID)),
            //                                 Ordem = a.ORDEM
            //                             }).ToList<UAEntity>();
            //return resultado;

            return new List<UAEntity>();
        }

        public IList<UAEntity> Imprimir(int UgeId)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs.DefaultIfEmpty()
                                         join u in this.Db.TB_UNIDADEs.DefaultIfEmpty() on a.TB_UNIDADE_ID equals u.TB_UNIDADE_ID
                                         into b from u in b.DefaultIfEmpty()
                                         join c in this.Db.TB_CENTRO_CUSTOs on a.TB_CENTRO_CUSTO_ID equals c.TB_CENTRO_CUSTO_ID
                                         into d from c in d.DefaultIfEmpty()
                                         where a.TB_UGE_ID == UgeId                                         
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity
                                             {
                                                 Id = a.TB_UNIDADE_ID,
                                                 Descricao = u.TB_UNIDADE_DESCRICAO
                                             }),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity { Id = a.TB_CENTRO_CUSTO_ID, Descricao = c.TB_CENTRO_CUSTO_DESCRICAO }),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Uge = (new UGEEntity(a.TB_UGE_ID))
                                         }).Distinct().OrderBy(a => a.Descricao).ToList<UAEntity>();
            return resultado;
        }

        public IList<UAEntity> ListarPorUGE(int UGEId)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in this.Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in this.Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                         join org in this.Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         where (a.TB_UGE_ID == UGEId)
                                         orderby a.TB_UA_DESCRICAO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(org.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(a.TB_UGE_ID))
                                         }).OrderBy(a => a.Descricao)
                                         .Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<UAEntity>();

            this.totalregistros = (from a in Db.TB_UAs
                                   join uge in this.Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                   join uo in this.Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                   join org in this.Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (a.TB_UGE_ID == UGEId)
                                   orderby a.TB_UA_CODIGO
                                   select new
                                   {
                                       id = a.TB_UA_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UAEntity> ListarPorUGE(int UGEId, int UACodigo)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in this.Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in this.Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                         join org in this.Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         where (   a.TB_UGE_ID == UGEId
                                                && a.TB_UA_CODIGO == UACodigo)
                                         orderby a.TB_UA_DESCRICAO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(org.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(a.TB_UGE_ID))
                                         }).OrderBy(a => a.Descricao)
                                         .Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<UAEntity>();

            this.totalregistros = resultado.Count();

            return resultado;
        }

        public IList<UAEntity> ListarPorOrgao(int OrgaoId)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID 
                                         join org in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         where (org.TB_ORGAO_ID == OrgaoId)
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(org.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(uge.TB_UGE_ID))
                                         })
                                         .Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<UAEntity>();

            this.totalregistros = (from a in this.Db.TB_UAs
                                   join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                   join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID 
                                   join org in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (org.TB_ORGAO_ID == OrgaoId)
                                   orderby a.TB_UA_CODIGO
                                   select new
                                   {
                                       id = a.TB_UA_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UAEntity> ListarUasTodosCod(int? OrgaoId) 
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_UO_ID
                                         join org in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         where (org.TB_ORGAO_ID == OrgaoId)
                                         where (a.TB_UA_INDICADOR_ATIVIDADE == true)
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Uge = new UGEEntity
                                             {
                                                 Id = a.TB_UGE.TB_UGE_ID,
                                                 Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                             },
                                             Uo = new UOEntity
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_UO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                             },
                                             Orgao = new OrgaoEntity 
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                             }
                                         })
                                         .ToList();

            return resultado;
        }

        public IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         where (uge.TB_UGE_ID == UgeId)
                                         where (a.TB_UA_INDICADOR_ATIVIDADE == true)
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Uge = new UGEEntity
                                             {
                                                 Id = a.TB_UGE.TB_UGE_ID,
                                                 Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                             },
                                             Uo = new UOEntity
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_UO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                             },
                                             Orgao = new OrgaoEntity
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                             }
                                         })
                                         .ToList();

            return resultado;
        }

        public IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId, IList<DivisaoEntity> divisaoList)
        {
            List<UAEntity> resultadoCompleto = new List<UAEntity>();
              IList<UAEntity> resultado = null;

              foreach (var divisao in divisaoList)
              {
                  resultado = (from a in this.Db.TB_UAs
                               join div in Db.TB_DIVISAOs on a.TB_UA_ID equals div.TB_UA_ID
                               where (a.TB_UGE_ID == UgeId)
                               where div.TB_DIVISAO_ID == divisao.Id
                               where a.TB_UA_INDICADOR_ATIVIDADE == true
                               orderby a.TB_UA_CODIGO
                               select new UAEntity
                               {
                                   Id = a.TB_UA_ID,
                                   Codigo = a.TB_UA_CODIGO,
                                   Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                   Descricao = a.TB_UA_DESCRICAO,
                                   UaVinculada = a.TB_UA_VINCULADA,
                                   CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                   IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                  
                               }).ToList();

                  foreach (var res in resultado)
                  {
                      if (resultadoCompleto.Where(a => a.Id == res.Id).Count() == 0)
                          resultadoCompleto.Add(res);
                  }
              }

            return resultadoCompleto;
        }

        public IList<UAEntity> ListarUasTodosCodPorUo(int? UoId)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         where (a.TB_UGE.TB_UO.TB_UO_ID == UoId)
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Uge = new UGEEntity
                                             {
                                                 Id = a.TB_UGE.TB_UGE_ID,
                                                 Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                             },
                                             Uo = new UOEntity
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_UO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                             },
                                             Orgao = new OrgaoEntity
                                             {
                                                 Id = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                 Codigo = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                 Descricao = a.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                             }
                                         })
                                         .ToList();

            return resultado;
        }

        public IList<UAEntity> ListarUAsPorUO(int codigoUO)
        {
            IList<UAEntity> lstRetorno = (from uaSAM in this.Db.TB_UAs
                                         where (uaSAM.TB_UGE.TB_UO.TB_UO_CODIGO == codigoUO)
                                         orderby uaSAM.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = uaSAM.TB_UA_ID,
                                             Codigo = uaSAM.TB_UA_CODIGO,
                                             Descricao = uaSAM.TB_UA_DESCRICAO,
                                             Uge = new UGEEntity
                                                                 {
                                                                     Id = uaSAM.TB_UGE.TB_UGE_ID,
                                                                     Codigo = uaSAM.TB_UGE.TB_UGE_CODIGO,
                                                                     Descricao = uaSAM.TB_UGE.TB_UGE_DESCRICAO
                                                                 },
                                             Uo = new UOEntity
                                                                 {
                                                                     Id = uaSAM.TB_UGE.TB_UO.TB_UO_ID,
                                                                     Codigo = uaSAM.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                                     Descricao = uaSAM.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                                                 },
                                             Orgao = new OrgaoEntity
                                                                     {
                                                                         Id = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                                         Codigo = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                                         Descricao = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                                                     }
                                         })
                                           .ToList();

            return lstRetorno;
        }

        public IList<UAEntity> ListarUAsPorUGE(int codigoUGE)
        {
            IList<UAEntity> lstRetorno = (from uaSAM in this.Db.TB_UAs
                                          where (uaSAM.TB_UGE.TB_UGE_CODIGO == codigoUGE)
                                          orderby uaSAM.TB_UA_CODIGO
                                          select new UAEntity
                                          {
                                              Id = uaSAM.TB_UA_ID,
                                              Codigo = uaSAM.TB_UA_CODIGO,
                                              Descricao = uaSAM.TB_UA_DESCRICAO,
                                              Uge = new UGEEntity
                                              {
                                                  Id = uaSAM.TB_UGE.TB_UGE_ID,
                                                  Codigo = uaSAM.TB_UGE.TB_UGE_CODIGO,
                                                  Descricao = uaSAM.TB_UGE.TB_UGE_DESCRICAO
                                              },
                                              Uo = new UOEntity
                                              {
                                                  Id = uaSAM.TB_UGE.TB_UO.TB_UO_ID,
                                                  Codigo = uaSAM.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                  Descricao = uaSAM.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                              },
                                              Orgao = new OrgaoEntity
                                              {
                                                  Id = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                  Codigo = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                  Descricao = uaSAM.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                              }
                                          })
                                           .ToList();

            return lstRetorno;
        }

        public IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almox)
        {
            IList<UAEntity> resultado = (from divisao in Db.TB_DIVISAOs
                                            where (divisao.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almox.Id)
                                            where divisao.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID == almox.Gestor.Id.Value

                                            orderby divisao.TB_UA.TB_UA_CODIGO
                                            select new UAEntity
                                            {
                                                Id = divisao.TB_UA.TB_UA_ID,
                                                Codigo = divisao.TB_UA.TB_UA_CODIGO,
                                                Unidade = (new UnidadeEntity(divisao.TB_UA.TB_UNIDADE_ID)),
                                                Descricao = divisao.TB_UA.TB_UA_DESCRICAO,
                                                UaVinculada = divisao.TB_UA.TB_UA_VINCULADA,
                                                CentroCusto = (new CentroCustoEntity(divisao.TB_UA.TB_CENTRO_CUSTO_ID)),
                                                IndicadorAtividade = divisao.TB_UA.TB_UA_INDICADOR_ATIVIDADE,
                                                Uge = new UGEEntity
                                                {
                                                    Id = divisao.TB_UA.TB_UGE.TB_UGE_ID,
                                                    Codigo = divisao.TB_UA.TB_UGE.TB_UGE_CODIGO,
                                                    //Descricao = divisao.TB_UA.TB_UGE.TB_UGE_DESCRICAO                                                 
                                                    Descricao = divisao.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                                },
                                                Uo = new UOEntity
                                                {
                                                    Id = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                                    Codigo = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                    //Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                                    Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                                },
                                                Orgao = new OrgaoEntity
                                                {
                                                    Id = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                                    Codigo = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                                    //Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                                    Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO,
                                                }
                                            })
                                            .ToList();

            resultado = resultado.DistinctBy(UA => UA.Id).ToList();

            return resultado;
        }

        /// <summary>
        /// Sobrecarga criada para evitar alterar o comportamento do método antigo, este método foi criado para
        /// ser utilizado na tela Chamado Suporte para resolver o problema de usuários que não estavam conseguindo
        /// abrir chamados
        /// </summary>
        /// <param name="almox">Almoxarifado</param>
        /// <param name="mostraDivisaoEspecial">True - Para comportamento antido do método / False - Para novo comportamento (Específico do Chamado Suporte)</param>
        /// <returns></returns>
        public IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almox, bool mostraDivisaoEspecial)
        {
            IList<UAEntity> resultado;

            if (mostraDivisaoEspecial)
                resultado = ListarUasTodosCodPorAlmoxarifado(almox);
            else
                resultado = (from divisao in Db.TB_DIVISAOs
                                join ua in Db.TB_UAs on divisao.TB_UA_ID equals ua.TB_UA_ID
                                join uge in Db.TB_UGEs on ua.TB_UGE_ID equals uge.TB_UGE_ID
                                join al in Db.TB_ALMOXARIFADOs on uge.TB_UGE_ID equals al.TB_UGE_ID
                                where al.TB_ALMOXARIFADO_ID == almox.Id
                                    && al.TB_GESTOR_ID == almox.Gestor.Id.Value

                                orderby divisao.TB_UA.TB_UA_CODIGO
                                select new UAEntity
                                {
                                    Id = divisao.TB_UA.TB_UA_ID,
                                    Codigo = divisao.TB_UA.TB_UA_CODIGO,
                                    Unidade = (new UnidadeEntity(divisao.TB_UA.TB_UNIDADE_ID)),
                                    Descricao = divisao.TB_UA.TB_UA_DESCRICAO,
                                    UaVinculada = divisao.TB_UA.TB_UA_VINCULADA,
                                    CentroCusto = (new CentroCustoEntity(divisao.TB_UA.TB_CENTRO_CUSTO_ID)),
                                    IndicadorAtividade = divisao.TB_UA.TB_UA_INDICADOR_ATIVIDADE,
                                    Uge = new UGEEntity
                                    {
                                        Id = divisao.TB_UA.TB_UGE.TB_UGE_ID,
                                        Codigo = divisao.TB_UA.TB_UGE.TB_UGE_CODIGO,
                                        //Descricao = divisao.TB_UA.TB_UGE.TB_UGE_DESCRICAO                                                 
                                        Descricao = divisao.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                    },
                                    Uo = new UOEntity
                                    {
                                        Id = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                        Codigo = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                        //Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO
                                        Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                    },
                                    Orgao = new OrgaoEntity
                                    {
                                        Id = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_ID,
                                        Codigo = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_CODIGO,
                                        //Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO
                                        Descricao = divisao.TB_UA.TB_UGE.TB_UO.TB_ORGAO.TB_ORGAO_DESCRICAO,
                                    }
                                })
                                .ToList();

            return resultado;
        }

        public bool PodeExcluir()
        {
            bool retorno = true;
            TB_UA tbUa = this.Db.TB_UAs.Where(a => a.TB_UA_ID == this.Entity.Id.Value).FirstOrDefault();
            if (tbUa.TB_DIVISAOs.Count > 0)
                retorno = false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            int iContador = 0;

            if (this.Entity.Id.HasValue)
               iContador = this.Db.TB_UAs.Where(uaSAM =>   uaSAM.TB_UA_CODIGO == this.Entity.Codigo 
                                                        && uaSAM.TB_UA_ID != this.ua.Id 
                                                        && uaSAM.TB_UA_INDICADOR_ATIVIDADE == true
                                                        && uaSAM.TB_GESTOR_ID == this.Entity.Gestor.Id.Value).Count();

            return (iContador >= 1);
        }


        public UAEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<UAEntity> ListarTodosCod()
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_ORGAO_ID
                                         join org in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(org.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(uge.TB_UGE_ID)),
                                             Uo = new UOEntity(uo.TB_UO_ID)
                                         })
                                         .ToList();
            return resultado;
        }

        public IList<UAEntity> Listar(System.Linq.Expressions.Expression<Func<UAEntity, bool>> where)
        {
            IList<UAEntity> resultado = (from a in this.Db.TB_UAs
                                         join uge in Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                                         join uo in Db.TB_UOs on uge.TB_UO_ID equals uo.TB_ORGAO_ID
                                         join org in Db.TB_ORGAOs on uo.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,
                                             Unidade = (new UnidadeEntity(a.TB_UNIDADE_ID)),
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,
                                             CentroCusto = (new CentroCustoEntity(a.TB_CENTRO_CUSTO_ID)),
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,
                                             Orgao = (new OrgaoEntity(org.TB_ORGAO_ID)),
                                             Uge = (new UGEEntity(uge.TB_UGE_ID)),
                                             Uo = new UOEntity(uo.TB_UO_ID)
                                         }).Where(where)
                                         .ToList();
            return resultado;
        }

        public UAEntity ObterUA(int uaID)
        {
            UAEntity objRetorno = (from ua in this.Db.TB_UAs
                                   where ua.TB_UA_ID == uaID
                                   select new UAEntity
                                   {
                                       Id = ua.TB_UA_ID,
                                       Codigo = ua.TB_UA_CODIGO,
                                       Unidade = new UnidadeEntity(ua.TB_UNIDADE_ID),
                                       Descricao = ua.TB_UA_DESCRICAO,
                                       UaVinculada = ua.TB_UA_VINCULADA,
                                       CentroCusto = new CentroCustoEntity(ua.TB_CENTRO_CUSTO_ID),
                                       IndicadorAtividade = ua.TB_UA_INDICADOR_ATIVIDADE,
                                       Uge = new UGEEntity(ua.TB_UGE.TB_UGE_ID) { Codigo = ua.TB_UGE.TB_UGE_CODIGO, Descricao = ua.TB_UGE.TB_UGE_DESCRICAO },
                                       Gestor = new GestorEntity(ua.TB_GESTOR.TB_GESTOR_ID) { CodigoGestao = ua.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, NomeReduzido = ua.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO }
                                   }).AsQueryable()
                                   .FirstOrDefault();

            return objRetorno;
        }

        public UAEntity ObterUAPorCodigo(int uaCodigo, int gestorId)
        {
            UAEntity objRetorno = (from ua in this.Db.TB_UAs
                                       join _gestor in this.Db.TB_GESTORs on ua.TB_GESTOR_ID equals _gestor.TB_GESTOR_ID
                                       //join _orgao in Db.TB_ORGAOs on _gestor.TB_ORGAO_ID equals _orgao.TB_ORGAO_ID
                                   where ua.TB_UA_CODIGO == uaCodigo && _gestor.TB_GESTOR_ID== gestorId
                                   select new UAEntity
                                   {
                                       Id = ua.TB_UA_ID,
                                       Codigo = ua.TB_UA_CODIGO,
                                       Unidade = new UnidadeEntity(ua.TB_UNIDADE_ID),
                                       Descricao = ua.TB_UA_DESCRICAO,
                                       UaVinculada = ua.TB_UA_VINCULADA,
                                       CentroCusto = new CentroCustoEntity(ua.TB_CENTRO_CUSTO_ID),
                                       IndicadorAtividade = ua.TB_UA_INDICADOR_ATIVIDADE,
                                       Uge = new UGEEntity(ua.TB_UGE.TB_UGE_ID)
                                       {
                                           Codigo = ua.TB_UGE.TB_UGE_CODIGO,
                                           Descricao = ua.TB_UGE.TB_UGE_DESCRICAO,
                                           Uo = new UOEntity(ua.TB_UGE.TB_UO_ID) { Codigo = ua.TB_UGE.TB_UO.TB_UO_CODIGO, Descricao = ua.TB_UGE.TB_UO.TB_UO_DESCRICAO }
                                       },
                                       Gestor = new GestorEntity(ua.TB_GESTOR.TB_GESTOR_ID)
                                       {
                                           CodigoGestao = ua.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO,
                                           NomeReduzido = ua.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO,
                                           Orgao = new OrgaoEntity(ua.TB_GESTOR.TB_ORGAO_ID) { Codigo = ua.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO, Descricao = ua.TB_GESTOR.TB_ORGAO.TB_ORGAO_DESCRICAO }
                                       }
                                   }).AsQueryable()
                                   .FirstOrDefault();

            return objRetorno;
        }

        /// <summary>
        /// Obtém a primeira UA ATIVA por Código
        /// </summary>
        /// <param name="uaCodigo">Código da UA</param>
        /// <returns></returns>
        public UAEntity ObterUAAtivaPorCodigo(int uaCodigo)
        {
            UAEntity objRetorno = (from ua in this.Db.TB_UAs
                                   where ua.TB_UA_CODIGO == uaCodigo
                                      && ua.TB_UA_INDICADOR_ATIVIDADE == true
                                   select new UAEntity
                                   {
                                       Id = ua.TB_UA_ID,
                                       Codigo = ua.TB_UA_CODIGO,
                                       Unidade = new UnidadeEntity(ua.TB_UNIDADE_ID),
                                       Descricao = ua.TB_UA_DESCRICAO,
                                       UaVinculada = ua.TB_UA_VINCULADA,
                                       CentroCusto = new CentroCustoEntity(ua.TB_CENTRO_CUSTO_ID),
                                       IndicadorAtividade = ua.TB_UA_INDICADOR_ATIVIDADE,
                                       Uge = new UGEEntity(ua.TB_UGE.TB_UGE_ID)
                                       {
                                           Codigo = ua.TB_UGE.TB_UGE_CODIGO,
                                           Descricao = ua.TB_UGE.TB_UGE_DESCRICAO,
                                           Uo = new UOEntity(ua.TB_UGE.TB_UO_ID) { Codigo = ua.TB_UGE.TB_UO.TB_UO_CODIGO, Descricao = ua.TB_UGE.TB_UO.TB_UO_DESCRICAO }
                                       },
                                       Gestor = new GestorEntity(ua.TB_GESTOR.TB_GESTOR_ID)
                                       {
                                           CodigoGestao = ua.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO,
                                           NomeReduzido = ua.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO,
                                           Orgao = new OrgaoEntity(ua.TB_GESTOR.TB_ORGAO_ID) { Codigo = ua.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO, Descricao = ua.TB_GESTOR.TB_ORGAO.TB_ORGAO_DESCRICAO }
                                       }
                                   }).AsQueryable()
                                   .FirstOrDefault();

            return objRetorno;
        }
    }
}
