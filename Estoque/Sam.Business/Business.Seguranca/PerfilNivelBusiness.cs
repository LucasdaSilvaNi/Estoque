using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Entity;
using Sam.Infrastructure;


namespace Sam.Business
{
    public class PerfilNivelBusiness : BaseBusinessSeguranca
    {


        private Sam.Entity.PerfilNivel perfil_nivel = new Sam.Entity.PerfilNivel();

        public Sam.Entity.PerfilNivel PerfilNivel
        {
            get { return perfil_nivel; }
            set { perfil_nivel = value; }
        }

        public IList<PerfilNivel> LerNivelAceso(int PerfilId)
        {
            return new Infrastructure.PerfilNivelInfrastructureAntigo().LerNivel(PerfilId);
        }
    }
}
