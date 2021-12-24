using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using System.Linq.Expressions;


namespace Sam.Domain.Infrastructure
{
    public partial class UGEInfraestructure : BaseInfraestructure, IUGEService
    {
        #region ICrudBaseService<UGEntity> Members

        private UGEEntity uge = new UGEEntity();
        public UGEEntity Entity
        {
            get { return uge; }
            set { uge = value; }
        }


        public void Salvar()
        {
            try
            {
                TB_UGE ugeDb = new TB_UGE();

                if (this.Entity.Id.HasValue)
                    ugeDb = this.Db.TB_UGEs.Where(a => a.TB_UGE_ID == this.Entity.Id.Value).FirstOrDefault();
                else
                    Db.TB_UGEs.InsertOnSubmit(ugeDb);

                ugeDb.TB_UGE_CODIGO = this.Entity.Codigo.Value;
                ugeDb.TB_UGE_DESCRICAO = this.Entity.Descricao;
                ugeDb.TB_UGE_TIPO = Convert.ToChar(this.Entity.TipoUGE);
                ugeDb.TB_UO_ID = this.Entity.Uo.Id.Value;
                ugeDb.TB_UGE_STATUS = this.Entity.Ativo;
                ugeDb.TB_UGE_INTEGRACAO_SIAFEM = Convert.ToBoolean(this.Entity.IntegracaoSIAFEM);
                ugeDb.TB_UGE_IMPLANTADO = Convert.ToBoolean(this.Entity.Implantado);
                this.Db.SubmitChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<TipoUGEEntity> ListarTipoUGE()
        {
            TipoUGEEntity  normal = new TipoUGEEntity(0);
            normal.Descricao = "Normal";
            TipoUGEEntity  contabil= new TipoUGEEntity(1);
            contabil.Descricao = "Contábil";

            IList<TipoUGEEntity> resultado = new List<TipoUGEEntity>();
            resultado.Add(normal);
            resultado.Add(contabil);

            /*IList<TipoUGEEntity> resultado = (from a in Db.TB_TIPO_UGEs
                                              orderby a.TB_TIPO_UGE_ID
                                              select new TipoUGEEntity
                                              {
                                                  Id = a.TB_TIPO_UGE_ID,
                                                  Descricao = a.TB_TIPO_UGE_DESCRICAO
                                              }).ToList<TipoUGEEntity>();
             * */
            return resultado;
        }

        public IList<UGEEntity> Listar()
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID))
                                              ,
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                         .Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<UGEEntity>();

            this.totalregistros = (from a in this.Db.TB_UGEs
                                   orderby a.TB_UGE_CODIGO
                                   select new
                                   {
                                       Id = a.TB_UGE_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UGEEntity> Imprimir()
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          orderby a.TB_UGE_ID
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID))
                                              ,
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                              
                                          }).ToList<UGEEntity>();

            return resultado;
        }

        public IList<UGEEntity> Imprimir(int OrgaoId, int UoId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          join uo in Db.TB_UOs on a.TB_UO_ID equals uo.TB_UO_ID
                                          where (a.TB_UO_ID == UoId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID)),
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                        .ToList<UGEEntity>();
            return resultado;
        }

        public IList<UGEEntity> Listar(int? UoId)
        {

            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          where (a.TB_UO_ID == UoId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID)),
                                              Ativo = a.TB_UGE_STATUS,
                                              IntegracaoSIAFEM = a.TB_UGE_INTEGRACAO_SIAFEM,
                                              Implantado=a.TB_UGE_IMPLANTADO,
                                              // TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                       .Skip(this.SkipRegistros)
                                       .Take(this.RegistrosPagina)
                                       .ToList<UGEEntity>();
            this.totalregistros = (from a in this.Db.TB_UGEs
                                   where (a.TB_UO_ID == UoId)
                                   orderby a.TB_UGE_CODIGO
                                   select new
                                   {
                                       Id = a.TB_UGE_ID,
                                   }).Count();
            return resultado;

        }

        public IList<UGEEntity> ListarUgesTodosCod(int? OrgaoId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          join uo in Db.TB_UOs on a.TB_UO_ID equals uo.TB_UO_ID
                                          where (uo.TB_ORGAO_ID == OrgaoId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_CODIGO + " - " + a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID)),
                                              Orgao = (new OrgaoEntity(OrgaoId)),
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                       .ToList<UGEEntity>();

            this.totalregistros = (from a in this.Db.TB_UGEs
                                   join uo in Db.TB_UOs on a.TB_UO_ID equals uo.TB_UO_ID
                                   where (uo.TB_ORGAO_ID == OrgaoId)
                                   orderby a.TB_UGE_CODIGO
                                   select new
                                   {
                                       Id = a.TB_UGE_ID,
                                   }).Count();

            return resultado;

        }

        public IList<UGEEntity> ListarUgesTodosCodPorUo(int? UoId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          where (a.TB_UO_ID == UoId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Uo = (new UOEntity(a.TB_UO_ID)),
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                       .ToList<UGEEntity>();

            this.totalregistros = (from a in this.Db.TB_UGEs
                                   where (a.TB_UO_ID == UoId)
                                   orderby a.TB_UGE_CODIGO
                                   select new
                                   {
                                       Id = a.TB_UGE_ID,
                                   }).Count();

            return resultado;

        }

        public IList<UGEEntity> ListarUgesTodosCodPorUo(int? UoId, IList<DivisaoEntity> divisaoList)
        {
            List<UGEEntity> resultadoCompleto = new List<UGEEntity>();
            IList<UGEEntity> resultado = null;

            foreach (var divisao in divisaoList)
            {
                resultado = (from a in Db.TB_UGEs
                             join ua in Db.TB_UAs on a.TB_UGE_ID equals ua.TB_UGE_ID
                             join div in Db.TB_DIVISAOs on ua.TB_UA_ID equals div.TB_UA_ID
                             where (a.TB_UO_ID == UoId)
                             where div.TB_DIVISAO_ID == divisao.Id
                             orderby a.TB_UGE_CODIGO
                             select new UGEEntity
                             {
                                 Id = a.TB_UGE_ID,
                                 Codigo = a.TB_UGE_CODIGO,
                                 Descricao = a.TB_UGE_DESCRICAO,
                                 Uo = (new UOEntity(a.TB_UO_ID)),
                                 //TipoUGE = a.TB_TIPO_UGE_ID
                             }).ToList<UGEEntity>();

                foreach (var res in resultado)
                {
                    if (resultadoCompleto.Where(a => a.Id == res.Id).Count() == 0)
                        resultadoCompleto.Add(res);
                }
            }

            return resultadoCompleto;

        }

        public IList<UGEEntity> Listar(int OrgaoId, int UoId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          where (a.TB_UO_ID == UoId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              Orgao = (new OrgaoEntity(OrgaoId)),
                                              Uo = (new UOEntity(a.TB_UO_ID)),
                                              //TipoUGE = a.TB_TIPO_UGE_ID
                                          })
                                        .Skip(this.SkipRegistros)
                                        .Take(this.RegistrosPagina)
                                        .ToList<UGEEntity>();

            this.totalregistros = (from a in this.Db.TB_UGEs
                                   where (a.TB_UO_ID == UoId)
                                   select new
                                   {
                                       Id = a.TB_UGE_ID,
                                   }).Count();

            return resultado;

        }

        public void Excluir()
        {
            TB_UGE uge
                = this.Db.TB_UGEs.Where(a => a.TB_UGE_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UGEs.DeleteOnSubmit(uge);
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_UGEs
                .Where(a => a.TB_UGE_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_UGE_ID != this.Entity.Id.Value)
                .Where(a => a.TB_UO_ID == this.Entity.Uo.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_UGEs
                .Where(a => a.TB_UGE_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_UO_ID == this.Entity.Uo.Id.Value)
                .Count() > 0;
            }
            return retorno;
        }
 
        #endregion

        public UGEEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public UGEEntity LerRegistro(int pIntUgeID)
        {
            UGEEntity lObjRetorno = (from Uge in this.Db.TB_UGEs
                                     join Uo in Db.TB_UOs on Uge.TB_UO_ID equals Uo.TB_UO_ID
                                     join Orgao in Db.TB_ORGAOs on Uo.TB_ORGAO_ID equals Orgao.TB_ORGAO_ID
                                     orderby Uge.TB_UGE_CODIGO
                                     where Uge.TB_UGE_ID == pIntUgeID
                                     select new UGEEntity()
                                     {
                                         Id         = Uge.TB_UGE_ID,
                                         Codigo     = Uge.TB_UGE_CODIGO,
                                         Descricao  = Uge.TB_UGE_DESCRICAO,
                                         TipoUGE    = Uge.TB_UGE_TIPO.ToString(),
                                         Orgao      = new OrgaoEntity(Orgao.TB_ORGAO_ID)  { Codigo = Orgao.TB_ORGAO_CODIGO, Descricao = Orgao.TB_ORGAO_DESCRICAO   },
                                         Uo         = new UOEntity(Uge.TB_UO.TB_ORGAO_ID) { Codigo = Uge.TB_UO.TB_UO_CODIGO, Descricao = Uge.TB_UO.TB_UO_DESCRICAO }
                                     }).FirstOrDefault();
            return lObjRetorno;
        }

        public IList<UGEEntity> Listar(System.Linq.Expressions.Expression<Func<UGEEntity, bool>> where)
        {
            return (from a in Db.TB_UGEs
                    join b in Db.TB_UOs on a.TB_UO_ID equals b.TB_UO_ID
                    join c in Db.TB_ORGAOs on b.TB_ORGAO_ID equals c.TB_ORGAO_ID
                    orderby a.TB_UGE_CODIGO
                    select new UGEEntity
                    {
                        Id = a.TB_UGE_ID,
                        Codigo = a.TB_UGE_CODIGO,
                        Descricao = a.TB_UGE_DESCRICAO,
                        Orgao = new OrgaoEntity(c.TB_ORGAO_ID),
                        Uo = new UOEntity(a.TB_UO_ID)
                    }).Where(where)
                    .ToList<UGEEntity>();
        }

        public IList<UGEEntity> ListarTodosCod()
        {
            return (from a in Db.TB_UGEs
                         join b in Db.TB_UOs on a.TB_UO_ID equals b.TB_UO_ID
                         join c in Db.TB_ORGAOs on b.TB_ORGAO_ID equals c.TB_ORGAO_ID
                         orderby a.TB_UGE_CODIGO
                         select new UGEEntity
                         {
                             Id = a.TB_UGE_ID,
                             Codigo = a.TB_UGE_CODIGO,
                             Descricao = a.TB_UGE_DESCRICAO,
                             Orgao = new OrgaoEntity(c.TB_ORGAO_ID),
                             Uo = new UOEntity(a.TB_UO_ID)
                         })
                    .ToList<UGEEntity>();
        }

        public IList<UGEEntity> ListarTodosCodPorGestor(int GestorId)
        {
            IList<UGEEntity> resultado = null;

            GestorEntity gestor = (from a in Db.TB_GESTORs
                                   where a.TB_GESTOR_ID == GestorId
                                   select new GestorEntity { 
                                    Id = GestorId,
                                    Orgao = new OrgaoEntity(a.TB_ORGAO_ID),
                                    Uo = (a.TB_UO_ID != null ? new UOEntity(a.TB_UO_ID) : null),
                                    Uge = (a.TB_UGE_ID != null ? new UGEEntity(a.TB_UGE_ID) : null)
                                   }
                                   ).FirstOrDefault();

            if (gestor != null) 
            {
                resultado = (from a in Db.TB_UGEs
                             join b in Db.TB_UOs on a.TB_UO_ID equals b.TB_UO_ID
                             join c in Db.TB_ORGAOs on b.TB_ORGAO_ID equals c.TB_ORGAO_ID
                             where (c.TB_ORGAO_ID == gestor.Orgao.Id)
                            orderby a.TB_UGE_CODIGO
                            select new UGEEntity
                            {
                                Id = a.TB_UGE_ID,
                                Codigo = a.TB_UGE_CODIGO,
                                Descricao = a.TB_UGE_DESCRICAO,
                                Uo = new UOEntity(a.TB_UO_ID)
                            })
                        .ToList<UGEEntity>();

                if(gestor.Uge != null)
                    resultado = resultado.Where(a => a.Id == gestor.Uge.Id).ToList();

                if(gestor.Uo != null)
                    resultado = resultado.Where(a => a.Uo.Id == gestor.Uo.Id).ToList();

                //where (gestor.Uge != null ? a.TB_UGE_ID == gestor.Uge.Id : true)
                //             where (gestor.Uo != null ? b.TB_UO_ID == gestor.Uo.Id : true)
            }

            return resultado;
        }

     

        public IList<UGEEntity> ListarUGESaldoTodosCod(int gestorId, int almoxarifadoId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_SALDO_SUBITEMs
                                          where a.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID == gestorId
                                          where a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almoxarifadoId 
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE.TB_UGE_ID,
                                              Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE.TB_UGE_DESCRICAO,
                                          }).Distinct().ToList();
            return resultado;
        }

        /// <summary>
        /// Retorna uges com saldo
        /// </summary>
        /// <param name="subItemId">Id do subitem</param>
        /// <param name="almoxarifadoId">id do almoxarifado</param>
        /// <param name="ugeId">desconsiderar</param>
        /// <returns></returns>
        public IList<UGEEntity> ListarUGESaldoTodosCod(int subItemId, int almoxarifadoId, int ugeId)
        {
            IList<UGEEntity> resultado = (from a in Db.TB_UGEs
                                          join saldo in Db.TB_SALDO_SUBITEMs on a.TB_UGE_ID equals saldo.TB_UGE_ID
                                          where (saldo.TB_SUBITEM_MATERIAL_ID == subItemId)
                                          && (saldo.TB_ALMOXARIFADO_ID == almoxarifadoId)
                                          && (saldo.TB_UGE_ID == ugeId)
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                          }).Distinct().ToList();
            return resultado;
        }




        public IList<UGEEntity> ListarUGEsComSaldoParaSubitem(long subItemCodigo, int almoxarifadoId)
        {
            IList<UGEEntity> lLstRetorno      = null;
            IQueryable<UGEEntity> lQryRetorno = null;

            lQryRetorno = (from SaldoSubItem in Db.TB_SALDO_SUBITEMs
                           join SubItemMaterial in Db.TB_SUBITEM_MATERIALs on SaldoSubItem.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                           where SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == subItemCodigo
                           where SaldoSubItem.TB_ALMOXARIFADO_ID == almoxarifadoId
                           select new UGEEntity
                           {
                               Id        = SaldoSubItem.TB_UGE.TB_UGE_ID,
                               Codigo    = SaldoSubItem.TB_UGE.TB_UGE_CODIGO,
                               Descricao = SaldoSubItem.TB_UGE.TB_UGE_DESCRICAO,
                           }).Distinct();

            lLstRetorno = lQryRetorno.ToList();

            return lLstRetorno;
        }

        public int ObterCodigoGestao(int codigoUGE)
        {
            int codigoGestao = 0;
            IQueryable<int> qryConsulta = null;

            qryConsulta = (from gestorUGE in Db.TB_GESTORs
                           join orgaoGestor in Db.TB_ORGAOs on gestorUGE.TB_ORGAO_ID equals orgaoGestor.TB_ORGAO_ID
                           join uoUGE in Db.TB_UOs on gestorUGE.TB_ORGAO_ID equals uoUGE.TB_ORGAO_ID
                           join _uge in Db.TB_UGEs on uoUGE.TB_UO_ID equals _uge.TB_UO_ID
                           where _uge.TB_UGE_CODIGO == codigoUGE
                           select gestorUGE.TB_GESTOR_CODIGO_GESTAO).AsQueryable();

            codigoGestao = qryConsulta.FirstOrDefault();
            
            string strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));                        
            return codigoGestao;
        }
            
        public IList<UGEEntity> ListarUaAssociadaUge(int? uoId)
        {
            var resultado = (from a in this.Db.TB_UAs
                             join uge in this.Db.TB_UGEs on a.TB_UGE_ID equals uge.TB_UGE_ID
                             where uge.TB_UGE_CODIGO == uoId && uge.TB_UGE_STATUS == true
                             select new UGEEntity
                             {
                                 Id = a.TB_UA_CODIGO,
                                 Codigo = uge.TB_UGE_CODIGO,
                                 Descricao = uge.TB_UGE_DESCRICAO,
                                 Ativo = uge.TB_UGE_STATUS
                             }).ToList<UGEEntity>();
            return resultado;
        }
      
        public IList<UGEEntity> ListarUgeImplantada(int idOrgao)
        {
            IList<UGEEntity> resultado = null;

            resultado = (from orgao in this.Db.TB_ORGAOs
                         join uo in this.Db.TB_UOs on orgao.TB_ORGAO_ID equals uo.TB_ORGAO_ID
                         join uge in this.Db.TB_UGEs on uo.TB_UO_ID equals uge.TB_UO_ID
                         join almo in this.Db.TB_ALMOXARIFADOs on uge.TB_UGE_ID equals almo.TB_UGE_ID

                         join saldo in this.Db.TB_SALDO_SUBITEMs on almo.TB_ALMOXARIFADO_ID equals saldo.TB_ALMOXARIFADO_ID into sal
                         from x in sal.DefaultIfEmpty()
                         where (orgao.TB_ORGAO_ID == idOrgao && uge.TB_UGE_IMPLANTADO == true && uge.TB_UGE_STATUS == true)
                    
                         group new { uge, x } by new { uge.TB_UGE_CODIGO, uge.TB_UGE_DESCRICAO } into g
                         let Codigo=g.OrderByDescending(uge => uge.uge.TB_UGE_CODIGO)
                         select new UGEEntity
                         {
                             Codigo = g.Key.TB_UGE_CODIGO,
                             Descricao = g.Key.TB_UGE_DESCRICAO.ToUpper(),                          
                             SaldoValor = g.Sum(x => x.x == null ? 0: x.x.TB_SALDO_SUBITEM_SALDO_VALOR)

                         }).ToList<UGEEntity>();
            return resultado;
        }

        public IList<UGEEntity> ListarUgeComAlmoxarifado(int idOrgao)
        {
            IList<UGEEntity> resultado = null;

            resultado = (from orgao in this.Db.TB_ORGAOs
                         join uo in this.Db.TB_UOs on orgao.TB_ORGAO_ID equals uo.TB_ORGAO_ID
                         join uge in this.Db.TB_UGEs on uo.TB_UO_ID equals uge.TB_UO_ID
                        // join almo in this.Db.TB_ALMOXARIFADOs on uge.TB_UGE_ID equals almo.TB_UGE_ID                        
                         where (orgao.TB_ORGAO_ID == idOrgao && uge.TB_UGE_STATUS==true)                        
                         select new UGEEntity
                         {
                             Id = uge.TB_UGE_ID,
                             Codigo = uge.TB_UGE_CODIGO,
                             Descricao = uge.TB_UGE_CODIGO + " - " + uge.TB_UGE_DESCRICAO,                           
                         }).ToList<UGEEntity>();
            return resultado;
        }      
    }
}
