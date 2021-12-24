using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;

namespace Sam.Infrastructure
{
    public class UaInfrastructure : AbstractCrud<TB_UA, SAMwebEntities>
    {
        public IList<UAEntity> SelectById(int uaId)
        {
            try
            {
                this.Context.ContextOptions.LazyLoadingEnabled = true;                

                var query = this.Context.CreateQuery<TB_UA>(EntitySetName).Where(x=>x.TB_UA_ID == uaId).ToList();

                var resultado = (from a in query                                 
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
                                     Gestor = new Sam.Domain.Entity.GestorEntity(a.TB_GESTOR.TB_GESTOR_ID)
                                 }).ToList();

                return resultado;               
            }
            catch (Exception ex)
            {   
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

    }
}
