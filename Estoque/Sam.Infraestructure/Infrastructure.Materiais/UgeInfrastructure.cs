using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Domain.Entity;


namespace Sam.Infrastructure
{
    public class UgeInfrastructure : AbstractCrud<TB_UGE, SAMwebEntities>
    {
        public IList<UGEEntity> SelectByExpression(Expression<Func<TB_UGE, bool>> _expression)
        {
            try
            {
                this.Context.ContextOptions.LazyLoadingEnabled = true;

                var query = this.Context.CreateQuery<TB_UGE>(EntitySetName).Where(_expression).ToList();
                var result = (from a in query
                              join uo in this.Context.TB_UO on a.TB_UO_ID equals uo.TB_UO_ID                              
                              join orgao in this.Context.TB_ORGAO on uo.TB_ORGAO_ID equals orgao.TB_ORGAO_ID
                              orderby a.TB_UGE_CODIGO
                              select new UGEEntity
                              {
                                  Id = a.TB_UGE_ID,
                                  Codigo = a.TB_UGE_CODIGO,
                                  Descricao = string.Format("{0} - {1}", a.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), a.TB_UGE_DESCRICAO),
                                  Uo = (new UOEntity(a.TB_UO_ID)),
                                  Orgao = (new OrgaoEntity(orgao.TB_ORGAO_ID)),
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        internal UGEEntity MapearDTO(TB_UGE rowTabela)
        {
            this.Context.ContextOptions.LazyLoadingEnabled = true;
            UGEEntity objEntidade = new UGEEntity();

            objEntidade.Id              = rowTabela.TB_UGE_ID;
            objEntidade.Codigo          = rowTabela.TB_UGE_CODIGO;
            objEntidade.CodigoDescricao = string.Format("{0} - {1}", rowTabela.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), rowTabela.TB_UGE_DESCRICAO);
            objEntidade.Descricao       = rowTabela.TB_UGE_DESCRICAO;
            objEntidade.Uo              = new UOEntity(rowTabela.TB_UO.TB_UO_ID);
            objEntidade.Orgao           = new OrgaoEntity(rowTabela.TB_UO.TB_UO_ID);


            return objEntidade;
        }
        internal TB_UGE MapearEntity(UGEEntity objEntidade)
        {
            TB_UGE rowTabela = new TB_UGE();

            rowTabela.TB_UGE_CODIGO     = objEntidade.Codigo.Value;
            rowTabela.TB_UGE_DESCRICAO  = objEntidade.Descricao;
            rowTabela.TB_UO             = new TB_UO() { TB_UO_CODIGO = objEntidade.Uo.Codigo, TB_UO_DESCRICAO = objEntidade.Descricao };
            
            return rowTabela;
        }

        public IList<TB_UGE> ListarUGE(int UoId)
        {
            IQueryable<TB_UGE> resultado = (from a in Context.TB_UGE select a);
            var retorno = resultado.Cast<TB_UGE>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_UO_ID == UoId
                                group r by new
                                {
                                    r.TB_UGE_ID,
                                    r.TB_UGE_CODIGO,
                                    r.TB_UGE_DESCRICAO,
                                } into am
                                select new TB_UGE()
                                {
                                    TB_UGE_ID = am.Key.TB_UGE_ID,
                                    TB_UGE_CODIGO = am.Key.TB_UGE_CODIGO,
                                    TB_UGE_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_UGE_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_UGE_CODIGO).Cast<TB_UGE>().ToList();
        }

        //public GestorEntity ObterGestorUGE(int ugeID)
        //{
        //    GestorEntity objRetorno = null;
        //    var gestorInfra = new GestorInfrastructure();

        //    var rowTabela = this.SelectOne(UGE => UGE.TB_UGE_ID == ugeID);
        //    if (rowTabela.IsNotNull())
        //        objRetorno = gestorInfra.ObterID(rowTabela.TB_GESTOR_ID);

        //    return objRetorno;
        //}

        public IList<TB_UGE> ObterUGE(int ugeID)
        {
            IQueryable<TB_UGE> resultado = (from a in Context.TB_UGE select a);
            var retorno = resultado.Cast<TB_UGE>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_UGE_ID == ugeID
                                group r by new
                                {
                                    r.TB_UGE_ID,
                                    r.TB_UO_ID,
                                } into am
                                select new TB_UGE()
                                {
                                    TB_UGE_ID = am.Key.TB_UGE_ID,
                                    TB_UO_ID =  am.Key.TB_UO_ID,
                                });

            return retornoLista.Cast<TB_UGE>().ToList();
        }    
    }
}
