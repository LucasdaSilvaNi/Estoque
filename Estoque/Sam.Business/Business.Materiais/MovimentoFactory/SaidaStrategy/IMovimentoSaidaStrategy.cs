using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.Business.MovimentoFactory
{
    public interface IMovimentoSaidaStrategy
    {
        void SairMaterial(MovimentoEntity movimento);
        void EstornarSaidaMaterial(MovimentoEntity movimento);
    }
}
