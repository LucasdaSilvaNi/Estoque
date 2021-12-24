using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ILoginService : ICatalogoBaseService, ICrudBaseService<UsuarioEntity>
    {
       bool Validar();
    }
}
