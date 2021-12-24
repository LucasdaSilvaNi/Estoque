﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IUFService : ICatalogoBaseService, ICrudBaseService<UFEntity>
    {
        
    }
}
