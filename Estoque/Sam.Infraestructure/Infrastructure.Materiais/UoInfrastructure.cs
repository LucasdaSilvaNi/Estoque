using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class UoInfrastructure : AbstractCrud<TB_UO, SAMwebEntities>
    {
        public IList<TB_UO> ListarOrgao(int OrgaoId)
        {
            IQueryable<TB_UO> resultado = (from a in Context.TB_UO select a);
            var retorno = resultado.Cast<TB_UO>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_ORGAO_ID == OrgaoId
                                group r by new
                                {
                                    r.TB_UO_ID,
                                    r.TB_UO_CODIGO,
                                    r.TB_UO_DESCRICAO,
                                } into am
                                select new TB_UO()
                                {
                                    TB_UO_ID = am.Key.TB_UO_ID,
                                    TB_UO_CODIGO = am.Key.TB_UO_CODIGO,
                                    //TB_ALMOXARIFADO_DESCRICAO = am.Key.TB_ALMOXARIFADO_CODIGO.ToString() + " - " +  am.Key.TB_ALMOXARIFADO_DESCRICAO,
                                    TB_UO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_UO_CODIGO.ToString().PadLeft(5, '0'), am.Key.TB_UO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_UO_CODIGO).Cast<TB_UO>().ToList();
        }


    }
}
