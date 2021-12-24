using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;

namespace Sam.Infrastructure.Infrastructure.Segurança
{
    public class NivelInfrastructure : AbstractCrud<TB_NIVEL, SEGwebEntities>
    {
        public IList<TB_NIVEL> getNivel()
        {
            IQueryable<TB_NIVEL> query = (from q in Context.TB_NIVEL select q).AsQueryable();

            return query.ToList();

        }

       
    }
}
