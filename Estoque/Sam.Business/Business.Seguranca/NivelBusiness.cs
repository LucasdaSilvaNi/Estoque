using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure.Infrastructure.Segurança;
using Sam.Infrastructure;

namespace Sam.Business.Business.Seguranca
{
    public class NivelBusiness
    {
        private NivelInfrastructure NivelInfra = null;

        public NivelBusiness() {
            this.NivelInfra = new NivelInfrastructure();
        }

        public IList<TB_NIVEL> getNivel()
        {
            return NivelInfra.getNivel();
        }


    }
}
