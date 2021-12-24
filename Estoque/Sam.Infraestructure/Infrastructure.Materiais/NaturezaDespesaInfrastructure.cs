using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;

namespace Sam.Infrastructure
{
    public class NaturezaDespesaInfrastructure : AbstractCrud<TB_NATUREZA_DESPESA, SAMwebEntities>
    {
        public List<TB_ITEM_NATUREZA_DESPESA> ItemNaturezaDespesa(int idNatureza, int idItem)
        {
            var result = (from a in Context.TB_ITEM_NATUREZA_DESPESA
                          where a.TB_ITEM_MATERIAL_ID == idItem && a.TB_NATUREZA_DESPESA_ID == idNatureza
                          select a).ToList();

            return result;
        }

        public bool ExisteItemNatureza(int idNatureza, int idItem)
        {
            var result = (from a in Context.TB_ITEM_NATUREZA_DESPESA
                          where a.TB_ITEM_MATERIAL_ID == idItem && a.TB_NATUREZA_DESPESA_ID == idNatureza
                          select a).Count();

            if (result > 0)
                return true;
            else
                return false;
        }

    }
}
