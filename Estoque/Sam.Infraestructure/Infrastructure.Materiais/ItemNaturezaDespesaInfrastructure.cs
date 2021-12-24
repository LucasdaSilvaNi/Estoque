using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class ItemNaturezaDespesaInfrastructure : AbstractCrud<TB_ITEM_NATUREZA_DESPESA, SAMwebEntities>
    {
        public bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa)
        {
            bool blnRetorno = false;
            int iCodigoNaturezaDespesa = Convert.ToInt32(strCodigoNaturezaDespesa);

            IQueryable<TB_ITEM_NATUREZA_DESPESA> tabelaItemNaturezaDespesa = null;
            tabelaItemNaturezaDespesa = (from ind in Context.TB_ITEM_NATUREZA_DESPESA
                                         where ind.TB_ITEM_MATERIAL_ID == iItemMaterial_ID
                                         where ind.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == iCodigoNaturezaDespesa
                                         select ind);

            if (tabelaItemNaturezaDespesa.Count() > 0)
                blnRetorno = true;

            return blnRetorno;
        }

        public void AtualizaRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int intNaturezaDespesaId)
        {
            TB_ITEM_NATUREZA_DESPESA tabelaItemNaturezaDespesa = new TB_ITEM_NATUREZA_DESPESA();

            tabelaItemNaturezaDespesa.TB_ITEM_MATERIAL_ID = iItemMaterial_ID;
            tabelaItemNaturezaDespesa.TB_NATUREZA_DESPESA_ID = intNaturezaDespesaId;

            this.Insert(tabelaItemNaturezaDespesa);
            this.SaveChanges();
        }
    }
}
