using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;

namespace Sam.Domain.Business
{
    public class MovimentoEstBusiness : BaseBusiness
    {


        private MovimentoEstEntity movimentoEst = new MovimentoEstEntity();

        public MovimentoEstEntity MovimentoEst {
            get { return movimentoEst; }
            set { movimentoEst = value; }
        }

        public long? MaxNumeroDocumento()
        {
            //IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().MaxNumeroDocumento();
            //try
            //{
            //    long? max = retorno.Max(a => Common.Util.TratamentoDados.TryParseLong(a.NumeroDocumento));
            //    return max + 1;
            //}
            //catch
            //{
            //    return 0;
            //}

            return null;
        }


    #region Validadores


    #endregion

    #region Consistencias

         public bool ConsistirMovimentoEst()
        {

            if (this.ListaErro.Count == 0)
            {
                return true;
            }

            return false;
        }


    #endregion

    

    }
}
