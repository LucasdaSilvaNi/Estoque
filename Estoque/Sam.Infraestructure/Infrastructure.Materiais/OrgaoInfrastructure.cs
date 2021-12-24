using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class OrgaoInfrastructure : AbstractCrud<TB_ORGAO, SAMwebEntities>
    {
        public IList<TB_ORGAO> ListarOrgao()
        {
            IQueryable<TB_ORGAO> resultado = (from a in Context.TB_ORGAO select a);
            var retorno = resultado.Cast<TB_ORGAO>().ToList();

            var retornoLista = (from r in retorno
                                group r by new
                                {
                                    r.TB_ORGAO_ID,
                                    r.TB_ORGAO_CODIGO,
                                    r.TB_ORGAO_DESCRICAO,
                                } into am
                                select new TB_ORGAO()
                                {
                                    TB_ORGAO_ID = am.Key.TB_ORGAO_ID,
                                    TB_ORGAO_CODIGO = am.Key.TB_ORGAO_CODIGO,
                                    //TB_ALMOXARIFADO_DESCRICAO = am.Key.TB_ALMOXARIFADO_CODIGO.ToString() + " - " +  am.Key.TB_ALMOXARIFADO_DESCRICAO,
                                    TB_ORGAO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ORGAO_CODIGO.ToString().PadLeft(2, '0'), am.Key.TB_ORGAO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_ORGAO_CODIGO).Cast<TB_ORGAO>().ToList();
        }

        public TB_ORGAO RecuperaOrgao(int orgaoId)
        {
            return (from a in Context.TB_ORGAO
                                              where a.TB_ORGAO_ID == orgaoId
                                              select a).FirstOrDefault();

        }
    }
}
