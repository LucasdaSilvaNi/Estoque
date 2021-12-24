using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class PerfilInfrastructure : AbstractCrud<TB_PERFIL, SEGwebEntities>
    {
        public short GetNovoPerfilId()
        {
            int perfilId = Context.TB_PERFIL.Max(p => p.TB_PERFIL_ID) + 1;

            return (short) perfilId;
        }
    }
}
