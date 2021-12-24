using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.Business.MovimentoFactory
{
    public interface IMovimentoEntradaStrategy
    {
        void EntrarMaterial(MovimentoEntity movimento);
        void EstornarEntradaMaterial(MovimentoEntity movimento);
    }
}
