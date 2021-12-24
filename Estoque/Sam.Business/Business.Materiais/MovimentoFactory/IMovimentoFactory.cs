using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Business.MovimentoFactory;

namespace Sam.Business.MovimentoFactory
{
    public interface IMovimentoFactory
    {
        void EntrarMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento);
        void EstornarMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento);
        void ValidarMovimento(ParametrizacaoEntrada param, MovimentoEntity movimento);
        void SairMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento);
        void EstornarSaidaMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento);
    }
}
