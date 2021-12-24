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
    public class EstruturaBusiness
    {
        public Sam.Entity.Estrutura BuscarEstruturas(int loginId)
        {
            return new Infrastructure.EstruturasInfrastructure().BuscarEstruturas(loginId);
        }

        public int ConsultaEstruturaNivel(int estruturaId, int nivelId)
        {
            return new Infrastructure.EstruturasInfrastructure().ConsultaEstruturaNivel(estruturaId, nivelId);
        }
    }
}
