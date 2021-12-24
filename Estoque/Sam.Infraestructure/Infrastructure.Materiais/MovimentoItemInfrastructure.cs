using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class MovimentoItemInfrastructure : AbstractCrud<TB_MOVIMENTO_ITEM, SAMwebEntities>
    {
        public ObjectResult<MOVIMENTO_ITEM_SALDO_Result> ConsultarMovimentoItemSaldo()
        {
            Context.CommandTimeout = 180;
             return Context.MOVIMENTO_ITEM_SALDO();
        }
    }
}
