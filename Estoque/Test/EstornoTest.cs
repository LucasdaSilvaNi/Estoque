using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sam.Domain.Business;
using Sam.Domain.Entity;

namespace Test
{
    [TestClass]
    public class EstornoTest
    {
        /// <summary>
        /// Retorna 1 documento ativo para estornar
        /// </summary>
        /// <returns></returns>
        public MovimentoEntity getMovimento()
        {
            Sam.Business.MovimentoBusiness businessNovo = new Sam.Business.MovimentoBusiness();
            var movimentoNovo = businessNovo.SelectOne(a => a.TB_MOVIMENTO_ATIVO == true);

            MovimentoEntity movimento = new MovimentoEntity();
            movimento.NumeroDocumento = movimentoNovo.TB_MOVIMENTO_NUMERO_DOCUMENTO;

            return movimento;
        }

        private void Entrada()
        {
        }


        [TestMethod]
        public void EstornoEntradaTest()
        {
            MovimentoBusiness business = new MovimentoBusiness();
            business.Movimento = getMovimento();
            //bool result = business.EstornarMovimentoEntrada();

            //Assert.AreEqual(result, true);
        }
    }
}
