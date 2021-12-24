using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Business.MovimentoFactory;
using Sam.Domain.Entity;

namespace Sam.Business.MovimentoFactory
{
    public class MovimentoFactory
    {
        private MovimentoFactory() { }

        public static IMovimentoFactory instance;
        public static IMovimentoFactory GetIMovimentoFactory()
        {
            if (instance == null)
            {
                instance = new MovimentoFactoryImp();
            }

            return instance;
        }
    }
}
